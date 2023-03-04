using Unity.Entities;
using UnityEngine;

public class VehicleAuthoring : MonoBehaviour
{
	[SerializeField] private Tag _tag;

	private enum Tag
	{
		Player,
		AI,
		Police
	}

	class Baker : Baker<VehicleAuthoring>
	{
		public override void Bake(VehicleAuthoring authoring)
		{
			switch(authoring._tag)
			{
				case Tag.Player:
					AddComponent(new PlayerVehicleTag());
					break;
				case Tag.AI:
					AddComponent(new AIVehicleTag());
					break;
				case Tag.Police:
					AddComponent(new PoliceVehicleTag());
					break;
			}			
		}
	}
}