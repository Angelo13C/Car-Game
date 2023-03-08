using Unity.Entities;
using Unity.Physics.Authoring;
using UnityEngine;

public class VehicleMoverAuthoring : MonoBehaviour
{
    [Header("Force")]
    [SerializeField] [Min(0)] private float _gasForce = 30000;
    [SerializeField] [Min(0)] private float _brakeForce = 15000;
    
    [Header("Offset")]
    [SerializeField] private Vector3 _centerOfMassOffset;

	class Baker : Baker<VehicleMoverAuthoring>
	{
		public override void Bake(VehicleMoverAuthoring authoring)
		{
			var vehicleMover = new VehicleMover { 
                BrakeForce = authoring._brakeForce,
                GasForce = authoring._gasForce,
                CenterOfMassOffset = authoring._centerOfMassOffset,
                CurrentGasAndBrake = 0
			};

			AddComponent(vehicleMover);
		}
	}

#if UNITY_EDITOR
    private void OnDrawGizmos() {
        Gizmos.color = Color.green;
        var body = GetComponent<PhysicsBodyAuthoring>();
        var forcePoint = (Vector3) body.CustomMassDistribution.Transform.pos + transform.TransformPoint(_centerOfMassOffset);
        Gizmos.DrawWireSphere(forcePoint, 0.1f);  
    }
#endif
}