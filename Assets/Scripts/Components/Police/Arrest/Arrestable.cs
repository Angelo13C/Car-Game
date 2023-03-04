using Unity.Entities;

public struct Arrestable : IComponentData
{
    public float CurrentPoints;
    public int RequiredPointsToArrest;

    public float RemovedPointsOverSecond;
}