using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;

[UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst = true)]
[UpdateBefore(typeof(PhysicsSystemGroup))]
[BurstCompile]
public partial struct WheelsRotationSystem : ISystem
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
        foreach(var (wheels, wheelsEntity) in SystemAPI.Query<Wheels>().WithEntityAccess())
        {
            if(SystemAPI.HasBuffer<Child>(wheelsEntity))
            {
                var carSpeed = VehicleMoverSystem.SpeedOfVehicle(SystemAPI.GetComponent<PhysicsVelocity>(wheels.CarBody));
                var rotationSpeed = carSpeed;

                var children = SystemAPI.GetBuffer<Child>(wheelsEntity);
                for(var i = 0; i < children.Length; i++)
                {
                    var childTransform = SystemAPI.GetAspectRW<TransformAspect>(children[i].Value);
                    childTransform.WorldRotation = math.mul(childTransform.WorldRotation, Unity.Mathematics.quaternion.EulerXYZ(rotationSpeed, 0, 0));
                }
            }
        }
    }
}