namespace EcsLte.Exceptions
{
	public class NoComponentsException : EcsLteException
	{
		public NoComponentsException()
			: base("No components in project.",
				  "Must have at least one component in project.")
		{ }
	}
}