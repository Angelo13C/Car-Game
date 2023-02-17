using Unity.Entities;
using UnityEngine;

public class DamageOnCollisionAuthoring : MonoBehaviour
{
    [SerializeField] [Min(0)] private float _minForceToDamage;
    [SerializeField] [Min(0)] private int _damageToDealForMinForce;

	class Baker : Baker<DamageOnCollisionAuthoring>
	{
		public override void Bake(DamageOnCollisionAuthoring authoring)
		{
			var damageOnCollision = new DamageOnCollision {
				MinForceToDamage = authoring._minForceToDamage,
				DamageToDealForMinForce = authoring._damageToDealForMinForce
			};

			AddComponent(damageOnCollision);
		}
	}
}