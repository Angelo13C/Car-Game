using Unity.Entities;
using UnityEngine;

public class CityVehiclesSpawnerAuthoring : MonoBehaviour
{
	[SerializeField] private int _vehiclesCount;
    [SerializeField] private GameObject _vehiclePrefab;

	class Baker : Baker<CityVehiclesSpawnerAuthoring>
	{
		public override void Bake(CityVehiclesSpawnerAuthoring authoring)
		{
			var vehiclesSpawner = new CityVehiclesSpawner {
				VehiclesCount = authoring._vehiclesCount,
				VehiclePrefab = GetEntity(authoring._vehiclePrefab)
			};

			AddComponent(vehiclesSpawner);
		}
	}
}