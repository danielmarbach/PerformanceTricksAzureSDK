# Performance tricks I learned from contributing to the Azure .NET SDK

As a practical learner, I've found that performance optimizations have been my biggest challenge and the place I've learned the tricks that are the most helpful. These lessons have come by trial and error. As it turns out, the Azure .NET SDK was a perfect “playground” for learning new tricks because it's maintained by people who care and give feedback. 

Over the past few years, I've contributed over fifty pull requests to the Azure .NET SDK and some of them got accepted. In this session, I'll walk you through the performance improvements I learned from my experiments. Some “superpowers” you will learn are spotting closure allocations and opportunities for memory pooling and, best of all, how to improve them. 

## Outline

.NET has been evolving over the years into a modern and high-performance platform. The languages running on .NET have also been improving and allowing to fall to lower levels without giving up on the safety guarantees too much or only where it is really needed. It is less and less needed to use unmanaged languages like C/C++ to achieve code that performs well at scale.

## Caveats

In this talk, I'm going to focus on some performance optimizations that can be done in code that is library and framework like. I won't be talking about architectural patterns like vertical or horizontal scaling. The focus is purely on code with examples in CSharp. Some optimizations shown here can be seen as esoteric in typical line of business applications, and I wouldn't recommend jumping to conclusions and applying those everywhere. It is important to note that for code that is executed under scale, optimizations on code can bring a lot of benefit to the table due to not only being fast but also being more efficient in resource usage, execution time, throughput and memory usage.

But what does at scale even mean? How can I find out whether the optimizations I'm trying to make have value, and I'm not getting called out by my colleagues for premature optimizations?

## What does at scale mean?

I've heard countless times already: "Wow, that's crazy, is the complexity of this change really worth it? Isn't that premature optimization?" While it is true that performance improvements can be addictive, it is also true that nobody likes to optimize code that is "fast enough" or is only executed a few times a day as a background job. 

David Fowler: Scale for an application can mean the number of users that will concurrently connect to the application at any given time, the amount of input to process (for example the size of the data) or the number of times data needs to be processed (for example the number of requests per second). For us, as engineers, it means we have to know what to ignore and knowing what to pay close attention to.

A good way to explore what scale means is to discover the assumptions that have accumulated over time in a given code base by paying close attention to what is instantiated, parsed, processed etc. per request and how those assumptions in the code base affect the performance characteristics (memory, throughput...) at scale.

## General rules of thumb

- Avoid excessive allocations to reduce the GC overhead
  - Be aware of closure allocations
  - Be aware of parameter overloads
  - Where possible and feasible use value types but pay attention to unnecessary boxing
  - Think twice before using LINQ or unnecessary enumeration on the hot path
  - Pool and re-use buffers
  - For smaller local buffers, consider using the stack
- Avoid unnecessary copying of memory

## Brief overview over the terminologies used

Quick sample of Azure Service Bus SDK
Explain the layering

## Avoid excessive allocations to reduce the GC overhead

### Think twice before using LINQ or unnecessary enumeration on the hot path

LINQ is great, and I wouldn't want to miss it at all. Yet, on the hot path it is far too easy to get into troubles with LINQ because it can cause hidden allocations and is difficult for the JIT to optimize. Let's look at a piece of code from the `AmqpReceiver`

```csharp

public class AmqpReceiver 
{
    ConcurrentBag<Guid> _requestResponseLockedMessages = new ();
    
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
```
The public API accepts the broadest possible enumeration type `IEnumerable<T>` by design and then converts the strings into `Guid`. Then it checks by using `Any` whether there is a lock token contained in the provided tokens that was previously already seen. Let's look how the code looks like in the decompiler

```csharp
public class AmqpReceiver
{
    [Serializable]
    [CompilerGenerated]
    private sealed class <>c
    {
        public static readonly <>c <>9 = new <>c();

        public static Func<string, Guid> <>9__2_0;

        internal Guid <CompleteInternalAsync>b__2_0(string token)
        {
            return new Guid(token);
        }

    }

    // omitted for brevity

    private Task CompleteInternalAsync(IEnumerable<string> lockTokens)
    {
        Enumerable.Any(Enumerable.ToArray(Enumerable.Select(lockTokens, <>c.<>9__2_0 ?? (<>c.<>9__2_0 = new Func<string, Guid>(<>c.<>9.<CompleteInternalAsync>b__2_0)))), new Func<Guid, bool>(<CompleteInternalAsync>b__2_1));
        return Task.CompletedTask;
    }

    [CompilerGenerated]
    private bool <CompleteInternalAsync>b__2_1(Guid lockToken)
    {
        return Enumerable.Contains(_requestResponseLockedMessages, lockToken);
    }
}
```

