// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFPHashMap.cs" company="">
//   
// </copyright>
// <summary>
//   The ifp hash map.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HaloMap.Plugins
{
    using System;
    using System.Collections;

    using Globals;

    using HaloMap.Map;

    /// <summary>
    /// The ifp hash map.
    /// </summary>
    /// <remarks></remarks>
    public sealed class IFPHashMap
    {
        #region Constants and Fields

        /// <summary>
        /// The h 1 ifp hash.
        /// </summary>
        public static Hashtable H1IFPHash = new Hashtable();

        /// <summary>
        /// The h 2 ifp hash.
        /// </summary>
        public static Hashtable H2IFPHash = new Hashtable();

        #endregion

        #region Public Methods

        /// <summary>
        /// The get ifp.
        /// </summary>
        /// <param name="TagType">The tag type.</param>
        /// <param name="map">The map.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static IFPIO GetIfp(string TagType, HaloVersionEnum HaloVersion)
        {
            IFPIO tempifp;
            if (HaloVersion == HaloVersionEnum.Halo2 ||
                HaloVersion == HaloVersionEnum.Halo2Vista)
            {
                tempifp = (IFPIO)H2IFPHash[TagType];
                if (tempifp == null)
                {
                    tempifp = new IFPIO();

                    // string temps = Global.StartupPath + "\\plugins\\" + TagType.Trim() + ".ifp";
                    string temps = Prefs.pathPluginsFolder + "\\" + TagType.Trim() + ".ent";
                    temps = temps.Replace("<", "_");
                    temps = temps.Replace(">", "_");
                    try
                    {
                        tempifp.ReadIFP(temps);
                    }
                    catch (Exception e)
                    {
                        throw new Exception("Error Reading Ifp: " + TagType, e);
                    }

                    // IFPHashMap.H2IFPHash.Remove(TagType);
                    H2IFPHash.Add(TagType, tempifp);
                }
            }
            else
            {
                // Halo 1 or Halo CE
                tempifp = (IFPIO)H1IFPHash[TagType];
                if (tempifp == null)
                {
                    tempifp = new IFPIO();

                    // string temps = Global.StartupPath + "\\plugins\\" + TagType.Trim() + ".ifp";
                    string temps = Global.StartupPath + "\\Plugins\\Halo 1\\ent\\" + TagType.Trim() + ".ent";
                    temps = temps.Replace("<", "_");
                    temps = temps.Replace(">", "_");
                    try
                    {
                        tempifp.ReadIFP(temps /* + "2"*/);
                    }
                    catch (Exception ex)
                    {
                        Global.ShowErrorMsg("Error Reading Ifp: " + TagType, ex);
                    }

                    H1IFPHash.Add(TagType, tempifp);
                }
            }

            return tempifp;
        }

        /// <summary>
        /// The remove ifp.
        /// </summary>
        /// <param name="TagType">The tag type.</param>
        /// <param name="map">The map.</param>
        /// <remarks></remarks>
        public static void RemoveIfp(string TagType, Map map)
        {
            if (map.HaloVersion == HaloVersionEnum.Halo2)
            {
                try
                {
                    H2IFPHash.Remove(TagType);
                }
                catch
                {
                }
            }
            else
            {
                try
                {
                    H1IFPHash.Remove(TagType);
                }
                catch
                {
                }
            }
        }

        #endregion
    }
}