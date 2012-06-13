// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SpawnLoads.cs" company="">
//   
// </copyright>
// <summary>
//   The spawn loads.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HaloMap.Render
{
    using System.Collections.Generic;

    using HaloMap.Map;
    using HaloMap.Meta;
    using HaloMap.RawData;

    using Microsoft.DirectX;
    using Microsoft.DirectX.Direct3D;

    /// <summary>
    /// The spawn loads.
    /// </summary>
    /// <remarks></remarks>
    public class SpawnLoads
    {
        #region Constants and Fields

        /// <summary>
        /// 
        /// </summary>
        private Map map;

        /// <summary>
        /// The hill display.
        /// </summary>
        public Mesh[] hillDisplay = new Mesh[5];

        // objects for displaying hills
        /// <summary>
        /// The hills loaded.
        /// </summary>
        public bool hillsLoaded;

        /// <summary>
        /// The bsp.
        /// </summary>
        private readonly BSPModel bsp;

        /// <summary>
        /// The bounding box model.
        /// </summary>
        private Mesh[] BoundingBoxModel;

        /// <summary>
        /// The spawn model.
        /// </summary>
        private List<ParsedModel> SpawnModel = new List<ParsedModel>();

        /// <summary>
        /// The device.
        /// </summary>
        private Device device;

        /// <summary>
        /// The spawnmodelindex.
        /// </summary>
        private int[] spawnmodelindex;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SpawnLoads"/> class.
        /// </summary>
        /// <param name="map">The map.</param>
        /// <param name="bsp">The BSP.</param>
        /// <param name="device">The device.</param>
        /// <remarks></remarks>
        public SpawnLoads(Map map, BSPModel bsp, Device device)
        {
            this.map = map;
            this.bsp = bsp;
            this.device = device;
            SpawnModel = new List<ParsedModel>();
            spawnmodelindex = new int[bsp.Spawns.Spawn.Count];
            BoundingBoxModel = new Mesh[bsp.Spawns.Spawn.Count];
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// The load obstacles.
        /// </summary>
        /// <param name="ObstacleList">The obstacle list.</param>
        /// <remarks></remarks>
        public void LoadObstacles(ref List<SceneryInfo> ObstacleList)
        {
            if (ObstacleList == null)
            {
                ObstacleList = new List<SceneryInfo>();
            }
            else
            {
                ObstacleList.Clear();
            }

            map.OpenMap(MapTypes.Internal);

            // Lists all Obstacles
            for (int i = 0; i < map.MapHeader.fileCount; i++)
            {
                if (map.MetaInfo.TagType[i] == "scnr")
                {
                    Meta m = new Meta(map);

                    // Base address of SCNR tag, offset of Crate Palette pointer (+816)
                    map.BR.BaseStream.Position = map.MetaInfo.Offset[i] + 816;
                    int chunkCount = map.BR.ReadInt32();
                    int chunkOffset = map.BR.ReadInt32() - map.SecondaryMagic;

                    // Crate (Obstacle) Palette Objects
                    for (int a = 0; a < chunkCount; a++)
                    {
                        SceneryInfo Obstacle = new SceneryInfo();

                        // The Palette Chunk #
                        Obstacle.ScenPalNumber = a;

                        // Each chunk is 40 bytes apart
                        map.BR.BaseStream.Position = chunkOffset + a * 40;
                        char[] tagName = map.BR.ReadChars(4);
                        Obstacle.ScenTagNumber = map.Functions.ForMeta.FindMetaByID(map.BR.ReadInt32());
                        if (Obstacle.ScenTagNumber != -1)
                        {
                            // Retrieve the Model HLMT tag from the Scenery tag (+56)
                            map.BR.BaseStream.Position = map.MetaInfo.Offset[Obstacle.ScenTagNumber] + 56;
                            Obstacle.HlmtTagNumber = map.Functions.ForMeta.FindMetaByID(map.BR.ReadInt32());

                            // Base address of HLMT tag, offset of MODE pointer (+4)
                            map.BR.BaseStream.Position = map.MetaInfo.Offset[Obstacle.HlmtTagNumber] + 4;
                            Obstacle.ModelTagNumber = map.Functions.ForMeta.FindMetaByID(map.BR.ReadInt32());

                            m.ReadMetaFromMap(Obstacle.ModelTagNumber, false);
                            Obstacle.Model = new ParsedModel(ref m);
                            ParsedModel.DisplayedInfo.LoadDirectXTexturesAndBuffers(ref device, ref Obstacle.Model);

                            string[] s = map.FileNames.Name[Obstacle.ScenTagNumber].Split('\\');
                            Obstacle.Name = s[s.Length - 1];
                            Obstacle.TagPath = map.FileNames.Name[Obstacle.ScenTagNumber];
                            Obstacle.TagType = map.MetaInfo.TagType[Obstacle.ScenTagNumber];
                            ObstacleList.Add(Obstacle);
                        }
                    }

                    break;
                }
            }

            map.CloseMap();
        }

        /// <summary>
        /// The load scenery.
        /// </summary>
        /// <param name="SceneryList">The scenery list.</param>
        /// <remarks></remarks>
        public void LoadScenery(ref List<SceneryInfo> SceneryList)
        {
            if (SceneryList == null)
            {
                SceneryList = new List<SceneryInfo>();
            }
            else
            {
                SceneryList.Clear();
            }

            map.OpenMap(MapTypes.Internal);

            // Lists all Scenery
            for (int i = 0; i < map.MapHeader.fileCount; i++)
            {
                if (map.MetaInfo.TagType[i] == "scnr")
                {
                    Meta m = new Meta(map);

                    // Base address of SCNR tag, offset of Scenery Palette pointer (+88)
                    map.BR.BaseStream.Position = map.MetaInfo.Offset[i] + 88;
                    int chunkCount = map.BR.ReadInt32();
                    int chunkOffset = map.BR.ReadInt32() - map.SecondaryMagic;

                    // Scenery Palette Objects
                    for (int a = 0; a < chunkCount; a++)
                    {
                        SceneryInfo Scenery = new SceneryInfo();

                        // The Palette Chunk #
                        Scenery.ScenPalNumber = a;

                        // Each chunk is 40 bytes apart
                        map.BR.BaseStream.Position = chunkOffset + a * 40;
                        char[] tagName = map.BR.ReadChars(4);
                        Scenery.ScenTagNumber = map.Functions.ForMeta.FindMetaByID(map.BR.ReadInt32());

                        try
                        {
                            // Retrieve the Model HLMT tag from the Scenery tag (+56)
                            map.BR.BaseStream.Position = map.MetaInfo.Offset[Scenery.ScenTagNumber] + 56;
                            Scenery.HlmtTagNumber = map.Functions.ForMeta.FindMetaByID(map.BR.ReadInt32());

                            // Base address of HLMT tag, offset of MODE pointer (+4)
                            map.BR.BaseStream.Position = map.MetaInfo.Offset[Scenery.HlmtTagNumber] + 4;
                            Scenery.ModelTagNumber = map.Functions.ForMeta.FindMetaByID(map.BR.ReadInt32());

                            if (Scenery.ModelTagNumber != -1)
                            {
                                m.ReadMetaFromMap(Scenery.ModelTagNumber, false);
                                Scenery.Model = new ParsedModel(ref m);
                            }
                            else
                            {
                                Scenery.Model = null;
                            }

                            ParsedModel.DisplayedInfo.LoadDirectXTexturesAndBuffers(ref device, ref Scenery.Model);

                            string[] s = map.FileNames.Name[Scenery.ScenTagNumber].Split('\\');
                            Scenery.Name = s[s.Length - 1];
                            Scenery.TagPath = map.FileNames.Name[Scenery.ScenTagNumber];
                            Scenery.TagType = map.MetaInfo.TagType[Scenery.ScenTagNumber];
                            SceneryList.Add(Scenery);
                        }
                        catch
                        {
                        }
                    }

                    break;
                }
            }

            map.CloseMap();
        }

        /// <summary>
        /// The load sound scenery.
        /// </summary>
        /// <param name="SoundsList">The sounds list.</param>
        /// <remarks></remarks>
        public void LoadSoundScenery(ref List<SceneryInfo> SoundsList)
        {
            if (SoundsList == null)
            {
                SoundsList = new List<SceneryInfo>();
            }
            else
            {
                SoundsList.Clear();
            }

            map.OpenMap(MapTypes.Internal);

            // Lists all Scenery & Obstacles
            for (int i = 0; i < map.MapHeader.fileCount; i++)
            {
                if (map.MetaInfo.TagType[i] == "scnr")
                {
                    Meta m = new Meta(map);

                    // Base address of SCNR tag, offset of Sound Scenery Palette pointer (+224)
                    map.BR.BaseStream.Position = map.MetaInfo.Offset[i] + 224;
                    int chunkCount = map.BR.ReadInt32();
                    int chunkOffset = map.BR.ReadInt32() - map.SecondaryMagic;

                    // Scenery Palette Objects
                    for (int a = 0; a < chunkCount; a++)
                    {
                        SceneryInfo Sound = new SceneryInfo();

                        // The Palette Chunk #
                        Sound.ScenPalNumber = a;

                        // Each chunk is 40 bytes apart
                        map.BR.BaseStream.Position = chunkOffset + a * 40;
                        char[] tagName = map.BR.ReadChars(4);
                        Sound.ScenTagNumber = map.Functions.ForMeta.FindMetaByID(map.BR.ReadInt32());

                        if (Sound.ScenTagNumber != -1)
                        {
                            string[] s = map.FileNames.Name[Sound.ScenTagNumber].Split('\\');
                            Sound.Name = s[s.Length - 1];
                            Sound.TagPath = map.FileNames.Name[Sound.ScenTagNumber];
                            Sound.TagType = map.MetaInfo.TagType[Sound.ScenTagNumber];
                            SoundsList.Add(Sound);
                        }
                    }

                    break;
                }
            }

            map.CloseMap();
        }

        /// <summary>
        /// The load weapons.
        /// </summary>
        /// <param name="WeaponsList">The weapons list.</param>
        /// <remarks></remarks>
        public void LoadWeapons(ref List<CollectionInfo> WeaponsList)
        {
            if (WeaponsList == null)
            {
                WeaponsList = new List<CollectionInfo>();
            }
            else
            {
                WeaponsList.Clear();
            }

            map.OpenMap(MapTypes.Internal);

            // Lists all weapons
            for (int i = 0; i < map.MetaInfo.TagType.Length; i++)
            {
                if ((map.MetaInfo.TagType[i] == "itmc") || (map.MetaInfo.TagType[i] == "vehc"))
                {
                    CollectionInfo Weapon = new CollectionInfo();
                    Meta m = new Meta(map);
                    m.ReadMetaFromMap(i, false);

                    Weapon.ItmcTagNumber = i;

                    // Base address of ITMC tag, offset of WEAP pointer (+20)
                    map.BR.BaseStream.Position = map.MetaInfo.Offset[Weapon.ItmcTagNumber] + 20;
                    Weapon.WeapTagNumber = map.Functions.ForMeta.FindMetaByID(map.BR.ReadInt32());
                    if (Weapon.WeapTagNumber == -1)
                    {
                        continue;
                    }

                    // Base address of WEAP tag, offset of HLMT pointer (+56)
                    map.BR.BaseStream.Position = map.MetaInfo.Offset[Weapon.WeapTagNumber] + 56;
                    Weapon.HlmtTagNumber = map.Functions.ForMeta.FindMetaByID(map.BR.ReadInt32());
                    if (Weapon.HlmtTagNumber != -1)
                    {
                        // Base address of HLMT tag, offset of MODE pointer (+4)
                        map.BR.BaseStream.Position = map.MetaInfo.Offset[Weapon.HlmtTagNumber] + 4;
                        Weapon.ModelTagNumber = map.Functions.ForMeta.FindMetaByID(map.BR.ReadInt32());
                        m.ReadMetaFromMap(Weapon.ModelTagNumber, false);
                        Weapon.Model = new ParsedModel(ref m);
                        ParsedModel.DisplayedInfo.LoadDirectXTexturesAndBuffers(ref device, ref Weapon.Model);

                        // Store names into Weapon
                        Weapon.TagPath = map.FileNames.Name[i];
                        Weapon.TagType = map.MetaInfo.TagType[i];
                        int xx = map.Functions.ForMeta.FindByNameAndTagType(Weapon.TagType, Weapon.TagPath);
                        string[] NameSplit = map.FileNames.Name[xx].Split('\\');
                        Weapon.Name = NameSplit[NameSplit.Length - 1];
                        Weapon.Name = Weapon.Name.Replace('_', ' ');
                        WeaponsList.Add(Weapon);
                    }
                }
            }

            map.CloseMap();
        }

        /// <summary>
        /// The create hills.
        /// </summary>
        /// <remarks></remarks>
        public void createHills()
        {
            // Hold our possible 5 hill points listings
            List<Vector3>[] hillPoints = new List<Vector3>[5];

            for (int x = 0; x < bsp.Spawns.Spawn.Count; x++)
            {
                if (bsp.Spawns.Spawn[x].Type == SpawnInfo.SpawnType.Objective)
                {
                    SpawnInfo.ObjectiveSpawn currObj = (SpawnInfo.ObjectiveSpawn)bsp.Spawns.Spawn[x];
                    string s1 = currObj.ObjectiveType.ToString();
                    if (s1.Substring(0, s1.Length - 1) == "KingOfTheHill_")
                    {
                        int num = int.Parse(s1[s1.Length - 1].ToString()) - 1;
                        if (hillPoints[num] == null)
                        {
                            hillPoints[num] = new List<Vector3>();
                        }

                        hillPoints[num].Add(new Vector3(currObj.X, currObj.Y, currObj.Z));
                    }
                }
            }

            for (int i = 0; i < 5; i++)
            {
                if (hillPoints[i] != null)
                {
                    hillDisplay[i] = Hills.createHillObject(device, hillPoints[i]);
                }
            }

            hillsLoaded = true;
        }

        #endregion

        /// <summary>
        /// The hills.
        /// </summary>
        /// <remarks></remarks>
        internal class Hills
        {
            #region Constants and Fields

            /// <summary>
            /// The hill height.
            /// </summary>
            public static float hillHeight = 0.9f;

            #endregion

            #region Public Methods

            /// <summary>
            /// The create hill object.
            /// </summary>
            /// <param name="device">The device.</param>
            /// <param name="hillPoints">The hill points.</param>
            /// <returns></returns>
            /// <remarks></remarks>
            public static Mesh createHillObject(Device device, List<Vector3> hillPoints)
            {
                Mesh mesh;

                short[] arrayIndices = new short[(hillPoints.Count * 2 - 1) * 6];
                CustomVertex.PositionTextured[] arrayVertices = new CustomVertex.PositionTextured[hillPoints.Count * 2];
                AttributeRange attributeRange = new AttributeRange();

                // Create mesh with desired vertex format and desired size
                mesh = new Mesh(
                    arrayIndices.Length / 3,
                    arrayVertices.Length,
                    MeshFlags.SystemMemory,
                    CustomVertex.PositionTextured.Format,
                    device);

                // For each point in the height field calculate the x, y, z and
                // texture coordinates.
                for (int y = 0; y < hillPoints.Count; y++)
                {
                    CustomVertex.PositionTextured vertex = new CustomVertex.PositionTextured(
                        hillPoints[y].X, hillPoints[y].Y, hillPoints[y].Z, 0, 0);
                    arrayVertices[y * 2] = vertex;
                    vertex = new CustomVertex.PositionTextured(
                        hillPoints[y].X, hillPoints[y].Y, hillPoints[y].Z + hillHeight, 0, 0);
                    arrayVertices[y * 2 + 1] = vertex;
                }

                // Calculate the index buffer.
                for (int y = 0; y < hillPoints.Count; y++)
                {
                    int arrayIndex = (y * 2) * 6;
                    int vertexIndex = y * 2;

                    if (y != hillPoints.Count - 1)
                    {
                        arrayIndices[arrayIndex + 0] = (short)vertexIndex;
                        arrayIndices[arrayIndex + 1] = (short)(vertexIndex + 1);
                        arrayIndices[arrayIndex + 2] = (short)(vertexIndex + 2);
                        arrayIndices[arrayIndex + 3] = (short)(vertexIndex + 2);
                        arrayIndices[arrayIndex + 4] = (short)(vertexIndex + 1);
                        arrayIndices[arrayIndex + 5] = (short)(vertexIndex + 3);
                    }
                    else
                    {
                        arrayIndices[arrayIndex + 0] = (short)vertexIndex;
                        arrayIndices[arrayIndex + 1] = (short)(vertexIndex + 1);
                        arrayIndices[arrayIndex + 2] = 0;
                        arrayIndices[arrayIndex + 3] = 0;
                        arrayIndices[arrayIndex + 4] = (short)(vertexIndex + 1);
                        arrayIndices[arrayIndex + 5] = 1;
                    }
                }

                // There is only one attribute value for this mesh.
                // By specifying an attribute range the DrawSubset function
                // does not have to scan the entire mesh for all faces that are
                // are marked with a particular attribute id.
                attributeRange.AttributeId = 0;
                attributeRange.FaceStart = 0;
                attributeRange.FaceCount = arrayIndices.Length / 3;
                attributeRange.VertexStart = 0;
                attributeRange.VertexCount = arrayVertices.Length;

                mesh.VertexBuffer.SetData(arrayVertices, 0, LockFlags.None);
                mesh.IndexBuffer.SetData(arrayIndices, 0, LockFlags.None);
                mesh.SetAttributeTable(new[] { attributeRange });

                return mesh;
            }

            #endregion
        }

        /// <summary>
        /// The base info.
        /// </summary>
        /// <remarks></remarks>
        public class BaseInfo
        {
            #region Constants and Fields

            /// <summary>
            /// The hlmt tag number.
            /// </summary>
            public int HlmtTagNumber;

            /// <summary>
            /// The model.
            /// </summary>
            public ParsedModel Model;

            /// <summary>
            /// The model tag number.
            /// </summary>
            public int ModelTagNumber;

            /// <summary>
            /// The name.
            /// </summary>
            public string Name;

            /// <summary>
            /// The tag path.
            /// </summary>
            public string TagPath;

            /// <summary>
            /// The tag type.
            /// </summary>
            public string TagType;

            #endregion
        }

        /// <summary>
        /// The collection info.
        /// </summary>
        /// <remarks></remarks>
        public class CollectionInfo : BaseInfo
        {
            #region Constants and Fields

            /// <summary>
            /// The itmc tag number.
            /// </summary>
            public int ItmcTagNumber; // can also be VEHC

            /// <summary>
            /// The weap tag number.
            /// </summary>
            public int WeapTagNumber;

            #endregion
        }

        /// <summary>
        /// The scenery info.
        /// </summary>
        /// <remarks></remarks>
        public class SceneryInfo : BaseInfo
        {
            #region Constants and Fields

            /// <summary>
            /// The scen pal number.
            /// </summary>
            public int ScenPalNumber;

            /// <summary>
            /// The scen tag number.
            /// </summary>
            public int ScenTagNumber; // Can also be BLOC

            #endregion
        }
    }
}