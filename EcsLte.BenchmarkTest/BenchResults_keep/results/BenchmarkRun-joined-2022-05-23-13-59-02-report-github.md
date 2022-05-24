``` ini

BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19043.1348 (21H1/May2021Update)
AMD Ryzen 9 3900X, 1 CPU, 24 logical and 12 physical cores
  [Host]     : .NET Framework 4.8 (4.8.4420.0), X86 LegacyJIT
  Job-FTOBUC : .NET Framework 4.8 (4.8.4420.0), X86 LegacyJIT

InvocationCount=1  UnrollFactor=1  

```
|                             Namespace |                       Type |                            Method |        CompArr |           ReadWrite |        Mean |    Error |   StdDev |
|-------------------------------------- |--------------------------- |---------------------------------- |--------------- |-------------------- |------------:|---------:|---------:|
|  **EcsLte.BenchmarkTest.EcsContextTests** |    **EcsContext_CreateEntity** |                    **CreateEntities** |     **Normal_Bx4** |                   **?** |    **31.68 ms** | **0.238 ms** | **0.199 ms** |
|  EcsLte.BenchmarkTest.EcsContextTests |    EcsContext_CreateEntity |                    CreateEntities |     Shared_Bx4 |                   ? |    31.45 ms | 0.205 ms | 0.182 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |    EcsContext_CreateEntity |                    CreateEntities |     Normal_Mx4 |                   ? |    32.09 ms | 0.263 ms | 0.246 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |    EcsContext_CreateEntity |                    CreateEntities |     Shared_Mx4 |                   ? |    31.79 ms | 0.278 ms | 0.246 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |    EcsContext_CreateEntity |                    CreateEntities | Normal_Bx2_Mx2 |                   ? |    31.78 ms | 0.432 ms | 0.404 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |    EcsContext_CreateEntity |                    CreateEntities | Shared_Bx2_Mx2 |                   ? |    31.43 ms | 0.178 ms | 0.166 ms |
|                                       |                            |                                   |                |                     |             |          |          |
|  **EcsLte.BenchmarkTest.EcsContextTests** |    **EcsContext_CreateEntity** |              **CreateEntities_Reuse** |     **Normal_Bx4** |                   **?** |    **28.73 ms** | **0.559 ms** | **0.621 ms** |
|  EcsLte.BenchmarkTest.EcsContextTests |    EcsContext_CreateEntity |              CreateEntities_Reuse |     Shared_Bx4 |                   ? |    29.19 ms | 0.565 ms | 0.555 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |    EcsContext_CreateEntity |              CreateEntities_Reuse |     Normal_Mx4 |                   ? |    28.74 ms | 0.558 ms | 0.548 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |    EcsContext_CreateEntity |              CreateEntities_Reuse |     Shared_Mx4 |                   ? |    29.16 ms | 0.581 ms | 0.597 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |    EcsContext_CreateEntity |              CreateEntities_Reuse | Normal_Bx2_Mx2 |                   ? |    28.64 ms | 0.557 ms | 0.572 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |    EcsContext_CreateEntity |              CreateEntities_Reuse | Shared_Bx2_Mx2 |                   ? |    28.93 ms | 0.555 ms | 0.519 ms |
|                                       |                            |                                   |                |                     |             |          |          |
|  **EcsLte.BenchmarkTest.EcsContextTests** |    **EcsContext_CreateEntity** |                      **CreateEntity** |     **Normal_Bx4** |                   **?** |   **520.76 ms** | **1.595 ms** | **1.332 ms** |
|  EcsLte.BenchmarkTest.EcsContextTests |    EcsContext_CreateEntity |                      CreateEntity |     Shared_Bx4 |                   ? |   517.21 ms | 2.450 ms | 1.913 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |    EcsContext_CreateEntity |                      CreateEntity |     Normal_Mx4 |                   ? |   525.91 ms | 2.282 ms | 1.782 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |    EcsContext_CreateEntity |                      CreateEntity |     Shared_Mx4 |                   ? |   521.27 ms | 2.340 ms | 2.189 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |    EcsContext_CreateEntity |                      CreateEntity | Normal_Bx2_Mx2 |                   ? |   529.65 ms | 6.153 ms | 5.756 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |    EcsContext_CreateEntity |                      CreateEntity | Shared_Bx2_Mx2 |                   ? |   523.47 ms | 3.568 ms | 3.337 ms |
|                                       |                            |                                   |                |                     |             |          |          |
|  **EcsLte.BenchmarkTest.EcsContextTests** |    **EcsContext_CreateEntity** |                **CreateEntity_Reuse** |     **Normal_Bx4** |                   **?** |   **504.45 ms** | **1.400 ms** | **1.241 ms** |
|  EcsLte.BenchmarkTest.EcsContextTests |    EcsContext_CreateEntity |                CreateEntity_Reuse |     Shared_Bx4 |                   ? |   504.40 ms | 1.515 ms | 1.418 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |    EcsContext_CreateEntity |                CreateEntity_Reuse |     Normal_Mx4 |                   ? |   511.10 ms | 5.724 ms | 5.354 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |    EcsContext_CreateEntity |                CreateEntity_Reuse |     Shared_Mx4 |                   ? |   506.24 ms | 0.842 ms | 0.746 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |    EcsContext_CreateEntity |                CreateEntity_Reuse | Normal_Bx2_Mx2 |                   ? |   502.36 ms | 2.095 ms | 1.959 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |    EcsContext_CreateEntity |                CreateEntity_Reuse | Shared_Bx2_Mx2 |                   ? |   510.84 ms | 4.613 ms | 4.315 ms |
|                                       |                            |                                   |                |                     |             |          |          |
|  **EcsLte.BenchmarkTest.EcsContextTests** |    **EcsContext_CreateEntity** |                   **DestroyEntities** |     **Normal_Bx4** |                   **?** |    **63.48 ms** | **0.280 ms** | **0.249 ms** |
|  EcsLte.BenchmarkTest.EcsContextTests |    EcsContext_CreateEntity |                   DestroyEntities |     Shared_Bx4 |                   ? |    63.85 ms | 0.355 ms | 0.297 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |    EcsContext_CreateEntity |                   DestroyEntities |     Normal_Mx4 |                   ? |    64.11 ms | 0.373 ms | 0.291 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |    EcsContext_CreateEntity |                   DestroyEntities |     Shared_Mx4 |                   ? |    63.54 ms | 0.304 ms | 0.269 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |    EcsContext_CreateEntity |                   DestroyEntities | Normal_Bx2_Mx2 |                   ? |    63.33 ms | 0.206 ms | 0.172 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |    EcsContext_CreateEntity |                   DestroyEntities | Shared_Bx2_Mx2 |                   ? |    63.72 ms | 0.466 ms | 0.389 ms |
|                                       |                            |                                   |                |                     |             |          |          |
|  **EcsLte.BenchmarkTest.EcsContextTests** |    **EcsContext_CreateEntity** |             **DestroyEntities_Reuse** |     **Normal_Bx4** |                   **?** |    **59.60 ms** | **0.222 ms** | **0.208 ms** |
|  EcsLte.BenchmarkTest.EcsContextTests |    EcsContext_CreateEntity |             DestroyEntities_Reuse |     Shared_Bx4 |                   ? |    59.67 ms | 0.215 ms | 0.201 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |    EcsContext_CreateEntity |             DestroyEntities_Reuse |     Normal_Mx4 |                   ? |    59.85 ms | 0.090 ms | 0.080 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |    EcsContext_CreateEntity |             DestroyEntities_Reuse |     Shared_Mx4 |                   ? |    59.45 ms | 0.233 ms | 0.218 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |    EcsContext_CreateEntity |             DestroyEntities_Reuse | Normal_Bx2_Mx2 |                   ? |    59.49 ms | 0.115 ms | 0.090 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |    EcsContext_CreateEntity |             DestroyEntities_Reuse | Shared_Bx2_Mx2 |                   ? |    59.35 ms | 0.097 ms | 0.081 ms |
|                                       |                            |                                   |                |                     |             |          |          |
|  **EcsLte.BenchmarkTest.EcsContextTests** |    **EcsContext_CreateEntity** |                     **DestroyEntity** |     **Normal_Bx4** |                   **?** |    **75.21 ms** | **0.278 ms** | **0.246 ms** |
|  EcsLte.BenchmarkTest.EcsContextTests |    EcsContext_CreateEntity |                     DestroyEntity |     Shared_Bx4 |                   ? |    75.08 ms | 0.321 ms | 0.300 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |    EcsContext_CreateEntity |                     DestroyEntity |     Normal_Mx4 |                   ? |    75.71 ms | 0.438 ms | 0.409 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |    EcsContext_CreateEntity |                     DestroyEntity |     Shared_Mx4 |                   ? |    75.35 ms | 0.316 ms | 0.264 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |    EcsContext_CreateEntity |                     DestroyEntity | Normal_Bx2_Mx2 |                   ? |    75.23 ms | 0.231 ms | 0.216 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |    EcsContext_CreateEntity |                     DestroyEntity | Shared_Bx2_Mx2 |                   ? |    75.41 ms | 0.273 ms | 0.228 ms |
|                                       |                            |                                   |                |                     |             |          |          |
|  **EcsLte.BenchmarkTest.EcsContextTests** |    **EcsContext_CreateEntity** |               **DestroyEntity_Reuse** |     **Normal_Bx4** |                   **?** |    **72.20 ms** | **0.372 ms** | **0.348 ms** |
|  EcsLte.BenchmarkTest.EcsContextTests |    EcsContext_CreateEntity |               DestroyEntity_Reuse |     Shared_Bx4 |                   ? |    71.44 ms | 0.266 ms | 0.236 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |    EcsContext_CreateEntity |               DestroyEntity_Reuse |     Normal_Mx4 |                   ? |    73.05 ms | 0.436 ms | 0.408 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |    EcsContext_CreateEntity |               DestroyEntity_Reuse |     Shared_Mx4 |                   ? |    71.16 ms | 0.273 ms | 0.255 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |    EcsContext_CreateEntity |               DestroyEntity_Reuse | Normal_Bx2_Mx2 |                   ? |    71.94 ms | 0.193 ms | 0.161 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |    EcsContext_CreateEntity |               DestroyEntity_Reuse | Shared_Bx2_Mx2 |                   ? |    70.68 ms | 0.278 ms | 0.260 ms |
|                                       |                            |                                   |                |                     |             |          |          |
|  **EcsLte.BenchmarkTest.EcsContextTests** |        **EcsContext_Transfer** |          **TransferEntities_Destroy** |     **Normal_Bx4** |                   **?** |   **165.08 ms** | **0.878 ms** | **0.733 ms** |
|  EcsLte.BenchmarkTest.EcsContextTests |        EcsContext_Transfer |          TransferEntities_Destroy |     Shared_Bx4 |                   ? |   164.82 ms | 1.116 ms | 0.989 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |        EcsContext_Transfer |          TransferEntities_Destroy |     Normal_Mx4 |                   ? |   164.00 ms | 1.040 ms | 0.868 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |        EcsContext_Transfer |          TransferEntities_Destroy |     Shared_Mx4 |                   ? |   165.19 ms | 1.154 ms | 1.023 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |        EcsContext_Transfer |          TransferEntities_Destroy | Normal_Bx2_Mx2 |                   ? |   164.09 ms | 1.372 ms | 1.146 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |        EcsContext_Transfer |          TransferEntities_Destroy | Shared_Bx2_Mx2 |                   ? |   164.85 ms | 1.337 ms | 1.186 ms |
|                                       |                            |                                   |                |                     |             |          |          |
|  **EcsLte.BenchmarkTest.EcsContextTests** |        **EcsContext_Transfer** |        **TransferEntities_NoDestroy** |     **Normal_Bx4** |                   **?** |   **107.28 ms** | **0.941 ms** | **0.880 ms** |
|  EcsLte.BenchmarkTest.EcsContextTests |        EcsContext_Transfer |        TransferEntities_NoDestroy |     Shared_Bx4 |                   ? |   109.03 ms | 0.814 ms | 0.680 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |        EcsContext_Transfer |        TransferEntities_NoDestroy |     Normal_Mx4 |                   ? |   107.41 ms | 1.335 ms | 1.114 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |        EcsContext_Transfer |        TransferEntities_NoDestroy |     Shared_Mx4 |                   ? |   108.88 ms | 0.908 ms | 0.850 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |        EcsContext_Transfer |        TransferEntities_NoDestroy | Normal_Bx2_Mx2 |                   ? |   108.29 ms | 1.166 ms | 0.974 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |        EcsContext_Transfer |        TransferEntities_NoDestroy | Shared_Bx2_Mx2 |                   ? |   108.59 ms | 1.164 ms | 1.032 ms |
|                                       |                            |                                   |                |                     |             |          |          |
|  **EcsLte.BenchmarkTest.EcsContextTests** |        **EcsContext_Transfer** |   **TransferEntityArcheType_Destroy** |     **Normal_Bx4** |                   **?** |    **40.86 ms** | **0.541 ms** | **0.506 ms** |
|  EcsLte.BenchmarkTest.EcsContextTests |        EcsContext_Transfer |   TransferEntityArcheType_Destroy |     Shared_Bx4 |                   ? |    41.03 ms | 0.673 ms | 0.630 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |        EcsContext_Transfer |   TransferEntityArcheType_Destroy |     Normal_Mx4 |                   ? |    34.97 ms | 0.697 ms | 0.774 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |        EcsContext_Transfer |   TransferEntityArcheType_Destroy |     Shared_Mx4 |                   ? |    41.23 ms | 0.503 ms | 0.471 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |        EcsContext_Transfer |   TransferEntityArcheType_Destroy | Normal_Bx2_Mx2 |                   ? |    41.43 ms | 0.503 ms | 0.471 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |        EcsContext_Transfer |   TransferEntityArcheType_Destroy | Shared_Bx2_Mx2 |                   ? |    41.39 ms | 0.647 ms | 0.606 ms |
|                                       |                            |                                   |                |                     |             |          |          |
|  **EcsLte.BenchmarkTest.EcsContextTests** |        **EcsContext_Transfer** | **TransferEntityArcheType_NoDestroy** |     **Normal_Bx4** |                   **?** |    **27.60 ms** | **0.530 ms** | **0.567 ms** |
|  EcsLte.BenchmarkTest.EcsContextTests |        EcsContext_Transfer | TransferEntityArcheType_NoDestroy |     Shared_Bx4 |                   ? |    32.53 ms | 0.627 ms | 0.644 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |        EcsContext_Transfer | TransferEntityArcheType_NoDestroy |     Normal_Mx4 |                   ? |    31.70 ms | 0.617 ms | 0.606 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |        EcsContext_Transfer | TransferEntityArcheType_NoDestroy |     Shared_Mx4 |                   ? |    32.19 ms | 0.564 ms | 0.528 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |        EcsContext_Transfer | TransferEntityArcheType_NoDestroy | Normal_Bx2_Mx2 |                   ? |    31.84 ms | 0.599 ms | 0.588 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |        EcsContext_Transfer | TransferEntityArcheType_NoDestroy | Shared_Bx2_Mx2 |                   ? |    32.18 ms | 0.506 ms | 0.473 ms |
|                                       |                            |                                   |                |                     |             |          |          |
|  **EcsLte.BenchmarkTest.EcsContextTests** |        **EcsContext_Transfer** |       **TransferEntityQuery_Destroy** |     **Normal_Bx4** |                   **?** |    **41.55 ms** | **0.800 ms** | **0.786 ms** |
|  EcsLte.BenchmarkTest.EcsContextTests |        EcsContext_Transfer |       TransferEntityQuery_Destroy |     Shared_Bx4 |                   ? |    42.25 ms | 0.722 ms | 0.675 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |        EcsContext_Transfer |       TransferEntityQuery_Destroy |     Normal_Mx4 |                   ? |    41.24 ms | 0.674 ms | 0.630 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |        EcsContext_Transfer |       TransferEntityQuery_Destroy |     Shared_Mx4 |                   ? |    36.47 ms | 0.666 ms | 0.623 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |        EcsContext_Transfer |       TransferEntityQuery_Destroy | Normal_Bx2_Mx2 |                   ? |    41.92 ms | 0.800 ms | 0.786 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |        EcsContext_Transfer |       TransferEntityQuery_Destroy | Shared_Bx2_Mx2 |                   ? |    41.40 ms | 0.690 ms | 0.646 ms |
|                                       |                            |                                   |                |                     |             |          |          |
|  **EcsLte.BenchmarkTest.EcsContextTests** |        **EcsContext_Transfer** |     **TransferEntityQuery_NoDestroy** |     **Normal_Bx4** |                   **?** |    **31.88 ms** | **0.617 ms** | **0.606 ms** |
|  EcsLte.BenchmarkTest.EcsContextTests |        EcsContext_Transfer |     TransferEntityQuery_NoDestroy |     Shared_Bx4 |                   ? |    27.54 ms | 0.526 ms | 0.585 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |        EcsContext_Transfer |     TransferEntityQuery_NoDestroy |     Normal_Mx4 |                   ? |    32.18 ms | 0.496 ms | 0.464 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |        EcsContext_Transfer |     TransferEntityQuery_NoDestroy |     Shared_Mx4 |                   ? |    32.12 ms | 0.570 ms | 0.533 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |        EcsContext_Transfer |     TransferEntityQuery_NoDestroy | Normal_Bx2_Mx2 |                   ? |    32.80 ms | 0.651 ms | 0.639 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |        EcsContext_Transfer |     TransferEntityQuery_NoDestroy | Shared_Bx2_Mx2 |                   ? |    27.30 ms | 0.512 ms | 0.503 ms |
|                                       |                            |                                   |                |                     |             |          |          |
|  **EcsLte.BenchmarkTest.EcsContextTests** |        **EcsContext_Transfer** |            **TransferEntity_Destroy** |     **Normal_Bx4** |                   **?** |   **371.45 ms** | **1.690 ms** | **1.580 ms** |
|  EcsLte.BenchmarkTest.EcsContextTests |        EcsContext_Transfer |            TransferEntity_Destroy |     Shared_Bx4 |                   ? |   501.05 ms | 5.145 ms | 4.812 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |        EcsContext_Transfer |            TransferEntity_Destroy |     Normal_Mx4 |                   ? |   370.82 ms | 1.337 ms | 1.116 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |        EcsContext_Transfer |            TransferEntity_Destroy |     Shared_Mx4 |                   ? |   499.78 ms | 3.089 ms | 2.890 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |        EcsContext_Transfer |            TransferEntity_Destroy | Normal_Bx2_Mx2 |                   ? |   373.34 ms | 2.141 ms | 2.002 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |        EcsContext_Transfer |            TransferEntity_Destroy | Shared_Bx2_Mx2 |                   ? |   498.42 ms | 1.682 ms | 1.491 ms |
|                                       |                            |                                   |                |                     |             |          |          |
|  **EcsLte.BenchmarkTest.EcsContextTests** |        **EcsContext_Transfer** |          **TransferEntity_NoDestroy** |     **Normal_Bx4** |                   **?** |   **315.55 ms** | **1.486 ms** | **1.390 ms** |
|  EcsLte.BenchmarkTest.EcsContextTests |        EcsContext_Transfer |          TransferEntity_NoDestroy |     Shared_Bx4 |                   ? |   439.34 ms | 2.359 ms | 2.207 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |        EcsContext_Transfer |          TransferEntity_NoDestroy |     Normal_Mx4 |                   ? |   318.33 ms | 3.618 ms | 3.207 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |        EcsContext_Transfer |          TransferEntity_NoDestroy |     Shared_Mx4 |                   ? |   438.14 ms | 1.803 ms | 1.598 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |        EcsContext_Transfer |          TransferEntity_NoDestroy | Normal_Bx2_Mx2 |                   ? |   314.29 ms | 1.338 ms | 1.186 ms |
|  EcsLte.BenchmarkTest.EcsContextTests |        EcsContext_Transfer |          TransferEntity_NoDestroy | Shared_Bx2_Mx2 |                   ? |   438.21 ms | 5.008 ms | 4.440 ms |
|                                       |                            |                                   |                |                     |             |          |          |
|  **EcsLte.BenchmarkTest.EcsContextTests** | **EcsContext_UpdateComponent** |            **UpdateComponent_Entity** |     **Normal_Bx4** |                   **?** |   **663.71 ms** | **4.839 ms** | **4.526 ms** |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_UpdateComponent |            UpdateComponent_Entity |     Shared_Bx4 |                   ? | 2,431.61 ms | 4.511 ms | 3.522 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_UpdateComponent |            UpdateComponent_Entity |     Normal_Mx4 |                   ? |   658.28 ms | 0.838 ms | 0.784 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_UpdateComponent |            UpdateComponent_Entity |     Shared_Mx4 |                   ? | 2,459.70 ms | 9.329 ms | 7.790 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_UpdateComponent |            UpdateComponent_Entity | Normal_Bx2_Mx2 |                   ? |   657.72 ms | 2.970 ms | 2.778 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_UpdateComponent |            UpdateComponent_Entity | Shared_Bx2_Mx2 |                   ? | 2,424.79 ms | 7.238 ms | 6.417 ms |
|                                       |                            |                                   |                |                     |             |          |          |
|  **EcsLte.BenchmarkTest.EcsContextTests** | **EcsContext_UpdateComponent** |         **UpdateComponents_Entities** |     **Normal_Bx4** |                   **?** |   **545.81 ms** | **2.147 ms** | **1.904 ms** |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_UpdateComponent |         UpdateComponents_Entities |     Shared_Bx4 |                   ? |   705.31 ms | 2.323 ms | 2.173 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_UpdateComponent |         UpdateComponents_Entities |     Normal_Mx4 |                   ? |   546.16 ms | 1.815 ms | 1.609 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_UpdateComponent |         UpdateComponents_Entities |     Shared_Mx4 |                   ? |   707.42 ms | 2.120 ms | 1.880 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_UpdateComponent |         UpdateComponents_Entities | Normal_Bx2_Mx2 |                   ? |   540.15 ms | 2.179 ms | 2.038 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_UpdateComponent |         UpdateComponents_Entities | Shared_Bx2_Mx2 |                   ? |   708.01 ms | 1.224 ms | 1.145 ms |
|                                       |                            |                                   |                |                     |             |          |          |
|  **EcsLte.BenchmarkTest.EcsContextTests** | **EcsContext_UpdateComponent** |  **UpdateComponents_EntityArcheType** |     **Normal_Bx4** |                   **?** |    **77.94 ms** | **0.260 ms** | **0.217 ms** |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_UpdateComponent |  UpdateComponents_EntityArcheType |     Shared_Bx4 |                   ? |    48.76 ms | 0.205 ms | 0.192 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_UpdateComponent |  UpdateComponents_EntityArcheType |     Normal_Mx4 |                   ? |    77.90 ms | 0.142 ms | 0.126 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_UpdateComponent |  UpdateComponents_EntityArcheType |     Shared_Mx4 |                   ? |    48.73 ms | 0.135 ms | 0.127 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_UpdateComponent |  UpdateComponents_EntityArcheType | Normal_Bx2_Mx2 |                   ? |    77.94 ms | 0.192 ms | 0.180 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_UpdateComponent |  UpdateComponents_EntityArcheType | Shared_Bx2_Mx2 |                   ? |    48.77 ms | 0.228 ms | 0.202 ms |
|                                       |                            |                                   |                |                     |             |          |          |
|  **EcsLte.BenchmarkTest.EcsContextTests** | **EcsContext_UpdateComponent** |      **UpdateComponents_EntityQuery** |     **Normal_Bx4** |                   **?** |    **88.08 ms** | **1.324 ms** | **1.238 ms** |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_UpdateComponent |      UpdateComponents_EntityQuery |     Shared_Bx4 |                   ? |   179.60 ms | 3.418 ms | 3.510 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_UpdateComponent |      UpdateComponents_EntityQuery |     Normal_Mx4 |                   ? |    88.57 ms | 1.439 ms | 1.346 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_UpdateComponent |      UpdateComponents_EntityQuery |     Shared_Mx4 |                   ? |   176.16 ms | 1.047 ms | 0.817 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_UpdateComponent |      UpdateComponents_EntityQuery | Normal_Bx2_Mx2 |                   ? |    92.19 ms | 1.796 ms | 2.138 ms |
|  EcsLte.BenchmarkTest.EcsContextTests | EcsContext_UpdateComponent |      UpdateComponents_EntityQuery | Shared_Bx2_Mx2 |                   ? |   175.91 ms | 0.878 ms | 0.733 ms |
|                                       |                            |                                   |                |                     |             |          |          |
| **EcsLte.BenchmarkTest.EntityQueryTests** |        **EntityQuery_ForEach** |                           **ForEach** |              **?** |                **R0W0** |    **23.51 ms** | **0.458 ms** | **0.596 ms** |
| EcsLte.BenchmarkTest.EntityQueryTests |        EntityQuery_ForEach |                           ForEach |              ? |     R0W4_Normal_Bx4 | 1,141.55 ms | 3.781 ms | 3.536 ms |
| EcsLte.BenchmarkTest.EntityQueryTests |        EntityQuery_ForEach |                           ForEach |              ? |     R4W0_Normal_Bx4 |   903.01 ms | 5.182 ms | 4.847 ms |
| EcsLte.BenchmarkTest.EntityQueryTests |        EntityQuery_ForEach |                           ForEach |              ? |     R0W4_Normal_Mx4 | 1,149.50 ms | 8.138 ms | 7.612 ms |
| EcsLte.BenchmarkTest.EntityQueryTests |        EntityQuery_ForEach |                           ForEach |              ? |     R4W0_Normal_Mx4 |   905.03 ms | 5.938 ms | 5.555 ms |
| EcsLte.BenchmarkTest.EntityQueryTests |        EntityQuery_ForEach |                           ForEach |              ? |     R0W4_Shared_Bx4 | 1,896.80 ms | 4.908 ms | 3.832 ms |
| EcsLte.BenchmarkTest.EntityQueryTests |        EntityQuery_ForEach |                           ForEach |              ? |     R4W0_Shared_Bx4 |   907.32 ms | 5.838 ms | 5.461 ms |
| EcsLte.BenchmarkTest.EntityQueryTests |        EntityQuery_ForEach |                           ForEach |              ? |     R0W4_Shared_Mx4 | 1,922.62 ms | 6.806 ms | 6.366 ms |
| EcsLte.BenchmarkTest.EntityQueryTests |        EntityQuery_ForEach |                           ForEach |              ? |     R4W0_Shared_Mx4 |   907.29 ms | 7.037 ms | 6.583 ms |
| EcsLte.BenchmarkTest.EntityQueryTests |        EntityQuery_ForEach |                           ForEach |              ? | R0W4_Normal_Bx2_Mx2 | 1,140.26 ms | 6.914 ms | 6.468 ms |
| EcsLte.BenchmarkTest.EntityQueryTests |        EntityQuery_ForEach |                           ForEach |              ? | R4W0_Normal_Bx2_Mx2 |   905.14 ms | 4.573 ms | 4.278 ms |
| EcsLte.BenchmarkTest.EntityQueryTests |        EntityQuery_ForEach |                           ForEach |              ? | R0W4_Shared_Bx2_Mx2 | 1,922.11 ms | 8.179 ms | 6.830 ms |
| EcsLte.BenchmarkTest.EntityQueryTests |        EntityQuery_ForEach |                           ForEach |              ? | R4W0_Shared_Bx2_Mx2 |   907.15 ms | 5.497 ms | 4.873 ms |
