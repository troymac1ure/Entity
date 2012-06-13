// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelViewer.cs" company="">
//   
// </copyright>
// <summary>
//   The model viewer.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace entity.Renderers
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    using entity;

    using HaloMap.RawData;
    using HaloMap.Render;

    using Microsoft.DirectX;
    using Microsoft.DirectX.Direct3D;

    using HaloMap;

    /// <summary>
    /// The model viewer.
    /// </summary>
    /// <remarks></remarks>
    public partial class ModelViewer : Form
    {
        #region Constants and Fields

        /// <summary>
        /// The firsttimeload.
        /// </summary>
        public bool firsttimeload = true;

        /// <summary>
        /// The render.
        /// </summary>
        private readonly Renderer render = new Renderer();

        /// <summary>
        /// The cam.
        /// </summary>
        private Camera2 cam;

        /// <summary>
        /// The m 1.
        /// </summary>
        private Material m1;

        /// <summary>
        /// The pm.
        /// </summary>
        private ParsedModel pm;

        // Our global variables for this project
        /// <summary>
        /// The use shaders.
        /// </summary>
        private bool useShaders = true;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ModelViewer"/> class.
        /// </summary>
        /// <param name="temppm">The temppm.</param>
        /// <remarks></remarks>
        public ModelViewer(ParsedModel temppm)
        {
            // Set the initial size of our form
            this.ClientSize = new Size(400, 300);

            // And its caption
            this.Text = "Model Viewer";

            pm = temppm;

            this.MouseDown += ModelViewer_MouseDown;
            this.MouseMove += this.ModelViewer_MouseDownx;
            this.MouseUp += this.ModelViewer_MouseUp;

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
            render.CreateDevice(this);

            ParsedModel.DisplayedInfo.LoadDirectXTexturesAndBuffers(ref render.device, ref pm);
            cam = new Camera2(this);
            cam.speed = 0.005f;

            cam.Position.X = 0.5741551f;
            cam.Position.Y = 0.01331316f;
            cam.Position.Z = 0.4271703f;

            cam.radianv = 6.161014f;
            cam.radianh = 3.14159f;

            cam.x = 0.5741551f;
            cam.y = 0.01331316f;
            cam.z = 0.4271703f;

            cam.ComputePosition();

            m1 = new Material();
            m1.Diffuse = Color.White;
            m1.Ambient = Color.White;
            m1.Specular = Color.White;

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
            using (ModelViewer frm = this)
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
            switch (e.KeyChar)
            {
                case (char)27: // ESC
                    this.Dispose(); // Esc was pressed
                    break;
                case '\t': // TAB
                    useShaders = !useShaders;
                    break;
                case 'i':
                case 'I':
                    MessageBox.Show(
                        "Camera Cords: X:" + cam.Position.X + " Y:" + cam.Position.Y + " Z:" + cam.Position.Z);
                    MessageBox.Show("Looking at: H:" + cam.radianh + " V:" + cam.radianv);
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
            // this.WindowState = (FormWindowState.Minimized) || !this.Visible;
            // Application.DoEvents();
            // pause = false;
            // this.Render();
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

            render.BeginScene(Color.Black);

            cam.move();
            SetupMatrices();

            render.device.RenderState.Lighting = true;
            render.device.Lights[0].Type = LightType.Directional;
            render.device.Lights[0].Ambient = Color.FromArgb(0x80, 0x80, 0x80);
            render.device.Lights[0].Diffuse = Color.FromArgb(0x80, 0x80, 0x80);
            render.device.Lights[0].Direction = new Vector3(2.5f, 5.0f, 2.5f);

            render.device.Material = m1;
            if (useShaders)
            {
                ParsedModel.DisplayedInfo.Draw(ref render.device, pm);
            }
            else
            {
                ParsedModel.DisplayedInfo.DrawMeshes(ref render.device, pm);
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