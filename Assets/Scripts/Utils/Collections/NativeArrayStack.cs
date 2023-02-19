using System;
using Unity.Collections;

public struct NativeArrayStack<T>: IDisposable where T : struct
{
    private NativeArray<T> _elements;
    public int Length { get; private set; }

    public bool IsEmpty() => Length == 0;

    public NativeArrayStack(int capacity, Allocator allocator)
    {
        _elements = new NativeArray<T>(capacity, allocator);
        Length = 0;
    }

    public void Push(T element)
    {
        _elements[Length] = element;
        Length++;
    }
    public T Pop()
    {
        Length--;
        return _elements[Length];
    }

    public void Clear() => Length = 0;

    public void Dispose() => _elements.Dispose();
}
