using Unity.Entities;
using UnityEngine;

public class CarCountUI : MonoBehaviour
{
    public TMPro.TextMeshProUGUI countText;
    private EntityManager entityManager;
    private EntityQuery carCountQuery;

    void Start()
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        carCountQuery = entityManager.CreateEntityQuery(typeof(CarCountData));
    }

    void Update()
    {
        if (carCountQuery.CalculateEntityCount() == 0)
            return;

        var carCount = carCountQuery.GetSingleton<CarCountData>();
        countText.text = $"Active Cars: {carCount.Total}";
    }
}
