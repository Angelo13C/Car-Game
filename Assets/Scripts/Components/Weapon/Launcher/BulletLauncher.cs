using Unity.Entities;

public struct BulletLauncher : IComponentData
{
    public Entity FirePoint;
    public Entity BulletPrefab;
}