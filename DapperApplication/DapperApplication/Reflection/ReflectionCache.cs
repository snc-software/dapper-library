using System.Reflection;

namespace DapperApplication.Reflection;


public class ReflectionTypeCache
{
    public const BindingFlags PublicInstance = BindingFlags.Public | BindingFlags.Instance;
    
    public ReflectedProperty? PrimaryIdentifierProperty { get; set; }
    public PropertyInfo[] PropertyInfos { get; set; }
    public IDictionary<string, ReflectedProperty> IndexedProperties { get; set; }
    public ReflectedProperty[] Properties { get; set; }
}

public class ReflectedProperty
{
    public PropertyInfo? PropertyInfo { get; }
    public Attribute[] Attributes { get; }

    public ReflectedProperty(PropertyInfo? property)
    {
        PropertyInfo = property;
        Attributes = property.GetCustomAttributes().ToArray();
    }

    public string Name => PropertyInfo.Name;

    public bool HasAttribute<TAttribute>() where TAttribute : Attribute
    {
        return Attributes.Any(a => a is TAttribute);
    }
}