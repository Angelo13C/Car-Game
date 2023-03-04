using Unity.Entities;
using UnityEngine;

public class VehicleAuthoring : MonoBehaviour
{
	[SerializeField] private VehicleTag _tag;
	public VehicleTag Tag => _tag;

	public enum VehicleTag
	{
		Player,
		Citizen,
		Police
	}

	class Baker : Baker<VehicleAuthoring>
	{
		public override void Bake(VehicleAuthoring authoring)
		{
			switch(authoring._tag)
			{
				case VehicleTag.Player:
					AddComponent(new PlayerVehicleTag());
					break;
				case VehicleTag.Citizen:
					AddComponent(new CitizenVehicleTag());
					break;
				case VehicleTag.Police:
					AddComponent(new PoliceVehicleTag());
					break;
			}			
		}
	}
}