``` ini

BenchmarkDotNet=v0.13.1, OS=linuxmint 20.3
11th Gen Intel Core i7-1185G7 3.00GHz, 1 CPU, 8 logical and 4 physical cores
.NET SDK=6.0.201
  [Host]     : .NET 6.0.3 (6.0.322.12309), X64 RyuJIT
  Job-UGQPJO : .NET 6.0.3 (6.0.322.12309), X64 RyuJIT

InvocationCount=12800000  

```
|                      Method |      Mean |     Error |    StdDev | Ratio | RatioSD |  Gen 0 | Allocated |
|---------------------------- |----------:|----------:|----------:|------:|--------:|-------:|----------:|
|          BufferAndBlockCopy | 10.821 ns | 0.2176 ns | 0.2830 ns |  1.00 |    0.00 | 0.0063 |      40 B |
|                  BufferPool | 21.339 ns | 0.1525 ns | 0.1352 ns |  1.99 |    0.06 |      - |         - |
|          StackallocWithGuid |  5.877 ns | 0.0452 ns | 0.0378 ns |  0.55 |    0.02 |      - |         - |
| StackallocWithMemoryMarshal |  6.074 ns | 0.0284 ns | 0.0252 ns |  0.57 |    0.02 |      - |         - |
