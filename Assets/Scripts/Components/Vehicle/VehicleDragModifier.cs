using Unity.Entities;

public struct VehicleDragModifier : IComponentData
{
    public float NormalDrag;
    
    public float MinSpeedToReduceDragSqr;
    public float SlowSpeedDrag;

    public float NotGroundedDrag;
}