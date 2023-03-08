using Unity.Burst;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;
using Unity.Mathematics;

[UpdateInGroup(typeof(WeaponFireSystemGroup))]
[UpdateAfter(typeof(BulletLauncherPreparationSystem))]
[BurstCompile]
public partial struct BulletLauncherSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var deltaTime = SystemAPI.Time.DeltaTime;
        var velocityLookup = SystemAPI.GetComponentLookup<PhysicsVelocity>();
        foreach(var (bulletLauncher, bulletsToLaunch) in SystemAPI.Query<BulletLauncher, DynamicBuffer<BulletToLaunch>>())
        {
            for(var i = 0; i < bulletsToLaunch.Length; i++)
            {
                ref var bulletToLaunch = ref bulletsToLaunch.ElementAt(i);
                bulletToLaunch.RemainingDelay -= deltaTime;
                if(bulletToLaunch.RemainingDelay <= 0)
                {
                    var firePointTransform = SystemAPI.GetComponent<WorldTransform>(bulletToLaunch.FirePoint);
                    var spawnedBullet = state.EntityManager.Instantiate(bulletLauncher.BulletPrefab);
                    SystemAPI.SetComponent(spawnedBullet, (LocalTransform) firePointTransform);
                    var spawnedBulletVelocity = velocityLookup.GetRefRW(spawnedBullet, false);
                    spawnedBulletVelocity.ValueRW.Linear = math.mul(firePointTransform.Rotation, math.forward() * bulletLauncher.LaunchSpeed);

                    bulletsToLaunch.RemoveAtSwapBack(i);
                    i--;
                }
            }
        }
    }
}