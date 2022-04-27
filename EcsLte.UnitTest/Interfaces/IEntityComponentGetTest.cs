using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcsLte.UnitTest
{
    internal interface IEntityComponentGetTest
    {
        void GetAllComponents_Destroyed();
        void GetAllComponents_Normal();
        void GetAllComponents_Normal_Large();
        void GetAllComponents_NormalShared();
        void GetAllComponents_NormalShared_Large();
        void GetAllComponents_NormalSharedUnique();
        void GetAllComponents_NormalUnique();
        void GetAllComponents_NormalUniqueShared();
        void GetAllComponents_Shared();
        void GetAllComponents_Shared_Large();
        void GetAllComponents_SharedNormal();
        void GetAllComponents_SharedNormal_Large();
        void GetAllComponents_SharedNormalUnique();
        void GetAllComponents_SharedUnique();
        void GetAllComponents_SharedUniqueNormal();
        void GetAllComponents_Unique();
        void GetAllComponents_UniqueNormal();
        void GetAllComponents_UniqueNormalShared();
        void GetAllComponents_UniqueShared();
        void GetAllComponents_UniqueSharedNormal();
        void GetComponent_Destroyed();
        void GetComponent_Normal();
        void GetComponent_Normal_Large();
        void GetComponent_Normal_Never();
        void GetComponent_NormalShared();
        void GetComponent_NormalShared_Large();
        void GetComponent_NormalSharedUnique();
        void GetComponent_NormalUnique();
        void GetComponent_NormalUniqueShared();
        void GetComponent_Shared();
        void GetComponent_Shared_Large();
        void GetComponent_Shared_Never();
        void GetComponent_SharedNormal();
        void GetComponent_SharedNormal_Large();
        void GetComponent_SharedNormalUnique();
        void GetComponent_SharedUnique();
        void GetComponent_SharedUniqueNormal();
        void GetComponent_Unique();
        void GetComponent_Unique_Never();
        void GetComponent_UniqueNormal();
        void GetComponent_UniqueNormalShared();
        void GetComponent_UniqueShared();
        void GetComponent_UniqueSharedNormal();
        void HasComponent_Destroyed();
        void HasComponent_Normal_Has();
        void HasComponent_Normal_Never();
        void HasComponent_Shared_Has();
        void HasComponent_Shared_Never();
        void HasComponent_Unique_Has();
        void HasComponent_Unique_Never();
    }
}
