using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;

[BurstCompile]
public partial struct TrafficLightSystem : ISystem
{
    public static Dictionary<Entity, (GameObject red, GameObject green)> Lights = new();

    public static void Register(Entity entity, GameObject red, GameObject green)
    {
        Lights[entity] = (red, green);
    }

    public void OnUpdate(ref SystemState state)
    {
        float dt = SystemAPI.Time.DeltaTime;

        foreach (var (tl, entity) in SystemAPI.Query<RefRW<TrafficLightData>>().WithEntityAccess())
        {
            tl.ValueRW.Timer += dt;

            switch (tl.ValueRO.State)
            {
                case LightState.Green:
                    if (tl.ValueRW.Timer >= tl.ValueRO.GreenDuration)
                    {
                        tl.ValueRW.State = LightState.Red;
                        tl.ValueRW.Timer = 0f;
                    }
                    break;
                case LightState.Red:
                    if (tl.ValueRW.Timer >= tl.ValueRO.RedDuration)
                    {
                        tl.ValueRW.State = LightState.Green;
                        tl.ValueRW.Timer = 0f;
                    }
                    break;
            }

            if (Lights.TryGetValue(entity, out var lightPair))
            {
                if (lightPair.red != null) lightPair.red.SetActive(tl.ValueRW.State == LightState.Red);
                if (lightPair.green != null) lightPair.green.SetActive(tl.ValueRW.State == LightState.Green);
            }
        }
    }
}
