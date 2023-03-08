using Unity.Entities;
using UnityEngine;

public class BulletLauncherAuthoring : MonoBehaviour
{
    [SerializeField] private GameObject _firePoint;
    [SerializeField] private GameObject _bulletPrefab;
	[SerializeField] [Range(1, 100)] private float _launchSpeed = 20;

	class Baker : Baker<BulletLauncherAuthoring>
	{
		public override void Bake(BulletLauncherAuthoring authoring)
		{
			var bulletLauncher = new BulletLauncher {
				FirePoint = GetEntity(authoring._firePoint),
				BulletPrefab = GetEntity(authoring._bulletPrefab),
				LaunchSpeed = authoring._launchSpeed
			};

			AddComponent(bulletLauncher);
		}
	}
}