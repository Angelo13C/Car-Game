using Unity.Entities;
using UnityEngine;

public class RoadTileAuthoring : MonoBehaviour
{
	class Baker : Baker<RoadTileAuthoring>
	{
		public override void Bake(RoadTileAuthoring authoring)
		{
			AddComponent(new RoadTile());
		}
	}
}