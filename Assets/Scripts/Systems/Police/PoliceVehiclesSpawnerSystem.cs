using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
[UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst = true)]
[UpdateBefore(typeof(BeginSimulationEntityCommandBufferSystem))]
public partial struct PoliceVehiclesSpawnerSystem : ISystem
{
    private VehicleSpawner _carSpawner;
    
    public void OnCreate(ref SystemState state)
    {
        _carSpawner = new VehicleSpawner(state.GetEntityQuery(typeof(PoliceVehicleTag)));
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var vehicleDefaultDirection = Direction.Up.ToAngle();
        foreach(var (policeVehiclesSpawner, streetNetwork, streetsTiles) in SystemAPI.Query<PoliceVehiclesSpawner, StreetNetwork, DynamicBuffer<StreetTile>>())
        {
            var prefabTransform = SystemAPI.GetComponent<LocalTransform>(policeVehiclesSpawner.CarPrefab);
            var prefabAISteer = SystemAPI.GetComponent<VehicleAISteer>(policeVehiclesSpawner.CarPrefab);
            foreach(var (wanted, wantedLevels, wantedTransform, wantedEntity) in SystemAPI.Query<Wanted, DynamicBuffer<WantedLevel>, WorldTransform>().WithEntityAccess())
            {
                var wantedGridPosition = streetNetwork.PositionToGridPosition(new float2(wantedTransform.Position.x, wantedTransform.Position.z));
                EntityCommandBuffer? entityCommandBuffer = null;
                
                wanted.GetCurrentWantedLevel(wantedLevels, out var wantedLevel);
                _carSpawner.StartSpawning(SystemAPI.Time, wantedLevel.MinPoliceCars, policeVehiclesSpawner.CarPrefab, out var carFrame);
                if(carFrame.VehiclesToSpawnCount > 0)
                {
                    var entityCommandBufferSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
                    entityCommandBuffer = entityCommandBufferSingleton.CreateCommandBuffer(state.WorldUnmanaged);
                    
                    for(var i = 0; i < carFrame.VehiclesToSpawnCount; i++)
                    {
                        var cellPosition = carFrame.GetRandomRoadSpawnPosition(streetNetwork, streetsTiles, wantedGridPosition, policeVehiclesSpawner.SpawnZoneSize, out var cellGridPosition);

                        var direction = DirectionExtensions.FromAngle(math.atan2(cellPosition.y - wantedTransform.Position.y, cellPosition.x - wantedTransform.Position.x));

                        var angle = direction.ToAngle() - vehicleDefaultDirection;
                        var spawnedVehicle = carFrame.Spawn(entityCommandBuffer.Value, prefabTransform, new float3(cellPosition.x, 0, cellPosition.y), angle);
                        entityCommandBuffer.Value.SetComponent(spawnedVehicle, new VehicleAISteer { 
                            SteerAngleBounds = prefabAISteer.SteerAngleBounds,
                            TargetAngle = angle,
                            CurrentAngle = angle
                        });
                        entityCommandBuffer.Value.SetComponent(spawnedVehicle, new Follower {
                            Target = wantedEntity,
                            PredictedTime = carFrame.Rng.NextFloat(0.1f, 0.5f)
                        });
                    }
                }
            }
        }
    }
}