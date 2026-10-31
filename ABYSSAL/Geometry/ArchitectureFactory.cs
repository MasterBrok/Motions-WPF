using System.Windows.Media.Media3D;

namespace ABYSSAL.Geometry;

public class ArchitectureFactory
{
    private readonly Random _seededRandom;

    public ArchitectureFactory(int seed = 42)
    {
        _seededRandom = new Random(seed);
    }

    public MeshGeometry3D GenerateColumn(double radius, double height, int segments, bool hasCapital = true)
    {
        var mesh = CreateGeometry.CreateCylinder(radius, height, segments);

        if (hasCapital)
        {
            var capital = GenerateCapital(radius, height, segments);
            mesh = MergeMeshes(mesh, capital);
        }

        return mesh;
    }

    private MeshGeometry3D GenerateCapital(double radius, double height, int segments)
    {
        var mesh = new MeshGeometry3D();
        var baseY = height / 2;

        mesh.Positions.Add(new Point3D(-radius * 1.5, baseY, -radius * 1.5));
        mesh.Positions.Add(new Point3D(radius * 1.5, baseY, -radius * 1.5));
        mesh.Positions.Add(new Point3D(radius * 1.5, baseY, radius * 1.5));
        mesh.Positions.Add(new Point3D(-radius * 1.5, baseY, radius * 1.5));

        mesh.Positions.Add(new Point3D(-radius * 1.2, baseY + radius * 0.3, -radius * 1.2));
        mesh.Positions.Add(new Point3D(radius * 1.2, baseY + radius * 0.3, -radius * 1.2));
        mesh.Positions.Add(new Point3D(radius * 1.2, baseY + radius * 0.3, radius * 1.2));
        mesh.Positions.Add(new Point3D(-radius * 1.2, baseY + radius * 0.3, radius * 1.2));

        mesh.TriangleIndices.Add(0); mesh.TriangleIndices.Add(5); mesh.TriangleIndices.Add(4);
        mesh.TriangleIndices.Add(0); mesh.TriangleIndices.Add(1); mesh.TriangleIndices.Add(5);
        mesh.TriangleIndices.Add(1); mesh.TriangleIndices.Add(6); mesh.TriangleIndices.Add(5);
        mesh.TriangleIndices.Add(1); mesh.TriangleIndices.Add(2); mesh.TriangleIndices.Add(6);
        mesh.TriangleIndices.Add(2); mesh.TriangleIndices.Add(7); mesh.TriangleIndices.Add(6);
        mesh.TriangleIndices.Add(2); mesh.TriangleIndices.Add(3); mesh.TriangleIndices.Add(7);
        mesh.TriangleIndices.Add(3); mesh.TriangleIndices.Add(4); mesh.TriangleIndices.Add(7);
        mesh.TriangleIndices.Add(3); mesh.TriangleIndices.Add(0); mesh.TriangleIndices.Add(4);

        return mesh;
    }

    public MeshGeometry3D GeneratePlatform(double width, double height, double depth)
    {
        return CreateGeometry.CreateBox(width, height, depth);
    }

    public MeshGeometry3D GeneratePyramid(double baseSize, double height, int segments)
    {
        var mesh = new MeshGeometry3D();

        var halfBase = baseSize / 2;

        mesh.Positions.Add(new Point3D(-halfBase, 0, -halfBase));
        mesh.Positions.Add(new Point3D(halfBase, 0, -halfBase));
        mesh.Positions.Add(new Point3D(halfBase, 0, halfBase));
        mesh.Positions.Add(new Point3D(-halfBase, 0, halfBase));

        mesh.Positions.Add(new Point3D(0, height, 0));

        for (int i = 0; i < 4; i++)
        {
            mesh.TriangleIndices.Add(i);
            mesh.TriangleIndices.Add(4);
            mesh.TriangleIndices.Add((i + 1) % 4);
        }

        for (int i = 0; i < 4; i++)
        {
            mesh.TriangleIndices.Add(i);
            mesh.TriangleIndices.Add((i + 1) % 4);
            mesh.TriangleIndices.Add(4);
        }

        return mesh;
    }

    public MeshGeometry3D GenerateStep(double width, double height, double depth)
    {
        return CreateGeometry.CreateBox(width, height, depth);
    }

    public MeshGeometry3D GeneratePillar(double radius, double height, int segments)
    {
        return CreateGeometry.CreateCylinder(radius, height, segments);
    }

    public MeshGeometry3D GenerateArchway(double width, double height, double thickness)
    {
        var mesh = new MeshGeometry3D();

        var leftPillar = CreateGeometry.CreateBox(thickness, height, thickness);
        var leftTransform = new TranslateTransform3D(-width / 2, 0, 0);

        var rightPillar = CreateGeometry.CreateBox(thickness, height, thickness);
        var rightTransform = new TranslateTransform3D(width / 2, 0, 0);

        var archTop = CreateGeometry.CreateBox(width + thickness * 2, thickness, thickness);
        var archTransform = new TranslateTransform3D(0, height / 2, 0);

        mesh = MergeMeshes(leftPillar, rightPillar);
        mesh = MergeMeshes(mesh, archTop);

        return mesh;
    }

