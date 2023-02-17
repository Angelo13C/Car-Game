using Unity.Burst;
using Unity.Entities;
using Unity.Physics;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateBefore(typeof(VehicleMoverSystem))]
[BurstCompile]
public partial struct VehicleGearboxSystem : ISystem
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
        foreach(var (vehicleGearbox, vehicleMover, velocity) in SystemAPI.Query<DynamicBuffer<VehicleGear>, RefRW<VehicleMover>, PhysicsVelocity>())
        {
            var speed = VehicleMoverSystem.SpeedOfVehicle(velocity);
        //UnityEngine.Debug.Log("Speed: " + Unity.Mathematics.math.floor(speed) + "km/h");

            void SetGear(int gearIndex)
            {
                vehicleMover.ValueRW.GasForce = vehicleGearbox[gearIndex].GasForce;
            }
            
            for(var i = 1; i < vehicleGearbox.Length; i++)
            {
                if(speed < vehicleGearbox[i].MinSpeed)
                {
                    SetGear(i - 1);
                    break;
                }
            }
            if(speed >= vehicleGearbox[vehicleGearbox.Length - 1].MinSpeed)
                SetGear(vehicleGearbox.Length - 1);
        }
    }
}