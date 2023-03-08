using Unity.Entities;
using UnityEngine;

public class MagazineAmmoAuthoring : MonoBehaviour
{
	[SerializeField] [Range(1, 1000)] private short _maxAmmo;

	class Baker : Baker<MagazineAmmoAuthoring>
	{
		public override void Bake(MagazineAmmoAuthoring authoring)
		{
			var magazineAmmo = new MagazineAmmo {
				Current = authoring._maxAmmo
			};

			AddComponent(magazineAmmo);
		}
	}
}