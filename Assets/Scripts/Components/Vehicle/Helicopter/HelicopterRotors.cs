using Unity.Entities;

public struct HelicopterRotors : IComponentData
{
    public Entity MainRotor, TailRotor;

    public float MinYForFullSpeed;
    public float FullSpeedRotationPerSecond;
}