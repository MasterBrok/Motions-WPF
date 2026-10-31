using System.Windows.Media;
using System.Windows.Media.Media3D;
using ABYSSAL.Geometry;
using ABYSSAL.Materials;

namespace ABYSSAL.Scene;

public class OceanScene
{
    public void Build(Model3DGroup root, double detail)
    {
        CreateOceanSurface(root, detail);
        CreateUpperWaterVolume(root, detail);
    }

    private void CreateOceanSurface(Model3DGroup root, double detail)
    {
        var mesh = new MeshGeometry3D();
        var width = 500.0;
        var depth = 500.0;
        var segments = (int)(50 * detail);

        for (int z = 0; z <= segments; z++)
        {
            for (int x = 0; x <= segments; x++)
            {
                var px = (x / (double)segments - 0.5) * width;
                var pz = (z / (double)segments - 0.5) * depth;
                var py = 80 + Math.Sin(x * 0.1) * 0.5 + Math.Cos(z * 0.15) * 0.5;

                mesh.Positions.Add(new Point3D(px, py, pz));
            }
        }

        for (int z = 0; z < segments; z++)
        {
            for (int x = 0; x < segments; x++)
            {
                var idx = z * (segments + 1) + x;
                mesh.TriangleIndices.Add(idx);
                mesh.TriangleIndices.Add(idx + 1);
                mesh.TriangleIndices.Add(idx + segments + 1);

                mesh.TriangleIndices.Add(idx + 1);
                mesh.TriangleIndices.Add(idx + segments + 2);
                mesh.TriangleIndices.Add(idx + segments + 1);
            }
        }

        var material = new DiffuseMaterial
        {
            Brush = new LinearGradientBrush(
                Color.FromArgb(200, 100, 180, 220),
                Color.FromArgb(150, 50, 100, 150),
                90)
        };

        var model = new GeometryModel3D
        {
            Geometry = mesh,
            Material = material,
            BackMaterial = material
        };

        root.Children.Add(model);
    }

    private void CreateUpperWaterVolume(Model3DGroup root, double detail)
    {
    }
}
