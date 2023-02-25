using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class CityGenerator : IComponentData
{
    public Texture2D InputImage;
    
	public float2 CellSize;
	public int2 CitySize;
	public uint Seed;
	public int N;

	public Color32 StreetColor;

	public StreetsPrefabs StreetsPrefabs;

    public bool Generate;
}

public struct StreetsPrefabs
{
	public Entity StraightStreetPrefab;
	public Entity DeadEndStreetPrefab;
	public Entity CrossStreetPrefab;
	public Entity IntersectionStreetPrefab;
	public Entity CurveStreetPrefab;
}

[InternalBufferCapacity(30)]
public struct CityPlaceableObject : IBufferElementData
{
	public Entity Prefab;
	public int2 Size;
	public CityObjectType Type;
}

public enum CityObjectType
{
	House
}