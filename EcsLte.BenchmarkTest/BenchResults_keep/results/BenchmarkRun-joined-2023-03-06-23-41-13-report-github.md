``` ini

BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19045
AMD Ryzen 9 3900X, 1 CPU, 24 logical and 12 physical cores
  [Host]     : .NET Framework 4.8 (4.8.4515.0), X86 LegacyJIT  [AttachedDebugger]
  Job-FTOBUC : .NET Framework 4.8 (4.8.4515.0), X86 LegacyJIT

InvocationCount=1  UnrollFactor=1  

```
|                              Type |                                Method |    CompArr |       ReadWrite | Parallel |       Mean |     Error |    StdDev |     Median |
|---------------------------------- |-------------------------------------- |----------- |---------------- |--------- |-----------:|----------:|----------:|-----------:|
|           **EcsContext_GetComponent** |                   **GetComponent_Entity** |          **?** |               **?** |        **?** | **198.258 ms** | **0.6886 ms** | **0.6441 ms** | **198.218 ms** |
|                                   |                                       |            |                 |          |            |           |           |            |
|           **EcsContext_GetComponent** |         **GetComponents_EntityArcheType** |          **?** |               **?** |        **?** |   **4.703 ms** | **0.0938 ms** | **0.2487 ms** |   **4.706 ms** |
|                                   |                                       |            |                 |          |            |           |           |            |
|           **EcsContext_GetComponent** |             **GetComponents_EntityQuery** |          **?** |               **?** |        **?** |   **4.704 ms** | **0.1041 ms** | **0.3070 ms** |   **4.689 ms** |
|                                   |                                       |            |                 |          |            |           |           |            |
|               **EcsContext_Tracking** |        **CreateEntities_Reuse_NoTracker** |          **?** |               **?** |        **?** |  **11.790 ms** | **0.0946 ms** | **0.0885 ms** |  **11.756 ms** |
|                                   |                                       |            |                 |          |            |           |           |            |
|               **EcsContext_Tracking** |      **CreateEntities_Reuse_WithTracker** |          **?** |               **?** |        **?** |  **12.947 ms** | **0.1215 ms** | **0.1077 ms** |  **12.924 ms** |
|                                   |                                       |            |                 |          |            |           |           |            |
|               **EcsContext_Tracking** |           **Update_Managed_x4_NoTracker** |          **?** |               **?** |        **?** |  **11.128 ms** | **0.3953 ms** | **1.1657 ms** |  **10.273 ms** |
|                                   |                                       |            |                 |          |            |           |           |            |
|               **EcsContext_Tracking** |         **Update_Managed_x4_WithTracker** |          **?** |               **?** |        **?** |  **12.675 ms** | **0.4926 ms** | **1.4526 ms** |  **13.118 ms** |
|                                   |                                       |            |                 |          |            |           |           |            |
|               **EcsContext_Tracking** |            **Update_Normal_x4_NoTracker** |          **?** |               **?** |        **?** |   **2.024 ms** | **0.0367 ms** | **0.0393 ms** |   **2.024 ms** |
|                                   |                                       |            |                 |          |            |           |           |            |
|               **EcsContext_Tracking** |          **Update_Normal_x4_WithTracker** |          **?** |               **?** |        **?** |   **3.136 ms** | **0.0601 ms** | **0.0590 ms** |   **3.126 ms** |
|                                   |                                       |            |                 |          |            |           |           |            |
| **EcsContext_UpdateComponentAdapter** |                **UpdateComponent_Entity** |          **?** |               **?** |        **?** | **107.398 ms** | **0.2837 ms** | **0.2515 ms** | **107.359 ms** |
|                                   |                                       |            |                 |          |            |           |           |            |
| **EcsContext_UpdateComponentAdapter** |        **UpdateComponent_Entity_Adapter** |          **?** |               **?** |        **?** | **137.620 ms** | **0.3509 ms** | **0.2930 ms** | **137.549 ms** |
|                                   |                                       |            |                 |          |            |           |           |            |
| **EcsContext_UpdateComponentAdapter** |         **UpdateManagedComponent_Entity** |          **?** |               **?** |        **?** | **127.949 ms** | **0.3628 ms** | **0.3029 ms** | **127.962 ms** |
|                                   |                                       |            |                 |          |            |           |           |            |
| **EcsContext_UpdateComponentAdapter** | **UpdateManagedComponent_Entity_Adapter** |          **?** |               **?** |        **?** | **148.802 ms** | **0.4105 ms** | **0.3839 ms** | **148.665 ms** |
|                                   |                                       |            |                 |          |            |           |           |            |
|  **EcsContext_UpdateSharedComponent** | **UpdateSharedComponent_EntityArcheType** |          **?** |               **?** |        **?** |   **8.527 ms** | **0.1637 ms** | **0.1451 ms** |   **8.481 ms** |
|                                   |                                       |            |                 |          |            |           |           |            |
|           **EcsContext_CreateEntity** |                        **CreateEntities** |  **Normal_x4** |               **?** |        **?** |  **44.025 ms** | **0.4207 ms** | **0.3729 ms** |  **44.030 ms** |
|           EcsContext_CreateEntity |                        CreateEntities | Managed_x4 |               ? |        ? |  42.745 ms | 0.6093 ms | 0.5402 ms |  42.689 ms |
|           EcsContext_CreateEntity |                        CreateEntities |  Shared_x4 |               ? |        ? |  14.714 ms | 0.2886 ms | 0.2964 ms |  14.732 ms |
|                                   |                                       |            |                 |          |            |           |           |            |
|           **EcsContext_CreateEntity** |                  **CreateEntities_Reuse** |  **Normal_x4** |               **?** |        **?** |  **12.381 ms** | **0.2754 ms** | **0.8119 ms** |  **12.359 ms** |
|           EcsContext_CreateEntity |                  CreateEntities_Reuse | Managed_x4 |               ? |        ? |  22.064 ms | 0.4381 ms | 1.1617 ms |  22.084 ms |
|           EcsContext_CreateEntity |                  CreateEntities_Reuse |  Shared_x4 |               ? |        ? |  10.172 ms | 0.2805 ms | 0.8272 ms |  10.337 ms |
|                                   |                                       |            |                 |          |            |           |           |            |
|           **EcsContext_CreateEntity** |                          **CreateEntity** |  **Normal_x4** |               **?** |        **?** | **243.754 ms** | **3.1347 ms** | **2.9322 ms** | **243.230 ms** |
|           EcsContext_CreateEntity |                          CreateEntity | Managed_x4 |               ? |        ? | 228.070 ms | 0.6395 ms | 0.5669 ms | 228.018 ms |
|           EcsContext_CreateEntity |                          CreateEntity |  Shared_x4 |               ? |        ? |  81.929 ms | 0.5854 ms | 0.5476 ms |  82.076 ms |
|                                   |                                       |            |                 |          |            |           |           |            |
|           **EcsContext_CreateEntity** |                    **CreateEntity_Reuse** |  **Normal_x4** |               **?** |        **?** | **172.434 ms** | **0.4842 ms** | **0.4043 ms** | **172.491 ms** |
|           EcsContext_CreateEntity |                    CreateEntity_Reuse | Managed_x4 |               ? |        ? | 192.818 ms | 0.3324 ms | 0.3110 ms | 192.739 ms |
|           EcsContext_CreateEntity |                    CreateEntity_Reuse |  Shared_x4 |               ? |        ? |  63.485 ms | 0.3111 ms | 0.2910 ms |  63.495 ms |
|                                   |                                       |            |                 |          |            |           |           |            |
|           **EcsContext_CreateEntity** |                       **DestroyEntities** |  **Normal_x4** |               **?** |        **?** |  **74.430 ms** | **0.1858 ms** | **0.1738 ms** |  **74.448 ms** |
|           EcsContext_CreateEntity |                       DestroyEntities | Managed_x4 |               ? |        ? |  61.232 ms | 0.2059 ms | 0.1926 ms |  61.177 ms |
|           EcsContext_CreateEntity |                       DestroyEntities |  Shared_x4 |               ? |        ? |  47.173 ms | 0.1767 ms | 0.1566 ms |  47.174 ms |
|                                   |                                       |            |                 |          |            |           |           |            |
|           **EcsContext_CreateEntity** |                 **DestroyEntities_Reuse** |  **Normal_x4** |               **?** |        **?** |  **42.495 ms** | **0.1371 ms** | **0.1216 ms** |  **42.458 ms** |
|           EcsContext_CreateEntity |                 DestroyEntities_Reuse | Managed_x4 |               ? |        ? |  42.175 ms | 0.0640 ms | 0.0568 ms |  42.150 ms |
|           EcsContext_CreateEntity |                 DestroyEntities_Reuse |  Shared_x4 |               ? |        ? |  42.351 ms | 0.2081 ms | 0.1845 ms |  42.317 ms |
|                                   |                                       |            |                 |          |            |           |           |            |
|           **EcsContext_CreateEntity** |                         **DestroyEntity** |  **Normal_x4** |               **?** |        **?** |  **72.560 ms** | **0.1415 ms** | **0.1181 ms** |  **72.584 ms** |
|           EcsContext_CreateEntity |                         DestroyEntity | Managed_x4 |               ? |        ? |  68.424 ms | 0.3516 ms | 0.3289 ms |  68.333 ms |
|           EcsContext_CreateEntity |                         DestroyEntity |  Shared_x4 |               ? |        ? |  45.841 ms | 0.1652 ms | 0.1379 ms |  45.801 ms |
|                                   |                                       |            |                 |          |            |           |           |            |
|           **EcsContext_CreateEntity** |                   **DestroyEntity_Reuse** |  **Normal_x4** |               **?** |        **?** |  **35.156 ms** | **0.1835 ms** | **0.1627 ms** |  **35.136 ms** |
|           EcsContext_CreateEntity |                   DestroyEntity_Reuse | Managed_x4 |               ? |        ? |  34.359 ms | 0.1105 ms | 0.0980 ms |  34.349 ms |
|           EcsContext_CreateEntity |                   DestroyEntity_Reuse |  Shared_x4 |               ? |        ? |  35.060 ms | 0.1521 ms | 0.1348 ms |  35.040 ms |
|                                   |                                       |            |                 |          |            |           |           |            |
|               **EcsContext_Transfer** |            **TransferEntities_NoDestroy** |  **Normal_x4** |               **?** |        **?** | **111.765 ms** | **0.4660 ms** | **0.3638 ms** | **111.823 ms** |
|               EcsContext_Transfer |            TransferEntities_NoDestroy | Managed_x4 |               ? |        ? | 263.399 ms | 1.4166 ms | 1.3251 ms | 262.655 ms |
|               EcsContext_Transfer |            TransferEntities_NoDestroy |  Shared_x4 |               ? |        ? |  79.141 ms | 0.6314 ms | 0.5906 ms |  79.481 ms |
|                                   |                                       |            |                 |          |            |           |           |            |
|               **EcsContext_Transfer** |     **TransferEntityArcheType_NoDestroy** |  **Normal_x4** |               **?** |        **?** |  **12.542 ms** | **0.2502 ms** | **0.7378 ms** |  **12.869 ms** |
|               EcsContext_Transfer |     TransferEntityArcheType_NoDestroy | Managed_x4 |               ? |        ? |  15.919 ms | 0.4911 ms | 1.4482 ms |  15.893 ms |
|               EcsContext_Transfer |     TransferEntityArcheType_NoDestroy |  Shared_x4 |               ? |        ? |   6.867 ms | 0.2761 ms | 0.8142 ms |   7.284 ms |
|                                   |                                       |            |                 |          |            |           |           |            |
|               **EcsContext_Transfer** |              **TransferEntity_NoDestroy** |  **Normal_x4** |               **?** |        **?** | **168.241 ms** | **0.9517 ms** | **0.8902 ms** | **167.831 ms** |
|               EcsContext_Transfer |              TransferEntity_NoDestroy | Managed_x4 |               ? |        ? | 313.034 ms | 0.5355 ms | 0.5009 ms | 313.086 ms |
|               EcsContext_Transfer |              TransferEntity_NoDestroy |  Shared_x4 |               ? |        ? | 430.896 ms | 2.8180 ms | 2.6359 ms | 430.611 ms |
|                                   |                                       |            |                 |          |            |           |           |            |
|        **EcsContext_UpdateComponent** |                **UpdateComponent_Entity** |  **Normal_x4** |               **?** |        **?** | **213.962 ms** | **0.4100 ms** | **0.3635 ms** | **213.972 ms** |
|        EcsContext_UpdateComponent |                UpdateComponent_Entity | Managed_x4 |               ? |        ? | 254.190 ms | 0.2924 ms | 0.2592 ms | 254.223 ms |
|        EcsContext_UpdateComponent |                UpdateComponent_Entity |  Shared_x4 |               ? |        ? | 968.195 ms | 1.8891 ms | 1.6746 ms | 968.009 ms |
|                                   |                                       |            |                 |          |            |           |           |            |
|       **EcsContext_UpdateComponents** |               **UpdateComponents_Entity** |  **Normal_x4** |               **?** |        **?** | **265.298 ms** | **0.4501 ms** | **0.3759 ms** | **265.130 ms** |
|       EcsContext_UpdateComponents |               UpdateComponents_Entity | Managed_x4 |               ? |        ? | 287.996 ms | 1.0987 ms | 1.0277 ms | 287.601 ms |
|       EcsContext_UpdateComponents |               UpdateComponents_Entity |  Shared_x4 |               ? |        ? | 543.545 ms | 1.4990 ms | 1.4022 ms | 543.730 ms |
|                                   |                                       |            |                 |          |            |           |           |            |
|           **EntityQueryForEachTests** |                               **ForEach** |          **?** |            **R0W0** |       **No** |  **10.642 ms** | **0.0910 ms** | **0.0807 ms** |  **10.616 ms** |
|           EntityQueryForEachTests |                               ForEach |          ? |            R0W0 |      Yes |   2.491 ms | 0.0498 ms | 0.0489 ms |   2.494 ms |
|           EntityQueryForEachTests |                               ForEach |          ? |  R0W4_Normal_x4 |       No |  88.539 ms | 0.3168 ms | 0.2809 ms |  88.468 ms |
|           EntityQueryForEachTests |                               ForEach |          ? |  R0W4_Normal_x4 |      Yes |   8.894 ms | 0.0931 ms | 0.0871 ms |   8.896 ms |
|           EntityQueryForEachTests |                               ForEach |          ? |  R4W0_Normal_x4 |       No |  87.077 ms | 0.6357 ms | 0.5946 ms |  87.251 ms |
|           EntityQueryForEachTests |                               ForEach |          ? |  R4W0_Normal_x4 |      Yes |   8.735 ms | 0.0583 ms | 0.0487 ms |   8.741 ms |
|           EntityQueryForEachTests |                               ForEach |          ? | R0W4_Managed_x4 |       No | 114.774 ms | 0.2112 ms | 0.1763 ms | 114.767 ms |
|           EntityQueryForEachTests |                               ForEach |          ? | R0W4_Managed_x4 |      Yes |  51.160 ms | 0.4258 ms | 0.3983 ms |  51.104 ms |
|           EntityQueryForEachTests |                               ForEach |          ? | R4W0_Managed_x4 |       No | 113.973 ms | 0.5210 ms | 0.4619 ms | 113.832 ms |
|           EntityQueryForEachTests |                               ForEach |          ? | R4W0_Managed_x4 |      Yes |  12.297 ms | 0.0843 ms | 0.0789 ms |  12.316 ms |
|           EntityQueryForEachTests |                               ForEach |          ? |  R0W4_Shared_x4 |       No | 170.552 ms | 0.7308 ms | 0.6478 ms | 170.439 ms |
|           EntityQueryForEachTests |                               ForEach |          ? |  R0W4_Shared_x4 |      Yes |  78.598 ms | 1.0832 ms | 1.0132 ms |  78.638 ms |
|           EntityQueryForEachTests |                               ForEach |          ? |  R4W0_Shared_x4 |       No |  25.768 ms | 0.1670 ms | 0.1562 ms |  25.743 ms |
|           EntityQueryForEachTests |                               ForEach |          ? |  R4W0_Shared_x4 |      Yes |   4.045 ms | 0.0701 ms | 0.0621 ms |   4.038 ms |
