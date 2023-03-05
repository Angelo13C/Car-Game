using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class CityVehiclesSpawnerAuthoring : MonoBehaviour
{
	[SerializeField] private int _vehiclesCount;
    [SerializeField] private GameObject _vehiclePrefab;
	[SerializeField] private Vector2Int _spawnZoneSize = new Vector2Int(40, 40);

	class Baker : Baker<CityVehiclesSpawnerAuthoring>
	{
		public override void Bake(CityVehiclesSpawnerAuthoring authoring)
		{
			var vehiclesSpawner = new CityVehiclesSpawner {
				VehiclesCount = authoring._vehiclesCount,
				VehiclePrefab = GetEntity(authoring._vehiclePrefab),
				SpawnZoneSize = new int2(authoring._spawnZoneSize.x, authoring._spawnZoneSize.y)
			};

			AddComponent(vehiclesSpawner);
		}
	}
}