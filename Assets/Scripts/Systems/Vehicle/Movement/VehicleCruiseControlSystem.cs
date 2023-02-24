using Unity.Burst;
using Unity.Entities;
using Unity.Physics;

[BurstCompile]
public partial struct VehicleCruiseControlSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach(var (cruiseControl, velocity, vehicleMover) in SystemAPI.Query<VehicleCruiseControl, PhysicsVelocity, RefRW<VehicleMover>>())
        {
            var currentSpeedSqr = VehicleMoverSystem.SpeedOfVehicleSqr(velocity);
            if(currentSpeedSqr < cruiseControl.SpeedToReach * cruiseControl.SpeedToReach)
                vehicleMover.ValueRW.CurrentGasAndBrake = 1;
            else
                vehicleMover.ValueRW.CurrentGasAndBrake = 0;
        }
    }
}