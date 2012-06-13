// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AboutBox.cs" company="">
//   
// </copyright>
// <summary>
//   The about box.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace entity.Main
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Windows.Forms;

    /// <summary>
    /// The about box.
    /// </summary>
    /// <remarks></remarks>
    internal partial class AboutBox : Form
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AboutBox"/> class.
        /// </summary>
        /// <remarks></remarks>
        public AboutBox()
        {
            InitializeComponent();
            this.Text = String.Format("About {0}", AssemblyTitle);
            this.labelProductName.Text = AssemblyProduct;
            this.labelVersion.Text += " " + Assembly.GetExecutingAssembly().GetName().Version.Major + "." +
                                      Assembly.GetExecutingAssembly().GetName().Version.Minor + "." +
                                      Assembly.GetExecutingAssembly().GetName().Version.Build;
            this.labelBuildVersion.Text += " " + Assembly.GetExecutingAssembly().GetName().Version.Revision;
            this.labelBuildVersion.Left = 450 - this.labelBuildVersion.Width;

            // this.labelDate.Text = AssemblyDescription.ToString();
            // this.labelDate.Left = 450 - this.labelDate.Width;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets AssemblyCompany.
        /// </summary>
        /// <remarks></remarks>
        public string AssemblyCompany
        {
            get
            {
                object[] attributes =
                    Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                if (attributes.Length == 0)
                {
                    return string.Empty;
                }

                return ((AssemblyCompanyAttribute)attributes[0]).Company;
            }
        }

        /// <summary>
        /// Gets AssemblyCopyright.
        /// </summary>
        /// <remarks></remarks>
        public string AssemblyCopyright
        {
            get
            {
                object[] attributes =
                    Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                if (attributes.Length == 0)
                {
                    return string.Empty;
                }

                return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
            }
        }

        /// <summary>
        /// Gets AssemblyDescription.
        /// </summary>
        /// <remarks></remarks>
        public string AssemblyDescription
        {
            get
            {
                object[] attributes =
                    Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                if (attributes.Length == 0)
                {
                    return string.Empty;
                }

                return ((AssemblyDescriptionAttribute)attributes[0]).Description;
            }
        }

        /// <summary>
        /// Gets AssemblyProduct.
        /// </summary>
        /// <remarks></remarks>
        public string AssemblyProduct
        {
            get
            {
                object[] attributes =
                    Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                if (attributes.Length == 0)
                {
                    return string.Empty;
                }

                return ((AssemblyProductAttribute)attributes[0]).Product;
            }
        }

        /// <summary>
        /// Gets AssemblyTitle.
        /// </summary>
        /// <remarks></remarks>
        public string AssemblyTitle
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(
                    typeof(AssemblyTitleAttribute), false);
                if (attributes.Length > 0)
                {
                    AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                    if (titleAttribute.Title != string.Empty)
                    {
                        return titleAttribute.Title;
                    }
                }

                return Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
            }
        }

        /// <summary>
        /// Gets AssemblyVersion.
        /// </summary>
        /// <remarks></remarks>
        public string AssemblyVersion
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }

        #endregion
    }
}