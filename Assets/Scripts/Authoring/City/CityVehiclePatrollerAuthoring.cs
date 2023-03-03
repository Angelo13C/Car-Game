using Unity.Entities;
using UnityEngine;

public class CityVehiclePatrollerAuthoring : MonoBehaviour
{
	class Baker : Baker<CityVehiclePatrollerAuthoring>
	{
		public override void Bake(CityVehiclePatrollerAuthoring authoring)
		{
			var cityVehiclePatroller = new CityVehiclePatroller {
				LastStreetTileIndex = -1
			};

			AddComponent(cityVehiclePatroller);
		}
	}
}