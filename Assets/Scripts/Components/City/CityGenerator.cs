using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class CityGenerator : IComponentData
{
    public Texture2D InputImage;
    
	public int2 CitySize;
	public uint Seed;

    public bool Generate;
}