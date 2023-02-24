using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

[UpdateAfter(typeof(CheckDeathSystem))]
[UpdateBefore(typeof(DestroyOnDeathSystem))]
[BurstCompile]
public partial struct ExplodeOnDeathSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var entityCommandBuffer = new EntityCommandBuffer(Allocator.Temp);
        foreach(var (explosive, transform) in SystemAPI.Query<Explosive, TransformAspect>().WithAll<Dead>())
        {
            explosive.Explode(entityCommandBuffer, transform.WorldPosition);
        }
        entityCommandBuffer.Playback(state.EntityManager);
    }
}