using System.IO;
using Unity.Mathematics;
using Unity.Collections.LowLevel.Unsafe;

// Directly copied from Unity's internal StreamBinaryWriter
// https://github.com/needle-mirror/com.unity.entities/blob/85dc36a489eb40741392d89c574c7a6013355379/Unity.Entities/Serialization/BinarySerialization.cs#L285
internal unsafe class StreamBinaryWriter : Unity.Entities.Serialization.BinaryWriter
{
    private Stream stream;
    private byte[] buffer;
    public long Position
    {
        get => stream.Position;
        set => stream.Position = value;
    }

    public StreamBinaryWriter(string fileName, int bufferSize = 65536)
    {
        stream = File.Open(fileName, FileMode.Create, FileAccess.Write);
        buffer = new byte[bufferSize];
    }

    public void Dispose()
    {
        stream.Dispose();
    }

    public void WriteBytes(void* data, int bytes)
    {
        int remaining = bytes;
        int bufferSize = buffer.Length;

        fixed (byte* fixedBuffer = buffer)
        {
            while (remaining != 0)
            {
                int bytesToWrite = math.min(remaining, bufferSize);
                UnsafeUtility.MemCpy(fixedBuffer, data, bytesToWrite);
                stream.Write(buffer, 0, bytesToWrite);
                data = (byte*) data + bytesToWrite;
                remaining -= bytesToWrite;
            }
        }
    }

    public long Length => stream.Length;
}