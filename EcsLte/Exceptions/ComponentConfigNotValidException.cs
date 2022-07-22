using System;
using System.Collections.Generic;
using System.Text;

namespace EcsLte.Exceptions
{
    public class ComponentConfigNotValidException : EcsLteException
    {
        public ComponentConfigNotValidException()
            : base($"ComponentConfig is not valid. Only use configs from EcsLte.ComponentConfig<TComponent> or EcsLte.ComponentConfigs.")
        {
        }
    }
}
