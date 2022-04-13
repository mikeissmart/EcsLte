namespace EcsLte.BencharkTest.Data
{
    /*[MemoryDiagnoser]
    [GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
    public class Component_ArcheTypeTest_AppendComponent
    {
        private Component_ArcheType_Managed[] _archeType_Managed;
        private Component_ArcheType_Native[] _archeType_Native;
        private ComponentConfig[] _componentConfigs;

        public IEnumerable<int> ComponentCountSource { get => Enumerable.Range(1, 100).Where(x => x % 10 == 0); }

        [ParamsSource(nameof(ComponentCountSource))]
        public int ConfigCount { get; set; }

        [IterationSetup]
        public void IterationSetup()
        {
            _archeType_Managed = new Component_ArcheType_Managed[ConfigCount];
            _archeType_Native = new Component_ArcheType_Native[ConfigCount];
            _componentConfigs = new ComponentConfig[ConfigCount];
            for (int i = 0; i < ConfigCount; i++)
            {
                _componentConfigs[i] = new ComponentConfig
                {
                    ComponentIndex = i,
                };
            }
        }

        [IterationCleanup]
        public void IterationCleanup()
        {
            for (int i = 0; i < ConfigCount; i++)
                _archeType_Native[i].Dispose();
        }

        [Benchmark]
        public void Managed()
        {
            for (int i = 1; i < ConfigCount; i++)
            {
                _archeType_Managed[i] = Component_ArcheType_Managed.AppendComponent(_archeType_Managed[i - 1], _componentConfigs[i]);
            }
        }

        [Benchmark]
        public void Native()
        {
            for (int i = 1; i < ConfigCount; i++)
            {
                _archeType_Native[i] = Component_ArcheType_Native.AppendComponent(_archeType_Native[i - 1], _componentConfigs[i]);
            }
        }
    }
    
    [MemoryDiagnoser]
    [GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
    public class Component_ArcheTypeTest_RemoveComponent
    {
        private Component_ArcheType_Managed[] _archeType_Managed;
        private Component_ArcheType_Native[] _archeType_Native;
        private Component_ArcheType_Managed[] _archeType_Managed_Result;
        private Component_ArcheType_Native[] _archeType_Native_Result;
        private ComponentConfig[] _componentConfigs;

        public IEnumerable<int> ComponentCountSource { get => Enumerable.Range(1, 100).Where(x => x % 10 == 0); }

        [ParamsSource(nameof(ComponentCountSource))]
        public int ConfigCount { get; set; }

        [IterationSetup]
        public void IterationSetup()
        {
            _archeType_Managed = new Component_ArcheType_Managed[ConfigCount];
            _archeType_Native = new Component_ArcheType_Native[ConfigCount];
            _archeType_Managed_Result = new Component_ArcheType_Managed[ConfigCount];
            _archeType_Native_Result = new Component_ArcheType_Native[ConfigCount];
            _componentConfigs = new ComponentConfig[ConfigCount];
            for (int i = 0; i < ConfigCount; i++)
            {
                _componentConfigs[i] = new ComponentConfig
                {
                    ComponentIndex = i,
                };
            }
            for (int i = 1; i < ConfigCount; i++)
            {
                _archeType_Managed[i] = Component_ArcheType_Managed.AppendComponent(_archeType_Managed[i - 1], _componentConfigs[i]);
                _archeType_Native[i] = Component_ArcheType_Native.AppendComponent(_archeType_Native[i - 1], _componentConfigs[i]);
            }
        }

        [IterationCleanup]
        public void IterationCleanup()
        {
            for (int i = 0; i < ConfigCount; i++)
            {
                _archeType_Native[i].Dispose();
                _archeType_Native_Result[i].Dispose();
            }
        }

        [Benchmark]
        public void Last_Managed()
        {
            for (int i = 0; i < ConfigCount - 1; i++)
            {
                _archeType_Managed_Result[i] = Component_ArcheType_Managed.RemoveComponent(_archeType_Managed[i + 1], _componentConfigs[i + 1]);
            }
        }

        [Benchmark]
        public void Last_Native()
        {
            for (int i = 0; i < ConfigCount - 1; i++)
            {
                _archeType_Native_Result[i] = Component_ArcheType_Native.RemoveComponent(_archeType_Native[i + 1], _componentConfigs[i + 1]);
            }
        }

        [Benchmark]
        public void First_Managed()
        {
            for (int i = 0; i < ConfigCount - 1; i++)
            {
                _archeType_Managed_Result[i] = Component_ArcheType_Managed.RemoveComponent(_archeType_Managed[i + 1], _componentConfigs[1]);
            }
        }

        [Benchmark]
        public void First_Native()
        {
            for (int i = 0; i < ConfigCount - 1; i++)
            {
                _archeType_Native_Result[i] = Component_ArcheType_Native.RemoveComponent(_archeType_Native[i + 1], _componentConfigs[1]);
            }
        }
    }

    [MemoryDiagnoser]
    [GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
    public class Component_ArcheTypeTest_HasComponent
    {
        private Component_ArcheType_Managed[] _archeType_Managed;
        private Component_ArcheType_Native[] _archeType_Native;
        private ComponentConfig[] _componentConfigs;

        public IEnumerable<int> ComponentCountSource { get => Enumerable.Range(1, 100).Where(x => x % 10 == 0); }

        [ParamsSource(nameof(ComponentCountSource))]
        public int ConfigCount { get; set; }

        [IterationSetup]
        public void IterationSetup()
        {
            _archeType_Managed = new Component_ArcheType_Managed[ConfigCount];
            _archeType_Native = new Component_ArcheType_Native[ConfigCount];
            _componentConfigs = new ComponentConfig[ConfigCount];
            for (int i = 0; i < ConfigCount; i++)
            {
                _componentConfigs[i] = new ComponentConfig
                {
                    ComponentIndex = i,
                };
            }
            for (int i = 1; i < ConfigCount; i++)
            {
                _archeType_Managed[i] = Component_ArcheType_Managed.AppendComponent(_archeType_Managed[i - 1], _componentConfigs[i]);
                _archeType_Native[i] = Component_ArcheType_Native.AppendComponent(_archeType_Native[i - 1], _componentConfigs[i]);
            }
        }

        [Benchmark]
        public void Managed()
        {
            var has = false;
            for (int i = 1; i < ConfigCount; i++)
            {
                has = _archeType_Managed[i].HasComponentConfig(_componentConfigs[i]);
            }
        }

        [Benchmark]
        public void Native()
        {
            var has = false;
            for (int i = 1; i < ConfigCount; i++)
            {
                has = _archeType_Native[i].HasComponentConfig(_componentConfigs[i]);
            }
        }
    }

    [MemoryDiagnoser]
    [GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
    public class Component_ArcheTypeTest_GetHashCode
    {
        private Component_ArcheType_Managed[] _archeType_Managed;
        private Component_ArcheType_Native[] _archeType_Native;
        private ComponentConfig[] _componentConfigs;

        public IEnumerable<int> ComponentCountSource { get => Enumerable.Range(1, 100).Where(x => x % 10 == 0); }

        [ParamsSource(nameof(ComponentCountSource))]
        public int ConfigCount { get; set; }

        [IterationSetup]
        public void IterationSetup()
        {
            _archeType_Managed = new Component_ArcheType_Managed[ConfigCount];
            _archeType_Native = new Component_ArcheType_Native[ConfigCount];
            _componentConfigs = new ComponentConfig[ConfigCount];
            for (int i = 0; i < ConfigCount; i++)
            {
                _componentConfigs[i] = new ComponentConfig
                {
                    ComponentIndex = i,
                };
            }
            for (int i = 1; i < ConfigCount; i++)
            {
                _archeType_Managed[i] = Component_ArcheType_Managed.AppendComponent(_archeType_Managed[i - 1], _componentConfigs[i]);
                _archeType_Native[i] = Component_ArcheType_Native.AppendComponent(_archeType_Native[i - 1], _componentConfigs[i]);
            }
        }

        [Benchmark]
        public void Managed()
        {
            var hashCode = 0;
            for (int i = 1; i < ConfigCount; i++)
            {
                hashCode = _archeType_Managed[i].GetHashCode();
            }
        }

        [Benchmark]
        public void Native()
        {
            var hashCode = 0;
            for (int i = 1; i < ConfigCount; i++)
            {
                hashCode = _archeType_Native[i].GetHashCode();
            }
        }
    }*/
}
