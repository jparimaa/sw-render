using System.Numerics;

class Camera
{
    public Vector3 position;
    public Vector3 rotation;
    public float AspectRatio { get; set; }

    private readonly Vector4 c_forward;
    private readonly Vector4 c_right;

    public Camera()
    {
        position = new Vector3(0, 0, 3.0f);
        rotation = new Vector3(0, 0, 0);
        AspectRatio = 800 / 600;
        c_forward = new Vector4(0, 0, -1, 0);
        c_right = new Vector4(1, 0, 0, 0);
    }

    public Vector3 GetForward()
    {
        var forward4 = Vector4.Transform(c_forward, GetYawPitchRoll());
        return new Vector3(forward4.X, forward4.Y, forward4.Z);
    }

    public Vector3 GetRight()
    {
        var right4 = Vector4.Transform(c_right, GetYawPitchRoll());
        return new Vector3(right4.X, right4.Y, right4.Z);
    }

    public Matrix4x4 GetViewMatrix()
    {
        return Matrix4x4.CreateLookAt(position, position + GetForward(), new Vector3(0.0f, 1.0f, 0.0f));
    }

    public Matrix4x4 GetProjectionMatrix()
    {
        return Matrix4x4.CreatePerspectiveFieldOfView(0.785398f, AspectRatio, 0.1f, 1000.0f);
    }

    private Matrix4x4 GetYawPitchRoll()
    {
        return Matrix4x4.CreateFromYawPitchRoll(rotation.Y, rotation.X, rotation.Z);
    }
}
