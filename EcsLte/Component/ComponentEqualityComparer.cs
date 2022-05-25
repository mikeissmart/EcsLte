using System.Collections.Generic;

namespace EcsLte
{
    internal class ComponentEqualityComparer<TComponent> : IEqualityComparer<TComponent>
        where TComponent : IComponent
    {
        internal static ComponentEqualityComparer<TComponent> Comparer => new ComponentEqualityComparer<TComponent>();

        public bool Equals(TComponent x, TComponent y) => x.Equals(y);
        public int GetHashCode(TComponent obj) => obj.GetHashCode();
    }
}
