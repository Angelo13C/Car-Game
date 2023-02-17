using Unity.Entities;
using Unity.Physics;

public struct EditableRoadTileMap : IComponentData
{
    public CollisionFilter CollisionFilter;

    public bool Edit;
    public RaycastInput RaycastInput;
}