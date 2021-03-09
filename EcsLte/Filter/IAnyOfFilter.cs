namespace EcsLte
{
	public interface IAnyOfFilter : INoneOfFilter
	{
		INoneOfFilter NoneOf(params IFilter[] filters);

		INoneOfFilter NoneOf<TComponent>()
			where TComponent : IComponent;

		INoneOfFilter NoneOf<TComponent1, TComponent2>()
			where TComponent1 : IComponent
			where TComponent2 : IComponent;

		INoneOfFilter NoneOf<TComponent1, TComponent2, TComponent3>()
			where TComponent1 : IComponent
			where TComponent2 : IComponent
			where TComponent3 : IComponent;

		INoneOfFilter NoneOf<TComponent1, TComponent2, TComponent3, TComponent4>()
			where TComponent1 : IComponent
			where TComponent2 : IComponent
			where TComponent3 : IComponent
			where TComponent4 : IComponent;

		INoneOfFilter NoneOf<TComponent1, TComponent2, TComponent3, TComponent4, TComponent5>()
			where TComponent1 : IComponent
			where TComponent2 : IComponent
			where TComponent3 : IComponent
			where TComponent4 : IComponent
			where TComponent5 : IComponent;
	}
}