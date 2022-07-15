namespace Nodsoft.WowsReplaysUnpack.Core.Entities;

[AttributeUsage(AttributeTargets.Method)]
public class MethodSubscriptionAttribute: Attribute
{
	public string EntityName { get; }
	public string MethodName { get; }
	public MethodSubscriptionAttribute(string entityName, string methodName)
	{
		EntityName = entityName;
		MethodName = methodName;
	}
}
