using System.IO;
using Unity.Mathematics;
using Unity.Collections.LowLevel.Unsafe;

// Copied from Unity's internal StreamBinaryReader (with some slight changes)
// https://github.com/needle-mirror/com.unity.entities/blob/85dc36a489eb40741392d89c574c7a6013355379/Unity.Entities/Serialization/BinarySerialization.cs#L209
internal unsafe class StreamBinaryReader : Unity.Entities.Serialization.BinaryReader
{
    private Stream stream;
    private byte[] buffer;
    public long Position
    {
        get => stream.Position;
        set => stream.Position = value;
    }

    public StreamBinaryReader(string filePath, long bufferSize = 65536)
    {
        stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        buffer = new byte[bufferSize];
    }

    public void Dispose()
    {
        stream.Dispose();
    }

    public void ReadBytes(void* data, int bytes)
    {
        int remaining = bytes;
        int bufferSize = buffer.Length;

        fixed(byte* fixedBuffer = buffer)
        {
            while (remaining != 0)
            {
                int read = stream.Read(buffer, 0, math.min(remaining, bufferSize));
                remaining -= read;
                UnsafeUtility.MemCpy(data, fixedBuffer, read);
                data = (byte*)data + read;
            }
        }
    }
}