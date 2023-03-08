using Unity.Entities;
using UnityEngine;

public class FireRateAuthoring : MonoBehaviour
{
	[SerializeField] [Range(1, 5000)] private int _bulletsPerMinute = 200;

	class Baker : Baker<FireRateAuthoring>
	{
		public override void Bake(FireRateAuthoring authoring)
		{
			const float SECONDS_IN_A_MINUTE = 60f;
			var fireRate = new FireRate {
				TimeBetweenFire = SECONDS_IN_A_MINUTE / authoring._bulletsPerMinute,
				LastFireTime = float.NegativeInfinity
			};

			AddComponent(fireRate);
		}
	}
}