using System;
using System.Reflection;

namespace UniDi
{
    public static class Extentions
    {
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
        public static bool IsProperty(this MemberInfo member)
        {
            return member is PropertyInfo;
        }

        public static bool IsMethod(this MemberInfo member)
        {
            return member is MethodInfo;
        }

        /// <summary>
        /// Gets all members of type marked with [Inject] or [Inject("somename")]
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static MemberInfo[] GetMembersToInject(this Type type)
        {
            return type.FindMembers(MemberTypes.All,
                BindingFlags.SetProperty | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
                DelegateToSearchCriteria, typeof(InjectAttribute));
        }

        private static bool DelegateToSearchCriteria(MemberInfo objMemberInfo, object objSearch)
        {
            return objMemberInfo.IsDefined((Type)objSearch, true);
        }
    }
}