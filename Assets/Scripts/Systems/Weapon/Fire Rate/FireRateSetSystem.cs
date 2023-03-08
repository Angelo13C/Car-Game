using Unity.Burst;
using Unity.Entities;

[UpdateInGroup(typeof(WeaponFireSystemGroup))]
[BurstCompile]
public partial struct FireRateSetSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var time = (float) SystemAPI.Time.ElapsedTime;
        foreach(var (trigger, fireRate) in SystemAPI.Query<WeaponTrigger, RefRW<FireRate>>())
        {
            if(trigger.ShouldFire)
            {
                fireRate.ValueRW.LastFireTime = time;
            }
        }
    }
}