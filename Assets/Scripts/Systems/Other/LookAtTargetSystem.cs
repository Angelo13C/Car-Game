using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[BurstCompile]
public partial struct LookAtTargetSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var invDeltaTime = 1f / SystemAPI.Time.DeltaTime;
        var transformLookup = SystemAPI.GetComponentLookup<WorldTransform>();
        foreach(var (lookAtTarget, transform, velocity, mass) in SystemAPI.Query<LookAtTarget, TransformAspect, RefRW<PhysicsVelocity>, PhysicsMass>())
        {
            if(lookAtTarget.Target != Entity.Null)
            {
                if(transformLookup.TryGetComponent(lookAtTarget.Target, out var targetTransform))
                {
                    var speed = invDeltaTime * lookAtTarget.Speed;
                    var direction = targetTransform.Position - transform.WorldPosition;
                    var targetRotation = quaternion.LookRotationSafe(direction, math.up());
                    var targetRigidTransform = new RigidTransform(targetRotation, transform.WorldPosition);
                    var velocityToTarget = PhysicsVelocity.CalculateVelocityToTarget(mass, transform.WorldPosition, transform.WorldRotation, targetRigidTransform, speed);
                    velocity.ValueRW.Angular = velocityToTarget.Angular;
                }
            }
        }
    }
}