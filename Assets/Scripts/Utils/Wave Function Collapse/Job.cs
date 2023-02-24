using Unity.Jobs;
using Unity.Collections;
using static ImagePatternSetCreator;
using UnityEngine;
using Unity.Burst;

[BurstCompile]
public struct WaveFunctionCollapseJob : IJob
{
    [ReadOnly] public NativeArray<ColorRGB> InputImage;
    [ReadOnly] public Grid InputImageGrid;
    [ReadOnly] public uint Seed;
    [ReadOnly] public int N;
    public bool Error;

    [ReadOnly] public Grid OutputGrid;
    public NativeArray<Cell> CollapsedResult;
    public NativeList<PatternIdAndColor> PatternIdByColorResult;

    public WaveFunctionCollapseJob(Texture2D image, Grid outputGrid, uint seed, int n, Allocator resultAllocation)
    {
        InputImage = image.GetPixelData<ColorRGB>(0);
        InputImageGrid = new Grid(image.width, image.height);
        OutputGrid = outputGrid;
        Seed = seed;
        N = n;
        Error = false;
        CollapsedResult = new NativeArray<Cell>(OutputGrid.Area, resultAllocation, NativeArrayOptions.UninitializedMemory);
        PatternIdByColorResult = new NativeList<PatternIdAndColor>(resultAllocation);
    }
    
    [BurstCompile]
    public void Execute()
    {
        ImagePatternSetCreator.GeneratePatternGrid(Allocator.Temp, InputImage, InputImageGrid, out var patternGrid);
        PatternSet patternSet;
        Result result;
        if(N > 1)
            result = new PatternSetCreatorNN().PatternGridToPatternSetNN(ref patternGrid, N, Allocator.Temp, out patternSet);
        else
            result = new PatternSetCreator().PatternGridToPatternSet(patternGrid, Allocator.Temp, out patternSet);
        if(result == Result.NotEnoughSpace)
        {
            Error = true;
            return;
        }
        
        PatternIdByColorResult.CopyFrom(patternGrid.PatternIdByColor);
        var waveFunctionCollapse = new WaveFunctionCollapse(OutputGrid, patternSet);
        waveFunctionCollapse.Collapse(Seed, ref CollapsedResult);
    }
}