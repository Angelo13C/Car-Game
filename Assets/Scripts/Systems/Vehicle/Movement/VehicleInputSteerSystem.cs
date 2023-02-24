using Unity.Burst;
using Unity.Entities;
using UnityEngine;

[BurstCompile]
public partial struct VehicleInputSteerSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach(var (vehicleSteer, vehicleInputSteer) in SystemAPI.Query<RefRW<VehicleSteer>, RefRO<VehicleInputSteer>>())
        {
            var right = Input.GetKey(vehicleInputSteer.ValueRO.RightKey);
            var left = Input.GetKey(vehicleInputSteer.ValueRO.LeftKey);
            vehicleSteer.ValueRW.CurrentSteer = (right ? 1 : 0) - (left ? 1 : 0);
        }
    }
}