using System.IO;
using Unity.Entities;
using UnityEngine;

public class SavableRoadTileMapAuthoring : MonoBehaviour
{
	[SerializeField] private string _saveFilePath;
	
	private string ActualSaveFilePath => Application.persistentDataPath + "/" + _saveFilePath;

	[ContextMenu("Delete save file")]
	private void DeleteSaveFile()
	{
		if(File.Exists(ActualSaveFilePath))
			File.Delete(ActualSaveFilePath);
	}

	class Baker : Baker<SavableRoadTileMapAuthoring>
	{
		public override void Bake(SavableRoadTileMapAuthoring authoring)
		{
			var savableTileMap = new SavableRoadTileMap {
				SaveFilePath = authoring.ActualSaveFilePath,
				ShouldLoad = true
			};

			AddComponent(savableTileMap);
		}
	}
}