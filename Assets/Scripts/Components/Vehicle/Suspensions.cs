using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;

public struct Suspensions : IComponentData
{
    public const float FLOATING_SUSPENSION_HEIGHT = float.PositiveInfinity;

    public CollisionFilter CollisionFilter;
    public float Height;
    public float Force;
    public byte SuspensionsGroundedCount;
}

[InternalBufferCapacity(7)]
public struct Suspension : IBufferElementData
{
    public float3 Position;
    public float CurrentHeight;
}