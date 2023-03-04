using Unity.Entities;
using UnityEngine;

public class HealthAuthoring : MonoBehaviour
{
	[SerializeField] [Min(0)] private int _maxHealth;

	class Baker : Baker<HealthAuthoring>
	{
		public override void Bake(HealthAuthoring authoring)
		{
			var health = new Health {
				Current = authoring._maxHealth,
				Max = authoring._maxHealth,
				LastDamagerEntity = Entity.Null
			};

			AddComponent(health);
		}
	}
}