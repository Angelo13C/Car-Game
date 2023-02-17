using Unity.Entities;
using Unity.Mathematics;

public struct VehicleMover : IComponentData
{
    public float GasForce;
    public float BrakeForce;
    public float CurrentGasAndBrake { get; set; }
    
    public float3 CenterOfMassOffset;
    
    public const float METERS_PER_SECOND_TO_KM_PER_HOUR = 3.6f;
}