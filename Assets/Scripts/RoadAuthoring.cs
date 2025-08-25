using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class RoadAuthoring : MonoBehaviour
{
    [Header("General Settings")]
    [Min(0.1f)] public float Length = 10f;
    public float SpeedLimit = 10f;
    public GameObject TrafficLight;
    public GameObject[] NextRoads;

    [Header("Bezier Curve Settings")]
    public bool UseBezier = false;
    public Transform ControlPoint1;
    public Transform ControlPoint2;
    public Transform EndPoint;
    public Transform StartPoint;
    public Transform DespawnPoint;

    class Baker : Baker<RoadAuthoring>
    {
        public override void Bake(RoadAuthoring a)
        {
            float3 start;
            float3 end;

            if (a.UseBezier && a.EndPoint != null)
            {
                start = (float3)a.StartPoint.position;
                end = (float3)a.EndPoint.position;
            }
            else
            {
                start = (float3)a.transform.position - (float3)a.transform.forward * a.Length;
                end = start + (float3)a.transform.forward * a.Length*2f;
            }
            var dir = math.normalize(end - start);

            var e = GetEntity(TransformUsageFlags.None);

            if (a.UseBezier && a.ControlPoint1 && a.ControlPoint2)
            {
                AddComponent(e, new RoadData
                {
                    IsBezier = true,
                    P0 = start,
                    P1 = (float3)a.ControlPoint1.position,
                    P2 = (float3)a.ControlPoint2.position,
                    P3 = end,
                    DespawnPoint = a.DespawnPoint != null ? (float3)a.DespawnPoint.position : float3.zero,
                    SpeedLimit = a.SpeedLimit,
                    Length = a.Length,
                    TrafficLight = a.TrafficLight
                        ? GetEntity(a.TrafficLight, TransformUsageFlags.None)
                        : Entity.Null
                });
            }
            else
            {
                AddComponent(e, new RoadData
                {
                    IsBezier = false,
                    P0 = start,
                    P3 = end,
                    DespawnPoint = a.DespawnPoint != null ? (float3)a.DespawnPoint.position : float3.zero,
                    Direction = dir,
                    SpeedLimit = a.SpeedLimit,
                    TrafficLight = a.TrafficLight
                        ? GetEntity(a.TrafficLight, TransformUsageFlags.None)
                        : Entity.Null
                });
            }

            var buf = AddBuffer<RoadConnection>(e);
            if (a.NextRoads != null)
            {
                foreach (var go in a.NextRoads)
                {
                    if (!go) continue;
                    buf.Add(new RoadConnection
                    {
                        NextRoad = GetEntity(go, TransformUsageFlags.None)
                    });
                }
            }
        }
    }
}