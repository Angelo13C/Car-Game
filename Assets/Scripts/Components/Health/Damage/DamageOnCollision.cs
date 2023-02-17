using Unity.Entities;

public struct DamageOnCollision : IComponentData
{
    public float MinForceToDamage;
    public int DamageToDealForMinForce;
}