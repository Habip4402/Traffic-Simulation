using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

public struct ReturnedToPoolTag : IComponentData { }

[BurstCompile]
[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(VehicleMovementSystem))]
public partial struct PoolCountUpdateSystem : ISystem
{
    private EntityQuery _returnedQuery;
    private ComponentLookup<CarPoolData> _poolLookup;

    public void OnCreate(ref SystemState state)
    {
        _returnedQuery = new EntityQueryBuilder(Allocator.Temp)
            .WithAll<ReturnedToPoolTag, SpawnedBy>()
            .WithOptions(EntityQueryOptions.IncludeDisabledEntities)
            .Build(ref state);

        _poolLookup = state.GetComponentLookup<CarPoolData>(isReadOnly: false);
    }

    public void OnUpdate(ref SystemState state)
    {
        _poolLookup.Update(ref state);

        var ecb = new EntityCommandBuffer(Allocator.Temp);

        using var ents = _returnedQuery.ToEntityArray(Allocator.Temp);
        using var spawnedBy = _returnedQuery.ToComponentDataArray<SpawnedBy>(Allocator.Temp);

        for (int i = 0; i < ents.Length; i++)
        {
            var spawner = spawnedBy[i].Spawner;
            if (_poolLookup.HasComponent(spawner))
            {
                var pool = _poolLookup[spawner];
                pool.ActiveCount = math.max(0, pool.ActiveCount - 1);
                _poolLookup[spawner] = pool;
            }
            ecb.RemoveComponent<ReturnedToPoolTag>(ents[i]);
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}
