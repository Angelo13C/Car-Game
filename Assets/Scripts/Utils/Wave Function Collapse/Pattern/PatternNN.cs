

using Unity.Collections;
using Unity.Mathematics;

public struct PatternsNN
{
    public readonly int N { get; }
    private NativeList<byte> _values;
    private NativeList<ushort> _weights;

    public int Length => _weights.Length;

    public PatternsNN(int n, Allocator allocator)
    {
        N = n;
        _values = new NativeList<byte>(N * N * 30, allocator);
        _weights = new NativeList<ushort>(N * 30, allocator);
    }
    
    public struct PatternNN
    {
        public int Index { get; }
        public PatternNN(int index)
        {
            Index = index;
        }
    }

    public PatternNN AddPattern()
    {
        _values.ResizeUninitialized(_values.Length + N * N);
        _weights.Add(1);
        return new PatternNN(Length - 1);
    }

    public void RemoveNewPatternIfAlreadyExists()
    {
        var lastPattern = new PatternNN(Length - 1);
        var lastPatternAddress = PatternAndPositionToAddress(lastPattern, new int2(0, 0));
        for(var i = 0; i < lastPattern.Index; i++)
        {
            var currentAddress = i * N * N;
            var equals = true;
            for(var j = 0; j < N * N && equals; j++)
            {
                var a = _values[lastPatternAddress + j];
                var b = _values[currentAddress + j];
                equals = _values[currentAddress + j] == _values[lastPatternAddress + j];
            }
            
            if(equals)
            {
                _values.Length -= N * N;
                _weights.Length--;
                _weights[i]++;
                return;
            }
        }
    }

    private int PatternAndPositionToAddress(PatternNN pattern, int2 position) => pattern.Index * N * N + position.x + position.y * N;
    public void SetValue(PatternNN pattern, int2 position, byte value) => _values[PatternAndPositionToAddress(pattern, position)] = value;
    public byte GetValue(PatternNN pattern, int2 position) => _values[PatternAndPositionToAddress(pattern, position)];

    public PatternNN GetPatternAtIndex(int i) => new PatternNN(i);

    public ushort GetWeight(PatternNN pattern) => _weights[pattern.Index];
}
