using System;
using Unity.Burst;
using Unity.Collections;

[BurstCompile]
public struct NativeArrayStack<T>: IDisposable where T : unmanaged
{
    private NativeList<T> _elements;
    public int Length => _elements.Length;

    [BurstCompile]
    public bool IsEmpty() => Length == 0;

    public NativeArrayStack(int capacity, Allocator allocator)
    {
        _elements = new NativeList<T>(capacity, allocator);
    }

    [BurstCompile]
    public void Push(T element)
    {
        _elements.Add(element);
    }
    [BurstCompile]
    public T Pop()
    {
        var value = _elements[Length - 1];
        _elements.RemoveAt(Length - 1);
        return value;
    }

    [BurstCompile]
    public void Clear() => _elements.Clear();

    [BurstCompile]
    public void Dispose() => _elements.Dispose();
}
