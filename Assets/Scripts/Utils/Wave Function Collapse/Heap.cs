using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

[BurstCompile]
public struct EntropyCell
{
    public byte Entropy;
    public int CellCoords;
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
    // A value of -1 indicates that the cell with those coords isn't in the heap
    private NativeArray<int> _indexOfEntropyCellByCoords;
     
    public EntropyHeap(int area, Allocator allocator)
    {
        _heap = new NativeArrayHeap<EntropyCell, EntropyCellComparator>(area, allocator);
        _indexOfEntropyCellByCoords = new NativeArray<int>(area, allocator, NativeArrayOptions.UninitializedMemory);
        // Set every index to -1
        unsafe { UnsafeUtility.MemSet(_indexOfEntropyCellByCoords.GetUnsafePtr(), 0xff, _indexOfEntropyCellByCoords.Length * UnsafeUtility.SizeOf<int>()); }
    }

    [BurstCompile]
    public void Dispose()
    {
        _heap.Dispose();
        _indexOfEntropyCellByCoords.Dispose();
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
        var index = _indexOfEntropyCellByCoords[coords];
        if(index != -1 && entropyCell.Entropy <= 1)
        {
            _heap.RemoveElement(index);
            _indexOfEntropyCellByCoords[coords] = -1;
        }
        else
        {
            if(index == -1)
            {
                _indexOfEntropyCellByCoords[coords] = _heap.Add(entropyCell);
            }
            else
            {
                _indexOfEntropyCellByCoords[coords] = _heap.UpdateElement(entropyCell, index);
            }
        }
    }
}