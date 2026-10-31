using System.Windows.Media.Media3D;

namespace ABYSSAL.Geometry;

public class MonumentFactory
{
    private readonly Random _seededRandom;

    public MonumentFactory(int seed = 789)
    {
        _seededRandom = new Random(seed);
    }

    public MeshGeometry3D GenerateStatue(double baseWidth, double height, int detail)
    {
        var mesh = new MeshGeometry3D();

        var torso = CreateGeometry.CreateBox(baseWidth, height * 0.5, baseWidth * 0.6);

        var head = CreateGeometry.CreateSphere(baseWidth * 0.3, detail, (int)(detail * 0.75));
        var headTransform = new TranslateTransform3D(0, height * 0.7, 0);
        ApplyTransform(head, headTransform);

        var leftArm = CreateGeometry.CreateBox(baseWidth * 0.2, height * 0.4, baseWidth * 0.2);
        var leftArmTransform = new TranslateTransform3D(-baseWidth * 0.6, height * 0.3, 0);
        ApplyTransform(leftArm, leftArmTransform);

        var rightArm = CreateGeometry.CreateBox(baseWidth * 0.2, height * 0.4, baseWidth * 0.2);
        var rightArmTransform = new TranslateTransform3D(baseWidth * 0.6, height * 0.3, 0);
        ApplyTransform(rightArm, rightArmTransform);

        var leftLeg = CreateGeometry.CreateBox(baseWidth * 0.25, height * 0.35, baseWidth * 0.25);
        var leftLegTransform = new TranslateTransform3D(-baseWidth * 0.25, -height * 0.325, 0);
        ApplyTransform(leftLeg, leftLegTransform);

        var rightLeg = CreateGeometry.CreateBox(baseWidth * 0.25, height * 0.35, baseWidth * 0.25);
        var rightLegTransform = new TranslateTransform3D(baseWidth * 0.25, -height * 0.325, 0);
        ApplyTransform(rightLeg, rightLegTransform);

        mesh = MergeMeshes(mesh, torso);
        mesh = MergeMeshes(mesh, head);
        mesh = MergeMeshes(mesh, leftArm);
        mesh = MergeMeshes(mesh, rightArm);
        mesh = MergeMeshes(mesh, leftLeg);
        mesh = MergeMeshes(mesh, rightLeg);

        var baseMesh = CreateGeometry.CreateBox(baseWidth * 1.5, height * 0.1, baseWidth * 1.2);
        var baseTransform = new TranslateTransform3D(0, -height * 0.45, 0);
        ApplyTransform(baseMesh, baseTransform);
        mesh = MergeMeshes(mesh, baseMesh);

        return mesh;
    }

    public MeshGeometry3D GenerateWornStatue(double baseWidth, double height, int detail)
    {
        var mesh = GenerateStatue(baseWidth, height, detail);

        for (int i = 0; i < mesh.Positions.Count; i++)
        {
            var pos = mesh.Positions[i];
            var erosion = _seededRandom.NextDouble() * 0.15;

            var newPos = new Point3D(
                pos.X * (1 - erosion),
                pos.Y * (1 - erosion * 0.5),
                pos.Z * (1 - erosion)
            );

            mesh.Positions[i] = newPos;
        }

        return mesh;
    }

    private void ApplyTransform(MeshGeometry3D mesh, TranslateTransform3D transform)
    {
        for (int i = 0; i < mesh.Positions.Count; i++)
        {
            mesh.Positions[i] = transform.Transform(mesh.Positions[i]);
        }
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
