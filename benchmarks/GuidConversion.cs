using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Jobs;

[Config(typeof(Config))]
public class GuidConversion
{
    private ArraySegment<byte> data;

    private class Config : ManualConfig
    {
        public Config()
        {
            AddExporter(MarkdownExporter.GitHub);
            AddDiagnoser(MemoryDiagnoser.Default);
            AddJob(Job.Default.WithInvocationCount(25600000));
        }
    }
    [IterationSetup]
    public void Setup()
    {
        data = new ArraySegment<byte>(Guid.NewGuid().ToByteArray());
    }

    [Benchmark(Baseline = true)]
    public Guid BufferAndBlockCopy()
    {
        var guidBuffer = new byte[16];
        Buffer.BlockCopy(data.Array, data.Offset, guidBuffer, 0, 16);
        return new Guid(guidBuffer);
    }

    [Benchmark]
    public Guid BufferPool()
    {
        byte[] guidBuffer =  ArrayPool<byte>.Shared.Rent(16);
        Buffer.BlockCopy(data.Array, data.Offset, guidBuffer, 0, 16);
        var lockTokenGuid = new Guid(guidBuffer);
        ArrayPool<byte>.Shared.Return(guidBuffer);
        return lockTokenGuid;
    }

    [Benchmark]
    public Guid StackallocWithGuid()
    {
        Span<byte> guidBytes = stackalloc byte[16];
        data.AsSpan().CopyTo(guidBytes);
        return new Guid(guidBytes);
    }

    [Benchmark]
    [SkipLocalsInit]
    public Guid StackallocWithGuidAndLocalsInit()
    {
        Span<byte> guidBytes = stackalloc byte[16];
        data.AsSpan().CopyTo(guidBytes);
        return new Guid(guidBytes);
    }

    [Benchmark]
    public Guid StackallocWithMemoryMarshal()
    {
        Span<byte> guidBytes = stackalloc byte[16];
        data.AsSpan().CopyTo(guidBytes);
        if (!MemoryMarshal.TryRead<Guid>(guidBytes, out var lockTokenGuid))
        {
            lockTokenGuid = new Guid(guidBytes.ToArray());
        }
        return lockTokenGuid;
    }
}