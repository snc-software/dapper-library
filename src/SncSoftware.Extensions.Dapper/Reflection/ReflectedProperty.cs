using System.Reflection;

namespace SncSoftware.Extensions.Dapper.Reflection;

internal class ReflectedProperty
{
    public PropertyInfo? PropertyInfo { get; }
    public Attribute[] Attributes { get; }

    public ReflectedProperty(PropertyInfo? property)
    {
        PropertyInfo = property;
        Attributes = property.GetCustomAttributes().ToArray();
    }

    public string Name => PropertyInfo?.Name ?? string.Empty;

    public bool HasAttribute<TAttribute>() where TAttribute : Attribute
    {
        return Attributes.Any(a => a is TAttribute);
    }
}