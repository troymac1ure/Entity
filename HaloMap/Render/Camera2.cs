// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Camera2.cs" company="">
//   
// </copyright>
// <summary>
//   The camera 2.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HaloMap.Render
{
    using System;
    using System.Windows.Forms;

    using Microsoft.DirectX;
    using Microsoft.DirectX.DirectInput;

    /// <summary>
    /// The camera 2.
    /// </summary>
    /// <remarks></remarks>
    public class Camera2 : IDisposable
    {
        #region Constants and Fields

        /// <summary>
        /// The look at.
        /// </summary>
        public Vector3 LookAt = new Vector3(0, 0, 0f);

        /// <summary>
        /// The position.
        /// </summary>
        public Vector3 Position;

        /// <summary>
        /// The up vector.
        /// </summary>
        public Vector3 UpVector = new Vector3(0, 0, 1);

        /// <summary>
        /// The device.
        /// </summary>
        public Device device;

        /// <summary>
        /// The fixedrotation.
        /// </summary>
        public bool fixedrotation;

        /// <summary>
        /// The i.
        /// </summary>
        public float i;

        /// <summary>
        /// The j.
        /// </summary>
        public float j;

        /// <summary>
        /// The k.
        /// </summary>
        public float k;

        /// <summary>
        /// The oldx.
        /// </summary>
        public int oldx;

        /// <summary>
        /// The oldy.
        /// </summary>
        public int oldy;

        /// <summary>
        /// The radianh.
        /// </summary>
        public float radianh;

        /// <summary>
        /// The radianv.
        /// </summary>
        public float radianv;

        /// <summary>
        /// The radius.
        /// </summary>
        public float radius = 1.0f;

        /// <summary>
        /// The speed.
        /// </summary>
        public float speed = 0.5f;

        /// <summary>
        /// The x.
        /// </summary>
        public float x;

        /// <summary>
        /// The y.
        /// </summary>
        public float y = -3f;

        /// <summary>
        /// The z.
        /// </summary>
        public float z = 1f;

        /// <summary>
        /// The m_frustum.
        /// </summary>
        private Plane[] m_frustum = new Plane[6];

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Camera2"/> class.
        /// </summary>
        /// <param name="form">The form.</param>
        /// <remarks></remarks>
        public Camera2(Form form)
        {
            device = new Device(SystemGuid.Keyboard);
            device.SetCooperativeLevel(form, CooperativeLevelFlags.Foreground | CooperativeLevelFlags.NonExclusive);

            radianh = DegreesToRadian(90.0f);
            radianv = DegreesToRadian(-20.0f);
            Position = new Vector3(x, y, z);
            ComputePosition();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// The aim camera.
        /// </summary>
        /// <param name="v">The v.</param>
        /// <remarks></remarks>
        public void AimCamera(Vector3 v)
        {
            i = v.X;
            j = v.Y;
            k = v.Z;
            LookAt.X = i;
            LookAt.Y = j;
            LookAt.Z = k;
        }

        /// <summary>
        /// The compute position.
        /// </summary>
        /// <remarks></remarks>
        public void ComputePosition()
        {
            // Keep all rotations between 0 and 2PI
            // if (fixedcam==true){return;}
            radianh = radianh > (float)Math.PI * 2 ? radianh - (float)Math.PI * 2 : radianh;
            radianh = radianh < 0 ? radianh + (float)Math.PI * 2 : radianh;

            radianv = radianv > (float)Math.PI * 2 ? radianv - (float)Math.PI * 2 : radianv;
            radianv = radianv < 0 ? radianv + (float)Math.PI * 2 : radianv;

            i = radius * (float)(Math.Cos(radianh) * Math.Cos(radianv));
            j = radius * (float)(Math.Sin(radianh) * Math.Cos(radianv));
            k = radius * (float)Math.Sin(radianv);
            

            if (fixedrotation)
            {
                Position.X = i + x;
                Position.Y = j + y;
                Position.Z = k + z;
            }
            else
            {
                LookAt.X = i + x;
                LookAt.Y = j + y;
                LookAt.Z = k + z;
            }
        }

        /// <summary>
        /// The compute strafe.
        /// </summary>
        /// <param name="right">The right.</param>
        /// <remarks></remarks>
        public void ComputeStrafe(bool right)
        {
            // Keep all rotations between 0 and 2PI
            radianh = radianh > (float)Math.PI * 2 ? radianh - (float)Math.PI * 2 : radianh;
            radianh = radianh < 0 ? radianh + (float)Math.PI * 2 : radianh;

            radianv = radianv > (float)Math.PI * 2 ? radianv - (float)Math.PI * 2 : radianv;
            radianv = radianv < 0 ? radianv + (float)Math.PI * 2 : radianv;

            // Switch up-vector based on vertical rotation
            // UpVector = Position.X < 0 ? new Vector3(-1, 0, 1) : new Vector3(1, 0, 1);
            // radianv > Math.PI / 2 && radianv < Math.PI / 2 * 3 ?
            // new Vector3(0,  1,0) : new Vector3(0,  -1,0);
            float tempi = radius * (float)(Math.Cos(radianh - 1.57) * Math.Cos(radianv)) * this.speed;
            float tempj = radius * (float)(Math.Sin(radianh - 1.57) * Math.Cos(radianv)) * this.speed;

            if (right)
            {
                LookAt.X += tempi;
                LookAt.Y += tempj;

                x += tempi;
                y += tempj;

                Position.X = x;
                Position.Y = y;
            }
            else
            {
                LookAt.X -= tempi;
                LookAt.Y -= tempj;

                x -= tempi;
                y -= tempj;

                Position.X = x;
                Position.Y = y;
            }
        }

        /// <summary>
        /// The degrees to radian.
        /// </summary>
        /// <param name="degree">The degree.</param>
        /// <returns>The degrees to radian.</returns>
        /// <remarks></remarks>
        public float DegreesToRadian(float degree)
        {
            return (float)(degree * (Math.PI / 180));
        }

        /// <summary>
        /// The set fixed.
        /// </summary>
        /// <remarks></remarks>
        public void SetFixed()
        {
            fixedrotation = true;
        }

        /// <summary>
        /// The change.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <remarks></remarks>
        public void change(int x, int y)
        {
            int tempx = oldx - x;
            int tempy = oldy - y;

            radianh += DegreesToRadian(tempx);
            radianv += DegreesToRadian(tempy);

            ComputePosition();
            oldx = x;
            oldy = y;
        }

        /// <summary>
        /// The move.
        /// </summary>
        /// <returns>The move.</returns>
        /// <remarks></remarks>
        public bool move()
        {
            bool speedChange = false;
            try
            {
                device.Acquire();
            }
            catch
            {
                return speedChange;
            }

            foreach (Key kk in device.GetPressedKeys())
            {
                switch (kk.ToString())
                {
                    case "W":
                        x += i * speed;
                        y += j * speed;
                        z += k * speed;
                        Position.X = x;
                        Position.Y = y;
                        Position.Z = z;
                        break;
                    case "S":
                        x -= i * speed;
                        y -= j * speed;
                        z -= k * speed;
                        Position.X = x;
                        Position.Y = y;
                        Position.Z = z;

                        break;
                    case "A":
                        ComputeStrafe(false);

                        // y += speed;
                        // Position.Y = y;
                        break;
                    case "D":
                        ComputeStrafe(true);

                        // y -= speed;
                        // Position.Y = y;
                        break;
                    case "Z":
                        z -= speed;
                        Position.Z = z;
                        break;
                    case "X":
                        z += speed;
                        Position.Z = z;
                        break;

                    case "Equals":
                    case "Add":
                        if (speed < 1.0)
                        {
                            speed += 0.01f;
                        }
                        else
                        {
                            speed += 0.1f;
                        }

                        if (speed >= 6.0)
                        {
                            speed = 6.0f;
                        }

                        speedChange = true;
                        break;
                    case "Minus":
                    case "NumPadMinus":
                        if (speed <= 1.0)
                        {
                            speed -= 0.01f;
                        }
                        else
                        {
                            speed -= 0.1f;
                        }

                        if (speed <= 0.01f)
                        {
                            speed = 0.01f;
                        }

                        speedChange = true;
                        break;
                }

                ComputePosition();
            }

            return speedChange;
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
            device = null;
        }

        #endregion

        #endregion

        /*
        public void BuildViewFrustum(ref DirectX.Direct3D.Device device)
        {
            Matrix viewProjection = device.Transform.View * device.Transform.Projection;

            // Left plane
            m_frustum[0].A = viewProjection.M14 + viewProjection.M11;
            m_frustum[0].B = viewProjection.M24 + viewProjection.M21;
            m_frustum[0].C = viewProjection.M34 + viewProjection.M31;
            m_frustum[0].D = viewProjection.M44 + viewProjection.M41;

            // Right plane
            m_frustum[1].A = viewProjection.M14 - viewProjection.M11;
            m_frustum[1].B = viewProjection.M24 - viewProjection.M21;
            m_frustum[1].C = viewProjection.M34 - viewProjection.M31;
            m_frustum[1].D = viewProjection.M44 - viewProjection.M41;

            // Top plane
            m_frustum[2].A = viewProjection.M14 - viewProjection.M12;
            m_frustum[2].B = viewProjection.M24 - viewProjection.M22;
            m_frustum[2].C = viewProjection.M34 - viewProjection.M32;
            m_frustum[2].D = viewProjection.M44 - viewProjection.M42;

            // Bottom plane
            m_frustum[3].A = viewProjection.M14 + viewProjection.M12;
            m_frustum[3].B = viewProjection.M24 + viewProjection.M22;
            m_frustum[3].C = viewProjection.M34 + viewProjection.M32;
            m_frustum[3].D = viewProjection.M44 + viewProjection.M42;

            // Near plane
            m_frustum[4].A = viewProjection.M13;
            m_frustum[4].B = viewProjection.M23;
            m_frustum[4].C = viewProjection.M33;
            m_frustum[4].D = viewProjection.M43;

            // Far plane
            m_frustum[5].A = viewProjection.M14 - viewProjection.M13;
            m_frustum[5].B = viewProjection.M24 - viewProjection.M23;
            m_frustum[5].C = viewProjection.M34 - viewProjection.M33;
            m_frustum[5].D = viewProjection.M44 - viewProjection.M43;

            // Normalize planes
            for (int i = 0; i < 6; i++)
            {
                m_frustum[i] = Plane.Normalize(m_frustum[i]);
            }
        }

        public bool SphereInFrustum(Vector3 position, float radius)
        {
            Vector4 position4 = new Vector4(position.X, position.Y, position.Z, 1f);
            for (int i = 0; i < 6; i++)
            {
                if (m_frustum[i].Dot(position4) + radius < 0)
                {
                    // Outside the frustum, reject it!
                    return false;
                }
            }
            return true;
        }
        */
    }
}