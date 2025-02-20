```

BenchmarkDotNet v0.14.0, macOS Sequoia 15.3.1 (24D70) [Darwin 24.3.0]
Apple M1, 1 CPU, 8 logical and 8 physical cores
.NET SDK 8.0.202
  [Host]     : .NET 8.0.3 (8.0.324.11423), Arm64 RyuJIT AdvSIMD
  DefaultJob : .NET 8.0.3 (8.0.324.11423), Arm64 RyuJIT AdvSIMD
  Job-VXOXAZ : .NET 8.0.3 (8.0.324.11423), Arm64 RyuJIT AdvSIMD


```
| Method            | Job        | InvocationCount | UnrollFactor | Mean        | Error       | StdDev      | Median      | Gen0   | Allocated |
|------------------ |----------- |---------------- |------------- |------------:|------------:|------------:|------------:|-------:|----------:|
| GetAllCategories  | DefaultJob | Default         | 16           |    360.0 μs |     6.86 μs |     7.04 μs |    361.7 μs |      - |    4.3 KB |
| GetCategoryById   | DefaultJob | Default         | 16           |    392.1 μs |     7.83 μs |    11.73 μs |    392.2 μs |      - |   4.31 KB |
| GetCategoryByName | DefaultJob | Default         | 16           |    413.0 μs |    11.76 μs |    32.98 μs |    403.4 μs | 0.4883 |   4.49 KB |
| CreateCategory    | DefaultJob | Default         | 16           | 10,596.3 μs | 1,542.27 μs | 4,547.42 μs | 10,967.2 μs |      - |    5.3 KB |
| UpdateCategory    | DefaultJob | Default         | 16           |    660.3 μs |    27.88 μs |    76.80 μs |    644.6 μs |      - |    5.2 KB |
| DeleteCategory    | Job-VXOXAZ | 1               | 1            |  1,117.9 μs |    33.96 μs |    95.23 μs |  1,111.7 μs |      - |  37.68 KB |
