using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateAfter(typeof(TransformSystemGroup))]
[BurstCompile]
public partial struct WheelsPositionSystem : ISystem
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
            var suspensions = SystemAPI.GetBuffer<Suspension>(wheels.CarBody);
            var children = SystemAPI.GetBuffer<Child>(wheelsEntity);
            for(var i = 0; i < children.Length; i++)
            {
                if(suspensions[i].CurrentHeight != Suspensions.FLOATING_SUSPENSION_HEIGHT)
                {
                    var wheelTransform = SystemAPI.GetAspectRW<TransformAspect>(children[i].Value);
                    wheelTransform.LocalPosition = new float3(wheelTransform.LocalPosition.x, suspensions[i].Position.y - suspensions[i].CurrentHeight, wheelTransform.LocalPosition.z);
                }
            }
        }
    }
}