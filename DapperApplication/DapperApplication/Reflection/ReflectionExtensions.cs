using System.Reflection;
using DapperApplication.Attributes;

namespace DapperApplication.Reflection;

public static class ReflectionExtensions
{
    public static PropertyInfo? ReflectPrimaryIdentifierProperty(this Type type)
    {
        return GetCacheFor(type).PrimaryIdentifierProperty?.PropertyInfo;
    }
    
    public static PropertyInfo[] ReflectProperties(this Type type)
    {
        return GetCacheFor(type).PropertyInfos;
    }

    public static PropertyInfo[] ReflectProperties(this object obj)
    {
        return obj.GetType().ReflectProperties();
    }
    
    #region Reflection Cache

    private const BindingFlags PublicInstance = BindingFlags.Public | BindingFlags.Instance;

    private static readonly IDictionary<Type, ReflectionTypeCache> Types = new Dictionary<Type, ReflectionTypeCache>();
    private static readonly object Lock = new();

    private static ReflectionTypeCache GetCacheFor(Type type)
    {
        lock (Lock)
        {
            if (!Types.ContainsKey(type))
            {
                InitialiseCacheFor(type);
            }

            return Types[type];
        }
    }

    private static void InitialiseCacheFor(Type type)
    {
        var cache = new ReflectionTypeCache();
        InitialiseProperties(type, cache);
        Types.Add(type, cache);
    }

    private static void InitialiseProperties(Type type, ReflectionTypeCache cache)
    {
        cache.PropertyInfos = type.GetProperties(PublicInstance);
        cache.Properties = cache.PropertyInfos.Select(p => new ReflectedProperty(p)).ToArray();
        cache.IndexedProperties = cache.Properties.ToDictionary(p => p.Name, p => p);
        cache.PrimaryIdentifierProperty = cache.Properties.SingleOrDefault(
            p => p.HasAttribute<PrimaryIdentifierAttribute>());
    }
    
    #endregion
}