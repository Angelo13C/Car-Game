using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
public partial struct CityGeneratorSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        foreach(var (cityGenerator, placeableObjects, entity) in SystemAPI.Query<CityGenerator, DynamicBuffer<CityPlaceableObject>>().WithEntityAccess())
        {
            if(cityGenerator.Generate)
            {
                cityGenerator.Generate = false;
                    
                var entityCommandBufferSystem = SystemAPI.GetSingletonRW<BeginSimulationEntityCommandBufferSystem.Singleton>();
                var entityCommandBuffer = entityCommandBufferSystem.ValueRW.CreateCommandBuffer(state.WorldUnmanaged);
                var job = new GenerateCityJob(cityGenerator.InputImage, new Grid(cityGenerator.CitySize), cityGenerator.Seed, 
                    cityGenerator.N, cityGenerator.CellSize, cityGenerator.StreetColor, cityGenerator.StreetsPrefabs, entity, 
                    entityCommandBuffer, placeableObjects.ToNativeArray(Allocator.TempJob),
                    SystemAPI.GetComponentLookup<LocalTransform>(true));
                state.Dependency = job.Schedule();
                job.Dispose(state.Dependency);
            }
        }
    }
}


[BurstCompile]
public struct GenerateCityJob : IJob, IDisposable, INativeDisposable
{
    private WaveFunctionCollapseJob _waveFunctionCollapseJob;
    private Color32 _streetColor;
    private EntityCommandBuffer _entityCommandBuffer;
    private NativeArray<CityPlaceableObject> _placeableObjects;
    private Entity _cityGeneratorEntity;
    private StreetsPrefabs _streetsPrefabs;
	private float2 _cellSize;
    private uint _seed;
    
    [ReadOnly] private ComponentLookup<LocalTransform> _localTransformLookup;

    public GenerateCityJob(Texture2D image, Grid outputGrid, uint seed, int n, float2 cellSize, Color32 roadColor, StreetsPrefabs streetsPrefabs, Entity cityGeneratorEntity, EntityCommandBuffer entityCommandBuffer, NativeArray<CityPlaceableObject> placeableObjects, ComponentLookup<LocalTransform> localTransformLookup)
    {
        _waveFunctionCollapseJob = new WaveFunctionCollapseJob(image, outputGrid, seed, n, Allocator.TempJob);
        _streetColor = roadColor;
        _seed = seed;
        _cellSize = cellSize;
        _streetsPrefabs = streetsPrefabs;
        _cityGeneratorEntity = cityGeneratorEntity;
        _entityCommandBuffer = entityCommandBuffer;
        _placeableObjects = placeableObjects;
        _localTransformLookup = localTransformLookup;
    }
    
