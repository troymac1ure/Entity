using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.IO;                 
using Microsoft.DirectX;
using Microsoft.DirectX.DirectInput;
using Microsoft.DirectX.Direct3D;
using Direct3D = Microsoft.DirectX.Direct3D;

namespace entity.Renderer
{
    public class BSPCollisionViewer : Form
    {
        // Our global variables for this project
        PresentParameters presentParams = new PresentParameters();
        //Materials
        Material BlueMaterial;
        Material BlackMaterial;
        Material DefaultMaterial;
        Material GreenMaterial;

        Matrix WorldTransform = Matrix.Identity;
        entity.Renderer.Camera2 cam;
        BSP.BSPModel.BSPCollision coll;
 
        Direct3D.Device device = null; // Our rendering device
        MetaContainers.DirectXBSPCollision dxcoll;
        int mapnumber;
        bool pause = false;

        entity.Renderer.Renderer render = new entity.Renderer.Renderer();
        Matrix[] TranslationMatrix;
        Mesh sphere; 
        List<int> SelectedPoints = new List<int>();
        List<polygonInfo> polygons = new List<polygonInfo>();
        int currentSurface = -1;
        Meta bspMeta;

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

        // List the current editing mode
        editingModes currentMode;        
        private enum editingModes
        {
            None = 0,
            Surface = 1,
            Point = 2,

        }

        public class polygonInfo
        {
            public short[] indices;        // The indices of the polygon
            public int plane;              // The plane index of the polygon
            public byte flags;             // Polygon flags
                                           //   <option name="Two-Sided" bit="0" />
                                           //   <option name="Invisible" bit="1" />
                                           //   <option name="Climable" bit="2" />
                                           //   <option name="Breakable" bit="3" />
                                           //   <option name="Invaild" bit="4" />
                                           //   <option name="Conveyor" bit="5" /
            public byte breakSurface;      // Breakable Surface # ?? Maybe part of flags?
            public short material;         // The material of the polygon

            public polygonInfo()
            {
            }
        }


        public BSPCollisionViewer(BSP.BSPModel.BSPCollision tempcoll, int mapnumberx)
            
