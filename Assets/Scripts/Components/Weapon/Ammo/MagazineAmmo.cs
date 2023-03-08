using Unity.Entities;

public struct MagazineAmmo : IComponentData
{
    public short Current;

    public bool HasLeft => Current > 0;
}