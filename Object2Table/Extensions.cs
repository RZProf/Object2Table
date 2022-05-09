using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;
using Sigil.NonGeneric;

namespace Object2Table
{
    internal static class Extensions
    {
        internal static Type ToElementType(this Type type)
        {
            if (IsEnumerable(type))
            {
                return type.GetElementType() ?? type.GenericTypeArguments.FirstOrDefault();
            }

            return type;
        }

        internal static bool IsEnumerable(this Type type)
        {
            return typeof(IEnumerable).IsAssignableFrom(type) && type != typeof(string);
        }

        internal static bool IsEnumerable(this object data, out IEnumerable enumerable)
        {
            var dataIsEnumerable = data is IEnumerable;
            enumerable = dataIsEnumerable ? (IEnumerable) data : null;
            return !(data is string) && dataIsEnumerable && !(data is IDictionary);
        }
        
        internal static PropertyEmitter[] GetPropertyEmitters(this Type type)
        {
            return type.GetProperties()
                .Where(i => i.GetCustomAttribute<IgnoreDataMemberAttribute>() is null)
                .Select(i => new PropertyEmitter
                {
                    Property = i.GetCustomAttribute<DataMemberAttribute>() is null
                        ? i.Name
                        : i.GetCustomAttribute<DataMemberAttribute>()?.Name,
                    Emitter = GetEmitter(type, i)
                }).ToArray();
        }

        private static Delegate GetEmitter(Type elementType, PropertyInfo propertyInfo)
        {
            return Emit.NewDynamicMethod(propertyInfo.PropertyType, new []{elementType})
                .LoadArgument(0)
                .Call(propertyInfo.GetGetMethod())
                .Return()
                .CreateDelegate(typeof(Func<,>).MakeGenericType(elementType, propertyInfo.PropertyType));
        }
    }
}