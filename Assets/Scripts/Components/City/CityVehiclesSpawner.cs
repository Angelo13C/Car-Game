using Unity.Entities;
using Unity.Mathematics;

public struct CityVehiclesSpawner : IComponentData
{
    public int VehiclesCount;
    public Entity VehiclePrefab;
    public int2 SpawnZoneSize;
}