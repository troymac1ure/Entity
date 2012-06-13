// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Model.cs" company="">
//   
// </copyright>
// <summary>
//   The halo vertex.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HaloMap.RawData
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Drawing.Imaging;
    using System.Globalization;
    using System.IO;
    using System.Windows.Forms;

    using HaloMap.ChunkCloning;
    using HaloMap.H2MetaContainers;
    using HaloMap.Map;
    using HaloMap.Meta;
    using HaloMap.Plugins;
    using HaloMap.Render;

    using Microsoft.DirectX;
    using Microsoft.DirectX.Direct3D;

    /// <summary>
    /// Summary description for Model.
    /// </summary>
    /// <remarks></remarks>
    public class ParsedModel : IDisposable
    {
        #region Constants and Fields

        /// <summary>
        /// 
        /// </summary>
        protected Map map;

        /// <summary>
        /// The bounding box.
        /// </summary>
        public BoundingBoxContainer BoundingBox;

        /// <summary>
        /// The display.
        /// </summary>
        public DisplayedInfo Display;

        /// <summary>
        /// The frames.
        /// </summary>
        public FrameHierarchy Frames;

        /// <summary>
        /// The lod.
        /// </summary>
        public LODInfo LOD;

        /// <summary>
        /// The permutation string.
        /// </summary>
        public string PermutationString;

        /// <summary>
        /// The raw data meta chunks.
        /// </summary>
        public RawDataMetaChunk[] RawDataMetaChunks;

        /// <summary>
        /// The shaders.
        /// </summary>
        public ShaderContainer Shaders;

        /// <summary>
        /// The hlmt.
        /// </summary>
        public hlmtContainer hlmt;

        /// <summary>
        /// The name.
        /// </summary>
        public string name;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ParsedModel"/> class.
        /// </summary>
        /// <remarks></remarks>
        public ParsedModel()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParsedModel"/> class.
        /// </summary>
        /// <param name="meta">The meta.</param>
        /// <remarks></remarks>
        public ParsedModel(ref Meta meta)
        {
            this.map = meta.Map;

            switch (meta.Map.HaloVersion)
            {
                case HaloVersionEnum.Halo2:
                    H2ParsedModel(ref meta);
                    break;
                case HaloVersionEnum.Halo1:
                case HaloVersionEnum.HaloCE:
                    CEParsedModel(ref meta);
                    break;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// The compress normal.
        /// </summary>
        /// <param name="normal">The normal.</param>
        /// <returns>The compress normal.</returns>
        /// <remarks></remarks>
        public static int CompressNormal(Vector3 normal)
        {
            bool negx = false;
            if (normal.X < 0)
            {
                negx = true;
                normal.X = -normal.X;
            }

            float bbb = normal.X * Convert.ToSingle(0x3ff);
            int b = Convert.ToInt32(bbb);
            if (negx)
            {
                b = -b + 1 | 0x400;
            }

            bool negy = false;
            if (normal.Y < 0)
            {
                negy = true;
                normal.Y = -normal.Y;
            }

            float bbb2 = normal.Y * Convert.ToSingle(0x3ff);
            int b2 = Convert.ToInt32(bbb2);
            if (negy)
            {
                b2 = -b2 + 1 | 0x400;
            }

            b2 <<= 11;

            bool negz = false;
            if (normal.Z < 0)
            {
                negz = true;
                normal.Z = -normal.Z;
            }

            float bbb3 = normal.Z * Convert.ToSingle(0x1ff);
            int b3 = Convert.ToInt32(bbb3);
            if (negz)
            {
                b3 = -b3 + 1 | 0x200;
            }

            b3 <<= 22;

            int result = b3 | b2 | b;

            return result;
        }

        /// <summary>
        /// The decompress normal.
        /// </summary>
        /// <param name="compressednormal">The compressednormal.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static Vector3 DecompressNormal(int compressednormal)
        {
            int xx = compressednormal & 0x3ff;
            float xxx = Convert.ToSingle(xx) / Convert.ToSingle(0x3ff);
            int bitx = (compressednormal >> 10) & 0x1;
            if (bitx == 1)
            {
                xxx = 1 - xxx;
                xxx = -xxx;
            }

            int yy = (compressednormal >> 11) & 0x3ff;
            float yyy = Convert.ToSingle(yy) / Convert.ToSingle(0x3ff);
            int bity = (compressednormal >> 21) & 0x1;
            if (bity == 1)
            {
                yyy = 1 - yyy;
                yyy = -yyy;
            }

            int zz = (compressednormal >> 22) & 0x1ff;
            float zzz = Convert.ToSingle(zz) / Convert.ToSingle(0x1ff);
            int bitz = (compressednormal >> 31) & 0x1;
            if (bitz == 1)
            {
                zzz = 1 - zzz;
                zzz = -zzz;
            }

            return new Vector3(xxx, yyy, zzz);
        }

        /// <summary>
        /// The ce parsed model.
        /// </summary>
        /// <param name="meta">The meta.</param>
        /// <remarks></remarks>
        public void CEParsedModel(ref Meta meta)
        {
            Display = new DisplayedInfo();
            string[] temps = meta.name.Split('\\');
            name = temps[temps.Length - 1];
            BoundingBox = new BoundingBoxContainer();
            BinaryReader BR = new BinaryReader(meta.MS);
            BR.BaseStream.Position = 208;
            int tempc = BR.ReadInt32();
            int tempr = BR.ReadInt32() - meta.magic - meta.offset;
            BR.BaseStream.Position = tempr + 36;
            tempc = BR.ReadInt32();
            RawDataMetaChunks = new RawDataMetaChunk[meta.raw.rawChunks.Count / 2];
            for (int x = 0; x < meta.raw.rawChunks.Count / 2; x++)
            {
                RawDataMetaChunks[x] = new RawDataMetaChunk();
                RawDataMetaChunks[x].IndiceCount = meta.raw.rawChunks[x * 2].size / 2;
                BinaryReader BRX = new BinaryReader(meta.raw.rawChunks[x * 2].MS);
                BRX.BaseStream.Position = 0;
                RawDataMetaChunks[x].Indices = new short[RawDataMetaChunks[x].IndiceCount];
                for (int y = 0; y < RawDataMetaChunks[x].IndiceCount; y++)
                {
                    RawDataMetaChunks[x].Indices[y] = BRX.ReadInt16();
                }

                RawDataMetaChunks[x].Indices = Renderer.DecompressIndices(
                    RawDataMetaChunks[x].Indices, 0, RawDataMetaChunks[x].Indices.Length);
                RawDataMetaChunks[x].IndiceCount = RawDataMetaChunks[x].Indices.Length;
                RawDataMetaChunks[x].FaceCount = RawDataMetaChunks[x].IndiceCount / 3;
                RawDataMetaChunks[x].SubMeshInfo = new RawDataMetaChunk.ModelSubMeshInfo[1];
                RawDataMetaChunks[x].SubMeshInfo[0] = new RawDataMetaChunk.ModelSubMeshInfo();
                RawDataMetaChunks[x].SubMeshInfo[0].IndiceCount = RawDataMetaChunks[x].IndiceCount;
                RawDataMetaChunks[x].SubMeshInfo[0].ShaderNumber = meta.raw.rawChunks[x * 2].shadernumber;
                RawDataMetaChunks[x].Vertices = new List<Vector3>();
                RawDataMetaChunks[x].UVs = new List<Vector2>();
                RawDataMetaChunks[x].Normals = new List<Vector3>();
                RawDataMetaChunks[x].Binormals = new List<Vector3>();
                RawDataMetaChunks[x].Tangents = new List<Vector3>();
                int chunksize = 68;
                if (meta.Map.HaloVersion == HaloVersionEnum.Halo1)
                {
                    chunksize = 32;
                }

                RawDataMetaChunks[x].VerticeCount = meta.raw.rawChunks[x * 2 + 1].size / chunksize;
                BRX = new BinaryReader(meta.raw.rawChunks[x * 2 + 1].MS);

                for (int y = 0; y < RawDataMetaChunks[x].VerticeCount; y++)
                {
                    BRX.BaseStream.Position = y * chunksize;
                    Vector3 vertice = new Vector3(BRX.ReadSingle(), BRX.ReadSingle(), BRX.ReadSingle());
                    RawDataMetaChunks[x].Vertices.Add(vertice);
                    switch (meta.Map.HaloVersion)
                    {
                        case HaloVersionEnum.HaloCE:
                            Vector3 normal = new Vector3(BRX.ReadSingle(), BRX.ReadSingle(), BRX.ReadSingle());
                            RawDataMetaChunks[x].Normals.Add(normal);
                            Vector3 binormal = new Vector3(BRX.ReadSingle(), BRX.ReadSingle(), BRX.ReadSingle());
                            RawDataMetaChunks[x].Binormals.Add(binormal);
                            Vector3 tangent = new Vector3(BRX.ReadSingle(), BRX.ReadSingle(), BRX.ReadSingle());
                            RawDataMetaChunks[x].Tangents.Add(tangent);
                            Vector2 uv = new Vector2(BRX.ReadSingle(), BRX.ReadSingle());

                            RawDataMetaChunks[x].UVs.Add(uv);
                            break;
                        case HaloVersionEnum.Halo1:
                            int test = BRX.ReadInt32();
                            Vector3 normal2 = DecompressNormal(test);
                            RawDataMetaChunks[x].Normals.Add(normal2);
                            test = BRX.ReadInt32();
                            Vector3 binormal2 = DecompressNormal(test);
                            RawDataMetaChunks[x].Binormals.Add(binormal2);
                            test = BRX.ReadInt32();
                            Vector3 tangent2 = DecompressNormal(test);
                            RawDataMetaChunks[x].Tangents.Add(tangent2);
                            short testx = BRX.ReadInt16();
                            float u = DecompressVertice(Convert.ToSingle(testx), -1, 1); // %1 ;
                            testx = BRX.ReadInt16();
                            float v = DecompressVertice(Convert.ToSingle(testx), -1, 1); // % 1;
                            Vector2 uv2 = new Vector2(u, v);

                            RawDataMetaChunks[x].UVs.Add(uv2);
                            break;
                    }
                }

                if (x < tempc)
                {
                    Display.Chunk.Add(x);
                }
            }

            Shaders = new ShaderContainer();

            BR.BaseStream.Position = 220;
            tempc = BR.ReadInt32();
            tempr = BR.ReadInt32() - meta.magic - meta.offset;
            Shaders.Shader = new ShaderInfo[tempc];
            Display.ShaderIndex = new int[tempc];
            for (int x = 0; x < tempc; x++)
            {
                Display.ShaderIndex[x] = x;
                BR.BaseStream.Position = tempr + (x * 32) + 12;
                int temptag = meta.Map.Functions.ForMeta.FindMetaByID(BR.ReadInt32());
                Shaders.Shader[x] = new ShaderInfo(temptag, meta.Map);
            }
        }

        /// <summary>
        /// The decompress vertice.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="min">The min.</param>
        /// <param name="max">The max.</param>
        /// <returns>The decompress vertice.</returns>
        /// <remarks></remarks>
        public float DecompressVertice(float input, float min, float max)
        {
            float percent = (input + 32768) / 65535;
            float result = (percent * (max - min)) + min;
            return result;
        }

        /// <summary>
        /// The extract mesh.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <remarks></remarks>
        public void ExtractMesh(string path)
        {
            if (path[path.Length - 1] != '\\')
            {
                path += "\\";
            }

            string texturepath = path + "Textures\\";
            Directory.CreateDirectory(texturepath);

            List<string> names = new List<string>();
            string mtllib = this.name + ".mtl";
            FileStream FS = new FileStream(path + mtllib, FileMode.Create);
            StreamWriter SW = new StreamWriter(FS);
            for (int x = 0; x < this.Shaders.Shader.Length; x++)
            {
                string[] namesplit = this.Shaders.Shader[x].shaderName.Split('\\');
                string temps = namesplit[namesplit.Length - 1];
                names.Add(temps);
                this.Shaders.Shader[x].MainBitmap.Save(texturepath + temps + ".bmp", ImageFormat.Bmp);
                if (x > 0)
                {
                    SW.WriteLine(string.Empty);
                }

                SW.WriteLine("newmtl " + temps);
                SW.WriteLine("Ka 1.000000 1.000000 1.000000");
                SW.WriteLine("Kd 1.000000 1.000000 1.000000");
                SW.WriteLine("Ks 1.000000 1.000000 1.000000");
                SW.WriteLine("Ns 0.000000");
                SW.WriteLine("map_Kd .\\Textures\\" + temps + ".bmp");
            }

            SW.Close();
            FS.Close();

            FS = new FileStream(path + this.name + ".obj", FileMode.Create);
            SW = new StreamWriter(FS);

            SW.WriteLine("# ----- -------------------------------");
            SW.WriteLine("# Halo 2 Model - Extracted with Entity");
            SW.WriteLine("# ------------------------------------");
            SW.WriteLine("mtllib " + mtllib);

            for (int x = 0; x < this.RawDataMetaChunks.Length; x++)
            {
                for (int y = 0; y < this.RawDataMetaChunks[x].VerticeCount; y++)
                {
                    string temps = "v " + this.RawDataMetaChunks[x].Vertices[y].X + " " +
                                   this.RawDataMetaChunks[x].Vertices[y].Y + " " +
                                   this.RawDataMetaChunks[x].Vertices[y].Z;
                    SW.WriteLine(temps);
                }
            }

            for (int x = 0; x < this.RawDataMetaChunks.Length; x++)
            {
                if (this.RawDataMetaChunks[x].permutation == this.LOD.PermutationStrings[0] &&
                    this.RawDataMetaChunks[x].lod == 4)
                {
                    continue;
                }

                // SW.WriteLine("# " + this.RawDataMetaChunks[x].Vertices.Length.ToString() + " vertices");
                for (int y = 0; y < this.RawDataMetaChunks[x].VerticeCount; y++)
                {
                    string temps = "vt " + this.RawDataMetaChunks[x].UVs[y].X + " " + this.RawDataMetaChunks[x].UVs[y].Y;
                    SW.WriteLine(temps);
                }
            }

            // SW.WriteLine("# " + this.RawDataMetaChunks[x].Vertices.Length.ToString() + " texture vertices");
            for (int x = 0; x < this.RawDataMetaChunks.Length; x++)
            {
                if (this.RawDataMetaChunks[x].permutation == this.LOD.PermutationStrings[0] &&
                    this.RawDataMetaChunks[x].lod == 4)
                {
                    continue;
                }

                for (int y = 0; y < this.RawDataMetaChunks[x].VerticeCount; y++)
                {
                    string temps = "vn " + 1.0 + " " + 1.0 + " " + 1.0;
                    SW.WriteLine(temps);
                }

                SW.WriteLine("# " + this.RawDataMetaChunks[x].Vertices.Count + " normals");
            }

            int vertcount = 0;
            for (int x = 0; x < this.RawDataMetaChunks.Length; x++)
            {
                if (this.RawDataMetaChunks[x].permutation == this.LOD.PermutationStrings[0] &&
                    this.RawDataMetaChunks[x].lod == 4)
                {
                    continue;
                }

                for (int y = 0; y < this.RawDataMetaChunks[x].SubMeshInfo.Length; y++)
                {
                    SW.WriteLine("g " + x + "." + y);
                    SW.WriteLine("s " + x + "." + y);
                    SW.WriteLine("usemtl  " + names[this.RawDataMetaChunks[x].SubMeshInfo[y].ShaderNumber]);

                    short[] shite = new short[100000];
                    int s = 0;
                    if (this.RawDataMetaChunks[x].FaceCount * 3 != this.RawDataMetaChunks[x].IndiceCount)
                    {
                        int m = this.RawDataMetaChunks[x].SubMeshInfo[y].IndiceStart;

                        bool dir = false;
                        short tempx;
                        short tempy;
                        short tempz;

                        do
                        {
                            // if (mode.EndOfIndices[x][j]>m+2){break;}
                            tempx = this.RawDataMetaChunks[x].Indices[m];
                            tempy = this.RawDataMetaChunks[x].Indices[m + 1];
                            tempz = this.RawDataMetaChunks[x].Indices[m + 2];
                            tempx += (short)vertcount;
                            tempy += (short)vertcount;
                            tempz += (short)vertcount;

                            if (tempx != tempy && tempx != tempz && tempy != tempz)
                            {
                                if (dir == false)
                                {
                                    shite[s] = tempx;
                                    shite[s + 1] = tempy;
                                    shite[s + 2] = tempz;
                                    s += 3;

                                    dir = true;
                                }
                                else
                                {
                                    shite[s] = tempx;
                                    shite[s + 1] = tempz;
                                    shite[s + 2] = tempy;
                                    s += 3;
                                    dir = false;
                                }

                                m += 1;
                            }
                            else
                            {
                                if (dir)
                                {
                                    dir = false;
                                }
                                else
                                {
                                    dir = true;
                                }

                                m += 1;
                            }
                        }
                        while (m <
                               this.RawDataMetaChunks[x].SubMeshInfo[y].IndiceStart +
                               this.RawDataMetaChunks[x].SubMeshInfo[y].IndiceCount - 2);
                    }
                    else
                    {
                        Array.Copy(
                            this.RawDataMetaChunks[x].Indices, 
                            this.RawDataMetaChunks[x].SubMeshInfo[y].IndiceStart, 
                            shite, 
                            0, 
                            this.RawDataMetaChunks[x].SubMeshInfo[y].IndiceCount);
                        s = this.RawDataMetaChunks[x].SubMeshInfo[y].IndiceCount;

                        // this.RawDataMetaChunks[x].IndiceCount;
                    }

                    for (int xx = 0; xx < s; xx += 3)
                    {
                        string temps = "f " + (shite[xx] + 1) + "/" + (shite[xx] + 1) + "/" + (shite[xx] + 1) + " " +
                                       (shite[xx + 1] + 1) + "/" + (shite[xx + 1] + 1) + "/" + (shite[xx + 1] + 1) + " " +
                                       (shite[xx + 2] + 1) + "/" + (shite[xx + 2] + 1) + "/" + (shite[xx + 2] + 1);
                        SW.WriteLine(temps);
                    }

                    SW.WriteLine("# " + (s / 3) + " elements");
                }

                vertcount += this.RawDataMetaChunks[x].VerticeCount;
            }

            SW.Close();
            FS.Close();
        }

        /// <summary>
        /// The extract meshes to obj.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <remarks></remarks>
        public virtual void ExtractMeshesToOBJ(string path)
        {
            if (path[path.Length - 1] != '\\')
            {
                path += "\\";
            }

            string texturepath = path + "Textures\\";
            Directory.CreateDirectory(texturepath);

            List<string> names = new List<string>();
            string mtllib = this.name + ".mtl";
            FileStream FS = new FileStream(path + mtllib, FileMode.Create);
            StreamWriter SW = new StreamWriter(FS);
            if (Shaders == null)
            {
                goto hce1;
            }

            Panel p = new Panel();
            Renderer r = new Renderer();
            r.CreateDevice(p);

            for (int x = 0; x < this.Shaders.Shader.Length; x++)
            {
                string[] namesplit = this.Shaders.Shader[x].shaderName.Split('\\');
                string temps = namesplit[namesplit.Length - 1];
                temps = temps.Replace(' ', '_');
                names.Add(temps);

                this.Shaders.Shader[x].MakeTextures(ref r.device);

                // if (this.Shaders.Shader[x].MainTexture == null) { continue; }
                TextureLoader.Save(
                    texturepath + temps + ".dds", ImageFileFormat.Dds, this.Shaders.Shader[x].MainTexture);

                // this.Shaders.Shader[x].MainBitmap.Save(texturepath + temps + ".gif", System.Drawing.Imaging.ImageFormat.Gif);
                if (x > 0)
                {
                    SW.WriteLine(string.Empty);
                }

                SW.WriteLine("newmtl " + temps);
                SW.WriteLine("Ka 1.000000 1.000000 1.000000");
                SW.WriteLine("Kd 1.000000 1.000000 1.000000");
                SW.WriteLine("Ks 1.000000 1.000000 1.000000");
                SW.WriteLine("Ns 0.000000");
                SW.WriteLine("map_Kd .\\Textures\\" + temps + ".dds");
            }

            SW.Close();
            FS.Close();

            hce1:
            FileStream FS2 = new FileStream(path + this.name + ".obj", FileMode.Create);
            StreamWriter SW2 = new StreamWriter(FS2);
            int faces = 0;
            int pass = 0;
            SW2.WriteLine("# ------------------------------------");
            SW2.WriteLine("# Halo 2 Model - Extracted with Entity");
            SW2.WriteLine("# ------------------------------------");
            for (int x = 0; x < this.RawDataMetaChunks.Length; x++)
            {
                SW2.WriteLine();
                SW2.WriteLine("g Section_" + x.ToString().PadLeft(3, '0'));
                writeOBJ(SW2, mtllib, this.RawDataMetaChunks[x], names, ref pass, ref faces);

                FS = new FileStream(path + this.name + "[" + x + "].obj", FileMode.Create);
                SW = new StreamWriter(FS);
                SW.WriteLine("# ------------------------------------");
                SW.WriteLine("# Halo 2 Model - Extracted with Entity");
                SW.WriteLine("# ------------------------------------");
                int ps = 0;
                int fc = 0;
                writeOBJ(SW, mtllib, this.RawDataMetaChunks[x], names, ref ps, ref fc);

                /*
                SW.WriteLine("mtllib " + mtllib);

                for (int y = 0; y < this.RawDataMetaChunks[x].VerticeCount; y++)
                {
                    string temps = "v " + this.RawDataMetaChunks[x].Vertices[y].X.ToString()
                                 + " " + this.RawDataMetaChunks[x].Vertices[y].Y.ToString()
                                 + " " + this.RawDataMetaChunks[x].Vertices[y].Z.ToString();
                    SW.WriteLine(temps);
                }
                SW.WriteLine("# " + this.RawDataMetaChunks[x].Vertices.Count.ToString() + " vertices");

                for (int y = 0; y < this.RawDataMetaChunks[x].VerticeCount; y++)

                {
                    string temps = "vt " + this.RawDataMetaChunks[x].UVs[y].X.ToString()
                                 + " " + this.RawDataMetaChunks[x].UVs[y].Y.ToString();

                    SW.WriteLine(temps);
                }
                SW.WriteLine("# " + this.RawDataMetaChunks[x].Vertices.Count.ToString() + " texture vertices");
                for (int y = 0; y < this.RawDataMetaChunks[x].VerticeCount; y++)
                {
                    string temps = "vn " + this.RawDataMetaChunks[x].Normals[y].X
                                 + " " + this.RawDataMetaChunks[x].Normals[y].Y
                                 + " " + this.RawDataMetaChunks[x].Normals[y].Z;
                    SW.WriteLine(temps);
                }
                SW.WriteLine("# " + this.RawDataMetaChunks[x].Vertices.Count.ToString() + " normals");

                for (int y = 0; y < this.RawDataMetaChunks[x].SubMeshInfo.Length; y++)
                {

                    SW.WriteLine("g 0." + y.ToString());
               //     SW.WriteLine("s 0." + y.ToString());
                    try
                    {
                        SW.WriteLine("usemtl  " + names[this.RawDataMetaChunks[x].SubMeshInfo[y].ShaderNumber]);
                    }
                    catch
                    {
                    }
                    short[] shite = new short[100000];
                    int s = 0;
                    if (this.RawDataMetaChunks[x].FaceCount * 3 != this.RawDataMetaChunks[x].IndiceCount)
                    {

                        int m = this.RawDataMetaChunks[x].SubMeshInfo[y].IndiceStart;

                        bool dir = false;
                        short tempx;
                        short tempy;
                        short tempz;

                        do
                        {
                            //if (mode.EndOfIndices[x][j]>m+2){break;}

                            tempx = this.RawDataMetaChunks[x].Indices[m];
                            tempy = this.RawDataMetaChunks[x].Indices[m + 1];
                            tempz = this.RawDataMetaChunks[x].Indices[m + 2];

                            if (tempx != tempy && tempx != tempz && tempy != tempz)
                            {
                                if (dir == false)
                                {
                                    shite[s] = tempx;
                                    shite[s + 1] = tempy;
                                    shite[s + 2] = tempz;
                                    s += 3;

                                    dir = true;
                                }
                                else
                                {
                                    shite[s] = tempx;
                                    shite[s + 1] = tempz;
                                    shite[s + 2] = tempy;
                                    s += 3;
                                    dir = false;
                                }
                                m += 1;
                            }
                            else
                            {
                                if (dir == true) { dir = false; }
                                else { dir = true; }

                                m += 1;
                            }
                        }
                        while (m < this.RawDataMetaChunks[x].SubMeshInfo[y].IndiceStart + this.RawDataMetaChunks[x].SubMeshInfo[y].IndiceCount - 2);

                    }
                    else
                    {

                        Array.Copy(this.RawDataMetaChunks[x].Indices, this.RawDataMetaChunks[x].SubMeshInfo[y].IndiceStart, shite, 0, this.RawDataMetaChunks[x].SubMeshInfo[y].IndiceCount);
                        s = this.RawDataMetaChunks[x].SubMeshInfo[y].IndiceCount;//this.RawDataMetaChunks[x].IndiceCount;
                    }

                    for (int xx = 0; xx < s; xx += 3)
                    {
                        string temps = "f " + (shite[xx] + 1) + "/" + (shite[xx] + 1) + "/" + (shite[xx] + 1)
                                    + " " + (shite[xx + 1] + 1) + "/" + (shite[xx + 1] + 1) + "/" + (shite[xx + 1] + 1)
                                    + " " + (shite[xx + 2] + 1) + "/" + (shite[xx + 2] + 1) + "/" + (shite[xx + 2] + 1);
                        SW.WriteLine(temps);
                    }
                    SW.WriteLine("# " + (s / 3) + " elements");

                }
                */
                SW.Close();
                FS.Close();
            }

            SW2.Close();
            FS2.Close();
        }

        /// <summary>
        /// The extract meshes to x.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <remarks></remarks>
        public void ExtractMeshesToX(string path)
        {
            if (path[path.Length - 1] != '\\')
            {
                path += "\\";
            }

            string texturepath = path + "Textures\\";
            Directory.CreateDirectory(texturepath);

            List<string> materialnames = new List<string>();
            Panel f = new Panel();
            Renderer r = new Renderer();
            r.CreateDevice(f);

            for (int x = 0; x < this.Shaders.Shader.Length; x++)
            {
                this.Shaders.Shader[x].MakeTextures(ref r.device);
                string[] namesplit = this.Shaders.Shader[x].shaderName.Split('\\');
                string temps = namesplit[namesplit.Length - 1];
                temps = temps.Replace(" ", "_");
                temps = temps.Replace("1", "One");
                temps = temps.Replace("2", "Two");
                temps = temps.Replace("3", "Three");
                temps = temps.Replace("4", "Four");
                temps = temps.Replace("5", "Five");
                temps = temps.Replace("6", "Six");
                temps = temps.Replace("7", "Seven");
                temps = temps.Replace("8", "Eight");
                temps = temps.Replace("9", "Nine");
                temps = temps.Replace("0", "Zero");

                // if (materialnames.IndexOf(temps)==-1)
                materialnames.Add(temps);
                TextureLoader.Save(
                    texturepath + temps + ".dds", ImageFileFormat.Dds, this.Shaders.Shader[x].MainTexture);
            }

            for (int x = 0; x < this.RawDataMetaChunks.Length; x++)
            {
                FileStream FS = new FileStream(path + this.name + "[" + x + "].X", FileMode.Create);
                StreamWriter SW = new StreamWriter(FS);

                

                SW.WriteLine("xof 0303txt 0032");
                SW.WriteLine(string.Empty);
                SW.WriteLine("Header {");
                SW.WriteLine("1;");
                SW.WriteLine("0;");
                SW.WriteLine("1;");
                SW.WriteLine("}");

                

                SW.WriteLine(string.Empty); // blankline

                #region Write_Materials

                for (int y = 0; y < materialnames.Count; y++)
                {
                    SW.WriteLine("Material " + materialnames[y] + "{");
                    SW.WriteLine("1.000000; 1.000000; 1.000000; 1.000000;;"); // R = 1.0, G = 0.0, B = 0.0
                    SW.WriteLine("0.000000;");
                    SW.WriteLine("0.000000;0.000000;0.000000;;");
                    SW.WriteLine("0.000000;0.000000;0.000000;;");
                    SW.WriteLine("TextureFilename {");
                    string tempmloc = "\"Textures\\" + materialnames[y] + ".dds\";";
                    tempmloc = tempmloc.Replace("\\", "\\\\");

                    SW.WriteLine(tempmloc);
                    SW.WriteLine("}");
                    SW.WriteLine("}");
                }

                #endregion

                SW.WriteLine(string.Empty); // blankline

                #region Write_Frame_Heirarchy

                // Write_Frame_Header("Scene_Root", Matrix.Identity, ref SW);
                if (this.Frames != null)
                {
                    WriteRecursiveFrameHeirarchy(this.Frames.Frame[0], ref SW);
                }

                #endregion

                #region Write_Mesh

                ////scene root matrix
                Matrix tempm = Matrix.Identity;
                Write_Frame_Header("Scene_Root", tempm, ref SW);
                SW.WriteLine(string.Empty); // blankline
                Write_Frame_Header("Root", tempm, ref SW);

                #region Write_Mesh_Header

                SW.WriteLine("Mesh TestMesh {");
                SW.WriteLine(this.RawDataMetaChunks[x].VerticeCount + "; // vertices count");

                #endregion

                #region Write_Vertices

                for (int y = 0; y < this.RawDataMetaChunks[x].VerticeCount; y++)
                {
                    string temps = this.RawDataMetaChunks[x].Vertices[y].X + ";" +
                                   this.RawDataMetaChunks[x].Vertices[y].Y + ";" +
                                   this.RawDataMetaChunks[x].Vertices[y].Z;
                    temps += y != this.RawDataMetaChunks[x].VerticeCount - 1 ? ";," : ";;";
                    SW.WriteLine(temps);
                }

                #endregion

                SW.WriteLine(string.Empty); // blankline

                #region Face_Fields

                int[] faceCounts = new int[this.RawDataMetaChunks[x].SubMeshInfo.Length];
                short[] tempIndices = new short[100000];
                int totalindicecount = 0;
                int totalfacecount = 0;

                #endregion

                #region Process Faces

                for (int y = 0; y < this.RawDataMetaChunks[x].SubMeshInfo.Length; y++)
                {
                    int s = 0;

                    // check if indices are triangle list or strip
                    if (this.RawDataMetaChunks[x].FaceCount * 3 != this.RawDataMetaChunks[x].IndiceCount)
                    {
                        #region Decompress_Triangle_Strips

                        int m = this.RawDataMetaChunks[x].SubMeshInfo[y].IndiceStart;

                        bool dir = false;
                        short tempx;
                        short tempy;
                        short tempz;

                        do
                        {
                            tempx = this.RawDataMetaChunks[x].Indices[m];
                            tempy = this.RawDataMetaChunks[x].Indices[m + 1];
                            tempz = this.RawDataMetaChunks[x].Indices[m + 2];

                            if (tempx != tempy && tempx != tempz && tempy != tempz)
                            {
                                if (dir == false)
                                {
                                    tempIndices[totalindicecount + s] = tempx;
                                    tempIndices[totalindicecount + s + 1] = tempy;
                                    tempIndices[totalindicecount + s + 2] = tempz;
                                    s += 3;

                                    dir = true;
                                }
                                else
                                {
                                    tempIndices[totalindicecount + s] = tempx;
                                    tempIndices[totalindicecount + s + 1] = tempz;
                                    tempIndices[totalindicecount + s + 2] = tempy;
                                    s += 3;
                                    dir = false;
                                }

                                m += 1;
                            }
                            else
                            {
                                if (dir)
                                {
                                    dir = false;
                                }
                                else
                                {
                                    dir = true;
                                }

                                m += 1;
                            }
                        }
                        while (m <
                               this.RawDataMetaChunks[x].SubMeshInfo[y].IndiceStart +
                               this.RawDataMetaChunks[x].SubMeshInfo[y].IndiceCount - 2);

                        #endregion
                    }
                    else
                    {
                        Array.Copy(
                            this.RawDataMetaChunks[x].Indices, 
                            this.RawDataMetaChunks[x].SubMeshInfo[y].IndiceStart, 
                            tempIndices, 
                            totalindicecount, 
                            this.RawDataMetaChunks[x].SubMeshInfo[y].IndiceCount);
                        s = this.RawDataMetaChunks[x].SubMeshInfo[y].IndiceCount;
                    }

                    faceCounts[y] = s / 3;
                    totalindicecount += s;
                }

                totalfacecount = totalindicecount / 3;

                #endregion

                #region WriteFaces

                SW.WriteLine(totalfacecount + "; // face count");
                for (int xx = 0; xx < totalindicecount; xx += 3)
                {
                    string temps = "3;" + tempIndices[xx] + "," + tempIndices[xx + 1] + "," + tempIndices[xx + 2];
                    temps += xx != totalindicecount - 3 ? ";," : ";;";
                    SW.WriteLine(temps);
                }

                #endregion

                SW.WriteLine(string.Empty); // blankline

                #region MeshMaterialList

                #region MeshMaterialList_Header

                SW.WriteLine("MeshMaterialList {");
                SW.WriteLine(this.RawDataMetaChunks[x].SubMeshInfo.Length + ";"); // Amount of Textures
                SW.WriteLine(totalfacecount + ";"); // Total Face Count

                #endregion

                #region WriteFaceTextures

                for (int y = 0; y < this.RawDataMetaChunks[x].SubMeshInfo.Length; y++)
                {
                    for (int xx = 0; xx < faceCounts[y]; xx ++)
                    {
                        string temps = y + ";";

                        // temps+=(y==this.RawDataMetaChunks[x].SubMeshInfo.Length-1&&xx==faceCounts[y]-1)?";:":";,";
                        SW.WriteLine(temps);
                    }
                }

                #endregion

                #region WriteTextureNames

                for (int y = 0; y < this.RawDataMetaChunks[x].SubMeshInfo.Length; y++)
                {
                    SW.WriteLine("{" + materialnames[this.RawDataMetaChunks[x].SubMeshInfo[y].ShaderNumber] + "}");
                }

                #endregion

                SW.WriteLine("}");

                #endregion

                SW.WriteLine(string.Empty); // blankline

                #region Write_Normals

                #region Write_Normals_Header

                SW.WriteLine("MeshNormals {");
                SW.WriteLine(this.RawDataMetaChunks[x].VerticeCount + "; // normal count");

                #endregion

                #region OutputNormals

                for (int y = 0; y < this.RawDataMetaChunks[x].VerticeCount; y++)
                {
                    NumberFormatInfo ni = new NumberFormatInfo();
                    ni.NumberDecimalDigits = 7;
                    string xs = this.RawDataMetaChunks[x].Normals[y].X.ToString("f", ni);
                    string ys = this.RawDataMetaChunks[x].Normals[y].Y.ToString("f", ni);
                    string zs = this.RawDataMetaChunks[x].Normals[y].Z.ToString("f", ni);
                    string temps = xs + ";" + ys + ";" + zs;

                    temps += y != this.RawDataMetaChunks[x].VerticeCount - 1 ? ";," : ";;";
                    SW.WriteLine(temps);
                }

                #endregion

                SW.WriteLine(string.Empty); // blankline

                #region WriteRedundantNormalFaces

                SW.WriteLine(totalfacecount + "; // face count");
                for (int xx = 0; xx < totalindicecount; xx += 3)
                {
                    string temps = "3;" + tempIndices[xx] + "," + tempIndices[xx + 1] + "," + tempIndices[xx + 2];
                    temps += xx != totalindicecount - 3 ? ";," : ";;";
                    SW.WriteLine(temps);
                }

                #endregion

                SW.WriteLine("}");

                #endregion

                #region Write_UVs

                SW.WriteLine("MeshTextureCoords {");
                SW.WriteLine(this.RawDataMetaChunks[x].VerticeCount + "; // uv count");
                for (int y = 0; y < this.RawDataMetaChunks[x].VerticeCount; y++)
                {
                    string temps = this.RawDataMetaChunks[x].UVs[y].X + ";" + this.RawDataMetaChunks[x].UVs[y].Y;
                    temps += y != this.RawDataMetaChunks[x].VerticeCount - 1 ? ";," : ";;";
                    SW.WriteLine(temps);
                }

                SW.WriteLine("}");

                #endregion

                #region Write_Bones

                #region Skin_Mesh_Header

                SW.WriteLine("XSkinMeshHeader {");
                SW.WriteLine("2;");
                SW.WriteLine("4;");
                SW.WriteLine(this.RawDataMetaChunks[x].BoneMap.Count + ";");
                SW.WriteLine("}");

                #endregion

                #region Skin_Weights

                for (int y = 0; y < this.RawDataMetaChunks[x].BoneMap.Count; y++)
                {
                    SW.WriteLine("SkinWeights {");
                    SW.WriteLine("\"" + this.Frames.Frame[this.RawDataMetaChunks[x].BoneMap[y]].Name + "\";");
                    List<int> tempintarray = new List<int>();
                    for (int z = 0; z < this.RawDataMetaChunks[x].VerticeBones.Count; z++)
                    {
                        if (this.RawDataMetaChunks[x].VerticeBones[z].BoneIndex[0] == y)
                        {
                            tempintarray.Add(z);
                        }
                    }

                    SW.WriteLine(tempintarray.Count + ";");
                    for (int z = 0; z < tempintarray.Count; z++)
                    {
                        SW.WriteLine(tempintarray[z] + ((z == tempintarray.Count - 1) ? ";" : ","));
                    }

                    for (int z = 0; z < tempintarray.Count; z++)
                    {
                        SW.WriteLine(
                            this.RawDataMetaChunks[x].VerticeBones[y].Weight[0] +
                            ((z == tempintarray.Count - 1) ? ";" : ","));
                    }

                    Matrix mm;

                    // if (y == 0)
                    // {
                    // mm = this.Frames.Frame[this.RawDataMetaChunks[x].BoneMap[y]].matrix;
                    // mm.Translate(-0.1f, -0.1f, 0);
                    // mm = mm - this.Frames.Frame[this.RawDataMetaChunks[x].BoneMap[y]].matrix;
                    // }
                    // else
                    // {
                    mm = Matrix.Identity;

                    // }
                    WriteMatrix(mm, ref SW);
                    SW.WriteLine("}");
                }

                #endregion

                #endregion

                SW.WriteLine("}");
                SW.WriteLine("}");

                SW.WriteLine("}");

                // SW.WriteLine("}");
                #endregion

                SW.Close();
                FS.Close();
            }
        }

        /// <summary>
        /// The h 2 parsed model.
        /// </summary>
        /// <param name="meta">The meta.</param>
        /// <remarks></remarks>
        public void H2ParsedModel(ref Meta meta)
        {
            if (meta.MS.Length == 0)
            {
                return;
            }

            string[] temps = meta.name.Split('\\');
            name = temps[temps.Length - 1];
            BoundingBox = new BoundingBoxContainer(ref meta);

            BinaryReader BR = new BinaryReader(meta.MS);
            BR.BaseStream.Position = 36;
            int tempc = BR.ReadInt32();
            int tempr = BR.ReadInt32() - meta.magic - meta.offset;
            RawDataMetaChunks = new RawDataMetaChunk[tempc];
            for (int x = 0; x < tempc; x++)
            {
                RawDataMetaChunks[x] = new RawDataMetaChunk(tempr + (x * 92), x, BoundingBox, ref meta);
            }

            Shaders = new ShaderContainer(ref meta);

            LOD = new LODInfo(ref meta, ref RawDataMetaChunks);

            int temphlmt = -1;
            for (int x = 0; x < meta.Map.IndexHeader.metaCount; x++)
            {
                if ("hlmt" == meta.Map.MetaInfo.TagType[x] && meta.Map.FileNames.Name[x] == meta.name)
                {
                    temphlmt = x;
                    break;
                }
            }

            if (temphlmt != -1)
            {
                hlmt = new hlmtContainer(temphlmt, meta.Map);
                PermutationString = hlmt.Permutations.Name;
            }

            // ** Length of Distance LOD
            Display = DisplayedInfo.FindDisplayedPieces(4, this);

            Frames = new FrameHierarchy();
            Frames.GetFramesFromHalo2Model(ref meta);
        }

        /// <summary>
        /// The inject model.
        /// </summary>
        /// <param name="FilePath">The file path.</param>
        /// <param name="meta">The meta.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public virtual Meta InjectModel(string FilePath, Meta meta)
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
                    int u = CompressNormal(this.RawDataMetaChunks[x].Normals[y]);
                    int v = 1;
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

            Meta m = MetaBuilder.BuildMeta(ms, map);
            m.raw = meta.raw;

            

            return m;
        }

        /// <summary>
        /// The load from obj.
        /// </summary>
        /// <param name="FilePath">The file path.</param>
        /// <remarks></remarks>
        public virtual void LoadFromOBJ(string FilePath)
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

                            // Materials.Add(Materials.Count, tempstrings[1]);
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

        /// <summary>
        /// The write obj.
        /// </summary>
        /// <param name="SW">The sw.</param>
        /// <param name="mtllib">The mtllib.</param>
        /// <param name="chunk">The chunk.</param>
        /// <param name="names">The names.</param>
        /// <param name="pass">The pass.</param>
        /// <param name="startFace">The start face.</param>
        /// <remarks></remarks>
        public void writeOBJ(
            StreamWriter SW, string mtllib, RawDataMetaChunk chunk, List<string> names, ref int pass, ref int startFace)
        {
            SW.WriteLine("mtllib " + mtllib);

            for (int y = 0; y < chunk.VerticeCount; y++)
            {
                string temps = "v " + chunk.Vertices[y].X + " " + chunk.Vertices[y].Y + " " + chunk.Vertices[y].Z;
                SW.WriteLine(temps);
            }

            SW.WriteLine("# " + chunk.Vertices.Count + " vertices");

            for (int y = 0; y < chunk.VerticeCount; y++)
            {
                string temps = "vt " + chunk.UVs[y].X + " " + chunk.UVs[y].Y;

                SW.WriteLine(temps);
            }

            SW.WriteLine("# " + chunk.Vertices.Count + " texture vertices");
            for (int y = 0; y < chunk.VerticeCount; y++)
            {
                string temps = "vn " + chunk.Normals[y].X + " " + chunk.Normals[y].Y + " " + chunk.Normals[y].Z;
                SW.WriteLine(temps);
            }

            SW.WriteLine("# " + chunk.Vertices.Count + " normals");

            for (int y = 0; y < chunk.SubMeshInfo.Length; y++)
            {
                SW.WriteLine("g " + pass + "." + y);
                try
                {
                    SW.WriteLine("usemtl  " + names[chunk.SubMeshInfo[y].ShaderNumber]);
                }
                catch
                {
                }

                short[] shite = new short[100000];
                int s = 0;
                if (chunk.FaceCount * 3 != chunk.IndiceCount)
                {
                    int m = chunk.SubMeshInfo[y].IndiceStart;

                    bool dir = false;
                    short tempx;
                    short tempy;
                    short tempz;

                    do
                    {
                        // if (mode.EndOfIndices[x][j]>m+2){break;}
                        tempx = chunk.Indices[m];
                        tempy = chunk.Indices[m + 1];
                        tempz = chunk.Indices[m + 2];

                        if (tempx != tempy && tempx != tempz && tempy != tempz)
                        {
                            if (dir == false)
                            {
                                shite[s] = tempx;
                                shite[s + 1] = tempy;
                                shite[s + 2] = tempz;
                                s += 3;

                                dir = true;
                            }
                            else
                            {
                                shite[s] = tempx;
                                shite[s + 1] = tempz;
                                shite[s + 2] = tempy;
                                s += 3;
                                dir = false;
                            }

                            m += 1;
                        }
                        else
                        {
                            if (dir)
                            {
                                dir = false;
                            }
                            else
                            {
                                dir = true;
                            }

                            m += 1;
                        }
                    }
                    while (m < chunk.SubMeshInfo[y].IndiceStart + chunk.SubMeshInfo[y].IndiceCount - 2);
                }
                else
                {
                    Array.Copy(
                        chunk.Indices, chunk.SubMeshInfo[y].IndiceStart, shite, 0, chunk.SubMeshInfo[y].IndiceCount);
                    s = chunk.SubMeshInfo[y].IndiceCount; // chunk.IndiceCount;
                }

                for (int xx = 0; xx < s; xx += 3)
                {
                    string temps = "f " + (shite[xx] + 1 + startFace) + "/" + (shite[xx] + 1 + startFace) + "/" +
                                   (shite[xx] + 1 + startFace) + " " + (shite[xx + 1] + 1 + startFace) + "/" +
                                   (shite[xx + 1] + 1 + startFace) + "/" + (shite[xx + 1] + 1 + startFace) + " " +
                                   (shite[xx + 2] + 1 + startFace) + "/" + (shite[xx + 2] + 1 + startFace) + "/" +
                                   (shite[xx + 2] + 1 + startFace);
                    SW.WriteLine(temps);
                }

                SW.WriteLine("# " + (s / 3) + " elements");
            }

            pass++;
            startFace += chunk.Vertices.Count;
        }

        #endregion

        #region Implemented Interfaces

        #region IDisposable

        /// <summary>
        /// The dispose.
        /// </summary>
        /// <remarks></remarks>
        public void Dispose()
        {
            this.BoundingBox = null;
            this.Display = null;
            this.hlmt = null;
            this.LOD = null;
            this.RawDataMetaChunks = null;
            for (int i = 0; i < this.Shaders.Shader.Length; i++)
            {
                this.Shaders.Shader[i].Bitmaps.Clear();
                this.Shaders.Shader[i].BitmapNames.Clear();
            }

            this.Shaders = null;
            GC.SuppressFinalize(this);
        }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// The write matrix.
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        /// <param name="SW">The sw.</param>
        /// <remarks></remarks>
        private void WriteMatrix(Matrix matrix, ref StreamWriter SW)
        {
            NumberFormatInfo ni = new NumberFormatInfo();
            ni.NumberDecimalDigits = 7;
            SW.Write(matrix.M11.ToString("f", ni) + ",");
            SW.Write(matrix.M12.ToString("f", ni) + ",");
            SW.Write(matrix.M13.ToString("f", ni) + ",");
            SW.Write(matrix.M14.ToString("f", ni) + ",");
            SW.Write(matrix.M21.ToString("f", ni) + ",");
            SW.Write(matrix.M22.ToString("f", ni) + ",");
            SW.Write(matrix.M23.ToString("f", ni) + ",");
            SW.Write(matrix.M24.ToString("f", ni) + ",");
            SW.Write(matrix.M31.ToString("f", ni) + ",");
            SW.Write(matrix.M32.ToString("f", ni) + ",");
            SW.Write(matrix.M33.ToString("f", ni) + ",");
            SW.Write(matrix.M34.ToString("f", ni) + ",");
            SW.Write(matrix.M41.ToString("f", ni) + ",");
            SW.Write(matrix.M42.ToString("f", ni) + ",");
            SW.Write(matrix.M43.ToString("f", ni) + ",");
            SW.Write(matrix.M44.ToString("f", ni) + ";;" + SW.NewLine);
        }

        /// <summary>
        /// The write recursive frame heirarchy.
        /// </summary>
        /// <param name="frame">The frame.</param>
        /// <param name="SW">The sw.</param>
        /// <remarks></remarks>
        private void WriteRecursiveFrameHeirarchy(FrameInfo frame, ref StreamWriter SW)
        {
            SW.WriteLine(string.Empty);
            Write_Frame_Header(frame.Name, frame.matrix, ref SW);

            if (frame.Child != -1)
            {
                WriteRecursiveFrameHeirarchy(Frames.Frame[frame.Child], ref SW);
            }

            SW.WriteLine(string.Empty);
            SW.WriteLine("}");
            if (frame.Sibling != -1)
            {
                WriteRecursiveFrameHeirarchy(Frames.Frame[frame.Sibling], ref SW);
            }
        }

        /// <summary>
        /// The write_ frame_ header.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="m">The m.</param>
        /// <param name="SW">The sw.</param>
        /// <remarks></remarks>
        private void Write_Frame_Header(string name, Matrix m, ref StreamWriter SW)
        {
            SW.WriteLine("Frame " + name + " {");
            SW.WriteLine(string.Empty);
            SW.WriteLine("FrameTransformMatrix {");
            SW.WriteLine(string.Empty);
            WriteMatrix(m, ref SW);
            SW.WriteLine(string.Empty);
            SW.WriteLine("}");
        }

        #endregion

        /// <summary>
        /// The bounding box container.
        /// </summary>
        /// <remarks></remarks>
        public class BoundingBoxContainer
        {
            #region Constants and Fields

            /// <summary>
            /// The max u.
            /// </summary>
            public float MaxU;

            /// <summary>
            /// The max v.
            /// </summary>
            public float MaxV;

            /// <summary>
            /// The max x.
            /// </summary>
            public float MaxX;

            /// <summary>
            /// The max y.
            /// </summary>
            public float MaxY;

            /// <summary>
            /// The max z.
            /// </summary>
            public float MaxZ;

            /// <summary>
            /// The min u.
            /// </summary>
            public float MinU;

            /// <summary>
            /// The min v.
            /// </summary>
            public float MinV;

            /// <summary>
            /// The min x.
            /// </summary>
            public float MinX;

            /// <summary>
            /// The min y.
            /// </summary>
            public float MinY;

            /// <summary>
            /// The min z.
            /// </summary>
            public float MinZ;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            /// Initializes a new instance of the <see cref="BoundingBoxContainer"/> class.
            /// </summary>
            /// <remarks></remarks>
            public BoundingBoxContainer()
            {
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="BoundingBoxContainer"/> class.
            /// </summary>
            /// <param name="meta">The meta.</param>
            /// <remarks></remarks>
            public BoundingBoxContainer(ref Meta meta)
            {
                BinaryReader BR = new BinaryReader(meta.MS);
                BR.BaseStream.Position = 20;
                int tempc = BR.ReadInt32();
                int tempr = BR.ReadInt32() - meta.magic - meta.offset;
                BR.BaseStream.Position = tempr;
                MinX = BR.ReadSingle();
                MaxX = BR.ReadSingle();
                MinY = BR.ReadSingle();
                MaxY = BR.ReadSingle();
                MinZ = BR.ReadSingle();
                MaxZ = BR.ReadSingle();
                MinU = BR.ReadSingle();
                MaxU = BR.ReadSingle();
                MinV = BR.ReadSingle();
                MaxV = BR.ReadSingle();
            }

            #endregion
        }

        /// <summary>
        /// The displayed info.
        /// </summary>
        /// <remarks></remarks>
        public class DisplayedInfo
        {
            #region Constants and Fields

            /// <summary>
            /// The chunk.
            /// </summary>
            public List<int> Chunk = new List<int>();

            /// <summary>
            /// The shader index.
            /// </summary>
            public int[] ShaderIndex;

            /// <summary>
            /// The index buffer.
            /// </summary>
            public IndexBuffer[] indexBuffer;

            /// <summary>
            /// The meshes.
            /// </summary>
            public Mesh[] meshes;

            /// <summary>
            /// The vertex buffer.
            /// </summary>
            public VertexBuffer[] vertexBuffer;

            #endregion

            #region Public Methods

            /// <summary>
            /// The draw.
            /// </summary>
            /// <param name="device">The device.</param>
            /// <param name="pm">The pm.</param>
            /// <remarks></remarks>
            public static void Draw(ref Device device, ParsedModel pm)
            {
                for (int i = 0; i < 5; i++)
                {
                    device.SetTexture(i, null);
                }

                for (int x = 0; x < pm.Display.Chunk.Count; x++)
                {
                    int rawindex = pm.Display.Chunk[x];

                    device.SetStreamSource(0, pm.Display.vertexBuffer[rawindex], 0);

                    // device.VertexFormat = HaloVertex.FVF;
                    device.VertexFormat = pm.Display.vertexBuffer[rawindex].Description.VertexFormat;

                    // This makes the sky look a little more correct...
                    // device.VertexFormat = VertexFormats.Texture0 | VertexFormats.PositionNormal | VertexFormats.Specular;
                    device.Indices = pm.Display.indexBuffer[rawindex];
                    for (int xx = 0; xx < pm.RawDataMetaChunks[rawindex].SubMeshInfo.Length; xx++)
                    {
                        device.RenderState.AlphaBlendEnable = false;
                        device.RenderState.SpecularEnable = false;

                        int tempshade = pm.RawDataMetaChunks[rawindex].SubMeshInfo[xx].ShaderNumber;
                        ShaderInfo shad = pm.Shaders.Shader[tempshade];
                        Renderer.SetAlphaBlending(shad.Alpha, ref device);

                        device.SetTexture(0, shad.MainTexture);
                        if (shad.Alpha == ShaderInfo.AlphaType.None)
                        {
                            device.TextureState[0].ColorOperation = TextureOperation.SelectArg1;
                            device.TextureState[0].ColorArgument1 = TextureArgument.TextureColor;
                            if (shad.BitmapNames.Count > 0 && shad.BitmapNames[0].ToLower().Contains("specular"))
                            {
                                device.RenderState.SpecularEnable = true;
                                device.RenderState.SpecularMaterialSource = ColorSource.Material;
                                device.TextureState[0].ColorOperation = TextureOperation.Modulate;
                                device.TextureState[0].ColorArgument1 = TextureArgument.TextureColor;
                                device.TextureState[0].ColorArgument2 = TextureArgument.Specular;
                            }
                        }
                        else
                        {
                            device.RenderState.AlphaBlendEnable = true;
                            device.RenderState.AlphaTestEnable = true;
                            device.RenderState.ReferenceAlpha = 180;
                            device.RenderState.AlphaFunction = Compare.Greater;

                            device.RenderState.SourceBlend = Blend.SourceAlpha;
                            device.RenderState.DestinationBlend = Blend.One; // was Blend.InvSourceAlpha;

                            device.TextureState[0].ColorOperation = TextureOperation.SelectArg1;
                            device.TextureState[0].ColorArgument1 = TextureArgument.TextureColor;
                            device.TextureState[0].ColorArgument2 = TextureArgument.Diffuse;

                            /*
                            device.TextureState[0].AlphaOperation = TextureOperation.Modulate; // BlendTextureAlpha;
                            device.TextureState[0].AlphaArgument1 = TextureArgument.TextureColor;
                            device.TextureState[0].AlphaArgument2 = TextureArgument.Current;
                            */
                        }

                        if (shad.BitmapTextures != null)
                        {
                            for (int xxx = 1; xxx <= shad.BitmapTextures.Length; xxx++)
                            {
                                device.SetTexture(xxx, shad.BitmapTextures[xxx - 1]);
                                if (shad.Alpha == ShaderInfo.AlphaType.None)
                                {
                                    // device.RenderState.AlphaBlendEnable = false;
                                    device.TextureState[xxx].AlphaOperation = TextureOperation.Disable;
                                    device.TextureState[xxx].AlphaArgument1 = TextureArgument.TextureColor;
                                    device.TextureState[xxx].AlphaArgument2 = TextureArgument.Current;

                                    device.TextureState[xxx].ColorOperation = TextureOperation.Disable;
                                    device.TextureState[xxx].ColorArgument1 = TextureArgument.TextureColor;
                                    device.TextureState[xxx].ColorArgument2 = TextureArgument.Current;
                                }
                                else
                                {
                                    // device.RenderState.AlphaBlendEnable = true;
                                    device.RenderState.SourceBlend = Blend.SourceColor;
                                    device.RenderState.DestinationBlend = Blend.InvSourceColor;

                                    /*
                                    device.TextureState[xxx].ColorOperation = TextureOperation.SelectArg1;
                                    device.TextureState[xxx].ColorArgument1 = TextureArgument.TextureColor;
                                    device.TextureState[xxx].ColorArgument2 = TextureArgument.Diffuse;
                                    */
                                    device.TextureState[xxx].AlphaOperation = TextureOperation.Modulate;
                                    device.TextureState[xxx].AlphaArgument1 = TextureArgument.TextureColor;
                                    device.TextureState[xxx].AlphaArgument2 = TextureArgument.Current;
                                }
                            }
                        }

                        PrimitiveType pt;
                        if (pm.RawDataMetaChunks[rawindex].FaceCount * 3 != pm.RawDataMetaChunks[rawindex].IndiceCount)
                        {
                            pt = PrimitiveType.TriangleStrip;

                            // The -2 is because it's a Triangle Strip, which has three vertices for trianlge 1 and one for
                            // each following traingle.
                            // So the total # of triangles are IndiceCount-2 (for the first two points)
                            // Thanks to Prey for pointing this out
                            try
                            {
                                device.DrawIndexedPrimitives(
                                    pt, 
                                    0, 
                                    0, 
                                    pm.RawDataMetaChunks[rawindex].VerticeCount, 
                                    pm.RawDataMetaChunks[rawindex].SubMeshInfo[xx].IndiceStart, 
                                    pm.RawDataMetaChunks[rawindex].SubMeshInfo[xx].IndiceCount - 2);
                            }
                            catch
                            {
                            }
                        }
                        else
                        {
                            pt = PrimitiveType.TriangleList;
                            device.DrawIndexedPrimitives(
                                pt, 
                                0, 
                                0, 
                                pm.RawDataMetaChunks[rawindex].VerticeCount, 
                                pm.RawDataMetaChunks[rawindex].SubMeshInfo[xx].IndiceStart, 
                                pm.RawDataMetaChunks[rawindex].SubMeshInfo[xx].IndiceCount / 3);
                        }
                    }
                }
            }

            /// <summary>
            /// The draw meshes.
            /// </summary>
            /// <param name="device">The device.</param>
            /// <param name="pm">The pm.</param>
            /// <remarks></remarks>
            public static void DrawMeshes(ref Device device, ParsedModel pm)
            {
                for (int x = 0; x < pm.Display.Chunk.Count; x++)
                {
                    int rawindex = pm.Display.Chunk[x];
                    VertexBuffer vb = pm.Display.meshes[x].VertexBuffer;
                    IndexBuffer ib = pm.Display.meshes[x].IndexBuffer;
                    device.SetStreamSource(0, vb, 0);
                    device.VertexFormat = HaloVertex.FVF;
                    device.Indices = ib;
                    for (int xx = 0; xx < pm.RawDataMetaChunks[rawindex].SubMeshInfo.Length; xx++)
                    {
                        int tempshade = pm.RawDataMetaChunks[rawindex].SubMeshInfo[xx].ShaderNumber;

                        Renderer.SetAlphaBlending(pm.Shaders.Shader[tempshade].Alpha, ref device);
                        device.SetTexture(0, pm.Shaders.Shader[tempshade].MainTexture);
                        device.TextureState[0].ColorOperation = TextureOperation.Modulate;
                        device.TextureState[0].ColorArgument1 = TextureArgument.TextureColor;
                        device.TextureState[0].ColorArgument2 = TextureArgument.Current;
                        device.RenderState.FillMode = FillMode.Solid;
                        pm.Display.meshes[x].DrawSubset(xx);
                    }
                }
            }

            /// <summary>
            /// The find displayed pieces.
            /// </summary>
            /// <param name="lodlevel">The lodlevel.</param>
            /// <param name="pm">The pm.</param>
            /// <returns></returns>
            /// <remarks></remarks>
            public static DisplayedInfo FindDisplayedPieces(int lodlevel, ParsedModel pm)
            {
                DisplayedInfo di = new DisplayedInfo();
                if (pm.hlmt == null || pm.hlmt.Permutations.Piece.Length == 0)
                {
                    for (int x = 0; x < pm.LOD.Piece.Length; x++)
                    {
                        for (int y = 0; y < pm.LOD.Piece[x].Permutation.Length; y++)
                        {
                            if (pm.LOD.Piece[x].Permutation[y].name == pm.LOD.PermutationStrings[0])
                            {
                                if (di.Chunk.IndexOf(pm.LOD.Piece[x].Permutation[y].pieceNumber[lodlevel]) == -1)
                                {
                                    di.Chunk.Add(pm.LOD.Piece[x].Permutation[y].pieceNumber[lodlevel]);
                                }
                            }
                        }
                    }

                    goto shadeshit;
                }

                int tempid = pm.hlmt.FindPermutation(pm.PermutationString);
                LODInfo.LODPieceInfo.LODPermutationInfo lodinfo = new LODInfo.LODPieceInfo.LODPermutationInfo();

                for (int x = 0; x < pm.hlmt.Permutations.Piece.Length; x++)
                {
                    string name1 = pm.hlmt.Permutations.Piece[x].PieceName;

                    // if (pm.hlmt.Permutations[tempid].Piece[x].Permutation.Length
                    for (int xx = 0; xx < pm.hlmt.Permutations.Piece[x].Permutation.Length; xx++)
                    {
                        string name2 = pm.hlmt.Permutations.Piece[x].Permutation[xx].PermutationNameX;
                        lodinfo = pm.LOD.FindPermutationInfo(name1, name2);

                        if (di.Chunk.IndexOf(lodinfo.pieceNumber[lodlevel]) == -1)
                        {
                            di.Chunk.Add(lodinfo.pieceNumber[lodlevel]);
                        }
                    }
                }

                shadeshit:
                int[] tempshade = new int[200];
                int tempshadecount = 1;
                for (int x = 0; x < di.Chunk.Count; x++)
                {
                    for (int y = 0; y < pm.RawDataMetaChunks[di.Chunk[x]].SubMeshInfo.Length; y++)
                    {
                        int r = pm.RawDataMetaChunks[di.Chunk[x]].SubMeshInfo[y].ShaderNumber;
                        if (Array.IndexOf(tempshade, r) == -1)
                        {
                            tempshade[tempshadecount] = r;
                            tempshadecount++;
                        }
                    }
                }

                di.ShaderIndex = new int[tempshadecount];

                Array.Copy(tempshade, 0, di.ShaderIndex, 0, tempshadecount);
                return di;
            }

            /// <summary>
            /// The load direct x textures and buffers.
            /// </summary>
            /// <param name="device">The device.</param>
            /// <param name="pm">The pm.</param>
            /// <remarks></remarks>
            public static void LoadDirectXTexturesAndBuffers(ref Device device, ref ParsedModel pm)
            {
                if (pm == null)
                {
                    return;
                }

                CreateVertexBuffers(ref device, ref pm);
                CreateIndexBuffers(ref device, ref pm);
                LoadShaderTextures(ref device, ref pm);
                MakeMeshes(ref device, ref pm);
            }

            /// <summary>
            /// The make meshes.
            /// </summary>
            /// <param name="device">The device.</param>
            /// <param name="pm">The pm.</param>
            /// <remarks></remarks>
            public static void MakeMeshes(ref Device device, ref ParsedModel pm)
            {
                pm.Display.meshes = new Mesh[pm.Display.Chunk.Count];
                for (int t = 0; t < pm.Display.Chunk.Count; t++)
                {
                    int rawindex = pm.Display.Chunk[t];
                    pm.Display.meshes[t] = Renderer.MakeMesh(ref pm, rawindex, ref device);
                }
            }

            #endregion

            #region Methods

            /// <summary>
            /// The create index buffers.
            /// </summary>
            /// <param name="device">The device.</param>
            /// <param name="pm">The pm.</param>
            /// <remarks></remarks>
            private static void CreateIndexBuffers(ref Device device, ref ParsedModel pm)
            {
                // Pool pool=;
                // if (device.DeviceCaps..
                pm.Display.indexBuffer = new IndexBuffer[pm.RawDataMetaChunks.Length];
                for (int rawindex = 0; rawindex < pm.Display.Chunk.Count; rawindex++)
                {
                    int x = pm.Display.Chunk[rawindex];
                    pm.Display.indexBuffer[x] = new IndexBuffer(
                        typeof(short), pm.RawDataMetaChunks[x].IndiceCount, device, Usage.WriteOnly, Pool.Managed);
                    IndexBuffer ib = pm.Display.indexBuffer[x];
                    ib.SetData(pm.RawDataMetaChunks[x].Indices, 0, LockFlags.None);
                    ib.Unlock();
                }
            }

            /// <summary>
            /// The create vertex buffers.
            /// </summary>
            /// <param name="device">The device.</param>
            /// <param name="pm">The pm.</param>
            /// <remarks></remarks>
            private static void CreateVertexBuffers(ref Device device, ref ParsedModel pm)
            {
                pm.Display.vertexBuffer = new VertexBuffer[pm.RawDataMetaChunks.Length];
                for (int x = 0; x < pm.Display.Chunk.Count; x++)
                {
                    int rawindex = pm.Display.Chunk[x];

                    pm.Display.vertexBuffer[rawindex] = new VertexBuffer(
                        typeof(HaloVertex), 
                        pm.RawDataMetaChunks[rawindex].VerticeCount, 
                        device, 
                        Usage.WriteOnly, 
                        HaloVertex.FVF, 
                        Pool.Managed);
                    HaloVertex[] verts = (HaloVertex[])pm.Display.vertexBuffer[rawindex].Lock(0, 0);

                    // Lock the buffer (which will return our structs)
                    for (int i = 0; i < pm.RawDataMetaChunks[rawindex].VerticeCount; i++)
                    {
                        verts[i].Position = new Vector3(
                            pm.RawDataMetaChunks[rawindex].Vertices[i].X, 
                            pm.RawDataMetaChunks[rawindex].Vertices[i].Y, 
                            pm.RawDataMetaChunks[rawindex].Vertices[i].Z);

                        verts[i].Tu0 = pm.RawDataMetaChunks[rawindex].UVs[i].X;
                        verts[i].Tv0 = pm.RawDataMetaChunks[rawindex].UVs[i].Y;
                        verts[i].Normal = new Vector3(
                            pm.RawDataMetaChunks[rawindex].Vertices[i].X, 
                            pm.RawDataMetaChunks[rawindex].Vertices[i].Y, 
                            pm.RawDataMetaChunks[rawindex].Vertices[i].Z);

                        verts[i].specular = 1;
                        verts[i].diffuse = 1;
                    }

                    // Unlock (and copy) the data
                    pm.Display.vertexBuffer[rawindex].Unlock();
                }
            }

            /// <summary>
            /// The load shader textures.
            /// </summary>
            /// <param name="device">The device.</param>
            /// <param name="pm">The pm.</param>
            /// <remarks></remarks>
            private static void LoadShaderTextures(ref Device device, ref ParsedModel pm)
            {
                for (int x = 0; x < pm.Display.ShaderIndex.Length; x++)
                {
                    try
                    {
                        pm.Shaders.Shader[pm.Display.ShaderIndex[x]].MakeTextures(ref device);
                    }
                    catch
                    {
                    }
                }

                // MessageBox.Show("test");
            }

            #endregion
        }

        /// <summary>
        /// The lod info.
        /// </summary>
        /// <remarks></remarks>
        public class LODInfo
        {
            #region Constants and Fields

            /// <summary>
            /// The permutation strings.
            /// </summary>
            public List<string> PermutationStrings = new List<string>();

            /// <summary>
            /// The piece.
            /// </summary>
            public LODPieceInfo[] Piece;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            /// Initializes a new instance of the <see cref="LODInfo"/> class.
            /// </summary>
            /// <param name="meta">The meta.</param>
            /// <param name="rd">The rd.</param>
            /// <remarks></remarks>
            public LODInfo(ref Meta meta, ref RawDataMetaChunk[] rd)
            {
                BinaryReader BR = new BinaryReader(meta.MS);
                BR.BaseStream.Position = 28;
                int tempc = BR.ReadInt32();
                int tempr = BR.ReadInt32() - meta.magic - meta.offset;
                Piece = new LODPieceInfo[tempc];

                for (int x = 0; x < tempc; x++)
                {
                    Piece[x] = new LODPieceInfo();
                    BR.BaseStream.Position = tempr + (x * 16);
                    Piece[x].Name = meta.Map.Strings.Name[BR.ReadInt16()];
                    BR.BaseStream.Position = tempr + (x * 16) + 8;
                    int tempc2 = BR.ReadInt32();
                    int tempr2 = BR.ReadInt32() - meta.magic - meta.offset;
                    Piece[x].Permutation = new LODPieceInfo.LODPermutationInfo[tempc2];
                    for (int xx = 0; xx < tempc2; xx++)
                    {
                        BR.BaseStream.Position = tempr2 + (xx * 16);
                        Piece[x].Permutation[xx] = new LODPieceInfo.LODPermutationInfo();
                        Piece[x].Permutation[xx].name = meta.Map.Strings.Name[BR.ReadInt16()];
                        if (PermutationStrings.IndexOf(Piece[x].Permutation[xx].name) == -1)
                        {
                            PermutationStrings.Add(Piece[x].Permutation[xx].name);
                        }

                        BR.ReadInt16();
                        Piece[x].Permutation[xx].pieceNumber = new int[5];
                        for (int w = 0; w < 5; w++)
                        {
                            Piece[x].Permutation[xx].pieceNumber[w] = BR.ReadInt16();
                            rd[Piece[x].Permutation[xx].pieceNumber[w]].piecename = Piece[x].Name;
                            rd[Piece[x].Permutation[xx].pieceNumber[w]].permutation = Piece[x].Permutation[xx].name;
                            rd[Piece[x].Permutation[xx].pieceNumber[w]].lod = w;
                        }
                    }
                }
            }

            #endregion

            #region Public Methods

            /// <summary>
            /// The find permutation info.
            /// </summary>
            /// <param name="piecename">The piecename.</param>
            /// <param name="permutationname">The permutationname.</param>
            /// <returns></returns>
            /// <remarks></remarks>
            public LODPieceInfo.LODPermutationInfo FindPermutationInfo(string piecename, string permutationname)
            {
                LODPieceInfo.LODPermutationInfo temp = null;
                for (int x = 0; x < Piece.Length; x++)
                {
                    if (Piece[x].Name == piecename)
                    {
                        for (int xx = 0; xx < Piece[x].Permutation.Length; xx++)
                        {
                            if (Piece[x].Permutation[xx].name == permutationname)
                            {
                                return Piece[x].Permutation[xx];
                            }
                        }

                        temp = Piece[x].Permutation[0];
                    }
                }

                return temp;
            }

            #endregion

            /// <summary>
            /// The lod piece info.
            /// </summary>
            /// <remarks></remarks>
            public class LODPieceInfo
            {
                #region Constants and Fields

                /// <summary>
                /// The name.
                /// </summary>
                public string Name;

                /// <summary>
                /// The permutation.
                /// </summary>
                public LODPermutationInfo[] Permutation;

                #endregion

                /// <summary>
                /// The lod permutation info.
                /// </summary>
                /// <remarks></remarks>
                public class LODPermutationInfo
                {
                    #region Constants and Fields

                    /// <summary>
                    /// The name.
                    /// </summary>
                    public string name;

                    /// <summary>
                    /// The piece number.
                    /// </summary>
                    public int[] pieceNumber;

                    #endregion
                }
            }
        }

        /// <summary>
        /// The raw data meta chunk.
        /// </summary>
        /// <remarks></remarks>
        public class RawDataMetaChunk
        {
            #region Constants and Fields

            /// <summary>
            /// The binormals.
            /// </summary>
            public List<Vector3> Binormals = new List<Vector3>();

            /// <summary>
            /// The bone map.
            /// </summary>
            public List<int> BoneMap = new List<int>();

            /// <summary>
            /// The face count.
            /// </summary>
            public int FaceCount;

            /// <summary>
            /// The header size.
            /// </summary>
            public int HeaderSize;

            /// <summary>
            /// The indice count.
            /// </summary>
            public int IndiceCount;

            /// <summary>
            /// The indices.
            /// </summary>
            public short[] Indices;

            /// <summary>
            /// The normals.
            /// </summary>
            public List<Vector3> Normals = new List<Vector3>();

            /// <summary>
            /// The raw data chunk info.
            /// </summary>
            public RawDataOffsetChunk[] RawDataChunkInfo;

            /// <summary>
            /// The sub mesh info.
            /// </summary>
            public ModelSubMeshInfo[] SubMeshInfo;

            /// <summary>
            /// The tangents.
            /// </summary>
            public List<Vector3> Tangents = new List<Vector3>();

            /// <summary>
            /// The u vs.
            /// </summary>
            public List<Vector2> UVs = new List<Vector2>();

            /// <summary>
            /// The vertice bones.
            /// </summary>
            public List<BoneInfo> VerticeBones = new List<BoneInfo>();

            /// <summary>
            /// The vertice count.
            /// </summary>
            public int VerticeCount;

            /// <summary>
            /// The vertices.
            /// </summary>
            public List<Vector3> Vertices = new List<Vector3>();

            /// <summary>
            /// The lod.
            /// </summary>
            public int lod;

            /// <summary>
            /// The permutation.
            /// </summary>
            public string permutation;

            /// <summary>
            /// The piecename.
            /// </summary>
            public string piecename;

            /// <summary>
            /// The type.
            /// </summary>
            public byte type;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            /// Initializes a new instance of the <see cref="RawDataMetaChunk"/> class.
            /// </summary>
            /// <remarks></remarks>
            public RawDataMetaChunk()
            {
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="RawDataMetaChunk"/> class.
            /// </summary>
            /// <param name="offset">The offset.</param>
            /// <param name="chunknumber">The chunknumber.</param>
            /// <param name="bb">The bb.</param>
            /// <param name="meta">The meta.</param>
            /// <remarks></remarks>
            public RawDataMetaChunk(int offset, int chunknumber, BoundingBoxContainer bb, ref Meta meta)
            {
                BinaryReader BR = new BinaryReader(meta.MS);
                BR.BaseStream.Position = offset + 4;
                VerticeCount = BR.ReadUInt16();
                FaceCount = BR.ReadUInt16();
                BR.BaseStream.Position = offset + 20;
                type = BR.ReadByte();
                BR.BaseStream.Position = offset + 68;
                HeaderSize = meta.raw.rawChunks[chunknumber].size - BR.ReadInt32() - 4;
                int tempc = BR.ReadInt32();
                int tempr = BR.ReadInt32() - meta.magic - meta.offset;
                RawDataChunkInfo = new RawDataOffsetChunk[tempc];
                for (int x = 0; x < tempc; x++)
                {
                    RawDataChunkInfo[x] = new RawDataOffsetChunk();
                    BR.BaseStream.Position = tempr + (x * 16) + 6;
                    RawDataChunkInfo[x].ChunkSize = BR.ReadUInt16();
                    RawDataChunkInfo[x].Size = BR.ReadInt32();
                    if (RawDataChunkInfo[x].ChunkSize == 0)
                    {
                        RawDataChunkInfo[x].ChunkSize = RawDataChunkInfo[x].Size;
                    }

                    RawDataChunkInfo[x].Offset = BR.ReadInt32();
                    RawDataChunkInfo[x].ChunkCount = RawDataChunkInfo[x].Size / RawDataChunkInfo[x].ChunkSize;
                }

                BR = new BinaryReader(meta.raw.rawChunks[chunknumber].MS);
                SubMeshInfo = new ModelSubMeshInfo[RawDataChunkInfo[0].ChunkCount];
                for (int x = 0; x < RawDataChunkInfo[0].ChunkCount; x++)
                {
                    SubMeshInfo[x] = new ModelSubMeshInfo();
                    BR.BaseStream.Position = HeaderSize + RawDataChunkInfo[0].Offset + (x * 72) + 4;
                    SubMeshInfo[x].ShaderNumber = BR.ReadUInt16();
                    SubMeshInfo[x].IndiceStart = BR.ReadUInt16();
                    SubMeshInfo[x].IndiceCount = BR.ReadUInt16();
                }

                BR.BaseStream.Position = 40;
                IndiceCount = BR.ReadUInt16();

                

                int indicechunk = 0;
                int verticechunk = 0;
                int uvchunk = 0;

                for (int x = 0; x < RawDataChunkInfo.Length; x++)
                {
                    if (RawDataChunkInfo[x].ChunkSize == 2)
                    {
                        indicechunk = x;
                        verticechunk = x + 2;
                        uvchunk = x + 3;
                        break;
                    }
                }

                int bonemapchunk = 0;
                BR.BaseStream.Position = 108;
                int tempbonemapcount = BR.ReadUInt16();
                if (tempbonemapcount > 0)
                {
                    for (int x = uvchunk + 1; x < RawDataChunkInfo.Length; x++)
                    {
                        if (RawDataChunkInfo[x].ChunkSize == 1)
                        {
                            bonemapchunk = x;
                            break;
                        }
                    }

                    BR.BaseStream.Position = HeaderSize + RawDataChunkInfo[bonemapchunk].Offset;
                    for (int x = 0; x < tempbonemapcount; x++)
                    {
                        BoneMap.Add(BR.ReadByte());
                    }
                }
                else
                {
                    BoneMap.Add(0);
                }

                

                RawDataChunkInfo[verticechunk].ChunkSize = RawDataChunkInfo[verticechunk].Size / VerticeCount;
                for (int x = 0; x < VerticeCount; x++)
                {
                    Vector3 vec = new Vector3();
                    BR.BaseStream.Position = HeaderSize + RawDataChunkInfo[verticechunk].Offset +
                                             (RawDataChunkInfo[verticechunk].ChunkSize * x);
                    vec.X = DecompressVertice(BR.ReadInt16(), bb.MinX, bb.MaxX);
                    vec.Y = DecompressVertice(BR.ReadInt16(), bb.MinY, bb.MaxY);
                    vec.Z = DecompressVertice(BR.ReadInt16(), bb.MinZ, bb.MaxZ);
                    Vertices.Add(vec);

                    // if (tempbonemapcount == 0) { continue; }

                    switch (RawDataChunkInfo[verticechunk].ChunkSize)
                    {
                        case 6:
                            BoneInfo b = new BoneInfo();
                            b.BoneIndex.Add(0);
                            b.Weight.Add(1.0f);
                            VerticeBones.Add(b);

                            break;
                        case 8:
                            BoneInfo c = new BoneInfo();
                            c.BoneIndex.Add(BR.ReadByte());
                            c.Weight.Add(1.0f);
                            byte tempb = BR.ReadByte();
                            if (tempb == 0)
                            {
                                VerticeBones.Add(c);
                                break;
                            }

                            BoneInfo c2 = new BoneInfo();
                            c2.BoneIndex.Add(tempb);
                            c2.Weight.Add(1.0f);
                            VerticeBones.Add(c2);
                            c.Weight[0] = 1.0f;
                            VerticeBones.Add(c);
                            break;
                        case 12:
                            BoneInfo bbb = new BoneInfo();
                            bbb.BoneIndex.Add(BR.ReadByte());
                            bbb.Weight.Add(0.99f);
                            VerticeBones.Add(bbb);
                            break;
                    }
                }

                RawDataChunkInfo[uvchunk].ChunkSize = 4;
                for (int x = 0; x < VerticeCount; x++)
                {
                    Vector2 tempuv = new Vector2();
                    BR.BaseStream.Position = HeaderSize + RawDataChunkInfo[uvchunk].Offset +
                                             (RawDataChunkInfo[uvchunk].ChunkSize * x);
                    tempuv.X = DecompressVertice(BR.ReadInt16(), bb.MinU, bb.MaxU) % 1;
                    tempuv.Y = DecompressVertice(BR.ReadInt16(), bb.MinV, bb.MaxV) % 1;

                    // if (tempuv.X > 1) { tempuv.X = tempuv.X - 1; }
                    // else
                    if (tempuv.X < 0)
                    {
                        tempuv.X = 1 - tempuv.X;
                    }

                    // if (tempuv.Y > 1) { tempuv.Y = tempuv.Y - 1; }
                    // else
                    if (tempuv.Y < 0)
                    {
                        tempuv.Y = 1 - tempuv.Y;
                    }

                    UVs.Add(tempuv);
                }

                RawDataChunkInfo[uvchunk + 1].ChunkSize = 12;
                for (int x = 0; x < VerticeCount; x++)
                {
                    BR.BaseStream.Position = HeaderSize + RawDataChunkInfo[uvchunk + 1].Offset +
                                             (RawDataChunkInfo[uvchunk + 1].ChunkSize * x);

                    int dword = BR.ReadInt32();

                    Vector3 tempnormal = DecompressNormal(dword);

                    int converto = CompressNormal(tempnormal);
                    Vector3 tempnormal2 = DecompressNormal(converto);
                    Normals.Add(tempnormal);
                }

                BR.BaseStream.Position = HeaderSize + RawDataChunkInfo[indicechunk].Offset;
                this.Indices = new short[IndiceCount];
                for (int x = 0; x < IndiceCount; x++)
                {
                    Indices[x] = (short)BR.ReadUInt16();
                }
            }

            #endregion

            #region Public Methods

            /// <summary>
            /// The compress vertice.
            /// </summary>
            /// <param name="input">The input.</param>
            /// <param name="min">The min.</param>
            /// <param name="max">The max.</param>
            /// <returns>The compress vertice.</returns>
            /// <remarks></remarks>
            public float CompressVertice(float input, float min, float max)
            {
                float result = input - min;
                result = result / (max - min);
                result = result * 65535;
                result = result - 32768;
                return result;
            }

            /// <summary>
            /// The decompress vertice.
            /// </summary>
            /// <param name="input">The input.</param>
            /// <param name="min">The min.</param>
            /// <param name="max">The max.</param>
            /// <returns>The decompress vertice.</returns>
            /// <remarks></remarks>
            public float DecompressVertice(float input, float min, float max)
            {
                float percent = (input + 32768) / 65535;
                float result = (percent * (max - min)) + min;
                return result;
            }

            #endregion

            /// <summary>
            /// The bone info.
            /// </summary>
            /// <remarks></remarks>
            public class BoneInfo
            {
                #region Constants and Fields

                /// <summary>
                /// The bone index.
                /// </summary>
                public List<int> BoneIndex = new List<int>();

                /// <summary>
                /// The weight.
                /// </summary>
                public List<float> Weight = new List<float>();

                #endregion
            }

            /// <summary>
            /// The model sub mesh info.
            /// </summary>
            /// <remarks></remarks>
            public class ModelSubMeshInfo
            {
                #region Constants and Fields

                /// <summary>
                /// The indice count.
                /// </summary>
                public int IndiceCount;

                /// <summary>
                /// The indice start.
                /// </summary>
                public int IndiceStart;

                /// <summary>
                /// The shader number.
                /// </summary>
                public int ShaderNumber;

                #endregion
            }

            /// <summary>
            /// The raw data offset chunk.
            /// </summary>
            /// <remarks></remarks>
            public class RawDataOffsetChunk
            {
                #region Constants and Fields

                /// <summary>
                /// The chunk count.
                /// </summary>
                public int ChunkCount;

                /// <summary>
                /// The chunk size.
                /// </summary>
                public int ChunkSize;

                /// <summary>
                /// The offset.
                /// </summary>
                public int Offset;

                /// <summary>
                /// The size.
                /// </summary>
                public int Size;

                #endregion
            }
        }

        /// <summary>
        /// The shader container.
        /// </summary>
        /// <remarks></remarks>
        public class ShaderContainer
        {
            #region Constants and Fields

            /// <summary>
            /// The shader.
            /// </summary>
            public ShaderInfo[] Shader;

            /// <summary>
            /// The count.
            /// </summary>
            public int count;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            /// Initializes a new instance of the <see cref="ShaderContainer"/> class.
            /// </summary>
            /// <remarks></remarks>
            public ShaderContainer()
            {
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="ShaderContainer"/> class.
            /// </summary>
            /// <param name="meta">The meta.</param>
            /// <remarks></remarks>
            public ShaderContainer(ref Meta meta)
            {
                BinaryReader BR = new BinaryReader(meta.MS);
                BR.BaseStream.Position = 96;
                int tempc = BR.ReadInt32();
                int tempr = BR.ReadInt32() - meta.magic - meta.offset;
                Shader = new ShaderInfo[tempc];

                count = tempc;
                for (int x = 0; x < tempc; x++)
                {
                    BR.BaseStream.Position = tempr + (x * 32) + 12;
                    int tempint = meta.Map.Functions.ForMeta.FindMetaByID(BR.ReadInt32());
                    Shader[x] = new ShaderInfo(tempint, meta.Map);
                }
            }

            #endregion
        }
    }
}