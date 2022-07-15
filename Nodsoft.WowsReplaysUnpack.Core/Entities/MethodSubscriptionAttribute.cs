namespace Nodsoft.WowsReplaysUnpack.Core.Entities;

[AttributeUsage(AttributeTargets.Method)]
public class MethodSubscriptionAttribute: Attribute
{
	public string EntityName { get; }
	public string MethodName { get; }
	public bool ParamsAsDictionary { get; }

	public MethodSubscriptionAttribute(string entityName, string methodName, bool paramsAsDictionary = false)
	{
		EntityName = entityName;
		MethodName = methodName;
		ParamsAsDictionary = paramsAsDictionary;
	}
}
