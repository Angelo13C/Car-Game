using Unity.Burst;
using Unity.Entities;
using Unity.Physics;

[UpdateInGroup(typeof(WeaponFireSystemGroup))]
[BurstCompile]
public partial struct BulletLauncherPreparationSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var velocityLookup = SystemAPI.GetComponentLookup<PhysicsVelocity>();
        foreach(var (trigger, firePoints, bulletsToLaunch) in SystemAPI.Query<WeaponTrigger, DynamicBuffer<FirePoint>, DynamicBuffer<BulletToLaunch>>())
        {
            for(var i = 0; i < trigger.BulletsToFireCount; i++)
            {
                foreach(var firePoint in firePoints)
                {
                    bulletsToLaunch.Add(new BulletToLaunch {
                        FirePoint = firePoint.Point,
                        RemainingDelay = firePoint.FireDelay
                    });
                }
            }
        }
    }
}