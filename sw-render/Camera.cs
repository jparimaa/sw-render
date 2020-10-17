using System.Numerics;

class Camera
{
    public Vector3 Position { get; set; }
    public Vector3 Rotation { get; set; }
    public float AspectRatio { get; set; }

    private readonly Vector4 c_forward;

    public Camera()
    {
        Position = new Vector3(0, 0, 10.0f);
        Rotation = new Vector3(0);
        AspectRatio = 800 / 600;
        c_forward = new Vector4(0, 0, -1, 0);
    }

    public Vector3 GetForward()
    {
        var forward4 = Vector4.Transform(c_forward, Matrix4x4.CreateFromYawPitchRoll(Rotation.X, Rotation.Y, Rotation.Z));
        return new Vector3(forward4.X, forward4.Y, forward4.Z);
    }

    public Matrix4x4 GetViewMatrix()
    {
        return Matrix4x4.CreateLookAt(Position, GetForward(), new Vector3(0.0f, 1.0f, 0.0f));
    }

    public Matrix4x4 GetProjectionMatrix()
    {
        return Matrix4x4.CreatePerspectiveFieldOfView(0.785398f, AspectRatio, 0.1f, 1000.0f);
    }
}
