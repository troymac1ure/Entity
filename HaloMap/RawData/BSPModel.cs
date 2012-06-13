// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BSPModel.cs" company="">
//   
// </copyright>
// <summary>
//   The halo bsp vertex.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HaloMap.RawData
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    using HaloMap.H2MetaContainers;
    using HaloMap.Map;
    using HaloMap.Meta;
    using HaloMap.Render;

    using Microsoft.DirectX;
    using Microsoft.DirectX.Direct3D;

    /// <summary>
    /// The bsp model.
    /// </summary>
    /// <remarks></remarks>
    public class BSPModel : IDisposable
    {
        #region Constants and Fields

        /// <summary>
        /// The bsp permutation raw data meta chunks.
        /// </summary>
        public BSPPermutationRawDataMetaChunk[] BSPPermutationRawDataMetaChunks;

        /// <summary>
        /// The bsp permutation raw data meta chunks offset.
        /// </summary>
        public int BSPPermutationRawDataMetaChunksOffset;

        /// <summary>
        /// The bsp raw data meta chunks.
        /// </summary>
        public BSPRawDataMetaChunk[] BSPRawDataMetaChunks;

        /// <summary>
        /// The bsp raw data meta chunks offset.
        /// </summary>
        public int BSPRawDataMetaChunksOffset;

        /// <summary>
        /// The bsp number.
        /// </summary>
        public int BspNumber;


        /// <summary>
        /// Performs culling of BSP mesh out of camera (buggy)
        /// </summary>
        public bool cameraCulling = false;

        /// <summary>
        /// The cluster info.
        /// </summary>
        public ClusterPVS[] ClusterInfo;

        /// <summary>
        /// The display.
        /// </summary>
        public BSPDisplayedInfo Display;

        /// <summary>
        /// The draw bsp permutations.
        /// </summary>
        public bool DrawBSPPermutations = true;

        /// <summary>
        /// The light map bitmap.
        /// </summary>
        public Bitmap[] LightMapBitmap;

        /// <summary>
        /// The light map parsed bitmap.
        /// </summary>
        public ParsedBitmap LightMapParsedBitmap;

        /// <summary>
        /// The light map texture.
        /// </summary>
        public Texture[] LightMapTexture;

        /// <summary>
        /// Map minimum X,Y,Z boundries
        /// </summary>
        public Vector3 minBoundries;

        /// <summary>
        /// Map maximum X,Y,Z boundries
        /// </summary>
        public Vector3 maxBoundries;

        /// <summary>
        /// The name.
        /// </summary>
        public string Name;

        /// <summary>
        /// The permutation info.
        /// </summary>
        public PermutationPlacement[] PermutationInfo;

        /// <summary>
        /// The render bsp lighting.
        /// </summary>
        public bool RenderBSPLighting = true;

        /// <summary>
        /// The scenery light map bitmap.
        /// </summary>
        public Bitmap[] SceneryLightMapBitmap;

        /// <summary>
        /// The scenery light map texture.
        /// </summary>
        public Texture[] SceneryLightMapTexture;

        /// <summary>
        /// The shaders.
        /// </summary>
        public BSPShaderContainer Shaders;

        /// <summary>
        /// The sky box.
        /// </summary>
        public ParsedModel SkyBox;

        /// <summary>
        /// The spawns.
        /// </summary>
        public SpawnInfo Spawns;

        /// <summary>
        /// The unknown chunks.
        /// </summary>
        public UnknownChunk[] UnknownChunks;

        /// <summary>
        /// The sky.
        /// </summary>
        public Sky sky;

        /// <summary>
        /// The bitm ptrs.
        /// </summary>
        private readonly List<IntPtr> bitmPtrs = new List<IntPtr>();

        /// <summary>
        /// The map.
        /// </summary>
        private readonly Map map;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BSPModel"/> class.
        /// </summary>
        /// <param name="meta">The meta.</param>
        /// <remarks></remarks>
        public BSPModel(ref Meta meta)
        {
            if (meta == null)
            {
                MessageBox.Show("ERROR: NO SBSP tag selected!");
                this.BspNumber = -1;
                return;
            }

            this.map = meta.Map;

            string[] temps = meta.name.Split('\\');
            Name = temps[temps.Length - 1];
            BspNumber = map.BSP.FindBSPNumberByBSPIdent(meta.ident);

            switch (map.HaloVersion)
            {
                case HaloVersionEnum.Halo2:
                case HaloVersionEnum.Halo2Vista:
                    // 10x 0.0000010
                    LoadModelStructure(ref meta);
                    // 5x 0.0000010
                    LoadPermutations(ref meta);
                    LoadUnknowns(ref meta);
                    // 5x 0.0000010
                    Shaders = new BSPShaderContainer(this, ref meta);
                    Display = new BSPDisplayedInfo();
                    // 2x 0.0000010
                    LoadLightmaps();
                    sky = new Sky();
                    LoadSky(ref meta);
                    Spawns = new SpawnInfo(map);
                    // 1x 0.0000010
                    ClusterInfo = ClustersPVS.GetClustersPVSInfo(ref meta);
                    break;
                case HaloVersionEnum.Halo1:
                case HaloVersionEnum.HaloCE:
                    Shaders = new BSPShaderContainer();
                    this.BSPPermutationRawDataMetaChunks = new BSPPermutationRawDataMetaChunk[0];
                    this.LightMapBitmap = new Bitmap[0];
                    this.SceneryLightMapBitmap = new Bitmap[0];
                    this.PermutationInfo = new PermutationPlacement[0];
                    LoadModelStructure(ref meta);

                    Display = new BSPDisplayedInfo();
                    Spawns = new SpawnInfo(map);
                    sky = new Sky();
                    break;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// The add compressed vertice.
        /// </summary>
        /// <param name="BR">The br.</param>
        /// <param name="chunk">The chunk.</param>
        /// <remarks></remarks>
        public static void AddCompressedVertice(ref BinaryReader BR, ref BSPRawDataMetaChunk chunk)
        {
            // vertexes
            Vector3 vertice = new Vector3(BR.ReadSingle(), BR.ReadSingle(), BR.ReadSingle());
            chunk.Vertices.Add(vertice);
            // normals
            int test = BR.ReadInt32();
            Vector3 normal2 = ParsedModel.DecompressNormal(test);
            chunk.Normals.Add(normal2);
            test = BR.ReadInt32();
            Vector3 binormal2 = ParsedModel.DecompressNormal(test);
            chunk.Binormals.Add(binormal2);
            test = BR.ReadInt32();
            Vector3 tangent2 = ParsedModel.DecompressNormal(test);
            chunk.Tangents.Add(tangent2);
            // basemap uvs

            // short testx = BR.ReadInt16();
            float u = BR.ReadSingle(); // chunk.DecompressVertice(Convert.ToSingle(testx), -1, 1);
            // testx = BR.ReadInt16();
            float v = BR.ReadSingle(); // chunk.DecompressVertice(Convert.ToSingle(testx), -1, 1);// % 1;
            Vector2 uv2 = new Vector2(u, v);
            chunk.UVs.Add(uv2);

            // lightmap uvs
            // testx = BR.ReadInt16();
            // float ux = chunk.DecompressVertice(Convert.ToSingle(testx), -1, 1);// %1 ;
            // testx = BR.ReadInt16();
            // float vx =chunk.DecompressVertice(Convert.ToSingle(testx), -1, 1);// % 1;
            // Vector2 uv2x = new Vector2(ux, vx);
            // chunk.LightMapUVs.Add(uv2);
        }

        /// <summary>
        /// The add uncompressed vertice.
        /// </summary>
        /// <param name="BR">The br.</param>
        /// <param name="chunk">The chunk.</param>
        /// <remarks></remarks>
        public static void AddUncompressedVertice(ref BinaryReader BR, ref BSPRawDataMetaChunk chunk)
        {
            Vector3 vertice = new Vector3(BR.ReadSingle(), BR.ReadSingle(), BR.ReadSingle());
            chunk.Vertices.Add(vertice);
            Vector3 normal = new Vector3(BR.ReadSingle(), BR.ReadSingle(), BR.ReadSingle());
            chunk.Normals.Add(normal);
            Vector3 binormal = new Vector3(BR.ReadSingle(), BR.ReadSingle(), BR.ReadSingle());
            chunk.Binormals.Add(binormal);
            Vector3 tangent = new Vector3(BR.ReadSingle(), BR.ReadSingle(), BR.ReadSingle());
            chunk.Tangents.Add(tangent);
            Vector2 uv = new Vector2(BR.ReadSingle(), BR.ReadSingle());

            chunk.UVs.Add(uv);
        }

        /// <summary>
        /// The extract model.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <remarks></remarks>
        public void ExtractModel(string path)
        {
            if (path[path.Length - 1] != '\\')
            {
                path += "\\";
            }

            

            string texturepath = path + "Textures\\";
            Directory.CreateDirectory(texturepath);
            List<string> names = new List<string>();
            string mtllib = this.Name + ".mtl";
            FileStream FS = new FileStream(path + mtllib, FileMode.Create);
            StreamWriter SW = new StreamWriter(FS);

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

                try
                {
                    TextureLoader.Save(
                        texturepath + temps + ".dds", ImageFileFormat.Dds, this.Shaders.Shader[x].MainTexture);
                }
                catch
                {
                }

                if (x > 0)
                {
                    // space
                    SW.WriteLine(string.Empty);
                }

                SW.WriteLine("newmtl " + temps);
                SW.WriteLine("Ka 1.000000 1.000000 1.000000");
                SW.WriteLine("Kd 1.000000 1.000000 1.000000");
                SW.WriteLine("Ks 1.000000 1.000000 1.000000");
                SW.WriteLine("Ns 0.000000");
                SW.WriteLine(@"map_Kd .\\Textures\\" + temps + ".dds");
            }

            SW.Close();
            FS.Close();

            

            #region ExportBSPMeshes

            for (int x = 0; x < this.BSPRawDataMetaChunks.Length; x++)
            {
                if (this.BSPRawDataMetaChunks[x].VerticeCount == 0)
                {
                    continue;
                }

                FS = new FileStream(path + this.Name + "[" + x + "].obj", FileMode.Create);
                SW = new StreamWriter(FS);
                SW.WriteLine("# ------------------------------------");
                SW.WriteLine("# Halo 2 BSP Mesh - Extracted with Entity");
                SW.WriteLine("# ------------------------------------");
                SW.WriteLine("mtllib " + mtllib);
                for (int y = 0; y < this.BSPRawDataMetaChunks[x].VerticeCount; y++)
                {
                    string temps = "v " + this.BSPRawDataMetaChunks[x].Vertices[y].X.ToString("R") + " " +
                                   this.BSPRawDataMetaChunks[x].Vertices[y].Y.ToString("R") + " " +
                                   this.BSPRawDataMetaChunks[x].Vertices[y].Z.ToString("R");
                    SW.WriteLine(temps);
                }

                SW.WriteLine("# " + this.BSPRawDataMetaChunks[x].Vertices.Count + " vertices");
                for (int y = 0; y < this.BSPRawDataMetaChunks[x].VerticeCount; y++)
                {
                    string temps = "vt " + this.BSPRawDataMetaChunks[x].UVs[y].X.ToString("R") + " " +
                                   this.BSPRawDataMetaChunks[x].UVs[y].Y.ToString("R");
                    SW.WriteLine(temps);
                }

                SW.WriteLine("# " + this.BSPRawDataMetaChunks[x].Vertices.Count + " texture vertices");
                for (int y = 0; y < this.BSPRawDataMetaChunks[x].VerticeCount; y++)
                {
                    string temps = "vn " + this.BSPRawDataMetaChunks[x].Normals[y].X.ToString("R") + " " +
                                   this.BSPRawDataMetaChunks[x].Normals[y].Y.ToString("R") + " " +
                                   this.BSPRawDataMetaChunks[x].Normals[y].Z.ToString("R");
                    SW.WriteLine(temps);
                }

                SW.WriteLine("# " + this.BSPRawDataMetaChunks[x].Vertices.Count + " normals");
                for (int y = 0; y < this.BSPRawDataMetaChunks[x].SubMeshInfo.Length; y++)
                {
                    SW.WriteLine("g 0." + y);
                    // SW.WriteLine("s 0." + y.ToString());
                    SW.WriteLine("usemtl  " + names[this.BSPRawDataMetaChunks[x].SubMeshInfo[y].ShaderNumber]);

                    int[] shite = new int[100000];
                    int s = 0;
                    // if (this.BSPRawDataMetaChunks[x].SubMeshInfo[y].IndiceCount == (this.BSPRawDataMetaChunks[x].SubMeshInfo[y].IndiceCount/3)*3)
                    if (this.BSPRawDataMetaChunks[x].FaceCount * 3 != this.BSPRawDataMetaChunks[x].IndiceCount)
                    {
                        int m = this.BSPRawDataMetaChunks[x].SubMeshInfo[y].IndiceStart;

                        bool dir = false;
                        short tempx;
                        short tempy;
                        short tempz;

                        do
                        {
                            // if (mode.EndOfIndices[x][j]>m+2){break;}
                            tempx = this.BSPRawDataMetaChunks[x].Indices[m];
                            tempy = this.BSPRawDataMetaChunks[x].Indices[m + 1];
                            tempz = this.BSPRawDataMetaChunks[x].Indices[m + 2];

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
                               this.BSPRawDataMetaChunks[x].SubMeshInfo[y].IndiceStart +
                               this.BSPRawDataMetaChunks[x].SubMeshInfo[y].IndiceCount - 2);
                    }
                    else
                    {
                        Array.Copy(
                            this.BSPRawDataMetaChunks[x].Indices, 
                            this.BSPRawDataMetaChunks[x].SubMeshInfo[y].IndiceStart, 
                            shite, 
                            0, 
                            this.BSPRawDataMetaChunks[x].SubMeshInfo[y].IndiceCount);
                        s = this.BSPRawDataMetaChunks[x].SubMeshInfo[y].IndiceCount;
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

                SW.Close();
                FS.Close();
            }

            #endregion

            #region ExportBSPPermutationMeshes

            for (int x = 0; x < this.BSPPermutationRawDataMetaChunks.Length; x++)
            {
                if (this.BSPPermutationRawDataMetaChunks[x].SubMeshInfo == null)
                {
                    continue;
                }

                FS = new FileStream(path + this.Name + "-Permutation[" + x + "].obj", FileMode.Create);
                SW = new StreamWriter(FS);
                SW.WriteLine("# ------------------------------------");
                SW.WriteLine("# Halo 2 BSP Permutation Mesh - Extracted with Entity");
                SW.WriteLine("# ------------------------------------");
                SW.WriteLine("mtllib " + mtllib);
                for (int y = 0; y < this.BSPPermutationRawDataMetaChunks[x].VerticeCount; y++)
                {
                    string temps = "v " + this.BSPPermutationRawDataMetaChunks[x].Vertices[y].X.ToString("R") + " " +
                                   this.BSPPermutationRawDataMetaChunks[x].Vertices[y].Y.ToString("R") + " " +
                                   this.BSPPermutationRawDataMetaChunks[x].Vertices[y].Z.ToString("R");
                    SW.WriteLine(temps);
                }

                SW.WriteLine("# " + this.BSPPermutationRawDataMetaChunks[x].Vertices.Count + " vertices");
                for (int y = 0; y < this.BSPPermutationRawDataMetaChunks[x].VerticeCount; y++)
                {
                    string temps = "vt " + this.BSPPermutationRawDataMetaChunks[x].UVs[y].X.ToString("R") + " " +
                                   this.BSPPermutationRawDataMetaChunks[x].UVs[y].Y.ToString("R");
                    SW.WriteLine(temps);
                }

                SW.WriteLine("# " + this.BSPPermutationRawDataMetaChunks[x].Vertices.Count + " texture vertices");
                for (int y = 0; y < this.BSPPermutationRawDataMetaChunks[x].VerticeCount; y++)
                {
                    string temps = "vn " + this.BSPPermutationRawDataMetaChunks[x].Normals[y].X.ToString("R") + " " +
                                   this.BSPPermutationRawDataMetaChunks[x].Normals[y].Y.ToString("R") + " " +
                                   this.BSPPermutationRawDataMetaChunks[x].Normals[y].Z.ToString("R");
                    SW.WriteLine(temps);
                }

                SW.WriteLine("# " + this.BSPPermutationRawDataMetaChunks[x].Vertices.Count + " normals");
                for (int y = 0; y < this.BSPPermutationRawDataMetaChunks[x].SubMeshInfo.Length; y++)
                {
                    SW.WriteLine("g 0." + y);
                    // SW.WriteLine("s 0." + y.ToString());
                    SW.WriteLine(
                        "usemtl  " + names[this.BSPPermutationRawDataMetaChunks[x].SubMeshInfo[y].ShaderNumber]);

                    short[] shite = new short[100000];
                    int s = 0;
                    if (this.BSPPermutationRawDataMetaChunks[x].FaceCount * 3 !=
                        this.BSPPermutationRawDataMetaChunks[x].IndiceCount)
                    {
                        int m = this.BSPPermutationRawDataMetaChunks[x].SubMeshInfo[y].IndiceStart;

                        bool dir = false;
                        short tempx;
                        short tempy;
                        short tempz;

                        do
                        {
                            // if (mode.EndOfIndices[x][j]>m+2){break;}
                            tempx = this.BSPPermutationRawDataMetaChunks[x].Indices[m];
                            tempy = this.BSPPermutationRawDataMetaChunks[x].Indices[m + 1];
                            tempz = this.BSPPermutationRawDataMetaChunks[x].Indices[m + 2];

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
                               this.BSPPermutationRawDataMetaChunks[x].SubMeshInfo[y].IndiceStart +
                               this.BSPPermutationRawDataMetaChunks[x].SubMeshInfo[y].IndiceCount - 2);
                    }
                    else
                    {
                        Array.Copy(
                            this.BSPPermutationRawDataMetaChunks[x].Indices, 
                            this.BSPPermutationRawDataMetaChunks[x].SubMeshInfo[y].IndiceStart, 
                            shite, 
                            0, 
                            this.BSPPermutationRawDataMetaChunks[x].SubMeshInfo[y].IndiceCount);
                        s = this.BSPPermutationRawDataMetaChunks[x].SubMeshInfo[y].IndiceCount;
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

                SW.Close();
                FS.Close();
            }

            #endregion

            if (this.UnknownChunks == null)
            {
                return;
            }

            #region ExportUnknown

            for (int x = 0; x < this.UnknownChunks.Length; x++)
            {
                if (this.UnknownChunks[x].IndiceCount == 0)
                {
                    continue;
                }

                FS = new FileStream(path + this.Name + "-Unknown[" + x + "].obj", FileMode.Create);
                SW = new StreamWriter(FS);
                SW.WriteLine("# ------------------------------------");
                SW.WriteLine("# Halo 2 BSP Unknown Mesh - Extracted with Entity");
                SW.WriteLine("# ------------------------------------");

                for (int y = 0; y < this.UnknownChunks[x].VerticeCount; y++)
                {
                    string temps = "v " + this.UnknownChunks[x].Vertices[y].X + " " +
                                   this.UnknownChunks[x].Vertices[y].Y + " " + this.UnknownChunks[x].Vertices[y].Z;
                    SW.WriteLine(temps);
                }

                SW.WriteLine("# " + this.UnknownChunks[x].Vertices.Count + " vertices");

                SW.WriteLine("g 0.0");

                short[] shite = new short[100000];
                int s = 0;

                Array.Copy(this.UnknownChunks[x].Indices, 0, shite, 0, UnknownChunks[x].IndiceCount);
                s = this.UnknownChunks[x].IndiceCount;

                for (int xx = 0; xx < s; xx += 3)
                {
                    string temps = "f " + (shite[xx] + 1) + "/" + (shite[xx] + 1) + "/" + (shite[xx] + 1) + " " +
                                   (shite[xx + 1] + 1) + "/" + (shite[xx + 1] + 1) + "/" + (shite[xx + 1] + 1) + " " +
                                   (shite[xx + 2] + 1) + "/" + (shite[xx + 2] + 1) + "/" + (shite[xx + 2] + 1);
                    SW.WriteLine(temps);
                }

                SW.WriteLine("# " + (s / 3) + " elements");

                SW.Close();
                FS.Close();
            }

            #endregion
        }

        /// <summary>
        /// The extract model as single mesh.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <remarks></remarks>
        public void ExtractModelAsSingleMesh(string path)
        {
            int ndex = path.LastIndexOf('\\');
            string fuck = path.Remove(ndex + 1);

            

            string texturepath = fuck + "Textures\\";
            Directory.CreateDirectory(texturepath);
            List<string> names = new List<string>();
            string mtllib = this.Name + ".mtl";
            FileStream FS = new FileStream(fuck + mtllib, FileMode.Create);
            StreamWriter SW = new StreamWriter(FS);
            Panel p = new Panel();
            Renderer r = new Renderer();
            r.CreateDevice(p);
            for (int x = 0; x < this.Shaders.Shader.Length; x++)
            {
                string[] namesplit = this.Shaders.Shader[x].shaderName.Split('\\');
                string temps = namesplit[namesplit.Length - 1];
                names.Add(temps);
                this.Shaders.Shader[x].MakeTextures(ref r.device);

                TextureLoader.Save(
                    texturepath + temps + ".dds", ImageFileFormat.Dds, this.Shaders.Shader[x].MainTexture);
                if (x > 0)
                {
                    SW.WriteLine(string.Empty);
                }

                SW.WriteLine("newmtl " + temps);
                SW.WriteLine("Ka 1.000000 1.000000 1.000000");
                SW.WriteLine("Kd 1.000000 1.000000 1.000000");
                SW.WriteLine("Ks 1.000000 1.000000 1.000000");
                SW.WriteLine("Ns 0.000000");
                SW.WriteLine(@"map_Kd .\\Textures\\" + temps + ".dds");
            }

            SW.Close();
            FS.Close();

            

            #region ExportBSPMeshes

            FS = new FileStream(path, FileMode.Create);
            SW = new StreamWriter(FS);
            SW.WriteLine("# ------------------------------------");
            SW.WriteLine("# Halo 2 BSP Mesh - Extracted with Entity");
            SW.WriteLine("# ------------------------------------");
            SW.WriteLine("mtllib " + mtllib);
            int vertcount = 0;

            for (int x = 0; x < this.BSPRawDataMetaChunks.Length; x++)
            {
                if (this.BSPRawDataMetaChunks[x].VerticeCount == 0)
                {
                    continue;
                }

                for (int y = 0; y < this.BSPRawDataMetaChunks[x].VerticeCount; y++)
                {
                    string temps = "v " + this.BSPRawDataMetaChunks[x].Vertices[y].X.ToString("R") + " " +
                                   this.BSPRawDataMetaChunks[x].Vertices[y].Y.ToString("R") + " " +
                                   this.BSPRawDataMetaChunks[x].Vertices[y].Z.ToString("R");
                    SW.WriteLine(temps);
                }

                SW.WriteLine("# " + this.BSPRawDataMetaChunks[x].Vertices.Count + " vertices");
                for (int y = 0; y < this.BSPRawDataMetaChunks[x].VerticeCount; y++)
                {
                    string temps = "vt " + this.BSPRawDataMetaChunks[x].UVs[y].X.ToString("R") + " " +
                                   (1 - this.BSPRawDataMetaChunks[x].UVs[y].Y).ToString("R");
                    SW.WriteLine(temps);
                }

                SW.WriteLine("# " + this.BSPRawDataMetaChunks[x].Vertices.Count + " texture vertices");
                for (int y = 0; y < this.BSPRawDataMetaChunks[x].VerticeCount; y++)
                {
                    string temps = "vn " + this.BSPRawDataMetaChunks[x].Normals[y].X.ToString("R") + " " +
                                   this.BSPRawDataMetaChunks[x].Normals[y].Y.ToString("R") + " " +
                                   this.BSPRawDataMetaChunks[x].Normals[y].Z.ToString("R");
                    SW.WriteLine(temps);
                }

                SW.WriteLine("# " + this.BSPRawDataMetaChunks[x].Vertices.Count + " normals");
                for (int y = 0; y < this.BSPRawDataMetaChunks[x].SubMeshInfo.Length; y++)
                {
                    SW.WriteLine("g " + x + "." + y);
                    // SW.WriteLine("s "+x.ToString()+"." + y.ToString());
                    SW.WriteLine("usemtl  " + names[this.BSPRawDataMetaChunks[x].SubMeshInfo[y].ShaderNumber]);

                    // int[] shite = new int[100000];
                    int[] shite = new int[this.BSPRawDataMetaChunks[x].SubMeshInfo[y].IndiceCount];
                    int s = 0;
                    if (this.BSPRawDataMetaChunks[x].FaceCount * 3 != this.BSPRawDataMetaChunks[x].IndiceCount)
                    {
                        int m = this.BSPRawDataMetaChunks[x].SubMeshInfo[y].IndiceStart;
                        bool dir = false;
                        short tempx;
                        short tempy;
                        short tempz;

                        do
                        {
                            // if (mode.EndOfIndices[x][j]>m+2){break;}
                            tempx = this.BSPRawDataMetaChunks[x].Indices[m];
                            tempy = this.BSPRawDataMetaChunks[x].Indices[m + 1];
                            tempz = this.BSPRawDataMetaChunks[x].Indices[m + 2];

                            if (tempx != tempy && tempx != tempz && tempy != tempz)
                            {
                                if (dir == false)
                                {
                                    shite[s] = vertcount + tempx;
                                    shite[s + 1] = vertcount + tempy;
                                    shite[s + 2] = vertcount + tempz;
                                    s += 3;

                                    dir = true;
                                }
                                else
                                {
                                    shite[s] = vertcount + tempx;
                                    shite[s + 1] = vertcount + tempz;
                                    shite[s + 2] = vertcount + tempy;
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
                               this.BSPRawDataMetaChunks[x].SubMeshInfo[y].IndiceStart +
                               this.BSPRawDataMetaChunks[x].SubMeshInfo[y].IndiceCount - 2);
                    }
                    else
                    {
                        for (int u = 0; u < this.BSPRawDataMetaChunks[x].SubMeshInfo[y].IndiceCount; u++)
                        {
                            shite[u] = vertcount +
                                        this.BSPRawDataMetaChunks[x].Indices[
                                            this.BSPRawDataMetaChunks[x].SubMeshInfo[y].IndiceStart + u];
                        }

                        s = this.BSPRawDataMetaChunks[x].SubMeshInfo[y].IndiceCount;
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

                vertcount += this.BSPRawDataMetaChunks[x].VerticeCount;
            }

            #endregion

            SW.Close();
            FS.Close();

            #region ExportBSPPermutationMeshes

            FS = new FileStream(path.Substring(0, path.LastIndexOf('.')) + "-permutations.obj", FileMode.Create);
            SW = new StreamWriter(FS);
            SW.WriteLine("# ------------------------------------");
            SW.WriteLine("# Halo 2 BSP Permutation Mesh");
            SW.WriteLine("# ------------------------------------");
            SW.WriteLine("mtllib " + mtllib);
            vertcount = 0;

            for (int tx = 0; tx < this.PermutationInfo.Length; tx++)
            {
                int x = this.PermutationInfo[tx].sceneryIndex;
                if ((this.BSPPermutationRawDataMetaChunks[x].RawDataChunkInfo.Length == 0) ||
                    (this.BSPPermutationRawDataMetaChunks[x].VerticeCount == 0))
                {
                    continue;
                }

                for (int y = 0; y < this.BSPPermutationRawDataMetaChunks[x].VerticeCount; y++)
                {
                    Vector3 tv3 = new Vector3(
                        this.BSPPermutationRawDataMetaChunks[x].Vertices[y].X, 
                        this.BSPPermutationRawDataMetaChunks[x].Vertices[y].Y, 
                        this.BSPPermutationRawDataMetaChunks[x].Vertices[y].Z);
                    tv3.TransformCoordinate(this.PermutationInfo[tx].mat);
                    string temps = "v " + tv3.X.ToString("R") + " " + tv3.Y.ToString("R") + " " + tv3.Z.ToString("R");
                    SW.WriteLine(temps);
                }

                SW.WriteLine("# " + this.BSPPermutationRawDataMetaChunks[x].Vertices.Count + " vertices");
                for (int y = 0; y < this.BSPPermutationRawDataMetaChunks[x].VerticeCount; y++)
                {
                    string temps = "vt " + this.BSPPermutationRawDataMetaChunks[x].UVs[y].X.ToString("R") + " " +
                                   this.BSPPermutationRawDataMetaChunks[x].UVs[y].Y.ToString("R");
                    SW.WriteLine(temps);
                }

                SW.WriteLine("# " + this.BSPPermutationRawDataMetaChunks[x].Vertices.Count + " texture vertices");
                for (int y = 0; y < this.BSPPermutationRawDataMetaChunks[x].VerticeCount; y++)
                {
                    Vector3 tv3 = new Vector3(
                        this.BSPPermutationRawDataMetaChunks[x].Normals[y].X, 
                        this.BSPPermutationRawDataMetaChunks[x].Normals[y].Y, 
                        this.BSPPermutationRawDataMetaChunks[x].Normals[y].Z);
                    tv3.TransformNormal(this.PermutationInfo[tx].mat);
                    string temps = "vn " + tv3.X.ToString("R") + " " + tv3.Y.ToString("R") + " " + tv3.Z.ToString("R");
                    /*
                    string temps = "vn " + this.BSPPermutationRawDataMetaChunks[x].Normals[y].X.ToString()
                                   + " " + this.BSPPermutationRawDataMetaChunks[x].Normals[y].Y.ToString()
                                   + " " + this.BSPPermutationRawDataMetaChunks[x].Normals[y].Z.ToString();
                    */
                    SW.WriteLine(temps);
                }

                SW.WriteLine("# " + this.BSPPermutationRawDataMetaChunks[x].Vertices.Count + " normals");
                for (int y = 0; y < this.BSPPermutationRawDataMetaChunks[x].SubMeshInfo.Length; y++)
                {
                    SW.WriteLine("g " + tx + "." + y);
                    SW.WriteLine(
                        "usemtl  " + names[this.BSPPermutationRawDataMetaChunks[x].SubMeshInfo[y].ShaderNumber]);

                    // int[] shite = new int[100000];
                    int[] shite = new int[this.BSPPermutationRawDataMetaChunks[x].SubMeshInfo[y].IndiceCount];

                    int s = 0;
                    if (this.BSPPermutationRawDataMetaChunks[x].FaceCount * 3 !=
                        this.BSPPermutationRawDataMetaChunks[x].IndiceCount)
                    {
                        int m = this.BSPPermutationRawDataMetaChunks[x].SubMeshInfo[y].IndiceStart;

                        bool dir = false;
                        short tempx;
                        short tempy;
                        short tempz;

                        do
                        {
                            // if (mode.EndOfIndices[x][j]>m+2){break;}
                            tempx = this.BSPPermutationRawDataMetaChunks[x].Indices[m];
                            tempy = this.BSPPermutationRawDataMetaChunks[x].Indices[m + 1];
                            tempz = this.BSPPermutationRawDataMetaChunks[x].Indices[m + 2];

                            if (tempx != tempy && tempx != tempz && tempy != tempz)
                            {
                                if (dir == false)
                                {
                                    shite[s] = vertcount + tempx;
                                    shite[s + 1] = vertcount + tempy;
                                    shite[s + 2] = vertcount + tempz;
                                    s += 3;

                                    dir = true;
                                }
                                else
                                {
                                    shite[s] = vertcount + tempx;
                                    shite[s + 1] = vertcount + tempz;
                                    shite[s + 2] = vertcount + tempy;
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
                               this.BSPPermutationRawDataMetaChunks[x].SubMeshInfo[y].IndiceStart +
                               this.BSPPermutationRawDataMetaChunks[x].SubMeshInfo[y].IndiceCount - 2);
                    }
                    else
                    {
                        for (int u = 0; u < this.BSPPermutationRawDataMetaChunks[x].SubMeshInfo[y].IndiceCount; u++)
                        {
                            shite[u] = vertcount +
                                        this.BSPPermutationRawDataMetaChunks[x].Indices[
                                            this.BSPPermutationRawDataMetaChunks[x].SubMeshInfo[y].IndiceStart + u];
                        }

                        s = this.BSPPermutationRawDataMetaChunks[x].SubMeshInfo[y].IndiceCount;
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

                vertcount += this.BSPPermutationRawDataMetaChunks[x].VerticeCount;
            }

            SW.Close();
            FS.Close();

            #endregion
        }

        /// <summary>
        /// The inject model.
        /// </summary>
        /// <param name="FilePath">The file path.</param>
        /// <param name="meta">The meta.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public Meta InjectModel(string FilePath, Meta meta)
        {
            LoadFromOBJ(FilePath);

            char[] crsr = new[] { 'c', 'r', 's', 'r' };
            char[] fklb = new[] { 'f', 'k', 'l', 'b' };

            for (int x = 0; x < this.BSPRawDataMetaChunks.Length; x++)
            {
                

                MemoryStream raw = new MemoryStream();
                BinaryWriter newraw = new BinaryWriter(raw);
                BinaryReader oldraw = new BinaryReader(meta.raw.rawChunks[x].MS);
                int newrawsize = 0;
                int rawchunkid = 0;

                #region Write Header

                oldraw.BaseStream.Position = 0;
                newraw.BaseStream.Position = 0;
                newraw.Write(oldraw.ReadBytes(this.BSPRawDataMetaChunks[x].HeaderSize));
                newrawsize += this.BSPRawDataMetaChunks[x].HeaderSize;

                #endregion

                #region Write Submesh Info

                newraw.BaseStream.Position = newrawsize;
                newraw.Write(crsr);
                newrawsize += 4;
                for (int y = 0; y < this.BSPRawDataMetaChunks[x].SubMeshInfo.Length; y++)
                {
                    oldraw.BaseStream.Position = this.BSPRawDataMetaChunks[x].HeaderSize +
                                                 this.BSPRawDataMetaChunks[x].RawDataChunkInfo[0].Offset + (y * 72);

                    newraw.BaseStream.Position = newrawsize + (y * 72);
                    newraw.Write(oldraw.ReadBytes(72));
                    newraw.BaseStream.Position = newrawsize + 4 + (y * 72);
                    newraw.Write((short)this.BSPRawDataMetaChunks[x].SubMeshInfo[y].ShaderNumber);
                    newraw.Write((short)this.BSPRawDataMetaChunks[x].SubMeshInfo[y].IndiceStart);
                    newraw.Write((short)this.BSPRawDataMetaChunks[x].SubMeshInfo[y].IndiceCount);
                    // newrawsize += 72;
                }

                this.BSPRawDataMetaChunks[x].RawDataChunkInfo[0].ChunkCount =
                    this.BSPRawDataMetaChunks[x].SubMeshInfo.Length;
                this.BSPRawDataMetaChunks[x].RawDataChunkInfo[0].Size =
                    this.BSPRawDataMetaChunks[x].SubMeshInfo.Length * 72;
                newrawsize += this.BSPRawDataMetaChunks[x].SubMeshInfo.Length * 72;
                // write count
                newraw.BaseStream.Position = 8;
                newraw.Write(this.BSPRawDataMetaChunks[x].SubMeshInfo.Length);

                #endregion

                #region Write Unknown

                rawchunkid = 1;

                while (newrawsize > 0)
                {
                    if (this.BSPRawDataMetaChunks[x].RawDataChunkInfo[rawchunkid].ChunkSize == 2)
                    {
                        break;
                    }

                    oldraw.BaseStream.Position = this.BSPRawDataMetaChunks[x].HeaderSize +
                                                 this.BSPRawDataMetaChunks[x].RawDataChunkInfo[rawchunkid].Offset;
                    newraw.BaseStream.Position = newrawsize;
                    newraw.Write(crsr);
                    newrawsize += 4;
                    newraw.Write(oldraw.ReadBytes(this.BSPRawDataMetaChunks[x].RawDataChunkInfo[rawchunkid].Size));
                    this.BSPRawDataMetaChunks[x].RawDataChunkInfo[rawchunkid].Offset = newrawsize -
                                                                                       this.BSPRawDataMetaChunks[x].
                                                                                           HeaderSize;
                    newrawsize += this.BSPRawDataMetaChunks[x].RawDataChunkInfo[rawchunkid].Size;
                    rawchunkid++;
                }

                #endregion

                #region Write Indices

                int indicechunkid = rawchunkid;
                newraw.BaseStream.Position = newrawsize;
                newraw.Write(crsr);
                newrawsize += 4;

                for (int y = 0; y < this.BSPRawDataMetaChunks[x].Indices.Length; y++)
                {
                    newraw.Write(this.BSPRawDataMetaChunks[x].Indices[y]);
                }

                this.BSPRawDataMetaChunks[x].RawDataChunkInfo[rawchunkid].Offset = newrawsize -
                                                                                   this.BSPRawDataMetaChunks[x].
                                                                                       HeaderSize;
                this.BSPRawDataMetaChunks[x].RawDataChunkInfo[rawchunkid].ChunkCount =
                    this.BSPRawDataMetaChunks[x].Indices.Length;
                this.BSPRawDataMetaChunks[x].RawDataChunkInfo[rawchunkid].Size =
                    this.BSPRawDataMetaChunks[x].Indices.Length * 2;
                newrawsize += this.BSPRawDataMetaChunks[x].Indices.Length * 2;
                // indice count
                newraw.BaseStream.Position = 40;
                newraw.Write((short)this.BSPRawDataMetaChunks[x].Indices.Length);
                rawchunkid++;

                #endregion

                #region Write Unknown

                // Pad to 16, after CRSR tag
                if ((newrawsize + 4) % 16 != 0)
                {
                    int pad = 16 - ((newrawsize + 4) % 16);
                    newraw.BaseStream.Position = newrawsize;
                    newraw.Write(new byte[pad]);
                    newrawsize += pad;
                }

                int verticechunkid = 0;
                while (newrawsize > 0)
                {
                    if (this.BSPRawDataMetaChunks[x].RawDataChunkInfo[rawchunkid].ChunkCount == 1)
                    {
                        verticechunkid = rawchunkid;
                        break;
                    }

                    oldraw.BaseStream.Position = this.BSPRawDataMetaChunks[x].HeaderSize +
                                                 this.BSPRawDataMetaChunks[x].RawDataChunkInfo[rawchunkid].Offset;
                    newraw.BaseStream.Position = newrawsize;
                    newraw.Write(crsr);
                    newrawsize += 4;
                    newraw.Write(oldraw.ReadBytes(this.BSPRawDataMetaChunks[x].RawDataChunkInfo[rawchunkid].Size));
                    this.BSPRawDataMetaChunks[x].RawDataChunkInfo[rawchunkid].Offset = newrawsize -
                                                                                       this.BSPRawDataMetaChunks[x].
                                                                                           HeaderSize;
                    newrawsize += this.BSPRawDataMetaChunks[x].RawDataChunkInfo[rawchunkid].Size;

                    rawchunkid++;
                }

                #endregion

                #region Write Vertices

                newraw.BaseStream.Position = newrawsize;
                newraw.Write(crsr);
                newrawsize += 4;
                this.BSPRawDataMetaChunks[x].RawDataChunkInfo[verticechunkid].ChunkSize = 12;
                newraw.Write(
                    new byte[
                        this.BSPRawDataMetaChunks[x].RawDataChunkInfo[verticechunkid].ChunkSize *
                        this.BSPRawDataMetaChunks[x].VerticeCount]);

                for (int y = 0; y < this.BSPRawDataMetaChunks[x].VerticeCount; y++)
                {
                    newraw.BaseStream.Position = newrawsize +
                                                 (y *
                                                  this.BSPRawDataMetaChunks[x].RawDataChunkInfo[verticechunkid].
                                                      ChunkSize);
                    float vx = this.BSPRawDataMetaChunks[x].Vertices[y].X;
                    float vy = this.BSPRawDataMetaChunks[x].Vertices[y].Y;
                    float vz = this.BSPRawDataMetaChunks[x].Vertices[y].Z;
                    newraw.Write(vx); // xxx.934xxx
                    newraw.Write(vy);
                    newraw.Write(vz);
                }

                this.BSPRawDataMetaChunks[x].RawDataChunkInfo[rawchunkid].Offset = newrawsize -
                                                                                   this.BSPRawDataMetaChunks[x].
                                                                                       HeaderSize;
                newrawsize += this.BSPRawDataMetaChunks[x].RawDataChunkInfo[verticechunkid].ChunkSize *
                              this.BSPRawDataMetaChunks[x].VerticeCount;
                this.BSPRawDataMetaChunks[x].RawDataChunkInfo[rawchunkid].Size =
                    this.BSPRawDataMetaChunks[x].RawDataChunkInfo[verticechunkid].ChunkSize *
                    this.BSPRawDataMetaChunks[x].VerticeCount;
                this.BSPRawDataMetaChunks[x].RawDataChunkInfo[rawchunkid].ChunkSize = 0;
                    // this.BSPRawDataMetaChunks[x].RawDataChunkInfo[verticechunkid].ChunkSize * this.BSPRawDataMetaChunks[x].VerticeCount; ;
                // newraw.BaseStream.Position = 100;
                // newraw.Write(this.BSPRawDataMetaChunks[x].VerticeCount);
                rawchunkid++;

                #endregion

                #region Write UVs

                int uvchunkid = verticechunkid + 1;
                newraw.BaseStream.Position = newrawsize;
                newraw.Write(crsr);
                newrawsize += 4;
                this.BSPRawDataMetaChunks[x].RawDataChunkInfo[rawchunkid].ChunkSize = 8;
                for (int y = 0; y < this.BSPRawDataMetaChunks[x].VerticeCount; y++)
                {
                    float u = this.BSPRawDataMetaChunks[x].UVs[y].X;
                    float v = this.BSPRawDataMetaChunks[x].UVs[y].Y;

                    newraw.Write(u);
                    newraw.Write(v);
                }

                this.BSPRawDataMetaChunks[x].RawDataChunkInfo[rawchunkid].Offset = newrawsize -
                                                                                   this.BSPRawDataMetaChunks[x].
                                                                                       HeaderSize;
                this.BSPRawDataMetaChunks[x].RawDataChunkInfo[rawchunkid].ChunkSize = 1;
                this.BSPRawDataMetaChunks[x].RawDataChunkInfo[rawchunkid].Size = 8 *
                                                                                 this.BSPRawDataMetaChunks[x].
                                                                                     VerticeCount;
                newrawsize += 8 * this.BSPRawDataMetaChunks[x].VerticeCount;
                rawchunkid++;

                #endregion

                #region Write Normals

                newraw.BaseStream.Position = newrawsize;
                newraw.Write(crsr);
                newrawsize += 4;

                this.BSPRawDataMetaChunks[x].RawDataChunkInfo[rawchunkid].ChunkSize = 12;
                for (int y = 0; y < this.BSPRawDataMetaChunks[x].VerticeCount; y++)
                {
                    int cn = ParsedModel.CompressNormal(this.BSPRawDataMetaChunks[x].Normals[y]);

                    // Binormals & Tangents should be recalculated here instead of using old values!!!
                    // int cb = Raw.ParsedModel.CompressNormal(this.BSPRawDataMetaChunks[x].Binormals[y]);
                    // int ct = Raw.ParsedModel.CompressNormal(this.BSPRawDataMetaChunks[x].Tangents[y]);
                    int cb = ParsedModel.CompressNormal(this.BSPRawDataMetaChunks[x].Normals[y]);
                    int ct = ParsedModel.CompressNormal(this.BSPRawDataMetaChunks[x].Normals[y]);
                    // oldraw.BaseStream.Position = newraw.BaseStream.Position;
                    // cn = oldraw.ReadInt32();
                    // cb = oldraw.ReadInt32();
                    // ct = oldraw.ReadInt32();
                    newraw.Write(cn);
                    newraw.Write(cb);
                    newraw.Write(ct);
                }

                this.BSPRawDataMetaChunks[x].RawDataChunkInfo[rawchunkid].Offset = newrawsize -
                                                                                   this.BSPRawDataMetaChunks[x].
                                                                                       HeaderSize;
                this.BSPRawDataMetaChunks[x].RawDataChunkInfo[rawchunkid].ChunkSize = 2;
                this.BSPRawDataMetaChunks[x].RawDataChunkInfo[rawchunkid].Size = 12 *
                                                                                 this.BSPRawDataMetaChunks[x].
                                                                                     VerticeCount;
                newrawsize += 12 * this.BSPRawDataMetaChunks[x].VerticeCount;
                rawchunkid++;

                #endregion

                #region Write Other Stuff Not Yet Implemented

                int tempCount = 0;
                while (rawchunkid < this.BSPRawDataMetaChunks[x].RawDataChunkInfo.Length)
                {
                    oldraw.BaseStream.Position = this.BSPRawDataMetaChunks[x].HeaderSize +
                                                 this.BSPRawDataMetaChunks[x].RawDataChunkInfo[rawchunkid].Offset;
                    newraw.BaseStream.Position = newrawsize;
                    newraw.Write(crsr);
                    newrawsize += 4;
                    newraw.Write(oldraw.ReadBytes(this.BSPRawDataMetaChunks[x].RawDataChunkInfo[rawchunkid].Size));
                    this.BSPRawDataMetaChunks[x].RawDataChunkInfo[rawchunkid].Offset = newrawsize -
                                                                                       this.BSPRawDataMetaChunks[x].
                                                                                           HeaderSize;
                    this.BSPRawDataMetaChunks[x].RawDataChunkInfo[rawchunkid].ChunkSize = 3 + tempCount;
                    newrawsize += this.BSPRawDataMetaChunks[x].RawDataChunkInfo[rawchunkid].Size;
                    rawchunkid++;
                    tempCount++;
                }

                #endregion

                // footer
                newraw.BaseStream.Position = newrawsize;
                newraw.Write(fklb);
                newrawsize += 4;
                // raw size
                int rawsize = newrawsize - this.BSPRawDataMetaChunks[x].HeaderSize - 4;
                newraw.BaseStream.Position = 4;
                newraw.Write(rawsize);
                meta.raw.rawChunks[x].size = newrawsize;
                meta.raw.rawChunks[x].MS = raw;

                
            }

            

            #region metachunks

            BinaryWriter BW = new BinaryWriter(meta.MS);

            for (int l = 0; l < this.BSPRawDataMetaChunks.Length; l++)
            {
                BW.BaseStream.Position = this.BSPRawDataMetaChunksOffset + (176 * l);
                short facecount = (short)this.BSPRawDataMetaChunks[l].FaceCount;
                BW.Write((short)this.BSPRawDataMetaChunks[l].VerticeCount);
                BW.Write(facecount);
                BW.BaseStream.Position = this.BSPRawDataMetaChunksOffset + (176 * l) + 52;
                BW.Write(meta.raw.rawChunks[l].size - this.BSPRawDataMetaChunks[l].HeaderSize - 4);

                for (int h = 0; h < this.BSPRawDataMetaChunks[l].RawDataChunkInfo.Length; h++)
                {
                    BW.BaseStream.Position = this.BSPRawDataMetaChunks[l].rawdatainfooffset + (h * 16) + 6;
                    BW.Write((short)this.BSPRawDataMetaChunks[l].RawDataChunkInfo[h].ChunkSize);
                    BW.Write(this.BSPRawDataMetaChunks[l].RawDataChunkInfo[h].Size);
                    BW.Write(this.BSPRawDataMetaChunks[l].RawDataChunkInfo[h].Offset);
                }
            }

            #endregion

            

            return meta;
        }

        /// <summary>
        /// The load from obj.
        /// </summary>
        /// <param name="FilePath">The file path.</param>
        /// <remarks></remarks>
        public void LoadFromOBJ(string FilePath)
        {
            if (FilePath[FilePath.Length - 1] != '\\')
            {
                FilePath += "\\";
            }

            

            FileStream FS = new FileStream(FilePath + this.Name + ".mtl", FileMode.Open);
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
                    if (MaterialNames.IndexOf(split[1]) == -1)
                    {
                        MaterialNames.Add(split[1]);
                    }
                }
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

            for (int x = 0; x < this.BSPRawDataMetaChunks.Length; x++)
            {
                #region Fields

                int verticecount = 0;
                int facecount = 0;
                List<Vector3> vertices = new List<Vector3>();
                List<Vector3> normals = new List<Vector3>();
                List<Vector2> uvs = new List<Vector2>();

                List<List<int>> faces = new List<List<int>>();
                List<List<int>> facesuv = new List<List<int>>();
                List<List<int>> facesnormal = new List<List<int>>();
                Hashtable Materials = new Hashtable();
                int groupcount = 0;

                #endregion

                FS = new FileStream(FilePath + this.Name + "[" + x + "].obj", FileMode.Open);
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
                            if ((faces.Count == 0) || (faces[faces.Count - 1].Count > 0) ||
                                (facesuv[facesuv.Count - 1].Count > 0) || (facesnormal[facesnormal.Count - 1].Count > 0))
                            {
                                List<int> templist = new List<int>();
                                List<int> templist2 = new List<int>();
                                List<int> templist3 = new List<int>();
                                faces.Add(templist);
                                facesuv.Add(templist2);
                                facesnormal.Add(templist3);
                                groupcount++;
                            }

                            break;

                            #endregion

                            #region Faces

                        case "f":
                            string[] split1 = tempstrings[1].Split('/');
                            string[] split2 = tempstrings[2].Split('/');
                            string[] split3 = tempstrings[3].Split('/');
                            int temp1 = int.Parse(split1[0]);
                            int temp2 = int.Parse(split2[0]);
                            int temp3 = int.Parse(split3[0]);
                            temp1--;
                            temp2--;
                            temp3--;
                            faces[groupcount - 1].Add(temp1);
                            faces[groupcount - 1].Add(temp2);
                            faces[groupcount - 1].Add(temp3);

                            temp1 = int.Parse(split1[1]);
                            temp2 = int.Parse(split2[1]);
                            temp3 = int.Parse(split3[1]);
                            temp1--;
                            temp2--;
                            temp3--;
                            facesuv[groupcount - 1].Add(temp1);
                            facesuv[groupcount - 1].Add(temp2);
                            facesuv[groupcount - 1].Add(temp3);

                            temp1 = int.Parse(split1[2]);
                            temp2 = int.Parse(split2[2]);
                            temp3 = int.Parse(split3[2]);
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

                // Here we have all the vertices/textures/normals loaded into groups
                ///     vertices / uvs     / normals       as Vector3
                ///     faces    / facesuv / facesnormal   as pointers to the above
                /// 
                /// 
                /// we need to output to (Vetcor3):
                ///     this.BSPRawDataMetaChunks[x].Vertices
                ///     this.BSPRawDataMetaChunks[x].UVs
                ///     this.BSPRawDataMetaChunks[x].Normals
                ///     

                Renderer temprender = new Renderer();
                Panel fakepanel = new Panel();
                temprender.CreateDevice(fakepanel);
                List<List<short>> Faces = new List<List<short>>();
                List<List<short>> Facesuv = new List<List<short>>();
                List<List<short>> Facesnormal = new List<List<short>>();
                List<short> newIndices = new List<short>();

                #region Submeshes

                this.BSPRawDataMetaChunks[x].SubMeshInfo = new ParsedModel.RawDataMetaChunk.ModelSubMeshInfo[groupcount];
                int totalindicecount = 0;
                this.BSPRawDataMetaChunks[x].Vertices.Clear();
                this.BSPRawDataMetaChunks[x].UVs.Clear();
                this.BSPRawDataMetaChunks[x].Normals.Clear();

                for (int y = 0; y < groupcount; y++)
                {
                    Application.DoEvents();
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
                                    newIndices.Add(Faces[i][j]);

                                    goto gohere;
                                }
                            }
                        }

                        gohere1:
                        this.BSPRawDataMetaChunks[x].Vertices.Add(vertices[faces[y][h]]);
                        this.BSPRawDataMetaChunks[x].UVs.Add(uvs[facesuv[y][h]]);
                        this.BSPRawDataMetaChunks[x].Normals.Add(normals[facesnormal[y][h]]);
                        newIndices.Add((short)(this.BSPRawDataMetaChunks[x].Vertices.Count - 1));

                        Faces[y].Add((short)(this.BSPRawDataMetaChunks[x].Vertices.Count - 1));
                        gohere:
                        ;
                    }

                    #region SubmeshInfo

                    ParsedModel.RawDataMetaChunk.ModelSubMeshInfo submesh =
                        new ParsedModel.RawDataMetaChunk.ModelSubMeshInfo();
                    submesh.IndiceStart = totalindicecount;
                    totalindicecount += faces[y].Count;
                    submesh.IndiceCount = faces[y].Count;
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

                    this.BSPRawDataMetaChunks[x].SubMeshInfo[y] = submesh;

                    #endregion
                }

                #endregion

                this.BSPRawDataMetaChunks[x].FaceCount = facecount / 3;

                int temp = 0;
                for (int i = 0; i < faces.Count; i++)
                {
                    this.BSPRawDataMetaChunks[x].SubMeshInfo[i].IndiceCount = faces[i].Count;
                    this.BSPRawDataMetaChunks[x].SubMeshInfo[i].IndiceStart = temp;
                    temp += faces[i].Count;
                }

                this.BSPRawDataMetaChunks[x].Indices = newIndices.ToArray();
                this.BSPRawDataMetaChunks[x].IndiceCount = this.BSPRawDataMetaChunks[x].Indices.Length;
                this.BSPRawDataMetaChunks[x].VerticeCount = this.BSPRawDataMetaChunks[x].Vertices.Count;

                #region Displays each mesh in a window. Debugging ONLY!

                /*****************************************
                Form tForm = new Form();
                temprender = new entity.Renderer.Renderer();
                fakepanel = new Panel();
                temprender.CreateDevice(fakepanel);
                fakepanel.Size = tForm.Size;
                tForm.Controls.Add(fakepanel);
                tForm.Show();

                temprender.device.VertexFormat = HaloBSPVertex.FVF;
                temprender.device.RenderState.CullMode = Cull.None;
                temprender.device.Transform.World = Matrix.Identity;
                temprender.device.SamplerState[0].AddressU = TextureAddress.Wrap;
                temprender.device.SamplerState[0].AddressV = TextureAddress.Wrap;
                temprender.device.RenderState.Lighting = true;
                temprender.device.RenderState.ZBufferEnable = true;
                temprender.device.RenderState.ZBufferWriteEnable = true;

                temprender.device.SetTexture(0, null);
                temprender.device.SetTexture(1, null);
                temprender.device.RenderState.AlphaBlendEnable = false;
                temprender.device.RenderState.AlphaTestEnable = false;
                //                    cam.Position = new Vector3(this.BSPRawDataMetaChunks[x].Vertices[0].X - 10, this.BSPRawDataMetaChunks[x].Vertices[0].Y - 10, this.BSPRawDataMetaChunks[x].Vertices[0].Z);
                //                    cam.LookAt = this.BSPRawDataMetaChunks[x].Vertices[0];
                Material WhiteMaterial = new Material();
                WhiteMaterial.Diffuse = System.Drawing.Color.White;
                WhiteMaterial.Ambient = System.Drawing.Color.White;
                Material BlackMaterial = new Material();
                BlackMaterial.Diffuse = System.Drawing.Color.Black;
                BlackMaterial.Ambient = System.Drawing.Color.Black;
                //Mesh m2 = Mesh.Box(temprender.device, 5, 5, 5);

                temprender.device.Transform.Projection = Matrix.PerspectiveFovLH((float)Math.PI / 4, tForm.Width / tForm.Height, 1f, 250f);
                //temprender.device.Transform.View = Matrix.LookAtLH(new Vector3(20, 20, 20), new Vector3(0, 0, 0), new Vector3(0, 0, 1));
                temprender.device.Transform.View = Matrix.LookAtLH(
                    new Vector3(this.BSPRawDataMetaChunks[x].Vertices[0].X - 5, this.BSPRawDataMetaChunks[x].Vertices[0].Y + 20, this.BSPRawDataMetaChunks[x].Vertices[0].Z - 5),
                    this.BSPRawDataMetaChunks[x].Vertices[0],
                    new Vector3(0, 0, 1));

                List<short> tInd = new List<short>(this.BSPPermutationRawDataMetaChunks[x].Indices);
                Mesh mesh = temprender.MakeMesh(this.BSPPermutationRawDataMetaChunks[x].Vertices, tInd, this.BSPPermutationRawDataMetaChunks[x].UVs);
                // While the form is still valid, render and process messages
                while (tForm.Created)
                {
                    temprender.device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, System.Drawing.Color.Black, 1.0f, 0);
                    temprender.BeginScene(System.Drawing.Color.Blue);
                    temprender.device.Transform.World = Matrix.Translation(0, 0, 0);
                    for (int ll = 0; ll < 1; ll++)
                    {
                        temprender.device.Material = WhiteMaterial;
                        temprender.device.RenderState.FillMode = FillMode.Solid;
                        mesh.DrawSubset(ll);
                        temprender.device.Material = BlackMaterial;
                        temprender.device.RenderState.FillMode = FillMode.WireFrame;
                        mesh.DrawSubset(ll);
                    }
                    //m2.DrawSubset(0);
                    temprender.EndScene();

                    Application.DoEvents();
                    GC.Collect(0);
                    GC.WaitForPendingFinalizers();
                    GC.Collect(0);
                }
                /********************************************/
                #endregion
            }

            #endregion
        }

        /// <summary>
        /// The load lightmaps.
        /// </summary>
        /// <remarks></remarks>
        public void LoadLightmaps()
        {
            if (map.BSP.sbsp[this.BspNumber].LightMap_TagNumber == -1)
            {
                LightMapBitmap = new Bitmap[this.BSPRawDataMetaChunks.Length];
                SceneryLightMapBitmap = new Bitmap[this.PermutationInfo.Length];
                return;
            }

            map.OpenMap(MapTypes.Internal);
            Meta m = new Meta(map);
            m.ReadMetaFromMap(map.BSP.sbsp[this.BspNumber].LightMap_TagNumber, false);

            ParsedBitmap pb = new ParsedBitmap(ref m, map);
            LightMapBitmap = new Bitmap[this.BSPRawDataMetaChunks.Length];
            for (int x = 0; x < this.BSPRawDataMetaChunks.Length; x++)
            {
                int visualchunk = map.BSP.sbsp[this.BspNumber].VisualChunk_Bitmap_Index[x];
                if (visualchunk == -1)
                {
                    LightMapBitmap[x] = null;
                    continue;
                }

                LightMapBitmap[x] = pb.FindChunkAndDecode(visualchunk, 0, 0, ref m, m.Map, x, this.BspNumber);
            }

            SceneryLightMapBitmap = new Bitmap[this.PermutationInfo.Length];
            for (int x = 0; x < this.PermutationInfo.Length; x++)
            {
                int visualchunk = map.BSP.sbsp[this.BspNumber].SceneryChunk_Bitmap_Index[x];
                if (visualchunk == -1)
                {
                    SceneryLightMapBitmap[x] = null;
                    continue;
                }

                SceneryLightMapBitmap[x] = pb.FindChunkAndDecode(
                    visualchunk, 0, 0, ref m, m.Map, -x - 1, this.BspNumber);
            }

            map.CloseMap();
        }

        /// <summary>
        /// The load model structure.
        /// </summary>
        /// <param name="meta">The meta.</param>
        /// <remarks></remarks>
        public void LoadModelStructure(ref Meta meta)
        {
            map.OpenMap(MapTypes.Internal);
            BinaryReader BR = new BinaryReader(meta.MS);
            switch (map.HaloVersion)
            {
                case HaloVersionEnum.Halo2:

                    // Loads the maps min & max boundries
                    float x1, x2, y1, y2, z1, z2;
                    // 68 = offset to World Boundries
                    BR.BaseStream.Position = 68;
                    x1 = BR.ReadSingle();
                    x2 = BR.ReadSingle();
                    y1 = BR.ReadSingle();
                    y2 = BR.ReadSingle();
                    z1 = BR.ReadSingle();
                    z2 = BR.ReadSingle();
                    minBoundries = new Vector3(x1, y1, z1);
                    maxBoundries = new Vector3(x2, y2, z2);


                    BR.BaseStream.Position = 172;
                    int tempc = BR.ReadInt32();
                    int tempr = BR.ReadInt32() - meta.magic - meta.offset;
                    BSPRawDataMetaChunks = new BSPRawDataMetaChunk[tempc];
                    BSPRawDataMetaChunksOffset = tempr;
                    for (int x = 0; x < tempc; x++)
                    {
                        BSPRawDataMetaChunks[x] = new BSPRawDataMetaChunk(tempr + (x * 176), x, ref meta, null);
                    }

                    

                    break;
                case HaloVersionEnum.HaloCE:
                case HaloVersionEnum.Halo1:
                    List<BSPRawDataMetaChunk> tempchunks = new List<BSPRawDataMetaChunk>();

                    BR.BaseStream.Position = map.BSP.sbsp[BspNumber].lightmapoffset + 248;
                    int facescount = BR.ReadInt32();
                    int facestranslation = BR.ReadInt32() - meta.magic - meta.offset;
                    Face[] faces = new Face[facescount];

                    BR.BaseStream.Position = facestranslation;
                    for (int x = 0; x < facescount; x++)
                    {
                        faces[x] = new Face();
                        faces[x].vertex0Index = BR.ReadInt16();
                        faces[x].vertex1Index = BR.ReadInt16();
                        faces[x].vertex2Index = BR.ReadInt16();
                    }

                    BR.BaseStream.Position = map.BSP.sbsp[BspNumber].lightmapoffset + 260;
                    int tempc2 = BR.ReadInt32();
                    int tempr2 = BR.ReadInt32() - meta.magic - meta.offset;

                    List<ShaderInfo> tempshad = new List<ShaderInfo>();
                    BSPRawDataMetaChunksOffset = tempr2;
                    for (int x = 0; x < tempc2; x++)
                    {
                        BR.BaseStream.Position = tempr2 + (32 * x) + 20;
                        int tempc22 = BR.ReadInt32();
                        int tempr22 = BR.ReadInt32() - meta.magic - meta.offset;
                        for (int xx = 0; xx < tempc22; xx++)
                        {
                            BSPRawDataMetaChunk tempraw = new BSPRawDataMetaChunk(
                                tempr22 + (256 * xx), tempchunks.Count, ref meta, faces);
                            tempchunks.Add(tempraw);
                            tempshad.Add(new ShaderInfo(tempraw.shadertagnumber, map));
                        }
                    }

                    this.Shaders.Shader = tempshad.ToArray();
                    this.BSPRawDataMetaChunks = tempchunks.ToArray();

                    break;
            }

            map.CloseMap();
        }

        /// <summary>
        /// The load permutations.
        /// </summary>
        /// <param name="meta">The meta.</param>
        /// <remarks></remarks>
        public void LoadPermutations(ref Meta meta)
        {
            map.OpenMap(MapTypes.Internal);
            BinaryReader BR = new BinaryReader(meta.MS);
            BR.BaseStream.Position = 328;
            int tempc = BR.ReadInt32();
            int tempr = BR.ReadInt32() - meta.magic - meta.offset;
            BSPPermutationRawDataMetaChunks = new BSPPermutationRawDataMetaChunk[tempc];
            for (int x = 0; x < tempc; x++)
            {
                BSPPermutationRawDataMetaChunks[x] = new BSPPermutationRawDataMetaChunk(tempr + (x * 200), x, ref meta);
            }

            BR.BaseStream.Position = 336;
            tempc = BR.ReadInt32();
            tempr = BR.ReadInt32() - meta.magic - meta.offset;
            PermutationInfo = new PermutationPlacement[tempc];
            for (int x = 0; x < tempc; x++)
            {
                BR.BaseStream.Position = tempr + (x * 88);
                PermutationInfo[x] = new PermutationPlacement(ref BR);
            }

            map.CloseMap();
            // MessageBox.Show("Test");
        }

        /// <summary>
        /// The load sky.
        /// </summary>
        /// <param name="meta">The meta.</param>
        /// <remarks></remarks>
        public void LoadSky(ref Meta meta)
        {
            map.OpenMap(MapTypes.Internal);

            map.BR.BaseStream.Position = map.MetaInfo.Offset[3] + 8;
            int tempc = map.BR.ReadInt32();
            int tempr = map.BR.ReadInt32() - map.SecondaryMagic;
            if (tempc == 0)
            {
                return;
            }

            map.BR.BaseStream.Position = tempr + 4;

            int tempident = map.Functions.ForMeta.FindMetaByID(map.BR.ReadInt32());

            if (tempident != -1)
            {
                sky = new Sky(tempident, map);

                map.BR.BaseStream.Position = map.MetaInfo.Offset[tempident] + 4;
                tempident = map.Functions.ForMeta.FindMetaByID(map.BR.ReadInt32());
                if (tempident != -1)
                {
                    Meta tempmeta = new Meta(map);
                    tempmeta.ReadMetaFromMap(tempident, false);
                    SkyBox = new ParsedModel(ref tempmeta);
                }
            }

            map.CloseMap();
        }

        /// <summary>
        /// The load unknowns.
        /// </summary>
        /// <param name="meta">The meta.</param>
        /// <remarks></remarks>
        public void LoadUnknowns(ref Meta meta)
        {
            map.OpenMap(MapTypes.Internal);
            BinaryReader BR = new BinaryReader(meta.MS);
            BR.BaseStream.Position = 580;
            int tempc = BR.ReadInt32();
            int tempr = BR.ReadInt32() - meta.magic - meta.offset;
            BR.BaseStream.Position = tempr + 16;
            tempc = BR.ReadInt32();
            tempr = BR.ReadInt32() - meta.magic - meta.offset;
            UnknownChunks = new UnknownChunk[tempc];
            for (int x = 0; x < tempc; x++)
            {
                UnknownChunks[x] = new UnknownChunk(tempr + (x * 44), x, ref meta);
            }

            map.CloseMap();
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
            foreach (IntPtr ip in bitmPtrs)
            {
                Marshal.FreeHGlobal(ip);
            }

            bitmPtrs.Clear();

            for (int i = 0; i < this.LightMapBitmap.Length; i++)
            {
                if (this.LightMapBitmap[i] != null)
                {
                    this.LightMapBitmap[i].Dispose();
                    this.LightMapBitmap[i] = null;
                }
            }

            for (int i = 0; i < this.SceneryLightMapBitmap.Length; i++)
            {
                if (this.SceneryLightMapBitmap[i] != null)
                {
                    this.SceneryLightMapBitmap[i].Dispose();
                }
            }

            this.BSPPermutationRawDataMetaChunks = null;
            this.PermutationInfo = null;
            this.BSPRawDataMetaChunks = null;
            this.Display = null;

            this.Shaders = null;
            this.SkyBox = null;
            this.Spawns = null;
            GC.SuppressFinalize(this);
        }

        #endregion

        #endregion

        /// <summary>
        /// The bsp collision.
        /// </summary>
        /// <remarks></remarks>
        public class BSPCollision
        {
            #region Constants and Fields

            /// <summary>
            /// The face reflexive count.
            /// </summary>
            public int FaceReflexiveCount;

            /// <summary>
            /// The face reflexive offset.
            /// </summary>
            public int FaceReflexiveOffset;

            /// <summary>
            /// The face reflexive translation.
            /// </summary>
            public int FaceReflexiveTranslation;

            /// <summary>
            /// The faces.
            /// </summary>
            public ushort[] Faces;

            /// <summary>
            /// The plane reflexive count.
            /// </summary>
            public int PlaneReflexiveCount;

            /// <summary>
            /// The plane reflexive offset.
            /// </summary>
            public int PlaneReflexiveOffset;

            /// <summary>
            /// The plane reflexive translation.
            /// </summary>
            public int PlaneReflexiveTranslation;

            /// <summary>
            /// The planes.
            /// </summary>
            public Vector4[] Planes;

            /// <summary>
            /// The surface reflexive count.
            /// </summary>
            public int SurfaceReflexiveCount;

            /// <summary>
            /// The surface reflexive translation.
            /// </summary>
            public int SurfaceReflexiveTranslation;

            /// <summary>
            /// The vertice reflexive count.
            /// </summary>
            public int VerticeReflexiveCount;

            /// <summary>
            /// The vertice reflexive offset.
            /// </summary>
            public int VerticeReflexiveOffset;

            /// <summary>
            /// The vertice reflexive translation.
            /// </summary>
            public int VerticeReflexiveTranslation;

            /// <summary>
            /// The vertices.
            /// </summary>
            public Vector3[] Vertices;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            /// Initializes a new instance of the <see cref="BSPCollision"/> class.
            /// </summary>
            /// <param name="meta">The meta.</param>
            /// <remarks></remarks>
            public BSPCollision(Meta meta)
            {
                BinaryReader BR = new BinaryReader(meta.MS);
                if (meta.Map.HaloVersion == HaloVersionEnum.Halo2)
                {
                    BR.BaseStream.Position = 36;

                    int tempc = BR.ReadInt32();
                    int tempr = BR.ReadInt32() - meta.magic - meta.offset;
                    BR.BaseStream.Position = tempr + 40;
                    SurfaceReflexiveCount = BR.ReadInt32();
                    SurfaceReflexiveTranslation = BR.ReadInt32() - meta.magic - meta.offset;

                    // planes
                    PlaneReflexiveOffset = tempr + 8;
                    BR.BaseStream.Position = PlaneReflexiveOffset;
                    PlaneReflexiveCount = BR.ReadInt32();
                    PlaneReflexiveTranslation = BR.ReadInt32() - meta.magic - meta.offset;
                    Planes = new Vector4[PlaneReflexiveCount];
                    for (int x = 0; x < FaceReflexiveCount; x++)
                    {
                        Planes[x].X = BR.ReadSingle();
                        Planes[x].Y = BR.ReadSingle();
                        Planes[x].Z = BR.ReadSingle();
                        Planes[x].W = BR.ReadSingle();
                    }

                    // faces
                    FaceReflexiveOffset = tempr + 48;
                    BR.BaseStream.Position = FaceReflexiveOffset;
                    FaceReflexiveCount = BR.ReadInt32();
                    FaceReflexiveTranslation = BR.ReadInt32() - meta.magic - meta.offset;

                    Faces = new ushort[FaceReflexiveCount * 3];
                    for (int x = 0; x < FaceReflexiveCount; x++)
                    {
                        BR.BaseStream.Position = FaceReflexiveTranslation + (x * 12);
                        Faces[x * 3] = BR.ReadUInt16();
                        Faces[(x * 3) + 1] = BR.ReadUInt16();
                        Faces[(x * 3) + 2] = Faces[(x * 3) + 1];
                    }

                    // vertices
                    VerticeReflexiveOffset = tempr + 56;
                    BR.BaseStream.Position = VerticeReflexiveOffset;
                    VerticeReflexiveCount = BR.ReadInt32();
                    VerticeReflexiveTranslation = BR.ReadInt32() - meta.magic - meta.offset;
                    Vertices = new Vector3[VerticeReflexiveCount];
                    for (int x = 0; x < VerticeReflexiveCount; x++)
                    {
                        BR.BaseStream.Position = VerticeReflexiveTranslation + (x * 16);
                        Vertices[x].X = BR.ReadSingle();
                        Vertices[x].Y = BR.ReadSingle();
                        Vertices[x].Z = BR.ReadSingle();
                    }
                }
                else
                {
                    BR.BaseStream.Position = 200;
                    int tempc = BR.ReadInt32();
                    int tempr = BR.ReadInt32() - meta.magic - meta.offset;

                    BR.BaseStream.Position = tempr + 40;
                    SurfaceReflexiveCount = BR.ReadInt32();
                    SurfaceReflexiveTranslation = BR.ReadInt32() - meta.magic - meta.offset;

                    // planes
                    PlaneReflexiveOffset = tempr + 12;
                    BR.BaseStream.Position = PlaneReflexiveOffset;
                    PlaneReflexiveCount = BR.ReadInt32();
                    PlaneReflexiveTranslation = BR.ReadInt32() - meta.magic - meta.offset;
                    Planes = new Vector4[PlaneReflexiveCount];
                    for (int x = 0; x < FaceReflexiveCount; x++)
                    {
                        Planes[x].X = BR.ReadSingle();
                        Planes[x].Y = BR.ReadSingle();
                        Planes[x].Z = BR.ReadSingle();
                        Planes[x].W = BR.ReadSingle();
                    }

                    // faces
                    FaceReflexiveOffset = tempr + 72;
                    BR.BaseStream.Position = FaceReflexiveOffset;
                    FaceReflexiveCount = BR.ReadInt32();
                    FaceReflexiveTranslation = BR.ReadInt32() - meta.magic - meta.offset;

                    Faces = new ushort[FaceReflexiveCount * 3];
                    for (int x = 0; x < FaceReflexiveCount; x++)
                    {
                        BR.BaseStream.Position = FaceReflexiveTranslation + (x * 24);

                        Faces[x * 3] = (ushort)BR.ReadInt32();
                        Faces[(x * 3) + 1] = (ushort)BR.ReadInt32();
                        Faces[(x * 3) + 2] = Faces[(x * 3) + 1];
                    }

                    // vertices
                    VerticeReflexiveOffset = tempr + 84;
                    BR.BaseStream.Position = VerticeReflexiveOffset;
                    VerticeReflexiveCount = BR.ReadInt32();
                    VerticeReflexiveTranslation = BR.ReadInt32() - meta.magic - meta.offset;
                    Vertices = new Vector3[VerticeReflexiveCount];
                    for (int x = 0; x < VerticeReflexiveCount; x++)
                    {
                        BR.BaseStream.Position = VerticeReflexiveTranslation + (x * 16);
                        Vertices[x].X = BR.ReadSingle();
                        Vertices[x].Y = BR.ReadSingle();
                        Vertices[x].Z = BR.ReadSingle();
                    }
                }
            }

            #endregion

            #region Public Methods

            /// <summary>
            /// The extract collison mesh.
            /// </summary>
            /// <param name="FilePath">The file path.</param>
            /// <remarks></remarks>
            public void ExtractCollisonMesh(string FilePath)
            {
                FileStream FS = new FileStream(FilePath, FileMode.Create);
                StreamWriter SW = new StreamWriter(FS);
                SW.WriteLine("# ------------------------------------");
                SW.WriteLine("# Halo 2 BSP Collision Mesh - Extracted with Entity");
                SW.WriteLine("# ------------------------------------");
                for (int x = 0; x < Vertices.Length; x++)
                {
                    string temps = "v " + this.Vertices[x].X + " " + this.Vertices[x].Y + " " + this.Vertices[x].Z;
                    SW.WriteLine(temps);
                }

                for (int x = 0; x < Faces.Length; x += 3)
                {
                    string temps = "f " + (Faces[x] + 1) + " " + (Faces[x + 1] + 1) + " " + (Faces[x + 2] + 1);
                    SW.WriteLine(temps);
                }

                SW.Close();
                FS.Close();
            }

            /// <summary>
            /// The inject collison mesh.
            /// </summary>
            /// <param name="FilePath">The file path.</param>
            /// <param name="meta">The meta.</param>
            /// <remarks></remarks>
            public void InjectCollisonMesh(string FilePath, Meta meta)
            {
                FileStream FS = new FileStream(FilePath, FileMode.Open);
                StreamReader SR = new StreamReader(FS);
                string temps = string.Empty;
                int verticecount = 0;
                int facecount = 0;
                List<Vector3> vertices = new List<Vector3>();
                List<ushort> faces = new List<ushort>();
                do
                {
                    temps = SR.ReadLine();
                    if (temps == null)
                    {
                        continue;
                    }

                    char[] search = new[] { ',', ' ' };
                    string[] tempstrings = temps.Split(search, StringSplitOptions.RemoveEmptyEntries);
                    switch (tempstrings[0])
                    {
                        case "v":
                            Vector3 tempv = new Vector3();
                            tempv.X = float.Parse(tempstrings[1]);
                            tempv.Y = float.Parse(tempstrings[2]);
                            tempv.Z = float.Parse(tempstrings[3]);
                            vertices.Add(tempv);
                            verticecount++;

                            break;
                        case "f":
                            string[] split1 = tempstrings[1].Split('/');
                            string[] split2 = tempstrings[2].Split('/');
                            string[] split3 = tempstrings[3].Split('/');
                            ushort temp1 = ushort.Parse(split1[0]);
                            ushort temp2 = ushort.Parse(split2[0]);
                            ushort temp3 = ushort.Parse(split3[0]);
                            temp1--;
                            temp2--;
                            temp3--;
                            faces.Add(temp1);
                            faces.Add(temp2);
                            faces.Add(temp3);
                            facecount += 3;
                            break;
                    }
                }
                while (temps != null);
                SR.Close();
                FS.Close();
                if (this.Vertices.Length >= verticecount && this.Faces.Length >= facecount)
                {
                    meta.Map.OpenMap(MapTypes.Internal);
                    meta.Map.BW.BaseStream.Position = meta.offset + this.VerticeReflexiveOffset;
                    meta.Map.BW.Write(verticecount);
                    for (int x = 0; x < verticecount; x++)
                    {
                        meta.Map.BW.BaseStream.Position = meta.offset + this.VerticeReflexiveTranslation + (x * 16);
                        meta.Map.BW.Write(vertices[x].X);
                        meta.Map.BW.Write(vertices[x].Y);
                        meta.Map.BW.Write(vertices[x].Z);
                    }

                    meta.Map.BW.BaseStream.Position = meta.offset + this.FaceReflexiveOffset;
                    int tempint = facecount / 3;
                    meta.Map.BW.Write(tempint);
                    for (int x = 0; x < facecount; x += 3)
                    {
                        int fuck = x / 3;
                        if (meta.Map.HaloVersion == HaloVersionEnum.Halo2 ||
                            meta.Map.HaloVersion == HaloVersionEnum.Halo2Vista)
                        {
                            meta.Map.BW.BaseStream.Position = meta.offset + this.FaceReflexiveTranslation + (fuck * 12);
                            meta.Map.BW.Write(faces[x]);

                            meta.Map.BW.Write(faces[x + 1]);
                        }
                        else
                        {
                            meta.Map.BW.BaseStream.Position = meta.offset + this.FaceReflexiveTranslation + (fuck * 24);
                            meta.Map.BW.Write((int)faces[x]);

                            meta.Map.BW.Write((int)faces[x + 1]);
                        }
                    }

                    meta.Map.CloseMap();
                }
            }

            #endregion
        }

        /// <summary>
        /// The bsp displayed info.
        /// </summary>
        /// <remarks></remarks>
        public class BSPDisplayedInfo : ParsedModel.DisplayedInfo
        {
            // private Mesh[] mesh;
            #region Constants and Fields

            /// <summary>
            /// The permindex buffer.
            /// </summary>
            private IndexBuffer[] permindexBuffer;

            /// <summary>
            /// The permvertex buffer.
            /// </summary>
            private VertexBuffer[] permvertexBuffer;

            /// <summary>
            /// The watermatrix.
            /// </summary>
            private Matrix watermatrix = Matrix.Identity;

            #endregion

            #region Public Methods

            /// <summary>
            /// The create index buffers.
            /// </summary>
            /// <param name="device">The device.</param>
            /// <param name="bsp">The bsp.</param>
            /// <remarks></remarks>
            public static void CreateIndexBuffers(ref Device device, ref BSPModel bsp)
            {
                bsp.Display.indexBuffer = new IndexBuffer[bsp.BSPRawDataMetaChunks.Length];
                for (int x = 0; x < bsp.BSPRawDataMetaChunks.Length; x++)
                {
                    if (bsp.BSPRawDataMetaChunks[x].RawDataChunkInfo.Length == 0)
                    {
                        continue;
                    }

                    bsp.Display.indexBuffer[x] = new IndexBuffer(
                        typeof(short), bsp.BSPRawDataMetaChunks[x].IndiceCount, device, Usage.WriteOnly, Pool.Managed);
                    IndexBuffer ib = bsp.Display.indexBuffer[x];
                    ib.SetData(bsp.BSPRawDataMetaChunks[x].Indices, 0, LockFlags.None);
                    ib.Unlock();
                }

                bsp.Display.permindexBuffer = new IndexBuffer[bsp.BSPPermutationRawDataMetaChunks.Length];
                for (int x = 0; x < bsp.BSPPermutationRawDataMetaChunks.Length; x++)
                {
                    if (bsp.BSPPermutationRawDataMetaChunks[x].RawDataChunkInfo.Length == 0)
                    {
                        continue;
                    }

                    bsp.Display.permindexBuffer[x] = new IndexBuffer(
                        typeof(short), 
                        bsp.BSPPermutationRawDataMetaChunks[x].IndiceCount, 
                        device, 
                        Usage.WriteOnly, 
                        Pool.Managed);
                    IndexBuffer ib = bsp.Display.permindexBuffer[x];
                    ib.SetData(bsp.BSPPermutationRawDataMetaChunks[x].Indices, 0, LockFlags.None);
                    ib.Unlock();
                }
            }

            /// <summary>
            /// The create vertex buffers.
            /// </summary>
            /// <param name="device">The device.</param>
            /// <param name="bsp">The bsp.</param>
            /// <remarks></remarks>
            public static void CreateVertexBuffers(ref Device device, ref BSPModel bsp)
            {
                bsp.Display.vertexBuffer = new VertexBuffer[bsp.BSPRawDataMetaChunks.Length];
                for (int x = 0; x < bsp.BSPRawDataMetaChunks.Length; x++)
                {
                    int rawindex = x;
                    if (bsp.BSPRawDataMetaChunks[rawindex].RawDataChunkInfo.Length == 0)
                    {
                        continue;
                    }

                    bsp.Display.vertexBuffer[rawindex] = new VertexBuffer(
                        typeof(HaloBSPVertex), 
                        bsp.BSPRawDataMetaChunks[rawindex].VerticeCount, 
                        device, 
                        Usage.WriteOnly, 
                        HaloBSPVertex.FVF, 
                        Pool.Managed);
                    HaloBSPVertex[] verts = (HaloBSPVertex[])bsp.Display.vertexBuffer[rawindex].Lock(0, 0);
                        // Lock the buffer (which will return our structs)
                    for (int i = 0; i < bsp.BSPRawDataMetaChunks[rawindex].VerticeCount; i++)
                    {
                        verts[i].Position = new Vector3(
                            bsp.BSPRawDataMetaChunks[rawindex].Vertices[i].X, 
                            bsp.BSPRawDataMetaChunks[rawindex].Vertices[i].Y, 
                            bsp.BSPRawDataMetaChunks[rawindex].Vertices[i].Z);
                        verts[i].Tu0 = bsp.BSPRawDataMetaChunks[rawindex].UVs[i].X;
                        verts[i].Tv0 = bsp.BSPRawDataMetaChunks[rawindex].UVs[i].Y;
                        verts[i].Normal = bsp.BSPRawDataMetaChunks[rawindex].Normals[i];

                        // verts[i].specular = 1;
                        verts[i].diffuse = 1;

                        verts[i].Tu1 = verts[i].Tu0;
                        verts[i].Tv1 = verts[i].Tv0;
                        if (bsp.BSPRawDataMetaChunks[rawindex].LightMapUVs.Count != 0)
                        {
                            verts[i].Tu2 = bsp.BSPRawDataMetaChunks[rawindex].LightMapUVs[i].X;
                            verts[i].Tv2 = bsp.BSPRawDataMetaChunks[rawindex].LightMapUVs[i].Y;
                        }
                        else
                        {
                            verts[i].Tu2 = verts[i].Tu0;
                            verts[i].Tv2 = verts[i].Tv0;
                        }

                        verts[i].Tu3 = verts[i].Tu0;
                        verts[i].Tv3 = verts[i].Tv0;
                    }

                    bsp.Display.vertexBuffer[rawindex].Unlock();
                }

                bsp.Display.permvertexBuffer = new VertexBuffer[bsp.BSPPermutationRawDataMetaChunks.Length];
                for (int x = 0; x < bsp.BSPPermutationRawDataMetaChunks.Length; x++)
                {
                    int rawindex = x;
                    if (bsp.BSPPermutationRawDataMetaChunks[rawindex].RawDataChunkInfo.Length == 0)
                    {
                        continue;
                    }

                    bsp.Display.permvertexBuffer[rawindex] = new VertexBuffer(
                        typeof(HaloBSPVertex), 
                        bsp.BSPPermutationRawDataMetaChunks[rawindex].VerticeCount, 
                        device, 
                        Usage.WriteOnly, 
                        HaloBSPVertex.FVF, 
                        Pool.Managed);
                    HaloBSPVertex[] verts = (HaloBSPVertex[])bsp.Display.permvertexBuffer[rawindex].Lock(0, 0);
                        // Lock the buffer (which will return our structs)
                    for (int i = 0; i < bsp.BSPPermutationRawDataMetaChunks[rawindex].VerticeCount; i++)
                    {
                        verts[i].Position = new Vector3(
                            bsp.BSPPermutationRawDataMetaChunks[rawindex].Vertices[i].X, 
                            bsp.BSPPermutationRawDataMetaChunks[rawindex].Vertices[i].Y, 
                            bsp.BSPPermutationRawDataMetaChunks[rawindex].Vertices[i].Z);
                        verts[i].Tu0 = bsp.BSPPermutationRawDataMetaChunks[rawindex].UVs[i].X;
                        verts[i].Tv0 = bsp.BSPPermutationRawDataMetaChunks[rawindex].UVs[i].Y;
                        verts[i].Normal = bsp.BSPPermutationRawDataMetaChunks[rawindex].Normals[i];

                        // verts[i].specular = 1;
                        verts[i].diffuse = 1;
                        verts[i].Tu1 = verts[i].Tu0;
                        verts[i].Tv1 = verts[i].Tv0;
                        if (bsp.BSPPermutationRawDataMetaChunks[rawindex].LightMapUVs.Count != 0)
                        {
                            verts[i].Tu2 = bsp.BSPPermutationRawDataMetaChunks[rawindex].LightMapUVs[i].X;
                            verts[i].Tv2 = bsp.BSPPermutationRawDataMetaChunks[rawindex].LightMapUVs[i].Y;
                        }
                        else
                        {
                            verts[i].Tu2 = verts[i].Tu0;
                            verts[i].Tv2 = verts[i].Tv0;
                        }

                        verts[i].Tu3 = verts[i].Tu0;
                        verts[i].Tv3 = verts[i].Tv0;
                    }

                    bsp.Display.permvertexBuffer[rawindex].Unlock();
                }
            }

            /// <summary>
            /// The draw.
            /// </summary>
            /// <param name="device">The device.</param>
            /// <param name="bsp">The bsp.</param>
            /// <param name="Textured">The textured.</param>
            /// <param name="cam">The cam.</param>
            /// <param name="shaderx">The shaderx.</param>
            /// <remarks></remarks>
            public static void Draw(
                ref Device device, ref BSPModel bsp, bool Textured, ref Camera2 cam, DXShader shaderx)
            {

                Optimization opt = new Optimization(device);

                int count = 0;
                for (int x = 0; x < bsp.BSPRawDataMetaChunks.Length; x++)
                {
                    // Check if we are to render chunk
                    if (!bsp.BSPRawDataMetaChunks[x].render)
                        continue;

                    int rawindex = x;
                    // FaceCount is set to 0 when BSP sections are unselected
                    if (bsp.BSPRawDataMetaChunks[rawindex].RawDataChunkInfo.Length == 0 ||
                        bsp.BSPRawDataMetaChunks[rawindex].FaceCount == 0)
                    {
                        continue;
                    }

                    if (bsp.cameraCulling && !opt.IsInViewFrustum(bsp.BSPRawDataMetaChunks[rawindex]))
                        continue;

                    device.SetStreamSource(0, bsp.Display.vertexBuffer[rawindex], 0);
                    device.VertexFormat = HaloBSPVertex.FVF;
                    device.Indices = bsp.Display.indexBuffer[rawindex];
                    for (int xx = 0; xx < bsp.BSPRawDataMetaChunks[rawindex].SubMeshInfo.Length; xx++)
                    {
                        ResetTextureStates(ref device);
                        int tempshade = bsp.BSPRawDataMetaChunks[rawindex].SubMeshInfo[xx].ShaderNumber;

                        #region AlphaBlending

                        Renderer.SetAlphaBlending(bsp.Shaders.Shader[tempshade].Alpha, ref device);

                        #endregion

                        #region ChooseTexture

                        if (Textured)
                        {
                            if (bsp.Shaders.Shader[tempshade].MainName.IndexOf("water!") != -1)
                            {
                                for (int u = 0; u < bsp.Shaders.Shader[tempshade].BitmapTextures.Length; u++)
                                {
                                    if (bsp.Shaders.Shader[tempshade].BitmapNames[u].IndexOf("reflection") != -1 ||
                                        bsp.Shaders.Shader[tempshade].BitmapNames[u].IndexOf("mask") != -1)
                                    {
                                        device.SetTexture(0, bsp.Shaders.Shader[tempshade].BitmapTextures[u]);

                                        if (bsp.Shaders.Shader[tempshade].MainName.IndexOf("mip") != -1)
                                        {
                                            device.SetTexture(0, bsp.Shaders.Shader[tempshade].BitmapTextures[u]);
                                            device.SetTexture(1, bsp.Shaders.Shader[tempshade].BitmapTextures[u]);
                                        }
                                        else
                                        {
                                            device.SetTexture(1, bsp.Shaders.Shader[tempshade].MainTexture);
                                        }

                                        device.TextureState[0].ColorOperation = TextureOperation.BumpEnvironmentMap;
                                        device.TextureState[0].ColorArgument1 = TextureArgument.TextureColor;
                                        device.TextureState[0].ColorArgument2 = TextureArgument.Current;
                                        float r = 0.07F;
                                        device.TextureState[0].BumpEnvironmentMaterial00 = r *
                                                                                           (float)
                                                                                           Math.Cos(
                                                                                               (Environment.TickCount /
                                                                                                4) * 11.0F);
                                        device.TextureState[0].BumpEnvironmentMaterial01 = -r *
                                                                                           (float)
                                                                                           Math.Sin(
                                                                                               (Environment.TickCount /
                                                                                                4) * 11.0F);
                                        device.TextureState[0].BumpEnvironmentMaterial10 = r *
                                                                                           (float)
                                                                                           Math.Sin(
                                                                                               (Environment.TickCount /
                                                                                                4) * 11.0F);
                                        device.TextureState[0].BumpEnvironmentMaterial11 = r *
                                                                                           (float)
                                                                                           Math.Cos(
                                                                                               (Environment.TickCount /
                                                                                                4) * 11.0F);

                                        device.TextureState[1].ColorOperation = TextureOperation.Modulate;
                                        device.TextureState[1].ColorArgument1 = TextureArgument.TextureColor;
                                        device.TextureState[1].ColorArgument2 = TextureArgument.Current;

                                        if (bsp.LightMapTexture[x] != null &&
                                            bsp.BSPRawDataMetaChunks[x].LightMapUVs.Count != 0)
                                        {
                                            // device.SetTexture(2, bsp.LightMapTexture[x]);
                                            // device.TextureState[2].ColorOperation = TextureOperation.Modulate2X;
                                            // device.TextureState[2].ColorArgument1 = TextureArgument.TextureColor;
                                            // device.TextureState[2].ColorArgument2 = TextureArgument.Current;
                                        }

                                        break;
                                    }

                                    if (u == bsp.Shaders.Shader[tempshade].BitmapTextures.Length - 1)
                                    {
                                        device.SetTexture(0, bsp.Shaders.Shader[tempshade].MainTexture);
                                        device.TextureState[0].ColorOperation = TextureOperation.Modulate;
                                        device.TextureState[0].ColorArgument1 = TextureArgument.TextureColor;
                                        device.TextureState[0].ColorArgument2 = TextureArgument.Current;
                                    }
                                }
                            }
                            else if (bsp.Shaders.Shader[tempshade].MainName.IndexOf("ground!") != -1)
                            {
                                if (bsp.Shaders.Shader[tempshade].MainName.IndexOf("unwrapped") != -1)
                                {
                                    // device.SamplerState[0].AddressU = TextureAddress.Clamp;
                                    // device.SamplerState[0].AddressV = TextureAddress.Clamp;
                                }

                                device.SetTexture(0, bsp.Shaders.Shader[tempshade].MainTexture);
                                device.TextureState[0].ColorOperation = TextureOperation.Modulate;
                                device.TextureState[0].ColorArgument1 = TextureArgument.TextureColor;
                                device.TextureState[0].ColorArgument2 = TextureArgument.Current;
                                device.TextureState[0].AlphaOperation = TextureOperation.SelectArg1;
                                device.TextureState[0].AlphaArgument1 = TextureArgument.TextureColor;
                                // device.TextureState[0].ResultArgument = TextureArgument.Current;
                                device.TextureState[1].TextureTransform = TextureTransform.Count2;
                                device.Transform.Texture1 =
                                    Matrix.Scaling(
                                        bsp.Shaders.Shader[tempshade].primarydetailuscale, 
                                        bsp.Shaders.Shader[tempshade].primarydetailvscale, 
                                        bsp.Shaders.Shader[tempshade].primarydetailwscale);
                                // device.Transform.Texture1 = Matrix.Scaling(bsp.Shaders.Shader[tempshade].secondarydetailuscale, bsp.Shaders.Shader[tempshade].secondarydetailvscale, bsp.Shaders.Shader[tempshade].secondarydetailwscale);
                                device.SetTexture(1, bsp.Shaders.Shader[tempshade].BitmapTextures[2]);
                                device.TextureState[1].ColorOperation = TextureOperation.Modulate2X;
                                device.TextureState[1].ColorArgument1 = TextureArgument.TextureColor;
                                device.TextureState[1].ColorArgument2 = TextureArgument.Current;
                                device.TextureState[1].AlphaOperation = TextureOperation.Add;
                                device.TextureState[1].AlphaArgument1 = TextureArgument.Current;

                                device.TextureState[2].TextureTransform = TextureTransform.Count2;
                                device.Transform.Texture2 =
                                    Matrix.Scaling(
                                        bsp.Shaders.Shader[tempshade].secondarydetailuscale, 
                                        bsp.Shaders.Shader[tempshade].secondarydetailvscale, 
                                        bsp.Shaders.Shader[tempshade].secondarydetailwscale);
                                device.SetTexture(2, bsp.Shaders.Shader[tempshade].BitmapTextures[3]);
                                device.TextureState[2].ColorOperation = TextureOperation.Modulate2X;
                                device.TextureState[2].ColorArgument1 = TextureArgument.TextureColor;
                                device.TextureState[2].ColorArgument2 = TextureArgument.Current;
                                device.TextureState[2].TextureCoordinateIndex = 3;

                                if (bsp.LightMapTexture[x] != null && bsp.BSPRawDataMetaChunks[x].LightMapUVs.Count != 0)
                                {
                                    device.SetTexture(3, bsp.LightMapTexture[x]);
                                    device.TextureState[3].ColorOperation = TextureOperation.Modulate2X;
                                    device.TextureState[3].ColorArgument1 = TextureArgument.TextureColor;
                                    device.TextureState[3].ColorArgument2 = TextureArgument.Current;
                                    device.TextureState[3].TextureCoordinateIndex = 2;
                                }
                            }
                            else
                            {
                                device.SetTexture(0, bsp.Shaders.Shader[tempshade].MainTexture);

                                device.TextureState[0].ColorOperation = TextureOperation.Modulate;
                                device.TextureState[0].ColorArgument1 = TextureArgument.TextureColor;
                                device.TextureState[0].ColorArgument2 = TextureArgument.Current;
                                device.TextureState[0].AlphaOperation = TextureOperation.BlendTextureAlpha;
                                device.TextureState[0].AlphaArgument1 = TextureArgument.TextureColor;
                                device.TextureState[0].AlphaArgument2 = TextureArgument.Diffuse;

                                /*
                                            if (bsp.Shaders.Shader[tempshade].microdetailName != null)
                                            {

                                                device.TextureState[1].TextureTransform = TextureTransform.Count2;
                                                device.Transform.Texture1 = Matrix.Scaling(bsp.Shaders.Shader[tempshade].microdetailuscale, bsp.Shaders.Shader[tempshade].microdetailvscale, bsp.Shaders.Shader[tempshade].microdetailwscale);
                                                device.SetTexture(1, bsp.Shaders.Shader[tempshade].microdetailTexture);
                                                device.TextureState[1].ColorOperation = TextureOperation.Modulate2X;
                                                device.TextureState[1].ColorArgument1 = TextureArgument.TextureColor;
                                                device.TextureState[1].ColorArgument2 = TextureArgument.Current;
                                                device.TextureState[1].AlphaOperation = TextureOperation.BlendCurrentAlpha;
                                                device.TextureState[1].AlphaArgument1 = TextureArgument.Current;
                                            }

                                          
                                if (bsp.Shaders.Shader[tempshade].primarydetailTexture != null)
                              {

                                  device.TextureState[1].TextureTransform = TextureTransform.Count2;
                                  device.Transform.Texture1 = Matrix.Scaling(bsp.Shaders.Shader[tempshade].primarydetailuscale, bsp.Shaders.Shader[tempshade].primarydetailvscale, bsp.Shaders.Shader[tempshade].primarydetailwscale);
                               device.SetTexture(1, bsp.Shaders.Shader[tempshade].primarydetailTexture);
                               device.TextureState[1].ColorOperation = TextureOperation.Lerp;
                               device.TextureState[1].ColorArgument1 = TextureArgument.TextureColor;
                                  device.TextureState[1].ColorArgument2 = TextureArgument.Current;
                                 device.TextureState[1].AlphaOperation = TextureOperation.Modulate;
                                 device.TextureState[1].AlphaArgument1 = TextureArgument.TextureColor;
                              }
                           
                                if (bsp.Shaders.Shader[tempshade].secondarydetailTexture != null)
                                {
                      
                                    device.TextureState[2].TextureTransform = TextureTransform.Count2;
                                    device.Transform.Texture2 = Matrix.Scaling(bsp.Shaders.Shader[tempshade].secondarydetailuscale, bsp.Shaders.Shader[tempshade].secondarydetailvscale, bsp.Shaders.Shader[tempshade].secondarydetailwscale);
                                    device.SetTexture(2, bsp.Shaders.Shader[tempshade].secondarydetailTexture);
                                    device.TextureState[2].ColorOperation = TextureOperation.Lerp;
                                    device.TextureState[2].ColorArgument1 = TextureArgument.TextureColor;
                                    device.TextureState[2].ColorArgument2 = TextureArgument.Current;
                                    device.TextureState[2].TextureCoordinateIndex = 3;
                                    //device.TextureState[2].AlphaOperation = TextureOperation.BlendCurrentAlpha;
                                    //device.TextureState[1].AlphaArgument1 = TextureArgument.Current;
                   
                                }

                                */
                                if (bsp.LightMapTexture != null)
                                {
                                    if (bsp.LightMapTexture[x] != null &&
                                        bsp.BSPRawDataMetaChunks[x].LightMapUVs.Count != 0 && bsp.RenderBSPLighting)
                                    {
                                        device.SetTexture(1, bsp.LightMapTexture[x]);
                                        device.TextureState[1].ColorOperation = TextureOperation.Modulate2X;
                                        device.TextureState[1].ColorArgument1 = TextureArgument.TextureColor;
                                        device.TextureState[1].ColorArgument2 = TextureArgument.Current;
                                        device.TextureState[1].TextureCoordinateIndex = 2;
                                    }
                                }
                            }
                        }
                        else
                        {
                            // If not Textured
                            device.SetTexture(0, null);
                            device.SetTexture(1, null);

                            if (bsp.LightMapTexture != null)
                            {
                                if (bsp.LightMapTexture[x] != null && bsp.BSPRawDataMetaChunks[x].LightMapUVs.Count != 0 &&
                                    bsp.RenderBSPLighting)
                                {
                                    device.SetTexture(0, bsp.LightMapTexture[x]);
                                    device.TextureState[0].ColorOperation = TextureOperation.Modulate2X;
                                    device.TextureState[0].ColorArgument1 = TextureArgument.TextureColor;
                                    device.TextureState[0].ColorArgument2 = TextureArgument.Current;
                                    device.TextureState[0].TextureCoordinateIndex = 2;
                                }
                            }
                        }

                        #endregion

                        PrimitiveType pt;
                        // if (bsp.BSPRawDataMetaChunks[rawindex].SubMeshInfo[xx].IndiceCount != (bsp.BSPRawDataMetaChunks[rawindex].SubMeshInfo[xx].IndiceCount/3)*3)
                        if (bsp.BSPRawDataMetaChunks[rawindex].FaceCount * 3 !=
                            bsp.BSPRawDataMetaChunks[rawindex].IndiceCount)
                        {
                            pt = PrimitiveType.TriangleStrip;
                            device.RenderState.FillMode = FillMode.Solid;
                            Material mm = new Material();
                            mm.Diffuse = Color.White;
                            mm.Ambient = Color.White;
                            // if (x == 1) { mm.Diffuse = System.Drawing.Color.Red; mm.Ambient = System.Drawing.Color.Red; }
                            device.Material = mm;
                            device.DrawIndexedPrimitives(
                                pt, 
                                0, 
                                0, 
                                bsp.BSPRawDataMetaChunks[rawindex].VerticeCount, 
                                bsp.BSPRawDataMetaChunks[rawindex].SubMeshInfo[xx].IndiceStart, 
                                bsp.BSPRawDataMetaChunks[rawindex].SubMeshInfo[xx].IndiceCount);
                        }
                        else
                        {
                            count++;
                            pt = PrimitiveType.TriangleList;

                            device.RenderState.FillMode = FillMode.Solid;
                            Material mm = new Material();
                            mm.Diffuse = Color.White;
                            mm.Ambient = Color.White;
                            // if (x == 1) { mm.Diffuse = System.Drawing.Color.Red; mm.Ambient = System.Drawing.Color.Red; }
                            device.Material = mm;

                            device.DrawIndexedPrimitives(
                                pt, 
                                0, 
                                0, 
                                bsp.BSPRawDataMetaChunks[rawindex].VerticeCount, 
                                bsp.BSPRawDataMetaChunks[rawindex].SubMeshInfo[xx].IndiceStart, 
                                bsp.BSPRawDataMetaChunks[rawindex].SubMeshInfo[xx].IndiceCount / 3);
                        }

                        #region RenderToScene

                        #region WireFrame

                        if (Textured == false)
                        {
                            Material m = new Material();
                            m.Diffuse = Color.Black;
                            m.Ambient = Color.Black;
                            device.Material = m;
                            device.RenderState.FillMode = FillMode.WireFrame;
                            device.DrawIndexedPrimitives(
                                pt, 
                                0, 
                                0, 
                                bsp.BSPRawDataMetaChunks[rawindex].VerticeCount, 
                                bsp.BSPRawDataMetaChunks[rawindex].SubMeshInfo[xx].IndiceStart, 
                                bsp.BSPRawDataMetaChunks[rawindex].SubMeshInfo[xx].IndiceCount / 3);
                        }

                        #endregion

                        #endregion
                    }
                }

                

                #region BSPPermutationMesh

                if (bsp.DrawBSPPermutations)
                {
                    for (int x = 0; x < bsp.PermutationInfo.Length; x++)
                    {
                        // continue;
                        int rawindex = bsp.PermutationInfo[x].sceneryIndex;
                        if (bsp.BSPPermutationRawDataMetaChunks[rawindex].RawDataChunkInfo.Length == 0)
                        {
                            continue;
                        }

                        device.Transform.World = bsp.PermutationInfo[x].mat;
                        device.SetStreamSource(0, bsp.Display.permvertexBuffer[rawindex], 0);
                        device.VertexFormat = HaloVertex.FVF;
                        device.Indices = bsp.Display.permindexBuffer[rawindex];
                        for (int xx = 0; xx < bsp.BSPPermutationRawDataMetaChunks[rawindex].SubMeshInfo.Length; xx++)
                        {
                            ResetTextureStates(ref device);
                            int tempshade = bsp.BSPPermutationRawDataMetaChunks[rawindex].SubMeshInfo[xx].ShaderNumber;

                            #region AlphaBlending

                            if (bsp.Shaders.Shader[tempshade].Alpha == ShaderInfo.AlphaType.AlphaBlend)
                            {
                                device.RenderState.AlphaBlendEnable = true;
                                device.RenderState.AlphaTestEnable = false;
                            }
                            else if (bsp.Shaders.Shader[tempshade].Alpha == ShaderInfo.AlphaType.AlphaTest)
                            {
                                device.RenderState.AlphaBlendEnable = false;
                                device.RenderState.AlphaTestEnable = true;
                            }
                            else
                            {
                                device.RenderState.AlphaBlendEnable = false;
                                device.RenderState.AlphaTestEnable = false;
                            }

                            #endregion

                            #region ChooseTexture

                            device.SetTexture(0, bsp.Shaders.Shader[tempshade].MainTexture);

                            device.TextureState[0].ColorOperation = TextureOperation.Modulate;
                            device.TextureState[0].ColorArgument1 = TextureArgument.TextureColor;
                            device.TextureState[0].ColorArgument2 = TextureArgument.Current;

                            // device.SetTexture(1, bsp.Shaders.Shader[tempshade].BumpMapTexture);
                            // device.TextureState[1].ColorOperation = TextureOperation.BumpEnvironmentMap;
                            // device.TextureState[1].ColorArgument1 = TextureArgument.TextureColor;
                            // device.TextureState[1].ColorArgument2 = TextureArgument.Current;
                            if (bsp.SceneryLightMapTexture != null)
                            {
                                if (bsp.SceneryLightMapTexture[x] != null &&
                                    bsp.BSPPermutationRawDataMetaChunks[rawindex].LightMapUVs.Count != 0)
                                {
                                    device.SetTexture(1, bsp.SceneryLightMapTexture[x]);
                                    device.TextureState[1].ColorOperation = TextureOperation.Modulate;
                                    device.TextureState[1].ColorArgument1 = TextureArgument.TextureColor;
                                    device.TextureState[1].ColorArgument2 = TextureArgument.Current;
                                    device.TextureState[1].TextureCoordinateIndex = 2;
                                }
                                else
                                {
                                    // device.TextureState[0].ColorOperation = TextureOperation.Disable;
                                    device.TextureState[0].ColorOperation = TextureOperation.Modulate2X;
                                    device.TextureState[0].ColorArgument1 = TextureArgument.TextureColor;
                                    device.TextureState[0].ColorArgument2 = TextureArgument.Current;
                                }
                            }

                            #endregion

                            #region RenderToScene

                            PrimitiveType pt;
                            pt = PrimitiveType.TriangleList;
                            device.RenderState.FillMode = FillMode.Solid;
                            Material mm = new Material();
                            mm.Diffuse = Color.White;
                            mm.Ambient = Color.White;
                            device.Material = mm;
                            device.DrawIndexedPrimitives(
                                pt, 
                                0, 
                                0, 
                                bsp.BSPPermutationRawDataMetaChunks[rawindex].VerticeCount, 
                                bsp.BSPPermutationRawDataMetaChunks[rawindex].SubMeshInfo[xx].IndiceStart, 
                                bsp.BSPPermutationRawDataMetaChunks[rawindex].SubMeshInfo[xx].IndiceCount / 3);

                            #region WireFrame

                            if (Textured == false)
                            {
                                Material m = new Material();
                                m.Diffuse = Color.Red;
                                m.Ambient = Color.Red;
                                device.Material = m;
                                device.RenderState.FillMode = FillMode.WireFrame;
                                device.DrawIndexedPrimitives(
                                    pt, 
                                    0, 
                                    0, 
                                    bsp.BSPPermutationRawDataMetaChunks[rawindex].VerticeCount, 
                                    bsp.BSPPermutationRawDataMetaChunks[rawindex].SubMeshInfo[xx].IndiceStart, 
                                    bsp.BSPPermutationRawDataMetaChunks[rawindex].SubMeshInfo[xx].IndiceCount / 3);
                            }

                            #endregion

                            #endregion
                        }
                    }
                }

                #endregion

                ResetTextureStates(ref device);
            }

            /// <summary>
            /// The draw cluster.
            /// </summary>
            /// <param name="rawindex">The rawindex.</param>
            /// <param name="outlinecolor">The outlinecolor.</param>
            /// <param name="device">The device.</param>
            /// <param name="bsp">The bsp.</param>
            /// <param name="Textured">The textured.</param>
            /// <param name="WireFrame">The wire frame.</param>
            /// <param name="cam">The cam.</param>
            /// <remarks></remarks>
            public static void DrawCluster(
                int rawindex, 
                Color outlinecolor, 
                ref Device device, 
                ref BSPModel bsp, 
                bool Textured, 
                bool WireFrame, 
                ref Camera2 cam)
            {
                

                if (bsp.BSPRawDataMetaChunks[rawindex].RawDataChunkInfo.Length == 0)
                {
                    return;
                }

                // Vector3 tempv = new Vector3();
                // tempv.X = bsp.Spawns.Spawn[x].X;
                // tempv.Y = bsp.Spawns.Spawn[x].Y;
                // tempv.Z = bsp.Spawns.Spawn[x].Z;
                // if (!cam.SphereInFrustum(tempv, 10f)) { continue; }
                device.SetStreamSource(0, bsp.Display.vertexBuffer[rawindex], 0);
                device.VertexFormat = CustomVertex.PositionNormalTextured.Format;
                device.Indices = bsp.Display.indexBuffer[rawindex];
                for (int xx = 0; xx < bsp.BSPRawDataMetaChunks[rawindex].SubMeshInfo.Length; xx++)
                {
                    int tempshade = bsp.BSPRawDataMetaChunks[rawindex].SubMeshInfo[xx].ShaderNumber;

                    #region AlphaBlending

                    if (bsp.Shaders.Shader[tempshade].Alpha == ShaderInfo.AlphaType.AlphaBlend)
                    {
                        // MessageBox.Show("test");
                        device.RenderState.AlphaBlendEnable = true;
                        device.RenderState.AlphaTestEnable = false;
                    }
                    else if (bsp.Shaders.Shader[tempshade].Alpha == ShaderInfo.AlphaType.AlphaTest)
                    {
                        // MessageBox.Show("test2");
                        device.RenderState.AlphaBlendEnable = false;
                        device.RenderState.AlphaTestEnable = true;
                    }
                    else
                    {
                        device.RenderState.AlphaBlendEnable = false;
                        device.RenderState.AlphaTestEnable = false;
                    }

                    #endregion

                    #region ChooseTexture

                    if (Textured)
                    {
                        // device.TextureState[0] = device.TextureState[2];
                        device.SetTexture(0, bsp.Shaders.Shader[tempshade].MainTexture);
                    }
                    else
                    {
                        device.SetTexture(0, null);
                    }

                    #endregion

                    #region RenderToScene

                    PrimitiveType pt;
                    pt = PrimitiveType.TriangleList;
                    device.RenderState.FillMode = FillMode.Solid;
                    Material mm = new Material();
                    mm.Diffuse = Color.Black;
                    mm.Ambient = Color.Black;
                    device.Material = mm;
                    device.DrawIndexedPrimitives(
                        pt, 
                        0, 
                        0, 
                        bsp.BSPRawDataMetaChunks[rawindex].VerticeCount, 
                        bsp.BSPRawDataMetaChunks[rawindex].SubMeshInfo[xx].IndiceStart, 
                        bsp.BSPRawDataMetaChunks[rawindex].SubMeshInfo[xx].IndiceCount / 3);

                    #region WireFrame

                    if (WireFrame)
                    {
                        Material m = new Material();
                        m.Diffuse = outlinecolor;
                        m.Ambient = outlinecolor;
                        device.Material = m;
                        device.RenderState.FillMode = FillMode.WireFrame;
                        device.DrawIndexedPrimitives(
                            pt, 
                            0, 
                            0, 
                            bsp.BSPRawDataMetaChunks[rawindex].VerticeCount, 
                            bsp.BSPRawDataMetaChunks[rawindex].SubMeshInfo[xx].IndiceStart, 
                            bsp.BSPRawDataMetaChunks[rawindex].SubMeshInfo[xx].IndiceCount / 3);
                    }

                    #endregion

                    #endregion
                }

                
            }

            /// <summary>
            /// The find displayed pieces.
            /// </summary>
            /// <remarks></remarks>
            public static void FindDisplayedPieces()
            {
            }

            /// <summary>
            /// The load direct x textures and buffers.
            /// </summary>
            /// <param name="device">The device.</param>
            /// <param name="bsp">The bsp.</param>
            /// <remarks></remarks>
            public static void LoadDirectXTexturesAndBuffers(ref Device device, ref BSPModel bsp)
            {
                CreateVertexBuffers(ref device, ref bsp);
                CreateIndexBuffers(ref device, ref bsp);
                LoadShaderTextures(ref device, ref bsp);
                LoadLightmapTextures(ref device, ref bsp);
            }

            /// <summary>
            /// The load lightmap textures.
            /// </summary>
            /// <param name="device">The device.</param>
            /// <param name="bsp">The bsp.</param>
            /// <remarks></remarks>
            public static void LoadLightmapTextures(ref Device device, ref BSPModel bsp)
            {
                bsp.LightMapTexture = new Texture[bsp.BSPRawDataMetaChunks.Length];
                for (int x = 0; x < bsp.LightMapBitmap.Length; x++)
                {
                    bsp.LightMapTexture[x] = ShaderInfo.CreateTexture(ref device, bsp.LightMapBitmap[x]);
                }

                bsp.SceneryLightMapTexture = new Texture[bsp.SceneryLightMapBitmap.Length];
                for (int x = 0; x < bsp.SceneryLightMapBitmap.Length; x++)
                {
                    bsp.SceneryLightMapTexture[x] = ShaderInfo.CreateTexture(ref device, bsp.SceneryLightMapBitmap[x]);
                }
            }

            /// <summary>
            /// The load shader textures.
            /// </summary>
            /// <param name="device">The device.</param>
            /// <param name="bsp">The bsp.</param>
            /// <remarks></remarks>
            public static void LoadShaderTextures(ref Device device, ref BSPModel bsp)
            {
                for (int x = 0; x < bsp.Shaders.Shader.Length; x++)
                {
                    bsp.Shaders.Shader[x].MakeTextures(ref device);
                    bsp.Shaders.Shader[x].KillBitmaps();
                }

                // MessageBox.Show("test");
            }

            /// <summary>
            /// The reset texture states.
            /// </summary>
            /// <param name="device">The device.</param>
            /// <remarks></remarks>
            public static void ResetTextureStates(ref Device device)
            {
                device.SamplerState[0].AddressU = TextureAddress.Wrap;
                device.SamplerState[0].AddressV = TextureAddress.Wrap;
                device.SamplerState[1].AddressU = TextureAddress.Wrap;
                device.SamplerState[1].AddressV = TextureAddress.Wrap;
                device.SamplerState[2].AddressU = TextureAddress.Wrap;
                device.SamplerState[2].AddressV = TextureAddress.Wrap;
                device.SamplerState[3].AddressU = TextureAddress.Wrap;
                device.SamplerState[3].AddressV = TextureAddress.Wrap;

                device.Transform.Texture0 = Matrix.Identity;
                device.TextureState[0].TextureTransform = TextureTransform.Disable;
                device.TextureState[0].TextureCoordinateIndex = 0;
                device.TextureState[0].ColorOperation = TextureOperation.Disable;
                device.SetTexture(0, null);

                device.Transform.Texture1 = Matrix.Identity;
                device.TextureState[1].TextureTransform = TextureTransform.Disable;
                device.TextureState[1].TextureCoordinateIndex = 1;
                device.TextureState[1].ColorOperation = TextureOperation.Disable;
                device.SetTexture(1, null);

                device.Transform.Texture2 = Matrix.Identity;
                device.TextureState[2].TextureTransform = TextureTransform.Disable;
                device.TextureState[2].TextureCoordinateIndex = 2;
                device.TextureState[2].ColorOperation = TextureOperation.Disable;
                device.SetTexture(2, null);

                device.Transform.Texture3 = Matrix.Identity;
                device.TextureState[3].TextureTransform = TextureTransform.Disable;
                device.TextureState[3].TextureCoordinateIndex = 3;
                device.TextureState[3].ColorOperation = TextureOperation.Disable;
                device.SetTexture(3, null);
            }

            #endregion
        }

        /// <summary>
        /// The bsp permutation raw data meta chunk.
        /// </summary>
        /// <remarks></remarks>
        public class BSPPermutationRawDataMetaChunk : ParsedModel.RawDataMetaChunk
        {
            #region Constants and Fields

            /// <summary>
            /// The bounding box.
            /// </summary>
            public BSPPermutationBoundingBoxContainer BoundingBox;

            /// <summary>
            /// The light map u vs.
            /// </summary>
            public List<Vector2> LightMapUVs = new List<Vector2>();

            #endregion

            #region Constructors and Destructors

            /// <summary>
            /// Initializes a new instance of the <see cref="BSPPermutationRawDataMetaChunk"/> class.
            /// </summary>
            /// <param name="offset">The offset.</param>
            /// <param name="chunknumber">The chunknumber.</param>
            /// <param name="meta">The meta.</param>
            /// <remarks></remarks>
            public BSPPermutationRawDataMetaChunk(int offset, int chunknumber, ref Meta meta)
            {
                

                BinaryReader BR = new BinaryReader(meta.MS);
                BR.BaseStream.Position = offset;
                this.VerticeCount = BR.ReadUInt16();
                this.FaceCount = BR.ReadUInt16();

                #region LoadBoundingBox

                BR.BaseStream.Position = offset + 24;
                int tempc = BR.ReadInt32();
                int tempr = BR.ReadInt32() - meta.magic - meta.offset;
                BR.BaseStream.Position = tempr;
                // BoundingBox = new BSPPermutationBoundingBoxContainer(ref BR);
                #endregion

                BR.BaseStream.Position = offset + 48;
                this.HeaderSize = BR.ReadInt32() + 8;

                #region LoadRawDataChunkInfo

                BR.BaseStream.Position = offset + 56;
                tempc = BR.ReadInt32();
                tempr = BR.ReadInt32() - meta.magic - meta.offset;
                this.RawDataChunkInfo = new RawDataOffsetChunk[tempc];
                for (int x = 0; x < tempc; x++)
                {
                    this.RawDataChunkInfo[x] = new RawDataOffsetChunk();
                    BR.BaseStream.Position = tempr + (x * 16) + 6;
                    this.RawDataChunkInfo[x].ChunkSize = BR.ReadUInt16();
                    this.RawDataChunkInfo[x].Size = BR.ReadInt32();
                    if (this.RawDataChunkInfo[x].ChunkSize == 0)
                    {
                        this.RawDataChunkInfo[x].ChunkSize = this.RawDataChunkInfo[x].Size;
                    }

                    this.RawDataChunkInfo[x].Offset = BR.ReadInt32();
                    this.RawDataChunkInfo[x].ChunkCount = this.RawDataChunkInfo[x].Size /
                                                          this.RawDataChunkInfo[x].ChunkSize;
                }

                if (this.RawDataChunkInfo.Length == 0)
                {
                    return;
                }

                #endregion

                

                #region RawData

                int firstbsp2chunk = 0;

                #region FindFirstPermutationRawData

                for (int x = 0; x < meta.raw.rawChunks.Count; x++)
                {
                    if (meta.raw.rawChunks[x].rawDataType == RawDataType.bsp2)
                    {
                        firstbsp2chunk = x;
                        break;
                    }
                }

                #endregion

                #region LoadSubMeshInfo

                BR = new BinaryReader(meta.raw.rawChunks[firstbsp2chunk + chunknumber].MS);
                this.SubMeshInfo = new ModelSubMeshInfo[this.RawDataChunkInfo[0].ChunkCount];
                for (int x = 0; x < this.RawDataChunkInfo[0].ChunkCount; x++)
                {
                    this.SubMeshInfo[x] = new ModelSubMeshInfo();
                    BR.BaseStream.Position = this.HeaderSize + this.RawDataChunkInfo[0].Offset + (x * 72) + 4;
                    this.SubMeshInfo[x].ShaderNumber = BR.ReadUInt16();
                    this.SubMeshInfo[x].IndiceStart = BR.ReadUInt16();
                    this.SubMeshInfo[x].IndiceCount = BR.ReadUInt16();
                }

                #endregion

                #region DetermineChunkNumers

                int indicechunk = 0;
                int verticechunk = 0;
                int uvchunk = 0;
                int normalchunk = 0;
                for (int x = 0; x < RawDataChunkInfo.Length; x++)
                {
                    if (RawDataChunkInfo[x].ChunkSize == 2)
                    {
                        indicechunk = x;
                        break;
                    }
                }

                for (int x = indicechunk; x < RawDataChunkInfo.Length; x++)
                {
                    if (RawDataChunkInfo[x].ChunkCount == 1)
                    {
                        verticechunk = x;
                        uvchunk = x + 1;
                        normalchunk = x + 2;
                        break;
                    }
                }

                #endregion

                #region Faces

                BR.BaseStream.Position = 40;
                this.IndiceCount = BR.ReadUInt16();
                this.Indices = new short[this.IndiceCount];
                BR.BaseStream.Position = this.HeaderSize + this.RawDataChunkInfo[indicechunk].Offset;
                for (int x = 0; x < this.IndiceCount; x++)
                {
                    this.Indices[x] = (short)BR.ReadUInt16();
                }

                #endregion

                #region Vertices

                this.RawDataChunkInfo[verticechunk].ChunkSize = this.RawDataChunkInfo[verticechunk].Size /
                                                                this.VerticeCount;

                for (int x = 0; x < this.VerticeCount; x++)
                {
                    Vector3 vec = new Vector3();
                    BR.BaseStream.Position = this.HeaderSize + this.RawDataChunkInfo[verticechunk].Offset +
                                             (this.RawDataChunkInfo[verticechunk].ChunkSize * x);
                    vec.X = BR.ReadSingle();
                    vec.Y = BR.ReadSingle();
                    vec.Z = BR.ReadSingle();
                    Vertices.Add(vec);
                }

                #endregion

                #region UV's

                this.RawDataChunkInfo[uvchunk].ChunkSize = 8;
                for (int x = 0; x < this.VerticeCount; x++)
                {
                    Vector2 tempuv = new Vector2();
                    BR.BaseStream.Position = this.HeaderSize + this.RawDataChunkInfo[uvchunk].Offset +
                                             (this.RawDataChunkInfo[uvchunk].ChunkSize * x);
                    tempuv.X = BR.ReadSingle();
                    // if (this.UVs[x].X > 1) { this.UVs[x].X = this.UVs[x].X - 1; }
                    // if (this.UVs[x].X < 0) { this.UVs[x].X = this.UVs[x].X + 1; }
                    tempuv.Y = BR.ReadSingle();
                    // if (this.UVs[x].Y > 1) { this.UVs[x].Y = this.UVs[x].Y - 1; }
                    // if (this.UVs[x].Y < 0) { this.UVs[x].Y = this.UVs[x].Y + 1; }
                    this.UVs.Add(tempuv);
                }

                #endregion

                #region Normals

                this.RawDataChunkInfo[normalchunk].ChunkSize = 12;
                for (int x = 0; x < this.VerticeCount; x++)
                {
                    Vector2 tempuv = new Vector2();
                    BR.BaseStream.Position = this.HeaderSize + this.RawDataChunkInfo[normalchunk].Offset +
                                             (this.RawDataChunkInfo[normalchunk].ChunkSize * x);
                    Vector3 normal = ParsedModel.DecompressNormal(BR.ReadInt32());
                    this.Normals.Add(normal);
                    Vector3 binormal = ParsedModel.DecompressNormal(BR.ReadInt32());
                    this.Binormals.Add(binormal);
                    Vector3 tangent = ParsedModel.DecompressNormal(BR.ReadInt32());
                    this.Tangents.Add(tangent);
                }

                #endregion

                int lightmapuvchunk = -1;
                for (int x = normalchunk + 1; x < RawDataChunkInfo.Length; x++)
                {
                    if (RawDataChunkInfo[x].ChunkSize == 3)
                    {
                        lightmapuvchunk = x;
                        break;
                    }
                }

                if (lightmapuvchunk == -1)
                {
                    return;
                }

                RawDataChunkInfo[lightmapuvchunk].ChunkSize = 4;
                for (int x = 0; x < this.VerticeCount; x++)
                {
                    Vector2 tempuv = new Vector2();
                    BR.BaseStream.Position = this.HeaderSize + this.RawDataChunkInfo[lightmapuvchunk].Offset +
                                             (this.RawDataChunkInfo[lightmapuvchunk].ChunkSize * x);
                    short testx = BR.ReadInt16();
                    float u = DecompressVertice(Convert.ToSingle(testx), -1, 1);
                    testx = BR.ReadInt16();
                    float v = DecompressVertice(Convert.ToSingle(testx), -1, 1);
                    // if (u < 0) u += 1;
                    // if (v < 0) v += 1;
                    Vector2 uv2 = new Vector2(u, v);
                    this.LightMapUVs.Add(uv2);
                }
            }

            #endregion

            #endregion

            /// <summary>
            /// The bsp permutation bounding box container.
            /// </summary>
            /// <remarks></remarks>
            public class BSPPermutationBoundingBoxContainer
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
                /// Initializes a new instance of the <see cref="BSPPermutationBoundingBoxContainer"/> class.
                /// </summary>
                /// <param name="BR">The BR.</param>
                /// <remarks></remarks>
                public BSPPermutationBoundingBoxContainer(ref BinaryReader BR)
                {
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
        }

        /// <summary>
        /// The bsp raw data meta chunk.
        /// </summary>
        /// <remarks></remarks>
        public class BSPRawDataMetaChunk : ParsedModel.RawDataMetaChunk
        {
            #region Constants and Fields

            public BSPBoundingBoxContainer BoundingBox;

            /// <summary>
            /// The light map u vs.
            /// </summary>
            public List<Vector2> LightMapUVs = new List<Vector2>();

            /// <summary>
            /// The rawdatainfooffset.
            /// </summary>
            public int rawdatainfooffset;

            /// <summary>
            /// Used to decide if chunk will be rendered
            /// </summary>
            public bool render = true;

            /// <summary>
            /// The shadertagnumber.
            /// </summary>
            public int shadertagnumber;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            /// Initializes a new instance of the <see cref="BSPRawDataMetaChunk"/> class.
            /// </summary>
            /// <remarks></remarks>
            public BSPRawDataMetaChunk()
            {
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="BSPRawDataMetaChunk"/> class.
            /// </summary>
            /// <param name="offset">The offset.</param>
            /// <param name="chunknumber">The chunknumber.</param>
            /// <param name="meta">The meta.</param>
            /// <param name="faces">The faces.</param>
            /// <remarks></remarks>
            public BSPRawDataMetaChunk(int offset, int chunknumber, ref Meta meta, Face[] faces)
            {
                switch (meta.Map.HaloVersion)
                {
                    case HaloVersionEnum.Halo2:
                    case HaloVersionEnum.Halo2Vista:
                        H2BSPRawDataMetaChunk(offset, chunknumber, ref meta);
                        break;
                    case HaloVersionEnum.HaloCE:
                        CEBSPRawDataMetaChunk(offset, chunknumber, ref meta, faces);
                        break;
                    case HaloVersionEnum.Halo1:
                        H1BSPRawDataMetaChunk(offset, chunknumber, ref meta, faces);
                        break;
                }
            }

            #endregion

            public class BSPBoundingBoxContainer
            {
                #region Constants and Fields

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
                /// Initializes a new instance of the <see cref="BSPBoundingBoxContainer"/> class.
                /// </summary>
                /// <param name="BR">The BR.</param>
                /// <remarks></remarks>
                public BSPBoundingBoxContainer(List<Vector3> Vertices)
                {
                    MinX = 10000;
                    MaxX = -10000;
                    MinY = 10000;
                    MaxY = -10000;
                    MinZ = 10000;
                    MaxZ = -10000;

                    foreach (Vector3 position in Vertices)
                    {
                        if (position.X < MinX)
                            MinX = position.X;
                        if (position.Y < MinY)
                            MinY = position.Y;
                        if (position.Z < MinZ)
                            MinZ = position.Z;

                        if (position.X > MaxX)
                            MaxX = position.X;
                        if (position.Y > MaxY)
                            MaxY = position.Y;
                        if (position.Z > MaxZ)
                            MaxZ = position.Z;
                    }
                }

                #endregion
            }

            #region Public Methods

            /// <summary>
            /// The cebsp raw data meta chunk.
            /// </summary>
            /// <param name="offset">The offset.</param>
            /// <param name="chunknumber">The chunknumber.</param>
            /// <param name="meta">The meta.</param>
            /// <param name="faces">The faces.</param>
            /// <remarks></remarks>
            public void CEBSPRawDataMetaChunk(int offset, int chunknumber, ref Meta meta, Face[] faces)
            {
                BinaryReader BR = new BinaryReader(meta.MS);
                BR.BaseStream.Position = offset + 12;
                this.shadertagnumber = meta.Map.Functions.ForMeta.FindMetaByID(BR.ReadInt32());
                this.RawDataChunkInfo = new RawDataOffsetChunk[1];
                BR.BaseStream.Position = offset + 20;
                int surfacestart = BR.ReadInt32();
                this.FaceCount = BR.ReadInt32();
                this.SubMeshInfo = new ModelSubMeshInfo[1];
                this.SubMeshInfo[0] = new ModelSubMeshInfo();
                this.SubMeshInfo[0].IndiceCount = this.FaceCount * 3;
                this.SubMeshInfo[0].ShaderNumber = chunknumber;
                this.IndiceCount = this.SubMeshInfo[0].IndiceCount;
                this.Indices = new short[this.IndiceCount];
                for (int x = 0; x < this.FaceCount; x++)
                {
                    this.Indices[(x * 3) + 0] = (short)faces[surfacestart + x].vertex0Index;
                    this.Indices[(x * 3) + 1] = (short)faces[surfacestart + x].vertex1Index;
                    this.Indices[(x * 3) + 2] = (short)faces[surfacestart + x].vertex2Index;
                }

                BR.BaseStream.Position = offset + 180;
                this.VerticeCount = BR.ReadInt32();
                BR.BaseStream.Position = offset + 228;
                int vertstranslation = BR.ReadInt32() - meta.magic - meta.offset;
                BR.BaseStream.Position = vertstranslation;
                BSPRawDataMetaChunk tempchunk = new BSPRawDataMetaChunk();
                for (int x = 0; x < this.VerticeCount; x++)
                {
                    AddUncompressedVertice(ref BR, ref tempchunk);
                }

                this.Vertices = tempchunk.Vertices;
                this.Normals = tempchunk.Normals;
                this.Binormals = tempchunk.Binormals;
                this.Tangents = tempchunk.Tangents;
                this.UVs = tempchunk.UVs;
                this.LightMapUVs = tempchunk.LightMapUVs;
            }

            /// <summary>
            /// The h 1 bsp raw data meta chunk.
            /// </summary>
            /// <param name="offset">The offset.</param>
            /// <param name="chunknumber">The chunknumber.</param>
            /// <param name="meta">The meta.</param>
            /// <param name="faces">The faces.</param>
            /// <remarks></remarks>
            public void H1BSPRawDataMetaChunk(int offset, int chunknumber, ref Meta meta, Face[] faces)
            {
                BinaryReader BR = new BinaryReader(meta.MS);
                BR.BaseStream.Position = offset + 12;
                this.shadertagnumber = meta.Map.Functions.ForMeta.FindMetaByID(BR.ReadInt32());
                this.RawDataChunkInfo = new RawDataOffsetChunk[1];
                BR.BaseStream.Position = offset + 20;
                int surfacestart = BR.ReadInt32();
                this.FaceCount = BR.ReadInt32();
                this.SubMeshInfo = new ModelSubMeshInfo[1];
                this.SubMeshInfo[0] = new ModelSubMeshInfo();
                this.SubMeshInfo[0].IndiceCount = this.FaceCount * 3;
                this.SubMeshInfo[0].ShaderNumber = chunknumber;
                this.IndiceCount = this.SubMeshInfo[0].IndiceCount;
                this.Indices = new short[this.IndiceCount];
                for (int x = 0; x < this.FaceCount; x++)
                {
                    this.Indices[(x * 3) + 0] = (short)faces[surfacestart + x].vertex0Index;
                    this.Indices[(x * 3) + 1] = (short)faces[surfacestart + x].vertex1Index;
                    this.Indices[(x * 3) + 2] = (short)faces[surfacestart + x].vertex2Index;
                }

                BR.BaseStream.Position = offset + 180;
                this.VerticeCount = BR.ReadInt32();
                BR.BaseStream.Position = offset + 248;
                int vertstranslation = BR.ReadInt32() - meta.magic - meta.offset;
                BR.BaseStream.Position = vertstranslation;
                BSPRawDataMetaChunk tempchunk = new BSPRawDataMetaChunk();
                for (int x = 0; x < this.VerticeCount; x++)
                {
                    AddCompressedVertice(ref BR, ref tempchunk);
                }

                this.Vertices = tempchunk.Vertices;
                this.Normals = tempchunk.Normals;
                this.Binormals = tempchunk.Binormals;
                this.Tangents = tempchunk.Tangents;
                this.UVs = tempchunk.UVs;
                this.LightMapUVs = tempchunk.LightMapUVs;
            }

            /// <summary>
            /// The h 2 bsp raw data meta chunk.
            /// </summary>
            /// <param name="offset">The offset.</param>
            /// <param name="chunknumber">The chunknumber.</param>
            /// <param name="meta">The meta.</param>
            /// <remarks></remarks>
            public void H2BSPRawDataMetaChunk(int offset, int chunknumber, ref Meta meta)
            {
                BinaryReader BR = new BinaryReader(meta.MS);
                BR.BaseStream.Position = offset;
                this.VerticeCount = BR.ReadUInt16();
                this.FaceCount = BR.ReadUInt16();
                BR.BaseStream.Position = offset + 48;
                this.HeaderSize = BR.ReadInt32() + 8;
                BR.ReadInt32();
                int tempc = BR.ReadInt32();
                int tempr = BR.ReadInt32() - meta.magic - meta.offset;
                this.RawDataChunkInfo = new RawDataOffsetChunk[tempc];
                rawdatainfooffset = tempr;
                for (int x = 0; x < tempc; x++)
                {
                    this.RawDataChunkInfo[x] = new RawDataOffsetChunk();
                    BR.BaseStream.Position = tempr + (x * 16) + 6;
                    this.RawDataChunkInfo[x].ChunkSize = BR.ReadUInt16();
                    this.RawDataChunkInfo[x].Size = BR.ReadInt32();
                    if (this.RawDataChunkInfo[x].ChunkSize == 0)
                    {
                        this.RawDataChunkInfo[x].ChunkSize = this.RawDataChunkInfo[x].Size;
                    }

                    this.RawDataChunkInfo[x].Offset = BR.ReadInt32();
                    this.RawDataChunkInfo[x].ChunkCount = this.RawDataChunkInfo[x].Size /
                                                          this.RawDataChunkInfo[x].ChunkSize;
                }

                if (this.RawDataChunkInfo.Length == 0)
                {
                    return;
                }

                BR = new BinaryReader(meta.raw.rawChunks[chunknumber].MS);
                this.SubMeshInfo = new ModelSubMeshInfo[this.RawDataChunkInfo[0].ChunkCount];
                for (int x = 0; x < this.RawDataChunkInfo[0].ChunkCount; x++)
                {
                    this.SubMeshInfo[x] = new ModelSubMeshInfo();
                    BR.BaseStream.Position = this.HeaderSize + this.RawDataChunkInfo[0].Offset + (x * 72) + 4;
                    this.SubMeshInfo[x].ShaderNumber = BR.ReadUInt16();
                    this.SubMeshInfo[x].IndiceStart = BR.ReadUInt16();
                    this.SubMeshInfo[x].IndiceCount = BR.ReadUInt16();
                }

                BR.BaseStream.Position = 40;
                this.IndiceCount = BR.ReadUInt16();
                int indicechunk = 0;
                int verticechunk = 0;
                int uvchunk = 0;
                for (int x = 0; x < RawDataChunkInfo.Length; x++)
                {
                    if (RawDataChunkInfo[x].ChunkSize == 2)
                    {
                        indicechunk = x;
                        break;
                    }
                }

                int normalchunk = 0;
                for (int x = indicechunk; x < RawDataChunkInfo.Length; x++)
                {
                    if (RawDataChunkInfo[x].ChunkCount == 1)
                    {
                        verticechunk = x;
                        uvchunk = x + 1;
                        normalchunk = x + 2;
                        break;
                    }
                }

                this.Indices = new short[this.RawDataChunkInfo[indicechunk].ChunkCount];
                BR.BaseStream.Position = this.HeaderSize + this.RawDataChunkInfo[indicechunk].Offset;
                for (int x = 0; x < this.IndiceCount; x++)
                {
                    this.Indices[x] = (short)BR.ReadUInt16();
                }

                this.RawDataChunkInfo[verticechunk].ChunkSize = this.RawDataChunkInfo[verticechunk].Size / VerticeCount;
                for (int x = 0; x < this.VerticeCount; x++)
                {
                    Vector3 vec = new Vector3();
                    BR.BaseStream.Position = this.HeaderSize + this.RawDataChunkInfo[verticechunk].Offset +
                                             (this.RawDataChunkInfo[verticechunk].ChunkSize * x);
                    vec.X = BR.ReadSingle();
                    vec.Y = BR.ReadSingle();
                    vec.Z = BR.ReadSingle();
                    Vertices.Add(vec);
                }

                this.RawDataChunkInfo[uvchunk].ChunkSize = 8;
                for (int x = 0; x < this.VerticeCount; x++)
                {
                    Vector2 tempuv = new Vector2();
                    BR.BaseStream.Position = this.HeaderSize + this.RawDataChunkInfo[uvchunk].Offset +
                                             (this.RawDataChunkInfo[uvchunk].ChunkSize * x);
                    tempuv.X = BR.ReadSingle();
                    tempuv.Y = BR.ReadSingle();
                    this.UVs.Add(tempuv);
                }

                this.RawDataChunkInfo[normalchunk].ChunkSize = 12;
                for (int x = 0; x < this.VerticeCount; x++)
                {
                    Vector2 tempuv = new Vector2();
                    BR.BaseStream.Position = this.HeaderSize + this.RawDataChunkInfo[normalchunk].Offset +
                                             (this.RawDataChunkInfo[normalchunk].ChunkSize * x);
                    Vector3 normal = ParsedModel.DecompressNormal(BR.ReadInt32());
                    this.Normals.Add(normal);
                    Vector3 binormal = ParsedModel.DecompressNormal(BR.ReadInt32());
                    this.Binormals.Add(binormal);
                    Vector3 tangent = ParsedModel.DecompressNormal(BR.ReadInt32());
                    this.Tangents.Add(tangent);
                }

                int lightmapuvchunk = -1;
                for (int x = normalchunk + 1; x < RawDataChunkInfo.Length; x++)
                {
                    if (RawDataChunkInfo[x].ChunkSize == 3)
                    {
                        lightmapuvchunk = x;
                        break;
                    }
                }

                if (lightmapuvchunk == -1)
                {
                    return;
                }

                RawDataChunkInfo[lightmapuvchunk].ChunkSize = 4;
                for (int x = 0; x < this.VerticeCount; x++)
                {
                    Vector2 tempuv = new Vector2();
                    BR.BaseStream.Position = this.HeaderSize + this.RawDataChunkInfo[lightmapuvchunk].Offset +
                                             (this.RawDataChunkInfo[lightmapuvchunk].ChunkSize * x);
                    short testx = BR.ReadInt16();
                    float u = DecompressVertice(Convert.ToSingle(testx), -1, 1);
                    testx = BR.ReadInt16();
                    float v = DecompressVertice(Convert.ToSingle(testx), -1, 1);
                    Vector2 uv2 = new Vector2(u, v);
                    this.LightMapUVs.Add(uv2);
                }
                BoundingBox = new BSPBoundingBoxContainer(this.Vertices);
            }

            #endregion
        }

        /// <summary>
        /// The bsp shader container.
        /// </summary>
        /// <remarks></remarks>
        public class BSPShaderContainer : ParsedModel.ShaderContainer
        {
            #region Constructors and Destructors

            /// <summary>
            /// Initializes a new instance of the <see cref="BSPShaderContainer"/> class.
            /// </summary>
            /// <remarks></remarks>
            public BSPShaderContainer()
            {
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="BSPShaderContainer"/> class.
            /// </summary>
            /// <param name="bsp">The BSP.</param>
            /// <param name="meta">The meta.</param>
            /// <remarks></remarks>
            public BSPShaderContainer(BSPModel bsp, ref Meta meta)
            {
                switch (meta.Map.HaloVersion)
                {
                    case HaloVersionEnum.Halo2:
                    case HaloVersionEnum.Halo2Vista:
                        H2BSPShaderContainer(ref meta);
                        break;
                }
            }

            #endregion

            #region Public Methods

            /// <summary>
            /// The h 2 bsp shader container.
            /// </summary>
            /// <param name="meta">The meta.</param>
            /// <remarks></remarks>
            public void H2BSPShaderContainer(ref Meta meta)
            {
                BinaryReader BR = new BinaryReader(meta.MS);
                BR.BaseStream.Position = 180;
                int tempc = BR.ReadInt32();
                int tempr = BR.ReadInt32() - meta.magic - meta.offset;
                this.Shader = new ShaderInfo[tempc];

                meta.Map.OpenMap(MapTypes.Internal);
                for (int x = 0; x < tempc; x++)
                {
                    BR.BaseStream.Position = tempr + (x * 32) + 12;
                    int tempint = meta.Map.Functions.ForMeta.FindMetaByID(BR.ReadInt32());
                    this.Shader[x] = new ShaderInfo(tempint, meta.Map);
                }
                meta.Map.CloseMap();
            }

            #endregion
        }

        /// <summary>
        /// The face.
        /// </summary>
        /// <remarks></remarks>
        public class Face
        {
            #region Constants and Fields

            /// <summary>
            /// The vertex 0 index.
            /// </summary>
            public int vertex0Index;

            /// <summary>
            /// The vertex 1 index.
            /// </summary>
            public int vertex1Index;

            /// <summary>
            /// The vertex 2 index.
            /// </summary>
            public int vertex2Index;

            #endregion
        }

        /// <summary>
        /// The permutation placement.
        /// </summary>
        /// <remarks></remarks>
        public class PermutationPlacement
        {
            #region Constants and Fields

            /// <summary>
            /// The mat.
            /// </summary>
            public Matrix mat = Matrix.Identity;

            /// <summary>
            /// The scale.
            /// </summary>
            public float scale;

            /// <summary>
            /// The scenery index.
            /// </summary>
            public short sceneryIndex;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            /// Initializes a new instance of the <see cref="PermutationPlacement"/> class.
            /// </summary>
            /// <param name="BR">The BR.</param>
            /// <remarks></remarks>
            public PermutationPlacement(ref BinaryReader BR)
            {
                byte[] tmpBuff = BR.ReadBytes(0x58);
                scale = BitConverter.ToSingle(tmpBuff, 0x0);

                mat = Matrix.Identity;
                mat.M11 = BitConverter.ToSingle(tmpBuff, 0x4);
                mat.M12 = BitConverter.ToSingle(tmpBuff, 0x8);
                mat.M13 = BitConverter.ToSingle(tmpBuff, 0xC);
                mat.M14 = 0;

                mat.M21 = BitConverter.ToSingle(tmpBuff, 0x10);
                mat.M22 = BitConverter.ToSingle(tmpBuff, 0x14);
                mat.M23 = BitConverter.ToSingle(tmpBuff, 0x18);
                mat.M24 = 0;

                mat.M31 = BitConverter.ToSingle(tmpBuff, 0x1C);
                mat.M32 = BitConverter.ToSingle(tmpBuff, 0x20);
                mat.M33 = BitConverter.ToSingle(tmpBuff, 0x24);
                mat.M34 = 0;

                mat.M41 = BitConverter.ToSingle(tmpBuff, 0x28);
                mat.M42 = BitConverter.ToSingle(tmpBuff, 0x2C);
                mat.M43 = BitConverter.ToSingle(tmpBuff, 0x30);
                mat.M44 = 1;

                sceneryIndex = BitConverter.ToInt16(tmpBuff, 0x34);
                tmpBuff = null;
            }

            #endregion
        }

        /// <summary>
        /// The unknown chunk.
        /// </summary>
        /// <remarks></remarks>
        public class UnknownChunk
        {
            #region Constants and Fields

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
            /// The raw data chunk info.
            /// </summary>
            public ParsedModel.RawDataMetaChunk.RawDataOffsetChunk[] RawDataChunkInfo;

            /// <summary>
            /// The vertice count.
            /// </summary>
            public int VerticeCount;

            /// <summary>
            /// The vertices.
            /// </summary>
            public List<Vector3> Vertices = new List<Vector3>();

            /// <summary>
            /// The facecount.
            /// </summary>
            public int facecount;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            /// Initializes a new instance of the <see cref="UnknownChunk"/> class.
            /// </summary>
            /// <param name="offset">The offset.</param>
            /// <param name="chunknumber">The chunknumber.</param>
            /// <param name="meta">The meta.</param>
            /// <remarks></remarks>
            public UnknownChunk(int offset, int chunknumber, ref Meta meta)
            {
                

                BinaryReader BR = new BinaryReader(meta.MS);
                BR.BaseStream.Position = offset;

                BR.BaseStream.Position = offset + 8;
                this.HeaderSize = BR.ReadInt32() + 8;

                #region LoadRawDataChunkInfo

                BR.BaseStream.Position = offset + 16;
                int tempc = BR.ReadInt32();
                int tempr = BR.ReadInt32() - meta.magic - meta.offset;
                this.RawDataChunkInfo = new ParsedModel.RawDataMetaChunk.RawDataOffsetChunk[tempc];
                for (int x = 0; x < tempc; x++)
                {
                    this.RawDataChunkInfo[x] = new ParsedModel.RawDataMetaChunk.RawDataOffsetChunk();
                    BR.BaseStream.Position = tempr + (x * 16) + 6;
                    this.RawDataChunkInfo[x].ChunkSize = BR.ReadUInt16();
                    this.RawDataChunkInfo[x].Size = BR.ReadInt32();
                    if (this.RawDataChunkInfo[x].ChunkSize == 0)
                    {
                        this.RawDataChunkInfo[x].ChunkSize = this.RawDataChunkInfo[x].Size;
                    }

                    this.RawDataChunkInfo[x].Offset = BR.ReadInt32();
                    this.RawDataChunkInfo[x].ChunkCount = this.RawDataChunkInfo[x].Size /
                                                          this.RawDataChunkInfo[x].ChunkSize;
                }

                if (this.RawDataChunkInfo.Length == 0)
                {
                    return;
                }

                #endregion

                

                #region RawData

                int firstbsp2chunk = 0;

                #region FindFirstRawData

                for (int x = 0; x < meta.raw.rawChunks.Count; x++)
                {
                    if (meta.raw.rawChunks[x].rawDataType == RawDataType.bsp3)
                    {
                        firstbsp2chunk = x;
                        break;
                    }
                }

                #endregion

                BR = new BinaryReader(meta.raw.rawChunks[firstbsp2chunk + chunknumber].MS);

                #region DetermineChunkNumers

                int indicechunk = 0;
                int verticechunk = 0;

                for (int x = 0; x < RawDataChunkInfo.Length; x++)
                {
                    if (RawDataChunkInfo[x].ChunkSize == 32)
                    {
                        verticechunk = x;
                        indicechunk = x + 1;
                        break;
                    }
                }

                #endregion

                #region Faces

                // BR.BaseStream.Position = 88;
                this.IndiceCount = this.RawDataChunkInfo[indicechunk].ChunkCount;
                this.Indices = new short[this.IndiceCount];
                BR.BaseStream.Position = this.HeaderSize + this.RawDataChunkInfo[indicechunk].Offset;
                for (int x = 0; x < this.IndiceCount; x++)
                {
                    this.Indices[x] = (short)BR.ReadUInt16();
                }

                #endregion

                #region Vertices

                this.VerticeCount = this.RawDataChunkInfo[verticechunk].Size / 32;

                for (int x = 0; x < this.VerticeCount; x++)
                {
                    Vector3 vec = new Vector3();
                    BR.BaseStream.Position = this.HeaderSize + this.RawDataChunkInfo[verticechunk].Offset +
                                             (this.RawDataChunkInfo[verticechunk].ChunkSize * x);
                    vec.X = BR.ReadSingle();
                    vec.Y = BR.ReadSingle();
                    vec.Z = BR.ReadSingle();
                    Vertices.Add(vec);
                }

                #endregion
            }

            #endregion

            #endregion
        }

        /// <summary>
        /// The cluster pvs.
        /// </summary>
        /// <remarks></remarks>
        public class ClusterPVS
        {
            #region Constants and Fields

            /// <summary>
            /// The back visible clusters.
            /// </summary>
            public List<int> BackVisibleClusters = new List<int>();

            /// <summary>
            /// The front visible clusters.
            /// </summary>
            public List<int> FrontVisibleClusters = new List<int>();

            #endregion

            #region Constructors and Destructors

            /// <summary>
            /// Initializes a new instance of the <see cref="ClusterPVS"/> class.
            /// </summary>
            /// <param name="BR">The BR.</param>
            /// <param name="ClusterCount">The cluster count.</param>
            /// <remarks></remarks>
            public ClusterPVS(ref BinaryReader BR, int ClusterCount)
            {
                int bytecount = ClusterCount / 8;
                if (ClusterCount % 8 != 0)
                {
                    bytecount++;
                }

                int paddedtobytecount = bytecount;
                if (paddedtobytecount % 4 != 0)
                {
                    paddedtobytecount += paddedtobytecount % 4;
                }

                int currcount = 0;
                while (currcount != -1)
                {
                    byte temp = BR.ReadByte();
                    bool done = false;
                    for (int x = 0; x < 8; x++)
                    {
                        byte tempbit = (byte)(temp & 0x80);
                        if (tempbit == 0x80)
                        {
                            FrontVisibleClusters.Add(ClusterCount - currcount - 1);
                        }

                        temp = (byte)(temp << 1);
                        currcount += 1;
                        if (currcount == ClusterCount)
                        {
                            done = true;
                            break;
                        }
                    }

                    if (done)
                    {
                        break;
                    }
                }

                int difference = paddedtobytecount - bytecount;
                while (difference > 1)
                {
                    BR.ReadByte();
                    difference--;
                }

                currcount = 0;
                while (currcount != -1)
                {
                    byte temp = BR.ReadByte();
                    bool done = false;
                    for (int x = 0; x < 8; x++)
                    {
                        byte tempbit = (byte)(temp & 0x80);
                        if (tempbit == 0x80)
                        {
                            BackVisibleClusters.Add(ClusterCount - currcount - 1);
                        }

                        temp = (byte)(temp << 1);
                        currcount += 1;
                        if (currcount == ClusterCount)
                        {
                            done = true;
                            break;
                        }
                    }

                    if (done)
                    {
                        break;
                    }
                }

                while (difference > 1)
                {
                    BR.ReadByte();
                    difference--;
                }
            }

            #endregion
        }

        /// <summary>
        /// The clusters pvs.
        /// </summary>
        /// <remarks></remarks>
        public sealed class ClustersPVS
        {
            #region Public Methods

            /// <summary>
            /// The get clusters pvs info.
            /// </summary>
            /// <param name="meta">The meta.</param>
            /// <returns></returns>
            /// <remarks></remarks>
            public static ClusterPVS[] GetClustersPVSInfo(ref Meta meta)
            {
                BinaryReader BR = new BinaryReader(meta.MS);
                BR.BaseStream.Position = 172;
                int clustercount = BR.ReadInt32();

                BR.BaseStream.Position = 100;
                int tempc = BR.ReadInt32();
                int tempr = BR.ReadInt32() - meta.magic - meta.offset;
                BR.BaseStream.Position = tempr;
                ClusterPVS[] tempclusters = new ClusterPVS[clustercount];
                for (int x = 0; x < clustercount; x++)
                {
                    tempclusters[x] = new ClusterPVS(ref BR, clustercount);
                }

                return tempclusters;
            }

            #endregion
        }
    }
}