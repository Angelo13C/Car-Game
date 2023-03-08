using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

[UpdateInGroup(typeof(WeaponTriggerSystemGroup))]
[BurstCompile]
public partial struct WeaponTriggerIfInLOSSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var transformLookup = SystemAPI.GetComponentLookup<WorldTransform>();
        var physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
        foreach(var (trigger, triggerIfInLOS, lookAtTarget, transform) in SystemAPI.Query<RefRW<WeaponTrigger>, WeaponTriggerIfInLOS, LookAtTarget, WorldTransform>())
        {
            if(lookAtTarget.Target != Entity.Null)
            {
                if(transformLookup.TryGetComponent(lookAtTarget.Target, out var targetTransform))
                {
                    var dir = targetTransform.Position - transform.Position;
                    var directionLengthSqr = math.lengthsq(dir);
                    if(directionLengthSqr <= triggerIfInLOS.MaxDistance * triggerIfInLOS.MaxDistance)
                    {
                        var normalizedDirection = dir / math.sqrt(directionLengthSqr);
                        var forwardDirection = math.mul(transform.Rotation, math.forward());
                        var dotDirectionToTarget = math.dot(normalizedDirection, forwardDirection);
                        var angle = math.acos(dotDirectionToTarget);
                        if(math.abs(angle) <= triggerIfInLOS.MaxAngle)
                        {
                            var raycastInput = new RaycastInput {
                                Start = transform.Position,
                                End = targetTransform.Position,
                                Filter = triggerIfInLOS.Filter
                            };
                            if(!physicsWorld.CastRay(raycastInput, out var _))
                            {
                                trigger.ValueRW.Trigger();
                            }
                        }
                    }
                }
            }
        }
    }
}