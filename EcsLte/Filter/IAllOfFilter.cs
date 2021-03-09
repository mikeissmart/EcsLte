namespace EcsLte
{
	public interface IAllOfFilter : IAnyOfFilter
	{
		IAnyOfFilter AnyOf(params IFilter[] filters);

		IAnyOfFilter AnyOf<TComponent>()
			where TComponent : IComponent;

		IAnyOfFilter AnyOf<TComponent1, TComponent2>()
			where TComponent1 : IComponent
			where TComponent2 : IComponent;

		IAnyOfFilter AnyOf<TComponent1, TComponent2, TComponent3>()
			where TComponent1 : IComponent
			where TComponent2 : IComponent
			where TComponent3 : IComponent;

		IAnyOfFilter AnyOf<TComponent1, TComponent2, TComponent3, TComponent4>()
			where TComponent1 : IComponent
			where TComponent2 : IComponent
			where TComponent3 : IComponent
			where TComponent4 : IComponent;

		IAnyOfFilter AnyOf<TComponent1, TComponent2, TComponent3, TComponent4, TComponent5>()
			where TComponent1 : IComponent
			where TComponent2 : IComponent
			where TComponent3 : IComponent
			where TComponent4 : IComponent
			where TComponent5 : IComponent;
	}
}