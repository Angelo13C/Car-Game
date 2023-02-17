using Unity.Entities;
using UnityEngine;

public class VehicleSteerAuthoring : MonoBehaviour
{
    [SerializeField] [Range(1000, 50000)] private float _steerForce;

	class Baker : Baker<VehicleSteerAuthoring>
	{
		public override void Bake(VehicleSteerAuthoring authoring)
		{
			var vehicleSteer = new VehicleSteer {
				CurrentSteer = 0,
				Force = authoring._steerForce
			};

			AddComponent(vehicleSteer);
		}
	}
}