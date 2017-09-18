using System.Collections.Generic;
using JetBrains.Annotations;

namespace VMwareStatsCollector.Entities
{
	/// <summary>
	/// Host statistical and hardware data read model.
	/// </summary>
	public sealed class Host
	{
		public Host(string name,
			string cpuModel,
			int ramVolume,
			int coresCount,
			int usedCoresCount,
			int consumedCpuPercentage,
			int consumedMemoryPercentage,
			int maxConsumedCpuPercentage,
			IReadOnlyCollection<VirtualMachine> virtualMachines)
		{
			Name = name;
			CpuModel = cpuModel;
			RamVolume = ramVolume;
			CoresCount = coresCount;
			UsedCoresCount = usedCoresCount;
			ConsumedCpuPercentage = consumedCpuPercentage;
			ConsumedMemoryPercentage = consumedMemoryPercentage;
			MaxConsumedCpuPercentage = maxConsumedCpuPercentage;
			VirtualMachines = virtualMachines;
		}

		[NotNull]
		public string Name { get; }

		public string CpuModel { get; }

		public int RamVolume { get; }

		public int CoresCount { get; }

		public int UsedCoresCount { get; }

		public int ConsumedCpuPercentage { get; }

		public int ConsumedMemoryPercentage { get; }

		public int MaxConsumedCpuPercentage { get; }

		public IReadOnlyCollection<VirtualMachine> VirtualMachines { get; }
	}
}
