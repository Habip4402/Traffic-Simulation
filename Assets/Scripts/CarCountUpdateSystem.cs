using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

[BurstCompile]
[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct CarCountUpdateSystem : ISystem
{
    private EntityQuery _poolQuery;
    private Entity _carCountEntity;

    public void OnCreate(ref SystemState state)
    {
        _poolQuery = state.GetEntityQuery(typeof(CarPoolData));

        _carCountEntity = state.EntityManager.CreateEntity(typeof(CarCountData));
    }

    public void OnUpdate(ref SystemState state)
    {
        int total = 0;

        using var pools = _poolQuery.ToComponentDataArray<CarPoolData>(Allocator.Temp);
        for (int i = 0; i < pools.Length; i++)
            total += pools[i].ActiveCount;

        state.EntityManager.SetComponentData(_carCountEntity, new CarCountData { Total = total });
    }
}
