``` ini

BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19043.1348 (21H1/May2021Update)
AMD Ryzen 9 3900X, 1 CPU, 24 logical and 12 physical cores
  [Host]     : .NET Framework 4.8 (4.8.4420.0), X86 LegacyJIT
  Job-FTOBUC : .NET Framework 4.8 (4.8.4420.0), X86 LegacyJIT
  DefaultJob : .NET Framework 4.8 (4.8.4420.0), X86 LegacyJIT

InvocationCount=1  

```
|                             Namespace |                    Type |                Method |        Job | UnrollFactor |             CompArr |   ReadWrite |        Mean |     Error |   StdDev |
|-------------------------------------- |------------------------ |---------------------- |----------- |------------- |-------------------- |------------ |------------:|----------:|---------:|
|  **EcsLte.BenchmarkTest.EcsContextTests** | **EcsContext_CreateEntity** |        **CreateEntities** | **Job-FTOBUC** |            **1** |           **Normal_x1** |           **?** |    **29.84 ms** |  **0.220 ms** | **0.195 ms** |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_CreateEntity |        CreateEntities | Job-FTOBUC |            1 |           Normal_x2 |           ? |    26.01 ms |  0.171 ms | 0.160 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_CreateEntity |        CreateEntities | Job-FTOBUC |            1 |           Shared_x1 |           ? |    25.04 ms |  0.127 ms | 0.113 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_CreateEntity |        CreateEntities | Job-FTOBUC |            1 |           Shared_x2 |           ? |    30.99 ms |  0.613 ms | 0.602 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_CreateEntity |        CreateEntities | Job-FTOBUC |            1 | Normal_x1_Shared_x1 |           ? |    30.50 ms |  0.180 ms | 0.159 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_CreateEntity |        CreateEntities | Job-FTOBUC |            1 | Normal_x1_Shared_x2 |           ? |    31.44 ms |  0.161 ms | 0.134 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_CreateEntity |        CreateEntities | Job-FTOBUC |            1 | Normal_x2_Shared_x1 |           ? |    30.90 ms |  0.179 ms | 0.150 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_CreateEntity |        CreateEntities | Job-FTOBUC |            1 | Normal_x2_Shared_x2 |           ? |    24.98 ms |  0.180 ms | 0.150 ms |
|                                       |                         |                       |            |              |                     |             |             |           |          |
|  **EcsLte.BenchmarkTest.EcsContextTests** | **EcsContext_CreateEntity** |  **CreateEntities_Reuse** | **Job-FTOBUC** |            **1** |           **Normal_x1** |           **?** |    **28.55 ms** |  **0.548 ms** | **0.609 ms** |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_CreateEntity |  CreateEntities_Reuse | Job-FTOBUC |            1 |           Normal_x2 |           ? |    29.30 ms |  0.570 ms | 0.634 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_CreateEntity |  CreateEntities_Reuse | Job-FTOBUC |            1 |           Shared_x1 |           ? |    28.46 ms |  0.568 ms | 0.558 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_CreateEntity |  CreateEntities_Reuse | Job-FTOBUC |            1 |           Shared_x2 |           ? |    29.33 ms |  0.561 ms | 0.576 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_CreateEntity |  CreateEntities_Reuse | Job-FTOBUC |            1 | Normal_x1_Shared_x1 |           ? |    29.04 ms |  0.578 ms | 0.568 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_CreateEntity |  CreateEntities_Reuse | Job-FTOBUC |            1 | Normal_x1_Shared_x2 |           ? |    29.06 ms |  0.578 ms | 0.593 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_CreateEntity |  CreateEntities_Reuse | Job-FTOBUC |            1 | Normal_x2_Shared_x1 |           ? |    24.39 ms |  0.477 ms | 0.531 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_CreateEntity |  CreateEntities_Reuse | Job-FTOBUC |            1 | Normal_x2_Shared_x2 |           ? |    29.15 ms |  0.571 ms | 0.586 ms |
|                                       |                         |                       |            |              |                     |             |             |           |          |
|  **EcsLte.BenchmarkTest.EcsContextTests** | **EcsContext_CreateEntity** |          **CreateEntity** | **Job-FTOBUC** |            **1** |           **Normal_x1** |           **?** |    **79.35 ms** |  **0.681 ms** | **0.637 ms** |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_CreateEntity |          CreateEntity | Job-FTOBUC |            1 |           Normal_x2 |           ? |    90.30 ms |  0.500 ms | 0.467 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_CreateEntity |          CreateEntity | Job-FTOBUC |            1 |           Shared_x1 |           ? |    79.88 ms |  0.591 ms | 0.553 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_CreateEntity |          CreateEntity | Job-FTOBUC |            1 |           Shared_x2 |           ? |    90.59 ms |  0.779 ms | 0.729 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_CreateEntity |          CreateEntity | Job-FTOBUC |            1 | Normal_x1_Shared_x1 |           ? |    89.86 ms |  0.254 ms | 0.225 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_CreateEntity |          CreateEntity | Job-FTOBUC |            1 | Normal_x1_Shared_x2 |           ? |    99.83 ms |  0.700 ms | 0.585 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_CreateEntity |          CreateEntity | Job-FTOBUC |            1 | Normal_x2_Shared_x1 |           ? |    97.58 ms |  0.758 ms | 0.709 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_CreateEntity |          CreateEntity | Job-FTOBUC |            1 | Normal_x2_Shared_x2 |           ? |   105.32 ms |  0.982 ms | 0.919 ms |
|                                       |                         |                       |            |              |                     |             |             |           |          |
|  **EcsLte.BenchmarkTest.EcsContextTests** | **EcsContext_CreateEntity** |    **CreateEntity_Reuse** | **Job-FTOBUC** |            **1** |           **Normal_x1** |           **?** |    **68.51 ms** |  **0.221 ms** | **0.196 ms** |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_CreateEntity |    CreateEntity_Reuse | Job-FTOBUC |            1 |           Normal_x2 |           ? |    77.70 ms |  0.262 ms | 0.232 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_CreateEntity |    CreateEntity_Reuse | Job-FTOBUC |            1 |           Shared_x1 |           ? |    77.02 ms |  0.566 ms | 0.530 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_CreateEntity |    CreateEntity_Reuse | Job-FTOBUC |            1 |           Shared_x2 |           ? |    76.90 ms |  0.098 ms | 0.082 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_CreateEntity |    CreateEntity_Reuse | Job-FTOBUC |            1 | Normal_x1_Shared_x1 |           ? |    77.21 ms |  0.212 ms | 0.198 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_CreateEntity |    CreateEntity_Reuse | Job-FTOBUC |            1 | Normal_x1_Shared_x2 |           ? |    83.88 ms |  0.172 ms | 0.153 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_CreateEntity |    CreateEntity_Reuse | Job-FTOBUC |            1 | Normal_x2_Shared_x1 |           ? |    83.11 ms |  0.404 ms | 0.337 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_CreateEntity |    CreateEntity_Reuse | Job-FTOBUC |            1 | Normal_x2_Shared_x2 |           ? |    88.09 ms |  0.397 ms | 0.372 ms |
|                                       |                         |                       |            |              |                     |             |             |           |          |
|  **EcsLte.BenchmarkTest.EcsContextTests** | **EcsContext_CreateEntity** |       **DestroyEntities** | **Job-FTOBUC** |            **1** |           **Normal_x1** |           **?** |    **37.46 ms** |  **0.200 ms** | **0.178 ms** |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_CreateEntity |       DestroyEntities | Job-FTOBUC |            1 |           Normal_x2 |           ? |    39.25 ms |  0.163 ms | 0.144 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_CreateEntity |       DestroyEntities | Job-FTOBUC |            1 |           Shared_x1 |           ? |    37.45 ms |  0.230 ms | 0.204 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_CreateEntity |       DestroyEntities | Job-FTOBUC |            1 |           Shared_x2 |           ? |    42.25 ms |  0.234 ms | 0.182 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_CreateEntity |       DestroyEntities | Job-FTOBUC |            1 | Normal_x1_Shared_x1 |           ? |    39.57 ms |  0.407 ms | 0.381 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_CreateEntity |       DestroyEntities | Job-FTOBUC |            1 | Normal_x1_Shared_x2 |           ? |    40.75 ms |  0.171 ms | 0.152 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_CreateEntity |       DestroyEntities | Job-FTOBUC |            1 | Normal_x2_Shared_x1 |           ? |    42.02 ms |  0.821 ms | 0.768 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_CreateEntity |       DestroyEntities | Job-FTOBUC |            1 | Normal_x2_Shared_x2 |           ? |    42.29 ms |  0.270 ms | 0.239 ms |
|                                       |                         |                       |            |              |                     |             |             |           |          |
|  **EcsLte.BenchmarkTest.EcsContextTests** | **EcsContext_CreateEntity** | **DestroyEntities_Reuse** | **Job-FTOBUC** |            **1** |           **Normal_x1** |           **?** |    **33.58 ms** |  **0.105 ms** | **0.093 ms** |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_CreateEntity | DestroyEntities_Reuse | Job-FTOBUC |            1 |           Normal_x2 |           ? |    35.43 ms |  0.169 ms | 0.150 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_CreateEntity | DestroyEntities_Reuse | Job-FTOBUC |            1 |           Shared_x1 |           ? |    33.47 ms |  0.069 ms | 0.061 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_CreateEntity | DestroyEntities_Reuse | Job-FTOBUC |            1 |           Shared_x2 |           ? |    35.81 ms |  0.079 ms | 0.070 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_CreateEntity | DestroyEntities_Reuse | Job-FTOBUC |            1 | Normal_x1_Shared_x1 |           ? |    35.33 ms |  0.102 ms | 0.095 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_CreateEntity | DestroyEntities_Reuse | Job-FTOBUC |            1 | Normal_x1_Shared_x2 |           ? |    36.77 ms |  0.141 ms | 0.132 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_CreateEntity | DestroyEntities_Reuse | Job-FTOBUC |            1 | Normal_x2_Shared_x1 |           ? |    37.27 ms |  0.462 ms | 0.432 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_CreateEntity | DestroyEntities_Reuse | Job-FTOBUC |            1 | Normal_x2_Shared_x2 |           ? |    38.90 ms |  0.130 ms | 0.115 ms |
|                                       |                         |                       |            |              |                     |             |             |           |          |
|  **EcsLte.BenchmarkTest.EcsContextTests** | **EcsContext_CreateEntity** |         **DestroyEntity** | **Job-FTOBUC** |            **1** |           **Normal_x1** |           **?** |    **41.81 ms** |  **0.250 ms** | **0.222 ms** |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_CreateEntity |         DestroyEntity | Job-FTOBUC |            1 |           Normal_x2 |           ? |    44.15 ms |  0.345 ms | 0.306 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_CreateEntity |         DestroyEntity | Job-FTOBUC |            1 |           Shared_x1 |           ? |    42.21 ms |  0.778 ms | 0.728 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_CreateEntity |         DestroyEntity | Job-FTOBUC |            1 |           Shared_x2 |           ? |    44.18 ms |  0.248 ms | 0.220 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_CreateEntity |         DestroyEntity | Job-FTOBUC |            1 | Normal_x1_Shared_x1 |           ? |    44.67 ms |  0.425 ms | 0.397 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_CreateEntity |         DestroyEntity | Job-FTOBUC |            1 | Normal_x1_Shared_x2 |           ? |    46.10 ms |  0.207 ms | 0.193 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_CreateEntity |         DestroyEntity | Job-FTOBUC |            1 | Normal_x2_Shared_x1 |           ? |    45.95 ms |  0.287 ms | 0.269 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_CreateEntity |         DestroyEntity | Job-FTOBUC |            1 | Normal_x2_Shared_x2 |           ? |    47.27 ms |  0.170 ms | 0.142 ms |
|                                       |                         |                       |            |              |                     |             |             |           |          |
|  **EcsLte.BenchmarkTest.EcsContextTests** | **EcsContext_CreateEntity** |   **DestroyEntity_Reuse** | **Job-FTOBUC** |            **1** |           **Normal_x1** |           **?** |    **37.70 ms** |  **0.146 ms** | **0.130 ms** |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_CreateEntity |   DestroyEntity_Reuse | Job-FTOBUC |            1 |           Normal_x2 |           ? |    39.94 ms |  0.144 ms | 0.128 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_CreateEntity |   DestroyEntity_Reuse | Job-FTOBUC |            1 |           Shared_x1 |           ? |    38.21 ms |  0.469 ms | 0.392 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_CreateEntity |   DestroyEntity_Reuse | Job-FTOBUC |            1 |           Shared_x2 |           ? |    39.93 ms |  0.080 ms | 0.066 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_CreateEntity |   DestroyEntity_Reuse | Job-FTOBUC |            1 | Normal_x1_Shared_x1 |           ? |    40.50 ms |  0.330 ms | 0.309 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_CreateEntity |   DestroyEntity_Reuse | Job-FTOBUC |            1 | Normal_x1_Shared_x2 |           ? |    41.76 ms |  0.205 ms | 0.181 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_CreateEntity |   DestroyEntity_Reuse | Job-FTOBUC |            1 | Normal_x2_Shared_x1 |           ? |    41.76 ms |  0.120 ms | 0.107 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_CreateEntity |   DestroyEntity_Reuse | Job-FTOBUC |            1 | Normal_x2_Shared_x2 |           ? |    43.41 ms |  0.215 ms | 0.180 ms |
|                                       |                         |                       |            |              |                     |             |             |           |          |
| **EcsLte.BenchmarkTest.EntityQueryTests** |     **EntityQuery_ForEach** |               **ForEach** | **DefaultJob** |           **16** |                   **?** |        **R0W0** |    **24.07 ms** |  **0.070 ms** | **0.066 ms** |
| EcsLte.BenchmarkTest.EntityQueryTests |     EntityQuery_ForEach |               ForEach | DefaultJob |           16 |                   ? | R0W1_Normal |    67.91 ms |  0.318 ms | 0.298 ms |
| EcsLte.BenchmarkTest.EntityQueryTests |     EntityQuery_ForEach |               ForEach | DefaultJob |           16 |                   ? | R0W2_Normal |   112.46 ms |  0.524 ms | 0.409 ms |
| EcsLte.BenchmarkTest.EntityQueryTests |     EntityQuery_ForEach |               ForEach | DefaultJob |           16 |                   ? | R0W1_Shared |    68.05 ms |  0.192 ms | 0.179 ms |
| EcsLte.BenchmarkTest.EntityQueryTests |     EntityQuery_ForEach |               ForEach | DefaultJob |           16 |                   ? | R0W2_Shared | 1,160.59 ms | 10.960 ms | 9.152 ms |
| EcsLte.BenchmarkTest.EntityQueryTests |     EntityQuery_ForEach |               ForEach | DefaultJob |           16 |                   ? | R1W0_Normal |    24.79 ms |  0.205 ms | 0.182 ms |
| EcsLte.BenchmarkTest.EntityQueryTests |     EntityQuery_ForEach |               ForEach | DefaultJob |           16 |                   ? | R1W1_Normal |    69.21 ms |  0.585 ms | 0.519 ms |
| EcsLte.BenchmarkTest.EntityQueryTests |     EntityQuery_ForEach |               ForEach | DefaultJob |           16 |                   ? | R1W0_Shared |    24.67 ms |  0.080 ms | 0.075 ms |
| EcsLte.BenchmarkTest.EntityQueryTests |     EntityQuery_ForEach |               ForEach | DefaultJob |           16 |                   ? | R1W1_Shared |   605.90 ms |  6.946 ms | 6.158 ms |
| EcsLte.BenchmarkTest.EntityQueryTests |     EntityQuery_ForEach |               ForEach | DefaultJob |           16 |                   ? | R2W0_Normal |    25.96 ms |  0.077 ms | 0.060 ms |
| EcsLte.BenchmarkTest.EntityQueryTests |     EntityQuery_ForEach |               ForEach | DefaultJob |           16 |                   ? | R2W0_Shared |    25.92 ms |  0.127 ms | 0.118 ms |
