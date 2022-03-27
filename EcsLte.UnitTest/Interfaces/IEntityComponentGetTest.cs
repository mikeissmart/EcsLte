using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcsLte.UnitTest.Interfaces
{
    internal interface IEntityComponentGetTest
    {
        void GetAllComponents_Destroyed();
        void GetAllComponents_None();
        void GetAllComponents_Null();
        void GetComponent_Destroyed();
        void GetComponent_Null();
        void GetUniqueComponent_Destroyed();
        void GetUniqueComponent_None();
        void GetUniqueEntity_Destroyed();
        void GetUniqueEntity_None();
        void HasComponent_Destroyed();
        void HasComponent_Null();
        void HasUniqueComponent_Destroyed();
        void HasUniqueComponent_None();
    }
}
