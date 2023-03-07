using EcsLte.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EcsLte
{
    public class EntityTrackerManager
    {
        public EcsContext Context { get; private set; }

        internal EntityTrackerManager(EcsContext context) => Context = context;

        public EntityTracker SetTrackingComponent<TComponent>(bool tracking)
            where TComponent : IComponent
            => SetTrackingComponent(ComponentConfig<TComponent>.Config, tracking);

        public EntityTracker SetTrackingComponent(ComponentConfig config, bool tracking)
            => new EntityTracker(Context)
                .SetTrackingComponent(config, tracking);

        public EntityTracker SetAllTrackingComponents(bool tracking)
            => new EntityTracker(Context)
                .SetAllTrackingComponents(tracking);

        public EntityTracker SetTrackingMode(EntityTrackerMode mode)
            => new EntityTracker(Context)
                .SetTrackingMode(mode);

        public EntityTracker SetChangeVersion(ChangeVersion changeVersion)
            => new EntityTracker(Context)
                .SetChangeVersion(changeVersion);

        internal void InternalDestroy()
        {

        }
    }
}
