using Unity.Burst;
using Unity.Entities;
using Unity.Physics;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[BurstCompile]
public partial struct VehicleDragModifierSystem : ISystem
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
        foreach(var (vehicleMover, dragModifier, suspensions, physicsDamping, velocity) in SystemAPI.Query<VehicleMover, VehicleDragModifier, Suspensions, RefRW<PhysicsDamping>, PhysicsVelocity>())
        {
            var vehicleSpeedSqr = VehicleMoverSystem.SpeedOfVehicleSqr(velocity);
            if(suspensions.SuspensionsGroundedCount <= 1)
                physicsDamping.ValueRW.Linear = dragModifier.NotGroundedDrag;
            else if(vehicleMover.CurrentGasAndBrake == 0 && vehicleSpeedSqr >= dragModifier.MinSpeedToReduceDragSqr)
                physicsDamping.ValueRW.Linear = dragModifier.SlowSpeedDrag;
            else
                physicsDamping.ValueRW.Linear = dragModifier.NormalDrag;
/*
            if(vehicleSpeedSqr <= 5 * 5)
                physicsDamping.ValueRW.Angular = 10;
            else
                physicsDamping.ValueRW.Angular = 1.5f;*/
        }
    }
}