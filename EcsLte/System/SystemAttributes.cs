using EcsLte.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EcsLte
{
    public class AfterSystemAttribute : SystemBaseAttribute
    {
        public AfterSystemAttribute(params Type[] systems)
            : base(systems)
        {
        }
    }

    public class BeforeSystemAttribute : SystemBaseAttribute
    {
        public BeforeSystemAttribute(params Type[] systems)
            : base(systems)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public abstract class SystemBaseAttribute : Attribute
    {
        public Type[] Systems { get; private set; }

        protected SystemBaseAttribute(params Type[] systems)
        {
            var hash = new HashSet<Type>();
            foreach (var system in systems)
            {
                if (!system.IsSubclassOf(typeof(SystemBase)))
                    throw new SystemAttributeException(system);
                hash.Add(system);
            }
            Systems = hash.ToArray();
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class DontAutoAddSystem : Attribute
    {

    }
}
