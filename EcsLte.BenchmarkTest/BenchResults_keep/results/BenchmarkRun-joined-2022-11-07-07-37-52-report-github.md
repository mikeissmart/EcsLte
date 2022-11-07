``` ini

BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19043.1348 (21H1/May2021Update)
AMD Ryzen 9 3900X, 1 CPU, 24 logical and 12 physical cores
  [Host]     : .NET Framework 4.8 (4.8.4420.0), X86 LegacyJIT
  Job-FTOBUC : .NET Framework 4.8 (4.8.4420.0), X86 LegacyJIT

InvocationCount=1  UnrollFactor=1  

```
|                             Type |                                Method |    CompArr |       ReadWrite |         Mean |     Error |    StdDev |       Median |
|--------------------------------- |-------------------------------------- |----------- |---------------- |-------------:|----------:|----------:|-------------:|
|          **EcsContext_GetComponent** |                   **GetComponent_Entity** |          **?** |               **?** |   **176.794 ms** | **0.5233 ms** | **0.4895 ms** |   **176.535 ms** |
|                                  |                                       |            |                 |              |           |           |              |
|          **EcsContext_GetComponent** |         **GetComponents_EntityArcheType** |          **?** |               **?** |     **5.736 ms** | **0.2500 ms** | **0.7371 ms** |     **5.837 ms** |
|                                  |                                       |            |                 |              |           |           |              |
|          **EcsContext_GetComponent** |             **GetComponents_EntityQuery** |          **?** |               **?** |     **5.670 ms** | **0.2305 ms** | **0.6796 ms** |     **5.812 ms** |
|                                  |                                       |            |                 |              |           |           |              |
| **EcsContext_UpdateSharedComponent** | **UpdateSharedComponent_EntityArcheType** |          **?** |               **?** |     **4.591 ms** | **0.0913 ms** | **0.2023 ms** |     **4.510 ms** |
|                                  |                                       |            |                 |              |           |           |              |
| **EcsContext_UpdateSharedComponent** |     **UpdateSharedComponent_EntityQuery** |          **?** |               **?** |     **6.737 ms** | **0.1364 ms** | **0.3758 ms** |     **6.684 ms** |
|                                  |                                       |            |                 |              |           |           |              |
|          **EcsContext_CreateEntity** |                        **CreateEntities** |  **Normal_x4** |               **?** |    **37.353 ms** | **0.5372 ms** | **0.5025 ms** |    **37.308 ms** |
|          EcsContext_CreateEntity |                        CreateEntities | Managed_x4 |               ? |    43.596 ms | 0.8606 ms | 2.0285 ms |    43.421 ms |
|          EcsContext_CreateEntity |                        CreateEntities |  Shared_x4 |               ? |    11.761 ms | 0.2344 ms | 0.3785 ms |    11.797 ms |
|                                  |                                       |            |                 |              |           |           |              |
|          **EcsContext_CreateEntity** |                  **CreateEntities_Reuse** |  **Normal_x4** |               **?** |     **9.315 ms** | **0.3439 ms** | **1.0141 ms** |     **9.283 ms** |
|          EcsContext_CreateEntity |                  CreateEntities_Reuse | Managed_x4 |               ? |    14.388 ms | 0.3429 ms | 1.0110 ms |    14.652 ms |
|          EcsContext_CreateEntity |                  CreateEntities_Reuse |  Shared_x4 |               ? |     7.004 ms | 0.2851 ms | 0.8361 ms |     6.792 ms |
|                                  |                                       |            |                 |              |           |           |              |
|          **EcsContext_CreateEntity** |                          **CreateEntity** |  **Normal_x4** |               **?** |   **268.019 ms** | **2.0566 ms** | **1.9237 ms** |   **268.520 ms** |
|          EcsContext_CreateEntity |                          CreateEntity | Managed_x4 |               ? |   268.433 ms | 3.6209 ms | 3.2098 ms |   268.471 ms |
|          EcsContext_CreateEntity |                          CreateEntity |  Shared_x4 |               ? |   139.309 ms | 2.5784 ms | 2.4118 ms |   139.424 ms |
|                                  |                                       |            |                 |              |           |           |              |
|          **EcsContext_CreateEntity** |                    **CreateEntity_Reuse** |  **Normal_x4** |               **?** |   **200.541 ms** | **0.6994 ms** | **0.6200 ms** |   **200.845 ms** |
|          EcsContext_CreateEntity |                    CreateEntity_Reuse | Managed_x4 |               ? |   213.286 ms | 0.3197 ms | 0.2496 ms |   213.358 ms |
|          EcsContext_CreateEntity |                    CreateEntity_Reuse |  Shared_x4 |               ? |   121.215 ms | 0.5387 ms | 0.5039 ms |   121.379 ms |
|                                  |                                       |            |                 |              |           |           |              |
|          **EcsContext_CreateEntity** |                       **DestroyEntities** |  **Normal_x4** |               **?** |    **63.028 ms** | **0.5831 ms** | **0.5454 ms** |    **62.895 ms** |
|          EcsContext_CreateEntity |                       DestroyEntities | Managed_x4 |               ? |    83.521 ms | 0.3160 ms | 0.2801 ms |    83.566 ms |
|          EcsContext_CreateEntity |                       DestroyEntities |  Shared_x4 |               ? |    48.122 ms | 0.4060 ms | 0.3599 ms |    48.249 ms |
|                                  |                                       |            |                 |              |           |           |              |
|          **EcsContext_CreateEntity** |                 **DestroyEntities_Reuse** |  **Normal_x4** |               **?** |    **60.966 ms** | **0.1944 ms** | **0.1624 ms** |    **60.996 ms** |
|          EcsContext_CreateEntity |                 DestroyEntities_Reuse | Managed_x4 |               ? |    81.850 ms | 0.3704 ms | 0.3093 ms |    81.656 ms |
|          EcsContext_CreateEntity |                 DestroyEntities_Reuse |  Shared_x4 |               ? |    45.810 ms | 0.0843 ms | 0.0748 ms |    45.820 ms |
|                                  |                                       |            |                 |              |           |           |              |
|          **EcsContext_CreateEntity** |                         **DestroyEntity** |  **Normal_x4** |               **?** |    **56.327 ms** | **0.4911 ms** | **0.4353 ms** |    **56.398 ms** |
|          EcsContext_CreateEntity |                         DestroyEntity | Managed_x4 |               ? |    79.629 ms | 0.5628 ms | 0.4700 ms |    79.677 ms |
|          EcsContext_CreateEntity |                         DestroyEntity |  Shared_x4 |               ? |    44.749 ms | 0.3630 ms | 0.3218 ms |    44.746 ms |
|                                  |                                       |            |                 |              |           |           |              |
|          **EcsContext_CreateEntity** |                   **DestroyEntity_Reuse** |  **Normal_x4** |               **?** |    **54.257 ms** | **0.1186 ms** | **0.1051 ms** |    **54.264 ms** |
|          EcsContext_CreateEntity |                   DestroyEntity_Reuse | Managed_x4 |               ? |    77.198 ms | 0.1829 ms | 0.1528 ms |    77.176 ms |
|          EcsContext_CreateEntity |                   DestroyEntity_Reuse |  Shared_x4 |               ? |    42.656 ms | 0.1010 ms | 0.0945 ms |    42.657 ms |
|                                  |                                       |            |                 |              |           |           |              |
|              **EcsContext_Transfer** |            **TransferEntities_NoDestroy** |  **Normal_x4** |               **?** |   **150.053 ms** | **0.8128 ms** | **0.7205 ms** |   **150.024 ms** |
|              EcsContext_Transfer |            TransferEntities_NoDestroy | Managed_x4 |               ? |   209.442 ms | 0.9708 ms | 0.9081 ms |   209.150 ms |
|              EcsContext_Transfer |            TransferEntities_NoDestroy |  Shared_x4 |               ? |   126.150 ms | 0.8338 ms | 0.7799 ms |   126.283 ms |
|                                  |                                       |            |                 |              |           |           |              |
|              **EcsContext_Transfer** |     **TransferEntityArcheType_NoDestroy** |  **Normal_x4** |               **?** |     **9.460 ms** | **0.2706 ms** | **0.7979 ms** |     **9.712 ms** |
|              EcsContext_Transfer |     TransferEntityArcheType_NoDestroy | Managed_x4 |               ? |    11.789 ms | 0.3084 ms | 0.9094 ms |    11.890 ms |
|              EcsContext_Transfer |     TransferEntityArcheType_NoDestroy |  Shared_x4 |               ? |     7.239 ms | 0.2996 ms | 0.8833 ms |     7.580 ms |
|                                  |                                       |            |                 |              |           |           |              |
|              **EcsContext_Transfer** |         **TransferEntityQuery_NoDestroy** |  **Normal_x4** |               **?** |    **43.189 ms** | **0.8010 ms** | **0.7867 ms** |    **43.144 ms** |
|              EcsContext_Transfer |         TransferEntityQuery_NoDestroy | Managed_x4 |               ? |    93.568 ms | 1.2583 ms | 1.1770 ms |    93.657 ms |
|              EcsContext_Transfer |         TransferEntityQuery_NoDestroy |  Shared_x4 |               ? |    12.813 ms | 0.2556 ms | 0.7293 ms |    12.553 ms |
|                                  |                                       |            |                 |              |           |           |              |
|              **EcsContext_Transfer** |              **TransferEntity_NoDestroy** |  **Normal_x4** |               **?** |   **206.334 ms** | **0.6720 ms** | **0.6286 ms** |   **206.399 ms** |
|              EcsContext_Transfer |              TransferEntity_NoDestroy | Managed_x4 |               ? |   270.315 ms | 0.5341 ms | 0.4996 ms |   270.517 ms |
|              EcsContext_Transfer |              TransferEntity_NoDestroy |  Shared_x4 |               ? |   481.382 ms | 0.6646 ms | 0.5549 ms |   481.366 ms |
|                                  |                                       |            |                 |              |           |           |              |
|       **EcsContext_UpdateComponent** |                **UpdateComponent_Entity** |  **Normal_x4** |               **?** |   **260.448 ms** | **0.3013 ms** | **0.2516 ms** |   **260.429 ms** |
|       EcsContext_UpdateComponent |                UpdateComponent_Entity | Managed_x4 |               ? |   266.760 ms | 0.4381 ms | 0.3658 ms |   266.741 ms |
|       EcsContext_UpdateComponent |                UpdateComponent_Entity |  Shared_x4 |               ? | 1,079.332 ms | 3.2103 ms | 3.0030 ms | 1,080.117 ms |
|                                  |                                       |            |                 |              |           |           |              |
|      **EcsContext_UpdateComponents** |               **UpdateComponents_Entity** |  **Normal_x4** |               **?** |   **383.987 ms** | **1.1899 ms** | **1.1131 ms** |   **384.421 ms** |
|      EcsContext_UpdateComponents |               UpdateComponents_Entity | Managed_x4 |               ? |   337.667 ms | 0.9773 ms | 0.9142 ms |   337.831 ms |
|      EcsContext_UpdateComponents |               UpdateComponents_Entity |  Shared_x4 |               ? |   627.457 ms | 2.1740 ms | 2.0336 ms |   627.655 ms |
|                                  |                                       |            |                 |              |           |           |              |
|          **EntityQueryForEachTests** |                               **ForEach** |          **?** |            **R0W0** |    **11.028 ms** | **0.1303 ms** | **0.1155 ms** |    **11.001 ms** |
|          EntityQueryForEachTests |                               ForEach |          ? |  R0W4_Normal_x4 |    83.370 ms | 0.2875 ms | 0.2401 ms |    83.341 ms |
|          EntityQueryForEachTests |                               ForEach |          ? |  R4W0_Normal_x4 |    81.433 ms | 0.1846 ms | 0.1541 ms |    81.394 ms |
|          EntityQueryForEachTests |                               ForEach |          ? | R0W4_Managed_x4 |    85.358 ms | 0.1771 ms | 0.1570 ms |    85.334 ms |
|          EntityQueryForEachTests |                               ForEach |          ? | R4W0_Managed_x4 |    81.223 ms | 0.3587 ms | 0.3180 ms |    81.309 ms |
|          EntityQueryForEachTests |                               ForEach |          ? |  R0W4_Shared_x4 |   230.816 ms | 2.5286 ms | 2.3653 ms |   231.321 ms |
|          EntityQueryForEachTests |                               ForEach |          ? |  R4W0_Shared_x4 |    22.293 ms | 0.1812 ms | 0.1695 ms |    22.306 ms |
