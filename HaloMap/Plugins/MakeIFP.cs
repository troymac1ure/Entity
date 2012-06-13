// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MakeIFP.cs" company="">
//   
// </copyright>
// <summary>
//   The make ifps.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HaloMap.Plugins
{
    using System.Collections;
    using System.Collections.Generic;

    using HaloMap.Map;
    using HaloMap.Meta;

    /// <summary>
    /// The make ifps.
    /// </summary>
    /// <remarks></remarks>
    public sealed class MakeIFPS
    {
        #region Constants and Fields

        /// <summary>
        /// The results.
        /// </summary>
        public static Hashtable Results;

        #endregion

        #region Public Methods

        /// <summary>
        /// The analyze references.
        /// </summary>
        /// <param name="m">The m.</param>
        /// <remarks></remarks>
        public static void AnalyzeReferences(ref Meta m)
        {
        }

        /// <summary>
        /// The dissect meta.
        /// </summary>
        /// <param name="m">The m.</param>
        /// <remarks></remarks>
        public static void DissectMeta(ref Meta m)
        {
            object tempobject = Results[m.type];
            if (tempobject == null)
            {
                MetaResult mr = new MetaResult();
                IFPIO io = IFPHashMap.GetIfp(m.type, m.Map.HaloVersion);
                mr.IFP = io;
                Results.Add(m.type, mr);
                tempobject = Results[m.type];
            }

            MetaResult CurrentResult = tempobject as MetaResult;

            // figure out headersize for current meta
            int headersize = m.size;
            for (int x = 0; x < m.items.Count; x++)
            {
                if (m.items[x].type == Meta.ItemType.Reflexive)
                {
                    Meta.Reflexive tempr = m.items[x] as Meta.Reflexive;
                    if (tempr.translation < headersize && tempr.intag == m.TagIndex)
                    {
                        headersize = tempr.translation;
                    }
                }
            }

            if (CurrentResult.HeaderSizes.IndexOf(headersize) == -1)
            {
                CurrentResult.HeaderSizes.Add(headersize);
            }

            // 
            AnalyzeReferences(ref m);
        }

        /// <summary>
        /// The to dir.
        /// </summary>
        /// <param name="map">The map.</param>
        /// <param name="path">The path.</param>
        /// <remarks></remarks>
        public static void ToDir(Map map, string path)
        {
            if (path[path.Length - 1] != '\\')
            {
                path += "\\";
            }

            Results = new Hashtable();
            List<Meta> metas = new List<Meta>();
            for (int x = 0; x < map.IndexHeader.metaCount; x++)
            {
                Meta m = new Meta(map);
                m.ReadMetaFromMap(x, true);
                m.scanner.ScanManually();

                metas.Add(m);
            }

            for (int x = 0; x < metas.Count; x++)
            {
                Meta m = metas[x];
                DissectMeta(ref m);
                metas[x] = m;
            }
        }

        #endregion

        /// <summary>
        /// The meta result.
        /// </summary>
        /// <remarks></remarks>
        public class MetaResult
        {
            #region Constants and Fields

            /// <summary>
            /// The header sizes.
            /// </summary>
            public List<int> HeaderSizes = new List<int>();

            /// <summary>
            /// The ifp.
            /// </summary>
            public IFPIO IFP;

            /// <summary>
            /// The tag type.
            /// </summary>
            public string TagType;

            #endregion
        }
    }
}