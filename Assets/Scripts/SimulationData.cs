using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct RoadData : IComponentData
{
    public bool IsBezier;

    public float3 P0;
    public float3 P1;
    public float3 P2;
    public float3 P3;
    public float3 DespawnPoint;

    public float3 Direction;
    public float SpeedLimit;
    public float Length;
    public Entity TrafficLight;
}
public struct RoadConnection : IBufferElementData
{
    public Entity NextRoad;
}
public struct VehicleData : IComponentData
{
    public float3 Position;
    public float Speed;
    public float BaseSpeed;
    public Entity CurrentRoad;
    public float SafeDistance;
    public float3 TargetPosition;
    public float RoadT;
}

public enum LightState
{
    Red,
    Green
}

public struct TrafficLightData : IComponentData
{
    public Entity RedLight;
    public Entity GreenLight;
    public LightState State;
    public float Timer;
    public float GreenDuration;
    public float RedDuration;
}

public struct CarSpawnerData : IComponentData
{
    public Entity RoadEntity;
    public float SpawnInterval;
    public float Timer;
}

public struct CarPoolData : IComponentData
{
    public int MaxCars;
    public int ActiveCount;
}

public struct SpawnedBy : IComponentData
{
    public Entity Spawner;
}

public struct CarCountData : IComponentData
{
    public int Total;
}

