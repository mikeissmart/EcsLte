using System;
using System.Collections;
using System.Linq;
using UnityEditor;

namespace EcsLte.Unity.Debugging.Scripts.Data.TypeDrawer
{
    public class ListTypeDrawer : ITypeDrawer
    {
        public bool HandlesType(Type type)
        {
            return type.GetInterfaces().Contains(typeof(IList));
        }

        public object DrawAndGetNewValue(Type memberType, string memberName, object value, object target)
        {
            var list = (IList)value;
            var elementType = memberType.GetGenericArguments()[0];
            if (list.Count == 0)
            {
                list = DrawAddElement(list, memberName, elementType);
            }
            else
            {
                EditorGUILayout.LabelField(memberName);
            }

            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = indent + 1;
            Func<IList> editAction = null;
            for (int i = 0; i < list.Count; i++)
            {
                var localIndex = i;
                EditorGUILayout.BeginHorizontal();
                {
                    EntityDrawer.DrawObjectMember(elementType, memberName + "[" + localIndex + "]", list[localIndex],
                        target, (newComponent, newValue) => list[localIndex] = newValue);

                    var action = DrawEditActions(list, elementType, localIndex);
                    if (action != null)
                    {
                        editAction = action;
                    }
                }
                EditorGUILayout.EndHorizontal();
            }

            if (editAction != null)
            {
                list = editAction();
            }
            EditorGUI.indentLevel = indent;

            return list;
        }

        private static Func<IList> DrawEditActions(IList list, Type elementType, int index)
        {
            if (EditorLayout.MiniButtonLeft("↑"))
            {
                if (index > 0)
                {
                    return () =>
                    {
                        var otherIndex = index - 1;
                        var other = list[otherIndex];
                        list[otherIndex] = list[index];
                        list[index] = other;
                        return list;
                    };
                }
            }

            if (EditorLayout.MiniButtonMid("↓"))
            {
                if (index < list.Count - 1)
                {
                    return () =>
                    {
                        var otherIndex = index + 1;
                        var other = list[otherIndex];
                        list[otherIndex] = list[index];
                        list[index] = other;
                        return list;
                    };
                }
            }

            if (EditorLayout.MiniButtonMid("+"))
            {
                object defaultValue;
                if (EntityDrawer.CreateDefault(elementType, out defaultValue))
                {
                    var insertAt = index + 1;
                    return () =>
                    {
                        list.Insert(insertAt, defaultValue);
                        return list;
                    };
                }
            }

            if (EditorLayout.MiniButtonRight("-"))
            {
                var removeAt = index;
                return () =>
                {
                    list.RemoveAt(removeAt);
                    return list;
                };
            }

            return null;
        }

        private IList DrawAddElement(IList list, string memberName, Type elementType)
        {
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField(memberName, "empty");
                if (EditorLayout.MiniButton("add " + elementType))
                {
                    object defaultValue;
                    if (EntityDrawer.CreateDefault(elementType, out defaultValue))
                    {
                        list.Add(defaultValue);
                    }
                }
            }
            EditorGUILayout.EndHorizontal();

            return list;
        }
    }
}