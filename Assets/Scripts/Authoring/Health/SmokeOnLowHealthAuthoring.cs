using Unity.Entities;
using UnityEngine;
using UnityEngine.VFX;

public class SmokeOnLowHealthAuthoring : MonoBehaviour
{
	[SerializeField] private VisualEffect _smokeVFX;

	[Header("Config")]
    [SerializeField] private int _minHealthToStartSmoking;
    [SerializeField] private int _initialSmokeParticlesCount;
    [SerializeField] private int _finalSmokeParticlesCount;

	class Baker : Baker<SmokeOnLowHealthAuthoring>
	{
		public override void Bake(SmokeOnLowHealthAuthoring authoring)
		{
			var smokeOnLowHealth = new SmokeOnLowHealth {
				SmokeVFX = authoring._smokeVFX,
				MinHealthToStartSmoking = authoring._minHealthToStartSmoking,
				InitialSmokeParticlesCount = authoring._initialSmokeParticlesCount,
				FinalSmokeParticlesCount = authoring._finalSmokeParticlesCount
			};

			AddComponentObject(smokeOnLowHealth);
		}
	}
}