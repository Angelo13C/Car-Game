using Unity.Entities;

public struct Explosion : IComponentData
{
    public ExplosionConfig Config;
    public bool ShouldExplode;
}