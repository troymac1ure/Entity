// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SpawnInfo.cs" company="">
//   
// </copyright>
// <summary>
//   The spawn info.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HaloMap.Render
{
    using System;
    using System.Collections.Generic;

    using HaloMap.H2MetaContainers;
    using HaloMap.Map;
using System.IO;

    /// <summary>
    /// The spawn info.
    /// </summary>
    /// <remarks></remarks>
    public class SpawnInfo
    {
        #region Constants and Fields

        /// <summary>
        /// The spawn.
        /// </summary>
        public List<BaseSpawn> Spawn = new List<BaseSpawn>();

        /// <summary>
        /// The hillshadertag.
        /// </summary>
        public int hillshadertag = -1;

        public static string NullTags = "null";

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SpawnInfo"/> class.
        /// </summary>
        /// <param name="map">The map.</param>
        /// <remarks></remarks>
        public SpawnInfo(Map map)
        {
            switch (map.HaloVersion)
            {
                case HaloVersionEnum.HaloCE:
                case HaloVersionEnum.Halo1:
                    H1SpawnInfo(map);
                    break;
                case HaloVersionEnum.Halo2Vista:
                case HaloVersionEnum.Halo2:
                    H2SpawnInfo(map);
                    break;
            }
        }

        #endregion

        #region Enums

        /// <summary>
        /// The spawn rotation type.
        /// </summary>
        /// <remarks></remarks>
        public enum SpawnRotationType
        {
            /// <summary>
            /// The direction.
            /// </summary>
            Direction, 

            /// <summary>
            /// The yaw pitch roll.
            /// </summary>
            YawPitchRoll
        }

        /// <summary>
        /// The spawn type.
        /// </summary>
        /// <remarks></remarks>
        [FlagsAttribute]
        public enum SpawnType
        {
            Player = 0x1, 
            Weapon = 0x2, 
            Collection = 0x4, 
            Vehicle = 0x8, 
            Obstacle = 0x10, 
            Machine = 0x20, 
            Scenery = 0x40, 
            Objective = 0x80, 
            Equipment = 0x100, 
            ItemCollection = 0x200, 
            Biped = 0x400, 
            Control = 0x800, 
            Special = 0x1000, 
            DeathZone = 0x2000, 
            Camera = 0x4000, 
            Light = 0x8000, 
            Sound = 0x10000, 
            AI_Squads = 0x20000,
            SpawnZone = 0x40000,
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// The h 1 spawn info.
        /// </summary>
        /// <param name="map">The map.</param>
        /// <remarks></remarks>
        public void H1SpawnInfo(Map map)
        {
            map.OpenMap(MapTypes.Internal);

            // collections
            map.BR.BaseStream.Position = map.MetaInfo.Offset[0] + 900;
            int tempc = map.BR.ReadInt32();
            int tempr = map.BR.ReadInt32() - map.PrimaryMagic;

            for (int x = 0; x < tempc; x++)
            {
                //map.OpenMap(MapTypes.Internal);
                H1Collection vs = new H1Collection();
                map.BR.BaseStream.Position = tempr + (144 * x);
                vs.Read(map);

                map.BR.BaseStream.Position = tempr + (144 * x) + 92;
                int tempbase = map.Functions.ForMeta.FindMetaByID(map.BR.ReadInt32());
                if (tempbase == -1)
                {
                    continue;
                }

                //map.CloseMap();
                vs.TagPath = map.FileNames.Name[tempbase];
                vs.ModelTagNumber = map.Functions.FindModelByBaseClass(tempbase);
                if (vs.ModelTagNumber == -1)
                {
                    continue;
                }

                vs.ModelName = map.FileNames.Name[vs.ModelTagNumber];
                Spawn.Add(vs);
            }

            // vehicle
            //map.OpenMap(MapTypes.Internal);
            map.BR.BaseStream.Position = map.MetaInfo.Offset[0] + 588;
            int[] temppalette = new int[map.BR.ReadInt32()];
            int[] temppalette2 = new int[temppalette.Length];
            tempr = map.BR.ReadInt32() - map.PrimaryMagic;
            for (int x = 0; x < temppalette.Length; x++)
            {
                //map.OpenMap(MapTypes.Internal);
                map.BR.BaseStream.Position = tempr + (x * 48) + 12;
                int tempbase = map.Functions.ForMeta.FindMetaByID(map.BR.ReadInt32());
                //map.CloseMap();
                temppalette2[x] = tempbase;
                temppalette[x] = map.Functions.FindModelByBaseClass(tempbase);
            }

            //map.OpenMap(MapTypes.Internal);
            map.BR.BaseStream.Position = map.MetaInfo.Offset[0] + 576;
            tempc = map.BR.ReadInt32();
            tempr = map.BR.ReadInt32() - map.PrimaryMagic;

            for (int x = 0; x < tempc; x++)
            {
                map.BR.BaseStream.Position = tempr + (120 * x);
                VehicleSpawn vs = new VehicleSpawn();
                vs.Read(map);

                if (vs.PaletteIndex == -1)
                {
                    continue;
                }

                vs.ModelTagNumber = temppalette[vs.PaletteIndex];
                if (vs.ModelTagNumber == -1)
                {
                    continue;
                }

                vs.TagPath = map.FileNames.Name[temppalette2[vs.PaletteIndex]];
                vs.ModelName = map.FileNames.Name[vs.ModelTagNumber];
                Spawn.Add(vs);
            }

            //map.OpenMap(MapTypes.Internal);
            map.BR.BaseStream.Position = map.MetaInfo.Offset[0] + 540;
            temppalette = new int[map.BR.ReadInt32()];
            temppalette2 = new int[temppalette.Length];
            tempr = map.BR.ReadInt32() - map.PrimaryMagic;
            for (int x = 0; x < temppalette.Length; x++)
            {
                //map.OpenMap(MapTypes.Internal);
                map.BR.BaseStream.Position = tempr + (x * 48) + 12;
                int tempbase = map.Functions.ForMeta.FindMetaByID(map.BR.ReadInt32());
                //map.CloseMap();
                temppalette2[x] = tempbase;
                temppalette[x] = map.Functions.FindModelByBaseClass(tempbase);
            }

            //map.OpenMap(MapTypes.Internal);
            map.BR.BaseStream.Position = map.MetaInfo.Offset[0] + 528;
            tempc = map.BR.ReadInt32();
            tempr = map.BR.ReadInt32() - map.PrimaryMagic;

            for (int x = 0; x < tempc; x++)
            {
                ScenerySpawn ss = new ScenerySpawn();
                map.BR.BaseStream.Position = tempr + (72 * x) + 8;
                ss.Read(map);

                if (ss.PaletteIndex == -1)
                {
                    continue;
                }

                ss.ModelTagNumber = temppalette[ss.PaletteIndex];
                if (ss.ModelTagNumber == -1)
                {
                    continue;
                }

                ss.TagPath = map.FileNames.Name[temppalette2[ss.PaletteIndex]];
                ss.ModelName = map.FileNames.Name[ss.ModelTagNumber];
                Spawn.Add(ss);
            }
        }

        /// <summary>
        /// The h 2 spawn info.
        /// </summary>
        /// <param name="map">The map.</param>
        /// <remarks></remarks>
        public void H2SpawnInfo(Map map)
        {
            map.OpenMap(MapTypes.Internal);
            
            // find default mc model
            map.BR.BaseStream.Position = map.MetaInfo.Offset[0] + 308;
            int tempr = map.BR.ReadInt32() - map.SecondaryMagic;
            map.BR.BaseStream.Position = tempr + 4;
            int tempbipdtag = map.Functions.ForMeta.FindMetaByID(map.BR.ReadInt32());
            //map.CloseMap();
            int bipdmodeltag = map.Functions.FindModelByBaseClass(tempbipdtag);

            int ctfmodeltag = -1;
            int ballmodeltag = -1;
            int juggernautdmodeltag = -1;
            int assultmodeltag = -1;

            #region  //// Find objective models ////

            try
            {
                //map.OpenMap(MapTypes.Internal);
                map.BR.BaseStream.Position =
                    map.MetaInfo.Offset[
                        map.Functions.ForMeta.FindByNameAndTagType("mulg", "multiplayer\\multiplayer_globals")] + 12;
                tempr = map.BR.ReadInt32();
                if (tempr != 0)
                {
                    tempr -= map.SecondaryMagic;
                    map.BR.BaseStream.Position = tempr + 4;
                    int tempCtftag = map.Functions.ForMeta.FindMetaByID(map.BR.ReadInt32());
                    map.BR.BaseStream.Position = tempr + 12;
                    int tempBalltag = map.Functions.ForMeta.FindMetaByID(map.BR.ReadInt32());

                    map.BR.BaseStream.Position = tempr + 36;
                    int tempHillShader = map.Functions.ForMeta.FindMetaByID(map.BR.ReadInt32());

                    // I believe the Hill Shader above is not used, but just in case. Otherwise, load the usual one below
                    if (tempHillShader == -1)
                    {
                        map.BR.BaseStream.Position = tempr + 1332;
                        int tempr2 = map.BR.ReadInt32();
                        if (tempr2 != 0)
                        {
                            tempr2 -= map.SecondaryMagic;
                            map.BR.BaseStream.Position = tempr2 + 196;
                            tempHillShader = map.Functions.ForMeta.FindMetaByID(map.BR.ReadInt32());
                        }
                    }

                    map.BR.BaseStream.Position = tempr + 52;
                    int tempJuggernauttag = map.Functions.ForMeta.FindMetaByID(map.BR.ReadInt32());
                    map.BR.BaseStream.Position = tempr + 60;
                    int tempAssaulttag = map.Functions.ForMeta.FindMetaByID(map.BR.ReadInt32());
                    //map.CloseMap();

                    ctfmodeltag = map.Functions.FindModelByBaseClass(tempCtftag);
                    ballmodeltag = map.Functions.FindModelByBaseClass(tempBalltag);

                    // *** This is not right. It's a shader, not a model. But I don't know how to display a shader...
                    hillshadertag = map.Functions.ForMeta.FindMetaByID(tempHillShader);

                    juggernautdmodeltag = map.Functions.FindModelByBaseClass(tempJuggernauttag);
                    assultmodeltag = map.Functions.FindModelByBaseClass(tempAssaulttag);
                }
            }
            catch (Exception e)
            {
               System.Windows.Forms.MessageBox.Show("Error loading an objective model (CTF/Juggernaut/Assault/Bomb)\n" + e.Message);
            }
            
            #endregion

            #region //// Player Spawns ////

            try
            {
                //map.OpenMap(MapTypes.Internal);
                map.BR.BaseStream.Position = map.MetaInfo.Offset[3] + 256;
                int tempc = map.BR.ReadInt32();
                tempr = map.BR.ReadInt32() - map.SecondaryMagic;
                for (int x = 0; x < tempc; x++)
                {
                    map.BR.BaseStream.Position = tempr + (52 * x);
                    PlayerSpawn ps = new PlayerSpawn();                    
                    ps.Read(map);

                    ps.ModelTagNumber = bipdmodeltag;
                    ps.ModelName = map.FileNames.Name[ps.ModelTagNumber];

                    Spawn.Add(ps);
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error loading player spawns", e);
            }

            #endregion

            #region //// trigger volumes / death zones ////

            try
            {
                map.BR.BaseStream.Position = map.MetaInfo.Offset[3] + 264;
                int tempc = map.BR.ReadInt32();
                tempr = map.BR.ReadInt32() - map.SecondaryMagic;
                for (int x = 0; x < tempc; x++)
                {
                    DeathZone tv = new DeathZone();
                    map.BR.BaseStream.Position = tempr + (68 * x);
                    tv.Read(map);
                    
                    Spawn.Add(tv);
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error loading death zones", e);
            }

            #endregion

            #region //// lights ////
            
            try
            {
                //// palette ////
                map.BR.BaseStream.Position = map.MetaInfo.Offset[3] + 240;
                int[] temppalette = new int[map.BR.ReadInt32()];
                int[] temppalette2 = new int[temppalette.Length];
                tempr = map.BR.ReadInt32() - map.SecondaryMagic;
                for (int x = 0; x < temppalette.Length; x++)
                {
                    map.BR.BaseStream.Position = tempr + (x * 40) + 4;
                    int tempbase = map.Functions.ForMeta.FindMetaByID(map.BR.ReadInt32());
                    temppalette2[x] = tempbase;
                    temppalette[x] = tempbase;
                }

                //// placement ////
                map.BR.BaseStream.Position = map.MetaInfo.Offset[3] + 232;
                int tempc = map.BR.ReadInt32();
                tempr = map.BR.ReadInt32() - map.SecondaryMagic;
                for (int x = 0; x < tempc; x++)
                {
                    map.BR.BaseStream.Position = tempr + (108 * x);
                    short tempshort = map.BR.ReadInt16();


                    LightSpawn ls = new LightSpawn();
                    map.BR.BaseStream.Position = tempr + (108 * x);
                    ls.Read(map);

                    if (ls.PaletteIndex == -1)
                    {
                        continue;
                    }

                    int nameIndex = temppalette2[tempshort];
                    if (nameIndex >= 0)
                        ls.TagPath = map.FileNames.Name[nameIndex];
                    ls.ModelTagNumber = temppalette[tempshort];
                    if (ls.ModelTagNumber == -1)
                    {
                        continue;
                    }

                    ls.ModelName = map.FileNames.Name[ls.ModelTagNumber];
                    Spawn.Add(ls);
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error loading Lights", e);
            }

            #endregion

            #region //// sounds ////

            try
            {
                //// Sound Scenery palette ////
                map.BR.BaseStream.Position = map.MetaInfo.Offset[3] + 224;

                int[] temppalette = new int[map.BR.ReadInt32()];
                int[] temppalette2 = new int[temppalette.Length];
                tempr = map.BR.ReadInt32() - map.SecondaryMagic;
                for (int x = 0; x < temppalette.Length; x++)
                {
                    map.BR.BaseStream.Position = tempr + (x * 40) + 4;
                    int tempbase = map.Functions.ForMeta.FindMetaByID(map.BR.ReadInt32());
                    temppalette2[x] = tempbase;
                    temppalette[x] = tempbase;
                }

                //// placement ////
                map.BR.BaseStream.Position = map.MetaInfo.Offset[3] + 216;
                int tempc = map.BR.ReadInt32();
                tempr = map.BR.ReadInt32() - map.SecondaryMagic;
                for (int x = 0; x < tempc; x++)
                {
                    SoundSpawn ss = new SoundSpawn();
                    map.BR.BaseStream.Position = tempr + (80 * x);
                    ss.Read(map);

                    if (ss.PaletteIndex == -1 || temppalette2[ss.PaletteIndex] == -1)
                    {
                        ss.TagPath = NullTags;
                    }
                    else
                    {
                        ss.TagPath = map.FileNames.Name[temppalette2[ss.PaletteIndex]];
                    }

                    ss.ModelTagNumber = temppalette[ss.PaletteIndex];
                    if (ss.ModelTagNumber == -1)
                    {
                        // { continue; }
                        ss.ModelName = null;
                    }
                    else
                    {
                        ss.ModelName = map.FileNames.Name[ss.ModelTagNumber];
                    }

                    Spawn.Add(ss);
                }

            }
            catch (Exception e)
            {
                throw new Exception("Error loading sounds", e);
            }

            #endregion

            #region //// objectives ////

            try
            {
                map.BR.BaseStream.Position = map.MetaInfo.Offset[3] + 280;
                int tempc = map.BR.ReadInt32();
                tempr = map.BR.ReadInt32() - map.SecondaryMagic;
                for (int x = 0; x < tempc; x++)
                {
                    ObjectiveSpawn os = new ObjectiveSpawn();
                    map.BR.BaseStream.Position = tempr + (32 * x);
                    os.Read(map);

                    if (os.ObjectiveType == ObjectiveSpawn.ObjectiveTypeEnum.OddballSpawn && ballmodeltag != -1)
                    {
                        os.ModelTagNumber = ballmodeltag;
                    }
                    else if (os.ObjectiveType == ObjectiveSpawn.ObjectiveTypeEnum.CTFRespawn && ctfmodeltag != -1)
                    {
                        os.ModelTagNumber = ctfmodeltag;
                    }
                    else if (
                        os.ObjectiveType.ToString().StartsWith(
                            ObjectiveSpawn.ObjectiveTypeEnum.KingOfTheHill_1.ToString().Substring(0, 13)) &&
                        ctfmodeltag != -1)
                    {
                        os.ModelTagNumber = ctfmodeltag;
                    }
                    else if (os.ObjectiveType == ObjectiveSpawn.ObjectiveTypeEnum.AssaultRespawn && assultmodeltag != -1)
                    {
                        os.ModelTagNumber = assultmodeltag;
                    }
                    else
                    {
                        os.ModelTagNumber = bipdmodeltag;
                    }

                    os.ModelName = map.FileNames.Name[os.ModelTagNumber];
                    Spawn.Add(os);
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error loading lights", e);
            }
            #endregion

            #region //// vehicles ////

            try
            {
                map.BR.BaseStream.Position = map.MetaInfo.Offset[3] + 120;
                int[] temppalette = new int[map.BR.ReadInt32()];
                int[] temppalette2 = new int[temppalette.Length];
                tempr = map.BR.ReadInt32() - map.SecondaryMagic;
                for (int x = 0; x < temppalette.Length; x++)
                {
                    //map.OpenMap(MapTypes.Internal);
                    map.BR.BaseStream.Position = tempr + (x * 40) + 4;
                    int tempbase = map.Functions.ForMeta.FindMetaByID(map.BR.ReadInt32());
                    //map.CloseMap();
                    temppalette2[x] = tempbase;
                    temppalette[x] = map.Functions.FindModelByBaseClass(tempbase);
                }

                //map.OpenMap(MapTypes.Internal);
                map.BR.BaseStream.Position = map.MetaInfo.Offset[3] + 112;
                int tempc = map.BR.ReadInt32();
                tempr = map.BR.ReadInt32() - map.SecondaryMagic;

                for (int x = 0; x < tempc; x++)
                {
                    VehicleSpawn vs = new VehicleSpawn();
                    map.BR.BaseStream.Position = tempr + (84 * x);
                    vs.Read(map);

                    if (vs.PaletteIndex == -1)
                    {
                        continue;
                    }

                    vs.TagPath = map.FileNames.Name[temppalette2[vs.PaletteIndex]];
                    vs.ModelTagNumber = temppalette[vs.PaletteIndex];
                    if (vs.ModelTagNumber == -1)
                    {
                        continue;
                    }

                    vs.ModelName = map.FileNames.Name[vs.ModelTagNumber];
                    Spawn.Add(vs);
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error loading vehicles", e);
            }

            #endregion

            #region //// equipment ////

            try
            {
                map.BR.BaseStream.Position = map.MetaInfo.Offset[3] + 136;
                int[] temppalette = new int[map.BR.ReadInt32()];
                int[] temppalette2 = new int[temppalette.Length];
                tempr = map.BR.ReadInt32() - map.SecondaryMagic;
                for (int x = 0; x < temppalette.Length; x++)
                {
                    //map.OpenMap(MapTypes.Internal);
                    map.BR.BaseStream.Position = tempr + (x * 40) + 4;
                    int tempbase = map.Functions.ForMeta.FindMetaByID(map.BR.ReadInt32());
                    //map.CloseMap();
                    temppalette2[x] = tempbase;
                    temppalette[x] = map.Functions.FindModelByBaseClass(tempbase);
                }

                //map.OpenMap(MapTypes.Internal);
                map.BR.BaseStream.Position = map.MetaInfo.Offset[3] + 128;
                int tempc = map.BR.ReadInt32();
                tempr = map.BR.ReadInt32() - map.SecondaryMagic;

                for (int x = 0; x < tempc; x++)
                {
                    EquipmentSpawn es = new EquipmentSpawn();
                    map.BR.BaseStream.Position = tempr + (56 * x);
                    es.Read(map);

                    if (es.PaletteIndex == -1)
                    {
                        continue;
                    }

                    es.TagPath = map.FileNames.Name[temppalette2[es.PaletteIndex]];
                    es.ModelTagNumber = temppalette[es.PaletteIndex];
                    if (es.ModelTagNumber == -1)
                    {
                        continue;
                    }

                    es.ModelName = map.FileNames.Name[es.ModelTagNumber];
                    Spawn.Add(es);
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error loading equipment", e);
            }
            
            #endregion

            #region //// bipeds ////

            try
            {
                map.BR.BaseStream.Position = map.MetaInfo.Offset[3] + 104;
                int[] temppalette = new int[map.BR.ReadInt32()];
                int[] temppalette2 = new int[temppalette.Length];
                tempr = map.BR.ReadInt32() - map.SecondaryMagic;
                for (int x = 0; x < temppalette.Length; x++)
                {
                    //map.OpenMap(MapTypes.Internal);
                    map.BR.BaseStream.Position = tempr + (x * 40) + 4;
                    int tempbase = map.Functions.ForMeta.FindMetaByID(map.BR.ReadInt32());
                    //map.CloseMap();
                    temppalette2[x] = tempbase;
                    temppalette[x] = map.Functions.FindModelByBaseClass(tempbase);
                }

                //map.OpenMap(MapTypes.Internal);
                map.BR.BaseStream.Position = map.MetaInfo.Offset[3] + 96;
                int tempc = map.BR.ReadInt32();
                tempr = map.BR.ReadInt32() - map.SecondaryMagic;

                for (int x = 0; x < tempc; x++)
                {
                    BipedSpawn bs = new BipedSpawn();
                    map.BR.BaseStream.Position = tempr + (84 * x);
                    bs.Read(map);
                    
                    if (bs.PaletteIndex == -1)
                    {
                        continue;
                    }

                    bs.TagPath = map.FileNames.Name[temppalette2[bs.PaletteIndex]];
                    bs.ModelTagNumber = temppalette[bs.PaletteIndex];
                    if (bs.ModelTagNumber == -1)
                    {
                        continue;
                    }

                    bs.ModelName = map.FileNames.Name[bs.ModelTagNumber];
                    Spawn.Add(bs);
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error loading Bipeds", e);
            }

            #endregion

            #region //// control ////

            try
            {
                map.BR.BaseStream.Position = map.MetaInfo.Offset[3] + 192;
                int[] temppalette = new int[map.BR.ReadInt32()];
                int[] temppalette2 = new int[temppalette.Length];
                tempr = map.BR.ReadInt32() - map.SecondaryMagic;
                for (int x = 0; x < temppalette.Length; x++)
                {
                    //map.OpenMap(MapTypes.Internal);
                    map.BR.BaseStream.Position = tempr + (x * 40) + 4;
                    int tempbase = map.Functions.ForMeta.FindMetaByID(map.BR.ReadInt32());
                    //map.CloseMap();
                    temppalette2[x] = tempbase;
                    temppalette[x] = map.Functions.FindModelByBaseClass(tempbase);
                }

                //map.OpenMap(MapTypes.Internal);
                map.BR.BaseStream.Position = map.MetaInfo.Offset[3] + 184;
                int tempc = map.BR.ReadInt32();
                tempr = map.BR.ReadInt32() - map.SecondaryMagic;

                for (int x = 0; x < tempc; x++)
                {
                    ControlSpawn cs = new ControlSpawn();
                    map.BR.BaseStream.Position = tempr + (68 * x);
                    cs.Read(map);

                    if (cs.PaletteIndex == -1)
                    {
                        continue;
                    }

                    cs.TagPath = map.FileNames.Name[temppalette2[cs.PaletteIndex]];
                    cs.ModelTagNumber = temppalette[cs.PaletteIndex];
                    if (cs.ModelTagNumber == -1)
                    {
                        continue;
                    }

                    cs.ModelName = map.FileNames.Name[cs.ModelTagNumber];
                    Spawn.Add(cs);
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error loading Control", e);
            }

            #endregion

            #region //// machines ////

            try
            {
                map.BR.BaseStream.Position = map.MetaInfo.Offset[3] + 176;
                int[] temppalette = new int[map.BR.ReadInt32()];
                int[] temppalette2 = new int[temppalette.Length];
                tempr = map.BR.ReadInt32() - map.SecondaryMagic;
                for (int x = 0; x < temppalette.Length; x++)
                {
                    //map.OpenMap(MapTypes.Internal);
                    map.BR.BaseStream.Position = tempr + (x * 40) + 4;
                    int tempbase = map.Functions.ForMeta.FindMetaByID(map.BR.ReadInt32());
                    //map.CloseMap();
                    temppalette2[x] = tempbase;
                    temppalette[x] = map.Functions.FindModelByBaseClass(tempbase);
                }

                //map.OpenMap(MapTypes.Internal);
                map.BR.BaseStream.Position = map.MetaInfo.Offset[3] + 168;
                int tempc = map.BR.ReadInt32();
                tempr = map.BR.ReadInt32() - map.SecondaryMagic;

                for (int x = 0; x < tempc; x++)
                {
                    MachineSpawn ms = new MachineSpawn();
                    map.BR.BaseStream.Position = tempr + (72 * x);
                    ms.Read(map);

                    if (ms.PaletteIndex == -1)
                    {
                        continue;
                    }

                    if (temppalette2[ms.PaletteIndex] == -1 || temppalette[ms.PaletteIndex] == -1)
                    {
                        continue;
                    }

                    ms.TagPath = map.FileNames.Name[temppalette2[ms.PaletteIndex]];
                    ms.ModelTagNumber = temppalette[ms.PaletteIndex];
                    if (ms.ModelTagNumber == -1)
                    {
                        continue;
                    }

                    ms.ModelName = map.FileNames.Name[ms.ModelTagNumber];
                    Spawn.Add(ms);
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error loading Machines", e);
            }

            #endregion

            #region //// scenery ////

            try
            {
                map.BR.BaseStream.Position = map.MetaInfo.Offset[3] + 88;
                int[] temppalette = new int[map.BR.ReadInt32()];
                int[] temppalette2 = new int[temppalette.Length];
                tempr = map.BR.ReadInt32() - map.SecondaryMagic;
                for (int x = 0; x < temppalette.Length; x++)
                {
                    //map.OpenMap(MapTypes.Internal);
                    map.BR.BaseStream.Position = tempr + (x * 40) + 4;
                    int tempbase = map.Functions.ForMeta.FindMetaByID(map.BR.ReadInt32());
                    //map.CloseMap();
                    temppalette2[x] = tempbase;
                    temppalette[x] = map.Functions.FindModelByBaseClass(tempbase);
                }

                //map.OpenMap(MapTypes.Internal);
                map.BR.BaseStream.Position = map.MetaInfo.Offset[3] + 80;
                int tempc = map.BR.ReadInt32();
                tempr = map.BR.ReadInt32() - map.SecondaryMagic;

                for (int x = 0; x < tempc; x++)
                {
                    ScenerySpawn ss = new ScenerySpawn();
                    map.BR.BaseStream.Position = tempr + (92 * x);
                    ss.Read(map);

                    if (ss.PaletteIndex == -1)
                    {
                        continue;
                    }

                    if (temppalette2[ss.PaletteIndex] == -1 || temppalette[ss.PaletteIndex] == -1)
                    {
                        continue;
                    }

                    ss.TagPath = map.FileNames.Name[temppalette2[ss.PaletteIndex]];
                    ss.ModelTagNumber = temppalette[ss.PaletteIndex];
                    if (ss.ModelTagNumber == -1)
                    {
                        continue;
                    }

                    ss.ModelName = map.FileNames.Name[ss.ModelTagNumber];
                    Spawn.Add(ss);
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error loading Scenery", e);
            }

            #endregion

            #region //// weapons ////

            try
            {
                map.BR.BaseStream.Position = map.MetaInfo.Offset[3] + 152;
                int[] temppalette = new int[map.BR.ReadInt32()];
                int[] temppalette2 = new int[temppalette.Length];
                tempr = map.BR.ReadInt32() - map.SecondaryMagic;
                for (int x = 0; x < temppalette.Length; x++)
                {
                    //map.OpenMap(MapTypes.Internal);
                    map.BR.BaseStream.Position = tempr + (x * 40) + 4;
                    int tempbase = map.Functions.ForMeta.FindMetaByID(map.BR.ReadInt32());
                    //map.CloseMap();
                    temppalette2[x] = tempbase;
                    temppalette[x] = map.Functions.FindModelByBaseClass(tempbase);
                }

                //map.OpenMap(MapTypes.Internal);
                map.BR.BaseStream.Position = map.MetaInfo.Offset[3] + 144;
                int tempc = map.BR.ReadInt32();
                tempr = map.BR.ReadInt32() - map.SecondaryMagic;

                for (int x = 0; x < tempc; x++)
                {
                    WeaponSpawn ws = new WeaponSpawn();
                    map.BR.BaseStream.Position = tempr + (84 * x);
                    ws.Read(map);

                    if (ws.PaletteIndex == -1)
                    {
                        continue;
                    }

                    ws.TagPath = map.FileNames.Name[temppalette2[ws.PaletteIndex]];
                    ws.ModelTagNumber = temppalette[ws.PaletteIndex];
                    if (ws.ModelTagNumber == -1)
                    {
                        continue;
                    }

                    ws.ModelName = map.FileNames.Name[ws.ModelTagNumber];
                    Spawn.Add(ws);
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error loading Weapons", e);
            }

            #endregion

            #region //// obstacles ////

            try
            {
                map.BR.BaseStream.Position = map.MetaInfo.Offset[3] + 816;
                int[] temppalette = new int[map.BR.ReadInt32()];
                int[] temppalette2 = new int[temppalette.Length];
                tempr = map.BR.ReadInt32() - map.SecondaryMagic;
                for (int x = 0; x < temppalette.Length; x++)
                {
                    //map.OpenMap(MapTypes.Internal);
                    map.BR.BaseStream.Position = tempr + (x * 40) + 4;
                    int tempbase = map.Functions.ForMeta.FindMetaByID(map.BR.ReadInt32());
                    //map.CloseMap();
                    temppalette2[x] = tempbase;
                    temppalette[x] = map.Functions.FindModelByBaseClass(tempbase);
                }

                //map.OpenMap(MapTypes.Internal);
                map.BR.BaseStream.Position = map.MetaInfo.Offset[3] + 808;
                int tempc = map.BR.ReadInt32();
                tempr = map.BR.ReadInt32() - map.SecondaryMagic;

                for (int x = 0; x < tempc; x++)
                {
                    ObstacleSpawn os = new ObstacleSpawn();
                    map.BR.BaseStream.Position = tempr + (76 * x);
                    os.Read(map);

                    if (os.PaletteIndex == -1)
                    {
                        continue;
                    }

                    if (temppalette2[os.PaletteIndex] == -1)
                    {
                        continue;
                    }

                    os.TagPath = map.FileNames.Name[temppalette2[os.PaletteIndex]];
                    os.ModelTagNumber = temppalette[os.PaletteIndex];
                    if (os.ModelTagNumber == -1)
                    {
                        continue;
                    }

                    os.ModelName = map.FileNames.Name[os.ModelTagNumber];
                    Spawn.Add(os);
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error loading Obstacles", e);
            }

            #endregion

            #region //// collections ////

            try
            {
                map.BR.BaseStream.Position = map.MetaInfo.Offset[3] + 288;
                int tempc = map.BR.ReadInt32();
                tempr = map.BR.ReadInt32() - map.SecondaryMagic;

                for (int x = 0; x < tempc; x++)
                {
                    //map.OpenMap(MapTypes.Internal);
                    Collection collect = new Collection();
                    map.BR.BaseStream.Position = tempr + (144 * x);
                    collect.Read(map);

                    // ID Type
                    if (collect.TagPath == NullTags)
                    {
                        continue;
                    }

                    if (collect.ModelTagNumber == -1)
                    {
                        continue;
                    }

                    collect.ModelName = map.FileNames.Name[collect.ModelTagNumber];
                    Spawn.Add(collect);
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error loading Collections", e);
            }

            #endregion

            #region //// cameras ////

            try
            {
                //map.OpenMap(MapTypes.Internal);
                map.BR.BaseStream.Position = map.MetaInfo.Offset[3] + 488;
                int tempc = map.BR.ReadInt32();
                tempr = map.BR.ReadInt32() - map.SecondaryMagic;

                for (int x = 0; x < tempc; x++)
                {
                    //map.OpenMap(MapTypes.Internal);
                    CameraSpawn cs = new CameraSpawn();
                    map.BR.BaseStream.Position = tempr + (64 * x);
                    cs.Read(map);

                    Spawn.Add(cs);
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error loading Collections", e);
            }

            #endregion

            #region //// AI_Squads ////

            try
            {
                // Reading Character Palette
                map.BR.BaseStream.Position = map.MetaInfo.Offset[3] + 376;
                int[] temppalette = new int[map.BR.ReadInt32()];
                int[] temppalette2 = new int[temppalette.Length];
                tempr = map.BR.ReadInt32() - map.SecondaryMagic;
                for (int x = 0; x < temppalette.Length; x++)
                {
                    map.BR.BaseStream.Position = tempr + (x * 8) + 4;
                    int tempbase = map.Functions.ForMeta.FindMetaByID(map.BR.ReadInt32());
                    temppalette2[x] = tempbase;
                    temppalette[x] = map.Functions.FindModelByBaseClass(tempbase);
                }

                // Reading ai squads reflexive
                map.BR.BaseStream.Position = map.MetaInfo.Offset[3] + 352;
                int tempc = map.BR.ReadInt32();
                tempr = map.BR.ReadInt32() - map.SecondaryMagic;

                for (int x = 0; x < tempc; x++)
                {
                    // Reads AI Squad character index
                    map.BR.BaseStream.Position = tempr + (x * 116) + 54;
                    short charIndex = map.BR.ReadInt16();

                    // Reading locations sub reflexive
                    map.BR.BaseStream.Position = tempr + (x * 116) + 72;
                    int locc = map.BR.ReadInt32();
                    int locr = map.BR.ReadInt32() - map.SecondaryMagic;


                    for (int y = 0; y < locc; y++)
                    {
                        AI_Squads ai = new AI_Squads(x);
                        map.BR.BaseStream.Position = locr + (100 * y);
                        ai.Read(map);


                        if (charIndex != -1)
                        {
                            ai.TagPath = map.FileNames.Name[temppalette2[charIndex]];
                            ai.ModelTagNumber = temppalette[charIndex];
                            if (ai.ModelTagNumber == -1)
                            {
                                continue;
                            }

                            // ai.ModelName = map.FileNames.Name[vs.ModelTagNumber];
                            Spawn.Add(ai);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error loading AI Squads", e);
            }

            #endregion

            #region //// Spawn Zones //// 

            try
            {
                // Reading Spawn Zone Section
                // 792 = Spawn Data
                map.BR.BaseStream.Position = map.MetaInfo.Offset[3] + 792;
                int SpawnDataCount = map.BR.ReadInt32();
                tempr = map.BR.ReadInt32() - map.SecondaryMagic;

                #region //// Inital Spawn Zones ////
                // 88 = Static Initial Spawn Zones
                map.BR.BaseStream.Position = tempr + 88;
                int[] temppalette = new int[map.BR.ReadInt32()];
                int initialR = map.BR.ReadInt32() - map.SecondaryMagic;

                for (int x = 0; x < temppalette.Length; x++)
                {
                    // Each Initial Spawn Zone chunk = 48 bytes
                    map.BR.BaseStream.Position = initialR + (x * 48);
                    SpawnZone spawnZone = new SpawnZone(SpawnZoneType.Inital);
                    spawnZone.Read(map);

                    Spawn.Add(spawnZone);
                }
                #endregion

                #region //// Respawn Zones ////
                // 80 = Static Respawn Zones
                map.BR.BaseStream.Position = tempr + 80;
                temppalette = new int[map.BR.ReadInt32()];
                int respawnR = map.BR.ReadInt32() - map.SecondaryMagic;

                for (int x = 0; x < temppalette.Length; x++)
                {
                    // Each Respawn Zone chunk = 48 bytes
                    map.BR.BaseStream.Position = respawnR + (x * 48);
                    SpawnZone spawnZone = new SpawnZone(SpawnZoneType.Respawn);
                    spawnZone.Read(map);

                    Spawn.Add(spawnZone);
                }
                #endregion
            }
            catch (Exception e)
            {
                throw new Exception("Error loading Spawn Zones (Initial)", e);
            }
            #endregion

            map.CloseMap();
        }

        #endregion

        /// <summary>
        /// AI squads.
        /// </summary>
        /// <remarks></remarks>
        public class AI_Squads : RotateDirectionBaseSpawn
        {
            #region Constants and Fields

            /// <summary>
            /// The squad number
            /// </summary>
            public int squadNumber;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            /// Initializes a new instance of the <see cref="AI_Squads"/> class.
            /// </summary>
            /// <remarks></remarks>
            public AI_Squads(int SquadNumber)
            {
                this.Type = SpawnType.AI_Squads;
                this.RotationType = SpawnRotationType.Direction;
                this.squadNumber = SquadNumber;
            }

            #endregion

            #region Loading & Saving routines
            /// <summary>
            /// Reads the AI_Squads meta chunk info from the map MemoryStream.
            /// <para>map.BR.BaseStream.Position must be set to start of chunk data.</para>
            /// <para>AI_Squads scnr offset = 352 / 72</para>
            /// <para>AI_Squads chunk size = 100</para>
            /// </summary>
            /// <param name="map">The HaloMap.Map.Map</param>
            public override void Read(Map map)
            {
                this.offset = (int)map.BR.BaseStream.Position;

                /*
                short SIDv = (Int16)map.BR.ReadInt16();
                map.BR.BaseStream.Position++;
                byte SIDl = map.BR.ReadByte();
                if (SIDv >= 0 && SIDv < map.Strings.Length.Length && SIDl == map.Strings.Length[SIDv])
                    this.ModelName = map.Strings.Name[SIDv];
                else
                    this.ModelName = "empty";
                */
                map.BR.BaseStream.Position = this.offset + 4;
                this.X = map.BR.ReadSingle();
                this.Y = map.BR.ReadSingle();
                this.Z = map.BR.ReadSingle();

                // facing direction
                map.BR.BaseStream.Position = this.offset + 20;
                this.RotationDirection = map.BR.ReadSingle();
            }

            /// <summary>
            /// Writes the AI_Squads meta chunk info to the map MemoryStream.
            /// </summary>
            /// <param name="map">The HaloMap.Map.Map</param>
            public override void Write(Map map)
            {
                map.BW.BaseStream.Position = this.offset + 4;
                map.BW.Write(this.X);
                map.BW.Write(this.Y);
                map.BW.Write(this.Z);

                map.BW.BaseStream.Position = this.offset + 20;
                map.BW.Write(this.RotationDirection);
            }
            #endregion

        }

        /// <summary>
        /// The base spawn.
        /// </summary>
        /// <remarks></remarks>
        public abstract class BaseSpawn
        {
            // Used for fixing bounding box positions
            #region Constants and Fields

            /// <summary>
            /// The tag path.
            /// </summary>
            public string TagPath;

            /// <summary>
            /// The tag type.
            /// </summary>
            public string TagType;

            /// <summary>
            /// The type.
            /// </summary>
            public SpawnType Type;

            /// <summary>
            /// The x.
            /// </summary>
            public float X;

            /// <summary>
            /// The y.
            /// </summary>
            public float Y;

            /// <summary>
            /// The z.
            /// </summary>
            public float Z;

            /// <summary>
            /// Bounding Box x difference
            /// </summary>
            public float bbXDiff;

            /// <summary>
            /// Bounding Box y difference
            /// </summary>
            public float bbYDiff;

            /// <summary>
            /// Bounding Box z difference
            /// </summary>
            public float bbZDiff;

            /// <summary>
            /// Indicates if object is frozen.
            /// </summary>
            public bool frozen;

            /// <summary>
            /// The offset.
            /// </summary>
            public int offset;

            #endregion

            /// <summary>
            /// Spawns must declare a function for reading their data from the map.
            /// It is expected that the given map.BR.BaseStream.Position is set already.
            /// </summary>
            /// <param name="map"></param>
            public abstract void Read(Map map);

            /// <summary>
            /// Spawns must declare a function for writing their data to the map.
            /// It is expected that this.offset has already been set.
            /// </summary>
            public abstract void Write(Map map);
        }

        /// <summary>
        /// The biped spawn.
        /// </summary>
        /// <remarks></remarks>
        public class BipedSpawn : ScaleRotateYawPitchRollSpawn
        {
            #region Constructors and Destructors

            /// <summary>
            /// Initializes a new instance of the <see cref="BipedSpawn"/> class.
            /// </summary>
            /// <remarks></remarks>
            public BipedSpawn()
            {
                this.Type = SpawnType.Biped;
                this.RotationType = SpawnRotationType.Direction;
            }
            #endregion

            #region Loading & Saving routines
            /// <summary>
            /// Reads the Biped Spawn meta chunk info from the map MemoryStream.
            /// <para>map.BR.BaseStream.Position must be set to start of chunk data.</para>
            /// <para>BipedSpawn scnr offst = 96</para>
            /// <para>BipedSpawn chunk size = 84</para>
            /// </summary>
            /// <param name="map">The HaloMap.Map.Map</param>
            public override void Read(Map map)
            {
                this.offset = (int)map.BR.BaseStream.Position;
                base.Read(map);
            }

            /// <summary>
            /// Writes the Biped Spawn meta chunk info to the map MemoryStream.
            /// <para>BipedSpawn scnr offst = 96</para>
            /// <para>BipedSpawn chunk size = 84</para>
            /// </summary>
            /// <param name="map">The HaloMap.Map.Map</param>
            public override void Write(Map map)
            {
                base.Write(map);
            }

            #endregion
        }

        /// <summary>
        /// The bounding box spawn.
        /// </summary>
        /// <remarks></remarks>
        public abstract class BoundingBoxSpawn : BaseSpawn
        {
            #region Constants and Fields

            /// <summary>
            /// The height.
            /// </summary>
            public float height;

            /// <summary>
            /// The length.
            /// </summary>
            public float length;

            /// <summary>
            /// The width.
            /// </summary>
            public float width;

            #endregion
        }

        /// <summary>
        /// The camera spawn.
        /// </summary>
        /// <remarks></remarks>
        public class CameraSpawn : RotateYawPitchRollBaseSpawn
        {
            #region Constants and Fields

            /// <summary>
            /// The fov.
            /// </summary>
            public float fov;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            /// Initializes a new instance of the <see cref="CameraSpawn"/> class.
            /// </summary>
            /// <remarks></remarks>
            public CameraSpawn()
            {
                this.Type = SpawnType.Camera;
                this.RotationType = SpawnRotationType.YawPitchRoll;
            }

            #endregion

            #region Loading & Saving routines
            /// <summary>
            /// Reads the Camera Spawn meta chunk info from the map MemoryStream.
            /// <para>map.BR.BaseStream.Position must be set to start of chunk data.</para>
            /// <para>CameraSpawn scnr offst = 488</para>
            /// <para>CameraSpawn chunk size = 64</para>
            /// </summary>
            /// <param name="map">The HaloMap.Map.Map</param>
            public override void Read(Map map)
            {
                this.offset = (int)map.BR.BaseStream.Position;

                map.BR.BaseStream.Position = this.offset + 36;
                this.X = map.BR.ReadSingle();
                this.Y = map.BR.ReadSingle();
                this.Z = map.BR.ReadSingle();
                this.Roll = map.BR.ReadSingle();
                this.Pitch = map.BR.ReadSingle();
                this.Yaw = map.BR.ReadSingle();
                this.fov = map.BR.ReadSingle();
                this.ModelTagNumber = -1;
            }

            /// <summary>
            /// Writes the Camera Spawn meta chunk info to the map MemoryStream.
            /// <para>CameraSpawn scnr offst = 488</para>
            /// <para>CameraSpawn chunk size = 64</para>
            /// </summary>
            /// <param name="map">The HaloMap.Map.Map</param>
            public override void Write(Map map)
            {
                map.BW.BaseStream.Position = this.offset + 36;
                map.BW.Write(this.X);
                map.BW.Write(this.Y);
                map.BW.Write(this.Z);
                map.BW.Write(this.Yaw);
                map.BW.Write(this.Pitch);
                map.BW.Write(this.Roll);
                map.BW.Write(this.fov);
            }
            #endregion
        }

        /// <summary>
        /// The Collection Spawn / Netgame Equipment
        /// </summary>
        /// <remarks></remarks>
        public class Collection : RotateYawPitchRollBaseSpawn
        {
            #region Constants and Fields

            /// <summary>
            /// The spawns in mode.
            /// </summary>
            public SpawnsInEnum SpawnsInMode;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            /// Initializes a new instance of the <see cref="Collection"/> class.
            /// </summary>
            /// <remarks></remarks>
            public Collection()
            {
                this.Type = SpawnType.Collection;
                this.RotationType = SpawnRotationType.YawPitchRoll;
            }

            #endregion

            #region Enums

            /// <summary>
            /// The spawns in enum.
            /// </summary>
            /// <remarks></remarks>
            public enum SpawnsInEnum
            {
                /// <summary>
                /// The none.
                /// </summary>
                None = 0, 

                /// <summary>
                /// The ctf and assault.
                /// </summary>
                CTFAndAssault = 1, 

                /// <summary>
                /// The slayer.
                /// </summary>
                Slayer = 2, 

                /// <summary>
                /// The oddball.
                /// </summary>
                Oddball = 3, 

                /// <summary>
                /// The koth only.
                /// </summary>
                KOTHOnly = 4, 

                /// <summary>
                /// The koth only 2.
                /// </summary>
                KOTHOnly2 = 5, 

                /// <summary>
                /// The koth only 3.
                /// </summary>
                KOTHOnly3 = 6, 

                /// <summary>
                /// The juggernaut.
                /// </summary>
                Juggernaut = 7, 

                /// <summary>
                /// The territories.
                /// </summary>
                Territories = 8, 

                /// <summary>
                /// The ctf and assault 2.
                /// </summary>
                CTFAndAssault2 = 9, 

                /// <summary>
                /// The koth only 4.
                /// </summary>
                KOTHOnly4 = 10, 

                /// <summary>
                /// The koth only 5.
                /// </summary>
                KOTHOnly5 = 11, 

                /// <summary>
                /// The all.
                /// </summary>
                All = 12, 

                /// <summary>
                /// The not ctf or assault.
                /// </summary>
                NotCTFOrAssault = 13, 

                /// <summary>
                /// The not ct for assault 2.
                /// </summary>
                NotCTForAssault2 = 14
            }

            #endregion

            #region Loading & Saving routines
            /// <summary>
            /// Reads the Collection Spawn meta chunk info from the map MemoryStream.
            /// <para>map.BR.BaseStream.Position must be set to start of chunk data.</para>
            /// <para>Collection scnr offst = 288</para>
            /// <para>Collection chunk size = 144</para>
            /// </summary>
            /// <param name="map">The HaloMap.Map.Map</param>
            public override void Read(Map map)
            {
                this.offset = (int)map.BR.BaseStream.Position;

                map.BR.BaseStream.Position = this.offset + 4;
                this.SpawnsInMode = (Collection.SpawnsInEnum)map.BR.ReadInt32();

                map.BR.BaseStream.Position = this.offset + 64;
                this.X = map.BR.ReadSingle();
                this.Y = map.BR.ReadSingle();
                this.Z = map.BR.ReadSingle();
                this.Roll = map.BR.ReadSingle();
                this.Pitch = map.BR.ReadSingle();
                this.Yaw = map.BR.ReadSingle();

                char[] c = map.BR.ReadChars(4);
                Array.Reverse(c);
                this.TagType = new string(c);

                // Tag Path ID
                int TagPathIndex = map.Functions.ForMeta.FindMetaByID(map.BR.ReadInt32());
                if (TagPathIndex != -1)
                    this.TagPath = map.FileNames.Name[TagPathIndex];
                else
                    this.TagPath = NullTags;
                
                this.ModelTagNumber = map.Functions.FindModelByBaseClass(TagPathIndex);

            }

            /// <summary>
            /// Writes the Collection Spawn meta chunk info to the map MemoryStream.
            /// <para>Collection scnr offst = 288</para>
            /// <para>Collection chunk size = 144</para>
            /// </summary>
            /// <param name="map">The HaloMap.Map.Map</param>
            public override void Write(Map map)
            {
                map.BW.BaseStream.Position = this.offset + 4;
                map.BW.Write((int)this.SpawnsInMode);

                map.BW.BaseStream.Position = this.offset + 64;
                map.BW.Write(this.X);
                map.BW.Write(this.Y);
                map.BW.Write(this.Z);
                map.BW.Write(this.Roll);
                map.BW.Write(this.Pitch);
                map.BW.Write(this.Yaw);
                
                // reverse tag type (CMTI, IHEV, etc)
                char[] c = this.TagType.ToCharArray();
                Array.Reverse(c);
                map.BW.Write(c);

                int TagNum = map.Functions.ForMeta.FindByNameAndTagType(this.TagType, this.TagPath);
                if (TagNum != -1)
                    map.BW.Write(map.MetaInfo.Ident[TagNum]);
                else
                    map.BW.Write(TagNum);

            }
            #endregion
        }

        /// <summary>
        /// The control spawn.
        /// </summary>
        /// <remarks></remarks>
        public class ControlSpawn : ScaleRotateYawPitchRollSpawn
        {
            #region Constructors and Destructors

            /// <summary>
            /// Initializes a new instance of the <see cref="ControlSpawn"/> class.
            /// </summary>
            /// <remarks></remarks>
            public ControlSpawn()
            {
                this.Type = SpawnType.Control;
                this.RotationType = SpawnRotationType.Direction;
            }

            #endregion

            #region Loading & Saving routines
            /// <summary>
            /// Reads the Control Spawn meta chunk info from the map MemoryStream.
            /// <para>map.BR.BaseStream.Position must be set to start of chunk data.</para>
            /// <para>ControlSpawn scnr offst = 184</para>
            /// <para>ControlSpawn chunk size = 68</para>
            /// </summary>
            /// <param name="map">The HaloMap.Map.Map</param>
            public override void Read(Map map)
            {
                this.offset = (int)map.BR.BaseStream.Position;
                base.Read(map);
            }

            /// <summary>
            /// Writes the Control Spawn meta chunk info to the map MemoryStream.
            /// <para>ControlSpawn scnr offst = 184</para>
            /// <para>ControlSpawn chunk size = 68</para>
            /// </summary>
            /// <param name="map">The HaloMap.Map.Map</param>
            public override void Write(Map map)
            {
                base.Write(map);
                //map.BW.BaseStream.Position = this.offset;
            }

            #endregion
        }

        /// <summary>
        /// Trigger Volumes / Death Zones
        /// </summary>
        /// <remarks></remarks>
        public class DeathZone : BoundingBoxSpawn
        {
            #region Constants and Fields

            /// <summary>
            /// The name.
            /// </summary>
            public string Name;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            /// Initializes a new instance of the <see cref="DeathZone"/> class.
            /// </summary>
            /// <remarks></remarks>
            public DeathZone()
            {
                this.Type = SpawnType.DeathZone;
            }

            #endregion

            #region Loading & Saving routines
            /// <summary>
            /// Reads the Trigger Volume meta chunk info from the map MemoryStream.
            /// <para>map.BR.BaseStream.Position must be set to start of chunk data.</para>
            /// <para>Trigger Volume chunk size = 68</para>
            /// </summary>
            /// <param name="map">The HaloMap.Map.Map</param>
            public override void Read(Map map)
            {
                this.offset = (int)map.BR.BaseStream.Position;

                // Load the deathzone name
                this.Name = map.Strings.Name[map.BR.ReadInt16()];

                map.BR.BaseStream.Position = this.offset + 36;
                // Load the deathzone coordinates
                this.X = map.BR.ReadSingle();
                this.Y = map.BR.ReadSingle();
                this.Z = map.BR.ReadSingle();

                // Use ABS() to make sure our sizes are always positive
                this.width = Math.Abs(map.BR.ReadSingle());
                this.height = Math.Abs(map.BR.ReadSingle());
                this.length = Math.Abs(map.BR.ReadSingle());

                // Deathzones are saved with a centre point and Width, Length, Height
                this.X += this.width / 2;
                this.Y += this.height / 2;
                this.Z += this.length / 2;
            }

            /// <summary>
            /// Writes the Trigger Volume meta chunk info to the map MemoryStream.
            /// </summary>
            /// <param name="map">The HaloMap.Map.Map</param>
            public override void Write(Map map)
            {
                map.BW.BaseStream.Position = this.offset + 36;
                // Deathzones are saved as a midpoint and width, length & height, so save as midpoint
                map.BW.Write(this.X - this.width / 2);
                map.BW.Write(this.Y - this.height / 2);
                map.BW.Write(this.Z - this.length / 2);
                map.BW.Write(this.width);
                map.BW.Write(this.height);
                map.BW.Write(this.length);
            }
            #endregion
        }

        /// <summary>
        /// The equipment spawn.
        /// </summary>
        /// <remarks></remarks>
        public class EquipmentSpawn : ScaleRotateYawPitchRollSpawn
        {
            #region Constructors and Destructors

            /// <summary>
            /// Initializes a new instance of the <see cref="EquipmentSpawn"/> class.
            /// </summary>
            /// <remarks></remarks>
            public EquipmentSpawn()
            {
                this.Type = SpawnType.Equipment;
                this.RotationType = SpawnRotationType.Direction;
            }

            #endregion

            #region Loading & Saving routines
            /// <summary>
            /// Reads the Equipment Spawn meta chunk info from the map MemoryStream.
            /// <para>map.BR.BaseStream.Position must be set to start of chunk data.</para>
            /// <para>EquipmentSpawn scnr offst = 128</para>
            /// <para>EquipmentSpawn chunk size = 56</para>
            /// </summary>
            /// <param name="map">The HaloMap.Map.Map</param>
            public override void Read(Map map)
            {
                this.offset = (int)map.BR.BaseStream.Position;
                base.Read(map);
            }

            /// <summary>
            /// Writes the Equipment Spawn meta chunk info to the map MemoryStream.
            /// <para>EquipmentSpawn scnr offst = 128</para>
            /// <para>EquipmentSpawn chunk size = 56</para>
            /// </summary>
            /// <param name="map">The HaloMap.Map.Map</param>
            public override void Write(Map map)
            {
                base.Write(map);
                //map.BW.BaseStream.Position = this.offset;
            }

            #endregion
        }

        /// <summary>
        /// The h 1 collection.
        /// </summary>
        /// <remarks></remarks>
        public class H1Collection : RotateDirectionBaseSpawn
        {
            #region Constants and Fields

            /// <summary>
            /// The spawns in mode.
            /// </summary>
            public H1CollectionTypeEnum SpawnsInMode;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            /// Initializes a new instance of the <see cref="H1Collection"/> class.
            /// </summary>
            /// <remarks></remarks>
            public H1Collection()
            {
                this.Type = SpawnType.Collection;
                this.RotationType = SpawnRotationType.Direction;
            }

            #endregion

            #region Enums

            /// <summary>
            /// The h 1 collection type enum.
            /// </summary>
            /// <remarks></remarks>
            public enum H1CollectionTypeEnum
            {
                /// <summary>
                /// The none.
                /// </summary>
                None = 0, 

                /// <summary>
                /// The ctf.
                /// </summary>
                CTF = 1, 

                /// <summary>
                /// The slayer.
                /// </summary>
                Slayer = 2, 

                /// <summary>
                /// The oddball.
                /// </summary>
                Oddball = 3, 

                /// <summary>
                /// The koth.
                /// </summary>
                KOTH = 4, 

                /// <summary>
                /// The race.
                /// </summary>
                Race = 5, 

                /// <summary>
                /// The terminator.
                /// </summary>
                Terminator = 6, 

                /// <summary>
                /// The stub.
                /// </summary>
                Stub = 7, 

                /// <summary>
                /// The ignored 1.
                /// </summary>
                Ignored1 = 8, 

                /// <summary>
                /// The ignored 2.
                /// </summary>
                Ignored2 = 9, 

                /// <summary>
                /// The ignored 3.
                /// </summary>
                Ignored3 = 10, 

                /// <summary>
                /// The ignored 4.
                /// </summary>
                Ignored4 = 11, 

                /// <summary>
                /// The all.
                /// </summary>
                All = 12, 

                /// <summary>
                /// The all except ctf.
                /// </summary>
                AllExceptCTF = 13, 

                /// <summary>
                /// The all except race and ctf.
                /// </summary>
                AllExceptRaceAndCTF = 14
            }

            #endregion

            #region Loading & Saving routines
            /// <summary>
            /// Reads the Light Spawn meta chunk info from the map MemoryStream.
            /// <para>map.BR.BaseStream.Position must be set to start of chunk data.</para>
            /// <para>LightSpawn scnr offst = 900</para>
            /// <para>LightSpawn chunk size = 144</para>
            /// </summary>
            /// <param name="map">The HaloMap.Map.Map</param>
            public override void Read(Map map)
            {
                this.offset = (int)map.BR.BaseStream.Position;
                map.BR.BaseStream.Position = this.offset + 4;
                this.SpawnsInMode = (H1Collection.H1CollectionTypeEnum)map.BR.ReadInt32();

                map.BR.BaseStream.Position = this.offset + 64;
                this.X = map.BR.ReadSingle();
                this.Y = map.BR.ReadSingle();
                this.Z = map.BR.ReadSingle();
                this.RotationDirection = Renderer.DegreeToRadian(map.BR.ReadSingle());
            }

            /// <summary>
            /// Writes the Light Spawn meta chunk info to the map MemoryStream.
            /// <para>LightSpawn scnr offst = 900</para>
            /// <para>LightSpawn chunk size = 144</para>
            /// </summary>
            /// <param name="map">The HaloMap.Map.Map</param>
            public override void Write(Map map)
            {
                map.BW.BaseStream.Position = this.offset + 4;
                map.BW.Write((int)this.SpawnsInMode);

                map.BR.BaseStream.Position = this.offset + 64;
                map.BW.Write(this.X);
                map.BW.Write(this.Y);
                map.BW.Write(this.Z);
                map.BW.Write(this.RotationDirection);
            }

            #endregion
        }

        /// <summary>
        /// The light spawn.
        /// </summary>
        /// <remarks></remarks>
        public class LightSpawn : ScaleRotateYawPitchRollSpawn
        {
            #region Constants and Fields

            /// <summary>
            /// The light info.
            /// </summary>
            public HaloLight LightInfo;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            /// Initializes a new instance of the <see cref="LightSpawn"/> class.
            /// </summary>
            /// <remarks></remarks>
            public LightSpawn()
            {
                this.Type = SpawnType.Light;
                this.RotationType = SpawnRotationType.YawPitchRoll;
            }

            #endregion

            #region Loading & Saving routines
            /// <summary>
            /// Reads the Light Spawn meta chunk info from the map MemoryStream.
            /// <para>map.BR.BaseStream.Position must be set to start of chunk data.</para>
            /// <para>LightSpawn scnr offst = 232</para>
            /// <para>LightSpawn chunk size = 108</para>
            /// </summary>
            /// <param name="map">The HaloMap.Map.Map</param>
            public override void Read(Map map)
            {
                this.offset = (int)map.BR.BaseStream.Position;
                base.Read(map);
            }

            /// <summary>
            /// Writes the Light Spawn meta chunk info to the map MemoryStream.
            /// <para>LightSpawn scnr offst = 232</para>
            /// <para>LightSpawn chunk size = 108</para>
            /// </summary>
            /// <param name="map">The HaloMap.Map.Map</param>
            public override void Write(Map map)
            {
                base.Write(map);
                //map.BW.BaseStream.Position = this.offset;
            }

            #endregion
        }

        /// <summary>
        /// The machine spawn.
        /// </summary>
        /// <remarks></remarks>
        public class MachineSpawn : ScaleRotateYawPitchRollSpawn
        {
            #region Constructors and Destructors

            /// <summary>
            /// Initializes a new instance of the <see cref="MachineSpawn"/> class.
            /// </summary>
            /// <remarks></remarks>
            public MachineSpawn()
            {
                this.Type = SpawnType.Machine;
                this.RotationType = SpawnRotationType.YawPitchRoll;
            }

            #endregion

            #region Loading & Saving routines
            /// <summary>
            /// Reads the Machines meta chunk info from the map MemoryStream.
            /// <para>map.BR.BaseStream.Position must be set to start of chunk data.</para>
            /// <para>Machines scnr offst = 168</para>
            /// <para>Machines chunk size = 72</para>
            /// </summary>
            /// <param name="map">The HaloMap.Map.Map</param>
            public override void Read(Map map)
            {
                this.offset = (int)map.BR.BaseStream.Position;
                base.Read(map);
            }

            /// <summary>
            /// Writes the Machines meta chunk info to the map MemoryStream.
            /// <para>Machines scnr offst = 168</para>
            /// <para>Machines chunk size = 72</para>
            /// </summary>
            /// <param name="map">The HaloMap.Map.Map</param>
            public override void Write(Map map)
            {
                base.Write(map);
                //map.BW.BaseStream.Position = this.offset;
            }

            #endregion
        }

        /// <summary>
        /// The Objective / Netgame Flags Spawn.
        /// </summary>
        /// <remarks></remarks>
        public class ObjectiveSpawn : RotateDirectionBaseSpawn
        {
            #region Constants and Fields

            /// <summary>
            /// The objective type.
            /// </summary>
            public ObjectiveTypeEnum ObjectiveType;

            /// <summary>
            /// The team.
            /// </summary>
            public TeamType Team;

            /// <summary>
            /// The number.
            /// </summary>
            public short number;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            /// Initializes a new instance of the <see cref="ObjectiveSpawn"/> class.
            /// </summary>
            /// <remarks></remarks>
            public ObjectiveSpawn()
            {
                this.Type = SpawnType.Objective;
                this.RotationType = SpawnRotationType.Direction;
            }

            #endregion

            #region Enums

            /// <summary>
            /// The objective type enum.
            /// </summary>
            /// <remarks></remarks>
            public enum ObjectiveTypeEnum : short
            {
                /// <summary>
                /// The ctf respawn.
                /// </summary>
                CTFRespawn = 0, 

                /// <summary>
                /// The ctf score.
                /// </summary>
                CTFScore = 1, 

                /// <summary>
                /// The assault respawn.
                /// </summary>
                AssaultRespawn = 2, 

                /// <summary>
                /// The arming circle.
                /// </summary>
                ArmingCircle = 3, 

                /// <summary>
                /// The oddball spawn.
                /// </summary>
                OddballSpawn = 4, 

                /// <summary>
                /// The teleporter entrance.
                /// </summary>
                TeleporterEntrance = 7, 

                /// <summary>
                /// The teleporter exit.
                /// </summary>
                TeleporterExit = 8, 

                /// <summary>
                /// The territory.
                /// </summary>
                Territory = 10, 

                /// <summary>
                /// The king of the hill_1.
                /// </summary>
                KingOfTheHill_1 = 11, 

                /// <summary>
                /// The king of the hill_2.
                /// </summary>
                KingOfTheHill_2 = 12, 

                /// <summary>
                /// The king of the hill_3.
                /// </summary>
                KingOfTheHill_3 = 13, 

                /// <summary>
                /// The king of the hill_4.
                /// </summary>
                KingOfTheHill_4 = 14, 

                /// <summary>
                /// The king of the hill_5.
                /// </summary>
                KingOfTheHill_5 = 15, 
            }

            /// <summary>
            /// The team type.
            /// </summary>
            /// <remarks></remarks>
            public enum TeamType : short
            {
                /// <summary>
                /// The red_ defense.
                /// </summary>
                Red_Defense = 0, 

                /// <summary>
                /// The blue_ offense.
                /// </summary>
                Blue_Offense = 1, 

                /// <summary>
                /// The yellow.
                /// </summary>
                Yellow = 2, 

                /// <summary>
                /// The green.
                /// </summary>
                Green = 3, 

                /// <summary>
                /// The purple.
                /// </summary>
                Purple = 4, 

                /// <summary>
                /// The orange.
                /// </summary>
                Orange = 5, 

                /// <summary>
                /// The brown.
                /// </summary>
                Brown = 6, 

                /// <summary>
                /// The pink.
                /// </summary>
                Pink = 7, 

                /// <summary>
                /// The neutral.
                /// </summary>
                Neutral = 8
            }

            #endregion

            #region Loading & Saving routines
            /// <summary>
            /// Reads the Objective meta chunk info from the map MemoryStream.
            /// <para>map.BR.BaseStream.Position must be set to start of chunk data.</para>
            /// <para>Objective scnr offst = 280</para>
            /// <para>Objective chunk size = 32</para>
            /// </summary>
            /// <param name="map">The HaloMap.Map.Map</param>
            public override void Read(Map map)
            {
                this.offset = (int)map.BR.BaseStream.Position;
                this.X = map.BR.ReadSingle();
                this.Y = map.BR.ReadSingle();
                this.Z = map.BR.ReadSingle();
                this.RotationDirection = map.BR.ReadSingle();
                this.ObjectiveType = (ObjectiveSpawn.ObjectiveTypeEnum)map.BR.ReadInt16();
                this.Team = (ObjectiveSpawn.TeamType)map.BR.ReadInt16();
                this.number = map.BR.ReadInt16();
            }

            /// <summary>
            /// Writes the Objective meta chunk info to the map MemoryStream.
            /// <para>Objective scnr offst = 280</para>
            /// <para>Objective chunk size = 32</para>
            /// </summary>
            /// <param name="map">The HaloMap.Map.Map</param>
            public override void Write(Map map)
            {
                map.BW.BaseStream.Position = this.offset;
                map.BW.Write(this.X);
                map.BW.Write(this.Y);
                map.BW.Write(this.Z);
                map.BW.Write(this.RotationDirection);
                map.BW.Write((short)this.ObjectiveType);
                map.BW.Write((short)this.Team);
                map.BW.Write(this.number);
            }

            #endregion
        }

        /// <summary>
        /// The Obstacle / Crates spawn.
        /// </summary>
        /// <remarks></remarks>
        public class ObstacleSpawn : ScaleRotateYawPitchRollSpawn
        {
            #region Constants and Fields

            /// <summary>
            /// The bloc number.
            /// </summary>
            public int BlocNumber;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            /// Initializes a new instance of the <see cref="ObstacleSpawn"/> class.
            /// </summary>
            /// <remarks></remarks>
            public ObstacleSpawn()
            {
                this.Type = SpawnType.Obstacle;
                this.RotationType = SpawnRotationType.YawPitchRoll;
            }

            #endregion

            #region Loading & Saving routines
            /// <summary>
            /// Reads the Obstacles meta chunk info from the map MemoryStream.
            /// <para>map.BR.BaseStream.Position must be set to start of chunk data.</para>
            /// <para>Obstacles scnr offst = 808</para>
            /// <para>Obstacles chunk size = 76</para>
            /// </summary>
            /// <param name="map">The HaloMap.Map.Map</param>
            public override void Read(Map map)
            {
                this.offset = (int)map.BR.BaseStream.Position;
                base.Read(map);
            }

            /// <summary>
            /// Writes the Obstacles meta chunk info to the map MemoryStream.
            /// <para>Obstacles scnr offst = 808</para>
            /// <para>Obstacles chunk size = 76</para>
            /// </summary>
            /// <param name="map">The HaloMap.Map.Map</param>
            public override void Write(Map map)
            {
                base.Write(map);
                //map.BW.BaseStream.Position = this.offset;
            }

            #endregion
        }

        /// <summary>
        /// The player spawn.
        /// </summary>
        /// <remarks></remarks>
        public class PlayerSpawn : RotateDirectionBaseSpawn
        {
            #region Constructors and Destructors

            /// <summary>
            /// Initializes a new instance of the <see cref="PlayerSpawn"/> class.
            /// </summary>
            /// <remarks></remarks>
            public PlayerSpawn()
            {
                this.Type = SpawnType.Player;
                this.RotationType = SpawnRotationType.Direction;
                this.ModelTagNumber = -1;
                this.ModelName = string.Empty;
            }

            #endregion

            #region Loading & Saving routines
            /// <summary>
            /// Reads the Player Spawn meta chunk info from the map MemoryStream.
            /// <para>map.BR.BaseStream.Position must be set to start of chunk data.</para>
            /// <para>PlayerSpawn scnr offst = 256</para>
            /// <para>PlayerSpawn chunk size = 52</para>
            /// </summary>
            /// <param name="map">The HaloMap.Map.Map</param>
            public override void Read(Map map)
            {
                this.offset = (int)map.BR.BaseStream.Position;
                this.X = map.BR.ReadSingle();
                this.Y = map.BR.ReadSingle();
                this.Z = map.BR.ReadSingle();
                this.RotationDirection = map.BR.ReadSingle();
            }

            /// <summary>
            /// Writes the Player Spawn meta chunk info to the map MemoryStream.
            /// <para>PlayerSpawn scnr offst = 256</para>
            /// <para>PlayerSpawn chunk size = 52</para>
            /// </summary>
            /// <param name="map">The HaloMap.Map.Map</param>
            public override void Write(Map map)
            {
                map.BW.BaseStream.Position = this.offset;
                map.BW.Write(this.X);
                map.BW.Write(this.Y);
                map.BW.Write(this.Z);
                map.BW.Write(this.RotationDirection);
            }

            #endregion
        }

        /// <summary>
        /// The rotate direction base spawn.
        /// </summary>
        /// <remarks></remarks>
        public abstract class RotateDirectionBaseSpawn : RotationSpawn
        {
            #region Constants and Fields

            /// <summary>
            /// The rotation direction.
            /// </summary>
            public float RotationDirection;

            #endregion
        }

        /// <summary>
        /// The rotate yaw pitch roll base spawn.
        /// </summary>
        /// <remarks></remarks>
        public abstract class RotateYawPitchRollBaseSpawn : RotationSpawn
        {
            #region Constants and Fields

            /// <summary>
            /// The pitch.
            /// </summary>
            public float Pitch;

            /// <summary>
            /// The roll.
            /// </summary>
            public float Roll;

            /// <summary>
            /// The yaw.
            /// </summary>
            public float Yaw;

            /// <summary>
            /// The is weird.
            /// </summary>
            public bool isWeird;

            #endregion
        }

        /// <summary>
        /// The rotation spawn.
        /// </summary>
        /// <remarks></remarks>
        public abstract class RotationSpawn : BaseSpawn
        {
            #region Constants and Fields

            /// <summary>
            /// The model name.
            /// </summary>
            public string ModelName;

            /// <summary>
            /// The model tag number.
            /// </summary>
            public int ModelTagNumber;

            /// <summary>
            /// The rotation type.
            /// </summary>
            public SpawnRotationType RotationType;

            #endregion
        }

        /// <summary>
        /// The scale rotate yaw pitch roll spawn.
        /// </summary>
        /// <remarks></remarks>
        public abstract class ScaleRotateYawPitchRollSpawn : RotateYawPitchRollBaseSpawn
        {
            #region Constants and Fields

            public short PaletteIndex;
            public short NameIndex;
            public PlacementFlags Placements;
            // public float X;
            // public float Y;
            // public float Z;
            public float Scale;
            public TransformFlags Transforms;
            public ManualBSPFlags ManualBSPs;
            public uint UniqueID;
            public Int16 OriginBSP;
            public SpawnTypeEnum MetaSpawnType;
            public SourceEnum Source;
            public BSPPolicyEnum BSPPolicy;  
            private byte Unused;
            public short EditorFolder;

            #region Bitmasks and Enums
            [FlagsAttribute]
            public enum PlacementFlags : int
            {
                NotAutomatically = 0x01,
                NotOnEasy = 0x02,
                NotOnNormal = 0x04,
                NotOnHard = 0x08,
                LockTypeToEnvObject = 0x10,
                LockTransformToEnvObject = 0x20,
                NeverPlaced = 0x40,
                LockNameToEnvObject = 0x80,
                CreateAtRest = 0x100
            }
            [FlagsAttribute]
            public enum TransformFlags : ushort
            {
                Mirrored = 0x01
            }            
            [FlagsAttribute]
            public enum ManualBSPFlags : ushort 
            {
                BSP00 = 0x0001,       BSP01 = 0x0002,
                BSP02 = 0x0004,       BSP03 = 0x0008,
                BSP04 = 0x0010,       BSP05 = 0x0020,
                BSP06 = 0x0040,       BSP07 = 0x0080,
                BSP08 = 0x0100,       BSP09 = 0x0200,
                BSP10 = 0x0400,       BSP11 = 0x0800,
                BSP12 = 0x1000,       BSP13 = 0x2000,
                BSP14 = 0x4000,       BSP15 = 0x8000,
            }
            public enum SpawnTypeEnum : byte
            {
                Biped = 0,
                Vehicle = 1,
                Weapon = 2,
                Equipment = 3,
                Garbage = 4,
                Projectile = 5,
                Scenery = 6,
                Machine = 7,
                Control = 8,
                LightFixture = 9,
                SoundScenery = 10,
                Crate = 11,
                Creature = 12
            }
            public enum SourceEnum : byte
            {
                Structure = 0x00,
                Editor = 0x01,
                Dynamic = 0x02,
                Legacy = 0x03
            }
            public enum BSPPolicyEnum : byte
            {
                Default = 0x00,
                AlwaysPlaces = 0x01,
                Manual = 0x02,
            }
            #endregion

            #endregion

            /// <summary>
            /// For ScaleRotateYawPitchRollSpawn objects, the first 52 bytes are all
            /// generic for Halo 2.
            /// </summary>
            /// <param name="map">The HaloMap.Map.Map</param>
            public override void Read(Map map)
            {
                map.BR.BaseStream.Position = this.offset;
                this.PaletteIndex = map.BR.ReadInt16();
                this.NameIndex = map.BR.ReadInt16();
                this.Placements = (PlacementFlags)map.BR.ReadInt32();

                this.X = map.BR.ReadSingle();
                this.Y = map.BR.ReadSingle();
                this.Z = map.BR.ReadSingle();
                switch (map.HaloVersion)
                {
                    // Halo 1 Stores values in Degrees
                    case HaloVersionEnum.Halo1:
                        this.Yaw = Renderer.DegreeToRadian(map.BR.ReadSingle());
                        this.Pitch = Renderer.DegreeToRadian(map.BR.ReadSingle());
                        this.Roll = Renderer.DegreeToRadian(map.BR.ReadSingle());
                        break;

                    case HaloVersionEnum.Halo2:
                    case HaloVersionEnum.Halo2Vista:
                        // Is this correct? I think it should be Yaw, Pitch, Roll.
                        // Before cleaing code into this shared read section some spawns
                        // were Y,P,R and some were R,P,Y.
                        this.Yaw = map.BR.ReadSingle();
                        this.Pitch = map.BR.ReadSingle();
                        this.Roll = map.BR.ReadSingle();
                        this.Scale = map.BR.ReadSingle();

                        // None of this is currently saved
                        this.Transforms = (TransformFlags)map.BR.ReadInt16();
                        this.ManualBSPs = (ManualBSPFlags)map.BR.ReadInt16();
                        this.UniqueID = map.BR.ReadUInt32();
                        this.OriginBSP = map.BR.ReadInt16();
                        this.MetaSpawnType = (SpawnTypeEnum)map.BR.ReadByte();
                        this.Source = (SourceEnum)map.BR.ReadByte();
                        this.BSPPolicy = (BSPPolicyEnum)map.BR.ReadByte(); ;
                        this.Unused = map.BR.ReadByte();
                        this.EditorFolder = map.BR.ReadInt16();
                        break;
                }
            }

            /// <summary>
            /// For ScaleRotateYawPitchRollSpawn objects, the first 52 bytes are all generic.
            /// </summary>
            /// <param name="map">The HaloMap.Map.Map</param>
            public override void Write(Map map)
            {
                map.BW.BaseStream.Position = this.offset;
                map.BW.Write(this.PaletteIndex);
                map.BW.Write(this.NameIndex);
                map.BW.Write((int)this.Placements);

                map.BW.Write(this.X);
                map.BW.Write(this.Y);
                map.BW.Write(this.Z);
                switch (map.HaloVersion)
                {
                    // Halo 1 Stores values in Degrees
                    case HaloVersionEnum.Halo1:
                        map.BW.Write(Renderer.RadianToDegree(this.Roll));
                        map.BW.Write(Renderer.RadianToDegree(this.Pitch));
                        map.BW.Write(Renderer.RadianToDegree(this.Yaw));
                        break;

                    case HaloVersionEnum.Halo2:
                    case HaloVersionEnum.Halo2Vista:
                        map.BW.Write(this.Yaw);
                        map.BW.Write(this.Pitch);
                        map.BW.Write(this.Roll);
                        map.BW.Write(this.Scale);

                        // None of this is currently saved
                        /*
                        this.Transforms = (TransformFlags)map.BR.ReadInt16();
                        this.ManualBSPs = (ManualBSPFlags)map.BR.ReadInt16();
                        this.UniqueID = map.BR.ReadUInt32();
                        this.OriginBSP = map.BR.ReadInt16();
                        this.SpawnType = (SpawnTypeEnum)map.BR.ReadByte();
                        this.Source = (SourceEnum)map.BR.ReadByte();
                        this.BSPPolicy = (BSPPolicyEnum)map.BR.ReadByte(); ;
                        this.Unused = map.BR.ReadByte();
                        this.EditorFolder = map.BR.ReadInt16();
                        */
                        break;
                }
            }
        }

        /// <summary>
        /// The scenery spawn.
        /// </summary>
        /// <remarks></remarks>
        public class ScenerySpawn : ScaleRotateYawPitchRollSpawn
        {
            #region Constants and Fields

            /// <summary>
            /// The scen number.
            /// </summary>
            public int ScenNumber;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            /// Initializes a new instance of the <see cref="ScenerySpawn"/> class.
            /// </summary>
            /// <remarks></remarks>
            public ScenerySpawn()
            {
                this.Type = SpawnType.Scenery;
                this.RotationType = SpawnRotationType.YawPitchRoll;
            }

            #endregion

            #region Loading & Saving routines
            /// <summary>
            /// Reads the Scenery Spawn meta chunk info from the map MemoryStream.
            /// <para>map.BR.BaseStream.Position must be set to start of chunk data.</para>
            /// <para>ScenerySpawn scnr offst = 80</para>
            /// <para>ScenerySpawn chunk size = 92</para>
            /// </summary>
            /// <param name="map">The HaloMap.Map.Map</param>
            public override void Read(Map map)
            {
                this.offset = (int)map.BR.BaseStream.Position;
                base.Read(map);
            }

            /// <summary>
            /// Writes the Scenery Spawn meta chunk info to the map MemoryStream.
            /// <para>ScenerySpawn scnr offst = 80</para>
            /// <para>ScenerySpawn chunk size = 92</para>
            /// </summary>
            /// <param name="map">The HaloMap.Map.Map</param>
            public override void Write(Map map)
            {
                base.Write(map);
            }

            #endregion
        }

        /// <summary>
        /// The sound spawn.
        /// </summary>
        /// <remarks></remarks>
        public class SoundSpawn : ScaleRotateYawPitchRollSpawn
        {
            #region Constants and Fields

            /// <summary>
            /// The cone angle lower.
            /// </summary>
            public float ConeAngleLower;

            /// <summary>
            /// The cone angle upper.
            /// </summary>
            public float ConeAngleUpper;

            /// <summary>
            /// The distance bounds lower.
            /// </summary>
            public float DistanceBoundsLower;

            /// <summary>
            /// The distance bounds upper.
            /// </summary>
            public float DistanceBoundsUpper;

            /// <summary>
            /// The height.
            /// </summary>
            public float Height;

            /// <summary>
            /// The outer cone gain.
            /// </summary>
            public float OuterConeGain;

            /// <summary>
            /// The volume type.
            /// </summary>
            public int VolumeType;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            /// Initializes a new instance of the <see cref="SoundSpawn"/> class.
            /// </summary>
            /// <remarks></remarks>
            public SoundSpawn()
            {
                this.Type = SpawnType.Sound;
                this.RotationType = SpawnRotationType.YawPitchRoll;
            }
            #endregion

            #region Loading & Saving routines
            /// <summary>
            /// Reads the Sound Spawn meta chunk info from the map MemoryStream.
            /// <para>map.BR.BaseStream.Position must be set to start of chunk data.</para>
            /// <para>SoundSpawn scnr offst = 216</para>
            /// <para>SoundSpawn chunk size = 80</para>
            /// </summary>
            /// <param name="map">The HaloMap.Map.Map</param>
            public override void Read(Map map)
            {
                this.offset = (int)map.BR.BaseStream.Position;
                base.Read(map);

                map.BR.BaseStream.Position = this.offset + 54;
                this.VolumeType = map.BR.ReadInt16();
                this.Height = map.BR.ReadSingle();
                this.DistanceBoundsLower = map.BR.ReadSingle();
                this.DistanceBoundsUpper = map.BR.ReadSingle();
                this.ConeAngleLower = map.BR.ReadSingle();
                this.ConeAngleUpper = map.BR.ReadSingle();
                this.OuterConeGain = map.BR.ReadSingle();
            }

            /// <summary>
            /// Writes the Sound Spawn meta chunk info to the map MemoryStream.
            /// <para>SoundSpawn scnr offst = 216</para>
            /// <para>SoundSpawn chunk size = 80</para>
            /// </summary>
            /// <param name="map">The HaloMap.Map.Map</param>
            public override void Write(Map map)
            {
                base.Write(map);

                map.BW.BaseStream.Position = this.offset + 54;
                map.BW.Write((short)this.VolumeType);
                map.BW.Write(this.Height);
                map.BW.Write(this.DistanceBoundsLower);
                map.BW.Write(this.DistanceBoundsUpper);
                map.BW.Write(this.ConeAngleLower);
                map.BW.Write(this.ConeAngleUpper);
                map.BW.Write(this.OuterConeGain);
            }
            #endregion
        }

        /// <summary>
        /// Spawning Zones
        /// </summary>
        /// <remarks></remarks>
        public class SpawnZone : BaseSpawn
        {
            #region Constants and Fields

            public SpawnZoneType ZoneType;
            public string Name;
            public int TeamColor;
            public int ApplicableGames;
            public int OptionModifiers;
            public float LowerHeight;
            public float UpperHeight;
            public float InnerRadius;
            public float OuterRadius;
            public float Weight;
            #endregion

            #region Constructors and Destructors

            /// <summary>
            /// Initializes a new instance of the <see cref="SpawnZone"/> class.
            /// </summary>
            /// <remarks></remarks>
            public SpawnZone(SpawnZoneType zoneType)
            {
                this.Type = SpawnType.SpawnZone;
                this.ZoneType = zoneType;
            }

            /// <summary>
            /// Reads the Spawn Zone data from the given map's current BaseStream position
            /// </summary>
            /// <param name="map"></param>
            public override void Read(Map map)
            {
                this.offset = (int)map.BR.BaseStream.Position;
                this.Name = string.Empty;
                #region StringID Name
                short SIDIndex = map.BR.ReadInt16();
                // String IDs always have a null byte after the Index # before the length
                if (map.BR.ReadByte() == 0)
                {
                    int SIDLength = map.BR.ReadByte();
                    if (SIDLength == map.Strings.Length[SIDIndex])
                        this.Name = map.Strings.Name[SIDIndex];
                }
                this.TeamColor = map.BR.ReadInt32();
                this.ApplicableGames = map.BR.ReadInt32();
                this.OptionModifiers = map.BR.ReadInt32();
                this.X = map.BR.ReadSingle();
                this.Y = map.BR.ReadSingle();
                this.Z = map.BR.ReadSingle();
                this.LowerHeight = map.BR.ReadSingle();
                this.UpperHeight = map.BR.ReadSingle();
                this.InnerRadius = map.BR.ReadSingle();
                this.OuterRadius = map.BR.ReadSingle();
                this.Weight = map.BR.ReadSingle();
                #endregion

            }

            /// <summary>
            /// Writes the Spawn Zone data to the given map's BaseStream
            /// </summary>
            /// <param name="map"></param>
            public override void Write(Map map)
            {
                map.BW.BaseStream.Position = this.offset;

                /*
                this.Name = string.Empty;
                #region StringID Name
                short SIDIndex = map.BR.ReadInt16();
                // String IDs always have a null byte after the Index # before the length
                if (map.BR.ReadByte() == 0)
                {
                    int SIDLength = map.BR.ReadByte();
                    if (SIDLength == map.Strings.Length[SIDIndex])
                        this.Name = map.Strings.Name[SIDIndex];
                }
                */
                map.BW.BaseStream.Position += 4;

                map.BW.Write(this.TeamColor);
                map.BW.Write(this.ApplicableGames);
                map.BW.Write(this.OptionModifiers);
                map.BW.Write(this.X);
                map.BW.Write(this.Y);
                map.BW.Write(this.Z);
                map.BW.Write(this.LowerHeight);
                map.BW.Write(this.UpperHeight);
                map.BW.Write(this.InnerRadius);
                map.BW.Write(this.OuterRadius);
                map.BW.Write(this.Weight);

            }

            public class Options
            {
                public int Value;
                public string Name;
                public bool Checked;

                public Options(int value, int bitValue, string name)
                {
                    this.Value = bitValue;
                    this.Name = name;
                    this.Checked = (value & (1 << bitValue)) != 0;
                }
            }

            public Options[] GetTeamColorOptions()
            {
                Options[] options = new Options[9];
                options[0] = new Options(this.TeamColor, 0, "Red");
                options[1] = new Options(this.TeamColor, 1, "Blue");
                options[2] = new Options(this.TeamColor, 2, "Yellow");
                options[3] = new Options(this.TeamColor, 3, "Green");
                options[4] = new Options(this.TeamColor, 4, "Purple");
                options[5] = new Options(this.TeamColor, 5, "Orange");
                options[6] = new Options(this.TeamColor, 6, "Brown");
                options[7] = new Options(this.TeamColor, 7, "Pink");
                options[8] = new Options(this.TeamColor, 8, "Neutral");
                return options;
            }

            public Options[] GetGameTypeOptions()
            {
                Options[] options = new Options[9];
                options[0] = new Options(this.ApplicableGames, 0, "Slayer");
                options[1] = new Options(this.ApplicableGames, 1, "Oddball");
                options[2] = new Options(this.ApplicableGames, 2, "KOTH");
                options[3] = new Options(this.ApplicableGames, 3, "CTF");
                options[4] = new Options(this.ApplicableGames, 4, "Race");
                options[5] = new Options(this.ApplicableGames, 5, "Headhunter");
                options[6] = new Options(this.ApplicableGames, 6, "Juggernaut");
                options[7] = new Options(this.ApplicableGames, 7, "Territories");
                return options;
            }

            public Options[] GetOptionModifiers()
            {
                Options[] options = new Options[9];
                options[0] = new Options(this.OptionModifiers, 0, "Disabled If Flag Home");
                options[1] = new Options(this.OptionModifiers, 1, "Disabled If Flag Away");
                options[2] = new Options(this.OptionModifiers, 2, "Disabled If Bomb Home");
                options[3] = new Options(this.OptionModifiers, 3, "Disabled If Bomb Away");
                return options;
            }

            #endregion
        }

        /// <summary>
        /// Spawn Zone Types
        /// </summary>
        public enum SpawnZoneType
        {
            Inital = 1,
            Respawn = 2
        }

        /// <summary>
        /// The special spawn (Unused?).
        /// </summary>
        /// <remarks></remarks>
        public class SpecialSpawn : BoundingBoxSpawn
        {
            #region Constants and Fields

            /// <summary>
            /// The name.
            /// </summary>
            public string Name;

            /// <summary>
            /// The special spawn type.
            /// </summary>
            public SpecialSpawnType specialSpawnType;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            /// Initializes a new instance of the <see cref="SpecialSpawn"/> class.
            /// </summary>
            /// <remarks></remarks>
            public SpecialSpawn()
            {
                this.Type = SpawnType.Special;
            }

            #endregion

            #region Enums

            /// <summary>
            /// Special spawn types (CTF, Inital)
            /// </summary>
            /// <remarks></remarks>
            public enum SpecialSpawnType
            {
                /// <summary>
                /// The ctf.
                /// </summary>
                CTF, 

                /// <summary>
                /// The initital.
                /// </summary>
                Initital
            }

            #endregion

            #region Loading & Saving routines
            /// <summary>
            /// Reads the Special Spawn meta chunk info from the map MemoryStream.
            /// <para>map.BR.BaseStream.Position must be set to start of chunk data.</para>
            /// <para>SpecialSpawn scnr offst = ?</para>
            /// <para>SpecialSpawn chunk size = ?</para>
            /// </summary>
            /// <param name="map">The HaloMap.Map.Map</param>
            public override void Read(Map map)
            {
                this.offset = (int)map.BR.BaseStream.Position;
            }

            /// <summary>
            /// Writes the Special Spawn meta chunk info to the map MemoryStream.
            /// <para>SpecialSpawn scnr offst = ?</para>
            /// <para>SpecialSpawn chunk size = ?</para>
            /// </summary>
            /// <param name="map">The HaloMap.Map.Map</param>
            public override void Write(Map map)
            {
                map.BW.BaseStream.Position = this.offset;
            }

            #endregion
        }

        /// <summary>
        /// The vehicle spawn.
        /// </summary>
        /// <remarks></remarks>
        public class VehicleSpawn : ScaleRotateYawPitchRollSpawn
        {
            #region Constructors and Destructors

            /// <summary>
            /// Initializes a new instance of the <see cref="VehicleSpawn"/> class.
            /// </summary>
            /// <remarks></remarks>
            public VehicleSpawn()
            {
                this.Type = SpawnType.Vehicle;
                this.RotationType = SpawnRotationType.YawPitchRoll;
            }

            #endregion

            #region Loading & Saving routines
            /// <summary>
            /// Reads the Vehicle Spawn meta chunk info from the map MemoryStream.
            /// <para>map.BR.BaseStream.Position must be set to start of chunk data.</para>
            /// <para>VehicleSpawn scnr offst = Halo1: 576 Halo2: 112</para>
            /// <para>VehicleSpawn chunk size = Halo1: 120 Halo2: 84</para>
            /// </summary>
            /// <param name="map">The HaloMap.Map.Map</param>
            public override void Read(Map map)
            {
                this.offset = (int)map.BR.BaseStream.Position;
                base.Read(map);
            }

            /// <summary>
            /// Writes the Vehicle Spawn meta chunk info to the map MemoryStream.
            /// <para>VehicleSpawn scnr offst = 112</para>
            /// <para>VehicleSpawn chunk size = 84</para>
            /// </summary>
            /// <param name="map">The HaloMap.Map.Map</param>
            public override void Write(Map map)
            {
                base.Write(map);
            }
            #endregion
        }

        /// <summary>
        /// The weapon spawn.
        /// </summary>
        /// <remarks></remarks>
        public class WeaponSpawn : ScaleRotateYawPitchRollSpawn
        {
            #region Constructors and Destructors

            /// <summary>
            /// Initializes a new instance of the <see cref="WeaponSpawn"/> class.
            /// </summary>
            /// <remarks></remarks>
            public WeaponSpawn()
            {
                this.Type = SpawnType.Weapon;
                this.RotationType = SpawnRotationType.YawPitchRoll;
            }

            #endregion

            #region Loading & Saving routines
            /// <summary>
            /// Reads the Weapon Spawn meta chunk info from the map MemoryStream.
            /// <para>map.BR.BaseStream.Position must be set to start of chunk data.</para>
            /// <para>WeaponSpawn scnr offst = 144</para>
            /// <para>WeaponSpawn chunk size = 84</para>
            /// </summary>
            /// <param name="map">The HaloMap.Map.Map</param>
            public override void Read(Map map)
            {
                this.offset = (int)map.BR.BaseStream.Position;
                base.Read(map);

            }

            /// <summary>
            /// Writes the Weapon Spawn meta chunk info to the map MemoryStream.
            /// <para>WeaponSpawn scnr offst = 144</para>
            /// <para>WeaponSpawn chunk size = 84</para>
            /// </summary>
            /// <param name="map">The HaloMap.Map.Map</param>
            public override void Write(Map map)
            {
                base.Write(map);
            }
            #endregion
        }
    }
}