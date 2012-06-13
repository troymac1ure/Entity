namespace entity.Renderers
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;
    using Microsoft.DirectX;
    using Microsoft.DirectX.Direct3D;

    using Globals;
    using HaloMap.H2MetaContainers;
    using HaloMap.Render;

    public partial class BSPViewer : Form
	{
        private Mesh loadBoundingBoxSpawn(SpawnInfo.BaseSpawn spawn)
        {
            SpawnInfo.BoundingBoxSpawn tempbox;
            tempbox = spawn as SpawnInfo.BoundingBoxSpawn;
            try
            {
                return (Mesh.Box(render.device, tempbox.width, tempbox.height, tempbox.length));
            }
            catch (Exception ex)
            {
                Global.ShowErrorMsg("Failure to create Bounding Box Mesh for [" + tempbox.TagType + "] " + tempbox.TagPath +
                    "\nWidth : " + tempbox.width.ToString() +
                    "\nHeight: " + tempbox.height.ToString() +
                    "\nLength: " + tempbox.length.ToString(),
                    ex);
            }
            return null;
        }

        private Mesh loadCameraSpawn(SpawnInfo.BaseSpawn spawn)
        {
            SpawnInfo.CameraSpawn tempbox;
            tempbox = spawn as SpawnInfo.CameraSpawn;
            
            try
            {
                return (Mesh.Cylinder(render.device, 0.1f, 2.0f, 10f, 10, 10));
            }
            catch (Exception ex)
            {
                Global.ShowErrorMsg("Failure to create Bounding Box Mesh for [" + tempbox.TagType + "] " + tempbox.TagPath, ex);
            }
            return null;
        }

        private Mesh loadSoundSpawn(SpawnInfo.BaseSpawn spawn)
        {
            SpawnInfo.SoundSpawn tempbox;
            tempbox = spawn as SpawnInfo.SoundSpawn;
            switch (tempbox.VolumeType)
            {
                case 0: // Sphere
                    if (tempbox.DistanceBoundsLower < 30)
                    {
                        return (Mesh.Sphere(
                            render.device,
                            tempbox.DistanceBoundsLower,
                            10 + (int)tempbox.DistanceBoundsLower,
                            10 + (int)tempbox.DistanceBoundsLower));
                    }
                    else
                    {
                        return (Mesh.Sphere(render.device, tempbox.DistanceBoundsLower, 30, 30));
                    }
                case 1: // Cylinder
                    return (Mesh.Cylinder(
                        render.device,
                        tempbox.DistanceBoundsLower,
                        tempbox.DistanceBoundsUpper,
                        10 + tempbox.Height,
                        10,
                        10));
                default:
                    return (Mesh.Cylinder(
                        render.device,
                        tempbox.DistanceBoundsLower,
                        tempbox.DistanceBoundsUpper,
                        tempbox.Height,
                        10,
                        10));
            }

        }
    }
}

