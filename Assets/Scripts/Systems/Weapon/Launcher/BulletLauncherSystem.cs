using Unity.Burst;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;
using Unity.Mathematics;

[UpdateInGroup(typeof(WeaponFireSystemGroup))]
[BurstCompile]
public partial struct BulletLauncherSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var velocityLookup = SystemAPI.GetComponentLookup<PhysicsVelocity>();
        foreach(var (trigger, bulletLauncher) in SystemAPI.Query<WeaponTrigger, BulletLauncher>())
        {
            if(trigger.ShouldFire)
            {
                var firePointTransform = SystemAPI.GetComponent<WorldTransform>(bulletLauncher.FirePoint);
                var spawnedBullet = state.EntityManager.Instantiate(bulletLauncher.BulletPrefab);
                SystemAPI.SetComponent(spawnedBullet, (LocalTransform) firePointTransform);
                var spawnedBulletVelocity = velocityLookup.GetRefRW(spawnedBullet, false);
                spawnedBulletVelocity.ValueRW.Linear = math.mul(firePointTransform.Rotation, math.forward() * bulletLauncher.LaunchSpeed);
            }
        }
    }
}