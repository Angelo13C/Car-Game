using Unity.Entities;
using UnityEngine;

public class VehicleInputMoverAuthoring : MonoBehaviour
{
	[SerializeField] private KeyCode _gasKey;
	[SerializeField] private KeyCode _brakeKey;

	class Baker : Baker<VehicleInputMoverAuthoring>
	{
		public override void Bake(VehicleInputMoverAuthoring authoring)
		{
			var vehicleInputMover = new VehicleInputMover {
				GasKey = authoring._gasKey,
				BrakeKey = authoring._brakeKey
			};

			AddComponent(vehicleInputMover);
		}
	}
}