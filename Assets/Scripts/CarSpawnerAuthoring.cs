using Unity.Entities;
using UnityEngine;

public class CarSpawnerAuthoring : MonoBehaviour
{
    public GameObject CarPrefab;
    public RoadAuthoring Road;
    public int MaxCars = 10;
    public float SpawnInterval = 3f;

    class Baker : Baker<CarSpawnerAuthoring>
        {
        public override void Bake(CarSpawnerAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new CarSpawnerData
            {
                RoadEntity = authoring.Road ? GetEntity(authoring.Road, TransformUsageFlags.None) : Entity.Null,
                SpawnInterval = authoring.SpawnInterval,
                Timer = 0f
            });
            AddComponent(entity, new CarPoolData
            {
                MaxCars = authoring.MaxCars,
                ActiveCount = 0
            });
            AddComponentObject(entity, new CarSpawnerPrefabRef
            {
                Prefab = GetEntity(authoring.CarPrefab, TransformUsageFlags.Dynamic)
            });
        }
    }
}
public class CarSpawnerPrefabRef : IComponentData
{
    public Entity Prefab;
}
