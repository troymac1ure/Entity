// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Renderer.cs" company="">
//   
// </copyright>
// <summary>
//   The renderer.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HaloMap.Render
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Windows.Forms;

    using HaloMap.H2MetaContainers;
    using HaloMap.RawData;

    using Microsoft.DirectX;
    using Microsoft.DirectX.Direct3D;

    /// <summary>
    /// The renderer.
    /// </summary>
    /// <remarks></remarks>
    public class Renderer : IDisposable
    {
        #region Constants and Fields

        /// <summary>
        /// The device.
        /// </summary>
        public Device device; // Our rendering device

        /// <summary>
        /// The lighting.
        /// </summary>
        public bool lighting;

        /// <summary>
        /// The pause.
        /// </summary>
        public bool pause;

        #endregion

        #region Public Methods

        /// <summary>
        /// The decompress indices.
        /// </summary>
        /// <param name="indices">The indices.</param>
        /// <param name="start">The start.</param>
        /// <param name="count">The count.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static short[] DecompressIndices(short[] indices, int start, int count)
        {
            bool dir = false;
            short tempx;
            short tempy;
            short tempz;
            short[] shite = new short[50000];
            int m = start;
            int s = 0;
            do
            {
                tempx = indices[m];
                tempy = indices[m + 1];
                tempz = indices[m + 2];

                if (tempx != tempy && tempx != tempz && tempy != tempz)
                {
                    if (dir == false)
                    {
                        shite[s] = tempx;
                        shite[s + 1] = tempy;
                        shite[s + 2] = tempz;
                        s += 3;

                        dir = true;
                    }
                    else
                    {
                        shite[s] = tempx;
                        shite[s + 1] = tempz;
                        shite[s + 2] = tempy;
                        s += 3;
                        dir = false;
                    }

                    m += 1;
                }
                else
                {
                    if (dir)
                    {
                        dir = false;
                    }
                    else
                    {
                        dir = true;
                    }

                    m += 1;
                }
            }
            while (m < start + count - 2);
            short[] uncompressedindices = new short[s];
            Array.Copy(shite, uncompressedindices, s);
            return uncompressedindices;
        }

        /// <summary>
        /// The degree to radian.
        /// </summary>
        /// <param name="degree">The degree.</param>
        /// <returns>The degree to radian.</returns>
        /// <remarks></remarks>
        public static float DegreeToRadian(float degree)
        {
            return (float)((Math.PI * degree) / 180.0);
        }

        /// <summary>
        /// The make mesh.
        /// </summary>
        /// <param name="pm">The pm.</param>
        /// <param name="chunknumber">The chunknumber.</param>
        /// <param name="device">The device.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static Mesh MakeMesh(ref ParsedModel pm, int chunknumber, ref Device device)
        {
            short[] indices;
            int[] facecount = new int[pm.RawDataMetaChunks[chunknumber].SubMeshInfo.Length];
            int[] facestart = new int[pm.RawDataMetaChunks[chunknumber].SubMeshInfo.Length];
            if (pm.RawDataMetaChunks[chunknumber].FaceCount * 3 != pm.RawDataMetaChunks[chunknumber].Indices.Length)
            {
                short[] tempindicesx = new short[pm.RawDataMetaChunks[chunknumber].FaceCount * 3];
                int tempc = 0;
                for (int x = 0; x < pm.RawDataMetaChunks[chunknumber].SubMeshInfo.Length; x++)
                {
                    short[] tempindices = DecompressIndices(
                        pm.RawDataMetaChunks[chunknumber].Indices, 
                        pm.RawDataMetaChunks[chunknumber].SubMeshInfo[x].IndiceStart, 
                        pm.RawDataMetaChunks[chunknumber].SubMeshInfo[x].IndiceCount);
                    Array.ConstrainedCopy(tempindices, 0, tempindicesx, tempc, tempindices.Length);

                    facecount[x] = tempindices.Length / 3;
                    facestart[x] = tempc;
                    tempc += tempindices.Length;
                }

                indices = new short[tempindicesx.Length];
                Array.Copy(tempindicesx, indices, tempindicesx.Length);
            }
            else
            {
                indices = pm.RawDataMetaChunks[chunknumber].Indices;
                for (int x = 0; x < pm.RawDataMetaChunks[chunknumber].SubMeshInfo.Length; x++)
                {
                    facecount[x] = pm.RawDataMetaChunks[chunknumber].SubMeshInfo[x].IndiceCount / 3;
                    facestart[x] = pm.RawDataMetaChunks[chunknumber].SubMeshInfo[x].IndiceStart / 3;
                }
            }

            Mesh m = new Mesh(
                indices.Length / 3, 
                pm.RawDataMetaChunks[chunknumber].Vertices.Count, 
                MeshFlags.Managed, 
                HaloVertex.FVF, 
                device);
            VertexBuffer vb = m.VertexBuffer;
            HaloVertex[] tempv = new HaloVertex[pm.RawDataMetaChunks[chunknumber].Vertices.Count];
            for (int x = 0; x < pm.RawDataMetaChunks[chunknumber].Vertices.Count; x++)
            {
                tempv[x].Position = pm.RawDataMetaChunks[chunknumber].Vertices[x];
                tempv[x].Normal = pm.RawDataMetaChunks[chunknumber].Normals[x];
                tempv[x].Tu0 = pm.RawDataMetaChunks[chunknumber].UVs[x].X;
                tempv[x].Tv0 = pm.RawDataMetaChunks[chunknumber].UVs[x].Y;
            }

            vb.SetData(tempv, 0, LockFlags.None);
            vb.Unlock();

            IndexBuffer ibx = m.IndexBuffer;
            ibx.SetData(indices, 0, LockFlags.None);

            return m;
        }

        /// <summary>
        /// The radian to degree.
        /// </summary>
        /// <param name="radian">The radian.</param>
        /// <returns>The radian to degree.</returns>
        /// <remarks></remarks>
        public static float RadianToDegree(float radian)
        {
            return (float)((radian * 180.0) / Math.PI);
        }

        /// <summary>
        /// The set alpha blending.
        /// </summary>
        /// <param name="alphatype">The alphatype.</param>
        /// <param name="device">The device.</param>
        /// <remarks></remarks>
        public static void SetAlphaBlending(ShaderInfo.AlphaType alphatype, ref Device device)
        {
            if (alphatype == ShaderInfo.AlphaType.AlphaBlend)
            {
                device.RenderState.AlphaBlendEnable = true;
                device.RenderState.AlphaTestEnable = false;
            }
            else if (alphatype == ShaderInfo.AlphaType.AlphaTest)
            {
                device.RenderState.AlphaBlendEnable = false;
                device.RenderState.AlphaTestEnable = true;
            }
            else
            {
                device.RenderState.AlphaBlendEnable = false;
                device.RenderState.AlphaTestEnable = false;
            }
        }

        /// <summary>
        /// The vector to rgba.
        /// </summary>
        /// <param name="v">The v.</param>
        /// <param name="height">The height.</param>
        /// <returns>The vector to rgba.</returns>
        /// <remarks></remarks>
        public static int VectorToRgba(Vector3 v, float height)
        {
            int r = (int)(127.0f * v.X + 128.0f);
            int g = (int)(127.0f * v.Y + 128.0f);
            int b = (int)(127.0f * v.Z + 128.0f);
            int a = (int)(255.0f * height);

            return (a << 24) + (r << 16) + (g << 8) + (b << 0);
        }

        /// <summary>
        /// The begin scene.
        /// </summary>
        /// <param name="backgroundcolor">The backgroundcolor.</param>
        /// <remarks></remarks>
        public void BeginScene(Color backgroundcolor)
        {
            // Clear the backbuffer to a blue color
            device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, backgroundcolor, 1.0f, 0);

            // Begin the scene
            device.BeginScene();
        }

        /// <summary>
        /// The create device.
        /// </summary>
        /// <param name="renderpanel">The renderpanel.</param>
        /// <remarks></remarks>
        public void CreateDevice(Control renderpanel)
        {
            PresentParameters presentParams = new PresentParameters();

            presentParams.Windowed = true; // We don't want to run fullscreen

            /*
            presentParams.BackBufferCount = 1; //Number of backbuffers to create
            presentParams.BackBufferFormat = Manager.Adapters.Default.CurrentDisplayMode.Format; //The current format of the display device
            presentParams.BackBufferWidth = renderpanel.Width;
            presentParams.BackBufferHeight = renderpanel.Height;
             */
            presentParams.SwapEffect = SwapEffect.Discard; // Discard the frames
            presentParams.AutoDepthStencilFormat = DepthFormat.D16; // 24 bits for the depth and 8 bits for the stencil
            presentParams.EnableAutoDepthStencil = true; // Let direct3d handle the depth buffers for the application
            presentParams.PresentationInterval = PresentInterval.One;

            IEnumerator i = Manager.Adapters.GetEnumerator();

            while (i.MoveNext())
            {
                AdapterInformation ai = i.Current as AdapterInformation;

                int adapterOrdinal = ai.Adapter;

                Caps hardware = Manager.GetDeviceCaps(adapterOrdinal, DeviceType.Hardware);

                CreateFlags flags = CreateFlags.SoftwareVertexProcessing;

                if (hardware.DeviceCaps.SupportsHardwareTransformAndLight)
                {
                    flags = CreateFlags.HardwareVertexProcessing;

                    // if (hardware.MaxActiveLights > 0)
                    // this.lighting = true;
                    // flags |= CreateFlags.PureDevice;
                    // if (hardware.DeviceCaps.m
                }

                device = new Device(adapterOrdinal, hardware.DeviceType, renderpanel, flags, presentParams);

                // Create a device
                if (device != null)
                {
                    break;
                }
            }

            device.DeviceReset += OnResetDevice;
            this.pause = false;
            OnResetDevice(device, null);
        }

        /// <summary>
        /// The end scene.
        /// </summary>
        /// <remarks></remarks>
        public void EndScene()
        {
            // End the scene
            device.EndScene();

            // Update the screen
            device.Present();
        }

        /// <summary>
        /// The make mesh.
        /// </summary>
        /// <param name="vertices">The vertices.</param>
        /// <param name="Indices">The indices.</param>
        /// <param name="uvs">The uvs.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public Mesh MakeMesh(List<Vector3> vertices, List<short> Indices, List<Vector2> uvs)
        {
            Mesh m = new Mesh(Indices.Count / 3, vertices.Count, MeshFlags.Managed, HaloVertex.FVF, device);
            HaloVertex[] tempv = new HaloVertex[vertices.Count];
            for (int x = 0; x < vertices.Count; x++)
            {
                tempv[x].Position = vertices[x];
                tempv[x].Tu0 = uvs[x].X;
                tempv[x].Tv0 = uvs[x].Y;
            }

            VertexBuffer vb = m.VertexBuffer;
            vb.SetData(tempv, 0, LockFlags.None);
            vb.Unlock();

            IndexBuffer ibx = m.IndexBuffer;
            ibx.SetData(Indices.ToArray(), 0, LockFlags.None);

            return m;
        }

        /// <summary>
        /// The mark 3 d cursor position.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="m">The m.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public Vector3 Mark3DCursorPosition(float x, float y, Matrix m)
        {
            Vector3 tempV3 = new Vector3();
            tempV3.Project(device.Viewport, device.Transform.Projection, device.Transform.View, m);

            tempV3.X = x;
            tempV3.Y = y;
            tempV3.Unproject(device.Viewport, device.Transform.Projection, device.Transform.View, m);

            return tempV3;
        }

        /// <summary>
        /// The mesh pick.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="mesh">The mesh.</param>
        /// <param name="mat">The mat.</param>
        /// <returns>The mesh pick.</returns>
        /// <remarks></remarks>
        public bool MeshPick(float x, float y, Mesh mesh, Matrix mat)
        {
            Vector3 s = Vector3.Unproject(
                new Vector3(x, y, 0), device.Viewport, device.Transform.Projection, device.Transform.View, mat);

            Vector3 d = Vector3.Unproject(
                new Vector3(x, y, 1), device.Viewport, device.Transform.Projection, device.Transform.View, mat);

            Vector3 rPosition = s;
            Vector3 rDirection = Vector3.Normalize(d - s);

            if (mesh.Intersect(rPosition, rDirection))
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        /// <summary>
        /// The on reset device.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        public void OnResetDevice(object sender, EventArgs e)
        {
            Device dev = (Device)sender;

            // Turn off culling, so we see the front and back of the triangle
            dev.RenderState.CullMode = Cull.None;

            // Turn off D3D lighting
            dev.RenderState.Lighting = true;

            // Turn on the ZBuffer
            dev.RenderState.ZBufferEnable = true;
            dev.RenderState.FillMode = FillMode.Solid;

            if (dev.DeviceCaps.SourceBlendCaps.SupportsInverseSourceAlpha &&
                dev.DeviceCaps.SourceBlendCaps.SupportsSourceAlpha)
            {
                dev.RenderState.SourceBlend = Blend.SourceAlpha;
                dev.RenderState.DestinationBlend = Blend.InvSourceAlpha;
            }

            if (dev.DeviceCaps.TextureFilterCaps.SupportsMinifyLinear)
            {
                device.SamplerState[0].MinFilter = TextureFilter.Linear;
                device.SamplerState[1].MinFilter = TextureFilter.Linear;
                device.SamplerState[2].MinFilter = TextureFilter.Linear;
                device.SamplerState[3].MinFilter = TextureFilter.Linear;
            }

            if (dev.DeviceCaps.TextureFilterCaps.SupportsMagnifyLinear)
            {
                device.SamplerState[0].MagFilter = TextureFilter.Linear;
                device.SamplerState[1].MagFilter = TextureFilter.Linear;
                device.SamplerState[2].MagFilter = TextureFilter.Linear;
                device.SamplerState[3].MagFilter = TextureFilter.Linear;
            }

            if (dev.DeviceCaps.TextureFilterCaps.SupportsMipMapLinear)
            {
                device.SamplerState[0].MipFilter = TextureFilter.Linear;
                device.SamplerState[1].MipFilter = TextureFilter.Linear;
                device.SamplerState[2].MipFilter = TextureFilter.Linear;
                device.SamplerState[3].MipFilter = TextureFilter.Linear;
            }

            /*

             device.Lights[0].Diffuse = Color.White;
             device.Lights[0].Type = LightType.Point; ;
             device.Lights[0].Position = new Vector3(0, -1, -3);
             device.Lights[0].Direction = new Vector3(0, 0, -3);
             device.Lights[0].Range = 1000f;
             device.Lights[0].Update(); // Or Update() with the 2004-Oct SDK
             device.Lights[0].Enabled = true;
            */

            // Add a little ambient light to the scene
            device.RenderState.Ambient = Color.FromArgb(0xAF, 0xAF, 0xAF);
        }

        #endregion

        #region Implemented Interfaces

        #region IDisposable

        /// <summary>
        /// The dispose.
        /// </summary>
        /// <remarks></remarks>
        public void Dispose()
        {
        }

        #endregion

        #endregion
    }
}