using System;
using Unity.Entities;

[Serializable]
[InternalBufferCapacity(7)]
public struct VehicleGear : IBufferElementData
{
    public float MinSpeed;
    public float GasForce;
}