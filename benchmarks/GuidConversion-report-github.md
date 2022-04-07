``` ini

BenchmarkDotNet=v0.13.1, OS=linuxmint 20.3
11th Gen Intel Core i7-1185G7 3.00GHz, 1 CPU, 8 logical and 4 physical cores
.NET SDK=6.0.201
  [Host]     : .NET 6.0.3 (6.0.322.12309), X64 RyuJIT
  Job-YQCSEL : .NET 6.0.3 (6.0.322.12309), X64 RyuJIT

InvocationCount=25600000  

```
|                                   Method |      Mean |     Error |    StdDev | Ratio | RatioSD |  Gen 0 | Allocated |
|----------------------------------------- |----------:|----------:|----------:|------:|--------:|-------:|----------:|
|                       BufferAndBlockCopy | 10.975 ns | 0.1860 ns | 0.1740 ns |  1.00 |    0.00 | 0.0064 |      40 B |
|                               BufferPool | 24.718 ns | 0.3059 ns | 0.2555 ns |  2.26 |    0.03 |      - |         - |
|                       StackallocWithGuid |  6.078 ns | 0.0362 ns | 0.0321 ns |  0.55 |    0.01 |      - |         - |
|          StackallocWithGuidAndLocalsInit |  6.193 ns | 0.1445 ns | 0.1281 ns |  0.56 |    0.01 |      - |         - |
|              StackallocWithMemoryMarshal |  6.265 ns | 0.1125 ns | 0.1052 ns |  0.57 |    0.02 |      - |         - |
| StackallocWithMemoryMarshalAndLocalsInit |  6.095 ns | 0.1184 ns | 0.1050 ns |  0.56 |    0.01 |      - |         - |
