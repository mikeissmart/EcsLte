using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EcsLte.PerformanceTest
{
    /*internal class Misc_Unboxing : BasePerformanceTest
    {
        private IStandardComponent[] _components;

        public override void PreRun()
        {
            base.PreRun();

            _components = new IStandardComponent[TestConsts.EntityLoopCount];
            var component = new TestStandardComponent1();
            for (var i = 0; i < _components.Length; i++)
                _components[i] = component;
        }

        public override void Run()
        {
            TestStandardComponent1 component;
            for (var i = 0; i < _components.Length; i++)
                component = (TestStandardComponent1)_components[i];
        }
    }*/
}