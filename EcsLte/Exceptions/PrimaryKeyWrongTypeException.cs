using System;

namespace EcsLte.Exceptions
{
    public class PrimaryKeyWrongTypeException : EcsLteException
    {
        public PrimaryKeyWrongTypeException(Type correctType, Type invalidType)
            : base($"PrimaryKey<{nameof(correctType)}> tried to use the wrong component type of '{nameof(invalidType)}'.",
                "Use the same type when using primary key.")
        {
        }
    }
}