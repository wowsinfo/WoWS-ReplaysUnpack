using Nodsoft.WowsReplaysUnpack.Core.Definitions;
using Nodsoft.WowsReplaysUnpack.Core.Entities;
using Nodsoft.WowsReplaysUnpack.Core.Models;
using Nodsoft.WowsReplaysUnpack.Core.Network.Packets;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Nodsoft.WowsReplaysUnpack.Services;

public class DefaultReplayController : IReplayController
{
	private static readonly Dictionary<string, MethodInfo> _methodSubscriptions;
	private static readonly Dictionary<string, MethodInfo> _propertyChangedSubscriptions;
	private readonly DefinitionStore _definitionStore;

	public virtual UnpackedReplay? Replay { get; protected set; }

	static DefaultReplayController()
	{
		_methodSubscriptions = typeof(DefaultReplayController).GetMethods()
			.Select(m => new { Attribute = m.GetCustomAttribute<MethodSubscriptionAttribute>(), MethodInfo = m })
			.Where(m => m.Attribute is not null)
			.ToDictionary(m => $"{m.Attribute!.EntityName}_{m.Attribute.MethodName}", m => m.MethodInfo);

		_propertyChangedSubscriptions = typeof(DefaultReplayController).GetMethods()
			.Select(m => new { Attribute = m.GetCustomAttribute<PropertyChangedSubscriptionAttribute>(), MethodInfo = m })
			.Where(m => m.Attribute is not null)
			.ToDictionary(m => $"{m.Attribute!.EntityName}_{m.Attribute.PropertyName}", m => m.MethodInfo);
	}

	public DefaultReplayController(DefinitionStore definitionStore)
	{
		_definitionStore = definitionStore;
	}

	public virtual UnpackedReplay CreateUnpackedReplay(ArenaInfo arenaInfo)
	{
		Replay = new UnpackedReplay(arenaInfo);
		return Replay!;
	}

	public virtual void HandleNetworkPacket(INetworkPacket networkPacket)
	{
		if (networkPacket is MapPacket mapPacket)
			OnMap(mapPacket);
		//else if (networkPacket is BasePlayerCreatePacket packet)
		//	OnBasePlayerCreate(packet);
	}

	public virtual void OnMap(MapPacket packet)
	{

	}
	public virtual void OnBasePlayerCreate(BasePlayerCreatePacket packet)
	{

	}
}
