using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Extensions;
using Unity.Transforms;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[BurstCompile]
public partial struct VehicleSteerSystem : ISystem
{    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var deltaTime = SystemAPI.Time.DeltaTime;
        foreach(var (vehicleSteer, suspensions, transform, velocity, mass) in SystemAPI.Query<VehicleSteer, Suspensions, TransformAspect, RefRW<PhysicsVelocity>, PhysicsMass>())
        {
            var suspensionsMultiplier = math.min(suspensions.SuspensionsGroundedCount, 2) / 2f;

            var steerForce = transform.Up * vehicleSteer.CurrentSteer * vehicleSteer.Force * suspensionsMultiplier;
            mass.GetImpulseFromForce(transform.WorldScale, in steerForce, ForceMode.Force, deltaTime, out var impulse, out var impulseMass);
            velocity.ValueRW.ApplyAngularImpulse(impulseMass, transform.WorldScale, impulse);
        }
    }
}