using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

[UpdateInGroup(typeof(SimulationSystemGroup))]
[BurstCompile]
public partial struct CheckDeathSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var entityCommandBuffer = new EntityCommandBuffer(Allocator.Temp);
        foreach(var (health, entity) in SystemAPI.Query<Health>().WithEntityAccess())
        {
            if(health.Current <= 0)
            {
                entityCommandBuffer.AddComponent<Dead>(entity);
            }
        }

        entityCommandBuffer.Playback(state.EntityManager);
    }
}