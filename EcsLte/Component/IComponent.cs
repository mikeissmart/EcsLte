using System;
using System.Collections.Generic;
using System.Text;

namespace EcsLte
{
    public interface IComponent
    {
    }

    public interface IRecordableComponent : IComponent
    {
    }

    public interface ISharedComponent : IComponent
    {
    }

    public interface IUniqueComponent : IComponent
    {
    }
}
