using EcsLte.Unity.Debugging.Scripts.Data.TypeDrawer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace EcsLte.Unity.Debugging.Scripts.Data
{
    public static class EntityDrawer
    {
        private static readonly ITypeDrawer[] _typeDrawers;

        static EntityDrawer()
        {
            var drawerType = typeof(ITypeDrawer);
            _typeDrawers = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(x =>
                    x.IsPublic &&
                    !x.IsAbstract &&
                    drawerType.IsAssignableFrom(x))
                .Select(x => (ITypeDrawer)Activator.CreateInstance(x))
                .ToArray();
        }

        public static bool CreateDefault(Type type, out object defaultValue)
        {
            try
            {
                defaultValue = Activator.CreateInstance(type);
                return true;
            }
            catch (Exception ex)
            {
                EditorUtility.DisplayDialog(ex.Message, "Ok", "Cancel");
            }

            defaultValue = null;
            return false;
        }

        public static bool DrawObjectMember(Type memberType, string memberName, object value, object target, Action<object, object> setValue)
        {
            if (value == null)
            {
                EditorGUI.BeginChangeCheck();
                {
                    var isUnityObject = memberType == typeof(UnityEngine.Object) || memberType.IsSubclassOf(typeof(UnityEngine.Object));
                    EditorGUILayout.BeginHorizontal();
                    {
                        if (isUnityObject)
                        {
                            setValue(target, EditorGUILayout.ObjectField(memberName, (UnityEngine.Object)value, memberType, true));
                        }
                        else
                        {
                            EditorGUILayout.LabelField(memberName, "null");
                        }

                        if (EditorLayout.MiniButton("new " + memberType))
                        {
                            object defaultValue;
                            if (CreateDefault(memberType, out defaultValue))
                            {
                                setValue(target, defaultValue);
                            }
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }

                return EditorGUI.EndChangeCheck();
            }

            if (!memberType.IsValueType)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.BeginVertical();
            }

            EditorGUI.BeginChangeCheck();
            {
                var typeDrawer = GetTypeDrawer(memberType);
                if (typeDrawer != null)
                {
                    var newValue = typeDrawer.DrawAndGetNewValue(memberType, memberName, value, target);
                    setValue(target, newValue);
                }
                else
                {
                    var targetType = target.GetType();

                    EditorGUILayout.LabelField(memberName, value.ToString());

                    var indent = EditorGUI.indentLevel;
                    EditorGUI.indentLevel += 1;

                    EditorGUILayout.BeginVertical();
                    {
                        foreach (var info in memberType.GetPublicMemberInfos())
                        {
                            var mValue = info.GetValue(value);
                            var mType = mValue == null ? info.Type : mValue.GetType();
                            DrawObjectMember(mType, info.Name, mValue, value, info.SetValue);
                            if (memberType.IsValueType)
                            {
                                setValue(target, value);
                            }
                        }
                    }
                    EditorGUILayout.EndVertical();

                    EditorGUI.indentLevel = indent;
                }

                if (!memberType.IsValueType)
                {
                    EditorGUILayout.EndVertical();
                    if (EditorLayout.MiniButton("×"))
                    {
                        setValue(target, null);
                    }

                    EditorGUILayout.EndHorizontal();
                }
            }

            return EditorGUI.EndChangeCheck();
        }

        private static ITypeDrawer GetTypeDrawer(Type type)
        {
            foreach (var drawer in _typeDrawers)
            {
                if (drawer.HandlesType(type))
                {
                    return drawer;
                }
            }

            return null;
        }
    }
}