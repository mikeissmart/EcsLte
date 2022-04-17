namespace EcsLte.HybridArcheType
{
    public class Test
    {
        /* Dont know if i want to implemnt this, my thinking
        TODOs
            figure out when/where ArcheTypeData new/updated entities will be cut-copied to stale
            try to make the only the only thing storing componentData unsafe

        Components
            seperate standard,shared,unique components
        EntityBlueprint
            components or component tyeps can only be added/removed with EntityBlueprint
            shared components require new blueprint
            blueprints can be added to other blueprints
            no need to look through archetype dictionary everytime an component is added
            can add, replace, remove components          
            when components change return new instance of EntityBlueprint
            singular cache blueprintData like it is now
            blueprint will cache ArcheTypeData after first Entity is created with it
            cannot be created during EntityQueries
        Entities can only be created with EntityBlueprint (probably shouldnt allow no component entities)
            create entity/entities require a blueprint
            entities can change blueprints
                when changing blueprints
                    same blueprint component types will be kept and not updated
                    same blueprint component data will be updated
                    different blueprint component types will be removed/added
                    different blueprint component data will be removed/added
        ArcheTypeData wont use dataChunks
            will have single byte array that will only grow by x(maybe 1000?) number of slots
                layout [Entity, Entity], [Component1, Component2], [Component1, Component2]
                prevent array getting to big with x2 resize
                smaller memory footprint
            when tracking is implmented
                3 different arrays for recently added, updated, stale
                maybe before running EntityCommands 
                    or seperate function in EcsContext?
                during EntityCommand when entity components are updated all of its data is cut/copied to updated buffer
                when coping from stale to updated fill empty spot with last entity in stale array
        EntityQuery
            can be filtered by component types
            can be grouped by shared components
            singular cache queryData like it is now
            can be run immediately or scheduled for later to EntityCommand
            can take upto 8 different in/out components
            out components will flag entity as updated reguardless if it was actually updated
            new, updated, stale entites can be queued seperatly or togeather in any combo
        EntityCommand
            seperate commands in structual change an non structual changes
                structual - single thread
                    create/destroy entities
                    changing blueprint
                    updating sharedComponents
                non structual - multi thread
                    component update (not sharedComponents)
            process directly added commands before EntityQuery commands
            combine simular commands
                create entities for same blueprint
                changing from same blueprint to same blueprint
        */
    }
}
