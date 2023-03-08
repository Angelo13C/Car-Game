using Unity.Entities;
using UnityEngine;

public class WeaponTriggerAuthoring : MonoBehaviour
{
	class Baker : Baker<WeaponTriggerAuthoring>
	{
		public override void Bake(WeaponTriggerAuthoring authoring)
		{
            var weaponTrigger = WeaponTrigger.Resetted;

			AddComponent(weaponTrigger);
		}
	}
}