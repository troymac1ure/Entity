// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BSPViewer.cs" company="">
//   
// </copyright>
// <summary>
//   The bsp viewer.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace entity.Renderers
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.IO;
    using System.Windows.Forms;

    using entity;

    using Globals;

    using HaloMap.H2MetaContainers;
    using HaloMap.Map;
    using HaloMap.Meta;
    using HaloMap.RawData;
    using HaloMap.Render;

    using Microsoft.DirectX;
    using Microsoft.DirectX.Direct3D;
    using Microsoft.DirectX.DirectInput;

    using HaloMap;

    /// <summary>
    /// The bsp viewer.
    /// </summary>
    /// <remarks></remarks>
    public partial class BSPViewer : Form
    {
        #region Constants and Fields

        /// <summary>
        /// The cam.
        /// </summary>
        public Camera2 cam;

        /// <summary>
        /// The visible spawns bit mask.
        /// </summary>
        public int visibleSpawnsBitMask;

        /// <summary>
        /// The map.
        /// </summary>
        private readonly Map map;

        /// <summary>
        /// The render.
        /// </summary>
        private readonly Renderer render = new Renderer();

        /// <summary>
        /// The black material.
        /// </summary>
        private Material BlackMaterial;

        /// <summary>
        /// The blue material.
        /// </summary>
        private Material BlueMaterial;

        /// <summary>
        /// The bounding box model.
        /// </summary>
        private Mesh[] BoundingBoxModel;

        /// <summary>
        /// The brown material.
        /// </summary>
        private Material BrownMaterial;

        /// <summary>
        /// The default material.
        /// </summary>
        private Material DefaultMaterial;

        /// <summary>
        /// The green material.
        /// </summary>
        private Material GreenMaterial;

        /// <summary>
        /// The halo light count.
        /// </summary>
        private int HaloLightCount = 1;

        /// <summary>
        /// The light map_ array.
        /// </summary>
        private byte[] LightMap_Array;

        /// <summary>
        /// The light map_ array_ backup.
        /// </summary>
        private byte[] LightMap_Array_Backup;

        /// <summary>
        /// The neutral material.
        /// </summary>
        private Material NeutralMaterial;

        /// <summary>
        /// The orange material.
        /// </summary>
        private Material OrangeMaterial;

        /// <summary>
        /// The pink material.
        /// </summary>
        private Material PinkMaterial;

        /// <summary>
        /// The purple material.
        /// </summary>
        private Material PurpleMaterial;

        /// <summary>
        /// The red material.
        /// </summary>
        private Material RedMaterial;

        /// <summary>
        /// The time.
        /// </summary>
        private TimeSpan Time;

        /// <summary>
        /// The translation matrix.
        /// </summary>
        private Matrix[] TranslationMatrix;

        /// <summary>
        /// The world transform.
        /// </summary>
        private Matrix WorldTransform = Matrix.Identity;

        /// <summary>
        /// The yellow material.
        /// </summary>
        private Material YellowMaterial;

        /// <summary>
        /// The aspect.
        /// </summary>
        private float aspect = 1f;

        /// <summary>
        /// The axis.
        /// </summary>
        private Gizmo.axis axis;

        /// <summary>
        /// The bsp.
        /// </summary>
        private BSPModel bsp;

        /// <summary>
        /// The current object.
        /// </summary>
        private int currentObject;

        /// <summary>
        /// The gizmo.
        /// </summary>
        private Gizmo gizmo;

        /// <summary>
        /// The in sizing.
        /// </summary>
        private bool inSizing = true;

        /// <summary>
        /// The itemrotate.
        /// </summary>
        private bool itemrotate;

        /// <summary>
        /// The light vector.
        /// </summary>
        private Vector3 lightVector = new Vector3(0.0f, 0.0f, 1.0f);

        /// <summary>
        /// The oldx.
        /// </summary>
        private int oldx;

        /// <summary>
        /// The oldy.
        /// </summary>
        private int oldy;

        /// <summary>
        /// The rotation bit mask.
        /// </summary>
        private int rotationBitMask;

        /// <summary>
        /// The selected spawn type.
        /// </summary>
        private SpawnInfo.SpawnType selectedSpawnType;

        /// <summary>
        /// The selection depth.
        /// </summary>
        private float selectionDepth;

        /// <summary>
        /// The selection height.
        /// </summary>
        private float selectionHeight;

        /// <summary>
        /// The selection mesh.
        /// </summary>
        private Mesh selectionMesh;

        /// <summary>
        /// The selection multi.
        /// </summary>
        private bool selectionMulti;

        /// <summary>
        /// The selection start.
        /// </summary>
        private Vector3 selectionStart;

        /// <summary>
        /// The selection width.
        /// </summary>
        private float selectionWidth;

        /// <summary>
        /// The shaderx.
        /// </summary>
        private DXShader shaderx;

        /// <summary>
        /// The spawnmodelindex.
        /// </summary>
        private int[] spawnmodelindex;

        /// <summary>
        /// The spawns.
        /// </summary>
        private SpawnLoads spawns;

        /// <summary>
        /// The trackint 1.
        /// </summary>
        private int trackint1;

        /// <summary>
        /// The trackint 2.
        /// </summary>
        private int trackint2;

        /// <summary>
        /// The trackint 3.
        /// </summary>
        private int trackint3;

        /// <summary>
        /// The update xyzypr.
        /// </summary>
        private bool updateXYZYPR = true;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BSPViewer"/> class.
        /// </summary>
        /// <param name="tempbsp">The tempbsp.</param>
        /// <param name="map">The map.</param>
        /// <remarks></remarks>
        public BSPViewer(BSPModel tempbsp, Map map)
        {
            // InitializeComponent
            InitializeComponent();

            dockControl1.LayoutSystem.Collapsed = true;
            dockControl2.LayoutSystem.Collapsed = true;
            dockControl3.LayoutSystem.Collapsed = true;
            dockControl4.LayoutSystem.Collapsed = true;
            dockControl5.LayoutSystem.Collapsed = true;

            Application.DoEvents();

            // Center label horizontal
            int tempnum1 = 155 / 2;
            int tempnum2 = Screen.PrimaryScreen.WorkingArea.Width / 2;
            int tempnum3 = tempnum2 - tempnum1;
            label3.Left = tempnum3;

            // Center label vertical
            int tempnum4 = 28 / 2;
            int tempnum5 = Screen.PrimaryScreen.WorkingArea.Height / 2;
            int tempnum6 = tempnum5 - tempnum4 - 30 - 25;
            label3.Top = tempnum6;

            this.BackColor = Color.Blue;

            #region Clear the labels
            toolStripLabel2.Text = "Camera Position: X: 0 • Y: 0 • Z: 0";
            tsLabel1.Text = "Type: <";
            tsButtonType.Text = string.Empty;
            tsLabel2.Text = "> (";
            tsLabelCount.Text = string.Empty;
            tsLabelX.Text = ") • X: ";
            tsTextBoxX.Text = string.Empty;
            tsLabelY.Text = " • Y: ";
            tsTextBoxY.Text = string.Empty;
            tsLabelZ.Text = " • Z: ";
            tsTextBoxZ.Text = string.Empty;

            tsLabelYaw.Text = string.Empty;
            tsTextBoxYaw.Text = string.Empty;
            tsLabelPitch.Text = string.Empty;
            tsTextBoxPitch.Text = string.Empty;
            tsLabelRoll.Text = string.Empty;
            tsTextBoxRoll.Text = string.Empty;
            #endregion

            // Set the initial size of our form
            // this.ClientSize = new System.Drawing.Size(800, 600);
            // And its caption
            this.Text = "BSP Viewer (" + tempbsp.Name + ")";

            Application.DoEvents();

            bsp = tempbsp;
            this.map = map;

            

            this.MouseDown += BSPViewer_MouseDown;
            this.MouseUp += BSPViewer_MouseUp;
            this.MouseMove += this.ModelViewer_MouseDown;
            this.MouseUp += this.ModelViewer_MouseUp;            

            #region VisibleSpawns

            string[] strings = Enum.GetNames(typeof(SpawnInfo.SpawnType));

            treeView1.Sorted = true;
            treeView1.ShowNodeToolTips = true;

            int CameraCount = 0;
            int DeathZoneCount = 0;
            int ObjectiveCount = 0;
            int PlayerCount = 0;

            foreach (string s in strings)
            {
                // Add the type the the CheckListBox
                checkedListBox1.Items.Add(s);

                // Add the type to the treeview
                TreeNode tn = new TreeNode();
                for (int i = 0; i < bsp.Spawns.Spawn.Count; i++)
                {
                    if (s == bsp.Spawns.Spawn[i].Type.ToString())
                    {
                        TreeNode tn2 = new TreeNode();
                        tn2.Text = string.Empty;
                        tn2.ToolTipText = " X: " + bsp.Spawns.Spawn[i].X.ToString("#0.0##").PadRight(9) + "  Y: " +
                                          bsp.Spawns.Spawn[i].Y.ToString("#0.0##").PadRight(9) + "  Z: " +
                                          bsp.Spawns.Spawn[i].Z.ToString("#0.0##").PadRight(9);

                        if (bsp.Spawns.Spawn[i].Type.ToString() == "Collection")
                        {
                            switch (map.HaloVersion)
                            {
                                case HaloVersionEnum.Halo2:
                                case HaloVersionEnum.Halo2Vista:
                                    string[] temps = bsp.Spawns.Spawn[i].TagPath.Split('\\');
                                    tn2.Text = temps[temps.Length - 1];
                                    break;
                                case HaloVersionEnum.Halo1:
                                case HaloVersionEnum.HaloCE:
                                    SpawnInfo.H1Collection tempspawnx = (SpawnInfo.H1Collection)bsp.Spawns.Spawn[i];
                                    tn2.Text = tempspawnx.TagPath;
                                    break;
                            }
                        }
                            
                            // else if (bsp.Spawns.Spawn[i].Type.ToString() == "Obstacle") {}
                            // else if (bsp.Spawns.Spawn[i].Type.ToString() == "Weapon")   {}
                            // else if (bsp.Spawns.Spawn[i].Type.ToString() == "Scenery")  {}
                            // else if (bsp.Spawns.Spawn[i].Type.ToString() == "Machine")  {}
                            // else if (bsp.Spawns.Spawn[i].Type.ToString() == "Control")  {}
                            // else if (bsp.Spawns.Spawn[i].Type.ToString() == "Biped")    {}
                            // else if (bsp.Spawns.Spawn[i].Type.ToString() == "Equipment"){}
                            // else if (bsp.Spawns.Spawn[i].Type.ToString() == "Vehicle")  {}
                            // else if (bsp.Spawns.Spawn[i].Type.ToString() == "Light")    {}
                            // else if (bsp.Spawns.Spawn[i].Type.ToString() == "Sound")    {}
                        else if (bsp.Spawns.Spawn[i].Type.ToString() == "Camera")
                        {
                            tn2.Text = bsp.Spawns.Spawn[i].Type + " {" + CameraCount + "}";
                            CameraCount += 1;
                        }
                        else if (bsp.Spawns.Spawn[i].Type.ToString() == "DeathZone")
                        {
                            SpawnInfo.DeathZone tempspawn = (SpawnInfo.DeathZone)bsp.Spawns.Spawn[i];
                            tn2.Text = tempspawn.Name;
                            tn2.ToolTipText += "\n Length: " + tempspawn.length.ToString("#0.0##") + "  Width: " +
                                               tempspawn.width.ToString("#0.0##") + " Height: " +
                                               tempspawn.height.ToString("#0.0##");
                            DeathZoneCount += 1;
                        }
                        else if (bsp.Spawns.Spawn[i].Type.ToString() == "Objective")
                        {
                            SpawnInfo.ObjectiveSpawn tempspawn = (SpawnInfo.ObjectiveSpawn)bsp.Spawns.Spawn[i];

                            // tn2.Text = bsp.Spawns.Spawn[i].Type.ToString() + " {" + ObjectiveCount.ToString() + "}";
                            tn2.Text = tempspawn.ObjectiveType + " (" + tempspawn.Team + ") #" + tempspawn.number;
                            tn2.ToolTipText += "\n Type: " + tempspawn.ObjectiveType + "  #" + tempspawn.number +
                                               "\n Team: " + tempspawn.Team;
                            ObjectiveCount += 1;
                        }
                        else if (bsp.Spawns.Spawn[i].Type.ToString() == "Player")
                        {
                            tn2.Text = bsp.Spawns.Spawn[i].Type + " {" + PlayerCount + "}";
                            PlayerCount += 1;
                        }
                        else
                        {
                            // tn2.Text = bsp.Spawns.Spawn[i].Type.ToString();
                            string[] temps = bsp.Spawns.Spawn[i].TagPath.Split('\\');
                            tn2.Text = temps[temps.Length - 1];
                        }

                        if (map.HaloVersion == HaloVersionEnum.Halo2 ||
                            map.HaloVersion == HaloVersionEnum.Halo2Vista)
                        {
                            #region BasicInfo For YawPitchRoll Rotations

                            if (bsp.Spawns.Spawn[i] is SpawnInfo.RotateYawPitchRollBaseSpawn)
                            {
                                SpawnInfo.RotateYawPitchRollBaseSpawn tempspawn =
                                    (SpawnInfo.RotateYawPitchRollBaseSpawn)bsp.Spawns.Spawn[i];
                                if (tn2.Text == null)
                                {
                                    string[] temps = tempspawn.TagPath.Split('\\');
                                    tn2.Text = temps[temps.Length - 1];
                                }

                                tn2.ToolTipText += "\n Yaw: " + tempspawn.Yaw.ToString("#0.0##") + "  Pitch: " +
                                                   tempspawn.Pitch.ToString("#0.0##") + " Roll: " +
                                                   tempspawn.Roll.ToString("#0.0##");
                            }

                                #endregion
                                #region BasicInfo For One Rotation
                            else if (bsp.Spawns.Spawn[i] is SpawnInfo.RotateDirectionBaseSpawn)
                            {
                                SpawnInfo.RotateDirectionBaseSpawn tempspawn =
                                    (SpawnInfo.RotateDirectionBaseSpawn)bsp.Spawns.Spawn[i];
                                if (tn2.Text == null)
                                {
                                    if (tempspawn.TagPath != null)
                                    {
                                        string[] temps = tempspawn.TagPath.Split('\\');
                                        tn2.Text = temps[temps.Length - 1];
                                    }
                                    else
                                    {
                                        tn2.Text = tempspawn.Type.ToString();
                                    }
                                }

                                tn2.ToolTipText += "\n Rotation: " + tempspawn.RotationDirection.ToString("#0.0##");
                            }

                            #endregion
                        }

                        tn2.Tag = i;
                        tn.Nodes.Add(tn2);
                    }
                }

                tn.Text = s;
                tn.Tag = -1;
                treeView1.Nodes.Add(tn);
            }

            #endregion

            #region Lightmaps

            Load_Lightmaps_Into_An_Array();
            int temppicboxX = 0;
            int temppicboxY = 0;
            int tempintforme = 0;
            for (int i = 0; i < map.BSP.sbsp[tempbsp.BspNumber].LightMap_Palettes.Count; i++)
            {
                PictureBox temppicbox = new PictureBox();
                temppicbox.Image = RenderLightmap(i);
                temppicbox.SizeMode = PictureBoxSizeMode.StretchImage;
                temppicbox.Width = 64;
                temppicbox.Height = 64;
                temppicbox.Location = new Point(temppicboxX, temppicboxY);
                temppicbox.Tag = i;
                temppicbox.Click += this.PictureBox_Click;
                panel1.Controls.Add(temppicbox);
                comboBox1.Items.Add(i);
                if (tempintforme <= 1)
                {
                    tempintforme += 1;
                    temppicboxX += 68;
                }
                else
                {
                    tempintforme = 0;
                    temppicboxX = 0;
                    temppicboxY += 68;
                }
            }

            #endregion

            if (map.HaloVersion == HaloVersionEnum.Halo2 ||
                map.HaloVersion == HaloVersionEnum.Halo2Vista)
            {
                this.NoCulling.Checked = false;
            }
            else
            {
                this.NoCulling.Checked = true;
            }

            Main();
        }

        #endregion

        #region Enums

        /// <summary>
        /// The selected item rotation type.
        /// </summary>
        /// <remarks></remarks>
        [FlagsAttribute]
        public enum SelectedItemRotationType
        {
            /// <summary>
            /// The none.
            /// </summary>
            None = 0x00, 

            /// <summary>
            /// The control.
            /// </summary>
            Control = 0x01, 

            /// <summary>
            /// The shift.
            /// </summary>
            Shift = 0x02, 

            /// <summary>
            /// The alt.
            /// </summary>
            Alt = 0x04
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// The h 2 bsp write raw data meta chunk.
        /// </summary>
        /// <param name="chunknumber">The chunknumber.</param>
        /// <param name="bsp">The bsp.</param>
        /// <param name="BSPNum">The bsp num.</param>
        /// <remarks></remarks>
        public void H2BSPWriteRawDataMetaChunk(int chunknumber, ref BSPModel bsp, int BSPNum)
        {
            BSPModel.BSPRawDataMetaChunk bspChunk = bsp.BSPRawDataMetaChunks[chunknumber];

            int BSPId = map.Functions.ForMeta.FindMetaByID(map.BSP.sbsp[BSPNum].ident);

            Meta meta = new Meta(map);
            meta.TagIndex = BSPId; // decides which tag to load into MemoryStream (MS)
            meta.ScanMetaItems(true, false);

            BinaryWriter BW = new BinaryWriter(meta.MS);
            BinaryReader BR = new BinaryReader(meta.MS);
            int temp = 0;

            BW.BaseStream.Position = bsp.BSPRawDataMetaChunksOffset;
            BW.Write(bspChunk.VerticeCount);
            BW.Write(bspChunk.FaceCount);
            BW.BaseStream.Position = bsp.BSPRawDataMetaChunksOffset + 48;
            BW.Write(bspChunk.HeaderSize - 8);

            BW.BaseStream.Position = bsp.BSPRawDataMetaChunksOffset + 52;
            temp = BR.ReadInt32(); // I dunno what this is or what value it is supposed to be! #1232
            BW.BaseStream.Position = bsp.BSPRawDataMetaChunksOffset + 52;
            BW.Write(temp); // I dunno what this is or what value it is supposed to be!

            BW.BaseStream.Position = bsp.BSPRawDataMetaChunksOffset + 56;
            BW.Write(bspChunk.RawDataChunkInfo.Length);

            BW.Write(bspChunk.rawdatainfooffset + meta.magic + meta.offset);

            // SO FAR SO GOOD UP TILL NOW!
            // this.RawDataChunkInfo = new RawDataOffsetChunk[tempc];
            int tempr = bspChunk.rawdatainfooffset;
            for (int x = 0; x < bspChunk.RawDataChunkInfo.Length; x++)
            {
                // this.RawDataChunkInfo[x] = new RawDataOffsetChunk();
                BW.BaseStream.Position = tempr + (x * 16) + 6;
                BW.Write((UInt16)bspChunk.RawDataChunkInfo[x].ChunkSize);
                BW.Write(bspChunk.RawDataChunkInfo[x].Size);

                // I think this may be a safety for a messed up map?
                // if (bspChunk.RawDataChunkInfo[x].ChunkSize == bspChunk.RawDataChunkInfo[x].Size) { bspChunk.RawDataChunkInfo[x].ChunkSize = 0; }
                BW.Write(bspChunk.RawDataChunkInfo[x].Offset);
            }

            BW = map.BW;
            BW.BaseStream.Position = meta.offset;
            BW.Write(meta.MS.GetBuffer(), 0, meta.size);

            if (bspChunk.RawDataChunkInfo.Length == 0)
            {
                return;
            }

            /*
                BW = new BinaryWriter(((RawDataChunk)meta.raw.rawChunks[chunknumber]).MS);
                bspChunk.SubMeshInfo = new ModelSubMeshInfo[bspChunk.RawDataChunkInfo[0].ChunkCount];
                for (int x = 0; x < bspChunk.RawDataChunkInfo[0].ChunkCount; x++)
                {
                    bspChunk.SubMeshInfo[x] = new ModelSubMeshInfo();
                    BR.BaseStream.Position = bspChunk.HeaderSize + bspChunk.RawDataChunkInfo[0].Offset + (x * 72) + 4;
                    bspChunk.SubMeshInfo[x].ShaderNumber = BR.ReadUInt16();
                    bspChunk.SubMeshInfo[x].IndiceStart = BR.ReadUInt16();
                    bspChunk.SubMeshInfo[x].IndiceCount = BR.ReadUInt16();

                }

                BR.BaseStream.Position = 40;
                bspChunk.IndiceCount = BR.ReadUInt16();
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
                bspChunk.Indices = new short[bspChunk.RawDataChunkInfo[indicechunk].ChunkCount];
                BR.BaseStream.Position = bspChunk.HeaderSize + bspChunk.RawDataChunkInfo[indicechunk].Offset;
                for (int x = 0; x < bspChunk.IndiceCount; x++)
                {
                    bspChunk.Indices[x] = (short)BR.ReadUInt16();

                }

                bspChunk.RawDataChunkInfo[verticechunk].ChunkSize = bspChunk.RawDataChunkInfo[verticechunk].Size / VerticeCount;
                for (int x = 0; x < bspChunk.VerticeCount; x++)
                {
                    Vector3 vec = new Vector3();
                    BR.BaseStream.Position = bspChunk.HeaderSize + bspChunk.RawDataChunkInfo[verticechunk].Offset + (bspChunk.RawDataChunkInfo[verticechunk].ChunkSize * x);
                    vec.X = BR.ReadSingle();
                    vec.Y = BR.ReadSingle();
                    vec.Z = BR.ReadSingle();
                    Vertices.Add(vec);
                }

                bspChunk.RawDataChunkInfo[uvchunk].ChunkSize = 8;
                for (int x = 0; x < bspChunk.VerticeCount; x++)
                {
                    Vector2 tempuv = new Vector2();
                    BR.BaseStream.Position = bspChunk.HeaderSize + bspChunk.RawDataChunkInfo[uvchunk].Offset + (bspChunk.RawDataChunkInfo[uvchunk].ChunkSize * x);
                    tempuv.X = BR.ReadSingle();
                    tempuv.Y = BR.ReadSingle();
                    bspChunk.UVs.Add(tempuv);
                }

                bspChunk.RawDataChunkInfo[normalchunk].ChunkSize = 12;
                for (int x = 0; x < bspChunk.VerticeCount; x++)
                {
                    Vector2 tempuv = new Vector2();
                    BR.BaseStream.Position = bspChunk.HeaderSize + bspChunk.RawDataChunkInfo[normalchunk].Offset + (bspChunk.RawDataChunkInfo[normalchunk].ChunkSize * x);
                    Vector3 normal = Raw.ParsedModel.DecompressNormal(BR.ReadInt32());
                    bspChunk.Normals.Add(normal);
                    Vector3 binormal = Raw.ParsedModel.DecompressNormal(BR.ReadInt32());
                    bspChunk.Binormals.Add(binormal);
                    Vector3 tangent = Raw.ParsedModel.DecompressNormal(BR.ReadInt32());
                    bspChunk.Tangents.Add(tangent);
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

                if (lightmapuvchunk == -1) return;
                RawDataChunkInfo[lightmapuvchunk].ChunkSize = 4;
                for (int x = 0; x < bspChunk.VerticeCount; x++)
                {
                    Vector2 tempuv = new Vector2();
                    BR.BaseStream.Position = bspChunk.HeaderSize + bspChunk.RawDataChunkInfo[lightmapuvchunk].Offset + (bspChunk.RawDataChunkInfo[lightmapuvchunk].ChunkSize * x);
                    short testx = BR.ReadInt16();
                    float u = DecompressVertice(Convert.ToSingle(testx), -1, 1);
                    testx = BR.ReadInt16();
                    float v = DecompressVertice(Convert.ToSingle(testx), -1, 1);
                     Vector2 uv2 = new Vector2(u, v);
                    bspChunk.LightMapUVs.Add(uv2);
                }
            */
        }

        /// <summary>
        /// The initialize graphics.
        /// </summary>
        /// <returns>The initialize graphics.</returns>
        /// <remarks></remarks>
        public bool InitializeGraphics()
        {
            // try
            // {
            this.Show();
            this.Focus();
            Application.DoEvents();
           
            render.CreateDevice(this);
            render.pause = true;

            #region CreateHaloDirectXResources

            #region List of all BSP sections

            checkedListBox2.Enabled = false;
            checkedListBox2.Items.Clear();

            for (int x = 0; x < bsp.BSPRawDataMetaChunks.Length; x++)
            {
                checkedListBox2.Items.Add("BSP #" + x.ToString() + " (" + (bsp.BSPRawDataMetaChunks[x].FaceCount).ToString() + " Faces)", true);
            }

            checkedListBox2.Enabled = true;

            #endregion

            this.label3.Text = ".:Loading Textures & Skybox:.";
            this.label3.Refresh();

            BSPModel.BSPDisplayedInfo.LoadDirectXTexturesAndBuffers(ref render.device, ref bsp);
            ParsedModel.DisplayedInfo.LoadDirectXTexturesAndBuffers(ref render.device, ref bsp.SkyBox);
            LoadSpawns();
            MakeMatrixes();

            #endregion

            #region InitializeMaterials

            RedMaterial = new Material();
            RedMaterial.Diffuse = Color.Red;
            RedMaterial.Ambient = Color.Red;
            BlueMaterial = new Material();
            BlueMaterial.Diffuse = Color.Blue;
            BlueMaterial.Ambient = Color.Blue;
            YellowMaterial = new Material();
            YellowMaterial.Diffuse = Color.Yellow;
            YellowMaterial.Ambient = Color.Yellow;
            GreenMaterial = new Material();
            GreenMaterial.Diffuse = Color.SpringGreen;
            GreenMaterial.Ambient = Color.SpringGreen;
            PurpleMaterial = new Material();
            PurpleMaterial.Diffuse = Color.BlueViolet;
            PurpleMaterial.Ambient = Color.BlueViolet;
            OrangeMaterial = new Material();
            OrangeMaterial.Diffuse = Color.Orange;
            OrangeMaterial.Ambient = Color.Orange;
            BrownMaterial = new Material();
            BrownMaterial.Diffuse = Color.Chocolate;
            BrownMaterial.Ambient = Color.Chocolate;
            PinkMaterial = new Material();
            PinkMaterial.Diffuse = Color.Pink;
            PinkMaterial.Ambient = Color.Pink;
            NeutralMaterial = new Material();
            NeutralMaterial.Diffuse = Color.GreenYellow;
            NeutralMaterial.Ambient = Color.GreenYellow;
            DefaultMaterial = new Material();
            DefaultMaterial.Diffuse = Color.Crimson;
            DefaultMaterial.Ambient = Color.Crimson;
            BlackMaterial = new Material();
            BlackMaterial.Diffuse = Color.Black;
            BlackMaterial.Ambient = Color.Black;
            BlackMaterial.Specular = Color.Black;

            #endregion

            // gizmo = new Entity.Renderer.Widget.Gizmo(render.device);

            render.pause = false;
            label3.Visible = false;
            
            return true;

            // }
            // catch (DirectXException)
            // {
            // Catch any errors and return a failure
            // 	return false;
            // 	}
        }

        /// <summary>
        /// The load spawns.
        /// </summary>
        /// <remarks></remarks>
        public void LoadSpawns()
        {
            spawns = new SpawnLoads(map, bsp, render.device);
            SpawnModel = new List<ParsedModel>();
            spawnmodelindex = new int[bsp.Spawns.Spawn.Count];
            BoundingBoxModel = new Mesh[bsp.Spawns.Spawn.Count];

            map.OpenMap(MapTypes.Internal);

            int blockCount = 0;
            int scenCount = 0;
            for (int x = 0; x < bsp.Spawns.Spawn.Count; x++)
            {
                // Display loading information
                if (x % 7 == 0)
                {
                    this.label3.Text = ".:Loading Spawns [" + x.ToString() + "/" + bsp.Spawns.Spawn.Count.ToString() + "]:.";
                    // Every 5 updates, refresh whole window, otherwise just update the label
                    if (x % 35 == 0)
                        Application.DoEvents();
                    else
                        this.label3.Refresh();
                }

                // This is the only way I could think of doing it right now...
                // Used for saving Obstacle & Scenery to their original places
                if (bsp.Spawns.Spawn[x] is SpawnInfo.ObstacleSpawn)
                {
                    ((SpawnInfo.ObstacleSpawn)bsp.Spawns.Spawn[x]).BlocNumber = blockCount++;
                }
                else if (bsp.Spawns.Spawn[x] is SpawnInfo.ScenerySpawn)
                {
                    ((SpawnInfo.ScenerySpawn)bsp.Spawns.Spawn[x]).ScenNumber = scenCount++;
                }

                if (bsp.Spawns.Spawn[x] is SpawnInfo.BoundingBoxSpawn)
                {
                    BoundingBoxModel[x] = loadBoundingBoxSpawn(bsp.Spawns.Spawn[x]);
                    continue;
                }

                #region CameraSpawn
                if (bsp.Spawns.Spawn[x] is SpawnInfo.CameraSpawn)
                {
                    BoundingBoxModel[x] = loadCameraSpawn(bsp.Spawns.Spawn[x]);
                    continue;
                }
                #endregion

                #region LightSpawn

                if (bsp.Spawns.Spawn[x] is SpawnInfo.LightSpawn)
                {
                    SpawnInfo.LightSpawn tempbox;
                    tempbox = bsp.Spawns.Spawn[x] as SpawnInfo.LightSpawn;
                    tempbox.LightInfo = new HaloLight(tempbox.ModelTagNumber, map);
                    bsp.Spawns.Spawn[x] = tempbox;
                    BoundingBoxModel[x] = Mesh.Cylinder(render.device, 0.5f, 0.0f, 1f, 10, 10);
                    if (render.lighting)
                    {
                        render.device.Lights[HaloLightCount].Type = LightType.Point;
                        render.device.Lights[HaloLightCount].Position = new Vector3(tempbox.X, tempbox.Y, tempbox.Z);
                        render.device.Lights[HaloLightCount].Direction = new Vector3(
                            -tempbox.Yaw, -tempbox.Pitch, -tempbox.Roll);
                        render.device.Lights[HaloLightCount].Range = 10f;

                        // render.device.Lights[HaloLightCount].=0.5f;
                        // render.device.Lights[HaloLightCount].p = 1.0f;
                        render.device.Lights[HaloLightCount].Falloff = 1.0f;
                        render.device.Lights[HaloLightCount].Attenuation0 = 1.0f;
                        render.device.Lights[HaloLightCount].Diffuse = Color.FromArgb(
                            tempbox.LightInfo.r, tempbox.LightInfo.g, tempbox.LightInfo.b);
                        render.device.Lights[HaloLightCount].Update();
                        render.device.Lights[HaloLightCount].Enabled = true;
                    }

                    HaloLightCount++;

                    continue;
                }

                #endregion

                #region SoundSpawn
                if (bsp.Spawns.Spawn[x] is SpawnInfo.SoundSpawn)
                {
                    BoundingBoxModel[x] = loadSoundSpawn(bsp.Spawns.Spawn[x]);
                    continue;
                }
                #endregion

                SpawnInfo.RotationSpawn tempspawn = bsp.Spawns.Spawn[x] as SpawnInfo.RotationSpawn;

                #region ScanForExistingModels
                bool found = false;
                for (int xx = 0; xx < x; xx++)
                {
                    SpawnInfo.RotationSpawn tempspawn2 = bsp.Spawns.Spawn[xx] as SpawnInfo.RotationSpawn;
                    if (bsp.Spawns.Spawn[xx] is SpawnInfo.BoundingBoxSpawn)
                    {
                        continue;
                    }

                    if (tempspawn.ModelTagNumber == tempspawn2.ModelTagNumber)
                    {
                        BoundingBoxModel[x] = BoundingBoxModel[xx];
                        spawnmodelindex[x] = spawnmodelindex[xx];
                        bsp.Spawns.Spawn[x].bbXDiff = bsp.Spawns.Spawn[xx].bbXDiff;
                        bsp.Spawns.Spawn[x].bbYDiff = bsp.Spawns.Spawn[xx].bbYDiff;
                        bsp.Spawns.Spawn[x].bbZDiff = bsp.Spawns.Spawn[xx].bbZDiff;
                        found = true;
                        break;
                    }
                }

                if (found)
                {
                    continue;
                }
                #endregion

                #region ReadSpawnMeta

                Meta m = new Meta(map);
                if (tempspawn.ModelTagNumber == -1)
                {
                    MessageBox.Show("Test");
                }

                m.ReadMetaFromMap(tempspawn.ModelTagNumber, false);

                #endregion

                #region DirectXModel

                ParsedModel pm = new ParsedModel(ref m);

                // pm.PermutationString=pm.hlmt.Permutations[pm.hlmt.FindPermutationByBaseClass,
                ParsedModel.DisplayedInfo.LoadDirectXTexturesAndBuffers(ref render.device, ref pm);
                SpawnModel.Add(pm);
                spawnmodelindex[x] = SpawnModel.Count - 1;
                m.Dispose();

                #endregion

                #region BoundingBox

                float boxwidth = pm.BoundingBox.MaxX - pm.BoundingBox.MinX;
                float boxheight = pm.BoundingBox.MaxY - pm.BoundingBox.MinY;
                float boxdepth = pm.BoundingBox.MaxZ - pm.BoundingBox.MinZ;
                try
                {
                    BoundingBoxModel[x] = Mesh.Box(render.device, boxwidth, boxheight, boxdepth);
                }
                catch (Exception ex)
                {
                    Global.ShowErrorMsg("Failure to create Bounding Box Mesh for " + pm.name +
                        "\nWidth : " + boxwidth.ToString() +
                        "\nHeight: " + boxheight.ToString() +
                        "\nLength: " + boxdepth.ToString(),
                        ex);
                }
                // Used for fixing position of bounding boxes
                bsp.Spawns.Spawn[x].bbXDiff = pm.BoundingBox.MaxX + pm.BoundingBox.MinX;
                bsp.Spawns.Spawn[x].bbYDiff = pm.BoundingBox.MaxY + pm.BoundingBox.MinY;
                bsp.Spawns.Spawn[x].bbZDiff = pm.BoundingBox.MaxZ + pm.BoundingBox.MinZ;

                #endregion
            }

            if (render.device.DeviceCaps.RasterCaps.SupportsFogTable && bsp.sky.fogenabled &&
                bsp.sky.fog.FogThickness != 0)
            {
                int a = (int)(bsp.sky.fog.A * 255);
                int r = (int)(bsp.sky.fog.R * 255);
                int g = (int)(bsp.sky.fog.G * 255);
                int b = (int)(bsp.sky.fog.B * 255);
                render.device.RenderState.FogColor = Color.FromArgb(a, r, g, b);
                render.device.RenderState.FogStart = bsp.sky.fog.Start;
                render.device.RenderState.FogEnd = bsp.sky.fog.End;
                render.device.RenderState.FogDensity = bsp.sky.fog.FogThickness; // bsp.sky.fog.FogThickness;

                render.device.RenderState.FogTableMode = FogMode.Linear;
                render.device.RenderState.FogEnable = true;
            }

            // render.device.RenderState.FogVertexMode = FogMode.Linear;

            /*
            this.label3.Text = ".:Loading Weapons Collection:.";
            this.ResumeLayout();
            this.SuspendLayout();
            #region LoadAllWeaponsForCollectionChangeBox
            WeaponsList.Clear();
            // Lists all weapons
            for (int i = 0; i < map.MetaInfo.TagType.Length; i++)
                if ((map.MetaInfo.TagType[i] == "itmc") ||
                    (map.MetaInfo.TagType[i] == "vehc"))
                {
                    CollectionInfo Weapon = new CollectionInfo();
                    Meta m = new Meta(map);
                    m.ReadMetaFromMap(i, map, false);

                    Weapon.ItmcTagNumber = i;
                    // Base address of ITMC tag, offset of WEAP pointer (+20)
                    map.BR.BaseStream.Position = map.MetaInfo.Offset[Weapon.ItmcTagNumber] + 20;
                    Weapon.WeapTagNumber = map.Functions.Meta.FindMetaByID(map.BR.ReadInt32(), map);
                    if (Weapon.WeapTagNumber == -1) { continue; }

                    // Base address of WEAP tag, offset of HLMT pointer (+56)
                    map.BR.BaseStream.Position = map.MetaInfo.Offset[Weapon.WeapTagNumber] + 56;
                    Weapon.HlmtTagNumber = map.Functions.Meta.FindMetaByID(map.BR.ReadInt32(), map);
                    if (Weapon.HlmtTagNumber != -1)
                    {
                        // Base address of HLMT tag, offset of MODE pointer (+4)
                        map.BR.BaseStream.Position = map.MetaInfo.Offset[Weapon.HlmtTagNumber] + 4;
                        Weapon.ModelTagNumber = map.Functions.Meta.FindMetaByID(map.BR.ReadInt32(), map);
                        m.ReadMetaFromMap(Weapon.ModelTagNumber, map, false);
                        Weapon.Model = new ParsedModel(ref m, map);
                        Raw.ParsedModel.DisplayedInfo.LoadDirectXTexturesAndBuffers(ref render.device, ref Weapon.Model);

                        // Store names into Weapon
                        Weapon.TagPath = map.FileNames.Name[i];
                        Weapon.TagType = map.MetaInfo.TagType[i];
                        int xx = map.Functions.Meta.FindByNameAndTagType(Weapon.TagType, Weapon.TagPath, map);
                        string[] NameSplit = map.FileNames.Name[xx].Split('\\');
                        Weapon.Name = NameSplit[NameSplit.Length - 1];
                        Weapon.Name = Weapon.Name.Replace('_', ' ');
                        WeaponsList.Add(Weapon);
                    }
                }
            #endregion
            #region LoadAllObjectsForObstacleAndSceneryChangeBox
            SceneryList.Clear();
            ObstacleList.Clear();
            // Lists all Scenery & Obstacles
            for (int i = 0; i < map.MapHeader.fileCount; i++)
            {
                if ((map.MetaInfo.TagType[i] == "scnr"))
                {
                    Meta m = new Meta(map);
                    //m.ReadMetaFromMap(i, map, false);

                    // Base address of SCNR tag, offset of Scenery Palette pointer (+88)
                    map.BR.BaseStream.Position = map.MetaInfo.Offset[i] + 88;
                    int chunkCount = map.BR.ReadInt32();
                    int chunkOffset = map.BR.ReadInt32() - map.SecondaryMagic;

                    #region Scenery Palette Objects
                    // Scenery Palette Objects
                    for (int a = 0; a < chunkCount; a++)
                    {
                        SceneryInfo Scenery = new SceneryInfo();

                        // The Palette Chunk #
                        Scenery.ScenPalNumber = a;

                        // Each chunk is 40 bytes apart
                        map.BR.BaseStream.Position = chunkOffset + a * 40;
                        char[] tagName = map.BR.ReadChars(4);
                        Scenery.ScenTagNumber = map.Functions.Meta.FindMetaByID(map.BR.ReadInt32(), map);

                        try
                        {
                            // Retrieve the Model HLMT tag from the Scenery tag (+56)
                            map.BR.BaseStream.Position = map.MetaInfo.Offset[Scenery.ScenTagNumber] + 56;
                            Scenery.HlmtTagNumber = map.Functions.Meta.FindMetaByID(map.BR.ReadInt32(), map);

                            // Base address of HLMT tag, offset of MODE pointer (+4)
                            map.BR.BaseStream.Position = map.MetaInfo.Offset[Scenery.HlmtTagNumber] + 4;
                            Scenery.ModelTagNumber = map.Functions.Meta.FindMetaByID(map.BR.ReadInt32(), map);

                            if (Scenery.ModelTagNumber != -1)
                            {
                                m.ReadMetaFromMap(Scenery.ModelTagNumber, map, false);
                                Scenery.Model = new ParsedModel(ref m, map);
                            }
                            else
                                Scenery.Model = null;
                            Raw.ParsedModel.DisplayedInfo.LoadDirectXTexturesAndBuffers(ref render.device, ref Scenery.Model);

                            string[] s = map.FileNames.Name[Scenery.ScenTagNumber].Split('\\');
                            Scenery.Name = s[s.Length - 1];
                            Scenery.TagPath = map.FileNames.Name[Scenery.ScenTagNumber];
                            Scenery.TagType = map.MetaInfo.TagType[Scenery.ScenTagNumber];
                            SceneryList.Add(Scenery);
                        }
                        catch { }
                    }
                    #endregion

                    // Base address of SCNR tag, offset of Sound Scenery Palette pointer (+224)
                    map.BR.BaseStream.Position = map.MetaInfo.Offset[i] + 224;
                    chunkCount = map.BR.ReadInt32();
                    chunkOffset = map.BR.ReadInt32() - map.SecondaryMagic;

                    #region Sound Scenery Palette Objects
                    // Scenery Palette Objects
                    for (int a = 0; a < chunkCount; a++)
                    {
                        SceneryInfo Sound = new SceneryInfo();

                        // The Palette Chunk #
                        Sound.ScenPalNumber = a;

                        // Each chunk is 40 bytes apart
                        map.BR.BaseStream.Position = chunkOffset + a * 40;
                        char[] tagName = map.BR.ReadChars(4);
                        Sound.ScenTagNumber = map.Functions.Meta.FindMetaByID(map.BR.ReadInt32(), map);

                        if (Sound.ScenTagNumber != -1)
                        {
                            string[] s = map.FileNames.Name[Sound.ScenTagNumber].Split('\\');
                            Sound.Name = s[s.Length - 1];
                            Sound.TagPath = map.FileNames.Name[Sound.ScenTagNumber];
                            Sound.TagType = map.MetaInfo.TagType[Sound.ScenTagNumber];
                            SoundsList.Add(Sound);
                        }
                    }
                    #endregion

                    // Base address of SCNR tag, offset of Crate Palette pointer (+816)
                    map.BR.BaseStream.Position = map.MetaInfo.Offset[i] + 816;
                    chunkCount = map.BR.ReadInt32();
                    chunkOffset = map.BR.ReadInt32() - map.SecondaryMagic;

                    #region Crate Palette Objects
                    // Crate (Obstacle) Palette Objects
                    for (int a = 0; a < chunkCount; a++)
                    {
                        SceneryInfo Obstacle = new SceneryInfo();

                        // The Palette Chunk #
                        Obstacle.ScenPalNumber = a;

                        // Each chunk is 40 bytes apart
                        map.BR.BaseStream.Position = chunkOffset + a * 40;
                        char[] tagName = map.BR.ReadChars(4);
                        Obstacle.ScenTagNumber = map.Functions.Meta.FindMetaByID(map.BR.ReadInt32(), map);
                        if (Obstacle.ScenTagNumber != -1)
                        {
                            // Retrieve the Model HLMT tag from the Scenery tag (+56)
                            map.BR.BaseStream.Position = map.MetaInfo.Offset[Obstacle.ScenTagNumber] + 56;
                            Obstacle.HlmtTagNumber = map.Functions.Meta.FindMetaByID(map.BR.ReadInt32(), map);

                            // Base address of HLMT tag, offset of MODE pointer (+4)
                            map.BR.BaseStream.Position = map.MetaInfo.Offset[Obstacle.HlmtTagNumber] + 4;
                            Obstacle.ModelTagNumber = map.Functions.Meta.FindMetaByID(map.BR.ReadInt32(), map);

                            m.ReadMetaFromMap(Obstacle.ModelTagNumber, map, false);
                            Obstacle.Model = new ParsedModel(ref m, map);
                            Raw.ParsedModel.DisplayedInfo.LoadDirectXTexturesAndBuffers(ref render.device, ref Obstacle.Model);

                            string[] s = map.FileNames.Name[Obstacle.ScenTagNumber].Split('\\');
                            Obstacle.Name = s[s.Length - 1];
                            Obstacle.TagPath = map.FileNames.Name[Obstacle.ScenTagNumber];
                            Obstacle.TagType = map.MetaInfo.TagType[Obstacle.ScenTagNumber];
                            ObstacleList.Add(Obstacle);
                        }
                    }
                    #endregion
                    break;
                }
            }
            #endregion
            */
            map.CloseMap();
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// <remarks></remarks>
        public void Main()
        {
            using (BSPViewer frm = this)
            {
                if (!frm.InitializeGraphics())
                {
                    // Initialize Direct3D
                    MessageBox.Show("Could not initialize Direct3D.  This tutorial will exit.");
                    return;
                }

                frm.Show();
                frm.Focus();
                cam = new Camera2(this); // Camera(device);

                // Position the camera at the center of the map
                setCameraPosition(
                        (bsp.maxBoundries.X - bsp.minBoundries.X) / 2 + bsp.minBoundries.X,
                        (bsp.maxBoundries.Y - bsp.minBoundries.Y) / 2 + bsp.minBoundries.Y,
                        (bsp.maxBoundries.Z - bsp.minBoundries.Z) / 2 + bsp.minBoundries.Z,
                        false);
                //cam.AimCamera(new Vector3(cam.x, cam.y - 15f, cam.z + 0.25f));
                //cam.ComputePosition();


                aspect = this.Width / (float)this.Height;
                this.speedBar_Update();

                // While the form is still valid, render and process messages
                while (frm.Created)
                {
                    if (NoCulling.Checked)
                    {
                        render.device.RenderState.CullMode = Cull.None;
                    }
                    else
                    {
                        render.device.RenderState.CullMode = Cull.Clockwise;
                    }

                    frm.Render();
                    Application.DoEvents();
                    GC.Collect(0);
                    GC.WaitForPendingFinalizers();
                    GC.Collect(0);

                    // System.Threading.Thread.Sleep(100);
                }
            }
        }

        /// <summary>
        /// The make matrix for spawn.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public Matrix MakeMatrixForSpawn(int x)
        {
            Matrix m = Matrix.Identity;

            

            if (bsp.Spawns.Spawn[x] is SpawnInfo.RotateDirectionBaseSpawn)
            {
                SpawnInfo.RotateDirectionBaseSpawn tempspawn;
                tempspawn = bsp.Spawns.Spawn[x] as SpawnInfo.RotateDirectionBaseSpawn;
                Matrix rotate = Matrix.Identity;
                rotate.RotateYawPitchRoll(0, 0, tempspawn.RotationDirection);

                // rotate.RotateYawPitchRoll(tempspawn.Yaw, tempspawn.Pitch, tempspawn.Roll);
                m.Multiply(rotate);
            }
                
                #region SpawnWithYawPitchRoll
            else if (bsp.Spawns.Spawn[x] is SpawnInfo.RotateYawPitchRollBaseSpawn)
            {
                SpawnInfo.RotateYawPitchRollBaseSpawn tempspawn;
                tempspawn = bsp.Spawns.Spawn[x] as SpawnInfo.RotateYawPitchRollBaseSpawn;

                Matrix rotate = Matrix.Identity;

                if (map.HaloVersion == HaloVersionEnum.Halo2 ||
                    map.HaloVersion == HaloVersionEnum.Halo2Vista)
                {
                    float tempf3 = tempspawn.Roll;

                    Matrix m1 = Matrix.Identity;
                    m1.RotateX(tempspawn.Yaw);
                    Matrix m2 = Matrix.Identity;
                    m2.RotateY(-tempspawn.Pitch); // Pitch is backwards in game
                    Matrix m3 = Matrix.Identity;
                    m3.RotateZ(tempspawn.Roll); // );

                    // Do NOT change the order! Finally this is right //
                    // (m3 * m2 * m1) != (m1 * m2 * m3) with matrix calculations
                    rotate = m3 * m2 * m1;
                }
                else
                {
                    rotate.RotateYawPitchRoll(tempspawn.Yaw, tempspawn.Pitch, tempspawn.Roll);
                }

                m = rotate;
            }

            #endregion

            m.Multiply(Matrix.Translation(bsp.Spawns.Spawn[x].X, bsp.Spawns.Spawn[x].Y, bsp.Spawns.Spawn[x].Z));
            return m;
        }

        /// <summary>
        /// The make matrixes.
        /// </summary>
        /// <remarks></remarks>
        public void MakeMatrixes()
        {
            TranslationMatrix = new Matrix[bsp.Spawns.Spawn.Count];
            for (int x = 0; x < bsp.Spawns.Spawn.Count; x++)
            {
                TranslationMatrix[x] = MakeMatrixForSpawn(x);
            }
        }

        /// <summary>
        /// The move spawns with keyboard.
        /// </summary>
        /// <remarks></remarks>
        public void MoveSpawnsWithKeyboard()
        {
            rotationBitMask = 0;
            try
            {
                cam.device.Acquire();
            }
            catch
            {
                return;
            }

            // Only allow BSP keypresses when side window closed. This will detect it open.
            // Also, if the Tool Strip Text Boxes are selected, will not allow movement.
            string s = string.Empty;
            if (this.ContainsFocus)
            {
                Control.ControlCollection cc = this.Controls;
                bool found = true;
                while (found)
                {
                    found = false;
                    for (int x = 0; x < cc.Count; x++)
                    {
                        if (cc[x].ContainsFocus)
                        {
                            if ((cc[x].Name == string.Empty) && (cc.Owner is ToolStrip))
                            {
                                ToolStrip TS = (ToolStrip)cc.Owner;
                                int j = 0;
                                for (int i = 0; i < TS.Items.Count; i++)
                                {
                                    if (TS.Items[i] is ToolStripControlHost)
                                    {
                                        if (j == x)
                                        {
                                            string[] s2 = TS.Items[i].GetType().ToString().Split('.');
                                            s += s2[s2.Length - 1] + "(" + TS.Items[i].Name + ")";
                                            break;
                                        }

                                        j++;
                                    }
                                }
                            }
                            else
                            {
                                s += cc[x].Name + "\\";
                            }

                            cc = cc[x].Controls;
                            found = true;
                            break;
                        }
                    }
                }
            }

            if ((!s.StartsWith("dockControl")) && (!s.StartsWith("statusStrip\\ToolStripTextBox")) &&
                (!s.StartsWith("statusStrip\\ToolStripComboBox")))
            {
                for (int x = 0; x < SelectedSpawn.Count; x++)
                {
                    if (bsp.Spawns.Spawn[SelectedSpawn[x]].Type == SpawnInfo.SpawnType.Objective)
                    {
                        if (
                            ((SpawnInfo.ObjectiveSpawn)bsp.Spawns.Spawn[SelectedSpawn[x]]).ObjectiveType.ToString().
                                StartsWith("KingOfTheHill_"))
                        {
                            spawns.hillsLoaded = false;
                        }
                    }

                    foreach (Key kk in cam.device.GetPressedKeys())
                    {
                        // tslabel.Text = kk.ToString();
                        switch (kk.ToString())
                        {
                            case "Up":
                                bsp.Spawns.Spawn[SelectedSpawn[x]].X += cam.speed;
                                break;
                            case "DownArrow":
                                bsp.Spawns.Spawn[SelectedSpawn[x]].X -= cam.speed;
                                break;
                            case "LeftArrow":
                                bsp.Spawns.Spawn[SelectedSpawn[x]].Y += cam.speed;
                                break;
                            case "Right":
                                bsp.Spawns.Spawn[SelectedSpawn[x]].Y -= cam.speed;
                                break;
                            case "PageDown":
                                bsp.Spawns.Spawn[SelectedSpawn[x]].Z -= cam.speed;
                                break;
                            case "PageUp":
                                bsp.Spawns.Spawn[SelectedSpawn[x]].Z += cam.speed;
                                break;
                            case "LeftControl":
                                rotationBitMask |= (int)SelectedItemRotationType.Control;
                                break;
                            case "RightControl":
                                rotationBitMask |= (int)SelectedItemRotationType.Control;
                                break;
                            case "LeftShift":
                                rotationBitMask |= (int)SelectedItemRotationType.Shift;
                                break;
                            case "RightShift":
                                rotationBitMask |= (int)SelectedItemRotationType.Shift;
                                break;
                            case "RightMenu":
                                rotationBitMask |= (int)SelectedItemRotationType.Alt;
                                break;
                            case "LeftMenu":
                                rotationBitMask |= (int)SelectedItemRotationType.Alt;
                                break;
                        }
                    }

                    TranslationMatrix[SelectedSpawn[x]] = MakeMatrixForSpawn(SelectedSpawn[x]);
                }

                if (!spawns.hillsLoaded)
                {
                    spawns.createHills();
                }

                if ((SelectedSpawn.Count > 0) && updateXYZYPR)
                {
                    int lastSelectedSpawn = SelectedSpawn[SelectedSpawn.Count - 1];
                    tsTextBoxX.Text = bsp.Spawns.Spawn[lastSelectedSpawn].X.ToString("#0.0000####");
                    tsTextBoxY.Text = bsp.Spawns.Spawn[lastSelectedSpawn].Y.ToString("#0.0000####");
                    tsTextBoxZ.Text = bsp.Spawns.Spawn[lastSelectedSpawn].Z.ToString("#0.0000####");
                    statusStrip.ResumeLayout();
                    statusStrip.SuspendLayout();
                }
            }
        }

        /// <summary>
        /// The render lightmap.
        /// </summary>
        /// <param name="LightmapIndex">The lightmap index.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public Image RenderLightmap(int LightmapIndex)
        {
            MemoryStream Image_MemoryStream = new MemoryStream();
            BinaryWriter Image_BW = new BinaryWriter(Image_MemoryStream);

            // Creat the header and write it
            byte[] MyHeader = {
                                  66, 77, 56, 3, 0, 0, 0, 0, 0, 0, 54, 0, 0, 0, 40, 0, 0, 0, 16, 0, 0, 0, 16, 0, 0, 0, 1, 
                                  0, 24, 0, 0, 0, 0, 0, 2, 3, 0, 0, 18, 11, 0, 0, 18, 11, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
                              };
            Image_BW.Write(MyHeader);

            // Write the RGB
            for (int i = 0; i < 1024; i += 4)
            {
                Image_BW.Write(LightMap_Array[(LightmapIndex * 1024) + i + 0]);
                Image_BW.Write(LightMap_Array[(LightmapIndex * 1024) + i + 1]);
                Image_BW.Write(LightMap_Array[(LightmapIndex * 1024) + i + 2]);
            }

            Image_BW.Write(0);
            Image_BW.Flush();

            Image tempimage;
            tempimage = Image.FromStream(Image_MemoryStream);
            tempimage.RotateFlip(RotateFlipType.Rotate180FlipX);

            return tempimage;
        }

        /// <summary>
        /// The check for intersection.
        /// </summary>
        /// <param name="e">The e.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public int[] checkForIntersection(MouseEventArgs e)
        {
            List<int> temp = new List<int>();

            for (int x = 0; x < bsp.Spawns.Spawn.Count; x++)
            {
                // check bitmask for object visibility
                if (((int)bsp.Spawns.Spawn[x].Type & visibleSpawnsBitMask) == 0)
                {
                    continue;
                }

                int tempcount = SpawnModel[spawnmodelindex[x]].Display.Chunk.Count;
                bool useboundingbox = false;

                

                switch (bsp.Spawns.Spawn[x].Type)
                {
                    case SpawnInfo.SpawnType.Camera:
                    case SpawnInfo.SpawnType.DeathZone:
                    case SpawnInfo.SpawnType.Light:
                    case SpawnInfo.SpawnType.Sound:
                        tempcount = 1;
                        useboundingbox = true;
                        break;
                }

                

                for (int yy = 0; yy < tempcount; yy++)
                {
                    // check for mesh intersection
                    Mesh tempm;
                    if (!useboundingbox)
                    {
                        tempm = SpawnModel[spawnmodelindex[x]].Display.meshes[yy];
                    }
                    else
                    {
                        tempm = BoundingBoxModel[x];
                    }

                    // Check under mouse cursor for object selection/deselection?
                    if (render.MeshPick(e.X, e.Y, tempm, TranslationMatrix[x]))
                    {
                        temp.Add(x);
                        break;
                    }
                }
            }

            return temp.ToArray();
        }

        /// <summary>
        /// The set camera position.
        /// </summary>
        /// <param name="X">The x.</param>
        /// <param name="Y">The y.</param>
        /// <param name="Z">The z.</param>
        /// <param name="exactLocation">The exact location.</param>
        /// <remarks></remarks>
        public void setCameraPosition(float X, float Y, float Z, bool exactLocation)
        {
            cam.Position.X = X;
            cam.Position.Y = Y;
            cam.Position.Z = Z;

            cam.radianv = 0;
            cam.radianh = 0;

            cam.x = X;
            cam.y = Y;
            cam.z = Z;
            if (!exactLocation)
            {
                // X-0.8 & Z+1.4 allows us to see the object from a decent angle
                cam.Position.X += -0.8F;
                cam.Position.Y += 0;
                cam.Position.Z += 1.40F;
                cam.radianv = 5.32F; // Radians or 305 degrees
                cam.x += -0.8F;
                cam.y += 0;
                cam.z += 1.40F;
            }

            // Make sure we don't get that dumb jump when we go to move after Dbl Clicking
            cam.oldx = MousePosition.X;
            cam.oldy = MousePosition.Y;
            cam.change(cam.oldx, cam.oldy);
        }

        /// <summary>
        /// The speed bar_ update.
        /// </summary>
        /// <remarks></remarks>
        public void speedBar_Update()
        {
            if (cam.speed < 1)
            {
                this.speedBar.Value = (int)(cam.speed * 100);
            }
            else
            {
                this.speedBar.Value = (int)(cam.speed * 10) + 90;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The on key press.
        /// </summary>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            if ((byte)e.KeyChar == (int)Keys.Escape)
            {
                this.Dispose(); // Esc was pressed
            }

            switch ((int)e.KeyChar)
            {
                case (int)Keys.Up:
                case (int)Keys.Down:
                case (int)Keys.Left:
                case (int)Keys.Right:
                    this.Dispose();
                    break;
            }
        }

        /// <summary>
        /// The on paint.
        /// </summary>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        protected override void OnPaint(PaintEventArgs e)
        {
            this.Render(); // Render on painting
        }

        /// <summary>
        /// The on resize.
        /// </summary>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        protected override void OnResize(EventArgs e)
        {
            // this.Render();//
            render.pause = (this.WindowState == FormWindowState.Minimized) || !this.Visible;
        }

        /// <summary>
        /// The wnd proc.
        /// </summary>
        /// <param name="m">The m.</param>
        /// <remarks></remarks>
        protected override void WndProc(ref Message m)
        {
            const int WM_SYSCOMMAND = 0x0112;
            const int SC_MAXIMIZE = 0xF030;
            const int SC_DBLTITLECLICK = 0xF032;
            const int SC_RESTORE = 0xF120;
            const int WM_SIZING = 0x214;
            const int WM_EXITSIZEMOVE = 0x232;
            const int WM_MOVE = 0x0003;

            /*
            if (m.Msg == WM_SIZING)
            {
                inSizing = true;
            }
            */
            if (m.Msg == WM_EXITSIZEMOVE && inSizing)
            {
                // WM_EXITSIZEMOVE
                // OnFormResizeEnd();
                inSizing = true;
            }

            if (m.Msg == WM_SIZING)
            {
                // OnFormResizeEnd();
                OnFormResizeEnd();
            }

            if (m.Msg == WM_SYSCOMMAND)
            {
                if (((int)m.WParam == SC_DBLTITLECLICK) || ((int)m.WParam == SC_MAXIMIZE) ||
                    ((((int)m.WParam) & 0xFFF0) == SC_RESTORE))
                {
                    // OnFormResizeEnd();
                    inSizing = true;
                }
            }

            base.WndProc(ref m);

            // This must come after WndProc. Do not move above!
            if (m.Msg == WM_MOVE && inSizing)
            {
                OnFormResizeEnd();
                inSizing = false;
            }
        }

        /// <summary>
        /// The bsp lighting_ checked changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void BSPLighting_CheckedChanged(object sender, EventArgs e)
        {
            bsp.RenderBSPLighting = ((CheckBox)sender).Checked;
        }

        /// <summary>
        /// The bsp permutations_ checked changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void BSPPermutations_CheckedChanged(object sender, EventArgs e)
        {
            bsp.DrawBSPPermutations = ((CheckBox)sender).Checked;
        }

        private void cbBSPTextures_CheckedChanged(object sender, EventArgs e)
        {
            ;
        }

        /// <summary>
        /// Selects/Deselects Camera Culling option
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CameraCulling_CheckedChanged(object sender, EventArgs e)
        {
            bsp.cameraCulling = ((CheckBox)sender).Checked;
        }

        /// <summary>
        /// The bsp viewer_ mouse down.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void BSPViewer_MouseDown(object sender, MouseEventArgs e)
        {
            

            if (e.Button == MouseButtons.Middle)
            {
                selectionHeight = 0;
                selectionWidth = 0;
                selectionDepth = 0;
                selectionMulti = true;
                selectionStart = render.Mark3DCursorPosition(e.X, e.Y, Matrix.Identity);
            }
                
                #region StartCameraRotation (Mouse Right Button)
            else if (e.Button == MouseButtons.Right)
            {
                cam.oldx = e.X;
                cam.oldy = e.Y;
                Time = DateTime.Now.TimeOfDay;
                this.ContextMenuStrip = identContext;
            }

                #endregion
                #region SpawnSelection (Mouse Left Button)
            else if (e.Button == MouseButtons.Left)
            {
                #region DecideUponObjectRotation

                if ((SelectedSpawn.Count > 0) && (rotationBitMask != 0))
                {
                    selectionStart = render.Mark3DCursorPosition(e.X, e.Y, Matrix.Identity);
                    oldx = e.X;
                    oldy = e.Y;
                    itemrotate = true;

                    // return;
                }

                    #endregion
                else
                {
                    #region CheckSpawnsForIntersection

                    for (int x = 0; x < bsp.Spawns.Spawn.Count; x++)
                    {
                        // check bitmask for object visibility
                        if (((int)bsp.Spawns.Spawn[x].Type & visibleSpawnsBitMask) == 0)
                        {
                            continue;
                        }

                        int tempcount = SpawnModel[spawnmodelindex[x]].Display.Chunk.Count;
                        bool useboundingbox = false;

                        #region Make Cameras, DeathZones, Sounds And Lights Use BoundingBoxes

                        switch (bsp.Spawns.Spawn[x].Type)
                        {
                            case SpawnInfo.SpawnType.Camera:
                            case SpawnInfo.SpawnType.DeathZone:
                            case SpawnInfo.SpawnType.Light:
                            case SpawnInfo.SpawnType.Sound:
                                tempcount = 1;
                                useboundingbox = true;
                                break;
                        }

                        #endregion

                        for (int yy = 0; yy < tempcount; yy++)
                        {
                            // check for mesh intersection
                            Mesh tempm;
                            if (!useboundingbox)
                            {
                                tempm = SpawnModel[spawnmodelindex[x]].Display.meshes[yy];
                            }
                            else
                            {
                                tempm = BoundingBoxModel[x];
                            }

                            // Check under mouse cursor for object selection/deselection?
                            if (render.MeshPick(e.X, e.Y, tempm, TranslationMatrix[x]))
                            {
                                if (bsp.Spawns.Spawn[x].frozen)
                                {
                                    break;
                                }

                                #region TurnSpawnOnOrOff

                                int tempi = SelectedSpawn.IndexOf(x);
                                if (tempi != -1)
                                {
                                    SelectedSpawn.RemoveAt(tempi);
                                    if (DeselectOne.Checked)
                                    {
                                        updateStatusPosition();
                                        return;
                                    }
                                }
                                else
                                {
                                    SelectedSpawn.Add(x);
                                    selectedSpawnType = bsp.Spawns.Spawn[x].Type;
                                }

                                #endregion

                                break;
                            }
                        }
                    }
                }

                #endregion CycleThroughSpawns
            }

            #region statusBarUpdates

            updateStatusPosition();

            #endregion statusBarUpdates

            #endregion
        }

        /// <summary>
        /// The bsp viewer_ mouse up.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void BSPViewer_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
            {
                return;
            }

            Time = DateTime.Now.TimeOfDay.Subtract(Time);

            if ((Time.Ticks / 1000000) < 3)
            {
                

                int[] spawns = checkForIntersection(e);
                for (int x = 0; x < spawns.Length; x++)
                {
                    currentObject = x;
                    int tempi = SelectedSpawn.IndexOf(spawns[x]);
                    if (tempi != -1)
                    {
                        SelectedSpawn.RemoveAt(tempi);
                        updateStatusPosition();
                    }

                    // bsp.Spawns.Spawn[spawns[x]].frozen = !bsp.Spawns.Spawn[spawns[x]].frozen;
                    break;
                }
            }
        }

        /// <summary>
        /// BSPSelection
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e param</param>
        private void checkedListBox2_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (checkedListBox2.Enabled == false) { return; }

            bsp.BSPRawDataMetaChunks[e.Index].render = (e.CurrentValue == CheckState.Unchecked);
        }

        /// <summary>
        /// The color balance.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="redShift">The red shift.</param>
        /// <param name="greenShift">The green shift.</param>
        /// <param name="blueShift">The blue shift.</param>
        /// <param name="preserve">The preserve.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        private byte[][] ColorBalance(byte[][] source, int redShift, int greenShift, int blueShift, bool preserve)
        {
            byte[][] adjusted = new byte[source.Length][];
            double shiftR = redShift / 100d;
            double shiftG = greenShift / 100d;
            double shiftB = blueShift / 100d;
            if (preserve)
            {
                // Avoid Complete Desaturation
                shiftR *= 0.99d;
                shiftG *= 0.99d;
                shiftB *= 0.99d;
            }

            for (int i = 0; i < source.Length; i++)
            {
                adjusted[i] = new byte[1024];
                for (int x = 0; x < 1024; x += 4)
                {
                    double B = source[i][x] / 255d;
                    double G = source[i][x + 1] / 255d;
                    double R = source[i][x + 2] / 255d;
                    if (preserve)
                    {
                        // RGB -> L
                        double max = Math.Max(Math.Max(R, G), B);
                        double min = Math.Min(Math.Min(R, G), B);
                        double L = 0.5 * (max + min);

                        // double S = L == 0 || L == 1 ? 0 : // Filter undefined
                        // 	L <= 0.5 ? (max-min)/(max+min) : (max-min)/(2-(max+min));

                        // RGB + Shift -> HS
                        B = shiftB <= 0 ? B * (1 + shiftB) : B + shiftB * (1 - B);
                        G = shiftG <= 0 ? G * (1 + shiftG) : G + shiftG * (1 - G);
                        R = shiftR <= 0 ? R * (1 + shiftR) : R + shiftR * (1 - R);
                        max = Math.Max(Math.Max(R, G), B);
                        min = Math.Min(Math.Min(R, G), B);
                        double tempL = 0.5d * (max + min);
                        double S = tempL == 0 || tempL == 1
                                       ? 0
                                       : // Filter undefined
                                   tempL <= 0.5 ? (max - min) / (max + min) : (max - min) / (2 - (max + min));
                        double H = S == 0
                                       ? 0
                                       : // saturation == 0
                                   R >= G && R >= B
                                       ? 60 * (G - B) / (max - min)
                                       : // max == red
                                   G >= R && G >= B
                                       ? 60 * (B - R) / (max - min) + 120
                                       : // max == green
                                   60 * (R - G) / (max - min) + 240; // max == blue

                        // HSL -> RGB
                        double d = L <= 0.5 ? S * L : S * (1 - L);
                        if (L == 0 || L == 1 || S == 0 || d == 0)
                        {
                            R = G = B = L;
                        }
                        else
                        {
                            while (H >= 360)
                            {
                                H -= 360;
                            }

                            while (H < 0)
                            {
                                H += 360;
                            }

                            // double d = L <= 0.5 ? S*L : S*(1-L);
                            min = L - d;
                            max = L + d;
                            double F = H < 60 ? H : H < 180 ? H - 120 : H < 300 ? H - 240 : H - 360;
                            double mid = min + Math.Abs(F / 60) * (max - min);
                            switch ((int)Math.Floor(H / 60))
                            {
                                case 0:
                                    R = max;
                                    G = mid;
                                    B = min;
                                    break;
                                case 1:
                                    R = mid;
                                    G = max;
                                    B = min;
                                    break;
                                case 2:
                                    R = min;
                                    G = max;
                                    B = mid;
                                    break;
                                case 3:
                                    R = min;
                                    G = mid;
                                    B = max;
                                    break;
                                case 4:
                                    R = mid;
                                    G = min;
                                    B = max;
                                    break;
                                case 5:
                                    R = max;
                                    G = min;
                                    B = mid;
                                    break;
                            }
                        }
                    }
                    else
                    {
                        B = shiftB <= 0 ? B * (1 + shiftB) : B + shiftB * (1 - B);
                        G = shiftG <= 0 ? G * (1 + shiftG) : G + shiftG * (1 - G);
                        R = shiftR <= 0 ? R * (1 + shiftR) : R + shiftR * (1 - R);
                    }

                    adjusted[i][x] = (byte)(B * 255);
                    adjusted[i][x + 1] = (byte)(G * 255);
                    adjusted[i][x + 2] = (byte)(R * 255);
                    adjusted[i][x + 3] = 255;
                }
            }

            return adjusted;
        }

        /// <summary>
        /// The draw skybox.
        /// </summary>
        /// <param name="pm">The pm.</param>
        /// <remarks></remarks>
        private void DrawSkybox(ParsedModel pm)
        {
            if (pm == null)
            {
                return;
            }

            for (int x = 0; x < pm.Display.Chunk.Count; x++)
            {
                if (x != 0 && x != 0) continue;
                int rawindex = pm.Display.Chunk[x];
                for (int xx = 0; xx < pm.RawDataMetaChunks[rawindex].SubMeshInfo.Length; xx++)
                {
                    // device.Material = meshmaterials[i];
                    int tempshade = pm.RawDataMetaChunks[rawindex].SubMeshInfo[xx].ShaderNumber;

                    //Renderer.SetAlphaBlending(ShaderInfo.AlphaType.AlphaBlend, ref render.device);
                    Renderer.SetAlphaBlending(pm.Shaders.Shader[tempshade].Alpha, ref render.device);

                    switch (pm.Shaders.Shader[tempshade].Alpha)
                    {
                        case ShaderInfo.AlphaType.AlphaBlend:
                            render.device.RenderState.SourceBlend = Blend.BothSourceAlpha;
                            render.device.RenderState.DestinationBlend = Blend.Zero;
                            break;
                        case ShaderInfo.AlphaType.None:
                            render.device.RenderState.SourceBlend = Blend.One;
                            render.device.RenderState.DestinationBlend = Blend.One;
                            break;
                    }

                    render.device.SetTexture(0, pm.Shaders.Shader[tempshade].MainTexture);

                    // Skybox textures
                    render.device.TextureState[0].ColorOperation = TextureOperation.SelectArg1;
                    render.device.TextureState[0].ColorArgument1 = TextureArgument.TextureColor;
                    render.device.TextureState[0].ColorArgument2 = TextureArgument.Current;
                    render.device.TextureState[0].AlphaOperation = TextureOperation.SelectArg1;
                    
                    // render.device.TextureState[0].AlphaOperation = TextureOperation.ModulateAlphaAddColor;
                    render.device.TextureState[0].AlphaArgument1 = TextureArgument.TextureColor;
                    render.device.TextureState[0].AlphaArgument2 = TextureArgument.Current;

                    // Skybox Lighting
                    /*
                    if (bsp.LightMapTexture[x] != null && bsp.BSPRawDataMetaChunks[x].LightMapUVs.Count != 0)
                    {
                        render.device.SetTexture(1, bsp.LightMapTexture[x]);
                        render.device.TextureState[1].ColorOperation = TextureOperation.Disable;
                        render.device.TextureState[1].AlphaOperation = TextureOperation.Disable;
                        render.device.TextureState[1].TextureCoordinateIndex = 2;
                    }
                    */
                    render.device.RenderState.FillMode = FillMode.WireFrame;

                    // render.device.SetTexture(0, meshtextures[i]);
                    pm.Display.meshes[x].DrawSubset(xx);
                }
            }
        }

        /// <summary>
        /// The edit lightmaps.
        /// </summary>
        /// <remarks></remarks>
        private void EditLightmaps()
        {
            if (radioButton1.Checked)
            {
                if (checkBox1.Checked)
                {
                    byte[][] temparray = new byte[1][];
                    for (int i = 0; i < map.BSP.sbsp[bsp.BspNumber].LightMap_Palettes.Count; i++)
                    {
                        temparray[0] = new byte[1024];
                        Array.ConstrainedCopy(LightMap_Array_Backup, i * 1024, temparray[0], 0, 1024);
                        Array.ConstrainedCopy(
                            HueSaturation(temparray, trackint1, trackint2, trackint3, checkBox2.Checked)[0], 
                            0, 
                            LightMap_Array, 
                            i * 1024, 
                            1024);
                    }

                    foreach (PictureBox picbox in panel1.Controls)
                    {
                        picbox.Image = RenderLightmap((int)((picbox).Tag));
                    }
                }
                else
                {
                    byte[][] temparray = new byte[1][];
                    temparray[0] = new byte[1024];
                    Array.ConstrainedCopy(
                        LightMap_Array_Backup, Convert.ToInt32(comboBox1.Text) * 1024, temparray[0], 0, 1024);
                    Array.ConstrainedCopy(
                        HueSaturation(temparray, trackint1, trackint2, trackint3, checkBox2.Checked)[0], 
                        0, 
                        LightMap_Array, 
                        Convert.ToInt32(comboBox1.Text) * 1024, 
                        1024);

                    foreach (PictureBox picbox in panel1.Controls)
                    {
                        if (((int)(picbox).Tag) == Convert.ToInt32(comboBox1.Text))
                        {
                            picbox.Image = RenderLightmap((int)((picbox).Tag));
                        }
                    }
                }
            }
            else
            {
                if (checkBox1.Checked)
                {
                    byte[][] temparray = new byte[1][];
                    for (int i = 0; i < map.BSP.sbsp[bsp.BspNumber].LightMap_Palettes.Count; i++)
                    {
                        temparray[0] = new byte[1024];
                        Array.ConstrainedCopy(LightMap_Array_Backup, i * 1024, temparray[0], 0, 1024);
                        Array.ConstrainedCopy(
                            ColorBalance(temparray, trackint1, trackint2, trackint3, checkBox2.Checked)[0], 
                            0, 
                            LightMap_Array, 
                            i * 1024, 
                            1024);
                    }

                    foreach (PictureBox picbox in panel1.Controls)
                    {
                        picbox.Image = RenderLightmap((int)((picbox).Tag));
                    }
                }
                else
                {
                    byte[][] temparray = new byte[1][];
                    temparray[0] = new byte[1024];
                    Array.ConstrainedCopy(
                        LightMap_Array_Backup, Convert.ToInt32(comboBox1.Text) * 1024, temparray[0], 0, 1024);
                    Array.ConstrainedCopy(
                        ColorBalance(temparray, trackint1, trackint2, trackint3, checkBox2.Checked)[0], 
                        0, 
                        LightMap_Array, 
                        Convert.ToInt32(comboBox1.Text) * 1024, 
                        1024);

                    foreach (PictureBox picbox in panel1.Controls)
                    {
                        if (((int)(picbox).Tag) == Convert.ToInt32(comboBox1.Text))
                        {
                            picbox.Image = RenderLightmap((int)((picbox).Tag));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The hue saturation.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="hueShift">The hue shift.</param>
        /// <param name="satShift">The sat shift.</param>
        /// <param name="lightShift">The light shift.</param>
        /// <param name="colorize">The colorize.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        private byte[][] HueSaturation(byte[][] source, int hueShift, int satShift, int lightShift, bool colorize)
        {
            byte[][] adjusted = new byte[source.Length][];
            double shiftH = hueShift;
            double shiftS = ((double)satShift) / 100;
            double shiftL = ((double)lightShift) / 100;
            for (int i = 0; i < source.Length; i++)
            {
                adjusted[i] = new byte[1024];
                for (int x = 0; x < 1024; x += 4)
                {
                    // RGB -> HSL
                    double B = ((double)source[i][x]) / 255;
                    double G = ((double)source[i][x + 1]) / 255;
                    double R = ((double)source[i][x + 2]) / 255;
                    double max = Math.Max(Math.Max(R, G), B);
                    double min = Math.Min(Math.Min(R, G), B);
                    double L = 0.5 * (max + min);
                    double S = L == 0 || L == 1
                                   ? 0
                                   : // Filter undefined
                               L <= 0.5 ? (max - min) / (max + min) : (max - min) / (2 - (max + min));
                    double H = colorize
                                   ? 0
                                   : // colorize
                               S == 0
                                   ? 0
                                   : // saturation == 0
                               R >= G && R >= B
                                   ? 60 * (G - B) / (max - min)
                                   : // max == red
                               G >= R && G >= B
                                   ? 60 * (B - R) / (max - min) + 120
                                   : // max == blue
                               60 * (R - G) / (max - min) + 240; // max == green

                    // Shift HSL
                    H = H + shiftH;
                    S = !colorize && S == 0
                            ? 0
                            : // Don't create saturation
                        shiftS <= 0 ? S * (1 + shiftS) : S + shiftS * (1 - S);
                    L = shiftL <= 0 ? L * (1 + shiftL) : L + shiftL * (1 - L);

                    // HSL -> RGB
                    double d = L <= 0.5 ? S * L : S * (1 - L);
                    if (L == 0 || L == 1 || S == 0 || d == 0)
                    {
                        R = G = B = L;
                    }
                    else
                    {
                        while (H >= 360)
                        {
                            H -= 360;
                        }

                        while (H < 0)
                        {
                            H += 360;
                        }

                        // double d = L <= 0.5 ? S*L : S*(1-L);
                        min = L - d;
                        max = L + d;
                        double F = H < 60 ? H : H < 180 ? H - 120 : H < 300 ? H - 240 : H - 360;
                        double mid = min + Math.Abs(F / 60) * (max - min);
                        switch ((int)Math.Floor(H / 60))
                        {
                            case 0:
                                R = max;
                                G = mid;
                                B = min;
                                break;
                            case 1:
                                R = mid;
                                G = max;
                                B = min;
                                break;
                            case 2:
                                R = min;
                                G = max;
                                B = mid;
                                break;
                            case 3:
                                R = min;
                                G = mid;
                                B = max;
                                break;
                            case 4:
                                R = mid;
                                G = min;
                                B = max;
                                break;
                            case 5:
                                R = max;
                                G = min;
                                B = mid;
                                break;
                        }
                    }

                    adjusted[i][x] = (byte)(B * 255);
                    adjusted[i][x + 1] = (byte)(G * 255);
                    adjusted[i][x + 2] = (byte)(R * 255);
                    adjusted[i][x + 3] = 255;
                }
            }

            return adjusted;
        }

        /// <summary>
        /// The load_ lightmaps_ into_ an_ array.
        /// </summary>
        /// <remarks></remarks>
        private void Load_Lightmaps_Into_An_Array()
        {
            LightMap_Array = new byte[map.BSP.sbsp[bsp.BspNumber].LightMap_Palettes.Count * 1024];

            LightMap_Array_Backup = new byte[LightMap_Array.Length];

            for (int i = 0; i < map.BSP.sbsp[bsp.BspNumber].LightMap_Palettes.Count; i++)
            {
                for (int j = 0; j < 256; j++)
                {
                    BSPContainer.Palette_Color temp = map.BSP.sbsp[bsp.BspNumber].LightMap_Palettes[i][j];

                    LightMap_Array[(i * 1024) + (j * 4) + 0] = Convert.ToByte(temp.r);
                    LightMap_Array[(i * 1024) + (j * 4) + 1] = Convert.ToByte(temp.g);
                    LightMap_Array[(i * 1024) + (j * 4) + 2] = Convert.ToByte(temp.b);
                    LightMap_Array[(i * 1024) + (j * 4) + 3] = Convert.ToByte(temp.a);
                }
            }

            Array.Copy(LightMap_Array, LightMap_Array_Backup, LightMap_Array.Length);
        }

        /// <summary>
        /// The model viewer_ mouse down.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void ModelViewer_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.None)
            {
                oldx = e.X;
                oldy = e.Y;
            }

            if (SelectedSpawn.Count > 0)
            {
                int i = SelectedSpawn[SelectedSpawn.Count - 1];
                axis = Gizmo.axis.none;
                if (e.Button == MouseButtons.None && gizmo != null)
                {
                    axis = gizmo.checkForIntersection(e, TranslationMatrix[i]);
                }

                // Only Last selection hilights ATM!
                if ((axis != Gizmo.axis.none) && (e.Button == MouseButtons.Left))
                {
                    float xDiff = (e.X - oldx) / 10.0f;
                    float yDiff = (e.Y - oldy) / 10.0f;
                    switch (axis)
                    {
                        case Gizmo.axis.X:
                            bsp.Spawns.Spawn[i].X -= xDiff / cam.speed;
                            break;
                        case Gizmo.axis.Y:
                            bsp.Spawns.Spawn[i].Y -= yDiff / cam.speed;
                            break;
                        case Gizmo.axis.Z:
                            bsp.Spawns.Spawn[i].Z -= yDiff / cam.speed;
                            break;
                        case Gizmo.axis.XY:
                            bsp.Spawns.Spawn[i].X += yDiff / cam.speed;
                            bsp.Spawns.Spawn[i].Y += xDiff / cam.speed;
                            break;
                        case Gizmo.axis.YZ:
                            bsp.Spawns.Spawn[i].Y += xDiff / cam.speed;
                            bsp.Spawns.Spawn[i].Z -= yDiff / cam.speed;
                            break;
                        case Gizmo.axis.XZ:
                            bsp.Spawns.Spawn[i].X -= xDiff / cam.speed;
                            bsp.Spawns.Spawn[i].Z -= yDiff / cam.speed;
                            break;
                    }

                    oldx = e.X;
                    oldy = e.Y;
                }
            }

            if (selectionMulti)
            {
                Vector3 tempvec = render.Mark3DCursorPosition(e.X, e.Y, Matrix.Identity);
                selectionWidth = tempvec.X - selectionStart.X;
                selectionHeight = tempvec.Y - selectionStart.Y;
                selectionDepth = tempvec.Z - selectionStart.Z;
                selectionWidth *= 2;
                selectionHeight *= 7;
                selectionDepth *= 2;
                float tempselectionWidth = selectionWidth;
                float tempselectionHeight = selectionHeight;
                float tempselectionDepth = selectionDepth;
                if (tempselectionWidth < 0)
                {
                    tempselectionWidth = -tempselectionWidth;
                }

                if (tempselectionHeight < 0)
                {
                    tempselectionHeight = -tempselectionHeight;
                }

                if (tempselectionDepth < 0)
                {
                    tempselectionDepth = -tempselectionDepth;
                }

                /************/
                tsLabel1.Text = selectionStart.X.ToString().PadRight(10) + " • " +
                                selectionStart.Y.ToString().PadRight(10) + selectionStart.Z.ToString().PadRight(10) +
                                " • " + selectionWidth.ToString().PadRight(10) + " • " +
                                selectionHeight.ToString().PadRight(10) + " • " + selectionDepth.ToString().PadRight(10);
                selectionMesh = Mesh.Box(render.device, tempselectionWidth, tempselectionHeight, tempselectionDepth);
            }
                
                #region SelectedSpawnRotation
            else if (rotationBitMask != 0 && itemrotate)
            {
                Vector3 tempvec = render.Mark3DCursorPosition(e.X, e.Y, Matrix.Identity);
                selectionWidth = tempvec.X - selectionStart.X;
                selectionHeight = tempvec.Y - selectionStart.Y;
                selectionDepth = tempvec.Z - selectionStart.Z;
                selectionStart = tempvec;

                foreach (int spawnid in SelectedSpawn)
                {
                    if (bsp.Spawns.Spawn[spawnid] is SpawnInfo.BoundingBoxSpawn)
                    {
                        continue;
                    }

                    #region ShiftAndRotate

                    if ((rotationBitMask & (int)SelectedItemRotationType.Shift) != 0)
                    {
                        if (bsp.Spawns.Spawn[spawnid] is SpawnInfo.RotateYawPitchRollBaseSpawn)
                        {
                            SpawnInfo.RotateYawPitchRollBaseSpawn temp;
                            temp = (SpawnInfo.RotateYawPitchRollBaseSpawn)bsp.Spawns.Spawn[spawnid];
                            temp.Yaw += selectionHeight * cam.speed * 5;
                            if (temp.Yaw > (float)Math.PI)
                            {
                                temp.Yaw = -(float)Math.PI;
                            }
                            else if (temp.Yaw < (float)-Math.PI)
                            {
                                temp.Yaw = (float)Math.PI;
                            }
                        }
                        else
                        {
                            SpawnInfo.RotateDirectionBaseSpawn temp;
                            temp = (SpawnInfo.RotateDirectionBaseSpawn)bsp.Spawns.Spawn[spawnid];
                            temp.RotationDirection += selectionHeight * cam.speed * 5;

                            // if (temp.RotationDirection > (float)Math.PI) { temp.RotationDirection = temp.RotationDirection - ((float)Math.PI * 2); }
                            // else if (temp.RotationDirection < (float)-Math.PI) { temp.RotationDirection = temp.RotationDirection + ((float)Math.PI * 2); }
                        }
                    }

                    #endregion

                    #region ControlAndRotate

                    if ((rotationBitMask & (int)SelectedItemRotationType.Control) != 0)
                    {
                        if (bsp.Spawns.Spawn[spawnid] is SpawnInfo.RotateYawPitchRollBaseSpawn)
                        {
                            SpawnInfo.RotateYawPitchRollBaseSpawn temp;
                            temp = (SpawnInfo.RotateYawPitchRollBaseSpawn)bsp.Spawns.Spawn[spawnid];
                            temp.Pitch += selectionWidth * cam.speed * 5;
                            if (temp.Pitch > (float)Math.PI)
                            {
                                temp.Pitch = -(float)Math.PI;
                            }
                            else if (temp.Pitch < (float)-Math.PI)
                            {
                                temp.Pitch = (float)Math.PI;
                            }
                        }
                    }

                    #endregion

                    #region AltAndRotate

                    if ((rotationBitMask & (int)SelectedItemRotationType.Alt) != 0)
                    {
                        if (bsp.Spawns.Spawn[spawnid] is SpawnInfo.RotateYawPitchRollBaseSpawn)
                        {
                            SpawnInfo.RotateYawPitchRollBaseSpawn temp;
                            temp = (SpawnInfo.RotateYawPitchRollBaseSpawn)bsp.Spawns.Spawn[spawnid];
                            temp.Roll += selectionWidth * cam.speed * 5;
                            if (temp.Roll > (float)Math.PI)
                            {
                                temp.Roll = -(float)Math.PI;
                            }
                            else if (temp.Roll < (float)-Math.PI)
                            {
                                temp.Roll = (float)Math.PI;
                            }
                        }
                    }

                    #endregion

                    TranslationMatrix[spawnid] = MakeMatrixForSpawn(spawnid);
                }

                updateStatusPosition();
            }

            #endregion

            #region CameraRotation

            if (e.Button == MouseButtons.Right)
            {
                cam.change(e.X, e.Y);
                this.ContextMenuStrip = null;
            }

            #endregion
        }

        /// <summary>
        /// The model viewer_ mouse up.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void ModelViewer_MouseUp(object sender, MouseEventArgs e)
        {
            

            if (itemrotate)
            {
                itemrotate = false;
                rotationBitMask = 0;
            }

            

            #region EndMultiSelect

            if (selectionMulti)
            {
                selectionMulti = false;
                selectionMesh = null;
                float halfwidth = selectionWidth / 2;
                float halfheight = selectionHeight / 2;
                float halfdepth = selectionDepth / 2;
                float poshalfwidth = halfwidth;
                float poshalfheight = halfheight;
                float poshalfdepth = halfdepth;

                float tempselectionWidth = selectionWidth;
                float tempselectionHeight = selectionHeight;
                float tempselectionDepth = selectionDepth;

                if (poshalfwidth < 0)
                {
                    poshalfwidth = -poshalfwidth;
                }

                if (poshalfheight < 0)
                {
                    poshalfheight = -poshalfheight;
                }

                if (poshalfdepth < 0)
                {
                    poshalfdepth = -poshalfdepth;
                }

                // float minx = selectionStart.X + halfwidth - poshalfwidth;
                // float maxx = selectionStart.X + poshalfwidth + halfwidth;
                // float miny = selectionStart.Y + halfheight - poshalfheight;
                // float maxy = selectionStart.Y + poshalfheight + halfheight;
                // float minz = selectionStart.Z + halfdepth - poshalfdepth;
                // float maxz = selectionStart.Z + poshalfdepth + halfdepth;
                float minx = selectionStart.X - poshalfwidth;
                float maxx = selectionStart.X + poshalfwidth;
                float miny = selectionStart.Y - poshalfheight;
                float maxy = selectionStart.Y + poshalfheight;
                float minz = selectionStart.Z - poshalfdepth;
                float maxz = selectionStart.Z + poshalfdepth;
                SelectedSpawn.Clear();
                for (int i = 0; i < bsp.Spawns.Spawn.Count; i++)
                {
                    if (bsp.Spawns.Spawn[i].X > minx && bsp.Spawns.Spawn[i].X < maxx && bsp.Spawns.Spawn[i].Y > miny &&
                        bsp.Spawns.Spawn[i].Y < maxy && bsp.Spawns.Spawn[i].Z > minz && bsp.Spawns.Spawn[i].Z < maxz)
                    {
                        SelectedSpawn.Add(i);
                    }
                }
            }

            #endregion
        }

        /// <summary>
        /// The on form resize end.
        /// </summary>
        /// <remarks></remarks>
        private void OnFormResizeEnd()
        {
            speedBar.Left = this.Width - speedBar.Width;
            speedLabel.Left = speedBar.Left;
            speedBar.Refresh();
            if (render.device != null)
            {
                leftSandDock.Height = render.device.Viewport.Height - statusStrip.Height;
                statusStrip.Top = render.device.Viewport.Height - statusStrip.Height;
            }

            statusStrip.Width = this.Width;
            statusStrip.Refresh();
            aspect = this.Width / (float)this.Height;

            // cam is not established when the form is created and Maximized,
            // so it is least time/processor consuming to only check on resizes.
            if (cam != null)
            {
                updateStatusPosition();
            }
        }

        /// <summary>
        /// The picture box_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void PictureBox_Click(object sender, EventArgs e)
        {
            checkBox1.Checked = false;
            comboBox1.SelectedIndex = (int)((PictureBox)sender).Tag;
        }

        /// <summary>
        /// The reload from array.
        /// </summary>
        /// <remarks></remarks>
        private void ReloadFromArray()
        {
            for (int i = 0; i < map.BSP.sbsp[bsp.BspNumber].LightMap_Palettes.Count; i++)
            {
                for (int j = 0; j < 256; j++)
                {
                    BSPContainer.Palette_Color temp = new BSPContainer.Palette_Color();

                    temp.r = LightMap_Array[(i * 1024) + (j * 4) + 0];
                    temp.g = LightMap_Array[(i * 1024) + (j * 4) + 1];
                    temp.b = LightMap_Array[(i * 1024) + (j * 4) + 2];
                    temp.a = LightMap_Array[(i * 1024) + (j * 4) + 3];

                    map.BSP.sbsp[bsp.BspNumber].LightMap_Palettes[i][j] = temp;
                }
            }
        }

        /// <summary>
        /// The render.
        /// </summary>
        /// <remarks></remarks>
        private void Render()
        {
            if (render.pause)
            {
                return;
            }

            if (RenderSky.Checked)
            {
                render.BeginScene(Color.Black);
            }
            else
            {
                render.BeginScene(Color.Blue);
            }

            SetupMatrices();

            

            if (cam.move())
            {
                this.speedBar_Update();
            }

            MoveSpawnsWithKeyboard();

            

            #region RenderBSP

            render.device.RenderState.Ambient = Color.LightGray;
            if (RenderSky.Checked)
            {
                render.device.Transform.World = Matrix.RotationZ((float)Math.PI / 3) *
                                                Matrix.Translation(cam.x, cam.y, cam.z);

                // render.device.Transform.World = Matrix.Identity;
                // Clamp to get rid of texture seams or mirror or wrap?
                render.device.SamplerState[0].AddressU = TextureAddress.Mirror;
                render.device.SamplerState[0].AddressV = TextureAddress.Mirror;
                render.device.RenderState.Lighting = true;
                render.device.RenderState.ZBufferEnable = false;
                render.device.RenderState.ZBufferWriteEnable = false;

                // Raw.ParsedModel.DisplayedInfo.Draw(ref render.device, bsp.SkyBox);
                DrawSkybox(bsp.SkyBox);
            }

            render.device.Transform.World = Matrix.Identity;
            render.device.SamplerState[0].AddressU = TextureAddress.Wrap;
            render.device.SamplerState[0].AddressV = TextureAddress.Wrap;
            render.device.RenderState.Lighting = true;
            render.device.RenderState.ZBufferEnable = true;
            render.device.RenderState.ZBufferWriteEnable = true;

            
            BSPModel.BSPDisplayedInfo.Draw(ref render.device, ref bsp, cbBSPTextures.Checked, ref cam, shaderx);
            
            render.device.RenderState.Ambient = Color.White;
            // Set camera postion
            string tempstring = toolStripLabel2.Text;
            string tempstring2 = "Camera Position: X: " + cam.x.ToString().PadRight(10) + " • Y: " +
                                 cam.y.ToString().PadRight(10) + " • Z: " + cam.z.ToString().PadRight(10);
            if (tempstring != tempstring2)
            {
                if (statusStrip.Items.IndexOf(toolStripLabel2) == -1)
                {
                    statusStrip.Items.Add(toolStripLabel2);
                }

                toolStripLabel2.Text = tempstring2;
                statusStrip.ResumeLayout();
                statusStrip.SuspendLayout();
            }

            #endregion

            //int lightCount = 0;

            #region RenderSpawns

            for (int x = 0; x < bsp.Spawns.Spawn.Count; x++)
            {
                if (((int)bsp.Spawns.Spawn[x].Type & visibleSpawnsBitMask) == 0)
                {
                    continue;
                }

                render.device.Transform.World = TranslationMatrix[x];
                Vector3 tempv = new Vector3();
                tempv.X = bsp.Spawns.Spawn[x].X;
                tempv.Y = bsp.Spawns.Spawn[x].Y;
                tempv.Z = bsp.Spawns.Spawn[x].Z;

                // if (!cam.SphereInFrustum(tempv,10f)){continue;}
                render.device.Material = DefaultMaterial;
                DefaultMaterial.Diffuse = Color.Crimson;
                DefaultMaterial.Ambient = Color.Crimson;

                bool drawModel = true;

                #region DrawLights

                if (bsp.Spawns.Spawn[x] is SpawnInfo.LightSpawn)
                {
                    /**** Lights not showing up.... Hmm...Oh well.
                    SpawnInfo.LightSpawn light = bsp.Spawns.Spawn[x] as Renderer.BSP_Renderer.SpawnInfo.LightSpawn;
                    render.device.Lights[lightCount].Type = LightType.Spot;
                    render.device.Lights[lightCount].Diffuse = Color.FromArgb(light.LightInfo.r, light.LightInfo.g, light.LightInfo.b);
                    render.device.Lights[lightCount].Position = new Vector3(light.X, light.Y, light.Z);
                    render.device.Lights[lightCount].Direction = new Vector3(0, -0.5f, 0);
                    render.device.Lights[lightCount].Range = 5.0f;
                    render.device.Lights[lightCount].InnerConeAngle = 0.5f;
                    render.device.Lights[lightCount].OuterConeAngle = 1.0f;
                    render.device.Lights[lightCount].Falloff = 2.0f;
                    render.device.Lights[lightCount].Attenuation0 = 2.0f;
                    render.device.Lights[lightCount].Update();
                    render.device.Lights[lightCount].Enabled = false;
                    lightCount++;
                    */
                    /*** This should go below ;)
                    for (int i = lightCount; i < render.device.Lights.Count; i++)
                        render.device.Lights[i].Enabled = false;
                    */
                    render.device.Material = BlueMaterial;
                    render.device.SetTexture(0, null);
                    render.device.RenderState.AlphaBlendEnable = false;
                    render.device.RenderState.AlphaTestEnable = false;
                    render.device.RenderState.FillMode = FillMode.WireFrame;
                    BoundingBoxModel[x].DrawSubset(0);

                    // This is the selected color
                    render.device.Material = RedMaterial;
                    drawModel = false;
                }

                #endregion

                #region DrawSounds

                if (bsp.Spawns.Spawn[x] is SpawnInfo.SoundSpawn)
                {
                    render.device.Material = RedMaterial;
                    render.device.SetTexture(0, null);
                    render.device.RenderState.AlphaBlendEnable = false;
                    render.device.RenderState.AlphaTestEnable = false;
                    render.device.RenderState.FillMode = FillMode.WireFrame;
                    BoundingBoxModel[x].DrawSubset(0);

                    render.device.Material = RedMaterial;
                    render.device.RenderState.AlphaBlendEnable = true;
                    render.device.RenderState.AlphaTestEnable = true;
                    render.device.RenderState.DestinationBlend = Blend.DestinationAlpha;
                    render.device.RenderState.SourceBlend = Blend.SourceAlpha;
                    render.device.RenderState.FillMode = FillMode.Solid;
                    BoundingBoxModel[x].DrawSubset(0);

                    // Renderer.BSP_Renderer.SpawnInfo.SoundSpawn tempbox = bsp.Spawns.Spawn[x] as Renderer.BSP_Renderer.SpawnInfo.SoundSpawn;
                    // render.device.Material = GreenMaterial;
                    // BoundingBoxModel[x] = D3D.Mesh.Sphere(render.device, tempbox.DistanceBoundsLower, 10 + (int)tempbox.DistanceBoundsUpper, 10 + (int)tempbox.DistanceBoundsUpper);
                    // BoundingBoxModel[x].DrawSubset(0);
                    // This is the selected color
                    render.device.Material = BlueMaterial;
                    drawModel = false;
                }

                #endregion

                #region DrawCameras

                if (bsp.Spawns.Spawn[x] is SpawnInfo.CameraSpawn)
                {
                    render.device.Material = BlueMaterial;
                    render.device.SetTexture(0, null);
                    render.device.RenderState.AlphaBlendEnable = false;
                    render.device.RenderState.AlphaTestEnable = false;
                    render.device.RenderState.FillMode = FillMode.WireFrame;
                    BoundingBoxModel[x].DrawSubset(0);

                    // This is the selected color
                    render.device.Material = RedMaterial;
                    drawModel = false;
                }

                #endregion

                #region DrawBoundingBoxOnly_Deathzones

                if (bsp.Spawns.Spawn[x] is SpawnInfo.BoundingBoxSpawn)
                {
                    if (bsp.Spawns.Spawn[x].Type == SpawnInfo.SpawnType.DeathZone)
                    {
                        render.device.Material = PinkMaterial;
                        render.device.SetTexture(0, null);
                        render.device.RenderState.AlphaBlendEnable = true;
                        render.device.RenderState.AlphaTestEnable = true;
                        render.device.RenderState.DestinationBlend = Blend.DestinationAlpha;
                        render.device.RenderState.SourceBlend = Blend.SourceAlpha;
                        render.device.RenderState.FillMode = FillMode.Solid;
                        BoundingBoxModel[x].DrawSubset(0);
                    }

                    render.device.Material = RedMaterial;
                    drawModel = false;
                }

                #endregion

                #region ObjectiveSpawnColoring

                if (bsp.Spawns.Spawn[x] is SpawnInfo.ObjectiveSpawn)
                {
                    render.device.RenderState.FillMode = FillMode.Solid;
                    SpawnInfo.ObjectiveSpawn os = bsp.Spawns.Spawn[x] as SpawnInfo.ObjectiveSpawn;
                    switch (os.Team)
                    {
                        case SpawnInfo.ObjectiveSpawn.TeamType.Red_Defense:
                            render.device.Material = RedMaterial;
                            break;
                        case SpawnInfo.ObjectiveSpawn.TeamType.Blue_Offense:
                            render.device.Material = BlueMaterial;
                            break;
                        case SpawnInfo.ObjectiveSpawn.TeamType.Yellow:
                            render.device.Material = YellowMaterial;
                            break;
                        case SpawnInfo.ObjectiveSpawn.TeamType.Green:
                            render.device.Material = GreenMaterial;
                            break;
                        case SpawnInfo.ObjectiveSpawn.TeamType.Purple:
                            render.device.Material = PurpleMaterial;
                            break;
                        case SpawnInfo.ObjectiveSpawn.TeamType.Orange:
                            render.device.Material = OrangeMaterial;
                            break;
                        case SpawnInfo.ObjectiveSpawn.TeamType.Brown:
                            render.device.Material = BrownMaterial;
                            break;
                        case SpawnInfo.ObjectiveSpawn.TeamType.Pink:
                            render.device.Material = PinkMaterial;
                            break;
                        case SpawnInfo.ObjectiveSpawn.TeamType.Neutral:
                            render.device.Material = NeutralMaterial;
                            break;
                        default:
                            render.device.Material = DefaultMaterial;
                            break;
                    }
                }

                #endregion

                /*
                 * // Add support for particles somewhere, this shows a box at least
                #region ObstacleSpawn
                if (bsp.Spawns.Spawn[x] is Entity.Renderer.BSP_Renderer.SpawnInfo.ObstacleSpawn)
                {
                    render.device.RenderState.FillMode = D3D.FillMode.Solid;
                    Entity.Renderer.BSP_Renderer.SpawnInfo.ObstacleSpawn os = bsp.Spawns.Spawn[x] as Entity.Renderer.BSP_Renderer.SpawnInfo.ObstacleSpawn;
                    string sss = os.ModelName;
                    BoundingBoxModel[x] = BoundingBoxModel[x - 1];
                }
                #endregion
                */
                #region DrawBoxOnSelections

                for (int i = 0; i < SelectedSpawn.Count; i++)
                {
                    if (SelectedSpawn[i] == x)
                    {
                        render.device.SetTexture(0, null);
                        render.device.RenderState.AlphaBlendEnable = false;
                        render.device.RenderState.AlphaTestEnable = false;
                        render.device.RenderState.FillMode = FillMode.WireFrame;

                        // Adjust center position of Bounding Boxes to proper offset
                        Matrix mat = Matrix.Identity;
                        mat = Matrix.Add(
                            mat, 
                            Matrix.Translation(
                                bsp.Spawns.Spawn[SelectedSpawn[i]].bbXDiff, 
                                bsp.Spawns.Spawn[SelectedSpawn[i]].bbYDiff, 
                                bsp.Spawns.Spawn[SelectedSpawn[i]].bbZDiff));
                        render.device.Transform.World = mat * TranslationMatrix[x];
                        BoundingBoxModel[x].DrawSubset(0);

                        /***************/
                        float s1 = SpawnModel[spawnmodelindex[x]].BoundingBox.MaxX -
                                   SpawnModel[spawnmodelindex[x]].BoundingBox.MinX;
                        float s2 = SpawnModel[spawnmodelindex[x]].BoundingBox.MaxY -
                                   SpawnModel[spawnmodelindex[x]].BoundingBox.MinY;
                        float s3 = SpawnModel[spawnmodelindex[x]].BoundingBox.MaxZ -
                                   SpawnModel[spawnmodelindex[x]].BoundingBox.MinZ;
                        Vector4 v4 = Vector3.Transform(cam.Position, TranslationMatrix[x]);

                        SpawnInfo.BaseSpawn s = bsp.Spawns.Spawn[x];
                        Vector3 c = cam.Position;
                        float scale = (cam.Position.X - s.X) + (cam.Position.Y - s.Y) + (cam.Position.Z - s.Z);

                        scale = (((SpawnModel[spawnmodelindex[x]].BoundingBox.MaxX -
                                   SpawnModel[spawnmodelindex[x]].BoundingBox.MinX) +
                                  (SpawnModel[spawnmodelindex[x]].BoundingBox.MaxY -
                                   SpawnModel[spawnmodelindex[x]].BoundingBox.MinY) +
                                  (SpawnModel[spawnmodelindex[x]].BoundingBox.MaxZ -
                                   SpawnModel[spawnmodelindex[x]].BoundingBox.MinZ)) / 3) * 12;

                        scale = (v4.X + v4.Y + v4.Z) / 3;
                        if (gizmo != null)
                        {
                            gizmo.draw(scale / 50.0f);
                        }

                        /**********/
                    }
                }

                #endregion

                render.device.Transform.World = TranslationMatrix[x];

                /*  This was an attempt at adding scaling, but not right, so not right now.
                Entity.Raw.ParsedModel pm = SpawnModel[spawnmodelindex[x]];
                if (bsp.Spawns.Spawn[x] is Entity.Renderer.BSP_Renderer.SpawnInfo.ScaleRotateYawPitchRollSpawn)
                {
                    Entity.Renderer.BSP_Renderer.SpawnInfo.ScaleRotateYawPitchRollSpawn tempBsp = bsp.Spawns.Spawn[x] as Entity.Renderer.BSP_Renderer.SpawnInfo.ScaleRotateYawPitchRollSpawn;
                    for (int i = 0; i < pm.RawDataMetaChunks.Length; i++)
                        for (int j = 0; j < pm.RawDataMetaChunks[i].VerticeCount; j++)
                            pm.RawDataMetaChunks[i].Vertices[j] = Vector3.Scale(SpawnModel[spawnmodelindex[x]].RawDataMetaChunks[i].Vertices[j], tempBsp.Scale + 1.0f);
                }
                */

                if (drawModel)
                {
                    // Store old cull mode
                    Cull cm = render.device.RenderState.CullMode;
                    render.device.RenderState.CullMode = Cull.None;

                    if (bsp.Spawns.Spawn[x].frozen)
                    {
                        render.device.RenderState.FillMode = FillMode.WireFrame;
                    }
                    else
                    {
                        render.device.RenderState.FillMode = FillMode.Solid;
                    }

                    ParsedModel.DisplayedInfo.Draw(ref render.device, SpawnModel[spawnmodelindex[x]]);
                    // Restore old cull mode
                    render.device.RenderState.CullMode = cm;
                }
            }

            #endregion

            #region RenderHills

            if ((visibleSpawnsBitMask & (int)SpawnInfo.SpawnType.Objective) != 0)
            {
                Cull cm = render.device.RenderState.CullMode;
                render.device.RenderState.CullMode = Cull.None;
                render.device.Transform.World = Matrix.Identity;
                for (int i = 0; i < spawns.hillDisplay.Length; i++)
                {
                    if (spawns.hillDisplay[i] == null)
                    {
                        continue;
                    }

                    render.device.SetTexture(0, bsp.Shaders.Shader[1].MainTexture);
                    render.device.RenderState.AlphaBlendEnable = true;
                    render.device.RenderState.AlphaTestEnable = false;
                    render.device.RenderState.SourceBlend = Blend.One;
                    render.device.RenderState.DestinationBlend = Blend.One;
                    render.device.RenderState.FillMode = FillMode.Solid;

                    // Adjust center position of Bounding Boxes to proper offset
                    spawns.hillDisplay[i].DrawSubset(0);
                }

                render.device.RenderState.CullMode = cm;
            }

            #endregion

            /*
            #region RenderWater
            if (1 != 0)
            {
                render.device.Transform.World = Matrix.Identity;
                for (int i = 0; i < 1; i++) // bsp.Water.Length
                {
                    if (bsp.Water == null) { continue; }

                    render.device.SetTexture(0, null);
                    render.device.RenderState.AlphaBlendEnable = true;
                    render.device.RenderState.AlphaTestEnable = false;
                    render.device.RenderState.Ambient = Color.Aqua;
                    render.device.RenderState.SourceBlend = D3D.Blend.One;
                    render.device.RenderState.DestinationBlend = D3D.Blend.One;
                    render.device.RenderState.FillMode = D3D.FillMode.Solid;
                    // Adjust center position of Bounding Boxes to proper offset
                    render.device.Transform.World = Matrix.Translation(bsp.Water.centerX, bsp.Water.centerY, bsp.Water.height-10);
                    D3D.Mesh m = D3D.Mesh.Box(render.device, bsp.Water.extentX, bsp.Water.extentY, 10);
                    m.DrawSubset(0);
                    m.Dispose();
                }
            }
            #endregion
            */
            #region MultiSelectMesh

            if (selectionMesh != null)
            {
                Matrix m = Matrix.Identity;
                float halfwidth = selectionWidth / 2;
                float halfheight = selectionHeight / 2;
                float halfdepth = selectionDepth / 2;

                // m.Multiply(Matrix.Translation(selectionStart.X + halfwidth, selectionStart.Y + halfheight, selectionStart.Z + halfdepth));
                m.Multiply(Matrix.Translation(selectionStart.X, selectionStart.Y, selectionStart.Z));
                render.device.Transform.World = m;
                render.device.SetTexture(0, null);
                render.device.RenderState.AlphaBlendEnable = false;
                render.device.RenderState.AlphaTestEnable = false;
                render.device.RenderState.FillMode = FillMode.WireFrame;
                render.device.Material = RedMaterial;

                selectionMesh.DrawSubset(0);
            }

            #endregion

            if (this.BackColor == Color.Blue)
            {
                this.BackColor = Color.FromArgb(235, 233, 237);
            }

            render.EndScene();
        }

        // SaveChanges
        /// <summary>
        /// The save changes_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void SaveChanges_Click(object sender, EventArgs e)
        {
            SelectedSpawn.Clear();
            map.OpenMap(MapTypes.Internal);
            for (int i = 0; i < bsp.Spawns.Spawn.Count; i++)
            {
                map.BW.BaseStream.Position = bsp.Spawns.Spawn[i].offset;
                if (bsp.Spawns.Spawn[i].Type == SpawnInfo.SpawnType.DeathZone)
                {
                    SpawnInfo.BoundingBoxSpawn tempbox;
                    tempbox = bsp.Spawns.Spawn[i] as SpawnInfo.BoundingBoxSpawn;

                    // Deathzones are saved as a midpoint and width, length & height, so save as midpoint
                    map.BW.Write(tempbox.X - tempbox.width / 2);
                    map.BW.Write(tempbox.Y - tempbox.height / 2);
                    map.BW.Write(tempbox.Z - tempbox.length / 2);
                }
                else
                {
                    map.BW.Write(bsp.Spawns.Spawn[i].X);
                    map.BW.Write(bsp.Spawns.Spawn[i].Y);
                    map.BW.Write(bsp.Spawns.Spawn[i].Z);
                }

                

                #region RotationOneDirection

                if (bsp.Spawns.Spawn[i] is SpawnInfo.RotateDirectionBaseSpawn)
                {
                    SpawnInfo.RotateDirectionBaseSpawn tempspawn =
                        bsp.Spawns.Spawn[i] as SpawnInfo.RotateDirectionBaseSpawn;
                    map.BW.Write(tempspawn.RotationDirection);
                }

                    #endregion
                    #region RotationYawPitchRoll
                else if (bsp.Spawns.Spawn[i] is SpawnInfo.RotateYawPitchRollBaseSpawn)
                {
                    SpawnInfo.RotateYawPitchRollBaseSpawn tempspawn =
                        bsp.Spawns.Spawn[i] as SpawnInfo.RotateYawPitchRollBaseSpawn;
                    if (map.HaloVersion == HaloVersionEnum.Halo2 ||
                        map.HaloVersion == HaloVersionEnum.Halo2Vista)
                    {
                        map.BW.Write(tempspawn.Roll);

                        // if (tempspawn.isWeird == true) { tempspawn.Pitch = -tempspawn.Pitch; }
                        map.BW.Write(tempspawn.Pitch);
                        map.BW.Write(tempspawn.Yaw);
                        if (bsp.Spawns.Spawn[i] is SpawnInfo.ScaleRotateYawPitchRollSpawn)
                        {
                            SpawnInfo.ScaleRotateYawPitchRollSpawn tspawn =
                                bsp.Spawns.Spawn[i] as SpawnInfo.ScaleRotateYawPitchRollSpawn;
                            map.BW.Write(tspawn.Scale);
                        }
                    }
                    else
                    {
                        map.BW.Write(Renderer.RadianToDegree(tempspawn.Yaw));
                        map.BW.Write(Renderer.RadianToDegree(tempspawn.Pitch));
                        map.BW.Write(Renderer.RadianToDegree(tempspawn.Roll));
                    }
                }

                #endregion

                

                #region ObjectiveSpawn

                if (bsp.Spawns.Spawn[i] is SpawnInfo.ObjectiveSpawn)
                {
                    SpawnInfo.ObjectiveSpawn os;
                    os = bsp.Spawns.Spawn[i] as SpawnInfo.ObjectiveSpawn;
                    map.BW.BaseStream.Position = bsp.Spawns.Spawn[i].offset + 16;
                    map.BW.Write((short)os.ObjectiveType);
                    map.BW.Write((short)os.Team);
                    map.BW.Write(os.number);
                }

                    #endregion
                    #region Collection
                else if (bsp.Spawns.Spawn[i] is SpawnInfo.Collection)
                {
                    // Spawn[].offset doesn't point to the start, but to the X Position, 64 bytes into the section??
                    SpawnInfo.Collection os;
                    os = bsp.Spawns.Spawn[i] as SpawnInfo.Collection;
                    map.BW.BaseStream.Position = bsp.Spawns.Spawn[i].offset - 60;
                    map.BW.Write((int)os.SpawnsInMode);

                    // offset 24 = collection TAG/ID
                    map.BW.BaseStream.Position = bsp.Spawns.Spawn[i].offset + 24;

                    // reverse tag type (CMTI, IHEV, etc)
                    char[] c = new char[4];
                    c[0] = os.TagType[3];
                    c[1] = os.TagType[2];
                    c[2] = os.TagType[1];
                    c[3] = os.TagType[0];
                    int TagNum = map.Functions.ForMeta.FindByNameAndTagType(os.TagType, os.TagPath);
                    if (TagNum == -1)
                    {
                        MessageBox.Show("Error finding [" + os.TagType + "] " + os.TagPath);
                    }
                    else
                    {
                        map.BW.Write(c);
                        map.BW.Write(map.MetaInfo.Ident[TagNum]);
                    }

                    // My hampsters programming contribution -> " h 6"
                }

                    #endregion
                    #region Obstacle
                else if (bsp.Spawns.Spawn[i] is SpawnInfo.ObstacleSpawn)
                {
                    if (ObstacleList == null)
                    {
                        continue;
                    }

                    // Spawn[].offset doesn't point to the start, but to the X Position, 8 bytes in
                    SpawnInfo.ObstacleSpawn os;
                    os = bsp.Spawns.Spawn[i] as SpawnInfo.ObstacleSpawn;

                    // Base of SCNR tag, Pointer to Crates/Obstacles (+808)
                    map.BR.BaseStream.Position = map.MetaInfo.Offset[3] + 808;

                    // # of Obstacle chunks & offset to start
                    int count = map.BR.ReadInt32();
                    int BlocOffset = map.BR.ReadInt32() - map.SecondaryMagic;

                    // Each Block is 76 bytes in size
                    map.BR.BaseStream.Position = BlocOffset + os.BlocNumber * 76;

                    for (int yy = 0; yy < ObstacleList.Count; yy++)
                    {
                        if (ObstacleList[yy].TagPath == os.TagPath)
                        {
                            map.BW.Write((Int16)ObstacleList[yy].ScenPalNumber);
                            break;
                        }
                    }
                }

                    #endregion
                    #region Scenery
                else if (bsp.Spawns.Spawn[i] is SpawnInfo.ScenerySpawn && SceneryList != null)
                {
                    // Spawn[].offset doesn't point to the start, but to the X Position, 8 bytes in
                    SpawnInfo.ScenerySpawn os;
                    os = bsp.Spawns.Spawn[i] as SpawnInfo.ScenerySpawn;

                    // Base of SCNR tag, Pointer to Scenery (+80)
                    map.BR.BaseStream.Position = map.MetaInfo.Offset[3] + 80;

                    // # of Scenery chunks & offset to start
                    int count = map.BR.ReadInt32();
                    int ScenOffset = map.BR.ReadInt32() - map.SecondaryMagic;

                    map.BR.BaseStream.Position = ScenOffset + os.ScenNumber * 92;
                    for (int yy = 0; yy < SceneryList.Count; yy++)
                    {
                        if (SceneryList[yy].TagPath == os.TagPath)
                        {
                            map.BW.Write((Int16)SceneryList[yy].ScenPalNumber);
                            break;
                        }
                    }

                    /****
                    // Base of SCNR tag, Pointer to Scenery Palette (+88)
                    map.BR.BaseStream.Position = map.MetaInfo.Offset[3] + 88;
                    // # of Scenery Palette chunks & offset to start
                    int SPcount = map.BR.ReadInt32();
                    int SPOffset = map.BR.ReadInt32() - map.SecondaryMagic;
                    for (int xx = 0; xx < count; xx++)
                    {
                        map.BR.BaseStream.Position = SPOffset + xx * 40;
                        byte cc = map.BR.ReadByte();
                        cc = map.BR.ReadByte();
                        cc = map.BR.ReadByte();
                        cc = map.BR.ReadByte();
                        //necs
                        int aa = map.BR.ReadInt32();
                        int aaa = map.Functions.Meta.FindMetaByID(aa, map);
                    }
                    ****/
                }

                    #endregion
                    #region Sound
                else if (bsp.Spawns.Spawn[i] is SpawnInfo.SoundSpawn)
                {
                    // Spawn[].offset doesn't point to the start, but to the X Position, 8 bytes in
                    SpawnInfo.SoundSpawn os;
                    os = bsp.Spawns.Spawn[i] as SpawnInfo.SoundSpawn;

                    map.BW.BaseStream.Position = bsp.Spawns.Spawn[i].offset + 54 - 8;
                    map.BW.Write((short)os.VolumeType);
                    map.BW.Write(os.Height);
                    map.BW.Write(os.DistanceBoundsLower);
                    map.BW.Write(os.DistanceBoundsUpper);
                    map.BW.Write(os.ConeAngleLower);
                    map.BW.Write(os.ConeAngleUpper);
                    map.BW.Write(os.OuterConeGain);
                }

                #endregion
            }

            

            /*
            switch (map.HaloVersion)
            {
                case Map.HaloVersionEnum.Halo2:
                    #region H2
                    // *** only for maps with 1 BSP for now !!
                    int BSPNum = 0;
                    if (map.BSP.bspcount > 1) { break; }
                    map.BW.BaseStream.Position = 172;
                    map.BW.Write( (Int32) bsp.BSPRawDataMetaChunks.Length);
                    map.BW.Write( (Int32) bsp.BSPRawDataMetaChunksOffset+
                                                        + map.BSP.sbsp[BSPNum].magic
                                                        + map.BSP.sbsp[BSPNum].offset);
                     * for (int x = 0; x < bsp.BSPRawDataMetaChunks.Length; x++)
                    {
                        H2BSPWriteRawDataMetaChunk( x, ref bsp, BSPNum);
                    }
                    #endregion
                    break;
            }
            */

            
            map.CloseMap();

            MessageBox.Show("Done");
        }

        /// <summary>
        /// The select all spawns_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void SelectAllSpawns_Click(object sender, EventArgs e)
        {
            SelectedSpawn.Clear();
            for (int i = 0; i < bsp.Spawns.Spawn.Count; i++)
            {
                if (((int)bsp.Spawns.Spawn[i].Type & visibleSpawnsBitMask) == 0)
                {
                    continue;
                }

                SelectedSpawn.Add(i);
            }
        }

        /// <summary>
        /// The setup matrices.
        /// </summary>
        /// <remarks></remarks>
        private void SetupMatrices()
        {
            render.device.Transform.World = Matrix.Identity;

            // Matrix.RotationAxis(new Vector3((float)Math.Cos(Environment.TickCount / 250.0f), 1, (float)Math.Sin(Environment.TickCount / 250.0f)), Environment.TickCount / 1000.0f);
            render.device.Transform.View = Matrix.LookAtRH(cam.Position, cam.LookAt, cam.UpVector);
            render.device.Transform.Projection = Matrix.PerspectiveFovRH(0.785f, aspect, 0.2f, 1000.0f);

            // cam.BuildViewFrustum(ref render.device);
        }

        /// <summary>
        /// The spawn list_ check.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void SpawnList_Check(object sender, ItemCheckEventArgs e)
        {
            setSpawnBox(checkedListBox1.Items[e.Index].ToString(), e.NewValue);
            updateStatusPosition();
        }

        /// <summary>
        /// The tool strip menu item rotate_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void ToolStripMenuItemRotate_Click(object sender, EventArgs e)
        {
            float change = 0.0f;
            switch (((ToolStripDropDownItem)sender).Text.Substring(0, ((ToolStripDropDownItem)sender).Text.IndexOf('*'))
                )
            {
                case "45":
                    change = (float)Math.PI / 4;
                    break;
                case "90":
                    change = (float)Math.PI / 2;
                    break;
                case "180":
                    change = (float)Math.PI;
                    break;
            }

            if (((ToolStripDropDownItem)sender).Name.ToUpper().Contains("CCW"))
            {
                change = -change;
            }

            if (((ToolStripDropDownItem)sender).OwnerItem == ToolStripDropDownButtonRotateYaw)
            {
                foreach (int i in SelectedSpawn)
                {
                    SpawnInfo.BaseSpawn spawn = bsp.Spawns.Spawn[i];
                    if (spawn is SpawnInfo.RotateYawPitchRollBaseSpawn)
                    {
                        ((SpawnInfo.RotateYawPitchRollBaseSpawn)spawn).Yaw += change;
                    }
                    else if (spawn is SpawnInfo.RotateDirectionBaseSpawn)
                    {
                        ((SpawnInfo.RotateDirectionBaseSpawn)spawn).RotationDirection -= change;
                    }
                }
            }
            else if (((ToolStripDropDownItem)sender).OwnerItem == ToolStripDropDownButtonRotatePitch)
            {
                foreach (int i in SelectedSpawn)
                {
                    SpawnInfo.BaseSpawn spawn = bsp.Spawns.Spawn[i];
                    if (spawn is SpawnInfo.RotateYawPitchRollBaseSpawn)
                    {
                        ((SpawnInfo.RotateYawPitchRollBaseSpawn)spawn).Pitch -= change;
                    }
                }
            }
            else if (((ToolStripDropDownItem)sender).OwnerItem == toolStripDropDownButtonRotateRoll)
            {
                foreach (int i in SelectedSpawn)
                {
                    SpawnInfo.BaseSpawn spawn = bsp.Spawns.Spawn[i];
                    if (spawn is SpawnInfo.RotateYawPitchRollBaseSpawn)
                    {
                        ((SpawnInfo.RotateYawPitchRollBaseSpawn)spawn).Roll -= change;
                    }
                }
            }
        }

        /// <summary>
        /// The vector to rgba.
        /// </summary>
        /// <param name="v">The v.</param>
        /// <param name="height">The height.</param>
        /// <returns>The vector to rgba.</returns>
        /// <remarks></remarks>
        private int VectorToRgba(Vector3 v, float height)
        {
            int r = (int)(127.0f * v.X + 128.0f);
            int g = (int)(127.0f * v.Y + 128.0f);
            int b = (int)(127.0f * v.Z + 128.0f);
            int a = (int)(255.0f * height);

            return (a << 24) + (r << 16) + (g << 8) + (b << 0);
        }

        /// <summary>
        /// The button 1_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void button1_Click(object sender, EventArgs e)
        {
            trackBar1.Value = 0;
            trackBar2.Value = 0;
            trackBar3.Value = 0;

            Array.ConstrainedCopy(LightMap_Array_Backup, 0, LightMap_Array, 0, LightMap_Array.Length);

            foreach (PictureBox picbox in panel1.Controls)
            {
                picbox.Image = RenderLightmap((int)((picbox).Tag));
            }
        }

        /// <summary>
        /// The button 2_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void button2_Click(object sender, EventArgs e)
        {
            ReloadFromArray();

            bsp.LoadLightmaps();

            BSPModel.BSPDisplayedInfo.LoadLightmapTextures(ref render.device, ref bsp);
        }

        /// <summary>
        /// The button 3_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void button3_Click(object sender, EventArgs e)
        {
            // Save the lightmap
            map.BSP.sbsp[bsp.BspNumber].WritePalettes(map);

            // Apply the new palettes
            Array.ConstrainedCopy(LightMap_Array, 0, LightMap_Array_Backup, 0, LightMap_Array.Length);

            trackBar1.Value = 0;
            trackBar2.Value = 0;
            trackBar3.Value = 0;

            MessageBox.Show("Done");
        }

        /// <summary>
        /// The button 4_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void button4_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < checkedListBox2.Items.Count; i++)
            {
                checkedListBox2.SetItemChecked(i, true);
            }
        }

        /// <summary>
        /// The button 5_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void button5_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < checkedListBox2.Items.Count; i++)
            {
                checkedListBox2.SetItemChecked(i, false);
            }
        }

        /// <summary>
        /// The check box 1_ checked changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                comboBox1.Enabled = false;
            }
            else
            {
                comboBox1.Enabled = true;
            }
        }

        /// <summary>
        /// The check box 2_ checked changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            EditLightmaps();
        }

        /// <summary>
        /// The check box 3_ checked changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox3.Checked)
            {
                for (int i = 0; i < checkedListBox1.Items.Count; i++)
                {
                    checkedListBox1.SetItemCheckState(i, CheckState.Checked);
                }
            }
            else
            {
                for (int i = 0; i < checkedListBox1.Items.Count; i++)
                {
                    checkedListBox1.SetItemCheckState(i, CheckState.Unchecked);
                }
            }
        }

        /// <summary>
        /// The combo_ selected index changed collection.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void combo_SelectedIndexChangedCollection(object sender, EventArgs e)
        {
            // We need this here so that when the program changes the box, it doesn't change everything selected!
            if (!((ToolStripComboBox)sender).Focused)
            {
                return;
            }

            ToolStripComboBox cb = sender as ToolStripComboBox;

            foreach (int i in SelectedSpawn)
            {
                if (bsp.Spawns.Spawn[i].Type == SpawnInfo.SpawnType.Collection)
                {
                    SpawnInfo.Collection os = bsp.Spawns.Spawn[i] as SpawnInfo.Collection;
                    object test = Enum.Parse(
                        typeof(SpawnInfo.Collection.SpawnsInEnum), cb.Items[cb.SelectedIndex].ToString(), true);
                    os.SpawnsInMode = (SpawnInfo.Collection.SpawnsInEnum)test;
                    bsp.Spawns.Spawn[i] = os;
                }
            }
        }

        /// <summary>
        /// The combo_ selected index changed collection model.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void combo_SelectedIndexChangedCollectionModel(object sender, EventArgs e)
        {
            // We need this here so that when the program changes the box, it doesn't change everything selected!
            if (!((ToolStripComboBox)sender).Focused)
            {
                return;
            }

            ToolStripComboBox cb = sender as ToolStripComboBox;

            // looks for a model already on the map. if not FOUND, adds it to the SpawnModels
            bool found = false;
            int SpawnModelNum = -1;

            // Lists all weapons
            for (int i = 0; i < WeaponsList.Count; i++)
            {
                if (WeaponsList[i].Name == cb.SelectedItem.ToString())
                {
                    for (int j = 0; j < SpawnModel.Count; j++)
                    {
                        if (SpawnModel[j].name == WeaponsList[i].Model.name)
                        {
                            SpawnModelNum = j;
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                    {
                        SpawnModel.Add(WeaponsList[i].Model);
                        SpawnModelNum = SpawnModel.Count - 1;
                    }

                    // Change the bounding box for the model
                    float boxwidth = SpawnModel[SpawnModelNum].BoundingBox.MaxX -
                                     SpawnModel[SpawnModelNum].BoundingBox.MinX;
                    float boxheight = SpawnModel[SpawnModelNum].BoundingBox.MaxY -
                                      SpawnModel[SpawnModelNum].BoundingBox.MinY;
                    float boxdepth = SpawnModel[SpawnModelNum].BoundingBox.MaxZ -
                                     SpawnModel[SpawnModelNum].BoundingBox.MinZ;

                    for (int j = 0; j < this.SelectedSpawn.Count; j++)
                    {
                        spawnmodelindex[SelectedSpawn[j]] = SpawnModelNum;
                        BoundingBoxModel[SelectedSpawn[j]] = Mesh.Box(render.device, boxwidth, boxheight, boxdepth);

                        SpawnInfo.Collection bspInfo = (SpawnInfo.Collection)bsp.Spawns.Spawn[SelectedSpawn[j]];
                        bspInfo.bbXDiff = SpawnModel[SpawnModelNum].BoundingBox.MaxX +
                                          SpawnModel[SpawnModelNum].BoundingBox.MinX;
                        bspInfo.bbYDiff = SpawnModel[SpawnModelNum].BoundingBox.MaxY +
                                          SpawnModel[SpawnModelNum].BoundingBox.MinY;
                        bspInfo.bbZDiff = SpawnModel[SpawnModelNum].BoundingBox.MaxZ +
                                          SpawnModel[SpawnModelNum].BoundingBox.MinZ;

                        bspInfo.ModelTagNumber = WeaponsList[i].ModelTagNumber;
                        bspInfo.ModelName = map.FileNames.Name[WeaponsList[i].ModelTagNumber];
                        bspInfo.TagPath = WeaponsList[i].TagPath;
                        bspInfo.TagType = WeaponsList[i].TagType;

                        for (int a = 0; a < treeView1.Nodes.Count; a++)
                        {
                            for (int aa = 0; aa < treeView1.Nodes[a].Nodes.Count; aa++)
                            {
                                if (treeView1.Nodes[a].Nodes[aa].Tag.ToString() == SelectedSpawn[j].ToString())
                                {
                                    string[] temps = bspInfo.TagPath.Split('\\');
                                    this.treeView1.Nodes[a].Nodes[aa].Text = temps[temps.Length - 1];
                                }
                            }
                        }
                    }

                    treeView1.Sort();
                }
            }
        }

        /// <summary>
        /// The combo_ selected index changed objective.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void combo_SelectedIndexChangedObjective(object sender, EventArgs e)
        {
            // We need this here so that when the program changes the box, it doesn't change everything selected!
            if (!((ToolStripComboBox)sender).Focused)
            {
                return;
            }

            ToolStripComboBox cb = sender as ToolStripComboBox;

            foreach (int i in SelectedSpawn)
            {
                if (bsp.Spawns.Spawn[i].Type == SpawnInfo.SpawnType.Objective)
                {
                    SpawnInfo.ObjectiveSpawn os = bsp.Spawns.Spawn[i] as SpawnInfo.ObjectiveSpawn;

                    object test = Enum.Parse(
                        typeof(SpawnInfo.ObjectiveSpawn.ObjectiveTypeEnum), cb.Items[cb.SelectedIndex].ToString(), true);
                    os.ObjectiveType = (SpawnInfo.ObjectiveSpawn.ObjectiveTypeEnum)test;
                    bsp.Spawns.Spawn[i] = os;
                }
            }
        }

        /// <summary>
        /// The combo_ selected index changed obstacle model.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void combo_SelectedIndexChangedObstacleModel(object sender, EventArgs e)
        {
            // We need this here so that when the program changes the box, it doesn't change everything selected!
            if (!((ToolStripComboBox)sender).Focused)
            {
                return;
            }

            ToolStripComboBox cb = sender as ToolStripComboBox;

            // looks for a model already on the map. if not FOUND, adds it to the SpawnModels
            bool found = false;
            int SpawnModelNum = -1;

            // Lists all Scenery & Obstacles
            for (int i = 0; i < ObstacleList.Count; i++)
            {
                if (ObstacleList[i].Name == cb.SelectedItem.ToString())
                {
                    for (int j = 0; j < SpawnModel.Count; j++)
                    {
                        if (SpawnModel[j].name == ObstacleList[i].Model.name)
                        {
                            SpawnModelNum = j;
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                    {
                        SpawnModel.Add(ObstacleList[i].Model);
                        SpawnModelNum = SpawnModel.Count - 1;
                    }

                    // Change the bounding box for the model
                    float boxwidth = SpawnModel[SpawnModelNum].BoundingBox.MaxX -
                                     SpawnModel[SpawnModelNum].BoundingBox.MinX;
                    float boxheight = SpawnModel[SpawnModelNum].BoundingBox.MaxY -
                                      SpawnModel[SpawnModelNum].BoundingBox.MinY;
                    float boxdepth = SpawnModel[SpawnModelNum].BoundingBox.MaxZ -
                                     SpawnModel[SpawnModelNum].BoundingBox.MinZ;

                    for (int j = 0; j < this.SelectedSpawn.Count; j++)
                    {
                        spawnmodelindex[SelectedSpawn[j]] = SpawnModelNum;
                        BoundingBoxModel[SelectedSpawn[j]] = Mesh.Box(render.device, boxwidth, boxheight, boxdepth);

                        SpawnInfo.BaseSpawn bspInfo = bsp.Spawns.Spawn[SelectedSpawn[j]];
                        bspInfo.bbXDiff = SpawnModel[SpawnModelNum].BoundingBox.MaxX +
                                          SpawnModel[SpawnModelNum].BoundingBox.MinX;
                        bspInfo.bbYDiff = SpawnModel[SpawnModelNum].BoundingBox.MaxY +
                                          SpawnModel[SpawnModelNum].BoundingBox.MinY;
                        bspInfo.bbZDiff = SpawnModel[SpawnModelNum].BoundingBox.MaxZ +
                                          SpawnModel[SpawnModelNum].BoundingBox.MinZ;

                        bspInfo.TagPath = ObstacleList[i].TagPath;
                        bspInfo.TagType = ObstacleList[i].TagType;

                        for (int a = 0; a < treeView1.Nodes.Count; a++)
                        {
                            for (int aa = 0; aa < treeView1.Nodes[a].Nodes.Count; aa++)
                            {
                                if (treeView1.Nodes[a].Nodes[aa].Tag.ToString() == SelectedSpawn[j].ToString())
                                {
                                    string[] temps = bspInfo.TagPath.Split('\\');
                                    this.treeView1.Nodes[a].Nodes[aa].Text = temps[temps.Length - 1];
                                }
                            }
                        }
                    }

                    treeView1.Sort();
                }
            }
        }

        /// <summary>
        /// The combo_ selected index changed scenery model.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void combo_SelectedIndexChangedSceneryModel(object sender, EventArgs e)
        {
            // We need this here so that when the program changes the box, it doesn't change everything selected!
            if (!((ToolStripComboBox)sender).Focused)
            {
                return;
            }

            ToolStripComboBox cb = sender as ToolStripComboBox;

            // looks for a model already on the map. if not FOUND, adds it to the SpawnModels
            bool found = false;
            int SpawnModelNum = -1;

            // Lists all Scenery & Obstacles
            for (int i = 0; i < SceneryList.Count; i++)
            {
                if (SceneryList[i].Name == cb.SelectedItem.ToString())
                {
                    for (int j = 0; j < SpawnModel.Count; j++)
                    {
                        if (SpawnModel[j].name == SceneryList[i].Model.name)
                        {
                            SpawnModelNum = j;
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                    {
                        SpawnModel.Add(SceneryList[i].Model);
                        SpawnModelNum = SpawnModel.Count - 1;
                    }

                    // Change the bounding box for the model
                    float boxwidth = SpawnModel[SpawnModelNum].BoundingBox.MaxX -
                                     SpawnModel[SpawnModelNum].BoundingBox.MinX;
                    float boxheight = SpawnModel[SpawnModelNum].BoundingBox.MaxY -
                                      SpawnModel[SpawnModelNum].BoundingBox.MinY;
                    float boxdepth = SpawnModel[SpawnModelNum].BoundingBox.MaxZ -
                                     SpawnModel[SpawnModelNum].BoundingBox.MinZ;

                    for (int j = 0; j < this.SelectedSpawn.Count; j++)
                    {
                        spawnmodelindex[SelectedSpawn[j]] = SpawnModelNum;
                        BoundingBoxModel[SelectedSpawn[j]] = Mesh.Box(render.device, boxwidth, boxheight, boxdepth);

                        SpawnInfo.BaseSpawn bspInfo = bsp.Spawns.Spawn[SelectedSpawn[j]];
                        bspInfo.bbXDiff = SpawnModel[SpawnModelNum].BoundingBox.MaxX +
                                          SpawnModel[SpawnModelNum].BoundingBox.MinX;
                        bspInfo.bbYDiff = SpawnModel[SpawnModelNum].BoundingBox.MaxY +
                                          SpawnModel[SpawnModelNum].BoundingBox.MinY;
                        bspInfo.bbZDiff = SpawnModel[SpawnModelNum].BoundingBox.MaxZ +
                                          SpawnModel[SpawnModelNum].BoundingBox.MinZ;

                        bspInfo.TagPath = SceneryList[i].TagPath;
                        bspInfo.TagType = SceneryList[i].TagType;

                        for (int a = 0; a < treeView1.Nodes.Count; a++)
                        {
                            for (int aa = 0; aa < treeView1.Nodes[a].Nodes.Count; aa++)
                            {
                                if (treeView1.Nodes[a].Nodes[aa].Tag.ToString() == SelectedSpawn[j].ToString())
                                {
                                    string[] temps = bspInfo.TagPath.Split('\\');
                                    this.treeView1.Nodes[a].Nodes[aa].Text = temps[temps.Length - 1];
                                }
                            }
                        }
                    }

                    treeView1.Sort();
                }
            }
        }

        /// <summary>
        /// The combo_ selected index changed team.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void combo_SelectedIndexChangedTeam(object sender, EventArgs e)
        {
            // We need this here so that when the program changes the box, it doesn't change everything selected!
            if (!((ToolStripComboBox)sender).Focused)
            {
                return;
            }

            ToolStripComboBox cb = sender as ToolStripComboBox;

            foreach (int i in SelectedSpawn)
            {
                if (bsp.Spawns.Spawn[i].Type == SpawnInfo.SpawnType.Objective)
                {
                    SpawnInfo.ObjectiveSpawn os = bsp.Spawns.Spawn[i] as SpawnInfo.ObjectiveSpawn;
                    object test = Enum.Parse(
                        typeof(SpawnInfo.ObjectiveSpawn.TeamType), cb.Items[cb.SelectedIndex].ToString(), true);
                    os.Team = (SpawnInfo.ObjectiveSpawn.TeamType)test;
                    bsp.Spawns.Spawn[i] = os;
                }
            }
        }

        /// <summary>
        /// The data grid row_ lost focus.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void dataGridRow_LostFocus(object sender, DataGridViewCellEventArgs e)
        {
            // If we want to change how it is displayed somehow, we need this to change it back...
        }

        /// <summary>
        /// The data grid row_ select.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void dataGridRow_Select(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView dgView = (DataGridView)sender;
            if (!dgView.Focused)
            {
                return;
            }

            // bsp.Spawns.Spawn[spawnNumber].        // Possibly change the mesh color
        }

        /// <summary>
        /// The data grid_ cell click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void dataGrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView dgView = (DataGridView)sender;
            if ((!dgView.Focused) || (dgView.CurrentCell.RowIndex == dgView.RowCount - 1))
            {
                return;
            }

            // If click happened on Remove("X") column, remove row and selection
            if (dgView.CurrentCell.ColumnIndex == dgView.ColumnCount - 1)
            {
                removeRow(ref dgView, e.RowIndex);
            }
            else if (SelectedSpawn.Count != 0)
            {
                int spawnNumber = int.Parse(dgView[1, dgView.CurrentCell.RowIndex].Value.ToString());
                setCameraPosition(
                    bsp.Spawns.Spawn[spawnNumber].X, 
                    bsp.Spawns.Spawn[spawnNumber].Y, 
                    bsp.Spawns.Spawn[spawnNumber].Z, 
                    false);
            }

            Render();
            updateStatusPosition();
            dgView.Focus();

            // bsp.Spawns.Spawn[spawnNumber].        // Possibly change the mesh color
        }

        /// <summary>
        /// The data grid_resize.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void dataGrid_resize(object sender, EventArgs e)
        {
            DataGridView grid = (DataGridView)sender;
            int totalHeight = grid.ColumnHeadersHeight + 2;
            for (int i = 0; i < grid.Rows.Count; i++)
            {
                totalHeight += grid.Rows[i].Height;
            }

            

            grid.Columns[0].Width = 70;
            grid.Columns[3].Width = 60;
            grid.Columns[4].Width = 60;
            grid.Columns[5].Width = 60;
            grid.Columns[6].Width = 50;
            int totalWidth = grid.Columns[0].Width;
            for (int i = 3; i < 7; i++)
            {
                totalWidth += grid.Columns[i].Width;
            }

            if (totalHeight > (grid.Parent.Height - 50))
            {
                grid.Columns[2].Width = grid.Parent.Width - 40 - totalWidth;
            }
            else
            {
                grid.Columns[2].Width = grid.Parent.Width - 23 - totalWidth;
            }

            
        }

        /// <summary>
        /// The do info.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <remarks></remarks>
        private void doInfo(string name)
        {
            ToolStripTextBox tsInfo = new ToolStripTextBox("tsInfo");
            tsInfo.BackColor = Color.Red;
            tsInfo.BorderStyle = BorderStyle.FixedSingle;
            tsInfo.Padding = new Padding(15, 2, 15, 2);
            tsInfo.Size = new Size(245, 22);
            tsInfo.Text = "Loading " + name + "s, Please wait...";
            tsInfo.TextBoxTextAlign = HorizontalAlignment.Center;
            statusStrip.Items.Insert(0, tsInfo);
            statusStrip.ResumeLayout();
            statusStrip.SuspendLayout();
        }

        /// <summary>
        /// The dock control 6_ enter.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void dockControl6_Enter(object sender, EventArgs e)
        {
            /*  No changes are saved, aka doesn't brick maps ***
            if (firstBSPUsage)
            {
                MessageBox.Show("This is a preliminary testing area! Do NOT save files without all boxes selected or the map will be destroyed?");
                firstBSPUsage = false;
            }
            */
        }

        // D3D.Material[] meshmaterials, Texture[] meshtextures)

        /// <summary>
        /// The fcordbutton_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void fcordbutton_Click(object sender, EventArgs e)
        {
            try
            {
                setCameraPosition(
                    Convert.ToSingle(fcordx.Text), Convert.ToSingle(fcordy.Text), Convert.ToSingle(fcordz.Text), true);
            }
            catch (Exception ex)
            {
                Global.ShowErrorMsg("There was a problem finding your Coordinate.", ex);
            }
        }

        /// <summary>
        /// The findspawn_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void findspawn_Click(object sender, EventArgs e)
        {
            if (SelectedSpawn.Count == 0)
            {
                return;
            }

            cam.Position.X = bsp.Spawns.Spawn[SelectedSpawn[0]].X;
            cam.Position.Y = bsp.Spawns.Spawn[SelectedSpawn[0]].Y;
            cam.Position.Z = bsp.Spawns.Spawn[SelectedSpawn[0]].Z;

            cam.radianv = 0;
            cam.radianh = 0;

            cam.x = bsp.Spawns.Spawn[SelectedSpawn[0]].X;
            cam.y = bsp.Spawns.Spawn[SelectedSpawn[0]].Y;
            cam.z = bsp.Spawns.Spawn[SelectedSpawn[0]].Z;
        }

        /// <summary>
        /// The ident context_ opening.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void identContext_Opening(object sender, CancelEventArgs e)
        {
            if (identContext.SourceControl is TreeView)
            {
                TreeView c = identContext.SourceControl as TreeView;
                if (!c.Focused)
                {
                    c.Focus();
                }

                this.selectFreezeMenuItem.Visible = false;
                this.selectFreezeAllMenuItem.Visible = false;
                this.selectUnFreezeAllMenuItem.Visible = false;
                this.selectCurrentToolStripMenuItem.Visible = false;
                this.selectGroupToolStripMenuItem.Visible = false;
                if (c.SelectedNode == null)
                {
                    this.selectAllToolStripMenuItem.Visible = false;
                    return;
                }
                else
                {
                    this.selectAllToolStripMenuItem.Visible = true;
                    this.selectAllToolStripMenuItem.Tag = "ALL" + c.SelectedNode.Tag;
                }

                // Sets the parent node or the selected node if it is already the top Node
                string Parent;
                if (c.SelectedNode.Parent != null)
                {
                    Parent = c.SelectedNode.Parent.Text;
                    this.selectCurrentToolStripMenuItem.Visible = true;
                    if (SelectedSpawn.IndexOf(int.Parse(c.SelectedNode.Tag.ToString())) == -1)
                    {
                        this.selectCurrentToolStripMenuItem.Text = "Select";
                        this.selectCurrentToolStripMenuItem.Tag = "SEL" + c.SelectedNode.Tag;
                    }
                    else
                    {
                        this.selectCurrentToolStripMenuItem.Text = "Deselect";
                        this.selectCurrentToolStripMenuItem.Tag = "DES" + c.SelectedNode.Tag;
                    }
                }
                else
                {
                    Parent = c.SelectedNode.Text;
                }

                string tag = c.SelectedNode.Text;
                tag = tag.Substring(0, (tag + " ").IndexOf(" "));

                // Updates right-click menu selections
                this.selectAllToolStripMenuItem.Text = "Select all " + Parent + "s";
                if (Parent.ToUpper() == "OBJECTIVE")
                {
                    // Option to select the Arming Circle
                    if (tag == SpawnInfo.ObjectiveSpawn.ObjectiveTypeEnum.ArmingCircle.ToString())
                    {
                        this.selectGroupToolStripMenuItem.Text = "Select group> " +
                                                                 c.SelectedNode.Text.Substring(
                                                                     0, c.SelectedNode.Text.LastIndexOf('#'));
                        this.selectGroupToolStripMenuItem.Tag = "ARM" + c.SelectedNode.Tag;
                        this.selectGroupToolStripMenuItem.Visible = true;
                    }

                    // Option to select all of one hill
                    if (
                        tag.StartsWith(
                            SpawnInfo.ObjectiveSpawn.ObjectiveTypeEnum.KingOfTheHill_1.ToString().Substring(0, 13)))
                    {
                        this.selectGroupToolStripMenuItem.Text = "Select group> Hill #" +
                                                                 tag.Substring(tag.Length - 1, 1);
                        this.selectGroupToolStripMenuItem.Tag = "HIL" + c.SelectedNode.Tag;
                        this.selectGroupToolStripMenuItem.Visible = true;
                    }

                    // Territories are selected by same color and #
                    if (tag == SpawnInfo.ObjectiveSpawn.ObjectiveTypeEnum.Territory.ToString())
                    {
                        this.selectGroupToolStripMenuItem.Text = "Select group> " + c.SelectedNode.Text;
                        this.selectGroupToolStripMenuItem.Tag = "TER" + c.SelectedNode.Tag;
                        this.selectGroupToolStripMenuItem.Visible = true;
                    }
                }

                this.selectNoneToolStripMenuItem.Tag = "DES-1";
            }
            else if (identContext.SourceControl is BSPViewer)
            {
                #region BSPViewer_Click
                    Time = DateTime.Now.TimeOfDay.Subtract(Time);
                if ((Time.Ticks / 1000000) >= 3)
                {
                    identContext.Hide();
                    return;
                }

                BSPViewer c = identContext.SourceControl as BSPViewer;

                // + 20 for Title bar
                MouseEventArgs me = new MouseEventArgs(
                    MouseButtons, 
                    1, 
                    MousePosition.X - this.Left, 
                    MousePosition.Y - this.Top - SystemInformation.CaptionHeight -
                    (SystemInformation.BorderSize.Height * 2), 
                    0);
                int[] intersect = checkForIntersection(me);
                if (intersect.Length > 0)
                {
                    currentObject = intersect[0];
                }
                else
                {
                    currentObject = -1;
                }

                this.selectFreezeMenuItem.Visible = false;
                this.selectFreezeAllMenuItem.Visible = true;
                this.selectUnFreezeAllMenuItem.Visible = true;
                this.selectCurrentToolStripMenuItem.Visible = false;
                this.selectGroupToolStripMenuItem.Visible = false;

                string tag = null;
                if (currentObject > -1)
                {
                    this.selectFreezeMenuItem.Visible = true;
                    if (bsp.Spawns.Spawn[currentObject].frozen)
                    {
                        this.selectFreezeMenuItem.Text = "UnFreeze";
                    }
                    else
                    {
                        this.selectFreezeMenuItem.Text = "Freeze";
                    }

                    switch (bsp.Spawns.Spawn[currentObject].Type)
                    {
                        case SpawnInfo.SpawnType.Objective:
                            SpawnInfo.ObjectiveSpawn spawn = (SpawnInfo.ObjectiveSpawn)bsp.Spawns.Spawn[currentObject];
                            tag = spawn.ObjectiveType + "#" + spawn.number;

                            // Option to select the Arming Circle
                            if (spawn.ObjectiveType == SpawnInfo.ObjectiveSpawn.ObjectiveTypeEnum.ArmingCircle)
                            {
                                this.selectGroupToolStripMenuItem.Text = "Select group> " + spawn.ObjectiveType;
                                this.selectGroupToolStripMenuItem.Tag = "ARM" + tag;
                                this.selectGroupToolStripMenuItem.Visible = true;
                            }

                                // Option to select all of one hill
                            else if (
                                spawn.ObjectiveType.ToString().StartsWith(
                                    SpawnInfo.ObjectiveSpawn.ObjectiveTypeEnum.KingOfTheHill_1.ToString().Substring(
                                        0, 13)))
                            {
                                this.selectGroupToolStripMenuItem.Text = "Select group> Hill #" + spawn.number;
                                this.selectGroupToolStripMenuItem.Tag = "HIL" + tag;
                                this.selectGroupToolStripMenuItem.Visible = true;
                            }

                                // Territories are selected by same color and #
                            else if (spawn.ObjectiveType == SpawnInfo.ObjectiveSpawn.ObjectiveTypeEnum.Territory)
                            {
                                this.selectGroupToolStripMenuItem.Text = "Select group> " + spawn.ObjectiveType +
                                                                         "#" + spawn.number;
                                this.selectGroupToolStripMenuItem.Tag = "TER" + tag;
                                this.selectGroupToolStripMenuItem.Visible = true;
                            }

                            break;
                        case SpawnInfo.SpawnType.Light:
                        case SpawnInfo.SpawnType.Sound:
                        case SpawnInfo.SpawnType.Scenery:
                        case SpawnInfo.SpawnType.Collection:
                            string[] s = bsp.Spawns.Spawn[currentObject].TagPath.Split('\\');
                            tag = s[s.Length - 1];
                            break;
                        default:
                            tag = bsp.Spawns.Spawn[currentObject].Type.ToString();
                            break;
                    }

                    this.selectAllToolStripMenuItem.Visible = true;
                    this.selectAllToolStripMenuItem.Text = "Select all " + bsp.Spawns.Spawn[currentObject].Type + "s";
                }
                else
                {
                    this.selectAllToolStripMenuItem.Visible = false;
                }

                for (int i = 0; i < this.identContext.Items.Count; i++)
                {
                    this.identContext.Items[i].Tag = "viewer";
                }

                #endregion
            }
        }

        /// <summary>
        /// The left sand dock_ mouse leave.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void leftSandDock_MouseLeave(object sender, EventArgs e)
        {
            if (!statusStrip.ContainsFocus)
            {
                speedLabel.Focus();
            }
        }

        /// <summary>
        /// The list form_resize.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void listForm_resize(object sender, EventArgs e)
        {
            DataGridView grid = (DataGridView)((Form)sender).Controls[0];
            grid.Width = ((Form)sender).Width - 20;
            grid.Height = ((Form)sender).Height - 50;
        }

        /// <summary>
        /// The make digits only.
        /// </summary>
        /// <param name="tb">The tb.</param>
        /// <remarks></remarks>
        private void makeDigitsOnly(ref ToolStripTextBox tb)
        {
            try
            {
                float.Parse(tb.Text);
            }
            catch
            {
                int i = 0;
                while (i < tb.Text.Length)
                {
                    if (!char.IsDigit(tb.Text[i]) && !char.IsPunctuation(tb.Text[i]))
                    {
                        tb.Text = tb.Text.Remove(i, 1);
                    }
                    else
                    {
                        i++;
                    }
                }

                if (tb.Text == string.Empty)
                {
                    tb.Text = "0";
                }

                tb.SelectionStart = 0;
                tb.SelectionLength = tb.Text.Length;
            }
        }

        /// <summary>
        /// The make digits only.
        /// </summary>
        /// <param name="tb">The tb.</param>
        /// <returns>The make digits only.</returns>
        /// <remarks></remarks>
        private float makeDigitsOnly(ToolStripTextBox tb)
        {
            string t = tb.Text;
            try
            {
                float.Parse(t);
            }
            catch
            {
                int i = 0;
                while (i < t.Length)
                {
                    if (!char.IsDigit(t[i]) && !char.IsPunctuation(tb.Text[i]))
                    {
                        tb.Text = tb.Text.Remove(i, 1);
                    }
                    else
                    {
                        i++;
                    }
                }

                if (tb.Text == string.Empty)
                {
                    tb.Text = "0";
                }

                tb.SelectionStart = 0;
                tb.SelectionLength = tb.Text.Length;
            }

            return float.Parse(tb.Text);
        }

        /// <summary>
        /// The mi 4_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void mi4_Click(object sender, EventArgs e)
        {
            if (SelectedSpawn.Count != 1)
            {
                MessageBox.Show("Select One Object Or Spawn");
            }
            else
            {
                for (int i = 0; i < bsp.Spawns.Spawn.Count; i++)
                {
                    if (((int)bsp.Spawns.Spawn[i].Type & visibleSpawnsBitMask) == 0)
                    {
                        continue;
                    }

                    bsp.Spawns.Spawn[i].X = bsp.Spawns.Spawn[SelectedSpawn[0]].X;
                    bsp.Spawns.Spawn[i].Y = bsp.Spawns.Spawn[SelectedSpawn[0]].Y;
                    bsp.Spawns.Spawn[i].Z = bsp.Spawns.Spawn[SelectedSpawn[0]].Z;
                    TranslationMatrix[i] = MakeMatrixForSpawn(i);
                }
            }
        }

        /// <summary>
        /// The radio button 1_ checked changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                checkBox2.Text = "Colorize";
                checkBox2.Checked = false;
                trackBar1.Value = 0;
                trackBar2.Value = 0;
                trackBar3.Value = 0;
                Array.ConstrainedCopy(LightMap_Array_Backup, 0, LightMap_Array, 0, LightMap_Array.Length);

                foreach (PictureBox picbox in panel1.Controls)
                {
                    picbox.Image = RenderLightmap((int)((picbox).Tag));
                }
            }
            else
            {
                checkBox2.Text = "Preserve Lightness";
                checkBox2.Checked = true;
                trackBar1.Value = 0;
                trackBar2.Value = 0;
                trackBar3.Value = 0;
                Array.ConstrainedCopy(LightMap_Array_Backup, 0, LightMap_Array, 0, LightMap_Array.Length);

                foreach (PictureBox picbox in panel1.Controls)
                {
                    picbox.Image = RenderLightmap((int)((picbox).Tag));
                }
            }
        }

        /// <summary>
        /// The radio button 2_ checked changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                checkBox2.Text = "Colorize";
                checkBox2.Checked = false;
                trackBar1.Value = 0;
                trackBar2.Value = 0;
                trackBar3.Value = 0;
                Array.ConstrainedCopy(LightMap_Array_Backup, 0, LightMap_Array, 0, LightMap_Array.Length);

                foreach (PictureBox picbox in panel1.Controls)
                {
                    picbox.Image = RenderLightmap((int)((picbox).Tag));
                }

                EditLightmaps();
            }
            else
            {
                checkBox2.Text = "Preserve Lightness";
                checkBox2.Checked = true;
                trackBar1.Value = 0;
                trackBar2.Value = 0;
                trackBar3.Value = 0;
                Array.ConstrainedCopy(LightMap_Array_Backup, 0, LightMap_Array, 0, LightMap_Array.Length);

                foreach (PictureBox picbox in panel1.Controls)
                {
                    picbox.Image = RenderLightmap((int)((picbox).Tag));
                }

                EditLightmaps();
            }
        }

        /// <summary>
        /// The remove row.
        /// </summary>
        /// <param name="dgView">The dg view.</param>
        /// <param name="rowNumber">The row number.</param>
        /// <remarks></remarks>
        private void removeRow(ref DataGridView dgView, int rowNumber)
        {
            if (SelectedSpawn.Count == dgView.RowCount - 1)
            {
                SelectedSpawn.RemoveAt(dgView.CurrentCell.RowIndex);
            }
            else
            {
                // Safety in case something messed up...
                int number = int.Parse(dgView[1, dgView.CurrentCell.RowIndex].Value.ToString());
                for (int i = 0; i < SelectedSpawn.Count; i++)
                {
                    if (number == SelectedSpawn[i])
                    {
                        SelectedSpawn.RemoveAt(i);
                        break;
                    }
                }
            }

            dgView.Rows.RemoveAt(dgView.CurrentCell.RowIndex);
            if ((rowNumber > 0) && (rowNumber == dgView.RowCount - 1))
            {
                dgView.CurrentCell = dgView[0, rowNumber - 1];

                // = dgView.CurrentCell;
            }
        }

        /// <summary>
        /// The select spawn.
        /// </summary>
        /// <param name="tagNumber">The tag number.</param>
        /// <remarks></remarks>
        private void selectSpawn(int tagNumber)
        {
            // This section makes sure that the type we have selected is checked for viewing

            
            int index = checkedListBox1.FindString(bsp.Spawns.Spawn[tagNumber].Type.ToString(), 0);
            if (checkedListBox1.GetItemCheckState(index) == CheckState.Unchecked)
            {
                checkedListBox1.SetItemCheckState(index, CheckState.Checked);
            }

            

            #region TurnSpawnOnOrOff

            // Find out what section our tagNumber is in
            int tempi = SelectedSpawn.IndexOf(tagNumber);

            // If our spawn is already selected, deselect and move it to the last position
            if (tempi != -1)
            {
                SelectedSpawn.RemoveAt(tempi);
            }

            if (!bsp.Spawns.Spawn[tagNumber].frozen)
            {
                SelectedSpawn.Add(tagNumber);
            }

            // Update the global current selected spawn type
            selectedSpawnType = bsp.Spawns.Spawn[tagNumber].Type;

            #endregion
        }

        /// <summary>
        /// The select tool strip menu item_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void selectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                switch (((ToolStripMenuItem)sender).Tag.ToString())
                {
                    case "viewer":
                        selectToolStripMenuItem_ViewerClick(sender, e);
                        break;
                    default:
                        selectToolStripMenuItem_TreeClick(sender, e);
                        break;
                }
            }
            catch (Exception ex)
            {
                Global.ShowErrorMsg("Unkown location error", ex);
            }
        }

        /// <summary>
        /// The select tool strip menu item_ tree click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void selectToolStripMenuItem_TreeClick(object sender, EventArgs e)
        {
            TreeView c = identContext.SourceControl as TreeView;
            TreeNode cNode = c.SelectedNode;
            string s = ((ToolStripMenuItem)sender).Tag.ToString();
            string id = s.Substring(0, 3);
            int tagNumber = int.Parse(s.Substring(3));

            SpawnInfo.SpawnType spType;
            SpawnInfo.ObjectiveSpawn spObj;
            switch (id)
            {
                    // For a single spawn selection
                case "SEL":
                    selectSpawn(tagNumber);
                    break;

                    // For a single spawn selection
                case "DES":
                    if (tagNumber != -1)
                    {
                        SelectedSpawn.RemoveAt(SelectedSpawn.IndexOf(tagNumber));
                    }
                    else
                    {
                        SelectedSpawn.Clear();
                    }

                    break;

                    // For selecting all spawns of a certain Type
                case "ALL":
                    if (tagNumber == -1)
                    {
                        cNode = cNode.FirstNode;
                        if (cNode == null)
                        {
                            return;
                        }

                        tagNumber = int.Parse(cNode.Tag.ToString());
                    }

                    spType = bsp.Spawns.Spawn[tagNumber].Type;
                    for (int x = 0; x < bsp.Spawns.Spawn.Count; x++)
                    {
                        if (bsp.Spawns.Spawn[x].Type == spType)
                        {
                            selectSpawn(x);
                        }
                    }

                    break;

                    // For selecting Arming Circles
                case "ARM":
                    spType = bsp.Spawns.Spawn[tagNumber].Type;
                    spObj = (SpawnInfo.ObjectiveSpawn)bsp.Spawns.Spawn[tagNumber];
                    for (int x = 0; x < bsp.Spawns.Spawn.Count; x++)
                    {
                        if (bsp.Spawns.Spawn[x].Type == spType)
                        {
                            SpawnInfo.ObjectiveSpawn currObj = (SpawnInfo.ObjectiveSpawn)bsp.Spawns.Spawn[x];
                            if ((currObj.ObjectiveType == spObj.ObjectiveType) && (currObj.Team == spObj.Team))
                            {
                                selectSpawn(x);
                            }
                        }
                    }

                    break;

                    // For selecting King of the Hills
                case "HIL":
                    spType = bsp.Spawns.Spawn[tagNumber].Type;
                    spObj = (SpawnInfo.ObjectiveSpawn)bsp.Spawns.Spawn[tagNumber];
                    for (int x = 0; x < bsp.Spawns.Spawn.Count; x++)
                    {
                        if (bsp.Spawns.Spawn[x].Type == spType)
                        {
                            SpawnInfo.ObjectiveSpawn currObj = (SpawnInfo.ObjectiveSpawn)bsp.Spawns.Spawn[x];
                            if ((currObj.ObjectiveType == spObj.ObjectiveType) && (currObj.Team == spObj.Team))
                            {
                                selectSpawn(x);
                            }
                        }
                    }

                    break;
            }

            updateStatusPosition();
        }

        /// <summary>
        /// The select tool strip menu item_ viewer click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void selectToolStripMenuItem_ViewerClick(object sender, EventArgs e)
        {
            ToolStripDropDownItem current = (ToolStripDropDownItem)sender;
            if (current.Text.ToLower() == "unfreeze all")
            {
                for (int i = 0; i < bsp.Spawns.Spawn.Count; i++)
                {
                    bsp.Spawns.Spawn[i].frozen = false;
                }
            }
            else if (current.Text.ToLower().Contains("freeze all"))
            {
                for (int i = 0; i < bsp.Spawns.Spawn.Count; i++)
                {
                    if (SelectedSpawn.IndexOf(i) == -1)
                    {
                        bsp.Spawns.Spawn[i].frozen = true;
                    }
                }
            }
            else if (current.Text.ToLower().Contains("freeze"))
            {
                int ss = SelectedSpawn.IndexOf(currentObject);
                if (ss != -1)
                {
                    SelectedSpawn.RemoveAt(ss);
                }

                bsp.Spawns.Spawn[currentObject].frozen = !bsp.Spawns.Spawn[currentObject].frozen;
            }
            else if (current.Text.ToLower().StartsWith("select group"))
            {
                SpawnInfo.ObjectiveSpawn objSpawn = (SpawnInfo.ObjectiveSpawn)bsp.Spawns.Spawn[currentObject];
                for (int i = 0; i < bsp.Spawns.Spawn.Count; i++)
                {
                    if (bsp.Spawns.Spawn[i].Type == SpawnInfo.SpawnType.Objective &&
                        ((SpawnInfo.ObjectiveSpawn)bsp.Spawns.Spawn[i]).ObjectiveType == objSpawn.ObjectiveType)
                    {
                        switch (objSpawn.ObjectiveType)
                        {
                            case SpawnInfo.ObjectiveSpawn.ObjectiveTypeEnum.KingOfTheHill_1:
                            case SpawnInfo.ObjectiveSpawn.ObjectiveTypeEnum.KingOfTheHill_2:
                            case SpawnInfo.ObjectiveSpawn.ObjectiveTypeEnum.KingOfTheHill_3:
                            case SpawnInfo.ObjectiveSpawn.ObjectiveTypeEnum.KingOfTheHill_4:
                            case SpawnInfo.ObjectiveSpawn.ObjectiveTypeEnum.KingOfTheHill_5:
                                selectSpawn(i);
                                break;
                            case SpawnInfo.ObjectiveSpawn.ObjectiveTypeEnum.ArmingCircle:
                                if (((SpawnInfo.ObjectiveSpawn)bsp.Spawns.Spawn[i]).Team == objSpawn.Team)
                                {
                                    selectSpawn(i);
                                }

                                break;
                            case SpawnInfo.ObjectiveSpawn.ObjectiveTypeEnum.OddballSpawn:
                            case SpawnInfo.ObjectiveSpawn.ObjectiveTypeEnum.Territory:
                                if (((SpawnInfo.ObjectiveSpawn)bsp.Spawns.Spawn[i]).number == objSpawn.number)
                                {
                                    selectSpawn(i);
                                }

                                break;
                        }
                    }
                }
            }
            else if (current.Text.ToLower() == "select ...")
            {
                selectSpawn(currentObject);
            }
            else if (current.Text.ToLower().StartsWith("select all"))
            {
                for (int i = 0; i < bsp.Spawns.Spawn.Count; i++)
                {
                    if (bsp.Spawns.Spawn[i].Type == bsp.Spawns.Spawn[currentObject].Type)
                    {
                        selectSpawn(i);
                    }
                }
            }
            else if (current.Text.ToLower().StartsWith("deselect all"))
            {
                SelectedSpawn.Clear();
            }
            else if (current.Text.ToLower().StartsWith("deselect"))
            {
                SelectedSpawn.Clear();
            }

            /*
            string id = s.Substring(0, 3);
            int tagNumber = int.Parse(s.Substring(3));

            Renderer.BSP_Renderer.SpawnInfo.SpawnType spType;
            Renderer.BSP_Renderer.SpawnInfo.ObjectiveSpawn spObj;
            switch (id)
            {
                // For a single spawn selection
                case "SEL":
                    selectSpawn(tagNumber);
                    break;

                // For a single spawn selection
                case "DES":
                    if (tagNumber != -1)
                        SelectedSpawn.RemoveAt(SelectedSpawn.IndexOf(tagNumber));
                    else
                        SelectedSpawn.Clear();
                    break;

                // For selecting all spawns of a certain Type
                case "ALL":
                    if (tagNumber == -1)
                    {
                        cNode = cNode.FirstNode;
                        if (cNode == null) { return; }
                        tagNumber = int.Parse(cNode.Tag.ToString());
                    }
                    spType = bsp.Spawns.Spawn[tagNumber].Type;
                    for (int x = 0; x < bsp.Spawns.Spawn.Count; x++)
                    {
                        if (bsp.Spawns.Spawn[x].Type == spType)
                            selectSpawn(x);
                    }
                    break;

                // For selecting Arming Circles
                case "ARM":
                    spType = bsp.Spawns.Spawn[tagNumber].Type;
                    spObj = (Renderer.BSP_Renderer.SpawnInfo.ObjectiveSpawn)bsp.Spawns.Spawn[tagNumber];
                    for (int x = 0; x < bsp.Spawns.Spawn.Count; x++)
                    {
                        if (bsp.Spawns.Spawn[x].Type == spType)
                        {
                            Renderer.BSP_Renderer.SpawnInfo.ObjectiveSpawn currObj = (Renderer.BSP_Renderer.SpawnInfo.ObjectiveSpawn)bsp.Spawns.Spawn[x];
                            if ((currObj.ObjectiveType == spObj.ObjectiveType) && (currObj.Team == spObj.Team))
                                selectSpawn(x);
                        }
                    }
                    break;

                // For selecting King of the Hills
                case "HIL":
                    spType = bsp.Spawns.Spawn[tagNumber].Type;
                    spObj = (Renderer.BSP_Renderer.SpawnInfo.ObjectiveSpawn)bsp.Spawns.Spawn[tagNumber];
                    for (int x = 0; x < bsp.Spawns.Spawn.Count; x++)
                    {
                        if (bsp.Spawns.Spawn[x].Type == spType)
                        {
                            Renderer.BSP_Renderer.SpawnInfo.ObjectiveSpawn currObj = (Renderer.BSP_Renderer.SpawnInfo.ObjectiveSpawn)bsp.Spawns.Spawn[x];
                            if ((currObj.ObjectiveType == spObj.ObjectiveType) && (currObj.Team == spObj.Team))
                                selectSpawn(x);
                        }
                    }
                    break;
            }
            */
            updateStatusPosition();
        }

        /// <summary>
        /// The set spawn box.
        /// </summary>
        /// <param name="checkeditemsname">The checkeditemsname.</param>
        /// <param name="state">The state.</param>
        /// <remarks></remarks>
        private void setSpawnBox(string checkeditemsname, CheckState state)
        {
            Type typ = typeof(SpawnInfo.SpawnType);
            SpawnInfo.SpawnType bm = (SpawnInfo.SpawnType)Enum.Parse(typ, checkeditemsname, true);

            switch (bm)
            {
                case SpawnInfo.SpawnType.Collection:
                    if (WeaponsList == null)
                    {
                        doInfo(bm.ToString());
                        spawns.LoadWeapons(ref WeaponsList);
                        statusStrip.Items.Clear();
                    }

                    break;
                case SpawnInfo.SpawnType.Objective:
                    if (!spawns.hillsLoaded)
                    {
                        doInfo(bm.ToString());
                        spawns.createHills();
                        statusStrip.Items.Clear();
                    }

                    break;
                case SpawnInfo.SpawnType.Obstacle:
                    if (ObstacleList == null)
                    {
                        doInfo(bm.ToString());
                        spawns.LoadObstacles(ref ObstacleList);
                        statusStrip.Items.Clear();
                    }

                    break;
                case SpawnInfo.SpawnType.Scenery:
                    if (SceneryList == null)
                    {
                        doInfo(bm.ToString());
                        spawns.LoadScenery(ref SceneryList);
                        statusStrip.Items.Clear();
                    }

                    break;
                case SpawnInfo.SpawnType.Sound:
                    if (SoundsList == null)
                    {
                        doInfo(bm.ToString());
                        spawns.LoadSoundScenery(ref SoundsList);
                        statusStrip.Items.Clear();
                    }

                    break;
            }

            if (state == CheckState.Checked)
            {
                visibleSpawnsBitMask |= (int)bm;
            }
            else
            {
                visibleSpawnsBitMask &= int.MaxValue - (int)bm;
            }
        }

        /// <summary>
        /// The speed bar_ key down.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void speedBar_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode.ToString())
            {
                case "Left":
                case "Right":
                case "Up":
                case "Down":
                case "PageUp":
                case "Next": // PageDown
                    e.SuppressKeyPress = true;
                    break;
                default:

                    // MessageBox.Show(e.KeyCode.ToString());
                    break;
            }
        }

        /// <summary>
        /// The speed bar_ scroll.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void speedBar_Scroll(object sender, EventArgs e)
        {
            if (speedBar.Value < 100)
            {
                cam.speed = speedBar.Value / 100.0F;
            }
            else
            {
                cam.speed = (speedBar.Value - 90) / 10.0F;
            }
        }

        /// <summary>
        /// The speed bar_ value changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void speedBar_ValueChanged(object sender, EventArgs e)
        {
            speedLabel.Text = ((float)((int)(cam.speed * 100)) / 100).ToString();
        }

        /// <summary>
        /// The tb_ text changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void tb_TextChanged(object sender, EventArgs e)
        {
            ToolStripTextBox tb = sender as ToolStripTextBox;

            try
            {
                if (short.Parse(tb.Text) < 0)
                {
                    tb.Text = "0";
                }
            }
            catch
            {
                return;
            }

            foreach (int i in SelectedSpawn)
            {
                if (bsp.Spawns.Spawn[i].Type == SpawnInfo.SpawnType.Objective)
                {
                    SpawnInfo.ObjectiveSpawn os = bsp.Spawns.Spawn[i] as SpawnInfo.ObjectiveSpawn;
                    os.number = short.Parse(tb.Text);
                    bsp.Spawns.Spawn[i] = os;
                }
            }

            this.Focus();
        }

        /// <summary>
        /// The text sound_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void textSound_Click(object sender, EventArgs e)
        {
            // We need this here so that when the program changes the box, it doesn't give it focus!
            if (!((ToolStripTextBox)sender).Focused)
            {
                return;
            }

            ToolStripTextBox cb = sender as ToolStripTextBox;
        }

        /// <summary>
        /// The text sound_ got focus.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void textSound_GotFocus(object sender, EventArgs e)
        {
            ToolStripTextBox cb = sender as ToolStripTextBox;
            for (int j = 0; j < this.SelectedSpawn.Count; j++)
            {
                SpawnInfo.SoundSpawn sp = bsp.Spawns.Spawn[SelectedSpawn[j]] as SpawnInfo.SoundSpawn;

                // Clear our old mesh and show a new mesh for the outer size
                BoundingBoxModel[SelectedSpawn[j]].Dispose();
                switch (sp.VolumeType)
                {
                    case 0:
                        if (sp.DistanceBoundsUpper < 30)
                        {
                            BoundingBoxModel[SelectedSpawn[j]] = Mesh.Sphere(
                                render.device, 
                                sp.DistanceBoundsUpper, 
                                10 + (int)sp.DistanceBoundsUpper, 
                                10 + (int)sp.DistanceBoundsUpper);
                        }
                        else
                        {
                            BoundingBoxModel[SelectedSpawn[j]] = Mesh.Sphere(
                                render.device, sp.DistanceBoundsUpper, 30, 30);
                        }

                        break;
                    case 1:
                        BoundingBoxModel[SelectedSpawn[j]] = Mesh.Cylinder(
                            render.device, 
                            sp.DistanceBoundsUpper, 
                            sp.DistanceBoundsUpper, 
                            sp.Height, 
                            10 + (int)sp.DistanceBoundsLower, 
                            10 + (int)sp.DistanceBoundsLower);
                        break;
                    default:
                        BoundingBoxModel[SelectedSpawn[j]] = Mesh.Cylinder(
                            render.device, 
                            sp.DistanceBoundsLower, 
                            sp.DistanceBoundsUpper, 
                            sp.Height, 
                            10 + (int)sp.DistanceBoundsLower, 
                            10 + (int)sp.DistanceBoundsLower);
                        break;
                }

                if (cb.SelectionLength != cb.Text.Length)
                {
                    cb.SelectionStart = 0;
                    cb.SelectionLength = cb.Text.Length;
                }
            }
        }

        /// <summary>
        /// The text sound_ lost focus.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void textSound_LostFocus(object sender, EventArgs e)
        {
            ToolStripTextBox cb = sender as ToolStripTextBox;

            // This makes it stay large even with mouse over viewer.... hmmm
            // if (statusStrip.Focused) { return; }

            for (int j = 0; j < this.SelectedSpawn.Count; j++)
            {
                SpawnInfo.SoundSpawn sp = bsp.Spawns.Spawn[SelectedSpawn[j]] as SpawnInfo.SoundSpawn;
                // Clear our old mesh and show a new mesh for the outer size
                BoundingBoxModel[SelectedSpawn[j]].Dispose();
                switch (sp.VolumeType)
                {
                    case 0:
                        if (sp.DistanceBoundsLower < 30)
                        {
                            BoundingBoxModel[SelectedSpawn[j]] = Mesh.Sphere(
                                render.device, 
                                sp.DistanceBoundsLower, 
                                10 + (int)sp.DistanceBoundsLower, 
                                10 + (int)sp.DistanceBoundsLower);
                        }
                        else
                        {
                            BoundingBoxModel[SelectedSpawn[j]] = Mesh.Sphere(
                                render.device, sp.DistanceBoundsLower, 30, 30);
                        }

                        break;
                    case 1:
                        BoundingBoxModel[SelectedSpawn[j]] = Mesh.Cylinder(
                            render.device, 
                            sp.DistanceBoundsUpper, 
                            sp.DistanceBoundsUpper, 
                            sp.Height, 
                            10 + (int)sp.DistanceBoundsLower, 
                            10 + (int)sp.DistanceBoundsLower);
                        break;
                    default:
                        BoundingBoxModel[SelectedSpawn[j]] = Mesh.Cylinder(
                            render.device, 
                            sp.DistanceBoundsLower, 
                            sp.DistanceBoundsUpper, 
                            sp.Height, 
                            10 + (int)sp.DistanceBoundsLower, 
                            10 + (int)sp.DistanceBoundsLower);
                        break;
                }
            }
        }

        /// <summary>
        /// The text sound_ lost mouse focus.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void textSound_LostMouseFocus(object sender, EventArgs e)
        {
            ToolStripTextBox cb = sender as ToolStripTextBox;
            if (!cb.Focused)
            {
                textSound_LostFocus(sender, e);
            }
        }

        /// <summary>
        /// The text sound_ text changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void textSound_TextChanged(object sender, EventArgs e)
        {
            // We need this here so that when the program changes the box, it doesn't give it focus!
            if (!((ToolStripTextBox)sender).Focused)
            {
                return;
            }

            ToolStripTextBox cb = sender as ToolStripTextBox;

            for (int j = 0; j < this.SelectedSpawn.Count; j++)
            {
                SpawnInfo.SoundSpawn sp = bsp.Spawns.Spawn[SelectedSpawn[j]] as SpawnInfo.SoundSpawn;

                // Clear our old mesh for the creation a new mesh of either size
                BoundingBoxModel[SelectedSpawn[j]].Dispose();
                if (cb.Name == "SoundSceneryInnerSize")
                {
                    // Update the value stored in the SoundScenery
                    sp.DistanceBoundsLower = makeDigitsOnly(cb);
                    switch (sp.VolumeType)
                    {
                        case 0:
                            if (sp.DistanceBoundsLower < 30)
                            {
                                BoundingBoxModel[SelectedSpawn[j]] = Mesh.Sphere(
                                    render.device, 
                                    sp.DistanceBoundsLower, 
                                    10 + (int)sp.DistanceBoundsLower, 
                                    10 + (int)sp.DistanceBoundsLower);
                            }
                            else
                            {
                                BoundingBoxModel[SelectedSpawn[j]] = Mesh.Sphere(
                                    render.device, sp.DistanceBoundsLower, 30, 30);
                            }

                            break;
                        case 1:
                            BoundingBoxModel[SelectedSpawn[j]] = Mesh.Cylinder(
                                render.device, 
                                sp.DistanceBoundsLower, 
                                sp.DistanceBoundsUpper, 
                                sp.Height, 
                                10 + (int)sp.DistanceBoundsLower, 
                                10 + (int)sp.DistanceBoundsLower);
                            break;
                        default:
                            BoundingBoxModel[SelectedSpawn[j]] = Mesh.Cylinder(
                                render.device, 
                                sp.DistanceBoundsLower, 
                                sp.DistanceBoundsUpper, 
                                sp.Height, 
                                10 + (int)sp.DistanceBoundsLower, 
                                10 + (int)sp.DistanceBoundsLower);
                            break;
                    }
                }
                else if (cb.Name == "SoundSceneryOuterSize")
                {
                    sp.DistanceBoundsUpper = makeDigitsOnly(cb);
                    switch (sp.VolumeType)
                    {
                        case 0:
                            if (sp.DistanceBoundsUpper < 30)
                            {
                                BoundingBoxModel[SelectedSpawn[j]] = Mesh.Sphere(
                                    render.device, 
                                    sp.DistanceBoundsUpper, 
                                    10 + (int)sp.DistanceBoundsUpper, 
                                    10 + (int)sp.DistanceBoundsUpper);
                            }
                            else
                            {
                                BoundingBoxModel[SelectedSpawn[j]] = Mesh.Sphere(
                                    render.device, sp.DistanceBoundsUpper, 30, 30);
                            }

                            break;
                        case 1:
                            BoundingBoxModel[SelectedSpawn[j]] = Mesh.Cylinder(
                                render.device, 
                                sp.DistanceBoundsLower, 
                                sp.DistanceBoundsUpper, 
                                sp.Height, 
                                10 + (int)sp.DistanceBoundsLower, 
                                10 + (int)sp.DistanceBoundsLower);
                            break;
                        default:
                            BoundingBoxModel[SelectedSpawn[j]] = Mesh.Cylinder(
                                render.device, 
                                sp.DistanceBoundsLower, 
                                sp.DistanceBoundsUpper, 
                                sp.Height, 
                                10 + (int)sp.DistanceBoundsLower, 
                                10 + (int)sp.DistanceBoundsLower);
                            break;
                    }
                }

                // Used for updating tree display nodes
                for (int a = 0; a < treeView1.Nodes.Count; a++)
                {
                    for (int aa = 0; aa < treeView1.Nodes[a].Nodes.Count; aa++)
                    {
                        if (treeView1.Nodes[a].Nodes[aa].Tag.ToString() == SelectedSpawn[j].ToString())
                        {
                            // MessageBox.Show("Node Updates");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The tool strip button reset_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void toolStripButtonReset_Click(object sender, EventArgs e)
        {
            foreach (int i in SelectedSpawn)
            {
                SpawnInfo.BaseSpawn spawn = bsp.Spawns.Spawn[i];
                if (spawn is SpawnInfo.RotateYawPitchRollBaseSpawn)
                {
                    ((SpawnInfo.RotateYawPitchRollBaseSpawn)spawn).Yaw = 0;
                    ((SpawnInfo.RotateYawPitchRollBaseSpawn)spawn).Pitch = 0;
                    ((SpawnInfo.RotateYawPitchRollBaseSpawn)spawn).Roll = 0;
                }
                else if (spawn is SpawnInfo.RotateDirectionBaseSpawn)
                {
                    ((SpawnInfo.RotateDirectionBaseSpawn)spawn).RotationDirection = 0;
                }
            }
        }

        /// <summary>
        /// The track bar 1_ scroll.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            trackint1 = trackBar1.Value;
            EditLightmaps();
        }

        /// <summary>
        /// The track bar 2_ scroll.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            trackint2 = trackBar2.Value;
            EditLightmaps();
        }

        /// <summary>
        /// The track bar 3_ scroll.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            trackint3 = trackBar3.Value;
            EditLightmaps();
        }

        // Makes right clicks select node under cursofr as well
        /// <summary>
        /// The tree view 1_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void treeView1_Click(object sender, EventArgs e)
        {
            MouseEventArgs me = e as MouseEventArgs;
            if (me.Button == MouseButtons.Right)
            {
                TreeNode c = treeView1.GetNodeAt(me.Location);
                treeView1.SelectedNode = c;
            }
        }

        /// <summary>
        /// The tree view 1_ double click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void treeView1_DoubleClick(object sender, EventArgs e)
        {
            int tempint = (int)((TreeView)sender).SelectedNode.Tag;
            if (tempint >= 0)
            {
                setCameraPosition(
                    bsp.Spawns.Spawn[tempint].X, bsp.Spawns.Spawn[tempint].Y, bsp.Spawns.Spawn[tempint].Z, false);

                

                checkedListBox1.SetItemCheckState(
                    checkedListBox1.FindString(bsp.Spawns.Spawn[tempint].Type.ToString(), 0), CheckState.Checked);

                

                #region AutoSelect

                if (checkBox4.Checked)
                {
                    #region ClearStatus

                    statusStrip.Items.Clear();

                    // SelectedSpawn.Clear();
                    #endregion

                    #region DisplayCameraPosition

                    // Set camera postion
                    string tempstring = toolStripLabel2.Text;
                    string tempstring2 = "Camera Position: X: " + cam.x.ToString().PadRight(10) + " • Y: " +
                                         cam.y.ToString().PadRight(10) + " • Z: " + cam.z.ToString().PadRight(10);
                    if (tempstring != tempstring2)
                    {
                        toolStripLabel2.Text = tempstring2;
                        statusStrip.ResumeLayout();
                        statusStrip.SuspendLayout();
                    }

                    #endregion

                    #region Turn Spawn On And Move To End Of List

                    int tempi = SelectedSpawn.IndexOf(tempint);

                    if (tempi != -1)
                    {
                        SelectedSpawn.RemoveAt(tempi);
                    }

                    SelectedSpawn.Add(tempint);

                    selectedSpawnType = bsp.Spawns.Spawn[tempint].Type;

                    #endregion

                    updateStatusPosition();
                }

                #endregion

                Application.DoEvents();
                this.Focus();
                Application.DoEvents();
            }
        }

        /// <summary>
        /// The ts button_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void tsButton_Click(object sender, EventArgs e)
        {
            ToolStripButton tb = (ToolStripButton)sender;

            Form listForm = new Form();

            listForm.FormBorderStyle = FormBorderStyle.Sizable;

            // listForm.Size = new Size(620, 450);
            listForm.StartPosition = FormStartPosition.Manual;
            int tempHeight = SelectedSpawn.Count * 18 + 90;
            if (tempHeight < (this.Height * 90 / 100))
            {
                listForm.Size = new Size(this.Width * 65 / 100, tempHeight);
            }
            else
            {
                listForm.Size = new Size(this.Width * 65 / 100, this.Height * 90 / 100);
            }

            listForm.Location = new Point(10, this.Height - (listForm.Size.Height + 35));
            listForm.Text = "Currently Selected Spawns";
            listForm.Resize += this.listForm_resize;

            #region DataGrid Data

            DataGridView dataGrid = new DataGridView();
            dataGrid.AllowUserToResizeRows = false;
            dataGrid.ColumnCount = 6;
            dataGrid.ReadOnly = true;
            dataGrid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dataGrid.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            dataGrid.GridColor = Color.Black;
            dataGrid.RowHeadersVisible = false;
            dataGrid.Columns[0].Name = "Type";
            dataGrid.Columns[1].Name = "SpawnNumber"; // Invisible
            dataGrid.Columns[2].Name = "Tag";
            dataGrid.Columns[3].Name = "X";
            dataGrid.Columns[4].Name = "Y";
            dataGrid.Columns[5].Name = "Z";
            dataGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            dataGrid.Location = new Point(10, 10);
            dataGrid.Name = "dataGrid";
            dataGrid.ShowCellToolTips = false;
            dataGrid.Size = new Size(listForm.Width - 20, listForm.Height - 60);
            dataGrid.TabIndex = 0;
            dataGrid.CellClick += dataGrid_CellClick;
            dataGrid.RowEnter += this.dataGridRow_Select;
            dataGrid.RowLeave += this.dataGridRow_LostFocus;
            dataGrid.Resize += this.dataGrid_resize;

            #region Add a button column.

            DataGridViewButtonColumn buttonColumn = new DataGridViewButtonColumn();
            buttonColumn.HeaderText = "Remove";
            buttonColumn.Name = "Remove";
            buttonColumn.Text = "x";
            buttonColumn.UseColumnTextForButtonValue = true;

            // buttonColumn.
            dataGrid.Columns.Add(buttonColumn);

            #endregion

            dataGrid.Columns[1].Visible = false;

            #endregion

            #region Selected Spawn Data

            // dataGrid.Items.Count = SelectedSpawn.Count;
            for (int i = 0; i < SelectedSpawn.Count; i++)
            {
                SpawnInfo.BaseSpawn thisSpawn = bsp.Spawns.Spawn[SelectedSpawn[i]];
                string extraInfo = thisSpawn.TagPath;
                if (thisSpawn is SpawnInfo.DeathZone)
                {
                    extraInfo = ((SpawnInfo.DeathZone)thisSpawn).Name;
                }
                else if (thisSpawn is SpawnInfo.ObjectiveSpawn)
                {
                    SpawnInfo.ObjectiveSpawn tempspawn = (SpawnInfo.ObjectiveSpawn)thisSpawn;
                    extraInfo = tempspawn.ObjectiveType + " (" + tempspawn.Team + ") #" + tempspawn.number;
                }
                else if (thisSpawn is SpawnInfo.PlayerSpawn)
                {
                    extraInfo = thisSpawn.Type + " Spawn";
                }

                dataGrid.Rows.Add(
                    new object[]
                        {
                            thisSpawn.Type.ToString(), SelectedSpawn[i].ToString(), extraInfo, thisSpawn.X.ToString(), 
                            thisSpawn.Y.ToString(), thisSpawn.Z.ToString()
                        });
                dataGrid.Rows[i].Height = 18;
            }

            #endregion

            #region Add Controls

            listForm.Controls.Add(dataGrid);
            listForm_resize(listForm, e);
            listForm.ShowDialog();

            #endregion
        }

        /// <summary>
        /// The ts text box_ change.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void tsTextBox_Change(object sender, EventArgs e)
        {
            ToolStripTextBox tb = (ToolStripTextBox)sender;
            if (tb.Focused == false)
            {
                return;
            }

            // In case all digits are deleted, must contain at least one digit!!!
            int lastSelectedSpawn = SelectedSpawn[SelectedSpawn.Count - 1];

            // Removes any letters and makes blanks zero
            makeDigitsOnly(ref tb);

            // get the difference in location between the old & new last spawn position
            float diffX = float.Parse(tsTextBoxX.Text) - bsp.Spawns.Spawn[lastSelectedSpawn].X;
            float diffY = float.Parse(tsTextBoxY.Text) - bsp.Spawns.Spawn[lastSelectedSpawn].Y;
            float diffZ = float.Parse(tsTextBoxZ.Text) - bsp.Spawns.Spawn[lastSelectedSpawn].Z;
            float diffYaw = 0;
            float diffPitch = 0;
            float diffRoll = 0;
            if (bsp.Spawns.Spawn[lastSelectedSpawn] is SpawnInfo.RotateYawPitchRollBaseSpawn)
            {
                SpawnInfo.RotateYawPitchRollBaseSpawn YPRSpawn =
                    bsp.Spawns.Spawn[lastSelectedSpawn] as SpawnInfo.RotateYawPitchRollBaseSpawn;
                diffYaw = float.Parse(tsTextBoxYaw.Text) - YPRSpawn.Yaw;
                diffPitch = float.Parse(tsTextBoxPitch.Text) - YPRSpawn.Pitch;
                diffRoll = float.Parse(tsTextBoxRoll.Text) - YPRSpawn.Roll;

                // Update the last selected spawn with the new location
                YPRSpawn.Yaw = float.Parse(tsTextBoxYaw.Text);
                YPRSpawn.Pitch = float.Parse(tsTextBoxPitch.Text);
                YPRSpawn.Roll = float.Parse(tsTextBoxRoll.Text);
            }
            else if (bsp.Spawns.Spawn[lastSelectedSpawn] is SpawnInfo.RotateDirectionBaseSpawn)
            {
                SpawnInfo.RotateDirectionBaseSpawn YPRSpawn =
                    bsp.Spawns.Spawn[lastSelectedSpawn] as SpawnInfo.RotateDirectionBaseSpawn;
                diffYaw = float.Parse(tsTextBoxYaw.Text) - YPRSpawn.RotationDirection;

                // Update the last selected spawn with the new location
                YPRSpawn.RotationDirection = float.Parse(tsTextBoxYaw.Text);
            }

            // Update the last selected spawn with the new location
            bsp.Spawns.Spawn[lastSelectedSpawn].X = float.Parse(tsTextBoxX.Text);
            bsp.Spawns.Spawn[lastSelectedSpawn].Y = float.Parse(tsTextBoxY.Text);
            bsp.Spawns.Spawn[lastSelectedSpawn].Z = float.Parse(tsTextBoxZ.Text);

            TranslationMatrix[lastSelectedSpawn] = MakeMatrixForSpawn(lastSelectedSpawn);

            // Move all other selected spawn the same distance as the last spawn moved
            for (int i = 0; i < SelectedSpawn.Count - 1; i++)
            {
                bsp.Spawns.Spawn[SelectedSpawn[i]].X += diffX;
                bsp.Spawns.Spawn[SelectedSpawn[i]].Y += diffY;
                bsp.Spawns.Spawn[SelectedSpawn[i]].Z += diffZ;

                if (bsp.Spawns.Spawn[SelectedSpawn[i]] is SpawnInfo.RotateYawPitchRollBaseSpawn)
                {
                    SpawnInfo.RotateYawPitchRollBaseSpawn YPRSpawn =
                        bsp.Spawns.Spawn[SelectedSpawn[i]] as SpawnInfo.RotateYawPitchRollBaseSpawn;

                    // Update all selected spawns with the new location
                    YPRSpawn.Yaw = float.Parse(tsTextBoxYaw.Text);
                    YPRSpawn.Pitch = float.Parse(tsTextBoxPitch.Text);
                    YPRSpawn.Roll = float.Parse(tsTextBoxRoll.Text);
                }
                else if (bsp.Spawns.Spawn[lastSelectedSpawn] is SpawnInfo.RotateDirectionBaseSpawn)
                {
                    SpawnInfo.RotateDirectionBaseSpawn YPRSpawn =
                        bsp.Spawns.Spawn[SelectedSpawn[i]] as SpawnInfo.RotateDirectionBaseSpawn;

                    // Update all selected spawns with the new location
                    YPRSpawn.RotationDirection = float.Parse(tsTextBoxYaw.Text);
                }

                TranslationMatrix[SelectedSpawn[i]] = MakeMatrixForSpawn(SelectedSpawn[i]);
            }

            updateXYZYPR = false;
            Point pt = tb.GetPositionFromCharIndex(tb.Text.Length - 1);
            if (pt.X < 45)
            {
                pt.X = 45;
            }

            tb.Width = pt.X + 15;
        }

        /// <summary>
        /// The ts text box_ got focus.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void tsTextBox_GotFocus(object sender, EventArgs e)
        {
            ToolStripTextBox tb = (ToolStripTextBox)sender;
            tb.Width = 80;
            tb.SelectionStart = 0;
            tb.SelectionLength = 0;
            Point pt = tb.GetPositionFromCharIndex(tb.Text.Length - 1);
            if (pt.X < 45)
            {
                pt.X = 45;
            }

            tb.Width = pt.X + 15;
        }

        /// <summary>
        /// The ts text box_ lost focus.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void tsTextBox_LostFocus(object sender, EventArgs e)
        {
            ToolStripTextBox tb = (ToolStripTextBox)sender;
            updateXYZYPR = true;
            tb.Width = 45;
            tb.SelectionStart = 0;
            tb.SelectionLength = 0;

            /*
            int lastSelectedSpawn = SelectedSpawn[SelectedSpawn.Count - 1];

            // get the difference in location between the old & new last spawn position
            float diffX = float.Parse(tsTextBoxX.Text) - bsp.Spawns.Spawn[lastSelectedSpawn].X;
            float diffY = float.Parse(tsTextBoxY.Text) - bsp.Spawns.Spawn[lastSelectedSpawn].Y;
            float diffZ = float.Parse(tsTextBoxZ.Text) - bsp.Spawns.Spawn[lastSelectedSpawn].Z;

            // Update the last selected spawn with the new location
            bsp.Spawns.Spawn[lastSelectedSpawn].X = float.Parse(tsTextBoxX.Text);
            bsp.Spawns.Spawn[lastSelectedSpawn].Y = float.Parse(tsTextBoxY.Text);
            bsp.Spawns.Spawn[lastSelectedSpawn].Z = float.Parse(tsTextBoxZ.Text);

            // Move all other selected spawn the same distance as the last spawn moved
            for (int i = 0; i < SelectedSpawn.Count-1; i++)
            {
                bsp.Spawns.Spawn[SelectedSpawn[i]].X += diffX;
                bsp.Spawns.Spawn[SelectedSpawn[i]].Y += diffY;
                bsp.Spawns.Spawn[SelectedSpawn[i]].Z += diffZ;
            }
            */
        }

        /// <summary>
        /// The update status position.
        /// </summary>
        /// <remarks></remarks>
        private void updateStatusPosition()
        {
            if (SelectedSpawn.Count > 0)
            {
                toolStrip.Visible = true;
                bool showCollection = true, 
                     showObjective = true, 
                     showObstacles = true, 
                     showScenery = true, 
                     showSounds = true;

                // bool isOther = false;
                for (int count = 0; count < SelectedSpawn.Count; count++)
                {
                    if (bsp.Spawns.Spawn[SelectedSpawn[count]] is SpawnInfo.Collection)
                    {
                        showObjective = false;
                        showObstacles = false;
                        showScenery = false;
                        showSounds = false;
                    }
                    else if (bsp.Spawns.Spawn[SelectedSpawn[count]] is SpawnInfo.ObjectiveSpawn)
                    {
                        showCollection = false;
                        showObstacles = false;
                        showScenery = false;
                        showSounds = false;
                    }
                    else if (bsp.Spawns.Spawn[SelectedSpawn[count]] is SpawnInfo.ScenerySpawn)
                    {
                        showCollection = false;
                        showObjective = false;
                        showObstacles = false;
                        showSounds = false;
                    }
                    else if (bsp.Spawns.Spawn[SelectedSpawn[count]] is SpawnInfo.ObstacleSpawn)
                    {
                        showCollection = false;
                        showObjective = false;
                        showScenery = false;
                        showSounds = false;
                    }
                    else if (bsp.Spawns.Spawn[SelectedSpawn[count]] is SpawnInfo.SoundSpawn)
                    {
                        showCollection = false;
                        showObjective = false;
                        showObstacles = false;
                        showScenery = false;
                    }
                    else
                    {
                        showCollection = false;
                        showObjective = false;
                        showObstacles = false;
                        showScenery = false;
                        showSounds = false;
                    }
                }

                int lastSelectedSpawn = SelectedSpawn[SelectedSpawn.Count - 1];
                string itemType = bsp.Spawns.Spawn[lastSelectedSpawn].Type.ToString();
                for (int i = 0; i < SelectedSpawn.Count - 1; i++)
                {
                    string temp = bsp.Spawns.Spawn[SelectedSpawn[i]].Type.ToString();
                    if (itemType != temp)
                    {
                        itemType = "mixed";
                        statusStrip.Items.Clear();
                    }
                }

                // tsLabel1.Text = "Type: <";
                tsButtonType.Text = itemType;

                // tsLabel2.Text = "> (";
                tsLabelCount.Text = SelectedSpawn.Count.ToString();

                // Stop it from overwriting text in boxes when user is trying to change it!
                if (updateXYZYPR)
                {
                    tsLabelX.Text = ") • X: ";
                    tsTextBoxX.Text = bsp.Spawns.Spawn[lastSelectedSpawn].X.ToString("#0.0000####");
                    tsLabelY.Text = " • Y: ";
                    tsTextBoxY.Text = bsp.Spawns.Spawn[lastSelectedSpawn].Y.ToString("#0.0000####");
                    tsLabelZ.Text = " • Z: ";
                    tsTextBoxZ.Text = bsp.Spawns.Spawn[lastSelectedSpawn].Z.ToString("#0.0000####");
                }

                int rot = 0;

                #region YawPitchRollInfo

                if (bsp.Spawns.Spawn[lastSelectedSpawn] is SpawnInfo.RotateYawPitchRollBaseSpawn)
                {
                    rot = 2;

                    // Stop it from overwriting text in boxes when user is trying to change it!
                    if (updateXYZYPR)
                    {
                        SpawnInfo.RotateYawPitchRollBaseSpawn temprot;
                        temprot = bsp.Spawns.Spawn[lastSelectedSpawn] as SpawnInfo.RotateYawPitchRollBaseSpawn;
                        tsLabelYaw.Text = " • Yaw:";
                        tsTextBoxYaw.Text = temprot.Yaw.ToString("#0.0000####");
                        tsLabelPitch.Text = " • Pitch:";
                        tsTextBoxPitch.Text = temprot.Pitch.ToString("#0.0000####");
                        tsLabelRoll.Text = " • Roll:";
                        tsTextBoxRoll.Text = temprot.Roll.ToString("#0.0000####");
                    }
                }
                else if (bsp.Spawns.Spawn[lastSelectedSpawn] is SpawnInfo.RotateDirectionBaseSpawn)
                {
                    if (updateXYZYPR)
                    {
                        SpawnInfo.RotateDirectionBaseSpawn temprot;
                        temprot = bsp.Spawns.Spawn[lastSelectedSpawn] as SpawnInfo.RotateDirectionBaseSpawn;

                        // If multiple selections are made and at least one contains YawPitchRoll, show it
                        SpawnInfo.RotateYawPitchRollBaseSpawn temprot2;
                        for (int x = SelectedSpawn.Count - 1; x >= 0; x--)
                        {
                            if (bsp.Spawns.Spawn[SelectedSpawn[x]] is SpawnInfo.RotateYawPitchRollBaseSpawn)
                            {
                                rot = 2;
                                temprot2 = bsp.Spawns.Spawn[SelectedSpawn[x]] as SpawnInfo.RotateYawPitchRollBaseSpawn;
                                tsLabelYaw.Text = " • Yaw:";
                                tsTextBoxYaw.Text = temprot.RotationDirection.ToString("#0.0000####");
                                tsLabelPitch.Text = " • Pitch:";
                                tsTextBoxPitch.Text = temprot2.Pitch.ToString("#0.0000####");
                                tsLabelRoll.Text = " • Roll:";
                                tsTextBoxRoll.Text = temprot2.Roll.ToString("#0.0000####");
                                break;
                            }
                        }

                        // ...otherwise just show rotation
                        rot = 1;
                        tsLabelYaw.Text = " • Yaw:";
                        tsTextBoxYaw.Text = temprot.RotationDirection.ToString("#0.0000####");
                        tsLabelPitch.Text = string.Empty;
                        tsLabelRoll.Text = string.Empty;
                    }
                }

                #endregion

                // statusStrip.Items.Clear();
                #region StatusStripIndividualDisplays

                #region CollectionObjectsOnly

                if (showCollection)
                {
                    // Selects Last Spawn Clicked
                    SpawnInfo.Collection os;
                    os = bsp.Spawns.Spawn[lastSelectedSpawn] as SpawnInfo.Collection;

                    ToolStripComboBox combo;
                    Array testx;
                    int tempindex;

                    #region CollectionSpawnComboBox

                    if (statusStrip.Items.IndexOfKey("CollectionSpawn") == -1)
                    {
                        combo = new ToolStripComboBox();
                        testx = Enum.GetNames(typeof(SpawnInfo.Collection.SpawnsInEnum));

                        // Add all the Collection options
                        for (int y = 0; y < testx.Length; y++)
                        {
                            combo.Items.Add(testx.GetValue(y).ToString());
                        }

                        combo.SelectedIndexChanged += combo_SelectedIndexChangedCollection;
                        combo.BackColor = Color.Gray;
                        combo.FlatStyle = FlatStyle.Flat;
                        combo.Name = "CollectionSpawn";
                        combo.DropDownStyle = ComboBoxStyle.DropDownList;
                        statusStrip.Items.Add(combo);
                    }

                    #endregion

                    #region CollectionModelComboBox

                    if (statusStrip.Items.IndexOfKey("CollectionModel") == -1)
                    {
                        combo = new ToolStripComboBox();

                        for (int y = 0; y < WeaponsList.Count; y++)
                        {
                            combo.Items.Add(WeaponsList[y].Name);
                        }

                        combo.SelectedIndexChanged += combo_SelectedIndexChangedCollectionModel;
                        combo.BackColor = Color.Gray;
                        combo.DropDownStyle = ComboBoxStyle.DropDownList;
                        combo.FlatStyle = FlatStyle.Flat;
                        combo.Name = "CollectionModel";
                        combo.Sorted = true;
                        statusStrip.Items.Add(combo);
                    }

                    #endregion

                    if (os != null)
                    {
                        // Update Spawn Combo box
                        int collNumber = statusStrip.Items.IndexOfKey("CollectionSpawn");
                        ToolStripComboBox cbCollSpawn = (ToolStripComboBox)statusStrip.Items[collNumber];
                        tempindex = cbCollSpawn.Items.IndexOf(os.SpawnsInMode.ToString());

                        cbCollSpawn.SelectedIndex = tempindex;

                        // If not all selected Collections are the same Spawn Type, blank the box out
                        for (int x = 0; x < SelectedSpawn.Count; x++)
                        {
                            if (((SpawnInfo.Collection)bsp.Spawns.Spawn[SelectedSpawn[x]]).SpawnsInMode !=
                                os.SpawnsInMode)
                            {
                                cbCollSpawn.SelectedIndex = -1;
                            }
                        }

                        // Update Collection Model Type Box
                        string[] tempOS = os.TagPath.Split('\\');
                        int collModel = statusStrip.Items.IndexOfKey("CollectionModel");
                        ToolStripComboBox cbCollModel = (ToolStripComboBox)statusStrip.Items[collModel];
                        tempindex = cbCollModel.Items.IndexOf(tempOS[tempOS.Length - 1].Replace('_', ' '));

                        cbCollModel.SelectedIndex = tempindex;

                        // If not all selected Collections are the same Models, blank the box out
                        for (int x = 0; x < SelectedSpawn.Count; x++)
                        {
                            if (bsp.Spawns.Spawn[SelectedSpawn[x]].TagPath != os.TagPath)
                            {
                                cbCollModel.SelectedIndex = -1;
                            }
                        }
                    }
                }

                #endregion Collection

                #region ObjectiveObjectsOnly

                if (showObjective)
                {
                    // Selects Last Spawn Clicked
                    SpawnInfo.ObjectiveSpawn os;
                    os = bsp.Spawns.Spawn[lastSelectedSpawn] as SpawnInfo.ObjectiveSpawn;

                    ToolStripComboBox comboSpawn;
                    ToolStripComboBox comboTeam;
                    Array testx;
                    int tempindex;

                    #region ObjectiveTypeComboBox

                    if (statusStrip.Items.IndexOfKey("ObjectiveSpawn") == -1)
                    {
                        // Add Objective Types to first combo box
                        comboSpawn = new ToolStripComboBox();
                        testx = Enum.GetNames(typeof(SpawnInfo.ObjectiveSpawn.ObjectiveTypeEnum));

                        for (int y = 0; y < testx.Length; y++)
                        {
                            comboSpawn.Items.Add(testx.GetValue(y).ToString());
                        }

                        comboSpawn.SelectedIndexChanged += combo_SelectedIndexChangedObjective;

                        comboSpawn.BackColor = Color.Gray;
                        comboSpawn.FlatStyle = FlatStyle.Flat;
                        comboSpawn.DropDownStyle = ComboBoxStyle.DropDownList;
                        comboSpawn.Name = "ObjectiveSpawn";
                        statusStrip.Items.Add(comboSpawn);
                    }

                    #endregion

                    #region ObjectiveTeamComboBox

                    if (statusStrip.Items.IndexOfKey("ObjectiveTeam") == -1)
                    {
                        // Add Objective Teams to second combo box
                        comboTeam = new ToolStripComboBox();
                        testx = Enum.GetNames(typeof(SpawnInfo.ObjectiveSpawn.TeamType));

                        for (int y = 0; y < testx.Length; y++)
                        {
                            comboTeam.Items.Add(testx.GetValue(y).ToString());
                        }

                        comboTeam.SelectedIndexChanged += combo_SelectedIndexChangedTeam;
                        comboTeam.BackColor = Color.Gray;
                        comboTeam.FlatStyle = FlatStyle.Flat;
                        comboTeam.DropDownStyle = ComboBoxStyle.DropDownList;
                        comboTeam.Name = "ObjectiveTeam";
                        statusStrip.Items.Add(comboTeam);
                    }

                    #endregion

                    if (os != null)
                    {
                        // Update Spawn Combo box
                        int objNumber = statusStrip.Items.IndexOfKey("ObjectiveSpawn");
                        ToolStripComboBox cbObjSpawn = (ToolStripComboBox)statusStrip.Items[objNumber];
                        tempindex = cbObjSpawn.Items.IndexOf(os.ObjectiveType.ToString());
                        if (tempindex != -1)
                        {
                            cbObjSpawn.SelectedIndex = tempindex;

                            // If not all selected Objectives are the same type, blank the box out
                            for (int x = 0; x < SelectedSpawn.Count; x++)
                            {
                                if (((SpawnInfo.ObjectiveSpawn)bsp.Spawns.Spawn[SelectedSpawn[x]]).ObjectiveType !=
                                    os.ObjectiveType)
                                {
                                    cbObjSpawn.SelectedIndex = -1;
                                }
                            }
                        }

                        // Update Team Combo box
                        objNumber = statusStrip.Items.IndexOfKey("ObjectiveTeam");
                        ToolStripComboBox cbObjTeam = (ToolStripComboBox)statusStrip.Items[objNumber];
                        tempindex = cbObjTeam.Items.IndexOf(os.Team.ToString());
                        if (tempindex != -1)
                        {
                            cbObjTeam.SelectedIndex = tempindex;

                            // If not all selected Objectives are the same Team, blank the box out
                            for (int x = 0; x < SelectedSpawn.Count; x++)
                            {
                                if (((SpawnInfo.ObjectiveSpawn)bsp.Spawns.Spawn[SelectedSpawn[x]]).Team != os.Team)
                                {
                                    cbObjTeam.SelectedIndex = -1;
                                }
                            }
                        }
                    }

                    #region ObjectiveIndexTextBox

                    if (statusStrip.Items.IndexOfKey("ObjectiveText") == -1)
                    {
                        ToolStripTextBox tb = new ToolStripTextBox();
                        tb.AutoSize = false;
                        tb.BackColor = Color.White;
                        tb.BorderStyle = BorderStyle.FixedSingle;
                        tb.Height = 20;
                        tb.Width = 30;
                        if (os != null)
                        {
                            tb.Text = os.number.ToString();
                        }
                        else
                        {
                            tb.Text = string.Empty;
                        }

                        tb.Name = "ObjectiveText";
                        tb.TextChanged += tb_TextChanged;
                        statusStrip.Items.Add(tb);
                    }

                    #endregion
                }

                #endregion Objective

                #region ObstacleObjectsOnly

                if (showObstacles)
                {
                    ToolStripComboBox comboBlock;
                    int tempindex;

                    #region CollectionModelComboBox

                    if (statusStrip.Items.IndexOfKey("ObstacleModel") == -1)
                    {
                        comboBlock = new ToolStripComboBox();

                        for (int y = 0; y < ObstacleList.Count; y++)
                        {
                            comboBlock.Items.Add(ObstacleList[y].Name);
                        }

                        comboBlock.SelectedIndexChanged += combo_SelectedIndexChangedObstacleModel;
                        comboBlock.BackColor = Color.Gray;
                        comboBlock.DropDownStyle = ComboBoxStyle.DropDownList;
                        comboBlock.FlatStyle = FlatStyle.Flat;
                        comboBlock.Name = "ObstacleModel";
                        comboBlock.Size = new Size(200, statusStrip.Height - 10);
                        comboBlock.Sorted = true;
                        statusStrip.Items.Add(comboBlock);
                    }

                    #endregion

                    if (bsp.Spawns.Spawn[lastSelectedSpawn] != null)
                    {
                        string[] tempOS;
                        int scenModel = statusStrip.Items.IndexOfKey("ObstacleModel");
                        ToolStripComboBox cbBlocModel = (ToolStripComboBox)statusStrip.Items[scenModel];

                        // Update Obstacle Model Type Box
                        tempOS = bsp.Spawns.Spawn[lastSelectedSpawn].TagPath.Split('\\');
                        tempindex = cbBlocModel.Items.IndexOf(tempOS[tempOS.Length - 1]);
                        cbBlocModel.SelectedIndex = tempindex;

                        // If not all selected Sceneries are the same Model, blank the box out
                        for (int x = 0; x < SelectedSpawn.Count; x++)
                        {
                            if (bsp.Spawns.Spawn[SelectedSpawn[x]].TagPath !=
                                bsp.Spawns.Spawn[lastSelectedSpawn].TagPath)
                            {
                                cbBlocModel.SelectedIndex = -1;
                            }
                        }
                    }
                }

                #endregion

                #region SceneryObjectsOnly

                if (showScenery)
                {
                    ToolStripComboBox comboScen;
                    int tempindex;

                    #region CollectionModelComboBox

                    if (statusStrip.Items.IndexOfKey("SceneryModel") == -1)
                    {
                        comboScen = new ToolStripComboBox();

                        for (int y = 0; y < SceneryList.Count; y++)
                        {
                            comboScen.Items.Add(SceneryList[y].Name);
                        }

                        comboScen.SelectedIndexChanged += combo_SelectedIndexChangedSceneryModel;
                        comboScen.BackColor = Color.Gray;
                        comboScen.DropDownStyle = ComboBoxStyle.DropDownList;
                        comboScen.FlatStyle = FlatStyle.Flat;
                        comboScen.Name = "SceneryModel";
                        comboScen.Size = new Size(200, statusStrip.Height - 10);
                        comboScen.Sorted = true;
                        statusStrip.Items.Add(comboScen);
                    }

                    #endregion

                    if (bsp.Spawns.Spawn[lastSelectedSpawn] != null)
                    {
                        string[] tempOS;
                        int scenModel = statusStrip.Items.IndexOfKey("SceneryModel");
                        ToolStripComboBox cbScenModel = (ToolStripComboBox)statusStrip.Items[scenModel];

                        // Update Scenery Model Type Box
                        tempOS = bsp.Spawns.Spawn[lastSelectedSpawn].TagPath.Split('\\');
                        tempindex = cbScenModel.Items.IndexOf(tempOS[tempOS.Length - 1]);
                        cbScenModel.SelectedIndex = tempindex;

                        // If not all selected Sceneries are the same Model, blank the box out
                        for (int x = 0; x < SelectedSpawn.Count; x++)
                        {
                            if (bsp.Spawns.Spawn[SelectedSpawn[x]].TagPath !=
                                bsp.Spawns.Spawn[lastSelectedSpawn].TagPath)
                            {
                                cbScenModel.SelectedIndex = -1;
                            }
                        }
                    }
                }

                #endregion

                #region SoundObjectsOnly

                if (showSounds)
                {
                    SpawnInfo.SoundSpawn os;
                    os = bsp.Spawns.Spawn[lastSelectedSpawn] as SpawnInfo.SoundSpawn;

                    ToolStripComboBox comboSound;
                    ToolStripTextBox textSound;
                    int tempindex;

                    #region SoundsComboBox

                    if (statusStrip.Items.IndexOfKey("SoundScenery") == -1)
                    {
                        comboSound = new ToolStripComboBox();

                        for (int y = 0; y < SoundsList.Count; y++)
                        {
                            comboSound.Items.Add(SoundsList[y].Name);
                        }

                        comboSound.SelectedIndexChanged += combo_SelectedIndexChangedSceneryModel;
                        comboSound.BackColor = Color.Gray;
                        comboSound.DropDownStyle = ComboBoxStyle.DropDownList;
                        comboSound.FlatStyle = FlatStyle.Flat;
                        comboSound.Name = "SoundScenery";
                        comboSound.Size = new Size(200, statusStrip.Height - 10);
                        comboSound.Sorted = true;
                        statusStrip.Items.Add(comboSound);
                    }

                    #endregion

                    #region SoundsInnerSizeBox

                    if (statusStrip.Items.IndexOfKey("SoundSceneryInnerSize") == -1)
                    {
                        textSound = new ToolStripTextBox();

                        textSound.Text = os.DistanceBoundsLower.ToString();
                        textSound.TextBox.TextAlign = HorizontalAlignment.Center;
                        textSound.BackColor = Color.Gray;
                        textSound.Name = "SoundSceneryInnerSize";
                        textSound.Size = new Size(40, statusStrip.Height - 10);

                        textSound.Click += textSound_Click;
                        textSound.TextChanged += textSound_TextChanged;
                        statusStrip.Items.Add(textSound);
                    }

                    #endregion

                    #region SoundsOuterSizeBox

                    if (statusStrip.Items.IndexOfKey("SoundSceneryOuterSize") == -1)
                    {
                        textSound = new ToolStripTextBox();

                        textSound.Text = os.DistanceBoundsUpper.ToString();
                        textSound.TextBox.TextAlign = HorizontalAlignment.Center;
                        textSound.BackColor = Color.Gray;
                        textSound.Name = "SoundSceneryOuterSize";
                        textSound.Size = new Size(40, statusStrip.Height - 10);

                        textSound.Click += textSound_Click;
                        textSound.GotFocus += textSound_GotFocus;
                        textSound.LostFocus += textSound_LostFocus;
                        textSound.MouseHover += textSound_GotFocus;
                        textSound.MouseLeave += textSound_LostMouseFocus;
                        textSound.TextChanged += textSound_TextChanged;
                        statusStrip.Items.Add(textSound);
                    }

                    #endregion

                    if (bsp.Spawns.Spawn[lastSelectedSpawn] != null)
                    {
                        string[] tempOS;
                        int soundScen = statusStrip.Items.IndexOfKey("SoundScenery");
                        ToolStripComboBox cbSoundScen = (ToolStripComboBox)statusStrip.Items[soundScen];
                        int soundInner = statusStrip.Items.IndexOfKey("SoundSceneryInnerSize");
                        ToolStripTextBox cbSoundInner = (ToolStripTextBox)statusStrip.Items[soundInner];
                        int soundOuter = statusStrip.Items.IndexOfKey("SoundSceneryOuterSize");
                        ToolStripTextBox cbSoundOuter = (ToolStripTextBox)statusStrip.Items[soundOuter];

                        // Update Scenery Model Type Box
                        tempOS = bsp.Spawns.Spawn[lastSelectedSpawn].TagPath.Split('\\');
                        tempindex = cbSoundScen.Items.IndexOf(tempOS[tempOS.Length - 1]);
                        cbSoundScen.SelectedIndex = tempindex;

                        // If not all selected Sceneries are the same sound, blank the box out
                        // Same for sizes
                        for (int x = 0; x < SelectedSpawn.Count; x++)
                        {
                            if (bsp.Spawns.Spawn[SelectedSpawn[x]].TagPath != os.TagPath)
                            {
                                cbSoundScen.SelectedIndex = -1;
                            }

                            if (((SpawnInfo.SoundSpawn)bsp.Spawns.Spawn[SelectedSpawn[x]]).DistanceBoundsLower !=
                                os.DistanceBoundsLower)
                            {
                                cbSoundInner.Text = string.Empty;
                            }

                            if (((SpawnInfo.SoundSpawn)bsp.Spawns.Spawn[SelectedSpawn[x]]).DistanceBoundsUpper !=
                                os.DistanceBoundsUpper)
                            {
                                cbSoundOuter.Text = string.Empty;
                            }
                        }
                    }
                }

                #endregion

                #endregion

                // Add the position and rotation info to the start of the status bar
                if (statusStrip.Items.IndexOf(tsLabel1) == -1)
                {
                    statusStrip.Items.Insert(0, tsLabel1);
                }

                if (statusStrip.Items.IndexOf(tsButtonType) == -1)
                {
                    statusStrip.Items.Insert(1, tsButtonType);
                }

                if (statusStrip.Items.IndexOf(tsLabel2) == -1)
                {
                    statusStrip.Items.Insert(2, tsLabel2);
                }

                if (statusStrip.Items.IndexOf(tsLabelCount) == -1)
                {
                    statusStrip.Items.Insert(3, tsLabelCount);
                }

                if (statusStrip.Items.IndexOf(tsLabelX) == -1)
                {
                    statusStrip.Items.Insert(4, tsLabelX);
                }

                if (statusStrip.Items.IndexOf(tsTextBoxX) == -1)
                {
                    statusStrip.Items.Insert(5, tsTextBoxX);
                }

                if (statusStrip.Items.IndexOf(tsLabelY) == -1)
                {
                    statusStrip.Items.Insert(6, tsLabelY);
                }

                if (statusStrip.Items.IndexOf(tsTextBoxY) == -1)
                {
                    statusStrip.Items.Insert(7, tsTextBoxY);
                }

                if (statusStrip.Items.IndexOf(tsLabelZ) == -1)
                {
                    statusStrip.Items.Insert(8, tsLabelZ);
                }

                if (statusStrip.Items.IndexOf(tsTextBoxZ) == -1)
                {
                    statusStrip.Items.Insert(9, tsTextBoxZ);
                }

                if (rot > 0)
                {
                    if (statusStrip.Items.IndexOf(tsLabelYaw) == -1)
                    {
                        statusStrip.Items.Insert(10, tsLabelYaw);
                    }

                    if (statusStrip.Items.IndexOf(tsTextBoxYaw) == -1)
                    {
                        statusStrip.Items.Insert(11, tsTextBoxYaw);
                    }
                }

                if (rot > 1)
                {
                    if (statusStrip.Items.IndexOf(tsLabelPitch) == -1)
                    {
                        statusStrip.Items.Insert(12, tsLabelPitch);
                    }

                    if (statusStrip.Items.IndexOf(tsTextBoxPitch) == -1)
                    {
                        statusStrip.Items.Insert(13, tsTextBoxPitch);
                    }

                    if (statusStrip.Items.IndexOf(tsLabelRoll) == -1)
                    {
                        statusStrip.Items.Insert(14, tsLabelRoll);
                    }

                    if (statusStrip.Items.IndexOf(tsTextBoxRoll) == -1)
                    {
                        statusStrip.Items.Insert(15, tsTextBoxRoll);
                    }
                }
            }
            else
            {
                statusStrip.Items.Clear();
                toolStrip.Visible = false;
            }

            // Add the camera position
            toolStripLabel2.Text = "Camera Position: X: " + cam.x.ToString().PadRight(10) + " • Y: " +
                                   cam.y.ToString().PadRight(10) + " • Z: " + cam.z.ToString().PadRight(10);
            statusStrip.Items.Add(toolStripLabel2);
            statusStrip.ResumeLayout();

            // Allow quick update of statusBar, then disable again
            statusStrip.SuspendLayout();
        }

        #endregion

        /// <summary>
        /// The ts combo box.
        /// </summary>
        /// <remarks></remarks>
        public class TSComboBox : ToolStripComboBox
        {
            #region Methods

            /// <summary>
            /// The on key down.
            /// </summary>
            /// <param name="e">The e.</param>
            /// <remarks></remarks>
            protected override void OnKeyDown(KeyEventArgs e)
            {
                e.Handled = true;
            }

            /// <summary>
            /// The on key press.
            /// </summary>
            /// <param name="e">The e.</param>
            /// <remarks></remarks>
            protected override void OnKeyPress(KeyPressEventArgs e)
            {
                e.Handled = true;
            }

            #endregion
        }

        /// <summary>
        /// The ts text box.
        /// </summary>
        /// <remarks></remarks>
        public class TSTextBox : ToolStripTextBox
        {
            #region Methods

            /// <summary>
            /// The on key down.
            /// </summary>
            /// <param name="e">The e.</param>
            /// <remarks></remarks>
            protected override void OnKeyDown(KeyEventArgs e)
            {
                e.Handled = true;
            }

            /// <summary>
            /// The on key press.
            /// </summary>
            /// <param name="e">The e.</param>
            /// <remarks></remarks>
            protected override void OnKeyPress(KeyPressEventArgs e)
            {
                e.Handled = true;
            }

            #endregion
        }
    }
}