        {
            //InitializeComponent
            InitializeComponent();

            mapnumber = mapnumberx;
            this.coll = tempcoll;

            bspMeta = Maps.map[mapnumber].SelectedMeta;
            Maps.map[mapnumber].OpenMap(MapTypes.Internal);

            // Load the collision planes into the BSP Collision as they are not laoded by default
            for (int i = 0; i < coll.PlaneReflexiveCount; i++)
            {
                Maps.map[mapnumber].BR.BaseStream.Position = Maps.map[mapnumber].SelectedMeta.offset + coll.PlaneReflexiveTranslation + i * 16;
                Vector4 temp = new Vector4();
                temp.X = Maps.map[mapnumber].BR.ReadSingle();
                temp.Y = Maps.map[mapnumber].BR.ReadSingle();
                temp.Z = Maps.map[mapnumber].BR.ReadSingle();
                temp.W = Maps.map[mapnumber].BR.ReadSingle();
                coll.Planes[i].Add(temp);
            }

            // This will build our solid surfaces from the point / edge lists
            for (int i = 0; i < coll.SurfaceReflexiveCount; i++)
            {
                // Creates a new empty polygon (* Collision surfaces are not necessarily triangles)
                polygonInfo pi = new polygonInfo();

                // Collision BSP / 3D Nodes (Offset 36 / 0) [each entry is 8 bytes long]
                Maps.map[mapnumber].BR.BaseStream.Position = Maps.map[mapnumber].SelectedMeta.offset + coll.SurfaceReflexiveTranslation + i * 8;
                pi.plane = Maps.map[mapnumber].BR.ReadInt16();
                int startEdge = Maps.map[mapnumber].BR.ReadInt16();
                pi.flags = Maps.map[mapnumber].BR.ReadByte();
                pi.breakSurface = Maps.map[mapnumber].BR.ReadByte();
                pi.material = Maps.map[mapnumber].BR.ReadInt16();
                
                // The edges are listed in a circular order, we know the first edge, so cycle through until we get back home
                List<short> indices = new List<short>();
                int currentEdge = startEdge;
                do
                {
                    // Collision BSP / Edges (Offset 36 / 48) [each entry is 12 bytes long]
                    Maps.map[mapnumber].BR.BaseStream.Position = Maps.map[mapnumber].SelectedMeta.offset + coll.FaceReflexiveTranslation + currentEdge * 12;
                    short startVertex = Maps.map[mapnumber].BR.ReadInt16();
                    short endVertex = Maps.map[mapnumber].BR.ReadInt16();
                    short edge1 = Maps.map[mapnumber].BR.ReadInt16();
                    short edge2 = Maps.map[mapnumber].BR.ReadInt16();
                    short surfL = Maps.map[mapnumber].BR.ReadInt16();
                    short surfR = Maps.map[mapnumber].BR.ReadInt16();

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
            Maps.map[mapnumber].CloseMap();
            
            Main();
        }

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

        void BSPCollisionViewer_MouseDown(object sender, MouseEventArgs e)
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
                        render.device.Viewport,
                        render.device.Transform.Projection,
                        render.device.Transform.View,
                        Matrix.Identity);

                    Vector3 d = Vector3.Unproject(new Vector3(e.X, e.Y, 1),
                        render.device.Viewport,
                        render.device.Transform.Projection,
                        render.device.Transform.View,
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

        

        public bool InitializeGraphics()
        {
            
            //try
            //{
            this.Show();
            this.Focus();
            Application.DoEvents();

            render.CreateDevice(this);
            device = render.device;

            presentParams.Windowed = true; // We don't want to run fullscreen
            presentParams.PresentationInterval = Direct3D.PresentInterval.Default;
            presentParams.FullScreenRefreshRateInHz = Direct3D.PresentParameters.DefaultPresentRate;
            pause = false;

            sphere = Mesh.Sphere(device, 0.15f, 5, 5);
            sphere.ComputeNormals();
   
       
            DefaultMaterial = new Material();
            DefaultMaterial.Diffuse = Color.White;
            DefaultMaterial.Ambient = Color.White;
            BlackMaterial  = new Material();
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

        public void LoadMesh()
        {
            dxcoll = new entity.MetaContainers.DirectXBSPCollision(ref device,ref coll);
        }

        public void OnResetDevice(object sender, EventArgs e)
        {
            Direct3D.Device dev = (Direct3D.Device)sender;
            // Turn off culling, so we see the front and back of the triangle
            dev.RenderState.CullMode = Cull.None ;
            // Turn off D3D lighting
            dev.RenderState.Lighting = true;
            // Turn on the ZBuffer
            dev.RenderState.ZBufferEnable = true;
            dev.RenderState.FillMode = FillMode.WireFrame;
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


        private void SetupMatrices()
        {
            device.Transform.World =  Matrix.Identity;// Matrix.RotationAxis(new Vector3((float)Math.Cos(Environment.TickCount / 250.0f), 1, (float)Math.Sin(Environment.TickCount / 250.0f)), Environment.TickCount / 1000.0f);
            device.Transform.View = Matrix.LookAtRH(cam.Position, cam.LookAt, cam.UpVector);
            device.Transform.Projection = Matrix.PerspectiveFovRH((float)Math.PI / 4.0f, 1.0f, 0.1f, 10000.0f);
        }

        private void Render()
        {
            if (pause)
                return;

            
            render.BeginScene(System.Drawing.Color.Black);
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
            device.Transform.World=Matrix.Identity;
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

            // Update the screen
            render.EndScene();
        }

        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            if (this.Focused)
                this.Render(); // Render on painting
        }
        protected override void OnKeyPress(System.Windows.Forms.KeyPressEventArgs e)
        {
            if ((int)(byte)e.KeyChar == (int)System.Windows.Forms.Keys.Escape)
                this.Dispose(); // Esc was pressed
            e.Handled = true;
        }
        protected override void OnResize(System.EventArgs e)
        {
       //     this.Render();//
            pause = ((this.WindowState == FormWindowState.Minimized) || !this.Visible);
        }



        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        public void Main()
        {

            using (BSPCollisionViewer frm = (BSPCollisionViewer)this)
            {
                if (!frm.InitializeGraphics()) // Initialize Direct3D
                {
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
                cam = new entity.Renderer.Camera2(this);// Camera(device);

                // While the form is still valid, render and process messages
                while (frm.Created)
                {
                    frm.Render();
                    Application.DoEvents();
                }
            }
        }

        private void InitializeComponent()
        {
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblTabChange = new System.Windows.Forms.Label();
            this.lblZ = new System.Windows.Forms.Label();
            this.lblY = new System.Windows.Forms.Label();
            this.lblX = new System.Windows.Forms.Label();
            this.lblZLabel = new System.Windows.Forms.Label();
            this.lblYLabel = new System.Windows.Forms.Label();
            this.lblXLabel = new System.Windows.Forms.Label();
            this.lblEditMode = new System.Windows.Forms.Label();
            this.lblEditModeLabel = new System.Windows.Forms.Label();
            this.gbSurfaceFlags = new System.Windows.Forms.GroupBox();
            this.cbConveyor = new System.Windows.Forms.CheckBox();
            this.cbInvalid = new System.Windows.Forms.CheckBox();
            this.cbBreakable = new System.Windows.Forms.CheckBox();
            this.cbClimable = new System.Windows.Forms.CheckBox();
            this.cbInvisible = new System.Windows.Forms.CheckBox();
            this.cbTwoSided = new System.Windows.Forms.CheckBox();
            this.panel1.SuspendLayout();
            this.gbSurfaceFlags.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.AutoSize = true;
            this.panel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel1.BackColor = System.Drawing.SystemColors.ControlDark;
            this.panel1.Controls.Add(this.lblTabChange);
            this.panel1.Controls.Add(this.lblZ);
            this.panel1.Controls.Add(this.lblY);
            this.panel1.Controls.Add(this.lblX);
            this.panel1.Controls.Add(this.lblZLabel);
            this.panel1.Controls.Add(this.lblYLabel);
            this.panel1.Controls.Add(this.lblXLabel);
            this.panel1.Controls.Add(this.lblEditMode);
            this.panel1.Controls.Add(this.lblEditModeLabel);
            this.panel1.Controls.Add(this.gbSurfaceFlags);
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(113, 180);
            this.panel1.TabIndex = 0;
            this.panel1.MouseLeave += new System.EventHandler(this.panel1_MouseLeave);
            this.panel1.MouseEnter += new System.EventHandler(this.panel1_MouseEnter);
            // 
            // lblTabChange
            // 
            this.lblTabChange.AutoSize = true;
            this.lblTabChange.Location = new System.Drawing.Point(3, 20);
            this.lblTabChange.Name = "lblTabChange";
            this.lblTabChange.Size = new System.Drawing.Size(86, 13);
            this.lblTabChange.TabIndex = 9;
            this.lblTabChange.Text = "(TAB to Change)";
            // 
            // lblZ
            // 
            this.lblZ.AutoSize = true;
            this.lblZ.Font = new System.Drawing.Font(FontFamily.GenericMonospace.Name, 10);
            this.lblZ.Font = new System.Drawing.Font(FontFamily.GenericSansSerif.Name, 12);
            this.lblZ.Location = new System.Drawing.Point(28, 67);
            this.lblZ.Name = "lblZ";
            this.lblZ.Size = new System.Drawing.Size(13, 13);
            this.lblZ.TabIndex = 7;
            this.lblZ.Text = "0";
            this.lblZ.Visible = false;
            // 
            // lblY
            // 
            this.lblY.AutoSize = true;
            this.lblY.Font = new System.Drawing.Font(FontFamily.GenericMonospace.Name, 10);
            this.lblY.Location = new System.Drawing.Point(28, 51);
            this.lblY.Name = "lblY";
            this.lblY.Size = new System.Drawing.Size(13, 13);
            this.lblY.TabIndex = 6;
            this.lblY.Text = "0";
            this.lblY.Visible = false;
            // 
            // lblX
            // 
            this.lblX.AutoSize = true;
            this.lblX.Font = new System.Drawing.Font(FontFamily.GenericMonospace.Name, 10);
            this.lblX.Location = new System.Drawing.Point(28, 35);
            this.lblX.Name = "lblX";
            this.lblX.Size = new System.Drawing.Size(13, 13);
            this.lblX.TabIndex = 5;
            this.lblX.Text = "0";
            this.lblX.Visible = false;
            // 
            // lblZLabel
            // 
            this.lblZLabel.AutoSize = true;
            this.lblZLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblZLabel.Location = new System.Drawing.Point(3, 67);
            this.lblZLabel.Name = "lblZLabel";
            this.lblZLabel.Size = new System.Drawing.Size(19, 13);
            this.lblZLabel.TabIndex = 4;
            this.lblZLabel.Text = "Z:";
            this.lblZLabel.Visible = false;
            // 
            // lblYLabel
            // 
            this.lblYLabel.AutoSize = true;
            this.lblYLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblYLabel.Location = new System.Drawing.Point(3, 51);
            this.lblYLabel.Name = "lblYLabel";
            this.lblYLabel.Size = new System.Drawing.Size(19, 13);
            this.lblYLabel.TabIndex = 3;
            this.lblYLabel.Text = "Y:";
            this.lblYLabel.Visible = false;
            // 
            // lblXLabel
            // 
            this.lblXLabel.AutoSize = true;
            this.lblXLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblXLabel.Location = new System.Drawing.Point(3, 35);
            this.lblXLabel.Name = "lblXLabel";
            this.lblXLabel.Size = new System.Drawing.Size(19, 13);
            this.lblXLabel.TabIndex = 2;
            this.lblXLabel.Text = "X:";
            this.lblXLabel.Visible = false;
            // 
            // lblEditMode
            // 
            this.lblEditMode.AutoSize = true;
            this.lblEditMode.Location = new System.Drawing.Point(67, 7);
            this.lblEditMode.Name = "lblEditMode";
            this.lblEditMode.Size = new System.Drawing.Size(33, 13);
            this.lblEditMode.TabIndex = 1;
            this.lblEditMode.Text = "None";
            // 
            // lblEditModeLabel
            // 
            this.lblEditModeLabel.AutoSize = true;
            this.lblEditModeLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblEditModeLabel.Location = new System.Drawing.Point(3, 7);
            this.lblEditModeLabel.Name = "lblEditModeLabel";
            this.lblEditModeLabel.Size = new System.Drawing.Size(68, 13);
            this.lblEditModeLabel.TabIndex = 0;
            this.lblEditModeLabel.Text = "Edit Mode:";
            // 
            // gbSurfaceFlags
            // 
            this.gbSurfaceFlags.Controls.Add(this.cbConveyor);
            this.gbSurfaceFlags.Controls.Add(this.cbInvalid);
            this.gbSurfaceFlags.Controls.Add(this.cbBreakable);
            this.gbSurfaceFlags.Controls.Add(this.cbClimable);
            this.gbSurfaceFlags.Controls.Add(this.cbInvisible);
            this.gbSurfaceFlags.Controls.Add(this.cbTwoSided);
            this.gbSurfaceFlags.Location = new System.Drawing.Point(6, 67);
            this.gbSurfaceFlags.Name = "gbSurfaceFlags";
            this.gbSurfaceFlags.Size = new System.Drawing.Size(104, 110);
            this.gbSurfaceFlags.TabIndex = 8;
            this.gbSurfaceFlags.TabStop = false;
            this.gbSurfaceFlags.Text = "Surface Flags";
            this.gbSurfaceFlags.Visible = false;
            // 
            // cbConveyor
            // 
            this.cbConveyor.AutoSize = true;
            this.cbConveyor.Location = new System.Drawing.Point(6, 90);
            this.cbConveyor.Name = "cbConveyor";
            this.cbConveyor.Size = new System.Drawing.Size(71, 17);
            this.cbConveyor.TabIndex = 5;
            this.cbConveyor.Text = "Conveyor";
            this.cbConveyor.UseVisualStyleBackColor = true;
            this.cbConveyor.MouseLeave += new System.EventHandler(this.panel1_MouseLeave);
            this.cbConveyor.CheckedChanged += new System.EventHandler(this.cb_CheckedChanged);
            // 
            // cbInvalid
            // 
            this.cbInvalid.AutoSize = true;
            this.cbInvalid.Location = new System.Drawing.Point(6, 75);
            this.cbInvalid.Name = "cbInvalid";
            this.cbInvalid.Size = new System.Drawing.Size(57, 17);
            this.cbInvalid.TabIndex = 4;
            this.cbInvalid.Text = "Invalid";
            this.cbInvalid.UseVisualStyleBackColor = true;
            this.cbInvalid.MouseLeave += new System.EventHandler(this.panel1_MouseLeave);
            this.cbInvalid.CheckedChanged += new System.EventHandler(this.cb_CheckedChanged);
            // 
            // cbBreakable
            // 
            this.cbBreakable.AutoSize = true;
            this.cbBreakable.Location = new System.Drawing.Point(6, 60);
            this.cbBreakable.Name = "cbBreakable";
            this.cbBreakable.Size = new System.Drawing.Size(74, 17);
            this.cbBreakable.TabIndex = 3;
            this.cbBreakable.Text = "Breakable";
            this.cbBreakable.UseVisualStyleBackColor = true;
            this.cbBreakable.MouseLeave += new System.EventHandler(this.panel1_MouseLeave);
            this.cbBreakable.CheckedChanged += new System.EventHandler(this.cb_CheckedChanged);
            // 
            // cbClimable
            // 
            this.cbClimable.AutoSize = true;
            this.cbClimable.Location = new System.Drawing.Point(6, 45);
            this.cbClimable.Name = "cbClimable";
            this.cbClimable.Size = new System.Drawing.Size(65, 17);
            this.cbClimable.TabIndex = 2;
            this.cbClimable.Text = "Climable";
            this.cbClimable.UseVisualStyleBackColor = true;
            this.cbClimable.MouseLeave += new System.EventHandler(this.panel1_MouseLeave);
            this.cbClimable.CheckedChanged += new System.EventHandler(this.cb_CheckedChanged);
            // 
            // cbInvisible
            // 
            this.cbInvisible.AutoSize = true;
            this.cbInvisible.Location = new System.Drawing.Point(6, 30);
            this.cbInvisible.Name = "cbInvisible";
            this.cbInvisible.Size = new System.Drawing.Size(64, 17);
            this.cbInvisible.TabIndex = 1;
            this.cbInvisible.Text = "Invisible";
            this.cbInvisible.UseVisualStyleBackColor = true;
            this.cbInvisible.MouseLeave += new System.EventHandler(this.panel1_MouseLeave);
            this.cbInvisible.CheckedChanged += new System.EventHandler(this.cb_CheckedChanged);
            // 
            // cbTwoSided
            // 
            this.cbTwoSided.AutoSize = true;
            this.cbTwoSided.Location = new System.Drawing.Point(6, 15);
            this.cbTwoSided.Name = "cbTwoSided";
            this.cbTwoSided.Size = new System.Drawing.Size(77, 17);
            this.cbTwoSided.TabIndex = 0;
            this.cbTwoSided.Text = "Two Sided";
            this.cbTwoSided.UseVisualStyleBackColor = true;
            this.cbTwoSided.MouseLeave += new System.EventHandler(this.panel1_MouseLeave);
            this.cbTwoSided.CheckedChanged += new System.EventHandler(this.cb_CheckedChanged);
            // 
            // BSPCollisionViewer
            // 
            this.ClientSize = new System.Drawing.Size(800, 600);
            this.Controls.Add(this.panel1);
            this.Name = "BSPCollisionViewer";
            this.Text = "BSP Viewer";
            this.MouseEnter += new System.EventHandler(this.BSPCollisionViewer_MouseEnter);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.BSPCollisionViewer_MouseDown);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.BSPCollisionViewer_FormClosing);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ModelViewer_MouseDown);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.gbSurfaceFlags.ResumeLayout(false);
            this.gbSurfaceFlags.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void ModelViewer_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                cam.change(e.X, e.Y);
            }
            // if (cam.mousenavigation == Camera.MouseNavigationTypes.None)
            // {
            //    cam.StartMouseNavigation(e);
            //  }
            //  cam.NavigateByMouseTrackball(e);

        }


