using UnityEngine;
using Unity.Entities;

public class FollowerAuthoring : MonoBehaviour
{
	[SerializeField] private GameObject _target;
	[SerializeField] [Range(0, 5)] private float _predictedTime;

	class Baker : Baker<FollowerAuthoring>
	{
		public override void Bake(FollowerAuthoring authoring)
		{
			var follower = new Follower {
				Target = GetEntity(authoring._target),
				PredictedTime = authoring._predictedTime
			};

			AddComponent(follower);
		}
	}
}