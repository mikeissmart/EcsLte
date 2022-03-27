using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcsLte.UnitTest
{
    public interface IEcsContextType { }

    public class EcsContextType_Managed : IEcsContextType { }

    public class EcsContextType_Managed_ArcheType : IEcsContextType { }

    public class EcsContextType_Native : IEcsContextType { }

    public class EcsContextType_Native_ArcheType : IEcsContextType { }

    public class EcsContextType_Native_ArcheType_Continuous : IEcsContextType { }
}
