// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Globals.cs" company="">
//   
// </copyright>
// <summary>
//   The global.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Globals
{
    using System;
    using System.IO;
    using System.Windows.Forms;

    /// <summary>
    /// The global.
    /// </summary>
    /// <remarks></remarks>
    public static class Global
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes static members of the <see cref="Global"/> class.
        /// </summary>
        /// <remarks></remarks>
        static Global()
        {
#if !DEBUG
            StartupPath = Application.StartupPath;
#else
            StartupPath = Path.GetDirectoryName(Application.StartupPath) + "\\Release";
#endif
        }

        #endregion

        #region Properties

        /// <summary>
        /// Use this instead of Application.StartUpPath, so all additional
        /// files (ie. plugins) can only exist in the RELEASE folder and this
        /// string will point to that even when running in debug.
        /// </summary>
        /// <remarks></remarks>
        public static string StartupPath { get; private set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Shows a messagebox with the specified error text and information from
        /// the generated error report
        /// </summary>
        /// <param name="informativeError">The informative Error.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        public static void ShowErrorMsg(string informativeError, Exception e)
        {
            MessageBox.Show(
                string.Format(
                    "{0}\r\n\r\nInternal Error:\r\n\r\n{1}\r\n---\r\n{2}", 
                    informativeError, 
                    e.Message, 
                    e.StackTrace.Substring(e.StackTrace.LastIndexOf(" at ") + 1)), 
                "Error", 
                MessageBoxButtons.OK, 
                MessageBoxIcon.Warning);
        }

        /// <summary>
        /// Properly clears all controls off of a control by disposing of each one
        /// </summary>
        /// <param name="c"></param>
        public static void ClearControls(Control c)
        {
            for (int i = c.Controls.Count - 1; i >= 0; i--)
            {
                if (c.Controls[i].Controls.Count > 0)
                    ClearControls(c.Controls[i]);
                c.Controls[i].Dispose();
            }
            c.Controls.Clear();
        }
        #endregion
    }
}