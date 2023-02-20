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

    [ReadOnly] public Grid OutputGrid;
    public NativeArray<Cell> CollapsedResult;
    public NativeList<PatternIdAndColor> PatternIdByColorResult;

    public WaveFunctionCollapseJob(Texture2D image, Grid outputGrid, uint seed, Allocator resultAllocation)
    {
        InputImage = image.GetPixelData<ColorRGB>(0);
        InputImageGrid = new Grid(image.width, image.height);
        OutputGrid = outputGrid;
        Seed = seed;
        CollapsedResult = new NativeArray<Cell>(OutputGrid.Area, resultAllocation);
        PatternIdByColorResult = new NativeList<PatternIdAndColor>(resultAllocation);
    }
    
    [BurstCompile]
    public void Execute()
    {
        ImagePatternSetCreator.GeneratePatternGrid(Allocator.Temp, InputImage, InputImageGrid, out var patternGrid);
        PatternIdByColorResult.CopyFrom(patternGrid.PatternIdByColor);
        ImagePatternSetCreator.PatternGridToPatternSet(patternGrid, Allocator.Temp, out var patternSet);
        var waveFunctionCollapse = new WaveFunctionCollapse(OutputGrid, patternSet);
        waveFunctionCollapse.Collapse(Seed, ref CollapsedResult);
    }
}