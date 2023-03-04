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
        _vehiclesQuery = state.GetEntityQuery(typeof(CitizenVehicleTag));
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
                var vehicleAISteer = SystemAPI.GetComponent<VehicleAISteer>(vehiclesSpawner.VehiclePrefab);
                var vehicleDefaultDirection = Direction.Up.ToAngle();
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
                    var cellOffset = new float2(streetNetwork.StreetTileSize.x + streetNetwork.LaneWidth, streetNetwork.StreetTileSize.y) / 2f;
                    var cellPosition = cellOffset + new float2(cellGridPosition.x * streetNetwork.StreetTileSize.x, cellGridPosition.y * streetNetwork.StreetTileSize.y);

                    var direction = Direction.Up;
                    if(streetNetwork.IsStreet(cellGridPosition + new int2(1, 0), streetsTiles))
                        direction = Direction.Right;
                    else if(streetNetwork.IsStreet(cellGridPosition + new int2(0, -1), streetsTiles))
                        direction = Direction.Down;
                    else if(streetNetwork.IsStreet(cellGridPosition + new int2(-1, 0), streetsTiles))
                        direction = Direction.Left;

                    var spawnedTransform = transform;
                    spawnedTransform.Position = new float3(cellPosition.x, 0, cellPosition.y);
                    var angle = direction.ToAngle() - vehicleDefaultDirection;
                    spawnedTransform = spawnedTransform.RotateY(angle);
                    entityCommandBuffer.SetComponent(spawnedVehicle, spawnedTransform);
                    entityCommandBuffer.SetComponent(spawnedVehicle, new VehicleAISteer { 
                        SteerAngleBounds = vehicleAISteer.SteerAngleBounds,
                        TargetAngle = spawnedTransform.Rotation.ComputeYAngle(),
                        CurrentAngle = angle
                    });
                }
            }
        }
    }
}