using Unity.Entities;

[InternalBufferCapacity(8)]
public struct FirePoint : IBufferElementData
{
    public Entity Point;
    public float FireDelay;
}

[InternalBufferCapacity(8)]
public struct BulletToLaunch : IBufferElementData
{
    public Entity FirePoint;
    public float RemainingDelay;
}