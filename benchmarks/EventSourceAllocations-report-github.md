``` ini

BenchmarkDotNet=v0.13.1, OS=linuxmint 20.3
11th Gen Intel Core i7-1185G7 3.00GHz, 1 CPU, 8 logical and 4 physical cores
.NET SDK=6.0.201
  [Host]   : .NET 6.0.3 (6.0.322.12309), X64 RyuJIT
  ShortRun : .NET 6.0.3 (6.0.322.12309), X64 RyuJIT

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
|           Method |     Mean |     Error |   StdDev | Ratio | RatioSD |
|----------------- |---------:|----------:|---------:|------:|--------:|
|  ParamsAndBoxing | 99.46 ns | 45.686 ns | 2.504 ns |  1.00 |    0.00 |
| NoParamsNoBoxing | 95.86 ns |  4.814 ns | 0.264 ns |  0.96 |    0.03 |
