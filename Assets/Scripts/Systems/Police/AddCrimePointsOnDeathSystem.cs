using Unity.Burst;
using Unity.Entities;

[UpdateAfter(typeof(CheckDeathSystem))]
[UpdateBefore(typeof(DestroyOnDeathSystem))]
[BurstCompile]
public partial struct AddCrimePointsOnDeathSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var wantedLookup = SystemAPI.GetComponentLookup<Wanted>();
        foreach(var (addCrimePointsOnDeath, health) in SystemAPI.Query<AddCrimePointsOnDeath, Health>().WithAll<Dead>())
        {
            var wanted = wantedLookup.GetRefRWOptional(health.LastDamagerEntity, false);
            if(wanted.IsValid)
            {
                wanted.ValueRW.CurrentCrimePoints += addCrimePointsOnDeath.PointsToAdd;
            }
        }
    }
}