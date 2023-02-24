using Unity.Burst;
using Unity.Entities;
using UnityEngine;

[BurstCompile]
public partial struct VehicleInputMoverSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach(var (vehicleMover, vehicleInputMover) in SystemAPI.Query<RefRW<VehicleMover>, RefRO<VehicleInputMover>>())
        {
            var gas = Input.GetKey(vehicleInputMover.ValueRO.GasKey);
            var brake = Input.GetKey(vehicleInputMover.ValueRO.BrakeKey);
            vehicleMover.ValueRW.CurrentGasAndBrake = (gas ? 1 : 0) - (brake ? 1 : 0);
        }
    }
}