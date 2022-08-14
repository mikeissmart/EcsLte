``` ini

BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19043.1348 (21H1/May2021Update)
AMD Ryzen 9 3900X, 1 CPU, 24 logical and 12 physical cores
  [Host]     : .NET Framework 4.8 (4.8.4420.0), X86 LegacyJIT
  Job-FTOBUC : .NET Framework 4.8 (4.8.4420.0), X86 LegacyJIT

InvocationCount=1  UnrollFactor=1  

```
|                             Type |                                Method |    CompArr |       ReadWrite |         Mean |      Error |     StdDev |       Median |
|--------------------------------- |-------------------------------------- |----------- |---------------- |-------------:|-----------:|-----------:|-------------:|
|          **EcsContext_GetComponent** |                   **GetComponent_Entity** |          **?** |               **?** |   **178.834 ms** |  **1.0422 ms** |  **0.9239 ms** |   **179.089 ms** |
|                                  |                                       |            |                 |              |            |            |              |
|          **EcsContext_GetComponent** |         **GetComponents_EntityArcheType** |          **?** |               **?** |     **4.999 ms** |  **0.1327 ms** |  **0.3787 ms** |     **4.988 ms** |
|                                  |                                       |            |                 |              |            |            |              |
|          **EcsContext_GetComponent** |             **GetComponents_EntityQuery** |          **?** |               **?** |     **5.065 ms** |  **0.1416 ms** |  **0.3993 ms** |     **5.068 ms** |
|                                  |                                       |            |                 |              |            |            |              |
| **EcsContext_UpdateSharedComponent** | **UpdateSharedComponent_EntityArcheType** |          **?** |               **?** |     **4.603 ms** |  **0.0910 ms** |  **0.1687 ms** |     **4.546 ms** |
|                                  |                                       |            |                 |              |            |            |              |
| **EcsContext_UpdateSharedComponent** |     **UpdateSharedComponent_EntityQuery** |          **?** |               **?** |     **6.748 ms** |  **0.1346 ms** |  **0.3118 ms** |     **6.664 ms** |
|                                  |                                       |            |                 |              |            |            |              |
|          **EcsContext_CreateEntity** |                        **CreateEntities** |  **Normal_x4** |               **?** |    **16.380 ms** |  **0.3261 ms** |  **0.8872 ms** |    **16.258 ms** |
|          EcsContext_CreateEntity |                        CreateEntities | Managed_x4 |               ? |    39.250 ms |  0.7706 ms |  1.1295 ms |    39.094 ms |
|          EcsContext_CreateEntity |                        CreateEntities |  Shared_x4 |               ? |     9.932 ms |  0.2326 ms |  0.6637 ms |     9.902 ms |
|                                  |                                       |            |                 |              |            |            |              |
|          **EcsContext_CreateEntity** |                  **CreateEntities_Reuse** |  **Normal_x4** |               **?** |     **9.744 ms** |  **0.3620 ms** |  **1.0618 ms** |     **9.831 ms** |
|          EcsContext_CreateEntity |                  CreateEntities_Reuse | Managed_x4 |               ? |    15.000 ms |  0.3573 ms |  1.0534 ms |    15.157 ms |
|          EcsContext_CreateEntity |                  CreateEntities_Reuse |  Shared_x4 |               ? |     7.470 ms |  0.3469 ms |  1.0229 ms |     7.646 ms |
|                                  |                                       |            |                 |              |            |            |              |
|          **EcsContext_CreateEntity** |                          **CreateEntity** |  **Normal_x4** |               **?** |   **230.808 ms** |  **2.7148 ms** |  **2.5394 ms** |   **229.571 ms** |
|          EcsContext_CreateEntity |                          CreateEntity | Managed_x4 |               ? |   264.741 ms |  1.8372 ms |  1.6286 ms |   264.534 ms |
|          EcsContext_CreateEntity |                          CreateEntity |  Shared_x4 |               ? |   136.927 ms |  1.3987 ms |  1.3084 ms |   136.524 ms |
|                                  |                                       |            |                 |              |            |            |              |
|          **EcsContext_CreateEntity** |                    **CreateEntity_Reuse** |  **Normal_x4** |               **?** |   **202.200 ms** |  **0.9352 ms** |  **0.7809 ms** |   **202.254 ms** |
|          EcsContext_CreateEntity |                    CreateEntity_Reuse | Managed_x4 |               ? |   216.274 ms |  3.5334 ms |  3.1323 ms |   215.549 ms |
|          EcsContext_CreateEntity |                    CreateEntity_Reuse |  Shared_x4 |               ? |   121.762 ms |  0.5580 ms |  0.5219 ms |   121.694 ms |
|                                  |                                       |            |                 |              |            |            |              |
|          **EcsContext_CreateEntity** |                       **DestroyEntities** |  **Normal_x4** |               **?** |    **62.945 ms** |  **0.2093 ms** |  **0.1855 ms** |    **62.964 ms** |
|          EcsContext_CreateEntity |                       DestroyEntities | Managed_x4 |               ? |    85.057 ms |  0.4137 ms |  0.3454 ms |    85.038 ms |
|          EcsContext_CreateEntity |                       DestroyEntities |  Shared_x4 |               ? |    48.498 ms |  0.3783 ms |  0.3354 ms |    48.478 ms |
|                                  |                                       |            |                 |              |            |            |              |
|          **EcsContext_CreateEntity** |                 **DestroyEntities_Reuse** |  **Normal_x4** |               **?** |    **61.604 ms** |  **0.5498 ms** |  **0.4873 ms** |    **61.466 ms** |
|          EcsContext_CreateEntity |                 DestroyEntities_Reuse | Managed_x4 |               ? |    83.012 ms |  0.2852 ms |  0.2382 ms |    83.048 ms |
|          EcsContext_CreateEntity |                 DestroyEntities_Reuse |  Shared_x4 |               ? |    46.344 ms |  0.2794 ms |  0.2477 ms |    46.308 ms |
|                                  |                                       |            |                 |              |            |            |              |
|          **EcsContext_CreateEntity** |                         **DestroyEntity** |  **Normal_x4** |               **?** |    **56.999 ms** |  **0.4463 ms** |  **0.4175 ms** |    **57.007 ms** |
|          EcsContext_CreateEntity |                         DestroyEntity | Managed_x4 |               ? |    79.735 ms |  0.3544 ms |  0.3142 ms |    79.738 ms |
|          EcsContext_CreateEntity |                         DestroyEntity |  Shared_x4 |               ? |    45.232 ms |  0.3929 ms |  0.3483 ms |    45.217 ms |
|                                  |                                       |            |                 |              |            |            |              |
|          **EcsContext_CreateEntity** |                   **DestroyEntity_Reuse** |  **Normal_x4** |               **?** |    **54.570 ms** |  **0.2068 ms** |  **0.1727 ms** |    **54.636 ms** |
|          EcsContext_CreateEntity |                   DestroyEntity_Reuse | Managed_x4 |               ? |    78.552 ms |  0.2226 ms |  0.1973 ms |    78.555 ms |
|          EcsContext_CreateEntity |                   DestroyEntity_Reuse |  Shared_x4 |               ? |    43.109 ms |  0.2143 ms |  0.2005 ms |    43.084 ms |
|                                  |                                       |            |                 |              |            |            |              |
|              **EcsContext_Transfer** |            **TransferEntities_NoDestroy** |  **Normal_x4** |               **?** |   **155.537 ms** |  **0.9238 ms** |  **0.8190 ms** |   **155.717 ms** |
|              EcsContext_Transfer |            TransferEntities_NoDestroy | Managed_x4 |               ? |   214.405 ms |  2.1191 ms |  1.8785 ms |   214.542 ms |
|              EcsContext_Transfer |            TransferEntities_NoDestroy |  Shared_x4 |               ? |   128.438 ms |  0.8867 ms |  0.7860 ms |   128.301 ms |
|                                  |                                       |            |                 |              |            |            |              |
|              **EcsContext_Transfer** |     **TransferEntityArcheType_NoDestroy** |  **Normal_x4** |               **?** |     **9.973 ms** |  **0.3276 ms** |  **0.9451 ms** |    **10.027 ms** |
|              EcsContext_Transfer |     TransferEntityArcheType_NoDestroy | Managed_x4 |               ? |    12.034 ms |  0.3179 ms |  0.9071 ms |    12.069 ms |
|              EcsContext_Transfer |     TransferEntityArcheType_NoDestroy |  Shared_x4 |               ? |     7.646 ms |  0.3445 ms |  1.0159 ms |     7.924 ms |
|                                  |                                       |            |                 |              |            |            |              |
|              **EcsContext_Transfer** |         **TransferEntityQuery_NoDestroy** |  **Normal_x4** |               **?** |    **36.735 ms** |  **0.6127 ms** |  **0.5731 ms** |    **36.745 ms** |
|              EcsContext_Transfer |         TransferEntityQuery_NoDestroy | Managed_x4 |               ? |    94.213 ms |  1.1625 ms |  1.0305 ms |    94.181 ms |
|              EcsContext_Transfer |         TransferEntityQuery_NoDestroy |  Shared_x4 |               ? |    13.091 ms |  0.2608 ms |  0.6199 ms |    12.959 ms |
|                                  |                                       |            |                 |              |            |            |              |
|              **EcsContext_Transfer** |              **TransferEntity_NoDestroy** |  **Normal_x4** |               **?** |   **210.765 ms** |  **1.1560 ms** |  **0.9653 ms** |   **210.588 ms** |
|              EcsContext_Transfer |              TransferEntity_NoDestroy | Managed_x4 |               ? |   271.774 ms |  2.5742 ms |  2.2819 ms |   271.490 ms |
|              EcsContext_Transfer |              TransferEntity_NoDestroy |  Shared_x4 |               ? |   484.583 ms |  5.3925 ms |  4.7803 ms |   484.266 ms |
|                                  |                                       |            |                 |              |            |            |              |
|       **EcsContext_UpdateComponent** |                **UpdateComponent_Entity** |  **Normal_x4** |               **?** |   **241.026 ms** |  **3.6111 ms** |  **3.3778 ms** |   **239.536 ms** |
|       EcsContext_UpdateComponent |                UpdateComponent_Entity | Managed_x4 |               ? |   244.343 ms |  2.4792 ms |  2.3190 ms |   243.445 ms |
|       EcsContext_UpdateComponent |                UpdateComponent_Entity |  Shared_x4 |               ? | 1,032.582 ms |  3.7731 ms |  3.5293 ms | 1,031.412 ms |
|                                  |                                       |            |                 |              |            |            |              |
|          **EntityQueryForEachTests** |                               **ForEach** |          **?** |            **R0W0** |    **10.341 ms** |  **0.2065 ms** |  **0.1830 ms** |    **10.332 ms** |
|          EntityQueryForEachTests |                               ForEach |          ? |  R0W4_Normal_x4 |    83.194 ms |  1.6614 ms |  1.8467 ms |    82.027 ms |
|          EntityQueryForEachTests |                               ForEach |          ? |  R4W0_Normal_x4 |    80.985 ms |  0.3113 ms |  0.2912 ms |    81.013 ms |
|          EntityQueryForEachTests |                               ForEach |          ? | R0W4_Managed_x4 |    83.860 ms |  0.3653 ms |  0.3238 ms |    83.876 ms |
|          EntityQueryForEachTests |                               ForEach |          ? | R4W0_Managed_x4 |    81.457 ms |  0.4146 ms |  0.3878 ms |    81.435 ms |
|          EntityQueryForEachTests |                               ForEach |          ? |  R0W4_Shared_x4 |   908.343 ms | 18.0860 ms | 45.3743 ms |   918.106 ms |
|          EntityQueryForEachTests |                               ForEach |          ? |  R4W0_Shared_x4 |    22.037 ms |  0.1766 ms |  0.1566 ms |    21.995 ms |
