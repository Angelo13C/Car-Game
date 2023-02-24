using Unity.Burst;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;
using Unity.Physics.Extensions;
using Unity.Physics.Systems;

[UpdateInGroup(typeof(BeforePhysicsSystemGroup))]
[BurstCompile]
public partial struct SuspensionsSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
        var deltaTime = SystemAPI.Time.DeltaTime;
        
        foreach(var (suspensionsConfig, suspensions, velocity, mass, transform) in SystemAPI.Query<RefRW<Suspensions>, DynamicBuffer<Suspension>, RefRW<PhysicsVelocity>, RefRO<PhysicsMass>, TransformAspect>())
        {            
            var force = transform.Up * suspensionsConfig.ValueRO.Force;

            suspensionsConfig.ValueRW.SuspensionsGroundedCount = 0;

            for(var i = 0; i < suspensions.Length; i++)
            {
                var start = transform.TransformPointLocalToWorld(suspensions[i].Position);
                var ray = new RaycastInput {
                    Start = start,
                    End = start + transform.Down * suspensionsConfig.ValueRO.Height,
                    Filter = suspensionsConfig.ValueRO.CollisionFilter
                };
                
                var currentHeight = Suspensions.FLOATING_SUSPENSION_HEIGHT;
                if(physicsWorld.CastRay(ray, out var hit))
                {
                    suspensionsConfig.ValueRW.SuspensionsGroundedCount++;

                    currentHeight = hit.Fraction * suspensionsConfig.ValueRO.Height;
                    
                    var compressionRatio = 1 - hit.Fraction;
                    var hoverForce = force * compressionRatio;
                    
                    mass.ValueRO.GetImpulseFromForce(transform.WorldScale, in hoverForce, ForceMode.Force, deltaTime, out var impulse, out var impulseMass);
                    velocity.ValueRW.ApplyImpulse(in impulseMass, transform.WorldPosition, transform.WorldRotation, transform.WorldScale, in impulse, start);
                }
                
                suspensions.ElementAt(i).CurrentHeight = currentHeight;
            }
        }
    }
}