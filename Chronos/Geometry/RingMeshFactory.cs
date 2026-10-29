using System.Windows;
using System.Windows.Media.Media3D;

namespace Chronos.Geometry;

public static class RingMeshFactory
{
    public static MeshGeometry3D Create(double innerRadius, double outerRadius, double thickness, int segments = 64)
    {
        var mesh = new MeshGeometry3D();
        var halfThickness = thickness / 2;

        for (int i = 0; i < segments; i++)
        {
            double angle = 2 * Math.PI * i / segments;
            double cos = Math.Cos(angle);
            double sin = Math.Sin(angle);

            double innerX = cos * innerRadius;
            double innerZ = sin * innerRadius;
            double outerX = cos * outerRadius;
            double outerZ = sin * outerRadius;

            var outerNormal = new Vector3D(cos, 0, sin);
            var innerNormal = new Vector3D(-cos, 0, -sin);
            var topNormal = new Vector3D(0, 1, 0);
            var bottomNormal = new Vector3D(0, -1, 0);

            int outerTop = mesh.Positions.Count;
            mesh.Positions.Add(new Point3D(outerX, halfThickness, outerZ));
            mesh.Normals.Add(outerNormal);
            mesh.TextureCoordinates.Add(new Point((double)i / segments, 0));

            int outerBottom = mesh.Positions.Count;
            mesh.Positions.Add(new Point3D(outerX, -halfThickness, outerZ));
            mesh.Normals.Add(outerNormal);
            mesh.TextureCoordinates.Add(new Point((double)i / segments, 1));

            int innerTop = mesh.Positions.Count;
            mesh.Positions.Add(new Point3D(innerX, halfThickness, innerZ));
            mesh.Normals.Add(innerNormal);
            mesh.TextureCoordinates.Add(new Point((double)i / segments, 0));

            int innerBottom = mesh.Positions.Count;
            mesh.Positions.Add(new Point3D(innerX, -halfThickness, innerZ));
            mesh.Normals.Add(innerNormal);
            mesh.TextureCoordinates.Add(new Point((double)i / segments, 1));

            int nextOuterTop = mesh.Positions.Count;
            double nextAngle = 2 * Math.PI * (i + 1) / segments;
            mesh.Positions.Add(new Point3D(Math.Cos(nextAngle) * outerRadius, halfThickness, Math.Sin(nextAngle) * outerRadius));
            mesh.Normals.Add(outerNormal);
            mesh.TextureCoordinates.Add(new Point((double)(i + 1) / segments, 0));

            int nextOuterBottom = mesh.Positions.Count;
            mesh.Positions.Add(new Point3D(Math.Cos(nextAngle) * outerRadius, -halfThickness, Math.Sin(nextAngle) * outerRadius));
            mesh.Normals.Add(outerNormal);
            mesh.TextureCoordinates.Add(new Point((double)(i + 1) / segments, 1));

            int nextInnerTop = mesh.Positions.Count;
            mesh.Positions.Add(new Point3D(Math.Cos(nextAngle) * innerRadius, halfThickness, Math.Sin(nextAngle) * innerRadius));
            mesh.Normals.Add(innerNormal);
            mesh.TextureCoordinates.Add(new Point((double)(i + 1) / segments, 0));

            int nextInnerBottom = mesh.Positions.Count;
            mesh.Positions.Add(new Point3D(Math.Cos(nextAngle) * innerRadius, -halfThickness, Math.Sin(nextAngle) * innerRadius));
            mesh.Normals.Add(innerNormal);
            mesh.TextureCoordinates.Add(new Point((double)(i + 1) / segments, 1));

            AddQuad(mesh, outerTop, nextOuterTop, nextOuterBottom, outerBottom);
            AddQuad(mesh, nextInnerTop, innerTop, innerBottom, nextInnerBottom);
            AddQuad(mesh, outerTop, outerBottom, innerBottom, innerTop);
            AddQuad(mesh, nextOuterTop, nextOuterBottom, nextInnerBottom, nextInnerTop);

            int capTopCenter = mesh.Positions.Count;
            mesh.Positions.Add(new Point3D(0, halfThickness, 0));
            mesh.Normals.Add(new Vector3D(0, 1, 0));
            mesh.TextureCoordinates.Add(new Point(0.5, 0.5));

            int capBottomCenter = mesh.Positions.Count;
            mesh.Positions.Add(new Point3D(0, -halfThickness, 0));
            mesh.Normals.Add(new Vector3D(0, -1, 0));
            mesh.TextureCoordinates.Add(new Point(0.5, 0.5));

            mesh.TriangleIndices.Add(capTopCenter);
            mesh.TriangleIndices.Add(outerTop);
            mesh.TriangleIndices.Add(outerBottom);

            mesh.TriangleIndices.Add(capBottomCenter);
            mesh.TriangleIndices.Add(innerTop);
            mesh.TriangleIndices.Add(innerBottom);
        }

        mesh.Freeze();
        return mesh;
    }

    private static void AddQuad(MeshGeometry3D mesh, int a, int b, int c, int d)
    {
        mesh.TriangleIndices.Add(a);
        mesh.TriangleIndices.Add(b);
        mesh.TriangleIndices.Add(c);
        mesh.TriangleIndices.Add(a);
        mesh.TriangleIndices.Add(c);
        mesh.TriangleIndices.Add(d);
    }
}