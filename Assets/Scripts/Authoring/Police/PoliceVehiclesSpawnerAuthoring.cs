using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class PoliceVehiclesSpawnerAuthoring : MonoBehaviour
{
	[SerializeField] private GameObject _policeCarPrefab;
	[SerializeField] private Vector2Int _spawnZoneSize = new Vector2Int(5, 10);

	class Baker : Baker<PoliceVehiclesSpawnerAuthoring>
	{
		public override void Bake(PoliceVehiclesSpawnerAuthoring authoring)
		{
			var policeVehiclesSpawner = new PoliceVehiclesSpawner {
				CarPrefab = GetEntity(authoring._policeCarPrefab),
				SpawnZoneSize = new int2(authoring._spawnZoneSize.x, authoring._spawnZoneSize.y)
			};

			AddComponent(policeVehiclesSpawner);
		}
	}
}