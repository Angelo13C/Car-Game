using Unity.Entities;

public struct VehicleAISteer : IComponentData
{
    public float SteerAngleBounds;

    public float TargetAngle;
}