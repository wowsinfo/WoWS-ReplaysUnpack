using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nodsoft.WowsReplaysUnpack.Services;
using System;
using System.IO;

namespace Nodsoft.WowsReplaysUnpack.Benchmark
{
	public class UnpackBenchmark
	{
		private static string samplePath = Path.Join(Environment.CurrentDirectory, "sample");
		private static readonly MemoryStream _ms;
		private static readonly IReplayUnpackerService _defaultUnpacker;

		static UnpackBenchmark()
		{
			if (_ms is null)
			{
				FileStream fs = File.OpenRead(Path.Join(samplePath, "0.11.2.wowsreplay"));
				_ms = new();
				fs.CopyTo(_ms);
				fs.Dispose();
			}
			var services = new ServiceCollection()
					.AddWowsReplayUnpacker()
					.AddLogging(l => l.ClearProviders())
					.BuildServiceProvider();

			var factory = services.GetRequiredService<ReplayUnpackerFactory>();
			_defaultUnpacker = factory.GetUnpacker();
		}

		[Benchmark]
		public void DefaultUnpack()
		{
			using MemoryStream ms = new(_ms.ToArray());
			_ = _defaultUnpacker.Unpack(ms);
		}
	}
}
