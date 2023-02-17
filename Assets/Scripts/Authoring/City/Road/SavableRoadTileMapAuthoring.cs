using Unity.Entities;
using UnityEngine;

public class SavableRoadTileMapAuthoring : MonoBehaviour
{
	[SerializeField] private string _saveFilePath;

	class Baker : Baker<SavableRoadTileMapAuthoring>
	{
		public override void Bake(SavableRoadTileMapAuthoring authoring)
		{
			var savableTileMap = new SavableRoadTileMap {
				SaveFilePath = Application.persistentDataPath + "/" + authoring._saveFilePath,
				ShouldLoad = true
			};

			AddComponent(savableTileMap);
		}
	}
}