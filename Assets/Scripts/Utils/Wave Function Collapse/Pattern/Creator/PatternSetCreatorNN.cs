using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;

public struct PatternSetCreatorNN
{    
    [BurstCompile]
    public void PatternGridToPatternSetNN(ref ImagePatternSetCreator.PatternGrid patternGrid, int n, Allocator allocator, out PatternSet result)
    {
        var patternsNN = new PatternsNN(n, allocator);

        for(var y = 0; y < patternGrid.Size.y; y++)
        {
            for(var x = 0; x < patternGrid.Size.x; x++)
            {
                var newPattern = patternsNN.AddPattern();
                for(var yOffset = 0; yOffset < n; yOffset++)
                {
                    for(var xOffset = 0; xOffset < n; xOffset++)
                    {
                        var gridPosition = new int2(PatternSetCreator.mod(x + xOffset, patternGrid.Size.x), PatternSetCreator.mod(y + yOffset, patternGrid.Size.y));
                        patternsNN.SetValue(newPattern, new int2(xOffset, yOffset), patternGrid.Get(gridPosition));
                    }   
                }
                patternsNN.RemoveNewPatternIfAlreadyExists();
            }
        }
        
        var newPatternIdByColor =  new NativeArray<ImagePatternSetCreator.PatternIdAndColor>(patternsNN.Length, allocator, NativeArrayOptions.UninitializedMemory);        
        var patterns = new NativeArray<Pattern>(patternsNN.Length, allocator);
        for(var i = 0; i < patterns.Length; i++)
        {
            var patternI = patternsNN.GetPatternAtIndex(i);

            newPatternIdByColor[i] = new ImagePatternSetCreator.PatternIdAndColor {
                Color = patternGrid.PatternIdByColor[patternsNN.GetValue(patternI, new int2(0, 0))].Color,
                PatternId = new PatternId((uint) 1 << i)
            };
            
            var updatedPattern = patterns[i];
            updatedPattern.Weight = patternsNN.GetWeight(patternI);
            patterns[i] = updatedPattern;

            for(var j = 0;  j < patterns.Length; j++)
            {
                if(i == j)
                    continue;
                
                var patternJ = patternsNN.GetPatternAtIndex(j);

                void Check(Direction direction)
                {
                    var compatible = true;

                    var start = direction.ToPosition();
                    for(var y = start.y; y < n && compatible; y++)
                    {
                        for(var x = start.x; x < n && compatible; x++)
                        {
                            compatible = patternsNN.GetValue(patternI, new int2(x, y)) == patternsNN.GetValue(patternJ, new int2(x - start.x, y - start.y));
                        }   
                    }
                    if(compatible)
                    {
                        var pattern = patterns[i];
                        pattern.ID = new PatternId((uint) 1 << i);
                        pattern.AddValidNeighbourInDirection(direction, new Cell { SuperPosition = (uint) 1 << j });
                        patterns[i] = pattern;

                        var otherPattern = patterns[j];
                        otherPattern.AddValidNeighbourInDirection(direction.Opposite(), new Cell { SuperPosition = pattern.ID.Value });
                        patterns[j] = otherPattern;
                    }
                }
                Check(Direction.Right);
                Check(Direction.Up);
            }
        }

        patternGrid.PatternIdByColor.Dispose();
        patternGrid.PatternIdByColor = newPatternIdByColor;
        
        result = new PatternSet(patterns);
    }
}