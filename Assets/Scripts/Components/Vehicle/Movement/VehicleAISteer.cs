using Unity.Entities;
using Unity.Mathematics;

public struct VehicleAISteer : IComponentData
{
    public float SteerAngleBounds;

    public float CurrentAngle;

    public void AddAngle(float value)
    {
        CurrentAngle += value;
    }

    public float DeltaAngle()
    {
        var diff = (TargetAngle - CurrentAngle + math.PI ) % (2 * math.PI) - math.PI;
        return diff < -math.PI ? diff + (2 * math.PI) : diff;
    }
    
    public float TargetAngle;
    public const float NO_TARGET_ANGLE = float.PositiveInfinity;
}