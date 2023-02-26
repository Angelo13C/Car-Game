using Unity.Entities;

public struct CityVehiclesSpawner : IComponentData
{
    public int VehiclesCount;
    public Entity VehiclePrefab;
}