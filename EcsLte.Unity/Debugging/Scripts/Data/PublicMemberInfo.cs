using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace EcsLte.Unity.Debugging.Scripts.Data
{
    public static class PublicMemberInfoExtension
    {
        public static List<PublicMemberInfo> GetPublicMemberInfos(this Type type)
        {
            var memberInfos = new List<PublicMemberInfo>();

            foreach (var field in type.GetFields(BindingFlags.Instance | BindingFlags.Public))
                memberInfos.Add(new PublicMemberInfo(field));

            foreach (var property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                if (property.CanRead && property.CanWrite && property.GetIndexParameters().Length == 0)
                    memberInfos.Add(new PublicMemberInfo(property));
            }

            return memberInfos;
        }
    }

    public class PublicMemberInfo
    {
        private FieldInfo _fieldInfo;
        private PropertyInfo _propertyInfo;

        public string Name { get; private set; }
        public Type Type { get; private set; }
        public AttributeInfo[] Attributes { get; private set; }

        public PublicMemberInfo(FieldInfo info)
        {
            _fieldInfo = info;
            Type = _fieldInfo.FieldType;
            Name = _fieldInfo.Name;
            GetAttributes(info.GetCustomAttributes(inherit: false));
        }

        public PublicMemberInfo(PropertyInfo info)
        {
            _propertyInfo = info;
            Type = _propertyInfo.PropertyType;
            Name = _propertyInfo.Name;
            GetAttributes(info.GetCustomAttributes(inherit: false));
        }

        public object GetValue(object obj)
        {
            return _fieldInfo != null
                ? _fieldInfo.GetValue(obj)
                : _propertyInfo.GetValue(obj);
        }

        public void SetValue(object obj, object value)
        {
            if (_fieldInfo != null)
                _fieldInfo.SetValue(obj, value);
            else
                _propertyInfo.SetValue(obj, value);
        }

        private void GetAttributes(object[] attributes)
        {
            Attributes = new AttributeInfo[attributes.Length];
            for (int i = 0; i < attributes.Length; i++)
            {
                object obj = attributes[i];
                Attributes[i] = new AttributeInfo(obj, obj.GetType().GetPublicMemberInfos());
            }
        }
    }

    public class AttributeInfo
    {
        public object Attribute { get; private set; }

        public List<PublicMemberInfo> MemberInfos { get; private set; }

        public AttributeInfo(object attribute, List<PublicMemberInfo> memberInfos)
        {
            Attribute = attribute;
            MemberInfos = memberInfos;
        }
    }
}