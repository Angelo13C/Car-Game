using Unity.Collections;
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
/*
    public int SuspensionsCount => SuspensionsPositions.Length;

    public int GetSuspensionsGroundedCount()
    {
        var groundedCount = 0;
        foreach(var suspensionCurrentHeight in SuspensionsCurrentHeight)
        {
            if(suspensionCurrentHeight <= Height)
                groundedCount++;
        }
        return groundedCount;
    }*/
}

[InternalBufferCapacity(7)]
public struct Suspension : IBufferElementData
{
    public float3 Position;
    public float CurrentHeight;
}