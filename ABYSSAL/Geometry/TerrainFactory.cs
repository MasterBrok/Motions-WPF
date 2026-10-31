using System.Windows.Media.Media3D;

namespace ABYSSAL.Geometry;

public class TerrainFactory
{
    private readonly Random _seededRandom;

    public TerrainFactory(int seed = 123)
    {
        _seededRandom = new Random(seed);
    }

    public MeshGeometry3D GenerateTerrainMesh(double minY, double maxY, double size, int segments)
    {
        var mesh = new MeshGeometry3D();

        for (int z = 0; z <= segments; z++)
        {
            for (int x = 0; x <= segments; x++)
            {
                var px = (x / (double)segments - 0.5) * size;
                var pz = (z / (double)segments - 0.5) * size;

                var noise = GenerateNoise(x * 0.1, z * 0.1);
                var py = minY + (maxY - minY) * (0.5 + noise * 0.5);

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

        return mesh;
    }

    public MeshGeometry3D GenerateCanyonMesh(double width, double depth, double length, int segments)
    {
        var mesh = new MeshGeometry3D();

        for (int z = 0; z <= segments; z++)
        {
            for (int x = 0; x <= segments; x++)
            {
                var px = (x / (double)segments - 0.5) * width;
                var pz = (z / (double)segments - 0.5) * length;

                var canyonWidth = Math.Sin(z * Math.PI / segments) * 0.3 + 0.5;
                var edgeNoise = GenerateNoise(x * 0.2, z * 0.2) * 10;

                var baseY = depth;
                var py = baseY + edgeNoise;

                if (Math.Abs(px) < width * canyonWidth * 0.3)
                {
                    py += 30;
                }

                mesh.Positions.Add(new Point3D(px, py, pz));
            }
        }

        for (int z = 0; z < segments; z++)
        {
            for (int x = 0; x < segments; x++)
            {
                var idx = z * (segments + 1) + x;
                mesh.TriangleIndices.Add(idx);
                mesh.TriangleIndices.Add(idx + segments + 1);
                mesh.TriangleIndices.Add(idx + 1);

                mesh.TriangleIndices.Add(idx + 1);
                mesh.TriangleIndices.Add(idx + segments + 1);
                mesh.TriangleIndices.Add(idx + segments + 2);
            }
        }

        return mesh;
    }

    public MeshGeometry3D GenerateIrregularRock(double baseSize, double height, int detail)
    {
        var mesh = new MeshGeometry3D();

        var segments = detail * 4;
        var rings = detail * 2;

        for (int ring = 0; ring <= rings; ring++)
        {
            var phi = Math.PI * ring / rings;
            var y = height * Math.Cos(phi);
            var ringRadius = baseSize * Math.Sin(phi);

            var noise = GenerateNoise(ring * 0.5, 0) * 0.3 + 1;

            for (int seg = 0; seg <= segments; seg++)
            {
                var theta = 2 * Math.PI * seg / segments;
                var segNoise = GenerateNoise(seg * 0.3, ring * 0.5) * 0.4 + 1;

                var x = ringRadius * Math.Cos(theta) * segNoise * noise;
                var z = ringRadius * Math.Sin(theta) * segNoise * noise;

                mesh.Positions.Add(new Point3D(x, y, z));
            }
        }

        for (int ring = 0; ring < rings; ring++)
        {
            for (int seg = 0; seg < segments; seg++)
            {
                var current = ring * (segments + 1) + seg;
                var next = current + segments + 1;

                mesh.TriangleIndices.Add(current);
                mesh.TriangleIndices.Add(next);
                mesh.TriangleIndices.Add(current + 1);

                mesh.TriangleIndices.Add(current + 1);
                mesh.TriangleIndices.Add(next);
                mesh.TriangleIndices.Add(next + 1);
            }
        }

        return mesh;
    }

    public MeshGeometry3D GenerateCliff(double width, double height, int detail)
    {
        var mesh = new MeshGeometry3D();

        var segments = detail * 2;

        for (int z = 0; z <= segments; z++)
        {
            for (int x = 0; x <= segments; x++)
            {
                var px = (x / (double)segments - 0.5) * width;
                var pz = (z / (double)segments - 0.5) * width * 0.3;

                var baseNoise = GenerateNoise(x * 0.2, z * 0.3);
                var py = -height / 2 + baseNoise * 5;

                if (px > width * 0.3)
                {
                    py = -height / 2 + (px - width * 0.3) / (width * 0.7) * height * 0.8;
                }

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

        return mesh;
    }

    private double GenerateNoise(double x, double y)
    {
        var n = Math.Sin(x * 12.9898 + y * 78.233) * 43758.5453;
        return n - Math.Floor(n);
    }
}
