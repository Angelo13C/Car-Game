using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

[UpdateInGroup(typeof(WeaponCanFireCheckSystemGroup))]
[BurstCompile]
public partial struct FireRateCheckSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var time = (float) SystemAPI.Time.ElapsedTime;
        var deltaTime = SystemAPI.Time.DeltaTime;
        foreach(var (trigger, fireRate) in SystemAPI.Query<RefRW<WeaponTrigger>, FireRate>())
        {
            if(trigger.ValueRO.IsTriggered && trigger.ValueRO.ShouldFire)
            {
                var timeSinceLastFire = time - fireRate.LastFireTime;
                if(timeSinceLastFire >= fireRate.TimeBetweenFire)
                {
                    trigger.ValueRW.BulletsToFire = (byte) math.ceil(deltaTime / fireRate.TimeBetweenFire);
                }
                else
                {
                    trigger.ValueRW.ShouldFire = false;
                }
            }
        }
    }
}