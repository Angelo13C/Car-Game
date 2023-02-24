using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;

public enum Result
{
    Ok,
    NotEnoughSpace,
}

public struct PatternSetCreator
{
    public static int mod(int x, int m) {
        var r = x % m;
        return r < 0 ? r + m : r;
    }

    [BurstCompile]
    public Result PatternGridToPatternSet(in ImagePatternSetCreator.PatternGrid patternGrid, Allocator allocator, out PatternSet result)
    {
        var patterns = new NativeArray<Pattern>(patternGrid.PatternIdByColor.Length, allocator);

        for(var y = 0; y < patternGrid.Size.y; y++)
        {
            for(var x = 0; x < patternGrid.Size.x; x++)
            {
                var patternId = patternGrid.Get(new int2(x, y));

                if(patternId >= sizeof(ulong) * 8)
                {
                    result = new PatternSet();
                    return Result.NotEnoughSpace;
                }

                var upNeighbour = patternGrid.Get(new int2(x, mod(y - 1, patternGrid.Size.y)));
                var rightNeighbour = patternGrid.Get(new int2(mod(x + 1, patternGrid.Size.x), y));
                var downNeighbour = patternGrid.Get(new int2(x, mod(y + 1, patternGrid.Size.y)));
                var leftNeighbour = patternGrid.Get(new int2(mod(x - 1, patternGrid.Size.x), y));
                
                patterns[patternId] = new Pattern {
                    ID = new PatternId((ulong) 1 << patternId),
                    Weight = patterns[patternId].Weight + 1,
                    UpValidNeighbours = patterns[patternId].UpValidNeighbours.Union(new Cell { SuperPosition = (ulong) 1 << upNeighbour }),
                    RightValidNeighbours = patterns[patternId].RightValidNeighbours.Union(new Cell { SuperPosition = (ulong) 1 << rightNeighbour }),
                    DownValidNeighbours = patterns[patternId].DownValidNeighbours.Union(new Cell { SuperPosition = (ulong) 1 << downNeighbour }),
                    LeftValidNeighbours = patterns[patternId].LeftValidNeighbours.Union(new Cell { SuperPosition = (ulong) 1 << leftNeighbour }),
                };
            }            
        }
        
        result = new PatternSet(patterns);

        return Result.Ok;
    }
}