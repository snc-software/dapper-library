using System.Reflection;

namespace SncSoftware.Extensions.Dapper.Reflection;

internal class ReflectionTypeCache
{
    public ReflectedProperty? PrimaryIdentifierProperty { get; set; }
    public PropertyInfo[] PropertyInfos { get; set; }
    public IDictionary<string, ReflectedProperty> IndexedProperties { get; set; }
    public ReflectedProperty[] Properties { get; set; }
}