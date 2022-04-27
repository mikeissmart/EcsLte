using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcsLte.UnitTest
{
    internal interface IEntityGetTest
    {
        void HasEntity_Destroyed();
        void HasEntity_Has();
        void HasEntity_Never();
        void GetEntities();
        void GetEntities_Destroyed();
        void GetEntities_Large();
    }
}
