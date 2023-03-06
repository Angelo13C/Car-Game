using Unity.Entities;
using Unity.Mathematics;

public struct PoliceVehiclesSpawner : IComponentData
{
    public Entity CarPrefab;
    public int2 SpawnZoneSize;
}