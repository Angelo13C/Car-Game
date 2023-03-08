using Unity.Burst;
using Unity.Entities;

[UpdateInGroup(typeof(WeaponFireSystemGroup))]
[BurstCompile]
public partial struct MagazineAmmoRemoveSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach(var (trigger, magazineAmmo) in SystemAPI.Query<WeaponTrigger, RefRW<MagazineAmmo>>())
        {
            magazineAmmo.ValueRW.Current -= trigger.BulletsToFireCount;
        }
    }
}