using Unity.Burst;
using Unity.Core;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
[UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst = true)]
[UpdateBefore(typeof(BeginSimulationEntityCommandBufferSystem))]
public partial struct CityVehiclesSpawnerSystem : ISystem
{
    private VehicleSpawner _vehicleSpawner;

    public void OnCreate(ref SystemState state)
    {
        _vehicleSpawner = new VehicleSpawner(state.GetEntityQuery(typeof(CitizenVehicleTag)));
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float2? playerPosition = null;
        foreach(var playerTransform in SystemAPI.Query<WorldTransform>().WithAll<PlayerVehicleTag>())
            playerPosition = new float2(playerTransform.Position.x, playerTransform.Position.z);

        foreach(var (vehiclesSpawner, streetNetwork, streetsTiles) in SystemAPI.Query<CityVehiclesSpawner, StreetNetwork, DynamicBuffer<StreetTile>>())
        {
            var playerGridPosition = playerPosition.HasValue ? streetNetwork.PositionToGridPosition(playerPosition.Value) : streetNetwork.Grid.Size / 2;
            _vehicleSpawner.StartSpawning(SystemAPI.Time, vehiclesSpawner.VehiclesCount, vehiclesSpawner.VehiclePrefab, out var frame);
            if(frame.VehiclesToSpawnCount > 0)
            {
                var entityCommandBufferSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
                var entityCommandBuffer = entityCommandBufferSingleton.CreateCommandBuffer(state.WorldUnmanaged);
                var prefabTransform = SystemAPI.GetComponent<LocalTransform>(vehiclesSpawner.VehiclePrefab);
                var vehicleAISteer = SystemAPI.GetComponent<VehicleAISteer>(vehiclesSpawner.VehiclePrefab);
                var vehicleDefaultDirection = Direction.Up.ToAngle();
                for(var i = 0; i < frame.VehiclesToSpawnCount; i++)
                {
                    var cellPosition = frame.GetRandomRoadSpawnPosition(streetNetwork, streetsTiles, playerGridPosition, vehiclesSpawner.SpawnZoneSize, out var cellGridPosition);

                    var direction = Direction.Up;
                    if(streetNetwork.IsStreet(cellGridPosition + new int2(1, 0), streetsTiles))
                        direction = Direction.Right;
                    else if(streetNetwork.IsStreet(cellGridPosition + new int2(0, -1), streetsTiles))
                        direction = Direction.Down;
                    else if(streetNetwork.IsStreet(cellGridPosition + new int2(-1, 0), streetsTiles))
                        direction = Direction.Left;

                    var angle = direction.ToAngle() - vehicleDefaultDirection;
                    var spawnedVehicle = frame.Spawn(entityCommandBuffer, prefabTransform, new float3(cellPosition.x, 0, cellPosition.y), angle);
                    entityCommandBuffer.SetComponent(spawnedVehicle, new VehicleAISteer { 
                        SteerAngleBounds = vehicleAISteer.SteerAngleBounds,
                        TargetAngle = angle,
                        CurrentAngle = angle
                    });
                }
            }
        }
    }
}

[BurstCompile]
public struct VehicleSpawner
{
    private EntityQuery _vehiclesQuery;

    public VehicleSpawner(EntityQuery vehiclesQuery)
    {
        _vehiclesQuery = vehiclesQuery;
    }

    public struct Frame
    {
        public int VehiclesToSpawnCount;
        public Random Rng;
        private Entity _prefab;

        public Frame(int vehiclesToSpawnCount, Random rng, Entity prefab)
        {
            VehiclesToSpawnCount = vehiclesToSpawnCount;
            Rng = rng;
            _prefab = prefab;
        }

        [BurstCompile]
        public Entity Spawn(EntityCommandBuffer entityCommandBuffer, LocalTransform prefabTransform, float3 position, float angle)
        {
            var spawnedVehicle = entityCommandBuffer.Instantiate(_prefab);
            var newTransform = new LocalTransform {
                Position = position,
                Rotation = prefabTransform.RotateY(angle).Rotation,
                Scale = prefabTransform.Scale
            };

            entityCommandBuffer.SetComponent(spawnedVehicle, newTransform);

            return spawnedVehicle;
        }

        [BurstCompile]
        public float2 GetRandomRoadSpawnPosition(StreetNetwork streetNetwork, DynamicBuffer<StreetTile> streetsTiles, int2 center, int2 size, out int2 cellGridPosition)
        {
            var start = math.clamp(center - size, int2.zero, streetNetwork.Grid.Size);
            var end = math.clamp(center + size, int2.zero, streetNetwork.Grid.Size);
            do
            {
                cellGridPosition = Rng.NextInt2(start, end);
            } while(!streetsTiles[streetNetwork.Grid.GridPositionToIndex(cellGridPosition)].IsStreet);

            var cellOffset = new float2(streetNetwork.StreetTileSize.x + streetNetwork.LaneWidth, streetNetwork.StreetTileSize.y) / 2f;
            return cellOffset + new float2(cellGridPosition.x * streetNetwork.StreetTileSize.x, cellGridPosition.y * streetNetwork.StreetTileSize.y);
        }
    }

    [BurstCompile]
    public void StartSpawning(TimeData time, int minVehiclesCount, Entity prefab, out Frame result)
    {
        var vehiclesCount = _vehiclesQuery.CalculateEntityCount();
        var vehiclesToSpawn = minVehiclesCount - vehiclesCount;
        if(vehiclesToSpawn > 0)
        {
            var rng = new Random((uint) (time.ElapsedTime * 10000));

            result = new Frame(vehiclesToSpawn, rng, prefab);
        }
        else
        {
            result = new Frame {
                VehiclesToSpawnCount = 0
            };
        }
    }
}