using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;

public class VehicleAISteerAuthoring : MonoBehaviour
{
	[SerializeField] [Range(0, 180)] private float _steerAngleBounds;

	class Baker : Baker<VehicleAISteerAuthoring>
	{
		public override void Bake(VehicleAISteerAuthoring authoring)
		{
			var angle = authoring.transform.localRotation.eulerAngles.y;
			var vehicleAISteer = new VehicleAISteer {
				SteerAngleBounds = math.radians(authoring._steerAngleBounds),
				TargetAngle = 0,
			};

			AddComponent(vehicleAISteer);
		}
	}
}