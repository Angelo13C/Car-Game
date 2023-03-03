using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
public partial struct CityVehiclePatrollerSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        if(SystemAPI.TryGetSingletonEntity<StreetTile>(out var streetNetworkEntity))
        {
            var streetNetwork = SystemAPI.GetComponent<StreetNetwork>(streetNetworkEntity);
            var streets = SystemAPI.GetBuffer<StreetTile>(streetNetworkEntity);
            var rng = new Random((uint) (SystemAPI.Time.ElapsedTime * 10000));
            foreach(var (cityVehiclePatroller, vehicleAISteer, transform) in SystemAPI.Query<RefRW<CityVehiclePatroller>, RefRW<VehicleAISteer>, LocalTransform>())
            {
                var currentStreetTileGridPosition = streetNetwork.PositionToGridPosition(new float2(transform.Position.x, transform.Position.z));
                if(!streetNetwork.Grid.IsGridPositionValid(currentStreetTileGridPosition))
                    continue;

                var currentStreetTileIndex = streetNetwork.Grid.GridPositionToIndex(currentStreetTileGridPosition);
                if(currentStreetTileIndex != cityVehiclePatroller.ValueRO.LastStreetTileIndex)
                {
                    cityVehiclePatroller.ValueRW.LastStreetTileIndex = currentStreetTileIndex;
                    var angle = vehicleAISteer.ValueRO.TargetAngle;
                    if(angle == VehicleAISteer.NO_TARGET_ANGLE)
                        continue;

                    angle += Direction.Up.ToAngle();
                    if(angle < 0)
                        angle += 2 * math.PI;
                    var currentDirectionAngle = (int) math.round(angle / (math.PI / 2f));
                    var oppositeDirection = ((Direction) currentDirectionAngle).Opposite();
                    var oppositeDirectionMask = ~(1 << (int) oppositeDirection);
                    var adjacency = (int) streetNetwork.GetAdjacencyStreets(currentStreetTileGridPosition, streets) & oppositeDirectionMask;
                    var adjacencyCount = math.countbits(adjacency);

                    // Find the value of the least significant bit of adjacency
                    for(var randomAdjacencyPosition = rng.NextInt(adjacencyCount); randomAdjacencyPosition > 0; randomAdjacencyPosition--) 
                    {
                        adjacency &= adjacency - 1;
                    }
                    var chosenAdjacency = (Adjacency) (adjacency & ~(adjacency - 1));
                    
                    if(chosenAdjacency == Adjacency.Right)
                        vehicleAISteer.ValueRW.TargetAngle = 0f;
                    else if(chosenAdjacency == Adjacency.Up)
                        vehicleAISteer.ValueRW.TargetAngle = math.PI / 2f;
                    else if(chosenAdjacency == Adjacency.Left)
                        vehicleAISteer.ValueRW.TargetAngle = math.PI;
                    else if(chosenAdjacency == Adjacency.Down)
                        vehicleAISteer.ValueRW.TargetAngle = math.PI * 3f / 2f;
                    vehicleAISteer.ValueRW.TargetAngle -= Direction.Up.ToAngle();
                }
            }
        }
    }
}