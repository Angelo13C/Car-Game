using Unity.Entities;
using UnityEngine;

public class VehicleInputSteerAuthoring : MonoBehaviour
{
	[SerializeField] private KeyCode _rightKey;
	[SerializeField] private KeyCode _leftKey;

	class Baker : Baker<VehicleInputSteerAuthoring>
	{
		public override void Bake(VehicleInputSteerAuthoring authoring)
		{
			var vehicleInputSteer = new VehicleInputSteer {
				RightKey = authoring._rightKey,
				LeftKey = authoring._leftKey
			};

			AddComponent(vehicleInputSteer);
		}
	}
}