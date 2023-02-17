public struct Range<T>
{
    public T Start { get; private set; }
    public T End { get; private set; }

    public Range(T start, T end)
    {
        Start = start;
        End = end;
    }
}