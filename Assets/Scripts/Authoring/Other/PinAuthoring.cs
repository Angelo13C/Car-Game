using Unity.Entities;
using UnityEngine;

public class PinAuthoring : MonoBehaviour
{
	[SerializeField] private Transform _pinnedObject;

	class Baker : Baker<PinAuthoring>
	{
		public override void Bake(PinAuthoring authoring)
		{
			var pin = new Pin {
				PinnedEntity = GetEntity(authoring._pinnedObject)
			};

			AddComponent(pin);
		}
	}
}