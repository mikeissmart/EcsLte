namespace EcsLte.UnitTest.Interfaces
{
    public interface IEntityLifeTest
    {
        void CreateEntities();
        void CreateEntities_Destroy();
        void CreateEntities_Large();
        void CreateEntities_Reuse();
        void CreateEntities_Reuse_Large();
        void CreateEntity();
        void CreateEntity_Destroy();
        void CreateEntity_Large();
        void CreateEntity_Reuse();
        void CreateEntity_Reuse_Large();
        void DestroyEntities();
        void DestroyEntities_Destroy();
        void DestroyEntities_Large();
        void DestroyEntities_EntityNull();
        void DestroyEntities_Null();
        void DestroyEntity();
        void DestroyEntity_Destroy();
        void DestroyEntity_Null();
    }
}
