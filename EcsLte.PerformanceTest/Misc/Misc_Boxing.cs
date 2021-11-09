using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EcsLte.PerformanceTest
{
    /*internal class Misc_Boxing : BasePerformanceTest
    {
        private IStandardComponent[] _components;
        private TestStandardComponent1 _component;

        public override void PreRun()
        {
            base.PreRun();

            _component = new TestStandardComponent1();
            _components = new IStandardComponent[TestConsts.EntityLoopCount];
        }

        public override void Run()
        {
            for (var i = 0; i < _components.Length; i++)
                _components[i] = _component;
        }
    }*/
}