using Unity.Entities;
using UnityEngine;

public class VehicleGearboxAuthoring : MonoBehaviour
{
    [SerializeField] private VehicleGear[] _gears = new VehicleGear[6];

	class Baker : Baker<VehicleGearboxAuthoring>
	{
		public override void Bake(VehicleGearboxAuthoring authoring)
		{
			var vehicleGearbox = AddBuffer<VehicleGear>();
			vehicleGearbox.CopyFrom(authoring._gears);
		}
	}
}