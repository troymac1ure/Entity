// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LightmapViewer.cs" company="">
//   
// </copyright>
// <summary>
//   The lightmap viewer.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace entity.Renderers
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Threading;
    using System.Windows.Forms;

    using entity;

    using HaloMap.Meta;
    using HaloMap.RawData;
    using HaloMap.Render;

    using Microsoft.DirectX;
    using Microsoft.DirectX.Direct3D;
    using Microsoft.DirectX.DirectInput;

    using HaloMap;

    using Font = Microsoft.DirectX.Direct3D.Font;

    /// <summary>
    /// The lightmap viewer.
    /// </summary>
    /// <remarks></remarks>
    public partial class LightmapViewer : Form
    {
        #region Constants and Fields

        /// <summary>
        /// The face count.
        /// </summary>
        private readonly int[] faceCount;

        /// <summary>
        /// The render.
        /// </summary>
        private readonly Renderer render = new Renderer();

        /// <summary>
        /// The bsp.
        /// </summary>
        private BSPModel bsp;

        /// <summary>
        /// The bsp meshes.
        /// </summary>
        private Mesh[] bspMeshes;

        /// <summary>
        /// The cam.
        /// </summary>
        private Camera2 cam;

        /// <summary>
        /// A list of all the polygons making up the collision mesh
        /// </summary>
        List<polygonInfo> polygons = new List<polygonInfo>();

        /// <summary>
        /// The currently selected surface (surface editing mode)
        /// </summary>
        int currentSurface = -1;

        /// <summary>
        /// The text.
        /// </summary>
        private Font text;

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
        /// Initializes a new instance of the <see cref="LightmapViewer"/> class.
        /// </summary>
        /// <param name="meta">The meta.</param>
        /// <remarks></remarks>
        public LightmapViewer(ref Meta meta)
        {
            // Set the initial size of our form
            this.ClientSize = new Size(
                Screen.PrimaryScreen.WorkingArea.Width - 4, Screen.PrimaryScreen.WorkingArea.Height - 4);

            // And its caption
            this.Text = "Model Viewer";

            this.MouseDown += ModelViewer_MouseDown;
            this.MouseMove += this.ModelViewer_MouseDownx;
            this.MouseUp += this.ModelViewer_MouseUp;

            bsp = new BSPModel(ref meta);

            bsp.LoadLightmaps();

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
                    case "Tab":
                        bsp.RenderBSPLighting = !bsp.RenderBSPLighting;
                        Thread.Sleep(120);
                        break;
                    case "Right":
                    case "Up":

                        // portal += 1;
                        // if (portal == portals.Length) { portal = 0; }
                        Thread.Sleep(120);
                        break;
                    case "LeftArrow":
                    case "DownArrow":

                        // portal -= 1;
                        // if (portal == -1) { portal = portals.Length - 1; }
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

            //pc = new LightmapContainer(null, ref render.device);
            BSPModel.BSPDisplayedInfo.CreateVertexBuffers(ref render.device, ref bsp);
            BSPModel.BSPDisplayedInfo.CreateIndexBuffers(ref render.device, ref bsp);
            BSPModel.BSPDisplayedInfo.LoadShaderTextures(ref render.device, ref bsp);
            BSPModel.BSPDisplayedInfo.LoadLightmapTextures(ref render.device, ref bsp);

            System.Drawing.Font systemfont = new System.Drawing.Font("Arial", 12f, FontStyle.Regular);
            text = new Font(render.device, systemfont);

            cam = new Camera2(this);
            cam.speed = 0.30f;

            // cam.fixedrotation = true;
            bspMeshes = new Mesh[bsp.BSPPermutationRawDataMetaChunks.Length];
            //GraphicsStream vertexData;

            for (int x = 0; x < bsp.BSPPermutationRawDataMetaChunks.Length; x++)
            {
                BSPModel.BSPPermutationRawDataMetaChunk tempChunk = bsp.BSPPermutationRawDataMetaChunks[x];

                // Compute the bounding box for a mesh.
                // VertexBufferDescription description = bsp.Display.vertexBuffer[x].Description;
                // vertexData = bsp.Display.vertexBuffer[x].Lock(0, 0, LockFlags.ReadOnly);
                /*
                Geometry.ComputeBoundingBox(vertexData,
                    meshes[i].NumberVertices, description.VertexFormat,
                    out meshBoundingBoxMinValues[i],
                    out meshBoundingBoxMaxValues[i]);
                bsp.Display.vertexBuffer[x].Unlock();
                bspMeshes[x] = new Mesh(tempChunk.FaceCount, tempChunk.VerticeCount, MeshFlags.Dynamic, bsp.Display.vertexBuffer[x], render.device);
            //                bspMeshes[x] = Mesh.Box(render.device, .BoundingBox.MaxX - bsp.BSPPermutationRawDataMetaChunks[x].BoundingBox.MinX,
            //                                                       bsp.BSPPermutationRawDataMetaChunks[x].BoundingBox.MaxY - bsp.BSPPermutationRawDataMetaChunks[x].BoundingBox.MinY,
            //                                                       bsp.BSPPermutationRawDataMetaChunks[x].BoundingBox.MaxZ - bsp.BSPPermutationRawDataMetaChunks[x].BoundingBox.MinZ);
                */
            }

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
            using (LightmapViewer frm = this)
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
            if (e.Button == MouseButtons.Left)
            {
                #region Check for collision intersection of Surfaces
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

                for (int x = 0; x < bsp.BSPRawDataMetaChunks.Length; x++)
                {
                    //check for intersection
                    IntersectInformation ii;
                    for (int xx = 0; xx < bsp.BSPRawDataMetaChunks[x].Indices.Length; xx += 3)
                    {

                        Geometry.IntersectTri(
                            bsp.BSPRawDataMetaChunks[x].Vertices[bsp.BSPRawDataMetaChunks[x].Indices[xx + 0]],
                            bsp.BSPRawDataMetaChunks[x].Vertices[bsp.BSPRawDataMetaChunks[x].Indices[xx + 1]],
                            bsp.BSPRawDataMetaChunks[x].Vertices[bsp.BSPRawDataMetaChunks[x].Indices[xx + 2]],
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
                            currentSurface = (x << 16) + xx;
                            break;
                        };
                    }
                    //updateInfo();
                }

                #endregion

            }

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

            // Reset all face Counts to proper amount, then we zero out any that are drawn as Lightmap surfaces
            for (int x = 0; x < faceCount.Length; x++)
            {
                bsp.BSPRawDataMetaChunks[x].FaceCount = faceCount[x];
            }

            /*
            text.DrawText(null, "[Left/Right] Portal #" + portal.ToString() + " / " + portals.Length.ToString(), new Point(5, 5), Color.White);
            text.DrawText(null, "Cluster Count", new Point(5, 30), Color.White);
            text.DrawText(null, "[1] Front front", new Point(10, 44), showFrontFronts ? Color.Red : Color.White);
            text.DrawText(null, bsp.ClusterInfo[portals[portal].FrontCluster].FrontVisibleClusters.Count.ToString(), new Point(120, 44), Color.White);
            text.DrawText(null, "[2] Front back", new Point(10, 58), showFrontBacks ? Color.Purple : Color.White);
            text.DrawText(null, bsp.ClusterInfo[portals[portal].FrontCluster].BackVisibleClusters.Count.ToString(), new Point(120, 58), Color.White);
            text.DrawText(null, "[3] Back front", new Point(10, 72), showBackFronts ? Color.Yellow : Color.White);
            text.DrawText(null, bsp.ClusterInfo[portals[portal].BackCluster].FrontVisibleClusters.Count.ToString(), new Point(120, 72), Color.White);
            text.DrawText(null, "[4] Back back", new Point(10, 86), showBackBacks ? Color.Green : Color.White);
            text.DrawText(null, bsp.ClusterInfo[portals[portal].BackCluster].BackVisibleClusters.Count.ToString(), new Point(120, 86), Color.White);
            */
            render.device.Transform.World = Matrix.Identity;
            BSPModel.BSPDisplayedInfo.Draw(ref render.device, ref bsp, false, ref cam, null);

            render.device.Transform.World = Matrix.Identity;

            // pc.DrawIndex(lightmap, ref render.device);
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