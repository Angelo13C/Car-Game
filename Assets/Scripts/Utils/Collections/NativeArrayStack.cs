using System;
using Unity.Burst;
using Unity.Collections;

[BurstCompile]
public struct NativeArrayStack<T>: IDisposable where T : struct
{
    private NativeArray<T> _elements;
    public int Length { get; private set; }

    [BurstCompile]
    public bool IsEmpty() => Length == 0;

    public NativeArrayStack(int capacity, Allocator allocator)
    {
        _elements = new NativeArray<T>(capacity, allocator);
        Length = 0;
    }

    [BurstCompile]
    public void Push(T element)
    {
        _elements[Length] = element;
        Length++;
    }
    [BurstCompile]
    public T Pop()
    {
        Length--;
        return _elements[Length];
    }

    [BurstCompile]
    public void Clear() => Length = 0;

    [BurstCompile]
    public void Dispose() => _elements.Dispose();
}
