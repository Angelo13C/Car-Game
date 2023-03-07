using Unity.Entities;
using UnityEngine;

public class LookAtTargetAuthoring : MonoBehaviour
{
	[SerializeField] private GameObject _target;
	[SerializeField] [Range(0, 1)] private float _speed;

	class Baker : Baker<LookAtTargetAuthoring>
	{
		public override void Bake(LookAtTargetAuthoring authoring)
		{
			var lookAtTarget = new LookAtTarget {
				Target = GetEntity(authoring._target),
				Speed = authoring._speed
			};

			AddComponent(lookAtTarget);
		}
	}
}