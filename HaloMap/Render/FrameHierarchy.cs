// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FrameHierarchy.cs" company="">
//   
// </copyright>
// <summary>
//   The frame hierarchy.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HaloMap.Render
{
    using System.Collections.Generic;
    using System.IO;

    using HaloMap.Meta;

    using Microsoft.DirectX;

    /// <summary>
    /// The frame hierarchy.
    /// </summary>
    /// <remarks></remarks>
    public class FrameHierarchy
    {
        #region Constants and Fields

        /// <summary>
        /// The frame.
        /// </summary>
        public List<FrameInfo> Frame = new List<FrameInfo>();

        #endregion

        #region Public Methods

        /// <summary>
        /// The get frames from halo 2 model.
        /// </summary>
        /// <param name="meta">The meta.</param>
        /// <remarks></remarks>
        public void GetFramesFromHalo2Model(ref Meta meta)
        {
            BinaryReader BR = new BinaryReader(meta.MS);
            BR.BaseStream.Position = 72;
            int tempc = BR.ReadInt32();
            int tempr = BR.ReadInt32() - meta.magic - meta.offset;
            for (int x = 0; x < tempc; x++)
            {
                FrameInfo f = new FrameInfo();
                BR.BaseStream.Position = tempr + (96 * x);
                f.Name = meta.Map.Strings.Name[BR.ReadUInt16()];
                BR.ReadUInt16();
                f.Parent = BR.ReadInt16();
                f.Child = BR.ReadInt16();
                f.Sibling = BR.ReadInt16();
                BR.BaseStream.Position = tempr + (96 * x) + 12;
                f.x = BR.ReadSingle();
                f.y = BR.ReadSingle();
                f.z = BR.ReadSingle();
                f.i = BR.ReadSingle();
                f.j = BR.ReadSingle();
                f.k = BR.ReadSingle();
                f.w = BR.ReadSingle();
                f.matrix = Matrix.Identity;

                Quaternion q = new Quaternion(f.i, f.j, f.k, f.w);
                f.matrix.Multiply(Matrix.Translation(f.x, f.y, f.z));
                f.matrix.RotateQuaternion(q);

                BR.ReadSingle();

                f.skinweightmatrix = Matrix.Identity;
                f.skinweightmatrix.M11 = BR.ReadSingle();
                f.skinweightmatrix.M12 = BR.ReadSingle();
                f.skinweightmatrix.M13 = BR.ReadSingle();
                f.skinweightmatrix.M14 = 0.0f;

                f.skinweightmatrix.M21 = BR.ReadSingle();
                f.skinweightmatrix.M22 = BR.ReadSingle();
                f.skinweightmatrix.M23 = BR.ReadSingle();
                f.skinweightmatrix.M24 = 0.0f;

                f.skinweightmatrix.M31 = BR.ReadSingle();
                f.skinweightmatrix.M32 = BR.ReadSingle();
                f.skinweightmatrix.M33 = BR.ReadSingle();
                f.skinweightmatrix.M34 = 0.0f;

                f.skinweightmatrix.M41 = BR.ReadSingle();
                f.skinweightmatrix.M42 = BR.ReadSingle();
                f.skinweightmatrix.M43 = BR.ReadSingle();
                f.skinweightmatrix.M44 = 1.0f;

                f.DistanceFromParent = BR.ReadSingle();

                // f.matrix.MultiplyTranspose(f.skinweightmatrix);//=Matrix.Transformation(new Vector3(0, 0, 0), new Quaternion(),new Vector3(1,1,1), new Vector3(0, 0, 0), q, new Vector3(f.x, f.y, f.z));
                Frame.Add(f);
            }
        }

        #endregion
    }

    /// <summary>
    /// The frame info.
    /// </summary>
    /// <remarks></remarks>
    public class FrameInfo
    {
        #region Constants and Fields

        /// <summary>
        /// The child.
        /// </summary>
        public int Child = -1;

        /// <summary>
        /// The distance from parent.
        /// </summary>
        public float DistanceFromParent;

        /// <summary>
        /// The name.
        /// </summary>
        public string Name;

        /// <summary>
        /// The parent.
        /// </summary>
        public int Parent = -1;

        /// <summary>
        /// The sibling.
        /// </summary>
        public int Sibling = -1;

        /// <summary>
        /// The i.
        /// </summary>
        public float i; // quaternion

        /// <summary>
        /// The j.
        /// </summary>
        public float j; // quaternion

        /// <summary>
        /// The k.
        /// </summary>
        public float k; // quaternion

        /// <summary>
        /// The matrix.
        /// </summary>
        public Matrix matrix;

        /// <summary>
        /// The skinweightmatrix.
        /// </summary>
        public Matrix skinweightmatrix;

        /// <summary>
        /// The w.
        /// </summary>
        public float w; // quaternion

        /// <summary>
        /// The x.
        /// </summary>
        public float x;

        /// <summary>
        /// The y.
        /// </summary>
        public float y;

        /// <summary>
        /// The z.
        /// </summary>
        public float z;

        #endregion
    }
}