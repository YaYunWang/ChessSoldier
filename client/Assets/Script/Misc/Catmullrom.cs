using UnityEngine;
using System.Collections.Generic;

public static class Catmullrom
{
    public static void CalcCatmulRom(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, int amountOfPoints, ref List<Vector3> points, float alpha = 0.5f)
    {
        points.Clear();

        float t0 = 0.0f;
        float t1 = GetT(t0, p0, p1, alpha);
        float t2 = GetT(t1, p1, p2, alpha);
        float t3 = GetT(t2, p2, p3, alpha);

        for (float t = t1; t < t2; t += ((t2 - t1) / amountOfPoints))
        {
            Vector3 A1 = (t1 - t) / (t1 - t0) * p0 + (t - t0) / (t1 - t0) * p1;
            Vector3 A2 = (t2 - t) / (t2 - t1) * p1 + (t - t1) / (t2 - t1) * p2;
            Vector3 A3 = (t3 - t) / (t3 - t2) * p2 + (t - t2) / (t3 - t2) * p3;

            Vector3 B1 = (t2 - t) / (t2 - t0) * A1 + (t - t0) / (t2 - t0) * A2;
            Vector3 B2 = (t3 - t) / (t3 - t1) * A2 + (t - t1) / (t3 - t1) * A3;

            Vector3 C = (t2 - t) / (t2 - t1) * B1 + (t - t1) / (t2 - t1) * B2;

            points.Add(C);
        }
    }

    private static float GetT(float t, Vector3 p0, Vector3 p1, float alpha)
    {
        float a = Mathf.Pow((p1.x - p0.x), 2.0f) + Mathf.Pow((p1.y - p0.y), 2.0f) + Mathf.Pow((p1.z - p0.z), 2.0f);
        float b = Mathf.Pow(a, 0.5f);
        float c = Mathf.Pow(b, alpha);

        return (c + t);
    }
}
