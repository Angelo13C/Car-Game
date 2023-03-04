using Unity.Entities;
using UnityEngine;

public class ArrestableAuthoring : MonoBehaviour
{
	[SerializeField] private int _requiredPointsToArrest;

	class Baker : Baker<ArrestableAuthoring>
	{
		public override void Bake(ArrestableAuthoring authoring)
		{
			var arrestable = new Arrestable {
				CurrentPoints = 0f,
				RequiredPointsToArrest = authoring._requiredPointsToArrest
			};

			AddComponent(arrestable);
		}
	}
}