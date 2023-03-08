using Unity.Burst;
using Unity.Entities;

[UpdateInGroup(typeof(WeaponTriggerSystemGroup), OrderFirst = true)]
[BurstCompile]
public partial struct WeaponTriggerSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach(var trigger in SystemAPI.Query<RefRW<WeaponTrigger>>())
        {
            trigger.ValueRW = WeaponTrigger.Resetted;
        }
    }
}