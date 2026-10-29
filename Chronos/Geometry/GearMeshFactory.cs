using System.Windows;
using System.Windows.Media.Media3D;

namespace Chronos.Geometry;

public static class GearMeshFactory
{
    public static MeshGeometry3D Create(int teeth, double innerRadius, double outerRadius, double thickness, double hubRadius = 0.3, int segments = 32)
    {
        var mesh = new MeshGeometry3D();
        int totalSegments = teeth * segments;
        var toothHeight = (outerRadius - innerRadius) * 0.4;
        var toothWidth = (2 * Math.PI * innerRadius) / (teeth * 2);

        for (int i = 0; i < totalSegments; i++)
        {
            double baseAngle = 2 * Math.PI * i / totalSegments;
            double toothIndex = Math.Floor(i / (double)segments);
            double localPos = (i % segments) / (double)segments;
            bool onTooth = localPos > 0.3 && localPos < 0.7;
            double r = onTooth ? outerRadius : innerRadius;

            double x = Math.Cos(baseAngle) * r;
            double z = Math.Sin(baseAngle) * r;

            mesh.Positions.Add(new Point3D(x, thickness / 2, z));
            mesh.Positions.Add(new Point3D(x, -thickness / 2, z));

            var normal = new Vector3D(Math.Cos(baseAngle), 0, Math.Sin(baseAngle));
            mesh.Normals.Add(normal);
            mesh.Normals.Add(normal);

            mesh.TextureCoordinates.Add(new Point((double)i / totalSegments, 0));
            mesh.TextureCoordinates.Add(new Point((double)i / totalSegments, 1));
        }

        for (int i = 0; i < totalSegments; i++)
        {
            int idx = i * 2;
            mesh.TriangleIndices.Add(idx);
            mesh.TriangleIndices.Add(idx + 1);
            mesh.TriangleIndices.Add(idx + 2);
            mesh.TriangleIndices.Add(idx + 1);
            mesh.TriangleIndices.Add(idx + 3);
            mesh.TriangleIndices.Add(idx + 2);
        }

        AddFace(mesh, thickness / 2, totalSegments, outerRadius, innerRadius);
        AddFace(mesh, -thickness / 2, totalSegments, outerRadius, innerRadius);

        if (hubRadius > 0)
        {
            AddHub(mesh, hubRadius, thickness, totalSegments);
        }

        mesh.Freeze();
        return mesh;
    }

    private static void AddFace(MeshGeometry3D mesh, double y, int segments, double outerRadius, double innerRadius)
    {
        int centerIdx = mesh.Positions.Count;
        mesh.Positions.Add(new Point3D(0, y, 0));
        mesh.Normals.Add(new Vector3D(0, y > 0 ? 1 : -1, 0));
        mesh.TextureCoordinates.Add(new Point(0.5, 0.5));

        for (int i = 0; i < segments; i++)
        {
            double angle = 2 * Math.PI * i / segments;
            double r = outerRadius;
            mesh.Positions.Add(new Point3D(Math.Cos(angle) * r, y, Math.Sin(angle) * r));
            mesh.Normals.Add(new Vector3D(0, y > 0 ? 1 : -1, 0));
            mesh.TextureCoordinates.Add(new Point(Math.Cos(angle) * 0.5 + 0.5, Math.Sin(angle) * 0.5 + 0.5));
        }

        for (int i = 0; i < segments; i++)
        {
            mesh.TriangleIndices.Add(centerIdx);
            mesh.TriangleIndices.Add(centerIdx + 1 + i);
            mesh.TriangleIndices.Add(centerIdx + 1 + (i + 1) % segments);
        }
    }

    private static void AddHub(MeshGeometry3D mesh, double radius, double thickness, int segments)
    {
        for (int i = 0; i < segments; i++)
        {
            double angle = 2 * Math.PI * i / segments;
            double x = Math.Cos(angle) * radius;
            double z = Math.Sin(angle) * radius;

            mesh.Positions.Add(new Point3D(x, thickness / 2, z));
            mesh.Positions.Add(new Point3D(x, -thickness / 2, z));

            var normal = new Vector3D(Math.Cos(angle), 0, Math.Sin(angle));
            mesh.Normals.Add(normal);
            mesh.Normals.Add(normal);
        }

        int baseIdx = mesh.Positions.Count - segments * 2;
        for (int i = 0; i < segments; i++)
        {
            mesh.TriangleIndices.Add(baseIdx + i * 2);
            mesh.TriangleIndices.Add(baseIdx + (i * 2 + 2) % (segments * 2));
            mesh.TriangleIndices.Add(baseIdx + i * 2 + 1);
            mesh.TriangleIndices.Add(baseIdx + i * 2 + 1);
            mesh.TriangleIndices.Add(baseIdx + (i * 2 + 2) % (segments * 2));
            mesh.TriangleIndices.Add(baseIdx + (i * 2 + 3) % (segments * 2));
        }
    }

