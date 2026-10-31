using System.Windows.Media.Media3D;

namespace ABYSSAL;

public class CinematicCameraRig
{
    public PerspectiveCamera Camera { get; private set; }

    private Point3D _position;
    private Vector3D _lookDirection;
    private Vector3D _upDirection = new(0, 1, 0);
    private double _fieldOfView = 60;
    private double _banking;

    public void Initialize(Point3D position, Vector3D lookDirection)
    {
        _position = position;
        _lookDirection = lookDirection;
        _lookDirection.Normalize();

        Camera = new PerspectiveCamera
        {
            Position = _position,
            LookDirection = _lookDirection,
            UpDirection = _upDirection,
            FieldOfView = _fieldOfView
        };
    }

    public void Update(CameraShot? shot, TimeSpan time)
    {
        if (shot == null) return;

        var t = shot.GetProgress(time);
        var easedT = EaseInOutCubic(t);

        var position = GetInterpolatedPosition(shot, easedT);
        var lookTarget = shot.LookTarget;

        var direction = lookTarget - position;
        if (direction.Length > 0.001)
        {
            direction.Normalize();
            _lookDirection = direction;
        }

        _position = position;
        Camera.Position = _position;
        Camera.LookDirection = _lookDirection;

        Camera.UpDirection = CalculateUpDirection(_lookDirection, _banking);
    }

    private Point3D GetInterpolatedPosition(CameraShot shot, double t)
    {
        var start = shot.StartPosition;
        var end = shot.EndPosition;

        return new Point3D(
            CatmullRomInterpolate(start.X, end.X, t),
            CatmullRomInterpolate(start.Y, end.Y, t),
            CatmullRomInterpolate(start.Z, end.Z, t)
        );
    }

    private static double CatmullRomInterpolate(double p0, double p1, double t)
    {
        double t2 = t * t;
        double t3 = t2 * t;
        return p0 + (p1 - p0) * (3 * t2 - 2 * t3);
    }

    private static double EaseInOutCubic(double t)
    {
        return t < 0.5 ? 4 * t * t * t : 1 - Math.Pow(-2 * t + 2, 3) / 2;
    }

    private Vector3D CalculateUpDirection(Vector3D lookDir, double banking)
    {
        var up = _upDirection;
        if (Math.Abs(Vector3D.DotProduct(lookDir, up)) > 0.95)
        {
            up = new Vector3D(0, 0, 1);
        }

        var right = Vector3D.CrossProduct(lookDir, up);
        if (right.Length < 0.001)
        {
            right = new Vector3D(1, 0, 0);
        }
        right.Normalize();

        var bankedUp = RotateVectorAround(up, right, banking);
        if (bankedUp.Length > 0.001) bankedUp.Normalize();

        return bankedUp;
    }

    private static Vector3D RotateVectorAround(Vector3D v, Vector3D axis, double angleRad)
    {
        var cos = Math.Cos(angleRad);
        var sin = Math.Sin(angleRad);
        var dot = Vector3D.DotProduct(axis, v);
        var cross = Vector3D.CrossProduct(axis, v);

        return new Vector3D(
            v.X * cos + cross.X * sin + axis.X * dot * (1 - cos),
            v.Y * cos + cross.Y * sin + axis.Y * dot * (1 - cos),
            v.Z * cos + cross.Z * sin + axis.Z * dot * (1 - cos)
        );
    }

    public void SetBanking(double degrees)
    {
        _banking = degrees * Math.PI / 180.0;
    }

    public void SetFieldOfView(double degrees)
    {
        _fieldOfView = degrees;
        Camera.FieldOfView = _fieldOfView;
    }
}
