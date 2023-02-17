using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

[UpdateAfter(typeof(CheckDeathSystem))]
[BurstCompile]
public partial struct DestroyOnDeathSystem : ISystem
{
    private EntityQuery _deadQuery;
    
    public void OnCreate(ref SystemState state)
    {
        _deadQuery = state.GetEntityQuery(typeof(Dead));
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var deadEntities = _deadQuery.ToEntityArray(Allocator.Temp);
        state.EntityManager.DestroyEntity(deadEntities);
        deadEntities.Dispose();
    }
}