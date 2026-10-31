using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace ABYSSAL.Diagnostics;

public class SceneStatistics
{
    public int GeometryCount { get; set; }
    public int TriangleCount { get; set; }
    public int ParticleCount { get; set; }
    public int CreatureCount { get; set; }
    public int LightCount { get; set; }
    public double LastUpdateTime { get; set; }

    public string GetReport()
    {
        return $"Scene Statistics:\n" +
               $"- Geometry Objects: {GeometryCount}\n" +
               $"- Approximate Triangles: {TriangleCount:N0}\n" +
               $"- Particles: {ParticleCount}\n" +
               $"- Creatures: {CreatureCount}\n" +
               $"- Lights: {LightCount}\n" +
               $"- Update Time: {LastUpdateTime:F2}ms";
    }
}
