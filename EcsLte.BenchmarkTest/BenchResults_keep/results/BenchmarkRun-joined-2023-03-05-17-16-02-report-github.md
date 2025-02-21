``` ini

BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19045
AMD Ryzen 9 3900X, 1 CPU, 24 logical and 12 physical cores
  [Host]     : .NET Framework 4.8 (4.8.4515.0), X86 LegacyJIT  [AttachedDebugger]
  Job-FTOBUC : .NET Framework 4.8 (4.8.4515.0), X86 LegacyJIT

InvocationCount=1  UnrollFactor=1  

```
|                             Type |                                Method |    CompArr |       ReadWrite |       Mean |     Error |    StdDev |     Median |
|--------------------------------- |-------------------------------------- |----------- |---------------- |-----------:|----------:|----------:|-----------:|
|          **EcsContext_GetComponent** |                   **GetComponent_Entity** |          **?** |               **?** | **196.360 ms** | **1.1369 ms** | **0.9494 ms** | **196.357 ms** |
|                                  |                                       |            |                 |            |           |           |            |
|          **EcsContext_GetComponent** |         **GetComponents_EntityArcheType** |          **?** |               **?** |   **4.719 ms** | **0.0937 ms** | **0.2748 ms** |   **4.761 ms** |
|                                  |                                       |            |                 |            |           |           |            |
|          **EcsContext_GetComponent** |             **GetComponents_EntityQuery** |          **?** |               **?** |   **4.783 ms** | **0.0982 ms** | **0.2894 ms** |   **4.731 ms** |
|                                  |                                       |            |                 |            |           |           |            |
|              **EcsContext_Tracking** |        **CreateEntities_Reuse_NoTracker** |          **?** |               **?** |  **11.728 ms** | **0.1315 ms** | **0.1230 ms** |  **11.704 ms** |
|                                  |                                       |            |                 |            |           |           |            |
|              **EcsContext_Tracking** |      **CreateEntities_Reuse_WithTracker** |          **?** |               **?** |  **13.002 ms** | **0.1631 ms** | **0.1526 ms** |  **12.979 ms** |
|                                  |                                       |            |                 |            |           |           |            |
|              **EcsContext_Tracking** |           **Update_Managed_x4_NoTracker** |          **?** |               **?** |   **7.531 ms** | **0.0683 ms** | **0.0570 ms** |   **7.525 ms** |
|                                  |                                       |            |                 |            |           |           |            |
|              **EcsContext_Tracking** |         **Update_Managed_x4_WithTracker** |          **?** |               **?** |   **8.746 ms** | **0.1428 ms** | **0.1335 ms** |   **8.722 ms** |
|                                  |                                       |            |                 |            |           |           |            |
|              **EcsContext_Tracking** |            **Update_Normal_x4_NoTracker** |          **?** |               **?** |   **1.935 ms** | **0.0379 ms** | **0.0336 ms** |   **1.934 ms** |
|                                  |                                       |            |                 |            |           |           |            |
|              **EcsContext_Tracking** |          **Update_Normal_x4_WithTracker** |          **?** |               **?** |   **3.036 ms** | **0.0542 ms** | **0.0453 ms** |   **3.024 ms** |
|                                  |                                       |            |                 |            |           |           |            |
| **EcsContext_UpdateSharedComponent** | **UpdateSharedComponent_EntityArcheType** |          **?** |               **?** |   **8.219 ms** | **0.1175 ms** | **0.1041 ms** |   **8.193 ms** |
|                                  |                                       |            |                 |            |           |           |            |
|          **EcsContext_CreateEntity** |                        **CreateEntities** |  **Normal_x4** |               **?** |  **39.595 ms** | **0.3944 ms** | **0.3689 ms** |  **39.582 ms** |
|          EcsContext_CreateEntity |                        CreateEntities | Managed_x4 |               ? |  43.275 ms | 0.3436 ms | 0.3214 ms |  43.298 ms |
|          EcsContext_CreateEntity |                        CreateEntities |  Shared_x4 |               ? |  14.117 ms | 0.1679 ms | 0.1402 ms |  14.118 ms |
|                                  |                                       |            |                 |            |           |           |            |
|          **EcsContext_CreateEntity** |                  **CreateEntities_Reuse** |  **Normal_x4** |               **?** |  **12.252 ms** | **0.2741 ms** | **0.8082 ms** |  **12.338 ms** |
|          EcsContext_CreateEntity |                  CreateEntities_Reuse | Managed_x4 |               ? |  17.683 ms | 0.3526 ms | 0.8030 ms |  17.941 ms |
|          EcsContext_CreateEntity |                  CreateEntities_Reuse |  Shared_x4 |               ? |  10.266 ms | 0.2691 ms | 0.7935 ms |  10.395 ms |
|                                  |                                       |            |                 |            |           |           |            |
|          **EcsContext_CreateEntity** |                          **CreateEntity** |  **Normal_x4** |               **?** | **235.826 ms** | **0.7167 ms** | **0.6353 ms** | **235.900 ms** |
|          EcsContext_CreateEntity |                          CreateEntity | Managed_x4 |               ? | 245.714 ms | 0.9690 ms | 0.9064 ms | 245.813 ms |
|          EcsContext_CreateEntity |                          CreateEntity |  Shared_x4 |               ? |  83.460 ms | 0.8738 ms | 0.8173 ms |  83.722 ms |
|                                  |                                       |            |                 |            |           |           |            |
|          **EcsContext_CreateEntity** |                    **CreateEntity_Reuse** |  **Normal_x4** |               **?** | **173.081 ms** | **0.4054 ms** | **0.3792 ms** | **173.026 ms** |
|          EcsContext_CreateEntity |                    CreateEntity_Reuse | Managed_x4 |               ? | 184.644 ms | 0.9735 ms | 0.9107 ms | 184.553 ms |
|          EcsContext_CreateEntity |                    CreateEntity_Reuse |  Shared_x4 |               ? |  64.256 ms | 0.1799 ms | 0.1404 ms |  64.266 ms |
|                                  |                                       |            |                 |            |           |           |            |
|          **EcsContext_CreateEntity** |                       **DestroyEntities** |  **Normal_x4** |               **?** |  **74.445 ms** | **0.3341 ms** | **0.3125 ms** |  **74.425 ms** |
|          EcsContext_CreateEntity |                       DestroyEntities | Managed_x4 |               ? | 194.861 ms | 2.6661 ms | 2.4939 ms | 193.696 ms |
|          EcsContext_CreateEntity |                       DestroyEntities |  Shared_x4 |               ? |  38.678 ms | 0.3137 ms | 0.2935 ms |  38.656 ms |
|                                  |                                       |            |                 |            |           |           |            |
|          **EcsContext_CreateEntity** |                 **DestroyEntities_Reuse** |  **Normal_x4** |               **?** |  **42.640 ms** | **0.2392 ms** | **0.2120 ms** |  **42.605 ms** |
|          EcsContext_CreateEntity |                 DestroyEntities_Reuse | Managed_x4 |               ? |  42.702 ms | 0.1494 ms | 0.1398 ms |  42.733 ms |
|          EcsContext_CreateEntity |                 DestroyEntities_Reuse |  Shared_x4 |               ? |  42.714 ms | 0.2184 ms | 0.2043 ms |  42.625 ms |
|                                  |                                       |            |                 |            |           |           |            |
|          **EcsContext_CreateEntity** |                         **DestroyEntity** |  **Normal_x4** |               **?** |  **72.791 ms** | **0.2983 ms** | **0.2491 ms** |  **72.816 ms** |
|          EcsContext_CreateEntity |                         DestroyEntity | Managed_x4 |               ? | 175.176 ms | 0.6629 ms | 0.5536 ms | 175.015 ms |
|          EcsContext_CreateEntity |                         DestroyEntity |  Shared_x4 |               ? |  45.593 ms | 0.2078 ms | 0.1735 ms |  45.578 ms |
|                                  |                                       |            |                 |            |           |           |            |
|          **EcsContext_CreateEntity** |                   **DestroyEntity_Reuse** |  **Normal_x4** |               **?** |  **34.959 ms** | **0.1107 ms** | **0.0981 ms** |  **34.965 ms** |
|          EcsContext_CreateEntity |                   DestroyEntity_Reuse | Managed_x4 |               ? |  34.789 ms | 0.1323 ms | 0.1104 ms |  34.829 ms |
|          EcsContext_CreateEntity |                   DestroyEntity_Reuse |  Shared_x4 |               ? |  35.000 ms | 0.1023 ms | 0.0798 ms |  35.016 ms |
|                                  |                                       |            |                 |            |           |           |            |
|              **EcsContext_Transfer** |            **TransferEntities_NoDestroy** |  **Normal_x4** |               **?** | **110.560 ms** | **0.5998 ms** | **0.5318 ms** | **110.625 ms** |
|              EcsContext_Transfer |            TransferEntities_NoDestroy | Managed_x4 |               ? | 255.104 ms | 0.4692 ms | 0.4389 ms | 254.967 ms |
|              EcsContext_Transfer |            TransferEntities_NoDestroy |  Shared_x4 |               ? |  86.655 ms | 0.7733 ms | 0.7234 ms |  86.717 ms |
|                                  |                                       |            |                 |            |           |           |            |
|              **EcsContext_Transfer** |     **TransferEntityArcheType_NoDestroy** |  **Normal_x4** |               **?** |  **12.567 ms** | **0.2644 ms** | **0.7795 ms** |  **12.961 ms** |
|              EcsContext_Transfer |     TransferEntityArcheType_NoDestroy | Managed_x4 |               ? |  23.159 ms | 0.8714 ms | 2.5694 ms |  23.533 ms |
|              EcsContext_Transfer |     TransferEntityArcheType_NoDestroy |  Shared_x4 |               ? |  10.535 ms | 0.2910 ms | 0.8581 ms |  10.895 ms |
|                                  |                                       |            |                 |            |           |           |            |
|              **EcsContext_Transfer** |              **TransferEntity_NoDestroy** |  **Normal_x4** |               **?** | **161.492 ms** | **0.4199 ms** | **0.3722 ms** | **161.451 ms** |
|              EcsContext_Transfer |              TransferEntity_NoDestroy | Managed_x4 |               ? | 305.122 ms | 2.2528 ms | 1.7588 ms | 305.556 ms |
|              EcsContext_Transfer |              TransferEntity_NoDestroy |  Shared_x4 |               ? | 424.002 ms | 2.7970 ms | 2.6163 ms | 423.282 ms |
|                                  |                                       |            |                 |            |           |           |            |
|       **EcsContext_UpdateComponent** |                **UpdateComponent_Entity** |  **Normal_x4** |               **?** | **216.071 ms** | **0.5886 ms** | **0.5218 ms** | **215.979 ms** |
|       EcsContext_UpdateComponent |                UpdateComponent_Entity | Managed_x4 |               ? | 229.139 ms | 0.5368 ms | 0.4191 ms | 229.145 ms |
|       EcsContext_UpdateComponent |                UpdateComponent_Entity |  Shared_x4 |               ? | 968.276 ms | 2.8693 ms | 2.5435 ms | 968.758 ms |
|                                  |                                       |            |                 |            |           |           |            |
|      **EcsContext_UpdateComponents** |               **UpdateComponents_Entity** |  **Normal_x4** |               **?** | **296.922 ms** | **0.9399 ms** | **0.8332 ms** | **296.765 ms** |
|      EcsContext_UpdateComponents |               UpdateComponents_Entity | Managed_x4 |               ? | 265.159 ms | 1.1118 ms | 0.9856 ms | 265.207 ms |
|      EcsContext_UpdateComponents |               UpdateComponents_Entity |  Shared_x4 |               ? | 541.074 ms | 1.9820 ms | 1.8540 ms | 541.463 ms |
|                                  |                                       |            |                 |            |           |           |            |
|          **EntityQueryForEachTests** |                               **ForEach** |          **?** |            **R0W0** |  **10.596 ms** | **0.0495 ms** | **0.0413 ms** |  **10.600 ms** |
|          EntityQueryForEachTests |                               ForEach |          ? |  R0W4_Normal_x4 |  87.719 ms | 0.2033 ms | 0.1802 ms |  87.703 ms |
|          EntityQueryForEachTests |                               ForEach |          ? |  R4W0_Normal_x4 |  85.763 ms | 0.1881 ms | 0.1668 ms |  85.725 ms |
|          EntityQueryForEachTests |                               ForEach |          ? | R0W4_Managed_x4 |  90.669 ms | 0.3357 ms | 0.2976 ms |  90.680 ms |
|          EntityQueryForEachTests |                               ForEach |          ? | R4W0_Managed_x4 |  89.097 ms | 0.1640 ms | 0.1370 ms |  89.029 ms |
|          EntityQueryForEachTests |                               ForEach |          ? |  R0W4_Shared_x4 | 172.937 ms | 0.8311 ms | 0.7774 ms | 173.038 ms |
|          EntityQueryForEachTests |                               ForEach |          ? |  R4W0_Shared_x4 |  25.331 ms | 0.0974 ms | 0.0911 ms |  25.323 ms |
