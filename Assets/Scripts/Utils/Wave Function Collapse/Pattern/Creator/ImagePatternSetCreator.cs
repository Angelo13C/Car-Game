using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

[BurstCompile]
public struct ImagePatternSetCreator
{    
    [BurstCompile]
    public struct PatternGrid : IDisposable
    {
        public NativeArray<byte> Elements;
        public Grid Grid;
        public NativeArray<PatternIdAndColor> PatternIdByColor;

        public int2 Size => Grid.Size;

        [BurstCompile]
        public byte Get(int2 position) => Elements[Grid.GridPositionToIndex(position)];

        [BurstCompile]
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

    public struct ColorRGB : IEquatable<ColorRGB>
    {
        public byte R;
        public byte G;
        public byte B;
        public ColorRGB(byte r, byte g, byte b)
        {
            R = r;
            G = g;
            B = b;
        }

        public static implicit operator Color32(ColorRGB color) => new Color32(color.R, color.G, color.B, byte.MaxValue);
        public bool Equals(ColorRGB other) => R == other.R && G == other.G && B == other.B;
    }

    [BurstCompile]
    public void GeneratePatternGrid(Allocator allocator, out PatternGrid result)
    {
        var grid = new Grid(_image.width, _image.height);
        // For textures with alpha channel I need to use Color32 instead!!
        var pixels = _image.GetPixelData<ColorRGB>(0);

        GeneratePatternGrid(allocator, pixels, grid, out result);
    }

    [BurstCompile]
    public static void GeneratePatternGrid(Allocator allocator, in NativeArray<ColorRGB> pixelData, in Grid grid, out PatternGrid result)
    {
        var patternGridElements = new NativeArray<byte>(pixelData.Length, allocator, NativeArrayOptions.UninitializedMemory);
        var patternIdByColor = new NativeList<PatternIdAndColor>(8, Allocator.Temp);

        for(var i = 0; i < patternGridElements.Length; i++)
        {
            var index = patternIdByColor.IndexOf(pixelData[i]);
            if(index == -1)
            {
                index = patternIdByColor.Length;

                patternIdByColor.Add(new PatternIdAndColor {
                    Color = pixelData[i],
                    PatternId = new PatternId((uint) 1 << index)
                });
            }
            // I temporarily store the index instead of the actual id (which is the log2(id)) to make list lookup faster
            patternGridElements[i] = (byte) index;
        }
        
        var patternIdByColorArray = patternIdByColor.ToArray(allocator);
        patternIdByColor.Dispose();
        result = new PatternGrid { Elements = patternGridElements, Grid = grid, PatternIdByColor = patternIdByColorArray };
    }

    [BurstCompile]
    public struct PatternIdAndColor : IEquatable<ColorRGB>, IEquatable<PatternId>
    {
        public ColorRGB Color;
        public PatternId PatternId;

        [BurstCompile]
        public bool Equals(ColorRGB other) => Color.Equals(other);
        [BurstCompile]
        public bool Equals(PatternId other) => PatternId.Equals(other);
    }
}
