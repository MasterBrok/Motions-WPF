using System.Windows.Media.Media3D;

namespace ABYSSAL.Geometry;

public static class CreateGeometry
{
    public static MeshGeometry3D CreateBox(double width, double height, double depth)
    {
        var mesh = new MeshGeometry3D();

        var hw = width / 2;
        var hh = height / 2;
        var hd = depth / 2;

        mesh.Positions.Add(new Point3D(-hw, -hh, -hd));
        mesh.Positions.Add(new Point3D(hw, -hh, -hd));
        mesh.Positions.Add(new Point3D(hw, hh, -hd));
        mesh.Positions.Add(new Point3D(-hw, hh, -hd));

        mesh.Positions.Add(new Point3D(-hw, -hh, hd));
        mesh.Positions.Add(new Point3D(hw, -hh, hd));
        mesh.Positions.Add(new Point3D(hw, hh, hd));
        mesh.Positions.Add(new Point3D(-hw, hh, hd));

        mesh.TriangleIndices.Add(0); mesh.TriangleIndices.Add(2); mesh.TriangleIndices.Add(1);
        mesh.TriangleIndices.Add(0); mesh.TriangleIndices.Add(3); mesh.TriangleIndices.Add(2);

        mesh.TriangleIndices.Add(4); mesh.TriangleIndices.Add(5); mesh.TriangleIndices.Add(6);
        mesh.TriangleIndices.Add(4); mesh.TriangleIndices.Add(6); mesh.TriangleIndices.Add(7);

        mesh.TriangleIndices.Add(0); mesh.TriangleIndices.Add(1); mesh.TriangleIndices.Add(5);
        mesh.TriangleIndices.Add(0); mesh.TriangleIndices.Add(5); mesh.TriangleIndices.Add(4);

        mesh.TriangleIndices.Add(2); mesh.TriangleIndices.Add(3); mesh.TriangleIndices.Add(7);
        mesh.TriangleIndices.Add(2); mesh.TriangleIndices.Add(7); mesh.TriangleIndices.Add(6);

        mesh.TriangleIndices.Add(0); mesh.TriangleIndices.Add(4); mesh.TriangleIndices.Add(7);
        mesh.TriangleIndices.Add(0); mesh.TriangleIndices.Add(7); mesh.TriangleIndices.Add(3);

        mesh.TriangleIndices.Add(1); mesh.TriangleIndices.Add(2); mesh.TriangleIndices.Add(6);
        mesh.TriangleIndices.Add(1); mesh.TriangleIndices.Add(6); mesh.TriangleIndices.Add(5);

        return mesh;
    }

    public static MeshGeometry3D CreateSphere(double radius, int segments = 16, int rings = 12)
    {
        var mesh = new MeshGeometry3D();

        for (int ring = 0; ring <= rings; ring++)
        {
            var phi = Math.PI * ring / rings;
            var y = radius * Math.Cos(phi);
            var ringRadius = radius * Math.Sin(phi);

            for (int seg = 0; seg <= segments; seg++)
            {
                var theta = 2 * Math.PI * seg / segments;
                var x = ringRadius * Math.Cos(theta);
                var z = ringRadius * Math.Sin(theta);

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

    public static MeshGeometry3D CreateCylinder(double radius, double height, int segments = 16)
    {
        var mesh = new MeshGeometry3D();
        var halfHeight = height / 2;

        mesh.Positions.Add(new Point3D(0, halfHeight, 0));
        for (int i = 0; i <= segments; i++)
        {
            var angle = 2 * Math.PI * i / segments;
            mesh.Positions.Add(new Point3D(radius * Math.Cos(angle), halfHeight, radius * Math.Sin(angle)));
        }

        mesh.Positions.Add(new Point3D(0, -halfHeight, 0));
        for (int i = 0; i <= segments; i++)
        {
            var angle = 2 * Math.PI * i / segments;
            mesh.Positions.Add(new Point3D(radius * Math.Cos(angle), -halfHeight, radius * Math.Sin(angle)));
        }

        mesh.TriangleIndices.Add(0);
        for (int i = 1; i <= segments; i++)
        {
            mesh.TriangleIndices.Add(i);
        }
        mesh.TriangleIndices.Add(1);

        var bottomCenter = segments + 2;
        mesh.TriangleIndices.Add(bottomCenter);
        for (int i = segments; i >= 1; i--)
        {
            mesh.TriangleIndices.Add(bottomCenter + i);
        }
        mesh.TriangleIndices.Add(bottomCenter + segments);

        for (int i = 0; i < segments; i++)
        {
            mesh.TriangleIndices.Add(1 + i);
            mesh.TriangleIndices.Add(bottomCenter + 1 + i);
            mesh.TriangleIndices.Add(2 + i);

            mesh.TriangleIndices.Add(2 + i);
            mesh.TriangleIndices.Add(bottomCenter + 1 + i);
            mesh.TriangleIndices.Add(bottomCenter + 2 + i);
        }

        return mesh;
    }

    public static MeshGeometry3D CreateTorus(double majorRadius, double minorRadius, int majorSegments = 24, int minorSegments = 12)
    {
        var mesh = new MeshGeometry3D();

        for (int i = 0; i <= majorSegments; i++)
        {
            var u = 2 * Math.PI * i / majorSegments;
            var cu = Math.Cos(u);
            var su = Math.Sin(u);

            for (int j = 0; j <= minorSegments; j++)
            {
                var v = 2 * Math.PI * j / minorSegments;
                var cv = Math.Cos(v);
                var sv = Math.Sin(v);

                var x = (majorRadius + minorRadius * cv) * cu;
                var y = (majorRadius + minorRadius * cv) * su;
                var z = minorRadius * sv;

                mesh.Positions.Add(new Point3D(x, y, z));
            }
        }

        for (int i = 0; i < majorSegments; i++)
        {
            for (int j = 0; j < minorSegments; j++)
        {
                var current = i * (minorSegments + 1) + j;
                var next = current + minorSegments + 1;

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

    public static MeshGeometry3D CreatePlane(double width, double depth, int widthSegments = 1, int depthSegments = 1)
    {
        var mesh = new MeshGeometry3D();

        var hw = width / 2;
        var hd = depth / 2;

        for (int z = 0; z <= depthSegments; z++)
        {
            for (int x = 0; x <= widthSegments; x++)
            {
                var px = (x / (double)widthSegments - 0.5) * width;
                var pz = (z / (double)depthSegments - 0.5) * depth;
                mesh.Positions.Add(new Point3D(px, 0, pz));
            }
        }

        for (int z = 0; z < depthSegments; z++)
        {
            for (int x = 0; x < widthSegments; x++)
            {
                var idx = z * (widthSegments + 1) + x;
                mesh.TriangleIndices.Add(idx);
                mesh.TriangleIndices.Add(idx + 1);
                mesh.TriangleIndices.Add(idx + widthSegments + 1);

                mesh.TriangleIndices.Add(idx + 1);
                mesh.TriangleIndices.Add(idx + widthSegments + 2);
                mesh.TriangleIndices.Add(idx + widthSegments + 1);
            }
        }

        return mesh;
    }
}