    public static MeshGeometry3D CreateMechanicalPlate(double width, double height, double thickness, int detail = 2)
    {
        var mesh = new MeshGeometry3D();
        double hw = width / 2;
        double hh = height / 2;
        double ht = thickness / 2;

        mesh.Positions.Add(new Point3D(-hw, ht, -hh));
        mesh.Positions.Add(new Point3D(hw, ht, -hh));
        mesh.Positions.Add(new Point3D(hw, ht, hh));
        mesh.Positions.Add(new Point3D(-hw, ht, hh));

        mesh.Positions.Add(new Point3D(-hw, -ht, -hh));
        mesh.Positions.Add(new Point3D(hw, -ht, -hh));
        mesh.Positions.Add(new Point3D(hw, -ht, hh));
        mesh.Positions.Add(new Point3D(-hw, -ht, hh));

        for (int i = 0; i < 4; i++)
            mesh.Normals.Add(new Vector3D(0, 1, 0));
        for (int i = 0; i < 4; i++)
            mesh.Normals.Add(new Vector3D(0, -1, 0));

        mesh.TriangleIndices.Add(0); mesh.TriangleIndices.Add(1); mesh.TriangleIndices.Add(2);
        mesh.TriangleIndices.Add(0); mesh.TriangleIndices.Add(2); mesh.TriangleIndices.Add(3);

        mesh.TriangleIndices.Add(4); mesh.TriangleIndices.Add(6); mesh.TriangleIndices.Add(5);
        mesh.TriangleIndices.Add(4); mesh.TriangleIndices.Add(7); mesh.TriangleIndices.Add(6);

        mesh.TriangleIndices.Add(0); mesh.TriangleIndices.Add(4); mesh.TriangleIndices.Add(5);
        mesh.TriangleIndices.Add(0); mesh.TriangleIndices.Add(5); mesh.TriangleIndices.Add(1);

        mesh.TriangleIndices.Add(2); mesh.TriangleIndices.Add(6); mesh.TriangleIndices.Add(7);
        mesh.TriangleIndices.Add(2); mesh.TriangleIndices.Add(7); mesh.TriangleIndices.Add(3);

        mesh.TriangleIndices.Add(0); mesh.TriangleIndices.Add(3); mesh.TriangleIndices.Add(7);
        mesh.TriangleIndices.Add(0); mesh.TriangleIndices.Add(7); mesh.TriangleIndices.Add(4);

        mesh.TriangleIndices.Add(1); mesh.TriangleIndices.Add(5); mesh.TriangleIndices.Add(6);
        mesh.TriangleIndices.Add(1); mesh.TriangleIndices.Add(6); mesh.TriangleIndices.Add(2);

        mesh.Freeze();
        return mesh;
    }

    public static MeshGeometry3D CreateEnergyCore(double radius, int segments = 16)
    {
        var mesh = new MeshGeometry3D();

        for (int lat = 0; lat <= segments; lat++)
        {
            double theta = Math.PI * lat / segments;
            double sinTheta = Math.Sin(theta);
            double y = Math.Cos(theta);

            for (int lon = 0; lon <= segments; lon++)
            {
                double phi = 2 * Math.PI * lon / segments;
                double x = Math.Sin(phi) * sinTheta;
                double z = Math.Cos(phi) * sinTheta;

                mesh.Positions.Add(new Point3D(x * radius, y * radius, z * radius));
                mesh.Normals.Add(new Vector3D(x, y, z));
                mesh.TextureCoordinates.Add(new Point((double)lon / segments, (double)lat / segments));
            }
        }

        for (int lat = 0; lat < segments; lat++)
        {
            for (int lon = 0; lon < segments; lon++)
            {
                int first = lat * (segments + 1) + lon;
                int second = first + segments + 1;

                mesh.TriangleIndices.Add(first);
                mesh.TriangleIndices.Add(second);
                mesh.TriangleIndices.Add(first + 1);
                mesh.TriangleIndices.Add(first + 1);
                mesh.TriangleIndices.Add(second);
                mesh.TriangleIndices.Add(second + 1);
            }
        }

        mesh.Freeze();
        return mesh;
    }
}