using System;
using System.Linq;
using System.Reflection;

namespace UniDi
{
    public static class ReflectionExtentions
    {
        public static bool HasInjectAttribute(this MemberInfo member)
        {
            return member.GetCustomAttributes(typeof(InjectAttribute), false).Length > 0;
        }

        public static bool IsComponent(this Type type)
        {
            return typeof(UnityEngine.Component).IsAssignableFrom(type);
        }

        public static bool IsMonoBehaviour(this Type type)
        {
            return typeof(UnityEngine.MonoBehaviour).IsAssignableFrom(type);
        }

        public static bool IsUnityObject(this Type type)
        {
            return typeof(UnityEngine.Object).IsAssignableFrom(type);
        }

        public static InjectAttribute GetInjectAttribute(this MemberInfo member)
        {
            return member.GetCustomAttributes(typeof(InjectAttribute), false).FirstOrDefault() as InjectAttribute;
        }
    }
}