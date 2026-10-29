using System.Windows.Media.Media3D;
using Chronos.Geometry;
using Chronos.Materials;

namespace Chronos.Models;

public class TitleText3D
{
    public static Model3DGroup CreateTitle(string text)
    {
        var group = new Model3DGroup();
        double spacing = 1.4;
        double totalWidth = (text.Length - 1) * spacing;
        double startX = -totalWidth / 2;

        for (int i = 0; i < text.Length; i++)
        {
            char c = char.ToUpper(text[i]);
            var letterMesh = CreateLetterMesh(c);
            if (letterMesh != null)
            {
                var model = new GeometryModel3D
                {
                    Geometry = letterMesh,
                    Material = MaterialLibrary.CreatePolishedChrome(),
                    BackMaterial = MaterialLibrary.CreatePolishedChrome()
                };
                model.Freeze();

                var transform = new TranslateTransform3D(startX + i * spacing, -2.5, 5);
                var transformModel = new Model3DGroup();
                transformModel.Children.Add(model);
                transformModel.Transform = transform;

                group.Children.Add(transformModel);
            }
        }

        group.Freeze();
        return group;
    }

    private static MeshGeometry3D? CreateLetterMesh(char c)
    {
        return c switch
        {
            'C' => CreateCMesh(),
            'H' => CreateHMesh(),
            'R' => CreateRMesh(),
            'O' => CreateOMesh(),
            'S' => CreateSMesh(),
            _ => CreateBoxMesh(0.8, 1.0, 0.2)
        };
    }

    private static MeshGeometry3D CreateCMesh()
    {
        var mesh = new MeshGeometry3D();
        double width = 1.0;
        double height = 1.0;
        double thickness = 0.15;

        for (int side = 0; side < 2; side++)
        {
            double z = side == 0 ? thickness / 2 : -thickness / 2;
            int startIdx = mesh.Positions.Count;

            mesh.Positions.Add(new Point3D(width, height, z));
            mesh.Positions.Add(new Point3D(-width, height, z));
            mesh.Positions.Add(new Point3D(-width, -height, z));
            mesh.Positions.Add(new Point3D(width, -height, z));

            var normal = new Vector3D(0, 0, side == 0 ? 1 : -1);
            for (int j = 0; j < 4; j++) mesh.Normals.Add(normal);

            mesh.TriangleIndices.Add(startIdx);
            mesh.TriangleIndices.Add(startIdx + 1);
            mesh.TriangleIndices.Add(startIdx + 2);
            mesh.TriangleIndices.Add(startIdx);
            mesh.TriangleIndices.Add(startIdx + 2);
            mesh.TriangleIndices.Add(startIdx + 3);
        }

        AddSide(mesh, -width, -height, width, height, thickness);
        AddSide(mesh, width, -height, width, height, thickness);

        mesh.Freeze();
        return mesh;
    }

    private static MeshGeometry3D CreateHMesh()
    {
        var mesh = new MeshGeometry3D();
        double barWidth = 0.15;
        double height = 1.0;
        double thickness = 0.15;

        var leftBar = CylinderMeshFactory.Create(barWidth / 2, height * 2, 16, false);
        var rightBar = CylinderMeshFactory.Create(barWidth / 2, height * 2, 16, false);
        var middleBar = CylinderMeshFactory.Create(barWidth / 2, barWidth * 4, 16, false);

        var leftModel = new GeometryModel3D { Geometry = leftBar, Material = MaterialLibrary.CreatePolishedChrome() };
        var rightModel = new GeometryModel3D { Geometry = rightBar, Material = MaterialLibrary.CreatePolishedChrome() };
        var middleModel = new GeometryModel3D { Geometry = middleBar, Material = MaterialLibrary.CreatePolishedChrome() };

        leftModel.Transform = new TranslateTransform3D(-0.4, 0, 0);
        rightModel.Transform = new TranslateTransform3D(0.4, 0, 0);
        middleModel.Transform = new TranslateTransform3D(0, 0, 0);

        var group = new Model3DGroup();
        group.Children.Add(leftModel);
        group.Children.Add(rightModel);
        group.Children.Add(middleModel);

        return CreateBoxMesh(1.0, 2.0, thickness);
    }

    private static MeshGeometry3D CreateRMesh()
    {
        return CreateBoxMesh(0.9, 1.0, 0.15);
    }