    [BurstCompile]
    public void Execute()
    {
        _waveFunctionCollapseJob.Execute();

        if(_waveFunctionCollapseJob.Error)
            return;
            
        var grid = _waveFunctionCollapseJob.OutputGrid;
        
        var streets = _entityCommandBuffer.AddBuffer<StreetTile>(_cityGeneratorEntity);
        streets.ResizeUninitialized(grid.Area);
        for(var i = 0; i < streets.Length; i++)
        {            
            var cellSuperposition = _waveFunctionCollapseJob.CollapsedResult[i].SuperPosition;
            var cellIndex = _waveFunctionCollapseJob.PatternIdByColorResult.IndexOf(new PatternId(cellSuperposition));
            var cellColor = _waveFunctionCollapseJob.PatternIdByColorResult[cellIndex].Color;
            streets[i] = new StreetTile { IsStreet = cellColor == _streetColor };
        }
        
        var halfCellSize = _cellSize / 2;
        var rng = new Unity.Mathematics.Random(_seed);
        var housesObjects = _placeableObjects.GetSubArray(0, 2);
        for(var i = 0; i < _waveFunctionCollapseJob.CollapsedResult.Length; i++)
        {
            var cellGridPosition = grid.IndexToGridPosition(i);
            var cellPosition = new float2(cellGridPosition.x * _cellSize.x, cellGridPosition.y * _cellSize.y) + halfCellSize;
            var entityToSpawn = new ObjectSpawnData(Entity.Null, Direction.Down);
            if(streets[i].IsStreet)
            {
                var adjacency = Adjacency.Empty;

                bool IsStreet(int2 gridPosition, int2 offset) => grid.IsGridPositionValid(gridPosition + offset) && streets[grid.GridPositionToIndex(gridPosition + offset)].IsStreet;
                
                if(IsStreet(cellGridPosition, new int2(1, 0)))
                    adjacency |= Adjacency.Right;
                if(IsStreet(cellGridPosition, new int2(-1, 0)))
                    adjacency |= Adjacency.Left;
                if(IsStreet(cellGridPosition, new int2(0, 1)))
                    adjacency |= Adjacency.Up;
                if(IsStreet(cellGridPosition, new int2(0, -1)))
                    adjacency |= Adjacency.Down;
                
                if(adjacency == (Adjacency.Down | Adjacency.Up))
                    entityToSpawn = new ObjectSpawnData(_streetsPrefabs.StraightStreetPrefab, Direction.Up);
                else if(adjacency == (Adjacency.Left | Adjacency.Right))
                    entityToSpawn = new ObjectSpawnData(_streetsPrefabs.StraightStreetPrefab, Direction.Right);
                else if(adjacency == Adjacency.Down)
                    entityToSpawn = new ObjectSpawnData(_streetsPrefabs.DeadEndStreetPrefab, Direction.Down);
                else if(adjacency == Adjacency.Up)
                    entityToSpawn = new ObjectSpawnData(_streetsPrefabs.DeadEndStreetPrefab, Direction.Up);
                else if(adjacency == Adjacency.Right)
                    entityToSpawn = new ObjectSpawnData(_streetsPrefabs.DeadEndStreetPrefab, Direction.Left);
                else if(adjacency == Adjacency.Left)
                    entityToSpawn = new ObjectSpawnData(_streetsPrefabs.DeadEndStreetPrefab, Direction.Right);
                if(adjacency == (Adjacency.Down | Adjacency.Up | Adjacency.Left | Adjacency.Right))
                    entityToSpawn = new ObjectSpawnData(_streetsPrefabs.CrossStreetPrefab, Direction.Right);
                if(adjacency == (Adjacency.Down | Adjacency.Up | Adjacency.Right))
                    entityToSpawn = new ObjectSpawnData(_streetsPrefabs.IntersectionStreetPrefab, Direction.Up);
                if(adjacency == (Adjacency.Down | Adjacency.Up | Adjacency.Left))
                    entityToSpawn = new ObjectSpawnData(_streetsPrefabs.IntersectionStreetPrefab, Direction.Down);
                if(adjacency == (Adjacency.Left | Adjacency.Right | Adjacency.Up))
                    entityToSpawn = new ObjectSpawnData(_streetsPrefabs.IntersectionStreetPrefab, Direction.Right);
                if(adjacency == (Adjacency.Left | Adjacency.Right | Adjacency.Down))
                    entityToSpawn = new ObjectSpawnData(_streetsPrefabs.IntersectionStreetPrefab, Direction.Left);
                if(adjacency == (Adjacency.Right | Adjacency.Up))
                    entityToSpawn = new ObjectSpawnData(_streetsPrefabs.CurveStreetPrefab, Direction.Right);
                if(adjacency == (Adjacency.Right | Adjacency.Down))
                    entityToSpawn = new ObjectSpawnData(_streetsPrefabs.CurveStreetPrefab, Direction.Up);
                if(adjacency == (Adjacency.Left | Adjacency.Down))
                    entityToSpawn = new ObjectSpawnData(_streetsPrefabs.CurveStreetPrefab, Direction.Left);
                if(adjacency == (Adjacency.Left | Adjacency.Up))
                    entityToSpawn = new ObjectSpawnData(_streetsPrefabs.CurveStreetPrefab, Direction.Down);
            }
            else
            {
                var randomNumber = rng.NextInt(housesObjects.Length);
                var houseObject = housesObjects[randomNumber];
                entityToSpawn = new ObjectSpawnData(houseObject.Prefab, Direction.Down);
            }

            if(entityToSpawn.Prefab != Entity.Null)
            {
                var spawnedEntity = _entityCommandBuffer.Instantiate(entityToSpawn.Prefab);

                if(_localTransformLookup.TryGetComponent(entityToSpawn.Prefab, out var localTransform))
                {
                    localTransform.Position = new float3(cellPosition.x, 0.01f, cellPosition.y);
                    localTransform = localTransform.RotateY(entityToSpawn.Direction.ToAngle());
                    _entityCommandBuffer.SetComponent(spawnedEntity, localTransform);
                }
            }
        }
    }

    public void Dispose()
    {
        _waveFunctionCollapseJob.Dispose();
        _placeableObjects.Dispose();
    }

    public JobHandle Dispose(JobHandle inputDeps)
    {
        _waveFunctionCollapseJob.Dispose(inputDeps);
        _placeableObjects.Dispose(inputDeps);
        return inputDeps;
    }
}

public struct ObjectSpawnData
{
    public Entity Prefab;
    public Direction Direction;

    public ObjectSpawnData(Entity prefab, Direction direction)
    {
        Prefab = prefab;
        Direction = direction;
    }
}