```

BenchmarkDotNet v0.14.0, macOS Sequoia 15.3.1 (24D70) [Darwin 24.3.0]
Apple M1, 1 CPU, 8 logical and 8 physical cores
.NET SDK 8.0.202
  [Host]     : .NET 8.0.3 (8.0.324.11423), Arm64 RyuJIT AdvSIMD
  DefaultJob : .NET 8.0.3 (8.0.324.11423), Arm64 RyuJIT AdvSIMD


```
| Method                          | Mean        | Error      | StdDev     | Median      | Gen0       | Gen1       | Gen2      | Allocated  |
|-------------------------------- |------------:|-----------:|-----------:|------------:|-----------:|-----------:|----------:|-----------:|
| GetAllCategories                |    12.75 ms |   0.253 ms |   0.731 ms |    12.46 ms |   100.0000 |   100.0000 |  100.0000 |  707.33 KB |
| GetAllPagesCategories           | 5,564.30 ms | 110.546 ms | 193.613 ms | 5,506.24 ms | 16000.0000 | 16000.0000 | 6000.0000 | 42595.2 KB |
| GetAllCategoriesWithCursor      |          NA |         NA |         NA |          NA |         NA |         NA |        NA |         NA |
| GetAllCategoriesPagesWithCursor |          NA |         NA |         NA |          NA |         NA |         NA |        NA |         NA |

Benchmarks with issues:
  EndpointBenchmarks.GetAllCategoriesWithCursor: DefaultJob
  EndpointBenchmarks.GetAllCategoriesPagesWithCursor: DefaultJob
