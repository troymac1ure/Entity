// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Scripts.cs" company="">
//   
// </copyright>
// <summary>
//   The scripts.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace entity.Tools
{
    using System.IO;

    using HaloMap.Map;

    /// <summary>
    /// The scripts.
    /// </summary>
    /// <remarks></remarks>
    public class Scripts
    {
        #region Constants and Fields

        /// <summary>
        /// The syntaxes.
        /// </summary>
        private readonly Syntax[] syntaxes;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Scripts"/> class.
        /// </summary>
        /// <param name="map">The map.</param>
        /// <remarks></remarks>
        public Scripts(Map map)
        {
            map.OpenMap(MapTypes.Internal);
            map.BR.BaseStream.Position = map.MetaInfo.Offset[3] + 568;
            int tempc = map.BR.ReadInt32();
            int tempr = map.BR.ReadInt32() - map.SecondaryMagic;
            syntaxes = new Syntax[tempc];
            map.BR.BaseStream.Position = tempr;
            for (int x = 0; x < tempc; x++)
            {
                syntaxes[x] = new Syntax(ref map.BR);
            }

            map.CloseMap();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// The write script infoto text.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <remarks></remarks>
        public void WriteScriptInfotoText(string path)
        {
            FileStream FS = new FileStream(path, FileMode.Create);
            StreamWriter SW = new StreamWriter(FS);
            for (int x = 0; x < syntaxes.Length; x++)
            {
                if (syntaxes[x].expressionindex == 0)
                {
                    continue;
                }

                if (syntaxes[x].type == 9 && syntaxes[x].type2 == 2)
                {
                    continue;
                }

                string temps = "Chunk Number: " + x + " Script Table Offset: " + syntaxes[x].scripttableoffset +
                               SW.NewLine;
                temps += "Identity: " + syntaxes[x].identity;
                temps += " Type: " + syntaxes[x].type;
                temps += " Type2: " + syntaxes[x].type2 + SW.NewLine;
                temps += "--------------------------------------------------------------------" + SW.NewLine;
                SW.WriteLine(temps);
            }

            SW.Close();
            FS.Close();
        }

        #endregion

        /// <summary>
        /// The syntax.
        /// </summary>
        /// <remarks></remarks>
        public class Syntax
        {
            #region Constants and Fields

            /// <summary>
            /// The childpointer.
            /// </summary>
            public short childpointer;

            /// <summary>
            /// The expressionindex.
            /// </summary>
            public short expressionindex;

            /// <summary>
            /// The expressionnumber.
            /// </summary>
            public short expressionnumber;

            /// <summary>
            /// The identity.
            /// </summary>
            public short identity;

            /// <summary>
            /// The scripttableoffset.
            /// </summary>
            public int scripttableoffset;

            /// <summary>
            /// The siblingpointer.
            /// </summary>
            public short siblingpointer;

            /// <summary>
            /// The type.
            /// </summary>
            public short type;

            /// <summary>
            /// The type 2.
            /// </summary>
            public short type2;

            /// <summary>
            /// The value.
            /// </summary>
            public short value;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            /// Initializes a new instance of the <see cref="Syntax"/> class.
            /// </summary>
            /// <param name="BR">The BR.</param>
            /// <remarks></remarks>
            public Syntax(ref BinaryReader BR)
            {
                expressionindex = BR.ReadInt16();
                
                identity = BR.ReadInt16();
                type2 = BR.ReadInt16();
                type = BR.ReadInt16();
                expressionnumber = BR.ReadInt16();
                siblingpointer = BR.ReadInt16();
                scripttableoffset = BR.ReadInt32();
                
                value = BR.ReadInt16();
                
                childpointer = BR.ReadInt16();
                
            }

            #endregion
        }
    }
}