using System;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

public struct ImagePatternSetCreator
{    
    public struct PatternGrid : IDisposable
    {
        public NativeArray<int> Elements;
        public Grid Grid;
        public NativeArray<PatternIdAndColor> PatternIdByColor;

        public int2 Size => Grid.Size;

        public int Get(int2 position) => Elements[Grid.GridPositionToIndex(position)];

        public void Dispose()
        {
            Elements.Dispose();
            PatternIdByColor.Dispose();
        }
    }

    private Texture2D _image;

    public ImagePatternSetCreator(Texture2D image)
    {
        _image = image;
    }

    public PatternGrid GeneratePatternGrid(Allocator allocator)
    {
        var grid = new Grid(_image.width, _image.height);
        var patternGridElements = new NativeArray<int>(grid.Area, allocator);
        var patternIdByColor = new NativeList<PatternIdAndColor>(8, Allocator.Temp);
        var pixels = _image.GetPixelData<Color32>(0);

        for(var i = 0; i < patternGridElements.Length; i++)
        {
            var index = patternIdByColor.IndexOf(pixels[i]);
            if(index == -1)
            {
                index = patternIdByColor.Length;

                patternIdByColor.Add(new PatternIdAndColor {
                    Color = pixels[i],
                    PatternId = new PatternId((uint) 1 << index)
                });
            }
            // I temporarily store the index instead of the actual id (which is the log2(id)) to make list lookup faster
            patternGridElements[i] = index;
        }
        
        var patternIdByColorArray = patternIdByColor.ToArray(allocator);
        patternIdByColor.Dispose();
        return new PatternGrid { Elements = patternGridElements, Grid = grid, PatternIdByColor = patternIdByColorArray };
    }

    public PatternSet PatternGridToPatternSet(PatternGrid patternGrid, Allocator allocator)
    {
        var patterns = new NativeArray<Pattern>(patternGrid.PatternIdByColor.Length, allocator);

        for(var y = 0; y < patternGrid.Size.y; y++)
        {
            for(var x = 0; x < patternGrid.Size.x; x++)
            {
                var patternId = patternGrid.Get(new int2(x, y));

                int mod(int x, int m) {
                    var r = x % m;
                    return r < 0 ? r + m : r;
                }
                var upNeighbour = patternGrid.Get(new int2(x, mod(y - 1, patternGrid.Size.y)));
                var rightNeighbour = patternGrid.Get(new int2(mod(x + 1, patternGrid.Size.x), y));
                var downNeighbour = patternGrid.Get(new int2(x, mod(y + 1, patternGrid.Size.y)));
                var leftNeighbour = patternGrid.Get(new int2(mod(x - 1, patternGrid.Size.x), y));
                
                patterns[patternId] = new Pattern {
                    ID = new PatternId((uint) 1 << patternId),
                    Weight = patterns[patternId].Weight + 1,
                    UpValidNeighbours = patterns[patternId].UpValidNeighbours.Union(new Cell { SuperPosition = (uint) 1 << upNeighbour }),
                    RightValidNeighbours = patterns[patternId].RightValidNeighbours.Union(new Cell { SuperPosition = (uint) 1 << rightNeighbour }),
                    DownValidNeighbours = patterns[patternId].DownValidNeighbours.Union(new Cell { SuperPosition = (uint) 1 << downNeighbour }),
                    LeftValidNeighbours = patterns[patternId].LeftValidNeighbours.Union(new Cell { SuperPosition = (uint) 1 << leftNeighbour }),
                };
            }            
        }
        
        return new PatternSet(patterns);
    }

    public struct PatternIdAndColor : IEquatable<Color32>, IEquatable<PatternId>
    {
        public Color32 Color;
        public PatternId PatternId;

        public bool Equals(Color32 other) => Color.Equals(other);
        public bool Equals(PatternId other) => PatternId.Equals(other);
    }
}
