// --------------------------------------------------------------------------------------------------------------------
// <copyright file="coll.cs" company="">
//   
// </copyright>
// <summary>
//   The coll.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HaloMap.H2MetaContainers
{
    using System.Collections.Generic;
    using System.IO;

    using HaloMap.Meta;
    using HaloMap.RawData;

    using Microsoft.DirectX;
    using Microsoft.DirectX.Direct3D;

    /// <summary>
    /// The coll.
    /// </summary>
    /// <remarks></remarks>
    public class coll
    {
        #region Constants and Fields

        /// <summary>
        /// The condition strings.
        /// </summary>
        public List<string> ConditionStrings = new List<string>();

        /// <summary>
        /// The meshes.
        /// </summary>
        public List<CollisionMesh> Meshes = new List<CollisionMesh>();

        /// <summary>
        /// The spheres.
        /// </summary>
        public List<PathfindingSphere> Spheres = new List<PathfindingSphere>();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="coll"/> class.
        /// </summary>
        /// <param name="meta">The meta.</param>
        /// <remarks></remarks>
        public coll(ref Meta meta)
        {
            BinaryReader BR = new BinaryReader(meta.MS);
            BR.BaseStream.Position = 28;
            int tempc = BR.ReadInt32();
            int tempr = BR.ReadInt32() - meta.magic - meta.offset;
            for (int xxx = 0; xxx < tempc; xxx++)
            {
                BR.BaseStream.Position = tempr + (xxx * 12);
                string index1string = meta.Map.Strings.Name[BR.ReadUInt16()];
                BR.BaseStream.Position = tempr + (xxx * 12) + 4;
                int tempcxx = BR.ReadInt32();
                int temprxx = BR.ReadInt32() - meta.magic - meta.offset;
                for (int xx = 0; xx < tempcxx; xx++)
                {
                    BR.BaseStream.Position = temprxx + (xx * 20);
                    string index2string = meta.Map.Strings.Name[BR.ReadUInt16()];
                    BR.BaseStream.Position = temprxx + (xx * 20) + 4;
                    int tempcx = BR.ReadInt32();
                    int temprx = BR.ReadInt32() - meta.magic - meta.offset;

                    // Meshes = new CollisionMesh[tempc];
                    for (int x = 0; x < tempcx; x++)
                    {
                        CollisionMesh cm = new CollisionMesh();
                        cm.index1 = xxx;
                        cm.index1string = index1string;
                        cm.index2 = xx;
                        cm.index2string = index2string;
                        if (ConditionStrings.IndexOf(cm.index2string) == -1)
                        {
                            ConditionStrings.Add(cm.index2string);
                        }

                        cm.index3 = x;

                        BR.BaseStream.Position = temprx + (x * 68) + 52;
                        int tempc2 = BR.ReadInt32();
                        int tempr2 = BR.ReadInt32() - meta.magic - meta.offset;

                        cm.Faces = new ushort[tempc2 * 3];
                        for (int y = 0; y < tempc2; y++)
                        {
                            BR.BaseStream.Position = tempr2 + (y * 12);
                            cm.Faces[y * 3] = BR.ReadUInt16();
                            cm.Faces[(y * 3) + 1] = BR.ReadUInt16();
                            cm.Faces[(y * 3) + 2] = cm.Faces[(y * 3) + 1]; // BR.ReadUInt16();
                        }

                        // ***** Test Code ****//
                        BR.BaseStream.Position = temprx + (x * 68) + 12;
                        tempc2 = BR.ReadInt32();
                        tempr2 = BR.ReadInt32() - meta.magic - meta.offset;
                        cm.Normals = new Vector4[tempc2];
                        for (int y = 0; y < tempc2; y++)
                        {
                            BR.BaseStream.Position = tempr2 + (y * 16) + 0;
                            cm.Normals[y].X = BR.ReadSingle();
                            cm.Normals[y].Y = BR.ReadSingle();
                            cm.Normals[y].Z = BR.ReadSingle();
                            cm.Normals[y].W = BR.ReadSingle();
                        }

                        BR.BaseStream.Position = temprx + (x * 68) + 44;
                        tempc2 = BR.ReadInt32();
                        tempr2 = BR.ReadInt32() - meta.magic - meta.offset;
                        cm.SurfaceData = new surfaceData[tempc2];
                        for (int y = 0; y < tempc2; y++)
                        {
                            BR.BaseStream.Position = tempr2 + (y * 8) + 0;
                            cm.SurfaceData[y] = new surfaceData();
                            cm.SurfaceData[y].Plane = BR.ReadInt16();
                        }

                        BR.BaseStream.Position = temprx + (x * 68) + 52;
                        tempc2 = BR.ReadInt32();
                        tempr2 = BR.ReadInt32() - meta.magic - meta.offset;

                        int[] startEdges = new int[tempc2];
                        int[] endEdges = new int[tempc2];
                        int[] forwardEdges = new int[tempc2];
                        int[] reverseEdges = new int[tempc2];
                        int[] face1 = new int[tempc2];
                        int[] face2 = new int[tempc2];
                        for (int y = 0; y < tempc2; y++)
                        {
                            BR.BaseStream.Position = tempr2 + (y * 12) + 0;
                            startEdges[y] = BR.ReadUInt16();
                            endEdges[y] = BR.ReadUInt16();

                            BR.BaseStream.Position = tempr2 + (y * 12) + 4;
                            forwardEdges[y] = BR.ReadUInt16();
                            reverseEdges[y] = BR.ReadUInt16();

                            BR.BaseStream.Position = tempr2 + (y * 12) + 8;
                            face1[y] = BR.ReadUInt16();
                            face2[y] = BR.ReadUInt16();
                        }

                        for (int y = 0; y < forwardEdges.Length; y++)
                        {
                            int cSurface = face1[y];
                            if (cm.SurfaceData[cSurface].Vertices == null)
                            {
                                cm.SurfaceData[cSurface].Vertices = new List<int>();
                                int edge = y;
                                int nextEdge = 0;
                                cm.SurfaceData[cSurface].Vertices.Add(startEdges[edge]);
                                do
                                {
                                    if (cSurface == face1[edge])
                                    {
                                        cm.SurfaceData[cSurface].Vertices.Add(endEdges[edge]);
                                        edge = forwardEdges[edge];
                                    }
                                    else if (cSurface == face2[edge])
                                    {
                                        cm.SurfaceData[cSurface].Vertices.Add(startEdges[edge]);
                                        edge = reverseEdges[edge];
                                    }

                                    if (cSurface == face1[edge])
                                    {
                                        nextEdge = endEdges[edge];
                                    }
                                    else if (cSurface == face2[edge])
                                    {
                                        nextEdge = startEdges[edge];
                                    }
                                }
                                while (!cm.SurfaceData[cSurface].Vertices.Contains(nextEdge));
                            }
                        }

                        for (int y = 0; y < reverseEdges.Length; y++)
                        {
                            int cSurface = face2[y];
                            if (cm.SurfaceData[cSurface].Vertices == null)
                            {
                                cm.SurfaceData[cSurface].Vertices = new List<int>();
                                int edge = y;
                                int nextEdge = 0;
                                cm.SurfaceData[cSurface].Vertices.Add(endEdges[edge]);
                                do
                                {
                                    if (cSurface == face1[edge])
                                    {
                                        cm.SurfaceData[cSurface].Vertices.Add(endEdges[edge]);
                                        edge = forwardEdges[edge];
                                    }
                                    else if (cSurface == face2[edge])
                                    {
                                        cm.SurfaceData[cSurface].Vertices.Add(startEdges[edge]);
                                        edge = reverseEdges[edge];
                                    }

                                    if (cSurface == face1[edge])
                                    {
                                        nextEdge = endEdges[edge];
                                    }
                                    else if (cSurface == face2[edge])
                                    {
                                        nextEdge = startEdges[edge];
                                    }
                                }
                                while (!cm.SurfaceData[cSurface].Vertices.Contains(nextEdge));
                            }
                        }

                        // ***** End Test Code ****//
                        BR.BaseStream.Position = temprx + (x * 68) + 60;
                        tempc2 = BR.ReadInt32();
                        tempr2 = BR.ReadInt32() - meta.magic - meta.offset;

                        // Meshes[x] = new CollisionMesh();
                        cm.Vertices = new Vector3[tempc2];
                        for (int y = 0; y < tempc2; y++)
                        {
                            BR.BaseStream.Position = tempr2 + (y * 16);
                            cm.Vertices[y].X = BR.ReadSingle();
                            cm.Vertices[y].Y = BR.ReadSingle();
                            cm.Vertices[y].Z = BR.ReadSingle();
                        }

                        Meshes.Add(cm);
                    }
                }
            }

            BR.BaseStream.Position = 36;
            tempc = BR.ReadInt32();
            tempr = BR.ReadInt32() - meta.magic - meta.offset;
            for (int xxx = 0; xxx < tempc; xxx++)
            {
                BR.BaseStream.Position = tempr + (20 * xxx);
                PathfindingSphere ps = new PathfindingSphere();
                ps.nodeindex = BR.ReadInt32();
                ps.position = new Vector3();
                ps.position.X = BR.ReadSingle();
                ps.position.Y = BR.ReadSingle();
                ps.position.Z = BR.ReadSingle();
                ps.radius = BR.ReadSingle();
                Spheres.Add(ps);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// The extract meshes.
        /// </summary>
        /// <param name="FilePath">The file path.</param>
        /// <remarks></remarks>
        public void ExtractMeshes(string FilePath)
        {
            for (int x = 0; x < Meshes.Count; x++)
            {
                FileStream FS =
                    new FileStream(
                        FilePath + "[" + Meshes[x].index1string + "]" + "[" + Meshes[x].index2string + "]" + "[" +
                        Meshes[x].index3 + "]" + ".obj", 
                        FileMode.Create);
                StreamWriter SW = new StreamWriter(FS);
                SW.WriteLine("# ------------------------------------");
                SW.WriteLine("# Halo 2 Collision Mesh - Extracted with Entity");
                SW.WriteLine("# ------------------------------------");
                for (int xx = 0; xx < Meshes[x].Vertices.Length; xx++)
                {
                    string temps = "v " + Meshes[x].Vertices[xx].X + " " + Meshes[x].Vertices[xx].Y + " " +
                                   Meshes[x].Vertices[xx].Z;
                    SW.WriteLine(temps);
                }

                for (int xx = 0; xx < Meshes[x].Faces.Length; xx += 3)
                {
                    string temps = "f " + (Meshes[x].Faces[xx] + 1) + " " + (Meshes[x].Faces[xx + 1] + 1) + " " +
                                   (Meshes[x].Faces[xx + 1] + 1);
                    SW.WriteLine(temps);
                }

                SW.Close();
                FS.Close();
            }
        }

        #endregion

        /// <summary>
        /// The collision mesh.
        /// </summary>
        /// <remarks></remarks>
        public class CollisionMesh
        {
            #region Constants and Fields

            /// <summary>
            /// The edges.
            /// </summary>
            public ushort[] Edges;

            /// <summary>
            /// The faces.
            /// </summary>
            public ushort[] Faces;

            /// <summary>
            /// The normals.
            /// </summary>
            public Vector4[] Normals;

            /// <summary>
            /// The surface data.
            /// </summary>
            public surfaceData[] SurfaceData;

            /// <summary>
            /// The vertices.
            /// </summary>
            public Vector3[] Vertices;

            /// <summary>
            /// The index 1.
            /// </summary>
            public int index1;

            /// <summary>
            /// The index 1 string.
            /// </summary>
            public string index1string;

            /// <summary>
            /// The index 2.
            /// </summary>
            public int index2;

            /// <summary>
            /// The index 2 string.
            /// </summary>
            public string index2string;

            /// <summary>
            /// The index 3.
            /// </summary>
            public int index3;

            #endregion
        }

        /// <summary>
        /// The pathfinding sphere.
        /// </summary>
        /// <remarks></remarks>
        public class PathfindingSphere
        {
            #region Constants and Fields

            /// <summary>
            /// The nodeindex.
            /// </summary>
            public int nodeindex;

            /// <summary>
            /// The position.
            /// </summary>
            public Vector3 position;

            /// <summary>
            /// The radius.
            /// </summary>
            public float radius;

            #endregion
        }

        /// <summary>
        /// The surface data.
        /// </summary>
        /// <remarks></remarks>
        public class surfaceData
        {
            #region Constants and Fields

            /// <summary>
            /// The plane.
            /// </summary>
            public int Plane;

            /// <summary>
            /// The vertices.
            /// </summary>
            public List<int> Vertices;

            #endregion
        }

        // public List<int> points = new List<int>();
        // public List<int> faces = new List<int>();
    }

    /// <summary>
    /// The direct xbsp collision.
    /// </summary>
    /// <remarks></remarks>
    public class DirectXBSPCollision
    {
        #region Constants and Fields

        /// <summary>
        /// The face count.
        /// </summary>
        public int faceCount;

        /// <summary>
        /// The ib.
        /// </summary>
        public IndexBuffer ib;

        /// <summary>
        /// The vb.
        /// </summary>
        public VertexBuffer vb;

        /// <summary>
        /// The vertice count.
        /// </summary>
        public int verticeCount;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DirectXBSPCollision"/> class.
        /// </summary>
        /// <param name="device">The device.</param>
        /// <param name="tempcoll">The tempcoll.</param>
        /// <remarks></remarks>
        public DirectXBSPCollision(ref Device device, ref BSPModel.BSPCollision tempcoll)
        {
            CreateVertexBuffers(ref device, ref tempcoll);
            CreateIndexBuffers(ref device, ref tempcoll);
            verticeCount = tempcoll.Vertices.Length;
            faceCount = tempcoll.Faces.Length;
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
            device.SetStreamSource(0, vb, 0);
            device.VertexFormat = CustomVertex.PositionColored.Format;
            device.Indices = ib;

            device.RenderState.AlphaBlendEnable = false;
            device.RenderState.AlphaTestEnable = false;

            device.SetTexture(0, null);

            PrimitiveType pt;

            pt = PrimitiveType.TriangleList;
            device.DrawIndexedPrimitives(pt, 0, 0, this.verticeCount, 0, this.faceCount / 3);
        }

        #endregion

        #region Methods

        /// <summary>
        /// The create index buffers.
        /// </summary>
        /// <param name="device">The device.</param>
        /// <param name="tempcoll">The tempcoll.</param>
        /// <remarks></remarks>
        private void CreateIndexBuffers(ref Device device, ref BSPModel.BSPCollision tempcoll)
        {
            ib = new IndexBuffer(typeof(short), tempcoll.Faces.Length, device, Usage.WriteOnly, Pool.Default);
            ib.SetData(tempcoll.Faces, 0, LockFlags.None);
            ib.Unlock();
        }

        /// <summary>
        /// The create vertex buffers.
        /// </summary>
        /// <param name="device">The device.</param>
        /// <param name="tempcoll">The tempcoll.</param>
        /// <remarks></remarks>
        private void CreateVertexBuffers(ref Device device, ref BSPModel.BSPCollision tempcoll)
        {
            vb = new VertexBuffer(
                typeof(CustomVertex.PositionColored), 
                tempcoll.Vertices.Length, 
                device, 
                Usage.WriteOnly, 
                CustomVertex.PositionColored.Format, 
                Pool.Default);
            CustomVertex.PositionColored[] verts = (CustomVertex.PositionColored[])vb.Lock(0, 0);

            // Lock the buffer (which will return our structs)
            for (int i = 0; i < tempcoll.Vertices.Length; i++)
            {
                verts[i].Position = new Vector3(tempcoll.Vertices[i].X, tempcoll.Vertices[i].Y, tempcoll.Vertices[i].Z);
            }

            // Unlock (and copy) the data
            vb.Unlock();
        }

        #endregion
    }

    /// <summary>
    /// The direct x collision.
    /// </summary>
    /// <remarks></remarks>
    public class DirectXCollision
    {
        #region Constants and Fields

        /// <summary>
        /// The face count.
        /// </summary>
        public List<int> faceCount = new List<int>();

        /// <summary>
        /// The ib.
        /// </summary>
        public List<IndexBuffer> ib = new List<IndexBuffer>();

        /// <summary>
        /// The sphere.
        /// </summary>
        public List<Mesh> sphere = new List<Mesh>();

        /// <summary>
        /// The spherematrix.
        /// </summary>
        public List<Matrix> spherematrix = new List<Matrix>();

        /// <summary>
        /// The vb.
        /// </summary>
        public List<VertexBuffer> vb = new List<VertexBuffer>();

        /// <summary>
        /// The vertice count.
        /// </summary>
        public List<int> verticeCount = new List<int>();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DirectXCollision"/> class.
        /// </summary>
        /// <param name="device">The device.</param>
        /// <param name="tempcoll">The tempcoll.</param>
        /// <remarks></remarks>
        public DirectXCollision(ref Device device, ref coll tempcoll)
        {
            CreateVertexBuffers(ref device, ref tempcoll);
            CreateIndexBuffers(ref device, ref tempcoll);
            CreateSpheres(ref device, ref tempcoll);

            // verticeCount = tempcoll.Vertices.Length;

            // faceCount = tempcoll.Faces.Length;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// The draw meshes.
        /// </summary>
        /// <param name="device">The device.</param>
        /// <remarks></remarks>
        public void DrawMeshes(ref Device device)
        {
            for (int i = 0; i < this.vb.Count; i++)
            {
                device.SetStreamSource(0, vb[i], 0);
                device.VertexFormat = CustomVertex.PositionColored.Format;
                device.Indices = ib[i];

                device.RenderState.AlphaBlendEnable = false;
                device.RenderState.AlphaTestEnable = false;

                device.Transform.World = Matrix.Identity;
                device.SetTexture(0, null);

                PrimitiveType pt;

                pt = PrimitiveType.TriangleList;
                device.DrawIndexedPrimitives(pt, 0, 0, this.verticeCount[i], 0, this.faceCount[i] / 3);
            }
        }

        /// <summary>
        /// The draw spheres.
        /// </summary>
        /// <param name="device">The device.</param>
        /// <remarks></remarks>
        public void DrawSpheres(ref Device device)
        {
            for (int x = 0; x < this.sphere.Count; x++)
            {
                device.RenderState.AlphaBlendEnable = false;
                device.RenderState.AlphaTestEnable = false;

                device.Transform.World.Transpose(spherematrix[x]);
                device.SetTexture(0, null);

                this.sphere[x].DrawSubset(0);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The create index buffers.
        /// </summary>
        /// <param name="device">The device.</param>
        /// <param name="tempcoll">The tempcoll.</param>
        /// <remarks></remarks>
        private void CreateIndexBuffers(ref Device device, ref coll tempcoll)
        {
            for (int x = 0; x < tempcoll.Meshes.Count; x++)
            {
                if (tempcoll.Meshes[x].index2string == tempcoll.ConditionStrings[0])
                {
                    IndexBuffer tempib;
                    tempib = new IndexBuffer(
                        typeof(short), tempcoll.Meshes[x].Faces.Length, device, Usage.WriteOnly, Pool.Default);
                    tempib.SetData(tempcoll.Meshes[x].Faces, 0, LockFlags.None);
                    tempib.Unlock();
                    ib.Add(tempib);
                    faceCount.Add(tempcoll.Meshes[x].Faces.Length);
                }
            }
        }

        /// <summary>
        /// The create spheres.
        /// </summary>
        /// <param name="device">The device.</param>
        /// <param name="tempcoll">The tempcoll.</param>
        /// <remarks></remarks>
        private void CreateSpheres(ref Device device, ref coll tempcoll)
        {
            for (int x = 0; x < tempcoll.Spheres.Count; x++)
            {
                Mesh m = Mesh.Sphere(device, tempcoll.Spheres[x].radius, 10, 10);
                sphere.Add(m);
                Matrix mm = Matrix.Identity;
                
                mm.Multiply(Matrix.Translation(tempcoll.Spheres[x].position));
                spherematrix.Add(mm);
            }
        }

        /// <summary>
        /// The create vertex buffers.
        /// </summary>
        /// <param name="device">The device.</param>
        /// <param name="tempcoll">The tempcoll.</param>
        /// <remarks></remarks>
        private void CreateVertexBuffers(ref Device device, ref coll tempcoll)
        {
            for (int x = 0; x < tempcoll.Meshes.Count; x++)
            {
                if (tempcoll.Meshes[x].index2string == tempcoll.ConditionStrings[0])
                {
                    VertexBuffer vbx = new VertexBuffer(
                        typeof(CustomVertex.PositionColored), 
                        tempcoll.Meshes[x].Vertices.Length, 
                        device, 
                        Usage.WriteOnly, 
                        CustomVertex.PositionColored.Format, 
                        Pool.Default);
                    CustomVertex.PositionColored[] verts = (CustomVertex.PositionColored[])vbx.Lock(0, 0);

                    // Lock the buffer (which will return our structs)
                    for (int i = 0; i < tempcoll.Meshes[x].Vertices.Length; i++)
                    {
                        verts[i].Position = new Vector3(
                            tempcoll.Meshes[x].Vertices[i].X, 
                            tempcoll.Meshes[x].Vertices[i].Y, 
                            tempcoll.Meshes[x].Vertices[i].Z);
                    }

                    vbx.Unlock();
                    vb.Add(vbx);
                    verticeCount.Add(tempcoll.Meshes[x].Vertices.Length);
                }
            }
        }

        #endregion
    }
}