namespace Nodsoft.WowsReplaysUnpack.Core.Entities;

/// <summary>
/// Defines a method subscription for property changes within an entity.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public sealed class PropertyChangedSubscriptionAttribute : Attribute
{
	/// <summary>
	/// Name of the entity defining the property.
	/// </summary>
	public string EntityName { get; }

	/// <summary>
	/// Name of the property.
	/// </summary>
	public string PropertyName { get; }

	public PropertyChangedSubscriptionAttribute(string entityName, string propertyName) => (EntityName, PropertyName) = (entityName, propertyName);
}