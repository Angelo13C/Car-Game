using Unity.Entities;
using Unity.Collections;

public struct SavableRoadTileMap : IComponentData
{
    public FixedString512Bytes SaveFilePath;

    public bool ShouldLoad;
}