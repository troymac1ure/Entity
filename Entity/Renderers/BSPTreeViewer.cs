// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BSPTreeViewer.cs" company="">
//   
// </copyright>
// <summary>
//   The bsp collision viewer.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace entity.Renderers
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Windows.Forms;

    using HaloMap.H2MetaContainers;
    using HaloMap.Map;
    using HaloMap.Meta;
    using HaloMap.RawData;
    using HaloMap.Render;

    using Microsoft.DirectX;
    using Microsoft.DirectX.Direct3D;
    using Microsoft.DirectX.DirectInput;

    using Device = Microsoft.DirectX.Direct3D.Device;
    using DeviceType = Microsoft.DirectX.Direct3D.DeviceType;

    /// <summary>
    /// The bsp collision viewer.
    /// </summary>
    /// <remarks></remarks>
    public partial class BSPCollisionViewer : Form
    {
        #region Constants and Fields

        /// <summary>
        /// The present params.
        /// </summary>
        private readonly PresentParameters presentParams = new PresentParameters();

        /// <summary>
        /// The black material.
        /// </summary>
        private Material BlackMaterial;

        // Materials
        /// <summary>
        /// The blue material.
        /// </summary>
        private Material BlueMaterial;

        /// <summary>
        /// The default material.
        /// </summary>
        private Material DefaultMaterial;

        /// <summary>
        /// The green material.
        /// </summary>
        private Material GreenMaterial;

        /// <summary>
        /// The world transform.
        /// </summary>
        private Matrix WorldTransform = Matrix.Identity;

        /// <summary>
        /// The cam.
        /// </summary>
        private Camera2 cam;

        /// <summary>
        /// The coll.
        /// </summary>
        private BSPModel.BSPCollision coll;

        // Our global variables for this project
        /// <summary>
        /// The device.
        /// </summary>
        private Device device; // Our rendering device

        /// <summary>
        /// The dxcoll.
        /// </summary>
        private DirectXBSPCollision dxcoll;

        /// <summary>
        /// The map.
        /// </summary>
        private Map map;

        /// <summary>
        /// The pause.
        /// </summary>
        private bool pause;

        /// <summary>
        /// Used for MeshPick();
        /// </summary>
        HaloMap.Render.Renderer render = new HaloMap.Render.Renderer();
        
        /// <summary>
        /// Used for point translation (point editing mode)
        /// </summary>
        Matrix[] TranslationMatrix;

        /// <summary>
        /// A sphere mesh used for marking selected points (point editing mode)
        /// </summary>
        Mesh sphere;

        /// <summary>
        /// A list of all the currently selected points (point editing mode)
        /// </summary>
        List<int> SelectedPoints = new List<int>();

        /// <summary>
        /// A list of all the polygons making up the collision mesh
        /// </summary>
        List<polygonInfo> polygons = new List<polygonInfo>();

        /// <summary>
        /// The currently selected surface (surface editing mode)
        /// </summary>
        int currentSurface = -1;

        /// <summary>
        /// 
        /// </summary>
        Meta bspMeta;

        /// <summary>
        /// All the labels & sheck boxes used for the Collision Material editor.
        /// </summary>
        private Panel panel1;
        private Label lblEditMode;
        private Label lblEditModeLabel;
        private Label lblXLabel;
        private Label lblYLabel;
        private Label lblZLabel;
        private Label lblZ;
        private Label lblY;
        private Label lblX;
        private GroupBox gbSurfaceFlags;
        private CheckBox cbConveyor;
        private CheckBox cbInvalid;
        private CheckBox cbBreakable;
        private CheckBox cbClimable;
        private CheckBox cbInvisible;
        private CheckBox cbTwoSided;
        private Label lblTabChange;

        /// <summary>
        ///  Tracks the current editing mode
        /// </summary>
        editingModes currentMode;

        /// <summary>
        /// The allowed editing modes enumerator
        /// </summary>
        private enum editingModes
        {
            None = 0,
            Surface = 1,
            Point = 2,

        }

        /// <summary>
        /// All the info required for each polygon on the screen
        /// </summary>
        public class polygonInfo
        {
            /// <summary>
            /// The indices of the polygon
            /// </summary>
            public short[] indices;

            /// <summary>
            /// The plane index of the polygon
            /// </summary>
            public int plane;
            
            /// <summary>
            /// Polygon flags
            ///   <option name="Two-Sided" bit="0" />
            ///   <option name="Invisible" bit="1" />
            ///   <option name="Climable" bit="2" />
            ///   <option name="Breakable" bit="3" />
            ///   <option name="Invaild" bit="4" />
            ///   <option name="Conveyor" bit="5" />
            /// </summary>
            public byte flags;
            
            /// <summary>
            /// Breakable Surface # ?? Maybe part of flags?
            /// </summary>
            public byte breakSurface;

            /// <summary>
            /// The material of the polygon
            /// </summary>
            public short material;

            public polygonInfo()
            {
            }
        }


        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BSPCollisionViewer"/> class.
        /// </summary>
        /// <param name="tempcoll">The tempcoll.</param>
        /// <param name="map">The map.</param>
        /// <remarks></remarks>
        public BSPCollisionViewer(BSPModel.BSPCollision tempcoll, Map map)
        {
            //InitializeComponent
            InitializeComponent();

            this.map = map;
            coll = tempcoll;

            // Set the initial size of our form
            this.ClientSize = new Size(800, 600);

            // And its caption
            this.Text = "BSP Viewer";
           
            // Load our points/edges & build polygons for surface detection / modifying
            LoadMeta();

            Main();
        }

        #endregion

        #region Public Methods

        Vector4 calculatePlane(Vector3[] trianglePoints)
        {
            // Get Shortened Names For The Vertices Of The Face
            Vector3 v1 = trianglePoints[0];
            Vector3 v2 = trianglePoints[1];
            Vector3 v3 = trianglePoints[2];
            
            Vector4 plane;
            plane.X = v1.Y * (v2.Z - v3.Z) + v2.Y * (v3.Z - v1.Z) + v3.Y * (v1.Z - v2.Z);
            plane.Y = v1.Z * (v2.X - v3.X) + v2.Z * (v3.X - v1.X) + v3.Z * (v1.X - v2.X);
            plane.Z = v1.X * (v2.Y - v3.Y) + v2.X * (v3.Y - v1.Y) + v3.X * (v1.Y - v2.Y);
            plane.W = -(v1.X * (v2.Y * v3.Z - v3.Y * v2.Z) +
                        v2.X * (v3.Y * v1.Z - v1.Y * v3.Z) +
                        v3.X * (v1.Y * v2.Z - v2.Y * v1.Z));
            return plane;
        }

        Plane DeriveTrianglePlanes(Vector3[] trianglePoints)
        {
            float[] d0 = new float[3], 
                    d1 = new float[3], 
                    n = new float[3]; 
            float s;
            Vector3 v0 = trianglePoints[0];
            Vector3 v1 = trianglePoints[1];
            Vector3 v2 = trianglePoints[2];

            d0[0] = v1.X - v0.X;
            d0[1] = v1.Y - v0.Y;
            d0[2] = v1.Z - v0.Z;
            d1[0] = v2.X - v0.X;
            d1[1] = v2.Y - v0.Y;
            d1[2] = v2.Z - v0.Z;
            n[0] = d1[1] * d0[2] - d1[2] * d0[1];
            n[1] = d1[2] * d0[0] - d1[0] * d0[2];
            n[2] = d1[0] * d0[1] - d1[1] * d0[0];
            s = (float)(1.0f / Math.Sqrt(n[0] * n[0] + n[1] * n[1] + n[2] * n[2]));

            Plane planes;
            planes.A = n[0] * s;
            planes.B = n[1] * s;
            planes.C = n[2] * s;
            planes.D = -(planes.A * v0.X + planes.B * v0.Y + planes.C * v0.Z);
            return planes;
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
            /*
            presentParams.Windowed = true; // We don't want to run fullscreen
            presentParams.PresentationInterval = PresentInterval.Default;
            presentParams.FullScreenRefreshRateInHz = PresentParameters.DefaultPresentRate;

            presentParams.SwapEffect = SwapEffect.Discard; // Discard the frames
            presentParams.EnableAutoDepthStencil = true; // Turn on a Depth stencil
            presentParams.AutoDepthStencilFormat = DepthFormat.D16; // And the stencil format
            device = new Device(0, DeviceType.Hardware, this, CreateFlags.SoftwareVertexProcessing, presentParams);
            */

            render.CreateDevice(this);
            device = render.device;

            // Create a device
            device.DeviceReset += this.OnResetDevice;
            this.OnResetDevice(device, null);
            pause = false;

            // Create a sphere to be used by all selected points (point editing mode)
            sphere = Mesh.Sphere(device, 0.15f, 5, 5);
            sphere.ComputeNormals();

            DefaultMaterial = new Material();
            DefaultMaterial.Diffuse = Color.White;
            DefaultMaterial.Ambient = Color.White;
            BlackMaterial = new Material();
            BlackMaterial.Diffuse = Color.Black;
            BlackMaterial.Ambient = Color.Black;
            BlueMaterial = new Material();
            BlueMaterial.Diffuse = Color.Blue;
            BlueMaterial.Ambient = Color.Blue;
            GreenMaterial = new Material();
            GreenMaterial.Diffuse = Color.GreenYellow;
            GreenMaterial.Ambient = Color.GreenYellow;

            LoadMesh();
            return true;
        }

        /// <summary>
        /// Loads all the points / edges, builds the polygon and loads the polygon data.
        /// </summary>
        private void LoadMeta()
        {
            bspMeta = map.SelectedMeta;
            map.OpenMap(MapTypes.Internal);

            // Load the collision planes into the BSP Collision as they are not laoded by default
            for (int i = 0; i < coll.PlaneReflexiveCount; i++)
            {
                map.BR.BaseStream.Position = map.SelectedMeta.offset + coll.PlaneReflexiveTranslation + i * 16;
                Vector4 temp = new Vector4();
                temp.X = map.BR.ReadSingle();
                temp.Y = map.BR.ReadSingle();
                temp.Z = map.BR.ReadSingle();
                temp.W = map.BR.ReadSingle();
                coll.Planes[i].Add(temp);
            }

            // This will build our solid surfaces from the point / edge lists
            for (int i = 0; i < coll.SurfaceReflexiveCount; i++)
            {
                // Creates a new empty polygon (* Collision surfaces are not necessarily triangles)
                polygonInfo pi = new polygonInfo();

                // Collision BSP / 3D Nodes (Offset 36 / 0) [each entry is 8 bytes long]
                map.BR.BaseStream.Position = map.SelectedMeta.offset + coll.SurfaceReflexiveTranslation + i * 8;
                pi.plane = map.BR.ReadInt16();
                int startEdge = map.BR.ReadInt16();
                pi.flags = map.BR.ReadByte();
                pi.breakSurface = map.BR.ReadByte();
                pi.material = map.BR.ReadInt16();

                // The edges are listed in a circular order, we know the first edge, so cycle through until we get back home
                List<short> indices = new List<short>();
                int currentEdge = startEdge;
                do
                {
                    // Collision BSP / Edges (Offset 36 / 48) [each entry is 12 bytes long]
                    map.BR.BaseStream.Position = map.SelectedMeta.offset + coll.FaceReflexiveTranslation + currentEdge * 12;
                    short startVertex = map.BR.ReadInt16();
                    short endVertex = map.BR.ReadInt16();
                    short edge1 = map.BR.ReadInt16();
                    short edge2 = map.BR.ReadInt16();
                    short surfL = map.BR.ReadInt16();
                    short surfR = map.BR.ReadInt16();

                    // The edges are used by two surface that can be listed either left or right
                    // We know what surface we are working with, so we use the edge that relates to our surface #
                    // edge1 for Surface Left, edge2 for Surface Right
                    if (surfL == i)
                    {
                        currentEdge = edge1;
                        indices.Add(endVertex);
                    }
                    else if (surfR == i)
                    {
                        currentEdge = edge2;
                        indices.Add(startVertex);
                    }

                } while (currentEdge != startEdge);

                // The last vertices added is actually the first, so move it to the front
                indices.Insert(0, indices[indices.Count - 1]);
                indices.RemoveAt(indices.Count - 1);
                pi.indices = indices.ToArray();
                polygons.Add(pi);
            }
            map.CloseMap();
        }

        /// <summary>
        /// The load mesh.
        /// </summary>
        /// <remarks></remarks>
        public void LoadMesh()
        {
            dxcoll = new DirectXBSPCollision(ref device, ref coll);
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// <remarks></remarks>
        public void Main()
        {
            using (BSPCollisionViewer frm = this)
            {
                if (!frm.InitializeGraphics())
                {
                    // Initialize Direct3D
                    MessageBox.Show("Could not initialize Direct3D.  This tutorial will exit.");
                    return;
                }

                TranslationMatrix = new Matrix[coll.Vertices.Length];
                for (int i = 0; i < coll.Vertices.Length; i++)
                {
                    Matrix m = Matrix.Identity;
                    m.Multiply(Matrix.Translation(coll.Vertices[i].X, coll.Vertices[i].Y, coll.Vertices[i].Z));
                    TranslationMatrix[i] = m;
                }

                frm.Show();
                frm.Focus();
                cam = new Camera2(this); // Camera(device);

                // While the form is still valid, render and process messages
                while (frm.Created)
                {
                    frm.Render();
                    Application.DoEvents();
                }
            }
        }

        /// <summary>
        /// Keyboard movements
        /// </summary>
        public void MoveSpawnsWithKeyboard()
        {
            try
            {
                cam.device.Acquire();
            }
            catch
            {
                return;
            }

            Key[] keys = cam.device.GetPressedKeys();
            foreach (Key kk in keys)
            {
                switch (kk.ToString())
                {
                    case "Tab":
                        currentMode = (editingModes)((int)(currentMode + 1) % 3);
                        this.lblEditMode.Text = currentMode.ToString();
                        switch (currentMode)
                        {
                            case editingModes.None:
                                lblXLabel.Visible = false;
                                lblX.Visible = false;
                                lblYLabel.Visible = false;
                                lblY.Visible = false;
                                lblZLabel.Visible = false;
                                lblZ.Visible = false;
                                gbSurfaceFlags.Visible = false;
                                break;
                            case editingModes.Point:
                                lblXLabel.Visible = true;
                                lblX.Visible = true;
                                lblYLabel.Visible = true;
                                lblY.Visible = true;
                                lblZLabel.Visible = true;
                                lblZ.Visible = true;
                                gbSurfaceFlags.Visible = false;
                                lblXLabel.Text = "X:";
                                lblYLabel.Text = "Y:";
                                lblX.Location = new Point(28, 35);
                                lblY.Location = new Point(28, 51);
                                updateInfo();
                                break;
                            case editingModes.Surface:
                                lblXLabel.Visible = true;
                                lblX.Visible = true;
                                lblYLabel.Visible = true;
                                lblY.Visible = true;
                                lblZLabel.Visible = false;
                                lblZ.Visible = false;
                                gbSurfaceFlags.Visible = true;
                                lblXLabel.Text = "Surface #";
                                lblYLabel.Text = "   Plane #";
                                lblX.Location = new Point(65, 33);
                                lblY.Location = new Point(65, 49);
                                updateInfo();
                                break;

                        }
                        System.Threading.Thread.Sleep(220);
                        break;
                }

                // Movement of points (only available in point mode)
                if (currentMode == editingModes.Point)
                    for (int x = 0; x < SelectedPoints.Count; x++)
                    {
                        // tslabel.Text = kk.ToString();
                        switch (kk.ToString())
                        {
                            case "Up":
                                coll.Vertices[SelectedPoints[x]].X += cam.speed;
                                break;
                            case "DownArrow":
                                coll.Vertices[SelectedPoints[x]].X -= cam.speed;
                                break;
                            case "LeftArrow":
                                coll.Vertices[SelectedPoints[x]].Y += cam.speed;
                                break;
                            case "Right":
                                coll.Vertices[SelectedPoints[x]].Y -= cam.speed;
                                break;
                            case "PageDown":
                                coll.Vertices[SelectedPoints[x]].Z -= cam.speed;
                                break;
                            case "PageUp":
                                coll.Vertices[SelectedPoints[x]].Z += cam.speed;
                                break;
                            /*
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
                            */

                        }

                        TranslationMatrix[SelectedPoints[x]] = Matrix.Identity;
                        TranslationMatrix[SelectedPoints[x]].Multiply(Matrix.Translation(coll.Vertices[SelectedPoints[x]].X, coll.Vertices[SelectedPoints[x]].Y, coll.Vertices[SelectedPoints[x]].Z));
                        updateBSPInfo(SelectedPoints[x], coll.Vertices[SelectedPoints[x]].X, coll.Vertices[SelectedPoints[x]].Y, coll.Vertices[SelectedPoints[x]].Z);
                    }

            }
        }

        /// <summary>
        /// The on reset device.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        public void OnResetDevice(object sender, EventArgs e)
        {
            Device dev = (Device)sender;

            // Turn off culling, so we see the front and back of the triangle
            dev.RenderState.CullMode = Cull.None;

            // Turn off D3D lighting
            dev.RenderState.Lighting = true;

            // Turn on the ZBuffer
            dev.RenderState.ZBufferEnable = true;
            dev.RenderState.FillMode = FillMode.Solid;
            dev.RenderState.SourceBlend = Blend.SourceAlpha;
            dev.RenderState.DestinationBlend = Blend.InvSourceAlpha;
            dev.RenderState.Ambient = Color.White;
            device.SamplerState[0].MinFilter = TextureFilter.Linear;
            device.SamplerState[0].MagFilter = TextureFilter.Linear;
            device.SamplerState[0].MipFilter = TextureFilter.Linear;
            device.SamplerState[1].MinFilter = TextureFilter.Linear;
            device.SamplerState[1].MagFilter = TextureFilter.Linear;
            device.SamplerState[1].MipFilter = TextureFilter.Linear;
        }

        /// <summary>
        /// Supposed to save BSP Collision point movements and other stuff, but never finished this stuff.
        /// </summary>
        private void saveBSPCollisionChanges()
        {
            BinaryReader BR = new BinaryReader(bspMeta.MS);
            BinaryWriter BW = new BinaryWriter(bspMeta.MS);
            if (map.HaloVersion == HaloMap.Map.HaloVersionEnum.Halo2 ||
                map.HaloVersion == HaloMap.Map.HaloVersionEnum.Halo2Vista)
            {
                // Offset in table to BSP Collision Offset
                BR.BaseStream.Position = 36;

                int tempc = BR.ReadInt32();
                int tempr = BR.ReadInt32() - bspMeta.magic - bspMeta.offset;

                /*
                // +40 = Offset to start of surfaces!
                BW.BaseStream.Position = coll.SurfaceReflexiveTranslation;
                BW.Write(coll.SurfaceReflexiveCount);
                BW.Write(coll.SurfaceReflexiveTranslation + bspMeta.magic + bspMeta.offset);

                #region planes
                // coll.PlaneReflexiveOffset = tempr + 8;
                BW.BaseStream.Position = coll.PlaneReflexiveOffset;
                BW.Write(coll.PlaneReflexiveCount);
                BW.Write(coll.PlaneReflexiveTranslation + bspMeta.magic + bspMeta.offset);
                for (int x = 0; x < coll.PlaneReflexiveCount; x++)
                {
                    BW.Write(coll.Planes[x].X);
                    BW.Write(coll.Planes[x].Y);
                    BW.Write(coll.Planes[x].Z);
                    BW.Write(coll.Planes[x].W);
                }
                #endregion
                */
                #region faces
                // coll.FaceReflexiveOffset = tempr + 48;

                BW.BaseStream.Position = coll.FaceReflexiveOffset;
                BW.Write(coll.FaceReflexiveCount);
                BW.Write(coll.FaceReflexiveTranslation + bspMeta.magic + bspMeta.offset);

                for (int x = 0; x < coll.FaceReflexiveCount; x++)
                {
                    BW.BaseStream.Position = coll.FaceReflexiveTranslation + (x * 12);
                    BW.Write(coll.Faces[(x * 3)]);        // UInt16
                    BW.Write(coll.Faces[(x * 3) + 1]);    // UInt16
                    // coll.Faces[(x * 3) + 2] = coll.Faces[(x * 3) + 1];
                }
                #endregion
                #region vertices

                // coll.VerticeReflexiveOffset = tempr + 56;

                BW.BaseStream.Position = coll.VerticeReflexiveOffset;
                BW.Write(coll.VerticeReflexiveCount);
                BW.Write(coll.VerticeReflexiveTranslation + bspMeta.magic + bspMeta.offset);
                /*
                // Testing of plane calculations
                Vector4 v1 = calculatePlane(
                    new Vector3[] { 
                        coll.Vertices[0], 
                        coll.Vertices[1], 
                        coll.Vertices[2]
                    });

                Plane p1 = DeriveTrianglePlanes(
                    new Vector3[] { 
                        coll.Vertices[0], 
                        coll.Vertices[1], 
                        coll.Vertices[2]
                    });
                */
                for (int x = 0; x < coll.VerticeReflexiveCount; x++)
                {
                    BW.BaseStream.Position = coll.VerticeReflexiveTranslation + (x * 16);
                    BW.Write(coll.Vertices[x].X);
                    BW.Write(coll.Vertices[x].Y);
                    BW.Write(coll.Vertices[x].Z);
                }
                #endregion
            }
            /* // Halo 1 & CE 
            else
            {
                BW.BaseStream.Position = 200;
                int tempc = BW.ReadInt32();
                int tempr = BW.ReadInt32() - bspMeta.magic - bspMeta.offset;
                int tempblah = tempr + 44;
                SurfaceReflexiveCount = BW.ReadInt32();
                SurfaceReflexiveTranslation = BW.ReadInt32() - bspMeta.magic - bspMeta.offset;

                //planes
                PlaneReflexiveOffset = tempr + 12;
                BW.BaseStream.Position = PlaneReflexiveOffset;
                PlaneReflexiveCount = BW.ReadInt32();
                PlaneReflexiveTranslation = BW.ReadInt32() - bspMeta.magic - bspMeta.offset;
                Planes = new Vector4[PlaneReflexiveCount];
                for (int x = 0; x < FaceReflexiveCount; x++)
                {
                    Planes[x].X = BW.ReadSingle();
                    Planes[x].Y = BW.ReadSingle();
                    Planes[x].Z = BW.ReadSingle();
                    Planes[x].W = BW.ReadSingle();
                }
                //faces
                FaceReflexiveOffset = tempr + 72;
                BW.BaseStream.Position = FaceReflexiveOffset;
                FaceReflexiveCount = BW.ReadInt32();
                FaceReflexiveTranslation = BW.ReadInt32() - bspMeta.magic - bspMeta.offset;

                Faces = new ushort[FaceReflexiveCount * 3];
                for (int x = 0; x < FaceReflexiveCount; x++)
                {
                    BW.BaseStream.Position = FaceReflexiveTranslation + (x * 24);

                    Faces[(x * 3)] = (ushort)BW.ReadInt32();
                    Faces[(x * 3) + 1] = (ushort)BW.ReadInt32();
                    Faces[(x * 3) + 2] = Faces[(x * 3) + 1];
                }
                //vertices
                VerticeReflexiveOffset = tempr + 84;
                BW.BaseStream.Position = VerticeReflexiveOffset;
                VerticeReflexiveCount = BW.ReadInt32();
                VerticeReflexiveTranslation = BW.ReadInt32() - bspMeta.magic - bspMeta.offset;
                Vertices = new Vector3[VerticeReflexiveCount];
                for (int x = 0; x < VerticeReflexiveCount; x++)
                {
                    BW.BaseStream.Position = VerticeReflexiveTranslation + (x * 16);
                    Vertices[x].X = BW.ReadSingle();
                    Vertices[x].Y = BW.ReadSingle();
                    Vertices[x].Z = BW.ReadSingle();
                }
            }
            */
            map.OpenMap(MapTypes.Internal);
            map.BW.BaseStream.Position = bspMeta.offset;
            map.BW.BaseStream.Write(bspMeta.MS.ToArray(), 0, bspMeta.size);
            map.CloseMap();
        }

        private void saveBSPSurfaceChanges()
        {
            BinaryReader BR = new BinaryReader(bspMeta.MS);
            BinaryWriter BW = new BinaryWriter(bspMeta.MS);
            if (map.HaloVersion == HaloMap.Map.HaloVersionEnum.Halo2 ||
                map.HaloVersion == HaloMap.Map.HaloVersionEnum.Halo2Vista)
            {
                // Offset in table to BSP Collision Offset
                BR.BaseStream.Position = 36;

                int tempc = BR.ReadInt32();
                int tempr = BR.ReadInt32() - bspMeta.magic - bspMeta.offset;

                #region update Surface changes to MemoryStream
                // +40 = Offset to start of surfaces!
                BW.BaseStream.Position = tempr + 40;
                BW.Write(coll.SurfaceReflexiveCount);
                BW.Write(coll.SurfaceReflexiveTranslation + bspMeta.magic + bspMeta.offset);

                for (int x = 0; x < coll.SurfaceReflexiveCount; x++)
                {
                    BW.BaseStream.Position = coll.SurfaceReflexiveTranslation + x * 8 + 4;
                    BW.Write(polygons[x].flags);
                    BW.Write(polygons[x].breakSurface);
                    BW.Write(polygons[x].material);
                }
                #endregion
            }

            map.OpenMap(MapTypes.Internal);
            map.BW.BaseStream.Position = bspMeta.offset;
            map.BW.BaseStream.Write(bspMeta.MS.ToArray(), 0, bspMeta.size);
            map.CloseMap();
        }

        /// <summary>
        /// Used to reposition verticies in the collision mesh
        /// </summary>
        /// <param name="vertNumber">The vertex number to be moved</param>
        /// <param name="x">New X coordinate</param>
        /// <param name="y">New Y coordinate</param>
        /// <param name="z">New Z coordinate</param>
        void updateBSPInfo(int vertNumber, float x, float y, float z)
        {
            CustomVertex.PositionColored[] vbData =
                (CustomVertex.PositionColored[])dxcoll.vb.Lock(
                            vertNumber * CustomVertex.PositionColored.StrideSize,
                            typeof(CustomVertex.PositionColored),
                            LockFlags.None,
                            1);
            //set your vertices to something...
            vbData[0].Position = new Vector3(x, y, z);
            vbData[0].X = x;
            vbData[0].Y = y;
            vbData[0].Z = z;
            //Unlock the vb before you can use it elsewhere
            dxcoll.vb.Unlock();
        }

        /// <summary>
        /// Used to update the current Meta values.
        /// </summary>
        private void updateInfo()
        {
            switch (currentMode)
            {
                case editingModes.Point:
                    if (SelectedPoints.Count > 0)
                    {
                        // Shows selected point's coordinates with a 4 digit left padding (incl '-') and 5 digit right

                        lblX.Text = coll.Vertices[SelectedPoints[SelectedPoints.Count - 1]].X.ToString();
                        if (!lblX.Text.Contains(".")) lblX.Text += '.';
                        lblX.Text = lblX.Text.Substring(0, lblX.Text.LastIndexOf('.')).PadLeft(4, ' ')
                                  + lblX.Text.Substring(lblX.Text.LastIndexOf('.')).PadRight(6, '0');

                        lblY.Text = coll.Vertices[SelectedPoints[SelectedPoints.Count - 1]].Y.ToString();
                        if (!lblY.Text.Contains(".")) lblY.Text += '.';
                        lblY.Text = lblY.Text.Substring(0, lblY.Text.LastIndexOf('.')).PadLeft(4, ' ')
                                  + lblY.Text.Substring(lblY.Text.LastIndexOf('.')).PadRight(6, '0');

                        lblZ.Text = coll.Vertices[SelectedPoints[SelectedPoints.Count - 1]].Z.ToString();
                        if (!lblZ.Text.Contains(".")) lblZ.Text += '.';
                        lblZ.Text = lblZ.Text.Substring(0, lblZ.Text.LastIndexOf('.')).PadLeft(4, ' ')
                                  + lblZ.Text.Substring(lblZ.Text.LastIndexOf('.')).PadRight(6, '0');
                    }
                    else
                    {
                        lblX.Text = "0.00000";
                        lblY.Text = "0.00000";
                        lblZ.Text = "0.00000";
                    }
                    break;
                case editingModes.Surface:
                    if (currentSurface != -1)
                    {
                        lblX.Text = currentSurface.ToString();
                        lblY.Text = polygons[currentSurface].plane.ToString();
                        cbTwoSided.Checked = (polygons[currentSurface].flags & 1) > 0;
                        cbInvisible.Checked = (polygons[currentSurface].flags & 2) > 0;
                        cbClimable.Checked = (polygons[currentSurface].flags & 4) > 0;
                        cbBreakable.Checked = (polygons[currentSurface].flags & 8) > 0;
                        cbInvalid.Checked = (polygons[currentSurface].flags & 16) > 0;
                        cbConveyor.Checked = (polygons[currentSurface].flags & 32) > 0;
                    }
                    else
                    {
                        lblX.Text = "--";
                        lblY.Text = "--";
                        cbTwoSided.Checked = false;
                        cbInvisible.Checked = false;
                        cbClimable.Checked = false;
                        cbBreakable.Checked = false;
                        cbInvalid.Checked = false;
                        cbConveyor.Checked = false;
                    }
                    break;
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
        }

        /// <summary>
        /// The on paint.
        /// </summary>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        protected override void OnPaint(PaintEventArgs e)
        {
            if (this.Focused)
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
            pause = (this.WindowState == FormWindowState.Minimized) || !this.Visible;
        }

        /// <summary>
        /// Occurs when the form is closed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BSPCollisionViewer_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult result = MessageBox.Show("SAVING THIS WILL DESTROY THE COLLISION ON YOUR MAP!\n" +
                                                  "If you don't know what you should be pressing, hit NO now.\n\n" +
                                                  "Save changes to BSP Collision Points?\n\n" +
                                                  "(NOTE: Collision Surface changes are auto-saved)", "Save BSP Collision?", MessageBoxButtons.YesNoCancel);
            if (result == DialogResult.Yes) { saveBSPCollisionChanges(); }
            if (result == DialogResult.Cancel) { e.Cancel = true; }
            saveBSPSurfaceChanges();
        }

        /// <summary>
        /// The bsp collision viewer_ mouse down.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void BSPCollisionViewer_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                cam.oldx = e.X;
                cam.oldy = e.Y;
            }

            #region SpawnSelection (Mouse Left Button)
            else if (e.Button == MouseButtons.Left)
            {
                if (currentMode == editingModes.Surface)
                #region Check for collision intersection of Surfaces
                {
                    // This takes our mouse cursor and creates a line directly into the screen (2D -> 3D)
                    Vector3 s = Vector3.Unproject(new Vector3(e.X, e.Y, 0),
                        device.Viewport,
                        device.Transform.Projection,
                        device.Transform.View,
                        Matrix.Identity);

                    Vector3 d = Vector3.Unproject(new Vector3(e.X, e.Y, 1),
                        device.Viewport,
                        device.Transform.Projection,
                        device.Transform.View,
                        Matrix.Identity);

                    Vector3 rPosition = s;
                    Vector3 rDirection = Vector3.Normalize(d - s);

                    // Used to find the nearest polygon
                    IntersectInformation iiClosest = new IntersectInformation();
                    iiClosest.Dist = 10000; // Set a very far off distance

                    for (int x = 0; x < polygons.Count; x++)
                    {
                        //check for intersection
                        IntersectInformation ii;

                        for (int xx = 0; xx < polygons[x].indices.Length - 2; xx++)
                        {
                            Geometry.IntersectTri(
                                coll.Vertices[polygons[x].indices[0]],
                                coll.Vertices[polygons[x].indices[xx + 1]],
                                coll.Vertices[polygons[x].indices[xx + 2]],
                                rPosition,
                                rDirection,
                                out ii);

                            if (ii.Dist != 0)
                            {
                                if (iiClosest.Dist > ii.Dist)
                                    iiClosest = ii;
                                else
                                    continue;
                                // holds the surface # that is currently nearest
                                currentSurface = x;
                                break;
                            };
                        }
                        updateInfo();
                    }

                }
                #endregion

                /*
                #region DecideUponObjectRotation
                if ((SelectedSpawn.Count > 0) && (rotationBitMask != 0))
                {
                    selectionStart = render.Mark3DCursorPosition(e.X, e.Y, Matrix.Identity);
                    oldx = e.X;
                    oldy = e.Y;
                    itemrotate = true;
                    //return;
                }
                #endregion
                else
                */
                if (currentMode == editingModes.Point)
                #region Collision point movement mode
                {
                    Mesh checkSphere = Mesh.Sphere(device, 0.3f, 5, 5);
                    #region CheckSpawnsForIntersection
                    for (int x = 0; x < coll.Vertices.Length; x++)
                    {
                        //int tempcount = SpawnModel[spawnmodelindex[x]].Display.Chunk.Count;
                        int tempcount = 1;
                        for (int yy = 0; yy < tempcount; yy++)
                        {
                            // Check under mouse cursor for object selection/deselection?
                            if (render.MeshPick(e.X, e.Y, checkSphere, TranslationMatrix[x]) == true)
                            {

                                #region TurnSpawnOnOrOff
                                int tempi = SelectedPoints.IndexOf(x);
                                if (tempi != -1)
                                {
                                    SelectedPoints.RemoveAt(tempi);
                                }
                                else
                                {
                                    SelectedPoints.Add(x);
                                }
                                #endregion
                                updateInfo();
                                System.Threading.Thread.Sleep(300);                               

                                break;
                            }
                        }
                    }
                    #endregion CycleThroughSpawns
                    checkSphere.Dispose();
                }
                #endregion
            }
            #endregion

        }

        /// <summary>
        /// When the mouse enters the viewport, disable the broup box to remove focus
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BSPCollisionViewer_MouseEnter(object sender, EventArgs e)
        {
            // Remove focus from Checkboxes
            this.gbSurfaceFlags.Enabled = false;
            this.gbSurfaceFlags.Enabled = true;
        }

        /// <summary>
        /// Occurs when any of the checkBoxes are checked/unchecked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cb_CheckedChanged(object sender, EventArgs e)
        {
            if (currentSurface != -1 && ((Control)sender).Focused)
            {
                polygons[currentSurface].flags = (byte)(
                    (cbTwoSided.Checked ? 1 : 0) +
                    (cbInvisible.Checked ? 2 : 0) +
                    (cbClimable.Checked ? 4 : 0) +
                    (cbBreakable.Checked ? 8 : 0) +
                    (cbInvalid.Checked ? 16 : 0) +
                    (cbConveyor.Checked ? 32 : 0));
            }
        }

        /// <summary>
        /// The model viewer_ mouse down.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void ModelViewer_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                cam.change(e.X, e.Y);
            }

            // if (cam.mousenavigation == Camera.MouseNavigationTypes.None)
            // {
            // cam.StartMouseNavigation(e);
            // }
            // cam.NavigateByMouseTrackball(e);
        }

        /// <summary>
        /// The model viewer_ mouse up.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void ModelViewer_MouseUp(object sender, MouseEventArgs e)
        {
            // cam.StopMouseNavigation(e);
        }

        /// <summary>
        /// The render.
        /// </summary>
        /// <remarks></remarks>
        private void Render()
        {
            if (pause)
            {
                return;
            }

            device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);
            device.BeginScene();
            cam.move();

            SetupMatrices();
            MoveSpawnsWithKeyboard();

            device.RenderState.FillMode = FillMode.WireFrame;
            device.Material = DefaultMaterial;
            device.RenderState.Lighting = true;
            device.RenderState.CullMode = Cull.None;

            render.device.SetTexture(0, null);
            render.device.RenderState.AlphaBlendEnable = true;
            render.device.RenderState.AlphaTestEnable = true;
            render.device.RenderState.DestinationBlend = Blend.DestinationAlpha;
            render.device.RenderState.SourceBlend = Blend.SourceAlpha;
            device.Transform.World = Matrix.Identity;
            //Raw.ParsedModel.DisplayedInfo.Draw(ref device, ref bsp.SkyBox);
            dxcoll.Draw(ref device);

            if (currentMode == editingModes.Point)
            {
                device.RenderState.FillMode = FillMode.Point;
                device.Material = BlueMaterial;
                device.RenderState.PointSize = 4;
                dxcoll.Draw(ref device);

                #region Render Spheres over selected points
                device.Material = GreenMaterial;
                device.RenderState.FillMode = FillMode.WireFrame;
                device.VertexFormat = CustomVertex.PositionColored.Format;
                device.RenderState.AlphaBlendEnable = false;
                device.RenderState.AlphaTestEnable = false;
                device.SetTexture(0, null);


                for (int i = 0; i < SelectedPoints.Count; i++)
                {

                    device.Transform.World = Matrix.Translation(coll.Vertices[SelectedPoints[i]].X, coll.Vertices[SelectedPoints[i]].Y, coll.Vertices[SelectedPoints[i]].Z);
                    sphere.DrawSubset(0);
                }
                #endregion
            }
            else if (currentMode == editingModes.Surface)
            {
                if (currentSurface != -1)
                {

                    device.RenderState.Lighting = false;
                    device.RenderState.ZBufferEnable = false;
                    device.RenderState.CullMode = Cull.None;
                    device.RenderState.AlphaBlendEnable = true;
                    device.TextureState[0].ColorOperation = TextureOperation.SelectArg1;
                    device.TextureState[0].AlphaOperation = TextureOperation.SelectArg1;
                    device.TextureState[0].ColorArgument1 = TextureArgument.TextureColor;
                    device.TextureState[0].AlphaArgument1 = TextureArgument.TextureColor;

                    //device.Material = GreenMaterial;
                    device.RenderState.FillMode = FillMode.Solid;
                    device.Transform.World = Matrix.Identity;
                    device.DrawIndexedUserPrimitives(PrimitiveType.TriangleFan,
                            0,
                            coll.Vertices.Length,
                            polygons[currentSurface].indices.Length - 2,
                            polygons[currentSurface].indices,
                            true,
                            coll.Vertices);
                }
            }

            device.EndScene();

            // Update the screen
            device.Present();
        }

        /// <summary>
        /// The setup matrices.
        /// </summary>
        /// <remarks></remarks>
        private void SetupMatrices()
        {
            device.Transform.World = Matrix.Identity;

            // Matrix.RotationAxis(new Vector3((float)Math.Cos(Environment.TickCount / 250.0f), 1, (float)Math.Sin(Environment.TickCount / 250.0f)), Environment.TickCount / 1000.0f);
            device.Transform.View = Matrix.LookAtRH(cam.Position, cam.LookAt, cam.UpVector);
            device.Transform.Projection = Matrix.PerspectiveFovRH((float)Math.PI / 4.0f, 1.0f, 0.1f, 10000.0f);
        }

        #endregion
    }
}