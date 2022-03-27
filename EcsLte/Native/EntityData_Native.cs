using System;
using System.Collections.Generic;
using System.Text;

namespace EcsLte.Native
{
    public struct EntityData_Native
    {
        /// <summary>
        /// [byte,TComponent],[byte,TComponent]
        /// </summary>
        public unsafe byte* Components;
        public int ComponentCount;
    }
}
