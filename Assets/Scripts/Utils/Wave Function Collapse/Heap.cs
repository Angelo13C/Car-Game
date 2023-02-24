using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

[BurstCompile]
public struct EntropyCell : IHeapElement
{
    public byte Entropy;
    public int CellCoords;

    public int CustomIndex { get => CellCoords; set => CellCoords = value; }
}
[BurstCompile]
public struct EntropyCellComparator : IComparator<EntropyCell>
{
    [BurstCompile]
    public int Compare(EntropyCell a, EntropyCell b) => a.Entropy.CompareTo(b.Entropy);
}
[BurstCompile]
public struct EntropyHeap : IDisposable
{
    private NativeArrayHeap<EntropyCell, EntropyCellComparator> _heap;
     
    public EntropyHeap(int area, Allocator allocator)
    {
        _heap = new NativeArrayHeap<EntropyCell, EntropyCellComparator>(area, allocator);
    }

    [BurstCompile]
    public void Dispose()
    {
        _heap.Dispose();
    }

    [BurstCompile]
    public int GetMinEntropyCellCoords()
    {
        if(_heap.IsEmpty())
            return -1;
        else
            return _heap.RemoveFirst().CellCoords;
    }

    [BurstCompile]
    public void UpdateCoordsWithValue(int coords, Cell value)
    {
        var entropyCell = new EntropyCell { Entropy = value.GetEntropy(), CellCoords = coords };
        if(entropyCell.Entropy <= 1)
        {
            _heap.RemoveElement(entropyCell);
        }
        else
        {
            _heap.UpdateElement(entropyCell);
        }
    }
}