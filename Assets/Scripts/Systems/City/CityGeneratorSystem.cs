using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
public partial struct CityGeneratorSystem : ISystem, ISystemStartStop
{
    private RefRW<BeginSimulationEntityCommandBufferSystem.Singleton> _entityCommandBufferSystem;
    [ReadOnly] private ComponentLookup<LocalTransform> _localTransformLookup;

    [BurstCompile]
    public void OnStartRunning(ref SystemState state)
    {
        _localTransformLookup = SystemAPI.GetComponentLookup<LocalTransform>(false);
        _entityCommandBufferSystem = SystemAPI.GetSingletonRW<BeginSimulationEntityCommandBufferSystem.Singleton>();
    }

    public void OnStopRunning(ref SystemState state)
    {
    }

    public void OnUpdate(ref SystemState state)
    {
        foreach(var (cityGenerator, placeableObjects) in SystemAPI.Query<CityGenerator, DynamicBuffer<CityPlaceableObject>>())
        {
            if(cityGenerator.Generate)
            {
                cityGenerator.Generate = false;
                
                _localTransformLookup.Update(ref state);

                var entityCommandBuffer = _entityCommandBufferSystem.ValueRW.CreateCommandBuffer(state.WorldUnmanaged);
                var job = new GenerateCityJob(cityGenerator.InputImage, new Grid(cityGenerator.CitySize), cityGenerator.Seed, 
                    cityGenerator.N, cityGenerator.CellSize, cityGenerator.RoadColor, entityCommandBuffer, placeableObjects.ToNativeArray(Allocator.TempJob),
                    _localTransformLookup);
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
	public float2 _cellSize;
    private uint _seed;
    
    [ReadOnly] private ComponentLookup<LocalTransform> _localTransformLookup;

    public GenerateCityJob(Texture2D image, Grid outputGrid, uint seed, int n, float2 cellSize, Color32 roadColor, EntityCommandBuffer entityCommandBuffer, NativeArray<CityPlaceableObject> placeableObjects, ComponentLookup<LocalTransform> localTransformLookup)
    {
        _waveFunctionCollapseJob = new WaveFunctionCollapseJob(image, outputGrid, seed, n, Allocator.TempJob);
        _streetColor = roadColor;
        _seed = seed;
        _cellSize = cellSize;
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
        
        var rng = new Unity.Mathematics.Random(_seed);
        var grid = _waveFunctionCollapseJob.OutputGrid;
        var straightStreetObject = _placeableObjects[0];
        var housesObjects = _placeableObjects.GetSubArray(1, 2);
        for(var i = 0; i < _waveFunctionCollapseJob.CollapsedResult.Length; i++)
        {
            var cellSuperposition = _waveFunctionCollapseJob.CollapsedResult[i].SuperPosition;
            var cellIndex = _waveFunctionCollapseJob.PatternIdByColorResult.IndexOf(new PatternId(cellSuperposition));
            var cellColor = _waveFunctionCollapseJob.PatternIdByColorResult[cellIndex].Color;
            
            var cellGridPosition = grid.IndexToGridPosition(i);
            var cellPosition = new float2(cellGridPosition.x * _cellSize.x, cellGridPosition.y * _cellSize.y);
            var entityToSpawn = Entity.Null;
            if(cellColor == _streetColor)
            {
                entityToSpawn = straightStreetObject.Prefab;
            }
            else
            {
                var randomNumber = rng.NextInt(housesObjects.Length);
                var houseObject = housesObjects[randomNumber];
                entityToSpawn = houseObject.Prefab;
            }

            if(entityToSpawn != Entity.Null)
            {
                var spawnedEntity = _entityCommandBuffer.Instantiate(entityToSpawn);

                if(_localTransformLookup.TryGetComponent(entityToSpawn, out var localTransform))
                {
                    localTransform.Position = new float3(cellPosition.x, 0.01f, cellPosition.y);
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