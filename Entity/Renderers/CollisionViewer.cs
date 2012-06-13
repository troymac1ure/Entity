// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CollisionViewer.cs" company="">
//   
// </copyright>
// <summary>
//   The collision viewer.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace entity.Renderers
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Threading;
    using System.Windows.Forms;

    using HaloMap.H2MetaContainers;
    using HaloMap.Map;

    using Microsoft.DirectX;
    using Microsoft.DirectX.Direct3D;
    using Microsoft.DirectX.DirectInput;

    using HaloMap.Render;

    using Device = Microsoft.DirectX.Direct3D.Device;
    using DeviceType = Microsoft.DirectX.Direct3D.DeviceType;

    /// <summary>
    /// The collision viewer.
    /// </summary>
    /// <remarks></remarks>
    public partial class CollisionViewer : Form
    {
        #region Constants and Fields

        /// <summary>
        /// The keyboard.
        /// </summary>
        public Microsoft.DirectX.DirectInput.Device keyboard;

        /// <summary>
        /// The m list.
        /// </summary>
        private readonly List<Mesh> mList = new List<Mesh>();

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
        /// The vb.
        /// </summary>
        private VertexBuffer Vb;

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
        private coll coll;

        // Our global variables for this project
        /// <summary>
        /// The device.
        /// </summary>
        private Device device; // Our rendering device

        /// <summary>
        /// The dxcoll.
        /// </summary>
        private DirectXCollision dxcoll;

        /// <summary>
        /// The map.
        /// </summary>
        private Map map;

        /// <summary>
        /// The normal vertices.
        /// </summary>
        private CustomVertex.PositionColored[] normalVertices;

        /// <summary>
        /// The pause.
        /// </summary>
        private bool pause;

        /// <summary>
        /// The plane display.
        /// </summary>
        private int planeDisplay;

        /// <summary>
        /// The plane display all.
        /// </summary>
        private bool planeDisplayAll = true;

        /// <summary>
        /// The sphere display all.
        /// </summary>
        private bool sphereDisplayAll = true;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CollisionViewer"/> class.
        /// </summary>
        /// <param name="tempcoll">The tempcoll.</param>
        /// <param name="map">The map.</param>
        /// <remarks></remarks>
        public CollisionViewer(coll tempcoll, Map map)
        {
            coll = tempcoll;
            this.map = map;

            // Set the initial size of our form
            this.ClientSize = new Size(800, 600);

            // And its caption
            this.Text = "Collision Viewer";
            this.MouseDown += BSPCollisionViewer_MouseDown;
            this.MouseMove += this.ModelViewer_MouseDown;
            
            keyboard = new Microsoft.DirectX.DirectInput.Device(SystemGuid.Keyboard);

            Main();
        }

        #endregion

        #region Public Methods

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
            presentParams.Windowed = true; // We don't want to run fullscreen
            presentParams.PresentationInterval = PresentInterval.Default;
            presentParams.FullScreenRefreshRateInHz = PresentParameters.DefaultPresentRate;

            presentParams.SwapEffect = SwapEffect.Discard; // Discard the frames
            presentParams.EnableAutoDepthStencil = true; // Turn on a Depth stencil
            presentParams.AutoDepthStencilFormat = DepthFormat.D16; // And the stencil format
            device = new Device(0, DeviceType.Hardware, this, CreateFlags.SoftwareVertexProcessing, presentParams);

            // Create a device
            device.DeviceReset += this.OnResetDevice;
            this.OnResetDevice(device, null);
            pause = false;

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
        /// The load mesh.
        /// </summary>
        /// <remarks></remarks>
        public void LoadMesh()
        {
            dxcoll = new DirectXCollision(ref device, ref coll);

            for (int mc = 0; mc < coll.Meshes.Count; mc++)
            {
                for (int i = 0; i < coll.Meshes[mc].SurfaceData.Length; i++)
                {
                    // for (int i = 0; i < 1; i++)
                    Mesh m = new Mesh(
                        coll.Meshes[mc].SurfaceData.Length, 
                        coll.Meshes[mc].Vertices.Length, 
                        MeshFlags.Managed, 
                        HaloVertex.FVF, 
                        device);
                    VertexBuffer vb = m.VertexBuffer;
                    HaloVertex[] tempv = new HaloVertex[coll.Meshes[mc].Vertices.Length];
                    for (int x = 0; x < coll.Meshes[mc].Vertices.Length; x++)
                    {
                        tempv[x].Position = coll.Meshes[mc].Vertices[x];
                    }

                    vb.SetData(tempv, 0, LockFlags.None);
                    vb.Unlock();

                    short[] b = new short[(coll.Meshes[mc].SurfaceData[i].Vertices.Count - 2) * 3];
                    for (int ii = 0; ii < 3; ii++)
                    {
                        b[ii] = (short)coll.Meshes[mc].SurfaceData[i].Vertices[ii];
                    }

                    for (int ii = 1; ii < coll.Meshes[mc].SurfaceData[i].Vertices.Count - 2; ii++)
                    {
                        b[ii * 3] = b[0];
                        b[ii * 3 + 1] = b[ii * 3 - 1];
                        b[ii * 3 + 2] = (short)coll.Meshes[mc].SurfaceData[i].Vertices[ii + 2];
                    }

                    IndexBuffer ibx = m.IndexBuffer;
                    ibx.SetData(b, 0, LockFlags.None);
                    mList.Add(m);
                }

                normalVertices = new CustomVertex.PositionColored[coll.Meshes[mc].Normals.Length * 2];
                for (int i = 0; i < coll.Meshes[mc].Normals.Length; i++)
                {
                    normalVertices[i * 2 + 0].Color = Color.Yellow.ToArgb();
                    normalVertices[i * 2 + 0].Position =
                        new Vector3(
                            coll.Meshes[mc].Normals[i].X * coll.Meshes[mc].Normals[i].W, 
                            coll.Meshes[mc].Normals[i].Y * coll.Meshes[mc].Normals[i].W, 
                            coll.Meshes[mc].Normals[i].Z * coll.Meshes[mc].Normals[i].W);
                    normalVertices[i * 2 + 1].Color = Color.White.ToArgb();
                    normalVertices[i * 2 + 1].Position = new Vector3(
                        coll.Meshes[mc].Normals[i].X, coll.Meshes[mc].Normals[i].Y, coll.Meshes[mc].Normals[i].Z);

                    // normalVertices[i * 3 + 2].Position = new Vector4(1, 1, 1, 1.0f);
                    // normalVertices[i * 3 + 2].Color = Color.White.ToArgb();
                }
            }

            Vb = new VertexBuffer(
                typeof(CustomVertex.PositionColored), 
                normalVertices.Length, 
                device, 
                Usage.Dynamic | Usage.WriteOnly, 
                CustomVertex.PositionColored.Format, 
                Pool.Default);
            Vb.SetData(normalVertices, 0, LockFlags.None);
            device.SetStreamSource(0, Vb, 0);
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// <remarks></remarks>
        public void Main()
        {
            using (CollisionViewer frm = this)
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
                cam.speed = 0.30f;

                // While the form is still valid, render and process messages
                while (frm.Created)
                {
                    frm.Render();
                    frm.checkKeys();
                    Application.DoEvents();
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
        }

        /// <summary>
        /// The initialize component.
        /// </summary>
        /// <remarks></remarks>
        private void InitializeComponent()
        {
            this.SuspendLayout();

            // BSPViewer
            this.ClientSize = new Size(292, 266);
            this.Name = "BSPViewer";
            this.ResumeLayout(false);
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

            device.Material = DefaultMaterial;

            device.Transform.World = Matrix.Identity;
            device.RenderState.CullMode = Cull.Clockwise;

            // Raw.ParsedModel.DisplayedInfo.Draw(ref device, ref bsp.SkyBox);
            Material m = new Material();
            device.RenderState.CullMode = Cull.None;

            device.RenderState.FillMode = FillMode.WireFrame;
            device.RenderState.AlphaBlendEnable = false;
            device.RenderState.AlphaTestEnable = false;
            device.Material = DefaultMaterial;
            dxcoll.DrawMeshes(ref device);
            if (sphereDisplayAll)
            {
                device.Material = BlueMaterial;
                dxcoll.DrawSpheres(ref device);
                device.Transform.World = Matrix.Identity;
            }

            if (planeDisplayAll)
            {
                for (int i = 0; i < mList.Count; i++)
                {
                    m.Ambient = Color.FromArgb(130, (24 - (i % 25)) * 10, i % 25 * 10);
                    m.Diffuse = m.Ambient;
                    device.Material = m;

                    device.SetTexture(0, null);
                    device.RenderState.AlphaBlendEnable = true;
                    device.RenderState.AlphaTestEnable = true;
                    device.RenderState.DestinationBlend = Blend.Zero;
                    device.RenderState.SourceBlend = Blend.One;
                    device.RenderState.FillMode = FillMode.Solid;
                    mList[i].DrawSubset(0);
                }

                // Draw normals
                device.SetStreamSource(0, Vb, 0);
                device.Transform.World = Matrix.Identity;
                device.VertexFormat = CustomVertex.PositionColored.Format;
                device.DrawPrimitives(PrimitiveType.LineList, 0, normalVertices.Length / 2);
            }
            else
            {
                m.Ambient = Color.Red;
                m.Diffuse = Color.Red;
                device.Material = m;

                device.SetTexture(0, null);
                device.RenderState.AlphaBlendEnable = true;
                device.RenderState.AlphaTestEnable = true;
                device.RenderState.DestinationBlend = Blend.Zero;
                device.RenderState.SourceBlend = Blend.One;
                device.RenderState.FillMode = FillMode.Solid;
                mList[planeDisplay].DrawSubset(0);

                // Draw normals
                device.SetStreamSource(0, Vb, 0);
                device.Transform.World = Matrix.Identity;
                device.VertexFormat = CustomVertex.PositionColored.Format;

                int tpd = planeDisplay;
                int mesh = 0;
                while (tpd >= coll.Meshes[mesh].SurfaceData.Length)
                {
                    tpd -= coll.Meshes[mesh++].SurfaceData.Length;
                }
                short s = (short)coll.Meshes[mesh].SurfaceData[tpd].Plane;
                if (s >= 0)
                    device.DrawPrimitives(PrimitiveType.LineList, coll.Meshes[mesh].SurfaceData[tpd].Plane * 2, 1);
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

        /// <summary>
        /// The check keys.
        /// </summary>
        /// <remarks></remarks>
        private void checkKeys()
        {
            try
            {
                keyboard.Acquire();
            }
            catch
            {
                return;
            }

            foreach (Key kk in keyboard.GetPressedKeys())
            {
                switch (kk.ToString())
                {
                    case "Space":
                        sphereDisplayAll = !sphereDisplayAll;
                        Thread.Sleep(100);
                        break;
                    case "Tab":
                        planeDisplayAll = !planeDisplayAll;
                        Thread.Sleep(100);
                        break;
                    case "Comma":
                        planeDisplay--;
                        if (planeDisplay < 0)
                        {
                            planeDisplay = mList.Count - 1;
                        }

                        Thread.Sleep(100);
                        break;
                    case "Period":
                        planeDisplay++;
                        if (planeDisplay >= mList.Count)
                        {
                            planeDisplay = 0;
                        }

                        Thread.Sleep(100);
                        break;
                }
            }
        }

        #endregion
    }
}