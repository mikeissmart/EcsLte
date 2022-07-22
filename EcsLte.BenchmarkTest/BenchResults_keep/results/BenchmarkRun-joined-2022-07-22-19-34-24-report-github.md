``` ini

BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19043.1348 (21H1/May2021Update)
AMD Ryzen 9 3900X, 1 CPU, 24 logical and 12 physical cores
  [Host]     : .NET Framework 4.8 (4.8.4420.0), X86 LegacyJIT
  Job-FTOBUC : .NET Framework 4.8 (4.8.4420.0), X86 LegacyJIT

InvocationCount=1  UnrollFactor=1  

```
|                             Namespace |                             Type |                                Method |   CompArr |      ReadWrite |         Mean |     Error |    StdDev |
|-------------------------------------- |--------------------------------- |-------------------------------------- |---------- |--------------- |-------------:|----------:|----------:|
|  **EcsLte.BenchmarkTest.EcsContextTests** |          **EcsContext_GetComponent** |                   **GetComponent_Entity** |         **?** |              **?** |   **328.205 ms** | **1.0821 ms** | **0.9592 ms** |
|                                       |                                  |                                       |           |                |              |           |           |
|  **EcsLte.BenchmarkTest.EcsContextTests** |          **EcsContext_GetComponent** |         **GetComponents_EntityArcheType** |         **?** |              **?** |     **9.983 ms** | **0.1994 ms** | **0.3745 ms** |
|                                       |                                  |                                       |           |                |              |           |           |
|  **EcsLte.BenchmarkTest.EcsContextTests** |          **EcsContext_GetComponent** |             **GetComponents_EntityQuery** |         **?** |              **?** |    **10.029 ms** | **0.2002 ms** | **0.3558 ms** |
|                                       |                                  |                                       |           |                |              |           |           |
|  **EcsLte.BenchmarkTest.EcsContextTests** | **EcsContext_UpdateSharedComponent** | **UpdateSharedComponent_EntityArcheType** |         **?** |              **?** |     **9.306 ms** | **0.0828 ms** | **0.0774 ms** |
|                                       |                                  |                                       |           |                |              |           |           |
|  **EcsLte.BenchmarkTest.EcsContextTests** | **EcsContext_UpdateSharedComponent** |     **UpdateSharedComponent_EntityQuery** |         **?** |              **?** |    **25.272 ms** | **0.1376 ms** | **0.1149 ms** |
|                                       |                                  |                                       |           |                |              |           |           |
|  **EcsLte.BenchmarkTest.EcsContextTests** |          **EcsContext_CreateEntity** |                        **CreateEntities** | **Normal_x4** |              **?** |    **81.787 ms** | **0.2431 ms** | **0.2030 ms** |
|  EcsLte.BenchmarkTest.EcsContextTests |          EcsContext_CreateEntity |                        CreateEntities | Shared_x4 |              ? |    58.360 ms | 0.2449 ms | 0.1912 ms |
|                                       |                                  |                                       |           |                |              |           |           |
|  **EcsLte.BenchmarkTest.EcsContextTests** |          **EcsContext_CreateEntity** |                  **CreateEntities_Reuse** | **Normal_x4** |              **?** |    **76.730 ms** | **0.5489 ms** | **0.5135 ms** |
|  EcsLte.BenchmarkTest.EcsContextTests |          EcsContext_CreateEntity |                  CreateEntities_Reuse | Shared_x4 |              ? |    56.930 ms | 0.9947 ms | 0.9304 ms |
|                                       |                                  |                                       |           |                |              |           |           |
|  **EcsLte.BenchmarkTest.EcsContextTests** |          **EcsContext_CreateEntity** |                          **CreateEntity** | **Normal_x4** |              **?** |   **264.485 ms** | **1.6983 ms** | **1.5886 ms** |
|  EcsLte.BenchmarkTest.EcsContextTests |          EcsContext_CreateEntity |                          CreateEntity | Shared_x4 |              ? |   179.710 ms | 2.6752 ms | 2.5024 ms |
|                                       |                                  |                                       |           |                |              |           |           |
|  **EcsLte.BenchmarkTest.EcsContextTests** |          **EcsContext_CreateEntity** |                    **CreateEntity_Reuse** | **Normal_x4** |              **?** |   **241.094 ms** | **0.7683 ms** | **0.6810 ms** |
|  EcsLte.BenchmarkTest.EcsContextTests |          EcsContext_CreateEntity |                    CreateEntity_Reuse | Shared_x4 |              ? |   158.362 ms | 0.9421 ms | 0.8812 ms |
|                                       |                                  |                                       |           |                |              |           |           |
|  **EcsLte.BenchmarkTest.EcsContextTests** |          **EcsContext_CreateEntity** |                       **DestroyEntities** | **Normal_x4** |              **?** |    **46.998 ms** | **0.4012 ms** | **0.3557 ms** |
|  EcsLte.BenchmarkTest.EcsContextTests |          EcsContext_CreateEntity |                       DestroyEntities | Shared_x4 |              ? |    46.442 ms | 0.4837 ms | 0.4525 ms |
|                                       |                                  |                                       |           |                |              |           |           |
|  **EcsLte.BenchmarkTest.EcsContextTests** |          **EcsContext_CreateEntity** |                 **DestroyEntities_Reuse** | **Normal_x4** |              **?** |    **43.085 ms** | **0.1737 ms** | **0.1625 ms** |
|  EcsLte.BenchmarkTest.EcsContextTests |          EcsContext_CreateEntity |                 DestroyEntities_Reuse | Shared_x4 |              ? |    42.296 ms | 0.1991 ms | 0.1765 ms |
|                                       |                                  |                                       |           |                |              |           |           |
|  **EcsLte.BenchmarkTest.EcsContextTests** |          **EcsContext_CreateEntity** |                         **DestroyEntity** | **Normal_x4** |              **?** |    **60.375 ms** | **0.6421 ms** | **0.6007 ms** |
|  EcsLte.BenchmarkTest.EcsContextTests |          EcsContext_CreateEntity |                         DestroyEntity | Shared_x4 |              ? |    59.174 ms | 0.2904 ms | 0.2425 ms |
|                                       |                                  |                                       |           |                |              |           |           |
|  **EcsLte.BenchmarkTest.EcsContextTests** |          **EcsContext_CreateEntity** |                   **DestroyEntity_Reuse** | **Normal_x4** |              **?** |    **56.268 ms** | **0.2738 ms** | **0.2561 ms** |
|  EcsLte.BenchmarkTest.EcsContextTests |          EcsContext_CreateEntity |                   DestroyEntity_Reuse | Shared_x4 |              ? |    54.146 ms | 0.3837 ms | 0.3402 ms |
|                                       |                                  |                                       |           |                |              |           |           |
|  **EcsLte.BenchmarkTest.EcsContextTests** |              **EcsContext_Transfer** |            **TransferEntities_NoDestroy** | **Normal_x4** |              **?** |   **186.615 ms** | **0.7735 ms** | **0.6856 ms** |
|  EcsLte.BenchmarkTest.EcsContextTests |              EcsContext_Transfer |            TransferEntities_NoDestroy | Shared_x4 |              ? |   346.349 ms | 1.2276 ms | 1.1483 ms |
|                                       |                                  |                                       |           |                |              |           |           |
|  **EcsLte.BenchmarkTest.EcsContextTests** |              **EcsContext_Transfer** |     **TransferEntityArcheType_NoDestroy** | **Normal_x4** |              **?** |    **80.124 ms** | **0.7817 ms** | **0.7312 ms** |
|  EcsLte.BenchmarkTest.EcsContextTests |              EcsContext_Transfer |     TransferEntityArcheType_NoDestroy | Shared_x4 |              ? |    59.741 ms | 0.8909 ms | 0.8333 ms |
|                                       |                                  |                                       |           |                |              |           |           |
|  **EcsLte.BenchmarkTest.EcsContextTests** |              **EcsContext_Transfer** |         **TransferEntityQuery_NoDestroy** | **Normal_x4** |              **?** |    **81.728 ms** | **1.5261 ms** | **1.4275 ms** |
|  EcsLte.BenchmarkTest.EcsContextTests |              EcsContext_Transfer |         TransferEntityQuery_NoDestroy | Shared_x4 |              ? |    59.780 ms | 0.6398 ms | 0.5985 ms |
|                                       |                                  |                                       |           |                |              |           |           |
|  **EcsLte.BenchmarkTest.EcsContextTests** |              **EcsContext_Transfer** |              **TransferEntity_NoDestroy** | **Normal_x4** |              **?** |   **211.017 ms** | **0.8749 ms** | **0.8184 ms** |
|  EcsLte.BenchmarkTest.EcsContextTests |              EcsContext_Transfer |              TransferEntity_NoDestroy | Shared_x4 |              ? |   379.241 ms | 1.2749 ms | 1.1926 ms |
|                                       |                                  |                                       |           |                |              |           |           |
|  **EcsLte.BenchmarkTest.EcsContextTests** |       **EcsContext_UpdateComponent** |                **UpdateComponent_Entity** | **Normal_x4** |              **?** |   **342.315 ms** | **0.9381 ms** | **0.8775 ms** |
|  EcsLte.BenchmarkTest.EcsContextTests |       EcsContext_UpdateComponent |                UpdateComponent_Entity | Shared_x4 |              ? | 1,221.393 ms | 9.4216 ms | 8.8129 ms |
|                                       |                                  |                                       |           |                |              |           |           |
| **EcsLte.BenchmarkTest.EntityQueryTests** |              **EntityQuery_ForEach** |                               **ForEach** |         **?** |           **R0W0** |    **98.479 ms** | **0.6759 ms** | **0.6322 ms** |
| EcsLte.BenchmarkTest.EntityQueryTests |              EntityQuery_ForEach |                               ForEach |         ? | R0W4_Normal_x4 |   302.455 ms | 1.2713 ms | 1.1892 ms |
| EcsLte.BenchmarkTest.EntityQueryTests |              EntityQuery_ForEach |                               ForEach |         ? | R4W0_Normal_x4 |   197.857 ms | 0.9451 ms | 0.8378 ms |
| EcsLte.BenchmarkTest.EntityQueryTests |              EntityQuery_ForEach |                               ForEach |         ? | R0W4_Shared_x4 |   537.055 ms | 6.2652 ms | 5.8605 ms |
| EcsLte.BenchmarkTest.EntityQueryTests |              EntityQuery_ForEach |                               ForEach |         ? | R4W0_Shared_x4 |   212.625 ms | 1.8117 ms | 1.6060 ms |
