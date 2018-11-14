**Performance**

The benchmark can be found in [DotNetToolkit.Repository.Performance](/benchmarks/DotNetToolkit.Repository.Performance) and can be compiled via:
```
dotnet build DotNetToolkit.Repository.Performance.csproj --configuration Release
dotnet run DotNetToolkit.Repository.Performance.csproj --configuration Release --filter *
```
Output from the latest run is:

``` ini

BenchmarkDotNet=v0.11.2, OS=Windows 10.0.17134.228 (1803/April2018Update/Redstone4)
Intel Core i7-7700 CPU 3.60GHz (Kaby Lake), 1 CPU, 8 logical and 4 physical cores
Frequency=3515626 Hz, Resolution=284.4444 ns, Timer=TSC
  [Host] : .NET Framework 4.7.2 (CLR 4.0.30319.42000), 64bit RyuJIT-v4.7.3132.0


```
|            Provider |                               Method |           Mean | Rank |
|-------------------- |------------------------------------- |---------------:|-----:|
|            InMemory |                           FindWithId |       514.3 us |    1 |
|            InMemory |                FindWithPagingOptions |     1,958.7 us |    2 |
|            InMemory |               FindWithDefaultOptions |     2,203.3 us |    3 |
|     EntityFramework |                           FindWithId |     2,677.5 us |    4 |
|            InMemory |                    FindWithPredicate |     2,852.1 us |    5 |
|     EntityFramework |                FindWithPagingOptions |     3,066.6 us |    6 |
| EntityFrameworkCore |                           FindWithId |     4,765.9 us |    7 |
| EntityFrameworkCore |                FindWithPagingOptions |     5,427.5 us |    8 |
|              AdoNet |                           FindWithId |     6,300.4 us |    9 |
|     EntityFramework |                    FindWithPredicate |     6,941.9 us |   10 |
|              AdoNet |                FindWithPagingOptions |     6,960.4 us |   10 |
|              AdoNet |               FindWithDefaultOptions |     8,163.0 us |   11 |
|              AdoNet |                    FindWithPredicate |    11,561.8 us |   12 |
|     EntityFramework |               FindWithDefaultOptions |    12,679.4 us |   13 |
| EntityFrameworkCore |                    FindWithPredicate |    38,775.5 us |   14 |
| EntityFrameworkCore |               FindWithDefaultOptions |    40,435.5 us |   15 |
|                     |                                      |                |      |
|            InMemory |                             AddRange |     1,515.0 us |    1 |
|     EntityFramework |                             AddRange |     4,900.4 us |    2 |
| EntityFrameworkCore |                             AddRange |     6,184.7 us |    3 |
|            InMemory |                                  Add |     7,682.8 us |    4 |
|              AdoNet |                             AddRange |    11,200.0 us |    5 |
|              AdoNet |                                  Add |   551,078.0 us |    6 |
| EntityFrameworkCore |                                  Add | 1,071,438.5 us |    7 |
|     EntityFramework |                                  Add | 2,377,767.4 us |    8 |
|                     |                                      |                |      |
|            InMemory |                          DeleteRange |       785.1 us |    1 |
|            InMemory |                               Delete |     1,311.0 us |    2 |
|            InMemory |                         DeleteWithId |     2,719.9 us |    3 |
|            InMemory |                  DeleteWithPredicate |     4,522.1 us |    4 |
|     EntityFramework |                               Delete |     5,350.1 us |    5 |
|     EntityFramework |                          DeleteRange |     5,780.2 us |    6 |
|              AdoNet |                          DeleteRange |     6,025.1 us |    7 |
|              AdoNet |                               Delete |     6,103.3 us |    8 |
| EntityFrameworkCore |                          DeleteRange |     6,290.2 us |    9 |
| EntityFrameworkCore |                               Delete |     7,866.0 us |   10 |
|     EntityFramework |                         DeleteWithId |     8,325.1 us |   11 |
|              AdoNet |                  DeleteWithPredicate |    11,362.7 us |   12 |
| EntityFrameworkCore |                         DeleteWithId |    13,377.7 us |   13 |
|              AdoNet |                         DeleteWithId |    19,697.8 us |   14 |
|     EntityFramework |                  DeleteWithPredicate |    20,645.0 us |   15 |
| EntityFrameworkCore |                  DeleteWithPredicate |    21,527.9 us |   16 |
|                     |                                      |                |      |
|            InMemory |                          UpdateRange |       832.0 us |    1 |
|            InMemory |                               Update |     1,375.0 us |    2 |
|     EntityFramework |                          UpdateRange |     3,047.8 us |    3 |
| EntityFrameworkCore |                          UpdateRange |     5,225.2 us |    4 |
|              AdoNet |                          UpdateRange |     6,341.4 us |    5 |
|     EntityFramework |                               Update |     8,333.7 us |    6 |
| EntityFrameworkCore |                               Update |     8,560.6 us |    7 |
|              AdoNet |                               Update |     9,784.3 us |    8 |
|                     |                                      |                |      |
|            InMemory |                       Async_AddRange |       702.3 us |    1 |
|            InMemory |                            Async_Add |     1,817.0 us |    2 |
|     EntityFramework |                       Async_AddRange |     4,340.6 us |    3 |
| EntityFrameworkCore |                       Async_AddRange |     4,941.7 us |    4 |
|              AdoNet |                       Async_AddRange |    13,790.1 us |    5 |
| EntityFrameworkCore |                            Async_Add |    32,527.1 us |    6 |
|     EntityFramework |                            Async_Add |    36,977.5 us |    7 |
|              AdoNet |                            Async_Add |    40,856.7 us |    8 |
|                     |                                      |                |      |
|            InMemory |                    Async_UpdateRange |       841.1 us |    1 |
|            InMemory |                         Async_Update |     1,254.4 us |    2 |
|     EntityFramework |                    Async_UpdateRange |     1,942.2 us |    3 |
|     EntityFramework |                         Async_Update |     2,606.1 us |    4 |
| EntityFrameworkCore |                    Async_UpdateRange |     5,701.1 us |    5 |
| EntityFrameworkCore |                         Async_Update |     6,174.7 us |    6 |
|              AdoNet |                         Async_Update |     9,119.0 us |    7 |
|              AdoNet |                    Async_UpdateRange |    10,320.5 us |    8 |
|                     |                                      |                |      |
|            InMemory |                         Async_Delete |       991.9 us |    1 |
|            InMemory |                    Async_DeleteRange |     1,062.1 us |    2 |
|            InMemory |                   Async_DeleteWithId |     2,089.5 us |    3 |
|            InMemory |            Async_DeleteWithPredicate |     4,906.7 us |    4 |
|     EntityFramework |                    Async_DeleteRange |     5,126.8 us |    5 |
| EntityFrameworkCore |                    Async_DeleteRange |     8,411.0 us |    6 |
|              AdoNet |                    Async_DeleteRange |     9,434.2 us |    7 |
| EntityFrameworkCore |                         Async_Delete |    10,016.1 us |    8 |
|              AdoNet |                         Async_Delete |    10,229.5 us |    9 |
|     EntityFramework |                         Async_Delete |    10,321.6 us |    9 |
|     EntityFramework |            Async_DeleteWithPredicate |    23,275.5 us |   10 |
|              AdoNet |            Async_DeleteWithPredicate |    25,014.3 us |   11 |
|              AdoNet |                   Async_DeleteWithId |    32,289.6 us |   12 |
|     EntityFramework |                   Async_DeleteWithId |    55,468.4 us |   13 |
| EntityFrameworkCore |            Async_DeleteWithPredicate |    70,044.4 us |   14 |
| EntityFrameworkCore |                   Async_DeleteWithId |    82,909.8 us |   15 |
|                     |                                      |                |      |
|            InMemory |            FindAllWithDefaultOptions |     3,673.6 us |    1 |
|            InMemory |                 FindAllWithPredicate |     4,630.8 us |    2 |
|     EntityFramework |             FindAllWithPagingOptions |     4,632.5 us |    2 |
|            InMemory |             FindAllWithPagingOptions |     4,893.3 us |    3 |
|              AdoNet |             FindAllWithPagingOptions |     6,182.1 us |    4 |
|     EntityFramework |                 FindAllWithPredicate |     6,706.3 us |    5 |
|              AdoNet |                 FindAllWithPredicate |     7,680.9 us |    6 |
|              AdoNet |            FindAllWithDefaultOptions |     8,204.2 us |    7 |
| EntityFrameworkCore |             FindAllWithPagingOptions |    14,722.6 us |    8 |
|     EntityFramework |            FindAllWithDefaultOptions |    15,940.0 us |    9 |
| EntityFrameworkCore |            FindAllWithDefaultOptions |    28,341.5 us |   10 |
| EntityFrameworkCore |                 FindAllWithPredicate |    77,340.1 us |   11 |
|                     |                                      |                |      |
|            InMemory |        ToDictionaryWithPagingOptions |     6,706.9 us |    1 |
|     EntityFramework |        ToDictionaryWithPagingOptions |     6,906.6 us |    2 |
|              AdoNet |        ToDictionaryWithPagingOptions |     7,717.0 us |    3 |
| EntityFrameworkCore |        ToDictionaryWithPagingOptions |     9,929.4 us |    4 |
|            InMemory |       ToDictionaryWithDefaultOptions |    10,174.3 us |    5 |
|              AdoNet |       ToDictionaryWithDefaultOptions |    15,513.0 us |    6 |
|     EntityFramework |       ToDictionaryWithDefaultOptions |    25,687.6 us |    7 |
| EntityFrameworkCore |       ToDictionaryWithDefaultOptions |    40,089.0 us |    8 |
|                     |                                      |                |      |
|            InMemory |             GroupByWithPagingOptions |     6,379.8 us |    1 |
|     EntityFramework |             GroupByWithPagingOptions |     7,302.5 us |    2 |
|              AdoNet |             GroupByWithPagingOptions |     8,509.7 us |    3 |
|            InMemory |            GroupByWithDefaultOptions |     9,405.4 us |    4 |
|     EntityFramework |            GroupByWithDefaultOptions |     9,838.4 us |    5 |
| EntityFrameworkCore |             GroupByWithPagingOptions |    10,141.6 us |    6 |
|              AdoNet |            GroupByWithDefaultOptions |    11,812.1 us |    7 |
| EntityFrameworkCore |            GroupByWithDefaultOptions |    12,175.9 us |    8 |
|                     |                                      |                |      |
|            InMemory |                     Async_FindWithId |       447.4 us |    1 |
|            InMemory |          Async_FindWithPagingOptions |     1,948.7 us |    2 |
|            InMemory |         Async_FindWithDefaultOptions |     1,995.4 us |    3 |
|            InMemory |              Async_FindWithPredicate |     2,193.9 us |    4 |
|     EntityFramework |         Async_FindWithDefaultOptions |     2,680.9 us |    5 |
|     EntityFramework |                     Async_FindWithId |     2,882.8 us |    6 |
|     EntityFramework |          Async_FindWithPagingOptions |     2,994.9 us |    7 |
| EntityFrameworkCore |                     Async_FindWithId |     3,892.1 us |    8 |
| EntityFrameworkCore |          Async_FindWithPagingOptions |     5,174.3 us |    9 |
|              AdoNet |          Async_FindWithPagingOptions |     8,450.3 us |   10 |
|              AdoNet |         Async_FindWithDefaultOptions |     8,574.3 us |   11 |
|              AdoNet |                     Async_FindWithId |     8,659.1 us |   11 |
|              AdoNet |              Async_FindWithPredicate |    11,992.7 us |   12 |
| EntityFrameworkCore |         Async_FindWithDefaultOptions |    13,754.0 us |   13 |
| EntityFrameworkCore |              Async_FindWithPredicate |    14,406.8 us |   14 |
|     EntityFramework |              Async_FindWithPredicate |    34,267.6 us |   15 |
|                     |                                      |                |      |
|            InMemory |           Async_FindAllWithPredicate |     3,608.2 us |    1 |
|            InMemory |       Async_FindAllWithPagingOptions |     3,638.0 us |    1 |
|            InMemory |      Async_FindAllWithDefaultOptions |     3,770.6 us |    2 |
|     EntityFramework |           Async_FindAllWithPredicate |     4,767.6 us |    3 |
|     EntityFramework |      Async_FindAllWithDefaultOptions |     4,895.6 us |    4 |
|     EntityFramework |       Async_FindAllWithPagingOptions |     5,411.3 us |    5 |
|              AdoNet |       Async_FindAllWithPagingOptions |     7,415.7 us |    6 |
|              AdoNet |           Async_FindAllWithPredicate |     9,251.6 us |    7 |
|              AdoNet |      Async_FindAllWithDefaultOptions |     9,784.9 us |    8 |
| EntityFrameworkCore |       Async_FindAllWithPagingOptions |    20,619.1 us |    9 |
| EntityFrameworkCore |      Async_FindAllWithDefaultOptions |    30,744.7 us |   10 |
| EntityFrameworkCore |           Async_FindAllWithPredicate |   104,526.5 us |   11 |
|                     |                                      |                |      |
|            InMemory |      Async_GroupByWithDefaultOptions |     6,559.3 us |    1 |
|            InMemory |       Async_GroupByWithPagingOptions |     7,395.8 us |    2 |
|     EntityFramework |       Async_GroupByWithPagingOptions |     7,854.1 us |    3 |
|     EntityFramework |      Async_GroupByWithDefaultOptions |     7,864.3 us |    3 |
|              AdoNet |       Async_GroupByWithPagingOptions |     9,482.2 us |    4 |
| EntityFrameworkCore |      Async_GroupByWithDefaultOptions |     9,905.8 us |    5 |
| EntityFrameworkCore |       Async_GroupByWithPagingOptions |    10,432.9 us |    6 |
|              AdoNet |      Async_GroupByWithDefaultOptions |    16,051.8 us |    7 |
|                     |                                      |                |      |
|            InMemory |  Async_ToDictionaryWithPagingOptions |     6,713.2 us |    1 |
|            InMemory | Async_ToDictionaryWithDefaultOptions |     7,175.4 us |    2 |
|     EntityFramework |  Async_ToDictionaryWithPagingOptions |     8,350.4 us |    3 |
|              AdoNet |  Async_ToDictionaryWithPagingOptions |     9,011.5 us |    4 |
| EntityFrameworkCore |  Async_ToDictionaryWithPagingOptions |    10,513.6 us |    5 |
|              AdoNet | Async_ToDictionaryWithDefaultOptions |    18,959.4 us |    6 |
|     EntityFramework | Async_ToDictionaryWithDefaultOptions |    22,703.8 us |    7 |
| EntityFrameworkCore | Async_ToDictionaryWithDefaultOptions |    43,515.7 us |    8 |
