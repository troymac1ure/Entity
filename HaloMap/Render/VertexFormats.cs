using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace HaloMap.Render
{
    /// <summary>
    /// The halo bsp vertex.
    /// </summary>
    /// <remarks></remarks>
    public struct HaloBSPVertex
    {
        /// <summary>
        /// 
        /// </summary>
        public Vector3 Position;
        /// <summary>
        /// 
        /// </summary>
        public Vector3 Normal;

        /// <summary>
        /// 
        /// </summary>
        public int diffuse;
        // public int specular;
        /// <summary>
        /// 
        /// </summary>
        public float Tu0, Tv0;
        /// <summary>
        /// 
        /// </summary>
        public float Tu1, Tv1;
        /// <summary>
        /// 
        /// </summary>
        public float Tu2, Tv2;
        /// <summary>
        /// 
        /// </summary>
        public float Tu3, Tv3;
        /// <summary>
        /// 
        /// </summary>
        public static readonly VertexFormats FVF = VertexFormats.PositionNormal | VertexFormats.Diffuse | VertexFormats.Texture4;

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents this instance.</returns>
        /// <remarks></remarks>
        public override string ToString()
        {
            return Position.ToString();
        }
    }

    /// <summary>
    /// The halo vertex.
    /// </summary>
    /// <remarks></remarks>
    public struct HaloVertex
    {
        /// <summary>
        /// 
        /// </summary>
        public Vector3 Position;
        /// <summary>
        /// 
        /// </summary>
        public Vector3 Normal;

        /// <summary>
        /// 
        /// </summary>
        public int diffuse;
        /// <summary>
        /// 
        /// </summary>
        public int specular;
        /// <summary>
        /// 
        /// </summary>
        public float Tu0, Tv0;
        // public float Tu1, Tv1;
        /// <summary>
        /// 
        /// </summary>
        public static readonly VertexFormats FVF = VertexFormats.PositionNormal | VertexFormats.Diffuse | VertexFormats.Specular | VertexFormats.Texture1;
    }
}
