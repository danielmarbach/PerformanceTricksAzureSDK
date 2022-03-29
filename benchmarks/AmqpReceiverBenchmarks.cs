// See https://aka.ms/new-console-template for more information
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Jobs;

[Config(typeof(Config))]
public class AmqpReceiverBenchmarks
{
    private IEnumerable<string> input;
    private AmqpReceiverBefore before;
    private AmqpReceiverAfterV1 afterV1;
    private AmqpReceiverAfterV2 afterV2;

    class Config : ManualConfig
    {
        public Config()
        {
            AddExporter(MarkdownExporter.GitHub);
            AddDiagnoser(MemoryDiagnoser.Default);
            AddJob(Job.ShortRun.WithUnrollFactor(2048));
        }
    }

    [Params(0, 1, 4, 8, 16, 32)]
    public int Size { get; set; }

    [Params("List", "Array", "HashSet", "Enumerable")]
    public string Collection { get; set; }

    [IterationSetup]
    public void Setup()
    {
        switch (Collection)
        {
            case "List":
                input = Enumerable.Repeat(Guid.NewGuid().ToString(), Size).ToList();
                break;
            case "Array":
                input = Enumerable.Repeat(Guid.NewGuid().ToString(), Size).ToArray();
                break;
            case "HashSet":
                input = Enumerable.Repeat(Guid.NewGuid().ToString(), Size).ToHashSet();
                break;
            case "Enumerable":
                input = Enumerable.Repeat(Guid.NewGuid().ToString(), Size).AsEnumerable();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        before = new AmqpReceiverBefore();
        afterV1 = new AmqpReceiverAfterV1();
        afterV2 = new AmqpReceiverAfterV2();
    }

    // [Benchmark(Baseline = true)]
    // public Task Before()
    // {
    //     return before.CompleteAsync(input);
    // }


    //[Benchmark]
    [Benchmark(Baseline = true)]
    public Task AfterV1()
    {
        return afterV1.CompleteAsync(input);
    }

    [Benchmark]
    public Task AfterV2()
    {
        return afterV2.CompleteAsync(input);
    }

}

public class AmqpReceiverBefore
{
    ConcurrentBag<Guid> _requestResponseLockedMessages = new();

    public Task CompleteAsync(IEnumerable<string> lockTokens) => CompleteInternalAsync(lockTokens);

    private Task CompleteInternalAsync(IEnumerable<string> lockTokens)
    {
        Guid[] lockTokenGuids = lockTokens.Select(token => new Guid(token)).ToArray();
        if (lockTokenGuids.Any(lockToken => _requestResponseLockedMessages.Contains(lockToken)))
        {
            // do special path accessing lockTokenGuids
            return Task.CompletedTask;
        }
        // do normal path accessing lockTokenGuids
        return Task.CompletedTask;
    }
}

public class AmqpReceiverAfterV1
{
    ConcurrentBag<Guid> _requestResponseLockedMessages = new();

    public Task CompleteAsync(IEnumerable<string> lockTokens) => CompleteInternalAsync(lockTokens);

    private Task CompleteInternalAsync(IEnumerable<string> lockTokens)
    {
        Guid[] lockTokenGuids = lockTokens.Select(token => new Guid(token)).ToArray();
        foreach (var tokenGuid in lockTokenGuids)
        {
            if (_requestResponseLockedMessages.Contains(tokenGuid))
            {
                return Task.CompletedTask;
            }
        }
        return Task.CompletedTask;
    }
}

public class AmqpReceiverAfterV2
{
    ConcurrentBag<Guid> _requestResponseLockedMessages = new();

    public Task CompleteAsync(IEnumerable<string> lockTokens)
    {
        IReadOnlyCollection<string> readOnlyCollection = lockTokens switch
        {
            IReadOnlyCollection<string> asReadOnlyCollection => asReadOnlyCollection,
            _ => lockTokens.ToArray(),
        };
        return CompleteInternalAsync(readOnlyCollection);
    }


    private Task CompleteInternalAsync(IReadOnlyCollection<string> lockTokens)
    {
        int count = lockTokens.Count;
        Guid[] lockTokenGuids = count == 0 ? Array.Empty<Guid>() : new Guid[count];
        Unsafe.SkipInit(out lockTokenGuids);
        var asSpan = lockTokenGuids.AsSpan();
        int index = 0;
        foreach (var token in lockTokens)
        {
            var tokenGuid = new Guid(token);
            asSpan[index++] = tokenGuid;
            if (_requestResponseLockedMessages.Contains(tokenGuid))
            {
                return Task.CompletedTask;
            }
        }
        return Task.CompletedTask;
    }
}