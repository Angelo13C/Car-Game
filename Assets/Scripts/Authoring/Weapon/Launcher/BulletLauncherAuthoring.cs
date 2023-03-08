using Unity.Entities;
using UnityEngine;

public class BulletLauncherAuthoring : MonoBehaviour
{
    [SerializeField] private GameObject _bulletPrefab;
	[SerializeField] [Range(1, 100)] private float _launchSpeed = 20;
	
	[System.Serializable]
	private struct FirePointConfig
	{
		public GameObject Point;
		[Range(0, 10)] public float FireDelay;
	}
    [SerializeField] private FirePointConfig[] _firePoints;

	class Baker : Baker<BulletLauncherAuthoring>
	{
		public override void Bake(BulletLauncherAuthoring authoring)
		{
			var bulletLauncher = new BulletLauncher {
				BulletPrefab = GetEntity(authoring._bulletPrefab),
				LaunchSpeed = authoring._launchSpeed
			};

			AddComponent(bulletLauncher);

			var firePoints = AddBuffer<FirePoint>();
			firePoints.ResizeUninitialized(authoring._firePoints.Length);
			for(var i = 0; i < authoring._firePoints.Length; i++)
			{
				var firePointConfig = authoring._firePoints[i];
				firePoints[i] = new FirePoint { 
					Point = GetEntity(firePointConfig.Point),
					FireDelay = firePointConfig.FireDelay
				};
			}
			
			var bulletsToLaunch = AddBuffer<BulletToLaunch>();
		}
	}
}