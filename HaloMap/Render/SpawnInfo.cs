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
            /// <summary>
            /// The player.
            /// </summary>
            Player = 1, 

            /// <summary>
            /// The weapon.
            /// </summary>
            Weapon = 2, 

            /// <summary>
            /// The collection.
            /// </summary>
            Collection = 4, 

            /// <summary>
            /// The vehicle.
            /// </summary>
            Vehicle = 8, 

            /// <summary>
            /// The obstacle.
            /// </summary>
            Obstacle = 16, 

            /// <summary>
            /// The machine.
            /// </summary>
            Machine = 32, 

            /// <summary>
            /// The scenery.
            /// </summary>
            Scenery = 64, 

            /// <summary>
            /// The objective.
            /// </summary>
            Objective = 128, 

            /// <summary>
            /// The equipment.
            /// </summary>
            Equipment = 256, 

            /// <summary>
            /// The item collection.
            /// </summary>
            ItemCollection = 512, 

            /// <summary>
            /// The biped.
            /// </summary>
            Biped = 1024, 

            /// <summary>
            /// The control.
            /// </summary>
            Control = 2048, 

            /// <summary>
            /// The special.
            /// </summary>
            Special = 4096, 

            /// <summary>
            /// The death zone.
            /// </summary>
            DeathZone = 8192, 

            /// <summary>
            /// The camera.
            /// </summary>
            Camera = 16384, 

            /// <summary>
            /// The light.
            /// </summary>
            Light = 32768, 

            /// <summary>
            /// The sound.
            /// </summary>
            Sound = 65536, 

            /// <summary>
            /// The a i_ squads.
            /// </summary>
            AI_Squads = 131072, 
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
                map.BR.BaseStream.Position = tempr + (144 * x) + 4;
                vs.SpawnsInMode = (H1Collection.H1CollectionTypeEnum)map.BR.ReadInt32();
                map.BR.BaseStream.Position = tempr + (144 * x) + 64;
                vs.offset = tempr + (144 * x) + 64;
                vs.X = map.BR.ReadSingle();
                vs.Y = map.BR.ReadSingle();
                vs.Z = map.BR.ReadSingle();
                vs.RotationDirection = Renderer.DegreeToRadian(map.BR.ReadSingle());

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
                short tempshort = map.BR.ReadInt16();

                if (tempshort == -1)
                {
                    continue;
                }

                map.BR.BaseStream.Position = tempr + (120 * x) + 8;
                VehicleSpawn vs = new VehicleSpawn();
                vs.offset = tempr + (120 * x) + 8;
                vs.X = map.BR.ReadSingle();
                vs.Y = map.BR.ReadSingle();
                vs.Z = map.BR.ReadSingle();

                vs.Yaw = Renderer.DegreeToRadian(map.BR.ReadSingle());
                vs.Pitch = Renderer.DegreeToRadian(map.BR.ReadSingle());
                vs.Roll = Renderer.DegreeToRadian(map.BR.ReadSingle());
                vs.ModelTagNumber = temppalette[tempshort];
                if (vs.ModelTagNumber == -1)
                {
                    continue;
                }

                vs.TagPath = map.FileNames.Name[temppalette2[tempshort]];
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
                map.BR.BaseStream.Position = tempr + (72 * x);
                short tempshort = map.BR.ReadInt16();

                if (tempshort == -1)
                {
                    continue;
                }

                map.BR.BaseStream.Position = tempr + (72 * x) + 8;
                ScenerySpawn vs = new ScenerySpawn();
                vs.offset = tempr + (72 * x) + 8;
                vs.X = map.BR.ReadSingle();
                vs.Y = map.BR.ReadSingle();
                vs.Z = map.BR.ReadSingle();
                vs.Yaw = Renderer.DegreeToRadian(map.BR.ReadSingle());
                vs.Pitch = Renderer.DegreeToRadian(map.BR.ReadSingle());
                vs.Roll = Renderer.DegreeToRadian(map.BR.ReadSingle());
                vs.ModelTagNumber = temppalette[tempshort];
                if (vs.ModelTagNumber == -1)
                {
                    continue;
                }

                vs.TagPath = map.FileNames.Name[temppalette2[tempshort]];
                vs.ModelName = map.FileNames.Name[vs.ModelTagNumber];
                Spawn.Add(vs);
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
                    ps.offset = tempr + (52 * x);
                    ps.X = map.BR.ReadSingle();
                    ps.Y = map.BR.ReadSingle();
                    ps.Z = map.BR.ReadSingle();
                    ps.RotationDirection = map.BR.ReadSingle();
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

            #region //// death zones ////

            try
            {
                map.BR.BaseStream.Position = map.MetaInfo.Offset[3] + 264;
                int tempc = map.BR.ReadInt32();
                tempr = map.BR.ReadInt32() - map.SecondaryMagic;
                for (int x = 0; x < tempc; x++)
                {
                    DeathZone ps = new DeathZone();

                    // Load the deathzone name
                    map.BR.BaseStream.Position = tempr + (68 * x);
                    ps.Name = map.Strings.Name[map.BR.ReadInt16()];

                    // We set the offset to 36 b/c we don't care about saving the name... right now anyways.
                    ps.offset = tempr + (68 * x) + 36;

                    // Load the deathzone coordinates
                    map.BR.BaseStream.Position = tempr + (68 * x) + 36;
                    ps.X = map.BR.ReadSingle();
                    ps.Y = map.BR.ReadSingle();
                    ps.Z = map.BR.ReadSingle();

                    // Use ABS() to make sure our sizes are always positive
                    ps.width = Math.Abs(map.BR.ReadSingle());
                    ps.height = Math.Abs(map.BR.ReadSingle());
                    ps.length = Math.Abs(map.BR.ReadSingle());

                    // Deathzones are saved with a centre point and Width, Length, Height
                    ps.X += ps.width / 2;
                    ps.Y += ps.height / 2;
                    ps.Z += ps.length / 2;

                    Spawn.Add(ps);
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

                    if (tempshort == -1)
                    {
                        continue;
                    }

                    map.BR.BaseStream.Position = tempr + (108 * x) + 8;
                    LightSpawn vs = new LightSpawn();
                    vs.offset = tempr + (108 * x) + 8;
                    vs.X = map.BR.ReadSingle();
                    vs.Y = map.BR.ReadSingle();
                    vs.Z = map.BR.ReadSingle();
                    vs.Roll = map.BR.ReadSingle();
                    vs.Pitch = map.BR.ReadSingle();
                    vs.Yaw = map.BR.ReadSingle();
                    vs.Scale = map.BR.ReadSingle();

                    vs.TagPath = map.FileNames.Name[temppalette2[tempshort]];
                    vs.ModelTagNumber = temppalette[tempshort];
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
                    map.BR.BaseStream.Position = tempr + (80 * x);
                    short tempshort = map.BR.ReadInt16();

                    if (tempshort == -1)
                    {
                        continue;
                    }

                    SoundSpawn vs = new SoundSpawn();

                    map.BR.BaseStream.Position = tempr + (80 * x) + 8;
                    vs.offset = tempr + (80 * x) + 8;
                    vs.X = map.BR.ReadSingle();
                    vs.Y = map.BR.ReadSingle();
                    vs.Z = map.BR.ReadSingle();
                    vs.Roll = map.BR.ReadSingle();
                    vs.Pitch = map.BR.ReadSingle();
                    vs.Yaw = map.BR.ReadSingle();
                    vs.Scale = map.BR.ReadSingle();

                    map.BR.BaseStream.Position = tempr + (80 * x) + 54;
                    vs.VolumeType = map.BR.ReadInt16();
                    vs.Height = map.BR.ReadSingle();
                    vs.DistanceBoundsLower = map.BR.ReadSingle();
                    vs.DistanceBoundsUpper = map.BR.ReadSingle();
                    vs.ConeAngleLower = map.BR.ReadSingle();
                    vs.ConeAngleUpper = map.BR.ReadSingle();
                    vs.OuterConeGain = map.BR.ReadSingle();

                    if (temppalette2[tempshort] == -1)
                    {
                        vs.TagPath = "Nulled Out";
                    }
                    else
                    {
                        vs.TagPath = map.FileNames.Name[temppalette2[tempshort]];
                    }

                    vs.ModelTagNumber = temppalette[tempshort];
                    if (vs.ModelTagNumber == -1)
                    {
                        // { continue; }
                        vs.ModelName = null;
                    }
                    else
                    {
                        vs.ModelName = map.FileNames.Name[vs.ModelTagNumber];
                    }

                    Spawn.Add(vs);
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
                    ObjectiveSpawn ps = new ObjectiveSpawn();
                    ps.offset = tempr + (32 * x);
                    map.BR.BaseStream.Position = tempr + (32 * x);
                    ps.X = map.BR.ReadSingle();
                    ps.Y = map.BR.ReadSingle();
                    ps.Z = map.BR.ReadSingle();
                    ps.RotationDirection = map.BR.ReadSingle();
                    ps.ObjectiveType = (ObjectiveSpawn.ObjectiveTypeEnum)map.BR.ReadInt16();
                    ps.Team = (ObjectiveSpawn.TeamType)map.BR.ReadInt16();
                    ps.number = map.BR.ReadInt16();

                    if (ps.ObjectiveType == ObjectiveSpawn.ObjectiveTypeEnum.OddballSpawn && ballmodeltag != -1)
                    {
                        ps.ModelTagNumber = ballmodeltag;
                    }
                    else if (ps.ObjectiveType == ObjectiveSpawn.ObjectiveTypeEnum.CTFRespawn && ctfmodeltag != -1)
                    {
                        ps.ModelTagNumber = ctfmodeltag;
                    }
                    else if (
                        ps.ObjectiveType.ToString().StartsWith(
                            ObjectiveSpawn.ObjectiveTypeEnum.KingOfTheHill_1.ToString().Substring(0, 13)) &&
                        ctfmodeltag != -1)
                    {
                        ps.ModelTagNumber = ctfmodeltag;
                    }
                    else if (ps.ObjectiveType == ObjectiveSpawn.ObjectiveTypeEnum.AssaultRespawn && assultmodeltag != -1)
                    {
                        ps.ModelTagNumber = assultmodeltag;
                    }
                    else
                    {
                        ps.ModelTagNumber = bipdmodeltag;
                    }

                    ps.ModelName = map.FileNames.Name[ps.ModelTagNumber];
                    Spawn.Add(ps);
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
                    map.BR.BaseStream.Position = tempr + (84 * x);
                    short tempshort = map.BR.ReadInt16();

                    if (tempshort == -1)
                    {
                        continue;
                    }

                    map.BR.BaseStream.Position = tempr + (84 * x) + 8;
                    VehicleSpawn vs = new VehicleSpawn();
                    vs.offset = tempr + (84 * x) + 8;
                    vs.X = map.BR.ReadSingle();
                    vs.Y = map.BR.ReadSingle();
                    vs.Z = map.BR.ReadSingle();
                    vs.Roll = map.BR.ReadSingle();
                    vs.Pitch = map.BR.ReadSingle();
                    vs.Yaw = map.BR.ReadSingle();
                    vs.Scale = map.BR.ReadSingle();

                    vs.TagPath = map.FileNames.Name[temppalette2[tempshort]];
                    vs.ModelTagNumber = temppalette[tempshort];
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
                    map.BR.BaseStream.Position = tempr + (56 * x);
                    short tempshort = map.BR.ReadInt16();

                    if (tempshort == -1)
                    {
                        continue;
                    }

                    map.BR.BaseStream.Position = tempr + (56 * x) + 8;
                    EquipmentSpawn vs = new EquipmentSpawn();
                    vs.offset = tempr + (56 * x) + 8;
                    vs.X = map.BR.ReadSingle();
                    vs.Y = map.BR.ReadSingle();
                    vs.Z = map.BR.ReadSingle();

                    // vs.RotationDirection = map.BR.ReadSingle();
                    vs.Roll = map.BR.ReadSingle();
                    vs.Pitch = map.BR.ReadSingle();
                    vs.Yaw = map.BR.ReadSingle();
                    vs.Scale = map.BR.ReadSingle();

                    vs.TagPath = map.FileNames.Name[temppalette2[tempshort]];
                    vs.ModelTagNumber = temppalette[tempshort];
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
                    map.BR.BaseStream.Position = tempr + (84 * x);
                    short tempshort = map.BR.ReadInt16();

                    if (tempshort == -1)
                    {
                        continue;
                    }

                    map.BR.BaseStream.Position = tempr + (84 * x) + 8;
                    BipedSpawn vs = new BipedSpawn();
                    vs.offset = tempr + (84 * x) + 8;
                    vs.X = map.BR.ReadSingle();
                    vs.Y = map.BR.ReadSingle();
                    vs.Z = map.BR.ReadSingle();
                    vs.Roll = map.BR.ReadSingle();
                    vs.Pitch = map.BR.ReadSingle();
                    vs.Yaw = map.BR.ReadSingle();
                    vs.Scale = map.BR.ReadSingle();

                    vs.TagPath = map.FileNames.Name[temppalette2[tempshort]];
                    vs.ModelTagNumber = temppalette[tempshort];
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
                    map.BR.BaseStream.Position = tempr + (68 * x);
                    short tempshort = map.BR.ReadInt16();

                    if (tempshort == -1)
                    {
                        continue;
                    }

                    map.BR.BaseStream.Position = tempr + (68 * x) + 8;
                    ControlSpawn vs = new ControlSpawn();
                    vs.offset = tempr + (68 * x) + 8;
                    vs.X = map.BR.ReadSingle();
                    vs.Y = map.BR.ReadSingle();
                    vs.Z = map.BR.ReadSingle();

                    // vs.RotationDirection = map.BR.ReadSingle();
                    vs.Roll = map.BR.ReadSingle();
                    vs.Pitch = map.BR.ReadSingle();
                    vs.Yaw = map.BR.ReadSingle();
                    vs.Scale = map.BR.ReadSingle();

                    vs.TagPath = map.FileNames.Name[temppalette2[tempshort]];
                    vs.ModelTagNumber = temppalette[tempshort];
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
                    map.BR.BaseStream.Position = tempr + (72 * x);
                    short tempshort = map.BR.ReadInt16();

                    if (tempshort == -1)
                    {
                        continue;
                    }

                    map.BR.BaseStream.Position = tempr + (72 * x) + 8;
                    MachineSpawn vs = new MachineSpawn();
                    vs.offset = tempr + (72 * x) + 8;
                    vs.X = map.BR.ReadSingle();
                    vs.Y = map.BR.ReadSingle();
                    vs.Z = map.BR.ReadSingle();
                    vs.Roll = map.BR.ReadSingle();
                    vs.Pitch = map.BR.ReadSingle();
                    vs.Yaw = map.BR.ReadSingle();
                    vs.Scale = map.BR.ReadSingle();

                    if (temppalette2[tempshort] == -1 || temppalette[tempshort] == -1)
                    {
                        continue;
                    }

                    vs.TagPath = map.FileNames.Name[temppalette2[tempshort]];
                    vs.ModelTagNumber = temppalette[tempshort];
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
                    map.BR.BaseStream.Position = tempr + (92 * x);
                    short tempshort = map.BR.ReadInt16();

                    if (tempshort == -1)
                    {
                        continue;
                    }

                    map.BR.BaseStream.Position = tempr + (92 * x) + 8;
                    ScenerySpawn vs = new ScenerySpawn();
                    vs.offset = tempr + (92 * x) + 8;
                    vs.X = map.BR.ReadSingle();
                    vs.Y = map.BR.ReadSingle();
                    vs.Z = map.BR.ReadSingle();
                    vs.Roll = map.BR.ReadSingle();
                    vs.Pitch = map.BR.ReadSingle();
                    vs.Yaw = map.BR.ReadSingle();
                    vs.Scale = map.BR.ReadSingle();

                    if (temppalette2[tempshort] == -1 || temppalette[tempshort] == -1)
                    {
                        continue;
                    }

                    vs.TagPath = map.FileNames.Name[temppalette2[tempshort]];
                    vs.ModelTagNumber = temppalette[tempshort];
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
                    map.BR.BaseStream.Position = tempr + (84 * x);
                    short tempshort = map.BR.ReadInt16();

                    if (tempshort == -1)
                    {
                        continue;
                    }

                    map.BR.BaseStream.Position = tempr + (84 * x) + 8;
                    WeaponSpawn vs = new WeaponSpawn();
                    vs.offset = tempr + (84 * x) + 8;
                    vs.X = map.BR.ReadSingle();
                    vs.Y = map.BR.ReadSingle();
                    vs.Z = map.BR.ReadSingle();
                    vs.Roll = map.BR.ReadSingle();
                    vs.Pitch = map.BR.ReadSingle();
                    vs.Yaw = map.BR.ReadSingle();
                    vs.Scale = map.BR.ReadSingle();

                    vs.TagPath = map.FileNames.Name[temppalette2[tempshort]];
                    vs.ModelTagNumber = temppalette[tempshort];
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
                    map.BR.BaseStream.Position = tempr + (76 * x);
                    short tempshort = map.BR.ReadInt16();

                    if (tempshort == -1)
                    {
                        continue;
                    }

                    map.BR.BaseStream.Position = tempr + (76 * x) + 8;
                    ObstacleSpawn vs = new ObstacleSpawn();
                    vs.offset = tempr + (76 * x) + 8;
                    vs.X = map.BR.ReadSingle();
                    vs.Y = map.BR.ReadSingle();
                    vs.Z = map.BR.ReadSingle();
                    vs.Roll = map.BR.ReadSingle();
                    vs.Pitch = map.BR.ReadSingle();
                    vs.Yaw = map.BR.ReadSingle();
                    vs.Scale = map.BR.ReadSingle();

                    if (temppalette2[tempshort] == -1)
                    {
                        continue;
                    }

                    vs.TagPath = map.FileNames.Name[temppalette2[tempshort]];
                    vs.ModelTagNumber = temppalette[tempshort];
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
                    Collection vs = new Collection();
                    map.BR.BaseStream.Position = tempr + (144 * x) + 4;
                    vs.SpawnsInMode = (Collection.SpawnsInEnum)map.BR.ReadInt32();

                    // Why do they make the offset + 64? That just confuses stuff!!
                    map.BR.BaseStream.Position = tempr + (144 * x) + 64;
                    vs.offset = tempr + (144 * x) + 64;
                    vs.X = map.BR.ReadSingle();
                    vs.Y = map.BR.ReadSingle();
                    vs.Z = map.BR.ReadSingle();
                    vs.Roll = map.BR.ReadSingle();
                    vs.Pitch = map.BR.ReadSingle();
                    vs.Yaw = map.BR.ReadSingle();

                    // test
                    // if (vs.Pitch > 0) { vs.Pitch = -vs.Pitch; vs.isWeird = true; } else { vs.isWeird = false; }
                    map.BR.BaseStream.Position = tempr + (144 * x) + 88;

                    // ID Type
                    char[] c = map.BR.ReadChars(4);
                    vs.TagType = c[3].ToString() + c[2] + c[1] + c[0];

                    // Tag Path ID
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
                    CameraSpawn vs = new CameraSpawn();
                    map.BR.BaseStream.Position = tempr + (64 * x) + 36;
                    vs.offset = tempr + (64 * x) + 36;
                    vs.X = map.BR.ReadSingle();
                    vs.Y = map.BR.ReadSingle();
                    vs.Z = map.BR.ReadSingle();
                    vs.Roll = map.BR.ReadSingle();
                    vs.Pitch = map.BR.ReadSingle();
                    vs.Yaw = map.BR.ReadSingle();
                    vs.fov = map.BR.ReadSingle();
                    vs.ModelTagNumber = -1;

                    Spawn.Add(vs);
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
                    // Reads AI Squad palette chunk
                    map.BR.BaseStream.Position = tempr + (x * 116) + 54;
                    short charNum = map.BR.ReadInt16();

                    // Reading locations sub reflexive
                    map.BR.BaseStream.Position = tempr + 72;
                    int locc = map.BR.ReadInt32();
                    int locr = map.BR.ReadInt32() - map.SecondaryMagic;

                    AI_Squads vs = new AI_Squads();

                    // chunk size * x and starting position
                    map.BR.BaseStream.Position = locr + (100 * x);
                    vs.offset = tempr + (100 * x);
                    vs.ModelName = map.Strings.Name[(Int16)map.BR.ReadInt32()];
                    vs.X = map.BR.ReadSingle();
                    vs.Y = map.BR.ReadSingle();
                    vs.Z = map.BR.ReadSingle();

                    // facing direction
                    map.BR.BaseStream.Position = locr + (100 * x) + 20;
                    vs.RotationDirection = map.BR.ReadSingle();

                    if (charNum != -1)
                    {
                        vs.TagPath = map.FileNames.Name[temppalette2[charNum]];
                        vs.ModelTagNumber = temppalette[charNum];
                        if (vs.ModelTagNumber == -1)
                        {
                            continue;
                        }

                        // vs.ModelName = map.FileNames.Name[vs.ModelTagNumber];
                        Spawn.Add(vs);
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error loading AI Squads", e);
            }

            #endregion

            map.CloseMap();
        }

        #endregion

        /// <summary>
        /// The a i_ squads.
        /// </summary>
        /// <remarks></remarks>
        public class AI_Squads : RotateDirectionBaseSpawn
        {
            #region Constructors and Destructors

            /// <summary>
            /// Initializes a new instance of the <see cref="AI_Squads"/> class.
            /// </summary>
            /// <remarks></remarks>
            public AI_Squads()
            {
                this.Type = SpawnType.AI_Squads;
                this.RotationType = SpawnRotationType.Direction;
            }

            #endregion
        }

        /// <summary>
        /// The base spawn.
        /// </summary>
        /// <remarks></remarks>
        public class BaseSpawn
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
            /// The bb x diff.
            /// </summary>
            public float bbXDiff;

            /// <summary>
            /// The bb y diff.
            /// </summary>
            public float bbYDiff;

            /// <summary>
            /// The bb z diff.
            /// </summary>
            public float bbZDiff;

            /// <summary>
            /// The frozen.
            /// </summary>
            public bool frozen;

            /// <summary>
            /// The offset.
            /// </summary>
            public int offset;

            #endregion
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
        }

        /// <summary>
        /// The bounding box spawn.
        /// </summary>
        /// <remarks></remarks>
        public class BoundingBoxSpawn : BaseSpawn
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
        }

        /// <summary>
        /// The collection.
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
        }

        /// <summary>
        /// The death zone.
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
        }

        /// <summary>
        /// The objective spawn.
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
            public enum ObjectiveTypeEnum
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
            public enum TeamType
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
        }

        /// <summary>
        /// The obstacle spawn.
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
            }

            #endregion
        }

        /// <summary>
        /// The rotate direction base spawn.
        /// </summary>
        /// <remarks></remarks>
        public class RotateDirectionBaseSpawn : RotationSpawn
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
        public class RotateYawPitchRollBaseSpawn : RotationSpawn
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
        public class RotationSpawn : BaseSpawn
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
        public class ScaleRotateYawPitchRollSpawn : RotateYawPitchRollBaseSpawn
        {
            #region Constants and Fields

            /// <summary>
            /// The scale.
            /// </summary>
            public float Scale;

            #endregion
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
        }

        /// <summary>
        /// The special spawn.
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
            /// The special spawn type.
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
        }
    }
}