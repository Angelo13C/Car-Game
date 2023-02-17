using Unity.Entities;
using UnityEngine;

public class DamageOverTimeAuthoring : MonoBehaviour
{
    [SerializeField] private int _damagePerSecond;

	class Baker : Baker<DamageOverTimeAuthoring>
	{
		public override void Bake(DamageOverTimeAuthoring authoring)
		{
			var damageOverTime = new DamageOverTime {
				DamagePerSecond = authoring._damagePerSecond,
				DecimalNotDealtDamage = 0
			};

			AddComponent(damageOverTime);
		}
	}
}