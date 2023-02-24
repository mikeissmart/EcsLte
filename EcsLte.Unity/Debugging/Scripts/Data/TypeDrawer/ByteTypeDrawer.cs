using System;
using UnityEditor;

namespace EcsLte.Unity.Debugging.Scripts.Data.TypeDrawer
{
    public class ByteTypeDrawer : ITypeDrawer
    {
        public bool HandlesType(Type type)
        {
            return type == typeof(byte);
        }

        public object DrawAndGetNewValue(Type memberType, string memberName, object value, object target)
        {
            return (byte)EditorGUILayout.IntField(memberName, (byte)value);
        }
    }
}