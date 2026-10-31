using System.Windows.Media.Media3D;

namespace ABYSSAL.Geometry;

public class RingFactory
{
    public MeshGeometry3D GenerateRing(double radius, double thickness, int segments)
    {
        var mesh = new MeshGeometry3D();
        var height = thickness;

        for (int i = 0; i <= segments; i++)
        {
            var angle = 2 * Math.PI * i / segments;
            var cos = Math.Cos(angle);
            var sin = Math.Sin(angle);

            mesh.Positions.Add(new Point3D((radius - thickness / 2) * cos, -height / 2, (radius - thickness / 2) * sin));
            mesh.Positions.Add(new Point3D((radius + thickness / 2) * cos, -height / 2, (radius + thickness / 2) * sin));
            mesh.Positions.Add(new Point3D((radius + thickness / 2) * cos, height / 2, (radius + thickness / 2) * sin));
            mesh.Positions.Add(new Point3D((radius - thickness / 2) * cos, height / 2, (radius - thickness / 2) * sin));
        }

        for (int i = 0; i < segments; i++)
        {
            var idx = i * 4;

            mesh.TriangleIndices.Add(idx); mesh.TriangleIndices.Add(idx + 4); mesh.TriangleIndices.Add(idx + 1);
            mesh.TriangleIndices.Add(idx + 1); mesh.TriangleIndices.Add(idx + 4); mesh.TriangleIndices.Add(idx + 5);

            mesh.TriangleIndices.Add(idx + 1); mesh.TriangleIndices.Add(idx + 5); mesh.TriangleIndices.Add(idx + 2);
            mesh.TriangleIndices.Add(idx + 2); mesh.TriangleIndices.Add(idx + 5); mesh.TriangleIndices.Add(idx + 6);

            mesh.TriangleIndices.Add(idx + 2); mesh.TriangleIndices.Add(idx + 6); mesh.TriangleIndices.Add(idx + 3);
            mesh.TriangleIndices.Add(idx + 3); mesh.TriangleIndices.Add(idx + 6); mesh.TriangleIndices.Add(idx + 7);

            mesh.TriangleIndices.Add(idx + 3); mesh.TriangleIndices.Add(idx + 7); mesh.TriangleIndices.Add(idx);
            mesh.TriangleIndices.Add(idx); mesh.TriangleIndices.Add(idx + 7); mesh.TriangleIndices.Add(idx + 4);
        }

        return mesh;
    }

    public MeshGeometry3D GenerateDisc(double radius, int segments)
    {
        var mesh = new MeshGeometry3D();

        mesh.Positions.Add(new Point3D(0, 0, 0));

        for (int i = 0; i <= segments; i++)
        {
            var angle = 2 * Math.PI * i / segments;
            mesh.Positions.Add(new Point3D(radius * Math.Cos(angle), 0, radius * Math.Sin(angle)));
        }

        for (int i = 1; i <= segments; i++)
        {
            mesh.TriangleIndices.Add(0);
            mesh.TriangleIndices.Add(i);
            mesh.TriangleIndices.Add(i + 1);
        }

        return mesh;
    }

    public MeshGeometry3D GenerateCrystal(double size, int detail)
    {
        var mesh = new MeshGeometry3D();

        var mainAxis = new Vector3D(0, 1, 0);
        var lateralAxis = new Vector3D(1, 0, 0);

        var points = new List<Point3D>();

        points.Add(new Point3D(0, size, 0));

        for (int i = 0; i < 6; i++)
        {
            var innerAngle = i * Math.PI / 3;
            points.Add(new Point3D(
                size * 0.3 * Math.Cos(innerAngle),
                size * 0.2,
                size * 0.3 * Math.Sin(innerAngle)
            ));
        }

        for (int i = 0; i < 6; i++)
        {
            var outerAngle = (i + 0.5) * Math.PI / 3;
            points.Add(new Point3D(
                size * 0.4 * Math.Cos(outerAngle),
                -size * 0.1,
                size * 0.4 * Math.Sin(outerAngle)
            ));
        }

        points.Add(new Point3D(0, -size * 0.6, 0));

        foreach (var pt in points)
        {
            mesh.Positions.Add(pt);
        }

        for (int i = 1; i <= 6; i++)
        {
            mesh.TriangleIndices.Add(0);
            mesh.TriangleIndices.Add(i);
            mesh.TriangleIndices.Add((i % 6) + 1);
        }

        for (int i = 0; i < 6; i++)
        {
            var topIdx = i + 1;
            var bottomIdx = i + 7;
            var nextTop = (i % 6) + 1;
            var nextBottom = (i % 6) + 7;

            mesh.TriangleIndices.Add(topIdx);
            mesh.TriangleIndices.Add(bottomIdx);
            mesh.TriangleIndices.Add(nextTop);

            mesh.TriangleIndices.Add(nextTop);
            mesh.TriangleIndices.Add(bottomIdx);
            mesh.TriangleIndices.Add(nextBottom);
        }

        var baseTopIdx = 7;
        var baseBottomIdx = 13;
        for (int i = 0; i < 6; i++)
        {
            mesh.TriangleIndices.Add(baseTopIdx + i);
            mesh.TriangleIndices.Add(baseBottomIdx);
            mesh.TriangleIndices.Add(baseTopIdx + ((i + 1) % 6));
        }

        return mesh;
    }
}
