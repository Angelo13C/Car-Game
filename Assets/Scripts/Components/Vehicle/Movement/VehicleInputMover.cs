using Unity.Entities;
using UnityEngine;

public struct VehicleInputMover : IComponentData
{
	public KeyCode GasKey;
	public KeyCode BrakeKey;
}