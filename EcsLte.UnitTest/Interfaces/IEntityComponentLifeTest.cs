using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcsLte.UnitTest.Interfaces
{
    internal interface IEntityComponentLifeTest
    {
        void AddComponent_Destroyed();
        void AddComponent_Duplicate_Normal();
        void AddComponent_Duplicate_Shared();
        void AddComponent_Duplicate_Unique_Different();
        void AddComponent_Duplicate_Unique_Same();
        void AddComponent_Null();
        void AddUniqueComponent_Destroyed();
        void AddUniqueComponent_Duplicate();
        void RemoveAllComponents_Destroyed();
        void RemoveAllComponents_Null();
        void RemoveComponent_Destroyed();
        void RemoveComponent_Duplicate_Normal();
        void RemoveComponent_Duplicate_Shared();
        void RemoveComponent_Duplicate_Unique();
        void RemoveComponent_Null();
        void RemoveUniqueComponent_Destroyed();
        void RemoveUniqueComponent_Duplicate();
        void RemoveUniqueComponent_None();
        void ReplaceComponent_Destroyed();
        void ReplaceComponent_Null();
        void ReplaceUniqueComponent_Destroyed();
    }
}
