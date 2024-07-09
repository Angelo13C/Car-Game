using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;

public class HelicopterRotorsAuthoring : MonoBehaviour
{
	[SerializeField] private GameObject _mainRotor, _tailRotor;

	[SerializeField] [Range(0, 200)] private float _minYForFullSpeed;
	[SerializeField] [Min(0)] private float _fullSpeedRotationPerSecond;

	class Baker : Baker<HelicopterRotorsAuthoring>
	{
		public override void Bake(HelicopterRotorsAuthoring authoring)
		{
			var helicopterRotors = new HelicopterRotors {
				MainRotor = GetEntity(authoring._mainRotor),
				TailRotor = GetEntity(authoring._tailRotor),
				FullSpeedRotationPerSecond = math.radians(authoring._fullSpeedRotationPerSecond),
				MinYForFullSpeed = authoring._minYForFullSpeed
			};

			AddComponent(helicopterRotors);
		}
	}
}