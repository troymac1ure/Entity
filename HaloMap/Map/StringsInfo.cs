// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringsInfo.cs" company="">
//   
// </copyright>
// <summary>
//   The strings info.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HaloMap.Map
{
    using System.IO;

    /// <summary>
    /// The strings info.
    /// </summary>
    /// <remarks></remarks>
    public class StringsInfo
    {
        #region Constants and Fields

        /// <summary>
        /// The length.
        /// </summary>
        public int[] Length;

        /// <summary>
        /// The name.
        /// </summary>
        public string[] Name;

        /// <summary>
        /// The offset.
        /// </summary>
        public int[] Offset;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="StringsInfo"/> class.
        /// </summary>
        /// <param name="BR">The BR.</param>
        /// <param name="map">The map.</param>
        /// <remarks></remarks>
        public StringsInfo(ref BinaryReader BR, Map map)
        {
            Name = new string[map.MapHeader.scriptReferenceCount];
            Length = new int[map.MapHeader.scriptReferenceCount];
            Offset = new int[map.MapHeader.scriptReferenceCount];

            // 	Reads In Offsets Of	Meta Names
            BR.BaseStream.Position = map.MapHeader.offsetToStringIndex;
            for (int x = 0; x < map.MapHeader.scriptReferenceCount; x++)
            {
                Offset[x] = BR.ReadInt32();
            }

            for (int x = 0; x < map.MapHeader.scriptReferenceCount; x++)
            {
                // figures out length of string
                int len;
                if (x != map.MapHeader.scriptReferenceCount - 1)
                {
                    len = Offset[x + 1] - Offset[x];
                }
                else
                {
                    len = map.MapHeader.sizeOfScriptReference - Offset[x];
                }

                Length[x] = len - 1;

                ///	Reads in string
                BR.BaseStream.Position = map.MapHeader.offsetToStringNames2 + Offset[x];
                Name[x] = new string(BR.ReadChars(len - 1));
            }
        }

        #endregion
    }
}