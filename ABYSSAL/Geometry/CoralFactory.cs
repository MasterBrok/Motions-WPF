using System.Windows.Media.Media3D;

namespace ABYSSAL.Geometry;

public class CoralFactory
{
    private readonly Random _seededRandom;

    public CoralFactory(int seed = 999)
    {
        _seededRandom = new Random(seed);
    }

    public MeshGeometry3D GenerateBranchingCoral(double scale, int detail)
    {
        var mesh = new MeshGeometry3D();

        var branchCount = 3 + detail;
        var branches = new List<(Point3D Start, Vector3D Direction, double Length)>();

        branches.Add((new Point3D(0, 0, 0), new Vector3D(0, 1, 0), scale));

        for (int i = 0; i < branchCount; i++)
        {
            var parent = branches[_seededRandom.Next(branches.Count)];
            var dir = new Vector3D(
                _seededRandom.NextDouble() - 0.5,
                _seededRandom.NextDouble() * 0.5 + 0.5,
                _seededRandom.NextDouble() - 0.5
            );
            dir.Normalize();

            branches.Add((
                Point3D.Add(parent.Start, Vector3D.Multiply(parent.Direction, parent.Length)),
                dir,
                parent.Length * 0.6
            ));
        }

        foreach (var (start, dir, length) in branches)
        {
            var end = Point3D.Add(start, Vector3D.Multiply(dir, length));
            var radius = length * 0.1;

            var branchIndex = mesh.Positions.Count;

            var side1 = GetPerpendicular(dir);
            var side2 = Vector3D.CrossProduct(dir, side1);

            for (int seg = 0; seg < 6; seg++)
            {
                var angle = 2 * Math.PI * seg / 6;
                var offset = Vector3D.Add(
                    Vector3D.Multiply(side1, radius * Math.Cos(angle)),
                    Vector3D.Multiply(side2, radius * Math.Sin(angle))
                );

                mesh.Positions.Add(Point3D.Add(start, offset));
                mesh.Positions.Add(Point3D.Add(end, offset));
            }

            for (int seg = 0; seg < 6; seg++)
            {
                var next = (seg + 1) % 6;
                var base1 = branchIndex + seg * 2;
                var base2 = branchIndex + next * 2;

                mesh.TriangleIndices.Add(base1);
                mesh.TriangleIndices.Add(base1 + 1);
                mesh.TriangleIndices.Add(base2);

                mesh.TriangleIndices.Add(base2);
                mesh.TriangleIndices.Add(base1 + 1);
                mesh.TriangleIndices.Add(base2 + 1);
            }
        }

        return mesh;
    }

    public MeshGeometry3D GenerateCrystalFormation(double scale, int detail)
    {
        var mesh = new MeshGeometry3D();
        var crystalCount = 2 + detail / 3;

        for (int i = 0; i < crystalCount; i++)
        {
            var x = (_seededRandom.NextDouble() - 0.5) * scale * 1.5;
            var z = (_seededRandom.NextDouble() - 0.5) * scale * 1.5;
            var height = scale * (0.5 + _seededRandom.NextDouble());

            var crystal = GenerateSingleCrystal(x, z, height, scale * 0.3);

            var offset = mesh.Positions.Count;
            foreach (var pos in crystal.Positions)
            {
                mesh.Positions.Add(pos);
            }
            foreach (var idx in crystal.TriangleIndices)
            {
                mesh.TriangleIndices.Add(idx + offset);
            }
        }

        return mesh;
    }

    private MeshGeometry3D GenerateSingleCrystal(double x, double z, double height, double radius)
    {
        var mesh = new MeshGeometry3D();
        var points = 5;

        mesh.Positions.Add(new Point3D(x, height, z));

        for (int i = 0; i < points; i++)
        {
            var angle = 2 * Math.PI * i / points;
            mesh.Positions.Add(new Point3D(
                x + radius * Math.Cos(angle),
                0,
                z + radius * Math.Sin(angle)
            ));
        }

        for (int i = 1; i <= points; i++)
        {
            mesh.TriangleIndices.Add(0);
            mesh.TriangleIndices.Add(i);
            mesh.TriangleIndices.Add(i % points + 1);
        }

        return mesh;
    }

    public MeshGeometry3D GenerateCoralCluster(double scale, int detail)
    {
        var mesh = new MeshGeometry3D();
        var blobCount = 3 + detail;

        for (int i = 0; i < blobCount; i++)
        {
            var x = (_seededRandom.NextDouble() - 0.5) * scale * 2;
            var y = _seededRandom.NextDouble() * scale * 0.5;
            var z = (_seededRandom.NextDouble() - 0.5) * scale * 2;
            var radius = scale * (0.2 + _seededRandom.NextDouble() * 0.4);

            var segments = 6;
            var rings = 4;

            var startIndex = mesh.Positions.Count;

            for (int ring = 0; ring <= rings; ring++)
            {
                var phi = Math.PI * ring / rings;
                var ringY = y + radius * Math.Cos(phi);
                var ringRadius = radius * Math.Sin(phi);

                for (int seg = 0; seg <= segments; seg++)
                {
                    var theta = 2 * Math.PI * seg / segments;
                    mesh.Positions.Add(new Point3D(
                        x + ringRadius * Math.Cos(theta),
                        ringY,
                        z + ringRadius * Math.Sin(theta)
                    ));
                }
            }

            for (int ring = 0; ring < rings; ring++)
            {
                for (int seg = 0; seg < segments; seg++)
                {
                    var current = startIndex + ring * (segments + 1) + seg;
                    var next = current + segments + 1;

                    mesh.TriangleIndices.Add(current);
                    mesh.TriangleIndices.Add(next);
                    mesh.TriangleIndices.Add(current + 1);

                    mesh.TriangleIndices.Add(current + 1);
                    mesh.TriangleIndices.Add(next);
                    mesh.TriangleIndices.Add(next + 1);
                }
            }
        }

        return mesh;
    }

    private Vector3D GetPerpendicular(Vector3D dir)
    {
        if (Math.Abs(dir.X) < 0.9)
        {
            return Vector3D.CrossProduct(dir, new Vector3D(1, 0, 0));
        }
        return Vector3D.CrossProduct(dir, new Vector3D(0, 1, 0));
    }
}
