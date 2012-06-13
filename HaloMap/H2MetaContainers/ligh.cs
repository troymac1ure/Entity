// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ligh.cs" company="">
//   
// </copyright>
// <summary>
//   The halo light.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HaloMap.H2MetaContainers
{
    using HaloMap.Map;

    /// <summary>
    /// The halo light.
    /// </summary>
    /// <remarks></remarks>
    public class HaloLight
    {
        #region Constants and Fields

        /// <summary>
        /// The b.
        /// </summary>
        public int b;

        /// <summary>
        /// The g.
        /// </summary>
        public int g;

        /// <summary>
        /// The r.
        /// </summary>
        public int r;

        /// <summary>
        /// The range.
        /// </summary>
        public int range;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="HaloLight"/> class.
        /// </summary>
        /// <param name="tagIndex">Index of the tag.</param>
        /// <param name="map">The map.</param>
        /// <remarks></remarks>
        public HaloLight(int tagIndex, Map map)
        {
            map.BR.BaseStream.Position = map.MetaInfo.Offset[tagIndex] + 72;
            range = map.BR.ReadInt32();
            r = (int)(map.BR.ReadSingle() * 255);
            g = (int)(map.BR.ReadSingle() * 255);
            b = (int)(map.BR.ReadSingle() * 255);
        }

        #endregion
    }
}