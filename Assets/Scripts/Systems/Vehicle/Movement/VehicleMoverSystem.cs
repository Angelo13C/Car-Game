using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Extensions;
using Unity.Transforms;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[BurstCompile]
public partial struct VehicleMoverSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
    }
    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var deltaTime = SystemAPI.Time.DeltaTime;
        foreach(var (vehicleMover, suspensions, transform, velocity, mass) in SystemAPI.Query<VehicleMover, Suspensions, TransformAspect, RefRW<PhysicsVelocity>, RefRO<PhysicsMass>>())
        {
            var suspensionsMultiplier = math.min(suspensions.SuspensionsGroundedCount, 2) / 2f;
            var force = transform.Forward * vehicleMover.CurrentGasAndBrake * suspensionsMultiplier;
            force *= vehicleMover.CurrentGasAndBrake > 0 ? vehicleMover.GasForce : vehicleMover.BrakeForce;
            
            var forceOffset = transform.TransformPointLocalToWorld(mass.ValueRO.CenterOfMass + vehicleMover.CenterOfMassOffset);
            
            mass.ValueRO.GetImpulseFromForce(transform.WorldScale, in force, ForceMode.Force, deltaTime, out var impulse, out var impulseMass);
            PhysicsComponentExtensions.ApplyImpulse(ref velocity.ValueRW, in impulseMass, transform.WorldPosition, transform.WorldRotation, transform.WorldScale, in impulse, forceOffset);
        }
    }
    
    public static float SpeedOfVehicle(PhysicsVelocity vehicleVelocity)
    {
        return math.length(new float2(vehicleVelocity.Linear.x, vehicleVelocity.Linear.z)) * VehicleMover.METERS_PER_SECOND_TO_KM_PER_HOUR;
    }
    public static float SpeedOfVehicleSqr(PhysicsVelocity vehicleVelocity)
    {
        return math.lengthsq(new float2(vehicleVelocity.Linear.x, vehicleVelocity.Linear.z)) * VehicleMover.METERS_PER_SECOND_TO_KM_PER_HOUR * VehicleMover.METERS_PER_SECOND_TO_KM_PER_HOUR;
    }
}