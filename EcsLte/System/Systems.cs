using System;
using System.Collections.Generic;
using System.Linq;

namespace EcsLte
{
    internal class Systems
    {
        private static Systems _instance;

        private Systems()
        {
            Initialize();
        }

        public static Systems Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Systems();
                return _instance;
            }
        }

        public Type[] AllSystemTypes { get; private set; }
        public Type[] AutoAddSystemTypes { get; private set; }

        public bool IsAutoAdd(Type systemType)
        {
            return AutoAddSystemTypes.Any(x => x == systemType);
        }

        private void Initialize()
        {
            var systemBaseType = typeof(SystemBase);
            var systemTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(x =>
                    x.IsPublic &&
                    !x.IsAbstract &&
                    systemBaseType.IsAssignableFrom(x));

            var autoAddSystemTypes = new List<Type>();
            foreach (var type in systemTypes.OrderBy(x => x.FullName.ToString()))
            {
                if (type.GetCustomAttributes(typeof(SystemAutoAddAttribute), true).Length > 0)
                    autoAddSystemTypes.Add(type);
            }

            AllSystemTypes = systemTypes
                .OrderBy(x => x.Name)
                .ToArray();
            AutoAddSystemTypes = autoAddSystemTypes.ToArray();
        }
    }
}