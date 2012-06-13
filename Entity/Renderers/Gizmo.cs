// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Gizmo.cs" company="">
//   
// </copyright>
// <summary>
//   The gizmo.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace entity.Renderers
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.Windows.Forms;

    using Microsoft.DirectX;
    using Microsoft.DirectX.Direct3D;

    using Font = System.Drawing.Font;

    /// <summary>
    /// The gizmo.
    /// </summary>
    /// <remarks></remarks>
    internal class Gizmo
    {
        #region Constants and Fields

        /// <summary>
        /// The device.
        /// </summary>
        private readonly Device device;

        /// <summary>
        /// The fnt.
        /// </summary>
        private readonly Font fnt = new Font("Arial", 12);

        /// <summary>
        /// The font.
        /// </summary>
        private readonly Microsoft.DirectX.Direct3D.Font font;

        /// <summary>
        /// The gizmo.
        /// </summary>
        private Mesh gizmo;

        /// <summary>
        /// The scale.
        /// </summary>
        private float scale = 1.0f;

        /// <summary>
        /// The selected axis.
        /// </summary>
        private axis selectedAxis = axis.none;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Gizmo"/> class.
        /// </summary>
        /// <param name="device">The device.</param>
        /// <remarks></remarks>
        public Gizmo(Device device)
        {
            this.device = device;
            createMovementGizmo();
            this.device.RenderState.ZBufferEnable = true;
            font = new Microsoft.DirectX.Direct3D.Font(device, fnt);
        }

        #endregion

        #region Enums

        /// <summary>
        /// The axis.
        /// </summary>
        /// <remarks></remarks>
        public enum axis
        {
            /// <summary>
            /// The none.
            /// </summary>
            none, 

            /// <summary>
            /// The x.
            /// </summary>
            X, 

            /// <summary>
            /// The y.
            /// </summary>
            Y, 

            /// <summary>
            /// The z.
            /// </summary>
            Z, 

            /// <summary>
            /// The xy.
            /// </summary>
            XY, 

            /// <summary>
            /// The xz.
            /// </summary>
            XZ, 

            /// <summary>
            /// The yz.
            /// </summary>
            YZ
        }

        /// <summary>
        /// The transform.
        /// </summary>
        /// <remarks></remarks>
        public enum transform
        {
            /// <summary>
            /// The movement.
            /// </summary>
            movement, 

            /// <summary>
            /// The rotation.
            /// </summary>
            rotation, 

            /// <summary>
            /// The scale.
            /// </summary>
            scale
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Checks for intersection with a given mouse point. Pass the results from a call to System.Windows.Forms.MouseEventArgs()
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        /// <param name="mat">The mat.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public axis checkForIntersection(MouseEventArgs e, Matrix mat)
        {
            List<int> temp = new List<int>();
            temp = MeshPick(e.X, e.Y, this.gizmo, mat);
            if (temp.Count > 0)
            {
                switch (temp[0])
                {
                    case 0:
                    case 3:
                        this.selectedAxis = axis.X;
                        break;
                    case 1:
                    case 4:
                        this.selectedAxis = axis.Y;
                        break;
                    case 2:
                    case 5:
                        this.selectedAxis = axis.Z;
                        break;
                    case 6:
                        this.selectedAxis = axis.XY;
                        break;
                    case 7:
                        this.selectedAxis = axis.YZ;
                        break;
                    case 8:
                        this.selectedAxis = axis.XZ;
                        break;
                    default:
                        this.selectedAxis = axis.none;
                        break;
                }
            }
            else
            {
                this.selectedAxis = axis.none;
            }

            /*
            for (int x = 0; x < this.gizmo.Count; x++)
            {
                //check bitmask for object visibility

                    // Check under mouse cursor for object selection/deselection?
                }
            }
            */
            return this.selectedAxis;
        }

        /// <summary>
        /// The draw.
        /// </summary>
        /// <param name="scale">The scale.</param>
        /// <remarks></remarks>
        public void draw(float scale)
        {
            // Store current world matrix
            Matrix mat = device.Transform.World;

            // Set the world matrix to our desires for our gizmo
            device.Transform.World = Matrix.Scaling(scale, scale, scale) * mat;

            FillMode oldFill = device.RenderState.FillMode;
            device.RenderState.FillMode = FillMode.Solid;
            Cull oldCull = device.RenderState.CullMode;
            device.RenderState.CullMode = Cull.None;

            CustomVertex.PositionColored[] vertices = new CustomVertex.PositionColored[18];

            Color c1 = Color.Red;
            Color c2 = Color.Red;
            Color c3 = Color.Red;
            if (this.selectedAxis == axis.X)
            {
                c1 = Color.Yellow;
            }

            if (this.selectedAxis == axis.XY)
            {
                c1 = Color.Yellow;
                c2 = Color.Yellow;
            }

            if (this.selectedAxis == axis.XZ)
            {
                c1 = Color.Yellow;
                c3 = Color.Yellow;
            }

            vertices[0].Color = c1.ToArgb();
            vertices[1].Color = c1.ToArgb();
            vertices[2].Color = c2.ToArgb();
            vertices[3].Color = c2.ToArgb();
            vertices[4].Color = c3.ToArgb();
            vertices[5].Color = c3.ToArgb();
            vertices[0].Position = new Vector3(0f, 0f, 0f);
            vertices[1].Position = new Vector3(10f, 0f, 0f);
            vertices[2].Position = new Vector3(5f, 0f, 0f);
            vertices[3].Position = new Vector3(5f, 5f, 0f);
            vertices[4].Position = new Vector3(5f, 0f, 0f);
            vertices[5].Position = new Vector3(5f, 0f, 5f);
            Vector3 pos = new Vector3(13f, 1f, 0f);
            Vector3 plot2d = Vector3.Project(
                pos, this.device.Viewport, device.Transform.Projection, device.Transform.View, device.Transform.World);

            // Need to render in 3D for ZBuffer removal
            font.DrawText(null, "x", new Point((int)plot2d.X, (int)plot2d.Y), c1);

            c1 = Color.Green;
            c2 = Color.Green;
            c3 = Color.Green;
            if (this.selectedAxis == axis.Y)
            {
                c1 = Color.Yellow;
            }

            if (this.selectedAxis == axis.XY)
            {
                c1 = Color.Yellow;
                c2 = Color.Yellow;
            }

            if (this.selectedAxis == axis.YZ)
            {
                c1 = Color.Yellow;
                c3 = Color.Yellow;
            }

            vertices[6].Color = c1.ToArgb();
            vertices[7].Color = c1.ToArgb();
            vertices[8].Color = c2.ToArgb();
            vertices[9].Color = c2.ToArgb();
            vertices[10].Color = c3.ToArgb();
            vertices[11].Color = c3.ToArgb();
            vertices[6].Position = new Vector3(0f, 0f, 0f);
            vertices[7].Position = new Vector3(0f, 10f, 0f);
            vertices[8].Position = new Vector3(0f, 5f, 0f);
            vertices[9].Position = new Vector3(5f, 5f, 0f);
            vertices[10].Position = new Vector3(0f, 5f, 0f);
            vertices[11].Position = new Vector3(0f, 5f, 5f);
            pos = new Vector3(1f, 13f, 0f);
            plot2d = Vector3.Project(
                pos, this.device.Viewport, device.Transform.Projection, device.Transform.View, device.Transform.World);
            font.DrawText(null, "y", new Point((int)plot2d.X, (int)plot2d.Y), c1);

            c1 = Color.Blue;
            c2 = Color.Blue;
            c3 = Color.Blue;
            if (this.selectedAxis == axis.Z)
            {
                c1 = Color.Yellow;
            }

            if (this.selectedAxis == axis.XZ)
            {
                c1 = Color.Yellow;
                c2 = Color.Yellow;
            }

            if (this.selectedAxis == axis.YZ)
            {
                c1 = Color.Yellow;
                c3 = Color.Yellow;
            }

            vertices[12].Color = c1.ToArgb();
            vertices[13].Color = c1.ToArgb();
            vertices[14].Color = c2.ToArgb();
            vertices[15].Color = c2.ToArgb();
            vertices[16].Color = c3.ToArgb();
            vertices[17].Color = c3.ToArgb();
            vertices[12].Position = new Vector3(0f, 0f, 0f);
            vertices[13].Position = new Vector3(0f, 0f, 10f);
            vertices[14].Position = new Vector3(0f, 0f, 5f);
            vertices[15].Position = new Vector3(5f, 0f, 5f);
            vertices[16].Position = new Vector3(0f, 0f, 5f);
            vertices[17].Position = new Vector3(0f, 5f, 5f);
            pos = new Vector3(1f, 0f, 13f);
            plot2d = Vector3.Project(
                pos, this.device.Viewport, device.Transform.Projection, device.Transform.View, device.Transform.World);
            font.DrawText(null, "z", new Point((int)plot2d.X, (int)plot2d.Y), c1);

            device.VertexFormat = CustomVertex.PositionColored.Format;
            device.DrawUserPrimitives(PrimitiveType.LineList, 9, vertices);

            #region Drawing axis cones

            this.gizmo.DrawSubset(0); // X-Axis
            this.gizmo.DrawSubset(1); // Y-Axis
            this.gizmo.DrawSubset(2); // Z-Axis

            #endregion

            this.gizmo.DrawSubset(3); // Z-Axis
            this.gizmo.DrawSubset(4); // Z-Axis
            this.gizmo.DrawSubset(5); // Z-Axis

            /*
            for (int i = 3; i < 9; i++)
                this.gizmo.DrawSubset(i);
            */

            // Restore previous world matrix
            device.RenderState.FillMode = oldFill;
            device.RenderState.CullMode = oldCull;
            device.Transform.World = mat;
            this.scale = scale;
        }

        #endregion

        #region Methods

        /// <summary>
        /// The mesh pick.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="mesh">The mesh.</param>
        /// <param name="mat">The mat.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        private List<int> MeshPick(float x, float y, Mesh mesh, Matrix mat)
        {
            Vector3 s = Vector3.Unproject(
                new Vector3(x, y, 0), 
                device.Viewport, 
                device.Transform.Projection, 
                device.Transform.View, 
                Matrix.Scaling(scale, scale, scale) * mat);

            Vector3 d = Vector3.Unproject(
                new Vector3(x, y, 1), 
                device.Viewport, 
                device.Transform.Projection, 
                device.Transform.View, 
                Matrix.Scaling(scale, scale, scale) * mat);

            Vector3 rPosition = s;
            Vector3 rDirection = Vector3.Normalize(d - s);

            List<int> temp = new List<int>();
            for (int i = 0; i < 9; i++)
            {
                if (mesh.IntersectSubset(i, rPosition, rDirection))
                {
                    temp.Add(i);
                }
            }

            return temp;
        }

        /// <summary>
        /// The create movement gizmo.
        /// </summary>
        /// <remarks></remarks>
        private void createMovementGizmo()
        {
            int eachVLength = 17; // 9
            int eachILength = 24;
            Mesh m = new Mesh(
                (6 * eachILength + 18) / 3, 
                3 * eachVLength + 7, 
                MeshFlags.Managed, 
                CustomVertex.PositionColored.Format, 
                this.device);

            CustomVertex.PositionColored[] vertices = new CustomVertex.PositionColored[eachVLength * 3 + 7];
            for (int i = 0; i < 3; i++)
            {
                Color tempColor = i == 0 ? Color.DarkRed : i == 1 ? Color.DarkGreen : Color.DarkBlue;
                for (int ii = 1; ii < eachVLength; ii++)
                {
                    vertices[i * eachVLength + ii].Color = tempColor.ToArgb();
                }

                vertices[i * eachVLength + 0].Color = tempColor.ToArgb();
            }

            // X Cone (Red)
            vertices[0 * eachVLength + 0].Position = new Vector3(13f, 0f, 0f);
            vertices[0 * eachVLength + 1].Position = new Vector3(10f, 0f, 1f);
            vertices[0 * eachVLength + 2].Position = new Vector3(10f, 0.7f, 0.7f);
            vertices[0 * eachVLength + 3].Position = new Vector3(10f, 1.0f, 0f);
            vertices[0 * eachVLength + 4].Position = new Vector3(10f, 0.7f, -0.7f);
            vertices[0 * eachVLength + 5].Position = new Vector3(10f, 0f, -1.0f);
            vertices[0 * eachVLength + 6].Position = new Vector3(10f, -0.7f, -0.7f);
            vertices[0 * eachVLength + 7].Position = new Vector3(10f, -1.0f, 0f);
            vertices[0 * eachVLength + 8].Position = new Vector3(10f, -0.7f, 0.7f);

            // X Top Shaft (Not Visible, for mouse intersect only!)
            vertices[0 * eachVLength + 9].Position = new Vector3(5f, -0.25f, 0.25f);
            vertices[0 * eachVLength + 10].Position = new Vector3(5f, 0.25f, 0.25f);
            vertices[0 * eachVLength + 11].Position = new Vector3(5f, 0.25f, -0.25f);
            vertices[0 * eachVLength + 12].Position = new Vector3(5f, -0.25f, -0.25f);
            vertices[0 * eachVLength + 13].Position = new Vector3(10f, -0.25f, 0.25f);
            vertices[0 * eachVLength + 14].Position = new Vector3(10f, 0.25f, 0.25f);
            vertices[0 * eachVLength + 15].Position = new Vector3(10f, 0.25f, -0.25f);
            vertices[0 * eachVLength + 16].Position = new Vector3(10f, -0.25f, -0.25f);

            // Y Cone (Green)
            vertices[1 * eachVLength + 0].Position = new Vector3(0f, 13f, 0f);
            vertices[1 * eachVLength + 1].Position = new Vector3(0f, 10f, 1f);
            vertices[1 * eachVLength + 2].Position = new Vector3(0.7f, 10f, 0.7f);
            vertices[1 * eachVLength + 3].Position = new Vector3(1.0f, 10f, 0f);
            vertices[1 * eachVLength + 4].Position = new Vector3(0.7f, 10f, -0.7f);
            vertices[1 * eachVLength + 5].Position = new Vector3(0f, 10f, -1.0f);
            vertices[1 * eachVLength + 6].Position = new Vector3(-0.7f, 10f, -0.7f);
            vertices[1 * eachVLength + 7].Position = new Vector3(-1.0f, 10f, 0f);
            vertices[1 * eachVLength + 8].Position = new Vector3(-0.7f, 10f, 0.7f);

            // Y Top Shaft (Not Visible)
            vertices[1 * eachVLength + 9].Position = new Vector3(-0.25f, 5f, 0.25f);
            vertices[1 * eachVLength + 10].Position = new Vector3(0.25f, 5f, 0.25f);
            vertices[1 * eachVLength + 11].Position = new Vector3(0.25f, 5f, -0.25f);
            vertices[1 * eachVLength + 12].Position = new Vector3(-0.25f, 5f, -0.25f);
            vertices[1 * eachVLength + 13].Position = new Vector3(-0.25f, 10f, 0.25f);
            vertices[1 * eachVLength + 14].Position = new Vector3(0.25f, 10f, 0.25f);
            vertices[1 * eachVLength + 15].Position = new Vector3(0.25f, 10f, -0.25f);
            vertices[1 * eachVLength + 16].Position = new Vector3(-0.25f, 10f, -0.25f);

            // Z Cone (Blue)
            vertices[2 * eachVLength + 0].Position = new Vector3(0f, 0f, 13f);
            vertices[2 * eachVLength + 1].Position = new Vector3(0f, 1.0f, 10f);
            vertices[2 * eachVLength + 2].Position = new Vector3(0.7f, 0.7f, 10f);
            vertices[2 * eachVLength + 3].Position = new Vector3(1.0f, 0.0f, 10f);
            vertices[2 * eachVLength + 4].Position = new Vector3(0.7f, -0.7f, 10f);
            vertices[2 * eachVLength + 5].Position = new Vector3(0f, -1.0f, 10f);
            vertices[2 * eachVLength + 6].Position = new Vector3(-0.7f, -0.7f, 10f);
            vertices[2 * eachVLength + 7].Position = new Vector3(-1.0f, 0.0f, 10f);
            vertices[2 * eachVLength + 8].Position = new Vector3(-0.7f, 0.7f, 10f);

            // Z Top Shaft (Not Visible)
            vertices[2 * eachVLength + 9].Position = new Vector3(-0.25f, 0.25f, 5f);
            vertices[2 * eachVLength + 10].Position = new Vector3(0.25f, 0.25f, 5f);
            vertices[2 * eachVLength + 11].Position = new Vector3(0.25f, -0.25f, 5f);
            vertices[2 * eachVLength + 12].Position = new Vector3(-0.25f, -0.25f, 5f);
            vertices[2 * eachVLength + 13].Position = new Vector3(-0.25f, 0.25f, 10f);
            vertices[2 * eachVLength + 14].Position = new Vector3(0.25f, 0.25f, 10f);
            vertices[2 * eachVLength + 15].Position = new Vector3(0.25f, -0.25f, 10f);
            vertices[2 * eachVLength + 16].Position = new Vector3(-0.25f, -0.25f, 10f);

            // X Bottom Square Basic
            /*// Debugging only. These are NOT visible!
            for (int i = 0; i < 7; i++)
                vertices[3 * eachVLength + i].Color = Color.Yellow.ToArgb();
            */
            vertices[3 * eachVLength + 0].Position = new Vector3(0f, 0f, 0f);
            vertices[3 * eachVLength + 1].Position = new Vector3(5f, 0f, 0f);
            vertices[3 * eachVLength + 2].Position = new Vector3(0f, 5f, 0f);
            vertices[3 * eachVLength + 3].Position = new Vector3(0f, 0f, 5f);
            vertices[3 * eachVLength + 4].Position = new Vector3(5f, 5f, 0f);
            vertices[3 * eachVLength + 5].Position = new Vector3(5f, 0f, 5f);
            vertices[3 * eachVLength + 6].Position = new Vector3(0f, 5f, 5f);

            m.SetVertexBufferData(vertices, LockFlags.None);

            #region Indices Declaration

            short[] indices = new short[eachILength * 6 + 18];
            for (int i = 0; i < 3; i++)
            {
                // Cone
                indices[i * eachILength + 0] = (short)(i * eachVLength + 0);
                indices[i * eachILength + 1] = (short)(i * eachVLength + 1);
                indices[i * eachILength + 2] = (short)(i * eachVLength + 2);

                indices[i * eachILength + 3] = (short)(i * eachVLength + 0);
                indices[i * eachILength + 4] = (short)(i * eachVLength + 2);
                indices[i * eachILength + 5] = (short)(i * eachVLength + 3);

                indices[i * eachILength + 6] = (short)(i * eachVLength + 0);
                indices[i * eachILength + 7] = (short)(i * eachVLength + 3);
                indices[i * eachILength + 8] = (short)(i * eachVLength + 4);

                indices[i * eachILength + 9] = (short)(i * eachVLength + 0);
                indices[i * eachILength + 10] = (short)(i * eachVLength + 4);
                indices[i * eachILength + 11] = (short)(i * eachVLength + 5);

                indices[i * eachILength + 12] = (short)(i * eachVLength + 0);
                indices[i * eachILength + 13] = (short)(i * eachVLength + 5);
                indices[i * eachILength + 14] = (short)(i * eachVLength + 6);

                indices[i * eachILength + 15] = (short)(i * eachVLength + 0);
                indices[i * eachILength + 16] = (short)(i * eachVLength + 6);
                indices[i * eachILength + 17] = (short)(i * eachVLength + 7);

                indices[i * eachILength + 18] = (short)(i * eachVLength + 0);
                indices[i * eachILength + 19] = (short)(i * eachVLength + 7);
                indices[i * eachILength + 20] = (short)(i * eachVLength + 8);

                indices[i * eachILength + 21] = (short)(i * eachVLength + 0);
                indices[i * eachILength + 22] = (short)(i * eachVLength + 8);
                indices[i * eachILength + 23] = (short)(i * eachVLength + 1);
            }

            for (int i = 3; i < 6; i++)
            {
                // Shaft
                indices[i * eachILength + 0] = (short)((i - 3) * eachVLength + 9);
                indices[i * eachILength + 1] = (short)((i - 3) * eachVLength + 13);
                indices[i * eachILength + 2] = (short)((i - 3) * eachVLength + 10);
                indices[i * eachILength + 3] = (short)((i - 3) * eachVLength + 10);
                indices[i * eachILength + 4] = (short)((i - 3) * eachVLength + 13);
                indices[i * eachILength + 5] = (short)((i - 3) * eachVLength + 14);

                indices[i * eachILength + 6] = (short)((i - 3) * eachVLength + 10);
                indices[i * eachILength + 7] = (short)((i - 3) * eachVLength + 14);
                indices[i * eachILength + 8] = (short)((i - 3) * eachVLength + 11);
                indices[i * eachILength + 9] = (short)((i - 3) * eachVLength + 11);
                indices[i * eachILength + 10] = (short)((i - 3) * eachVLength + 14);
                indices[i * eachILength + 11] = (short)((i - 3) * eachVLength + 15);

                indices[i * eachILength + 12] = (short)((i - 3) * eachVLength + 11);
                indices[i * eachILength + 13] = (short)((i - 3) * eachVLength + 15);
                indices[i * eachILength + 14] = (short)((i - 3) * eachVLength + 12);
                indices[i * eachILength + 15] = (short)((i - 3) * eachVLength + 12);
                indices[i * eachILength + 16] = (short)((i - 3) * eachVLength + 15);
                indices[i * eachILength + 17] = (short)((i - 3) * eachVLength + 16);

                indices[i * eachILength + 18] = (short)((i - 3) * eachVLength + 12);
                indices[i * eachILength + 19] = (short)((i - 3) * eachVLength + 16);
                indices[i * eachILength + 20] = (short)((i - 3) * eachVLength + 13);
                indices[i * eachILength + 21] = (short)((i - 3) * eachVLength + 12);
                indices[i * eachILength + 22] = (short)((i - 3) * eachVLength + 13);
                indices[i * eachILength + 23] = (short)((i - 3) * eachVLength + 9);
            }

            // Squares
            indices[6 * eachILength + 0] = (short)(3 * eachVLength + 0);
            indices[6 * eachILength + 1] = (short)(3 * eachVLength + 1);
            indices[6 * eachILength + 2] = (short)(3 * eachVLength + 4);
            indices[6 * eachILength + 3] = (short)(3 * eachVLength + 0);
            indices[6 * eachILength + 4] = (short)(3 * eachVLength + 2);
            indices[6 * eachILength + 5] = (short)(3 * eachVLength + 4);

            indices[6 * eachILength + 6] = (short)(3 * eachVLength + 0);
            indices[6 * eachILength + 7] = (short)(3 * eachVLength + 2);
            indices[6 * eachILength + 8] = (short)(3 * eachVLength + 6);
            indices[6 * eachILength + 9] = (short)(3 * eachVLength + 0);
            indices[6 * eachILength + 10] = (short)(3 * eachVLength + 3);
            indices[6 * eachILength + 11] = (short)(3 * eachVLength + 6);

            indices[6 * eachILength + 12] = (short)(3 * eachVLength + 0);
            indices[6 * eachILength + 13] = (short)(3 * eachVLength + 3);
            indices[6 * eachILength + 14] = (short)(3 * eachVLength + 5);
            indices[6 * eachILength + 15] = (short)(3 * eachVLength + 0);
            indices[6 * eachILength + 16] = (short)(3 * eachVLength + 1);
            indices[6 * eachILength + 17] = (short)(3 * eachVLength + 5);

            m.SetIndexBufferData(indices, LockFlags.None);

            #endregion

            int[] attr = m.LockAttributeBufferArray(LockFlags.Discard);
            int step = eachILength / 3; // 1 face = 3 indices
            for (int i = 0; i < step; i++)
            {
                attr[step * 0 + i] = 0;
                attr[step * 1 + i] = 1;
                attr[step * 2 + i] = 2;
                attr[step * 3 + i] = 3;
                attr[step * 4 + i] = 4;
                attr[step * 5 + i] = 5;
            }

            for (int i = step * 6; i < attr.Length; i++)
            {
                attr[i] = 6 + (i - step * 6) / 2;
            }

            m.UnlockAttributeBuffer(attr);
            int[] adj = new int[m.NumberFaces * 3];
            m.GenerateAdjacency(0.001f, adj);
            m.OptimizeInPlace(MeshFlags.OptimizeVertexCache, adj);
            this.gizmo = m;
        }

        /// <summary>
        /// The set gizmo.
        /// </summary>
        /// <param name="tForm">The t form.</param>
        /// <remarks></remarks>
        private void setGizmo(transform tForm)
        {
            switch (tForm)
            {
                case transform.movement:
                    createMovementGizmo();
                    break;
                case transform.rotation:
                    break;
                case transform.scale:
                    break;
            }
        }

        #endregion
    }
}