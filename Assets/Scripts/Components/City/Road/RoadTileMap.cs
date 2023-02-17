using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

public struct RoadTileMap
{
    public int2 GridSize;
    public float2 CellSize;
    public BlobArray<bool> PlacedRoads;

    public bool IsGridPositionValid(int2 gridPosition)
    {
        return gridPosition.x >= 0 && gridPosition.x < GridSize.x && gridPosition.y >= 0 && gridPosition.y < GridSize.y;
    }
    
    public int PositionToIndex(float2 position)
    {
        return GridPositionToIndex(PositionToGridPosition(position));
    }
    public int2 PositionToGridPosition(float2 position)
    {
        return new int2((int) math.floor(position.x / CellSize.x), (int) math.floor(position.y / CellSize.y));
    }
    public int GridPositionToIndex(int2 gridPosition)
    {
        return gridPosition.x + gridPosition.y * GridSize.x;
    }

    public float2 GridPositionToPosition(int2 gridPosition)
    {
        return new float2(gridPosition.x * CellSize.x + CellSize.x / 2, gridPosition.y * CellSize.y + CellSize.y / 2);
    }
}

public struct RoadTileMapReference : IComponentData
{
    public BlobAssetReference<RoadTileMap> BlobAsset;
    public bool NeedsRefresh;
    public bool HasRefreshed;
 
    public static BlobAssetReference<RoadTileMap> CreateBlobAsset(bool[] tileMap, int2 gridSize, float2 cellSize)
    {
        using(var builder = new BlobBuilder(Allocator.Temp))
        {
            ref var tileMapBlob = ref builder.ConstructRoot<RoadTileMap>();

            tileMapBlob.GridSize = gridSize;
            tileMapBlob.CellSize = cellSize;
            
            var placedRoadsBuilder = builder.Allocate(ref tileMapBlob.PlacedRoads, gridSize.x * gridSize.y);
            for(var i = 0; i < tileMap.Length; i++)
                placedRoadsBuilder[i] = tileMap[i];

            var blobAsset = builder.CreateBlobAssetReference<RoadTileMap>(Allocator.Persistent);
        
            return blobAsset;
        }
    }

    public static void UpdateBlobAsset(ref BlobAssetReference<RoadTileMap> tileMapBlob, int2 gridPositionToInvert)
    {
        using(var builder = new BlobBuilder(Allocator.Temp))
        {
            ref var newTileMapBlob = ref builder.ConstructRoot<RoadTileMap>();

            {
                ref var oldTileMapBlob = ref tileMapBlob.Value;
                newTileMapBlob.GridSize = oldTileMapBlob.GridSize;
                newTileMapBlob.CellSize = oldTileMapBlob.CellSize;
                
                var placedRoadsBuilder = builder.Allocate(ref newTileMapBlob.PlacedRoads, newTileMapBlob.GridSize.x * newTileMapBlob.GridSize.y);
                for(var i = 0; i < oldTileMapBlob.PlacedRoads.Length; i++)
                    placedRoadsBuilder[i] = oldTileMapBlob.PlacedRoads[i];

                var indexToInvert = newTileMapBlob.GridPositionToIndex(gridPositionToInvert);
                placedRoadsBuilder[indexToInvert] = !placedRoadsBuilder[indexToInvert];
            }

            //tileMapBlob.Dispose();
            tileMapBlob = builder.CreateBlobAssetReference<RoadTileMap>(Allocator.Persistent);
        }
    }
}