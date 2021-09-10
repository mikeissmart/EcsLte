using System;
using System.Collections.Generic;
using System.Linq;
using EcsLte.Exceptions;

namespace EcsLte
{
    public class AfterSystemAttribute : SystemBaseAttribute
    {
        public AfterSystemAttribute(params Type[] systems) :
            base(systems)
        {
        }
    }

    public class BeforeSystemAttribute : SystemBaseAttribute
    {
        public BeforeSystemAttribute(params Type[] systems) :
            base(systems)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public abstract class SystemBaseAttribute : Attribute
    {
        public SystemBaseAttribute(params Type[] systems)
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

        public Type[] Systems { get; }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class SystemAutoAddAttribute : Attribute
    {

    }
}