using Unity.Entities;
using UnityEngine.VFX;

public class SmokeOnLowHealth : IComponentData
{
    public int MinHealthToStartSmoking;
    public int InitialSmokeParticlesCount;
    public int FinalSmokeParticlesCount;

    public VisualEffect SmokeVFX;
    public const string SPAWN_RATE_PROPERTY_NAME = "SpawnRate";
}