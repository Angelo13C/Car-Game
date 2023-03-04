using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

[BurstCompile]
public partial struct FollowerSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var transformLookup = SystemAPI.GetComponentLookup<WorldTransform>(true);
        foreach(var (follower, vehicleAISteer, cruiseControl, transform) in SystemAPI.Query<Follower, RefRW<VehicleAISteer>, RefRW<VehicleCruiseControl>, WorldTransform>())
        {
            if(follower.Target != Entity.Null)
            {
                var targetTransform = transformLookup.GetRefRO(follower.Target);
                var targetPosition = targetTransform.ValueRO.Position;
                var direction = targetPosition - transform.Position;
                var angle = math.atan2(direction.z, direction.x);
                vehicleAISteer.ValueRW.TargetAngle = angle - Direction.Up.ToAngle();
            }
        }
    }
}