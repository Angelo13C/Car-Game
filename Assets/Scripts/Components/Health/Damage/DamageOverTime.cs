using Unity.Entities;

public struct DamageOverTime : IComponentData
{
    public int DamagePerSecond;
    public float DecimalNotDealtDamage;
}