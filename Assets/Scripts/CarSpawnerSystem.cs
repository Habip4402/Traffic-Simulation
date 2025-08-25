using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using System.Diagnostics;

using UnityEngine;
public partial struct CarSpawnerSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        float dt = SystemAPI.Time.DeltaTime;
        var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

        foreach (var (spawner, pool, prefabRef, entity) in
                 SystemAPI.Query<RefRW<CarSpawnerData>, RefRW<CarPoolData>, CarSpawnerPrefabRef>()
                 .WithEntityAccess())
        {
            spawner.ValueRW.Timer += dt;

            if (spawner.ValueRW.Timer >= spawner.ValueRO.SpawnInterval && pool.ValueRW.ActiveCount < pool.ValueRW.MaxCars)
            {
                spawner.ValueRW.Timer = 0f;
                Entity car = ecb.Instantiate(prefabRef.Prefab);

                if (state.EntityManager.HasComponent<RoadData>(spawner.ValueRO.RoadEntity))
                {
                    var road = state.EntityManager.GetComponentData<RoadData>(spawner.ValueRO.RoadEntity);
                    var prefabVehicleData = state.EntityManager.GetComponentData<VehicleData>(prefabRef.Prefab);

                    ecb.SetComponent(car, new LocalTransform
                    {
                        Position = road.P0,
                        Rotation = quaternion.identity,
                        Scale = 1f
                    });

                    ecb.SetComponent(car, new VehicleData
                    {
                        CurrentRoad = spawner.ValueRO.RoadEntity,
                        BaseSpeed = prefabVehicleData.BaseSpeed,
                        Speed = prefabVehicleData.BaseSpeed,
                        SafeDistance = prefabVehicleData.SafeDistance,
                        RoadT = 0f,
                        Position = road.P0,
                        TargetPosition = road.P3
                    });
                    ecb.AddComponent(car, new SpawnedBy { Spawner = entity });
                }
                pool.ValueRW.ActiveCount++;
            }
        }
        ecb.Playback(state.EntityManager);
    }
}
