using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
[UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst = true)]
[UpdateBefore(typeof(BeginSimulationEntityCommandBufferSystem))]
public partial struct CityVehiclesSpawnerSystem : ISystem
{
    private EntityQuery _vehiclesQuery;

    public void OnCreate(ref SystemState state)
    {
        _vehiclesQuery = state.GetEntityQuery(typeof(VehicleTag));
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach(var (vehiclesSpawner, streetNetwork, streetsTiles) in SystemAPI.Query<CityVehiclesSpawner, StreetNetwork, DynamicBuffer<StreetTile>>())
        {
            var vehiclesCount = _vehiclesQuery.CalculateEntityCount();
            var vehiclesToSpawn = vehiclesSpawner.VehiclesCount - vehiclesCount;
            if(vehiclesToSpawn > 0)
            {
                var entityCommandBufferSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
                var entityCommandBuffer = entityCommandBufferSingleton.CreateCommandBuffer(state.WorldUnmanaged);

                var transform = SystemAPI.GetComponent<LocalTransform>(vehiclesSpawner.VehiclePrefab);
                var rng = new Random((uint) (SystemAPI.Time.ElapsedTime * 10000));
                for(var i = 0; i < vehiclesToSpawn; i++)
                {
                    var spawnedVehicle = entityCommandBuffer.Instantiate(vehiclesSpawner.VehiclePrefab);
                    
                    var cellIndex = -1;
                    do
                    {
                        cellIndex = rng.NextInt(streetsTiles.Length);
                    } while(!streetsTiles[cellIndex].IsStreet);

                    var cellGridPosition = streetNetwork.Grid.IndexToGridPosition(cellIndex);
                    var cellPosition = new float2(cellGridPosition.x * streetNetwork.StreetTileSize.x, cellGridPosition.y * streetNetwork.StreetTileSize.y);
                    transform.Position = new float3(cellPosition.x, 0, cellPosition.y);
                    entityCommandBuffer.SetComponent(spawnedVehicle, transform);
                }
            }
        }
    }
}