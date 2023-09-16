using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace System
{
public static class GetExtensions
{
    public static MethodInfo[] GetExtensionMethods(this Type t)
    {
        // Assembly thisAssembly = t.Assembly;
        // return  GetExtensionMethodsFromAssembly(thisAssembly, t).ToArray();

        List<Type> AssTypes = new List<Type>();

        foreach (Assembly item in AppDomain.CurrentDomain.GetAssemblies())
        {
            AssTypes.AddRange(item.GetTypes());
        }

        var query = from type in AssTypes
            where type.IsSealed && !type.IsGenericType && !type.IsNested
            from method in type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
            where method.IsDefined(typeof(ExtensionAttribute), false)
            where method.GetParameters()[0].ParameterType == t
            select method;
        return query.ToArray<MethodInfo>();
    }
    public static MethodInfo GetExtensionMethod(this Type t, string MethodeName)
    {
        var mi = from methode in t.GetExtensionMethods()
            where methode.Name == MethodeName
            select methode;
        if (mi.Count<MethodInfo>() <= 0)
            return null;
        else
            return mi.First<MethodInfo>();
    }
    static IEnumerable<MethodInfo> GetExtensionMethodsFromAssembly(Assembly assembly,
        Type extendedType)
    {
        var query = from type in assembly.GetTypes()
                    where type.IsSealed && !type.IsGenericType && !type.IsNested
                    from method in type.GetMethods(BindingFlags.Static
                        | BindingFlags.Public | BindingFlags.NonPublic)
                    where method.IsDefined(typeof(ExtensionAttribute), false)
                    where method.GetParameters()[0].ParameterType == extendedType
                    select method;
        return query;
    }

    public static IEnumerable<KeyValuePair<Type, MethodInfo>> GetExtensionMethodsDefinedInType(this Type t)
    {
        if (!t.IsSealed || t.IsGenericType || t.IsNested)
            return Enumerable.Empty<KeyValuePair<Type, MethodInfo>>();

        var methods = t.GetMethods(BindingFlags.Public | BindingFlags.Static)
                    .Where(m => m.IsDefined(typeof(ExtensionAttribute), false));

        List<KeyValuePair<Type, MethodInfo>> pairs = new List<KeyValuePair<Type, MethodInfo>>();
        foreach (var m in methods)
        {
            var parameters = m.GetParameters();
            if (parameters.Length > 0)
            {
                if (parameters[0].ParameterType.IsGenericParameter)
                {
                    if (m.ContainsGenericParameters)
                    {
                        var genericParameters = m.GetGenericArguments();
                        Type genericParam = genericParameters[parameters[0].ParameterType.GenericParameterPosition];
                        foreach (var constraint in genericParam.GetGenericParameterConstraints())
                            pairs.Add(new KeyValuePair<Type, MethodInfo>(parameters[0].ParameterType, m));
                    }
                }
                else
                    pairs.Add(new KeyValuePair<Type, MethodInfo>(parameters[0].ParameterType, m));
            }
        }

        return pairs;
    }
}
}