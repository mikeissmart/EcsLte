using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcsLte.BenchmarkTest
{
    public enum ComponentArrangement
    {
        Normal_x1,
        Normal_x2,
        Shared_x1,
        Shared_x2,
        Normal_x1_Shared_x1,
        Normal_x1_Shared_x2,
        Normal_x2_Shared_x1,
        Normal_x2_Shared_x2,
    }
}
