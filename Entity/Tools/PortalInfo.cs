// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Portals.cs" company="">
//   
// </copyright>
// <summary>
//   The portal.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace entity.Tools
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;

    using HaloMap.Map;
    using HaloMap.Meta;

    using Microsoft.DirectX;
    using Microsoft.DirectX.Direct3D;

    /// <summary>
    /// The portal.
    /// </summary>
    /// <remarks></remarks>
    public class Portal
    {
        #region Constants and Fields

        /// <summary>
        /// The back cluster.
        /// </summary>
        public short BackCluster;

        /// <summary>
        /// The bounding radius.
        /// </summary>
        public float BoundingRadius;

        /// <summary>
        /// The front cluster.
        /// </summary>
        public short FrontCluster;

        /// <summary>
        /// The plane.
        /// </summary>
        public int Plane;

        /// <summary>
        /// The vertices.
        /// </summary>
        public List<Vector3> Vertices = new List<Vector3>();

        /// <summary>
        /// The x.
        /// </summary>
        public float X;

        /// <summary>
        /// The y.
        /// </summary>
        public float Y;

        /// <summary>
        /// The z.
        /// </summary>
        public float Z;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Portal"/> class.
        /// </summary>
        /// <param name="BR">The BR.</param>
        /// <param name="magic">The magic.</param>
        /// <remarks></remarks>
        public Portal(ref BinaryReader BR, int magic)
        {
            FrontCluster = BR.ReadInt16();
            BackCluster = BR.ReadInt16();
            Plane = BR.ReadInt32();
            X = BR.ReadSingle();
            Y = BR.ReadSingle();
            Z = BR.ReadSingle();
            BoundingRadius = BR.ReadSingle();
            BR.ReadInt32();
            int tempc = BR.ReadInt32();
            int tempr = BR.ReadInt32() - magic;
            BR.BaseStream.Position = tempr;
            for (int x = 0; x < tempc; x++)
            {
                Vector3 v = new Vector3(BR.ReadSingle(), BR.ReadSingle(), BR.ReadSingle());
                Vertices.Add(v);
            }
        }

        #endregion
    }

    /// <summary>
    /// The portal container.
    /// </summary>
    /// <remarks></remarks>
    public class PortalContainer
    {
        #region Constants and Fields

        /// <summary>
        /// The black.
        /// </summary>
        private readonly Material Black;

        /// <summary>
        /// The default.
        /// </summary>
        private readonly Material Default;

        /// <summary>
        /// The red.
        /// </summary>
        private readonly Material Red;

        /// <summary>
        /// The red transparent.
        /// </summary>
        private readonly Material RedTransparent;

        /// <summary>
        /// The mat.
        /// </summary>
        private readonly Matrix[] mat;

        /// <summary>
        /// The spheres.
        /// </summary>
        private readonly Mesh[] spheres;

        /// <summary>
        /// The vb.
        /// </summary>
        private readonly VertexBuffer[] vb;

        /// <summary>
        /// The vertice count.
        /// </summary>
        private readonly int[] verticeCount;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PortalContainer"/> class.
        /// </summary>
        /// <param name="portals">The portals.</param>
        /// <param name="device">The device.</param>
        /// <remarks></remarks>
        public PortalContainer(Portal[] portals, ref Device device)
        {
            Default = new Material();
            Default.Diffuse = Color.White;
            Default.Ambient = Color.White;
            RedTransparent = new Material();
            RedTransparent.Diffuse = Color.FromArgb(50, 255, 0, 0);
            RedTransparent.Ambient = Color.FromArgb(50, 255, 0, 0);

            Red = new Material();
            Red.Diffuse = Color.Red;
            Red.Ambient = Color.Red;
            Black = new Material();
            Black.Diffuse = Color.Black;
            Black.Ambient = Color.Black;
            vb = new VertexBuffer[portals.Length];
            verticeCount = new int[portals.Length];
            spheres = new Mesh[portals.Length];
            mat = new Matrix[portals.Length];
            for (int x = 0; x < portals.Length; x++)
            {
                spheres[x] = Mesh.Sphere(device, portals[x].BoundingRadius, 10, 10);
                mat[x] = Matrix.Translation(portals[x].X, portals[x].Y, portals[x].Z);

                vb[x] = new VertexBuffer(
                    typeof(CustomVertex.PositionColored), 
                    portals[x].Vertices.Count, 
                    device, 
                    Usage.WriteOnly, 
                    CustomVertex.PositionColored.Format, 
                    Pool.Managed);
                CustomVertex.PositionColored[] verts = (CustomVertex.PositionColored[])vb[x].Lock(0, 0);

                // Lock the buffer (which will return our structs)
                verticeCount[x] = portals[x].Vertices.Count;
                for (int i = 0; i < portals[x].Vertices.Count; i++)
                {
                    verts[i].Position = new Vector3(
                        portals[x].Vertices[i].X, portals[x].Vertices[i].Y, portals[x].Vertices[i].Z);
                }

                vb[x].Unlock();
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// The draw.
        /// </summary>
        /// <param name="device">The device.</param>
        /// <remarks></remarks>
        public void Draw(ref Device device)
        {
            for (int i = 0; i < this.vb.Length; i++)
            {
                device.SetStreamSource(0, vb[i], 0);
                device.VertexFormat = CustomVertex.PositionColored.Format;

                device.RenderState.AlphaBlendEnable = false;
                device.RenderState.AlphaTestEnable = false;

                device.Transform.World = Matrix.Identity;
                device.SetTexture(0, null);
                device.RenderState.FillMode = FillMode.Solid;
                device.RenderState.AlphaBlendEnable = true;
                device.RenderState.AlphaTestEnable = false;
                device.Material = Red;
                PrimitiveType pt;

                pt = PrimitiveType.TriangleFan;
                device.DrawPrimitives(pt, 0, this.verticeCount[i] - 2);

                device.RenderState.AlphaBlendEnable = false;
                device.RenderState.AlphaTestEnable = false;
                device.RenderState.FillMode = FillMode.WireFrame;
                device.Material = Red;

                pt = PrimitiveType.TriangleFan;
                device.DrawPrimitives(pt, 0, this.verticeCount[i] - 2);

                device.Material = Default;
                device.Transform.World = mat[i];

                // spheres[i].DrawSubset(0);
            }
        }

        /// <summary>
        /// The draw index.
        /// </summary>
        /// <param name="portal">The portal.</param>
        /// <param name="device">The device.</param>
        /// <remarks></remarks>
        public void DrawIndex(int portal, ref Device device)
        {
            int i = portal;

            device.SetStreamSource(0, vb[i], 0);
            device.VertexFormat = CustomVertex.PositionColored.Format;

            device.RenderState.AlphaBlendEnable = false;
            device.RenderState.AlphaTestEnable = false;

            device.Transform.World = Matrix.Identity;
            device.SetTexture(0, null);
            device.RenderState.FillMode = FillMode.Solid;
            device.Material = Red;

            // device.RenderState.AlphaBlendEnable = true;
            // device.RenderState.AlphaTestEnable = true;
            PrimitiveType pt;

            pt = PrimitiveType.TriangleFan;
            device.DrawPrimitives(pt, 0, this.verticeCount[i] - 2);

            device.RenderState.AlphaBlendEnable = false;
            device.RenderState.AlphaTestEnable = false;
            device.RenderState.FillMode = FillMode.WireFrame;
            device.Material = Black;

            pt = PrimitiveType.TriangleFan;
            device.DrawPrimitives(pt, 0, this.verticeCount[i] - 2);

            device.Material = Default;
            device.Transform.World = mat[i];
            spheres[i].DrawSubset(0);
        }

        #endregion
    }

    /// <summary>
    /// The portals.
    /// </summary>
    /// <remarks></remarks>
    public sealed class Portals
    {
        #region Public Methods

        /// <summary>
        /// The get portals.
        /// </summary>
        /// <param name="meta">The meta.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static Portal[] GetPortals(ref Meta meta)
        {
            meta.Map.OpenMap(MapTypes.Internal);
            meta.Map.BR.BaseStream.Position = meta.offset + 108;
            int tempc = meta.Map.BR.ReadInt32();
            int tempr = meta.Map.BR.ReadInt32() - meta.magic;
            Portal[] temp = new Portal[tempc];
            for (int x = 0; x < tempc; x++)
            {
                meta.Map.BR.BaseStream.Position = tempr + (x * 36);
                temp[x] = new Portal(ref meta.Map.BR, meta.magic);
            }

            meta.Map.CloseMap();
            return temp;
        }

        #endregion
    }
}