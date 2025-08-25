using System.Diagnostics;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
[BurstCompile]
[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct VehicleMovementSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        float dt = SystemAPI.Time.DeltaTime;
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (vehicle, transform, ent) in
                 SystemAPI.Query<RefRW<VehicleData>, RefRW<LocalTransform>>().WithEntityAccess())
        {
            if (vehicle.ValueRO.CurrentRoad == Entity.Null) continue;

            var road = SystemAPI.GetComponent<RoadData>(vehicle.ValueRO.CurrentRoad);
            bool isBezier = road.IsBezier;

            if (isBezier && road.Length <= 0f)
            {
                float length = ApproximateBezierLength(road.P0, road.P1, road.P2, road.P3, 30);
                road.Length = length;

                SystemAPI.SetComponent(vehicle.ValueRO.CurrentRoad, road);
            }

            float t = vehicle.ValueRO.RoadT;

            float targetSpeed = math.min(vehicle.ValueRO.BaseSpeed, road.SpeedLimit);

            // Öndeki araç kontrolü
            float closestDist = float.MaxValue;
            float leaderSpeed = float.PositiveInfinity;
            Entity closestEntity = Entity.Null;

            foreach (var (other, otherEnt) in SystemAPI.Query<RefRO<VehicleData>>().WithEntityAccess())
            {
                if (otherEnt == ent) continue;

                float3 toOther = other.ValueRO.Position - vehicle.ValueRO.Position;
                float forwardDot = math.dot(math.normalize(toOther), math.normalize(road.Direction));
                if (forwardDot <= 0.5f) continue;

                float distToOther = math.length(toOther);
                if (distToOther < closestDist)
                {
                    closestDist = distToOther;
                    leaderSpeed = other.ValueRO.Speed;
                    closestEntity = otherEnt;
                }
            }

            if (closestEntity != Entity.Null && closestDist < vehicle.ValueRO.SafeDistance)
            {
                targetSpeed = math.min(targetSpeed, leaderSpeed);
            }

            // Trafik ýþýðý
            if (road.TrafficLight != Entity.Null)
            {
                var light = SystemAPI.GetComponent<TrafficLightData>(road.TrafficLight);
                if (light.State == LightState.Red)
                    targetSpeed = 0;
            }

            // Hýz güncelle
            float newSpeed = math.lerp(vehicle.ValueRO.Speed, targetSpeed, dt * 10f);
            vehicle.ValueRW.Speed = newSpeed;

            float moveAmount = newSpeed * dt;
            if (isBezier)
            {
                vehicle.ValueRW.RoadT += moveAmount / road.Length;
                vehicle.ValueRW.RoadT = math.min(vehicle.ValueRW.RoadT, 1f);
                vehicle.ValueRW.Position = CalculateBezierPoint(vehicle.ValueRW.RoadT, road.P0, road.P1, road.P2, road.P3);
            }
            else
            {
                vehicle.ValueRW.RoadT += moveAmount / math.distance(road.P0, road.P3);
                vehicle.ValueRW.RoadT = math.min(vehicle.ValueRW.RoadT, 1f);
                vehicle.ValueRW.Position = math.lerp(road.P0, road.P3, vehicle.ValueRW.RoadT);
            }

            transform.ValueRW.Position = vehicle.ValueRW.Position;

            // Yol sonuna gelme kontrolü
            if (vehicle.ValueRW.RoadT >= 1f)
            {
                if (SystemAPI.HasBuffer<RoadConnection>(vehicle.ValueRO.CurrentRoad))
                {
                    var conns = SystemAPI.GetBuffer<RoadConnection>(vehicle.ValueRO.CurrentRoad);
                    if (conns.Length > 0)
                    {
                        int randIndex = UnityEngine.Random.Range(0, conns.Length);
                        var nextRoad = conns[randIndex].NextRoad;
                        vehicle.ValueRW.CurrentRoad = nextRoad;
                        vehicle.ValueRW.RoadT = 0f;

                        var next = SystemAPI.GetComponent<RoadData>(nextRoad);
                        vehicle.ValueRW.Position = next.P0;
                        transform.ValueRW.Position = vehicle.ValueRW.Position;
                    }
                }
            }

            // Despawn noktasý
            if (!road.DespawnPoint.Equals(float3.zero))
            {
                float dist = math.distance(transform.ValueRO.Position, road.DespawnPoint);
                if (dist < 0.5f)
                {
                    if (!SystemAPI.HasComponent<ReturnedToPoolTag>(ent))
                        ecb.AddComponent(ent, new ReturnedToPoolTag());

                    if (!SystemAPI.HasComponent<Disabled>(ent))
                        ecb.AddComponent<Disabled>(ent);
                }
            }
        }
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }

    private static float3 CalculateBezierPoint(float t, float3 p0, float3 p1, float3 p2, float3 p3)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        float uuu = uu * u;
        float ttt = tt * t;

        float3 point = uuu * p0;
        point += 3 * uu * t * p1;
        point += 3 * u * tt * p2;
        point += ttt * p3;
        return point;
    }

    private static float ApproximateBezierLength(float3 p0, float3 p1, float3 p2, float3 p3, int segments = 20)
    {
        float length = 0f;
        float3 prev = p0;
        for (int i = 1; i <= segments; i++)
        {
            float t = i / (float)segments;
            float3 point = CalculateBezierPoint(t, p0, p1, p2, p3);
            length += math.distance(prev, point);
            prev = point;
        }
        return length;
    }
}
