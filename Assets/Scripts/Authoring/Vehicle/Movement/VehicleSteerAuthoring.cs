using Unity.Entities;
using UnityEngine;

public class VehicleSteerAuthoring : MonoBehaviour
{
    [SerializeField] [Min(0)] private float _steerForce = 8000;
    [SerializeField] [Range(0, 100)] private int _minSpeedToFullySteer;

	class Baker : Baker<VehicleSteerAuthoring>
	{
		public override void Bake(VehicleSteerAuthoring authoring)
		{
			var vehicleSteer = new VehicleSteer {
				CurrentSteer = 0,
				Force = authoring._steerForce,
				MinSpeedToFullySteer = authoring._minSpeedToFullySteer
			};

			AddComponent(vehicleSteer);
		}
	}
}