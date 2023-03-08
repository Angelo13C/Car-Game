using Unity.Entities;

public struct BulletLauncher : IComponentData
{
    public Entity BulletPrefab;

    public float LaunchSpeed;
}