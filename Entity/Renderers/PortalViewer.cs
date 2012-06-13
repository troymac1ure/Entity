// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PortalViewer.cs" company="">
//   
// </copyright>
// <summary>
//   The portal viewer.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace entity.Renderers
{
    using System;
    using System.Drawing;
    using System.Threading;
    using System.Windows.Forms;

    using entity;
    using entity.Tools;

    using HaloMap.Meta;
    using HaloMap.RawData;
    using HaloMap.Render;

    using Microsoft.DirectX;
    using Microsoft.DirectX.DirectInput;

    using HaloMap;

    using Font = Microsoft.DirectX.Direct3D.Font;

    /// <summary>
    /// The portal viewer.
    /// </summary>
    /// <remarks></remarks>
    public partial class PortalViewer : Form
    {
        #region Constants and Fields

        /// <summary>
        /// The face count.
        /// </summary>
        private readonly int[] faceCount;

        /// <summary>
        /// The portals.
        /// </summary>
        private readonly Portal[] portals;

        // Our global variables for this project
        /// <summary>
        /// The render.
        /// </summary>
        private readonly Renderer render = new Renderer();

        /// <summary>
        /// The bsp.
        /// </summary>
        private BSPModel bsp;

        /// <summary>
        /// The cam.
        /// </summary>
        private Camera2 cam;

        /// <summary>
        /// The pc.
        /// </summary>
        private PortalContainer pc;

        /// <summary>
        /// The portal.
        /// </summary>
        private int portal;

        /// <summary>
        /// The show back backs.
        /// </summary>
        private bool showBackBacks = true;

        /// <summary>
        /// The show back fronts.
        /// </summary>
        private bool showBackFronts = true;

        /// <summary>
        /// The show front backs.
        /// </summary>
        private bool showFrontBacks = true;

        /// <summary>
        /// The show front fronts.
        /// </summary>
        private bool showFrontFronts = true;

        /// <summary>
        /// The text.
        /// </summary>
        private Font text;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PortalViewer"/> class.
        /// </summary>
        /// <param name="meta">The meta.</param>
        /// <remarks></remarks>
        public PortalViewer(ref Meta meta)
        {
            // Set the initial size of our form
            this.ClientSize = new Size(
                Screen.PrimaryScreen.WorkingArea.Width - 4, Screen.PrimaryScreen.WorkingArea.Height - 4);

            // And its caption
            this.Text = "Model Viewer";

            this.MouseDown += ModelViewer_MouseDown;
            this.MouseMove += this.ModelViewer_MouseDownx;
            this.MouseUp += this.ModelViewer_MouseUp;

            portals = Portals.GetPortals(ref meta);

            bsp = new BSPModel(ref meta);

            faceCount = new int[bsp.BSPRawDataMetaChunks.Length];
            for (int x = 0; x < bsp.BSPRawDataMetaChunks.Length; x++)
            {
                faceCount[x] = bsp.BSPRawDataMetaChunks[x].FaceCount;
            }

            bsp.DrawBSPPermutations = false;

            Main();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// The check keys.
        /// </summary>
        /// <remarks></remarks>
        public void CheckKeys()
        {
            Key[] keys = new Key[0];
            try
            {
                keys = cam.device.GetPressedKeys();
            }
            catch
            {
            }

            foreach (Key kk in keys)
            {
                // tslabel.Text = kk.ToString();
                switch (kk.ToString())
                {
                    case "D1":
                        showFrontFronts = !showFrontFronts;
                        Thread.Sleep(120);
                        break;
                    case "D2":
                        showFrontBacks = !showFrontBacks;
                        Thread.Sleep(120);
                        break;
                    case "D3":
                        showBackFronts = !showBackFronts;
                        Thread.Sleep(120);
                        break;
                    case "D4":
                        showBackBacks = !showBackBacks;
                        Thread.Sleep(120);
                        break;
                    case "Right":
                    case "Up":
                        portal += 1;
                        if (portal == portals.Length)
                        {
                            portal = 0;
                        }

                        Thread.Sleep(120);
                        break;
                    case "LeftArrow":
                    case "DownArrow":
                        portal -= 1;
                        if (portal == -1)
                        {
                            portal = portals.Length - 1;
                        }

                        Thread.Sleep(120);
                        break;
                }
            }
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
            render.CreateDevice(this);

            pc = new PortalContainer(portals, ref render.device);
            BSPModel.BSPDisplayedInfo.CreateVertexBuffers(ref render.device, ref bsp);
            BSPModel.BSPDisplayedInfo.CreateIndexBuffers(ref render.device, ref bsp);
            BSPModel.BSPDisplayedInfo.LoadShaderTextures(ref render.device, ref bsp);

            System.Drawing.Font systemfont = new System.Drawing.Font("Arial", 12f, FontStyle.Regular);
            text = new Font(render.device, systemfont);

            cam = new Camera2(this);
            cam.speed = 0.05f;

            // cam.fixedrotation = true;
            return true;

            // }
            // catch (DirectXException)
            // {
            // Catch any errors and return a failure
            // 	return false;
            // 	}
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// <remarks></remarks>
        public void Main()
        {
            using (PortalViewer frm = this)
            {
                if (!frm.InitializeGraphics())
                {
                    // Initialize Direct3D
                    MessageBox.Show("Could not initialize Direct3D.  This tutorial will exit.");
                    return;
                }

                frm.Show();

                // While the form is still valid, render and process messages
                while (frm.Created)
                {
                    frm.Render();
                    Application.DoEvents();
                }
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
            this.Render(); // Render on painting
        }

        /// <summary>
        /// The on resize.
        /// </summary>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        protected override void OnResize(EventArgs e)
        {
            // this.WindowState = (FormWindowState.Minimized) || !this.Visible;
            // Application.DoEvents();
            // pause = false;
            // this.Render(
        }

        /// <summary>
        /// The initialize component.
        /// </summary>
        /// <remarks></remarks>
        private void InitializeComponent()
        {
            this.SuspendLayout();

            // ModelViewer
            this.ClientSize = new Size(292, 266);
            this.Name = "ModelViewer";

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
            cam.oldx = e.X;
            cam.oldy = e.Y;
        }

        /// <summary>
        /// The model viewer_ mouse downx.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void ModelViewer_MouseDownx(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                cam.change(e.X, e.Y);
            }
        }

        /// <summary>
        /// The model viewer_ mouse up.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void ModelViewer_MouseUp(object sender, MouseEventArgs e)
        {
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

            render.BeginScene(Color.Blue);

            cam.move();
            CheckKeys();
            SetupMatrices();

            // Reset all face Counts to proper amount, then we zero out any that are drawn as portal surfaces
            for (int x = 0; x < faceCount.Length; x++)
            {
                bsp.BSPRawDataMetaChunks[x].FaceCount = faceCount[x];
            }

            text.DrawText(null, "[Left/Right] Portal #" + portal + " / " + portals.Length, new Point(5, 5), Color.White);
            text.DrawText(null, "Cluster Count", new Point(5, 30), Color.White);
            text.DrawText(null, "[1] Front front", new Point(10, 44), showFrontFronts ? Color.Red : Color.White);
            text.DrawText(
                null, 
                bsp.ClusterInfo[portals[portal].FrontCluster].FrontVisibleClusters.Count.ToString(), 
                new Point(120, 44), 
                Color.White);
            text.DrawText(null, "[2] Front back", new Point(10, 58), showFrontBacks ? Color.Purple : Color.White);
            text.DrawText(
                null, 
                bsp.ClusterInfo[portals[portal].FrontCluster].BackVisibleClusters.Count.ToString(), 
                new Point(120, 58), 
                Color.White);
            text.DrawText(null, "[3] Back front", new Point(10, 72), showBackFronts ? Color.Yellow : Color.White);
            text.DrawText(
                null, 
                bsp.ClusterInfo[portals[portal].BackCluster].FrontVisibleClusters.Count.ToString(), 
                new Point(120, 72), 
                Color.White);
            text.DrawText(null, "[4] Back back", new Point(10, 86), showBackBacks ? Color.Green : Color.White);
            text.DrawText(
                null, 
                bsp.ClusterInfo[portals[portal].BackCluster].BackVisibleClusters.Count.ToString(), 
                new Point(120, 86), 
                Color.White);

            render.device.Transform.World = Matrix.Identity;

            if (portals.Length > 0)
            {
                if (showFrontFronts)
                {
                    for (int x = 0; x < bsp.ClusterInfo[portals[portal].FrontCluster].FrontVisibleClusters.Count; x++)
                    {
                        if (
                            bsp.BSPRawDataMetaChunks[
                                bsp.ClusterInfo[portals[portal].FrontCluster].FrontVisibleClusters[x]].FaceCount > 0)
                        {
                            BSPModel.BSPDisplayedInfo.DrawCluster(
                                bsp.ClusterInfo[portals[portal].FrontCluster].FrontVisibleClusters[x], 
                                Color.Red, 
                                ref render.device, 
                                ref bsp, 
                                true, 
                                true, 
                                ref cam);
                            bsp.BSPRawDataMetaChunks[
                                bsp.ClusterInfo[portals[portal].FrontCluster].FrontVisibleClusters[x]].FaceCount = 0;
                        }
                    }
                }

                if (showFrontBacks)
                {
                    for (int x = 0; x < bsp.ClusterInfo[portals[portal].FrontCluster].BackVisibleClusters.Count; x++)
                    {
                        if (
                            bsp.BSPRawDataMetaChunks[
                                bsp.ClusterInfo[portals[portal].FrontCluster].BackVisibleClusters[x]].FaceCount > 0)
                        {
                            BSPModel.BSPDisplayedInfo.DrawCluster(
                                bsp.ClusterInfo[portals[portal].FrontCluster].BackVisibleClusters[x], 
                                Color.Purple, 
                                ref render.device, 
                                ref bsp, 
                                true, 
                                true, 
                                ref cam);
                            bsp.BSPRawDataMetaChunks[
                                bsp.ClusterInfo[portals[portal].FrontCluster].BackVisibleClusters[x]].FaceCount = 0;
                        }
                    }
                }

                if (showBackFronts)
                {
                    for (int x = 0; x < bsp.ClusterInfo[portals[portal].BackCluster].FrontVisibleClusters.Count; x++)
                    {
                        if (
                            bsp.BSPRawDataMetaChunks[
                                bsp.ClusterInfo[portals[portal].BackCluster].FrontVisibleClusters[x]].FaceCount > 0)
                        {
                            BSPModel.BSPDisplayedInfo.DrawCluster(
                                bsp.ClusterInfo[portals[portal].BackCluster].FrontVisibleClusters[x], 
                                Color.Yellow, 
                                ref render.device, 
                                ref bsp, 
                                true, 
                                true, 
                                ref cam);
                            bsp.BSPRawDataMetaChunks[
                                bsp.ClusterInfo[portals[portal].BackCluster].FrontVisibleClusters[x]].FaceCount = 0;
                        }
                    }
                }

                if (showBackBacks)
                {
                    for (int x = 0; x < bsp.ClusterInfo[portals[portal].BackCluster].BackVisibleClusters.Count; x++)
                    {
                        if (
                            bsp.BSPRawDataMetaChunks[bsp.ClusterInfo[portals[portal].BackCluster].BackVisibleClusters[x]
                                ].FaceCount > 0)
                        {
                            BSPModel.BSPDisplayedInfo.DrawCluster(
                                bsp.ClusterInfo[portals[portal].BackCluster].BackVisibleClusters[x], 
                                Color.Green, 
                                ref render.device, 
                                ref bsp, 
                                true, 
                                true, 
                                ref cam);
                            bsp.BSPRawDataMetaChunks[bsp.ClusterInfo[portals[portal].BackCluster].BackVisibleClusters[x]
                                ].FaceCount = 0;
                        }
                    }
                }
            }

            render.device.Transform.World = Matrix.Identity;
            BSPModel.BSPDisplayedInfo.Draw(ref render.device, ref bsp, true, ref cam, null);

            if (portals.Length > 0)
            {
                render.device.Transform.World = Matrix.Identity;
                pc.DrawIndex(portal, ref render.device);
            }

            render.EndScene();
        }

        /// <summary>
        /// The setup matrices.
        /// </summary>
        /// <remarks></remarks>
        private void SetupMatrices()
        {
            render.device.Transform.World = Matrix.Identity;
            render.device.Transform.View = Matrix.LookAtRH(cam.Position, cam.LookAt, cam.UpVector);
            render.device.Transform.Projection = Matrix.PerspectiveFovRH(1.33f, 1.0f, 0.1f, 10000.0f);
        }

        #endregion
    }
}