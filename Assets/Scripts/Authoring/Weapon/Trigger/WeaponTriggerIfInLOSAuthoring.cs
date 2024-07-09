using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Authoring;
using UnityEngine;

[RequireComponent(typeof(LookAtTargetAuthoring))]
public class WeaponTriggerIfInLOSAuthoring : MonoBehaviour
{
	[SerializeField] [Range(0, 500)] private float _maxDistance = 20;
	[SerializeField] [Range(0, 90)] private float _maxAngle = 15;
	
	[SerializeField] private PhysicsCategoryTags _collisionFilter;

	class Baker : Baker<WeaponTriggerIfInLOSAuthoring>
	{
		public override void Bake(WeaponTriggerIfInLOSAuthoring authoring)
		{
			var weaponTriggerIfInLOS = new WeaponTriggerIfInLOS {
				MaxDistance = authoring._maxDistance,
				MaxAngle = math.radians(authoring._maxAngle),
				Filter = new CollisionFilter {
                    BelongsTo = authoring.GetComponentInChildren<PhysicsShapeAuthoring>().BelongsTo.Value,
                    CollidesWith = authoring._collisionFilter.Value,
                    GroupIndex = 0,
                }
			};

			AddComponent(weaponTriggerIfInLOS);
		}
	}
}