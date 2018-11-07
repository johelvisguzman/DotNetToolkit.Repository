**Performance**

The benchmark can be found in [DotNetToolkit.Repository.Performance](https://github.com/johelvisguzman/DotNetToolkit.Repository/tree/dev/benchmarks/DotNetToolkit.Repository.Performance) and can be compiled via:
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
|     EntityFramework |        ToDictionaryWithPagingOptions |     6,877.3 us |    1 |
|              AdoNet |        ToDictionaryWithPagingOptions |     7,191.3 us |    2 |
|            InMemory |        ToDictionaryWithPagingOptions |     8,082.5 us |    3 |
|            InMemory |       ToDictionaryWithDefaultOptions |    10,320.5 us |    4 |
| EntityFrameworkCore |        ToDictionaryWithPagingOptions |    11,253.8 us |    5 |
|              AdoNet |       ToDictionaryWithDefaultOptions |    14,867.1 us |    6 |
|     EntityFramework |       ToDictionaryWithDefaultOptions |    21,737.0 us |    7 |
| EntityFrameworkCore |       ToDictionaryWithDefaultOptions |    37,921.0 us |    8 |
|                     |                                      |                |      |
|     EntityFramework |             GroupByWithPagingOptions |     6,637.8 us |    1 |
|              AdoNet |             GroupByWithPagingOptions |     6,651.4 us |    1 |
|            InMemory |             GroupByWithPagingOptions |     6,778.9 us |    2 |
| EntityFrameworkCore |             GroupByWithPagingOptions |     8,930.4 us |    3 |
|            InMemory |            GroupByWithDefaultOptions |     9,141.5 us |    4 |
|     EntityFramework |            GroupByWithDefaultOptions |     9,832.4 us |    5 |
|              AdoNet |            GroupByWithDefaultOptions |    12,957.6 us |    6 |
| EntityFrameworkCore |            GroupByWithDefaultOptions |    13,560.0 us |    7 |
|                     |                                      |                |      |
|            InMemory |                           FindWithId |       870.1 us |    1 |
|            InMemory |                FindWithPagingOptions |     1,937.1 us |    2 |
|            InMemory |               FindWithDefaultOptions |     2,102.0 us |    3 |
|     EntityFramework |                           FindWithId |     2,349.2 us |    4 |
|            InMemory |                    FindWithPredicate |     2,959.1 us |    5 |
|     EntityFramework |                FindWithPagingOptions |     3,137.7 us |    6 |
| EntityFrameworkCore |                           FindWithId |     3,848.8 us |    7 |
| EntityFrameworkCore |                FindWithPagingOptions |     4,858.0 us |    8 |
|     EntityFramework |                    FindWithPredicate |     4,953.9 us |    9 |
|              AdoNet |                FindWithPagingOptions |     5,297.8 us |   10 |
|              AdoNet |                           FindWithId |     5,756.6 us |   11 |
|              AdoNet |               FindWithDefaultOptions |     6,383.5 us |   12 |
|              AdoNet |                    FindWithPredicate |     9,732.0 us |   13 |
|     EntityFramework |               FindWithDefaultOptions |    12,336.1 us |   14 |
| EntityFrameworkCore |                    FindWithPredicate |    31,584.1 us |   15 |
| EntityFrameworkCore |               FindWithDefaultOptions |    37,097.5 us |   16 |
|                     |                                      |                |      |
|            InMemory |                             AddRange |     1,464.9 us |    1 |
|     EntityFramework |                             AddRange |     3,335.4 us |    2 |
|              AdoNet |                             AddRange |     6,881.0 us |    3 |
| EntityFrameworkCore |                             AddRange |     7,214.9 us |    4 |
|            InMemory |                                  Add |     7,699.6 us |    5 |
|              AdoNet |                                  Add |    73,030.2 us |    6 |
| EntityFrameworkCore |                                  Add |   778,269.1 us |    7 |
|     EntityFramework |                                  Add | 2,381,734.3 us |    8 |
|                     |                                      |                |      |
|            InMemory |                          DeleteRange |       931.8 us |    1 |
|            InMemory |                               Delete |       967.1 us |    2 |
|            InMemory |                         DeleteWithId |     2,776.2 us |    3 |
|     EntityFramework |                          DeleteRange |     4,222.6 us |    4 |
|     EntityFramework |                               Delete |     5,250.8 us |    5 |
|            InMemory |                  DeleteWithPredicate |     5,808.1 us |    6 |
|              AdoNet |                          DeleteRange |     6,005.8 us |    7 |
|              AdoNet |                               Delete |     6,576.1 us |    8 |
|     EntityFramework |                         DeleteWithId |     7,609.5 us |    9 |
| EntityFrameworkCore |                               Delete |     8,197.7 us |   10 |
| EntityFrameworkCore |                          DeleteRange |     8,351.9 us |   11 |
|              AdoNet |                  DeleteWithPredicate |     9,596.6 us |   12 |
| EntityFrameworkCore |                         DeleteWithId |    14,292.5 us |   13 |
|              AdoNet |                         DeleteWithId |    17,348.0 us |   14 |
|     EntityFramework |                  DeleteWithPredicate |    19,670.7 us |   15 |
| EntityFrameworkCore |                  DeleteWithPredicate |    22,109.6 us |   16 |
|                     |                                      |                |      |
|            InMemory |                          UpdateRange |       989.0 us |    1 |
|            InMemory |                               Update |     1,418.2 us |    2 |
|     EntityFramework |                          UpdateRange |     2,113.4 us |    3 |
| EntityFrameworkCore |                          UpdateRange |     5,296.4 us |    4 |
|     EntityFramework |                               Update |     8,888.9 us |    5 |
|              AdoNet |                          UpdateRange |     9,022.9 us |    6 |
|              AdoNet |                               Update |    10,300.3 us |    7 |
| EntityFrameworkCore |                               Update |    10,571.9 us |    8 |
|                     |                                      |                |      |
|            InMemory |                       Async_AddRange |     1,450.7 us |    1 |
|            InMemory |                            Async_Add |     1,772.1 us |    2 |
|     EntityFramework |                       Async_AddRange |     3,964.6 us |    3 |
| EntityFrameworkCore |                       Async_AddRange |     6,062.4 us |    4 |
|              AdoNet |                       Async_AddRange |    11,257.5 us |    5 |
| EntityFrameworkCore |                            Async_Add |    32,122.6 us |    6 |
|     EntityFramework |                            Async_Add |    35,660.8 us |    7 |
|              AdoNet |                            Async_Add |    43,348.8 us |    8 |
|                     |                                      |                |      |
|            InMemory |                    Async_UpdateRange |       883.8 us |    1 |
|            InMemory |                         Async_Update |     1,196.4 us |    2 |
|     EntityFramework |                         Async_Update |     2,491.7 us |    3 |
|     EntityFramework |                    Async_UpdateRange |     2,690.6 us |    4 |
| EntityFrameworkCore |                    Async_UpdateRange |     5,296.6 us |    5 |
| EntityFrameworkCore |                         Async_Update |     5,847.9 us |    6 |
|              AdoNet |                    Async_UpdateRange |     9,232.5 us |    7 |
|              AdoNet |                         Async_Update |     9,722.9 us |    8 |
|                     |                                      |                |      |
|            InMemory |                         Async_Delete |     1,469.2 us |    1 |
|            InMemory |                    Async_DeleteRange |     1,520.6 us |    2 |
|            InMemory |                   Async_DeleteWithId |     2,501.7 us |    3 |
|     EntityFramework |                    Async_DeleteRange |     4,073.8 us |    4 |
|            InMemory |            Async_DeleteWithPredicate |     6,023.4 us |    5 |
| EntityFrameworkCore |                    Async_DeleteRange |     9,276.6 us |    6 |
|     EntityFramework |                         Async_Delete |     9,636.1 us |    7 |
|              AdoNet |                         Async_Delete |     9,907.2 us |    8 |
|              AdoNet |                    Async_DeleteRange |    10,399.9 us |    9 |
| EntityFrameworkCore |                         Async_Delete |    11,139.1 us |   10 |
|     EntityFramework |            Async_DeleteWithPredicate |    20,970.1 us |   11 |
|              AdoNet |            Async_DeleteWithPredicate |    25,013.2 us |   12 |
|              AdoNet |                   Async_DeleteWithId |    32,850.8 us |   13 |
|     EntityFramework |                   Async_DeleteWithId |    43,004.3 us |   14 |
| EntityFrameworkCore |            Async_DeleteWithPredicate |    57,219.4 us |   15 |
| EntityFrameworkCore |                   Async_DeleteWithId |    69,197.3 us |   16 |
|                     |                                      |                |      |
|            InMemory |            FindAllWithDefaultOptions |     3,307.8 us |    1 |
|            InMemory |                 FindAllWithPredicate |     3,623.0 us |    2 |
|            InMemory |             FindAllWithPagingOptions |     3,663.6 us |    3 |
|     EntityFramework |             FindAllWithPagingOptions |     4,752.2 us |    4 |
|     EntityFramework |                 FindAllWithPredicate |     5,823.1 us |    5 |
|              AdoNet |                 FindAllWithPredicate |     5,867.2 us |    5 |
|              AdoNet |             FindAllWithPagingOptions |     6,751.6 us |    6 |
|              AdoNet |            FindAllWithDefaultOptions |     6,874.2 us |    7 |
|     EntityFramework |            FindAllWithDefaultOptions |    13,334.5 us |    8 |
| EntityFrameworkCore |             FindAllWithPagingOptions |    15,443.9 us |    9 |
| EntityFrameworkCore |            FindAllWithDefaultOptions |    25,257.5 us |   10 |
| EntityFrameworkCore |                 FindAllWithPredicate |    69,913.0 us |   11 |
|                     |                                      |                |      |
|            InMemory |                     Async_FindWithId |       753.8 us |    1 |
|            InMemory |          Async_FindWithPagingOptions |     2,163.8 us |    2 |
|            InMemory |         Async_FindWithDefaultOptions |     2,286.6 us |    3 |
|            InMemory |              Async_FindWithPredicate |     2,451.9 us |    4 |
|     EntityFramework |         Async_FindWithDefaultOptions |     2,889.7 us |    5 |
|     EntityFramework |          Async_FindWithPagingOptions |     3,071.4 us |    6 |
|     EntityFramework |                     Async_FindWithId |     3,369.5 us |    7 |
| EntityFrameworkCore |                     Async_FindWithId |     3,964.6 us |    8 |
| EntityFrameworkCore |          Async_FindWithPagingOptions |     5,006.5 us |    9 |
|              AdoNet |          Async_FindWithPagingOptions |     7,439.4 us |   10 |
|              AdoNet |                     Async_FindWithId |     7,567.1 us |   11 |
|              AdoNet |         Async_FindWithDefaultOptions |     8,288.1 us |   12 |
|              AdoNet |              Async_FindWithPredicate |    10,975.6 us |   13 |
| EntityFrameworkCore |         Async_FindWithDefaultOptions |    12,627.9 us |   14 |
| EntityFrameworkCore |              Async_FindWithPredicate |    14,109.6 us |   15 |
|     EntityFramework |              Async_FindWithPredicate |    35,356.7 us |   16 |
|                     |                                      |                |      |
|            InMemory |           Async_FindAllWithPredicate |     3,710.0 us |    1 |
|            InMemory |       Async_FindAllWithPagingOptions |     3,866.5 us |    2 |
|            InMemory |      Async_FindAllWithDefaultOptions |     3,965.7 us |    3 |
|     EntityFramework |           Async_FindAllWithPredicate |     4,017.5 us |    4 |
|     EntityFramework |      Async_FindAllWithDefaultOptions |     4,906.1 us |    5 |
|     EntityFramework |       Async_FindAllWithPagingOptions |     5,815.2 us |    6 |
|              AdoNet |       Async_FindAllWithPagingOptions |     7,898.5 us |    7 |
|              AdoNet |           Async_FindAllWithPredicate |     8,291.0 us |    8 |
|              AdoNet |      Async_FindAllWithDefaultOptions |     8,438.6 us |    9 |
| EntityFrameworkCore |       Async_FindAllWithPagingOptions |    16,297.0 us |   10 |
| EntityFrameworkCore |      Async_FindAllWithDefaultOptions |    32,087.6 us |   11 |
| EntityFrameworkCore |           Async_FindAllWithPredicate |    90,902.4 us |   12 |
|                     |                                      |                |      |
|            InMemory |      Async_GroupByWithDefaultOptions |     6,851.4 us |    1 |
|            InMemory |       Async_GroupByWithPagingOptions |     7,263.9 us |    2 |
|     EntityFramework |      Async_GroupByWithDefaultOptions |     7,644.4 us |    3 |
|     EntityFramework |       Async_GroupByWithPagingOptions |     7,676.0 us |    3 |
| EntityFrameworkCore |       Async_GroupByWithPagingOptions |     9,044.8 us |    4 |
|              AdoNet |       Async_GroupByWithPagingOptions |     9,722.6 us |    5 |
| EntityFrameworkCore |      Async_GroupByWithDefaultOptions |    10,171.7 us |    6 |
|              AdoNet |      Async_GroupByWithDefaultOptions |    15,352.6 us |    7 |
|                     |                                      |                |      |
|            InMemory |  Async_ToDictionaryWithPagingOptions |     6,687.3 us |    1 |
|            InMemory | Async_ToDictionaryWithDefaultOptions |     7,394.1 us |    2 |
|              AdoNet |  Async_ToDictionaryWithPagingOptions |     8,082.2 us |    3 |
| EntityFrameworkCore |  Async_ToDictionaryWithPagingOptions |     9,317.3 us |    4 |
|     EntityFramework |  Async_ToDictionaryWithPagingOptions |     9,402.6 us |    4 |
|              AdoNet | Async_ToDictionaryWithDefaultOptions |    18,149.0 us |    5 |
|     EntityFramework | Async_ToDictionaryWithDefaultOptions |    24,162.4 us |    6 |
| EntityFrameworkCore | Async_ToDictionaryWithDefaultOptions |    39,547.4 us |    7 |