For every call of CompleteInternalAsync a new instance of `Func<Guid, bool>` is allocated that points to `<CompleteInternalAsync>b__2_1`. A closure captures the `_requestResponseLockedMessages` and the `lockToken` as state. This allocation is unnecessary. 

```csharp
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
```

get decompiled down to

```csharp
    private Task CompleteInternalAsync(IEnumerable<string> lockTokens)
    {
        Guid[] array = Enumerable.ToArray(Enumerable.Select(lockTokens, <>c.<>9__2_0 ?? (<>c.<>9__2_0 = new Func<string, Guid>(<>c.<>9.<CompleteInternalAsync>b__2_0))));
        int num = 0;
        while (num < array.Length)
        {
            Guid item = array[num];
            if (_requestResponseLockedMessages.Contains(item))
            {
                return Task.CompletedTask;
            }
            num++;
        }
        return Task.CompletedTask;
    }
```

Let's see what we got here.

![](benchmarks/LinqBeforeAfterComparison.png)

By getting rid of the `Any` we were able to squeeze out some good performance improvements. Sometimes, though, we can do even more. For example, there are a few general rules we can fully when we turn a refactor a code path using LINQ to collection-based operations. 

- Use `Array.Empty<T>` to represent empty arrays
- Use `Enumerable.Empty<T>` to represent empty enumerables
- When the size of the items to be added to the collection are known upfront, initialize the collection with the correct count to prevent the collection from growing and thus allocating more and reshuffling things internally
- Use the concrete collection type instead of interfaces or abstract types. For example, when enumerating through a `List<T>` with `foreach` it uses a non-allocating `List<T>.Enumerator` struct. But when it is used through for example `IEnumerable<T>` that struct is boxed to `IEnumerator<T>` in the foreach and thus allocates.
- With more modern CSharp versions that have good pattern matching support, it is sometimes possible to do a quick type check and based on the underlying collection type get access to the count without having to use `Count()`. With .NET 6, and later, it is also possible to use [`Enumerable.TryGetNonEnumeratedCount`](https://docs.microsoft.com/en-us/dotnet/api/system.linq.enumerable.trygetnonenumeratedcount) which internally does collection type checking to get the count without enumerating.
- Wait with instantiating collections until you really need them.

```csharp

List<object> data = data = Enumerable.Repeat(new SomeClass(), NumberOfItems).Cast<object>().ToList();

public List<SomeClass> Original()
{
    var copyList = new List<SomeClass>();
    foreach (var value in GetValue<SomeClass>())
    {
        copyList.Add(value);
    }

    return copyList;
}

IEnumerable<TValue> GetValue<TValue>()
{
    return data.Cast<TValue>();
}
```

```csharp
public List<SomeClass> List()
{
    List<SomeClass> copyList = null;
    var enumerable = GetValueList<SomeClass>();
    foreach (var value in enumerable)
    {
        copyList ??= enumerable is IReadOnlyCollection<SomeClass> readOnlyList
            ? new List<SomeClass>(readOnlyList.Count)
            : new List<SomeClass>();

        copyList.Add(value);
    }

    return copyList;
}

IEnumerable<TValue> GetValueList<TValue>()
{
    List<TValue> values = null;
    foreach (var item in data)
    {
        values ??= new List<TValue>(data.Count);
        values.Add((TValue)item);
    }
    return values ?? Enumerable.Empty<TValue>();
}
```

by slightly tweaking `GetValueList`

```csharp
List<TValue> GetValueListReturnList<TValue>()
{
    var values = new List<TValue>(data.Count);
    foreach (var item in data)
    {
        values.Add((TValue)item);
    }
    return values;
}
```

or turning the `GetValueListReturnList` `foreach` into a `for` loop

```csharp
List<TValue> GetValueListReturnListFor<TValue>()
{
    var values = new List<TValue>(data.Count);
    for (var index = 0; index < data.Count; index++)
    {
        var item = data[index];
        values.Add((TValue) item);
    }

    return values;
}
```

and then combining that with replacing the outer `foreach` with a `for` loop as well

```csharp
public List<SomeClass> ListForReturnListFor()
{
    List<SomeClass> copyList = null;
    var enumerable = GetValueListReturnListFor<SomeClass>();
    for (var index = 0; index < enumerable.Count; index++)
    {
        var value = enumerable[index];
        copyList ??= enumerable is IReadOnlyCollection<SomeClass> readOnlyList
            ? new List<SomeClass>(readOnlyList.Count)
            : new List<SomeClass>();

        copyList.Add(value);
    }

    return copyList;
}
```

![](benchmarks/CollectionComparison.png)

Now we are really getting in weird territory. Arguably, optimizing things at the level of `foreach` vs `for` can be considered to be too crazy and esoteric. Like with all things, it is crucial to know when to stop on a given code path and find other areas that are more impactful to optimize. The context of the piece of code that you are trying to optimize is crucial. For example, if you are trying to optimize something that uses `IEnumerable` that is passed based on the user input like as for the `AmqReceiver` by applying the rules above you might turn this piece of code:

```csharp
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
```

into something like

```csharp
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
    int index = 0;
    foreach (var token in lockTokens)
    {
        var tokenGuid = new Guid(token);
        lockTokenGuids[index++] = tokenGuid;
        if (_requestResponseLockedMessages.Contains(tokenGuid))
        {
            return Task.CompletedTask;
        }
    }
    return Task.CompletedTask;
}
```

let's see how this code behaves under various inputs passed as `IEnumerable<string>`.

![](benchmarks/LinqAfterComparison.png)

while we managed to get some additional savings in terms of allocations over some collection types, we can see actually passing an enumerable that gets lazy enumerated is behaving much worse than our first simple optimization. 

### Be aware of closure allocations

We have already touched a bit on closure allocations during our LINQ performance investigations. But closures can occur anywhere where we have lambdas (`Action` or `Func` delegates) being invoked that access state from the outside of the lambda. 

```csharp
internal async Task RunOperation(Func<TimeSpan, Task> operation, TransportConnectionScope scope, CancellationToken cancellationToken) 
{
    TimeSpan tryTimeout = CalculateTryTimeout(0);
    // omitted
    while (!cancellationToken.IsCancellationRequested)
    {
        if (IsServerBusy)
        {
            await Task.Delay(ServerBusyBaseSleepTime, cancellationToken).ConfigureAwait(false);
        }

        try
        {
            await operation(tryTimeout).ConfigureAwait(false);
            return;
        }
        catch 
        {
            // omitted
        }
    }
}
```

the usage of the retry policy:

```csharp
TransportMessageBatch messageBatch = null;
Task createBatchTask = _retryPolicy.RunOperation(async (timeout) =>
{
    messageBatch = await CreateMessageBatchInternalAsync(options, timeout).ConfigureAwait(false);
},
_connectionScope,
cancellationToken);
await createBatchTask.ConfigureAwait(false);
return messageBatch;
```

get compiled down to something like

```csharp
if (num1 != 0)
{
    this.\u003C\u003E8__1 = new AmqpSender.\u003C\u003Ec__DisplayClass16_0();
    this.\u003C\u003E8__1.\u003C\u003E4__this = this.\u003C\u003E4__this;
    this.\u003C\u003E8__1.options = this.options;
    this.\u003C\u003E8__1.messageBatch = (TransportMessageBatch) null;
    configuredTaskAwaiter = amqpSender._retryPolicy.RunOperation(new Func<TimeSpan, Task>((object) this.\u003C\u003E8__1, __methodptr(\u003CCreateMessageBatchAsync\u003Eb__0)), (TransportConnectionScope) amqpSender._connectionScope, this.cancellationToken).ConfigureAwait(false).GetAwaiter();
    if (!configuredTaskAwaiter.IsCompleted)
    {
        this.\u003C\u003E1__state = num2 = 0;
        this.\u003C\u003Eu__1 = configuredTaskAwaiter;
        ((AsyncTaskMethodBuilder<TransportMessageBatch>) ref this.\u003C\u003Et__builder).AwaitUnsafeOnCompleted<ConfiguredTaskAwaitable.ConfiguredTaskAwaiter, AmqpSender.\u003CCreateMessageBatchAsync\u003Ed__16>((M0&) ref configuredTaskAwaiter, (M1&) ref this);
        return;
    }
}
```

by leveraging the latest improvements to the language and the runtime such as static lambdas, ValueTasks and ValueTuples and introducing a generic parameter we can modify the code to allow passing in the required state from the outside into the lambda.

```csharp
internal async ValueTask RunOperation<T1>(
    Func<T1, TimeSpan, CancellationToken, ValueTask> operation,
    T1 t1,
    TransportConnectionScope scope,
    CancellationToken cancellationToken) =>
    await RunOperation(static async (value, timeout, token) =>
    {
        var (t1, operation) = value;
        await operation(t1, timeout, token).ConfigureAwait(false);
        return default(object);
    }, (t1, operation), scope, cancellationToken).ConfigureAwait(false);

internal async ValueTask<TResult> RunOperation<T1, TResult>(
    Func<T1, TimeSpan, CancellationToken, ValueTask<TResult>> operation,
    T1 t1,
    TransportConnectionScope scope,
    CancellationToken cancellationToken)
{
    TimeSpan tryTimeout = CalculateTryTimeout(0);
    // omitted
    while (!cancellationToken.IsCancellationRequested)
    {
        if (IsServerBusy)
        {
            await Task.Delay(ServerBusyBaseSleepTime, cancellationToken).ConfigureAwait(false);
        }

        try
        {
            return await operation(t1, tryTimeout, cancellationToken).ConfigureAwait(false);
        }
        catch 
        {
            // omitted
        }
    }
}
```

This then gets compiled down to

```csharp
if (num1 != 0)
{
    configuredTaskAwaiter = t1._retryPolicy.RunOperation<AmqpSender, CreateMessageBatchOptions, TransportMessageBatch>(AmqpSender.\u003C\u003Ec.\u003C\u003E9__16_0 ?? (AmqpSender.\u003C\u003Ec.\u003C\u003E9__16_0 = new Func<AmqpSender, CreateMessageBatchOptions, TimeSpan, CancellationToken, Task<TransportMessageBatch>>((object) AmqpSender.\u003C\u003Ec.\u003C\u003E9, __methodptr(\u003CCreateMessageBatchAsync\u003Eb__16_0))), t1, this.options, (TransportConnectionScope) t1._connectionScope, this.cancellationToken).ConfigureAwait(false).GetAwaiter();
    if (!configuredTaskAwaiter.IsCompleted)
    {
        this.\u003C\u003E1__state = num2 = 0;
        this.\u003C\u003Eu__1 = configuredTaskAwaiter;
        ((AsyncValueTaskMethodBuilder<TransportMessageBatch>) ref this.\u003C\u003Et__builder).AwaitUnsafeOnCompleted<ConfiguredTaskAwaitable<TransportMessageBatch>.ConfiguredTaskAwaiter, AmqpSender.\u003CCreateMessageBatchAsync\u003Ed__16>((M0&) ref configuredTaskAwaiter, (M1&) ref this);
        return;
    }
}
```

With that small change, we save the display class and the function delegate allocations and can properly usage methods that support value tasks without having to allocate a task instance when not necessary.

To demonstrate how these can add up in real-world scenarios, let me show you a before and after comparison when I [removed the closure allocations for NServiceBus pipeline execution](https://github.com/Particular/NServiceBus/pull/6237) code.

![](benchmarks/NServiceBusPipelineExecution.png)

But how would I detect those? When using memory tools, look out for excessive allocations of `*__DisplayClass*` or various variants of `Action` and `Func` allocations. Extensions like the [Heap Allocation Viewer](https://plugins.jetbrains.com/plugin/9223-heap-allocations-viewer) for Rider for example can also help to discover these types of issues while writing or refactoring code. Many built-in .NET types that use delegates have nowadays generic overloads that allow to pass state into the delegate. For example `ConcurrentDictionary` has `public TValue GetOrAdd<TArg> (TKey key, Func<TKey,TArg,TValue> valueFactory, TArg factoryArgument);` which allows to pass external state into the lambda. When you need to access multiple state parameters inside the `GetOrAdd` method you can use `ValueTuple` to pass the state into it.

```csharp
var someState1 = new object();
var someOtherState = 42;

var dictionary = new ConcurrentDictionary<string, string>();

dictionary.GetOrAdd("SomeKey", static (key, state) => 
{
    var (someState, someOtherState) = state;

    return $"{someState}_{someOtherState}";
}, (someState1, someOtherState));
```

### Pool and re-use buffers (and larger objects)

Azure Service Bus uses the concept lock tokens (a glorified GUID) in certain modes to ackknowledge messages. For messages loaded by the client there is a this lock token that needs to be turned into a GUID representation. The existing code looked like the following:

```csharp
// somewhere from the network we get a guid as a byte array
var data = new ArraySegment<byte>(Guid.NewGuid().ToByteArray());
```

```csharp
var guidBuffer = new byte[16];
Buffer.BlockCopy(data.Array, data.Offset, guidBuffer, 0, 16);
var lockTokenGuid = new Guid(guidBuffer);
```

For every message received a new byte array of length 16 is allocated on the heap and then the value of the ArraySegment is copied into the buffer. The buffer is then passed to the Guid constructor. When receiving lots of messages this creates a lot of unnecessary allocations.

.NET has a built in mechanism called `ArrayPool<T>` that allows to have pooled arrays that can be reused. Let's see if we can use that one to improve the performance characteristics of the code.

```csharp
byte[] guidBuffer =  ArrayPool<byte>.Shared.Rent(16);
Buffer.BlockCopy(data.Array, data.Offset, guidBuffer, 0, 16);
var lockTokenGuid = new Guid(guidBuffer);
ArrayPool<byte>.Shared.Return(guidBuffer);
```

Let's measure how we did.

![](/benchmarks/BufferAndBlockCopyPooling.png)

It turns out while we are saving allocations now we haven't really made things much better overall since the code now takes more than double the time to execute. It might very well be that this is an acceptable tradeoff for library or framework you are building. That being said we can do better. ArrayPool isn't the best usage for smaller arrays. For arrays to an certain threshold it is faster to allocate on the current method stack directly instead of paying the price of renting and return an array.

```csharp
Span<byte> guidBytes = stackalloc byte[16];
data.AsSpan().CopyTo(guidBytes);
var lockTokenGuid = new Guid(guidBytes);
```

With the introduction of `Span<T>` and the `stackalloc` keyword we can directly allocate the memory on the method's stack that is cleared when the method returns. 

![](/benchmarks/StackallocWithGuid.png.png)

## Parameter overloads and boxing

Should I add this?

## Avoid unnecessary copying of memory

Show https://github.com/Azure/azure-sdk-for-net/pull/11255/files
and
https://github.com/Azure/azure-sdk-for-net/pull/27598/files

## Interesting Pullrequests

- Do not copy unnecessary
  - [Not copying memory that is by design immutable](https://github.com/Azure/azure-sdk-for-net/pull/11255)
  - [Use MemoryMarshal to extract ArraySegments](https://github.com/Azure/azure-sdk-for-net/pull/19821)
  - [With knowledge of the underlying layers it is possible to achieve more](https://github.com/Azure/azure-sdk-for-net/pull/19823)
  - [Stackalloc and marshal the delivery tag](https://github.com/Azure/azure-sdk-for-net/pull/19857)
  - [Sometimes dirty tricks with a good concept achieve a lot](https://github.com/Azure/azure-sdk-for-net/pull/19996)
  - [Sender side](https://github.com/Azure/azure-sdk-for-net/pull/20098)
  - [Remove byte array allocations from AmqpReceiver DisposeMessagesAsync by buffering](https://github.com/Azure/azure-sdk-for-net/pull/20427)
  - [Use the appropriate type instead of converting all over the place](https://github.com/Azure/azure-sdk-for-net/pull/20543/files)
- Where possible and feasible, use ValueTypes
  - [ValueStopWatch instead of a StopWatch](https://github.com/Azure/azure-sdk-for-net/pull/11266)
  - [Event Source remove allocations and boxing](https://github.com/Azure/azure-sdk-for-net/pull/26989)
- Closure Allocations
  - [ServiceBusRetryPolicy generic overloads to avoid closure capturing](https://github.com/Azure/azure-sdk-for-net/pull/19522)
  - [Use new state-based overloads where possible to avoid closures](https://github.com/Azure/azure-sdk-for-net/pull/19884) 
  - [TrackPublishHandlerAsActiveAsync closure and synchronous invocation hint](https://github.com/Azure/azure-sdk-for-net/pull/26986/files)
  - [NServiceBUs v8 Pipeline Optimizations](https://github.com/Particular/NServiceBus/pull/6237)
- Enumerations and LINQ
  - [ServiceBusProcessor RunReceiveTaskAsync small improvements](https://github.com/Azure/azure-sdk-for-net/pull/19665) 
  - [[Azure Service Bus] Remove unnecessary LINQ on AmqpReceiver](https://github.com/Azure/azure-sdk-for-net/pull/11272)
  - [Use collection types directly and set the collection capacity if possible](https://github.com/Azure/azure-sdk-for-net/pull/20571)
  - [Transition enumerable types early and use readonly where possible](https://github.com/Azure/azure-sdk-for-net/pull/26911)

## Interesting further reading material

- [Konrad Kokosa - High-performance code design patterns in C#](https://prodotnetmemory.com/slides/PerformancePatternsLong)
- [David Fowler - Implementation details matter](https://speakerdeck.com/davidfowl/implementation-details-matter)
- [Reuben Bond - Performance Tuning for .NET Core](https://reubenbond.github.io/posts/dotnet-perf-tuning)
- [stackalloc expression](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/operators/stackalloc)