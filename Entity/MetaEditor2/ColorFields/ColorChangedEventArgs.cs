// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ColorChangedEventArgs.cs" company="">
//   
// </copyright>
// <summary>
//   The color changed event args.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace entity.MetaEditor2
{
    /// <summary>
    /// The color changed event args.
    /// </summary>
    /// <remarks></remarks>
    public class ColorChangedEventArgs : EventArgs
    {
        #region Constants and Fields

        /// <summary>
        /// The m hsv.
        /// </summary>
        private readonly ColorHandler.HSV mHSV;

        /// <summary>
        /// The m rgb.
        /// </summary>
        private readonly ColorHandler.RGB mRGB;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ColorChangedEventArgs"/> class.
        /// </summary>
        /// <param name="RGB">The RGB.</param>
        /// <param name="HSV">The HSV.</param>
        /// <remarks></remarks>
        public ColorChangedEventArgs(ColorHandler.RGB RGB, ColorHandler.HSV HSV)
        {
            mRGB = RGB;
            mHSV = HSV;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets HSV.
        /// </summary>
        /// <remarks></remarks>
        public ColorHandler.HSV HSV
        {
            get
            {
                return mHSV;
            }
        }

        /// <summary>
        /// Gets RGB.
        /// </summary>
        /// <remarks></remarks>
        public ColorHandler.RGB RGB
        {
            get
            {
                return mRGB;
            }
        }

        #endregion
    }
}