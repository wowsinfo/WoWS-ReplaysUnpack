namespace Nodsoft.WowsReplaysUnpack.Core.Entities;

[AttributeUsage(AttributeTargets.Method)]
public class MethodSubscriptionAttribute : Attribute
{
	public string EntityName { get; }
	public string MethodName { get; }
	public bool ParamsAsDictionary { get; set; }
	public bool IncludeEntity { get; set; }
	public bool IncludePacketTime { get; set; }

  /// <summary>
  /// Order in which methods will be called. Smaller = better
  /// -1 is reserved for security checks
  /// </summary>
	public int Priority { get; set; } = 0;

	public MethodSubscriptionAttribute(string entityName, string methodName)
	{
		EntityName = entityName;
		MethodName = methodName;
	}
}
