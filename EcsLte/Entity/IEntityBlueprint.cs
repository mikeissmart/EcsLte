using System;
using System.Collections.Generic;
using System.Text;

namespace EcsLte
{
    public interface IEntityBlueprint
    {
        bool HasComponent<TComponent>() where TComponent : unmanaged, IComponent;
        TComponent GetComponent<TComponent>() where TComponent : unmanaged, IComponent;
        void AddComponent<TComponent>(TComponent component) where TComponent : unmanaged, IComponent;
        void ReplaceComponent<TComponent>(TComponent component) where TComponent : unmanaged, IComponent;
        void RemoveComponent<TComponent>() where TComponent : unmanaged, IComponent;
    }
}
