using UnityEngine;
using Unity.Entities;

public class FollowerAuthoring : MonoBehaviour
{
	[SerializeField] private GameObject _target;

	class Baker : Baker<FollowerAuthoring>
	{
		public override void Bake(FollowerAuthoring authoring)
		{
			var follower = new Follower {
				Target = GetEntity(authoring._target)
			};

			AddComponent(follower);
		}
	}
}