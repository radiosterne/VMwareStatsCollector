using JetBrains.Annotations;

namespace VMwareStatsCollector.Entities
{
	/// <summary>
	/// Virtual machine statistical data read model.
	/// </summary>
	public sealed class VirtualMachine
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="name">VM name.</param>
		/// <param name="coresCount">CPU cores count.</param>
		/// <param name="ramVolume">RAM volume in megabytes.</param>
		/// <param name="consumedCpuPercentage">Instantaneous percentage value of consumed CPU resources.</param>
		/// <param name="consumedMemoryPercentage">Instantaneous percentage value of consumed RAM resources.</param>
		/// <param name="maxConsumedCpuPercentage">Maximal recorded percentage value of consumed CPU resources.</param>
		/// <param name="maxConsumedMemoryPercentage">Maximal recorded percentage value of consumed RAM resources.</param>
		/// <param name="consumedCpuFrequency"></param>
		public VirtualMachine(
			[NotNull] string name,
			int coresCount,
			int ramVolume,
			int consumedCpuPercentage,
			int consumedMemoryPercentage,
			int maxConsumedCpuPercentage,
			int maxConsumedMemoryPercentage,
			int consumedCpuFrequency)
		{
			Name = name;
			CoresCount = coresCount;
			RamVolume = ramVolume;
			ConsumedCpuPercentage = consumedCpuPercentage;
			ConsumedMemoryPercentage = consumedMemoryPercentage;
			MaxConsumedCpuPercentage = maxConsumedCpuPercentage;
			MaxConsumedMemoryPercentage = maxConsumedMemoryPercentage;
			ConsumedCpuFrequency = consumedCpuFrequency;
		}

		[NotNull]
		public string Name { get; }

		public int CoresCount { get; }

		public int RamVolume { get; }

		public int ConsumedCpuPercentage { get; }

		public int ConsumedCpuFrequency { get; }

		public int ConsumedMemoryPercentage { get; }

		public int MaxConsumedCpuPercentage { get; }

		public int MaxConsumedMemoryPercentage { get; }
	}
}
