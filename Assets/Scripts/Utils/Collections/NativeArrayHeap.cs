using System;
using Unity.Burst;
using Unity.Collections;

[BurstCompile]
public struct NativeArrayHeap<T, U>: IDisposable where T : struct where U : struct, IComparator<T>
{
    private NativeArray<T> _elements;
    private U _comparator;
    public int Length { get; private set; }
    
    [BurstCompile]
    public bool IsEmpty() => Length == 0;

    public NativeArrayHeap(int capacity, Allocator allocator, U comparator = default)
    {
        _elements = new NativeArray<T>(capacity, allocator);
        _comparator = comparator;
        Length = 0;
    }

    [BurstCompile]
    public int Add(T element)
    {
        _elements[Length] = element;
        var index = SortUp(Length);
        Length++;
        return index;
    }

    [BurstCompile]
    public T RemoveFirst()
    {
        var firstElement = _elements[0];
        RemoveElement(0);
        return firstElement;
    }

    [BurstCompile]
    public int UpdateElement(T element, int elementIndex)
    {
        _elements[elementIndex] = element;
        return SortUp(elementIndex);
        // I don't need sort down in this project since entropy is only going down, not up
        //SortDown(elementIndex);
    }

    [BurstCompile]
    private int SortUp(int elementIndex)
    {
        var parentIndex = (elementIndex - 1) / 2;
        while(true)
        {
            if(_comparator.Compare(_elements[elementIndex], _elements[parentIndex]) > 0)
            {
                Swap(elementIndex, parentIndex);
                elementIndex = parentIndex;
            }
            else
                break;

            parentIndex = (parentIndex - 1) / 2;
        }

        return elementIndex;
    }

    [BurstCompile]
    public void RemoveElement(int elementIndex)
    {
        Length--;
        _elements[elementIndex] = _elements[Length];
        SortDown(elementIndex);
    }

    [BurstCompile]
    private void SortDown(int elementIndex)
    {
        while(true)
        {
            var childIndexLeft = (elementIndex * 2) + 1;
            var childIndexRight = (elementIndex * 2) + 2;
            var swapIndex = 0;

            if(childIndexLeft < Length)
            {
                swapIndex = childIndexLeft;

                if(childIndexRight < Length)
                {
                    if(_comparator.Compare(_elements[childIndexLeft], _elements[childIndexRight]) < 0)
                        swapIndex = childIndexRight;
                }

                if(_comparator.Compare(_elements[elementIndex], _elements[swapIndex]) < 0)
                    Swap(elementIndex, swapIndex);
                else
                    return;
            }
            else
                return;
        }
    }

    [BurstCompile]
    private void Swap(int aIndex, int bIndex)
    {
        var temp = _elements[aIndex];
        _elements[aIndex] = _elements[bIndex];
        _elements[bIndex] = temp;
    }

    [BurstCompile]
    public void Clear() => Length = 0;

    [BurstCompile]
    public void Dispose() => _elements.Dispose();
}

public interface IComparator<T>
{
    abstract int Compare(T a, T b);
}