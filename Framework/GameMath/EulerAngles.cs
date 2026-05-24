using System;

namespace Framework.GameMath;

public struct EulerAngles
{
    // All values as radians
    public double Roll;     // x
    public double Pitch;    // y
    public double Yaw;      // z

    public EulerAngles(double roll, double pitch, double yaw)
    {
        Roll = roll;
        Pitch = pitch;
        Yaw = yaw;
    }
    
    public Quaternion AsQuaternion()
    {
        var cy = Math.Cos(Yaw * 0.5);
        var sy = Math.Sin(Yaw * 0.5);
        var cp = Math.Cos(Pitch * 0.5);
        var sp = Math.Sin(Pitch * 0.5);
        var cr = Math.Cos(Roll * 0.5);
        var sr = Math.Sin(Roll * 0.5);

        var q = new Quaternion();
        q.W = (float)(cr * cp * cy + sr * sp * sy);
        q.X = (float)(sr * cp * cy - cr * sp * sy);
        q.Y = (float)(cr * sp * cy + sr * cp * sy);
        q.Z = (float)(cr * cp * sy - sr * sp * cy);
        return q;
    }
}
