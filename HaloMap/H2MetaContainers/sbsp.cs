// --------------------------------------------------------------------------------------------------------------------
// <copyright file="sbsp.cs" company="">
//   
// </copyright>
// <summary>
//   The h 2 sbsp.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HaloMap.H2MetaContainers
{
    using HaloMap.Map;
    using HaloMap.Meta;

    /// <summary>
    /// The h 2 sbsp.
    /// </summary>
    /// <remarks></remarks>
    public class H2SBSP : TagInterface
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="H2SBSP"/> class.
        /// </summary>
        /// <param name="TagIndex">Index of the tag.</param>
        /// <param name="map">The map.</param>
        /// <remarks></remarks>
        public H2SBSP(int TagIndex, Map map)
            : base(TagIndex, map)
        {
        }

        #endregion

        /// <summary>
        /// The sbsp.
        /// </summary>
        /// <remarks></remarks>
        public class SBSP : TagBlockLayout
        {
            /// <summary>
            /// The bsp.
            /// </summary>
            /// <remarks></remarks>
            public class BSP : TagBlockLayout
            {
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
                    private readonly ShortInteger backchild = new ShortInteger("Back Child");

                    /// <summary>
                    /// The frontchild.
                    /// </summary>
                    private readonly ShortInteger frontchild = new ShortInteger("Front Child");

                    /// <summary>
                    /// The plane_index.
                    /// </summary>
                    private readonly ShortInteger plane_index = new ShortInteger("BSP_Plane Index");

                    /// <summary>
                    /// The unknown 1.
                    /// </summary>
                    private readonly ByteInteger unknown1 = new ByteInteger("Unknown 1");

                    /// <summary>
                    /// The unknown 2.
                    /// </summary>
                    private readonly ByteInteger unknown2 = new ByteInteger("Unknown 2");

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
                    private readonly ShortInteger leftchild = new ShortInteger("Left Child");

                    /// <summary>
                    /// The plane.
                    /// </summary>
                    private readonly RealPlane3D plane = new RealPlane3D("Plane");

                    /// <summary>
                    /// The rightchild.
                    /// </summary>
                    private readonly ShortInteger rightchild = new ShortInteger("Right Child");

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
                    private readonly ShortInteger bsp_2d_node_index = new ShortInteger("BSP_2D_Node Index");

                    /// <summary>
                    /// The plane_index.
                    /// </summary>
                    private readonly ShortInteger plane_index = new ShortInteger("BSP_Plane Index");

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
                    private readonly ShortInteger first_edge = new ShortInteger("First Edge");

                    /// <summary>
                    /// The flags.
                    /// </summary>
                    private readonly ByteFlags flags = new ByteFlags("Flags");

                    /// <summary>
                    /// The plane_index.
                    /// </summary>
                    private readonly ShortInteger plane_index = new ShortInteger("Plane Index");

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
                    private readonly ShortInteger EndVertex = new ShortInteger("End Vertex");

                    /// <summary>
                    /// The forward edge.
                    /// </summary>
                    private readonly ShortInteger ForwardEdge = new ShortInteger("Forward Edge");

                    /// <summary>
                    /// The left surface.
                    /// </summary>
                    private readonly ShortInteger LeftSurface = new ShortInteger("Left Surface");

                    /// <summary>
                    /// The reverse edge.
                    /// </summary>
                    private readonly ShortInteger ReverseEdge = new ShortInteger("Reverse Edge");

                    /// <summary>
                    /// The right surface.
                    /// </summary>
                    private readonly ShortInteger RightSurface = new ShortInteger("Right Surface");

                    /// <summary>
                    /// The start vertex.
                    /// </summary>
                    private readonly ShortInteger StartVertex = new ShortInteger("Start Vertex");

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
                    private readonly ByteInteger bsp2drefindex = new ByteInteger("BSP_2D_Reference Start Index");

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
                        Add(bsp2drefindex);
                        Add(bsp2drefcount);
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