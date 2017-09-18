using System;
using System.Linq;
using CommandLine;
using VMware.Vim;
using VMwareStatsCollector.Services;

namespace VMwareStatsCollector
{
	class Program
	{
		static void Main(string[] args)
		{
			var parserResult = Parser.Default.ParseArguments<Options>(args);
			parserResult.WithParsed(opts =>
			{
				var statsService = new VMwareStatsAccessorService(opts);
				var stats = statsService.GetStats();
				var printingService = new ExcelPrintingService();
				printingService.PrintHostStats(stats);
			});
		}

		static void FiddleAroundWithClient(VimClient client)
		{
			var virtualMachines = client
				.FindEntityViews(typeof(VirtualMachine), null, null, null)
				.OfType<VirtualMachine>()
				.Where(vm => vm.Runtime.PowerState == VirtualMachinePowerState.poweredOn)
				.ToArray();

			var hosts = client
				.FindEntityViews(typeof(HostSystem), null, null, null)
				.OfType<HostSystem>()
				.ToArray();

			foreach (var vm in virtualMachines)
			{
				var view = (VirtualMachine)client.GetView(vm.MoRef, null);
				//var performanceManager = (PerformanceManager)(object)client.ServiceContent.PerfManager;
			}
		}

		static VimClient Connect(Options options)
		{
			VimClient vimClient = new VimClient();
			try
			{
				vimClient.Connect(options.ServerAddress);
				var vimSession = vimClient.Login(options.Username, options.Password);
				var vimServiceContent = vimClient.ServiceContent;

				return vimClient;
			}
			catch(Exception ex)
			{
				//
				// VMware Exception occurred
				//
				//txtErrors.Text = "A server fault of type " + ex.MethodFault.GetType().Name + " with message '" + ex.Message + "' occured while performing requested operation.";
				//Error_Panel.Visible = true;
				return null;
			}
		}
	}
}
