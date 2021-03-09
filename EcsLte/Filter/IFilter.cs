namespace EcsLte
{
	public interface IFilter
	{
		int[] AllOfIndices { get; }
		int[] AnyOfIndices { get; }
		int[] NoneOfIndices { get; }

		bool Filtered(Entity entity);
	}
}