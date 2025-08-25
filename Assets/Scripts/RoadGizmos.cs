using UnityEngine;

[ExecuteAlways]
public class RoadGizmos : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        var road = GetComponent<RoadAuthoring>();
        if (!road) return;

        var start = road.StartPoint ? road.StartPoint.position : transform.position;
        var end = road.EndPoint ? road.EndPoint.position : (start + transform.forward * road.Length);

        Gizmos.color = Color.yellow;

        if (road.UseBezier && road.ControlPoint1 && road.ControlPoint2)
        {
            // Bezier eðrisi
            Vector3 p0 = start;
            Vector3 p1 = road.ControlPoint1.position;
            Vector3 p2 = road.ControlPoint2.position;
            Vector3 p3 = end;

            Vector3 prev = p0;
            int segments = 20;
            for (int i = 1; i <= segments; i++)
            {
                float t = i / (float)segments;
                Vector3 point = CalculateBezierPoint(t, p0, p1, p2, p3);
                Gizmos.DrawLine(prev, point);
                prev = point;
            }

            // Kontrol noktalarý
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(p1, 0.2f);
            Gizmos.DrawSphere(p2, 0.2f);
            Gizmos.DrawLine(p0, p1);
            Gizmos.DrawLine(p2, p3);

            // Yol uçlarý
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(p0, 0.25f);
            Gizmos.DrawSphere(p3, 0.25f);
        }
        else
        {
            // Düz yol
            Gizmos.DrawLine(start, end);

            Gizmos.color = Color.green;
            Gizmos.DrawSphere(start, 0.25f);
            Gizmos.DrawSphere(end, 0.25f);
        }
    }

    private Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        float uuu = uu * u;
        float ttt = tt * t;

        Vector3 point = uuu * p0;
        point += 3 * uu * t * p1;
        point += 3 * u * tt * p2;
        point += ttt * p3;

        return point;
    }
}
