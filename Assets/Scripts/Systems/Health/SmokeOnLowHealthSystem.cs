using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

public partial struct SmokeOnLowHealthSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
    }
    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    public void OnUpdate(ref SystemState state)
    {
        foreach(var (smokeOnLowHealth, health) in SystemAPI.Query<SmokeOnLowHealth, Health>())
        {
            uint particlesCount = 0;
            if(health.Current < smokeOnLowHealth.MinHealthToStartSmoking)
            {
                var particlesCountPercentage = 1f - ((float) health.Current) / smokeOnLowHealth.MinHealthToStartSmoking;
                particlesCount = (uint) math.round(math.lerp(smokeOnLowHealth.InitialSmokeParticlesCount, smokeOnLowHealth.FinalSmokeParticlesCount, particlesCountPercentage));
            }
            smokeOnLowHealth.SmokeVFX.SetUInt(SmokeOnLowHealth.SPAWN_RATE_PROPERTY_NAME, particlesCount);
        }
    }
}