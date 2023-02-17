using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Unity.Collections;
using Unity.Physics.Authoring;
using Unity.Physics;

public class RoadTileMapAuthoring : MonoBehaviour
{
	[SerializeField] private Vector2 _cellSize;
	[SerializeField] private Vector2Int _gridSize;
	
	[Space]
	[SerializeField] private GameObject _roadPrefab;

	[Header("Editing")]
	[SerializeField] private bool _isEditable = true;
	[SerializeField] private PhysicsCategoryTags _placeableRoadFilter;

	class Baker : Baker<RoadTileMapAuthoring>
	{
		public override void Bake(RoadTileMapAuthoring authoring)
		{
			var gridSize = new int2(authoring._gridSize.x, authoring._gridSize.y);
			var roadTileMap = new RoadTileMapReference {
				BlobAsset = RoadTileMapReference.CreateBlobAsset(new [] { false }, gridSize, authoring._cellSize),
				NeedsRefresh = false,
				HasRefreshed = false
			};
			AddComponent(roadTileMap);
			
			var roadTileSet = new RoadTileSet {
				RoadPrefab = GetEntity(authoring._roadPrefab)
			};
			AddComponent(roadTileSet);

			if(authoring._isEditable)
			{
				var editableRoadTileMap = new EditableRoadTileMap {
					CollisionFilter = new CollisionFilter {
						BelongsTo = CollisionFilter.Default.BelongsTo,
						CollidesWith = authoring._placeableRoadFilter.Value,
						GroupIndex = CollisionFilter.Default.GroupIndex
					},
					Edit = false
				};
				AddComponent(editableRoadTileMap);
			}
		}
	}
}