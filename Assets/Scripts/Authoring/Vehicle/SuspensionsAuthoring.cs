using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics.Authoring;
using Unity.Collections;
using UnityEngine;
using Unity.Physics;

public class SuspensionsAuthoring : MonoBehaviour
{
    [SerializeField] private Vector3 _suspensionsOffset;

    [SerializeField] [Range(0, 1)] private float _suspensionHeight;
    [SerializeField] [Range(500, 50000)] private float _suspensionForce;

	[SerializeField] private PhysicsCategoryTags _collisionFilter;

	private float3[] GetSuspensionsPosition()
	{
		var centerOfMass = GetComponent<PhysicsBodyAuthoring>().CustomMassDistribution.Transform.pos;
		return new [] {
			centerOfMass + (float3) _suspensionsOffset,
			new float3(centerOfMass.x - _suspensionsOffset.x, centerOfMass.y + _suspensionsOffset.y, centerOfMass.z + _suspensionsOffset.z),
			new float3(centerOfMass.x + _suspensionsOffset.x, centerOfMass.y + _suspensionsOffset.y, centerOfMass.z - _suspensionsOffset.z),
			new float3(centerOfMass.x - _suspensionsOffset.x, centerOfMass.y + _suspensionsOffset.y, centerOfMass.z - _suspensionsOffset.z),
		};
	}

	class Baker : Baker<SuspensionsAuthoring>
	{
		public override void Bake(SuspensionsAuthoring authoring)
		{
			var suspensionsConfig = new Suspensions { 
				Height = authoring._suspensionHeight,
				Force = authoring._suspensionForce,
				CollisionFilter = new CollisionFilter {
					GroupIndex = CollisionFilter.Default.GroupIndex,
					BelongsTo = GetComponent<PhysicsShapeAuthoring>().BelongsTo.Value,
					CollidesWith = authoring._collisionFilter.Value
				},
				SuspensionsGroundedCount = 0
			};

			AddComponent(suspensionsConfig);

			var suspensionsOffset = authoring.GetSuspensionsPosition();
			var suspensions = AddBuffer<Suspension>();
			suspensions.ResizeUninitialized(4);
			for(var i = 0; i < 4; i++)
			{
				suspensions[i] = new Suspension {
					Position = suspensionsOffset[i],
					CurrentHeight = 0
				};
			}
		}
	}

#if UNITY_EDITOR
    private void OnDrawGizmos() {        
		var suspensions = GetSuspensionsPosition();

        foreach(var suspension in suspensions)
        {
            var start = transform.rotation * suspension + transform.position;
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(start, 0.2f);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(start, _suspensionHeight);
        }  
    }
#endif
}