using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

[UpdateInGroup(typeof(WeaponFireSystemGroup))]
[BurstCompile]
public partial struct BulletLauncherSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach(var (trigger, bulletLauncher) in SystemAPI.Query<WeaponTrigger, BulletLauncher>())
        {
            if(trigger.ShouldFire)
            {
                var firePointTransform = SystemAPI.GetComponent<WorldTransform>(bulletLauncher.FirePoint);
                var spawnedBullet = state.EntityManager.Instantiate(bulletLauncher.BulletPrefab);
                SystemAPI.SetComponent(spawnedBullet, (LocalTransform) firePointTransform);
            }
        }
    }
}