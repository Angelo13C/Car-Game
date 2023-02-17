using Unity.Entities;
using Unity.Physics.Authoring;
using UnityEngine;

public class VehicleDragModifierAuthoring : MonoBehaviour
{
    [SerializeField] private float MinSpeedToReduceDrag;
    [SerializeField] private float SlowSpeedDrag;
	
	[Space]
    [SerializeField] private float NotGroundedDrag;

	class Baker : Baker<VehicleDragModifierAuthoring>
	{
		public override void Bake(VehicleDragModifierAuthoring authoring)
		{
			var vehicleDragModifier = new VehicleDragModifier {
				MinSpeedToReduceDragSqr = authoring.MinSpeedToReduceDrag * authoring.MinSpeedToReduceDrag,
				SlowSpeedDrag = authoring.SlowSpeedDrag,
				NotGroundedDrag = authoring.NotGroundedDrag,
				NormalDrag = authoring.GetComponent<PhysicsBodyAuthoring>().LinearDamping
			};

			AddComponent(vehicleDragModifier);
		}
	}
}