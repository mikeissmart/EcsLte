using EcsLte;
using EcsLte.Unity.Debugging.Scripts.Behaviours;
using EcsLte.Unity.Debugging.Scripts.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EcsLte.Unity.Debugging.Editor.Inspectors
{
    [UnityEditor.CustomEditor(typeof(EntityInspectorBehaviour))]
    public class EntityInspector : UnityEditor.Editor
    {
        private Color _color1;
        private Color _color2;
        private EcsContext _context;
        private Entity _entity;

        public override void OnInspectorGUI()
        {
            var behaviour = (EntityInspectorBehaviour)target;

            _color1 = behaviour.Color1;
            _color2 = behaviour.Color2;
            _context = behaviour.Context;
            _entity = behaviour.Entity;

            if (_entity == Entity.Null)
                return;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField($"Entity: {_entity}", EditorStyles.boldLabel);

            var bgColor = GUI.backgroundColor;
            GUI.backgroundColor = Color.red;
            /* Dont allow destroy entity in v1
            if (GUILayout.Button("Destroy Entity"))
            {
                _context.Entities.DestroyEntity(_entity);
                _context = null;
                behaviour.Entity = Entity.Null;
                return;
            }*/
            GUI.backgroundColor = bgColor;

            DrawComponents(_context.Entities.GetAllComponents(_entity));

            if (target != null)
                EditorUtility.SetDirty(target);
        }

        private void DrawComponents(IComponent[] components)
        {
            // Display Components
            EditorGUILayout.Space();
            for (var i = 0; i < components.Length; i++)
            {
                EditorLayout.BeginVerticalBox();
                DrawComponent(components[i], i % 2 == 0 ? _color1 : _color2);
                EditorGUILayout.EndVertical();
            }
        }

        private void DrawComponent(IComponent component, Color color)
        {
            var style = new GUIStyle(GUI.skin.box);

            EditorGUILayout.BeginVertical(style);
            {
                EditorGUILayout.BeginHorizontal();
                {
                    var memberInfos = component.GetType().GetPublicMemberInfos();
                    EditorGUILayout.LabelField(component.GetType().Name, EditorStyles.boldLabel);
                    /* Dont allow remove entity v1
                    if (EditorLayout.MiniButton("-"))
                    {
                        var a = component.GetType();
                        var b = ComponentConfigs.Instance.GetConfig(component.GetType());
                        _context.Entities.RemoveComponent(_entity,
                            ComponentConfigs.Instance.GetConfig(component.GetType()));
                    }*/
                }
                EditorGUILayout.EndHorizontal();

                var changed = false;
                foreach (var info in component.GetType().GetPublicMemberInfos())
                {
                    changed |= EntityDrawer.DrawObjectMember(info.Type, info.Name, info.GetValue(component), component, info.SetValue);
                }

                if (changed)
                    _context.Entities.UpdateComponents(_entity, component);
            }
            EditorLayout.EndVerticalBox();
        }
    }
}