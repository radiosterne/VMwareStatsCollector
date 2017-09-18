using CommandLine;

namespace VMwareStatsCollector
{
	public sealed class Options
	{
		[Option("server", Required = true, HelpText = "VMWare server address")]
		public string ServerAddress { get; set; }

		[Option("username", Required = true, HelpText = "VMWare user name")]
		public string Username { get; set; }

		[Option("password", Required = true, HelpText = "VMWare user password")]
		public string Password { get; set; }
	}
}