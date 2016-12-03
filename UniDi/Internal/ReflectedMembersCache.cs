using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

namespace UniDi
{
    public class ReflectedMembersCache
    {
        public Dictionary<Type, ReflectedMembersEntry> ReflectedTypes { get; private set; }

        private ReflectedMembersCache _instance;

        public ReflectedMembersEntry this[Type type]
        {
            get
            {
                ReflectedMembersEntry entry;
                if (ReflectedTypes.TryGetValue(type, out entry))
                    return entry;
                else
                    throw new ArgumentException("No such type found");
            }
        }

        public IEnumerable<Type> Types { get { return ReflectedTypes.Keys; }}

        public ReflectedMembersCache()
        {
            ReflectedTypes = new Dictionary<Type, ReflectedMembersEntry>();
        }

        public void AddRange(IEnumerable<Type> types)
        {
            foreach (var type in types)
            {
                Add(type);
            }
        }

        public void Add(Type type)
        {
            if(!Contains(type))
                ReflectedTypes.Add(
                    type,
                    new ReflectedMembersEntry(type));
        }

        public void Remove(Type type)
        {
            if (Contains(type))
                ReflectedTypes.Remove(type);
        }

        public bool Contains(Type type)
        {
            return ReflectedTypes.ContainsKey(type);
        }
    }

    public class ReflectedMembersEntry
    {
        public Type Type { get; private set; }

        public List<PropertyInfo> Properties { get; private set; }
        public List<FieldInfo> Fields { get; private set; }
        public List<MethodInfo> Methods { get; private set; }
        public List<ConstructorInfo> Constructors { get; private set; }

        public bool AnyPropertiesToInject { get { return Properties != null && Properties.Count > 0; } }
        public bool AnyFieldsToInject { get { return Fields != null && Fields.Count > 0; } }
        public bool AnyMethodsToInject { get { return Methods != null && Methods.Count > 0; } }
        public bool AnyConstructorsToInject { get { return Constructors != null && Constructors.Count > 0; } }

        public ReflectedMembersEntry(Type type)
        {
            Type = type;

            var members = GetMembersToInject();

            Properties = members.Where(m => m is PropertyInfo).Cast<PropertyInfo>().ToList();
            Fields = members.Where(m => m is FieldInfo).Cast<FieldInfo>().ToList();
            Methods = members.Where(m => m is MethodInfo).Cast<MethodInfo>().ToList();
            Constructors = members.Where(m => m is ConstructorInfo).Cast<ConstructorInfo>().ToList();
        }

        private MemberInfo[] GetMembersToInject()
        {
            return Type.FindMembers(MemberTypes.All,
                BindingFlags.SetProperty | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetField,
                DelegateToSearchCriteria, typeof(InjectAttribute));
        }

        private bool DelegateToSearchCriteria(MemberInfo objMemberInfo, object objSearch)
        {
            return objMemberInfo.IsDefined((Type)objSearch, true);
        }
    }
}