    private static MeshGeometry3D CreateOMesh()
    {
        var mesh = new MeshGeometry3D();
        double outerR = 0.5;
        double innerR = 0.35;
        double thickness = 0.15;

        int segments = 32;
        for (int i = 0; i < segments; i++)
        {
            double angle1 = 2 * Math.PI * i / segments;
            double angle2 = 2 * Math.PI * (i + 1) / segments;

            for (int z = 0; z < 2; z++)
            {
                double zPos = z == 0 ? thickness / 2 : -thickness / 2;
                int idx = mesh.Positions.Count;

                mesh.Positions.Add(new Point3D(Math.Cos(angle1) * outerR, Math.Sin(angle1) * outerR, zPos));
                mesh.Positions.Add(new Point3D(Math.Cos(angle2) * outerR, Math.Sin(angle2) * outerR, zPos));
                mesh.Positions.Add(new Point3D(Math.Cos(angle2) * innerR, Math.Sin(angle2) * innerR, zPos));
                mesh.Positions.Add(new Point3D(Math.Cos(angle1) * innerR, Math.Sin(angle1) * innerR, zPos));

                var normal = new Vector3D(0, 0, z == 0 ? 1 : -1);
                for (int j = 0; j < 4; j++) mesh.Normals.Add(normal);

                mesh.TriangleIndices.Add(idx);
                mesh.TriangleIndices.Add(idx + 1);
                mesh.TriangleIndices.Add(idx + 2);
                mesh.TriangleIndices.Add(idx);
                mesh.TriangleIndices.Add(idx + 2);
                mesh.TriangleIndices.Add(idx + 3);
            }
        }

        mesh.Freeze();
        return mesh;
    }

    private static MeshGeometry3D CreateSMesh()
    {
        return CreateBoxMesh(0.8, 1.0, 0.15);
    }

    private static MeshGeometry3D CreateBoxMesh(double width, double height, double depth)
    {
        var mesh = new MeshGeometry3D();
        double hw = width / 2;
        double hh = height / 2;
        double hd = depth / 2;

        mesh.Positions.Add(new Point3D(-hw, hh, hd));
        mesh.Positions.Add(new Point3D(hw, hh, hd));
        mesh.Positions.Add(new Point3D(hw, -hh, hd));
        mesh.Positions.Add(new Point3D(-hw, -hh, hd));

        mesh.Positions.Add(new Point3D(-hw, hh, -hd));
        mesh.Positions.Add(new Point3D(hw, hh, -hd));
        mesh.Positions.Add(new Point3D(hw, -hh, -hd));
        mesh.Positions.Add(new Point3D(-hw, -hh, -hd));

        mesh.Normals.Add(new Vector3D(0, 0, 1));
        mesh.Normals.Add(new Vector3D(0, 0, 1));
        mesh.Normals.Add(new Vector3D(0, 0, 1));
        mesh.Normals.Add(new Vector3D(0, 0, 1));

        mesh.Normals.Add(new Vector3D(0, 0, -1));
        mesh.Normals.Add(new Vector3D(0, 0, -1));
        mesh.Normals.Add(new Vector3D(0, 0, -1));
        mesh.Normals.Add(new Vector3D(0, 0, -1));

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

    private static void AddSide(MeshGeometry3D mesh, double x1, double y1, double x2, double y2, double thickness)
    {
        double z = thickness / 2;
        int idx = mesh.Positions.Count;

        mesh.Positions.Add(new Point3D(x1, y1, z));
        mesh.Positions.Add(new Point3D(x2, y2, z));
        mesh.Positions.Add(new Point3D(x2, y2, -z));
        mesh.Positions.Add(new Point3D(x1, y1, -z));

        var normal = new Vector3D(x2 - x1, y2 - y1, 0);
        normal.Normalize();
        for (int j = 0; j < 4; j++) mesh.Normals.Add(normal);

        mesh.TriangleIndices.Add(idx);
        mesh.TriangleIndices.Add(idx + 1);
        mesh.TriangleIndices.Add(idx + 2);
        mesh.TriangleIndices.Add(idx);
        mesh.TriangleIndices.Add(idx + 2);
        mesh.TriangleIndices.Add(idx + 3);
    }

    public static Model3DGroup CreateSubtitle()
    {
        var group = new Model3DGroup();

        for (int i = 0; i < 26; i++)
        {
            double x = -12 + i * 0.95;
            var mesh = CreateBoxMesh(0.7, 0.3, 0.05);
            var model = new GeometryModel3D
            {
                Geometry = mesh,
                Material = MaterialLibrary.CreateBrushedGold()
            };
            model.Freeze();

            var transform = new TranslateTransform3D(x, -3.8, 5);
            var transformModel = new Model3DGroup();
            transformModel.Children.Add(model);
            transformModel.Transform = transform;

            group.Children.Add(transformModel);
        }

        group.Freeze();
        return group;
    }
}