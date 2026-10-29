using System.Windows;
using System.Windows.Media.Media3D;

namespace Chronos.Geometry;

public static class CylinderMeshFactory
{
    public static MeshGeometry3D Create(double radius, double height, int segments = 32, bool caps = true)
    {
        var mesh = new MeshGeometry3D();
        var halfHeight = height / 2;

        for (int i = 0; i <= segments; i++)
        {
            double angle = 2 * Math.PI * i / segments;
            double x = Math.Cos(angle);
            double z = Math.Sin(angle);

            mesh.Positions.Add(new Point3D(x * radius, -halfHeight, z * radius));
            mesh.Positions.Add(new Point3D(x * radius, halfHeight, z * radius));

            var normal = new Vector3D(x, 0, z);
            mesh.Normals.Add(normal);
            mesh.Normals.Add(normal);

            double u = (double)i / segments;
            mesh.TextureCoordinates.Add(new Point(u, 0));
            mesh.TextureCoordinates.Add(new Point(u, 1));
        }

        for (int i = 0; i < segments; i++)
        {
            int idx = i * 2;
            mesh.TriangleIndices.Add(idx);
            mesh.TriangleIndices.Add(idx + 1);
            mesh.TriangleIndices.Add(idx + 2);
            mesh.TriangleIndices.Add(idx + 1);
            mesh.TriangleIndices.Add(idx + 3);
            mesh.TriangleIndices.Add(idx + 2);
        }

        if (caps)
        {
            AddCap(mesh, radius, -halfHeight, segments, false);
            AddCap(mesh, radius, halfHeight, segments, true);
        }

        mesh.Freeze();
        return mesh;
    }

    private static void AddCap(MeshGeometry3D mesh, double radius, double y, int segments, bool top)
    {
        int centerIndex = mesh.Positions.Count;
        mesh.Positions.Add(new Point3D(0, y, 0));
        mesh.Normals.Add(new Vector3D(0, top ? 1 : -1, 0));
        mesh.TextureCoordinates.Add(new Point(0.5, 0.5));

        int baseIndex = 0;
        if (top)
        {
            for (int i = 0; i <= segments; i++)
            {
                double angle = 2 * Math.PI * i / segments;
                double x = Math.Cos(angle) * radius;
                double z = Math.Sin(angle) * radius;
                mesh.Positions.Add(new Point3D(x, y, z));
                mesh.Normals.Add(new Vector3D(0, 1, 0));
                mesh.TextureCoordinates.Add(new Point(0.5 + x / (2 * radius), 0.5 + z / (2 * radius)));
            }
        }
        else
        {
            baseIndex = mesh.Positions.Count;
            for (int i = 0; i <= segments; i++)
            {
                double angle = 2 * Math.PI * i / segments;
                double x = Math.Cos(angle) * radius;
                double z = Math.Sin(angle) * radius;
                mesh.Positions.Add(new Point3D(x, y, z));
                mesh.Normals.Add(new Vector3D(0, -1, 0));
                mesh.TextureCoordinates.Add(new Point(0.5 + x / (2 * radius), 0.5 + z / (2 * radius)));
            }
        }

        for (int i = 0; i < segments; i++)
        {
            if (top)
            {
                mesh.TriangleIndices.Add(centerIndex);
                mesh.TriangleIndices.Add(baseIndex + 1 + i + 1);
                mesh.TriangleIndices.Add(baseIndex + 1 + i);
            }
            else
            {
                mesh.TriangleIndices.Add(centerIndex);
                mesh.TriangleIndices.Add(baseIndex + i);
                mesh.TriangleIndices.Add(baseIndex + i + 1);
            }
        }
    }
}