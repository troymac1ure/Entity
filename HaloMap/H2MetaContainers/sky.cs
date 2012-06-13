// --------------------------------------------------------------------------------------------------------------------
// <copyright file="sky.cs" company="">
//   
// </copyright>
// <summary>
//   The sky.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HaloMap.H2MetaContainers
{
    using HaloMap.Map;

    /// <summary>
    /// The sky.
    /// </summary>
    /// <remarks></remarks>
    public class Sky
    {
        #region Constants and Fields

        /// <summary>
        /// The fog.
        /// </summary>
        public Fog fog = new Fog();

        /// <summary>
        /// The fogenabled.
        /// </summary>
        public bool fogenabled;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Sky"/> class.
        /// </summary>
        /// <remarks></remarks>
        public Sky()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Sky"/> class.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <param name="map">The map.</param>
        /// <remarks></remarks>
        public Sky(int tag, Map map)
        {
            map.BR.BaseStream.Position = map.MetaInfo.Offset[tag] + 72;
            int tempc = map.BR.ReadInt32();
            int tempr = map.BR.ReadInt32() - map.SecondaryMagic;
            if (tempc != 0)
            {
                fogenabled = true;
                map.BR.BaseStream.Position = tempr;
                fog.R = map.BR.ReadSingle();
                fog.G = map.BR.ReadSingle();
                fog.B = map.BR.ReadSingle();
                fog.A = map.BR.ReadSingle();
                fog.Start = map.BR.ReadSingle();
                fog.End = map.BR.ReadSingle();
            }

            map.BR.BaseStream.Position = map.MetaInfo.Offset[tag] + 96;
            tempc = map.BR.ReadInt32();
            tempr = map.BR.ReadInt32() - map.SecondaryMagic;
            if (tempc != 0)
            {
                map.BR.BaseStream.Position = tempr + 24;
                fog.FogThickness = map.BR.ReadSingle();
                fog.FogVisibility = map.BR.ReadSingle();
            }
        }

        #endregion

        /// <summary>
        /// The fog.
        /// </summary>
        /// <remarks></remarks>
        public class Fog
        {
            #region Constants and Fields

            /// <summary>
            /// The a.
            /// </summary>
            public float A;

            /// <summary>
            /// The b.
            /// </summary>
            public float B;

            /// <summary>
            /// The end.
            /// </summary>
            public float End;

            /// <summary>
            /// The fog thickness.
            /// </summary>
            public float FogThickness;

            /// <summary>
            /// The fog visibility.
            /// </summary>
            public float FogVisibility;

            /// <summary>
            /// The g.
            /// </summary>
            public float G;

            /// <summary>
            /// The r.
            /// </summary>
            public float R;

            /// <summary>
            /// The start.
            /// </summary>
            public float Start;

            #endregion
        }
    }
}