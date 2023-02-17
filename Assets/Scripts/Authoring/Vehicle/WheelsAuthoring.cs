using Unity.Entities;
using UnityEngine;

public class WheelsAuthoring : MonoBehaviour
{
    [SerializeField] private Transform _body;

	class Baker : Baker<WheelsAuthoring>
	{
		public override void Bake(WheelsAuthoring authoring)
		{
			var wheels = new Wheels { 
                CarBody = GetEntity(authoring._body)
            };

			AddComponent(wheels);
		}
	}
}