namespace Nodsoft.WowsReplaysUnpack.Core.Entities;

[AttributeUsage(AttributeTargets.Method)]
public class PropertyChangedSubscriptionAttribute : Attribute
{
	public string EntityName { get; }
	public string PropertyName { get; }
	public PropertyChangedSubscriptionAttribute(string entityName, string propertyName)
	{
		EntityName = entityName;
		PropertyName = propertyName;
	}
}