        private void ModelViewer_MouseUp(object sender, MouseEventArgs e)
        {

          //  cam.StopMouseNavigation(e);

        }

        #region KeyBoardFunctions
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
                        currentMode = (editingModes)((int)(currentMode+1) % 3);
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
                                lblX.Location = new Point(60, 35);
                                lblY.Location = new Point(60, 51);
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
        #endregion
       
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
        /// Supposed to save BSP Collision point movements and other stuff, but never finished this stuff.
        /// </summary>
        private void saveBSPCollisionChanges()
        {
            BinaryReader BR = new BinaryReader(bspMeta.MS);
            BinaryWriter BW = new BinaryWriter(bspMeta.MS);
            if (Maps.map[bspMeta.mapnumber].HaloVersion == Map.HaloVersionEnum.Halo2)
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
            Maps.map[mapnumber].OpenMap(MapTypes.Internal);
            Maps.map[mapnumber].BW.BaseStream.Position = bspMeta.offset;
            Maps.map[mapnumber].BW.BaseStream.Write(bspMeta.MS.ToArray(), 0, bspMeta.size);
            Maps.map[mapnumber].CloseMap();
        }

        private void saveBSPSurfaceChanges()
        {
            BinaryReader BR = new BinaryReader(bspMeta.MS);
            BinaryWriter BW = new BinaryWriter(bspMeta.MS);
            if (Maps.map[bspMeta.mapnumber].HaloVersion == Map.HaloVersionEnum.Halo2)
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

            Maps.map[mapnumber].OpenMap(MapTypes.Internal);
            Maps.map[mapnumber].BW.BaseStream.Position = bspMeta.offset;
            Maps.map[mapnumber].BW.BaseStream.Write(bspMeta.MS.ToArray(), 0, bspMeta.size);
            Maps.map[mapnumber].CloseMap();
        }

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

        private void panel1_MouseEnter(object sender, EventArgs e)
        {
            //this.gbSurfaceFlags.Enabled = true;
        }

        private void panel1_MouseLeave(object sender, EventArgs e)
        {
        }

        private void BSPCollisionViewer_MouseEnter(object sender, EventArgs e)
        {
            // Remove focus from Checkboxes
            this.gbSurfaceFlags.Enabled = false;
            this.gbSurfaceFlags.Enabled = true;
        }

        private void cb_CheckedChanged(object sender, EventArgs e)
        {
            if (currentSurface != -1 && ((Control)sender).Focused)
            {
                polygons[currentSurface].flags = (byte)(
                    (cbTwoSided.Checked  ?  1 : 0) +
                    (cbInvisible.Checked ?  2 : 0) +
                    (cbClimable.Checked  ?  4 : 0) +
                    (cbBreakable.Checked ?  8 : 0) +
                    (cbInvalid.Checked   ? 16 : 0) +
                    (cbConveyor.Checked  ? 32 : 0));
            }
        }

}
}
