using System;
using System.Collections.Generic;
using System.Linq;
using VMware.Vim;
using VMwareStatsCollector.Entities;
using VirtualMachine = VMware.Vim.VirtualMachine;

namespace VMwareStatsCollector.Services
{
	public sealed class VMwareStatsAccessorService
	{
		public VMwareStatsAccessorService(Options options)
		{
			_client = new VimClient();
			_client.Connect(options.ServerAddress);
			_client.Login(options.Username, options.Password);
			// ReSharper disable once UnusedVariable
			// Used to check whether login was successful or not
			// Throws exception if login failed.
			var vimServiceContent = _client.ServiceContent;
			_perfManager = (PerformanceManager)_client.GetView(_client.ServiceContent.PerfManager, null);
		}

		/// <summary>
		/// Gets a collection of hosts from ESXi host\VSphere with ther corresponding statistical data.
		/// </summary>
		/// <returns></returns>
		public IReadOnlyCollection<Host> GetStats()
		{
			var virtualMachines = _client
				.FindEntityViews(typeof(VirtualMachine), null, null, null)
				.OfType<VirtualMachine>()
				.Where(vm => vm.Runtime.PowerState == VirtualMachinePowerState.poweredOn)
				.ToArray();

			var hosts = _client
				.FindEntityViews(typeof(HostSystem), null, null, null)
				.OfType<HostSystem>()
				.ToArray();

			var hostsStats = new List<Host>();

			foreach (var host in hosts)
			{
				var cpuPkgs = host.Hardware.CpuPkg;
				if (cpuPkgs.Length == 0)
				{
					throw new InvalidOperationException("Somehow your system has a running host with no CPUs!");
				}

				var hostVMs = virtualMachines
					.Where(vm => vm.Runtime.Host.Equals(host.MoRef))
					.Select(GetStats)
					.ToArray();

				var cpuName = cpuPkgs[0].Description; //Hoping no one would put different CPUs in one host.
				var coresCount = host.Hardware.CpuInfo.NumCpuCores;
				var ramVolume = host.Hardware.MemorySize / 1024 / 1024;
				var usedCoresCount = hostVMs.Aggregate(0, (acc, vm) => acc + vm.CoresCount);

				//Holy fuck, performance manager is driving me nuts
				//var metrics = _perfManager.QueryAvailablePerfMetric(host.MoRef, DateTime.Now.AddDays(-7), DateTime.Now, null);
				//var counters = _perfManager.QueryPerfCounter(metrics.Select(m => m.CounterId).ToArray());
				//var cpuCounter = counters.Where(c => c.GroupInfo.Key == "cpu" && c.UnitInfo.Key == "percent").First();
				//var id = new PerfMetricId
				//{
				//	CounterId = cpuCounter.Key,
				//	Instance = "*"
				//};

				//var spec = new PerfQuerySpec
				//{
				//	MetricId = new[] {id},
				//	Entity = host.MoRef
				//};

				//var ungodlyArrayOfValues = _perfManager.QueryPerf(new[] {spec});

				var combinedFrequency = (int)(coresCount * cpuPkgs[0].Hz / 1000 / 1000);
				var consumedCpuPercentage = GetPercent(combinedFrequency,
					hostVMs.Aggregate(0, (acc, vm) => acc + vm.ConsumedCpuFrequency));
				var consumedRamPercentage = GetPercent((int) ramVolume, hostVMs.Aggregate(0, (acc, vm) => acc + vm.RamVolume));


				var hostStats = new Host(
					host.Name,
					cpuName,
					(int)ramVolume,
					coresCount,
					usedCoresCount,
					consumedCpuPercentage,
					consumedRamPercentage,
					0,
					hostVMs);

				hostsStats.Add(hostStats);
			}

			return hostsStats;
		}

		private Entities.VirtualMachine GetStats(VirtualMachine vm)
		{
			var vmView = (VirtualMachine)_client.GetView(vm.MoRef, null);

			var coresCount = vmView.Summary.Config.NumCpu ?? 0;
			var ramVolume = vmView.Summary.Config.MemorySizeMB ?? 0;
			var consumedCpuPercentage = GetPercent(vmView.Summary.Runtime.MaxCpuUsage ?? 0, vmView.Summary.QuickStats.OverallCpuUsage ?? 0);
			var consumedMemoryPercentage = GetPercent(vmView.Summary.Runtime.MaxMemoryUsage ?? 0, vmView.Summary.QuickStats.GuestMemoryUsage ?? 0);

			return new Entities.VirtualMachine(
				vmView.Name,
				coresCount,
				ramVolume,
				consumedCpuPercentage,
				consumedMemoryPercentage,
				0,
				0,
				vmView.Summary.QuickStats.OverallCpuUsage ?? 0);
		}

		private static int GetPercent(int max, int current)
		{
			if (max == 0)
			{
				return 0;
			}
			return (int)Math.Ceiling(current / ((double)max / 100));
		}

		private readonly VimClient _client;
		private readonly PerformanceManager _perfManager;
	}
}
