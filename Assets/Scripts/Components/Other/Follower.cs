using Unity.Entities;

public struct Follower : IComponentData
{
    public Entity Target;

    public float PredictedTime;
}