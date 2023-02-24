using System;

namespace EcsLte.Unity.Debugging.Scripts.Data.TypeDrawer
{
    public interface ITypeDrawer
    {
        bool HandlesType(Type type);

        object DrawAndGetNewValue(Type memberType, string memberName, object value, object target);
    }
}