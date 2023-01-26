using UnityEngine;

public static class Utils
{
    public static float Vector3PlaneDistance(Vector3 v1, Vector3 v2, Vector3 planeNormal)
    {
        return Vector3.Distance(Vector3.ProjectOnPlane(v1, planeNormal), Vector3.ProjectOnPlane(v2, planeNormal));
    }
}
