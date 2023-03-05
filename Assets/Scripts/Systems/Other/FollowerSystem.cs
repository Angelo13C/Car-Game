using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Extensions;

[BurstCompile]
public partial struct FollowerSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var transformLookup = SystemAPI.GetComponentLookup<WorldTransform>(true);
        var velocityLookup = SystemAPI.GetComponentLookup<PhysicsVelocity>(true);
        var massLookup = SystemAPI.GetComponentLookup<PhysicsMass>(true);
        foreach(var (follower, vehicleAISteer, cruiseControl, transform) in SystemAPI.Query<Follower, RefRW<VehicleAISteer>, RefRW<VehicleCruiseControl>, WorldTransform>())
        {
            if(follower.Target != Entity.Null)
            {
                var targetPosition = transformLookup.GetRefRO(follower.Target).ValueRO.Position;
                if(velocityLookup.TryGetComponent(follower.Target, out var velocity) && massLookup.TryGetComponent(follower.Target, out var mass))
                {                    
                    var _ = quaternion.identity;
                    velocity.Integrate(mass, follower.PredictedTime, ref targetPosition, ref _);
                }

                var direction = targetPosition - transform.Position;
                var angle = math.atan2(direction.z, direction.x);
                vehicleAISteer.ValueRW.TargetAngle = angle - Direction.Up.ToAngle();
            }
        }
    }
}