    public MeshGeometry3D GenerateBridge(Point3D start, Point3D end, double width, double height)
    {
        var mesh = new MeshGeometry3D();

        var dx = end.X - start.X;
        var dz = end.Z - start.Z;
        var length = Math.Sqrt(dx * dx + dz * dz);
        var angle = Math.Atan2(dz, dx);

        var bridgeBase = CreateGeometry.CreateBox(length, height, width);

        return bridgeBase;
    }

    public MeshGeometry3D GenerateCircularPlatform(double radius, double height, int segments)
    {
        var mesh = CreateGeometry.CreateCylinder(radius, height, segments);
        return mesh;
    }

    public MeshGeometry3D GenerateRingSegments(double radius, double thickness, double height, int segments)
    {
        var mesh = new MeshGeometry3D();

        for (int i = 0; i < segments; i++)
        {
            var angle1 = 2 * Math.PI * i / segments;
            var angle2 = 2 * Math.PI * (i + 0.8) / segments;

            var x1 = radius * Math.Cos(angle1);
            var z1 = radius * Math.Sin(angle1);
            var x2 = radius * Math.Cos(angle2);
            var z2 = radius * Math.Sin(angle2);

            var nextI = (i + 1) % segments;
            var nextAngle1 = 2 * Math.PI * nextI / segments;
            var nextAngle2 = 2 * Math.PI * (nextI + 0.8) / segments;

            var nx1 = radius * Math.Cos(nextAngle1);
            var nz1 = radius * Math.Sin(nextAngle1);
            var nx2 = radius * Math.Cos(nextAngle2);
            var nz2 = radius * Math.Sin(nextAngle2);

            var baseIndex = mesh.Positions.Count;

            mesh.Positions.Add(new Point3D(x1 - thickness / 2, -height / 2, z1));
            mesh.Positions.Add(new Point3D(x2 + thickness / 2, -height / 2, z2));
            mesh.Positions.Add(new Point3D(x2 + thickness / 2, height / 2, z2));
            mesh.Positions.Add(new Point3D(x1 - thickness / 2, height / 2, z1));

            mesh.Positions.Add(new Point3D(nx1 - thickness / 2, -height / 2, nz1));
            mesh.Positions.Add(new Point3D(nx2 + thickness / 2, -height / 2, nz2));
            mesh.Positions.Add(new Point3D(nx2 + thickness / 2, height / 2, nz2));
            mesh.Positions.Add(new Point3D(nx1 - thickness / 2, height / 2, nz1));

            mesh.TriangleIndices.Add(baseIndex); mesh.TriangleIndices.Add(baseIndex + 1); mesh.TriangleIndices.Add(baseIndex + 5);
            mesh.TriangleIndices.Add(baseIndex); mesh.TriangleIndices.Add(baseIndex + 5); mesh.TriangleIndices.Add(baseIndex + 4);

            mesh.TriangleIndices.Add(baseIndex + 1); mesh.TriangleIndices.Add(baseIndex + 2); mesh.TriangleIndices.Add(baseIndex + 6);
            mesh.TriangleIndices.Add(baseIndex + 1); mesh.TriangleIndices.Add(baseIndex + 6); mesh.TriangleIndices.Add(baseIndex + 5);

            mesh.TriangleIndices.Add(baseIndex + 2); mesh.TriangleIndices.Add(baseIndex + 3); mesh.TriangleIndices.Add(baseIndex + 7);
            mesh.TriangleIndices.Add(baseIndex + 2); mesh.TriangleIndices.Add(baseIndex + 7); mesh.TriangleIndices.Add(baseIndex + 6);

            mesh.TriangleIndices.Add(baseIndex + 3); mesh.TriangleIndices.Add(baseIndex); mesh.TriangleIndices.Add(baseIndex + 4);
            mesh.TriangleIndices.Add(baseIndex + 3); mesh.TriangleIndices.Add(baseIndex + 4); mesh.TriangleIndices.Add(baseIndex + 7);
        }

        return mesh;
    }

    public MeshGeometry3D GenerateCarvedStoneBlock(double width, double height, double depth)
    {
        return CreateGeometry.CreateBox(width, height, depth);
    }

    private MeshGeometry3D MergeMeshes(MeshGeometry3D mesh1, MeshGeometry3D mesh2)
    {
        var merged = new MeshGeometry3D();

        var offset = mesh1.Positions.Count;

        foreach (var pos in mesh1.Positions)
        {
            merged.Positions.Add(pos);
        }

        foreach (var pos in mesh2.Positions)
        {
            merged.Positions.Add(pos);
        }

        foreach (var idx in mesh1.TriangleIndices)
        {
            merged.TriangleIndices.Add(idx);
        }

        foreach (var idx in mesh2.TriangleIndices)
        {
            merged.TriangleIndices.Add(idx + offset);
        }

        return merged;
    }
}
