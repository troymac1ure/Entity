// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VideoSettings.cs" company="">
//   
// </copyright>
// <summary>
//   The video settings.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace entity.Main
{
    using System;
    using System.Windows.Forms;

    using Microsoft.DirectX.Direct3D;

    /// <summary>
    /// The video settings.
    /// </summary>
    /// <remarks></remarks>
    public partial class VideoSettings : Form
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="VideoSettings"/> class.
        /// </summary>
        /// <remarks></remarks>
        public VideoSettings()
        {
            InitializeComponent();

            this.Resize += form_resize;

            Caps hardware = Manager.GetDeviceCaps(0, DeviceType.Hardware);

            ListBox lb = new ListBox();
            lb.Dock = DockStyle.Top;
            lb.Parent = this;
            lb.Items.Add("Number of Textures: \t\t" + hardware.MaxSimultaneousTextures);
            lb.Items.Add("Max Active Lights: \t\t\t" + hardware.MaxActiveLights);
            string s = "Culling Modes: \t\t\t";
            if (hardware.PrimitiveMiscCaps.SupportsCullNone)
            {
                s += "None, ";
            }

            if (hardware.PrimitiveMiscCaps.SupportsCullClockwise)
            {
                s += "ClockWise, ";
            }

            if (hardware.PrimitiveMiscCaps.SupportsCullCounterClockwise)
            {
                s += "CounterClockWise";
            }

            if (s.Length == 18)
            {
                s += "Unsupported";
            }

            lb.Items.Add(s);
            lb.Items.Add("Supports Blending: \t\t\t" + hardware.LineCaps.SupportsBlend);
            lb.Items.Add("Supports Bump Mapping: \t\t" + hardware.TextureOperationCaps.SupportsBumpEnvironmentMap);
            lb.Items.Add("Supports Fog: \t\t\t" + hardware.LineCaps.SupportsFog);
            lb.Items.Add("Supports Inverse Source Alpha: \t" + hardware.SourceBlendCaps.SupportsInverseSourceAlpha);
            lb.Items.Add("Supports Magnify Linear: \t\t" + hardware.TextureFilterCaps.SupportsMagnifyLinear);
            lb.Items.Add("Supports Minify Linear: \t\t" + hardware.TextureFilterCaps.SupportsMinifyLinear);
            lb.Items.Add("Supports MipMap: \t\t\t" + hardware.TextureCaps.SupportsMipMap);
            lb.Items.Add("Supports MipMap Linear: \t\t" + hardware.TextureFilterCaps.SupportsMipMapLinear);
            lb.Items.Add("Supports Auto Generate MipMap: \t" + hardware.DriverCaps.CanAutoGenerateMipMap);
            lb.Items.Add("Supports Modulate: \t\t" + hardware.TextureOperationCaps.SupportsModulate);
            lb.Items.Add("Supports Modulate2X: \t\t" + hardware.TextureOperationCaps.SupportsModulate2X);
            lb.Items.Add("Supports Source Alpha: \t\t" + hardware.SourceBlendCaps.SupportsSourceAlpha);
            lb.Items.Add(
                "Supports Texture State: \t\t" + hardware.PrimitiveMiscCaps.SupportsTextureStageStateArgumentTemp);
            lb.Items.Add("Supports Transform And Light: \t" + hardware.DeviceCaps.SupportsHardwareTransformAndLight);
            lb.Items.Add("SupportsWrap: \t\t\t" + hardware.VolumeTextureAddressCaps.SupportsWrap);
            form_resize(this, new EventArgs());
        }

        #endregion

        #region Methods

        /// <summary>
        /// The form_resize.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void form_resize(object sender, EventArgs e)
        {
            ((Form)sender).Controls[0].Size = this.Size;
        }

        #endregion
    }
}