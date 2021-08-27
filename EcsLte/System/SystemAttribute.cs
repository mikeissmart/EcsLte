using System;
using EcsLte.Exceptions;

namespace EcsLte
{
    public class AfterSystemAttribute : BaseSystemAttribute
    {
        public AfterSystemAttribute(params Type[] systems) :
            base(systems)
        { }
    }

    public class BeforeSystemAttribute : BaseSystemAttribute
    {
        public BeforeSystemAttribute(params Type[] systems) :
            base(systems)
        { }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = true, Inherited = true)]
    public abstract class BaseSystemAttribute : Attribute
    {
        public Type[] Systems { get; private set; }

        public BaseSystemAttribute(params Type[] systems)
        {
            foreach (var system in systems)
            {
                if (system.GetInterface(nameof(ISystem)) == null)
                    throw new SystemAttributeException(system);
            }
            Systems = systems;
        }
    }
}