using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Extensions;
using Unity.Physics.Systems;
using Unity.Transforms;

[UpdateInGroup(typeof(AfterPhysicsSystemGroup))]
[BurstCompile]
public partial struct ExplodeSystem : ISystem
{    
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
        var fixedDeltaTime = SystemAPI.Time.DeltaTime;
        foreach(var (explosion, transform) in SystemAPI.Query<RefRW<Explosion>, LocalTransform>())
        {
            if(explosion.ValueRO.ShouldExplode)
            {
                explosion.ValueRW.ShouldExplode = false;

                var explodeForceJobHandle = new ExplodeJob
                {
                    CollisionWorld = physicsWorld.CollisionWorld,
                    Explosion = explosion.ValueRO,
                    ExplosionPosition = transform.Position,
                    FixedDeltaTime = fixedDeltaTime,
                    PhysicsVelocityLookup = SystemAPI.GetComponentLookup<PhysicsVelocity>(false),
                    PhysicsColliderLookup = SystemAPI.GetComponentLookup<PhysicsCollider>(true),
                    PhysicsMassLookup = SystemAPI.GetComponentLookup<PhysicsMass>(true),
                }.Schedule(state.Dependency);
                
                state.Dependency = JobHandle.CombineDependencies(state.Dependency, explodeForceJobHandle);
            }
        }
    }

    [BurstCompile]
    public struct ExplodeJob : IJob
    {
        [ReadOnly] public CollisionWorld CollisionWorld;
        public Explosion Explosion;
        public float3 ExplosionPosition;
        public float FixedDeltaTime;
        
        public ComponentLookup<PhysicsVelocity> PhysicsVelocityLookup;
        [ReadOnly] public ComponentLookup<PhysicsMass> PhysicsMassLookup;
        [ReadOnly] public ComponentLookup<PhysicsCollider> PhysicsColliderLookup;
        
        [BurstCompile]
        public void Execute()
        {
            var hittedObjects = new NativeList<DistanceHit>(Allocator.Temp);
            var collisionFilter =  new CollisionFilter {
                GroupIndex = CollisionFilter.Default.GroupIndex,
                BelongsTo = CollisionFilter.Default.BelongsTo,
                CollidesWith = Explosion.Config.HittablePhysicsTags.Value
            };

            if(CollisionWorld.OverlapSphere(ExplosionPosition, Explosion.Config.Radius, ref hittedObjects, collisionFilter))
            {
                var alreadyHitRigidbodies = new NativeArray<int>(hittedObjects.Length, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
                var alreadyHitRigidbodiesIndex = 0;
                var up = new float3(0, 1, 0);
                foreach(var hit in hittedObjects)
                {
                    if(!alreadyHitRigidbodies.GetSubArray(0, alreadyHitRigidbodiesIndex).Contains(hit.RigidBodyIndex))
                    {
                        if(PhysicsVelocityLookup.HasComponent(hit.Entity))
                        {
                            alreadyHitRigidbodies[alreadyHitRigidbodiesIndex] = hit.RigidBodyIndex;
                            alreadyHitRigidbodiesIndex++;

                            var hitVelocity = PhysicsVelocityLookup.GetRefRW(hit.Entity, false);
                            var hitMass = PhysicsMassLookup.GetRefRO(hit.Entity);
                            var hitCollider = PhysicsColliderLookup.GetRefRO(hit.Entity);
                            
                            var hitBody = CollisionWorld.Bodies[hit.RigidBodyIndex];
                            hitVelocity.ValueRW.ApplyExplosionForce(hitMass.ValueRO, hitCollider.ValueRO, hitBody.WorldFromBody.pos, 
                                hitBody.WorldFromBody.rot, hitBody.Scale, Explosion.Config.Force, ExplosionPosition,
                                Explosion.Config.Radius, FixedDeltaTime, up, collisionFilter, 0, ForceMode.Impulse);
                        }
                    }
                }

                alreadyHitRigidbodies.Dispose();
            }

            hittedObjects.Dispose();
        }
    }
}