// --------------------------------------------------------------------------------------------------------------------
// <copyright file="sbsp.cs" company="">
//   
// </copyright>
// <summary>
//   The h 1 sbsp.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HaloMap.H1MetaContainers
{
    using HaloMap.Map;
    using HaloMap.Meta;

    /// <summary>
    /// The h 1 sbsp.
    /// </summary>
    /// <remarks></remarks>
    public class H1SBSP : TagInterface
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="H1SBSP"/> class.
        /// </summary>
        /// <param name="TagIndex">Index of the tag.</param>
        /// <param name="map">The map.</param>
        /// <remarks></remarks>
        public H1SBSP(int TagIndex, Map map)
            : base(TagIndex, map)
        {
            this.Header = new TagBlock("SBSP", SBSP.Layout, HaloVersionEnum.Halo1);
            this.Header.ReadElements();
        }

        #endregion

        /// <summary>
        /// The sbsp.
        /// </summary>
        /// <remarks></remarks>
        public class SBSP : TagBlockLayout
        {
            #region Constants and Fields

            /// <summary>
            /// The layout.
            /// </summary>
            public static readonly SBSP Layout = new SBSP();

            /// <summary>
            /// The bsp.
            /// </summary>
            public TagBlock bsp = new TagBlock("BSP Collision", BSP.Layout, HaloVersionEnum.Halo1);

            /// <summary>
            /// The temppadding.
            /// </summary>
            public PaddingX temppadding = new PaddingX(200, "TempPadding");

            /// <summary>
            /// The test.
            /// </summary>
            public RealRGBColor test = new RealRGBColor("rgb");

            #endregion

            #region Constructors and Destructors

            /// <summary>
            /// Initializes a new instance of the <see cref="SBSP"/> class.
            /// </summary>
            /// <remarks></remarks>
            public SBSP()
            {
                Add(temppadding);
                Add(bsp);
                Add(test);
            }

            #endregion

            /// <summary>
            /// The bsp.
            /// </summary>
            /// <remarks></remarks>
            public class BSP : TagBlockLayout
            {
                #region Constants and Fields

                /// <summary>
                /// The layout.
                /// </summary>
                public static readonly BSP Layout = new BSP();

                /// <summary>
                /// The bsp_2 d_nodes.
                /// </summary>
                public TagBlock bsp_2d_nodes = new TagBlock(
                    "BSP_2D_Nodes", BSP_2D_Node.Layout, HaloVersionEnum.Halo1);

                /// <summary>
                /// The bsp_2 d_references.
                /// </summary>
                public TagBlock bsp_2d_references = new TagBlock(
                    "BSP_2D_References", BSP_2D_Reference.Layout, HaloVersionEnum.Halo1);

                /// <summary>
                /// The bsp_3 d_nodes.
                /// </summary>
                public TagBlock bsp_3d_nodes = new TagBlock("BSP_3D_Nodes", BSP3DNode.Layout, HaloVersionEnum.Halo1);

                /// <summary>
                /// The bsp_edges.
                /// </summary>
                public TagBlock bsp_edges = new TagBlock("BSP_Edges", Edge.Layout, HaloVersionEnum.Halo1);

                /// <summary>
                /// The bsp_leaves.
                /// </summary>
                public TagBlock bsp_leaves = new TagBlock("BSP_Leaves", Leaf.Layout, HaloVersionEnum.Halo1);

                /// <summary>
                /// The bsp_planes.
                /// </summary>
                public TagBlock bsp_planes = new TagBlock("BSP_Planes", BSP_Plane.Layout, HaloVersionEnum.Halo1);

                /// <summary>
                /// The bsp_surfaces.
                /// </summary>
                public TagBlock bsp_surfaces = new TagBlock(
                    "BSP_Surfaces", BSP_Surface.Layout, HaloVersionEnum.Halo1);

                /// <summary>
                /// The bsp_vertices.
                /// </summary>
                public TagBlock bsp_vertices = new TagBlock("BSP_Vertices", Vertice.Layout, HaloVersionEnum.Halo1);

                #endregion

                #region Constructors and Destructors

                /// <summary>
                /// Initializes a new instance of the <see cref="BSP"/> class.
                /// </summary>
                /// <remarks></remarks>
                public BSP()
                {
                    Add(bsp_3d_nodes);
                    Add(bsp_planes);
                    Add(bsp_leaves);
                    Add(bsp_2d_references);
                    Add(bsp_2d_nodes);
                    Add(bsp_surfaces);
                    Add(bsp_edges);
                    Add(bsp_vertices);
                }

                #endregion

                /// <summary>
                /// The bs p 3 d node.
                /// </summary>
                /// <remarks></remarks>
                public class BSP3DNode : TagBlockLayout
                {
                    #region Constants and Fields

                    /// <summary>
                    /// The layout.
                    /// </summary>
                    public static readonly BSP3DNode Layout = new BSP3DNode();

                    /// <summary>
                    /// The backchild.
                    /// </summary>
                    private readonly LongInteger backchild = new LongInteger("Back Child");

                    /// <summary>
                    /// The frontchild.
                    /// </summary>
                    private readonly ShortInteger frontchild = new ShortInteger("Front Child");

                    /// <summary>
                    /// The plane_index.
                    /// </summary>
                    private readonly LongInteger plane_index = new LongInteger("BSP_Plane Index");

                    /// <summary>
                    /// The unknown 1.
                    /// </summary>
                    private readonly ByteInteger unknown1 = new ByteInteger("unknown1");

                    /// <summary>
                    /// The unknown 2.
                    /// </summary>
                    private readonly ByteInteger unknown2 = new ByteInteger("unknown2");

                    #endregion

                    #region Constructors and Destructors

                    /// <summary>
                    /// Initializes a new instance of the <see cref="BSP3DNode"/> class.
                    /// </summary>
                    /// <remarks></remarks>
                    public BSP3DNode()
                    {
                        Add(plane_index);
                        Add(backchild);
                        Add(unknown1);
                        Add(frontchild);
                        Add(unknown2);
                    }

                    #endregion
                }

                /// <summary>
                /// The bs p_2 d_ node.
                /// </summary>
                /// <remarks></remarks>
                public class BSP_2D_Node : TagBlockLayout
                {
                    #region Constants and Fields

                    /// <summary>
                    /// The layout.
                    /// </summary>
                    public static readonly BSP_2D_Node Layout = new BSP_2D_Node();

                    /// <summary>
                    /// The leftchild.
                    /// </summary>
                    private readonly LongInteger leftchild = new LongInteger("Left Child");

                    /// <summary>
                    /// The plane.
                    /// </summary>
                    private readonly RealPlane3D plane = new RealPlane3D("Plane");

                    /// <summary>
                    /// The rightchild.
                    /// </summary>
                    private readonly LongInteger rightchild = new LongInteger("Right Child");

                    #endregion

                    #region Constructors and Destructors

                    /// <summary>
                    /// Initializes a new instance of the <see cref="BSP_2D_Node"/> class.
                    /// </summary>
                    /// <remarks></remarks>
                    public BSP_2D_Node()
                    {
                        Add(plane);
                        Add(leftchild);
                        Add(rightchild);
                    }

                    #endregion
                }

                /// <summary>
                /// The bs p_2 d_ reference.
                /// </summary>
                /// <remarks></remarks>
                public class BSP_2D_Reference : TagBlockLayout
                {
                    #region Constants and Fields

                    /// <summary>
                    /// The layout.
                    /// </summary>
                    public static readonly BSP_2D_Reference Layout = new BSP_2D_Reference();

                    /// <summary>
                    /// The bsp_2 d_node_index.
                    /// </summary>
                    private readonly LongInteger bsp_2d_node_index = new LongInteger("BSP_2D_Node Index");

                    /// <summary>
                    /// The plane_index.
                    /// </summary>
                    private readonly LongInteger plane_index = new LongInteger("BSP_Plane Index");

                    #endregion

                    #region Constructors and Destructors

                    /// <summary>
                    /// Initializes a new instance of the <see cref="BSP_2D_Reference"/> class.
                    /// </summary>
                    /// <remarks></remarks>
                    public BSP_2D_Reference()
                    {
                        Add(plane_index);
                        Add(bsp_2d_node_index);
                    }

                    #endregion
                }

                /// <summary>
                /// The bs p_ plane.
                /// </summary>
                /// <remarks></remarks>
                public class BSP_Plane : TagBlockLayout
                {
                    #region Constants and Fields

                    /// <summary>
                    /// The layout.
                    /// </summary>
                    public static readonly BSP_Plane Layout = new BSP_Plane();

                    /// <summary>
                    /// The plane.
                    /// </summary>
                    private readonly RealPlane3D plane = new RealPlane3D("BSP_Plane");

                    #endregion

                    #region Constructors and Destructors

                    /// <summary>
                    /// Initializes a new instance of the <see cref="BSP_Plane"/> class.
                    /// </summary>
                    /// <remarks></remarks>
                    public BSP_Plane()
                    {
                        Add(plane);
                    }

                    #endregion
                }

                /// <summary>
                /// The bs p_ surface.
                /// </summary>
                /// <remarks></remarks>
                public class BSP_Surface : TagBlockLayout
                {
                    #region Constants and Fields

                    /// <summary>
                    /// The layout.
                    /// </summary>
                    public static readonly BSP_Surface Layout = new BSP_Surface();

                    /// <summary>
                    /// The breakablesurface_index.
                    /// </summary>
                    private readonly ByteInteger breakablesurface_index = new ByteInteger("Breakable Surface Index");

                    /// <summary>
                    /// The collision_material_index.
                    /// </summary>
                    private readonly ShortInteger collision_material_index = new ShortInteger(
                        "Collision Material Index");

                    /// <summary>
                    /// The first_edge.
                    /// </summary>
                    private readonly LongInteger first_edge = new LongInteger("First Edge");

                    /// <summary>
                    /// The flags.
                    /// </summary>
                    private readonly ByteFlags flags = new ByteFlags("Flags");

                    /// <summary>
                    /// The plane_index.
                    /// </summary>
                    private readonly LongInteger plane_index = new LongInteger("Plane Index");

                    #endregion

                    #region Constructors and Destructors

                    /// <summary>
                    /// Initializes a new instance of the <see cref="BSP_Surface"/> class.
                    /// </summary>
                    /// <remarks></remarks>
                    public BSP_Surface()
                    {
                        Add(plane_index);
                        Add(first_edge);
                        Add(flags);
                        Add(breakablesurface_index);
                        Add(collision_material_index);
                    }

                    #endregion
                }

                /// <summary>
                /// The edge.
                /// </summary>
                /// <remarks></remarks>
                public class Edge : TagBlockLayout
                {
                    #region Constants and Fields

                    /// <summary>
                    /// The layout.
                    /// </summary>
                    public static readonly Edge Layout = new Edge();

                    /// <summary>
                    /// The end vertex.
                    /// </summary>
                    private readonly LongInteger EndVertex = new LongInteger("End Vertex");

                    /// <summary>
                    /// The forward edge.
                    /// </summary>
                    private readonly LongInteger ForwardEdge = new LongInteger("Forward Edge");

                    /// <summary>
                    /// The left surface.
                    /// </summary>
                    private readonly LongInteger LeftSurface = new LongInteger("Left Surface");

                    /// <summary>
                    /// The reverse edge.
                    /// </summary>
                    private readonly LongInteger ReverseEdge = new LongInteger("Reverse Edge");

                    /// <summary>
                    /// The right surface.
                    /// </summary>
                    private readonly LongInteger RightSurface = new LongInteger("Right Surface");

                    /// <summary>
                    /// The start vertex.
                    /// </summary>
                    private readonly LongInteger StartVertex = new LongInteger("Start Vertex");

                    #endregion

                    #region Constructors and Destructors

                    /// <summary>
                    /// Initializes a new instance of the <see cref="Edge"/> class.
                    /// </summary>
                    /// <remarks></remarks>
                    public Edge()
                    {
                        Add(StartVertex);
                        Add(EndVertex);
                        Add(ForwardEdge);
                        Add(ReverseEdge);
                        Add(LeftSurface);
                        Add(RightSurface);
                    }

                    #endregion
                }

                /// <summary>
                /// The leaf.
                /// </summary>
                /// <remarks></remarks>
                public class Leaf : TagBlockLayout
                {
                    #region Constants and Fields

                    /// <summary>
                    /// The layout.
                    /// </summary>
                    public static readonly Leaf Layout = new Leaf();

                    /// <summary>
                    /// The bsp 2 drefcount.
                    /// </summary>
                    private readonly ShortInteger bsp2drefcount = new ShortInteger("BSP_2D_Reference Count");

                    /// <summary>
                    /// The bsp 2 drefindex.
                    /// </summary>
                    private readonly LongInteger bsp2drefindex = new LongInteger("BSP_2D_Reference Start Index");

                    /// <summary>
                    /// The flags.
                    /// </summary>
                    private readonly WordFlags flags = new WordFlags("Flags");

                    #endregion

                    #region Constructors and Destructors

                    /// <summary>
                    /// Initializes a new instance of the <see cref="Leaf"/> class.
                    /// </summary>
                    /// <remarks></remarks>
                    public Leaf()
                    {
                        Add(flags);
                        Add(bsp2drefcount);
                        Add(bsp2drefindex);
                    }

                    #endregion
                }

                /// <summary>
                /// The vertice.
                /// </summary>
                /// <remarks></remarks>
                public class Vertice : TagBlockLayout
                {
                    #region Constants and Fields

                    /// <summary>
                    /// The layout.
                    /// </summary>
                    public static readonly Vertice Layout = new Vertice();

                    /// <summary>
                    /// The firstedge.
                    /// </summary>
                    private readonly LongInteger firstedge = new LongInteger("First Edge");

                    /// <summary>
                    /// The vertice.
                    /// </summary>
                    private readonly RealPoint3D vertice = new RealPoint3D("Vertice");

                    #endregion

                    #region Constructors and Destructors

                    /// <summary>
                    /// Initializes a new instance of the <see cref="Vertice"/> class.
                    /// </summary>
                    /// <remarks></remarks>
                    public Vertice()
                    {
                        Add(vertice);
                        Add(firstedge);
                    }

                    #endregion
                }
            }
        }
    }
}