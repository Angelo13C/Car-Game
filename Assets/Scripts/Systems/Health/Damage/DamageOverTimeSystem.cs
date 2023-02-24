using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateBefore(typeof(CheckDeathSystem))]
[BurstCompile]
public partial struct DamageOverTimeSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var deltaTime = SystemAPI.Time.DeltaTime;
        foreach(var (health, damageOverTime) in SystemAPI.Query<RefRW<Health>, RefRW<DamageOverTime>>())
        {
            var damageToDeal = deltaTime * damageOverTime.ValueRO.DamagePerSecond;
            var truncatedDamageToDeal = (int) math.trunc(damageToDeal);
            damageOverTime.ValueRW.DecimalNotDealtDamage += damageToDeal - truncatedDamageToDeal;
            if(damageOverTime.ValueRO.DecimalNotDealtDamage >= 1)
            {
                damageOverTime.ValueRW.DecimalNotDealtDamage--;
                truncatedDamageToDeal++;
            }

            health.ValueRW.Current -= truncatedDamageToDeal;
        }
    }
}