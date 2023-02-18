using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics.Authoring;
using Unity.Transforms;

public struct ExplosionConfig
{
    public float Force;
    public float Radius;
    public PhysicsCategoryTags HittablePhysicsTags;
}

public struct Explosive : IComponentData
{
    public ExplosionConfig Config;
    public Entity ExplosionPrefab;
    
    [BurstCompile]
    public void Explode(EntityCommandBuffer entityCommandBuffer, float3 position)
    {
        var explosionEntity = entityCommandBuffer.Instantiate(ExplosionPrefab);
        entityCommandBuffer.SetComponent(explosionEntity, new LocalTransform {
            Position = position,
            Rotation = quaternion.identity,
            Scale = 1
        });
        entityCommandBuffer.SetComponent(explosionEntity, new Explosion {
            Config = Config,
            ShouldExplode = true
        });
    }
}