using Unity.Jobs;
using Unity.Collections;
using static ImagePatternSetCreator;
using UnityEngine;

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
    
    public void Execute()
    {
        var patternGrid = ImagePatternSetCreator.GeneratePatternGrid(Allocator.Temp, InputImage, InputImageGrid);
        PatternIdByColorResult.CopyFrom(patternGrid.PatternIdByColor);
        var patternSet = ImagePatternSetCreator.PatternGridToPatternSet(patternGrid, Allocator.Temp);
        var waveFunctionCollapse = new WaveFunctionCollapse(OutputGrid, patternSet);
        waveFunctionCollapse.Collapse(Seed, ref CollapsedResult);
    }
}