// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PRTMModel.cs" company="">
//   
// </copyright>
// <summary>
//   The prtm model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HaloMap.RawData
{
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Windows.Forms;

    using HaloMap.ChunkCloning;
    using HaloMap.H2MetaContainers;
    using HaloMap.Meta;
    using HaloMap.Plugins;
    using HaloMap.Render;

    using Microsoft.DirectX;
    using Microsoft.DirectX.Direct3D;

    /// <summary>
    /// The prtm model.
    /// </summary>
    /// <remarks></remarks>
    public class PRTMModel : ParsedModel
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PRTMModel"/> class.
        /// </summary>
        /// <param name="meta">The meta.</param>
        /// <remarks></remarks>
        public PRTMModel(ref Meta meta)
        {
            H2ParsedPRTM(ref meta);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// The h 2 parsed prtm.
        /// </summary>
        /// <param name="meta">The meta.</param>
        /// <remarks></remarks>
        public void H2ParsedPRTM(ref Meta meta)
        {
            string[] temps = meta.name.Split('\\');
            name = temps[temps.Length - 1];

            BoundingBox = new BoundingBoxContainer();

            BinaryReader BR = new BinaryReader(meta.MS);

            RawDataMetaChunks = new RawDataMetaChunk[1];
            RawDataMetaChunks[0] = new PRTMRawDataMetaChunk(ref meta);

            BR.BaseStream.Position = 28;
            int tempshad = BR.ReadInt32();
            int shadid = meta.Map.Functions.ForMeta.FindMetaByID(tempshad);
            Shaders = new ShaderContainer();
            Shaders.Shader = new ShaderInfo[1];
            Shaders.Shader[0] = new ShaderInfo(shadid, meta.Map);

            // LOD = new LODInfo(ref meta, meta.Map, ref RawDataMetaChunks);
            Display = new DisplayedInfo();
            Display.Chunk.Add(0);
            Display.ShaderIndex = new int[1];
            Display.ShaderIndex[0] = 0;

            // MessageBox.Show("test");
        }

        /// <summary>
        /// The inject model.
        /// </summary>
        /// <param name="FilePath">The file path.</param>
        /// <param name="meta">The meta.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public override Meta InjectModel(string FilePath, Meta meta)
        {
            LoadFromOBJ(FilePath);

            char[] crsr = new[] { 'c', 'r', 's', 'r' };
            char[] fklb = new[] { 'f', 'k', 'l', 'b' };
            for (int x = 0; x < this.RawDataMetaChunks.Length; x++)
            {
                MemoryStream raw = new MemoryStream();
                BinaryWriter newraw = new BinaryWriter(raw);
                BinaryReader oldraw = new BinaryReader(meta.raw.rawChunks[x].MS);

                int newrawsize = 0;
                int rawchunkid = 0;

                #region Write Header

                oldraw.BaseStream.Position = 0;
                newraw.BaseStream.Position = 0;
                newraw.Write(oldraw.ReadBytes(this.RawDataMetaChunks[x].HeaderSize));
                newrawsize += this.RawDataMetaChunks[x].HeaderSize;

                #endregion

                #region Write Submesh Info

                newraw.BaseStream.Position = newrawsize;
                newraw.Write(crsr);
                newrawsize += 4;
                for (int y = 0; y < this.RawDataMetaChunks[x].SubMeshInfo.Length; y++)
                {
                    oldraw.BaseStream.Position = this.RawDataMetaChunks[x].HeaderSize +
                                                 this.RawDataMetaChunks[x].RawDataChunkInfo[0].Offset;
                    newraw.BaseStream.Position = newrawsize + (y * 72);
                    newraw.Write(oldraw.ReadBytes(72));
                    newraw.BaseStream.Position = newrawsize + (y * 72) + 4;
                    newraw.Write((short)this.RawDataMetaChunks[x].SubMeshInfo[y].ShaderNumber);
                    newraw.Write((short)this.RawDataMetaChunks[x].SubMeshInfo[y].IndiceStart);
                    newraw.Write((short)this.RawDataMetaChunks[x].SubMeshInfo[y].IndiceCount);
                }

                this.RawDataMetaChunks[x].RawDataChunkInfo[0].ChunkCount = this.RawDataMetaChunks[x].SubMeshInfo.Length;
                this.RawDataMetaChunks[x].RawDataChunkInfo[0].Size = this.RawDataMetaChunks[x].SubMeshInfo.Length * 72;
                newrawsize += this.RawDataMetaChunks[x].SubMeshInfo.Length * 72;

                // write count
                newraw.BaseStream.Position = 8;
                newraw.Write(this.RawDataMetaChunks[x].SubMeshInfo.Length);

                #endregion

                #region Write Unknown

                rawchunkid = 1;

                while (newrawsize > 0)
                {
                    if (this.RawDataMetaChunks[x].RawDataChunkInfo[rawchunkid].ChunkSize == 2)
                    {
                        break;
                    }

                    oldraw.BaseStream.Position = this.RawDataMetaChunks[x].HeaderSize +
                                                 this.RawDataMetaChunks[x].RawDataChunkInfo[rawchunkid].Offset;
                    newraw.BaseStream.Position = newrawsize;
                    newraw.Write(crsr);
                    newrawsize += 4;
                    newraw.Write(oldraw.ReadBytes(this.RawDataMetaChunks[x].RawDataChunkInfo[rawchunkid].Size));
                    this.RawDataMetaChunks[x].RawDataChunkInfo[rawchunkid].Offset = newrawsize -
                                                                                    this.RawDataMetaChunks[x].HeaderSize;
                    newrawsize += this.RawDataMetaChunks[x].RawDataChunkInfo[rawchunkid].Size;
                    rawchunkid++;
                }

                #endregion

                #region Write Indices

                int indicechunkid = rawchunkid;
                newraw.BaseStream.Position = newrawsize;
                newraw.Write(crsr);
                newrawsize += 4;
                for (int y = 0; y < this.RawDataMetaChunks[x].Indices.Length; y++)
                {
                    newraw.Write(this.RawDataMetaChunks[x].Indices[y]);
                }

                this.RawDataMetaChunks[x].RawDataChunkInfo[rawchunkid].Offset = newrawsize -
                                                                                this.RawDataMetaChunks[x].HeaderSize;
                this.RawDataMetaChunks[x].RawDataChunkInfo[rawchunkid].ChunkCount =
                    this.RawDataMetaChunks[x].Indices.Length;
                this.RawDataMetaChunks[x].RawDataChunkInfo[rawchunkid].Size = this.RawDataMetaChunks[x].Indices.Length *
                                                                              2;
                newrawsize += this.RawDataMetaChunks[x].Indices.Length * 2;
                rawchunkid++;

                // indice count
                newraw.BaseStream.Position = 40;
                newraw.Write((short)this.RawDataMetaChunks[x].Indices.Length);

                #endregion

                #region Write Unknown

                oldraw.BaseStream.Position = this.RawDataMetaChunks[x].HeaderSize +
                                             this.RawDataMetaChunks[x].RawDataChunkInfo[rawchunkid].Offset;
                newraw.BaseStream.Position = newrawsize;
                newraw.Write(crsr);
                newrawsize += 4;
                newraw.Write(oldraw.ReadBytes(this.RawDataMetaChunks[x].RawDataChunkInfo[rawchunkid].Size));
                this.RawDataMetaChunks[x].RawDataChunkInfo[rawchunkid].Offset = newrawsize -
                                                                                this.RawDataMetaChunks[x].HeaderSize;
                newrawsize += this.RawDataMetaChunks[x].RawDataChunkInfo[rawchunkid].Size;

                rawchunkid++;

                #endregion

                #region Write Vertices

                int verticechunkid = indicechunkid + 2;
                newraw.BaseStream.Position = newrawsize;
                newraw.Write(crsr);
                newrawsize += 4;
                newraw.Write(
                    new byte[
                        this.RawDataMetaChunks[x].RawDataChunkInfo[verticechunkid].ChunkSize *
                        this.RawDataMetaChunks[x].VerticeCount]);
                for (int y = 0; y < this.RawDataMetaChunks[x].VerticeCount; y++)
                {
                    newraw.BaseStream.Position = newrawsize +
                                                 (y *
                                                  this.RawDataMetaChunks[x].RawDataChunkInfo[verticechunkid].ChunkSize);
                    short vx =
                        (short)
                        this.RawDataMetaChunks[x].CompressVertice(
                            this.RawDataMetaChunks[x].Vertices[y].X, this.BoundingBox.MinX, this.BoundingBox.MaxX);
                    short vy =
                        (short)
                        this.RawDataMetaChunks[x].CompressVertice(
                            this.RawDataMetaChunks[x].Vertices[y].Y, this.BoundingBox.MinY, this.BoundingBox.MaxY);
                    short vz =
                        (short)
                        this.RawDataMetaChunks[x].CompressVertice(
                            this.RawDataMetaChunks[x].Vertices[y].Z, this.BoundingBox.MinZ, this.BoundingBox.MaxZ);
                    newraw.Write(vx);
                    newraw.Write(vy);
                    newraw.Write(vz);
                }

                this.RawDataMetaChunks[x].RawDataChunkInfo[rawchunkid].Offset = newrawsize -
                                                                                this.RawDataMetaChunks[x].HeaderSize;
                newrawsize += this.RawDataMetaChunks[x].RawDataChunkInfo[verticechunkid].ChunkSize *
                              this.RawDataMetaChunks[x].VerticeCount;
                this.RawDataMetaChunks[x].RawDataChunkInfo[rawchunkid].Size =
                    this.RawDataMetaChunks[x].RawDataChunkInfo[verticechunkid].ChunkSize *
                    this.RawDataMetaChunks[x].VerticeCount;
                this.RawDataMetaChunks[x].RawDataChunkInfo[rawchunkid].ChunkSize = 0;

                // this.RawDataMetaChunks[x].RawDataChunkInfo[verticechunkid].ChunkSize * this.RawDataMetaChunks[x].VerticeCount; ;
                newraw.BaseStream.Position = 100;
                newraw.Write(this.RawDataMetaChunks[x].VerticeCount);

                rawchunkid++;

                #endregion

                #region Write UVs

                int uvchunkid = verticechunkid + 1;
                newraw.BaseStream.Position = newrawsize;
                newraw.Write(crsr);
                newrawsize += 4;

                for (int y = 0; y < this.RawDataMetaChunks[x].VerticeCount; y++)
                {
                    short u =
                        (short)
                        this.RawDataMetaChunks[x].CompressVertice(
                            this.RawDataMetaChunks[x].UVs[y].X, this.BoundingBox.MinU, this.BoundingBox.MaxU);
                    short v =
                        (short)
                        this.RawDataMetaChunks[x].CompressVertice(
                            this.RawDataMetaChunks[x].UVs[y].Y, this.BoundingBox.MinV, this.BoundingBox.MaxV);
                    newraw.Write(u);
                    newraw.Write(v);
                }

                this.RawDataMetaChunks[x].RawDataChunkInfo[rawchunkid].Offset = newrawsize -
                                                                                this.RawDataMetaChunks[x].HeaderSize;
                this.RawDataMetaChunks[x].RawDataChunkInfo[rawchunkid].ChunkSize = 1;
                this.RawDataMetaChunks[x].RawDataChunkInfo[rawchunkid].Size = 4 * this.RawDataMetaChunks[x].VerticeCount;
                newrawsize += 4 * this.RawDataMetaChunks[x].VerticeCount;
                rawchunkid++;

                #endregion

                #region Write Normals

                newraw.BaseStream.Position = newrawsize;
                newraw.Write(crsr);
                newrawsize += 4;

                for (int y = 0; y < this.RawDataMetaChunks[x].VerticeCount; y++)
                {
                    int u =
                        (int)
                        this.RawDataMetaChunks[x].CompressVertice(
                            this.RawDataMetaChunks[x].UVs[y].X, this.BoundingBox.MinU, this.BoundingBox.MaxU);
                    int v =
                        (int)
                        this.RawDataMetaChunks[x].CompressVertice(
                            this.RawDataMetaChunks[x].UVs[y].Y, this.BoundingBox.MinV, this.BoundingBox.MaxV);
                    newraw.Write(u);
                    newraw.Write(v);
                    newraw.Write(v);
                }

                this.RawDataMetaChunks[x].RawDataChunkInfo[rawchunkid].Offset = newrawsize -
                                                                                this.RawDataMetaChunks[x].HeaderSize;
                this.RawDataMetaChunks[x].RawDataChunkInfo[rawchunkid].ChunkSize = 2;
                this.RawDataMetaChunks[x].RawDataChunkInfo[rawchunkid].Size = 4 * this.RawDataMetaChunks[x].VerticeCount;
                newrawsize += 12 * this.RawDataMetaChunks[x].VerticeCount;
                rawchunkid++;

                #endregion

                #region Write Other Stuff Not Yet Implemented

                while (rawchunkid < this.RawDataMetaChunks[x].RawDataChunkInfo.Length)
                {
                    oldraw.BaseStream.Position = this.RawDataMetaChunks[x].HeaderSize +
                                                 this.RawDataMetaChunks[x].RawDataChunkInfo[rawchunkid].Offset;
                    newraw.BaseStream.Position = newrawsize;
                    newraw.Write(crsr);
                    newrawsize += 4;
                    newraw.Write(oldraw.ReadBytes(this.RawDataMetaChunks[x].RawDataChunkInfo[rawchunkid].Size));
                    this.RawDataMetaChunks[x].RawDataChunkInfo[rawchunkid].Offset = newrawsize -
                                                                                    this.RawDataMetaChunks[x].HeaderSize;
                    newrawsize += this.RawDataMetaChunks[x].RawDataChunkInfo[rawchunkid].Size;
                    rawchunkid++;
                }

                #endregion

                // footer
                newraw.Write(fklb);
                newrawsize += 4;

                // raw size
                int rawsize = newrawsize - this.RawDataMetaChunks[x].HeaderSize - 4;
                newraw.BaseStream.Position = 4;
                newraw.Write(rawsize);
                meta.raw.rawChunks[x].MS = raw;
                meta.raw.rawChunks[x].size = newrawsize;
            }

            MetaSplitter ms = new MetaSplitter();
            IFPIO io = IFPHashMap.GetIfp(meta.type, map.HaloVersion);
            ms.SplitWithIFP(ref io, ref meta, map);

            #region Write Bounding Box

            int c = 0;
            while (c != -1)
            {
                if (ms.Header.Chunks[0].ChunkResources[c].offset == 20)
                {
                    break;
                }

                c++;
            }

            MetaSplitter.SplitReflexive reflex = ms.Header.Chunks[0].ChunkResources[c] as MetaSplitter.SplitReflexive;

            BinaryWriter BW = new BinaryWriter(reflex.Chunks[0].MS);
            BW.BaseStream.Position = 0;
            BW.Write(this.BoundingBox.MinX);
            BW.Write(this.BoundingBox.MaxX);
            BW.Write(this.BoundingBox.MinY);
            BW.Write(this.BoundingBox.MaxY);
            BW.Write(this.BoundingBox.MinZ);
            BW.Write(this.BoundingBox.MaxZ);
            BW.Write(this.BoundingBox.MinU);
            BW.Write(this.BoundingBox.MaxU);
            BW.Write(this.BoundingBox.MinV);
            BW.Write(this.BoundingBox.MaxV);
            ms.Header.Chunks[0].ChunkResources[c] = reflex;

            #endregion

            #region metachunks

            while (c != -1)
            {
                if (ms.Header.Chunks[0].ChunkResources[c].offset == 36)
                {
                    break;
                }

                c++;
            }

            MetaSplitter.SplitReflexive reflexe = ms.Header.Chunks[0].ChunkResources[c] as MetaSplitter.SplitReflexive;

            for (int l = 0; l < reflexe.Chunks.Count; l++)
            {
                BW = new BinaryWriter(reflexe.Chunks[l].MS);
                BW.BaseStream.Position = 4;
                short facecount = (short)this.RawDataMetaChunks[l].FaceCount;
                BW.Write((short)this.RawDataMetaChunks[l].VerticeCount);
                BW.Write(facecount);
                BW.BaseStream.Position = 68;
                BW.Write(meta.raw.rawChunks[l].size - this.RawDataMetaChunks[l].HeaderSize - 4);

                for (int h = 0; h < reflexe.Chunks[l].ChunkResources.Count; h++)
                {
                    if (reflexe.Chunks[l].ChunkResources[h].offset == 72)
                    {
                        MetaSplitter.SplitReflexive reflexx =
                            reflexe.Chunks[l].ChunkResources[h] as MetaSplitter.SplitReflexive;
                        for (int k = 0; k < reflexx.Chunks.Count; k++)
                        {
                            BinaryWriter BWX = new BinaryWriter(reflexx.Chunks[k].MS);
                            BWX.BaseStream.Position = 6;
                            BWX.Write((short)this.RawDataMetaChunks[l].RawDataChunkInfo[k].ChunkSize);
                            BWX.Write(this.RawDataMetaChunks[l].RawDataChunkInfo[k].Size);
                            BWX.Write(this.RawDataMetaChunks[l].RawDataChunkInfo[k].Offset);
                        }

                        reflexe.Chunks[l].ChunkResources[h] = reflexx;
                        break;
                    }
                }
            }

            while (c != -1)
            {
                if (ms.Header.Chunks[0].ChunkResources[c].offset == 96)
                {
                    break;
                }

                c++;
            }

            reflexe = ms.Header.Chunks[0].ChunkResources[c] as MetaSplitter.SplitReflexive;

            if (this.Shaders.Shader.Length > this.Shaders.count)
            {
                int diff = this.Shaders.Shader.Length - reflexe.Chunks.Count;
                for (int x = 0; x < diff; x++)
                {
                    reflexe.Chunks.Add(reflexe.Chunks[0]);
                }
            }
            else
            {
            }

            #endregion

            Meta fucker = MetaBuilder.BuildMeta(ms, map);
            fucker.raw = meta.raw;

            

            return fucker;
        }

        /// <summary>
        /// The load from obj.
        /// </summary>
        /// <param name="FilePath">The file path.</param>
        /// <remarks></remarks>
        public override void LoadFromOBJ(string FilePath)
        {
            if (FilePath[FilePath.Length - 1] != '\\')
            {
                FilePath += "\\";
            }

            

            FileStream FS = new FileStream(FilePath + this.name + ".mtl", FileMode.Open);
            StreamReader SR = new StreamReader(FS);
            List<string> MaterialNames = new List<string>();
            string temps = string.Empty;
            while (temps != null)
            {
                temps = SR.ReadLine();
                if (temps == null)
                {
                    break;
                }

                string[] split = temps.Split(' ');
                if (split[0] == "newmtl")
                {
                    // if (MaterialNames.IndexOf(split[1]) == -1)
                    // {
                    MaterialNames.Add(split[1]);

                    // }
                }
            }

            if (MaterialNames.Count > this.Shaders.Shader.Length)
            {
                List<ShaderInfo> newlist = new List<ShaderInfo>();
                for (int x = 0; x < this.Shaders.Shader.Length; x++)
                {
                    newlist.Add(this.Shaders.Shader[x]);
                }

                int diff = MaterialNames.Count - this.Shaders.Shader.Length;
                for (int x = 0; x < diff; x++)
                {
                    newlist.Add(this.Shaders.Shader[0]);
                }

                this.Shaders.Shader = newlist.ToArray();
            }

            SR.Close();
            FS.Close();

            

            #region Bounding Box Fields

            float minx = 0;
            float maxx = 0;
            float miny = 0;
            float maxy = 0;
            float minz = 0;
            float maxz = 0;
            float minu = 0;
            float maxu = 0;
            float minv = 0;
            float maxv = 0;

            #endregion

            #region Read OBJ Files

            for (int x = 0; x < this.RawDataMetaChunks.Length; x++)
            {
                #region Fields

                int verticecount = 0;
                int facecount = 0;
                List<Vector3> vertices = new List<Vector3>();
                List<Vector3> normals = new List<Vector3>();
                List<Vector2> uvs = new List<Vector2>();

                List<List<short>> faces = new List<List<short>>();
                List<List<short>> facesuv = new List<List<short>>();
                List<List<short>> facesnormal = new List<List<short>>();
                Hashtable Materials = new Hashtable();
                int groupcount = 0;

                #endregion

                FS = new FileStream(FilePath + this.name + "[" + x + "].obj", FileMode.Open);
                SR = new StreamReader(FS);

                #region ParseFile

                do
                {
                    temps = SR.ReadLine();
                    if (temps == null)
                    {
                        continue;
                    }

                    temps = temps.Replace("  ", " ");
                    string[] tempstrings = temps.Split(',', ' ');
                    switch (tempstrings[0])
                    {
                            #region Vertices

                        case "v":
                            Vector3 tempv = new Vector3();
                            tempv.X = float.Parse(tempstrings[1]);
                            tempv.Y = float.Parse(tempstrings[2]);
                            tempv.Z = float.Parse(tempstrings[3]);
                            if (tempv.X < minx)
                            {
                                minx = tempv.X;
                            }

                            if (tempv.X > maxx)
                            {
                                maxx = tempv.X;
                            }

                            if (tempv.Y < miny)
                            {
                                miny = tempv.Y;
                            }

                            if (tempv.Y > maxy)
                            {
                                maxy = tempv.Y;
                            }

                            if (tempv.Z < minz)
                            {
                                minz = tempv.Z;
                            }

                            if (tempv.Z > maxz)
                            {
                                maxz = tempv.Z;
                            }

                            vertices.Add(tempv);
                            verticecount++;

                            break;

                            #endregion

                            #region Normals

                        case "vn":
                            Vector3 tempvn = new Vector3();
                            tempvn.X = float.Parse(tempstrings[1]);
                            tempvn.Y = float.Parse(tempstrings[2]);
                            tempvn.Z = float.Parse(tempstrings[3]);
                            normals.Add(tempvn);

                            break;

                            #endregion

                            #region UVs

                        case "vt":
                            Vector2 tempv2 = new Vector2();
                            tempv2.X = float.Parse(tempstrings[1]);
                            tempv2.Y = float.Parse(tempstrings[2]);
                            if (tempv2.X < minu)
                            {
                                minu = tempv2.X;
                            }

                            if (tempv2.X > maxu)
                            {
                                maxu = tempv2.X;
                            }

                            if (tempv2.Y < minv)
                            {
                                minv = tempv2.Y;
                            }

                            if (tempv2.Y > maxv)
                            {
                                maxv = tempv2.Y;
                            }

                            uvs.Add(tempv2);
                            verticecount++;
                            break;

                            #endregion

                            #region Group

                        case "g":

                            List<short> templist = new List<short>();
                            List<short> templist2 = new List<short>();
                            List<short> templist3 = new List<short>();
                            faces.Add(templist);
                            facesuv.Add(templist2);
                            facesnormal.Add(templist3);
                            groupcount++;
                            break;

                            #endregion

                            #region Faces

                        case "f":
                            string[] split1 = tempstrings[1].Split('/');
                            string[] split2 = tempstrings[2].Split('/');
                            string[] split3 = tempstrings[3].Split('/');
                            short temp1 = short.Parse(split1[0]);
                            short temp2 = short.Parse(split2[0]);
                            short temp3 = short.Parse(split3[0]);
                            temp1--;
                            temp2--;
                            temp3--;
                            faces[groupcount - 1].Add(temp1);
                            faces[groupcount - 1].Add(temp2);
                            faces[groupcount - 1].Add(temp3);

                            temp1 = short.Parse(split1[1]);
                            temp2 = short.Parse(split2[1]);
                            temp3 = short.Parse(split3[1]);
                            temp1--;
                            temp2--;
                            temp3--;
                            facesuv[groupcount - 1].Add(temp1);
                            facesuv[groupcount - 1].Add(temp2);
                            facesuv[groupcount - 1].Add(temp3);

                            temp1 = short.Parse(split1[2]);
                            temp2 = short.Parse(split2[2]);
                            temp3 = short.Parse(split3[2]);
                            temp1--;
                            temp2--;
                            temp3--;
                            facesnormal[groupcount - 1].Add(temp1);
                            facesnormal[groupcount - 1].Add(temp2);
                            facesnormal[groupcount - 1].Add(temp3);
                            facecount += 3;
                            break;

                            #endregion

                            #region Materials

                        case "usemtl":
                            Materials.Add(groupcount - 1, tempstrings[1]);
                            break;

                            #endregion
                    }
                }
                while (temps != null);

                #endregion

                SR.Close();
                FS.Close();
                int count = 0;
                while (count < faces.Count)
                {
                    start:
                    if (faces[count].Count == 0)
                    {
                        faces.RemoveAt(count);
                        facesuv.RemoveAt(count);
                        facesnormal.RemoveAt(count);
                        count = 0;
                        groupcount--;
                        goto start;
                    }

                    count++;
                }

                Renderer temprender = new Renderer();
                Panel fakepanel = new Panel();
                temprender.CreateDevice(fakepanel);
                List<List<short>> Faces = new List<List<short>>();
                List<List<short>> Facesuv = new List<List<short>>();
                List<List<short>> Facesnormal = new List<List<short>>();
                List<short> newIndices = new List<short>();

                #region Submeshes

                this.RawDataMetaChunks[x].SubMeshInfo = new RawDataMetaChunk.ModelSubMeshInfo[groupcount];
                int totalindicecount = 0;
                this.RawDataMetaChunks[x].Vertices.Clear();
                this.RawDataMetaChunks[x].UVs.Clear();
                this.RawDataMetaChunks[x].Normals.Clear();
                for (int y = 0; y < groupcount; y++)
                {
                    Faces.Add(new List<short>());
                    for (int h = 0; h < faces[y].Count; h++)
                    {
                        int tempvert = faces[y][h];
                        int tempuv = facesuv[y][h];
                        int tempnorm = facesnormal[y][h];
                        for (int i = 0; i < y + 1; i++)
                        {
                            for (int j = 0; j < faces[i].Count; j++)
                            {
                                if (i == y && j == h)
                                {
                                    goto gohere1;
                                }

                                int tempvert2 = faces[i][j];
                                int tempuv2 = facesuv[i][j];
                                int tempnorm2 = facesnormal[i][j];
                                if (tempvert == tempvert2 && tempuv == tempuv2 && tempnorm == tempnorm2)
                                {
                                    Faces[y].Add(Faces[i][j]);

                                    goto gohere;
                                }
                            }
                        }

                        gohere1:
                        this.RawDataMetaChunks[x].UVs.Add(uvs[facesuv[y][h]]);
                        this.RawDataMetaChunks[x].Vertices.Add(vertices[faces[y][h]]);
                        this.RawDataMetaChunks[x].Normals.Add(normals[facesnormal[y][h]]);
                        Faces[y].Add((short)(this.RawDataMetaChunks[x].Vertices.Count - 1));
                        gohere:
                        ;
                    }

                    #region Make Triangle Strip

                    Mesh m = temprender.MakeMesh(
                        this.RawDataMetaChunks[x].Vertices, Faces[y], this.RawDataMetaChunks[x].UVs);
                    //int striplength = 0;
                    int indicecount = 0;
                    unsafe
                    {
                        int[] adj = new int[m.NumberFaces * 3];
                        m.GenerateAdjacency(0.0f, adj);
                        int[] test;
                        int[] test2;
                        GraphicsStream oi;
                        Mesh oid = m.Optimize(
                            MeshFlags.OptimizeIgnoreVerts | MeshFlags.OptimizeAttributeSort, 
                            adj, 
                            out test, 
                            out test2, 
                            out oi);

                        IndexBuffer ib = oid.IndexBuffer;

                        ib = Mesh.ConvertMeshSubsetToSingleStrip(
                            oid, 
                            0, 
                            MeshFlags.OptimizeIgnoreVerts | MeshFlags.OptimizeStripeReorder | MeshFlags.IbManaged, 
                            out indicecount);
                        GraphicsStream xxxx = ib.Lock(0, 0, LockFlags.None);
                        short* ind = (short*)xxxx.InternalData.ToPointer();

                        for (int z = 0; z < indicecount; z++)
                        {
                            newIndices.Add(ind[z]);
                        }
                    }

                    #endregion

                    #region SubmeshInfo

                    RawDataMetaChunk.ModelSubMeshInfo submesh = new RawDataMetaChunk.ModelSubMeshInfo();
                    submesh.IndiceStart = totalindicecount;
                    totalindicecount += indicecount;
                    submesh.IndiceCount = indicecount;
                    submesh.ShaderNumber = 0;
                    object tempobject = Materials[y];
                    if (tempobject != null)
                    {
                        int tempint = MaterialNames.IndexOf((string)tempobject);
                        if (tempint != -1)
                        {
                            submesh.ShaderNumber = tempint;
                        }
                    }

                    this.RawDataMetaChunks[x].SubMeshInfo[y] = submesh;

                    #endregion
                }

                #endregion

                this.RawDataMetaChunks[x].Indices = newIndices.ToArray();
                this.RawDataMetaChunks[x].IndiceCount = this.RawDataMetaChunks[x].Indices.Length;

                this.RawDataMetaChunks[x].VerticeCount = this.RawDataMetaChunks[x].Vertices.Count;

                this.RawDataMetaChunks[x].FaceCount = facecount / 3;
            }

            #region Bounding Box

            this.BoundingBox.MinX = minx;
            this.BoundingBox.MaxX = maxx;
            this.BoundingBox.MinY = miny;
            this.BoundingBox.MaxY = maxy;
            this.BoundingBox.MinZ = minz;
            this.BoundingBox.MaxZ = maxz;
            this.BoundingBox.MinU = minu;
            this.BoundingBox.MaxU = maxu;
            this.BoundingBox.MinV = minv;
            this.BoundingBox.MaxV = maxv;

            #endregion

            #endregion
        }

        #endregion

        /// <summary>
        /// The prtm raw data meta chunk.
        /// </summary>
        /// <remarks></remarks>
        public class PRTMRawDataMetaChunk : RawDataMetaChunk
        {
            #region Constructors and Destructors

            /// <summary>
            /// Initializes a new instance of the <see cref="PRTMRawDataMetaChunk"/> class.
            /// </summary>
            /// <param name="meta">The meta.</param>
            /// <remarks></remarks>
            public PRTMRawDataMetaChunk(ref Meta meta)
            {
                BinaryReader BR = new BinaryReader(meta.MS);

                BR.BaseStream.Position = 176;
                int tempc = BR.ReadInt32();
                int tempr = BR.ReadInt32() - meta.Map.SecondaryMagic - meta.offset;
                RawDataChunkInfo = new RawDataOffsetChunk[tempc];
                for (int x = 0; x < tempc; x++)
                {
                    RawDataChunkInfo[x] = new RawDataOffsetChunk();
                    BR.BaseStream.Position = tempr + (x * 16) + 6;
                    RawDataChunkInfo[x].ChunkSize = 56;
                    RawDataChunkInfo[x].Size = BR.ReadInt32();
                    if (RawDataChunkInfo[x].ChunkSize == 0)
                    {
                        RawDataChunkInfo[x].ChunkSize = RawDataChunkInfo[x].Size;
                    }

                    RawDataChunkInfo[x].Offset = BR.ReadInt32();
                    RawDataChunkInfo[x].ChunkCount = RawDataChunkInfo[x].Size / RawDataChunkInfo[x].ChunkSize;
                }

                BR.BaseStream.Position = 136;
                VerticeCount = BR.ReadInt32();
                tempr = BR.ReadInt32() - meta.Map.SecondaryMagic - meta.offset;
                BR.BaseStream.Position = tempr;
                for (int x = 0; x < VerticeCount; x++)
                {
                    Vector3 vec = new Vector3();

                    vec.X = BR.ReadSingle();
                    vec.Y = BR.ReadSingle();
                    vec.Z = BR.ReadSingle();
                    Vertices.Add(vec);

                    Vector3 vec2 = new Vector3();
                    vec2.X = BR.ReadSingle();
                    vec2.Y = BR.ReadSingle();
                    vec2.Z = BR.ReadSingle();
                    Normals.Add(vec2);

                    Vector3 vec3 = new Vector3();
                    vec3.X = BR.ReadSingle();
                    vec3.Y = BR.ReadSingle();
                    vec3.Z = BR.ReadSingle();
                    Binormals.Add(vec3);

                    Vector3 vec4 = new Vector3();
                    vec4.X = BR.ReadSingle();
                    vec4.Y = BR.ReadSingle();
                    vec4.Z = BR.ReadSingle();
                    Tangents.Add(vec4);

                    Vector2 vec5 = new Vector2();
                    vec5.X = BR.ReadSingle();
                    vec5.Y = BR.ReadSingle();

                    UVs.Add(vec5);
                }

                BR.BaseStream.Position = 168;
                HeaderSize = BR.ReadInt32() + 8;

                BR.BaseStream.Position = 144;
                IndiceCount = BR.ReadInt32();
                tempr = BR.ReadInt32() - meta.Map.SecondaryMagic - meta.offset;
                BR.BaseStream.Position = tempr;
                FaceCount = IndiceCount / 3;

                this.Indices = new short[IndiceCount];
                for (int x = 0; x < IndiceCount; x++)
                {
                    Indices[x] = (short)BR.ReadUInt16();
                }

                SubMeshInfo = new ModelSubMeshInfo[1];
                for (int x = 0; x < 1; x++)
                {
                    SubMeshInfo[x] = new ModelSubMeshInfo();
                    BR.BaseStream.Position = HeaderSize + RawDataChunkInfo[0].Offset + (x * 72) + 4;
                    SubMeshInfo[x].ShaderNumber = 0;
                    SubMeshInfo[x].IndiceStart = 0;
                    SubMeshInfo[x].IndiceCount = IndiceCount;
                }
            }

            #endregion
        }
    }
}