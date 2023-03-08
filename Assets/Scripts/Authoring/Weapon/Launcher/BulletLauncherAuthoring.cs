using Unity.Entities;
using UnityEngine;

public class BulletLauncherAuthoring : MonoBehaviour
{
    [SerializeField] private GameObject _firePoint;
    [SerializeField] private GameObject _bulletPrefab;

	class Baker : Baker<BulletLauncherAuthoring>
	{
		public override void Bake(BulletLauncherAuthoring authoring)
		{
			var bulletLauncher = new BulletLauncher {
				FirePoint = GetEntity(authoring._firePoint),
				BulletPrefab = GetEntity(authoring._bulletPrefab)
			};

			AddComponent(bulletLauncher);
		}
	}
}