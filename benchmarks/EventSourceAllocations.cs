using System.Diagnostics.Tracing;
using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Jobs;

[Config(typeof(Config))]
public class EventSourceAllocations
{
    private SomeEventSourceListener listener;

    private class Config : ManualConfig
    {
        public Config()
        {
            AddExporter(MarkdownExporter.GitHub);
            //AddDiagnoser(MemoryDiagnoser.Default);

            AddJob(Job.ShortRun);
        }
    }

    [GlobalSetup]
    public void Setup()
    {
        listener = new SomeEventSourceListener();
    }

    [Benchmark(Baseline = true)]
    public void ParamsAndBoxing()
    {
        SomeEventSource.Log.ListOwnershipComplete("fullyQualifiedNamespace", "eventHubName", "consumerGroup", 42);
    }

    [Benchmark]
    public void NoParamsNoBoxing()
    {
        SomeEventSource.Log.ListOwnershipCompleteImproved("fullyQualifiedNamespace", "eventHubName", "consumerGroup", 42);
    }
}

sealed class SomeEventSourceListener : EventListener
{
    protected override void OnEventSourceCreated(EventSource eventSource)
    {
        if (eventSource.Name.Equals(SomeEventSource.EventSourceName))
        {
            EnableEvents(eventSource, EventLevel.Verbose, EventKeywords.All);
        }
    }
}

[EventSource(Name = EventSourceName)]
class SomeEventSource : EventSource
{
    public const string EventSourceName = "Some-EventSource";

    protected SomeEventSource()
    {
    }

    public static SomeEventSource Log { get; } = new SomeEventSource();

    [Event(22, Level = EventLevel.Verbose, Message = "Completed listing ownership for FullyQualifiedNamespace: '{0}'; EventHubName: '{1}'; ConsumerGroup: '{2}'.  There were {3} ownership entries were found.")]
    public virtual void ListOwnershipComplete(string fullyQualifiedNamespace,
                                              string eventHubName,
                                              string consumerGroup,
                                              int ownershipCount)
    {
        if (IsEnabled())
        {
            WriteEvent(22, fullyQualifiedNamespace ?? string.Empty, eventHubName ?? string.Empty, consumerGroup ?? string.Empty, ownershipCount);
        }
    }

    [Event(23, Level = EventLevel.Verbose, Message = "Completed listing ownership improved for FullyQualifiedNamespace: '{0}'; EventHubName: '{1}'; ConsumerGroup: '{2}'.  There were {3} ownership entries were found.")]
    public virtual void ListOwnershipCompleteImproved(string fullyQualifiedNamespace,
                                                      string eventHubName,
                                                      string consumerGroup,
                                                      int ownershipCount)
    {
        if (IsEnabled())
        {
            WriteEventImproved(23, fullyQualifiedNamespace, eventHubName, consumerGroup ?? string.Empty, ownershipCount);
        }
    }

    [NonEvent]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [SkipLocalsInit]
    private unsafe void WriteEventImproved<TValue>(int eventId,
                                                  string arg1,
                                                  string arg2,
                                                  string arg3,
                                                  TValue arg4)
        where TValue : struct
    {
        arg1 ??= "";
        arg2 ??= "";
        arg3 ??= "";

        fixed (char* arg1Ptr = arg1)
        fixed (char* arg2Ptr = arg2)
        fixed (char* arg3Ptr = arg3)
        {
            EventData* eventPayload = stackalloc EventData[4];

            eventPayload[0].DataPointer = (IntPtr)arg1Ptr;
            eventPayload[0].Size = checked(arg1.Length + 1) * sizeof(char);

            eventPayload[1].DataPointer = (IntPtr)arg2Ptr;
            eventPayload[1].Size = checked(arg2.Length + 1) * sizeof(char);

            eventPayload[2].DataPointer = (IntPtr)arg3Ptr;
            eventPayload[2].Size = checked(arg3.Length + 1) * sizeof(char);

            eventPayload[3].DataPointer = (IntPtr)Unsafe.AsPointer(ref arg4);
            eventPayload[3].Size = Unsafe.SizeOf<TValue>();

            WriteEventCore(eventId, 4, eventPayload);
        }
    }
}