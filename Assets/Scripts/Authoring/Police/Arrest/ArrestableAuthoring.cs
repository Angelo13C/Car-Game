using Unity.Entities;
using UnityEngine;

public class ArrestableAuthoring : MonoBehaviour
{
	[SerializeField] private int _requiredPointsToArrest;
    [SerializeField] [Min(0)] private float _removedPointsOverSecond = 1;

	class Baker : Baker<ArrestableAuthoring>
	{
		public override void Bake(ArrestableAuthoring authoring)
		{
			var arrestable = new Arrestable {
				CurrentPoints = 0f,
				RequiredPointsToArrest = authoring._requiredPointsToArrest,
				RemovedPointsOverSecond = authoring._removedPointsOverSecond
			};

			AddComponent(arrestable);
		}
	}
}