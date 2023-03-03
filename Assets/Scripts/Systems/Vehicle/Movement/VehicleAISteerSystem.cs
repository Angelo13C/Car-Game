using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Aspects;
using Unity.Transforms;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[BurstCompile]
public partial struct VehicleAISteerSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var deltaTime = SystemAPI.Time.DeltaTime;
        foreach(var (vehicleAISteer, vehicleSteer, rigidbody, velocity, transform) in SystemAPI.Query<RefRW<VehicleAISteer>, RefRW<VehicleSteer>, RigidBodyAspect, RefRW<PhysicsVelocity>, TransformAspect>())
        {
            if(vehicleAISteer.ValueRO.TargetAngle == VehicleAISteer.NO_TARGET_ANGLE)
                continue;
            
            vehicleAISteer.ValueRW.AddAngle(-velocity.ValueRO.Angular.y * deltaTime);
            var deltaAngle = vehicleAISteer.ValueRO.DeltaAngle();
            var deltaAngleSign = math.sign(deltaAngle);
            var newSteer = deltaAngleSign * math.unlerp(0, vehicleAISteer.ValueRO.SteerAngleBounds, deltaAngleSign * deltaAngle);
            vehicleSteer.ValueRW.CurrentSteer = -math.clamp(newSteer, -1f, 1f);
        }
    }
}