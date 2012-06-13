// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BSPConvert.cs" company="">
//   
// </copyright>
// <summary>
//   The bsp convert.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HaloMap.H1MetaContainers
{
    using System.Windows.Forms;

    using HaloMap.Map;

    /// <summary>
    /// The bsp convert.
    /// </summary>
    /// <remarks></remarks>
    public class H1BSPConvert
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BSPConvert"/> class.
        /// </summary>
        /// <param name="map">The map.</param>
        /// <param name="test">The test.</param>
        /// <remarks></remarks>
        public H1BSPConvert(Map map, ref PropertyGrid test)
        {
            switch (map.HaloVersion)
            {
                case HaloVersionEnum.Halo1:
                case HaloVersionEnum.HaloCE:
                    map.OpenMap(MapTypes.Internal);
                    H1SBSP h1bsp = new H1SBSP(map.BSP.sbsp[0].TagIndex, map);
                    test.SelectedObject = h1bsp.Header;
                    map.CloseMap();
                    break;
                case HaloVersionEnum.Halo2:
                case HaloVersionEnum.Halo2Vista:
                    MessageBox.Show("Open an H1 Map first");
                    break;
            }

            // OpenFileDialog open = new OpenFileDialog();
            // open.Filter = "*.map|*.map";
            // if (open.ShowDialog() == DialogResult.Cancel)  return;
            // int h2map=Maps.Add(open.FileName);
            // Halo2BSP h2bsp = new Halo2BSP(h2map);

            // Maps.Remove(h2map);
        }

        #endregion
    }
}