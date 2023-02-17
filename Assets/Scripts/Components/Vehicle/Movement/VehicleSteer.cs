using Unity.Entities;

public struct VehicleSteer : IComponentData
{
    public float Force;
    public float CurrentSteer;
}