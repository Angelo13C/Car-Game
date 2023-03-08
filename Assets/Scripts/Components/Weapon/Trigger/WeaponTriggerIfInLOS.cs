using Unity.Entities;
using Unity.Physics;

public struct WeaponTriggerIfInLOS : IComponentData
{
    public float MaxDistance;
    public float MaxAngle;

    public CollisionFilter Filter;
}