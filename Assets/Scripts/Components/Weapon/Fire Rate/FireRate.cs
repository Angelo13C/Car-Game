using Unity.Entities;

public struct FireRate : IComponentData
{
    public float TimeBetweenFire;
    public float LastFireTime;
}