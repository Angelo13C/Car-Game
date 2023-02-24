using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

[BurstCompile]
public struct NativeArrayHeap<T, U>: IDisposable where T : struct, IHeapElement where U : struct, IComparator<T>
{
    private NativeArray<T> _elements;
    
    // A value of -1 indicates that the element with that custom index isn't in the heap
    private NativeArray<int> _elementIndexByCustomIndex;
     
    private U _comparator;
    public int Length { get; private set; }
    
    [BurstCompile]
    public bool IsEmpty() => Length == 0;

    public NativeArrayHeap(int capacity, Allocator allocator, U comparator = default)
    {
        _elements = new NativeArray<T>(capacity, allocator, NativeArrayOptions.UninitializedMemory);
        _comparator = comparator;
        Length = 0;
        
        _elementIndexByCustomIndex = new NativeArray<int>(capacity, allocator, NativeArrayOptions.UninitializedMemory);
        // Set every index to -1
        unsafe { UnsafeUtility.MemSet(_elementIndexByCustomIndex.GetUnsafePtr(), 0xff, _elementIndexByCustomIndex.Length * UnsafeUtility.SizeOf<int>()); }
    }

    [BurstCompile]
    public bool Contains(T element) => _elementIndexByCustomIndex[element.CustomIndex] != -1;

    [BurstCompile]
    public void Add(T element)
    {
        _elementIndexByCustomIndex[element.CustomIndex] = Length;
        _elements[Length] = element;
        SortUp(element);
        Length++;
    }

    [BurstCompile]
    public T RemoveFirst()
    {
        var firstElement = _elements[0];
        RemoveElement(firstElement);
        return firstElement;
    }

    [BurstCompile]
    public void UpdateElement(T element)
    {
        if(!Contains(element))
            Add(element);
        else
        {
            _elements[_elementIndexByCustomIndex[element.CustomIndex]] = element;
            SortUp(element);
            // I don't need sort down in this project since entropy is only going down, not up
            //SortDown(elementIndex);
        }
    }

    [BurstCompile]
    private void SortUp(T element)
    {
        var parentIndex = (_elementIndexByCustomIndex[element.CustomIndex] - 1) / 2;
        while(true)
        {
            var parentElement = _elements[parentIndex];
            if(_comparator.Compare(element, parentElement) > 0)
            {
                Swap(element, parentElement);
                element = parentElement;
            }
            else
                break;

            parentIndex = (parentIndex - 1) / 2;
        }
    }

    [BurstCompile]
    public void RemoveElement(T element)
    {
        if(!Contains(element))
            return;
            
        Length--;
        Swap(element, _elements[Length]);
        SortDown(_elements[_elementIndexByCustomIndex[element.CustomIndex]]);
        _elementIndexByCustomIndex[element.CustomIndex] = -1;
    }

    [BurstCompile]
    private void SortDown(T element)
    {
        while(true)
        {
            var elementIndex = _elementIndexByCustomIndex[element.CustomIndex];
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

                if(_comparator.Compare(element, _elements[swapIndex]) < 0)
                {
                    Swap(element, _elements[swapIndex]);
                    element = _elements[swapIndex];
                }
                else
                    return;
            }
            else
                return;
        }
    }

    [BurstCompile]
    private void Swap(T a, T b)
    {
        var aIndex = _elementIndexByCustomIndex[a.CustomIndex];
        var bIndex = _elementIndexByCustomIndex[b.CustomIndex];
        _elements[aIndex] = b;
        _elements[bIndex] = a;

        _elementIndexByCustomIndex[a.CustomIndex] = bIndex;
        _elementIndexByCustomIndex[b.CustomIndex] = aIndex;
    }

    [BurstCompile]
    public void Clear() => Length = 0;

    [BurstCompile]
    public void Dispose() => _elements.Dispose();
}

public interface IHeapElement
{
    public int CustomIndex { get; set; }
}

public interface IComparator<T>
{
    abstract int Compare(T a, T b);
}