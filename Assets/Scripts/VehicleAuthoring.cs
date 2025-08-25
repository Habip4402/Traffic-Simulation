using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;

public class VehicleAuthoring : MonoBehaviour
{
    public float Speed = 5f;
    public float SafeDistance = 2f;
    public GameObject RoadReference;

    class Baker : Baker<VehicleAuthoring>
    {
        public override void Bake(VehicleAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            Entity roadEntity = GetEntity(authoring.RoadReference, TransformUsageFlags.None);

            AddComponent(entity, new VehicleData
            {
                Position = authoring.transform.position,
                Speed = authoring.Speed,
                BaseSpeed = authoring.Speed,
                SafeDistance = authoring.SafeDistance,
                CurrentRoad = roadEntity,

                RoadT = 0f,

                TargetPosition = authoring.transform.position
            });
        }
    }
}
