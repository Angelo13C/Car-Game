using Unity.Burst;
using Unity.Entities;
using Unity.Physics;
using UnityEngine;

[BurstCompile]
public partial struct RoadTileMapEditorInputSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
    }
    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    public void OnUpdate(ref SystemState state)
    {
        if(Input.GetMouseButtonDown(0))
        {
            foreach(var editableRoadTileMap in SystemAPI.Query<RefRW<EditableRoadTileMap>>())
            {
                editableRoadTileMap.ValueRW.Edit = true;
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                editableRoadTileMap.ValueRW.RaycastInput = new RaycastInput {
                    Start = ray.origin,
                    End = ray.origin + ray.direction * 1000,
                    Filter = editableRoadTileMap.ValueRO.CollisionFilter
                };
            }
        }
    }
}