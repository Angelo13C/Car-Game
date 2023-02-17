using Unity.Entities;
using UnityEngine;

public struct NodeGraphSpawner : IComponentData
{
    public BlobAssetReference<NodeGraph> Graph;
}
