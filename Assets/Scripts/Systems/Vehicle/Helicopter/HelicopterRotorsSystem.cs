using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

[BurstCompile]
public partial struct HelicopterRotorsSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var deltaTime = SystemAPI.Time.DeltaTime;
        var localTransformLookup = SystemAPI.GetComponentLookup<LocalTransform>();
        foreach(var (helicopterRotors, transform) in SystemAPI.Query<HelicopterRotors, LocalTransform>())
        {
            var heightPercentage = math.unlerp(0, helicopterRotors.MinYForFullSpeed, transform.Position.y);
            var rotationSpeed = heightPercentage * helicopterRotors.FullSpeedRotationPerSecond * deltaTime;

            var tailRotorTransform = localTransformLookup.GetRefRW(helicopterRotors.TailRotor, false);
            tailRotorTransform.ValueRW = tailRotorTransform.ValueRO.RotateX(rotationSpeed);

            var mainRotorTransform = localTransformLookup.GetRefRW(helicopterRotors.MainRotor, false);
            mainRotorTransform.ValueRW = mainRotorTransform.ValueRO.RotateY(rotationSpeed);
        }
    }
}