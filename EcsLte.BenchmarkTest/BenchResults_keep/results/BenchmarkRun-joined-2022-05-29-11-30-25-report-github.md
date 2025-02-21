``` ini

BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19043.1348 (21H1/May2021Update)
AMD Ryzen 9 3900X, 1 CPU, 24 logical and 12 physical cores
  [Host]     : .NET Framework 4.8 (4.8.4420.0), X86 LegacyJIT
  Job-FTOBUC : .NET Framework 4.8 (4.8.4420.0), X86 LegacyJIT

InvocationCount=1  UnrollFactor=1  

```
|                             Namespace |                       Type |                            Method | EntityCount |        CompArr |           ReadWrite |         Mean |      Error |     StdDev |
|-------------------------------------- |--------------------------- |---------------------------------- |------------ |--------------- |-------------------- |-------------:|-----------:|-----------:|
|  **EcsLte.BenchmarkTest.EcsContextTests** |    **EcsContext_GetComponent** |               **GetComponent_Entity** |       **10000** |              **?** |                   **?** |    **13.980 ms** |  **0.1455 ms** |  **0.1290 ms** |
|  EcsLte.BenchmarkTest.EcsContextTests |    EcsContext_GetComponent |               GetComponent_Entity |     1000000 |              ? |                   ? | 1,387.907 ms |  5.1496 ms |  4.8170 ms |
|                                       |                            |                                   |             |                |                     |              |            |            |
|  **EcsLte.BenchmarkTest.EcsContextTests** |    **EcsContext_GetComponent** |            **GetComponents_Entities** |       **10000** |              **?** |                   **?** |    **10.614 ms** |  **0.1794 ms** |  **0.1678 ms** |
|  EcsLte.BenchmarkTest.EcsContextTests |    EcsContext_GetComponent |            GetComponents_Entities |     1000000 |              ? |                   ? | 1,081.581 ms |  5.0851 ms |  4.7566 ms |
|                                       |                            |                                   |             |                |                     |              |            |            |
|  **EcsLte.BenchmarkTest.EcsContextTests** |    **EcsContext_GetComponent** |     **GetComponents_EntityArcheType** |       **10000** |              **?** |                   **?** |     **8.592 ms** |  **0.1706 ms** |  **0.1596 ms** |
|  EcsLte.BenchmarkTest.EcsContextTests |    EcsContext_GetComponent |     GetComponents_EntityArcheType |     1000000 |              ? |                   ? |   838.259 ms |  3.1950 ms |  2.4945 ms |
|                                       |                            |                                   |             |                |                     |              |            |            |
|  **EcsLte.BenchmarkTest.EcsContextTests** |    **EcsContext_GetComponent** |         **GetComponents_EntityQuery** |       **10000** |              **?** |                   **?** |     **8.607 ms** |  **0.1605 ms** |  **0.1648 ms** |
|  EcsLte.BenchmarkTest.EcsContextTests |    EcsContext_GetComponent |         GetComponents_EntityQuery |     1000000 |              ? |                   ? |   842.361 ms |  4.6034 ms |  4.3060 ms |
|                                       |                            |                                   |             |                |                     |              |            |            |
|  **EcsLte.BenchmarkTest.EcsContextTests** |    **EcsContext_CreateEntity** |                    **CreateEntities** |           **?** |     **Normal_Bx4** |                   **?** |    **37.567 ms** |  **0.2656 ms** |  **0.2355 ms** |
|  EcsLte.BenchmarkTest.EcsContextTests |    EcsContext_CreateEntity |                    CreateEntities |           ? |     Shared_Bx4 |                   ? |    32.649 ms |  0.2428 ms |  0.2271 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |    EcsContext_CreateEntity |                    CreateEntities |           ? |     Normal_Mx4 |                   ? |    32.133 ms |  0.4352 ms |  0.3858 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |    EcsContext_CreateEntity |                    CreateEntities |           ? |     Shared_Mx4 |                   ? |    32.097 ms |  0.3337 ms |  0.3121 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |    EcsContext_CreateEntity |                    CreateEntities |           ? | Normal_Bx2_Mx2 |                   ? |    37.412 ms |  0.3192 ms |  0.2830 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |    EcsContext_CreateEntity |                    CreateEntities |           ? | Shared_Bx2_Mx2 |                   ? |    38.130 ms |  0.4866 ms |  0.4313 ms |
|                                       |                            |                                   |             |                |                     |              |            |            |
|  **EcsLte.BenchmarkTest.EcsContextTests** |    **EcsContext_CreateEntity** |              **CreateEntities_Reuse** |           **?** |     **Normal_Bx4** |                   **?** |    **29.094 ms** |  **0.5767 ms** |  **0.8453 ms** |
|  EcsLte.BenchmarkTest.EcsContextTests |    EcsContext_CreateEntity |              CreateEntities_Reuse |           ? |     Shared_Bx4 |                   ? |    28.856 ms |  0.5361 ms |  0.5505 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |    EcsContext_CreateEntity |              CreateEntities_Reuse |           ? |     Normal_Mx4 |                   ? |    28.867 ms |  0.5688 ms |  0.6086 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |    EcsContext_CreateEntity |              CreateEntities_Reuse |           ? |     Shared_Mx4 |                   ? |    28.916 ms |  0.5603 ms |  0.6227 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |    EcsContext_CreateEntity |              CreateEntities_Reuse |           ? | Normal_Bx2_Mx2 |                   ? |    29.209 ms |  0.5802 ms |  0.9693 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |    EcsContext_CreateEntity |              CreateEntities_Reuse |           ? | Shared_Bx2_Mx2 |                   ? |    29.088 ms |  0.5782 ms |  0.8293 ms |
|                                       |                            |                                   |             |                |                     |              |            |            |
|  **EcsLte.BenchmarkTest.EcsContextTests** |    **EcsContext_CreateEntity** |                      **CreateEntity** |           **?** |     **Normal_Bx4** |                   **?** |   **518.972 ms** |  **7.9645 ms** |  **7.4500 ms** |
|  EcsLte.BenchmarkTest.EcsContextTests |    EcsContext_CreateEntity |                      CreateEntity |           ? |     Shared_Bx4 |                   ? |   505.280 ms |  3.0439 ms |  2.8473 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |    EcsContext_CreateEntity |                      CreateEntity |           ? |     Normal_Mx4 |                   ? |   507.192 ms |  2.1436 ms |  1.9003 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |    EcsContext_CreateEntity |                      CreateEntity |           ? |     Shared_Mx4 |                   ? |   519.270 ms |  6.2736 ms |  5.5614 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |    EcsContext_CreateEntity |                      CreateEntity |           ? | Normal_Bx2_Mx2 |                   ? |   512.993 ms |  1.8143 ms |  1.6083 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |    EcsContext_CreateEntity |                      CreateEntity |           ? | Shared_Bx2_Mx2 |                   ? |   512.600 ms |  3.9537 ms |  3.5049 ms |
|                                       |                            |                                   |             |                |                     |              |            |            |
|  **EcsLte.BenchmarkTest.EcsContextTests** |    **EcsContext_CreateEntity** |                **CreateEntity_Reuse** |           **?** |     **Normal_Bx4** |                   **?** |   **485.071 ms** |  **2.4180 ms** |  **2.2618 ms** |
|  EcsLte.BenchmarkTest.EcsContextTests |    EcsContext_CreateEntity |                CreateEntity_Reuse |           ? |     Shared_Bx4 |                   ? |   498.317 ms |  2.8990 ms |  2.5699 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |    EcsContext_CreateEntity |                CreateEntity_Reuse |           ? |     Normal_Mx4 |                   ? |   484.552 ms |  1.7754 ms |  1.5739 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |    EcsContext_CreateEntity |                CreateEntity_Reuse |           ? |     Shared_Mx4 |                   ? |   499.280 ms |  2.1845 ms |  2.0434 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |    EcsContext_CreateEntity |                CreateEntity_Reuse |           ? | Normal_Bx2_Mx2 |                   ? |   484.162 ms |  2.2451 ms |  1.8748 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |    EcsContext_CreateEntity |                CreateEntity_Reuse |           ? | Shared_Bx2_Mx2 |                   ? |   494.143 ms |  1.8008 ms |  1.6845 ms |
|                                       |                            |                                   |             |                |                     |              |            |            |
|  **EcsLte.BenchmarkTest.EcsContextTests** |    **EcsContext_CreateEntity** |                   **DestroyEntities** |           **?** |     **Normal_Bx4** |                   **?** |    **48.732 ms** |  **0.4806 ms** |  **0.4496 ms** |
|  EcsLte.BenchmarkTest.EcsContextTests |    EcsContext_CreateEntity |                   DestroyEntities |           ? |     Shared_Bx4 |                   ? |    48.467 ms |  0.3762 ms |  0.3519 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |    EcsContext_CreateEntity |                   DestroyEntities |           ? |     Normal_Mx4 |                   ? |    48.662 ms |  0.4828 ms |  0.4517 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |    EcsContext_CreateEntity |                   DestroyEntities |           ? |     Shared_Mx4 |                   ? |    50.141 ms |  0.4460 ms |  0.3954 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |    EcsContext_CreateEntity |                   DestroyEntities |           ? | Normal_Bx2_Mx2 |                   ? |    50.271 ms |  0.6025 ms |  0.5341 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |    EcsContext_CreateEntity |                   DestroyEntities |           ? | Shared_Bx2_Mx2 |                   ? |    49.916 ms |  0.4609 ms |  0.4311 ms |
|                                       |                            |                                   |             |                |                     |              |            |            |
|  **EcsLte.BenchmarkTest.EcsContextTests** |    **EcsContext_CreateEntity** |             **DestroyEntities_Reuse** |           **?** |     **Normal_Bx4** |                   **?** |    **44.754 ms** |  **0.2273 ms** |  **0.1898 ms** |
|  EcsLte.BenchmarkTest.EcsContextTests |    EcsContext_CreateEntity |             DestroyEntities_Reuse |           ? |     Shared_Bx4 |                   ? |    44.681 ms |  0.1851 ms |  0.1641 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |    EcsContext_CreateEntity |             DestroyEntities_Reuse |           ? |     Normal_Mx4 |                   ? |    44.657 ms |  0.1852 ms |  0.1547 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |    EcsContext_CreateEntity |             DestroyEntities_Reuse |           ? |     Shared_Mx4 |                   ? |    44.604 ms |  0.2318 ms |  0.2168 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |    EcsContext_CreateEntity |             DestroyEntities_Reuse |           ? | Normal_Bx2_Mx2 |                   ? |    45.299 ms |  0.2765 ms |  0.2309 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |    EcsContext_CreateEntity |             DestroyEntities_Reuse |           ? | Shared_Bx2_Mx2 |                   ? |    44.535 ms |  0.1385 ms |  0.1156 ms |
|                                       |                            |                                   |             |                |                     |              |            |            |
|  **EcsLte.BenchmarkTest.EcsContextTests** |    **EcsContext_CreateEntity** |                     **DestroyEntity** |           **?** |     **Normal_Bx4** |                   **?** |    **58.536 ms** |  **0.5482 ms** |  **0.5128 ms** |
|  EcsLte.BenchmarkTest.EcsContextTests |    EcsContext_CreateEntity |                     DestroyEntity |           ? |     Shared_Bx4 |                   ? |    58.368 ms |  0.4415 ms |  0.4130 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |    EcsContext_CreateEntity |                     DestroyEntity |           ? |     Normal_Mx4 |                   ? |    57.707 ms |  0.3160 ms |  0.2639 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |    EcsContext_CreateEntity |                     DestroyEntity |           ? |     Shared_Mx4 |                   ? |    59.615 ms |  0.4316 ms |  0.4037 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |    EcsContext_CreateEntity |                     DestroyEntity |           ? | Normal_Bx2_Mx2 |                   ? |    59.501 ms |  0.7574 ms |  0.7085 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |    EcsContext_CreateEntity |                     DestroyEntity |           ? | Shared_Bx2_Mx2 |                   ? |    59.385 ms |  0.7423 ms |  0.6943 ms |
|                                       |                            |                                   |             |                |                     |              |            |            |
|  **EcsLte.BenchmarkTest.EcsContextTests** |    **EcsContext_CreateEntity** |               **DestroyEntity_Reuse** |           **?** |     **Normal_Bx4** |                   **?** |    **54.392 ms** |  **0.4971 ms** |  **0.4650 ms** |
|  EcsLte.BenchmarkTest.EcsContextTests |    EcsContext_CreateEntity |               DestroyEntity_Reuse |           ? |     Shared_Bx4 |                   ? |    53.938 ms |  0.1401 ms |  0.1170 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |    EcsContext_CreateEntity |               DestroyEntity_Reuse |           ? |     Normal_Mx4 |                   ? |    54.146 ms |  0.1757 ms |  0.1557 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |    EcsContext_CreateEntity |               DestroyEntity_Reuse |           ? |     Shared_Mx4 |                   ? |    54.351 ms |  0.2499 ms |  0.2338 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |    EcsContext_CreateEntity |               DestroyEntity_Reuse |           ? | Normal_Bx2_Mx2 |                   ? |    54.336 ms |  0.2026 ms |  0.1796 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |    EcsContext_CreateEntity |               DestroyEntity_Reuse |           ? | Shared_Bx2_Mx2 |                   ? |    54.534 ms |  0.3973 ms |  0.3522 ms |
|                                       |                            |                                   |             |                |                     |              |            |            |
|  **EcsLte.BenchmarkTest.EcsContextTests** |        **EcsContext_Transfer** |          **TransferEntities_Destroy** |           **?** |     **Normal_Bx4** |                   **?** |   **152.970 ms** |  **2.3713 ms** |  **1.9802 ms** |
|  EcsLte.BenchmarkTest.EcsContextTests |        EcsContext_Transfer |          TransferEntities_Destroy |           ? |     Shared_Bx4 |                   ? |   152.087 ms |  2.9006 ms |  3.2240 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |        EcsContext_Transfer |          TransferEntities_Destroy |           ? |     Normal_Mx4 |                   ? |   156.961 ms |  2.4352 ms |  2.1588 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |        EcsContext_Transfer |          TransferEntities_Destroy |           ? |     Shared_Mx4 |                   ? |   149.613 ms |  2.7419 ms |  3.1576 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |        EcsContext_Transfer |          TransferEntities_Destroy |           ? | Normal_Bx2_Mx2 |                   ? |   157.620 ms |  3.0620 ms |  4.7672 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |        EcsContext_Transfer |          TransferEntities_Destroy |           ? | Shared_Bx2_Mx2 |                   ? |   150.657 ms |  1.9801 ms |  1.7553 ms |
|                                       |                            |                                   |             |                |                     |              |            |            |
|  **EcsLte.BenchmarkTest.EcsContextTests** |        **EcsContext_Transfer** |        **TransferEntities_NoDestroy** |           **?** |     **Normal_Bx4** |                   **?** |   **113.404 ms** |  **2.2638 ms** |  **3.9049 ms** |
|  EcsLte.BenchmarkTest.EcsContextTests |        EcsContext_Transfer |        TransferEntities_NoDestroy |           ? |     Shared_Bx4 |                   ? |   112.653 ms |  2.2312 ms |  3.6659 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |        EcsContext_Transfer |        TransferEntities_NoDestroy |           ? |     Normal_Mx4 |                   ? |   112.792 ms |  2.2353 ms |  3.9149 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |        EcsContext_Transfer |        TransferEntities_NoDestroy |           ? |     Shared_Mx4 |                   ? |   110.577 ms |  2.0400 ms |  2.7924 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |        EcsContext_Transfer |        TransferEntities_NoDestroy |           ? | Normal_Bx2_Mx2 |                   ? |   112.986 ms |  2.1804 ms |  2.5109 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |        EcsContext_Transfer |        TransferEntities_NoDestroy |           ? | Shared_Bx2_Mx2 |                   ? |   110.715 ms |  2.0942 ms |  2.2407 ms |
|                                       |                            |                                   |             |                |                     |              |            |            |
|  **EcsLte.BenchmarkTest.EcsContextTests** |        **EcsContext_Transfer** |   **TransferEntityArcheType_Destroy** |           **?** |     **Normal_Bx4** |                   **?** |    **43.637 ms** |  **0.8683 ms** |  **0.8122 ms** |
|  EcsLte.BenchmarkTest.EcsContextTests |        EcsContext_Transfer |   TransferEntityArcheType_Destroy |           ? |     Shared_Bx4 |                   ? |    43.960 ms |  0.8650 ms |  0.8091 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |        EcsContext_Transfer |   TransferEntityArcheType_Destroy |           ? |     Normal_Mx4 |                   ? |    40.323 ms |  0.7922 ms |  0.9123 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |        EcsContext_Transfer |   TransferEntityArcheType_Destroy |           ? |     Shared_Mx4 |                   ? |    43.448 ms |  0.8337 ms |  0.8561 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |        EcsContext_Transfer |   TransferEntityArcheType_Destroy |           ? | Normal_Bx2_Mx2 |                   ? |    43.238 ms |  0.8121 ms |  0.7597 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |        EcsContext_Transfer |   TransferEntityArcheType_Destroy |           ? | Shared_Bx2_Mx2 |                   ? |    43.396 ms |  0.8271 ms |  0.8123 ms |
|                                       |                            |                                   |             |                |                     |              |            |            |
|  **EcsLte.BenchmarkTest.EcsContextTests** |        **EcsContext_Transfer** | **TransferEntityArcheType_NoDestroy** |           **?** |     **Normal_Bx4** |                   **?** |    **34.404 ms** |  **0.6739 ms** |  **0.9447 ms** |
|  EcsLte.BenchmarkTest.EcsContextTests |        EcsContext_Transfer | TransferEntityArcheType_NoDestroy |           ? |     Shared_Bx4 |                   ? |    34.453 ms |  0.6640 ms |  0.8397 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |        EcsContext_Transfer | TransferEntityArcheType_NoDestroy |           ? |     Normal_Mx4 |                   ? |    34.564 ms |  0.6790 ms |  0.9519 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |        EcsContext_Transfer | TransferEntityArcheType_NoDestroy |           ? |     Shared_Mx4 |                   ? |    34.303 ms |  0.6762 ms |  0.9698 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |        EcsContext_Transfer | TransferEntityArcheType_NoDestroy |           ? | Normal_Bx2_Mx2 |                   ? |    34.291 ms |  0.6608 ms |  0.9045 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |        EcsContext_Transfer | TransferEntityArcheType_NoDestroy |           ? | Shared_Bx2_Mx2 |                   ? |    34.498 ms |  0.6730 ms |  0.9212 ms |
|                                       |                            |                                   |             |                |                     |              |            |            |
|  **EcsLte.BenchmarkTest.EcsContextTests** |        **EcsContext_Transfer** |       **TransferEntityQuery_Destroy** |           **?** |     **Normal_Bx4** |                   **?** |    **44.037 ms** |  **0.8671 ms** |  **1.0322 ms** |
|  EcsLte.BenchmarkTest.EcsContextTests |        EcsContext_Transfer |       TransferEntityQuery_Destroy |           ? |     Shared_Bx4 |                   ? |    44.017 ms |  0.8390 ms |  0.8616 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |        EcsContext_Transfer |       TransferEntityQuery_Destroy |           ? |     Normal_Mx4 |                   ? |    44.905 ms |  0.8724 ms |  1.0714 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |        EcsContext_Transfer |       TransferEntityQuery_Destroy |           ? |     Shared_Mx4 |                   ? |    45.004 ms |  0.8515 ms |  0.9111 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |        EcsContext_Transfer |       TransferEntityQuery_Destroy |           ? | Normal_Bx2_Mx2 |                   ? |    43.478 ms |  0.8570 ms |  0.8801 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |        EcsContext_Transfer |       TransferEntityQuery_Destroy |           ? | Shared_Bx2_Mx2 |                   ? |    43.644 ms |  0.8289 ms |  0.9867 ms |
|                                       |                            |                                   |             |                |                     |              |            |            |
|  **EcsLte.BenchmarkTest.EcsContextTests** |        **EcsContext_Transfer** |     **TransferEntityQuery_NoDestroy** |           **?** |     **Normal_Bx4** |                   **?** |    **34.636 ms** |  **0.6878 ms** |  **0.8447 ms** |
|  EcsLte.BenchmarkTest.EcsContextTests |        EcsContext_Transfer |     TransferEntityQuery_NoDestroy |           ? |     Shared_Bx4 |                   ? |    34.191 ms |  0.6834 ms |  0.7870 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |        EcsContext_Transfer |     TransferEntityQuery_NoDestroy |           ? |     Normal_Mx4 |                   ? |    34.733 ms |  0.6905 ms |  1.0121 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |        EcsContext_Transfer |     TransferEntityQuery_NoDestroy |           ? |     Shared_Mx4 |                   ? |    34.491 ms |  0.6766 ms |  0.9262 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |        EcsContext_Transfer |     TransferEntityQuery_NoDestroy |           ? | Normal_Bx2_Mx2 |                   ? |    34.597 ms |  0.6824 ms |  0.8873 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |        EcsContext_Transfer |     TransferEntityQuery_NoDestroy |           ? | Shared_Bx2_Mx2 |                   ? |    34.501 ms |  0.6877 ms |  0.8446 ms |
|                                       |                            |                                   |             |                |                     |              |            |            |
|  **EcsLte.BenchmarkTest.EcsContextTests** |        **EcsContext_Transfer** |            **TransferEntity_Destroy** |           **?** |     **Normal_Bx4** |                   **?** |   **249.631 ms** |  **1.3729 ms** |  **1.2842 ms** |
|  EcsLte.BenchmarkTest.EcsContextTests |        EcsContext_Transfer |            TransferEntity_Destroy |           ? |     Shared_Bx4 |                   ? |   283.533 ms |  1.9580 ms |  1.8315 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |        EcsContext_Transfer |            TransferEntity_Destroy |           ? |     Normal_Mx4 |                   ? |   250.529 ms |  1.6131 ms |  1.5089 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |        EcsContext_Transfer |            TransferEntity_Destroy |           ? |     Shared_Mx4 |                   ? |   282.995 ms |  0.7295 ms |  0.6467 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |        EcsContext_Transfer |            TransferEntity_Destroy |           ? | Normal_Bx2_Mx2 |                   ? |   247.154 ms |  0.6062 ms |  0.5374 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |        EcsContext_Transfer |            TransferEntity_Destroy |           ? | Shared_Bx2_Mx2 |                   ? |   287.554 ms |  4.2130 ms |  3.9408 ms |
|                                       |                            |                                   |             |                |                     |              |            |            |
|  **EcsLte.BenchmarkTest.EcsContextTests** |        **EcsContext_Transfer** |          **TransferEntity_NoDestroy** |           **?** |     **Normal_Bx4** |                   **?** |   **212.523 ms** |  **2.5337 ms** |  **2.2461 ms** |
|  EcsLte.BenchmarkTest.EcsContextTests |        EcsContext_Transfer |          TransferEntity_NoDestroy |           ? |     Shared_Bx4 |                   ? |   246.413 ms |  1.5844 ms |  1.4821 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |        EcsContext_Transfer |          TransferEntity_NoDestroy |           ? |     Normal_Mx4 |                   ? |   210.219 ms |  1.3939 ms |  1.3039 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |        EcsContext_Transfer |          TransferEntity_NoDestroy |           ? |     Shared_Mx4 |                   ? |   244.275 ms |  0.4852 ms |  0.3788 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |        EcsContext_Transfer |          TransferEntity_NoDestroy |           ? | Normal_Bx2_Mx2 |                   ? |   209.953 ms |  1.0560 ms |  0.9877 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |        EcsContext_Transfer |          TransferEntity_NoDestroy |           ? | Shared_Bx2_Mx2 |                   ? |   245.258 ms |  0.6228 ms |  0.5521 ms |
|                                       |                            |                                   |             |                |                     |              |            |            |
|  **EcsLte.BenchmarkTest.EcsContextTests** | **EcsContext_UpdateComponent** |            **UpdateComponent_Entity** |           **?** |     **Normal_Bx4** |                   **?** |   **650.664 ms** |  **3.7445 ms** |  **3.5026 ms** |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_UpdateComponent |            UpdateComponent_Entity |           ? |     Shared_Bx4 |                   ? | 1,686.312 ms |  4.7849 ms |  4.4758 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_UpdateComponent |            UpdateComponent_Entity |           ? |     Normal_Mx4 |                   ? |   647.798 ms |  3.3091 ms |  2.9334 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_UpdateComponent |            UpdateComponent_Entity |           ? |     Shared_Mx4 |                   ? | 1,694.471 ms |  8.2118 ms |  6.8572 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_UpdateComponent |            UpdateComponent_Entity |           ? | Normal_Bx2_Mx2 |                   ? |   647.218 ms |  4.2950 ms |  4.0175 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_UpdateComponent |            UpdateComponent_Entity |           ? | Shared_Bx2_Mx2 |                   ? | 1,686.219 ms |  4.4197 ms |  4.1342 ms |
|                                       |                            |                                   |             |                |                     |              |            |            |
|  **EcsLte.BenchmarkTest.EcsContextTests** | **EcsContext_UpdateComponent** |         **UpdateComponents_Entities** |           **?** |     **Normal_Bx4** |                   **?** |   **558.419 ms** |  **2.4330 ms** |  **2.2758 ms** |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_UpdateComponent |         UpdateComponents_Entities |           ? |     Shared_Bx4 |                   ? |   792.666 ms |  2.3854 ms |  2.2313 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_UpdateComponent |         UpdateComponents_Entities |           ? |     Normal_Mx4 |                   ? |   558.151 ms |  1.7500 ms |  1.5513 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_UpdateComponent |         UpdateComponents_Entities |           ? |     Shared_Mx4 |                   ? |   793.221 ms |  5.7057 ms |  5.0579 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_UpdateComponent |         UpdateComponents_Entities |           ? | Normal_Bx2_Mx2 |                   ? |   564.157 ms |  9.2106 ms |  8.6156 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_UpdateComponent |         UpdateComponents_Entities |           ? | Shared_Bx2_Mx2 |                   ? |   790.087 ms |  3.4885 ms |  3.2631 ms |
|                                       |                            |                                   |             |                |                     |              |            |            |
|  **EcsLte.BenchmarkTest.EcsContextTests** | **EcsContext_UpdateComponent** |  **UpdateComponents_EntityArcheType** |           **?** |     **Normal_Bx4** |                   **?** |    **79.036 ms** |  **0.4069 ms** |  **0.3806 ms** |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_UpdateComponent |  UpdateComponents_EntityArcheType |           ? |     Shared_Bx4 |                   ? |    49.620 ms |  0.9867 ms |  1.0132 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_UpdateComponent |  UpdateComponents_EntityArcheType |           ? |     Normal_Mx4 |                   ? |    78.994 ms |  0.4129 ms |  0.3863 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_UpdateComponent |  UpdateComponents_EntityArcheType |           ? |     Shared_Mx4 |                   ? |    49.698 ms |  0.9452 ms |  0.9707 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_UpdateComponent |  UpdateComponents_EntityArcheType |           ? | Normal_Bx2_Mx2 |                   ? |    78.997 ms |  0.3783 ms |  0.3538 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_UpdateComponent |  UpdateComponents_EntityArcheType |           ? | Shared_Bx2_Mx2 |                   ? |    50.572 ms |  0.8277 ms |  0.7742 ms |
|                                       |                            |                                   |             |                |                     |              |            |            |
|  **EcsLte.BenchmarkTest.EcsContextTests** | **EcsContext_UpdateComponent** |      **UpdateComponents_EntityQuery** |           **?** |     **Normal_Bx4** |                   **?** |    **79.325 ms** |  **0.5560 ms** |  **0.5201 ms** |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_UpdateComponent |      UpdateComponents_EntityQuery |           ? |     Shared_Bx4 |                   ? |   177.935 ms |  1.6490 ms |  1.5425 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_UpdateComponent |      UpdateComponents_EntityQuery |           ? |     Normal_Mx4 |                   ? |    79.023 ms |  0.4363 ms |  0.4081 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_UpdateComponent |      UpdateComponents_EntityQuery |           ? |     Shared_Mx4 |                   ? |   177.168 ms |  1.3552 ms |  1.2677 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_UpdateComponent |      UpdateComponents_EntityQuery |           ? | Normal_Bx2_Mx2 |                   ? |    78.772 ms |  0.2170 ms |  0.1924 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_UpdateComponent |      UpdateComponents_EntityQuery |           ? | Shared_Bx2_Mx2 |                   ? |   177.722 ms |  1.9421 ms |  1.8166 ms |
|                                       |                            |                                   |             |                |                     |              |            |            |
| **EcsLte.BenchmarkTest.EntityQueryTests** |        **EntityQuery_ForEach** |                           **ForEach** |           **?** |              **?** |                **R0W0** |    **77.689 ms** |  **0.7449 ms** |  **0.6603 ms** |
| EcsLte.BenchmarkTest.EntityQueryTests |        EntityQuery_ForEach |                           ForEach |           ? |              ? |     R0W4_Normal_Bx4 | 1,197.196 ms |  7.9004 ms |  7.3900 ms |
| EcsLte.BenchmarkTest.EntityQueryTests |        EntityQuery_ForEach |                           ForEach |           ? |              ? |     R4W0_Normal_Bx4 | 1,011.731 ms |  6.7345 ms |  6.2995 ms |
| EcsLte.BenchmarkTest.EntityQueryTests |        EntityQuery_ForEach |                           ForEach |           ? |              ? |     R0W4_Normal_Mx4 | 1,196.041 ms |  7.1430 ms |  6.6815 ms |
| EcsLte.BenchmarkTest.EntityQueryTests |        EntityQuery_ForEach |                           ForEach |           ? |              ? |     R4W0_Normal_Mx4 | 1,003.245 ms |  7.3547 ms |  6.5198 ms |
| EcsLte.BenchmarkTest.EntityQueryTests |        EntityQuery_ForEach |                           ForEach |           ? |              ? |     R0W4_Shared_Bx4 | 1,779.021 ms |  4.3601 ms |  3.6408 ms |
| EcsLte.BenchmarkTest.EntityQueryTests |        EntityQuery_ForEach |                           ForEach |           ? |              ? |     R4W0_Shared_Bx4 |   995.181 ms |  8.1146 ms |  7.1934 ms |
| EcsLte.BenchmarkTest.EntityQueryTests |        EntityQuery_ForEach |                           ForEach |           ? |              ? |     R0W4_Shared_Mx4 | 1,771.184 ms |  3.5506 ms |  2.9649 ms |
| EcsLte.BenchmarkTest.EntityQueryTests |        EntityQuery_ForEach |                           ForEach |           ? |              ? |     R4W0_Shared_Mx4 | 1,007.372 ms | 11.9748 ms | 11.2013 ms |
| EcsLte.BenchmarkTest.EntityQueryTests |        EntityQuery_ForEach |                           ForEach |           ? |              ? | R0W4_Normal_Bx2_Mx2 | 1,195.828 ms |  5.2206 ms |  4.6279 ms |
| EcsLte.BenchmarkTest.EntityQueryTests |        EntityQuery_ForEach |                           ForEach |           ? |              ? | R4W0_Normal_Bx2_Mx2 | 1,008.422 ms | 11.1529 ms | 10.4325 ms |
| EcsLte.BenchmarkTest.EntityQueryTests |        EntityQuery_ForEach |                           ForEach |           ? |              ? | R0W4_Shared_Bx2_Mx2 | 1,776.386 ms |  8.5633 ms |  8.0102 ms |
| EcsLte.BenchmarkTest.EntityQueryTests |        EntityQuery_ForEach |                           ForEach |           ? |              ? | R4W0_Shared_Bx2_Mx2 | 1,001.571 ms | 10.6103 ms |  9.9249 ms |
