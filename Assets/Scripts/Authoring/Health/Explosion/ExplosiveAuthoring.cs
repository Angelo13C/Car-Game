using Unity.Entities;
using Unity.Physics.Authoring;
using UnityEngine;

public class ExplosiveAuthoring : MonoBehaviour
{
    [SerializeField] private float _force;
    [SerializeField] private float _radius;
	[SerializeField] private PhysicsCategoryTags _hittablePhysicsTags;

	[Space] [SerializeField] private GameObject _explosionPrefab;

	class Baker : Baker<ExplosiveAuthoring>
	{
		public override void Bake(ExplosiveAuthoring authoring)
		{
			var explosive = new Explosive {
				Config = new ExplosionConfig {
					Force = authoring._force,
					Radius = authoring._radius,
					HittablePhysicsTags = authoring._hittablePhysicsTags
				},
				ExplosionPrefab = GetEntity(authoring._explosionPrefab)
			};
			
			AddComponent(explosive);
		}
	}

#if UNITY_EDITOR
	private void OnDrawGizmos() {
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, _radius);
	}
#endif
}