``` ini

BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19043.1348 (21H1/May2021Update)
AMD Ryzen 9 3900X, 1 CPU, 24 logical and 12 physical cores
  [Host]     : .NET Framework 4.8 (4.8.4420.0), X86 LegacyJIT
  Job-FTOBUC : .NET Framework 4.8 (4.8.4420.0), X86 LegacyJIT
  DefaultJob : .NET Framework 4.8 (4.8.4420.0), X86 LegacyJIT

InvocationCount=1  

```
|                             Namespace |                    Type |                Method |        Job | UnrollFactor |             CompArr |   ReadWrite |        Mean |    Error |   StdDev |
|-------------------------------------- |------------------------ |---------------------- |----------- |------------- |-------------------- |------------ |------------:|---------:|---------:|
|  **EcsLte.BenchmarkTest.EcsContextTests** | **EcsContext_CreateEntity** |        **CreateEntities** | **Job-FTOBUC** |            **1** |           **Normal_x1** |           **?** |    **33.14 ms** | **0.345 ms** | **0.322 ms** |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_CreateEntity |        CreateEntities | Job-FTOBUC |            1 |           Normal_x2 |           ? |    34.10 ms | 0.226 ms | 0.189 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_CreateEntity |        CreateEntities | Job-FTOBUC |            1 |           Shared_x1 |           ? |    32.87 ms | 0.643 ms | 0.765 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_CreateEntity |        CreateEntities | Job-FTOBUC |            1 |           Shared_x2 |           ? |    34.44 ms | 0.332 ms | 0.278 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_CreateEntity |        CreateEntities | Job-FTOBUC |            1 | Normal_x1_Shared_x1 |           ? |    34.09 ms | 0.186 ms | 0.165 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_CreateEntity |        CreateEntities | Job-FTOBUC |            1 | Normal_x1_Shared_x2 |           ? |    35.53 ms | 0.250 ms | 0.221 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_CreateEntity |        CreateEntities | Job-FTOBUC |            1 | Normal_x2_Shared_x1 |           ? |    35.94 ms | 0.719 ms | 1.240 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_CreateEntity |        CreateEntities | Job-FTOBUC |            1 | Normal_x2_Shared_x2 |           ? |    32.37 ms | 0.638 ms | 0.894 ms |
|                                       |                         |                       |            |              |                     |             |             |          |          |
|  **EcsLte.BenchmarkTest.EcsContextTests** | **EcsContext_CreateEntity** |  **CreateEntities_Reuse** | **Job-FTOBUC** |            **1** |           **Normal_x1** |           **?** |    **29.20 ms** | **0.577 ms** | **0.915 ms** |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_CreateEntity |  CreateEntities_Reuse | Job-FTOBUC |            1 |           Normal_x2 |           ? |    29.72 ms | 0.570 ms | 0.657 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_CreateEntity |  CreateEntities_Reuse | Job-FTOBUC |            1 |           Shared_x1 |           ? |    28.94 ms | 0.564 ms | 0.672 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_CreateEntity |  CreateEntities_Reuse | Job-FTOBUC |            1 |           Shared_x2 |           ? |    30.32 ms | 0.584 ms | 0.673 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_CreateEntity |  CreateEntities_Reuse | Job-FTOBUC |            1 | Normal_x1_Shared_x1 |           ? |    29.73 ms | 0.594 ms | 0.610 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_CreateEntity |  CreateEntities_Reuse | Job-FTOBUC |            1 | Normal_x1_Shared_x2 |           ? |    29.16 ms | 0.569 ms | 0.584 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_CreateEntity |  CreateEntities_Reuse | Job-FTOBUC |            1 | Normal_x2_Shared_x1 |           ? |    29.18 ms | 0.574 ms | 0.683 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_CreateEntity |  CreateEntities_Reuse | Job-FTOBUC |            1 | Normal_x2_Shared_x2 |           ? |    24.08 ms | 0.473 ms | 0.790 ms |
|                                       |                         |                       |            |              |                     |             |             |          |          |
|  **EcsLte.BenchmarkTest.EcsContextTests** | **EcsContext_CreateEntity** |          **CreateEntity** | **Job-FTOBUC** |            **1** |           **Normal_x1** |           **?** |   **120.48 ms** | **2.356 ms** | **2.805 ms** |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_CreateEntity |          CreateEntity | Job-FTOBUC |            1 |           Normal_x2 |           ? |   140.15 ms | 2.654 ms | 2.607 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_CreateEntity |          CreateEntity | Job-FTOBUC |            1 |           Shared_x1 |           ? |   121.08 ms | 1.544 ms | 1.369 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_CreateEntity |          CreateEntity | Job-FTOBUC |            1 |           Shared_x2 |           ? |   141.82 ms | 2.270 ms | 3.030 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_CreateEntity |          CreateEntity | Job-FTOBUC |            1 | Normal_x1_Shared_x1 |           ? |   141.38 ms | 1.799 ms | 1.502 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_CreateEntity |          CreateEntity | Job-FTOBUC |            1 | Normal_x1_Shared_x2 |           ? |   156.89 ms | 2.483 ms | 2.323 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_CreateEntity |          CreateEntity | Job-FTOBUC |            1 | Normal_x2_Shared_x1 |           ? |   157.37 ms | 2.622 ms | 2.325 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_CreateEntity |          CreateEntity | Job-FTOBUC |            1 | Normal_x2_Shared_x2 |           ? |   176.16 ms | 2.859 ms | 2.674 ms |
|                                       |                         |                       |            |              |                     |             |             |          |          |
|  **EcsLte.BenchmarkTest.EcsContextTests** | **EcsContext_CreateEntity** |    **CreateEntity_Reuse** | **Job-FTOBUC** |            **1** |           **Normal_x1** |           **?** |   **105.97 ms** | **0.678 ms** | **0.601 ms** |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_CreateEntity |    CreateEntity_Reuse | Job-FTOBUC |            1 |           Normal_x2 |           ? |   121.50 ms | 0.242 ms | 0.215 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_CreateEntity |    CreateEntity_Reuse | Job-FTOBUC |            1 |           Shared_x1 |           ? |   104.20 ms | 0.527 ms | 0.493 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_CreateEntity |    CreateEntity_Reuse | Job-FTOBUC |            1 |           Shared_x2 |           ? |   121.53 ms | 0.572 ms | 0.507 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_CreateEntity |    CreateEntity_Reuse | Job-FTOBUC |            1 | Normal_x1_Shared_x1 |           ? |   121.91 ms | 0.554 ms | 0.518 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_CreateEntity |    CreateEntity_Reuse | Job-FTOBUC |            1 | Normal_x1_Shared_x2 |           ? |   134.95 ms | 0.847 ms | 0.751 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_CreateEntity |    CreateEntity_Reuse | Job-FTOBUC |            1 | Normal_x2_Shared_x1 |           ? |   137.52 ms | 0.539 ms | 0.504 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_CreateEntity |    CreateEntity_Reuse | Job-FTOBUC |            1 | Normal_x2_Shared_x2 |           ? |   152.06 ms | 0.422 ms | 0.374 ms |
|                                       |                         |                       |            |              |                     |             |             |          |          |
|  **EcsLte.BenchmarkTest.EcsContextTests** | **EcsContext_CreateEntity** |       **DestroyEntities** | **Job-FTOBUC** |            **1** |           **Normal_x1** |           **?** |    **47.55 ms** | **0.576 ms** | **0.539 ms** |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_CreateEntity |       DestroyEntities | Job-FTOBUC |            1 |           Normal_x2 |           ? |    50.25 ms | 0.396 ms | 0.331 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_CreateEntity |       DestroyEntities | Job-FTOBUC |            1 |           Shared_x1 |           ? |    47.83 ms | 0.404 ms | 0.358 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_CreateEntity |       DestroyEntities | Job-FTOBUC |            1 |           Shared_x2 |           ? |    50.56 ms | 0.639 ms | 0.566 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_CreateEntity |       DestroyEntities | Job-FTOBUC |            1 | Normal_x1_Shared_x1 |           ? |    50.44 ms | 0.624 ms | 0.521 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_CreateEntity |       DestroyEntities | Job-FTOBUC |            1 | Normal_x1_Shared_x2 |           ? |    52.25 ms | 0.456 ms | 0.405 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_CreateEntity |       DestroyEntities | Job-FTOBUC |            1 | Normal_x2_Shared_x1 |           ? |    52.25 ms | 0.570 ms | 0.533 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_CreateEntity |       DestroyEntities | Job-FTOBUC |            1 | Normal_x2_Shared_x2 |           ? |    54.02 ms | 0.759 ms | 0.710 ms |
|                                       |                         |                       |            |              |                     |             |             |          |          |
|  **EcsLte.BenchmarkTest.EcsContextTests** | **EcsContext_CreateEntity** | **DestroyEntities_Reuse** | **Job-FTOBUC** |            **1** |           **Normal_x1** |           **?** |    **42.48 ms** | **0.194 ms** | **0.182 ms** |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_CreateEntity | DestroyEntities_Reuse | Job-FTOBUC |            1 |           Normal_x2 |           ? |    45.06 ms | 0.206 ms | 0.182 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_CreateEntity | DestroyEntities_Reuse | Job-FTOBUC |            1 |           Shared_x1 |           ? |    42.65 ms | 0.239 ms | 0.212 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_CreateEntity | DestroyEntities_Reuse | Job-FTOBUC |            1 |           Shared_x2 |           ? |    45.12 ms | 0.128 ms | 0.113 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_CreateEntity | DestroyEntities_Reuse | Job-FTOBUC |            1 | Normal_x1_Shared_x1 |           ? |    45.04 ms | 0.193 ms | 0.171 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_CreateEntity | DestroyEntities_Reuse | Job-FTOBUC |            1 | Normal_x1_Shared_x2 |           ? |    46.79 ms | 0.211 ms | 0.187 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_CreateEntity | DestroyEntities_Reuse | Job-FTOBUC |            1 | Normal_x2_Shared_x1 |           ? |    46.81 ms | 0.191 ms | 0.179 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_CreateEntity | DestroyEntities_Reuse | Job-FTOBUC |            1 | Normal_x2_Shared_x2 |           ? |    48.30 ms | 0.275 ms | 0.257 ms |
|                                       |                         |                       |            |              |                     |             |             |          |          |
|  **EcsLte.BenchmarkTest.EcsContextTests** | **EcsContext_CreateEntity** |         **DestroyEntity** | **Job-FTOBUC** |            **1** |           **Normal_x1** |           **?** |    **59.96 ms** | **0.633 ms** | **0.592 ms** |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_CreateEntity |         DestroyEntity | Job-FTOBUC |            1 |           Normal_x2 |           ? |    61.31 ms | 0.479 ms | 0.448 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_CreateEntity |         DestroyEntity | Job-FTOBUC |            1 |           Shared_x1 |           ? |    59.20 ms | 0.745 ms | 0.697 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_CreateEntity |         DestroyEntity | Job-FTOBUC |            1 |           Shared_x2 |           ? |    61.34 ms | 0.555 ms | 0.492 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_CreateEntity |         DestroyEntity | Job-FTOBUC |            1 | Normal_x1_Shared_x1 |           ? |    61.53 ms | 0.501 ms | 0.418 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_CreateEntity |         DestroyEntity | Job-FTOBUC |            1 | Normal_x1_Shared_x2 |           ? |    63.86 ms | 0.602 ms | 0.563 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_CreateEntity |         DestroyEntity | Job-FTOBUC |            1 | Normal_x2_Shared_x1 |           ? |    63.24 ms | 0.746 ms | 0.582 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_CreateEntity |         DestroyEntity | Job-FTOBUC |            1 | Normal_x2_Shared_x2 |           ? |    64.69 ms | 0.617 ms | 0.547 ms |
|                                       |                         |                       |            |              |                     |             |             |          |          |
|  **EcsLte.BenchmarkTest.EcsContextTests** | **EcsContext_CreateEntity** |   **DestroyEntity_Reuse** | **Job-FTOBUC** |            **1** |           **Normal_x1** |           **?** |    **53.69 ms** | **0.165 ms** | **0.146 ms** |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_CreateEntity |   DestroyEntity_Reuse | Job-FTOBUC |            1 |           Normal_x2 |           ? |    55.97 ms | 0.194 ms | 0.181 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_CreateEntity |   DestroyEntity_Reuse | Job-FTOBUC |            1 |           Shared_x1 |           ? |    53.72 ms | 0.262 ms | 0.233 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_CreateEntity |   DestroyEntity_Reuse | Job-FTOBUC |            1 |           Shared_x2 |           ? |    56.06 ms | 0.262 ms | 0.245 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_CreateEntity |   DestroyEntity_Reuse | Job-FTOBUC |            1 | Normal_x1_Shared_x1 |           ? |    56.14 ms | 0.174 ms | 0.154 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_CreateEntity |   DestroyEntity_Reuse | Job-FTOBUC |            1 | Normal_x1_Shared_x2 |           ? |    58.55 ms | 0.479 ms | 0.400 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_CreateEntity |   DestroyEntity_Reuse | Job-FTOBUC |            1 | Normal_x2_Shared_x1 |           ? |    57.66 ms | 0.227 ms | 0.190 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_CreateEntity |   DestroyEntity_Reuse | Job-FTOBUC |            1 | Normal_x2_Shared_x2 |           ? |    59.40 ms | 0.258 ms | 0.229 ms |
|                                       |                         |                       |            |              |                     |             |             |          |          |
| **EcsLte.BenchmarkTest.EntityQueryTests** |     **EntityQuery_ForEach** |               **ForEach** | **DefaultJob** |           **16** |                   **?** |        **R0W0** |    **25.03 ms** | **0.199 ms** | **0.176 ms** |
| EcsLte.BenchmarkTest.EntityQueryTests |     EntityQuery_ForEach |               ForEach | DefaultJob |           16 |                   ? | R0W1_Normal |   298.80 ms | 1.225 ms | 1.146 ms |
| EcsLte.BenchmarkTest.EntityQueryTests |     EntityQuery_ForEach |               ForEach | DefaultJob |           16 |                   ? | R0W2_Normal |   384.46 ms | 3.854 ms | 3.605 ms |
| EcsLte.BenchmarkTest.EntityQueryTests |     EntityQuery_ForEach |               ForEach | DefaultJob |           16 |                   ? | R0W1_Shared |   303.57 ms | 3.285 ms | 3.073 ms |
| EcsLte.BenchmarkTest.EntityQueryTests |     EntityQuery_ForEach |               ForEach | DefaultJob |           16 |                   ? | R0W2_Shared | 1,936.54 ms | 6.134 ms | 5.438 ms |
| EcsLte.BenchmarkTest.EntityQueryTests |     EntityQuery_ForEach |               ForEach | DefaultJob |           16 |                   ? | R1W0_Normal |    24.54 ms | 0.139 ms | 0.123 ms |
| EcsLte.BenchmarkTest.EntityQueryTests |     EntityQuery_ForEach |               ForEach | DefaultJob |           16 |                   ? | R1W1_Normal |   300.55 ms | 1.384 ms | 1.294 ms |
| EcsLte.BenchmarkTest.EntityQueryTests |     EntityQuery_ForEach |               ForEach | DefaultJob |           16 |                   ? | R1W0_Shared |    24.48 ms | 0.074 ms | 0.065 ms |
| EcsLte.BenchmarkTest.EntityQueryTests |     EntityQuery_ForEach |               ForEach | DefaultJob |           16 |                   ? | R1W1_Shared | 1,068.39 ms | 3.864 ms | 3.227 ms |
| EcsLte.BenchmarkTest.EntityQueryTests |     EntityQuery_ForEach |               ForEach | DefaultJob |           16 |                   ? | R2W0_Normal |    25.33 ms | 0.077 ms | 0.072 ms |
| EcsLte.BenchmarkTest.EntityQueryTests |     EntityQuery_ForEach |               ForEach | DefaultJob |           16 |                   ? | R2W0_Shared |    25.33 ms | 0.058 ms | 0.052 ms |
