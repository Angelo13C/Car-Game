using Unity.Entities;
using UnityEngine;

public struct VehicleInputSteer : IComponentData
{
	public KeyCode RightKey;
	public KeyCode LeftKey;
}