using Unity.Burst;
using Unity.Entities;

[UpdateAfter(typeof(RoadTileMapCreatorSystem))]
[BurstCompile]
public partial struct SaveRoadTileMapSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
    }
    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }
    
    public void OnUpdate(ref SystemState state)
    {
        foreach(var (savableRoadTileMap, roadTileMap) in SystemAPI.Query<SavableRoadTileMap, RoadTileMapReference>())
        {
            if(roadTileMap.HasRefreshed)
            {
                var binaryFileWriter = new StreamBinaryWriter(savableRoadTileMap.SaveFilePath.ToString());
                binaryFileWriter.Write(roadTileMap.BlobAsset);
                binaryFileWriter.Dispose();
            }
        }
    }
}