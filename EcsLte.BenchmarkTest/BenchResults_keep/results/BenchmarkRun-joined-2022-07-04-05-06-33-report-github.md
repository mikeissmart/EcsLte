``` ini

BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19043.1348 (21H1/May2021Update)
AMD Ryzen 9 3900X, 1 CPU, 24 logical and 12 physical cores
  [Host]     : .NET Framework 4.8 (4.8.4420.0), X86 LegacyJIT
  Job-FTOBUC : .NET Framework 4.8 (4.8.4420.0), X86 LegacyJIT

InvocationCount=1  UnrollFactor=1  

```
|                             Namespace |                             Type |                                Method |   CompArr |      ReadWrite |        Mean |    Error |   StdDev |
|-------------------------------------- |--------------------------------- |-------------------------------------- |---------- |--------------- |------------:|---------:|---------:|
|  **EcsLte.BenchmarkTest.EcsContextTests** |          **EcsContext_GetComponent** |                   **GetComponent_Entity** |         **?** |              **?** |   **236.00 ms** | **0.461 ms** | **0.409 ms** |
|                                       |                                  |                                       |           |                |             |          |          |
|  **EcsLte.BenchmarkTest.EcsContextTests** |          **EcsContext_GetComponent** |                **GetComponents_Entities** |         **?** |              **?** |   **230.55 ms** | **0.994 ms** | **0.930 ms** |
|                                       |                                  |                                       |           |                |             |          |          |
|  **EcsLte.BenchmarkTest.EcsContextTests** |          **EcsContext_GetComponent** |         **GetComponents_EntityArcheType** |         **?** |              **?** |    **10.13 ms** | **0.200 ms** | **0.312 ms** |
|                                       |                                  |                                       |           |                |             |          |          |
|  **EcsLte.BenchmarkTest.EcsContextTests** |          **EcsContext_GetComponent** |             **GetComponents_EntityQuery** |         **?** |              **?** |    **10.06 ms** | **0.197 ms** | **0.269 ms** |
|                                       |                                  |                                       |           |                |             |          |          |
|  **EcsLte.BenchmarkTest.EcsContextTests** | **EcsContext_UpdateSharedComponent** |        **UpdateSharedComponent_Entities** |         **?** |              **?** |   **693.42 ms** | **0.906 ms** | **0.707 ms** |
|                                       |                                  |                                       |           |                |             |          |          |
|  **EcsLte.BenchmarkTest.EcsContextTests** | **EcsContext_UpdateSharedComponent** | **UpdateSharedComponent_EntityArcheType** |         **?** |              **?** |    **18.26 ms** | **0.131 ms** | **0.123 ms** |
|                                       |                                  |                                       |           |                |             |          |          |
|  **EcsLte.BenchmarkTest.EcsContextTests** | **EcsContext_UpdateSharedComponent** |     **UpdateSharedComponent_EntityQuery** |         **?** |              **?** |    **54.02 ms** | **0.426 ms** | **0.356 ms** |
|                                       |                                  |                                       |           |                |             |          |          |
|  **EcsLte.BenchmarkTest.EcsContextTests** |          **EcsContext_CreateEntity** |                        **CreateEntities** | **Normal_x4** |              **?** |    **79.37 ms** | **0.981 ms** | **0.870 ms** |
|  EcsLte.BenchmarkTest.EcsContextTests |          EcsContext_CreateEntity |                        CreateEntities | Shared_x4 |              ? |    78.71 ms | 0.843 ms | 0.788 ms |
|                                       |                                  |                                       |           |                |             |          |          |
|  **EcsLte.BenchmarkTest.EcsContextTests** |          **EcsContext_CreateEntity** |                  **CreateEntities_Reuse** | **Normal_x4** |              **?** |    **75.21 ms** | **0.872 ms** | **0.773 ms** |
|  EcsLte.BenchmarkTest.EcsContextTests |          EcsContext_CreateEntity |                  CreateEntities_Reuse | Shared_x4 |              ? |    75.24 ms | 0.427 ms | 0.400 ms |
|                                       |                                  |                                       |           |                |             |          |          |
|  **EcsLte.BenchmarkTest.EcsContextTests** |          **EcsContext_CreateEntity** |                          **CreateEntity** | **Normal_x4** |              **?** |   **211.67 ms** | **3.122 ms** | **2.767 ms** |
|  EcsLte.BenchmarkTest.EcsContextTests |          EcsContext_CreateEntity |                          CreateEntity | Shared_x4 |              ? |   209.13 ms | 1.855 ms | 1.644 ms |
|                                       |                                  |                                       |           |                |             |          |          |
|  **EcsLte.BenchmarkTest.EcsContextTests** |          **EcsContext_CreateEntity** |                    **CreateEntity_Reuse** | **Normal_x4** |              **?** |   **180.86 ms** | **0.881 ms** | **0.824 ms** |
|  EcsLte.BenchmarkTest.EcsContextTests |          EcsContext_CreateEntity |                    CreateEntity_Reuse | Shared_x4 |              ? |   182.02 ms | 0.470 ms | 0.439 ms |
|                                       |                                  |                                       |           |                |             |          |          |
|  **EcsLte.BenchmarkTest.EcsContextTests** |          **EcsContext_CreateEntity** |                       **DestroyEntities** | **Normal_x4** |              **?** |    **48.34 ms** | **0.373 ms** | **0.349 ms** |
|  EcsLte.BenchmarkTest.EcsContextTests |          EcsContext_CreateEntity |                       DestroyEntities | Shared_x4 |              ? |    48.28 ms | 0.159 ms | 0.141 ms |
|                                       |                                  |                                       |           |                |             |          |          |
|  **EcsLte.BenchmarkTest.EcsContextTests** |          **EcsContext_CreateEntity** |                 **DestroyEntities_Reuse** | **Normal_x4** |              **?** |    **44.65 ms** | **0.211 ms** | **0.197 ms** |
|  EcsLte.BenchmarkTest.EcsContextTests |          EcsContext_CreateEntity |                 DestroyEntities_Reuse | Shared_x4 |              ? |    44.62 ms | 0.135 ms | 0.119 ms |
|                                       |                                  |                                       |           |                |             |          |          |
|  **EcsLte.BenchmarkTest.EcsContextTests** |          **EcsContext_CreateEntity** |                         **DestroyEntity** | **Normal_x4** |              **?** |    **57.71 ms** | **0.381 ms** | **0.356 ms** |
|  EcsLte.BenchmarkTest.EcsContextTests |          EcsContext_CreateEntity |                         DestroyEntity | Shared_x4 |              ? |    57.56 ms | 0.287 ms | 0.255 ms |
|                                       |                                  |                                       |           |                |             |          |          |
|  **EcsLte.BenchmarkTest.EcsContextTests** |          **EcsContext_CreateEntity** |                   **DestroyEntity_Reuse** | **Normal_x4** |              **?** |    **53.95 ms** | **0.159 ms** | **0.148 ms** |
|  EcsLte.BenchmarkTest.EcsContextTests |          EcsContext_CreateEntity |                   DestroyEntity_Reuse | Shared_x4 |              ? |    53.99 ms | 0.208 ms | 0.184 ms |
|                                       |                                  |                                       |           |                |             |          |          |
|  **EcsLte.BenchmarkTest.EcsContextTests** |              **EcsContext_Transfer** |              **TransferEntities_Destroy** | **Normal_x4** |              **?** |   **216.84 ms** | **2.008 ms** | **1.780 ms** |
|  EcsLte.BenchmarkTest.EcsContextTests |              EcsContext_Transfer |              TransferEntities_Destroy | Shared_x4 |              ? |   213.21 ms | 2.374 ms | 1.982 ms |
|                                       |                                  |                                       |           |                |             |          |          |
|  **EcsLte.BenchmarkTest.EcsContextTests** |              **EcsContext_Transfer** |            **TransferEntities_NoDestroy** | **Normal_x4** |              **?** |   **175.45 ms** | **1.663 ms** | **1.556 ms** |
|  EcsLte.BenchmarkTest.EcsContextTests |              EcsContext_Transfer |            TransferEntities_NoDestroy | Shared_x4 |              ? |   174.41 ms | 2.275 ms | 2.128 ms |
|                                       |                                  |                                       |           |                |             |          |          |
|  **EcsLte.BenchmarkTest.EcsContextTests** |              **EcsContext_Transfer** |       **TransferEntityArcheType_Destroy** | **Normal_x4** |              **?** |    **79.84 ms** | **0.769 ms** | **0.642 ms** |
|  EcsLte.BenchmarkTest.EcsContextTests |              EcsContext_Transfer |       TransferEntityArcheType_Destroy | Shared_x4 |              ? |    80.23 ms | 0.928 ms | 0.868 ms |
|                                       |                                  |                                       |           |                |             |          |          |
|  **EcsLte.BenchmarkTest.EcsContextTests** |              **EcsContext_Transfer** |     **TransferEntityArcheType_NoDestroy** | **Normal_x4** |              **?** |    **71.29 ms** | **0.562 ms** | **0.525 ms** |
|  EcsLte.BenchmarkTest.EcsContextTests |              EcsContext_Transfer |     TransferEntityArcheType_NoDestroy | Shared_x4 |              ? |    71.31 ms | 0.473 ms | 0.443 ms |
|                                       |                                  |                                       |           |                |             |          |          |
|  **EcsLte.BenchmarkTest.EcsContextTests** |              **EcsContext_Transfer** |           **TransferEntityQuery_Destroy** | **Normal_x4** |              **?** |    **81.41 ms** | **0.463 ms** | **0.433 ms** |
|  EcsLte.BenchmarkTest.EcsContextTests |              EcsContext_Transfer |           TransferEntityQuery_Destroy | Shared_x4 |              ? |    79.90 ms | 0.458 ms | 0.429 ms |
|                                       |                                  |                                       |           |                |             |          |          |
|  **EcsLte.BenchmarkTest.EcsContextTests** |              **EcsContext_Transfer** |         **TransferEntityQuery_NoDestroy** | **Normal_x4** |              **?** |    **71.85 ms** | **0.512 ms** | **0.479 ms** |
|  EcsLte.BenchmarkTest.EcsContextTests |              EcsContext_Transfer |         TransferEntityQuery_NoDestroy | Shared_x4 |              ? |    71.54 ms | 0.480 ms | 0.449 ms |
|                                       |                                  |                                       |           |                |             |          |          |
|  **EcsLte.BenchmarkTest.EcsContextTests** |              **EcsContext_Transfer** |                **TransferEntity_Destroy** | **Normal_x4** |              **?** |   **201.61 ms** | **0.621 ms** | **0.581 ms** |
|  EcsLte.BenchmarkTest.EcsContextTests |              EcsContext_Transfer |                TransferEntity_Destroy | Shared_x4 |              ? |   242.45 ms | 0.425 ms | 0.398 ms |
|                                       |                                  |                                       |           |                |             |          |          |
|  **EcsLte.BenchmarkTest.EcsContextTests** |              **EcsContext_Transfer** |              **TransferEntity_NoDestroy** | **Normal_x4** |              **?** |   **164.54 ms** | **0.485 ms** | **0.430 ms** |
|  EcsLte.BenchmarkTest.EcsContextTests |              EcsContext_Transfer |              TransferEntity_NoDestroy | Shared_x4 |              ? |   202.23 ms | 0.435 ms | 0.386 ms |
|                                       |                                  |                                       |           |                |             |          |          |
|  **EcsLte.BenchmarkTest.EcsContextTests** |       **EcsContext_UpdateComponent** |                **UpdateComponent_Entity** | **Normal_x4** |              **?** |   **337.39 ms** | **0.714 ms** | **0.633 ms** |
|  EcsLte.BenchmarkTest.EcsContextTests |       EcsContext_UpdateComponent |                UpdateComponent_Entity | Shared_x4 |              ? | 1,378.94 ms | 3.815 ms | 3.382 ms |
|                                       |                                  |                                       |           |                |             |          |          |
| **EcsLte.BenchmarkTest.EntityQueryTests** |              **EntityQuery_ForEach** |                               **ForEach** |         **?** |           **R0W0** |    **81.04 ms** | **0.614 ms** | **0.574 ms** |
| EcsLte.BenchmarkTest.EntityQueryTests |              EntityQuery_ForEach |                               ForEach |         ? | R0W4_Normal_x4 |   298.69 ms | 1.808 ms | 1.603 ms |
| EcsLte.BenchmarkTest.EntityQueryTests |              EntityQuery_ForEach |                               ForEach |         ? | R4W0_Normal_x4 |   186.93 ms | 0.522 ms | 0.462 ms |
| EcsLte.BenchmarkTest.EntityQueryTests |              EntityQuery_ForEach |                               ForEach |         ? | R0W4_Shared_x4 |   627.39 ms | 2.685 ms | 2.242 ms |
| EcsLte.BenchmarkTest.EntityQueryTests |              EntityQuery_ForEach |                               ForEach |         ? | R4W0_Shared_x4 |   194.80 ms | 0.326 ms | 0.305 ms |
