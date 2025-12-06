// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BSPViewer.cs" company="">
//   
// </copyright>
// <summary>
//   The bsp viewer.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace entity.Renderers
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading;
    using System.Windows.Forms;

    using entity;

    using Globals;

    using HaloMap.H2MetaContainers;
    using HaloMap.Map;
    using HaloMap.Meta;
    using HaloMap.RawData;
    using HaloMap.Render;

    using Microsoft.DirectX;
    using Microsoft.DirectX.Direct3D;
    using Microsoft.DirectX.DirectInput;

    using HaloMap;

    /// <summary>
    /// The bsp viewer.
    /// </summary>
    /// <remarks></remarks>
    public partial class BSPViewer : Form
    {
        #region Constants and Fields

        /// <summary>
        /// Whether theater mode is enabled (shows timeline, HUD, telemetry features).
        /// When false, opens as basic Visual Editor.
        /// </summary>
        private readonly bool theaterMode = false;

        /// <summary>
        /// The cam.
        /// </summary>
        public Camera2 cam;

        /// <summary>
        /// The visible spawns bit mask.
        /// </summary>
        public int visibleSpawnsBitMask;

        /// <summary>
        /// The map.
        /// </summary>
        private readonly Map map;

        /// <summary>
        /// The render.
        /// </summary>
        private readonly Renderer render = new Renderer();

        /// <summary>
        /// The black material.
        /// </summary>
        private Material BlackMaterial;

        /// <summary>
        /// The blue material.
        /// </summary>
        private Material BlueMaterial;

        /// <summary>
        /// The bounding box model.
        /// </summary>
        private Mesh[] BoundingBoxModel;

        /// <summary>
        /// The brown material.
        /// </summary>
        private Material BrownMaterial;

        /// <summary>
        /// The default material.
        /// </summary>
        private Material DefaultMaterial;

        /// <summary>
        /// The green material.
        /// </summary>
        private Material GreenMaterial;

        /// <summary>
        /// The halo light count.
        /// </summary>
        private int HaloLightCount = 1;

        /// <summary>
        /// The light map_ array.
        /// </summary>
        private byte[] LightMap_Array;

        /// <summary>
        /// The light map_ array_ backup.
        /// </summary>
        private byte[] LightMap_Array_Backup;

        /// <summary>
        /// The neutral material.
        /// </summary>
        private Material NeutralMaterial;

        /// <summary>
        /// The orange material.
        /// </summary>
        private Material OrangeMaterial;

        /// <summary>
        /// The pink material.
        /// </summary>
        private Material PinkMaterial;

        /// <summary>
        /// The purple material.
        /// </summary>
        private Material PurpleMaterial;

        /// <summary>
        /// The red material.
        /// </summary>
        private Material RedMaterial;

        /// <summary>
        /// The time.
        /// </summary>
        private TimeSpan Time;

        /// <summary>
        /// The translation matrix.
        /// </summary>
        private Matrix[] TranslationMatrix;

        /// <summary>
        /// The world transform.
        /// </summary>
        private Matrix WorldTransform = Matrix.Identity;

        /// <summary>
        /// The yellow material.
        /// </summary>
        private Material YellowMaterial;

        /// <summary>
        /// The aspect.
        /// </summary>
        private float aspect = 1f;

        /// <summary>
        /// The axis.
        /// </summary>
        private Gizmo.axis axis;

        /// <summary>
        /// The bsp.
        /// </summary>
        private BSPModel bsp;

        /// <summary>
        /// The current object.
        /// </summary>
        private int currentObject;

        /// <summary>
        /// The gizmo.
        /// </summary>
#pragma warning disable CS0649 // Field is never assigned
        private Gizmo gizmo;
#pragma warning restore CS0649

        /// <summary>
        /// The in sizing.
        /// </summary>
        private bool inSizing = true;

        /// <summary>
        /// The itemrotate.
        /// </summary>
        private bool itemrotate;

        /// <summary>
        /// The light vector.
        /// </summary>
        private Vector3 lightVector = new Vector3(0.0f, 0.0f, 1.0f);

        /// <summary>
        /// The oldx.
        /// </summary>
        private int oldx;

        /// <summary>
        /// The oldy.
        /// </summary>
        private int oldy;

        /// <summary>
        /// The rotation bit mask.
        /// </summary>
        private int rotationBitMask;

        /// <summary>
        /// The selected spawn type.
        /// </summary>
        private SpawnInfo.SpawnType selectedSpawnType;

        /// <summary>
        /// The selection depth.
        /// </summary>
        private float selectionDepth;

        /// <summary>
        /// The selection height.
        /// </summary>
        private float selectionHeight;

        /// <summary>
        /// The selection mesh.
        /// </summary>
        private Mesh selectionMesh;

        /// <summary>
        /// The selection multi.
        /// </summary>
        private bool selectionMulti;

        /// <summary>
        /// The selection start.
        /// </summary>
        private Vector3 selectionStart;

        /// <summary>
        /// The selection width.
        /// </summary>
        private float selectionWidth;

        /// <summary>
        /// The shaderx.
        /// </summary>
#pragma warning disable CS0649 // Field is never assigned
        private DXShader shaderx;
#pragma warning restore CS0649

        /// <summary>
        /// The spawnmodelindex.
        /// </summary>
        private int[] spawnmodelindex;

        /// <summary>
        /// The spawns.
        /// </summary>
        private SpawnLoads spawns;

        /// <summary>
        /// The trackint 1.
        /// </summary>
        private int trackint1;

        /// <summary>
        /// The trackint 2.
        /// </summary>
        private int trackint2;

        /// <summary>
        /// The trackint 3.
        /// </summary>
        private int trackint3;

        /// <summary>
        /// The update xyzypr.
        /// </summary>
        private bool updateXYZYPR = true;

        #region Player Path Animation Fields

        /// <summary>
        /// List of player path coordinates with timestamps (legacy single-player).
        /// </summary>
        private List<PlayerPathPoint> playerPath = new List<PlayerPathPoint>();

        /// <summary>
        /// Multi-player paths: Dictionary of player name -> list of path segments.
        /// Each segment is a list of points (segments break on respawn).
        /// </summary>
        private Dictionary<string, List<List<PlayerPathPoint>>> multiPlayerPaths = new Dictionary<string, List<List<PlayerPathPoint>>>();

        /// <summary>
        /// List of all unique player names in the loaded path data.
        /// </summary>
        private List<string> pathPlayerNames = new List<string>();

        /// <summary>
        /// Set of players to hide during playback/live view.
        /// </summary>
        private HashSet<string> hiddenPlayers = new HashSet<string>();

        /// <summary>
        /// Whether POV mode is enabled (camera follows selected player).
        /// </summary>
        private bool povModeEnabled = false;

        /// <summary>
        /// Player being followed in POV mode.
        /// </summary>
        private string povFollowPlayer = null;

        /// <summary>
        /// Minimum timestamp in path data (for timeline).
        /// </summary>
        private float pathMinTimestamp = 0;

        /// <summary>
        /// Maximum timestamp in path data (for timeline).
        /// </summary>
        private float pathMaxTimestamp = 0;

        /// <summary>
        /// Tracks kill events for timeline display.
        /// </summary>
        private struct KillEvent
        {
            public float Timestamp;
            public string PlayerName;
            public int Team;
            public string Weapon;
        }
        private List<KillEvent> killEvents = new List<KillEvent>();

        /// <summary>
        /// Tracks previous kill count per player to detect new kills.
        /// </summary>
        private Dictionary<string, int> playerPrevKills = new Dictionary<string, int>();

        /// <summary>
        /// Current playback timestamp.
        /// </summary>
        private float pathCurrentTimestamp = 0;

        /// <summary>
        /// Current index in the player path animation.
        /// </summary>
        private int pathCurrentIndex = 0;

        /// <summary>
        /// Whether path animation is playing.
        /// </summary>
        private bool pathIsPlaying = false;

        /// <summary>
        /// Playback speed multiplier.
        /// </summary>
        private float pathPlaybackSpeed = 1.0f;

        /// <summary>
        /// Time accumulator for animation.
        /// </summary>
        private float pathTimeAccumulator = 0;

        /// <summary>
        /// Last frame time for delta calculation.
        /// </summary>
        private DateTime pathLastFrameTime = DateTime.Now;

        /// <summary>
        /// Bookmark timestamp for loop playback.
        /// </summary>
        // Replaced by bookmarkStartTimestamp and bookmarkEndTimestamp for A-B loop

        /// <summary>
        /// Whether bookmark loop mode is enabled.
        /// </summary>
        private bool bookmarkLoopEnabled = false;

        /// <summary>
        /// Reference to the timeline panel for invalidation.
        /// </summary>
        private Panel timelinePanelRef = null;

        /// <summary>
        /// Bookmark button reference.
        /// </summary>
        private System.Windows.Forms.Button bookmarkButton = null;

        /// <summary>
        /// Loop toggle button reference.
        /// </summary>
        private System.Windows.Forms.Button loopButton = null;

        /// <summary>
        /// FPS counter fields.
        /// </summary>
        private int fpsFrameCount = 0;
        private DateTime fpsLastUpdate = DateTime.Now;
        private float currentFps = 0;
        private Microsoft.DirectX.Direct3D.Font fpsFont = null;

        /// <summary>
        /// Mesh for rendering the player marker.
        /// </summary>
        private Mesh playerMarkerMesh;

        /// <summary>
        /// Material for the player marker.
        /// </summary>
        private Material PlayerMarkerMaterial;

        /// <summary>
        /// Whether to show the path trail.
        /// </summary>
        private bool showPathTrail = false;

        /// <summary>
        /// Field of view in degrees for theater mode camera.
        /// </summary>
        private float theaterFOV = 78f;

        /// <summary>
        /// Bookmark start timestamp for loop playback (A-B loop start).
        /// </summary>
        private float bookmarkStartTimestamp = -1;

        /// <summary>
        /// Bookmark end timestamp for loop playback (A-B loop end).
        /// </summary>
        private float bookmarkEndTimestamp = -1;

        /// <summary>
        /// Timeline zoom level (1.0 = full timeline visible).
        /// </summary>
        private float timelineZoom = 1.0f;

        /// <summary>
        /// Timeline view offset for panning when zoomed.
        /// </summary>
        private float timelineOffset = 0f;

        /// <summary>
        /// Parsed biped model for rendering player on path.
        /// </summary>
        private ParsedModel playerBipedModel;

        /// <summary>
        /// Whether biped model loading has been attempted.
        /// </summary>
        private bool playerBipedModelLoaded = false;

        /// <summary>
        /// Player telemetry data structure with all fields.
        /// </summary>
        public class PlayerTelemetry
        {
            // Identity
            public string PlayerName;
            public string XboxId;
            public string MachineId;
            public int Team; // 0 = red, 1 = blue, 2 = green, 3 = orange, -1 = unknown

            // Emblem & Colors
            public int EmblemFg;
            public int EmblemBg;
            public int ColorPrimary;
            public int ColorSecondary;
            public int ColorTertiary;
            public int ColorQuaternary;

            // Timing
            public DateTime Timestamp;

            // Position & Velocity
            public float PosX;
            public float PosY;
            public float PosZ;
            public float VelX;
            public float VelY;
            public float VelZ;
            public float Speed;

            // Orientation (radians and degrees)
            public float Yaw;
            public float Pitch;
            public float YawDeg;
            public float PitchDeg;

            // Movement State
            public bool IsCrouching;
            public float CrouchBlend;
            public bool IsAirborne;
            public int AirborneTicks;

            // Weapons
            public int WeaponSlot;
            public string CurrentWeapon;
            public int FragGrenades;
            public int PlasmaGrenades;

            // K/D Stats
            public int Kills;
            public int Deaths;
            public int RespawnTimer;
            public bool IsDead;

            // Events
            public string Event;
        }

        /// <summary>
        /// Player path point structure.
        /// </summary>
        public struct PlayerPathPoint
        {
            public float X;
            public float Y;
            public float Z;
            public float Timestamp; // In seconds from start
            public int Team; // 0 = red, 1 = blue, 2 = green, 3 = orange, -1 = unknown
            public float FacingYaw;
            public string PlayerName;
            public string CurrentWeapon;
            public bool IsCrouching;
            public bool IsAirborne;
            public bool IsDead;

            // Emblem & Colors (same as PlayerTelemetry)
            public int EmblemFg;
            public int EmblemBg;
            public int ColorPrimary;
            public int ColorSecondary;
            public int ColorTertiary;
            public int ColorQuaternary;

            public PlayerPathPoint(float x, float y, float z, float timestamp, int team = -1,
                float facingYaw = 0, string playerName = "", string weapon = "",
                bool crouching = false, bool airborne = false, bool isDead = false,
                int emblemFg = 0, int emblemBg = 0, int colorPrimary = 0, int colorSecondary = 0,
                int colorTertiary = 0, int colorQuaternary = 0)
            {
                X = x;
                Y = y;
                Z = z;
                Timestamp = timestamp;
                Team = team;
                FacingYaw = facingYaw;
                PlayerName = playerName;
                CurrentWeapon = weapon;
                IsCrouching = crouching;
                IsAirborne = airborne;
                IsDead = isDead;
                EmblemFg = emblemFg;
                EmblemBg = emblemBg;
                ColorPrimary = colorPrimary;
                ColorSecondary = colorSecondary;
                ColorTertiary = colorTertiary;
                ColorQuaternary = colorQuaternary;
            }
        }

        #endregion

        #region Live Telemetry Network Fields

        /// <summary>
        /// UDP client for receiving live player telemetry.
        /// </summary>
        private UdpClient telemetryUdpClient;

        /// <summary>
        /// TCP listener for receiving live player telemetry.
        /// </summary>
        private TcpListener telemetryTcpListener;

        /// <summary>
        /// Thread for handling incoming UDP telemetry data.
        /// </summary>
        private Thread telemetryListenerThread;

        /// <summary>
        /// Thread for handling incoming TCP telemetry data.
        /// </summary>
        private Thread telemetryTcpListenerThread;

        /// <summary>
        /// Whether the telemetry listener is running.
        /// </summary>
        private volatile bool telemetryListenerRunning = false;

        /// <summary>
        /// Dictionary of live player data by player name.
        /// </summary>
        private Dictionary<string, PlayerTelemetry> livePlayers = new Dictionary<string, PlayerTelemetry>();

        /// <summary>
        /// Tracks death state per player (true = currently dead, waiting for respawn).
        /// </summary>
        private Dictionary<string, bool> playerDeadState = new Dictionary<string, bool>();

        /// <summary>
        /// Tracks previous death count per player to detect new deaths.
        /// </summary>
        private Dictionary<string, int> playerPrevDeaths = new Dictionary<string, int>();

        /// <summary>
        /// Tracks previous position per player to detect respawn.
        /// </summary>
        private Dictionary<string, Vector3> playerPrevPosition = new Dictionary<string, Vector3>();

        /// <summary>
        /// Lock object for thread-safe access to live player data.
        /// </summary>
        private object livePlayersLock = new object();

        /// <summary>
        /// Whether to show live telemetry instead of recorded path.
        /// </summary>
        private bool showLiveTelemetry = false;

        /// <summary>
        /// Whether to show the scoreboard overlay.
        /// </summary>
        private bool showScoreboard = false;

        /// <summary>
        /// Whether to show the killfeed overlay.
        /// </summary>
        private bool showKillfeed = false;

        /// <summary>
        /// Font for scoreboard text.
        /// </summary>
        private Microsoft.DirectX.Direct3D.Font scoreboardFont;

        /// <summary>
        /// Font for scoreboard header.
        /// </summary>
        private Microsoft.DirectX.Direct3D.Font scoreboardHeaderFont;

        /// <summary>
        /// List of live player names for dropdown population.
        /// </summary>
        private List<string> livePlayerNames = new List<string>();

        /// <summary>
        /// Font for drawing player names.
        /// </summary>
        private Microsoft.DirectX.Direct3D.Font playerNameFont;

        /// <summary>
        /// Mesh for team indicator circle.
        /// </summary>
        private Mesh teamCircleMesh;

        /// <summary>
        /// Column indices for CSV parsing (detected from header).
        /// </summary>
        private Dictionary<string, int> csvColumnIndices = new Dictionary<string, int>();

        /// <summary>
        /// Debug log of recent telemetry messages.
        /// </summary>
        private List<string> telemetryDebugLog = new List<string>();
        private object telemetryDebugLogLock = new object();
        private const int MaxDebugLogEntries = 50;

        /// <summary>
        /// Cached emblem textures by emblem key (EF_EB_P_S format).
        /// </summary>
        private Dictionary<string, Texture> emblemTextureCache = new Dictionary<string, Texture>();

        /// <summary>
        /// Sprite for drawing 2D textures.
        /// </summary>
        private Sprite emblemSprite;

        /// <summary>
        /// Set of emblem keys currently being loaded.
        /// </summary>
        private HashSet<string> emblemLoadingSet = new HashSet<string>();

        /// <summary>
        /// Cached weapon textures by weapon name.
        /// </summary>
        private Dictionary<string, Texture> weaponTextureCache = new Dictionary<string, Texture>();

        /// <summary>
        /// Set of weapon names currently being loaded.
        /// </summary>
        private HashSet<string> weaponLoadingSet = new HashSet<string>();

        #endregion

        #region Player Selection Wheel Fields

        /// <summary>
        /// Panel for player selection wheel (Price is Right style spinner).
        /// </summary>
        private Panel playerWheelPanel;

        /// <summary>
        /// Current spin offset for wheel animation (0-1 range per player slot).
        /// </summary>
        private float wheelSpinOffset = 0f;

        /// <summary>
        /// Target offset for spin animation.
        /// </summary>
        private float wheelTargetOffset = 0f;

        /// <summary>
        /// Whether wheel is currently spinning.
        /// </summary>
        private bool wheelIsSpinning = false;

        /// <summary>
        /// Spin velocity for deceleration animation.
        /// </summary>
        private float wheelSpinVelocity = 0f;

        /// <summary>
        /// Timer for wheel spin animation.
        /// </summary>
        private System.Windows.Forms.Timer wheelSpinTimer;

        /// <summary>
        /// Current selected index in player wheel.
        /// </summary>
        private int wheelSelectedIndex = 0;

        /// <summary>
        /// Cache of loaded emblem images for wheel display (GDI+ Bitmap).
        /// </summary>
        private Dictionary<string, System.Drawing.Image> wheelEmblemCache = new Dictionary<string, System.Drawing.Image>();

        /// <summary>
        /// Set of emblem keys currently being loaded for wheel.
        /// </summary>
        private HashSet<string> wheelEmblemLoadingSet = new HashSet<string>();

        /// <summary>
        /// Whether the game is a team game (affects background colors).
        /// </summary>
        private bool isTeamGame = true;

        #endregion

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BSPViewer"/> class.
        /// </summary>
        /// <param name="tempbsp">The tempbsp.</param>
        /// <param name="map">The map.</param>
        /// <param name="theaterMode">If true, opens in Theater Mode with full playback features.</param>
        /// <remarks></remarks>
        public BSPViewer(BSPModel tempbsp, Map map, bool theaterMode = false)
        {
            this.theaterMode = theaterMode;

            // InitializeComponent
            InitializeComponent();

            dockControl1.LayoutSystem.Collapsed = true;
            dockControl2.LayoutSystem.Collapsed = true;
            dockControl3.LayoutSystem.Collapsed = true;
            dockControl4.LayoutSystem.Collapsed = true;
            dockControl5.LayoutSystem.Collapsed = true;

            Application.DoEvents();

            // Center label horizontal
            int tempnum1 = 155 / 2;
            int tempnum2 = Screen.PrimaryScreen.WorkingArea.Width / 2;
            int tempnum3 = tempnum2 - tempnum1;
            label3.Left = tempnum3;

            // Center label vertical
            int tempnum4 = 28 / 2;
            int tempnum5 = Screen.PrimaryScreen.WorkingArea.Height / 2;
            int tempnum6 = tempnum5 - tempnum4 - 30 - 25;
            label3.Top = tempnum6;

            this.BackColor = Color.Blue;

            #region Clear the labels
            toolStripLabel2.Text = "Camera Position: X: 0 � Y: 0 � Z: 0";
            tsLabel1.Text = "Type: <";
            tsButtonType.Text = string.Empty;
            tsLabel2.Text = "> (";
            tsLabelCount.Text = string.Empty;
            tsLabelX.Text = ") � X: ";
            tsTextBoxX.Text = string.Empty;
            tsLabelY.Text = " � Y: ";
            tsTextBoxY.Text = string.Empty;
            tsLabelZ.Text = " � Z: ";
            tsTextBoxZ.Text = string.Empty;

            tsLabelYaw.Text = string.Empty;
            tsTextBoxYaw.Text = string.Empty;
            tsLabelPitch.Text = string.Empty;
            tsTextBoxPitch.Text = string.Empty;
            tsLabelRoll.Text = string.Empty;
            tsTextBoxRoll.Text = string.Empty;
            #endregion

            // Set the initial size of our form
            // this.ClientSize = new System.Drawing.Size(800, 600);
            // And its caption
            this.Text = "BSP Viewer (" + tempbsp.Name + ")";

            Application.DoEvents();

            bsp = tempbsp;
            this.map = map;

            

            this.MouseDown += BSPViewer_MouseDown;
            this.MouseUp += BSPViewer_MouseUp;
            this.MouseMove += this.ModelViewer_MouseDown;
            this.MouseUp += this.ModelViewer_MouseUp;            

            #region VisibleSpawns

            string[] strings = Enum.GetNames(typeof(SpawnInfo.SpawnType));

            treeView1.Sorted = true;
            treeView1.ShowNodeToolTips = true;

            int CameraCount = 0;
            int DeathZoneCount = 0;
            int ObjectiveCount = 0;
            int PlayerCount = 0;

            foreach (string s in strings)
            {
                // Add the type to the treeview
                TreeNode tn = new TreeNode();
                bool SpawnFound = false;
                for (int i = 0; i < bsp.Spawns.Spawn.Count; i++)
                {
                    if (s == bsp.Spawns.Spawn[i].Type.ToString())
                    {
                        // Only add spawns that exist on our map
                        if (!SpawnFound)
                        {
                            // Add the type the the CheckListBox
                            checkedListBox1.Items.Add(s);
                            SpawnFound = true;
                        }

                        TreeNode tn2 = new TreeNode();
                        tn2.Text = string.Empty;
                        tn2.ToolTipText = " X: " + bsp.Spawns.Spawn[i].X.ToString("#0.0##").PadRight(9) + "  Y: " +
                                          bsp.Spawns.Spawn[i].Y.ToString("#0.0##").PadRight(9) + "  Z: " +
                                          bsp.Spawns.Spawn[i].Z.ToString("#0.0##").PadRight(9);

                        if (bsp.Spawns.Spawn[i].Type.ToString() == "Collection")
                        {
                            switch (map.HaloVersion)
                            {
                                case HaloVersionEnum.Halo2:
                                case HaloVersionEnum.Halo2Vista:
                                    string[] temps = bsp.Spawns.Spawn[i].TagPath.Split('\\');
                                    tn2.Text = temps[temps.Length - 1];
                                    break;
                                case HaloVersionEnum.Halo1:
                                case HaloVersionEnum.HaloCE:
                                    SpawnInfo.H1Collection tempspawnx = (SpawnInfo.H1Collection)bsp.Spawns.Spawn[i];
                                    tn2.Text = tempspawnx.TagPath;
                                    break;
                            }
                        }
                            
                            // else if (bsp.Spawns.Spawn[i].Type.ToString() == "Obstacle") {}
                            // else if (bsp.Spawns.Spawn[i].Type.ToString() == "Weapon")   {}
                            // else if (bsp.Spawns.Spawn[i].Type.ToString() == "Scenery")  {}
                            // else if (bsp.Spawns.Spawn[i].Type.ToString() == "Machine")  {}
                            // else if (bsp.Spawns.Spawn[i].Type.ToString() == "Control")  {}
                            // else if (bsp.Spawns.Spawn[i].Type.ToString() == "Biped")    {}
                            // else if (bsp.Spawns.Spawn[i].Type.ToString() == "Equipment"){}
                            // else if (bsp.Spawns.Spawn[i].Type.ToString() == "Vehicle")  {}
                            // else if (bsp.Spawns.Spawn[i].Type.ToString() == "Light")    {}
                            // else if (bsp.Spawns.Spawn[i].Type.ToString() == "Sound")    {}
                        else if (bsp.Spawns.Spawn[i].Type.ToString() == "Camera")
                        {
                            tn2.Text = bsp.Spawns.Spawn[i].Type + " {" + CameraCount + "}";
                            CameraCount += 1;
                        }
                        else if (bsp.Spawns.Spawn[i].Type.ToString() == "DeathZone")
                        {
                            SpawnInfo.DeathZone tempspawn = (SpawnInfo.DeathZone)bsp.Spawns.Spawn[i];
                            tn2.Text = tempspawn.Name;
                            tn2.ToolTipText += "\n Length: " + tempspawn.length.ToString("#0.0##") + "  Width: " +
                                               tempspawn.width.ToString("#0.0##") + " Height: " +
                                               tempspawn.height.ToString("#0.0##");
                            DeathZoneCount += 1;
                        }
                        else if (bsp.Spawns.Spawn[i].Type.ToString() == "Objective")
                        {
                            SpawnInfo.ObjectiveSpawn tempspawn = (SpawnInfo.ObjectiveSpawn)bsp.Spawns.Spawn[i];

                            // tn2.Text = bsp.Spawns.Spawn[i].Type.ToString() + " {" + ObjectiveCount.ToString() + "}";
                            tn2.Text = tempspawn.ObjectiveType + " (" + tempspawn.Team + ") #" + tempspawn.number;
                            tn2.ToolTipText += "\n Type: " + tempspawn.ObjectiveType + "  #" + tempspawn.number +
                                               "\n Team: " + tempspawn.Team;
                            ObjectiveCount += 1;
                        }
                        else if (bsp.Spawns.Spawn[i].Type.ToString() == "Player")
                        {
                            tn2.Text = bsp.Spawns.Spawn[i].Type + " {" + PlayerCount + "}";
                            PlayerCount += 1;
                        }
                        else if (bsp.Spawns.Spawn[i].Type.ToString() == "AI_Squads")
                        {
                            string[] temps = bsp.Spawns.Spawn[i].TagPath.Split('\\');
                            tn2.Text = "Squad " + ((HaloMap.Render.SpawnInfo.AI_Squads)bsp.Spawns.Spawn[i]).squadNumber
                                + ": " + temps[temps.Length - 1];
                        }
                        else if (bsp.Spawns.Spawn[i].Type.ToString() == "SpawnZone")
                        {
                            SpawnInfo.SpawnZone tempSpawnZone = (SpawnInfo.SpawnZone)bsp.Spawns.Spawn[i];
                            if (tempSpawnZone.Name == string.Empty)
                                tn2.Text = "(" + tempSpawnZone.ZoneType.ToString() + ") Spawn Zone";
                            else
                                tn2.Text = "(" + tempSpawnZone.ZoneType.ToString() + ") " + tempSpawnZone.Name;

                        }
                        else
                        {
                            // tn2.Text = bsp.Spawns.Spawn[i].Type.ToString();
                            string[] temps = bsp.Spawns.Spawn[i].TagPath.Split('\\');
                            tn2.Text = temps[temps.Length - 1];
                        }

                        if (map.HaloVersion == HaloVersionEnum.Halo2 ||
                            map.HaloVersion == HaloVersionEnum.Halo2Vista)
                        {
                            #region BasicInfo For YawPitchRoll Rotations

                            if (bsp.Spawns.Spawn[i] is SpawnInfo.RotateYawPitchRollBaseSpawn)
                            {
                                SpawnInfo.RotateYawPitchRollBaseSpawn tempspawn =
                                    (SpawnInfo.RotateYawPitchRollBaseSpawn)bsp.Spawns.Spawn[i];
                                if (tn2.Text == null)
                                {
                                    string[] temps = tempspawn.TagPath.Split('\\');
                                    tn2.Text = temps[temps.Length - 1];
                                }

                                tn2.ToolTipText += "\n Yaw: " + tempspawn.Yaw.ToString("#0.0##") + "  Pitch: " +
                                                   tempspawn.Pitch.ToString("#0.0##") + " Roll: " +
                                                   tempspawn.Roll.ToString("#0.0##");
                            }

                                #endregion
                                #region BasicInfo For One Rotation
                            else if (bsp.Spawns.Spawn[i] is SpawnInfo.RotateDirectionBaseSpawn)
                            {
                                SpawnInfo.RotateDirectionBaseSpawn tempspawn =
                                    (SpawnInfo.RotateDirectionBaseSpawn)bsp.Spawns.Spawn[i];
                                if (tn2.Text == null)
                                {
                                    if (tempspawn.TagPath != null)
                                    {
                                        string[] temps = tempspawn.TagPath.Split('\\');
                                        tn2.Text = temps[temps.Length - 1];
                                    }
                                    else
                                    {
                                        tn2.Text = tempspawn.Type.ToString();
                                    }
                                }

                                tn2.ToolTipText += "\n Rotation: " + tempspawn.RotationDirection.ToString("#0.0##");
                            }

                            #endregion
                        }

                        tn2.Tag = i;
                        tn.Nodes.Add(tn2);
                    }
                }

                tn.Text = s;
                tn.Tag = -1;
                treeView1.Nodes.Add(tn);
            }

            #endregion

            #region Lightmaps

            Load_Lightmaps_Into_An_Array();
            int temppicboxX = 0;
            int temppicboxY = 0;
            int tempintforme = 0;
            for (int i = 0; i < map.BSP.sbsp[tempbsp.BspNumber].LightMap_Palettes.Count; i++)
            {
                PictureBox temppicbox = new PictureBox();
                temppicbox.Image = RenderLightmap(i);
                temppicbox.SizeMode = PictureBoxSizeMode.StretchImage;
                temppicbox.Width = 64;
                temppicbox.Height = 64;
                temppicbox.Location = new Point(temppicboxX, temppicboxY);
                temppicbox.Tag = i;
                temppicbox.Click += this.PictureBox_Click;
                panel1.Controls.Add(temppicbox);
                comboBox1.Items.Add(i);
                if (tempintforme <= 1)
                {
                    tempintforme += 1;
                    temppicboxX += 68;
                }
                else
                {
                    tempintforme = 0;
                    temppicboxX = 0;
                    temppicboxY += 68;
                }
            }

            #endregion

            if (map.HaloVersion == HaloVersionEnum.Halo2 ||
                map.HaloVersion == HaloVersionEnum.Halo2Vista)
            {
                this.NoCulling.Checked = false;
            }
            else
            {
                this.NoCulling.Checked = true;
            }

            // Initialize path playback controls (Theater Mode only)
            if (theaterMode)
            {
                InitializePathControls();
                this.Text = "Theater Mode - " + map.filePath;
            }

            // Clean up telemetry listener on close
            this.FormClosing += BSPViewer_FormClosing;

            Main();
        }

        private void BSPViewer_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Stop telemetry listener if running
            if (telemetryListenerRunning)
            {
                StopTelemetryListener();
            }
        }

        // UI controls that need to be updated when path is loaded
        private ToolStripDropDownButton pathPlayerDropdown;
        private ToolStripComboBox povPlayerDropdown;
        private TrackBar pathTimelineTrackBar;
        private Label pathTimeLabel;
        private ToolStripButton pathPlayPauseButton;

        /// <summary>
        /// Initializes the player path playback UI controls.
        /// </summary>
        private void InitializePathControls()
        {
            // Add separator
            ToolStripSeparator separator = new ToolStripSeparator();
            toolStrip.Items.Add(separator);

            // Load Path button
            ToolStripButton btnLoadPath = new ToolStripButton();
            btnLoadPath.Text = "Load Path";
            btnLoadPath.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnLoadPath.Click += btnLoadPath_Click;
            toolStrip.Items.Add(btnLoadPath);

            // Player dropdown with checkmarks for visibility
            ToolStripLabel lblPlayer = new ToolStripLabel();
            lblPlayer.Text = "Players:";
            toolStrip.Items.Add(lblPlayer);

            pathPlayerDropdown = new ToolStripDropDownButton();
            pathPlayerDropdown.Text = "All Visible";
            pathPlayerDropdown.DisplayStyle = ToolStripItemDisplayStyle.Text;
            toolStrip.Items.Add(pathPlayerDropdown);

            // POV mode dropdown - placed right next to Players
            ToolStripLabel lblPOV = new ToolStripLabel();
            lblPOV.Text = "POV:";
            toolStrip.Items.Add(lblPOV);

            povPlayerDropdown = new ToolStripComboBox();
            povPlayerDropdown.Items.Add("Free Camera");
            povPlayerDropdown.SelectedIndex = 0;
            povPlayerDropdown.DropDownStyle = ComboBoxStyle.DropDownList;
            povPlayerDropdown.Width = 100;
            povPlayerDropdown.SelectedIndexChanged += (s, e) => {
                List<string> playerNames = showLiveTelemetry ? livePlayerNames : pathPlayerNames;
                if (povPlayerDropdown.SelectedIndex == 0)
                {
                    povModeEnabled = false;
                    povFollowPlayer = null;
                }
                else if (povPlayerDropdown.SelectedIndex <= playerNames.Count)
                {
                    povModeEnabled = true;
                    povFollowPlayer = playerNames[povPlayerDropdown.SelectedIndex - 1];
                }
            };
            toolStrip.Items.Add(povPlayerDropdown);

            toolStrip.Items.Add(new ToolStripSeparator());

            // Play/Pause button
            pathPlayPauseButton = new ToolStripButton();
            pathPlayPauseButton.Text = "> Play";
            pathPlayPauseButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
            pathPlayPauseButton.Click += (s, e) => {
                TogglePathPlayback();
                pathPlayPauseButton.Text = pathIsPlaying ? "|| Pause" : "> Play";
            };
            toolStrip.Items.Add(pathPlayPauseButton);

            // Reset button
            ToolStripButton btnReset = new ToolStripButton();
            btnReset.Text = "[] Reset";
            btnReset.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnReset.Click += (s, e) => {
                ResetPathAnimation();
                pathPlayPauseButton.Text = "> Play";
            };
            toolStrip.Items.Add(btnReset);

            // Playback Speed label and dropdown
            ToolStripLabel lblPlaySpeed = new ToolStripLabel();
            lblPlaySpeed.Text = "Play:";
            toolStrip.Items.Add(lblPlaySpeed);

            ToolStripComboBox cboPlaySpeed = new ToolStripComboBox();
            cboPlaySpeed.Items.AddRange(new object[] { "0.25x", "0.5x", "1x", "2x", "4x", "10x" });
            cboPlaySpeed.SelectedIndex = 2; // Default 1x
            cboPlaySpeed.DropDownStyle = ComboBoxStyle.DropDownList;
            cboPlaySpeed.Width = 55;
            cboPlaySpeed.SelectedIndexChanged += (s, e) => {
                switch (cboPlaySpeed.SelectedIndex)
                {
                    case 0: pathPlaybackSpeed = 0.25f; break;
                    case 1: pathPlaybackSpeed = 0.5f; break;
                    case 2: pathPlaybackSpeed = 1.0f; break;
                    case 3: pathPlaybackSpeed = 2.0f; break;
                    case 4: pathPlaybackSpeed = 4.0f; break;
                    case 5: pathPlaybackSpeed = 10.0f; break;
                }
            };
            pathPlaybackSpeed = 1.0f; // Default 1x playback
            toolStrip.Items.Add(cboPlaySpeed);

            // Camera Speed label and dropdown
            ToolStripLabel lblCamSpeed = new ToolStripLabel();
            lblCamSpeed.Text = "Cam:";
            toolStrip.Items.Add(lblCamSpeed);

            ToolStripComboBox cboCamSpeed = new ToolStripComboBox();
            cboCamSpeed.Items.AddRange(new object[] { "0.1", "0.25", "0.5", "1.0", "2.0", "5.0" });
            cboCamSpeed.SelectedIndex = 1; // Default 0.25
            cboCamSpeed.DropDownStyle = ComboBoxStyle.DropDownList;
            cboCamSpeed.Width = 50;
            cboCamSpeed.SelectedIndexChanged += (s, e) => {
                switch (cboCamSpeed.SelectedIndex)
                {
                    case 0: cam.speed = 0.1f; break;
                    case 1: cam.speed = 0.25f; break;
                    case 2: cam.speed = 0.5f; break;
                    case 3: cam.speed = 1.0f; break;
                    case 4: cam.speed = 2.0f; break;
                    case 5: cam.speed = 5.0f; break;
                }
            };
            if (cam != null) cam.speed = 0.25f; // Default 0.25 camera speed
            toolStrip.Items.Add(cboCamSpeed);

            toolStrip.Items.Add(new ToolStripSeparator());

            // Show Trail checkbox
            ToolStripButton btnTrail = new ToolStripButton();
            btnTrail.Text = "Trail: OFF";
            btnTrail.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnTrail.Click += (s, e) => {
                showPathTrail = !showPathTrail;
                btnTrail.Text = showPathTrail ? "Trail: ON" : "Trail: OFF";
            };
            toolStrip.Items.Add(btnTrail);

            // FOV control
            ToolStripLabel lblFOV = new ToolStripLabel();
            lblFOV.Text = "FOV:";
            toolStrip.Items.Add(lblFOV);

            ToolStripTextBox txtFOV = new ToolStripTextBox();
            txtFOV.Text = "78";
            txtFOV.Width = 40;
            txtFOV.TextChanged += (s, e) => {
                if (float.TryParse(txtFOV.Text, out float fov))
                {
                    theaterFOV = Math.Max(30, Math.Min(120, fov));
                }
            };
            toolStrip.Items.Add(txtFOV);

            // Create timeline panel at bottom of form - Halo theater style
            Panel timelinePanel = new Panel();
            timelinePanel.Dock = DockStyle.Bottom;
            timelinePanel.Height = 45;
            timelinePanel.BackColor = Color.FromArgb(15, 25, 35); // Dark blue-black

            // Add subtle top border
            Panel borderPanel = new Panel();
            borderPanel.Dock = DockStyle.Top;
            borderPanel.Height = 2;
            borderPanel.BackColor = Color.FromArgb(0, 120, 180); // Halo blue accent
            timelinePanel.Controls.Add(borderPanel);

            pathTimeLabel = new Label();
            pathTimeLabel.Text = "0:00 / 0:00";
            pathTimeLabel.ForeColor = Color.FromArgb(0, 200, 255); // Cyan
            pathTimeLabel.Font = new System.Drawing.Font("Segoe UI", 10, FontStyle.Bold);
            pathTimeLabel.AutoSize = true;
            pathTimeLabel.Location = new Point(10, 14);
            pathTimeLabel.BackColor = Color.Transparent;
            timelinePanel.Controls.Add(pathTimeLabel);

            pathTimelineTrackBar = new TrackBar();
            pathTimelineTrackBar.Minimum = 0;
            pathTimelineTrackBar.Maximum = 1000;
            pathTimelineTrackBar.Value = 0;
            pathTimelineTrackBar.TickStyle = TickStyle.None;
            pathTimelineTrackBar.Location = new Point(100, 5);
            pathTimelineTrackBar.Height = 30;
            pathTimelineTrackBar.Scroll += PathTimelineTrackBar_Scroll;
            pathTimelineTrackBar.MouseDown += (s, e) => {
                if (e.Button == MouseButtons.Left)
                {
                    pathIsPlaying = false;
                    pathPlayPauseButton.Text = "▶ Play";
                }
            };
            // Right-click to set bookmark marker at clicked position
            pathTimelineTrackBar.MouseUp += (s, e) => {
                if (e.Button == MouseButtons.Right && pathMaxTimestamp > pathMinTimestamp)
                {
                    float clickRatio = (float)e.X / pathTimelineTrackBar.Width;
                    clickRatio = Math.Max(0, Math.Min(1, clickRatio));
                    float clickedTimestamp = pathMinTimestamp + clickRatio * (pathMaxTimestamp - pathMinTimestamp);
                    // Set the appropriate marker
                    if (bookmarkStartTimestamp < 0)
                        bookmarkStartTimestamp = clickedTimestamp;
                    else if (bookmarkEndTimestamp < 0)
                    {
                        bookmarkEndTimestamp = clickedTimestamp;
                        if (bookmarkEndTimestamp < bookmarkStartTimestamp)
                        {
                            float temp = bookmarkStartTimestamp;
                            bookmarkStartTimestamp = bookmarkEndTimestamp;
                            bookmarkEndTimestamp = temp;
                        }
                    }
                    else
                    {
                        // Reset and set new start
                        bookmarkStartTimestamp = clickedTimestamp;
                        bookmarkEndTimestamp = -1;
                        bookmarkLoopEnabled = false;
                    }
                    UpdateBookmarkButton();
                    UpdateLoopButton();
                    timelinePanelRef?.Invalidate();
                }
            };

            // Mouse wheel zoom on timeline
            pathTimelineTrackBar.MouseWheel += (s, e) => {
                if (pathMaxTimestamp > pathMinTimestamp)
                {
                    float zoomFactor = e.Delta > 0 ? 0.9f : 1.1f;
                    timelineZoom = Math.Max(0.1f, Math.Min(1.0f, timelineZoom * zoomFactor));
                    // Calculate offset to zoom at mouse position
                    float mouseRatio = (float)e.X / pathTimelineTrackBar.Width;
                    float centerTime = pathMinTimestamp + (pathMaxTimestamp - pathMinTimestamp) * mouseRatio;
                    timelineOffset = centerTime - (pathMaxTimestamp - pathMinTimestamp) * timelineZoom * 0.5f;
                    timelineOffset = Math.Max(pathMinTimestamp, Math.Min(pathMaxTimestamp - (pathMaxTimestamp - pathMinTimestamp) * timelineZoom, timelineOffset));
                    timelinePanelRef?.Invalidate();
                }
            };

            timelinePanel.Controls.Add(pathTimelineTrackBar);

            // Bookmark button - sets A-B markers at current position
            bookmarkButton = new System.Windows.Forms.Button();
            bookmarkButton.Text = "[A] Set";
            bookmarkButton.FlatStyle = FlatStyle.Flat;
            bookmarkButton.FlatAppearance.BorderColor = Color.FromArgb(0, 120, 180);
            bookmarkButton.BackColor = Color.FromArgb(30, 45, 60);
            bookmarkButton.ForeColor = Color.FromArgb(0, 200, 255);
            bookmarkButton.Font = new System.Drawing.Font("Segoe UI", 8, FontStyle.Bold);
            bookmarkButton.Size = new Size(55, 25);
            bookmarkButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            bookmarkButton.Click += (s, e) => {
                SetBookmarkMarker();
            };
            timelinePanel.Controls.Add(bookmarkButton);

            // Loop button - toggles loop mode between A-B markers
            loopButton = new System.Windows.Forms.Button();
            loopButton.Text = "Loop";
            loopButton.FlatStyle = FlatStyle.Flat;
            loopButton.FlatAppearance.BorderColor = Color.FromArgb(0, 120, 180);
            loopButton.BackColor = Color.FromArgb(30, 45, 60);
            loopButton.ForeColor = Color.Gray;
            loopButton.Font = new System.Drawing.Font("Segoe UI", 8, FontStyle.Bold);
            loopButton.Size = new Size(60, 25);
            loopButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            loopButton.Click += (s, e) => {
                if (bookmarkStartTimestamp >= 0 && bookmarkEndTimestamp >= 0)
                {
                    bookmarkLoopEnabled = !bookmarkLoopEnabled;
                    UpdateLoopButton();
                }
            };
            timelinePanel.Controls.Add(loopButton);

            // Store reference for invalidation
            timelinePanelRef = timelinePanel;

            // Add paint handler to draw kill markers and bookmark on timeline
            timelinePanel.Paint += TimelinePanel_Paint;

            // Handle resize to keep trackbar full width and position buttons
            timelinePanel.Resize += (s, e) => {
                int buttonAreaWidth = 130;
                if (pathTimelineTrackBar != null)
                    pathTimelineTrackBar.Width = timelinePanel.Width - 110 - buttonAreaWidth;
                if (bookmarkButton != null)
                    bookmarkButton.Location = new Point(timelinePanel.Width - buttonAreaWidth + 5, 10);
                if (loopButton != null)
                    loopButton.Location = new Point(timelinePanel.Width - buttonAreaWidth + 65, 10);
                timelinePanel.Invalidate();
            };

            this.Controls.Add(timelinePanel);
            timelinePanel.BringToFront();

            // Initial sizing after panel is added
            int initialButtonAreaWidth = 130;
            pathTimelineTrackBar.Width = this.ClientSize.Width - 110 - initialButtonAreaWidth;
            bookmarkButton.Location = new Point(this.ClientSize.Width - initialButtonAreaWidth + 5, 10);
            loopButton.Location = new Point(this.ClientSize.Width - initialButtonAreaWidth + 65, 10);

            // Initialize player selection wheel (Price is Right style spinner)
            InitializePlayerWheel();

            // Separator before live telemetry
            ToolStripSeparator separator2 = new ToolStripSeparator();
            toolStrip.Items.Add(separator2);

            // Live telemetry listener button
            ToolStripButton btnListen = new ToolStripButton();
            btnListen.Text = "📡 Listen";
            btnListen.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnListen.Click += (s, e) => {
                if (telemetryListenerRunning)
                {
                    StopTelemetryListener();
                    btnListen.Text = "📡 Listen";
                    showLiveTelemetry = false;
                }
                else
                {
                    StartTelemetryListener();
                    btnListen.Text = "🔴 Stop";
                    showLiveTelemetry = true;
                    EnableTelemetryViewOptions();
                }
            };
            toolStrip.Items.Add(btnListen);

            // Debug button to show incoming telemetry data
            ToolStripButton btnDebug = new ToolStripButton();
            btnDebug.Text = "🔍 Debug";
            btnDebug.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnDebug.Click += (s, e) => {
                ShowTelemetryDebug();
            };
            toolStrip.Items.Add(btnDebug);

            // Make toolbar visible so path controls are accessible
            toolStrip.Visible = true;
        }

        /// <summary>
        /// Initializes the player selection wheel UI (Price is Right style spinner).
        /// </summary>
        private void InitializePlayerWheel()
        {
            // Create the wheel panel - positioned on the right side of the form
            playerWheelPanel = new Panel();
            playerWheelPanel.Width = 140;
            playerWheelPanel.Height = 280;
            playerWheelPanel.BackColor = Color.FromArgb(15, 25, 35); // Dark blue-black matching timeline
            playerWheelPanel.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;

            // Enable double buffering for smooth animation
            typeof(Panel).InvokeMember("DoubleBuffered",
                System.Reflection.BindingFlags.SetProperty | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic,
                null, playerWheelPanel, new object[] { true });

            // Add subtle border and title
            playerWheelPanel.Paint += (s, e) => {
                // Draw Halo blue border
                using (Pen borderPen = new Pen(Color.FromArgb(0, 120, 180), 2))
                {
                    e.Graphics.DrawRectangle(borderPen, 1, 1, playerWheelPanel.Width - 3, playerWheelPanel.Height - 3);
                }

                // Draw title at top
                using (System.Drawing.Font titleFont = new System.Drawing.Font("Segoe UI", 9, FontStyle.Bold))
                using (SolidBrush titleBrush = new SolidBrush(Color.FromArgb(0, 200, 255)))
                {
                    string title = "POV SELECT";
                    SizeF titleSize = e.Graphics.MeasureString(title, titleFont);
                    e.Graphics.DrawString(title, titleFont, titleBrush,
                        (playerWheelPanel.Width - titleSize.Width) / 2, 5);
                }
            };

            // Custom paint for the wheel
            playerWheelPanel.Paint += PlayerWheelPanel_Paint;

            // Click to spin
            playerWheelPanel.MouseClick += PlayerWheelPanel_MouseClick;

            // Mouse wheel to scroll through players
            playerWheelPanel.MouseWheel += (s, e) => {
                if (!wheelIsSpinning)
                {
                    List<string> playerNames = showLiveTelemetry ? livePlayerNames : pathPlayerNames;
                    if (playerNames.Count > 0)
                    {
                        int delta = e.Delta > 0 ? -1 : 1;
                        wheelSelectedIndex = (wheelSelectedIndex + delta + playerNames.Count + 1) % (playerNames.Count + 1);
                        UpdatePOVFromWheel();
                        playerWheelPanel.Invalidate();
                    }
                }
            };

            // Spin animation timer
            wheelSpinTimer = new System.Windows.Forms.Timer();
            wheelSpinTimer.Interval = 16; // ~60 FPS
            wheelSpinTimer.Tick += WheelSpinTimer_Tick;

            // Position the panel - right side, above timeline
            this.Controls.Add(playerWheelPanel);
            playerWheelPanel.BringToFront();
            UpdatePlayerWheelPosition();

            // Update position when form resizes
            this.Resize += (s, e) => UpdatePlayerWheelPosition();
        }

        /// <summary>
        /// Updates player wheel position based on form size.
        /// </summary>
        private void UpdatePlayerWheelPosition()
        {
            if (playerWheelPanel != null)
            {
                // Position on right side, above the timeline (timeline is 45px)
                playerWheelPanel.Location = new Point(
                    this.ClientSize.Width - playerWheelPanel.Width - 10,
                    this.ClientSize.Height - playerWheelPanel.Height - 55
                );
            }
        }

        /// <summary>
        /// Paints the player selection wheel with emblems and team colors.
        /// </summary>
        private void PlayerWheelPanel_Paint(object sender, PaintEventArgs e)
        {
            List<string> playerNames = showLiveTelemetry ? livePlayerNames : pathPlayerNames;
            if (playerNames.Count == 0)
            {
                // Draw "No Players" message
                using (System.Drawing.Font font = new System.Drawing.Font("Segoe UI", 10, FontStyle.Bold))
                using (SolidBrush brush = new SolidBrush(Color.FromArgb(0, 200, 255)))
                {
                    string msg = "No Players";
                    SizeF sz = e.Graphics.MeasureString(msg, font);
                    e.Graphics.DrawString(msg, font, brush,
                        (playerWheelPanel.Width - sz.Width) / 2,
                        (playerWheelPanel.Height - sz.Height) / 2);
                }
                return;
            }

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

            int slotHeight = 50; // Height of each player slot
            int emblemSize = 36;
            int titleHeight = 25; // Space for title at top
            int hintHeight = 20; // Space for hint at bottom
            int centerY = titleHeight + (playerWheelPanel.Height - titleHeight - hintHeight) / 2;
            int totalSlots = playerNames.Count + 1; // +1 for "Free Camera"

            // Calculate visible slot range
            float currentOffset = wheelSpinOffset + wheelSelectedIndex;
            int visibleSlots = 5; // Show 5 slots

            // Draw selection highlight in center
            using (SolidBrush highlightBrush = new SolidBrush(Color.FromArgb(40, 0, 200, 255)))
            {
                e.Graphics.FillRectangle(highlightBrush,
                    5, centerY - slotHeight / 2 - 2,
                    playerWheelPanel.Width - 10, slotHeight + 4);
            }
            using (Pen highlightPen = new Pen(Color.FromArgb(0, 200, 255), 2))
            {
                e.Graphics.DrawRectangle(highlightPen,
                    5, centerY - slotHeight / 2 - 2,
                    playerWheelPanel.Width - 10, slotHeight + 4);
            }

            // Draw player slots with perspective effect
            for (int visOffset = -visibleSlots / 2; visOffset <= visibleSlots / 2; visOffset++)
            {
                float slotIndex = currentOffset + visOffset - wheelSpinOffset;
                int actualIndex = ((int)Math.Round(slotIndex) % totalSlots + totalSlots) % totalSlots;

                // Calculate Y position with wheel curve effect
                float offsetFromCenter = visOffset + (wheelSpinOffset % 1.0f);
                float yPos = centerY + offsetFromCenter * slotHeight;

                // Scale based on distance from center (perspective)
                float distFromCenter = Math.Abs(offsetFromCenter);
                float scale = 1.0f - (distFromCenter * 0.15f);
                float alpha = 1.0f - (distFromCenter * 0.3f);

                if (scale <= 0.3f || alpha <= 0.1f) continue;

                // Get player info
                string playerName;
                Color bgColor;
                PlayerTelemetry playerData = null;

                if (actualIndex == 0)
                {
                    playerName = "Free Camera";
                    bgColor = Color.FromArgb((int)(alpha * 255), 50, 60, 70);
                }
                else
                {
                    playerName = playerNames[actualIndex - 1];
                    int team = GetPlayerTeam(playerName);

                    // Get team background color (white for non-team games)
                    if (!isTeamGame)
                    {
                        bgColor = Color.FromArgb((int)(alpha * 200), 240, 240, 240);
                    }
                    else
                    {
                        Color teamColor = GetTeamColor(team);
                        bgColor = Color.FromArgb((int)(alpha * 200), teamColor.R, teamColor.G, teamColor.B);
                    }

                    // Get player telemetry data for emblem
                    if (showLiveTelemetry)
                    {
                        lock (livePlayersLock)
                        {
                            if (livePlayers.ContainsKey(playerName))
                                playerData = livePlayers[playerName];
                        }
                    }
                    else
                    {
                        // Get emblem data from path player (replay mode)
                        playerData = GetPathPlayerData(playerName);
                    }
                }

                // Calculate scaled dimensions
                int scaledHeight = (int)(slotHeight * scale);
                int scaledEmblemSize = (int)(emblemSize * scale);
                int slotY = (int)(yPos - scaledHeight / 2);
                int slotX = 10;
                int slotWidth = playerWheelPanel.Width - 20;

                // Draw slot background with team color
                using (SolidBrush bgBrush = new SolidBrush(bgColor))
                {
                    e.Graphics.FillRoundedRectangle(bgBrush, slotX, slotY, slotWidth, scaledHeight, 6);
                }

                // Draw emblem if player data available
                int emblemX = slotX + 5;
                int emblemY = slotY + (scaledHeight - scaledEmblemSize) / 2;

                if (playerData != null && actualIndex > 0)
                {
                    string emblemKey = GetEmblemKeyForWheel(playerData);
                    System.Drawing.Image emblemImg = GetOrLoadWheelEmblem(playerData, emblemKey);

                    if (emblemImg != null)
                    {
                        e.Graphics.DrawImage(emblemImg, emblemX, emblemY, scaledEmblemSize, scaledEmblemSize);
                    }
                    else
                    {
                        // Draw placeholder circle
                        using (SolidBrush placeholderBrush = new SolidBrush(Color.FromArgb((int)(alpha * 150), 100, 100, 100)))
                        {
                            e.Graphics.FillEllipse(placeholderBrush, emblemX, emblemY, scaledEmblemSize, scaledEmblemSize);
                        }
                    }
                }
                else if (actualIndex == 0)
                {
                    // Draw camera icon for Free Camera
                    using (System.Drawing.Font iconFont = new System.Drawing.Font("Segoe UI Symbol", 16 * scale, FontStyle.Regular))
                    using (SolidBrush iconBrush = new SolidBrush(Color.FromArgb((int)(alpha * 255), 0, 200, 255)))
                    {
                        e.Graphics.DrawString("📷", iconFont, iconBrush, emblemX, emblemY);
                    }
                }

                // Draw player name
                int textX = emblemX + scaledEmblemSize + 8;
                int textY = slotY + (scaledHeight - (int)(14 * scale)) / 2;
                Color textColor = actualIndex == 0 ?
                    Color.FromArgb((int)(alpha * 255), 0, 200, 255) :
                    Color.FromArgb((int)(alpha * 255), 255, 255, 255);

                using (System.Drawing.Font nameFont = new System.Drawing.Font("Segoe UI", 9 * scale, FontStyle.Bold))
                using (SolidBrush textBrush = new SolidBrush(textColor))
                {
                    // Truncate name if too long
                    string displayName = playerName;
                    SizeF textSize = e.Graphics.MeasureString(displayName, nameFont);
                    int maxWidth = slotWidth - scaledEmblemSize - 20;
                    while (textSize.Width > maxWidth && displayName.Length > 3)
                    {
                        displayName = displayName.Substring(0, displayName.Length - 1);
                        textSize = e.Graphics.MeasureString(displayName + "...", nameFont);
                    }
                    if (displayName != playerName) displayName += "...";

                    e.Graphics.DrawString(displayName, nameFont, textBrush, textX, textY);
                }
            }

            // Draw spin instruction at bottom
            using (System.Drawing.Font hintFont = new System.Drawing.Font("Segoe UI", 8, FontStyle.Regular))
            using (SolidBrush hintBrush = new SolidBrush(Color.FromArgb(150, 0, 200, 255)))
            {
                string hint = "Click to spin";
                SizeF hintSize = e.Graphics.MeasureString(hint, hintFont);
                e.Graphics.DrawString(hint, hintFont, hintBrush,
                    (playerWheelPanel.Width - hintSize.Width) / 2,
                    playerWheelPanel.Height - hintSize.Height - 5);
            }
        }

        /// <summary>
        /// Handles click on player wheel to start spinning.
        /// </summary>
        private void PlayerWheelPanel_MouseClick(object sender, MouseEventArgs e)
        {
            List<string> playerNames = showLiveTelemetry ? livePlayerNames : pathPlayerNames;
            if (playerNames.Count == 0) return;

            if (e.Button == MouseButtons.Left)
            {
                if (!wheelIsSpinning)
                {
                    // Start spin animation with random velocity
                    Random rnd = new Random();
                    wheelSpinVelocity = 15f + (float)(rnd.NextDouble() * 10);
                    wheelIsSpinning = true;
                    wheelSpinTimer.Start();
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                // Right click selects immediately without spin
                int slotHeight = 50;
                int titleHeight = 25;
                int hintHeight = 20;
                int centerY = titleHeight + (playerWheelPanel.Height - titleHeight - hintHeight) / 2;
                int clickedSlot = (int)Math.Round((e.Y - centerY) / (float)slotHeight);
                int totalSlots = playerNames.Count + 1;
                int newIndex = (wheelSelectedIndex + clickedSlot + totalSlots) % totalSlots;
                wheelSelectedIndex = newIndex;
                UpdatePOVFromWheel();
                playerWheelPanel.Invalidate();
            }
        }

        /// <summary>
        /// Timer tick for wheel spin animation.
        /// </summary>
        private void WheelSpinTimer_Tick(object sender, EventArgs e)
        {
            List<string> playerNames = showLiveTelemetry ? livePlayerNames : pathPlayerNames;
            int totalSlots = playerNames.Count + 1;

            // Apply deceleration
            wheelSpinVelocity *= 0.97f;
            wheelSpinOffset += wheelSpinVelocity * 0.016f; // 16ms per tick

            // Wrap offset
            while (wheelSpinOffset >= totalSlots) wheelSpinOffset -= totalSlots;
            while (wheelSpinOffset < 0) wheelSpinOffset += totalSlots;

            // Stop when velocity is low enough
            if (wheelSpinVelocity < 0.5f)
            {
                wheelSpinTimer.Stop();
                wheelIsSpinning = false;

                // Snap to nearest slot
                wheelSelectedIndex = (int)Math.Round(wheelSpinOffset) % totalSlots;
                wheelSpinOffset = 0;

                UpdatePOVFromWheel();
            }

            playerWheelPanel.Invalidate();
        }

        /// <summary>
        /// Updates POV mode based on wheel selection.
        /// </summary>
        private void UpdatePOVFromWheel()
        {
            List<string> playerNames = showLiveTelemetry ? livePlayerNames : pathPlayerNames;

            if (wheelSelectedIndex == 0)
            {
                // Free Camera
                povModeEnabled = false;
                povFollowPlayer = null;
            }
            else if (wheelSelectedIndex <= playerNames.Count)
            {
                // Follow selected player
                povModeEnabled = true;
                povFollowPlayer = playerNames[wheelSelectedIndex - 1];
            }

            // Sync with POV dropdown
            if (povPlayerDropdown != null && povPlayerDropdown.Items.Count > wheelSelectedIndex)
            {
                povPlayerDropdown.SelectedIndex = wheelSelectedIndex;
            }
        }

        /// <summary>
        /// Gets emblem key for wheel display.
        /// </summary>
        private string GetEmblemKeyForWheel(PlayerTelemetry player)
        {
            return $"{player.EmblemFg}_{player.EmblemBg}_{player.ColorPrimary}_{player.ColorSecondary}";
        }

        /// <summary>
        /// Gets or loads an emblem image for wheel display.
        /// </summary>
        private System.Drawing.Image GetOrLoadWheelEmblem(PlayerTelemetry player, string emblemKey)
        {
            // Return cached image if available
            if (wheelEmblemCache.ContainsKey(emblemKey))
            {
                return wheelEmblemCache[emblemKey];
            }

            // Start async load if not already loading
            if (!wheelEmblemLoadingSet.Contains(emblemKey))
            {
                wheelEmblemLoadingSet.Add(emblemKey);
                string url = GetEmblemUrl(player);

                System.Threading.ThreadPool.QueueUserWorkItem(_ =>
                {
                    try
                    {
                        using (var webClient = new System.Net.WebClient())
                        {
                            webClient.Headers.Add("User-Agent", "Entity-BSPViewer/1.0");
                            byte[] imageData = webClient.DownloadData(url);

                            this.BeginInvoke(new System.Action(() =>
                            {
                                try
                                {
                                    using (var ms = new System.IO.MemoryStream(imageData))
                                    {
                                        System.Drawing.Image img = System.Drawing.Image.FromStream(ms);
                                        wheelEmblemCache[emblemKey] = img;
                                        playerWheelPanel?.Invalidate();
                                    }
                                }
                                catch { }
                                finally
                                {
                                    wheelEmblemLoadingSet.Remove(emblemKey);
                                }
                            }));
                        }
                    }
                    catch
                    {
                        wheelEmblemLoadingSet.Remove(emblemKey);
                    }
                });
            }

            return null;
        }

        /// <summary>
        /// Refreshes the player wheel when player list changes.
        /// </summary>
        private void RefreshPlayerWheel()
        {
            // Ensure selected index is valid
            List<string> playerNames = showLiveTelemetry ? livePlayerNames : pathPlayerNames;
            int totalSlots = playerNames.Count + 1;
            wheelSelectedIndex = Math.Min(wheelSelectedIndex, totalSlots - 1);
            playerWheelPanel?.Invalidate();
        }

        /// <summary>
        /// Gets player telemetry data from path data for emblem display in replay mode.
        /// </summary>
        private PlayerTelemetry GetPathPlayerData(string playerName)
        {
            if (!multiPlayerPaths.ContainsKey(playerName))
                return null;

            var segments = multiPlayerPaths[playerName];
            if (segments.Count == 0)
                return null;

            // Find a point with emblem data (any point since emblem data doesn't change)
            foreach (var segment in segments)
            {
                if (segment.Count > 0)
                {
                    var pt = segment[0];
                    PlayerTelemetry data = new PlayerTelemetry();
                    data.PlayerName = pt.PlayerName;
                    data.Team = pt.Team;
                    data.EmblemFg = pt.EmblemFg;
                    data.EmblemBg = pt.EmblemBg;
                    data.ColorPrimary = pt.ColorPrimary;
                    data.ColorSecondary = pt.ColorSecondary;
                    data.ColorTertiary = pt.ColorTertiary;
                    data.ColorQuaternary = pt.ColorQuaternary;
                    return data;
                }
            }

            return null;
        }

        /// <summary>
        /// Enables recommended view options for replay/live telemetry mode.
        /// </summary>
        private void EnableTelemetryViewOptions()
        {
            // Enable RenderSky
            if (RenderSky != null)
                RenderSky.Checked = true;

            // Enable spawn types that are useful for viewing: Scenery, Collection, Obstacle
            string[] spawnTypesToEnable = { "Scenery", "Collection", "Obstacle", "Vehicle", "Weapon" };

            if (checkedListBox1 != null)
            {
                for (int i = 0; i < checkedListBox1.Items.Count; i++)
                {
                    string itemName = checkedListBox1.Items[i].ToString();
                    foreach (string spawnType in spawnTypesToEnable)
                    {
                        if (itemName == spawnType)
                        {
                            checkedListBox1.SetItemChecked(i, true);
                            setSpawnBox(itemName, CheckState.Checked);
                            break;
                        }
                    }
                }
            }

            // Enable BSP textures if available
            if (cbBSPTextures != null)
                cbBSPTextures.Checked = true;

            // Enable BSP lighting if available
            if (BSPLighting != null)
                BSPLighting.Checked = true;
        }

        /// <summary>
        /// Updates player dropdowns with live player names when in live telemetry mode.
        /// </summary>
        private void UpdateLivePlayerDropdowns()
        {
            if (!showLiveTelemetry)
                return;

            List<string> currentPlayers;
            lock (livePlayersLock)
            {
                currentPlayers = new List<string>(livePlayers.Keys);
            }

            // Check if player list changed
            bool changed = currentPlayers.Count != livePlayerNames.Count;
            if (!changed)
            {
                foreach (string name in currentPlayers)
                {
                    if (!livePlayerNames.Contains(name))
                    {
                        changed = true;
                        break;
                    }
                }
            }

            if (!changed)
                return;

            livePlayerNames = currentPlayers;

            // Update dropdowns on UI thread
            if (pathPlayerDropdown != null && pathPlayerDropdown.GetCurrentParent() != null)
            {
                try
                {
                    if (pathPlayerDropdown.GetCurrentParent().InvokeRequired)
                    {
                        pathPlayerDropdown.GetCurrentParent().BeginInvoke(new System.Action(() => RefreshPlayerDropdowns()));
                    }
                    else
                    {
                        RefreshPlayerDropdowns();
                    }
                }
                catch { }
            }
        }

        /// <summary>
        /// Gets a team color indicator prefix for display in dropdowns.
        /// </summary>
        private string GetTeamColorPrefix(int team)
        {
            switch (team)
            {
                case 0: return "🔴 "; // Red
                case 1: return "🔵 "; // Blue
                case 2: return "🟢 "; // Green
                case 3: return "🟠 "; // Orange
                default: return "⚪ "; // Unknown/FFA (white)
            }
        }

        /// <summary>
        /// Gets the team for a player name.
        /// </summary>
        private int GetPlayerTeam(string playerName)
        {
            if (showLiveTelemetry)
            {
                lock (livePlayersLock)
                {
                    if (livePlayers.ContainsKey(playerName))
                        return livePlayers[playerName].Team;
                }
            }
            else
            {
                // Check playback path data
                if (multiPlayerPaths.ContainsKey(playerName) && multiPlayerPaths[playerName].Count > 0)
                {
                    var firstSegment = multiPlayerPaths[playerName][0];
                    if (firstSegment.Count > 0)
                        return firstSegment[0].Team;
                }
            }
            return -1; // Unknown
        }

        /// <summary>
        /// Refreshes the player and POV dropdowns with current player names.
        /// </summary>
        private void RefreshPlayerDropdowns()
        {
            List<string> playerNames = showLiveTelemetry ? livePlayerNames : pathPlayerNames;
            string prevPovPlayer = povFollowPlayer;

            // Update player visibility dropdown (checkable menu)
            if (pathPlayerDropdown != null)
            {
                pathPlayerDropdown.DropDownItems.Clear();

                // Show All option
                ToolStripMenuItem showAllItem = new ToolStripMenuItem("✓ Show All");
                showAllItem.Click += (s, e) => {
                    hiddenPlayers.Clear();
                    UpdatePlayerDropdownChecks();
                };
                pathPlayerDropdown.DropDownItems.Add(showAllItem);

                // Hide All option
                ToolStripMenuItem hideAllItem = new ToolStripMenuItem("✗ Hide All");
                hideAllItem.Click += (s, e) => {
                    List<string> names = showLiveTelemetry ? livePlayerNames : pathPlayerNames;
                    foreach (string name in names)
                        hiddenPlayers.Add(name);
                    UpdatePlayerDropdownChecks();
                };
                pathPlayerDropdown.DropDownItems.Add(hideAllItem);

                pathPlayerDropdown.DropDownItems.Add(new ToolStripSeparator());

                // Add checkable item for each player
                foreach (string name in playerNames)
                {
                    int team = GetPlayerTeam(name);
                    string prefix = GetTeamColorPrefix(team);
                    bool isVisible = !hiddenPlayers.Contains(name);

                    ToolStripMenuItem playerItem = new ToolStripMenuItem(prefix + name);
                    playerItem.Checked = isVisible;
                    playerItem.CheckOnClick = true;
                    playerItem.Tag = name; // Store actual name without prefix
                    playerItem.CheckedChanged += (s, e) => {
                        ToolStripMenuItem item = (ToolStripMenuItem)s;
                        string playerName = (string)item.Tag;
                        if (item.Checked)
                            hiddenPlayers.Remove(playerName);
                        else
                            hiddenPlayers.Add(playerName);
                        UpdatePlayerDropdownText();
                    };
                    pathPlayerDropdown.DropDownItems.Add(playerItem);
                }

                UpdatePlayerDropdownText();
            }

            // Update POV dropdown
            if (povPlayerDropdown != null)
            {
                povPlayerDropdown.Items.Clear();
                povPlayerDropdown.Items.Add("Free Camera");
                foreach (string name in playerNames)
                {
                    int team = GetPlayerTeam(name);
                    string prefix = GetTeamColorPrefix(team);
                    povPlayerDropdown.Items.Add(prefix + name);
                }
                // Try to restore selection
                int povIdx = prevPovPlayer != null ? playerNames.IndexOf(prevPovPlayer) + 1 : 0;
                povPlayerDropdown.SelectedIndex = Math.Max(0, Math.Min(povIdx, povPlayerDropdown.Items.Count - 1));
            }

            // Refresh player wheel
            RefreshPlayerWheel();
        }

        /// <summary>
        /// Updates the player dropdown button text based on visibility state.
        /// </summary>
        private void UpdatePlayerDropdownText()
        {
            if (pathPlayerDropdown == null) return;
            List<string> playerNames = showLiveTelemetry ? livePlayerNames : pathPlayerNames;
            int visibleCount = playerNames.Count - hiddenPlayers.Count(n => playerNames.Contains(n));
            if (visibleCount == playerNames.Count)
                pathPlayerDropdown.Text = "All Visible";
            else if (visibleCount == 0)
                pathPlayerDropdown.Text = "None Visible";
            else
                pathPlayerDropdown.Text = $"{visibleCount}/{playerNames.Count} Visible";
        }

        /// <summary>
        /// Updates the checked state of player dropdown items.
        /// </summary>
        private void UpdatePlayerDropdownChecks()
        {
            if (pathPlayerDropdown == null) return;
            foreach (ToolStripItem item in pathPlayerDropdown.DropDownItems)
            {
                if (item is ToolStripMenuItem menuItem && menuItem.Tag is string playerName)
                {
                    menuItem.Checked = !hiddenPlayers.Contains(playerName);
                }
            }
            UpdatePlayerDropdownText();
        }

        private Form debugForm = null;
        private TextBox debugTextBox = null;
        private System.Windows.Forms.Timer debugTimer = null;

        private void ShowTelemetryDebug()
        {
            // If already open, just bring to front
            if (debugForm != null && !debugForm.IsDisposed)
            {
                debugForm.BringToFront();
                return;
            }

            // Create debug window
            debugForm = new Form();
            debugForm.Text = showLiveTelemetry ? "Live Telemetry Debug" : "Replay Data Debug";
            debugForm.Size = new System.Drawing.Size(600, 500);
            debugForm.StartPosition = FormStartPosition.CenterParent;

            debugTextBox = new TextBox();
            debugTextBox.Multiline = true;
            debugTextBox.ReadOnly = true;
            debugTextBox.ScrollBars = ScrollBars.Both;
            debugTextBox.Dock = DockStyle.Fill;
            debugTextBox.Font = new System.Drawing.Font("Consolas", 9);
            debugForm.Controls.Add(debugTextBox);

            // Timer to update every 100ms
            debugTimer = new System.Windows.Forms.Timer();
            debugTimer.Interval = 100;
            debugTimer.Tick += (s, e) => UpdateDebugText();
            debugTimer.Start();

            debugForm.FormClosed += (s, e) => {
                debugTimer.Stop();
                debugTimer.Dispose();
                debugTimer = null;
            };

            UpdateDebugText();
            debugForm.Show();
        }

        private void UpdateDebugText()
        {
            if (debugTextBox == null || debugTextBox.IsDisposed) return;

            StringBuilder sb = new StringBuilder();

            if (showLiveTelemetry)
            {
                // Live telemetry mode
                sb.AppendLine("=== LIVE TELEMETRY DEBUG (updates every 100ms) ===\n");

                sb.AppendLine($"Listener Running: {telemetryListenerRunning}");
                sb.AppendLine($"Show Live Telemetry: {showLiveTelemetry}");
                sb.AppendLine($"CSV Columns Parsed: {csvColumnIndices.Count}");

                // Show parsed column names
                sb.AppendLine("Columns: " + string.Join(", ", csvColumnIndices.Keys));

                lock (livePlayersLock)
                {
                    sb.AppendLine($"Live Players: {livePlayers.Count}");
                    foreach (var kvp in livePlayers)
                    {
                        var p = kvp.Value;
                        sb.AppendLine($"  - {p.PlayerName}: Pos=({p.PosX:F1}, {p.PosY:F1}, {p.PosZ:F1})");
                        sb.AppendLine($"    Team={p.Team} Spd={p.Speed:F1} Yaw={p.Yaw:F2} YawDeg={p.YawDeg:F0}°");
                        sb.AppendLine($"    Crouch={p.IsCrouching} Air={p.IsAirborne}");
                        sb.AppendLine($"    Weapon={p.CurrentWeapon} Frags={p.FragGrenades} Plasma={p.PlasmaGrenades}");
                        sb.AppendLine($"    K/D: {p.Kills}/{p.Deaths} RespawnTimer={p.RespawnTimer} IsDead={p.IsDead}");
                        sb.AppendLine($"    Emblem: FG={p.EmblemFg} BG={p.EmblemBg} Colors={p.ColorPrimary},{p.ColorSecondary},{p.ColorTertiary},{p.ColorQuaternary}");
                    }
                }

                sb.AppendLine("\n=== RECENT LOG ===\n");
                lock (telemetryDebugLogLock)
                {
                    foreach (var entry in telemetryDebugLog)
                    {
                        sb.AppendLine(entry);
                    }
                    if (telemetryDebugLog.Count == 0)
                    {
                        sb.AppendLine("(no data received yet)");
                    }
                }
            }
            else
            {
                // Replay mode - show path data
                sb.AppendLine("=== REPLAY DATA DEBUG ===\n");

                sb.AppendLine($"Path Points: {playerPath.Count}");
                sb.AppendLine($"Players: {pathPlayerNames.Count} - {string.Join(", ", pathPlayerNames)}");
                sb.AppendLine($"Kill Events: {killEvents.Count}");
                sb.AppendLine($"Timeline: {pathMinTimestamp:F1} to {pathMaxTimestamp:F1} (current: {pathCurrentTimestamp:F1})");
                sb.AppendLine($"Playing: {pathIsPlaying} Speed: {pathPlaybackSpeed}x");
                sb.AppendLine($"Hidden: {string.Join(", ", hiddenPlayers)}");

                // Show current player data at current timestamp
                sb.AppendLine("\n=== PLAYERS AT CURRENT TIME ===\n");

                foreach (var kvp in multiPlayerPaths)
                {
                    string playerName = kvp.Key;
                    PlayerPathPoint? currentPt = null;

                    foreach (var segment in kvp.Value)
                    {
                        foreach (var pt in segment)
                        {
                            if (pt.Timestamp <= pathCurrentTimestamp)
                                currentPt = pt;
                            else
                                break;
                        }
                    }

                    if (currentPt.HasValue)
                    {
                        var p = currentPt.Value;
                        sb.AppendLine($"  - {p.PlayerName}: Pos=({p.X:F1}, {p.Y:F1}, {p.Z:F1}) T={p.Timestamp:F1}");
                        sb.AppendLine($"    Team={p.Team} Yaw={p.FacingYaw:F1}° Dead={p.IsDead}");
                        sb.AppendLine($"    Weapon={p.CurrentWeapon} Crouch={p.IsCrouching} Air={p.IsAirborne}");
                        sb.AppendLine($"    Emblem: FG={p.EmblemFg} BG={p.EmblemBg} Colors={p.ColorPrimary},{p.ColorSecondary},{p.ColorTertiary},{p.ColorQuaternary}");
                    }
                }

                // Show recent kills
                sb.AppendLine("\n=== KILL EVENTS ===\n");
                var filteredKills = killEvents.Where(k => k.Timestamp <= pathCurrentTimestamp).ToList();
                var recentKills = filteredKills.Skip(Math.Max(0, filteredKills.Count - 10)).ToList();
                foreach (var kill in recentKills)
                {
                    string teamName = kill.Team == 0 ? "Red" : kill.Team == 1 ? "Blue" : kill.Team == 2 ? "Green" : kill.Team == 3 ? "Orange" : "FFA";
                    sb.AppendLine($"  T={kill.Timestamp:F1}: {kill.PlayerName} ({teamName}) - {kill.Weapon}");
                }
                if (!recentKills.Any())
                    sb.AppendLine("  (no kills yet)");
            }

            debugTextBox.Text = sb.ToString();
        }

        #endregion

        #region Enums

        /// <summary>
        /// The selected item rotation type.
        /// </summary>
        /// <remarks></remarks>
        [FlagsAttribute]
        public enum SelectedItemRotationType
        {
            /// <summary>
            /// The none.
            /// </summary>
            None = 0x00, 

            /// <summary>
            /// The control.
            /// </summary>
            Control = 0x01, 

            /// <summary>
            /// The shift.
            /// </summary>
            Shift = 0x02, 

            /// <summary>
            /// The alt.
            /// </summary>
            Alt = 0x04
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// The h 2 bsp write raw data meta chunk.
        /// </summary>
        /// <param name="chunknumber">The chunknumber.</param>
        /// <param name="bsp">The bsp.</param>
        /// <param name="BSPNum">The bsp num.</param>
        /// <remarks></remarks>
        public void H2BSPWriteRawDataMetaChunk(int chunknumber, ref BSPModel bsp, int BSPNum)
        {
            BSPModel.BSPRawDataMetaChunk bspChunk = bsp.BSPRawDataMetaChunks[chunknumber];

            int BSPId = map.Functions.ForMeta.FindMetaByID(map.BSP.sbsp[BSPNum].ident);

            Meta meta = new Meta(map);
            meta.TagIndex = BSPId; // decides which tag to load into MemoryStream (MS)
            meta.ScanMetaItems(true, false);

            BinaryWriter BW = new BinaryWriter(meta.MS);
            BinaryReader BR = new BinaryReader(meta.MS);
            int temp = 0;

            BW.BaseStream.Position = bsp.BSPRawDataMetaChunksOffset;
            BW.Write(bspChunk.VerticeCount);
            BW.Write(bspChunk.FaceCount);
            BW.BaseStream.Position = bsp.BSPRawDataMetaChunksOffset + 48;
            BW.Write(bspChunk.HeaderSize - 8);

            BW.BaseStream.Position = bsp.BSPRawDataMetaChunksOffset + 52;
            temp = BR.ReadInt32(); // I dunno what this is or what value it is supposed to be! #1232
            BW.BaseStream.Position = bsp.BSPRawDataMetaChunksOffset + 52;
            BW.Write(temp); // I dunno what this is or what value it is supposed to be!

            BW.BaseStream.Position = bsp.BSPRawDataMetaChunksOffset + 56;
            BW.Write(bspChunk.RawDataChunkInfo.Length);

            BW.Write(bspChunk.rawdatainfooffset + meta.magic + meta.offset);

            // SO FAR SO GOOD UP TILL NOW!
            // this.RawDataChunkInfo = new RawDataOffsetChunk[tempc];
            int tempr = bspChunk.rawdatainfooffset;
            for (int x = 0; x < bspChunk.RawDataChunkInfo.Length; x++)
            {
                // this.RawDataChunkInfo[x] = new RawDataOffsetChunk();
                BW.BaseStream.Position = tempr + (x * 16) + 6;
                BW.Write((UInt16)bspChunk.RawDataChunkInfo[x].ChunkSize);
                BW.Write(bspChunk.RawDataChunkInfo[x].Size);

                // I think this may be a safety for a messed up map?
                // if (bspChunk.RawDataChunkInfo[x].ChunkSize == bspChunk.RawDataChunkInfo[x].Size) { bspChunk.RawDataChunkInfo[x].ChunkSize = 0; }
                BW.Write(bspChunk.RawDataChunkInfo[x].Offset);
            }

            BW = map.BW;
            BW.BaseStream.Position = meta.offset;
            BW.Write(meta.MS.GetBuffer(), 0, meta.size);

            if (bspChunk.RawDataChunkInfo.Length == 0)
            {
                return;
            }

            /*
                BW = new BinaryWriter(((RawDataChunk)meta.raw.rawChunks[chunknumber]).MS);
                bspChunk.SubMeshInfo = new ModelSubMeshInfo[bspChunk.RawDataChunkInfo[0].ChunkCount];
                for (int x = 0; x < bspChunk.RawDataChunkInfo[0].ChunkCount; x++)
                {
                    bspChunk.SubMeshInfo[x] = new ModelSubMeshInfo();
                    BR.BaseStream.Position = bspChunk.HeaderSize + bspChunk.RawDataChunkInfo[0].Offset + (x * 72) + 4;
                    bspChunk.SubMeshInfo[x].ShaderNumber = BR.ReadUInt16();
                    bspChunk.SubMeshInfo[x].IndiceStart = BR.ReadUInt16();
                    bspChunk.SubMeshInfo[x].IndiceCount = BR.ReadUInt16();

                }

                BR.BaseStream.Position = 40;
                bspChunk.IndiceCount = BR.ReadUInt16();
                int indicechunk = 0;
                int verticechunk = 0;
                int uvchunk = 0;
                for (int x = 0; x < RawDataChunkInfo.Length; x++)
                {
                    if (RawDataChunkInfo[x].ChunkSize == 2)
                    {
                        indicechunk = x;
                        break;
                    }
                }
                int normalchunk = 0;
                for (int x = indicechunk; x < RawDataChunkInfo.Length; x++)
                {
                    if (RawDataChunkInfo[x].ChunkCount == 1)
                    {
                        verticechunk = x;
                        uvchunk = x + 1;
                        normalchunk = x + 2;
                        break;
                    }
                }
                bspChunk.Indices = new short[bspChunk.RawDataChunkInfo[indicechunk].ChunkCount];
                BR.BaseStream.Position = bspChunk.HeaderSize + bspChunk.RawDataChunkInfo[indicechunk].Offset;
                for (int x = 0; x < bspChunk.IndiceCount; x++)
                {
                    bspChunk.Indices[x] = (short)BR.ReadUInt16();

                }

                bspChunk.RawDataChunkInfo[verticechunk].ChunkSize = bspChunk.RawDataChunkInfo[verticechunk].Size / VerticeCount;
                for (int x = 0; x < bspChunk.VerticeCount; x++)
                {
                    Vector3 vec = new Vector3();
                    BR.BaseStream.Position = bspChunk.HeaderSize + bspChunk.RawDataChunkInfo[verticechunk].Offset + (bspChunk.RawDataChunkInfo[verticechunk].ChunkSize * x);
                    vec.X = BR.ReadSingle();
                    vec.Y = BR.ReadSingle();
                    vec.Z = BR.ReadSingle();
                    Vertices.Add(vec);
                }

                bspChunk.RawDataChunkInfo[uvchunk].ChunkSize = 8;
                for (int x = 0; x < bspChunk.VerticeCount; x++)
                {
                    Vector2 tempuv = new Vector2();
                    BR.BaseStream.Position = bspChunk.HeaderSize + bspChunk.RawDataChunkInfo[uvchunk].Offset + (bspChunk.RawDataChunkInfo[uvchunk].ChunkSize * x);
                    tempuv.X = BR.ReadSingle();
                    tempuv.Y = BR.ReadSingle();
                    bspChunk.UVs.Add(tempuv);
                }

                bspChunk.RawDataChunkInfo[normalchunk].ChunkSize = 12;
                for (int x = 0; x < bspChunk.VerticeCount; x++)
                {
                    Vector2 tempuv = new Vector2();
                    BR.BaseStream.Position = bspChunk.HeaderSize + bspChunk.RawDataChunkInfo[normalchunk].Offset + (bspChunk.RawDataChunkInfo[normalchunk].ChunkSize * x);
                    Vector3 normal = Raw.ParsedModel.DecompressNormal(BR.ReadInt32());
                    bspChunk.Normals.Add(normal);
                    Vector3 binormal = Raw.ParsedModel.DecompressNormal(BR.ReadInt32());
                    bspChunk.Binormals.Add(binormal);
                    Vector3 tangent = Raw.ParsedModel.DecompressNormal(BR.ReadInt32());
                    bspChunk.Tangents.Add(tangent);
                }

                int lightmapuvchunk = -1;
                for (int x = normalchunk + 1; x < RawDataChunkInfo.Length; x++)
                {
                    if (RawDataChunkInfo[x].ChunkSize == 3)
                    {
                        lightmapuvchunk = x;
                        break;
                    }
                }

                if (lightmapuvchunk == -1) return;
                RawDataChunkInfo[lightmapuvchunk].ChunkSize = 4;
                for (int x = 0; x < bspChunk.VerticeCount; x++)
                {
                    Vector2 tempuv = new Vector2();
                    BR.BaseStream.Position = bspChunk.HeaderSize + bspChunk.RawDataChunkInfo[lightmapuvchunk].Offset + (bspChunk.RawDataChunkInfo[lightmapuvchunk].ChunkSize * x);
                    short testx = BR.ReadInt16();
                    float u = DecompressVertice(Convert.ToSingle(testx), -1, 1);
                    testx = BR.ReadInt16();
                    float v = DecompressVertice(Convert.ToSingle(testx), -1, 1);
                     Vector2 uv2 = new Vector2(u, v);
                    bspChunk.LightMapUVs.Add(uv2);
                }
            */
        }

        /// <summary>
        /// The initialize graphics.
        /// </summary>
        /// <returns>The initialize graphics.</returns>
        /// <remarks></remarks>
        public bool InitializeGraphics()
        {
            // try
            // {
            this.Show();
            this.Focus();
            Application.DoEvents();
           
            render.CreateDevice(this);
            render.pause = true;

            #region CreateHaloDirectXResources

            #region List of all BSP sections

            checkedListBox2.Enabled = false;
            checkedListBox2.Items.Clear();

            for (int x = 0; x < bsp.BSPRawDataMetaChunks.Length; x++)
            {
                checkedListBox2.Items.Add("BSP #" + x.ToString() + " (" + (bsp.BSPRawDataMetaChunks[x].FaceCount).ToString() + " Faces)", true);
            }

            checkedListBox2.Enabled = true;

            #endregion

            this.label3.Text = ".:Loading Textures & Skybox:.";
            this.label3.Refresh();

            BSPModel.BSPDisplayedInfo.LoadDirectXTexturesAndBuffers(ref render.device, ref bsp);
            ParsedModel.DisplayedInfo.LoadDirectXTexturesAndBuffers(ref render.device, ref bsp.SkyBox);
            LoadSpawns();
            MakeMatrixes();

            #endregion

            #region InitializeMaterials

            RedMaterial = new Material();
            RedMaterial.Diffuse = Color.Red;
            RedMaterial.Ambient = Color.Red;
            BlueMaterial = new Material();
            BlueMaterial.Diffuse = Color.Blue;
            BlueMaterial.Ambient = Color.Blue;
            YellowMaterial = new Material();
            YellowMaterial.Diffuse = Color.Yellow;
            YellowMaterial.Ambient = Color.Yellow;
            GreenMaterial = new Material();
            GreenMaterial.Diffuse = Color.SpringGreen;
            GreenMaterial.Ambient = Color.SpringGreen;
            PurpleMaterial = new Material();
            PurpleMaterial.Diffuse = Color.BlueViolet;
            PurpleMaterial.Ambient = Color.BlueViolet;
            OrangeMaterial = new Material();
            OrangeMaterial.Diffuse = Color.Orange;
            OrangeMaterial.Ambient = Color.Orange;
            BrownMaterial = new Material();
            BrownMaterial.Diffuse = Color.Chocolate;
            BrownMaterial.Ambient = Color.Chocolate;
            PinkMaterial = new Material();
            PinkMaterial.Diffuse = Color.Pink;
            PinkMaterial.Ambient = Color.Pink;
            NeutralMaterial = new Material();
            NeutralMaterial.Diffuse = Color.GreenYellow;
            NeutralMaterial.Ambient = Color.GreenYellow;
            DefaultMaterial = new Material();
            DefaultMaterial.Diffuse = Color.Crimson;
            DefaultMaterial.Ambient = Color.Crimson;
            BlackMaterial = new Material();
            BlackMaterial.Diffuse = Color.Black;
            BlackMaterial.Ambient = Color.Black;
            BlackMaterial.Specular = Color.Black;

            #endregion

            // gizmo = new Entity.Renderer.Widget.Gizmo(render.device);

            render.pause = false;
            label3.Visible = false;
            
            return true;

            // }
            // catch (DirectXException)
            // {
            // Catch any errors and return a failure
            // 	return false;
            // 	}
        }

        /// <summary>
        /// The load spawns.
        /// </summary>
        /// <remarks></remarks>
        public void LoadSpawns()
        {
            spawns = new SpawnLoads(map, bsp, render.device);
            SpawnModel = new List<ParsedModel>();
            spawnmodelindex = new int[bsp.Spawns.Spawn.Count];
            BoundingBoxModel = new Mesh[bsp.Spawns.Spawn.Count];

            map.OpenMap(MapTypes.Internal);

            int blockCount = 0;
            int scenCount = 0;
            for (int x = 0; x < bsp.Spawns.Spawn.Count; x++)
            {
                // Display loading information
                if (x % 7 == 0)
                {
                    this.label3.Text = ".:Loading Spawns [" + x.ToString() + "/" + bsp.Spawns.Spawn.Count.ToString() + "]:.";
                    // Every 5 updates, refresh whole window, otherwise just update the label
                    if (x % 35 == 0)
                        Application.DoEvents();
                    else
                        this.label3.Refresh();
                }

                // This is the only way I could think of doing it right now...
                // Used for saving Obstacle & Scenery to their original places
                if (bsp.Spawns.Spawn[x] is SpawnInfo.ObstacleSpawn)
                {
                    ((SpawnInfo.ObstacleSpawn)bsp.Spawns.Spawn[x]).BlocNumber = blockCount++;
                }
                else if (bsp.Spawns.Spawn[x] is SpawnInfo.ScenerySpawn)
                {
                    ((SpawnInfo.ScenerySpawn)bsp.Spawns.Spawn[x]).ScenNumber = scenCount++;
                }

                if (bsp.Spawns.Spawn[x] is SpawnInfo.BoundingBoxSpawn)
                {
                    BoundingBoxModel[x] = loadBoundingBoxSpawn(bsp.Spawns.Spawn[x]);
                    continue;
                }

                #region CameraSpawn
                if (bsp.Spawns.Spawn[x] is SpawnInfo.CameraSpawn)
                {
                    BoundingBoxModel[x] = loadCameraSpawn(bsp.Spawns.Spawn[x]);
                    continue;
                }
                #endregion

                #region LightSpawn

                if (bsp.Spawns.Spawn[x] is SpawnInfo.LightSpawn)
                {
                    SpawnInfo.LightSpawn tempbox;
                    tempbox = bsp.Spawns.Spawn[x] as SpawnInfo.LightSpawn;
                    tempbox.LightInfo = new HaloLight(tempbox.ModelTagNumber, map);
                    bsp.Spawns.Spawn[x] = tempbox;
                    BoundingBoxModel[x] = Mesh.Cylinder(render.device, 0.5f, 0.0f, 1f, 10, 10);
                    if (render.lighting)
                    {
                        render.device.Lights[HaloLightCount].Type = LightType.Point;
                        render.device.Lights[HaloLightCount].Position = new Vector3(tempbox.X, tempbox.Y, tempbox.Z);
                        render.device.Lights[HaloLightCount].Direction = new Vector3(
                            -tempbox.Yaw, -tempbox.Pitch, -tempbox.Roll);
                        render.device.Lights[HaloLightCount].Range = 10f;

                        // render.device.Lights[HaloLightCount].=0.5f;
                        // render.device.Lights[HaloLightCount].p = 1.0f;
                        render.device.Lights[HaloLightCount].Falloff = 1.0f;
                        render.device.Lights[HaloLightCount].Attenuation0 = 1.0f;
                        render.device.Lights[HaloLightCount].Diffuse = Color.FromArgb(
                            tempbox.LightInfo.r, tempbox.LightInfo.g, tempbox.LightInfo.b);
                        render.device.Lights[HaloLightCount].Update();
                        render.device.Lights[HaloLightCount].Enabled = true;
                    }

                    HaloLightCount++;

                    continue;
                }

                #endregion

                #region SoundSpawn
                if (bsp.Spawns.Spawn[x] is SpawnInfo.SoundSpawn)
                {
                    BoundingBoxModel[x] = loadSoundSpawn(bsp.Spawns.Spawn[x]);
                    continue;
                }
                #endregion

                #region SpawnZone
                if (bsp.Spawns.Spawn[x] is SpawnInfo.SpawnZone)
                {
                    BoundingBoxModel[x] = loadSpawnZone(bsp.Spawns.Spawn[x]);
                    continue;
                }
                #endregion

                SpawnInfo.RotationSpawn tempspawn = bsp.Spawns.Spawn[x] as SpawnInfo.RotationSpawn;

                #region ScanForExistingModels
                bool found = false;
                for (int xx = 0; xx < x; xx++)
                {
                    SpawnInfo.RotationSpawn tempspawn2 = bsp.Spawns.Spawn[xx] as SpawnInfo.RotationSpawn;
                    if (bsp.Spawns.Spawn[xx] is SpawnInfo.BoundingBoxSpawn)
                    {
                        continue;
                    }

                    if (tempspawn.ModelTagNumber == tempspawn2.ModelTagNumber)
                    {
                        BoundingBoxModel[x] = BoundingBoxModel[xx];
                        spawnmodelindex[x] = spawnmodelindex[xx];
                        bsp.Spawns.Spawn[x].bbXDiff = bsp.Spawns.Spawn[xx].bbXDiff;
                        bsp.Spawns.Spawn[x].bbYDiff = bsp.Spawns.Spawn[xx].bbYDiff;
                        bsp.Spawns.Spawn[x].bbZDiff = bsp.Spawns.Spawn[xx].bbZDiff;
                        found = true;
                        break;
                    }
                }

                if (found)
                {
                    continue;
                }
                #endregion

                #region ReadSpawnMeta

                Meta m = new Meta(map);
                if (tempspawn.ModelTagNumber == -1)
                {
                    MessageBox.Show("Test");
                }

                m.ReadMetaFromMap(tempspawn.ModelTagNumber, false);

                #endregion

                #region DirectXModel

                ParsedModel pm = new ParsedModel(ref m);

                // pm.PermutationString=pm.hlmt.Permutations[pm.hlmt.FindPermutationByBaseClass,
                ParsedModel.DisplayedInfo.LoadDirectXTexturesAndBuffers(ref render.device, ref pm);
                SpawnModel.Add(pm);
                spawnmodelindex[x] = SpawnModel.Count - 1;
                m.Dispose();

                #endregion

                #region BoundingBox

                float boxwidth = pm.BoundingBox.MaxX - pm.BoundingBox.MinX;
                float boxheight = pm.BoundingBox.MaxY - pm.BoundingBox.MinY;
                float boxdepth = pm.BoundingBox.MaxZ - pm.BoundingBox.MinZ;
                try
                {
                    BoundingBoxModel[x] = Mesh.Box(render.device, boxwidth, boxheight, boxdepth);
                }
                catch (Exception ex)
                {
                    Global.ShowErrorMsg("Failure to create Bounding Box Mesh for " + pm.name +
                        "\nWidth : " + boxwidth.ToString() +
                        "\nHeight: " + boxheight.ToString() +
                        "\nLength: " + boxdepth.ToString(),
                        ex);
                }
                // Used for fixing position of bounding boxes
                bsp.Spawns.Spawn[x].bbXDiff = pm.BoundingBox.MaxX + pm.BoundingBox.MinX;
                bsp.Spawns.Spawn[x].bbYDiff = pm.BoundingBox.MaxY + pm.BoundingBox.MinY;
                bsp.Spawns.Spawn[x].bbZDiff = pm.BoundingBox.MaxZ + pm.BoundingBox.MinZ;

                #endregion
            }

            if (render.device.DeviceCaps.RasterCaps.SupportsFogTable && bsp.sky.fogenabled &&
                bsp.sky.fog.FogThickness != 0)
            {
                int a = (int)(bsp.sky.fog.A * 255);
                int r = (int)(bsp.sky.fog.R * 255);
                int g = (int)(bsp.sky.fog.G * 255);
                int b = (int)(bsp.sky.fog.B * 255);
                render.device.RenderState.FogColor = Color.FromArgb(a, r, g, b);
                render.device.RenderState.FogStart = bsp.sky.fog.Start;
                render.device.RenderState.FogEnd = bsp.sky.fog.End;
                render.device.RenderState.FogDensity = bsp.sky.fog.FogThickness; // bsp.sky.fog.FogThickness;

                render.device.RenderState.FogTableMode = FogMode.Linear;
                render.device.RenderState.FogEnable = true;
            }

            // render.device.RenderState.FogVertexMode = FogMode.Linear;

            /*
            this.label3.Text = ".:Loading Weapons Collection:.";
            this.ResumeLayout();
            this.SuspendLayout();
            #region LoadAllWeaponsForCollectionChangeBox
            WeaponsList.Clear();
            // Lists all weapons
            for (int i = 0; i < map.MetaInfo.TagType.Length; i++)
                if ((map.MetaInfo.TagType[i] == "itmc") ||
                    (map.MetaInfo.TagType[i] == "vehc"))
                {
                    CollectionInfo Weapon = new CollectionInfo();
                    Meta m = new Meta(map);
                    m.ReadMetaFromMap(i, map, false);

                    Weapon.ItmcTagNumber = i;
                    // Base address of ITMC tag, offset of WEAP pointer (+20)
                    map.BR.BaseStream.Position = map.MetaInfo.Offset[Weapon.ItmcTagNumber] + 20;
                    Weapon.WeapTagNumber = map.Functions.Meta.FindMetaByID(map.BR.ReadInt32(), map);
                    if (Weapon.WeapTagNumber == -1) { continue; }

                    // Base address of WEAP tag, offset of HLMT pointer (+56)
                    map.BR.BaseStream.Position = map.MetaInfo.Offset[Weapon.WeapTagNumber] + 56;
                    Weapon.HlmtTagNumber = map.Functions.Meta.FindMetaByID(map.BR.ReadInt32(), map);
                    if (Weapon.HlmtTagNumber != -1)
                    {
                        // Base address of HLMT tag, offset of MODE pointer (+4)
                        map.BR.BaseStream.Position = map.MetaInfo.Offset[Weapon.HlmtTagNumber] + 4;
                        Weapon.ModelTagNumber = map.Functions.Meta.FindMetaByID(map.BR.ReadInt32(), map);
                        m.ReadMetaFromMap(Weapon.ModelTagNumber, map, false);
                        Weapon.Model = new ParsedModel(ref m, map);
                        Raw.ParsedModel.DisplayedInfo.LoadDirectXTexturesAndBuffers(ref render.device, ref Weapon.Model);

                        // Store names into Weapon
                        Weapon.TagPath = map.FileNames.Name[i];
                        Weapon.TagType = map.MetaInfo.TagType[i];
                        int xx = map.Functions.Meta.FindByNameAndTagType(Weapon.TagType, Weapon.TagPath, map);
                        string[] NameSplit = map.FileNames.Name[xx].Split('\\');
                        Weapon.Name = NameSplit[NameSplit.Length - 1];
                        Weapon.Name = Weapon.Name.Replace('_', ' ');
                        WeaponsList.Add(Weapon);
                    }
                }
            #endregion
            #region LoadAllObjectsForObstacleAndSceneryChangeBox
            SceneryList.Clear();
            ObstacleList.Clear();
            // Lists all Scenery & Obstacles
            for (int i = 0; i < map.MapHeader.fileCount; i++)
            {
                if ((map.MetaInfo.TagType[i] == "scnr"))
                {
                    Meta m = new Meta(map);
                    //m.ReadMetaFromMap(i, map, false);

                    // Base address of SCNR tag, offset of Scenery Palette pointer (+88)
                    map.BR.BaseStream.Position = map.MetaInfo.Offset[i] + 88;
                    int chunkCount = map.BR.ReadInt32();
                    int chunkOffset = map.BR.ReadInt32() - map.SecondaryMagic;

                    #region Scenery Palette Objects
                    // Scenery Palette Objects
                    for (int a = 0; a < chunkCount; a++)
                    {
                        SceneryInfo Scenery = new SceneryInfo();

                        // The Palette Chunk #
                        Scenery.ScenPalNumber = a;

                        // Each chunk is 40 bytes apart
                        map.BR.BaseStream.Position = chunkOffset + a * 40;
                        char[] tagName = map.BR.ReadChars(4);
                        Scenery.ScenTagNumber = map.Functions.Meta.FindMetaByID(map.BR.ReadInt32(), map);

                        try
                        {
                            // Retrieve the Model HLMT tag from the Scenery tag (+56)
                            map.BR.BaseStream.Position = map.MetaInfo.Offset[Scenery.ScenTagNumber] + 56;
                            Scenery.HlmtTagNumber = map.Functions.Meta.FindMetaByID(map.BR.ReadInt32(), map);

                            // Base address of HLMT tag, offset of MODE pointer (+4)
                            map.BR.BaseStream.Position = map.MetaInfo.Offset[Scenery.HlmtTagNumber] + 4;
                            Scenery.ModelTagNumber = map.Functions.Meta.FindMetaByID(map.BR.ReadInt32(), map);

                            if (Scenery.ModelTagNumber != -1)
                            {
                                m.ReadMetaFromMap(Scenery.ModelTagNumber, map, false);
                                Scenery.Model = new ParsedModel(ref m, map);
                            }
                            else
                                Scenery.Model = null;
                            Raw.ParsedModel.DisplayedInfo.LoadDirectXTexturesAndBuffers(ref render.device, ref Scenery.Model);

                            string[] s = map.FileNames.Name[Scenery.ScenTagNumber].Split('\\');
                            Scenery.Name = s[s.Length - 1];
                            Scenery.TagPath = map.FileNames.Name[Scenery.ScenTagNumber];
                            Scenery.TagType = map.MetaInfo.TagType[Scenery.ScenTagNumber];
                            SceneryList.Add(Scenery);
                        }
                        catch { }
                    }
                    #endregion

                    // Base address of SCNR tag, offset of Sound Scenery Palette pointer (+224)
                    map.BR.BaseStream.Position = map.MetaInfo.Offset[i] + 224;
                    chunkCount = map.BR.ReadInt32();
                    chunkOffset = map.BR.ReadInt32() - map.SecondaryMagic;

                    #region Sound Scenery Palette Objects
                    // Scenery Palette Objects
                    for (int a = 0; a < chunkCount; a++)
                    {
                        SceneryInfo Sound = new SceneryInfo();

                        // The Palette Chunk #
                        Sound.ScenPalNumber = a;

                        // Each chunk is 40 bytes apart
                        map.BR.BaseStream.Position = chunkOffset + a * 40;
                        char[] tagName = map.BR.ReadChars(4);
                        Sound.ScenTagNumber = map.Functions.Meta.FindMetaByID(map.BR.ReadInt32(), map);

                        if (Sound.ScenTagNumber != -1)
                        {
                            string[] s = map.FileNames.Name[Sound.ScenTagNumber].Split('\\');
                            Sound.Name = s[s.Length - 1];
                            Sound.TagPath = map.FileNames.Name[Sound.ScenTagNumber];
                            Sound.TagType = map.MetaInfo.TagType[Sound.ScenTagNumber];
                            SoundsList.Add(Sound);
                        }
                    }
                    #endregion

                    // Base address of SCNR tag, offset of Crate Palette pointer (+816)
                    map.BR.BaseStream.Position = map.MetaInfo.Offset[i] + 816;
                    chunkCount = map.BR.ReadInt32();
                    chunkOffset = map.BR.ReadInt32() - map.SecondaryMagic;

                    #region Crate Palette Objects
                    // Crate (Obstacle) Palette Objects
                    for (int a = 0; a < chunkCount; a++)
                    {
                        SceneryInfo Obstacle = new SceneryInfo();

                        // The Palette Chunk #
                        Obstacle.ScenPalNumber = a;

                        // Each chunk is 40 bytes apart
                        map.BR.BaseStream.Position = chunkOffset + a * 40;
                        char[] tagName = map.BR.ReadChars(4);
                        Obstacle.ScenTagNumber = map.Functions.Meta.FindMetaByID(map.BR.ReadInt32(), map);
                        if (Obstacle.ScenTagNumber != -1)
                        {
                            // Retrieve the Model HLMT tag from the Scenery tag (+56)
                            map.BR.BaseStream.Position = map.MetaInfo.Offset[Obstacle.ScenTagNumber] + 56;
                            Obstacle.HlmtTagNumber = map.Functions.Meta.FindMetaByID(map.BR.ReadInt32(), map);

                            // Base address of HLMT tag, offset of MODE pointer (+4)
                            map.BR.BaseStream.Position = map.MetaInfo.Offset[Obstacle.HlmtTagNumber] + 4;
                            Obstacle.ModelTagNumber = map.Functions.Meta.FindMetaByID(map.BR.ReadInt32(), map);

                            m.ReadMetaFromMap(Obstacle.ModelTagNumber, map, false);
                            Obstacle.Model = new ParsedModel(ref m, map);
                            Raw.ParsedModel.DisplayedInfo.LoadDirectXTexturesAndBuffers(ref render.device, ref Obstacle.Model);

                            string[] s = map.FileNames.Name[Obstacle.ScenTagNumber].Split('\\');
                            Obstacle.Name = s[s.Length - 1];
                            Obstacle.TagPath = map.FileNames.Name[Obstacle.ScenTagNumber];
                            Obstacle.TagType = map.MetaInfo.TagType[Obstacle.ScenTagNumber];
                            ObstacleList.Add(Obstacle);
                        }
                    }
                    #endregion
                    break;
                }
            }
            #endregion
            */
            map.CloseMap();
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// <remarks></remarks>
        public void Main()
        {
            using (BSPViewer frm = this)
            {
                if (!frm.InitializeGraphics())
                {
                    // Initialize Direct3D
                    MessageBox.Show("Could not initialize Direct3D.  This tutorial will exit.");
                    return;
                }

                frm.Show();
                frm.Focus();
                cam = new Camera2(this); // Camera(device);

                // Position the camera at the center of the map
                setCameraPosition(
                        (bsp.maxBoundries.X - bsp.minBoundries.X) / 2 + bsp.minBoundries.X,
                        (bsp.maxBoundries.Y - bsp.minBoundries.Y) / 2 + bsp.minBoundries.Y,
                        (bsp.maxBoundries.Z - bsp.minBoundries.Z) / 2 + bsp.minBoundries.Z,
                        false);
                //cam.AimCamera(new Vector3(cam.x, cam.y - 15f, cam.z + 0.25f));
                //cam.ComputePosition();


                aspect = this.Width / (float)this.Height;
                this.speedBar_Update();

                // While the form is still valid, render and process messages
                while (frm.Created)
                {
                    if (NoCulling.Checked)
                    {
                        render.device.RenderState.CullMode = Cull.None;
                    }
                    else
                    {
                        render.device.RenderState.CullMode = Cull.Clockwise;
                    }

                    frm.Render();
                    Application.DoEvents();
                    GC.Collect(0);
                    GC.WaitForPendingFinalizers();
                    GC.Collect(0);

                    // System.Threading.Thread.Sleep(100);
                }
            }
        }

        /// <summary>
        /// The make matrix for spawn.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public Matrix MakeMatrixForSpawn(int x)
        {
            Matrix m = Matrix.Identity;

            

            if (bsp.Spawns.Spawn[x] is SpawnInfo.RotateDirectionBaseSpawn)
            {
                SpawnInfo.RotateDirectionBaseSpawn tempspawn;
                tempspawn = bsp.Spawns.Spawn[x] as SpawnInfo.RotateDirectionBaseSpawn;
                Matrix rotate = Matrix.Identity;
                rotate.RotateYawPitchRoll(0, 0, tempspawn.RotationDirection);

                // rotate.RotateYawPitchRoll(tempspawn.Yaw, tempspawn.Pitch, tempspawn.Roll);
                m.Multiply(rotate);
            }
                
                #region SpawnWithYawPitchRoll
            else if (bsp.Spawns.Spawn[x] is SpawnInfo.RotateYawPitchRollBaseSpawn)
            {
                SpawnInfo.RotateYawPitchRollBaseSpawn tempspawn;
                tempspawn = bsp.Spawns.Spawn[x] as SpawnInfo.RotateYawPitchRollBaseSpawn;

                Matrix rotate = Matrix.Identity;

                if (map.HaloVersion == HaloVersionEnum.Halo2 ||
                    map.HaloVersion == HaloVersionEnum.Halo2Vista)
                {
                    float tempf3 = tempspawn.Roll;

                    Matrix m1 = Matrix.Identity;
                    m1.RotateX(tempspawn.Yaw);
                    Matrix m2 = Matrix.Identity;
                    m2.RotateY(-tempspawn.Pitch); // Pitch is backwards in game
                    Matrix m3 = Matrix.Identity;
                    m3.RotateZ(tempspawn.Roll); // );

                    // Do NOT change the order! Finally this is right //
                    // (m3 * m2 * m1) != (m1 * m2 * m3) with matrix calculations
                    rotate = m3 * m2 * m1;
                }
                else
                {
                    rotate.RotateYawPitchRoll(tempspawn.Yaw, tempspawn.Pitch, tempspawn.Roll);
                }

                m = rotate;
            }

            #endregion

            m.Multiply(Matrix.Translation(bsp.Spawns.Spawn[x].X, bsp.Spawns.Spawn[x].Y, bsp.Spawns.Spawn[x].Z));
            return m;
        }

        /// <summary>
        /// The make matrixes.
        /// </summary>
        /// <remarks></remarks>
        public void MakeMatrixes()
        {
            TranslationMatrix = new Matrix[bsp.Spawns.Spawn.Count];
            for (int x = 0; x < bsp.Spawns.Spawn.Count; x++)
            {
                TranslationMatrix[x] = MakeMatrixForSpawn(x);
            }
        }

        /// <summary>
        /// The move spawns with keyboard.
        /// </summary>
        /// <remarks></remarks>
        public void MoveSpawnsWithKeyboard()
        {
            rotationBitMask = 0;
            try
            {
                cam.device.Acquire();
            }
            catch
            {
                return;
            }

            // Only allow BSP keypresses when side window closed. This will detect it open.
            // Also, if the Tool Strip Text Boxes are selected, will not allow movement.
            string s = string.Empty;
            if (this.ContainsFocus)
            {
                Control.ControlCollection cc = this.Controls;
                bool found = true;
                while (found)
                {
                    found = false;
                    for (int x = 0; x < cc.Count; x++)
                    {
                        if (cc[x].ContainsFocus)
                        {
                            if ((cc[x].Name == string.Empty) && (cc.Owner is ToolStrip))
                            {
                                ToolStrip TS = (ToolStrip)cc.Owner;
                                int j = 0;
                                for (int i = 0; i < TS.Items.Count; i++)
                                {
                                    if (TS.Items[i] is ToolStripControlHost)
                                    {
                                        if (j == x)
                                        {
                                            string[] s2 = TS.Items[i].GetType().ToString().Split('.');
                                            s += s2[s2.Length - 1] + "(" + TS.Items[i].Name + ")";
                                            break;
                                        }

                                        j++;
                                    }
                                }
                            }
                            else
                            {
                                s += cc[x].Name + "\\";
                            }

                            cc = cc[x].Controls;
                            found = true;
                            break;
                        }
                    }
                }
            }

            if ((!s.StartsWith("dockControl")) && (!s.StartsWith("statusStrip\\ToolStripTextBox")) &&
                (!s.StartsWith("statusStrip\\ToolStripComboBox")))
            {
                for (int x = 0; x < SelectedSpawn.Count; x++)
                {
                    if (bsp.Spawns.Spawn[SelectedSpawn[x]].Type == SpawnInfo.SpawnType.Objective)
                    {
                        if (
                            ((SpawnInfo.ObjectiveSpawn)bsp.Spawns.Spawn[SelectedSpawn[x]]).ObjectiveType.ToString().
                                StartsWith("KingOfTheHill_"))
                        {
                            spawns.hillsLoaded = false;
                        }
                    }

                    foreach (Key kk in cam.device.GetPressedKeys())
                    {
                        // tslabel.Text = kk.ToString();
                        switch (kk.ToString())
                        {
                            case "Up":
                                bsp.Spawns.Spawn[SelectedSpawn[x]].X += cam.speed;
                                break;
                            case "DownArrow":
                                bsp.Spawns.Spawn[SelectedSpawn[x]].X -= cam.speed;
                                break;
                            case "LeftArrow":
                                bsp.Spawns.Spawn[SelectedSpawn[x]].Y += cam.speed;
                                break;
                            case "Right":
                                bsp.Spawns.Spawn[SelectedSpawn[x]].Y -= cam.speed;
                                break;
                            case "PageDown":
                                bsp.Spawns.Spawn[SelectedSpawn[x]].Z -= cam.speed;
                                break;
                            case "PageUp":
                                bsp.Spawns.Spawn[SelectedSpawn[x]].Z += cam.speed;
                                break;
                            case "LeftControl":
                                rotationBitMask |= (int)SelectedItemRotationType.Control;
                                break;
                            case "RightControl":
                                rotationBitMask |= (int)SelectedItemRotationType.Control;
                                break;
                            case "LeftShift":
                                rotationBitMask |= (int)SelectedItemRotationType.Shift;
                                break;
                            case "RightShift":
                                rotationBitMask |= (int)SelectedItemRotationType.Shift;
                                break;
                            case "RightMenu":
                                rotationBitMask |= (int)SelectedItemRotationType.Alt;
                                break;
                            case "LeftMenu":
                                rotationBitMask |= (int)SelectedItemRotationType.Alt;
                                break;
                        }
                    }

                    TranslationMatrix[SelectedSpawn[x]] = MakeMatrixForSpawn(SelectedSpawn[x]);
                }

                if (!spawns.hillsLoaded)
                {
                    spawns.createHills();
                }

                if ((SelectedSpawn.Count > 0) && updateXYZYPR)
                {
                    int lastSelectedSpawn = SelectedSpawn[SelectedSpawn.Count - 1];
                    tsTextBoxX.Text = bsp.Spawns.Spawn[lastSelectedSpawn].X.ToString("#0.0000####");
                    tsTextBoxY.Text = bsp.Spawns.Spawn[lastSelectedSpawn].Y.ToString("#0.0000####");
                    tsTextBoxZ.Text = bsp.Spawns.Spawn[lastSelectedSpawn].Z.ToString("#0.0000####");
                    statusStrip.ResumeLayout();
                    statusStrip.SuspendLayout();
                }
            }
        }

        /// <summary>
        /// The render lightmap.
        /// </summary>
        /// <param name="LightmapIndex">The lightmap index.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public Image RenderLightmap(int LightmapIndex)
        {
            MemoryStream Image_MemoryStream = new MemoryStream();
            BinaryWriter Image_BW = new BinaryWriter(Image_MemoryStream);

            // Creat the header and write it
            byte[] MyHeader = {
                                  66, 77, 56, 3, 0, 0, 0, 0, 0, 0, 54, 0, 0, 0, 40, 0, 0, 0, 16, 0, 0, 0, 16, 0, 0, 0, 1, 
                                  0, 24, 0, 0, 0, 0, 0, 2, 3, 0, 0, 18, 11, 0, 0, 18, 11, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
                              };
            Image_BW.Write(MyHeader);

            // Write the RGB
            for (int i = 0; i < 1024; i += 4)
            {
                Image_BW.Write(LightMap_Array[(LightmapIndex * 1024) + i + 0]);
                Image_BW.Write(LightMap_Array[(LightmapIndex * 1024) + i + 1]);
                Image_BW.Write(LightMap_Array[(LightmapIndex * 1024) + i + 2]);
            }

            Image_BW.Write(0);
            Image_BW.Flush();

            Image tempimage;
            tempimage = Image.FromStream(Image_MemoryStream);
            tempimage.RotateFlip(RotateFlipType.Rotate180FlipX);

            return tempimage;
        }

        /// <summary>
        /// The check for intersection.
        /// </summary>
        /// <param name="e">The e.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public int[] checkForIntersection(MouseEventArgs e)
        {
            List<int> temp = new List<int>();

            for (int x = 0; x < bsp.Spawns.Spawn.Count; x++)
            {
                // check bitmask for object visibility
                if (((int)bsp.Spawns.Spawn[x].Type & visibleSpawnsBitMask) == 0)
                {
                    continue;
                }

                int tempcount = SpawnModel[spawnmodelindex[x]].Display.Chunk.Count;
                bool useboundingbox = false;

                

                switch (bsp.Spawns.Spawn[x].Type)
                {
                    case SpawnInfo.SpawnType.Camera:
                    case SpawnInfo.SpawnType.DeathZone:
                    case SpawnInfo.SpawnType.Light:
                    case SpawnInfo.SpawnType.Sound:
                    case SpawnInfo.SpawnType.SpawnZone:
                        tempcount = 1;
                        useboundingbox = true;
                        break;
                }

                

                for (int yy = 0; yy < tempcount; yy++)
                {
                    // check for mesh intersection
                    Mesh tempm;
                    if (!useboundingbox)
                    {
                        tempm = SpawnModel[spawnmodelindex[x]].Display.meshes[yy];
                    }
                    else
                    {
                        tempm = BoundingBoxModel[x];
                    }

                    // Check under mouse cursor for object selection/deselection?
                    if (render.MeshPick(e.X, e.Y, tempm, TranslationMatrix[x]))
                    {
                        temp.Add(x);
                        break;
                    }
                }
            }

            return temp.ToArray();
        }

        /// <summary>
        /// The set camera position.
        /// </summary>
        /// <param name="X">The x.</param>
        /// <param name="Y">The y.</param>
        /// <param name="Z">The z.</param>
        /// <param name="exactLocation">The exact location.</param>
        /// <remarks></remarks>
        public void setCameraPosition(float X, float Y, float Z, bool exactLocation)
        {
            cam.Position.X = X;
            cam.Position.Y = Y;
            cam.Position.Z = Z;

            cam.radianv = 0;
            cam.radianh = 0;

            cam.x = X;
            cam.y = Y;
            cam.z = Z;
            if (!exactLocation)
            {
                // X-0.8 & Z+1.4 allows us to see the object from a decent angle
                cam.Position.X += -0.8F;
                cam.Position.Y += 0;
                cam.Position.Z += 1.40F;
                cam.radianv = 5.32F; // Radians or 305 degrees
                cam.x += -0.8F;
                cam.y += 0;
                cam.z += 1.40F;
            }

            // Make sure we don't get that dumb jump when we go to move after Dbl Clicking
            cam.oldx = MousePosition.X;
            cam.oldy = MousePosition.Y;
            cam.change(cam.oldx, cam.oldy);
        }

        /// <summary>
        /// The speed bar_ update.
        /// </summary>
        /// <remarks></remarks>
        public void speedBar_Update()
        {
            if (cam.speed < 1)
            {
                this.speedBar.Value = (int)(cam.speed * 100);
            }
            else
            {
                this.speedBar.Value = (int)(cam.speed * 10) + 90;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The on key press.
        /// </summary>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            if ((byte)e.KeyChar == (int)Keys.Escape)
            {
                this.Dispose(); // Esc was pressed
            }

            switch ((int)e.KeyChar)
            {
                case (int)Keys.Up:
                case (int)Keys.Down:
                case (int)Keys.Left:
                case (int)Keys.Right:
                    this.Dispose();
                    break;
            }

            // Theater mode shortcuts
            if (theaterMode)
            {
                char key = char.ToLower(e.KeyChar);
                switch (key)
                {
                    case ' ': // Spacebar - play/pause
                        TogglePathPlayback();
                        if (pathPlayPauseButton != null)
                            pathPlayPauseButton.Text = pathIsPlaying ? "|| Pause" : "> Play";
                        e.Handled = true;
                        break;

                    case 'b': // Set bookmark
                        SetBookmarkMarker();
                        e.Handled = true;
                        break;

                    case 'l': // Toggle loop
                        if (bookmarkStartTimestamp >= 0 && bookmarkEndTimestamp >= 0)
                        {
                            bookmarkLoopEnabled = !bookmarkLoopEnabled;
                            UpdateLoopButton();
                            timelinePanelRef?.Invalidate();
                        }
                        e.Handled = true;
                        break;

                    case 'g': // Go to bookmark start
                        if (bookmarkStartTimestamp >= 0)
                        {
                            JumpToBookmark();
                            timelinePanelRef?.Invalidate();
                        }
                        e.Handled = true;
                        break;

                    case ',': // < key - previous tick
                    case '<':
                        SkipTicks(-1);
                        e.Handled = true;
                        break;

                    case '.': // > key - next tick
                    case '>':
                        SkipTicks(1);
                        e.Handled = true;
                        break;
                }
            }
        }

        /// <summary>
        /// Process command keys for arrow key handling in theater mode.
        /// </summary>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (theaterMode)
            {
                switch (keyData)
                {
                    case Keys.Space:
                        TogglePathPlayback();
                        if (pathPlayPauseButton != null)
                            pathPlayPauseButton.Text = pathIsPlaying ? "|| Pause" : "> Play";
                        return true;

                    case Keys.Left:
                        SkipSeconds(-5);
                        return true;

                    case Keys.Right:
                        SkipSeconds(5);
                        return true;

                    case Keys.Oemcomma: // < key
                        SkipTicks(-1);
                        return true;

                    case Keys.OemPeriod: // > key
                        SkipTicks(1);
                        return true;

                    case Keys.Tab: // Toggle scoreboard
                        showScoreboard = !showScoreboard;
                        return true;

                    case Keys.K: // Toggle killfeed
                        showKillfeed = !showKillfeed;
                        return true;
                }
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        /// <summary>
        /// Skips playback by specified number of seconds.
        /// </summary>
        private void SkipSeconds(float seconds)
        {
            if (playerPath.Count == 0) return;

            pathCurrentTimestamp = Math.Max(pathMinTimestamp,
                Math.Min(pathMaxTimestamp, pathCurrentTimestamp + seconds));
            pathTimeAccumulator = pathCurrentTimestamp;

            // Find the index for the new timestamp
            for (int i = 0; i < playerPath.Count - 1; i++)
            {
                if (playerPath[i].Timestamp <= pathCurrentTimestamp &&
                    playerPath[i + 1].Timestamp > pathCurrentTimestamp)
                {
                    pathCurrentIndex = i;
                    break;
                }
            }

            UpdateTimelineLabel();
            UpdateTrackBarFromTimestamp();
            timelinePanelRef?.Invalidate();
        }

        /// <summary>
        /// Skips playback by specified number of data ticks.
        /// </summary>
        private void SkipTicks(int ticks)
        {
            if (playerPath.Count == 0) return;

            // Find current index based on timestamp
            int currentIdx = 0;
            for (int i = 0; i < playerPath.Count - 1; i++)
            {
                if (playerPath[i].Timestamp <= pathCurrentTimestamp)
                    currentIdx = i;
                else
                    break;
            }

            currentIdx = Math.Max(0, Math.Min(playerPath.Count - 1, currentIdx + ticks));
            pathCurrentIndex = currentIdx;
            pathCurrentTimestamp = playerPath[currentIdx].Timestamp;
            pathTimeAccumulator = pathCurrentTimestamp;

            UpdateTimelineLabel();
            UpdateTrackBarFromTimestamp();
            timelinePanelRef?.Invalidate();
        }

        /// <summary>
        /// Sets bookmark markers (A-B loop). First press sets start, second sets end.
        /// </summary>
        private void SetBookmarkMarker()
        {
            if (bookmarkStartTimestamp < 0)
            {
                // Set start marker
                bookmarkStartTimestamp = pathCurrentTimestamp;
                bookmarkEndTimestamp = -1;
                bookmarkLoopEnabled = false;
            }
            else if (bookmarkEndTimestamp < 0)
            {
                // Set end marker
                bookmarkEndTimestamp = pathCurrentTimestamp;
                // Ensure start < end
                if (bookmarkEndTimestamp < bookmarkStartTimestamp)
                {
                    float temp = bookmarkStartTimestamp;
                    bookmarkStartTimestamp = bookmarkEndTimestamp;
                    bookmarkEndTimestamp = temp;
                }
            }
            else
            {
                // Clear both markers
                bookmarkStartTimestamp = -1;
                bookmarkEndTimestamp = -1;
                bookmarkLoopEnabled = false;
            }
            UpdateBookmarkButton();
            UpdateLoopButton();
            timelinePanelRef?.Invalidate();
        }

        /// <summary>
        /// Updates trackbar position from current timestamp.
        /// </summary>
        private void UpdateTrackBarFromTimestamp()
        {
            if (pathTimelineTrackBar != null && pathMaxTimestamp > pathMinTimestamp)
            {
                float ratio = (pathCurrentTimestamp - pathMinTimestamp) / (pathMaxTimestamp - pathMinTimestamp);
                pathTimelineTrackBar.Value = (int)(ratio * pathTimelineTrackBar.Maximum);
            }
        }

        /// <summary>
        /// The on paint.
        /// </summary>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        protected override void OnPaint(PaintEventArgs e)
        {
            this.Render(); // Render on painting
        }

        /// <summary>
        /// The on resize.
        /// </summary>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        protected override void OnResize(EventArgs e)
        {
            // this.Render();//
            render.pause = (this.WindowState == FormWindowState.Minimized) || !this.Visible;
        }

        /// <summary>
        /// The wnd proc.
        /// </summary>
        /// <param name="m">The m.</param>
        /// <remarks></remarks>
        protected override void WndProc(ref Message m)
        {
            const int WM_SYSCOMMAND = 0x0112;
            const int SC_MAXIMIZE = 0xF030;
            const int SC_DBLTITLECLICK = 0xF032;
            const int SC_RESTORE = 0xF120;
            const int WM_SIZING = 0x214;
            const int WM_EXITSIZEMOVE = 0x232;
            const int WM_MOVE = 0x0003;

            /*
            if (m.Msg == WM_SIZING)
            {
                inSizing = true;
            }
            */
            if (m.Msg == WM_EXITSIZEMOVE && inSizing)
            {
                // WM_EXITSIZEMOVE
                // OnFormResizeEnd();
                inSizing = true;
            }

            if (m.Msg == WM_SIZING)
            {
                // OnFormResizeEnd();
                OnFormResizeEnd();
            }

            if (m.Msg == WM_SYSCOMMAND)
            {
                if (((int)m.WParam == SC_DBLTITLECLICK) || ((int)m.WParam == SC_MAXIMIZE) ||
                    ((((int)m.WParam) & 0xFFF0) == SC_RESTORE))
                {
                    // OnFormResizeEnd();
                    inSizing = true;
                }
            }

            base.WndProc(ref m);

            // This must come after WndProc. Do not move above!
            if (m.Msg == WM_MOVE && inSizing)
            {
                OnFormResizeEnd();
                inSizing = false;
            }
        }

        /// <summary>
        /// The bsp lighting_ checked changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void BSPLighting_CheckedChanged(object sender, EventArgs e)
        {
            bsp.RenderBSPLighting = ((CheckBox)sender).Checked;
        }

        /// <summary>
        /// The bsp permutations_ checked changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void BSPPermutations_CheckedChanged(object sender, EventArgs e)
        {
            bsp.DrawBSPPermutations = ((CheckBox)sender).Checked;
        }

        private void cbBSPTextures_CheckedChanged(object sender, EventArgs e)
        {
            ;
        }

        /// <summary>
        /// Selects/Deselects Camera Culling option
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CameraCulling_CheckedChanged(object sender, EventArgs e)
        {
            bsp.cameraCulling = ((CheckBox)sender).Checked;
        }

        /// <summary>
        /// The bsp viewer_ mouse down.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void BSPViewer_MouseDown(object sender, MouseEventArgs e)
        {
            

            if (e.Button == MouseButtons.Middle)
            {
                selectionHeight = 0;
                selectionWidth = 0;
                selectionDepth = 0;
                selectionMulti = true;
                selectionStart = render.Mark3DCursorPosition(e.X, e.Y, Matrix.Identity);
            }
                
                #region StartCameraRotation (Mouse Right Button)
            else if (e.Button == MouseButtons.Right)
            {
                cam.oldx = e.X;
                cam.oldy = e.Y;
                Time = DateTime.Now.TimeOfDay;
                this.ContextMenuStrip = identContext;
            }

                #endregion
                #region SpawnSelection (Mouse Left Button)
            else if (e.Button == MouseButtons.Left)
            {
                #region DecideUponObjectRotation

                if ((SelectedSpawn.Count > 0) && (rotationBitMask != 0))
                {
                    selectionStart = render.Mark3DCursorPosition(e.X, e.Y, Matrix.Identity);
                    oldx = e.X;
                    oldy = e.Y;
                    itemrotate = true;

                    // return;
                }

                    #endregion
                else
                {
                    #region CheckSpawnsForIntersection

                    for (int x = 0; x < bsp.Spawns.Spawn.Count; x++)
                    {
                        // check bitmask for object visibility
                        if (((int)bsp.Spawns.Spawn[x].Type & visibleSpawnsBitMask) == 0)
                        {
                            continue;
                        }

                        int tempcount = SpawnModel[spawnmodelindex[x]].Display.Chunk.Count;
                        bool useboundingbox = false;

                        #region Make Cameras, DeathZones, Sounds Spawmn Zones And Lights Use BoundingBoxes

                        switch (bsp.Spawns.Spawn[x].Type)
                        {
                            case SpawnInfo.SpawnType.Camera:
                            case SpawnInfo.SpawnType.DeathZone:
                            case SpawnInfo.SpawnType.Light:
                            case SpawnInfo.SpawnType.Sound:
                            case SpawnInfo.SpawnType.SpawnZone:
                                tempcount = 1;
                                useboundingbox = true;
                                break;
                        }

                        #endregion

                        for (int yy = 0; yy < tempcount; yy++)
                        {
                            // check for mesh intersection
                            Mesh tempm;
                            if (!useboundingbox)
                            {
                                tempm = SpawnModel[spawnmodelindex[x]].Display.meshes[yy];
                            }
                            else
                            {
                                tempm = BoundingBoxModel[x];
                            }

                            // Check under mouse cursor for object selection/deselection?
                            if (render.MeshPick(e.X, e.Y, tempm, TranslationMatrix[x]))
                            {
                                if (bsp.Spawns.Spawn[x].frozen)
                                {
                                    break;
                                }

                                #region TurnSpawnOnOrOff

                                int tempi = SelectedSpawn.IndexOf(x);
                                if (tempi != -1)
                                {
                                    SelectedSpawn.RemoveAt(tempi);
                                    if (DeselectOne.Checked)
                                    {
                                        updateStatusPosition();
                                        return;
                                    }
                                }
                                else
                                {
                                    SelectedSpawn.Add(x);
                                    selectedSpawnType = bsp.Spawns.Spawn[x].Type;
                                }

                                #endregion

                                break;
                            }
                        }
                    }
                }

                #endregion CycleThroughSpawns
            }

            #region statusBarUpdates

            updateStatusPosition();

            #endregion statusBarUpdates

            #endregion
        }

        /// <summary>
        /// The bsp viewer_ mouse up.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void BSPViewer_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
            {
                return;
            }

            Time = DateTime.Now.TimeOfDay.Subtract(Time);

            if ((Time.Ticks / 1000000) < 3)
            {
                

                int[] spawns = checkForIntersection(e);
                for (int x = 0; x < spawns.Length; x++)
                {
                    currentObject = x;
                    int tempi = SelectedSpawn.IndexOf(spawns[x]);
                    if (tempi != -1)
                    {
                        SelectedSpawn.RemoveAt(tempi);
                        updateStatusPosition();
                    }

                    // bsp.Spawns.Spawn[spawns[x]].frozen = !bsp.Spawns.Spawn[spawns[x]].frozen;
                    break;
                }
            }
        }

        /// <summary>
        /// BSPSelection
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e param</param>
        private void checkedListBox2_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (checkedListBox2.Enabled == false) { return; }

            bsp.BSPRawDataMetaChunks[e.Index].render = (e.CurrentValue == CheckState.Unchecked);
        }

        /// <summary>
        /// The color balance.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="redShift">The red shift.</param>
        /// <param name="greenShift">The green shift.</param>
        /// <param name="blueShift">The blue shift.</param>
        /// <param name="preserve">The preserve.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        private byte[][] ColorBalance(byte[][] source, int redShift, int greenShift, int blueShift, bool preserve)
        {
            byte[][] adjusted = new byte[source.Length][];
            double shiftR = redShift / 100d;
            double shiftG = greenShift / 100d;
            double shiftB = blueShift / 100d;
            if (preserve)
            {
                // Avoid Complete Desaturation
                shiftR *= 0.99d;
                shiftG *= 0.99d;
                shiftB *= 0.99d;
            }

            for (int i = 0; i < source.Length; i++)
            {
                adjusted[i] = new byte[1024];
                for (int x = 0; x < 1024; x += 4)
                {
                    double B = source[i][x] / 255d;
                    double G = source[i][x + 1] / 255d;
                    double R = source[i][x + 2] / 255d;
                    if (preserve)
                    {
                        // RGB -> L
                        double max = Math.Max(Math.Max(R, G), B);
                        double min = Math.Min(Math.Min(R, G), B);
                        double L = 0.5 * (max + min);

                        // double S = L == 0 || L == 1 ? 0 : // Filter undefined
                        // 	L <= 0.5 ? (max-min)/(max+min) : (max-min)/(2-(max+min));

                        // RGB + Shift -> HS
                        B = shiftB <= 0 ? B * (1 + shiftB) : B + shiftB * (1 - B);
                        G = shiftG <= 0 ? G * (1 + shiftG) : G + shiftG * (1 - G);
                        R = shiftR <= 0 ? R * (1 + shiftR) : R + shiftR * (1 - R);
                        max = Math.Max(Math.Max(R, G), B);
                        min = Math.Min(Math.Min(R, G), B);
                        double tempL = 0.5d * (max + min);
                        double S = tempL == 0 || tempL == 1
                                       ? 0
                                       : // Filter undefined
                                   tempL <= 0.5 ? (max - min) / (max + min) : (max - min) / (2 - (max + min));
                        double H = S == 0
                                       ? 0
                                       : // saturation == 0
                                   R >= G && R >= B
                                       ? 60 * (G - B) / (max - min)
                                       : // max == red
                                   G >= R && G >= B
                                       ? 60 * (B - R) / (max - min) + 120
                                       : // max == green
                                   60 * (R - G) / (max - min) + 240; // max == blue

                        // HSL -> RGB
                        double d = L <= 0.5 ? S * L : S * (1 - L);
                        if (L == 0 || L == 1 || S == 0 || d == 0)
                        {
                            R = G = B = L;
                        }
                        else
                        {
                            while (H >= 360)
                            {
                                H -= 360;
                            }

                            while (H < 0)
                            {
                                H += 360;
                            }

                            // double d = L <= 0.5 ? S*L : S*(1-L);
                            min = L - d;
                            max = L + d;
                            double F = H < 60 ? H : H < 180 ? H - 120 : H < 300 ? H - 240 : H - 360;
                            double mid = min + Math.Abs(F / 60) * (max - min);
                            switch ((int)Math.Floor(H / 60))
                            {
                                case 0:
                                    R = max;
                                    G = mid;
                                    B = min;
                                    break;
                                case 1:
                                    R = mid;
                                    G = max;
                                    B = min;
                                    break;
                                case 2:
                                    R = min;
                                    G = max;
                                    B = mid;
                                    break;
                                case 3:
                                    R = min;
                                    G = mid;
                                    B = max;
                                    break;
                                case 4:
                                    R = mid;
                                    G = min;
                                    B = max;
                                    break;
                                case 5:
                                    R = max;
                                    G = min;
                                    B = mid;
                                    break;
                            }
                        }
                    }
                    else
                    {
                        B = shiftB <= 0 ? B * (1 + shiftB) : B + shiftB * (1 - B);
                        G = shiftG <= 0 ? G * (1 + shiftG) : G + shiftG * (1 - G);
                        R = shiftR <= 0 ? R * (1 + shiftR) : R + shiftR * (1 - R);
                    }

                    adjusted[i][x] = (byte)(B * 255);
                    adjusted[i][x + 1] = (byte)(G * 255);
                    adjusted[i][x + 2] = (byte)(R * 255);
                    adjusted[i][x + 3] = 255;
                }
            }

            return adjusted;
        }

        /// <summary>
        /// The draw skybox.
        /// </summary>
        /// <param name="pm">The pm.</param>
        /// <remarks></remarks>
        private void DrawSkybox(ParsedModel pm)
        {
            if (pm == null)
            {
                return;
            }

            for (int x = 0; x < pm.Display.Chunk.Count; x++)
            {
                int rawindex = pm.Display.Chunk[x];
                for (int xx = 0; xx < pm.RawDataMetaChunks[rawindex].SubMeshInfo.Length; xx++)
                {
                    // device.Material = meshmaterials[i];
                    int tempshade = pm.RawDataMetaChunks[rawindex].SubMeshInfo[xx].ShaderNumber;

                    //Renderer.SetAlphaBlending(ShaderInfo.AlphaType.AlphaBlend, ref render.device);
                    Renderer.SetAlphaBlending(pm.Shaders.Shader[tempshade].Alpha, ref render.device);

                    switch (pm.Shaders.Shader[tempshade].Alpha)
                    {
                        case ShaderInfo.AlphaType.AlphaBlend:
                            render.device.RenderState.SourceBlend = Blend.BothSourceAlpha;
                            render.device.RenderState.DestinationBlend = Blend.Zero;
                            break;
                        case ShaderInfo.AlphaType.None:
                            render.device.RenderState.SourceBlend = Blend.One;
                            render.device.RenderState.DestinationBlend = Blend.One;
                            break;
                    }

                    render.device.SetTexture(0, pm.Shaders.Shader[tempshade].MainTexture);

                    // Skybox textures
                    render.device.TextureState[0].ColorOperation = TextureOperation.SelectArg1;
                    render.device.TextureState[0].ColorArgument1 = TextureArgument.TextureColor;
                    render.device.TextureState[0].ColorArgument2 = TextureArgument.Current;
                    render.device.TextureState[0].AlphaOperation = TextureOperation.SelectArg1;
                    
                    // render.device.TextureState[0].AlphaOperation = TextureOperation.ModulateAlphaAddColor;
                    render.device.TextureState[0].AlphaArgument1 = TextureArgument.TextureColor;
                    render.device.TextureState[0].AlphaArgument2 = TextureArgument.Current;

                    // Skybox Lighting
                    /*
                    if (bsp.LightMapTexture[x] != null && bsp.BSPRawDataMetaChunks[x].LightMapUVs.Count != 0)
                    {
                        render.device.SetTexture(1, bsp.LightMapTexture[x]);
                        render.device.TextureState[1].ColorOperation = TextureOperation.Disable;
                        render.device.TextureState[1].AlphaOperation = TextureOperation.Disable;
                        render.device.TextureState[1].TextureCoordinateIndex = 2;
                    }
                    */
                    render.device.RenderState.FillMode = FillMode.Solid;

                    // render.device.SetTexture(0, meshtextures[i]);
                    pm.Display.meshes[x].DrawSubset(xx);
                }
            }
        }

        /// <summary>
        /// The edit lightmaps.
        /// </summary>
        /// <remarks></remarks>
        private void EditLightmaps()
        {
            if (radioButton1.Checked)
            {
                if (checkBox1.Checked)
                {
                    byte[][] temparray = new byte[1][];
                    for (int i = 0; i < map.BSP.sbsp[bsp.BspNumber].LightMap_Palettes.Count; i++)
                    {
                        temparray[0] = new byte[1024];
                        Array.ConstrainedCopy(LightMap_Array_Backup, i * 1024, temparray[0], 0, 1024);
                        Array.ConstrainedCopy(
                            HueSaturation(temparray, trackint1, trackint2, trackint3, checkBox2.Checked)[0], 
                            0, 
                            LightMap_Array, 
                            i * 1024, 
                            1024);
                    }

                    foreach (PictureBox picbox in panel1.Controls)
                    {
                        picbox.Image = RenderLightmap((int)((picbox).Tag));
                    }
                }
                else
                {
                    byte[][] temparray = new byte[1][];
                    temparray[0] = new byte[1024];
                    Array.ConstrainedCopy(
                        LightMap_Array_Backup, Convert.ToInt32(comboBox1.Text) * 1024, temparray[0], 0, 1024);
                    Array.ConstrainedCopy(
                        HueSaturation(temparray, trackint1, trackint2, trackint3, checkBox2.Checked)[0], 
                        0, 
                        LightMap_Array, 
                        Convert.ToInt32(comboBox1.Text) * 1024, 
                        1024);

                    foreach (PictureBox picbox in panel1.Controls)
                    {
                        if (((int)(picbox).Tag) == Convert.ToInt32(comboBox1.Text))
                        {
                            picbox.Image = RenderLightmap((int)((picbox).Tag));
                        }
                    }
                }
            }
            else
            {
                if (checkBox1.Checked)
                {
                    byte[][] temparray = new byte[1][];
                    for (int i = 0; i < map.BSP.sbsp[bsp.BspNumber].LightMap_Palettes.Count; i++)
                    {
                        temparray[0] = new byte[1024];
                        Array.ConstrainedCopy(LightMap_Array_Backup, i * 1024, temparray[0], 0, 1024);
                        Array.ConstrainedCopy(
                            ColorBalance(temparray, trackint1, trackint2, trackint3, checkBox2.Checked)[0], 
                            0, 
                            LightMap_Array, 
                            i * 1024, 
                            1024);
                    }

                    foreach (PictureBox picbox in panel1.Controls)
                    {
                        picbox.Image = RenderLightmap((int)((picbox).Tag));
                    }
                }
                else
                {
                    byte[][] temparray = new byte[1][];
                    temparray[0] = new byte[1024];
                    Array.ConstrainedCopy(
                        LightMap_Array_Backup, Convert.ToInt32(comboBox1.Text) * 1024, temparray[0], 0, 1024);
                    Array.ConstrainedCopy(
                        ColorBalance(temparray, trackint1, trackint2, trackint3, checkBox2.Checked)[0], 
                        0, 
                        LightMap_Array, 
                        Convert.ToInt32(comboBox1.Text) * 1024, 
                        1024);

                    foreach (PictureBox picbox in panel1.Controls)
                    {
                        if (((int)(picbox).Tag) == Convert.ToInt32(comboBox1.Text))
                        {
                            picbox.Image = RenderLightmap((int)((picbox).Tag));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The hue saturation.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="hueShift">The hue shift.</param>
        /// <param name="satShift">The sat shift.</param>
        /// <param name="lightShift">The light shift.</param>
        /// <param name="colorize">The colorize.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        private byte[][] HueSaturation(byte[][] source, int hueShift, int satShift, int lightShift, bool colorize)
        {
            byte[][] adjusted = new byte[source.Length][];
            double shiftH = hueShift;
            double shiftS = ((double)satShift) / 100;
            double shiftL = ((double)lightShift) / 100;
            for (int i = 0; i < source.Length; i++)
            {
                adjusted[i] = new byte[1024];
                for (int x = 0; x < 1024; x += 4)
                {
                    // RGB -> HSL
                    double B = ((double)source[i][x]) / 255;
                    double G = ((double)source[i][x + 1]) / 255;
                    double R = ((double)source[i][x + 2]) / 255;
                    double max = Math.Max(Math.Max(R, G), B);
                    double min = Math.Min(Math.Min(R, G), B);
                    double L = 0.5 * (max + min);
                    double S = L == 0 || L == 1
                                   ? 0
                                   : // Filter undefined
                               L <= 0.5 ? (max - min) / (max + min) : (max - min) / (2 - (max + min));
                    double H = colorize
                                   ? 0
                                   : // colorize
                               S == 0
                                   ? 0
                                   : // saturation == 0
                               R >= G && R >= B
                                   ? 60 * (G - B) / (max - min)
                                   : // max == red
                               G >= R && G >= B
                                   ? 60 * (B - R) / (max - min) + 120
                                   : // max == blue
                               60 * (R - G) / (max - min) + 240; // max == green

                    // Shift HSL
                    H = H + shiftH;
                    S = !colorize && S == 0
                            ? 0
                            : // Don't create saturation
                        shiftS <= 0 ? S * (1 + shiftS) : S + shiftS * (1 - S);
                    L = shiftL <= 0 ? L * (1 + shiftL) : L + shiftL * (1 - L);

                    // HSL -> RGB
                    double d = L <= 0.5 ? S * L : S * (1 - L);
                    if (L == 0 || L == 1 || S == 0 || d == 0)
                    {
                        R = G = B = L;
                    }
                    else
                    {
                        while (H >= 360)
                        {
                            H -= 360;
                        }

                        while (H < 0)
                        {
                            H += 360;
                        }

                        // double d = L <= 0.5 ? S*L : S*(1-L);
                        min = L - d;
                        max = L + d;
                        double F = H < 60 ? H : H < 180 ? H - 120 : H < 300 ? H - 240 : H - 360;
                        double mid = min + Math.Abs(F / 60) * (max - min);
                        switch ((int)Math.Floor(H / 60))
                        {
                            case 0:
                                R = max;
                                G = mid;
                                B = min;
                                break;
                            case 1:
                                R = mid;
                                G = max;
                                B = min;
                                break;
                            case 2:
                                R = min;
                                G = max;
                                B = mid;
                                break;
                            case 3:
                                R = min;
                                G = mid;
                                B = max;
                                break;
                            case 4:
                                R = mid;
                                G = min;
                                B = max;
                                break;
                            case 5:
                                R = max;
                                G = min;
                                B = mid;
                                break;
                        }
                    }

                    adjusted[i][x] = (byte)(B * 255);
                    adjusted[i][x + 1] = (byte)(G * 255);
                    adjusted[i][x + 2] = (byte)(R * 255);
                    adjusted[i][x + 3] = 255;
                }
            }

            return adjusted;
        }

        /// <summary>
        /// The load_ lightmaps_ into_ an_ array.
        /// </summary>
        /// <remarks></remarks>
        private void Load_Lightmaps_Into_An_Array()
        {
            LightMap_Array = new byte[map.BSP.sbsp[bsp.BspNumber].LightMap_Palettes.Count * 1024];

            LightMap_Array_Backup = new byte[LightMap_Array.Length];

            for (int i = 0; i < map.BSP.sbsp[bsp.BspNumber].LightMap_Palettes.Count; i++)
            {
                for (int j = 0; j < 256; j++)
                {
                    BSPContainer.Palette_Color temp = map.BSP.sbsp[bsp.BspNumber].LightMap_Palettes[i][j];

                    LightMap_Array[(i * 1024) + (j * 4) + 0] = Convert.ToByte(temp.r);
                    LightMap_Array[(i * 1024) + (j * 4) + 1] = Convert.ToByte(temp.g);
                    LightMap_Array[(i * 1024) + (j * 4) + 2] = Convert.ToByte(temp.b);
                    LightMap_Array[(i * 1024) + (j * 4) + 3] = Convert.ToByte(temp.a);
                }
            }

            Array.Copy(LightMap_Array, LightMap_Array_Backup, LightMap_Array.Length);
        }

        /// <summary>
        /// The model viewer_ mouse down.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void ModelViewer_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.None)
            {
                oldx = e.X;
                oldy = e.Y;
            }

            if (SelectedSpawn.Count > 0)
            {
                int i = SelectedSpawn[SelectedSpawn.Count - 1];
                axis = Gizmo.axis.none;
                if (e.Button == MouseButtons.None && gizmo != null)
                {
                    axis = gizmo.checkForIntersection(e, TranslationMatrix[i]);
                }

                // Only Last selection hilights ATM!
                if ((axis != Gizmo.axis.none) && (e.Button == MouseButtons.Left))
                {
                    float xDiff = (e.X - oldx) / 10.0f;
                    float yDiff = (e.Y - oldy) / 10.0f;
                    switch (axis)
                    {
                        case Gizmo.axis.X:
                            bsp.Spawns.Spawn[i].X -= xDiff / cam.speed;
                            break;
                        case Gizmo.axis.Y:
                            bsp.Spawns.Spawn[i].Y -= yDiff / cam.speed;
                            break;
                        case Gizmo.axis.Z:
                            bsp.Spawns.Spawn[i].Z -= yDiff / cam.speed;
                            break;
                        case Gizmo.axis.XY:
                            bsp.Spawns.Spawn[i].X += yDiff / cam.speed;
                            bsp.Spawns.Spawn[i].Y += xDiff / cam.speed;
                            break;
                        case Gizmo.axis.YZ:
                            bsp.Spawns.Spawn[i].Y += xDiff / cam.speed;
                            bsp.Spawns.Spawn[i].Z -= yDiff / cam.speed;
                            break;
                        case Gizmo.axis.XZ:
                            bsp.Spawns.Spawn[i].X -= xDiff / cam.speed;
                            bsp.Spawns.Spawn[i].Z -= yDiff / cam.speed;
                            break;
                    }

                    oldx = e.X;
                    oldy = e.Y;
                }
            }

            if (selectionMulti)
            {
                Vector3 tempvec = render.Mark3DCursorPosition(e.X, e.Y, Matrix.Identity);
                selectionWidth = tempvec.X - selectionStart.X;
                selectionHeight = tempvec.Y - selectionStart.Y;
                selectionDepth = tempvec.Z - selectionStart.Z;
                selectionWidth *= 2;
                selectionHeight *= 7;
                selectionDepth *= 2;
                float tempselectionWidth = selectionWidth;
                float tempselectionHeight = selectionHeight;
                float tempselectionDepth = selectionDepth;
                if (tempselectionWidth < 0)
                {
                    tempselectionWidth = -tempselectionWidth;
                }

                if (tempselectionHeight < 0)
                {
                    tempselectionHeight = -tempselectionHeight;
                }

                if (tempselectionDepth < 0)
                {
                    tempselectionDepth = -tempselectionDepth;
                }

                /************/
                tsLabel1.Text = selectionStart.X.ToString().PadRight(10) + " � " +
                                selectionStart.Y.ToString().PadRight(10) + selectionStart.Z.ToString().PadRight(10) +
                                " � " + selectionWidth.ToString().PadRight(10) + " � " +
                                selectionHeight.ToString().PadRight(10) + " � " + selectionDepth.ToString().PadRight(10);
                selectionMesh = Mesh.Box(render.device, tempselectionWidth, tempselectionHeight, tempselectionDepth);
            }
                
                #region SelectedSpawnRotation
            else if (rotationBitMask != 0 && itemrotate)
            {
                Vector3 tempvec = render.Mark3DCursorPosition(e.X, e.Y, Matrix.Identity);
                selectionWidth = tempvec.X - selectionStart.X;
                selectionHeight = tempvec.Y - selectionStart.Y;
                selectionDepth = tempvec.Z - selectionStart.Z;
                selectionStart = tempvec;

                foreach (int spawnid in SelectedSpawn)
                {
                    if (bsp.Spawns.Spawn[spawnid] is SpawnInfo.BoundingBoxSpawn)
                    {
                        continue;
                    }

                    #region ShiftAndRotate

                    if ((rotationBitMask & (int)SelectedItemRotationType.Shift) != 0)
                    {
                        if (bsp.Spawns.Spawn[spawnid] is SpawnInfo.RotateYawPitchRollBaseSpawn)
                        {
                            SpawnInfo.RotateYawPitchRollBaseSpawn temp;
                            temp = (SpawnInfo.RotateYawPitchRollBaseSpawn)bsp.Spawns.Spawn[spawnid];
                            temp.Yaw += selectionHeight * cam.speed * 5;
                            if (temp.Yaw > (float)Math.PI)
                            {
                                temp.Yaw = -(float)Math.PI;
                            }
                            else if (temp.Yaw < (float)-Math.PI)
                            {
                                temp.Yaw = (float)Math.PI;
                            }
                        }
                        else
                        {
                            SpawnInfo.RotateDirectionBaseSpawn temp;
                            temp = (SpawnInfo.RotateDirectionBaseSpawn)bsp.Spawns.Spawn[spawnid];
                            temp.RotationDirection += selectionHeight * cam.speed * 5;

                            // if (temp.RotationDirection > (float)Math.PI) { temp.RotationDirection = temp.RotationDirection - ((float)Math.PI * 2); }
                            // else if (temp.RotationDirection < (float)-Math.PI) { temp.RotationDirection = temp.RotationDirection + ((float)Math.PI * 2); }
                        }
                    }

                    #endregion

                    #region ControlAndRotate

                    if ((rotationBitMask & (int)SelectedItemRotationType.Control) != 0)
                    {
                        if (bsp.Spawns.Spawn[spawnid] is SpawnInfo.RotateYawPitchRollBaseSpawn)
                        {
                            SpawnInfo.RotateYawPitchRollBaseSpawn temp;
                            temp = (SpawnInfo.RotateYawPitchRollBaseSpawn)bsp.Spawns.Spawn[spawnid];
                            temp.Pitch += selectionWidth * cam.speed * 5;
                            if (temp.Pitch > (float)Math.PI)
                            {
                                temp.Pitch = -(float)Math.PI;
                            }
                            else if (temp.Pitch < (float)-Math.PI)
                            {
                                temp.Pitch = (float)Math.PI;
                            }
                        }
                    }

                    #endregion

                    #region AltAndRotate

                    if ((rotationBitMask & (int)SelectedItemRotationType.Alt) != 0)
                    {
                        if (bsp.Spawns.Spawn[spawnid] is SpawnInfo.RotateYawPitchRollBaseSpawn)
                        {
                            SpawnInfo.RotateYawPitchRollBaseSpawn temp;
                            temp = (SpawnInfo.RotateYawPitchRollBaseSpawn)bsp.Spawns.Spawn[spawnid];
                            temp.Roll += selectionWidth * cam.speed * 5;
                            if (temp.Roll > (float)Math.PI)
                            {
                                temp.Roll = -(float)Math.PI;
                            }
                            else if (temp.Roll < (float)-Math.PI)
                            {
                                temp.Roll = (float)Math.PI;
                            }
                        }
                    }

                    #endregion

                    TranslationMatrix[spawnid] = MakeMatrixForSpawn(spawnid);
                }

                updateStatusPosition();
            }

            #endregion

            #region CameraRotation

            if (e.Button == MouseButtons.Right)
            {
                cam.change(e.X, e.Y);
                this.ContextMenuStrip = null;
            }

            #endregion
        }

        /// <summary>
        /// The model viewer_ mouse up.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void ModelViewer_MouseUp(object sender, MouseEventArgs e)
        {
            

            if (itemrotate)
            {
                itemrotate = false;
                rotationBitMask = 0;
            }

            

            #region EndMultiSelect

            if (selectionMulti)
            {
                selectionMulti = false;
                selectionMesh = null;
                float halfwidth = selectionWidth / 2;
                float halfheight = selectionHeight / 2;
                float halfdepth = selectionDepth / 2;
                float poshalfwidth = halfwidth;
                float poshalfheight = halfheight;
                float poshalfdepth = halfdepth;

                float tempselectionWidth = selectionWidth;
                float tempselectionHeight = selectionHeight;
                float tempselectionDepth = selectionDepth;

                if (poshalfwidth < 0)
                {
                    poshalfwidth = -poshalfwidth;
                }

                if (poshalfheight < 0)
                {
                    poshalfheight = -poshalfheight;
                }

                if (poshalfdepth < 0)
                {
                    poshalfdepth = -poshalfdepth;
                }

                // float minx = selectionStart.X + halfwidth - poshalfwidth;
                // float maxx = selectionStart.X + poshalfwidth + halfwidth;
                // float miny = selectionStart.Y + halfheight - poshalfheight;
                // float maxy = selectionStart.Y + poshalfheight + halfheight;
                // float minz = selectionStart.Z + halfdepth - poshalfdepth;
                // float maxz = selectionStart.Z + poshalfdepth + halfdepth;
                float minx = selectionStart.X - poshalfwidth;
                float maxx = selectionStart.X + poshalfwidth;
                float miny = selectionStart.Y - poshalfheight;
                float maxy = selectionStart.Y + poshalfheight;
                float minz = selectionStart.Z - poshalfdepth;
                float maxz = selectionStart.Z + poshalfdepth;
                SelectedSpawn.Clear();
                for (int i = 0; i < bsp.Spawns.Spawn.Count; i++)
                {
                    if (bsp.Spawns.Spawn[i].X > minx && bsp.Spawns.Spawn[i].X < maxx && bsp.Spawns.Spawn[i].Y > miny &&
                        bsp.Spawns.Spawn[i].Y < maxy && bsp.Spawns.Spawn[i].Z > minz && bsp.Spawns.Spawn[i].Z < maxz)
                    {
                        SelectedSpawn.Add(i);
                    }
                }
            }

            #endregion
        }

        /// <summary>
        /// The on form resize end.
        /// </summary>
        /// <remarks></remarks>
        private void OnFormResizeEnd()
        {
            speedBar.Left = this.Width - speedBar.Width;
            speedLabel.Left = speedBar.Left;
            speedBar.Refresh();
            if (render.device != null)
            {
                leftSandDock.Height = render.device.Viewport.Height - statusStrip.Height;
                statusStrip.Top = render.device.Viewport.Height - statusStrip.Height;
            }

            statusStrip.Width = this.Width;
            statusStrip.Refresh();
            aspect = this.Width / (float)this.Height;

            // cam is not established when the form is created and Maximized,
            // so it is least time/processor consuming to only check on resizes.
            if (cam != null)
            {
                updateStatusPosition();
            }
        }

        /// <summary>
        /// The picture box_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void PictureBox_Click(object sender, EventArgs e)
        {
            checkBox1.Checked = false;
            comboBox1.SelectedIndex = (int)((PictureBox)sender).Tag;
        }

        /// <summary>
        /// The reload from array.
        /// </summary>
        /// <remarks></remarks>
        private void ReloadFromArray()
        {
            for (int i = 0; i < map.BSP.sbsp[bsp.BspNumber].LightMap_Palettes.Count; i++)
            {
                for (int j = 0; j < 256; j++)
                {
                    BSPContainer.Palette_Color temp = new BSPContainer.Palette_Color();

                    temp.r = LightMap_Array[(i * 1024) + (j * 4) + 0];
                    temp.g = LightMap_Array[(i * 1024) + (j * 4) + 1];
                    temp.b = LightMap_Array[(i * 1024) + (j * 4) + 2];
                    temp.a = LightMap_Array[(i * 1024) + (j * 4) + 3];

                    map.BSP.sbsp[bsp.BspNumber].LightMap_Palettes[i][j] = temp;
                }
            }
        }

        /// <summary>
        /// The render.
        /// </summary>
        /// <remarks></remarks>
        private void Render()
        {
            if (render.pause)
            {
                return;
            }

            if (cam == null)
            {
                return;
            }

            if (RenderSky.Checked)
            {
                render.BeginScene(Color.Black);
            }
            else
            {
                render.BeginScene(Color.Blue);
            }

            SetupMatrices();

            

            if (cam.move())
            {
                this.speedBar_Update();
            }

            MoveSpawnsWithKeyboard();

            

            #region RenderBSP

            render.device.RenderState.Ambient = Color.LightGray;
            if (RenderSky.Checked)
            {
                render.device.Transform.World = Matrix.RotationZ((float)Math.PI / 3) *
                                                Matrix.Translation(cam.x, cam.y, cam.z);

                // render.device.Transform.World = Matrix.Identity;
                // Clamp to get rid of texture seams or mirror or wrap?
                render.device.SamplerState[0].AddressU = TextureAddress.Mirror;
                render.device.SamplerState[0].AddressV = TextureAddress.Mirror;
                render.device.RenderState.Lighting = true;
                render.device.RenderState.ZBufferEnable = false;
                render.device.RenderState.ZBufferWriteEnable = false;

                // Raw.ParsedModel.DisplayedInfo.Draw(ref render.device, bsp.SkyBox);
                DrawSkybox(bsp.SkyBox);
            }

            render.device.Transform.World = Matrix.Identity;
            render.device.SamplerState[0].AddressU = TextureAddress.Wrap;
            render.device.SamplerState[0].AddressV = TextureAddress.Wrap;
            render.device.RenderState.Lighting = true;
            render.device.RenderState.ZBufferEnable = true;
            render.device.RenderState.ZBufferWriteEnable = true;

            
            BSPModel.BSPDisplayedInfo.Draw(ref render.device, ref bsp, cbBSPTextures.Checked, ref cam, shaderx);
            
            render.device.RenderState.Ambient = Color.White;
            // Set camera postion
            string tempstring = toolStripLabel2.Text;
            string tempstring2 = "Camera Position: X: " + cam.x.ToString().PadRight(10) + " � Y: " +
                                 cam.y.ToString().PadRight(10) + " � Z: " + cam.z.ToString().PadRight(10);
            if (tempstring != tempstring2)
            {
                if (statusStrip.Items.IndexOf(toolStripLabel2) == -1)
                {
                    statusStrip.Items.Add(toolStripLabel2);
                }

                toolStripLabel2.Text = tempstring2;
                statusStrip.ResumeLayout();
                statusStrip.SuspendLayout();
            }

            #endregion

            //int lightCount = 0;

            #region RenderSpawns

            for (int x = 0; x < bsp.Spawns.Spawn.Count; x++)
            {
                // Skip any Spawns that are invisible.
                if (((int)bsp.Spawns.Spawn[x].Type & visibleSpawnsBitMask) == 0)
                {
                    continue;
                }

                render.device.Transform.World = TranslationMatrix[x];
                Vector3 tempv = new Vector3();
                tempv.X = bsp.Spawns.Spawn[x].X;
                tempv.Y = bsp.Spawns.Spawn[x].Y;
                tempv.Z = bsp.Spawns.Spawn[x].Z;

                // if (!cam.SphereInFrustum(tempv,10f)){continue;}
                render.device.Material = DefaultMaterial;
                DefaultMaterial.Diffuse = Color.Crimson;
                DefaultMaterial.Ambient = Color.Crimson;

                bool drawModel = true;

                #region DrawLights

                if (bsp.Spawns.Spawn[x] is SpawnInfo.LightSpawn)
                {
                    /**** Lights not showing up.... Hmm...Oh well.
                    SpawnInfo.LightSpawn light = bsp.Spawns.Spawn[x] as Renderer.BSP_Renderer.SpawnInfo.LightSpawn;
                    render.device.Lights[lightCount].Type = LightType.Spot;
                    render.device.Lights[lightCount].Diffuse = Color.FromArgb(light.LightInfo.r, light.LightInfo.g, light.LightInfo.b);
                    render.device.Lights[lightCount].Position = new Vector3(light.X, light.Y, light.Z);
                    render.device.Lights[lightCount].Direction = new Vector3(0, -0.5f, 0);
                    render.device.Lights[lightCount].Range = 5.0f;
                    render.device.Lights[lightCount].InnerConeAngle = 0.5f;
                    render.device.Lights[lightCount].OuterConeAngle = 1.0f;
                    render.device.Lights[lightCount].Falloff = 2.0f;
                    render.device.Lights[lightCount].Attenuation0 = 2.0f;
                    render.device.Lights[lightCount].Update();
                    render.device.Lights[lightCount].Enabled = false;
                    lightCount++;
                    */
                    /*** This should go below ;)
                    for (int i = lightCount; i < render.device.Lights.Count; i++)
                        render.device.Lights[i].Enabled = false;
                    */
                    render.device.Material = BlueMaterial;
                    render.device.SetTexture(0, null);
                    render.device.RenderState.AlphaBlendEnable = false;
                    render.device.RenderState.AlphaTestEnable = false;
                    render.device.RenderState.FillMode = FillMode.WireFrame;
                    BoundingBoxModel[x].DrawSubset(0);

                    // This is the selected color
                    render.device.Material = RedMaterial;
                    drawModel = false;
                }

                #endregion

                #region DrawSounds

                if (bsp.Spawns.Spawn[x] is SpawnInfo.SoundSpawn)
                {
                    render.device.Material = RedMaterial;
                    render.device.SetTexture(0, null);
                    render.device.RenderState.AlphaBlendEnable = false;
                    render.device.RenderState.AlphaTestEnable = false;
                    render.device.RenderState.FillMode = FillMode.WireFrame;
                    BoundingBoxModel[x].DrawSubset(0);

                    render.device.Material = RedMaterial;
                    render.device.RenderState.AlphaBlendEnable = true;
                    render.device.RenderState.AlphaTestEnable = true;
                    render.device.RenderState.DestinationBlend = Blend.DestinationAlpha;
                    render.device.RenderState.SourceBlend = Blend.SourceAlpha;
                    render.device.RenderState.FillMode = FillMode.Solid;
                    BoundingBoxModel[x].DrawSubset(0);

                    // Renderer.BSP_Renderer.SpawnInfo.SoundSpawn tempbox = bsp.Spawns.Spawn[x] as Renderer.BSP_Renderer.SpawnInfo.SoundSpawn;
                    // render.device.Material = GreenMaterial;
                    // BoundingBoxModel[x] = D3D.Mesh.Sphere(render.device, tempbox.DistanceBoundsLower, 10 + (int)tempbox.DistanceBoundsUpper, 10 + (int)tempbox.DistanceBoundsUpper);
                    // BoundingBoxModel[x].DrawSubset(0);
                    // This is the selected color
                    render.device.Material = BlueMaterial;
                    drawModel = false;
                }

                #endregion

                #region DrawCameras

                if (bsp.Spawns.Spawn[x] is SpawnInfo.CameraSpawn)
                {
                    render.device.Material = BlueMaterial;
                    render.device.SetTexture(0, null);
                    render.device.RenderState.AlphaBlendEnable = false;
                    render.device.RenderState.AlphaTestEnable = false;
                    render.device.RenderState.FillMode = FillMode.WireFrame;
                    BoundingBoxModel[x].DrawSubset(0);

                    // This is the selected color
                    render.device.Material = RedMaterial;
                    drawModel = false;
                }

                #endregion

                #region DrawBoundingBoxOnly_Deathzones

                if (bsp.Spawns.Spawn[x] is SpawnInfo.BoundingBoxSpawn)
                {
                    if (bsp.Spawns.Spawn[x].Type == SpawnInfo.SpawnType.DeathZone)
                    {
                        render.device.Material = PinkMaterial;
                        render.device.SetTexture(0, null);
                        render.device.RenderState.AlphaBlendEnable = true;
                        render.device.RenderState.AlphaTestEnable = true;
                        render.device.RenderState.DestinationBlend = Blend.DestinationAlpha;
                        render.device.RenderState.SourceBlend = Blend.SourceAlpha;
                        render.device.RenderState.FillMode = FillMode.Solid;
                        BoundingBoxModel[x].DrawSubset(0);
                    }

                    render.device.Material = RedMaterial;
                    drawModel = false;
                }

                #endregion

                #region ObjectiveSpawnColoring

                if (bsp.Spawns.Spawn[x] is SpawnInfo.ObjectiveSpawn)
                {
                    render.device.RenderState.FillMode = FillMode.Solid;
                    SpawnInfo.ObjectiveSpawn os = bsp.Spawns.Spawn[x] as SpawnInfo.ObjectiveSpawn;
                    switch (os.Team)
                    {
                        case SpawnInfo.ObjectiveSpawn.TeamType.Red_Defense:
                            render.device.Material = RedMaterial;
                            break;
                        case SpawnInfo.ObjectiveSpawn.TeamType.Blue_Offense:
                            render.device.Material = BlueMaterial;
                            break;
                        case SpawnInfo.ObjectiveSpawn.TeamType.Yellow:
                            render.device.Material = YellowMaterial;
                            break;
                        case SpawnInfo.ObjectiveSpawn.TeamType.Green:
                            render.device.Material = GreenMaterial;
                            break;
                        case SpawnInfo.ObjectiveSpawn.TeamType.Purple:
                            render.device.Material = PurpleMaterial;
                            break;
                        case SpawnInfo.ObjectiveSpawn.TeamType.Orange:
                            render.device.Material = OrangeMaterial;
                            break;
                        case SpawnInfo.ObjectiveSpawn.TeamType.Brown:
                            render.device.Material = BrownMaterial;
                            break;
                        case SpawnInfo.ObjectiveSpawn.TeamType.Pink:
                            render.device.Material = PinkMaterial;
                            break;
                        case SpawnInfo.ObjectiveSpawn.TeamType.Neutral:
                            render.device.Material = NeutralMaterial;
                            break;
                        default:
                            render.device.Material = DefaultMaterial;
                            break;
                    }
                }

                #endregion

                #region DrawBoundingCylinder_SpawnZone

                if (bsp.Spawns.Spawn[x] is SpawnInfo.SpawnZone)
                {
                    render.device.Material = NeutralMaterial;
                    render.device.SetTexture(0, null);
                    render.device.RenderState.AlphaBlendEnable = true;
                    render.device.RenderState.AlphaTestEnable = true;
                    render.device.RenderState.DestinationBlend = Blend.DestinationAlpha;
                    render.device.RenderState.SourceBlend = Blend.SourceAlpha;
                    render.device.RenderState.FillMode = FillMode.Solid;
                    
                    // Adjust center position of Bounding Boxes to proper offset
                    Matrix mat = Matrix.Identity;
                    mat = Matrix.Add(
                        mat,
                        Matrix.Translation(
                            bsp.Spawns.Spawn[x].bbXDiff,
                            bsp.Spawns.Spawn[x].bbYDiff,
                            bsp.Spawns.Spawn[x].bbZDiff));
                    render.device.Transform.World = mat * TranslationMatrix[x];

                    BoundingBoxModel[x].DrawSubset(0);

                    render.device.Material = GreenMaterial;
                    drawModel = false;
                }

                #endregion


                /*
                 * // Add support for particles somewhere, this shows a box at least
                #region ObstacleSpawn
                if (bsp.Spawns.Spawn[x] is Entity.Renderer.BSP_Renderer.SpawnInfo.ObstacleSpawn)
                {
                    render.device.RenderState.FillMode = D3D.FillMode.Solid;
                    Entity.Renderer.BSP_Renderer.SpawnInfo.ObstacleSpawn os = bsp.Spawns.Spawn[x] as Entity.Renderer.BSP_Renderer.SpawnInfo.ObstacleSpawn;
                    string sss = os.ModelName;
                    BoundingBoxModel[x] = BoundingBoxModel[x - 1];
                }
                #endregion
                */

                #region DrawBoxOnSelections

                for (int i = 0; i < SelectedSpawn.Count; i++)
                {
                    if (SelectedSpawn[i] == x)
                    {
                        // Skip wireframe boxes for obstacles and scenery - just show solid models
                        if (bsp.Spawns.Spawn[x] is SpawnInfo.ObstacleSpawn ||
                            bsp.Spawns.Spawn[x] is SpawnInfo.ScenerySpawn)
                        {
                            break;
                        }

                        render.device.SetTexture(0, null);
                        render.device.RenderState.AlphaBlendEnable = false;
                        render.device.RenderState.AlphaTestEnable = false;
                        render.device.RenderState.FillMode = FillMode.WireFrame;

                        // Adjust center position of Bounding Boxes to proper offset
                        Matrix mat = Matrix.Identity;
                        mat = Matrix.Add(
                            mat,
                            Matrix.Translation(
                                bsp.Spawns.Spawn[SelectedSpawn[i]].bbXDiff,
                                bsp.Spawns.Spawn[SelectedSpawn[i]].bbYDiff,
                                bsp.Spawns.Spawn[SelectedSpawn[i]].bbZDiff));
                        render.device.Transform.World = mat * TranslationMatrix[x];
                        BoundingBoxModel[x].DrawSubset(0);

                        /***************/
                        float s1 = SpawnModel[spawnmodelindex[x]].BoundingBox.MaxX -
                                   SpawnModel[spawnmodelindex[x]].BoundingBox.MinX;
                        float s2 = SpawnModel[spawnmodelindex[x]].BoundingBox.MaxY -
                                   SpawnModel[spawnmodelindex[x]].BoundingBox.MinY;
                        float s3 = SpawnModel[spawnmodelindex[x]].BoundingBox.MaxZ -
                                   SpawnModel[spawnmodelindex[x]].BoundingBox.MinZ;
                        Vector4 v4 = Vector3.Transform(cam.Position, TranslationMatrix[x]);

                        SpawnInfo.BaseSpawn s = bsp.Spawns.Spawn[x];
                        Vector3 c = cam.Position;
                        float scale = (cam.Position.X - s.X) + (cam.Position.Y - s.Y) + (cam.Position.Z - s.Z);

                        scale = (((SpawnModel[spawnmodelindex[x]].BoundingBox.MaxX -
                                   SpawnModel[spawnmodelindex[x]].BoundingBox.MinX) +
                                  (SpawnModel[spawnmodelindex[x]].BoundingBox.MaxY -
                                   SpawnModel[spawnmodelindex[x]].BoundingBox.MinY) +
                                  (SpawnModel[spawnmodelindex[x]].BoundingBox.MaxZ -
                                   SpawnModel[spawnmodelindex[x]].BoundingBox.MinZ)) / 3) * 12;

                        scale = (v4.X + v4.Y + v4.Z) / 3;
                        if (gizmo != null)
                        {
                            gizmo.draw(scale / 50.0f);
                        }

                        /**********/
                    }
                }

                #endregion

                render.device.Transform.World = TranslationMatrix[x];

                /*  This was an attempt at adding scaling, but not right, so not right now.
                Entity.Raw.ParsedModel pm = SpawnModel[spawnmodelindex[x]];
                if (bsp.Spawns.Spawn[x] is Entity.Renderer.BSP_Renderer.SpawnInfo.ScaleRotateYawPitchRollSpawn)
                {
                    Entity.Renderer.BSP_Renderer.SpawnInfo.ScaleRotateYawPitchRollSpawn tempBsp = bsp.Spawns.Spawn[x] as Entity.Renderer.BSP_Renderer.SpawnInfo.ScaleRotateYawPitchRollSpawn;
                    for (int i = 0; i < pm.RawDataMetaChunks.Length; i++)
                        for (int j = 0; j < pm.RawDataMetaChunks[i].VerticeCount; j++)
                            pm.RawDataMetaChunks[i].Vertices[j] = Vector3.Scale(SpawnModel[spawnmodelindex[x]].RawDataMetaChunks[i].Vertices[j], tempBsp.Scale + 1.0f);
                }
                */

                if (drawModel)
                {                    
                    // Store old cull mode
                    Cull cm = render.device.RenderState.CullMode;
                    render.device.RenderState.CullMode = Cull.None;

                    if (bsp.Spawns.Spawn[x].frozen)
                    {
                        render.device.RenderState.FillMode = FillMode.WireFrame;
                    }
                    else
                    {
                        render.device.RenderState.FillMode = FillMode.Solid;
                    }

                    ParsedModel.DisplayedInfo.Draw(ref render.device, SpawnModel[spawnmodelindex[x]]);
                    // Restore old cull mode
                    render.device.RenderState.CullMode = cm;
                }
            }

            #endregion

            #region RenderHills

            if ((visibleSpawnsBitMask & (int)SpawnInfo.SpawnType.Objective) != 0)
            {
                Cull cm = render.device.RenderState.CullMode;
                render.device.RenderState.CullMode = Cull.None;
                render.device.Transform.World = Matrix.Identity;
                for (int i = 0; i < spawns.hillDisplay.Length; i++)
                {
                    if (spawns.hillDisplay[i] == null)
                    {
                        continue;
                    }

                    render.device.SetTexture(0, bsp.Shaders.Shader[1].MainTexture);
                    render.device.RenderState.AlphaBlendEnable = true;
                    render.device.RenderState.AlphaTestEnable = false;
                    render.device.RenderState.SourceBlend = Blend.One;
                    render.device.RenderState.DestinationBlend = Blend.One;
                    render.device.RenderState.FillMode = FillMode.Solid;

                    // Adjust center position of Bounding Boxes to proper offset
                    spawns.hillDisplay[i].DrawSubset(0);
                }

                render.device.RenderState.CullMode = cm;
            }

            #endregion

            /*
            #region RenderWater
            if (1 != 0)
            {
                render.device.Transform.World = Matrix.Identity;
                for (int i = 0; i < 1; i++) // bsp.Water.Length
                {
                    if (bsp.Water == null) { continue; }

                    render.device.SetTexture(0, null);
                    render.device.RenderState.AlphaBlendEnable = true;
                    render.device.RenderState.AlphaTestEnable = false;
                    render.device.RenderState.Ambient = Color.Aqua;
                    render.device.RenderState.SourceBlend = D3D.Blend.One;
                    render.device.RenderState.DestinationBlend = D3D.Blend.One;
                    render.device.RenderState.FillMode = D3D.FillMode.Solid;
                    // Adjust center position of Bounding Boxes to proper offset
                    render.device.Transform.World = Matrix.Translation(bsp.Water.centerX, bsp.Water.centerY, bsp.Water.height-10);
                    D3D.Mesh m = D3D.Mesh.Box(render.device, bsp.Water.extentX, bsp.Water.extentY, 10);
                    m.DrawSubset(0);
                    m.Dispose();
                }
            }
            #endregion
            */
            #region MultiSelectMesh

            if (selectionMesh != null)
            {
                Matrix m = Matrix.Identity;
                float halfwidth = selectionWidth / 2;
                float halfheight = selectionHeight / 2;
                float halfdepth = selectionDepth / 2;

                // m.Multiply(Matrix.Translation(selectionStart.X + halfwidth, selectionStart.Y + halfheight, selectionStart.Z + halfdepth));
                m.Multiply(Matrix.Translation(selectionStart.X, selectionStart.Y, selectionStart.Z));
                render.device.Transform.World = m;
                render.device.SetTexture(0, null);
                render.device.RenderState.AlphaBlendEnable = false;
                render.device.RenderState.AlphaTestEnable = false;
                render.device.RenderState.FillMode = FillMode.WireFrame;
                render.device.Material = RedMaterial;

                selectionMesh.DrawSubset(0);
            }

            #endregion

            if (this.BackColor == Color.Blue)
            {
                this.BackColor = Color.FromArgb(235, 233, 237);
            }

            #region RenderPlayerPath

            // Update and render player path animation
            UpdatePathAnimation();
            RenderPlayerPath();

            // Update camera for POV mode (live telemetry)
            if (showLiveTelemetry)
                UpdatePOVCamera();

            // Render live telemetry players
            RenderLivePlayers();

            // Draw HUD overlay (FPS, etc.) - Theater Mode only
            if (theaterMode)
            {
                DrawHUD();

                // Draw scoreboard overlay (Tab to toggle)
                if (showScoreboard)
                {
                    DrawScoreboard();
                }

                // Draw killfeed overlay (K to toggle)
                if (showKillfeed)
                {
                    DrawKillfeed();
                }
            }

            #endregion

            render.EndScene();
        }

        // SaveChanges
        /// <summary>
        /// The save changes_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void SaveChanges_Click(object sender, EventArgs e)
        {
            SelectedSpawn.Clear();
            map.OpenMap(MapTypes.Internal);
            for (int i = 0; i < bsp.Spawns.Spawn.Count; i++)
            {

                // Now handles all (???) data types. Some code kept below because I'm not
                // sure if it is handled here. Need to look into it more.
                bsp.Spawns.Spawn[i].Write(map);
                
                
                #region Obstacle
                if (bsp.Spawns.Spawn[i] is SpawnInfo.ObstacleSpawn)
                {
                    if (ObstacleList == null)
                    {
                        continue;
                    }

                    SpawnInfo.ObstacleSpawn os = bsp.Spawns.Spawn[i] as SpawnInfo.ObstacleSpawn;

                    // Base of SCNR tag, Pointer to Crates/Obstacles (+808)
                    map.BR.BaseStream.Position = map.MetaInfo.Offset[3] + 808;

                    // # of Obstacle chunks & offset to start
                    int count = map.BR.ReadInt32();
                    int BlocOffset = map.BR.ReadInt32() - map.SecondaryMagic;

                    // Each Block is 76 bytes in size
                    map.BR.BaseStream.Position = BlocOffset + os.BlocNumber * 76;

                    for (int yy = 0; yy < ObstacleList.Count; yy++)
                    {
                        if (ObstacleList[yy].TagPath == os.TagPath)
                        {
                            map.BW.Write((Int16)ObstacleList[yy].ScenPalNumber);
                            break;
                        }
                    }
                }

                #endregion
                #region Scenery
                else if (bsp.Spawns.Spawn[i] is SpawnInfo.ScenerySpawn && SceneryList != null)
                {
                    if (SceneryList == null)
                    {
                        continue;
                    }

                    // Spawn[].offset doesn't point to the start, but to the X Position, 8 bytes in
                    SpawnInfo.ScenerySpawn os;
                    os = bsp.Spawns.Spawn[i] as SpawnInfo.ScenerySpawn;

                    // Base of SCNR tag, Pointer to Scenery (+80)
                    map.BR.BaseStream.Position = map.MetaInfo.Offset[3] + 80;

                    // # of Scenery chunks & offset to start
                    int count = map.BR.ReadInt32();
                    int ScenOffset = map.BR.ReadInt32() - map.SecondaryMagic;

                    map.BR.BaseStream.Position = ScenOffset + os.ScenNumber * 92;
                    for (int yy = 0; yy < SceneryList.Count; yy++)
                    {
                        if (SceneryList[yy].TagPath == os.TagPath)
                        {
                            map.BW.Write((Int16)SceneryList[yy].ScenPalNumber);
                            break;
                        }
                    }

                    /****
                    // Base of SCNR tag, Pointer to Scenery Palette (+88)
                    map.BR.BaseStream.Position = map.MetaInfo.Offset[3] + 88;
                    // # of Scenery Palette chunks & offset to start
                    int SPcount = map.BR.ReadInt32();
                    int SPOffset = map.BR.ReadInt32() - map.SecondaryMagic;
                    for (int xx = 0; xx < count; xx++)
                    {
                        map.BR.BaseStream.Position = SPOffset + xx * 40;
                        byte cc = map.BR.ReadByte();
                        cc = map.BR.ReadByte();
                        cc = map.BR.ReadByte();
                        cc = map.BR.ReadByte();
                        //necs
                        int aa = map.BR.ReadInt32();
                        int aaa = map.Functions.Meta.FindMetaByID(aa, map);
                    }
                    ****/
                }

                #endregion
            }

            

            /*
            switch (map.HaloVersion)
            {
                case Map.HaloVersionEnum.Halo2:
                    #region H2
                    // *** only for maps with 1 BSP for now !!
                    int BSPNum = 0;
                    if (map.BSP.bspcount > 1) { break; }
                    map.BW.BaseStream.Position = 172;
                    map.BW.Write( (Int32) bsp.BSPRawDataMetaChunks.Length);
                    map.BW.Write( (Int32) bsp.BSPRawDataMetaChunksOffset+
                                                        + map.BSP.sbsp[BSPNum].magic
                                                        + map.BSP.sbsp[BSPNum].offset);
                     * for (int x = 0; x < bsp.BSPRawDataMetaChunks.Length; x++)
                    {
                        H2BSPWriteRawDataMetaChunk( x, ref bsp, BSPNum);
                    }
                    #endregion
                    break;
            }
            */

            
            map.CloseMap();

            MessageBox.Show("Done");
        }

        /// <summary>
        /// The select all spawns_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void SelectAllSpawns_Click(object sender, EventArgs e)
        {
            SelectedSpawn.Clear();
            for (int i = 0; i < bsp.Spawns.Spawn.Count; i++)
            {
                if (((int)bsp.Spawns.Spawn[i].Type & visibleSpawnsBitMask) == 0)
                {
                    continue;
                }

                SelectedSpawn.Add(i);
            }
        }

        /// <summary>
        /// The setup matrices.
        /// </summary>
        /// <remarks></remarks>
        private void SetupMatrices()
        {
            render.device.Transform.World = Matrix.Identity;

            // Matrix.RotationAxis(new Vector3((float)Math.Cos(Environment.TickCount / 250.0f), 1, (float)Math.Sin(Environment.TickCount / 250.0f)), Environment.TickCount / 1000.0f);
            render.device.Transform.View = Matrix.LookAtRH(cam.Position, cam.LookAt, cam.UpVector);
            // Use theater FOV if in theater mode, otherwise default 45 degrees (0.785 radians)
            float fovRadians = theaterMode ? (theaterFOV * (float)Math.PI / 180f) : 0.785f;
            render.device.Transform.Projection = Matrix.PerspectiveFovRH(fovRadians, aspect, 0.2f, 1000.0f);

            // cam.BuildViewFrustum(ref render.device);
        }

        /// <summary>
        /// The spawn list_ check.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void SpawnList_Check(object sender, ItemCheckEventArgs e)
        {
            setSpawnBox(checkedListBox1.Items[e.Index].ToString(), e.NewValue);
            updateStatusPosition();
        }

        /// <summary>
        /// The tool strip menu item rotate_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void ToolStripMenuItemRotate_Click(object sender, EventArgs e)
        {
            float change = 0.0f;
            switch (((ToolStripDropDownItem)sender).Text.Substring(0, ((ToolStripDropDownItem)sender).Text.IndexOf('*'))
                )
            {
                case "45":
                    change = (float)Math.PI / 4;
                    break;
                case "90":
                    change = (float)Math.PI / 2;
                    break;
                case "180":
                    change = (float)Math.PI;
                    break;
            }

            if (((ToolStripDropDownItem)sender).Name.ToUpper().Contains("CCW"))
            {
                change = -change;
            }

            if (((ToolStripDropDownItem)sender).OwnerItem == ToolStripDropDownButtonRotateYaw)
            {
                foreach (int i in SelectedSpawn)
                {
                    SpawnInfo.BaseSpawn spawn = bsp.Spawns.Spawn[i];
                    if (spawn is SpawnInfo.RotateYawPitchRollBaseSpawn)
                    {
                        ((SpawnInfo.RotateYawPitchRollBaseSpawn)spawn).Yaw += change;
                    }
                    else if (spawn is SpawnInfo.RotateDirectionBaseSpawn)
                    {
                        ((SpawnInfo.RotateDirectionBaseSpawn)spawn).RotationDirection -= change;
                    }
                }
            }
            else if (((ToolStripDropDownItem)sender).OwnerItem == ToolStripDropDownButtonRotatePitch)
            {
                foreach (int i in SelectedSpawn)
                {
                    SpawnInfo.BaseSpawn spawn = bsp.Spawns.Spawn[i];
                    if (spawn is SpawnInfo.RotateYawPitchRollBaseSpawn)
                    {
                        ((SpawnInfo.RotateYawPitchRollBaseSpawn)spawn).Pitch -= change;
                    }
                }
            }
            else if (((ToolStripDropDownItem)sender).OwnerItem == toolStripDropDownButtonRotateRoll)
            {
                foreach (int i in SelectedSpawn)
                {
                    SpawnInfo.BaseSpawn spawn = bsp.Spawns.Spawn[i];
                    if (spawn is SpawnInfo.RotateYawPitchRollBaseSpawn)
                    {
                        ((SpawnInfo.RotateYawPitchRollBaseSpawn)spawn).Roll -= change;
                    }
                }
            }
        }

        /// <summary>
        /// The vector to rgba.
        /// </summary>
        /// <param name="v">The v.</param>
        /// <param name="height">The height.</param>
        /// <returns>The vector to rgba.</returns>
        /// <remarks></remarks>
        private int VectorToRgba(Vector3 v, float height)
        {
            int r = (int)(127.0f * v.X + 128.0f);
            int g = (int)(127.0f * v.Y + 128.0f);
            int b = (int)(127.0f * v.Z + 128.0f);
            int a = (int)(255.0f * height);

            return (a << 24) + (r << 16) + (g << 8) + (b << 0);
        }

        /// <summary>
        /// The button 1_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void button1_Click(object sender, EventArgs e)
        {
            trackBar1.Value = 0;
            trackBar2.Value = 0;
            trackBar3.Value = 0;

            Array.ConstrainedCopy(LightMap_Array_Backup, 0, LightMap_Array, 0, LightMap_Array.Length);

            foreach (PictureBox picbox in panel1.Controls)
            {
                picbox.Image = RenderLightmap((int)((picbox).Tag));
            }
        }

        /// <summary>
        /// The button 2_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void button2_Click(object sender, EventArgs e)
        {
            ReloadFromArray();

            bsp.LoadLightmaps();

            BSPModel.BSPDisplayedInfo.LoadLightmapTextures(ref render.device, ref bsp);
        }

        /// <summary>
        /// The button 3_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void button3_Click(object sender, EventArgs e)
        {
            // Save the lightmap
            map.BSP.sbsp[bsp.BspNumber].WritePalettes(map);

            // Apply the new palettes
            Array.ConstrainedCopy(LightMap_Array, 0, LightMap_Array_Backup, 0, LightMap_Array.Length);

            trackBar1.Value = 0;
            trackBar2.Value = 0;
            trackBar3.Value = 0;

            MessageBox.Show("Done");
        }

        /// <summary>
        /// The button 4_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void button4_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < checkedListBox2.Items.Count; i++)
            {
                checkedListBox2.SetItemChecked(i, true);
            }
        }

        /// <summary>
        /// The button 5_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void button5_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < checkedListBox2.Items.Count; i++)
            {
                checkedListBox2.SetItemChecked(i, false);
            }
        }

        /// <summary>
        /// The check box 1_ checked changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                comboBox1.Enabled = false;
            }
            else
            {
                comboBox1.Enabled = true;
            }
        }

        /// <summary>
        /// The check box 2_ checked changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            EditLightmaps();
        }

        /// <summary>
        /// The check box 3_ checked changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox3.Checked)
            {
                for (int i = 0; i < checkedListBox1.Items.Count; i++)
                {
                    checkedListBox1.SetItemCheckState(i, CheckState.Checked);
                }
            }
            else
            {
                for (int i = 0; i < checkedListBox1.Items.Count; i++)
                {
                    checkedListBox1.SetItemCheckState(i, CheckState.Unchecked);
                }
            }
        }

        /// <summary>
        /// The combo_ selected index changed collection.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void combo_SelectedIndexChangedCollection(object sender, EventArgs e)
        {
            // We need this here so that when the program changes the box, it doesn't change everything selected!
            if (!((ToolStripComboBox)sender).Focused)
            {
                return;
            }

            ToolStripComboBox cb = sender as ToolStripComboBox;

            foreach (int i in SelectedSpawn)
            {
                if (bsp.Spawns.Spawn[i].Type == SpawnInfo.SpawnType.Collection)
                {
                    SpawnInfo.Collection os = bsp.Spawns.Spawn[i] as SpawnInfo.Collection;
                    object test = Enum.Parse(
                        typeof(SpawnInfo.Collection.SpawnsInEnum), cb.Items[cb.SelectedIndex].ToString(), true);
                    os.SpawnsInMode = (SpawnInfo.Collection.SpawnsInEnum)test;
                    bsp.Spawns.Spawn[i] = os;
                }
            }
        }

        /// <summary>
        /// The combo_ selected index changed collection model.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void combo_SelectedIndexChangedCollectionModel(object sender, EventArgs e)
        {
            // We need this here so that when the program changes the box, it doesn't change everything selected!
            if (!((ToolStripComboBox)sender).Focused)
            {
                return;
            }

            ToolStripComboBox cb = sender as ToolStripComboBox;

            // looks for a model already on the map. if not FOUND, adds it to the SpawnModels
            bool found = false;
            int SpawnModelNum = -1;

            // Lists all weapons
            for (int i = 0; i < WeaponsList.Count; i++)
            {
                if (WeaponsList[i].Name == cb.SelectedItem.ToString())
                {
                    for (int j = 0; j < SpawnModel.Count; j++)
                    {
                        if (SpawnModel[j].name == WeaponsList[i].Model.name)
                        {
                            SpawnModelNum = j;
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                    {
                        SpawnModel.Add(WeaponsList[i].Model);
                        SpawnModelNum = SpawnModel.Count - 1;
                    }

                    // Change the bounding box for the model
                    float boxwidth = SpawnModel[SpawnModelNum].BoundingBox.MaxX -
                                     SpawnModel[SpawnModelNum].BoundingBox.MinX;
                    float boxheight = SpawnModel[SpawnModelNum].BoundingBox.MaxY -
                                      SpawnModel[SpawnModelNum].BoundingBox.MinY;
                    float boxdepth = SpawnModel[SpawnModelNum].BoundingBox.MaxZ -
                                     SpawnModel[SpawnModelNum].BoundingBox.MinZ;

                    for (int j = 0; j < this.SelectedSpawn.Count; j++)
                    {
                        spawnmodelindex[SelectedSpawn[j]] = SpawnModelNum;
                        BoundingBoxModel[SelectedSpawn[j]] = Mesh.Box(render.device, boxwidth, boxheight, boxdepth);

                        SpawnInfo.Collection bspInfo = (SpawnInfo.Collection)bsp.Spawns.Spawn[SelectedSpawn[j]];
                        bspInfo.bbXDiff = SpawnModel[SpawnModelNum].BoundingBox.MaxX +
                                          SpawnModel[SpawnModelNum].BoundingBox.MinX;
                        bspInfo.bbYDiff = SpawnModel[SpawnModelNum].BoundingBox.MaxY +
                                          SpawnModel[SpawnModelNum].BoundingBox.MinY;
                        bspInfo.bbZDiff = SpawnModel[SpawnModelNum].BoundingBox.MaxZ +
                                          SpawnModel[SpawnModelNum].BoundingBox.MinZ;

                        bspInfo.ModelTagNumber = WeaponsList[i].ModelTagNumber;
                        bspInfo.ModelName = map.FileNames.Name[WeaponsList[i].ModelTagNumber];
                        bspInfo.TagPath = WeaponsList[i].TagPath;
                        bspInfo.TagType = WeaponsList[i].TagType;

                        for (int a = 0; a < treeView1.Nodes.Count; a++)
                        {
                            for (int aa = 0; aa < treeView1.Nodes[a].Nodes.Count; aa++)
                            {
                                if (treeView1.Nodes[a].Nodes[aa].Tag.ToString() == SelectedSpawn[j].ToString())
                                {
                                    string[] temps = bspInfo.TagPath.Split('\\');
                                    this.treeView1.Nodes[a].Nodes[aa].Text = temps[temps.Length - 1];
                                }
                            }
                        }
                    }

                    treeView1.Sort();
                }
            }
        }

        /// <summary>
        /// The combo_ selected index changed objective.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void combo_SelectedIndexChangedObjective(object sender, EventArgs e)
        {
            // We need this here so that when the program changes the box, it doesn't change everything selected!
            if (!((ToolStripComboBox)sender).Focused)
            {
                return;
            }

            ToolStripComboBox cb = sender as ToolStripComboBox;

            foreach (int i in SelectedSpawn)
            {
                if (bsp.Spawns.Spawn[i].Type == SpawnInfo.SpawnType.Objective)
                {
                    SpawnInfo.ObjectiveSpawn os = bsp.Spawns.Spawn[i] as SpawnInfo.ObjectiveSpawn;

                    object test = Enum.Parse(
                        typeof(SpawnInfo.ObjectiveSpawn.ObjectiveTypeEnum), cb.Items[cb.SelectedIndex].ToString(), true);
                    os.ObjectiveType = (SpawnInfo.ObjectiveSpawn.ObjectiveTypeEnum)test;
                    bsp.Spawns.Spawn[i] = os;
                }
            }
        }

        /// <summary>
        /// The combo_ selected index changed obstacle model.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void combo_SelectedIndexChangedObstacleModel(object sender, EventArgs e)
        {
            // We need this here so that when the program changes the box, it doesn't change everything selected!
            if (!((ToolStripComboBox)sender).Focused)
            {
                return;
            }

            ToolStripComboBox cb = sender as ToolStripComboBox;

            // looks for a model already on the map. if not FOUND, adds it to the SpawnModels
            bool found = false;
            int SpawnModelNum = -1;

            // Lists all Scenery & Obstacles
            for (int i = 0; i < ObstacleList.Count; i++)
            {
                if (ObstacleList[i].Name == cb.SelectedItem.ToString())
                {
                    for (int j = 0; j < SpawnModel.Count; j++)
                    {
                        if (SpawnModel[j].name == ObstacleList[i].Model.name)
                        {
                            SpawnModelNum = j;
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                    {
                        SpawnModel.Add(ObstacleList[i].Model);
                        SpawnModelNum = SpawnModel.Count - 1;
                    }

                    // Change the bounding box for the model
                    float boxwidth = SpawnModel[SpawnModelNum].BoundingBox.MaxX -
                                     SpawnModel[SpawnModelNum].BoundingBox.MinX;
                    float boxheight = SpawnModel[SpawnModelNum].BoundingBox.MaxY -
                                      SpawnModel[SpawnModelNum].BoundingBox.MinY;
                    float boxdepth = SpawnModel[SpawnModelNum].BoundingBox.MaxZ -
                                     SpawnModel[SpawnModelNum].BoundingBox.MinZ;

                    for (int j = 0; j < this.SelectedSpawn.Count; j++)
                    {
                        spawnmodelindex[SelectedSpawn[j]] = SpawnModelNum;
                        BoundingBoxModel[SelectedSpawn[j]] = Mesh.Box(render.device, boxwidth, boxheight, boxdepth);

                        SpawnInfo.BaseSpawn bspInfo = bsp.Spawns.Spawn[SelectedSpawn[j]];
                        bspInfo.bbXDiff = SpawnModel[SpawnModelNum].BoundingBox.MaxX +
                                          SpawnModel[SpawnModelNum].BoundingBox.MinX;
                        bspInfo.bbYDiff = SpawnModel[SpawnModelNum].BoundingBox.MaxY +
                                          SpawnModel[SpawnModelNum].BoundingBox.MinY;
                        bspInfo.bbZDiff = SpawnModel[SpawnModelNum].BoundingBox.MaxZ +
                                          SpawnModel[SpawnModelNum].BoundingBox.MinZ;

                        bspInfo.TagPath = ObstacleList[i].TagPath;
                        bspInfo.TagType = ObstacleList[i].TagType;

                        for (int a = 0; a < treeView1.Nodes.Count; a++)
                        {
                            for (int aa = 0; aa < treeView1.Nodes[a].Nodes.Count; aa++)
                            {
                                if (treeView1.Nodes[a].Nodes[aa].Tag.ToString() == SelectedSpawn[j].ToString())
                                {
                                    string[] temps = bspInfo.TagPath.Split('\\');
                                    this.treeView1.Nodes[a].Nodes[aa].Text = temps[temps.Length - 1];
                                }
                            }
                        }
                    }

                    treeView1.Sort();
                }
            }
        }

        /// <summary>
        /// The combo_ selected index changed scenery model.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void combo_SelectedIndexChangedSceneryModel(object sender, EventArgs e)
        {
            // We need this here so that when the program changes the box, it doesn't change everything selected!
            if (!((ToolStripComboBox)sender).Focused)
            {
                return;
            }

            ToolStripComboBox cb = sender as ToolStripComboBox;

            // looks for a model already on the map. if not FOUND, adds it to the SpawnModels
            bool found = false;
            int SpawnModelNum = -1;

            // Lists all Scenery & Obstacles
            for (int i = 0; i < SceneryList.Count; i++)
            {
                if (SceneryList[i].Name == cb.SelectedItem.ToString())
                {
                    for (int j = 0; j < SpawnModel.Count; j++)
                    {
                        if (SpawnModel[j].name == SceneryList[i].Model.name)
                        {
                            SpawnModelNum = j;
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                    {
                        SpawnModel.Add(SceneryList[i].Model);
                        SpawnModelNum = SpawnModel.Count - 1;
                    }

                    // Change the bounding box for the model
                    float boxwidth = SpawnModel[SpawnModelNum].BoundingBox.MaxX -
                                     SpawnModel[SpawnModelNum].BoundingBox.MinX;
                    float boxheight = SpawnModel[SpawnModelNum].BoundingBox.MaxY -
                                      SpawnModel[SpawnModelNum].BoundingBox.MinY;
                    float boxdepth = SpawnModel[SpawnModelNum].BoundingBox.MaxZ -
                                     SpawnModel[SpawnModelNum].BoundingBox.MinZ;

                    for (int j = 0; j < this.SelectedSpawn.Count; j++)
                    {
                        spawnmodelindex[SelectedSpawn[j]] = SpawnModelNum;
                        BoundingBoxModel[SelectedSpawn[j]] = Mesh.Box(render.device, boxwidth, boxheight, boxdepth);

                        SpawnInfo.BaseSpawn bspInfo = bsp.Spawns.Spawn[SelectedSpawn[j]];
                        bspInfo.bbXDiff = SpawnModel[SpawnModelNum].BoundingBox.MaxX +
                                          SpawnModel[SpawnModelNum].BoundingBox.MinX;
                        bspInfo.bbYDiff = SpawnModel[SpawnModelNum].BoundingBox.MaxY +
                                          SpawnModel[SpawnModelNum].BoundingBox.MinY;
                        bspInfo.bbZDiff = SpawnModel[SpawnModelNum].BoundingBox.MaxZ +
                                          SpawnModel[SpawnModelNum].BoundingBox.MinZ;

                        bspInfo.TagPath = SceneryList[i].TagPath;
                        bspInfo.TagType = SceneryList[i].TagType;

                        for (int a = 0; a < treeView1.Nodes.Count; a++)
                        {
                            for (int aa = 0; aa < treeView1.Nodes[a].Nodes.Count; aa++)
                            {
                                if (treeView1.Nodes[a].Nodes[aa].Tag.ToString() == SelectedSpawn[j].ToString())
                                {
                                    string[] temps = bspInfo.TagPath.Split('\\');
                                    this.treeView1.Nodes[a].Nodes[aa].Text = temps[temps.Length - 1];
                                }
                            }
                        }
                    }

                    treeView1.Sort();
                }
            }
        }

        /// <summary>
        /// The combo_ selected index changed team.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void combo_SelectedIndexChangedTeam(object sender, EventArgs e)
        {
            // We need this here so that when the program changes the box, it doesn't change everything selected!
            if (!((ToolStripComboBox)sender).Focused)
            {
                return;
            }

            ToolStripComboBox cb = sender as ToolStripComboBox;

            foreach (int i in SelectedSpawn)
            {
                if (bsp.Spawns.Spawn[i].Type == SpawnInfo.SpawnType.Objective)
                {
                    SpawnInfo.ObjectiveSpawn os = bsp.Spawns.Spawn[i] as SpawnInfo.ObjectiveSpawn;
                    object test = Enum.Parse(
                        typeof(SpawnInfo.ObjectiveSpawn.TeamType), cb.Items[cb.SelectedIndex].ToString(), true);
                    os.Team = (SpawnInfo.ObjectiveSpawn.TeamType)test;
                    bsp.Spawns.Spawn[i] = os;
                }
            }
        }

        /// <summary>
        /// The data grid row_ lost focus.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void dataGridRow_LostFocus(object sender, DataGridViewCellEventArgs e)
        {
            // If we want to change how it is displayed somehow, we need this to change it back...
        }

        /// <summary>
        /// The data grid row_ select.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void dataGridRow_Select(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView dgView = (DataGridView)sender;
            if (!dgView.Focused)
            {
                return;
            }

            // bsp.Spawns.Spawn[spawnNumber].        // Possibly change the mesh color
        }

        /// <summary>
        /// The data grid_ cell click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void dataGrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView dgView = (DataGridView)sender;
            if ((!dgView.Focused) || (dgView.CurrentCell.RowIndex == dgView.RowCount - 1))
            {
                return;
            }

            // If click happened on Remove("X") column, remove row and selection
            if (dgView.CurrentCell.ColumnIndex == dgView.ColumnCount - 1)
            {
                removeRow(ref dgView, e.RowIndex);
            }
            else if (SelectedSpawn.Count != 0)
            {
                int spawnNumber = int.Parse(dgView[1, dgView.CurrentCell.RowIndex].Value.ToString());
                setCameraPosition(
                    bsp.Spawns.Spawn[spawnNumber].X, 
                    bsp.Spawns.Spawn[spawnNumber].Y, 
                    bsp.Spawns.Spawn[spawnNumber].Z, 
                    false);
            }

            Render();
            updateStatusPosition();
            dgView.Focus();

            // bsp.Spawns.Spawn[spawnNumber].        // Possibly change the mesh color
        }

        /// <summary>
        /// The data grid_resize.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void dataGrid_resize(object sender, EventArgs e)
        {
            DataGridView grid = (DataGridView)sender;
            int totalHeight = grid.ColumnHeadersHeight + 2;
            for (int i = 0; i < grid.Rows.Count; i++)
            {
                totalHeight += grid.Rows[i].Height;
            }

            

            grid.Columns[0].Width = 70;
            grid.Columns[3].Width = 60;
            grid.Columns[4].Width = 60;
            grid.Columns[5].Width = 60;
            grid.Columns[6].Width = 50;
            int totalWidth = grid.Columns[0].Width;
            for (int i = 3; i < 7; i++)
            {
                totalWidth += grid.Columns[i].Width;
            }

            if (totalHeight > (grid.Parent.Height - 50))
            {
                grid.Columns[2].Width = grid.Parent.Width - 40 - totalWidth;
            }
            else
            {
                grid.Columns[2].Width = grid.Parent.Width - 23 - totalWidth;
            }

            
        }

        /// <summary>
        /// The do info.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <remarks></remarks>
        private void doInfo(string name)
        {
            ToolStripTextBox tsInfo = new ToolStripTextBox("tsInfo");
            tsInfo.BackColor = Color.Red;
            tsInfo.BorderStyle = BorderStyle.FixedSingle;
            tsInfo.Padding = new Padding(15, 2, 15, 2);
            tsInfo.Size = new Size(245, 22);
            tsInfo.Text = "Loading " + name + "s, Please wait...";
            tsInfo.TextBoxTextAlign = HorizontalAlignment.Center;
            statusStrip.Items.Insert(0, tsInfo);
            statusStrip.ResumeLayout();
            statusStrip.SuspendLayout();
        }

        /// <summary>
        /// The dock control 6_ enter.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void dockControl6_Enter(object sender, EventArgs e)
        {
            /*  No changes are saved, aka doesn't brick maps ***
            if (firstBSPUsage)
            {
                MessageBox.Show("This is a preliminary testing area! Do NOT save files without all boxes selected or the map will be destroyed?");
                firstBSPUsage = false;
            }
            */
        }

        // D3D.Material[] meshmaterials, Texture[] meshtextures)

        /// <summary>
        /// The fcordbutton_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void fcordbutton_Click(object sender, EventArgs e)
        {
            try
            {
                setCameraPosition(
                    Convert.ToSingle(fcordx.Text), Convert.ToSingle(fcordy.Text), Convert.ToSingle(fcordz.Text), true);
            }
            catch (Exception ex)
            {
                Global.ShowErrorMsg("There was a problem finding your Coordinate.", ex);
            }
        }

        /// <summary>
        /// The findspawn_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void findspawn_Click(object sender, EventArgs e)
        {
            if (SelectedSpawn.Count == 0)
            {
                return;
            }

            cam.Position.X = bsp.Spawns.Spawn[SelectedSpawn[0]].X;
            cam.Position.Y = bsp.Spawns.Spawn[SelectedSpawn[0]].Y;
            cam.Position.Z = bsp.Spawns.Spawn[SelectedSpawn[0]].Z;

            cam.radianv = 0;
            cam.radianh = 0;

            cam.x = bsp.Spawns.Spawn[SelectedSpawn[0]].X;
            cam.y = bsp.Spawns.Spawn[SelectedSpawn[0]].Y;
            cam.z = bsp.Spawns.Spawn[SelectedSpawn[0]].Z;
        }

        /// <summary>
        /// The ident context_ opening.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void identContext_Opening(object sender, CancelEventArgs e)
        {
            if (identContext.SourceControl is TreeView)
            {
                TreeView c = identContext.SourceControl as TreeView;
                if (!c.Focused)
                {
                    c.Focus();
                }

                this.selectFreezeMenuItem.Visible = false;
                this.selectFreezeAllMenuItem.Visible = false;
                this.selectUnFreezeAllMenuItem.Visible = false;
                this.selectCurrentToolStripMenuItem.Visible = false;
                this.selectGroupToolStripMenuItem.Visible = false;
                if (c.SelectedNode == null)
                {
                    this.selectAllToolStripMenuItem.Visible = false;
                    return;
                }
                else
                {
                    this.selectAllToolStripMenuItem.Visible = true;
                    this.selectAllToolStripMenuItem.Tag = "ALL" + c.SelectedNode.Tag;
                }

                // Sets the parent node or the selected node if it is already the top Node
                string Parent;
                if (c.SelectedNode.Parent != null)
                {
                    Parent = c.SelectedNode.Parent.Text;
                    this.selectCurrentToolStripMenuItem.Visible = true;
                    if (SelectedSpawn.IndexOf(int.Parse(c.SelectedNode.Tag.ToString())) == -1)
                    {
                        this.selectCurrentToolStripMenuItem.Text = "Select";
                        this.selectCurrentToolStripMenuItem.Tag = "SEL" + c.SelectedNode.Tag;
                    }
                    else
                    {
                        this.selectCurrentToolStripMenuItem.Text = "Deselect";
                        this.selectCurrentToolStripMenuItem.Tag = "DES" + c.SelectedNode.Tag;
                    }
                }
                else
                {
                    Parent = c.SelectedNode.Text;
                }

                string tag = c.SelectedNode.Text;
                tag = tag.Substring(0, (tag + " ").IndexOf(" "));

                // Updates right-click menu selections
                this.selectAllToolStripMenuItem.Text = "Select all " + Parent + "s";
                if (Parent.ToUpper() == "OBJECTIVE")
                {
                    // Option to select the Arming Circle
                    if (tag == SpawnInfo.ObjectiveSpawn.ObjectiveTypeEnum.ArmingCircle.ToString())
                    {
                        this.selectGroupToolStripMenuItem.Text = "Select group> " +
                                                                 c.SelectedNode.Text.Substring(
                                                                     0, c.SelectedNode.Text.LastIndexOf('#'));
                        this.selectGroupToolStripMenuItem.Tag = "ARM" + c.SelectedNode.Tag;
                        this.selectGroupToolStripMenuItem.Visible = true;
                    }

                    // Option to select all of one hill
                    if (
                        tag.StartsWith(
                            SpawnInfo.ObjectiveSpawn.ObjectiveTypeEnum.KingOfTheHill_1.ToString().Substring(0, 13)))
                    {
                        this.selectGroupToolStripMenuItem.Text = "Select group> Hill #" +
                                                                 tag.Substring(tag.Length - 1, 1);
                        this.selectGroupToolStripMenuItem.Tag = "HIL" + c.SelectedNode.Tag;
                        this.selectGroupToolStripMenuItem.Visible = true;
                    }

                    // Territories are selected by same color and #
                    if (tag == SpawnInfo.ObjectiveSpawn.ObjectiveTypeEnum.Territory.ToString())
                    {
                        this.selectGroupToolStripMenuItem.Text = "Select group> " + c.SelectedNode.Text;
                        this.selectGroupToolStripMenuItem.Tag = "TER" + c.SelectedNode.Tag;
                        this.selectGroupToolStripMenuItem.Visible = true;
                    }
                }

                this.selectNoneToolStripMenuItem.Tag = "DES-1";
            }
            else if (identContext.SourceControl is BSPViewer)
            {
                #region BSPViewer_Click
                    Time = DateTime.Now.TimeOfDay.Subtract(Time);
                if ((Time.Ticks / 1000000) >= 3)
                {
                    identContext.Hide();
                    return;
                }

                BSPViewer c = identContext.SourceControl as BSPViewer;

                // + 20 for Title bar
                MouseEventArgs me = new MouseEventArgs(
                    MouseButtons, 
                    1, 
                    MousePosition.X - this.Left, 
                    MousePosition.Y - this.Top - SystemInformation.CaptionHeight -
                    (SystemInformation.BorderSize.Height * 2), 
                    0);
                int[] intersect = checkForIntersection(me);
                if (intersect.Length > 0)
                {
                    currentObject = intersect[0];
                }
                else
                {
                    currentObject = -1;
                }

                this.selectFreezeMenuItem.Visible = false;
                this.selectFreezeAllMenuItem.Visible = true;
                this.selectUnFreezeAllMenuItem.Visible = true;
                this.selectCurrentToolStripMenuItem.Visible = false;
                this.selectGroupToolStripMenuItem.Visible = false;

                string tag = null;
                if (currentObject > -1)
                {
                    this.selectFreezeMenuItem.Visible = true;
                    if (bsp.Spawns.Spawn[currentObject].frozen)
                    {
                        this.selectFreezeMenuItem.Text = "UnFreeze";
                    }
                    else
                    {
                        this.selectFreezeMenuItem.Text = "Freeze";
                    }

                    switch (bsp.Spawns.Spawn[currentObject].Type)
                    {
                        case SpawnInfo.SpawnType.Objective:
                            SpawnInfo.ObjectiveSpawn spawn = (SpawnInfo.ObjectiveSpawn)bsp.Spawns.Spawn[currentObject];
                            tag = spawn.ObjectiveType + "#" + spawn.number;

                            // Option to select the Arming Circle
                            if (spawn.ObjectiveType == SpawnInfo.ObjectiveSpawn.ObjectiveTypeEnum.ArmingCircle)
                            {
                                this.selectGroupToolStripMenuItem.Text = "Select group> " + spawn.ObjectiveType;
                                this.selectGroupToolStripMenuItem.Tag = "ARM" + tag;
                                this.selectGroupToolStripMenuItem.Visible = true;
                            }

                                // Option to select all of one hill
                            else if (
                                spawn.ObjectiveType.ToString().StartsWith(
                                    SpawnInfo.ObjectiveSpawn.ObjectiveTypeEnum.KingOfTheHill_1.ToString().Substring(
                                        0, 13)))
                            {
                                this.selectGroupToolStripMenuItem.Text = "Select group> Hill #" + spawn.number;
                                this.selectGroupToolStripMenuItem.Tag = "HIL" + tag;
                                this.selectGroupToolStripMenuItem.Visible = true;
                            }

                                // Territories are selected by same color and #
                            else if (spawn.ObjectiveType == SpawnInfo.ObjectiveSpawn.ObjectiveTypeEnum.Territory)
                            {
                                this.selectGroupToolStripMenuItem.Text = "Select group> " + spawn.ObjectiveType +
                                                                         "#" + spawn.number;
                                this.selectGroupToolStripMenuItem.Tag = "TER" + tag;
                                this.selectGroupToolStripMenuItem.Visible = true;
                            }

                            break;
                        case SpawnInfo.SpawnType.Light:
                        case SpawnInfo.SpawnType.Sound:
                        case SpawnInfo.SpawnType.Scenery:
                        case SpawnInfo.SpawnType.Collection:
                            string[] s = bsp.Spawns.Spawn[currentObject].TagPath.Split('\\');
                            tag = s[s.Length - 1];
                            break;
                        default:
                            tag = bsp.Spawns.Spawn[currentObject].Type.ToString();
                            break;
                    }

                    this.selectAllToolStripMenuItem.Visible = true;
                    this.selectAllToolStripMenuItem.Text = "Select all " + bsp.Spawns.Spawn[currentObject].Type + "s";
                }
                else
                {
                    this.selectAllToolStripMenuItem.Visible = false;
                }

                for (int i = 0; i < this.identContext.Items.Count; i++)
                {
                    this.identContext.Items[i].Tag = "viewer";
                }

                #endregion
            }
        }

        /// <summary>
        /// The left sand dock_ mouse leave.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void leftSandDock_MouseLeave(object sender, EventArgs e)
        {
            if (!statusStrip.ContainsFocus)
            {
                speedLabel.Focus();
            }
        }

        /// <summary>
        /// The list form_resize.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void listForm_resize(object sender, EventArgs e)
        {
            DataGridView grid = (DataGridView)((Form)sender).Controls[0];
            grid.Width = ((Form)sender).Width - 20;
            grid.Height = ((Form)sender).Height - 50;
        }

        /// <summary>
        /// The make digits only.
        /// </summary>
        /// <param name="tb">The tb.</param>
        /// <remarks></remarks>
        private void makeDigitsOnly(ref ToolStripTextBox tb)
        {
            try
            {
                float.Parse(tb.Text);
            }
            catch
            {
                int i = 0;
                while (i < tb.Text.Length)
                {
                    if (!char.IsDigit(tb.Text[i]) && !char.IsPunctuation(tb.Text[i]))
                    {
                        tb.Text = tb.Text.Remove(i, 1);
                    }
                    else
                    {
                        i++;
                    }
                }

                if (tb.Text == string.Empty)
                {
                    tb.Text = "0";
                }

                tb.SelectionStart = 0;
                tb.SelectionLength = tb.Text.Length;
            }
        }

        /// <summary>
        /// The make digits only.
        /// </summary>
        /// <param name="tb">The tb.</param>
        /// <returns>The make digits only.</returns>
        /// <remarks></remarks>
        private float makeDigitsOnly(ToolStripTextBox tb)
        {
            try
            {
                float.Parse(tb.Text);
            }
            catch
            {
                int i = 0;
                while (i < tb.Text.Length)
                {
                    if (!char.IsDigit(tb.Text[i]) && !char.IsPunctuation(tb.Text[i]))
                    {
                        tb.Text = tb.Text.Remove(i, 1);
                    }
                    else
                    {
                        i++;
                    }
                }

                if (tb.Text == string.Empty)
                {
                    tb.Text = "0";
                }

                tb.SelectionStart = 0;
                tb.SelectionLength = tb.Text.Length;
            }

            return float.Parse(tb.Text);
        }

        /// <summary>
        /// The mi 4_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void mi4_Click(object sender, EventArgs e)
        {
            if (SelectedSpawn.Count != 1)
            {
                MessageBox.Show("Select One Object Or Spawn");
            }
            else
            {
                for (int i = 0; i < bsp.Spawns.Spawn.Count; i++)
                {
                    if (((int)bsp.Spawns.Spawn[i].Type & visibleSpawnsBitMask) == 0)
                    {
                        continue;
                    }

                    bsp.Spawns.Spawn[i].X = bsp.Spawns.Spawn[SelectedSpawn[0]].X;
                    bsp.Spawns.Spawn[i].Y = bsp.Spawns.Spawn[SelectedSpawn[0]].Y;
                    bsp.Spawns.Spawn[i].Z = bsp.Spawns.Spawn[SelectedSpawn[0]].Z;
                    TranslationMatrix[i] = MakeMatrixForSpawn(i);
                }
            }
        }

        /// <summary>
        /// The radio button 1_ checked changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                checkBox2.Text = "Colorize";
                checkBox2.Checked = false;
                trackBar1.Value = 0;
                trackBar2.Value = 0;
                trackBar3.Value = 0;
                Array.ConstrainedCopy(LightMap_Array_Backup, 0, LightMap_Array, 0, LightMap_Array.Length);

                foreach (PictureBox picbox in panel1.Controls)
                {
                    picbox.Image = RenderLightmap((int)((picbox).Tag));
                }
            }
            else
            {
                checkBox2.Text = "Preserve Lightness";
                checkBox2.Checked = true;
                trackBar1.Value = 0;
                trackBar2.Value = 0;
                trackBar3.Value = 0;
                Array.ConstrainedCopy(LightMap_Array_Backup, 0, LightMap_Array, 0, LightMap_Array.Length);

                foreach (PictureBox picbox in panel1.Controls)
                {
                    picbox.Image = RenderLightmap((int)((picbox).Tag));
                }
            }
        }

        /// <summary>
        /// The radio button 2_ checked changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                checkBox2.Text = "Colorize";
                checkBox2.Checked = false;
                trackBar1.Value = 0;
                trackBar2.Value = 0;
                trackBar3.Value = 0;
                Array.ConstrainedCopy(LightMap_Array_Backup, 0, LightMap_Array, 0, LightMap_Array.Length);

                foreach (PictureBox picbox in panel1.Controls)
                {
                    picbox.Image = RenderLightmap((int)((picbox).Tag));
                }

                EditLightmaps();
            }
            else
            {
                checkBox2.Text = "Preserve Lightness";
                checkBox2.Checked = true;
                trackBar1.Value = 0;
                trackBar2.Value = 0;
                trackBar3.Value = 0;
                Array.ConstrainedCopy(LightMap_Array_Backup, 0, LightMap_Array, 0, LightMap_Array.Length);

                foreach (PictureBox picbox in panel1.Controls)
                {
                    picbox.Image = RenderLightmap((int)((picbox).Tag));
                }

                EditLightmaps();
            }
        }

        /// <summary>
        /// The remove row.
        /// </summary>
        /// <param name="dgView">The dg view.</param>
        /// <param name="rowNumber">The row number.</param>
        /// <remarks></remarks>
        private void removeRow(ref DataGridView dgView, int rowNumber)
        {
            if (SelectedSpawn.Count == dgView.RowCount - 1)
            {
                SelectedSpawn.RemoveAt(dgView.CurrentCell.RowIndex);
            }
            else
            {
                // Safety in case something messed up...
                int number = int.Parse(dgView[1, dgView.CurrentCell.RowIndex].Value.ToString());
                for (int i = 0; i < SelectedSpawn.Count; i++)
                {
                    if (number == SelectedSpawn[i])
                    {
                        SelectedSpawn.RemoveAt(i);
                        break;
                    }
                }
            }

            dgView.Rows.RemoveAt(dgView.CurrentCell.RowIndex);
            if ((rowNumber > 0) && (rowNumber == dgView.RowCount - 1))
            {
                dgView.CurrentCell = dgView[0, rowNumber - 1];

                // = dgView.CurrentCell;
            }
        }

        /// <summary>
        /// The select spawn.
        /// </summary>
        /// <param name="tagNumber">The tag number.</param>
        /// <remarks></remarks>
        private void selectSpawn(int tagNumber)
        {
            // This section makes sure that the type we have selected is checked for viewing

            
            int index = checkedListBox1.FindString(bsp.Spawns.Spawn[tagNumber].Type.ToString(), 0);
            if (checkedListBox1.GetItemCheckState(index) == CheckState.Unchecked)
            {
                checkedListBox1.SetItemCheckState(index, CheckState.Checked);
            }

            

            #region TurnSpawnOnOrOff

            // Find out what section our tagNumber is in
            int tempi = SelectedSpawn.IndexOf(tagNumber);

            // If our spawn is already selected, deselect and move it to the last position
            if (tempi != -1)
            {
                SelectedSpawn.RemoveAt(tempi);
            }

            if (!bsp.Spawns.Spawn[tagNumber].frozen)
            {
                SelectedSpawn.Add(tagNumber);
            }

            // Update the global current selected spawn type
            selectedSpawnType = bsp.Spawns.Spawn[tagNumber].Type;

            #endregion
        }

        /// <summary>
        /// The select tool strip menu item_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void selectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                switch (((ToolStripMenuItem)sender).Tag.ToString())
                {
                    case "viewer":
                        selectToolStripMenuItem_ViewerClick(sender, e);
                        break;
                    default:
                        selectToolStripMenuItem_TreeClick(sender, e);
                        break;
                }
            }
            catch (Exception ex)
            {
                Global.ShowErrorMsg("Unkown location error", ex);
            }
        }

        /// <summary>
        /// The select tool strip menu item_ tree click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void selectToolStripMenuItem_TreeClick(object sender, EventArgs e)
        {
            TreeView c = identContext.SourceControl as TreeView;
            TreeNode cNode = c.SelectedNode;
            string s = ((ToolStripMenuItem)sender).Tag.ToString();
            string id = s.Substring(0, 3);
            int tagNumber = int.Parse(s.Substring(3));

            SpawnInfo.SpawnType spType;
            SpawnInfo.ObjectiveSpawn spObj;
            switch (id)
            {
                    // For a single spawn selection
                case "SEL":
                    selectSpawn(tagNumber);
                    break;

                    // For a single spawn selection
                case "DES":
                    if (tagNumber != -1)
                    {
                        SelectedSpawn.RemoveAt(SelectedSpawn.IndexOf(tagNumber));
                    }
                    else
                    {
                        SelectedSpawn.Clear();
                    }

                    break;

                    // For selecting all spawns of a certain Type
                case "ALL":
                    if (tagNumber == -1)
                    {
                        cNode = cNode.FirstNode;
                        if (cNode == null)
                        {
                            return;
                        }

                        tagNumber = int.Parse(cNode.Tag.ToString());
                    }

                    spType = bsp.Spawns.Spawn[tagNumber].Type;
                    for (int x = 0; x < bsp.Spawns.Spawn.Count; x++)
                    {
                        if (bsp.Spawns.Spawn[x].Type == spType)
                        {
                            selectSpawn(x);
                        }
                    }

                    break;

                    // For selecting Arming Circles
                case "ARM":
                    spType = bsp.Spawns.Spawn[tagNumber].Type;
                    spObj = (SpawnInfo.ObjectiveSpawn)bsp.Spawns.Spawn[tagNumber];
                    for (int x = 0; x < bsp.Spawns.Spawn.Count; x++)
                    {
                        if (bsp.Spawns.Spawn[x].Type == spType)
                        {
                            SpawnInfo.ObjectiveSpawn currObj = (SpawnInfo.ObjectiveSpawn)bsp.Spawns.Spawn[x];
                            if ((currObj.ObjectiveType == spObj.ObjectiveType) && (currObj.Team == spObj.Team))
                            {
                                selectSpawn(x);
                            }
                        }
                    }

                    break;

                    // For selecting King of the Hills
                case "HIL":
                    spType = bsp.Spawns.Spawn[tagNumber].Type;
                    spObj = (SpawnInfo.ObjectiveSpawn)bsp.Spawns.Spawn[tagNumber];
                    for (int x = 0; x < bsp.Spawns.Spawn.Count; x++)
                    {
                        if (bsp.Spawns.Spawn[x].Type == spType)
                        {
                            SpawnInfo.ObjectiveSpawn currObj = (SpawnInfo.ObjectiveSpawn)bsp.Spawns.Spawn[x];
                            if ((currObj.ObjectiveType == spObj.ObjectiveType) && (currObj.Team == spObj.Team))
                            {
                                selectSpawn(x);
                            }
                        }
                    }

                    break;
            }

            updateStatusPosition();
        }

        /// <summary>
        /// The select tool strip menu item_ viewer click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void selectToolStripMenuItem_ViewerClick(object sender, EventArgs e)
        {
            ToolStripDropDownItem current = (ToolStripDropDownItem)sender;
            if (current.Text.ToLower() == "unfreeze all")
            {
                for (int i = 0; i < bsp.Spawns.Spawn.Count; i++)
                {
                    bsp.Spawns.Spawn[i].frozen = false;
                }
            }
            else if (current.Text.ToLower().Contains("freeze all"))
            {
                for (int i = 0; i < bsp.Spawns.Spawn.Count; i++)
                {
                    if (SelectedSpawn.IndexOf(i) == -1)
                    {
                        bsp.Spawns.Spawn[i].frozen = true;
                    }
                }
            }
            else if (current.Text.ToLower().Contains("freeze"))
            {
                int ss = SelectedSpawn.IndexOf(currentObject);
                if (ss != -1)
                {
                    SelectedSpawn.RemoveAt(ss);
                }

                bsp.Spawns.Spawn[currentObject].frozen = !bsp.Spawns.Spawn[currentObject].frozen;
            }
            else if (current.Text.ToLower().StartsWith("select group"))
            {
                SpawnInfo.ObjectiveSpawn objSpawn = (SpawnInfo.ObjectiveSpawn)bsp.Spawns.Spawn[currentObject];
                for (int i = 0; i < bsp.Spawns.Spawn.Count; i++)
                {
                    if (bsp.Spawns.Spawn[i].Type == SpawnInfo.SpawnType.Objective &&
                        ((SpawnInfo.ObjectiveSpawn)bsp.Spawns.Spawn[i]).ObjectiveType == objSpawn.ObjectiveType)
                    {
                        switch (objSpawn.ObjectiveType)
                        {
                            case SpawnInfo.ObjectiveSpawn.ObjectiveTypeEnum.KingOfTheHill_1:
                            case SpawnInfo.ObjectiveSpawn.ObjectiveTypeEnum.KingOfTheHill_2:
                            case SpawnInfo.ObjectiveSpawn.ObjectiveTypeEnum.KingOfTheHill_3:
                            case SpawnInfo.ObjectiveSpawn.ObjectiveTypeEnum.KingOfTheHill_4:
                            case SpawnInfo.ObjectiveSpawn.ObjectiveTypeEnum.KingOfTheHill_5:
                                selectSpawn(i);
                                break;
                            case SpawnInfo.ObjectiveSpawn.ObjectiveTypeEnum.ArmingCircle:
                                if (((SpawnInfo.ObjectiveSpawn)bsp.Spawns.Spawn[i]).Team == objSpawn.Team)
                                {
                                    selectSpawn(i);
                                }

                                break;
                            case SpawnInfo.ObjectiveSpawn.ObjectiveTypeEnum.OddballSpawn:
                            case SpawnInfo.ObjectiveSpawn.ObjectiveTypeEnum.Territory:
                                if (((SpawnInfo.ObjectiveSpawn)bsp.Spawns.Spawn[i]).number == objSpawn.number)
                                {
                                    selectSpawn(i);
                                }

                                break;
                        }
                    }
                }
            }
            else if (current.Text.ToLower() == "select ...")
            {
                selectSpawn(currentObject);
            }
            else if (current.Text.ToLower().StartsWith("select all"))
            {
                for (int i = 0; i < bsp.Spawns.Spawn.Count; i++)
                {
                    if (bsp.Spawns.Spawn[i].Type == bsp.Spawns.Spawn[currentObject].Type)
                    {
                        selectSpawn(i);
                    }
                }
            }
            else if (current.Text.ToLower().StartsWith("deselect all"))
            {
                SelectedSpawn.Clear();
            }
            else if (current.Text.ToLower().StartsWith("deselect"))
            {
                SelectedSpawn.Clear();
            }

            /*
            string id = s.Substring(0, 3);
            int tagNumber = int.Parse(s.Substring(3));

            Renderer.BSP_Renderer.SpawnInfo.SpawnType spType;
            Renderer.BSP_Renderer.SpawnInfo.ObjectiveSpawn spObj;
            switch (id)
            {
                // For a single spawn selection
                case "SEL":
                    selectSpawn(tagNumber);
                    break;

                // For a single spawn selection
                case "DES":
                    if (tagNumber != -1)
                        SelectedSpawn.RemoveAt(SelectedSpawn.IndexOf(tagNumber));
                    else
                        SelectedSpawn.Clear();
                    break;

                // For selecting all spawns of a certain Type
                case "ALL":
                    if (tagNumber == -1)
                    {
                        cNode = cNode.FirstNode;
                        if (cNode == null) { return; }
                        tagNumber = int.Parse(cNode.Tag.ToString());
                    }
                    spType = bsp.Spawns.Spawn[tagNumber].Type;
                    for (int x = 0; x < bsp.Spawns.Spawn.Count; x++)
                    {
                        if (bsp.Spawns.Spawn[x].Type == spType)
                            selectSpawn(x);
                    }
                    break;

                // For selecting Arming Circles
                case "ARM":
                    spType = bsp.Spawns.Spawn[tagNumber].Type;
                    spObj = (Renderer.BSP_Renderer.SpawnInfo.ObjectiveSpawn)bsp.Spawns.Spawn[tagNumber];
                    for (int x = 0; x < bsp.Spawns.Spawn.Count; x++)
                    {
                        if (bsp.Spawns.Spawn[x].Type == spType)
                        {
                            Renderer.BSP_Renderer.SpawnInfo.ObjectiveSpawn currObj = (Renderer.BSP_Renderer.SpawnInfo.ObjectiveSpawn)bsp.Spawns.Spawn[x];
                            if ((currObj.ObjectiveType == spObj.ObjectiveType) && (currObj.Team == spObj.Team))
                                selectSpawn(x);
                        }
                    }
                    break;

                // For selecting King of the Hills
                case "HIL":
                    spType = bsp.Spawns.Spawn[tagNumber].Type;
                    spObj = (Renderer.BSP_Renderer.SpawnInfo.ObjectiveSpawn)bsp.Spawns.Spawn[tagNumber];
                    for (int x = 0; x < bsp.Spawns.Spawn.Count; x++)
                    {
                        if (bsp.Spawns.Spawn[x].Type == spType)
                        {
                            Renderer.BSP_Renderer.SpawnInfo.ObjectiveSpawn currObj = (Renderer.BSP_Renderer.SpawnInfo.ObjectiveSpawn)bsp.Spawns.Spawn[x];
                            if ((currObj.ObjectiveType == spObj.ObjectiveType) && (currObj.Team == spObj.Team))
                                selectSpawn(x);
                        }
                    }
                    break;
            }
            */
            updateStatusPosition();
        }

        /// <summary>
        /// The set spawn box.
        /// </summary>
        /// <param name="checkeditemsname">The checkeditemsname.</param>
        /// <param name="state">The state.</param>
        /// <remarks></remarks>
        private void setSpawnBox(string checkeditemsname, CheckState state)
        {
            Type typ = typeof(SpawnInfo.SpawnType);
            SpawnInfo.SpawnType bm = (SpawnInfo.SpawnType)Enum.Parse(typ, checkeditemsname, true);

            switch (bm)
            {
                case SpawnInfo.SpawnType.Collection:
                    if (WeaponsList == null)
                    {
                        doInfo(bm.ToString());
                        spawns.LoadWeapons(ref WeaponsList);
                        statusStrip.Items.Clear();
                    }

                    break;
                case SpawnInfo.SpawnType.Objective:
                    if (!spawns.hillsLoaded)
                    {
                        doInfo(bm.ToString());
                        spawns.createHills();
                        statusStrip.Items.Clear();
                    }

                    break;
                case SpawnInfo.SpawnType.Obstacle:
                    if (ObstacleList == null)
                    {
                        doInfo(bm.ToString());
                        spawns.LoadObstacles(ref ObstacleList);
                        statusStrip.Items.Clear();
                    }

                    break;
                case SpawnInfo.SpawnType.Scenery:
                    if (SceneryList == null)
                    {
                        doInfo(bm.ToString());
                        spawns.LoadScenery(ref SceneryList);
                        statusStrip.Items.Clear();
                    }

                    break;
                case SpawnInfo.SpawnType.Sound:
                    if (SoundsList == null)
                    {
                        doInfo(bm.ToString());
                        spawns.LoadSoundScenery(ref SoundsList);
                        statusStrip.Items.Clear();
                    }

                    break;
            }

            if (state == CheckState.Checked)
            {
                visibleSpawnsBitMask |= (int)bm;
            }
            else
            {
                visibleSpawnsBitMask &= int.MaxValue - (int)bm;
            }
        }

        /// <summary>
        /// The speed bar_ key down.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void speedBar_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode.ToString())
            {
                case "Left":
                case "Right":
                case "Up":
                case "Down":
                case "PageUp":
                case "Next": // PageDown
                    e.SuppressKeyPress = true;
                    break;
                default:

                    // MessageBox.Show(e.KeyCode.ToString());
                    break;
            }
        }

        /// <summary>
        /// The speed bar_ scroll.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void speedBar_Scroll(object sender, EventArgs e)
        {
            if (speedBar.Value < 100)
            {
                cam.speed = speedBar.Value / 100.0F;
            }
            else
            {
                cam.speed = (speedBar.Value - 90) / 10.0F;
            }
        }

        /// <summary>
        /// The speed bar_ value changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void speedBar_ValueChanged(object sender, EventArgs e)
        {
            speedLabel.Text = ((float)((int)(cam.speed * 100)) / 100).ToString();
        }

        /// <summary>
        /// The tb_ text changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void tb_TextChanged(object sender, EventArgs e)
        {
            ToolStripTextBox tb = sender as ToolStripTextBox;

            try
            {
                if (short.Parse(tb.Text) < 0)
                {
                    tb.Text = "0";
                }
            }
            catch
            {
                return;
            }

            foreach (int i in SelectedSpawn)
            {
                if (bsp.Spawns.Spawn[i].Type == SpawnInfo.SpawnType.Objective)
                {
                    SpawnInfo.ObjectiveSpawn os = bsp.Spawns.Spawn[i] as SpawnInfo.ObjectiveSpawn;
                    os.number = short.Parse(tb.Text);
                    bsp.Spawns.Spawn[i] = os;
                }
            }

            this.Focus();
        }

        /// <summary>
        /// The text sound_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void textSound_Click(object sender, EventArgs e)
        {
            // We need this here so that when the program changes the box, it doesn't give it focus!
            if (!((ToolStripTextBox)sender).Focused)
            {
                return;
            }

            ToolStripTextBox cb = sender as ToolStripTextBox;
        }

        /// <summary>
        /// The text sound_ got focus.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void textSound_GotFocus(object sender, EventArgs e)
        {
            ToolStripTextBox cb = sender as ToolStripTextBox;
            for (int j = 0; j < this.SelectedSpawn.Count; j++)
            {
                SpawnInfo.SoundSpawn sp = bsp.Spawns.Spawn[SelectedSpawn[j]] as SpawnInfo.SoundSpawn;

                // Clear our old mesh and show a new mesh for the outer size
                BoundingBoxModel[SelectedSpawn[j]].Dispose();
                switch (sp.VolumeType)
                {
                    case 0:
                        if (sp.DistanceBoundsUpper < 30)
                        {
                            BoundingBoxModel[SelectedSpawn[j]] = Mesh.Sphere(
                                render.device, 
                                sp.DistanceBoundsUpper, 
                                10 + (int)sp.DistanceBoundsUpper, 
                                10 + (int)sp.DistanceBoundsUpper);
                        }
                        else
                        {
                            BoundingBoxModel[SelectedSpawn[j]] = Mesh.Sphere(
                                render.device, sp.DistanceBoundsUpper, 30, 30);
                        }

                        break;
                    case 1:
                        BoundingBoxModel[SelectedSpawn[j]] = Mesh.Cylinder(
                            render.device, 
                            sp.DistanceBoundsUpper, 
                            sp.DistanceBoundsUpper, 
                            sp.Height, 
                            10 + (int)sp.DistanceBoundsLower, 
                            10 + (int)sp.DistanceBoundsLower);
                        break;
                    default:
                        BoundingBoxModel[SelectedSpawn[j]] = Mesh.Cylinder(
                            render.device, 
                            sp.DistanceBoundsLower, 
                            sp.DistanceBoundsUpper, 
                            sp.Height, 
                            10 + (int)sp.DistanceBoundsLower, 
                            10 + (int)sp.DistanceBoundsLower);
                        break;
                }

                if (cb.SelectionLength != cb.Text.Length)
                {
                    cb.SelectionStart = 0;
                    cb.SelectionLength = cb.Text.Length;
                }
            }
        }

        /// <summary>
        /// The text sound_ lost focus.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void textSound_LostFocus(object sender, EventArgs e)
        {
            ToolStripTextBox cb = sender as ToolStripTextBox;

            // This makes it stay large even with mouse over viewer.... hmmm
            // if (statusStrip.Focused) { return; }

            for (int j = 0; j < this.SelectedSpawn.Count; j++)
            {
                SpawnInfo.SoundSpawn sp = bsp.Spawns.Spawn[SelectedSpawn[j]] as SpawnInfo.SoundSpawn;
                // Clear our old mesh and show a new mesh for the outer size
                BoundingBoxModel[SelectedSpawn[j]].Dispose();
                switch (sp.VolumeType)
                {
                    case 0:
                        if (sp.DistanceBoundsLower < 30)
                        {
                            BoundingBoxModel[SelectedSpawn[j]] = Mesh.Sphere(
                                render.device, 
                                sp.DistanceBoundsLower, 
                                10 + (int)sp.DistanceBoundsLower, 
                                10 + (int)sp.DistanceBoundsLower);
                        }
                        else
                        {
                            BoundingBoxModel[SelectedSpawn[j]] = Mesh.Sphere(
                                render.device, sp.DistanceBoundsLower, 30, 30);
                        }

                        break;
                    case 1:
                        BoundingBoxModel[SelectedSpawn[j]] = Mesh.Cylinder(
                            render.device, 
                            sp.DistanceBoundsUpper, 
                            sp.DistanceBoundsUpper, 
                            sp.Height, 
                            10 + (int)sp.DistanceBoundsLower, 
                            10 + (int)sp.DistanceBoundsLower);
                        break;
                    default:
                        BoundingBoxModel[SelectedSpawn[j]] = Mesh.Cylinder(
                            render.device, 
                            sp.DistanceBoundsLower, 
                            sp.DistanceBoundsUpper, 
                            sp.Height, 
                            10 + (int)sp.DistanceBoundsLower, 
                            10 + (int)sp.DistanceBoundsLower);
                        break;
                }
            }
        }

        /// <summary>
        /// The text sound_ lost mouse focus.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void textSound_LostMouseFocus(object sender, EventArgs e)
        {
            ToolStripTextBox cb = sender as ToolStripTextBox;
            if (!cb.Focused)
            {
                textSound_LostFocus(sender, e);
            }
        }

        /// <summary>
        /// The text sound_ text changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void textSound_TextChanged(object sender, EventArgs e)
        {
            // We need this here so that when the program changes the box, it doesn't give it focus!
            if (!((ToolStripTextBox)sender).Focused)
            {
                return;
            }

            ToolStripTextBox cb = sender as ToolStripTextBox;

            for (int j = 0; j < this.SelectedSpawn.Count; j++)
            {
                SpawnInfo.SoundSpawn sp = bsp.Spawns.Spawn[SelectedSpawn[j]] as SpawnInfo.SoundSpawn;

                // Clear our old mesh for the creation a new mesh of either size
                BoundingBoxModel[SelectedSpawn[j]].Dispose();
                if (cb.Name == "SoundSceneryInnerSize")
                {
                    // Update the value stored in the SoundScenery
                    sp.DistanceBoundsLower = makeDigitsOnly(cb);
                    switch (sp.VolumeType)
                    {
                        case 0:
                            if (sp.DistanceBoundsLower < 30)
                            {
                                BoundingBoxModel[SelectedSpawn[j]] = Mesh.Sphere(
                                    render.device, 
                                    sp.DistanceBoundsLower, 
                                    10 + (int)sp.DistanceBoundsLower, 
                                    10 + (int)sp.DistanceBoundsLower);
                            }
                            else
                            {
                                BoundingBoxModel[SelectedSpawn[j]] = Mesh.Sphere(
                                    render.device, sp.DistanceBoundsLower, 30, 30);
                            }

                            break;
                        case 1:
                            BoundingBoxModel[SelectedSpawn[j]] = Mesh.Cylinder(
                                render.device, 
                                sp.DistanceBoundsLower, 
                                sp.DistanceBoundsUpper, 
                                sp.Height, 
                                10 + (int)sp.DistanceBoundsLower, 
                                10 + (int)sp.DistanceBoundsLower);
                            break;
                        default:
                            BoundingBoxModel[SelectedSpawn[j]] = Mesh.Cylinder(
                                render.device, 
                                sp.DistanceBoundsLower, 
                                sp.DistanceBoundsUpper, 
                                sp.Height, 
                                10 + (int)sp.DistanceBoundsLower, 
                                10 + (int)sp.DistanceBoundsLower);
                            break;
                    }
                }
                else if (cb.Name == "SoundSceneryOuterSize")
                {
                    sp.DistanceBoundsUpper = makeDigitsOnly(cb);
                    switch (sp.VolumeType)
                    {
                        case 0:
                            if (sp.DistanceBoundsUpper < 30)
                            {
                                BoundingBoxModel[SelectedSpawn[j]] = Mesh.Sphere(
                                    render.device, 
                                    sp.DistanceBoundsUpper, 
                                    10 + (int)sp.DistanceBoundsUpper, 
                                    10 + (int)sp.DistanceBoundsUpper);
                            }
                            else
                            {
                                BoundingBoxModel[SelectedSpawn[j]] = Mesh.Sphere(
                                    render.device, sp.DistanceBoundsUpper, 30, 30);
                            }

                            break;
                        case 1:
                            BoundingBoxModel[SelectedSpawn[j]] = Mesh.Cylinder(
                                render.device, 
                                sp.DistanceBoundsLower, 
                                sp.DistanceBoundsUpper, 
                                sp.Height, 
                                10 + (int)sp.DistanceBoundsLower, 
                                10 + (int)sp.DistanceBoundsLower);
                            break;
                        default:
                            BoundingBoxModel[SelectedSpawn[j]] = Mesh.Cylinder(
                                render.device, 
                                sp.DistanceBoundsLower, 
                                sp.DistanceBoundsUpper, 
                                sp.Height, 
                                10 + (int)sp.DistanceBoundsLower, 
                                10 + (int)sp.DistanceBoundsLower);
                            break;
                    }
                }

                // Used for updating tree display nodes
                for (int a = 0; a < treeView1.Nodes.Count; a++)
                {
                    for (int aa = 0; aa < treeView1.Nodes[a].Nodes.Count; aa++)
                    {
                        if (treeView1.Nodes[a].Nodes[aa].Tag.ToString() == SelectedSpawn[j].ToString())
                        {
                            // MessageBox.Show("Node Updates");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The tool strip button reset_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void toolStripButtonReset_Click(object sender, EventArgs e)
        {
            foreach (int i in SelectedSpawn)
            {
                SpawnInfo.BaseSpawn spawn = bsp.Spawns.Spawn[i];
                if (spawn is SpawnInfo.RotateYawPitchRollBaseSpawn)
                {
                    ((SpawnInfo.RotateYawPitchRollBaseSpawn)spawn).Yaw = 0;
                    ((SpawnInfo.RotateYawPitchRollBaseSpawn)spawn).Pitch = 0;
                    ((SpawnInfo.RotateYawPitchRollBaseSpawn)spawn).Roll = 0;
                }
                else if (spawn is SpawnInfo.RotateDirectionBaseSpawn)
                {
                    ((SpawnInfo.RotateDirectionBaseSpawn)spawn).RotationDirection = 0;
                }
            }
        }

        /// <summary>
        /// The track bar 1_ scroll.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            trackint1 = trackBar1.Value;
            EditLightmaps();
        }

        /// <summary>
        /// The track bar 2_ scroll.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            trackint2 = trackBar2.Value;
            EditLightmaps();
        }

        /// <summary>
        /// The track bar 3_ scroll.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            trackint3 = trackBar3.Value;
            EditLightmaps();
        }

        // Makes right clicks select node under cursofr as well
        /// <summary>
        /// The tree view 1_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void treeView1_Click(object sender, EventArgs e)
        {
            MouseEventArgs me = e as MouseEventArgs;
            if (me.Button == MouseButtons.Right)
            {
                TreeNode c = treeView1.GetNodeAt(me.Location);
                treeView1.SelectedNode = c;
            }
        }

        /// <summary>
        /// The tree view 1_ double click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void treeView1_DoubleClick(object sender, EventArgs e)
        {
            int tempint = (int)((TreeView)sender).SelectedNode.Tag;
            if (tempint >= 0)
            {
                setCameraPosition(
                    bsp.Spawns.Spawn[tempint].X, bsp.Spawns.Spawn[tempint].Y, bsp.Spawns.Spawn[tempint].Z, false);

                

                checkedListBox1.SetItemCheckState(
                    checkedListBox1.FindString(bsp.Spawns.Spawn[tempint].Type.ToString(), 0), CheckState.Checked);

                

                #region AutoSelect

                if (checkBox4.Checked)
                {
                    #region ClearStatus

                    statusStrip.Items.Clear();

                    // SelectedSpawn.Clear();
                    #endregion

                    #region DisplayCameraPosition

                    // Set camera postion
                    string tempstring = toolStripLabel2.Text;
                    string tempstring2 = "Camera Position: X: " + cam.x.ToString().PadRight(10) + " � Y: " +
                                         cam.y.ToString().PadRight(10) + " � Z: " + cam.z.ToString().PadRight(10);
                    if (tempstring != tempstring2)
                    {
                        toolStripLabel2.Text = tempstring2;
                        statusStrip.ResumeLayout();
                        statusStrip.SuspendLayout();
                    }

                    #endregion

                    #region Turn Spawn On And Move To End Of List

                    int tempi = SelectedSpawn.IndexOf(tempint);

                    if (tempi != -1)
                    {
                        SelectedSpawn.RemoveAt(tempi);
                    }

                    SelectedSpawn.Add(tempint);

                    selectedSpawnType = bsp.Spawns.Spawn[tempint].Type;

                    #endregion

                    updateStatusPosition();
                }

                #endregion

                Application.DoEvents();
                this.Focus();
                Application.DoEvents();
            }
        }

        /// <summary>
        /// The ts button_ click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void tsButton_Click(object sender, EventArgs e)
        {
            ToolStripButton tb = (ToolStripButton)sender;

            Form listForm = new Form();

            listForm.FormBorderStyle = FormBorderStyle.Sizable;

            // listForm.Size = new Size(620, 450);
            listForm.StartPosition = FormStartPosition.Manual;
            int tempHeight = SelectedSpawn.Count * 18 + 90;
            if (tempHeight < (this.Height * 90 / 100))
            {
                listForm.Size = new Size(this.Width * 65 / 100, tempHeight);
            }
            else
            {
                listForm.Size = new Size(this.Width * 65 / 100, this.Height * 90 / 100);
            }

            listForm.Location = new Point(10, this.Height - (listForm.Size.Height + 35));
            listForm.Text = "Currently Selected Spawns";
            listForm.Resize += this.listForm_resize;

            #region DataGrid Data

            DataGridView dataGrid = new DataGridView();
            dataGrid.AllowUserToResizeRows = false;
            dataGrid.ColumnCount = 6;
            dataGrid.ReadOnly = true;
            dataGrid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dataGrid.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            dataGrid.GridColor = Color.Black;
            dataGrid.RowHeadersVisible = false;
            dataGrid.Columns[0].Name = "Type";
            dataGrid.Columns[1].Name = "SpawnNumber"; // Invisible
            dataGrid.Columns[2].Name = "Tag";
            dataGrid.Columns[3].Name = "X";
            dataGrid.Columns[4].Name = "Y";
            dataGrid.Columns[5].Name = "Z";
            dataGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            dataGrid.Location = new Point(10, 10);
            dataGrid.Name = "dataGrid";
            dataGrid.ShowCellToolTips = false;
            dataGrid.Size = new Size(listForm.Width - 20, listForm.Height - 60);
            dataGrid.TabIndex = 0;
            dataGrid.CellClick += dataGrid_CellClick;
            dataGrid.RowEnter += this.dataGridRow_Select;
            dataGrid.RowLeave += this.dataGridRow_LostFocus;
            dataGrid.Resize += this.dataGrid_resize;

            #region Add a button column.

            DataGridViewButtonColumn buttonColumn = new DataGridViewButtonColumn();
            buttonColumn.HeaderText = "Remove";
            buttonColumn.Name = "Remove";
            buttonColumn.Text = "x";
            buttonColumn.UseColumnTextForButtonValue = true;

            // buttonColumn.
            dataGrid.Columns.Add(buttonColumn);

            #endregion

            dataGrid.Columns[1].Visible = false;

            #endregion

            #region Selected Spawn Data

            // dataGrid.Items.Count = SelectedSpawn.Count;
            for (int i = 0; i < SelectedSpawn.Count; i++)
            {
                SpawnInfo.BaseSpawn thisSpawn = bsp.Spawns.Spawn[SelectedSpawn[i]];
                string extraInfo = thisSpawn.TagPath;
                if (thisSpawn is SpawnInfo.DeathZone)
                {
                    extraInfo = ((SpawnInfo.DeathZone)thisSpawn).Name;
                }
                else if (thisSpawn is SpawnInfo.ObjectiveSpawn)
                {
                    SpawnInfo.ObjectiveSpawn tempspawn = (SpawnInfo.ObjectiveSpawn)thisSpawn;
                    extraInfo = tempspawn.ObjectiveType + " (" + tempspawn.Team + ") #" + tempspawn.number;
                }
                else if (thisSpawn is SpawnInfo.PlayerSpawn)
                {
                    extraInfo = thisSpawn.Type + " Spawn";
                }

                dataGrid.Rows.Add(
                    new object[]
                        {
                            thisSpawn.Type.ToString(), SelectedSpawn[i].ToString(), extraInfo, thisSpawn.X.ToString(), 
                            thisSpawn.Y.ToString(), thisSpawn.Z.ToString()
                        });
                dataGrid.Rows[i].Height = 18;
            }

            #endregion

            #region Add Controls

            listForm.Controls.Add(dataGrid);
            listForm_resize(listForm, e);
            listForm.ShowDialog();

            #endregion
        }

        /// <summary>
        /// The ts text box_ change.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void tsTextBox_Change(object sender, EventArgs e)
        {
            ToolStripTextBox tb = (ToolStripTextBox)sender;
            if (tb.Focused == false)
            {
                return;
            }

            // In case all digits are deleted, must contain at least one digit!!!
            int lastSelectedSpawn = SelectedSpawn[SelectedSpawn.Count - 1];

            // Removes any letters and makes blanks zero
            makeDigitsOnly(ref tb);

            // get the difference in location between the old & new last spawn position
            float diffX = float.Parse(tsTextBoxX.Text) - bsp.Spawns.Spawn[lastSelectedSpawn].X;
            float diffY = float.Parse(tsTextBoxY.Text) - bsp.Spawns.Spawn[lastSelectedSpawn].Y;
            float diffZ = float.Parse(tsTextBoxZ.Text) - bsp.Spawns.Spawn[lastSelectedSpawn].Z;
            float diffYaw = 0;
            float diffPitch = 0;
            float diffRoll = 0;
            if (bsp.Spawns.Spawn[lastSelectedSpawn] is SpawnInfo.RotateYawPitchRollBaseSpawn)
            {
                SpawnInfo.RotateYawPitchRollBaseSpawn YPRSpawn =
                    bsp.Spawns.Spawn[lastSelectedSpawn] as SpawnInfo.RotateYawPitchRollBaseSpawn;
                diffYaw = float.Parse(tsTextBoxYaw.Text) - YPRSpawn.Yaw;
                diffPitch = float.Parse(tsTextBoxPitch.Text) - YPRSpawn.Pitch;
                diffRoll = float.Parse(tsTextBoxRoll.Text) - YPRSpawn.Roll;

                // Update the last selected spawn with the new location
                YPRSpawn.Yaw = float.Parse(tsTextBoxYaw.Text);
                YPRSpawn.Pitch = float.Parse(tsTextBoxPitch.Text);
                YPRSpawn.Roll = float.Parse(tsTextBoxRoll.Text);
            }
            else if (bsp.Spawns.Spawn[lastSelectedSpawn] is SpawnInfo.RotateDirectionBaseSpawn)
            {
                SpawnInfo.RotateDirectionBaseSpawn YPRSpawn =
                    bsp.Spawns.Spawn[lastSelectedSpawn] as SpawnInfo.RotateDirectionBaseSpawn;
                diffYaw = float.Parse(tsTextBoxYaw.Text) - YPRSpawn.RotationDirection;

                // Update the last selected spawn with the new location
                YPRSpawn.RotationDirection = float.Parse(tsTextBoxYaw.Text);
            }

            // Update the last selected spawn with the new location
            bsp.Spawns.Spawn[lastSelectedSpawn].X = float.Parse(tsTextBoxX.Text);
            bsp.Spawns.Spawn[lastSelectedSpawn].Y = float.Parse(tsTextBoxY.Text);
            bsp.Spawns.Spawn[lastSelectedSpawn].Z = float.Parse(tsTextBoxZ.Text);

            TranslationMatrix[lastSelectedSpawn] = MakeMatrixForSpawn(lastSelectedSpawn);

            // Move all other selected spawn the same distance as the last spawn moved
            for (int i = 0; i < SelectedSpawn.Count - 1; i++)
            {
                bsp.Spawns.Spawn[SelectedSpawn[i]].X += diffX;
                bsp.Spawns.Spawn[SelectedSpawn[i]].Y += diffY;
                bsp.Spawns.Spawn[SelectedSpawn[i]].Z += diffZ;

                if (bsp.Spawns.Spawn[SelectedSpawn[i]] is SpawnInfo.RotateYawPitchRollBaseSpawn)
                {
                    SpawnInfo.RotateYawPitchRollBaseSpawn YPRSpawn =
                        bsp.Spawns.Spawn[SelectedSpawn[i]] as SpawnInfo.RotateYawPitchRollBaseSpawn;

                    // Update all selected spawns with the new location
                    YPRSpawn.Yaw = float.Parse(tsTextBoxYaw.Text);
                    YPRSpawn.Pitch = float.Parse(tsTextBoxPitch.Text);
                    YPRSpawn.Roll = float.Parse(tsTextBoxRoll.Text);
                }
                else if (bsp.Spawns.Spawn[lastSelectedSpawn] is SpawnInfo.RotateDirectionBaseSpawn)
                {
                    SpawnInfo.RotateDirectionBaseSpawn YPRSpawn =
                        bsp.Spawns.Spawn[SelectedSpawn[i]] as SpawnInfo.RotateDirectionBaseSpawn;

                    // Update all selected spawns with the new location
                    YPRSpawn.RotationDirection = float.Parse(tsTextBoxYaw.Text);
                }

                TranslationMatrix[SelectedSpawn[i]] = MakeMatrixForSpawn(SelectedSpawn[i]);
            }

            updateXYZYPR = false;
            Point pt = tb.GetPositionFromCharIndex(tb.Text.Length - 1);
            if (pt.X < 45)
            {
                pt.X = 45;
            }

            tb.Width = pt.X + 15;
        }

        /// <summary>
        /// The ts text box_ got focus.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void tsTextBox_GotFocus(object sender, EventArgs e)
        {
            ToolStripTextBox tb = (ToolStripTextBox)sender;
            tb.Width = 80;
            tb.SelectionStart = 0;
            tb.SelectionLength = 0;
            Point pt = tb.GetPositionFromCharIndex(tb.Text.Length - 1);
            if (pt.X < 45)
            {
                pt.X = 45;
            }

            tb.Width = pt.X + 15;
        }

        /// <summary>
        /// The ts text box_ lost focus.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        private void tsTextBox_LostFocus(object sender, EventArgs e)
        {
            ToolStripTextBox tb = (ToolStripTextBox)sender;
            updateXYZYPR = true;
            tb.Width = 45;
            tb.SelectionStart = 0;
            tb.SelectionLength = 0;

            /*
            int lastSelectedSpawn = SelectedSpawn[SelectedSpawn.Count - 1];

            // get the difference in location between the old & new last spawn position
            float diffX = float.Parse(tsTextBoxX.Text) - bsp.Spawns.Spawn[lastSelectedSpawn].X;
            float diffY = float.Parse(tsTextBoxY.Text) - bsp.Spawns.Spawn[lastSelectedSpawn].Y;
            float diffZ = float.Parse(tsTextBoxZ.Text) - bsp.Spawns.Spawn[lastSelectedSpawn].Z;

            // Update the last selected spawn with the new location
            bsp.Spawns.Spawn[lastSelectedSpawn].X = float.Parse(tsTextBoxX.Text);
            bsp.Spawns.Spawn[lastSelectedSpawn].Y = float.Parse(tsTextBoxY.Text);
            bsp.Spawns.Spawn[lastSelectedSpawn].Z = float.Parse(tsTextBoxZ.Text);

            // Move all other selected spawn the same distance as the last spawn moved
            for (int i = 0; i < SelectedSpawn.Count-1; i++)
            {
                bsp.Spawns.Spawn[SelectedSpawn[i]].X += diffX;
                bsp.Spawns.Spawn[SelectedSpawn[i]].Y += diffY;
                bsp.Spawns.Spawn[SelectedSpawn[i]].Z += diffZ;
            }
            */
        }

        /// <summary>
        /// The update status position.
        /// </summary>
        /// <remarks></remarks>
        private void updateStatusPosition()
        {
            if (SelectedSpawn.Count > 0)
            {
                toolStrip.Visible = true;
                bool showCollection = true, 
                     showObjective = true, 
                     showObstacles = true, 
                     showScenery = true, 
                     showSounds = true;

                // bool isOther = false;
                for (int count = 0; count < SelectedSpawn.Count; count++)
                {
                    if (bsp.Spawns.Spawn[SelectedSpawn[count]] is SpawnInfo.Collection)
                    {
                        showObjective = false;
                        showObstacles = false;
                        showScenery = false;
                        showSounds = false;
                    }
                    else if (bsp.Spawns.Spawn[SelectedSpawn[count]] is SpawnInfo.ObjectiveSpawn)
                    {
                        showCollection = false;
                        showObstacles = false;
                        showScenery = false;
                        showSounds = false;
                    }
                    else if (bsp.Spawns.Spawn[SelectedSpawn[count]] is SpawnInfo.ScenerySpawn)
                    {
                        showCollection = false;
                        showObjective = false;
                        showObstacles = false;
                        showSounds = false;
                    }
                    else if (bsp.Spawns.Spawn[SelectedSpawn[count]] is SpawnInfo.ObstacleSpawn)
                    {
                        showCollection = false;
                        showObjective = false;
                        showScenery = false;
                        showSounds = false;
                    }
                    else if (bsp.Spawns.Spawn[SelectedSpawn[count]] is SpawnInfo.SoundSpawn)
                    {
                        showCollection = false;
                        showObjective = false;
                        showObstacles = false;
                        showScenery = false;
                    }
                    else
                    {
                        showCollection = false;
                        showObjective = false;
                        showObstacles = false;
                        showScenery = false;
                        showSounds = false;
                    }
                }

                int lastSelectedSpawn = SelectedSpawn[SelectedSpawn.Count - 1];
                string itemType = bsp.Spawns.Spawn[lastSelectedSpawn].Type.ToString();
                for (int i = 0; i < SelectedSpawn.Count - 1; i++)
                {
                    string temp = bsp.Spawns.Spawn[SelectedSpawn[i]].Type.ToString();
                    if (itemType != temp)
                    {
                        itemType = "mixed";
                        statusStrip.Items.Clear();
                    }
                }

                // tsLabel1.Text = "Type: <";
                tsButtonType.Text = itemType;

                // tsLabel2.Text = "> (";
                tsLabelCount.Text = SelectedSpawn.Count.ToString();

                // Stop it from overwriting text in boxes when user is trying to change it!
                if (updateXYZYPR)
                {
                    tsLabelX.Text = ") � X: ";
                    tsTextBoxX.Text = bsp.Spawns.Spawn[lastSelectedSpawn].X.ToString("#0.0000####");
                    tsLabelY.Text = " � Y: ";
                    tsTextBoxY.Text = bsp.Spawns.Spawn[lastSelectedSpawn].Y.ToString("#0.0000####");
                    tsLabelZ.Text = " � Z: ";
                    tsTextBoxZ.Text = bsp.Spawns.Spawn[lastSelectedSpawn].Z.ToString("#0.0000####");
                }

                int rot = 0;

                #region YawPitchRollInfo

                if (bsp.Spawns.Spawn[lastSelectedSpawn] is SpawnInfo.RotateYawPitchRollBaseSpawn)
                {
                    rot = 2;

                    // Stop it from overwriting text in boxes when user is trying to change it!
                    if (updateXYZYPR)
                    {
                        SpawnInfo.RotateYawPitchRollBaseSpawn temprot;
                        temprot = bsp.Spawns.Spawn[lastSelectedSpawn] as SpawnInfo.RotateYawPitchRollBaseSpawn;
                        tsLabelYaw.Text = " � Yaw:";
                        tsTextBoxYaw.Text = temprot.Yaw.ToString("#0.0000####");
                        tsLabelPitch.Text = " � Pitch:";
                        tsTextBoxPitch.Text = temprot.Pitch.ToString("#0.0000####");
                        tsLabelRoll.Text = " � Roll:";
                        tsTextBoxRoll.Text = temprot.Roll.ToString("#0.0000####");
                    }
                }
                else if (bsp.Spawns.Spawn[lastSelectedSpawn] is SpawnInfo.RotateDirectionBaseSpawn)
                {
                    if (updateXYZYPR)
                    {
                        SpawnInfo.RotateDirectionBaseSpawn temprot;
                        temprot = bsp.Spawns.Spawn[lastSelectedSpawn] as SpawnInfo.RotateDirectionBaseSpawn;

                        // If multiple selections are made and at least one contains YawPitchRoll, show it
                        SpawnInfo.RotateYawPitchRollBaseSpawn temprot2;
                        for (int x = SelectedSpawn.Count - 1; x >= 0; x--)
                        {
                            if (bsp.Spawns.Spawn[SelectedSpawn[x]] is SpawnInfo.RotateYawPitchRollBaseSpawn)
                            {
                                rot = 2;
                                temprot2 = bsp.Spawns.Spawn[SelectedSpawn[x]] as SpawnInfo.RotateYawPitchRollBaseSpawn;
                                tsLabelYaw.Text = " � Yaw:";
                                tsTextBoxYaw.Text = temprot.RotationDirection.ToString("#0.0000####");
                                tsLabelPitch.Text = " � Pitch:";
                                tsTextBoxPitch.Text = temprot2.Pitch.ToString("#0.0000####");
                                tsLabelRoll.Text = " � Roll:";
                                tsTextBoxRoll.Text = temprot2.Roll.ToString("#0.0000####");
                                break;
                            }
                        }

                        // ...otherwise just show rotation
                        rot = 1;
                        tsLabelYaw.Text = " � Yaw:";
                        tsTextBoxYaw.Text = temprot.RotationDirection.ToString("#0.0000####");
                        tsLabelPitch.Text = string.Empty;
                        tsLabelRoll.Text = string.Empty;
                    }
                }

                #endregion

                // statusStrip.Items.Clear();
                #region StatusStripIndividualDisplays

                #region CollectionObjectsOnly

                if (showCollection)
                {
                    // Selects Last Spawn Clicked
                    SpawnInfo.Collection os;
                    os = bsp.Spawns.Spawn[lastSelectedSpawn] as SpawnInfo.Collection;

                    ToolStripComboBox combo;
                    Array testx;
                    int tempindex;

                    #region CollectionSpawnComboBox

                    if (statusStrip.Items.IndexOfKey("CollectionSpawn") == -1)
                    {
                        combo = new ToolStripComboBox();
                        testx = Enum.GetNames(typeof(SpawnInfo.Collection.SpawnsInEnum));

                        // Add all the Collection options
                        for (int y = 0; y < testx.Length; y++)
                        {
                            combo.Items.Add(testx.GetValue(y).ToString());
                        }

                        combo.SelectedIndexChanged += combo_SelectedIndexChangedCollection;
                        combo.BackColor = Color.Gray;
                        combo.FlatStyle = FlatStyle.Flat;
                        combo.Name = "CollectionSpawn";
                        combo.DropDownStyle = ComboBoxStyle.DropDownList;
                        statusStrip.Items.Add(combo);
                    }

                    #endregion

                    #region CollectionModelComboBox

                    if (statusStrip.Items.IndexOfKey("CollectionModel") == -1)
                    {
                        combo = new ToolStripComboBox();

                        for (int y = 0; y < WeaponsList.Count; y++)
                        {
                            combo.Items.Add(WeaponsList[y].Name);
                        }

                        combo.SelectedIndexChanged += combo_SelectedIndexChangedCollectionModel;
                        combo.BackColor = Color.Gray;
                        combo.DropDownStyle = ComboBoxStyle.DropDownList;
                        combo.FlatStyle = FlatStyle.Flat;
                        combo.Name = "CollectionModel";
                        combo.Sorted = true;
                        statusStrip.Items.Add(combo);
                    }

                    #endregion

                    if (os != null)
                    {
                        // Update Spawn Combo box
                        int collNumber = statusStrip.Items.IndexOfKey("CollectionSpawn");
                        ToolStripComboBox cbCollSpawn = (ToolStripComboBox)statusStrip.Items[collNumber];
                        tempindex = cbCollSpawn.Items.IndexOf(os.SpawnsInMode.ToString());

                        cbCollSpawn.SelectedIndex = tempindex;

                        // If not all selected Collections are the same Spawn Type, blank the box out
                        for (int x = 0; x < SelectedSpawn.Count; x++)
                        {
                            if (((SpawnInfo.Collection)bsp.Spawns.Spawn[SelectedSpawn[x]]).SpawnsInMode !=
                                os.SpawnsInMode)
                            {
                                cbCollSpawn.SelectedIndex = -1;
                            }
                        }

                        // Update Collection Model Type Box
                        string[] tempOS = os.TagPath.Split('\\');
                        int collModel = statusStrip.Items.IndexOfKey("CollectionModel");
                        ToolStripComboBox cbCollModel = (ToolStripComboBox)statusStrip.Items[collModel];
                        tempindex = cbCollModel.Items.IndexOf(tempOS[tempOS.Length - 1].Replace('_', ' '));

                        cbCollModel.SelectedIndex = tempindex;

                        // If not all selected Collections are the same Models, blank the box out
                        for (int x = 0; x < SelectedSpawn.Count; x++)
                        {
                            if (bsp.Spawns.Spawn[SelectedSpawn[x]].TagPath != os.TagPath)
                            {
                                cbCollModel.SelectedIndex = -1;
                            }
                        }
                    }
                }

                #endregion Collection

                #region ObjectiveObjectsOnly

                if (showObjective)
                {
                    // Selects Last Spawn Clicked
                    SpawnInfo.ObjectiveSpawn os;
                    os = bsp.Spawns.Spawn[lastSelectedSpawn] as SpawnInfo.ObjectiveSpawn;

                    ToolStripComboBox comboSpawn;
                    ToolStripComboBox comboTeam;
                    Array testx;
                    int tempindex;

                    #region ObjectiveTypeComboBox

                    if (statusStrip.Items.IndexOfKey("ObjectiveSpawn") == -1)
                    {
                        // Add Objective Types to first combo box
                        comboSpawn = new ToolStripComboBox();
                        testx = Enum.GetNames(typeof(SpawnInfo.ObjectiveSpawn.ObjectiveTypeEnum));

                        for (int y = 0; y < testx.Length; y++)
                        {
                            comboSpawn.Items.Add(testx.GetValue(y).ToString());
                        }

                        comboSpawn.SelectedIndexChanged += combo_SelectedIndexChangedObjective;

                        comboSpawn.BackColor = Color.Gray;
                        comboSpawn.FlatStyle = FlatStyle.Flat;
                        comboSpawn.DropDownStyle = ComboBoxStyle.DropDownList;
                        comboSpawn.Name = "ObjectiveSpawn";
                        statusStrip.Items.Add(comboSpawn);
                    }

                    #endregion

                    #region ObjectiveTeamComboBox

                    if (statusStrip.Items.IndexOfKey("ObjectiveTeam") == -1)
                    {
                        // Add Objective Teams to second combo box
                        comboTeam = new ToolStripComboBox();
                        testx = Enum.GetNames(typeof(SpawnInfo.ObjectiveSpawn.TeamType));

                        for (int y = 0; y < testx.Length; y++)
                        {
                            comboTeam.Items.Add(testx.GetValue(y).ToString());
                        }

                        comboTeam.SelectedIndexChanged += combo_SelectedIndexChangedTeam;
                        comboTeam.BackColor = Color.Gray;
                        comboTeam.FlatStyle = FlatStyle.Flat;
                        comboTeam.DropDownStyle = ComboBoxStyle.DropDownList;
                        comboTeam.Name = "ObjectiveTeam";
                        statusStrip.Items.Add(comboTeam);
                    }

                    #endregion

                    if (os != null)
                    {
                        // Update Spawn Combo box
                        int objNumber = statusStrip.Items.IndexOfKey("ObjectiveSpawn");
                        ToolStripComboBox cbObjSpawn = (ToolStripComboBox)statusStrip.Items[objNumber];
                        tempindex = cbObjSpawn.Items.IndexOf(os.ObjectiveType.ToString());
                        if (tempindex != -1)
                        {
                            cbObjSpawn.SelectedIndex = tempindex;

                            // If not all selected Objectives are the same type, blank the box out
                            for (int x = 0; x < SelectedSpawn.Count; x++)
                            {
                                if (((SpawnInfo.ObjectiveSpawn)bsp.Spawns.Spawn[SelectedSpawn[x]]).ObjectiveType !=
                                    os.ObjectiveType)
                                {
                                    cbObjSpawn.SelectedIndex = -1;
                                }
                            }
                        }

                        // Update Team Combo box
                        objNumber = statusStrip.Items.IndexOfKey("ObjectiveTeam");
                        ToolStripComboBox cbObjTeam = (ToolStripComboBox)statusStrip.Items[objNumber];
                        tempindex = cbObjTeam.Items.IndexOf(os.Team.ToString());
                        if (tempindex != -1)
                        {
                            cbObjTeam.SelectedIndex = tempindex;

                            // If not all selected Objectives are the same Team, blank the box out
                            for (int x = 0; x < SelectedSpawn.Count; x++)
                            {
                                if (((SpawnInfo.ObjectiveSpawn)bsp.Spawns.Spawn[SelectedSpawn[x]]).Team != os.Team)
                                {
                                    cbObjTeam.SelectedIndex = -1;
                                }
                            }
                        }
                    }

                    #region ObjectiveIndexTextBox

                    if (statusStrip.Items.IndexOfKey("ObjectiveText") == -1)
                    {
                        ToolStripTextBox tb = new ToolStripTextBox();
                        tb.AutoSize = false;
                        tb.BackColor = Color.White;
                        tb.BorderStyle = BorderStyle.FixedSingle;
                        tb.Height = 20;
                        tb.Width = 30;
                        if (os != null)
                        {
                            tb.Text = os.number.ToString();
                        }
                        else
                        {
                            tb.Text = string.Empty;
                        }

                        tb.Name = "ObjectiveText";
                        tb.TextChanged += tb_TextChanged;
                        statusStrip.Items.Add(tb);
                    }

                    #endregion
                }

                #endregion Objective

                #region ObstacleObjectsOnly

                if (showObstacles)
                {
                    ToolStripComboBox comboBlock;
                    int tempindex;

                    #region CollectionModelComboBox

                    if (statusStrip.Items.IndexOfKey("ObstacleModel") == -1)
                    {
                        comboBlock = new ToolStripComboBox();

                        for (int y = 0; y < ObstacleList.Count; y++)
                        {
                            comboBlock.Items.Add(ObstacleList[y].Name);
                        }

                        comboBlock.SelectedIndexChanged += combo_SelectedIndexChangedObstacleModel;
                        comboBlock.BackColor = Color.Gray;
                        comboBlock.DropDownStyle = ComboBoxStyle.DropDownList;
                        comboBlock.FlatStyle = FlatStyle.Flat;
                        comboBlock.Name = "ObstacleModel";
                        comboBlock.Size = new Size(200, statusStrip.Height - 10);
                        comboBlock.Sorted = true;
                        statusStrip.Items.Add(comboBlock);
                    }

                    #endregion

                    if (bsp.Spawns.Spawn[lastSelectedSpawn] != null)
                    {
                        string[] tempOS;
                        int scenModel = statusStrip.Items.IndexOfKey("ObstacleModel");
                        ToolStripComboBox cbBlocModel = (ToolStripComboBox)statusStrip.Items[scenModel];

                        // Update Obstacle Model Type Box
                        tempOS = bsp.Spawns.Spawn[lastSelectedSpawn].TagPath.Split('\\');
                        tempindex = cbBlocModel.Items.IndexOf(tempOS[tempOS.Length - 1]);
                        cbBlocModel.SelectedIndex = tempindex;

                        // If not all selected Sceneries are the same Model, blank the box out
                        for (int x = 0; x < SelectedSpawn.Count; x++)
                        {
                            if (bsp.Spawns.Spawn[SelectedSpawn[x]].TagPath !=
                                bsp.Spawns.Spawn[lastSelectedSpawn].TagPath)
                            {
                                cbBlocModel.SelectedIndex = -1;
                            }
                        }
                    }
                }

                #endregion

                #region SceneryObjectsOnly

                if (showScenery)
                {
                    ToolStripComboBox comboScen;
                    int tempindex;

                    #region CollectionModelComboBox

                    if (statusStrip.Items.IndexOfKey("SceneryModel") == -1)
                    {
                        comboScen = new ToolStripComboBox();

                        for (int y = 0; y < SceneryList.Count; y++)
                        {
                            comboScen.Items.Add(SceneryList[y].Name);
                        }

                        comboScen.SelectedIndexChanged += combo_SelectedIndexChangedSceneryModel;
                        comboScen.BackColor = Color.Gray;
                        comboScen.DropDownStyle = ComboBoxStyle.DropDownList;
                        comboScen.FlatStyle = FlatStyle.Flat;
                        comboScen.Name = "SceneryModel";
                        comboScen.Size = new Size(200, statusStrip.Height - 10);
                        comboScen.Sorted = true;
                        statusStrip.Items.Add(comboScen);
                    }

                    #endregion

                    if (bsp.Spawns.Spawn[lastSelectedSpawn] != null)
                    {
                        string[] tempOS;
                        int scenModel = statusStrip.Items.IndexOfKey("SceneryModel");
                        ToolStripComboBox cbScenModel = (ToolStripComboBox)statusStrip.Items[scenModel];

                        // Update Scenery Model Type Box
                        tempOS = bsp.Spawns.Spawn[lastSelectedSpawn].TagPath.Split('\\');
                        tempindex = cbScenModel.Items.IndexOf(tempOS[tempOS.Length - 1]);
                        cbScenModel.SelectedIndex = tempindex;

                        // If not all selected Sceneries are the same Model, blank the box out
                        for (int x = 0; x < SelectedSpawn.Count; x++)
                        {
                            if (bsp.Spawns.Spawn[SelectedSpawn[x]].TagPath !=
                                bsp.Spawns.Spawn[lastSelectedSpawn].TagPath)
                            {
                                cbScenModel.SelectedIndex = -1;
                            }
                        }
                    }
                }

                #endregion

                #region SoundObjectsOnly

                if (showSounds)
                {
                    SpawnInfo.SoundSpawn os;
                    os = bsp.Spawns.Spawn[lastSelectedSpawn] as SpawnInfo.SoundSpawn;

                    ToolStripComboBox comboSound;
                    ToolStripTextBox textSound;
                    int tempindex;

                    #region SoundsComboBox

                    if (statusStrip.Items.IndexOfKey("SoundScenery") == -1)
                    {
                        comboSound = new ToolStripComboBox();

                        for (int y = 0; y < SoundsList.Count; y++)
                        {
                            comboSound.Items.Add(SoundsList[y].Name);
                        }

                        comboSound.SelectedIndexChanged += combo_SelectedIndexChangedSceneryModel;
                        comboSound.BackColor = Color.Gray;
                        comboSound.DropDownStyle = ComboBoxStyle.DropDownList;
                        comboSound.FlatStyle = FlatStyle.Flat;
                        comboSound.Name = "SoundScenery";
                        comboSound.Size = new Size(200, statusStrip.Height - 10);
                        comboSound.Sorted = true;
                        statusStrip.Items.Add(comboSound);
                    }

                    #endregion

                    #region SoundsInnerSizeBox

                    if (statusStrip.Items.IndexOfKey("SoundSceneryInnerSize") == -1)
                    {
                        textSound = new ToolStripTextBox();

                        textSound.Text = os.DistanceBoundsLower.ToString();
                        textSound.TextBox.TextAlign = HorizontalAlignment.Center;
                        textSound.BackColor = Color.Gray;
                        textSound.Name = "SoundSceneryInnerSize";
                        textSound.Size = new Size(40, statusStrip.Height - 10);

                        textSound.Click += textSound_Click;
                        textSound.TextChanged += textSound_TextChanged;
                        statusStrip.Items.Add(textSound);
                    }

                    #endregion

                    #region SoundsOuterSizeBox

                    if (statusStrip.Items.IndexOfKey("SoundSceneryOuterSize") == -1)
                    {
                        textSound = new ToolStripTextBox();

                        textSound.Text = os.DistanceBoundsUpper.ToString();
                        textSound.TextBox.TextAlign = HorizontalAlignment.Center;
                        textSound.BackColor = Color.Gray;
                        textSound.Name = "SoundSceneryOuterSize";
                        textSound.Size = new Size(40, statusStrip.Height - 10);

                        textSound.Click += textSound_Click;
                        textSound.GotFocus += textSound_GotFocus;
                        textSound.LostFocus += textSound_LostFocus;
                        textSound.MouseHover += textSound_GotFocus;
                        textSound.MouseLeave += textSound_LostMouseFocus;
                        textSound.TextChanged += textSound_TextChanged;
                        statusStrip.Items.Add(textSound);
                    }

                    #endregion

                    if (bsp.Spawns.Spawn[lastSelectedSpawn] != null)
                    {
                        string[] tempOS;
                        int soundScen = statusStrip.Items.IndexOfKey("SoundScenery");
                        ToolStripComboBox cbSoundScen = (ToolStripComboBox)statusStrip.Items[soundScen];
                        int soundInner = statusStrip.Items.IndexOfKey("SoundSceneryInnerSize");
                        ToolStripTextBox cbSoundInner = (ToolStripTextBox)statusStrip.Items[soundInner];
                        int soundOuter = statusStrip.Items.IndexOfKey("SoundSceneryOuterSize");
                        ToolStripTextBox cbSoundOuter = (ToolStripTextBox)statusStrip.Items[soundOuter];

                        // Update Scenery Model Type Box
                        tempOS = bsp.Spawns.Spawn[lastSelectedSpawn].TagPath.Split('\\');
                        tempindex = cbSoundScen.Items.IndexOf(tempOS[tempOS.Length - 1]);
                        cbSoundScen.SelectedIndex = tempindex;

                        // If not all selected Sceneries are the same sound, blank the box out
                        // Same for sizes
                        for (int x = 0; x < SelectedSpawn.Count; x++)
                        {
                            if (bsp.Spawns.Spawn[SelectedSpawn[x]].TagPath != os.TagPath)
                            {
                                cbSoundScen.SelectedIndex = -1;
                            }

                            if (((SpawnInfo.SoundSpawn)bsp.Spawns.Spawn[SelectedSpawn[x]]).DistanceBoundsLower !=
                                os.DistanceBoundsLower)
                            {
                                cbSoundInner.Text = string.Empty;
                            }

                            if (((SpawnInfo.SoundSpawn)bsp.Spawns.Spawn[SelectedSpawn[x]]).DistanceBoundsUpper !=
                                os.DistanceBoundsUpper)
                            {
                                cbSoundOuter.Text = string.Empty;
                            }
                        }
                    }
                }

                #endregion

                #endregion

                // Add the position and rotation info to the start of the status bar
                if (statusStrip.Items.IndexOf(tsLabel1) == -1)
                {
                    statusStrip.Items.Insert(0, tsLabel1);
                }

                if (statusStrip.Items.IndexOf(tsButtonType) == -1)
                {
                    statusStrip.Items.Insert(1, tsButtonType);
                }

                if (statusStrip.Items.IndexOf(tsLabel2) == -1)
                {
                    statusStrip.Items.Insert(2, tsLabel2);
                }

                if (statusStrip.Items.IndexOf(tsLabelCount) == -1)
                {
                    statusStrip.Items.Insert(3, tsLabelCount);
                }

                if (statusStrip.Items.IndexOf(tsLabelX) == -1)
                {
                    statusStrip.Items.Insert(4, tsLabelX);
                }

                if (statusStrip.Items.IndexOf(tsTextBoxX) == -1)
                {
                    statusStrip.Items.Insert(5, tsTextBoxX);
                }

                if (statusStrip.Items.IndexOf(tsLabelY) == -1)
                {
                    statusStrip.Items.Insert(6, tsLabelY);
                }

                if (statusStrip.Items.IndexOf(tsTextBoxY) == -1)
                {
                    statusStrip.Items.Insert(7, tsTextBoxY);
                }

                if (statusStrip.Items.IndexOf(tsLabelZ) == -1)
                {
                    statusStrip.Items.Insert(8, tsLabelZ);
                }

                if (statusStrip.Items.IndexOf(tsTextBoxZ) == -1)
                {
                    statusStrip.Items.Insert(9, tsTextBoxZ);
                }

                if (rot > 0)
                {
                    if (statusStrip.Items.IndexOf(tsLabelYaw) == -1)
                    {
                        statusStrip.Items.Insert(10, tsLabelYaw);
                    }

                    if (statusStrip.Items.IndexOf(tsTextBoxYaw) == -1)
                    {
                        statusStrip.Items.Insert(11, tsTextBoxYaw);
                    }
                }

                if (rot > 1)
                {
                    if (statusStrip.Items.IndexOf(tsLabelPitch) == -1)
                    {
                        statusStrip.Items.Insert(12, tsLabelPitch);
                    }

                    if (statusStrip.Items.IndexOf(tsTextBoxPitch) == -1)
                    {
                        statusStrip.Items.Insert(13, tsTextBoxPitch);
                    }

                    if (statusStrip.Items.IndexOf(tsLabelRoll) == -1)
                    {
                        statusStrip.Items.Insert(14, tsLabelRoll);
                    }

                    if (statusStrip.Items.IndexOf(tsTextBoxRoll) == -1)
                    {
                        statusStrip.Items.Insert(15, tsTextBoxRoll);
                    }
                }
            }
            else
            {
                statusStrip.Items.Clear();
                // Always keep toolbar visible for path/telemetry controls
            }

            // Add the camera position
            toolStripLabel2.Text = "Camera Position: X: " + cam.x.ToString().PadRight(10) + " � Y: " +
                                   cam.y.ToString().PadRight(10) + " � Z: " + cam.z.ToString().PadRight(10);
            statusStrip.Items.Add(toolStripLabel2);
            statusStrip.ResumeLayout();

            // Allow quick update of statusBar, then disable again
            statusStrip.SuspendLayout();
        }

        #endregion

        #region Player Path Animation Methods

        /// <summary>
        /// Loads player path data from a CSV file.
        /// Format: x,y,z[,timestamp] - one point per line.
        /// If timestamp is omitted, points are evenly spaced at 0.1 second intervals.
        /// </summary>
        /// <param name="filePath">Path to the CSV file.</param>
        public void LoadPlayerPath(string filePath)
        {
            playerPath.Clear();
            multiPlayerPaths.Clear();
            pathPlayerNames.Clear();
            killEvents.Clear();
            playerPrevKills.Clear();
            pathCurrentIndex = 0;
            pathTimeAccumulator = 0;
            pathMinTimestamp = float.MaxValue;
            pathMaxTimestamp = float.MinValue;

            try
            {
                string[] lines = File.ReadAllLines(filePath);
                float autoTimestamp = 0;
                int skippedLines = 0;

                // Column indices - support full 36-column format
                Dictionary<string, int> cols = new Dictionary<string, int>();
                bool hasHeader = false;
                float timestampDivisor = 1.0f;

                // Check for header row and detect column layout
                if (lines.Length > 0)
                {
                    string[] headerParts = lines[0].Split(',');
                    string firstCol = headerParts[0].Trim().ToLowerInvariant();
                    hasHeader = firstCol == "timestamp" || firstCol == "playername" ||
                                firstCol == "x" || firstCol == "posx" || !firstCol.Contains("-");

                    if (hasHeader)
                    {
                        for (int i = 0; i < headerParts.Length; i++)
                        {
                            cols[headerParts[i].Trim().ToLowerInvariant()] = i;
                        }
                        if (cols.ContainsKey("gametimems"))
                            timestampDivisor = 1000.0f;
                    }
                    else
                    {
                        // Default simple format: x,y,z[,timestamp]
                        cols["x"] = 0; cols["y"] = 1; cols["z"] = 2; cols["timestamp"] = 3;
                    }
                }

                // Helper to get column value
                Func<string[], string, string> getStr = (parts, col) => {
                    if (cols.ContainsKey(col) && parts.Length > cols[col])
                        return parts[cols[col]].Trim();
                    return null;
                };
                Func<string[], string, float> getFloat = (parts, col) => {
                    float val = 0;
                    if (cols.ContainsKey(col) && parts.Length > cols[col])
                        float.TryParse(parts[cols[col]].Trim(), System.Globalization.NumberStyles.Float,
                            System.Globalization.CultureInfo.InvariantCulture, out val);
                    return val;
                };
                Func<string[], string, int> getInt = (parts, col) => {
                    int val = 0;
                    if (cols.ContainsKey(col) && parts.Length > cols[col])
                        int.TryParse(parts[cols[col]].Trim(), out val);
                    return val;
                };
                Func<string[], string, bool> getBool = (parts, col) => {
                    if (cols.ContainsKey(col) && parts.Length > cols[col])
                    {
                        string val = parts[cols[col]].Trim().ToLowerInvariant();
                        return val == "true" || val == "1" || val == "yes";
                    }
                    return false;
                };

                int startLine = hasHeader ? 1 : 0;
                Dictionary<string, PlayerPathPoint> lastPoints = new Dictionary<string, PlayerPathPoint>();

                for (int lineIdx = startLine; lineIdx < lines.Length; lineIdx++)
                {
                    string line = lines[lineIdx];
                    if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#") || line.StartsWith("//"))
                        continue;

                    string[] parts = line.Split(',');
                    if (parts.Length < 3)
                    {
                        skippedLines++;
                        continue;
                    }

                    // Parse position (try both x/y/z and posx/posy/posz)
                    float x = getFloat(parts, "posx");
                    float y = getFloat(parts, "posy");
                    float z = getFloat(parts, "posz");
                    if (x == 0 && y == 0 && z == 0)
                    {
                        x = getFloat(parts, "x");
                        y = getFloat(parts, "y");
                        z = getFloat(parts, "z");
                    }
                    if (x == 0 && y == 0 && z == 0 && parts.Length >= 3)
                    {
                        // Fall back to first 3 columns as x,y,z
                        float.TryParse(parts[0].Trim(), System.Globalization.NumberStyles.Float,
                            System.Globalization.CultureInfo.InvariantCulture, out x);
                        float.TryParse(parts[1].Trim(), System.Globalization.NumberStyles.Float,
                            System.Globalization.CultureInfo.InvariantCulture, out y);
                        float.TryParse(parts[2].Trim(), System.Globalization.NumberStyles.Float,
                            System.Globalization.CultureInfo.InvariantCulture, out z);
                    }

                    if (x == 0 && y == 0 && z == 0)
                    {
                        skippedLines++;
                        continue;
                    }

                    // Parse timestamp
                    float timestamp = getFloat(parts, "timestamp");
                    if (timestamp == 0) timestamp = getFloat(parts, "gametimems") / timestampDivisor;
                    if (timestamp == 0) timestamp = autoTimestamp;
                    autoTimestamp += 0.1f;

                    // Track min/max timestamps
                    if (timestamp < pathMinTimestamp) pathMinTimestamp = timestamp;
                    if (timestamp > pathMaxTimestamp) pathMaxTimestamp = timestamp;

                    // Parse player name
                    string playerName = getStr(parts, "playername") ?? "Player";

                    // Parse team
                    int team = -1;
                    string teamStr = getStr(parts, "team");
                    if (!string.IsNullOrEmpty(teamStr))
                    {
                        teamStr = teamStr.ToLowerInvariant();
                        if (teamStr.Contains("red") || teamStr == "0") team = 0;
                        else if (teamStr.Contains("blue") || teamStr == "1") team = 1;
                        else if (teamStr.Contains("green") || teamStr == "2") team = 2;
                        else if (teamStr.Contains("orange") || teamStr == "3") team = 3;
                        else int.TryParse(teamStr, out team);
                    }

                    // Parse all other fields
                    float yaw = getFloat(parts, "yawdeg");
                    if (yaw == 0) yaw = getFloat(parts, "yaw") * (180f / (float)Math.PI);
                    string weapon = getStr(parts, "currentweapon");
                    bool crouching = getBool(parts, "iscrouching");
                    bool airborne = getBool(parts, "isairborne");
                    bool isDead = getBool(parts, "isdead") || getInt(parts, "respawntimer") > 0;
                    int emblemFg = getInt(parts, "emblemfg");
                    int emblemBg = getInt(parts, "emblembg");
                    int colorPrimary = getInt(parts, "colorprimary");
                    int colorSecondary = getInt(parts, "colorsecondary");
                    int colorTertiary = getInt(parts, "colortertiary");
                    int colorQuaternary = getInt(parts, "colorquaternary");
                    int kills = getInt(parts, "kills");

                    // Track kill events
                    if (!playerPrevKills.ContainsKey(playerName))
                        playerPrevKills[playerName] = 0;
                    if (kills > playerPrevKills[playerName])
                    {
                        // New kill(s) detected
                        for (int k = 0; k < kills - playerPrevKills[playerName]; k++)
                        {
                            killEvents.Add(new KillEvent
                            {
                                Timestamp = timestamp,
                                PlayerName = playerName,
                                Team = team,
                                Weapon = weapon
                            });
                        }
                        playerPrevKills[playerName] = kills;
                    }

                    PlayerPathPoint point = new PlayerPathPoint(x, y, z, timestamp, team, yaw, playerName, weapon,
                        crouching, airborne, isDead, emblemFg, emblemBg, colorPrimary, colorSecondary, colorTertiary, colorQuaternary);

                    // Add to legacy single list
                    playerPath.Add(point);

                    // Add to multi-player structure with respawn detection
                    if (!multiPlayerPaths.ContainsKey(playerName))
                    {
                        multiPlayerPaths[playerName] = new List<List<PlayerPathPoint>>();
                        multiPlayerPaths[playerName].Add(new List<PlayerPathPoint>());
                        pathPlayerNames.Add(playerName);
                    }

                    // Detect respawn: position jump > 10 units or was dead and now alive
                    bool isRespawn = false;
                    if (lastPoints.ContainsKey(playerName))
                    {
                        var last = lastPoints[playerName];
                        float dist = (float)Math.Sqrt((x - last.X) * (x - last.X) + (y - last.Y) * (y - last.Y) + (z - last.Z) * (z - last.Z));
                        if (dist > 10.0f || (last.IsDead && !isDead))
                        {
                            isRespawn = true;
                        }
                    }

                    if (isRespawn)
                    {
                        // Start new segment
                        multiPlayerPaths[playerName].Add(new List<PlayerPathPoint>());
                    }

                    // Add point to current segment
                    var currentSegment = multiPlayerPaths[playerName][multiPlayerPaths[playerName].Count - 1];
                    currentSegment.Add(point);
                    lastPoints[playerName] = point;
                }

                // Clear hidden players when loading new path
                hiddenPlayers.Clear();
                povModeEnabled = false;
                povFollowPlayer = null;

                // Update player and POV dropdowns with team colors
                RefreshPlayerDropdowns();

                // Initialize timeline
                if (pathMinTimestamp == float.MaxValue) pathMinTimestamp = 0;
                if (pathMaxTimestamp == float.MinValue) pathMaxTimestamp = 0;
                pathCurrentTimestamp = pathMinTimestamp;
                UpdateTimelineLabel();

                // Silent load - no prompts
                if (playerPath.Count > 0)
                {
                    // Log to debug instead of showing dialog
                    AddDebugLog($"Loaded {playerPath.Count} path points for {pathPlayerNames.Count} player(s).");
                    int totalSegments = 0;
                    foreach (var kvp in multiPlayerPaths) totalSegments += kvp.Value.Count;
                    AddDebugLog($"{totalSegments} path segments (respawns detected).");
                }
            }
            catch (Exception ex)
            {
                // Log error silently
                AddDebugLog($"[ERROR] Loading path file: {ex.Message}");
            }
        }

        /// <summary>
        /// Opens a file dialog to load player path data.
        /// </summary>
        private void LoadPlayerPathDialog()
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Title = "Load Player Path Data";
                ofd.Filter = "CSV Files (*.csv)|*.csv|Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
                ofd.FilterIndex = 1;

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    LoadPlayerPath(ofd.FileName);
                    EnableTelemetryViewOptions();
                }
            }
        }

        /// <summary>
        /// Toggles path animation playback.
        /// </summary>
        private void TogglePathPlayback()
        {
            if (playerPath.Count == 0)
            {
                // No path data - silent return
                return;
            }

            pathIsPlaying = !pathIsPlaying;
            pathLastFrameTime = DateTime.Now;

            if (pathIsPlaying && pathCurrentIndex >= playerPath.Count - 1)
            {
                // Restart from beginning if at end
                pathCurrentIndex = 0;
                pathTimeAccumulator = 0;
            }
        }

        /// <summary>
        /// Resets path animation to beginning.
        /// </summary>
        private void ResetPathAnimation()
        {
            pathCurrentIndex = 0;
            pathTimeAccumulator = 0;
            pathCurrentTimestamp = pathMinTimestamp;
            pathIsPlaying = false;
            UpdateTimelineLabel();
        }

        /// <summary>
        /// Updates path animation state.
        /// </summary>
        private void UpdatePathAnimation()
        {
            if (!pathIsPlaying || playerPath.Count < 2)
                return;

            DateTime now = DateTime.Now;
            float deltaTime = (float)(now - pathLastFrameTime).TotalSeconds;
            pathLastFrameTime = now;

            pathTimeAccumulator += deltaTime * pathPlaybackSpeed;
            pathCurrentTimestamp = pathTimeAccumulator;

            // Find current position based on time
            while (pathCurrentIndex < playerPath.Count - 1 &&
                   pathTimeAccumulator >= playerPath[pathCurrentIndex + 1].Timestamp)
            {
                pathCurrentIndex++;
            }

            // Update timeline
            UpdateTimelineLabel();

            // Update camera for POV mode
            UpdatePOVCamera();

            // Handle A-B loop - loop back to A when reaching B
            if (bookmarkLoopEnabled && bookmarkStartTimestamp >= 0 && bookmarkEndTimestamp >= 0)
            {
                if (pathCurrentTimestamp >= bookmarkEndTimestamp)
                {
                    JumpToBookmark(); // Jump back to A
                }
            }
            // Handle end of playback (when not looping)
            else if (pathCurrentTimestamp >= pathMaxTimestamp || pathCurrentIndex >= playerPath.Count - 1)
            {
                pathIsPlaying = false;
                if (pathPlayPauseButton != null)
                    pathPlayPauseButton.Text = "> Play";
            }
        }

        /// <summary>
        /// Updates the camera to follow the selected player in POV mode.
        /// Works for both playback and live telemetry.
        /// </summary>
        private void UpdatePOVCamera()
        {
            if (!povModeEnabled || string.IsNullOrEmpty(povFollowPlayer))
                return;

            // Try live telemetry first
            if (showLiveTelemetry)
            {
                PlayerTelemetry livePlayer = null;
                lock (livePlayersLock)
                {
                    if (livePlayers.ContainsKey(povFollowPlayer))
                        livePlayer = livePlayers[povFollowPlayer];
                }

                if (livePlayer != null && !livePlayer.IsDead)
                {
                    cam.x = livePlayer.PosX;
                    cam.y = livePlayer.PosY;
                    cam.z = livePlayer.PosZ + 0.6f; // Eye height offset
                    cam.Position.X = cam.x;
                    cam.Position.Y = cam.y;
                    cam.Position.Z = cam.z;
                    // Use Yaw (radians) directly if available, otherwise convert YawDeg
                    cam.radianh = livePlayer.Yaw != 0 ? livePlayer.Yaw : livePlayer.YawDeg * (float)(Math.PI / 180.0);
                    cam.ComputePosition();
                    return;
                }
            }

            // Fall back to playback path data
            if (!multiPlayerPaths.ContainsKey(povFollowPlayer))
                return;

            // Find the point for the followed player at current timestamp
            // Skip dead points to find the most recent alive position
            PlayerPathPoint? currentPoint = null;
            PlayerPathPoint? lastAlivePoint = null;
            foreach (var segment in multiPlayerPaths[povFollowPlayer])
            {
                foreach (var pt in segment)
                {
                    if (pt.Timestamp <= pathCurrentTimestamp)
                    {
                        currentPoint = pt;
                        if (!pt.IsDead)
                            lastAlivePoint = pt;
                    }
                    else
                        break;
                }
            }

            // Prefer alive point, fallback to any point
            if (!lastAlivePoint.HasValue && !currentPoint.HasValue)
                return;

            var point = lastAlivePoint.HasValue ? lastAlivePoint.Value : currentPoint.Value;

            // Skip if player is dead at current timestamp
            if (point.IsDead)
                return;

            // Set camera position at player's eye level
            cam.x = point.X;
            cam.y = point.Y;
            cam.z = point.Z + 0.6f; // Eye height offset
            cam.Position.X = cam.x;
            cam.Position.Y = cam.y;
            cam.Position.Z = cam.z;

            // Set camera yaw to player's facing direction
            cam.radianh = point.FacingYaw * (float)(Math.PI / 180.0);
            cam.ComputePosition();
        }

        /// <summary>
        /// Gets the interpolated current position along the path.
        /// </summary>
        private Vector3 GetCurrentPathPosition()
        {
            if (playerPath.Count == 0)
                return new Vector3(0, 0, 0);

            if (pathCurrentIndex >= playerPath.Count - 1)
            {
                var last = playerPath[playerPath.Count - 1];
                return new Vector3(last.X, last.Y, last.Z);
            }

            var p1 = playerPath[pathCurrentIndex];
            var p2 = playerPath[pathCurrentIndex + 1];

            // Interpolate between current and next point
            float t = 0;
            float duration = p2.Timestamp - p1.Timestamp;
            if (duration > 0)
            {
                t = (pathTimeAccumulator - p1.Timestamp) / duration;
                t = Math.Max(0, Math.Min(1, t));
            }

            return new Vector3(
                p1.X + (p2.X - p1.X) * t,
                p1.Y + (p2.Y - p1.Y) * t,
                p1.Z + (p2.Z - p1.Z) * t
            );
        }

        /// <summary>
        /// Renders the player path and current position marker.
        /// </summary>
        private void RenderPlayerPath()
        {
            if (multiPlayerPaths.Count == 0 && playerPath.Count == 0)
                return;

            // Try to load biped model if not already attempted
            if (!playerBipedModelLoaded)
            {
                LoadPlayerBipedModel();
            }

            // Draw path trails as separate segments (not connected across respawns)
            if (showPathTrail)
            {
                render.device.SetTexture(0, null);
                render.device.RenderState.Lighting = false;
                render.device.VertexFormat = CustomVertex.PositionColored.Format;
                render.device.Transform.World = Matrix.Identity;

                foreach (var kvp in multiPlayerPaths)
                {
                    string playerName = kvp.Key;
                    if (hiddenPlayers.Contains(playerName))
                        continue;

                    foreach (var segment in kvp.Value)
                    {
                        if (segment.Count < 2) continue;

                        // Only draw points up to current timestamp
                        List<CustomVertex.PositionColored> verts = new List<CustomVertex.PositionColored>();
                        foreach (var pt in segment)
                        {
                            if (pt.Timestamp > pathCurrentTimestamp) break;
                            Color ptColor = pt.Team >= 0 ? GetTeamColor(pt.Team) : Color.White;
                            verts.Add(new CustomVertex.PositionColored(pt.X, pt.Y, pt.Z, ptColor.ToArgb()));
                        }

                        if (verts.Count >= 2)
                        {
                            render.device.DrawUserPrimitives(PrimitiveType.LineStrip, verts.Count - 1, verts.ToArray());
                        }
                    }
                }
                render.device.RenderState.Lighting = true;
            }

            // Find and render each player at their current position
            foreach (var kvp in multiPlayerPaths)
            {
                string playerName = kvp.Key;
                if (hiddenPlayers.Contains(playerName))
                    continue;

                // Find the point for this player at current timestamp
                PlayerPathPoint? currentPoint = null;
                PlayerPathPoint? prevPoint = null;

                foreach (var segment in kvp.Value)
                {
                    for (int i = 0; i < segment.Count; i++)
                    {
                        if (segment[i].Timestamp <= pathCurrentTimestamp)
                        {
                            prevPoint = currentPoint;
                            currentPoint = segment[i];
                        }
                        else break;
                    }
                }

                if (!currentPoint.HasValue) continue;
                var pt = currentPoint.Value;

                // Skip if dead
                if (pt.IsDead) continue;

                Color teamColor = pt.Team >= 0 ? GetTeamColor(pt.Team) : Color.White;
                float groundOffset = -0.2f;

                // Draw team color circle at ground level
                DrawTeamCircle(pt.X, pt.Y, pt.Z + groundOffset, teamColor);

                // Use biped model if available, otherwise fall back to cylinder
                if (playerBipedModel != null)
                {
                    float yawRadians = pt.FacingYaw * (float)(Math.PI / 180.0);
                    Matrix rotation = Matrix.RotationZ(yawRadians);
                    Matrix translation = Matrix.Translation(pt.X, pt.Y, pt.Z + groundOffset);
                    render.device.Transform.World = Matrix.Multiply(rotation, translation);

                    Material teamMaterial = new Material();
                    teamMaterial.Diffuse = teamColor;
                    teamMaterial.Ambient = teamColor;
                    teamMaterial.Emissive = Color.FromArgb(teamColor.R / 3, teamColor.G / 3, teamColor.B / 3);

                    render.device.RenderState.Lighting = true;
                    render.device.Material = teamMaterial;
                    ParsedModel.DisplayedInfo.Draw(ref render.device, playerBipedModel);
                }
                else
                {
                    if (playerMarkerMesh == null || playerMarkerMesh.Disposed)
                    {
                        playerMarkerMesh = Mesh.Cylinder(render.device, 0.2f, 0.1f, 0.7f, 8, 1);
                    }

                    PlayerMarkerMaterial = new Material();
                    PlayerMarkerMaterial.Diffuse = teamColor;
                    PlayerMarkerMaterial.Ambient = teamColor;
                    PlayerMarkerMaterial.Emissive = Color.FromArgb(teamColor.R / 2, teamColor.G / 2, teamColor.B / 2);

                    float yawRadians = pt.FacingYaw * (float)(Math.PI / 180.0);
                    Matrix yawRotation = Matrix.RotationZ(yawRadians);
                    Matrix tiltRotation = Matrix.RotationX((float)(Math.PI / 2));
                    Matrix translation = Matrix.Translation(pt.X, pt.Y, pt.Z + 0.35f + groundOffset);
                    render.device.Transform.World = Matrix.Multiply(Matrix.Multiply(tiltRotation, yawRotation), translation);
                    render.device.Material = PlayerMarkerMaterial;
                    render.device.SetTexture(0, null);
                    render.device.RenderState.FillMode = FillMode.Solid;
                    playerMarkerMesh.DrawSubset(0);
                }

                // Draw player name above head (create temporary telemetry object for DrawPlayerName)
                PlayerTelemetry tempTelemetry = new PlayerTelemetry();
                tempTelemetry.PlayerName = pt.PlayerName;
                tempTelemetry.Team = pt.Team;
                tempTelemetry.PosX = pt.X;
                tempTelemetry.PosY = pt.Y;
                tempTelemetry.PosZ = pt.Z;
                tempTelemetry.CurrentWeapon = pt.CurrentWeapon;
                tempTelemetry.EmblemFg = pt.EmblemFg;
                tempTelemetry.EmblemBg = pt.EmblemBg;
                tempTelemetry.ColorPrimary = pt.ColorPrimary;
                tempTelemetry.ColorSecondary = pt.ColorSecondary;
                tempTelemetry.ColorTertiary = pt.ColorTertiary;
                tempTelemetry.ColorQuaternary = pt.ColorQuaternary;
                tempTelemetry.IsDead = pt.IsDead;
                DrawPlayerName(tempTelemetry, pt.IsDead);
            }
        }

        /// <summary>
        /// Loads the player biped model from the map (Spartan/Master Chief).
        /// </summary>
        private void LoadPlayerBipedModel()
        {
            playerBipedModelLoaded = true;

            try
            {
                // Track map state to restore it properly
                int alreadyOpen = map.isOpen ? (int)map.openMapType : -1;
                if (alreadyOpen != (int)MapTypes.Internal)
                {
                    map.OpenMap(MapTypes.Internal);
                }

                // Find the default biped model (Master Chief/Spartan) from the scenario
                // This is stored at offset 308 in the scenario tag (meta 0)
                map.BR.BaseStream.Position = map.MetaInfo.Offset[0] + 308;
                int tempr = map.BR.ReadInt32();
                if (tempr == 0)
                {
                    RestoreMapState(alreadyOpen);
                    return;
                }

                tempr -= map.SecondaryMagic;
                map.BR.BaseStream.Position = tempr + 4;
                int bipdTagIndex = map.Functions.ForMeta.FindMetaByID(map.BR.ReadInt32());

                if (bipdTagIndex == -1)
                {
                    RestoreMapState(alreadyOpen);
                    return;
                }

                // Get the model tag number from the biped
                int modelTagNumber = map.Functions.FindModelByBaseClass(bipdTagIndex);

                if (modelTagNumber == -1)
                {
                    RestoreMapState(alreadyOpen);
                    return;
                }

                // Load the model
                Meta m = new Meta(map);
                m.ReadMetaFromMap(modelTagNumber, false);

                playerBipedModel = new ParsedModel(ref m);
                ParsedModel.DisplayedInfo.LoadDirectXTexturesAndBuffers(ref render.device, ref playerBipedModel);

                m.Dispose();
                RestoreMapState(alreadyOpen);
            }
            catch (Exception)
            {
                // If loading fails, we'll just use the cylinder fallback
                playerBipedModel = null;
            }
        }

        /// <summary>
        /// Restores the map to its previous open state.
        /// </summary>
        private void RestoreMapState(int previousState)
        {
            if (previousState == -1)
            {
                map.CloseMap();
            }
            else
            {
                map.OpenMap((MapTypes)previousState);
            }
        }

        /// <summary>
        /// Gets the color for a team.
        /// </summary>
        private Color GetTeamColor(int team)
        {
            switch (team)
            {
                case 0: return Color.Red;
                case 1: return Color.Blue;
                case 2: return Color.Green;
                case 3: return Color.Orange;
                default: return Color.Yellow;
            }
        }

        #endregion

        #region Live Telemetry Network Methods

        /// <summary>
        /// Starts the UDP telemetry listener on port 2222.
        /// </summary>
        public void StartTelemetryListener()
        {
            if (telemetryListenerRunning)
                return;

            try
            {
                telemetryListenerRunning = true;
                showLiveTelemetry = true;

                // Reset header parsing state
                csvColumnIndices.Clear();

                // Clear live player data for fresh start
                lock (livePlayersLock)
                {
                    livePlayers.Clear();
                }
                livePlayerNames.Clear();
                RefreshPlayerDropdowns();

                // Start UDP listener
                telemetryUdpClient = new UdpClient();
                telemetryUdpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                telemetryUdpClient.Client.Bind(new IPEndPoint(IPAddress.Any, 2222));

                telemetryListenerThread = new Thread(TelemetryListenerLoop);
                telemetryListenerThread.IsBackground = true;
                telemetryListenerThread.Start();

                // Start TCP listener
                telemetryTcpListener = new TcpListener(IPAddress.Any, 2222);
                telemetryTcpListener.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                telemetryTcpListener.Start();

                telemetryTcpListenerThread = new Thread(TelemetryTcpListenerLoop);
                telemetryTcpListenerThread.IsBackground = true;
                telemetryTcpListenerThread.Start();

                AddDebugLog("Telemetry listeners started on port 2222 (UDP + TCP)");
            }
            catch (Exception ex)
            {
                AddDebugLog($"Failed to start telemetry listener: {ex.Message}");
            }
        }

        /// <summary>
        /// Stops the telemetry listener.
        /// </summary>
        public void StopTelemetryListener()
        {
            telemetryListenerRunning = false;
            showLiveTelemetry = false;

            try
            {
                telemetryUdpClient?.Close();
                telemetryTcpListener?.Stop();
                telemetryListenerThread?.Join(1000);
                telemetryTcpListenerThread?.Join(1000);
            }
            catch { }

            lock (livePlayersLock)
            {
                livePlayers.Clear();
            }
            livePlayerNames.Clear();

            // Restore playback player dropdowns
            RefreshPlayerDropdowns();
        }

        /// <summary>
        /// Main loop for receiving UDP telemetry data.
        /// </summary>
        private void TelemetryListenerLoop()
        {
            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);

            AddDebugLog("UDP listener started, waiting for data on port 2222...");

            while (telemetryListenerRunning)
            {
                try
                {
                    byte[] data = telemetryUdpClient.Receive(ref remoteEP);
                    string packet = Encoding.UTF8.GetString(data).Trim();

                    // Split packet into multiple lines (one per player)
                    string[] lines = packet.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (string line in lines)
                    {
                        if (string.IsNullOrWhiteSpace(line)) continue;
                        ProcessTelemetryLine(line);
                    }
                }
                catch (SocketException)
                {
                    // Socket closed, exit loop
                    if (!telemetryListenerRunning) break;
                }
                catch (Exception ex)
                {
                    AddDebugLog($"[UDP ERROR] {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Main loop for receiving TCP telemetry data.
        /// </summary>
        private void TelemetryTcpListenerLoop()
        {
            AddDebugLog("TCP listener started, waiting for connections on port 2222...");

            while (telemetryListenerRunning)
            {
                try
                {
                    // Accept a TCP client
                    if (!telemetryTcpListener.Pending())
                    {
                        Thread.Sleep(100);
                        continue;
                    }

                    using (TcpClient client = telemetryTcpListener.AcceptTcpClient())
                    using (NetworkStream stream = client.GetStream())
                    using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                    {
                        AddDebugLog($"[TCP] Client connected from {client.Client.RemoteEndPoint}");

                        while (telemetryListenerRunning && client.Connected)
                        {
                            string line = reader.ReadLine();
                            if (line == null) break;
                            if (string.IsNullOrWhiteSpace(line)) continue;

                            ProcessTelemetryLine(line);
                        }
                    }
                }
                catch (SocketException)
                {
                    if (!telemetryListenerRunning) break;
                }
                catch (Exception ex)
                {
                    AddDebugLog($"[TCP ERROR] {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Process a single telemetry line (shared by UDP and TCP).
        /// </summary>
        private void ProcessTelemetryLine(string line)
        {
            string[] parts = line.Split(',');

            // Parse header row if we haven't yet
            if (csvColumnIndices.Count == 0)
            {
                string firstField = parts[0].Trim().ToLowerInvariant();
                bool isHeader = firstField == "timestamp" || firstField == "playername" ||
                                !firstField.Contains("-");

                if (isHeader)
                {
                    for (int i = 0; i < parts.Length; i++)
                    {
                        csvColumnIndices[parts[i].Trim().ToLowerInvariant()] = i;
                    }
                    int posxIdx = csvColumnIndices.ContainsKey("posx") ? csvColumnIndices["posx"] : -1;
                    int posyIdx = csvColumnIndices.ContainsKey("posy") ? csvColumnIndices["posy"] : -1;
                    int poszIdx = csvColumnIndices.ContainsKey("posz") ? csvColumnIndices["posz"] : -1;
                    AddDebugLog($"[HEADER] {csvColumnIndices.Count} cols, posx={posxIdx} posy={posyIdx} posz={poszIdx}");
                    return;
                }
                else
                {
                    SetDefaultColumnOrder();
                    AddDebugLog($"[AUTO] Using default column order ({csvColumnIndices.Count} columns)");
                }
            }

            // Debug: show first columns and total count to detect offset issues
            if (parts.Length > 3)
            {
                AddDebugLog($"[ROW] [0]={parts[0]} [1]={parts[1]} [2]={parts[2]} total={parts.Length}");
            }

            // Parse data row
            PlayerTelemetry telemetry = ParseTelemetryLine(parts, csvColumnIndices);
            if (telemetry != null)
            {
                lock (livePlayersLock)
                {
                    livePlayers[telemetry.PlayerName] = telemetry;
                }
                // Update dropdowns if player list changed
                UpdateLivePlayerDropdowns();
            }
        }

        private void AddDebugLog(string message)
        {
            lock (telemetryDebugLogLock)
            {
                telemetryDebugLog.Add($"{DateTime.Now:HH:mm:ss.fff} {message}");
                while (telemetryDebugLog.Count > MaxDebugLogEntries)
                {
                    telemetryDebugLog.RemoveAt(0);
                }
            }
        }

        /// <summary>
        /// Sets up default column order matching the expected CSV format.
        /// </summary>
        private void SetDefaultColumnOrder()
        {
            csvColumnIndices.Clear();
            // Matches actual telemetry sender format (36 columns):
            // 1-5: Timestamp, PlayerName, Team, XboxId, MachineId
            // 6-11: EmblemFg, EmblemBg, ColorPrimary, ColorSecondary, ColorTertiary, ColorQuaternary
            // 12-18: PosX, PosY, PosZ, VelX, VelY, VelZ, Speed
            // 19-22: Yaw, Pitch, YawDeg, PitchDeg
            // 23-28: IsDead, RespawnTimer, IsCrouching, CrouchBlend, IsAirborne, AirborneTicks
            // 29-32: WeaponSlot, CurrentWeapon, FragGrenades, PlasmaGrenades
            // 33-36: Kills, Deaths, Assists, Event
            string[] columns = {
                "timestamp", "playername", "team", "xboxid", "machineid",
                "emblemfg", "emblembg", "colorprimary", "colorsecondary", "colortertiary", "colorquaternary",
                "posx", "posy", "posz", "velx", "vely", "velz", "speed",
                "yaw", "pitch", "yawdeg", "pitchdeg",
                "isdead", "respawntimer", "iscrouching", "crouchblend", "isairborne", "airborneticks",
                "weaponslot", "currentweapon", "fraggrenades", "plasmagrenades",
                "kills", "deaths", "assists", "event"
            };
            for (int i = 0; i < columns.Length; i++)
            {
                csvColumnIndices[columns[i]] = i;
            }
        }

        /// <summary>
        /// Parses a telemetry CSV line into a PlayerTelemetry object.
        /// </summary>
        private PlayerTelemetry ParseTelemetryLine(string[] parts, Dictionary<string, int> cols)
        {
            try
            {
                PlayerTelemetry t = new PlayerTelemetry();

                // Helper to get string value
                Func<string, string> getStr = (col) => {
                    if (cols.ContainsKey(col) && parts.Length > cols[col])
                        return parts[cols[col]].Trim();
                    return null;
                };

                // Helper to get float value
                Func<string, float> getFloat = (col) => {
                    float val = 0;
                    if (cols.ContainsKey(col) && parts.Length > cols[col])
                        float.TryParse(parts[cols[col]].Trim(), System.Globalization.NumberStyles.Float,
                            System.Globalization.CultureInfo.InvariantCulture, out val);
                    return val;
                };

                // Helper to get int value
                Func<string, int> getInt = (col) => {
                    int val = 0;
                    if (cols.ContainsKey(col) && parts.Length > cols[col])
                        int.TryParse(parts[cols[col]].Trim(), out val);
                    return val;
                };

                // Helper to get bool value (handles True/False, 0/1, yes/no)
                Func<string, bool> getBool = (col) => {
                    if (cols.ContainsKey(col) && parts.Length > cols[col])
                    {
                        string val = parts[cols[col]].Trim().ToLowerInvariant();
                        return val == "true" || val == "1" || val == "yes";
                    }
                    return false;
                };

                // Required: PlayerName
                t.PlayerName = getStr("playername");
                if (string.IsNullOrEmpty(t.PlayerName))
                    return null;

                // Identity
                t.XboxId = getStr("xboxid");
                t.MachineId = getStr("machineid");

                // Team - handle both string names and numeric values
                string teamStr = getStr("team");
                if (!string.IsNullOrEmpty(teamStr))
                {
                    teamStr = teamStr.ToLowerInvariant();
                    if (teamStr.Contains("red") || teamStr == "0") t.Team = 0;
                    else if (teamStr.Contains("blue") || teamStr == "1") t.Team = 1;
                    else if (teamStr.Contains("green") || teamStr == "2") t.Team = 2;
                    else if (teamStr.Contains("orange") || teamStr == "3") t.Team = 3;
                    else int.TryParse(teamStr, out t.Team);
                }

                // Emblem & Colors
                t.EmblemFg = getInt("emblemfg");
                t.EmblemBg = getInt("emblembg");
                t.ColorPrimary = getInt("colorprimary");
                t.ColorSecondary = getInt("colorsecondary");
                t.ColorTertiary = getInt("colortertiary");
                t.ColorQuaternary = getInt("colorquaternary");

                // Timestamp
                string tsStr = getStr("timestamp");
                if (!string.IsNullOrEmpty(tsStr))
                    DateTime.TryParse(tsStr, out t.Timestamp);

                // Position
                t.PosX = getFloat("posx");
                t.PosY = getFloat("posy");
                t.PosZ = getFloat("posz");

                // Velocity (may not be present in all formats)
                t.VelX = getFloat("velx");
                t.VelY = getFloat("vely");
                t.VelZ = getFloat("velz");
                t.Speed = getFloat("speed");

                // Orientation (yaw/pitch in radians, yawdeg/pitchdeg in degrees)
                t.Yaw = getFloat("yaw");
                t.Pitch = getFloat("pitch");
                t.YawDeg = getFloat("yawdeg");
                t.PitchDeg = getFloat("pitchdeg");

                // Movement State
                t.IsCrouching = getBool("iscrouching");
                t.CrouchBlend = getFloat("crouchblend");
                t.IsAirborne = getBool("isairborne");
                t.AirborneTicks = getInt("airborneticks");

                // Weapons
                t.WeaponSlot = getInt("weaponslot");
                t.CurrentWeapon = getStr("currentweapon");
                t.FragGrenades = getInt("fraggrenades");
                t.PlasmaGrenades = getInt("plasmagrenades");

                // K/D Stats
                t.Kills = getInt("kills");
                t.Deaths = getInt("deaths");
                t.RespawnTimer = getInt("respawntimer");
                // Use both IsDead field and RespawnTimer for robust death detection
                t.IsDead = getBool("isdead") || t.RespawnTimer > 0;

                // Events
                t.Event = getStr("event");

                return t;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Renders live player telemetry.
        /// </summary>
        private void RenderLivePlayers()
        {
            if (!showLiveTelemetry)
                return;

            // Try to load biped model if not already attempted
            if (!playerBipedModelLoaded)
            {
                LoadPlayerBipedModel();
            }

            List<PlayerTelemetry> players;
            lock (livePlayersLock)
            {
                players = new List<PlayerTelemetry>(livePlayers.Values);
            }

            foreach (PlayerTelemetry player in players)
            {
                // Skip hidden players
                if (hiddenPlayers.Contains(player.PlayerName))
                    continue;

                Color teamColor = GetTeamColor(player.Team);

                // Check if player is dead (using IsDead field from telemetry)
                bool isDead = player.IsDead;

                // Ground offset to align with floor (-0.2 world units)
                float groundOffset = -0.2f;

                // Adjust Z position for crouching
                float zOffset = player.IsCrouching ? -0.2f * player.CrouchBlend : 0f;
                float modelZOffset = zOffset + groundOffset;

                // Calculate circle Z - stays on ground even when jumping
                // When airborne, estimate ground level by subtracting jump height
                float circleZ = player.PosZ + groundOffset;
                if (player.IsAirborne)
                {
                    // Keep circle lower when jumping (approximate ground level)
                    circleZ = player.PosZ + groundOffset - (player.AirborneTicks * 0.02f);
                }

                // Only draw model and circle if alive
                if (!isDead)
                {
                    // Draw team color circle at ground level
                    DrawTeamCircle(player.PosX, player.PosY, circleZ, teamColor);

                    // Draw ground shadow when airborne
                    if (player.IsAirborne && player.AirborneTicks > 5)
                    {
                        DrawGroundShadow(player.PosX, player.PosY, player.PosZ, teamColor);
                    }

                    // Draw velocity trail when moving fast
                    if (player.Speed > 2f)
                    {
                        DrawVelocityTrail(player, teamColor);
                    }

                    // Use biped model if available
                    if (playerBipedModel != null)
                    {
                        // Use Yaw (radians) directly if available, otherwise convert YawDeg
                        float yawRadians = player.Yaw != 0 ? player.Yaw : player.YawDeg * (float)(Math.PI / 180.0);
                        Matrix rotation = Matrix.RotationZ(yawRadians);
                        Matrix translation = Matrix.Translation(player.PosX, player.PosY, player.PosZ + modelZOffset);

                        // Apply rotation then translation (model rotates around its own origin, then moves to position)
                        render.device.Transform.World = Matrix.Multiply(rotation, translation);

                        Material teamMaterial = new Material();
                        teamMaterial.Diffuse = teamColor;
                        teamMaterial.Ambient = teamColor;
                        teamMaterial.Emissive = Color.FromArgb(teamColor.R / 3, teamColor.G / 3, teamColor.B / 3);

                        render.device.RenderState.Lighting = true;
                        render.device.Material = teamMaterial;
                        ParsedModel.DisplayedInfo.Draw(ref render.device, playerBipedModel);
                    }
                    else
                    {
                        // Fall back to cylinder marker
                        if (playerMarkerMesh == null || playerMarkerMesh.Disposed)
                        {
                            playerMarkerMesh = Mesh.Cylinder(render.device, 0.2f, 0.1f, 0.7f, 8, 1);
                        }

                        Material mat = new Material();
                        mat.Diffuse = teamColor;
                        mat.Ambient = teamColor;
                        mat.Emissive = Color.FromArgb(teamColor.R / 2, teamColor.G / 2, teamColor.B / 2);

                        // Apply yaw rotation and position
                        // Use Yaw (radians) directly if available, otherwise convert YawDeg
                        float yawRadians = player.Yaw != 0 ? player.Yaw : player.YawDeg * (float)(Math.PI / 180.0);
                        Matrix yawRotation = Matrix.RotationZ(yawRadians);
                        Matrix tiltRotation = Matrix.RotationX((float)(Math.PI / 2));
                        Matrix translation = Matrix.Translation(player.PosX, player.PosY, player.PosZ + 0.35f + modelZOffset);
                        // Tilt to stand upright, rotate by yaw, then translate to position
                        render.device.Transform.World = Matrix.Multiply(Matrix.Multiply(tiltRotation, yawRotation), translation);
                        render.device.Material = mat;
                        render.device.SetTexture(0, null);
                        render.device.RenderState.FillMode = FillMode.Solid;
                        playerMarkerMesh.DrawSubset(0);
                    }
                }

                // Draw player name/emblem above head (passes isDead to show X when dead)
                DrawPlayerName(player, isDead);
            }
        }

        /// <summary>
        /// Draws a team color circle at player's feet.
        /// </summary>
        private void DrawTeamCircle(float x, float y, float z, Color teamColor)
        {
            // Create or reuse circle mesh - flat disc on XY plane
            if (teamCircleMesh == null || teamCircleMesh.Disposed)
            {
                // Create a flat cylinder (disc) - Mesh.Cylinder creates height along Y axis
                // Radius 0.125 (4x smaller than original 0.5)
                teamCircleMesh = Mesh.Cylinder(render.device, 0.125f, 0.125f, 0.02f, 24, 1);
            }

            Material circleMat = new Material();
            circleMat.Diffuse = teamColor;
            circleMat.Ambient = teamColor;
            circleMat.Emissive = Color.FromArgb(teamColor.R / 2, teamColor.G / 2, teamColor.B / 2);

            // Rotate 90 degrees around X axis to lay flat on XY plane (disc faces up in Z)
            // Then translate to player position
            Matrix rotation = Matrix.RotationX((float)(Math.PI / 2.0));
            Matrix translation = Matrix.Translation(x, y, z - 0.15f);
            render.device.Transform.World = Matrix.Multiply(rotation, translation);
            render.device.Material = circleMat;
            render.device.SetTexture(0, null);
            render.device.RenderState.Lighting = true;
            render.device.RenderState.AlphaBlendEnable = true;
            render.device.RenderState.SourceBlend = Blend.SourceAlpha;
            render.device.RenderState.DestinationBlend = Blend.InvSourceAlpha;
            teamCircleMesh.DrawSubset(0);
            render.device.RenderState.AlphaBlendEnable = false;
        }

        /// <summary>
        /// Draws the player name and emblem above their head.
        /// </summary>
        private void DrawPlayerName(PlayerTelemetry player, bool isDead = false)
        {
            try
            {
                // Create font if needed
                if (playerNameFont == null || playerNameFont.Disposed)
                {
                    // Try Highway Gothic first (Halo's UI font), then fallbacks
                    string[] fontNames = { "Highway Gothic", "Conduit ITC", "Eurostile", "Arial" };
                    System.Drawing.Font drawFont = null;
                    foreach (string fontName in fontNames)
                    {
                        try
                        {
                            drawFont = new System.Drawing.Font(fontName, 14, FontStyle.Bold);
                            if (drawFont.Name.Equals(fontName, StringComparison.OrdinalIgnoreCase) ||
                                drawFont.OriginalFontName.Equals(fontName, StringComparison.OrdinalIgnoreCase))
                            {
                                break; // Found the font
                            }
                            drawFont.Dispose();
                            drawFont = null;
                        }
                        catch { }
                    }
                    if (drawFont == null)
                        drawFont = new System.Drawing.Font("Arial", 14, FontStyle.Bold);
                    playerNameFont = new Microsoft.DirectX.Direct3D.Font(render.device, drawFont);
                }

                // Create sprite if needed
                if (emblemSprite == null || emblemSprite.Disposed)
                {
                    emblemSprite = new Sprite(render.device);
                }

                // Project 3D position to screen coordinates
                Vector3 worldPos = new Vector3(player.PosX, player.PosY, player.PosZ + 1.5f); // Above head
                Vector3 screenPos = Vector3.Project(worldPos,
                    render.device.Viewport,
                    render.device.Transform.Projection,
                    render.device.Transform.View,
                    Matrix.Identity);

                // Only draw if in front of camera
                if (screenPos.Z > 0 && screenPos.Z < 1)
                {
                    Color teamColor = GetTeamColor(player.Team);
                    int centerX = (int)screenPos.X;
                    int topY = (int)screenPos.Y;

                    // Determine border color based on events
                    Color borderColor = Color.White; // Default
                    if (!string.IsNullOrEmpty(player.Event))
                    {
                        string evt = player.Event.ToLowerInvariant();
                        if (evt.Contains("fire") || evt.Contains("shot") || evt.Contains("shoot"))
                        {
                            borderColor = Color.Yellow; // Shooting
                        }
                        else if (evt.Contains("damage") || evt.Contains("hit") || evt.Contains("hurt"))
                        {
                            borderColor = Color.Red; // Taking damage
                        }
                    }

                    // Get emblem texture (load async if not cached)
                    string emblemKey = GetEmblemKey(player);
                    Texture emblemTexture = GetOrLoadEmblemTexture(player, emblemKey);

                    int emblemSize = 32;
                    int emblemX = centerX - emblemSize / 2;
                    int emblemY = topY - emblemSize - 8;

                    if (isDead)
                    {
                        // Draw bold red X for dead players (replaces emblem)
                        playerNameFont.DrawText(null, "X",
                            new System.Drawing.Rectangle(emblemX, emblemY, emblemSize, emblemSize),
                            DrawTextFormat.Center | DrawTextFormat.VerticalCenter | DrawTextFormat.NoClip, Color.Red);
                    }
                    else
                    {
                        // Draw border rectangle using Line (filled box)
                        using (Line line = new Line(render.device))
                        {
                            line.Width = emblemSize + 8;
                            line.Begin();
                            // Draw border
                            Vector2[] borderPts = {
                                new Vector2(emblemX - 4 + (emblemSize + 8) / 2, emblemY - 4),
                                new Vector2(emblemX - 4 + (emblemSize + 8) / 2, emblemY + emblemSize + 4)
                            };
                            line.Draw(borderPts, borderColor);
                            line.End();

                            // Draw inner fill
                            line.Width = emblemSize;
                            line.Begin();
                            Vector2[] innerPts = {
                                new Vector2(centerX, emblemY),
                                new Vector2(centerX, emblemY + emblemSize)
                            };
                            line.Draw(innerPts, teamColor);
                            line.End();
                        }

                        // Draw emblem texture if available (scale to fit emblemSize)
                        if (emblemTexture != null && !emblemTexture.Disposed)
                        {
                            float emblemScale = emblemSize / 256.0f; // Scale to 25% of current
                            emblemSprite.Begin(SpriteFlags.AlphaBlend);
                            emblemSprite.Transform = Matrix.Scaling(emblemScale, emblemScale, 1f) * Matrix.Translation(emblemX, emblemY, 0);
                            emblemSprite.Draw(emblemTexture, Vector3.Empty, Vector3.Empty, Color.White.ToArgb());
                            emblemSprite.Transform = Matrix.Identity;
                            emblemSprite.End();
                        }
                    }

                    // Draw player name
                    int nameY = topY;
                    playerNameFont.DrawText(null, player.PlayerName,
                        new System.Drawing.Rectangle(centerX - 80, nameY, 160, 24),
                        DrawTextFormat.Center | DrawTextFormat.NoClip, teamColor);

                    // Draw weapon icon after player name (scaled to 16px)
                    Texture weaponTexture = GetOrLoadWeaponTexture(player.CurrentWeapon);
                    if (weaponTexture != null && !weaponTexture.Disposed)
                    {
                        float scale = 0.25f; // Scale down weapon icons
                        emblemSprite.Begin(SpriteFlags.AlphaBlend);
                        emblemSprite.Transform = Matrix.Scaling(scale, scale, 1f) * Matrix.Translation(centerX + 50, nameY - 4, 0);
                        emblemSprite.Draw(weaponTexture, Vector3.Empty, Vector3.Empty, Color.White.ToArgb());
                        emblemSprite.Transform = Matrix.Identity; // Reset
                        emblemSprite.End();
                    }

                    // Draw blue waypoint arrow below name pointing down at player
                    int arrowY = nameY + 20;
                    playerNameFont.DrawText(null, "▼",
                        new System.Drawing.Rectangle(centerX - 20, arrowY, 40, 20),
                        DrawTextFormat.Center | DrawTextFormat.NoClip, Color.CornflowerBlue);
                }
            }
            catch
            {
                // Rendering failed, ignore
            }
        }

        /// <summary>
        /// Draws HUD overlay elements (FPS counter, mode indicator, etc.)
        /// </summary>
        private void DrawHUD()
        {
            // Update FPS counter
            fpsFrameCount++;
            double elapsed = (DateTime.Now - fpsLastUpdate).TotalSeconds;
            if (elapsed >= 1.0)
            {
                currentFps = (float)(fpsFrameCount / elapsed);
                fpsFrameCount = 0;
                fpsLastUpdate = DateTime.Now;
            }

            // Create font if needed - use smaller size for better fit
            if (fpsFont == null || fpsFont.Disposed)
            {
                fpsFont = new Microsoft.DirectX.Direct3D.Font(render.device, new System.Drawing.Font("Segoe UI", 11, FontStyle.Bold));
            }

            // Halo blue color scheme
            Color haloBlue = Color.FromArgb(0, 170, 255);
            Color haloCyan = Color.FromArgb(0, 220, 255);

            int screenWidth = render.device.Viewport.Width;
            int screenHeight = render.device.Viewport.Height;

            // Ensure FPS stays within bounds with padding
            int padding = 10;
            int fpsWidth = 80;

            // Draw FPS in top right - clamp to visible area
            string fpsText = $"{currentFps:F0} FPS";
            int fpsX = Math.Max(padding, screenWidth - fpsWidth - padding);
            Rectangle fpsRect = new Rectangle(fpsX, padding, fpsWidth, 24);
            fpsFont.DrawText(null, fpsText, fpsRect, DrawTextFormat.Right | DrawTextFormat.Top, haloBlue);

            // Draw mode indicator in top left
            string modeText = showLiveTelemetry ? "* LIVE" : (playerPath.Count > 0 ? "> REPLAY" : "");
            if (!string.IsNullOrEmpty(modeText))
            {
                Rectangle modeRect = new Rectangle(padding, padding, 100, 24);
                Color modeColor = showLiveTelemetry ? Color.FromArgb(255, 80, 80) : haloCyan;
                fpsFont.DrawText(null, modeText, modeRect, DrawTextFormat.Left | DrawTextFormat.Top, modeColor);
            }

            // Draw playback info when in replay mode - position above timeline
            if (!showLiveTelemetry && playerPath.Count > 0)
            {
                string timeText = FormatTime(pathCurrentTimestamp - pathMinTimestamp) + " / " + FormatTime(pathMaxTimestamp - pathMinTimestamp);
                string speedText = $"{pathPlaybackSpeed:F2}x";
                int bottomY = Math.Max(60, screenHeight - 70);
                Rectangle timeRect = new Rectangle(padding, bottomY, 150, 20);
                Rectangle speedRect = new Rectangle(Math.Max(padding, screenWidth - 60 - padding), bottomY, 60, 20);
                fpsFont.DrawText(null, timeText, timeRect, DrawTextFormat.Left, haloCyan);
                fpsFont.DrawText(null, speedText, speedRect, DrawTextFormat.Right, haloBlue);
            }

            // Draw player count below FPS
            int playerCount = showLiveTelemetry ? livePlayerNames.Count : pathPlayerNames.Count;
            if (playerCount > 0)
            {
                string playerText = $"[{playerCount}]";
                int playerX = Math.Max(padding, screenWidth - 50 - padding);
                Rectangle playerRect = new Rectangle(playerX, padding + 24, 50, 20);
                fpsFont.DrawText(null, playerText, playerRect, DrawTextFormat.Right, haloCyan);
            }
        }

        /// <summary>
        /// Draws the scoreboard overlay matching HTML style.
        /// </summary>
        private void DrawScoreboard()
        {
            try
            {
                // Create fonts if needed
                if (scoreboardFont == null || scoreboardFont.Disposed)
                {
                    scoreboardFont = new Microsoft.DirectX.Direct3D.Font(render.device,
                        new System.Drawing.Font("Overpass", 12, FontStyle.Regular));
                }
                if (scoreboardHeaderFont == null || scoreboardHeaderFont.Disposed)
                {
                    scoreboardHeaderFont = new Microsoft.DirectX.Direct3D.Font(render.device,
                        new System.Drawing.Font("Overpass", 14, FontStyle.Bold));
                }

                int screenWidth = render.device.Viewport.Width;
                int screenHeight = render.device.Viewport.Height;

                // Scoreboard position (top-left with padding)
                int sbX = 20;
                int sbY = 60;
                int sbWidth = 340;
                int rowHeight = 24;
                int headerHeight = 28;

                // Team colors matching HTML
                Color redTeamBg = Color.FromArgb(200, 197, 66, 69);    // #C54245 with alpha
                Color blueTeamBg = Color.FromArgb(200, 65, 105, 168);  // #4169A8 with alpha
                Color textColor = Color.White;

                // Get player data
                Dictionary<string, PlayerTelemetry> players = new Dictionary<string, PlayerTelemetry>();
                lock (livePlayersLock)
                {
                    if (showLiveTelemetry)
                    {
                        players = new Dictionary<string, PlayerTelemetry>(livePlayers);
                    }
                    else
                    {
                        // Build from path data - get latest point for each player
                        foreach (var kvp in multiPlayerPaths)
                        {
                            var allPoints = kvp.Value.SelectMany(seg => seg).ToList();
                            var latest = allPoints.LastOrDefault(p => p.Timestamp <= pathCurrentTimestamp);
                            if (!string.IsNullOrEmpty(latest.PlayerName))
                            {
                                players[kvp.Key] = new PlayerTelemetry
                                {
                                    PlayerName = latest.PlayerName,
                                    Team = latest.Team,
                                    IsDead = latest.IsDead,
                                    CurrentWeapon = latest.CurrentWeapon,
                                    EmblemFg = latest.EmblemFg,
                                    EmblemBg = latest.EmblemBg,
                                    ColorPrimary = latest.ColorPrimary,
                                    ColorSecondary = latest.ColorSecondary,
                                    ColorTertiary = latest.ColorTertiary,
                                    ColorQuaternary = latest.ColorQuaternary
                                };
                            }
                        }
                    }
                }

                // Separate by team
                var redPlayers = players.Values.Where(p => p.Team == 0).OrderByDescending(p => p.Kills).ToList();
                var bluePlayers = players.Values.Where(p => p.Team == 1).OrderByDescending(p => p.Kills).ToList();

                int currentY = sbY;

                // Calculate team scores (sum of kills for slayer)
                int redScore = redPlayers.Sum(p => p.Kills);
                int blueScore = bluePlayers.Sum(p => p.Kills);

                // Draw Red Team Header
                if (redPlayers.Count > 0)
                {
                    DrawFilledRect(sbX, currentY, sbWidth, headerHeight, redTeamBg);
                    scoreboardHeaderFont.DrawText(null, "Red Team",
                        new Rectangle(sbX + 10, currentY + 4, sbWidth - 60, headerHeight),
                        DrawTextFormat.Left | DrawTextFormat.VerticalCenter, textColor);
                    scoreboardHeaderFont.DrawText(null, redScore.ToString(),
                        new Rectangle(sbX, currentY + 4, sbWidth - 10, headerHeight),
                        DrawTextFormat.Right | DrawTextFormat.VerticalCenter, textColor);
                    currentY += headerHeight;
                }

                // Draw Blue Team Header
                if (bluePlayers.Count > 0)
                {
                    DrawFilledRect(sbX, currentY, sbWidth, headerHeight, blueTeamBg);
                    scoreboardHeaderFont.DrawText(null, "Blue Team",
                        new Rectangle(sbX + 10, currentY + 4, sbWidth - 60, headerHeight),
                        DrawTextFormat.Left | DrawTextFormat.VerticalCenter, textColor);
                    scoreboardHeaderFont.DrawText(null, blueScore.ToString(),
                        new Rectangle(sbX, currentY + 4, sbWidth - 10, headerHeight),
                        DrawTextFormat.Right | DrawTextFormat.VerticalCenter, textColor);
                    currentY += headerHeight;
                }

                // Gap between headers and players
                currentY += 6;

                // Draw Red Team Players
                foreach (var player in redPlayers)
                {
                    DrawPlayerRow(sbX, currentY, sbWidth, rowHeight, player, redTeamBg);
                    currentY += rowHeight + 2;
                }

                // Draw Blue Team Players
                foreach (var player in bluePlayers)
                {
                    DrawPlayerRow(sbX, currentY, sbWidth, rowHeight, player, blueTeamBg);
                    currentY += rowHeight + 2;
                }
            }
            catch (Exception ex)
            {
                AddDebugLog($"[SCOREBOARD] Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Draws a single player row in the scoreboard.
        /// </summary>
        private void DrawPlayerRow(int x, int y, int width, int height, PlayerTelemetry player, Color bgColor)
        {
            // Draw background
            DrawFilledRect(x, y, width, height, bgColor);

            int currentX = x + 4;

            // Draw emblem or dead X (22x22)
            int emblemSize = 22;
            if (player.IsDead)
            {
                scoreboardFont.DrawText(null, "X",
                    new Rectangle(currentX, y, emblemSize, height),
                    DrawTextFormat.Center | DrawTextFormat.VerticalCenter, Color.Red);
            }
            else
            {
                // Draw emblem texture if available
                string emblemKey = $"{player.EmblemFg}_{player.EmblemBg}_{player.ColorPrimary}_{player.ColorSecondary}_{player.ColorTertiary}_{player.ColorQuaternary}";
                if (emblemTextureCache.TryGetValue(emblemKey, out Texture emblemTex) && emblemTex != null && !emblemTex.Disposed)
                {
                    if (emblemSprite == null || emblemSprite.Disposed)
                    {
                        emblemSprite = new Sprite(render.device);
                    }
                    float scale = emblemSize / 256.0f;
                    emblemSprite.Begin(SpriteFlags.AlphaBlend);
                    emblemSprite.Transform = Matrix.Scaling(scale, scale, 1f) * Matrix.Translation(currentX, y + 1, 0);
                    emblemSprite.Draw(emblemTex, Vector3.Empty, Vector3.Empty, Color.White.ToArgb());
                    emblemSprite.Transform = Matrix.Identity;
                    emblemSprite.End();
                }
            }
            currentX += emblemSize + 4;

            // Draw player name
            int nameWidth = 180;
            scoreboardFont.DrawText(null, player.PlayerName ?? "Unknown",
                new Rectangle(currentX, y, nameWidth, height),
                DrawTextFormat.Left | DrawTextFormat.VerticalCenter | DrawTextFormat.NoClip, Color.White);
            currentX += nameWidth;

            // Draw K/D/A stats
            string stats = $"K {player.Kills}  D {player.Deaths}";
            scoreboardFont.DrawText(null, stats,
                new Rectangle(currentX, y, width - currentX - 4, height),
                DrawTextFormat.Right | DrawTextFormat.VerticalCenter, Color.White);
        }

        /// <summary>
        /// Draws a filled rectangle for UI backgrounds.
        /// </summary>
        private void DrawFilledRect(int x, int y, int width, int height, Color color)
        {
            // Use a simple line-based approach since we don't have a rectangle mesh
            // This is less efficient but works without additional setup
            CustomVertex.TransformedColored[] verts = new CustomVertex.TransformedColored[4];
            verts[0] = new CustomVertex.TransformedColored(x, y, 0, 1, color.ToArgb());
            verts[1] = new CustomVertex.TransformedColored(x + width, y, 0, 1, color.ToArgb());
            verts[2] = new CustomVertex.TransformedColored(x, y + height, 0, 1, color.ToArgb());
            verts[3] = new CustomVertex.TransformedColored(x + width, y + height, 0, 1, color.ToArgb());

            render.device.VertexFormat = CustomVertex.TransformedColored.Format;
            render.device.SetTexture(0, null);
            render.device.RenderState.AlphaBlendEnable = true;
            render.device.RenderState.SourceBlend = Blend.SourceAlpha;
            render.device.RenderState.DestinationBlend = Blend.InvSourceAlpha;
            render.device.DrawUserPrimitives(PrimitiveType.TriangleStrip, 2, verts);
        }

        /// <summary>
        /// Draws the killfeed overlay.
        /// </summary>
        private void DrawKillfeed()
        {
            try
            {
                if (scoreboardFont == null || scoreboardFont.Disposed)
                {
                    scoreboardFont = new Microsoft.DirectX.Direct3D.Font(render.device,
                        new System.Drawing.Font("Overpass", 12, FontStyle.Regular));
                }

                int screenWidth = render.device.Viewport.Width;
                int kfX = screenWidth - 320;
                int kfY = 60;
                int rowHeight = 22;

                // Get recent kills from killEvents
                var recentKills = killEvents
                    .Where(k => showLiveTelemetry || k.Timestamp <= pathCurrentTimestamp)
                    .OrderByDescending(k => k.Timestamp)
                    .Take(8)
                    .Reverse()
                    .ToList();

                int currentY = kfY;
                foreach (var kill in recentKills)
                {
                    // Draw killfeed entry: "Player got a kill with Weapon"
                    Color teamColor = kill.Team == 0 ? Color.FromArgb(197, 66, 69) :
                                     kill.Team == 1 ? Color.FromArgb(65, 105, 168) : Color.White;

                    string entry = $"{kill.PlayerName} [{kill.Weapon}]";
                    DrawFilledRect(kfX, currentY, 300, rowHeight, Color.FromArgb(150, 0, 0, 0));
                    scoreboardFont.DrawText(null, entry,
                        new Rectangle(kfX + 8, currentY, 290, rowHeight),
                        DrawTextFormat.Left | DrawTextFormat.VerticalCenter, teamColor);

                    currentY += rowHeight + 2;
                }
            }
            catch (Exception ex)
            {
                AddDebugLog($"[KILLFEED] Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Formats a timestamp as MM:SS.
        /// </summary>
        private string FormatTime(float seconds)
        {
            int mins = (int)(seconds / 60);
            int secs = (int)(seconds % 60);
            return $"{mins}:{secs:D2}";
        }

        /// <summary>
        /// Gets a unique key for an emblem based on player colors.
        /// </summary>
        private string GetEmblemKey(PlayerTelemetry player)
        {
            return $"{player.EmblemFg}_{player.EmblemBg}_{player.ColorPrimary}_{player.ColorSecondary}";
        }

        /// <summary>
        /// Gets the emblem URL for a player.
        /// </summary>
        private string GetEmblemUrl(PlayerTelemetry player)
        {
            // P=primary, S=secondary, EP=emblem primary, ES=emblem secondary, EF=foreground, EB=background, ET=toggle
            return $"http://104.207.143.249:3001/P{player.ColorPrimary}-S{player.ColorSecondary}-EP{player.ColorTertiary}-ES{player.ColorQuaternary}-EF{player.EmblemFg}-EB{player.EmblemBg}-ET0.png";
        }

        /// <summary>
        /// Tracks failed emblem loads to avoid retrying too quickly.
        /// </summary>
        private Dictionary<string, DateTime> emblemFailedCache = new Dictionary<string, DateTime>();

        /// <summary>
        /// Gets or loads an emblem texture, caching it for reuse.
        /// </summary>
        private Texture GetOrLoadEmblemTexture(PlayerTelemetry player, string emblemKey)
        {
            // Return cached texture if available
            if (emblemTextureCache.ContainsKey(emblemKey))
            {
                return emblemTextureCache[emblemKey];
            }

            // Check if this emblem recently failed - wait 60 seconds before retry
            if (emblemFailedCache.ContainsKey(emblemKey))
            {
                if ((DateTime.Now - emblemFailedCache[emblemKey]).TotalSeconds < 60)
                    return null;
                emblemFailedCache.Remove(emblemKey);
            }

            // Start async load if not already loading
            if (!emblemLoadingSet.Contains(emblemKey))
            {
                emblemLoadingSet.Add(emblemKey);
                string url = GetEmblemUrl(player);
                AddDebugLog($"[EMBLEM] Loading: {url}");

                // Load emblem in background thread with retry
                System.Threading.ThreadPool.QueueUserWorkItem(_ =>
                {
                    int maxRetries = 2;
                    int retryDelay = 1000;

                    for (int attempt = 0; attempt <= maxRetries; attempt++)
                    {
                        try
                        {
                            using (var webClient = new System.Net.WebClient())
                            {
                                // Add headers to help with Cloudflare
                                webClient.Headers.Add("User-Agent", "Entity-BSPViewer/1.0");
                                webClient.Headers.Add("Accept", "image/png,image/*");

                                byte[] imageData = webClient.DownloadData(url);
                                AddDebugLog($"[EMBLEM] Downloaded {imageData.Length} bytes for {player.PlayerName}");

                                // Must create texture on main thread
                                this.BeginInvoke(new System.Action(() =>
                                {
                                    try
                                    {
                                        using (var ms = new System.IO.MemoryStream(imageData))
                                        {
                                            Texture tex = TextureLoader.FromStream(render.device, ms);
                                            emblemTextureCache[emblemKey] = tex;
                                            AddDebugLog($"[EMBLEM] Texture created for {player.PlayerName}");
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        AddDebugLog($"[EMBLEM] Texture error: {ex.Message}");
                                        emblemFailedCache[emblemKey] = DateTime.Now;
                                    }
                                    finally
                                    {
                                        emblemLoadingSet.Remove(emblemKey);
                                    }
                                }));
                                return; // Success, exit retry loop
                            }
                        }
                        catch (System.Net.WebException wex)
                        {
                            var response = wex.Response as System.Net.HttpWebResponse;
                            int statusCode = response != null ? (int)response.StatusCode : 0;
                            AddDebugLog($"[EMBLEM] Download error (attempt {attempt + 1}): HTTP {statusCode} - {wex.Message}");

                            // Don't retry on 4xx errors (client errors)
                            if (statusCode >= 400 && statusCode < 500)
                            {
                                emblemFailedCache[emblemKey] = DateTime.Now;
                                emblemLoadingSet.Remove(emblemKey);
                                return;
                            }

                            if (attempt < maxRetries)
                            {
                                System.Threading.Thread.Sleep(retryDelay);
                                retryDelay *= 2; // Exponential backoff
                            }
                        }
                        catch (Exception ex)
                        {
                            AddDebugLog($"[EMBLEM] Download error (attempt {attempt + 1}): {ex.Message}");
                            if (attempt < maxRetries)
                            {
                                System.Threading.Thread.Sleep(retryDelay);
                                retryDelay *= 2;
                            }
                        }
                    }

                    // All retries failed
                    emblemFailedCache[emblemKey] = DateTime.Now;
                    emblemLoadingSet.Remove(emblemKey);
                });
            }

            return null; // Not loaded yet
        }

        /// <summary>
        /// Maps a weapon name from telemetry to a GitHub image filename.
        /// </summary>
        private string GetWeaponImageName(string weaponName)
        {
            if (string.IsNullOrEmpty(weaponName))
                return null;

            // Map common weapon names to image filenames
            string name = weaponName.ToLowerInvariant().Trim();

            if (name.Contains("battle") || name.Contains("br")) return "BattleRifle";
            if (name.Contains("sniper")) return "SniperRifle";
            if (name.Contains("rocket")) return "RocketLauncher";
            if (name.Contains("shotgun")) return "Shotgun";
            if (name.Contains("smg") || name.Contains("sub")) return "SmG";
            if (name.Contains("magnum") || name.Contains("pistol") && !name.Contains("plasma")) return "Magnum";
            if (name.Contains("sword") || name.Contains("energy")) return "EnergySword";
            if (name.Contains("needler")) return "Needler";
            if (name.Contains("carbine")) return "Carbine";
            if (name.Contains("beam") && name.Contains("rifle")) return "BeamRifle";
            if (name.Contains("brute") && name.Contains("shot")) return "BruteShot";
            if (name.Contains("brute") && name.Contains("plasma")) return "BrutePlasmaRifle";
            if (name.Contains("plasma") && name.Contains("pistol")) return "PlasmaPistol";
            if (name.Contains("plasma") && name.Contains("rifle")) return "PlasmaRifle";
            if (name.Contains("fuel") || name.Contains("rod")) return "FuelRod";
            if (name.Contains("sentinel")) return "SentinelBeam";
            if (name.Contains("flag")) return "Flag";
            if (name.Contains("ball") || name.Contains("odd")) return "OddBall";
            if (name.Contains("bomb")) return "AssaultBomb";
            if (name.Contains("frag") || name.Contains("grenade")) return "H2-M9HEDPFragmentationGrenade";

            return null;
        }

        /// <summary>
        /// Gets or loads a weapon texture from GitHub.
        /// </summary>
        private Texture GetOrLoadWeaponTexture(string weaponName)
        {
            string imageName = GetWeaponImageName(weaponName);
            if (imageName == null)
                return null;

            // Return cached texture if available
            if (weaponTextureCache.ContainsKey(imageName))
            {
                return weaponTextureCache[imageName];
            }

            // Start async load if not already loading
            if (!weaponLoadingSet.Contains(imageName))
            {
                weaponLoadingSet.Add(imageName);
                string url = $"https://raw.githubusercontent.com/i2aMpAnT/CarnageReport.com/main/assets/weapons/{imageName}.png";

                System.Threading.ThreadPool.QueueUserWorkItem(_ =>
                {
                    try
                    {
                        using (var webClient = new System.Net.WebClient())
                        {
                            byte[] imageData = webClient.DownloadData(url);
                            this.BeginInvoke(new System.Action(() =>
                            {
                                try
                                {
                                    using (var ms = new System.IO.MemoryStream(imageData))
                                    {
                                        Texture tex = TextureLoader.FromStream(render.device, ms);
                                        weaponTextureCache[imageName] = tex;
                                    }
                                }
                                catch { }
                                finally
                                {
                                    weaponLoadingSet.Remove(imageName);
                                }
                            }));
                        }
                    }
                    catch
                    {
                        weaponLoadingSet.Remove(imageName);
                    }
                });
            }

            return null;
        }

        /// <summary>
        /// Draws a ground shadow circle when player is airborne.
        /// </summary>
        private void DrawGroundShadow(float x, float y, float z, Color teamColor)
        {
            // Create a flat disc for shadow
            Mesh shadowMesh = Mesh.Cylinder(render.device, 0.3f, 0.3f, 0.01f, 16, 1);

            Material shadowMat = new Material();
            shadowMat.Diffuse = Color.FromArgb(100, 0, 0, 0); // Semi-transparent black
            shadowMat.Ambient = Color.FromArgb(100, 0, 0, 0);

            // Position shadow on ground (assume Z=0 or find ground level)
            render.device.Transform.World = Matrix.RotationX((float)(Math.PI / 2)) * Matrix.Translation(x, y, 0.01f);
            render.device.Material = shadowMat;
            render.device.SetTexture(0, null);
            render.device.RenderState.AlphaBlendEnable = true;
            render.device.RenderState.SourceBlend = Blend.SourceAlpha;
            render.device.RenderState.DestinationBlend = Blend.InvSourceAlpha;
            shadowMesh.DrawSubset(0);
            render.device.RenderState.AlphaBlendEnable = false;
            shadowMesh.Dispose();
        }

        /// <summary>
        /// Draws a velocity trail behind moving players.
        /// </summary>
        private void DrawVelocityTrail(PlayerTelemetry player, Color teamColor)
        {
            // Draw a line from current position backwards along velocity
            float trailLength = Math.Min(player.Speed * 0.3f, 2f);
            float vx = player.VelX / Math.Max(player.Speed, 0.1f);
            float vy = player.VelY / Math.Max(player.Speed, 0.1f);

            // Create trail points
            CustomVertex.PositionColored[] trailVerts = new CustomVertex.PositionColored[4];
            Color trailColor = Color.FromArgb(150, teamColor.R, teamColor.G, teamColor.B);

            // Trail start (behind player)
            float startX = player.PosX - vx * trailLength;
            float startY = player.PosY - vy * trailLength;

            trailVerts[0] = new CustomVertex.PositionColored(startX - vy * 0.1f, startY + vx * 0.1f, player.PosZ + 0.3f, trailColor.ToArgb());
            trailVerts[1] = new CustomVertex.PositionColored(startX + vy * 0.1f, startY - vx * 0.1f, player.PosZ + 0.3f, trailColor.ToArgb());
            trailVerts[2] = new CustomVertex.PositionColored(player.PosX - vy * 0.05f, player.PosY + vx * 0.05f, player.PosZ + 0.3f, teamColor.ToArgb());
            trailVerts[3] = new CustomVertex.PositionColored(player.PosX + vy * 0.05f, player.PosY - vx * 0.05f, player.PosZ + 0.3f, teamColor.ToArgb());

            render.device.Transform.World = Matrix.Identity;
            render.device.SetTexture(0, null);
            render.device.VertexFormat = CustomVertex.PositionColored.Format;
            render.device.RenderState.Lighting = false;
            render.device.RenderState.AlphaBlendEnable = true;
            render.device.RenderState.SourceBlend = Blend.SourceAlpha;
            render.device.RenderState.DestinationBlend = Blend.InvSourceAlpha;
            render.device.DrawUserPrimitives(PrimitiveType.TriangleStrip, 2, trailVerts);
            render.device.RenderState.AlphaBlendEnable = false;
            render.device.RenderState.Lighting = true;
        }

        /// <summary>
        /// Draws an event indicator (weapon fire, melee, grenade, etc.)
        /// </summary>
        private void DrawEventIndicator(PlayerTelemetry player, Color teamColor)
        {
            string evt = player.Event.ToLowerInvariant();
            Color eventColor = Color.White;
            float size = 0.2f;

            // Determine event type and color
            if (evt.Contains("fire") || evt.Contains("shot"))
            {
                eventColor = Color.Yellow; // Muzzle flash
                size = 0.3f;
            }
            else if (evt.Contains("melee"))
            {
                eventColor = Color.Orange;
                size = 0.25f;
            }
            else if (evt.Contains("grenade") || evt.Contains("frag") || evt.Contains("plasma"))
            {
                eventColor = Color.Lime;
                size = 0.35f;
            }
            else if (evt.Contains("damage") || evt.Contains("hit"))
            {
                eventColor = Color.Red;
                size = 0.2f;
            }
            else if (evt.Contains("death") || evt.Contains("kill"))
            {
                eventColor = Color.DarkRed;
                size = 0.5f;
            }
            else if (evt.Contains("reload"))
            {
                eventColor = Color.Cyan;
                size = 0.15f;
            }

            // Draw a sphere at player position + offset in facing direction
            Mesh eventMesh = Mesh.Sphere(render.device, size, 8, 8);

            Material eventMat = new Material();
            eventMat.Diffuse = eventColor;
            eventMat.Ambient = eventColor;
            eventMat.Emissive = eventColor; // Make it glow

            // Position in front of player at weapon height
            float offsetDist = 0.5f;
            float fx = player.PosX + (float)Math.Cos(player.Yaw) * offsetDist;
            float fy = player.PosY + (float)Math.Sin(player.Yaw) * offsetDist;

            render.device.Transform.World = Matrix.Translation(fx, fy, player.PosZ + 0.5f);
            render.device.Material = eventMat;
            render.device.SetTexture(0, null);
            render.device.RenderState.Lighting = true;
            eventMesh.DrawSubset(0);
            eventMesh.Dispose();
        }

        /// <summary>
        /// Event handler for Listen button click.
        /// </summary>
        private void btnListen_Click(object sender, EventArgs e)
        {
            if (telemetryListenerRunning)
            {
                StopTelemetryListener();
                if (sender is ToolStripButton btn)
                    btn.Text = "Listen";
            }
            else
            {
                StartTelemetryListener();
                if (sender is ToolStripButton btn)
                    btn.Text = "Stop";
            }
        }

        #endregion

        /// <summary>
        /// Event handler for Load Path button click.
        /// </summary>
        private void btnLoadPath_Click(object sender, EventArgs e)
        {
            LoadPlayerPathDialog();
        }

        /// <summary>
        /// Event handler for Play/Pause button click.
        /// </summary>
        private void btnPlayPausePath_Click(object sender, EventArgs e)
        {
            TogglePathPlayback();
        }

        /// <summary>
        /// Event handler for Reset button click.
        /// </summary>
        private void btnResetPath_Click(object sender, EventArgs e)
        {
            ResetPathAnimation();
        }

        /// <summary>
        /// Event handler for path speed trackbar change.
        /// </summary>
        private void trackBarPathSpeed_ValueChanged(object sender, EventArgs e)
        {
            if (sender is TrackBar tb)
            {
                pathPlaybackSpeed = tb.Value / 10.0f; // 0.1x to 5.0x speed
            }
        }

        /// <summary>
        /// Event handler for timeline trackbar scroll.
        /// </summary>
        private void PathTimelineTrackBar_Scroll(object sender, EventArgs e)
        {
            if (pathTimelineTrackBar == null || pathMaxTimestamp <= pathMinTimestamp)
                return;

            // Calculate timestamp from trackbar position
            float t = pathTimelineTrackBar.Value / 1000.0f;
            pathCurrentTimestamp = pathMinTimestamp + t * (pathMaxTimestamp - pathMinTimestamp);
            pathTimeAccumulator = pathCurrentTimestamp;

            // Update path index for legacy single-player path
            if (playerPath.Count > 0)
            {
                pathCurrentIndex = 0;
                for (int i = 0; i < playerPath.Count - 1; i++)
                {
                    if (playerPath[i + 1].Timestamp > pathCurrentTimestamp)
                        break;
                    pathCurrentIndex = i + 1;
                }
            }

            UpdateTimelineLabel();
        }

        /// <summary>
        /// Paints kill markers and bookmark on the timeline panel.
        /// </summary>
        private void TimelinePanel_Paint(object sender, PaintEventArgs e)
        {
            if (pathTimelineTrackBar == null)
                return;

            float timeRange = pathMaxTimestamp - pathMinTimestamp;
            if (timeRange <= 0) return;

            // Get trackbar bounds for positioning markers
            int trackLeft = pathTimelineTrackBar.Left + 10;
            int trackWidth = pathTimelineTrackBar.Width - 20;
            int markerY = 4;
            int markerHeight = 12;

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // Draw kill markers
            foreach (var kill in killEvents)
            {
                // Calculate X position based on timestamp
                float t = (kill.Timestamp - pathMinTimestamp) / timeRange;
                int x = trackLeft + (int)(t * trackWidth);

                // Halo-style team colors (brighter, more saturated)
                Color teamColor;
                switch (kill.Team)
                {
                    case 0: teamColor = Color.FromArgb(255, 60, 60); break;    // Red team
                    case 1: teamColor = Color.FromArgb(60, 140, 255); break;   // Blue team
                    case 2: teamColor = Color.FromArgb(60, 255, 120); break;   // Green team
                    case 3: teamColor = Color.FromArgb(255, 180, 60); break;   // Orange team
                    default: teamColor = Color.FromArgb(0, 200, 255); break;   // FFA - cyan
                }

                // Draw glowing marker line
                using (Pen glowPen = new Pen(Color.FromArgb(80, teamColor), 4))
                {
                    e.Graphics.DrawLine(glowPen, x, markerY, x, markerY + markerHeight);
                }
                using (Pen pen = new Pen(teamColor, 2))
                {
                    e.Graphics.DrawLine(pen, x, markerY, x, markerY + markerHeight);
                }

                // Draw diamond marker
                Point[] diamond = {
                    new Point(x, markerY - 2),
                    new Point(x + 4, markerY + 3),
                    new Point(x, markerY + 8),
                    new Point(x - 4, markerY + 3)
                };
                using (SolidBrush brush = new SolidBrush(teamColor))
                {
                    e.Graphics.FillPolygon(brush, diamond);
                }
                using (Pen outlinePen = new Pen(Color.FromArgb(180, 255, 255, 255), 1))
                {
                    e.Graphics.DrawPolygon(outlinePen, diamond);
                }
            }

            // Draw A-B bookmark markers if set
            if (pathMaxTimestamp > pathMinTimestamp)
            {
                Color markerAColor = Color.FromArgb(0, 200, 255);
                Color markerBColor = Color.FromArgb(255, 180, 0);
                Color loopColor = Color.FromArgb(255, 215, 0);

                // Draw shaded region between A and B if both set
                if (bookmarkStartTimestamp >= 0 && bookmarkEndTimestamp >= 0)
                {
                    float tA = (bookmarkStartTimestamp - pathMinTimestamp) / timeRange;
                    float tB = (bookmarkEndTimestamp - pathMinTimestamp) / timeRange;
                    int xA = trackLeft + (int)(tA * trackWidth);
                    int xB = trackLeft + (int)(tB * trackWidth);

                    Color fillColor = bookmarkLoopEnabled
                        ? Color.FromArgb(60, 255, 215, 0)
                        : Color.FromArgb(40, 0, 200, 255);
                    using (SolidBrush brush = new SolidBrush(fillColor))
                    {
                        e.Graphics.FillRectangle(brush, xA, 5, xB - xA, 35);
                    }
                }

                // Draw marker A (start)
                if (bookmarkStartTimestamp >= 0)
                {
                    float t = (bookmarkStartTimestamp - pathMinTimestamp) / timeRange;
                    int bx = trackLeft + (int)(t * trackWidth);
                    Color color = bookmarkLoopEnabled ? loopColor : markerAColor;

                    using (Pen pen = new Pen(color, 2))
                    {
                        e.Graphics.DrawLine(pen, bx, 2, bx, 40);
                    }
                    using (System.Drawing.Font font = new System.Drawing.Font("Segoe UI", 7, FontStyle.Bold))
                    using (SolidBrush textBrush = new SolidBrush(color))
                    {
                        e.Graphics.DrawString("A", font, textBrush, bx - 4, 2);
                    }
                }

                // Draw marker B (end)
                if (bookmarkEndTimestamp >= 0)
                {
                    float t = (bookmarkEndTimestamp - pathMinTimestamp) / timeRange;
                    int bx = trackLeft + (int)(t * trackWidth);
                    Color color = bookmarkLoopEnabled ? loopColor : markerBColor;

                    using (Pen pen = new Pen(color, 2))
                    {
                        e.Graphics.DrawLine(pen, bx, 2, bx, 40);
                    }
                    using (System.Drawing.Font font = new System.Drawing.Font("Segoe UI", 7, FontStyle.Bold))
                    using (SolidBrush textBrush = new SolidBrush(color))
                    {
                        e.Graphics.DrawString("B", font, textBrush, bx - 4, 2);
                    }
                }
            }
        }

        /// <summary>
        /// Updates the bookmark button appearance based on A-B marker state.
        /// </summary>
        private void UpdateBookmarkButton()
        {
            if (bookmarkButton == null) return;

            if (bookmarkStartTimestamp >= 0 && bookmarkEndTimestamp >= 0)
            {
                // Both markers set - click will clear
                bookmarkButton.Text = "[Clear]";
                bookmarkButton.BackColor = Color.FromArgb(60, 80, 40);
                bookmarkButton.ForeColor = Color.FromArgb(255, 215, 0);
            }
            else if (bookmarkStartTimestamp >= 0)
            {
                // Only A set - waiting for B
                bookmarkButton.Text = "[B] Set";
                bookmarkButton.BackColor = Color.FromArgb(50, 70, 90);
                bookmarkButton.ForeColor = Color.FromArgb(255, 180, 0);
            }
            else
            {
                // Neither set - waiting for A
                bookmarkButton.Text = "[A] Set";
                bookmarkButton.BackColor = Color.FromArgb(30, 45, 60);
                bookmarkButton.ForeColor = Color.FromArgb(0, 200, 255);
            }
        }

        /// <summary>
        /// Updates the loop button appearance.
        /// </summary>
        private void UpdateLoopButton()
        {
            if (loopButton == null) return;

            bool hasValidLoop = bookmarkStartTimestamp >= 0 && bookmarkEndTimestamp >= 0;

            if (bookmarkLoopEnabled && hasValidLoop)
            {
                loopButton.BackColor = Color.FromArgb(60, 80, 40);
                loopButton.ForeColor = Color.FromArgb(255, 215, 0);
            }
            else
            {
                loopButton.BackColor = Color.FromArgb(30, 45, 60);
                loopButton.ForeColor = hasValidLoop ? Color.FromArgb(0, 200, 255) : Color.Gray;
            }
        }

        /// <summary>
        /// Jumps playback to the bookmark start position.
        /// </summary>
        private void JumpToBookmark()
        {
            if (bookmarkStartTimestamp < 0 || pathMaxTimestamp <= pathMinTimestamp) return;

            pathCurrentTimestamp = bookmarkStartTimestamp;
            pathTimeAccumulator = bookmarkStartTimestamp;

            for (int i = 0; i < playerPath.Count - 1; i++)
            {
                if (playerPath[i + 1].Timestamp > pathCurrentTimestamp)
                {
                    pathCurrentIndex = i;
                    break;
                }
            }

            UpdateTimelineLabel();
            UpdateTrackBarFromTimestamp();
        }

        /// <summary>
        /// Updates the timeline label with current/total time.
        /// </summary>
        private void UpdateTimelineLabel()
        {
            if (pathTimeLabel == null) return;

            float currentSecs = pathCurrentTimestamp - pathMinTimestamp;
            float totalSecs = pathMaxTimestamp - pathMinTimestamp;

            int curMins = (int)(currentSecs / 60);
            int curSecs = (int)(currentSecs % 60);
            int totMins = (int)(totalSecs / 60);
            int totSecs = (int)(totalSecs % 60);

            pathTimeLabel.Text = $"{curMins}:{curSecs:D2} / {totMins}:{totSecs:D2}";

            // Update trackbar position if not being dragged
            if (pathTimelineTrackBar != null && !pathTimelineTrackBar.Focused && pathMaxTimestamp > pathMinTimestamp)
            {
                float t = (pathCurrentTimestamp - pathMinTimestamp) / (pathMaxTimestamp - pathMinTimestamp);
                pathTimelineTrackBar.Value = Math.Max(0, Math.Min(1000, (int)(t * 1000)));
            }
        }

        /// <summary>
        /// The ts combo box.
        /// </summary>
        /// <remarks></remarks>
        public class TSComboBox : ToolStripComboBox
        {
            #region Methods

            /// <summary>
            /// The on key down.
            /// </summary>
            /// <param name="e">The e.</param>
            /// <remarks></remarks>
            protected override void OnKeyDown(KeyEventArgs e)
            {
                e.Handled = true;
            }

            /// <summary>
            /// The on key press.
            /// </summary>
            /// <param name="e">The e.</param>
            /// <remarks></remarks>
            protected override void OnKeyPress(KeyPressEventArgs e)
            {
                e.Handled = true;
            }

            #endregion
        }

        /// <summary>
        /// The ts text box.
        /// </summary>
        /// <remarks></remarks>
        public class TSTextBox : ToolStripTextBox
        {
            #region Methods

            /// <summary>
            /// The on key down.
            /// </summary>
            /// <param name="e">The e.</param>
            /// <remarks></remarks>
            protected override void OnKeyDown(KeyEventArgs e)
            {
                e.Handled = true;
            }

            /// <summary>
            /// The on key press.
            /// </summary>
            /// <param name="e">The e.</param>
            /// <remarks></remarks>
            protected override void OnKeyPress(KeyPressEventArgs e)
            {
                e.Handled = true;
            }

            #endregion
        }
    }

    /// <summary>
    /// Extension methods for Graphics class.
    /// </summary>
    public static class GraphicsExtensions
    {
        /// <summary>
        /// Fills a rounded rectangle.
        /// </summary>
        public static void FillRoundedRectangle(this Graphics g, Brush brush, int x, int y, int width, int height, int radius)
        {
            if (radius <= 0)
            {
                g.FillRectangle(brush, x, y, width, height);
                return;
            }

            radius = Math.Min(radius, Math.Min(width, height) / 2);

            using (System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath())
            {
                path.AddArc(x, y, radius * 2, radius * 2, 180, 90);
                path.AddArc(x + width - radius * 2, y, radius * 2, radius * 2, 270, 90);
                path.AddArc(x + width - radius * 2, y + height - radius * 2, radius * 2, radius * 2, 0, 90);
                path.AddArc(x, y + height - radius * 2, radius * 2, radius * 2, 90, 90);
                path.CloseFigure();
                g.FillPath(brush, path);
            }
        }

        /// <summary>
        /// Draws a rounded rectangle outline.
        /// </summary>
        public static void DrawRoundedRectangle(this Graphics g, Pen pen, int x, int y, int width, int height, int radius)
        {
            if (radius <= 0)
            {
                g.DrawRectangle(pen, x, y, width, height);
                return;
            }

            radius = Math.Min(radius, Math.Min(width, height) / 2);

            using (System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath())
            {
                path.AddArc(x, y, radius * 2, radius * 2, 180, 90);
                path.AddArc(x + width - radius * 2, y, radius * 2, radius * 2, 270, 90);
                path.AddArc(x + width - radius * 2, y + height - radius * 2, radius * 2, radius * 2, 0, 90);
                path.AddArc(x, y + height - radius * 2, radius * 2, radius * 2, 90, 90);
                path.CloseFigure();
                g.DrawPath(pen, path);
            }
        }
    }
}