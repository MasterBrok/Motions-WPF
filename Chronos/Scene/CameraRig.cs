using System.Windows.Media.Media3D;

namespace Chronos.Scene;

public class CameraRig
{
    private PerspectiveCamera _camera = null!;
    private Point3D _currentPosition;
    private Vector3D _currentLookDirection;
    private Point3D _lookTarget = new(0, 0, 0);
    private Vector3D _upDirection = new(0, 1, 0);
    private double _fieldOfView = 60;

    public PerspectiveCamera Camera => _camera;

    public void Initialize(Point3D center)
    {
        _camera = new PerspectiveCamera
        {
            Position = new Point3D(0, 2, 15),
            LookDirection = new Vector3D(0, 0, -1),
            UpDirection = new Vector3D(0, 1, 0),
            FieldOfView = _fieldOfView,
            NearPlaneDistance = 0.1,
            FarPlaneDistance = 100
        };

        _currentPosition = _camera.Position;
        _currentLookDirection = _camera.LookDirection;
    }

    public void SnapTo(double t)
    {
        UpdateComputed(t, snap: true);
    }

    public void Update(TimeSpan elapsed)
    {
        UpdateComputed(elapsed.TotalSeconds, snap: false);
    }

    private void UpdateComputed(double t, bool snap)
    {
        double x, y, z;
        double lookX = 0, lookY = 0, lookZ = 0;

        if (t < 5)
        {
            double phase = t / 5.0;
            x = Math.Sin(phase * Math.PI * 0.3) * 2;
            y = 2 + phase * 2;
            z = 15 - phase * 5;
            _fieldOfView = 60 + phase * 20;
        }
        else if (t < 10)
        {
            double phase = (t - 5) / 5.0;
            double angle = phase * Math.PI * 1.5;
            x = Math.Sin(angle) * 12;
            y = Math.Sin(phase * Math.PI * 0.5) * 3;
            z = Math.Cos(angle) * 12;
            _fieldOfView = 80;
            lookY = Math.Sin(phase * Math.PI) * 0.5;
        }
        else if (t < 16)
        {
            double phase = (t - 10) / 6.0;
            double angle = Math.PI * 0.5 + phase * Math.PI * 0.5;
            x = Math.Sin(angle) * 15;
            y = 2 + Math.Sin(phase * Math.PI * 2) * 2;
            z = Math.Cos(angle) * 10 + phase * 5;
            _fieldOfView = 80 - phase * 10;
        }
        else if (t < 22)
        {
            double phase = (t - 16) / 6.0;
            double angle = Math.PI + phase * Math.PI * 0.3;
            x = Math.Sin(angle) * (18 - phase * 8);
            y = 4 - phase * 3;
            z = Math.Cos(angle) * (8 - phase * 3);
            _fieldOfView = 70;
            lookY = -phase * 0.5;
        }
        else if (t < 27)
        {
            double phase = (t - 22) / 5.0;
            double angle = Math.PI * 1.3 + phase * 0.3;
            x = Math.Sin(angle) * (10 - phase * 4);
            y = 1 + phase * 1;
            z = 5 - phase * 2;
            _fieldOfView = 70 - phase * 15;
            lookX = phase * 2;
        }
        else
        {
            double phase = (t - 27) / 3.0;
            x = Math.Sin(Math.PI * 1.6) * 6;
            y = 2 - phase * 0.3;
            z = 3 - phase * 0.5;
            _fieldOfView = 55;
            lookX = 2;
        }

        var targetPosition = new Point3D(x, y, z);
        var smoothFactor = snap ? 1.0 : 0.05;

        _currentPosition = new Point3D(
            _currentPosition.X + (targetPosition.X - _currentPosition.X) * smoothFactor,
            _currentPosition.Y + (targetPosition.Y - _currentPosition.Y) * smoothFactor,
            _currentPosition.Z + (targetPosition.Z - _currentPosition.Z) * smoothFactor
        );

        var targetLook = new Vector3D(lookX - x, lookY - y, lookZ - z);
        targetLook.Normalize();

        var smoothLook = snap ? 1.0 : 0.05;
        _currentLookDirection = new Vector3D(
            _currentLookDirection.X + (targetLook.X - _currentLookDirection.X) * smoothLook,
            _currentLookDirection.Y + (targetLook.Y - _currentLookDirection.Y) * smoothLook,
            _currentLookDirection.Z + (targetLook.Z - _currentLookDirection.Z) * smoothLook
        );

        _camera.Position = _currentPosition;
        _camera.LookDirection = _currentLookDirection;
        _camera.UpDirection = _upDirection;
        _camera.FieldOfView = _fieldOfView;
    }
}