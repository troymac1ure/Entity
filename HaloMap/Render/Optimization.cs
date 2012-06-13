using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace HaloMap.Render
{
    class Optimization : IDisposable
    {
        #region fields
        private Vector3[] _frustumCorners = new Vector3[8];
        private Plane[] _frustumPlanes = new Plane[6];
        #endregion

        #region Constructors & DeConstructors
        public Optimization(Device device)
        {
            ComputeViewFrustum(device);
#if DEBUG
            System.Windows.Forms.Form f = null;
            foreach (System.Windows.Forms.Form frm in System.Windows.Forms.Application.OpenForms)
            {
                if (frm.Name == "DebugInfo")
                    f = frm;
            }
            //f.Dispose();
            //f = null;
            if (f != null)
            {
                f = new System.Windows.Forms.Form();
                f.Dock = System.Windows.Forms.DockStyle.Bottom;
                f.Name = "DebugInfo";
                System.Windows.Forms.Label l = new System.Windows.Forms.Label();
                f.Controls.AddRange(new System.Windows.Forms.Control[] {
                    new System.Windows.Forms.Label(),
                    new System.Windows.Forms.Label(),
                    new System.Windows.Forms.Label(),
                    new System.Windows.Forms.Label(),
                    new System.Windows.Forms.Label(),
                    new System.Windows.Forms.Label(),
                    new System.Windows.Forms.Label(),
                    new System.Windows.Forms.Label() });
                for (int cc = 0; cc < f.Controls.Count; cc++)
                {
                    f.Controls[cc].Location = new System.Drawing.Point(70, 25 * cc);
                    f.Controls[cc].Size = new System.Drawing.Size(180, 16);
                }
                f.Show();
            }
#endif
        }
        #endregion

        #region IDisposable
        public void Dispose()
        {
        }
        #endregion

        /// <summary>
        /// Computes the radius of a given mesh
        /// </summary>
        /// <param name="_mesh">The mesh to compute the radius of</param>
        /// <returns>The radius of the Mesh</returns>
        private float ComputeRadius(Mesh _mesh, float _scale)
        { 
            using (VertexBuffer vertexBuffer = _mesh.VertexBuffer) 
            { 
                GraphicsStream gStream = vertexBuffer.Lock(0, 0, LockFlags.None);
                Vector3 tempCenter;
                float _radius = Geometry.ComputeBoundingSphere(
                    gStream, 
                    _mesh.NumberVertices, 
                    _mesh.VertexFormat, 
                    out tempCenter) * _scale;
                vertexBuffer.Unlock();
                return _radius;
            }
        }

        private void ComputeViewFrustum(Device device)
        {
            Matrix matrix = device.Transform.View * device.Transform.Projection; // _perspectiveMatrix;
            matrix.Invert();
            _frustumCorners[0] = new Vector3(-1.0f, -1.0f, 0.0f); // xyz
            _frustumCorners[1] = new Vector3(1.0f, -1.0f, 0.0f); // Xyz
            _frustumCorners[2] = new Vector3(-1.0f, 1.0f, 0.0f); // xYz
            _frustumCorners[3] = new Vector3(1.0f, 1.0f, 0.0f); // XYz
            _frustumCorners[4] = new Vector3(-1.0f, -1.0f, 1.0f); // xyZ
            _frustumCorners[5] = new Vector3(1.0f, -1.0f, 1.0f); // XyZ
            _frustumCorners[6] = new Vector3(-1.0f, 1.0f, 1.0f); // xYZ
            _frustumCorners[7] = new Vector3(1.0f, 1.0f, 1.0f); // XYZ
            for (int i = 0; i < _frustumCorners.Length; i++)
                _frustumCorners[i] = Vector3.TransformCoordinate(_frustumCorners[i], matrix);    // Now calculate the planes
            _frustumPlanes[0] = Plane.FromPoints(
                _frustumCorners[0],
                _frustumCorners[1],
                _frustumCorners[2]); // Near
            _frustumPlanes[1] = Plane.FromPoints(
                _frustumCorners[6],
                _frustumCorners[7],
                _frustumCorners[5]); // Far
            _frustumPlanes[2] = Plane.FromPoints(
                _frustumCorners[2],
                _frustumCorners[6],
                _frustumCorners[4]); // Left
            _frustumPlanes[3] = Plane.FromPoints(
                _frustumCorners[7],
                _frustumCorners[3],
                _frustumCorners[5]); // Right
            _frustumPlanes[4] = Plane.FromPoints(
                _frustumCorners[2],
                _frustumCorners[3],
                _frustumCorners[6]); // Top
            _frustumPlanes[5] = Plane.FromPoints(
                _frustumCorners[1],
                _frustumCorners[0],
                _frustumCorners[4]); // Bottom
        }

        /// <summary>
        /// Checks if a plane is in the viewing frustrum
        /// </summary>
        /// <param name="unitToCheck">Mesh</param>
        /// <returns>True if in view, false otherwise</returns>
        public bool IsInViewFrustum(HaloMap.RawData.BSPModel.BSPRawDataMetaChunk dataChunk )
        {
            Vector3 minVector = new Vector3(dataChunk.BoundingBox.MinX, dataChunk.BoundingBox.MinY, dataChunk.BoundingBox.MinZ);
            Vector3 maxVector = new Vector3(dataChunk.BoundingBox.MaxX, dataChunk.BoundingBox.MaxY, dataChunk.BoundingBox.MaxZ);
            int x = 0;
            int y = 0;
            int z = 0;
#if DEBUG
            System.Windows.Forms.Form f = null;
            foreach (System.Windows.Forms.Form frm in System.Windows.Forms.Application.OpenForms)
            {
                if (frm.Name == "DebugInfo")
                    f = frm;
            }
            if (f != null)
            {
                f.Controls[0].Text =
                    minVector.X.ToString().PadLeft(10, ' ') + ", " +
                    minVector.Y.ToString().PadLeft(10, ' ') + ", " +
                    minVector.Y.ToString().PadLeft(10, ' ');
                f.Controls[1].Text =
                    maxVector.X.ToString().PadLeft(10, ' ') + ", " +
                    maxVector.Y.ToString().PadLeft(10, ' ') + ", " +
                    maxVector.Y.ToString().PadLeft(10, ' ');
            }
#endif

            for (int count = 0; count < _frustumPlanes.Length; count++ )
            {
                Plane plane = _frustumPlanes[count];

                float a = plane.A * minVector.X + plane.B * minVector.Y + plane.C * minVector.Z + plane.D;
                float b = plane.A * maxVector.X + plane.B * maxVector.Y + plane.C * maxVector.Z + plane.D;
#if DEBUG
                if (f != null)
                {
                    switch (count)
                    {
                        // Z
                        case 0:
                        case 1:
                            f.Controls[6 + count].Text = a.ToString().PadLeft(12, ' ') + " : " + b.ToString().PadRight(12, ' ');
                            break;
                        // X
                        case 2:
                        case 3:
                        // Y
                        case 4:
                        case 5:
                            f.Controls[count].Text = a.ToString().PadLeft(12, ' ') + " : " + b.ToString().PadRight(12, ' ');
                            break;
                    }
                }

#endif
                if (a > 2 && b > 2)
                    switch (count)
                    {
                        case 0:
                            z--;
                            break;
                        case 1:
                            z++;
                            break;
                        case 2:
                            x--;
                            break;
                        case 3:
                            x++;
                            break;
                        case 4:
                            y--;
                            break;
                        case 5:
                            y++;
                            break;
                    }
                    //<= (-unitToCheck.Radius))
            }
            if (x != 0 || y != 0 || z != 0)
                return false;
             
            return true; 
        }
    }
}
