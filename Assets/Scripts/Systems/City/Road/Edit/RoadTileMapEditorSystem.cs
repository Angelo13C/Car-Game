using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;

[UpdateAfter(typeof(RoadTileMapEditorInputSystem))]
[BurstCompile]
public partial struct RoadTileMapEditorSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
        foreach(var (roadTileMap, editableRoadTileMap) in SystemAPI.Query<RefRW<RoadTileMapReference>, RefRW<EditableRoadTileMap>>())
        {
            if(editableRoadTileMap.ValueRO.Edit)
            {
                editableRoadTileMap.ValueRW.Edit = false;

                if(physicsWorld.CastRay(editableRoadTileMap.ValueRO.RaycastInput, out var hit))
                {
                    var hitPosition = new float2(hit.Position.x, hit.Position.z);

                    ref var oldRoadTileMapBlob = ref roadTileMap.ValueRO.BlobAsset.Value;
                    var hitGridPosition = oldRoadTileMapBlob.PositionToGridPosition(hitPosition);
                    if(oldRoadTileMapBlob.IsGridPositionValid(hitGridPosition))
                    {
                        RoadTileMapReference.UpdateBlobAsset(ref roadTileMap.ValueRW.BlobAsset, hitGridPosition);

                        roadTileMap.ValueRW.NeedsRefresh = true;
                    }
                }
            }
        }
    }
}