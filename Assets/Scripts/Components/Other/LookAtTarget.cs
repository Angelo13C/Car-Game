using Unity.Entities;

public struct LookAtTarget : IComponentData
{
    public Entity Target;

    public float Speed;
}