using Unity.Entities;
using UnityEngine;

public class VehicleCruiseControlAuthoring : MonoBehaviour
{
	[SerializeField] private float _initialSpeedToReach;

	class Baker : Baker<VehicleCruiseControlAuthoring>
	{
		public override void Bake(VehicleCruiseControlAuthoring authoring)
		{
			var cruiseControl = new VehicleCruiseControl {
				SpeedToReach = authoring._initialSpeedToReach
			};

			AddComponent(cruiseControl);
		}
	}
}