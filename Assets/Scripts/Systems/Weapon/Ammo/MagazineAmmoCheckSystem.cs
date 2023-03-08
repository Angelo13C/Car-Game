using Unity.Burst;
using Unity.Entities;

[UpdateInGroup(typeof(WeaponCanFireCheckSystemGroup))]
[BurstCompile]
public partial struct MagazineAmmoCheckSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach(var (trigger, magazineAmmo) in SystemAPI.Query<RefRW<WeaponTrigger>, RefRW<MagazineAmmo>>())
        {
            if(trigger.ValueRO.IsTriggered && trigger.ValueRO.ShouldFire)
            {
                trigger.ValueRW.ShouldFire = magazineAmmo.ValueRO.HasLeft;
            }
        }
    }
}