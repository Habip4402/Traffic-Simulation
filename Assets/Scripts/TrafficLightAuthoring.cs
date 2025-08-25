using Unity.Entities;
using UnityEngine;

public class TrafficLightAuthoring : MonoBehaviour
{
    public GameObject RedLight;
    public GameObject GreenLight;
    public float GreenDuration = 5f;
    public float RedDuration = 10f;
    public LightState InitialState = LightState.Red;
    public int OrderIndex = 0;

    class Baker : Baker<TrafficLightAuthoring>
    {
        public override void Bake(TrafficLightAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);
            float startTimer = 0f;
            if (authoring.InitialState == LightState.Red)
            {
                startTimer = authoring.OrderIndex * authoring.GreenDuration;
            }

            AddComponent(entity, new TrafficLightData
            {
                State = authoring.InitialState,
                Timer = startTimer,
                GreenDuration = authoring.GreenDuration,
                RedDuration = authoring.RedDuration,
                RedLight = authoring.RedLight
                        ? GetEntity(authoring.RedLight, TransformUsageFlags.None)
                        : Entity.Null,
                GreenLight = authoring.GreenLight
                        ? GetEntity(authoring.GreenLight, TransformUsageFlags.None)
                        : Entity.Null
            });
        }
    }
}
