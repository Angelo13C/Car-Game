using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Collections;

[BurstCompile]
public partial struct ArresterSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var entityCommandBuffer = new EntityCommandBuffer(Allocator.Temp);
        var deltaTime = SystemAPI.Time.DeltaTime;
        foreach(var (arrestable, arrestableTransform, arrestableEntity) in SystemAPI.Query<RefRW<Arrestable>, WorldTransform>().WithEntityAccess().WithNone<Arrested>())
        {
            var arrestablePosition = arrestableTransform.Position;
            var arresting = false;
            foreach(var (arrester, arresterTransform) in SystemAPI.Query<Arrester, WorldTransform>())
            {
                if(math.distancesq(arrestablePosition, arresterTransform.Position) <= arrester.ArrestRadiusSqr)
                {
                    arresting = true;
                    arrestable.ValueRW.CurrentPoints += deltaTime;
                    if(arrestable.ValueRO.CurrentPoints >= arrestable.ValueRO.RequiredPointsToArrest)
                    {
                        entityCommandBuffer.AddComponent<Arrested>(arrestableEntity);
                        break;
                    }
                }
            }

            if(!arresting)
            {
                var pointsToRemove = deltaTime * arrestable.ValueRO.RemovedPointsOverSecond;
                arrestable.ValueRW.CurrentPoints = math.max(arrestable.ValueRO.CurrentPoints - pointsToRemove, 0);
            }
        }

        entityCommandBuffer.Playback(state.EntityManager);
    }
}