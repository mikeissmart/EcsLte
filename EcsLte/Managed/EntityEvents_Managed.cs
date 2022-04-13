namespace EcsLte.Managed
{
    public delegate void EntityComponentChangeEvent_Managed(
        Entity entity, ComponentConfig config);
    public delegate void EntityComponentReplaceEvent_Managed(
        Entity entity, ComponentConfig config);
}
