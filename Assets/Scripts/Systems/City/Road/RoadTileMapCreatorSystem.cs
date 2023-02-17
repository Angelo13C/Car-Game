using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateAfter(typeof(RoadTileMapEditorSystem))]
[BurstCompile]
public partial struct RoadTileMapCreatorSystem : ISystem
{
    private EntityQuery _roadTilesQuery;
    
    public void OnCreate(ref SystemState state)
    {
        _roadTilesQuery = state.GetEntityQuery(typeof(RoadTile));
    }
    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var entityCommandBuffer = new EntityCommandBuffer(Allocator.Temp);
        foreach(var (roadTileMap, roadTileSet) in SystemAPI.Query<RefRW<RoadTileMapReference>, RoadTileSet>())
        {
            roadTileMap.ValueRW.HasRefreshed = roadTileMap.ValueRO.NeedsRefresh;
            if(roadTileMap.ValueRO.NeedsRefresh)
            {
                roadTileMap.ValueRW.NeedsRefresh = false;

                entityCommandBuffer.DestroyEntity(_roadTilesQuery);

                ref var roadTileMapBlob = ref roadTileMap.ValueRO.BlobAsset.Value;

                for(var y = 0; y < roadTileMapBlob.GridSize.y; y++)
                {
                    for(var x = 0; x < roadTileMapBlob.GridSize.x; x++)
                    {
                        if(roadTileMapBlob.PlacedRoads[roadTileMapBlob.GridPositionToIndex(new int2(x, y))])
                        {
                            var roadEntity = entityCommandBuffer.Instantiate(roadTileSet.RoadPrefab);
                            var roadTilePosition = roadTileMapBlob.GridPositionToPosition(new int2(x, y));
                            entityCommandBuffer.SetComponent(roadEntity, new LocalTransform {
                                Position = new float3(roadTilePosition.x, 0.01f, roadTilePosition.y),
                                Rotation = quaternion.RotateX(math.radians(90)),
                                Scale = 8
                            });
                        }
                    }
                }
            }
        }

        entityCommandBuffer.Playback(state.EntityManager);
    }
}