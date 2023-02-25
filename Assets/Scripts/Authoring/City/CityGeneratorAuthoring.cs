using Unity.Entities;
using UnityEngine;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Jobs;
#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
#endif

public class CityGeneratorAuthoring : MonoBehaviour
{
    [SerializeField] private Texture2D _inputCityImage;
	public Texture2D InputCityImage => _inputCityImage;
	
	[SerializeField] private Vector2Int _roadCellPosition;

	[SerializeField] private Vector2 _cellSize = new Vector2(5, 5);

	[SerializeField] private Vector2Int _citySize = new Vector2Int(50, 50);
	public Vector2Int CitySize => _citySize;

	[Range(1, 5)] public int N = 1;

	[Min(1)] public uint Seed = 1;

	[System.Serializable]
	private struct CityPlaceableObjectAuthoring
	{
		public GameObject Prefab;
		public Vector2Int Size;
		public CityObjectType Type;
	}
	[SerializeField] private CityPlaceableObjectAuthoring[] _cityPlaceableObject;

	class Baker : Baker<CityGeneratorAuthoring>
	{
		public override void Bake(CityGeneratorAuthoring authoring)
		{
			var roadColor = authoring._inputCityImage.GetPixel(authoring._roadCellPosition.x, authoring._roadCellPosition.y);
            var cityGenerator = new CityGenerator {
                InputImage = authoring._inputCityImage,
				CellSize = authoring._cellSize,
				CitySize = new int2(authoring.CitySize.x, authoring.CitySize.y),
				Seed = authoring.Seed,
				N = authoring.N,
				RoadColor = roadColor,
                Generate = true,
            };

			AddComponentObject(cityGenerator);

			var cityPlaceableObjects = AddBuffer<CityPlaceableObject>();
			cityPlaceableObjects.ResizeUninitialized(authoring._cityPlaceableObject.Length);
			for(var i = 0; i < authoring._cityPlaceableObject.Length; i++)
			{
				var currentObject = authoring._cityPlaceableObject[i];
				cityPlaceableObjects[i] = new CityPlaceableObject {
					Prefab = GetEntity(currentObject.Prefab),
					Size = new int2(currentObject.Size.x, currentObject.Size.y),
					Type = currentObject.Type
				};
			}
		}
	}
}

#if UNITY_EDITOR
[CustomEditor(typeof(CityGeneratorAuthoring))]
public class CityGeneratorAuthoringEditor : Editor 
{
	private Dictionary<CityGeneratorAuthoring, Texture2D> _textureByCityGenerator = new Dictionary<CityGeneratorAuthoring, Texture2D>();
	private Dictionary<CityGeneratorAuthoring, (WaveFunctionCollapseJob, JobHandle)> _collapseJobByCityGenerator = new Dictionary<CityGeneratorAuthoring, (WaveFunctionCollapseJob, JobHandle)>();
	
    public override void OnInspectorGUI()
    {
		base.OnInspectorGUI();

        var cityGeneratorAuthoring = (CityGeneratorAuthoring) target;
        if (cityGeneratorAuthoring.InputCityImage == null)
			return;

		GUILayout.Space(20);
		GUILayout.BeginHorizontal();
        GUILayout.Label("", GUILayout.Height(150), GUILayout.Width(150));
        GUI.DrawTexture(GUILayoutUtility.GetLastRect(), cityGeneratorAuthoring.InputCityImage, ScaleMode.ScaleToFit);
		Texture2D resultTexture;
		if(GUI.changed)
		{
			var grid = new Grid(cityGeneratorAuthoring.CitySize.x, cityGeneratorAuthoring.CitySize.y);
			var job = new WaveFunctionCollapseJob(cityGeneratorAuthoring.InputCityImage, grid, 
				cityGeneratorAuthoring.Seed, cityGeneratorAuthoring.N, Allocator.Persistent);
			var jobHandle = job.Schedule();
			_collapseJobByCityGenerator[cityGeneratorAuthoring] = (job, jobHandle);
		}
		if(_collapseJobByCityGenerator.TryGetValue(cityGeneratorAuthoring, out var collapseJob))
		{
			if(collapseJob.Item2.IsCompleted)
			{
				_collapseJobByCityGenerator.Remove(cityGeneratorAuthoring);
				collapseJob.Item2.Complete();
				if(collapseJob.Item1.Error)
				{
					UnityEngine.Debug.LogError("There was an error in the wave function collapse");
					return;
				}
				
				var result = collapseJob.Item1.CollapsedResult;
				var patternIdByColor = collapseJob.Item1.PatternIdByColorResult;
				var resultTexturePixels = new NativeArray<Color32>(result.Length, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
				for(var i = 0; i < result.Length; i++)
				{
					var colorIndex = patternIdByColor.IndexOf(new PatternId(result[i].SuperPosition));
					resultTexturePixels[i] =  patternIdByColor[colorIndex].Color;
				}
				
				if(_textureByCityGenerator.TryGetValue(cityGeneratorAuthoring, out resultTexture))
				{
					if(new Vector2(resultTexture.width, resultTexture.height) != cityGeneratorAuthoring.CitySize)
					{
						_textureByCityGenerator.Remove(cityGeneratorAuthoring);
						DestroyImmediate(resultTexture);
					}
				}

				if(!_textureByCityGenerator.TryGetValue(cityGeneratorAuthoring, out resultTexture))
				{
					resultTexture = new Texture2D(cityGeneratorAuthoring.CitySize.x, cityGeneratorAuthoring.CitySize.y);
					resultTexture.filterMode = FilterMode.Point;
					
					_textureByCityGenerator.Add(cityGeneratorAuthoring, resultTexture);
				}
				resultTexture.SetPixelData(resultTexturePixels, 0);
				resultTexture.Apply();

				result.Dispose();
				patternIdByColor.Dispose();
			}
		}
		
		GUILayout.Label("", GUILayout.Height(50), GUILayout.Width(50));
		GUI.DrawTexture(GUILayoutUtility.GetLastRect(), EditorGUIUtility.IconContent("PlayButton").image, ScaleMode.ScaleToFit);
		
		if(_textureByCityGenerator.TryGetValue(cityGeneratorAuthoring, out resultTexture))
		{
			GUILayout.Label("", GUILayout.Height(150), GUILayout.Width(150));
			GUI.DrawTexture(GUILayoutUtility.GetLastRect(), resultTexture, ScaleMode.ScaleToFit);
		}

		GUILayout.EndHorizontal();
    }
}
#endif