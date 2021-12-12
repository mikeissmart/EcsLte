namespace EcsLte.UnitTest.InterfaceTests
{
	public interface IComponentLifeTest
	{
		void AddUniqueComponent();
		void ReplaceUniqueComponent();
		void RemoveUniqueComponent();
		void AddComponent();
		void ReplaceComponent();
		void RemoveComponent();
		void RemoveAllComponents();
	}
}