using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcsLte.UnitTest
{
    internal interface IEntityComponentLifeTest
    {
        void UpdateComponent_Destroyed();
        void UpdateComponent_Normal();
        void UpdateComponent_Normal_Large();
        void UpdateComponent_Normal_Never();
        void UpdateComponent_NormalShared();
        void UpdateComponent_NormalShared_Large();
        void UpdateComponent_NormalSharedUnique();
        void UpdateComponent_NormalUnique();
        void UpdateComponent_NormalUniqueShared();
        void UpdateComponent_Shared();
        void UpdateComponent_Shared_Large();
        void UpdateComponent_Shared_Never();
        void UpdateComponent_SharedNormal();
        void UpdateComponent_SharedNormal_Large();
        void UpdateComponent_SharedNormalUnique();
        void UpdateComponent_SharedUnique();
        void UpdateComponent_SharedUniqueNormal();
        void UpdateComponent_Unique();
        void UpdateComponent_Unique_Never();
        void UpdateComponent_UniqueNormal();
        void UpdateComponent_UniqueNormalShared();
        void UpdateComponent_UniqueShared();
        void UpdateComponent_UniqueSharedNormal();
    }
}
