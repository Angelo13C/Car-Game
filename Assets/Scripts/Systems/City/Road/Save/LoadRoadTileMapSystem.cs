using Unity.Burst;
using Unity.Entities;
using System.IO;

[UpdateBefore(typeof(RoadTileMapEditorInputSystem))]
[BurstCompile]
public partial struct LoadRoadTileMapSystem : ISystem
{    
    public void OnUpdate(ref SystemState state)
    {
        foreach(var (savableRoadTileMap, roadTileMap) in SystemAPI.Query<RefRW<SavableRoadTileMap>, RefRW<RoadTileMapReference>>())
        {
            if(savableRoadTileMap.ValueRO.ShouldLoad)
            {
                savableRoadTileMap.ValueRW.ShouldLoad = false;

                var path = savableRoadTileMap.ValueRO.SaveFilePath.ToString();
                if(File.Exists(path))
                {
                    var binaryFileReader = new StreamBinaryReader(path);
                    roadTileMap.ValueRW.BlobAsset = binaryFileReader.Read<RoadTileMap>();
                    binaryFileReader.Dispose();

                    roadTileMap.ValueRW.NeedsRefresh = true;
                }
            }
        }
    }
}