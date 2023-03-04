using Unity.Entities;
using UnityEngine;

public class AddCrimePointsOnDeathAuthoring : MonoBehaviour
{
	[SerializeField] [Min(1)] private int _pointsToAdd = 1;

	class Baker : Baker<AddCrimePointsOnDeathAuthoring>
	{
		public override void Bake(AddCrimePointsOnDeathAuthoring authoring)
		{
			var addCrimePointsOnDeath = new AddCrimePointsOnDeath {
				PointsToAdd = authoring._pointsToAdd
			};

			AddComponent(addCrimePointsOnDeath);
		}
	}
}