using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[BurstCompile]
public partial struct VehicleAISteerSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach(var (vehicleAISteer, vehicleSteer, velocity, transform) in SystemAPI.Query<VehicleAISteer, RefRW<VehicleSteer>, RefRW<PhysicsVelocity>, TransformAspect>())
        {
            var deltaAngle = transform.WorldRotation.ComputeYAngle() - vehicleAISteer.TargetAngle;
            var deltaAngleSign = math.sign(vehicleAISteer.TargetAngle);
            var newSteer = deltaAngleSign * math.unlerp(0, vehicleAISteer.SteerAngleBounds, deltaAngleSign * deltaAngle);
            vehicleSteer.ValueRW.CurrentSteer = math.clamp(newSteer, -0.7f, 0.7f);
        }
    }
}