using Unity.Entities;
using Unity.Mathematics;

public struct Node
{
    public BlobArray<short> Links;
    public float3 Position;
}
