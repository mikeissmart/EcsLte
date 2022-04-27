using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcsLte.UnitTest
{
    internal interface IEntityComponentUniqueGetTest
    {
        void Unique_GetUniqueComponent_Destroyed();
        void Unique_GetUniqueComponent();
        void Unique_GetUniqueEntity_Destroyed();
        void Unique_GetUniqueEntity();
        void Unique_HasUniqueComponent_Destroyed();
        void Unique_HasUniqueComponent();
    }
}
