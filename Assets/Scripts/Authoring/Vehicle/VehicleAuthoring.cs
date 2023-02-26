using Unity.Entities;
using UnityEngine;

public class VehicleAuthoring : MonoBehaviour
{
	class Baker : Baker<VehicleAuthoring>
	{
		public override void Bake(VehicleAuthoring authoring)
		{
			AddComponent(new VehicleTag());
		}
	}
}