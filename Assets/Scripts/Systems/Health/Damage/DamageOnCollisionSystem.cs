using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(PhysicsSystemGroup))]
[BurstCompile]
public partial struct DamageOnCollisionSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        state.Dependency = new CollisionEventJob
        {
            PhysicsWorldSingleton = SystemAPI.GetSingleton<PhysicsWorldSingleton>(),
            DamageOnCollision = SystemAPI.GetComponentLookup<DamageOnCollision>(true),
            Health = SystemAPI.GetComponentLookup<Health>(false),
        }.Schedule(SystemAPI.GetSingleton<SimulationSingleton>(), state.Dependency);
    }
    
    [BurstCompile]
    public partial struct CollisionEventJob : ICollisionEventsJob
    {
        [ReadOnly] public PhysicsWorldSingleton PhysicsWorldSingleton;
        
        [ReadOnly] public ComponentLookup<DamageOnCollision> DamageOnCollision;
        public ComponentLookup<Health> Health;

        public void Execute(CollisionEvent collisionEvent)
        {
            var entityA = Entity.Null;
            var entityB = Entity.Null;
            if(DamageOnCollision.HasComponent(collisionEvent.EntityA))
                entityA = collisionEvent.EntityA;
            if(DamageOnCollision.HasComponent(collisionEvent.EntityB))
                entityB = collisionEvent.EntityB;

            if(entityA != Entity.Null || entityB != Entity.Null)
            {
                var collisionDetails = collisionEvent.CalculateDetails(ref PhysicsWorldSingleton.PhysicsWorld);
                var collisionForce = collisionDetails.EstimatedImpulse;
                
                // Had to duplicate the code for each entity instead of using a function because I got the error CS1673
                if(entityA != Entity.Null)
                {
                    var damageOnCollision = DamageOnCollision.GetRefRO(entityA);
                    if(collisionForce >= damageOnCollision.ValueRO.MinForceToDamage)
                    {
                        var health = Health.GetRefRW(entityA, false);
                        var damage = damageOnCollision.ValueRO.DamageToDealForMinForce;
                        health.ValueRW.Current -= damage;
                    }
                }
                if(entityB != Entity.Null)
                {
                    var damageOnCollision = DamageOnCollision.GetRefRO(entityB);
                    if(collisionForce >= damageOnCollision.ValueRO.MinForceToDamage)
                    {
                        var health = Health.GetRefRW(entityB, false);
                        var damage = damageOnCollision.ValueRO.DamageToDealForMinForce;
                        health.ValueRW.Current -= damage;
                    }
                }
            }
        }
    }
}