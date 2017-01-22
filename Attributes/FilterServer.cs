using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YahurrBot_v._2.Attributes
{
	[AttributeUsage (AttributeTargets.Class)]
	internal class FilterServer : Attribute
	{
		private List<string> filteredServers = new List<string> ();
		private FilterSetting setting;

		public FilterServer ( string server, FilterSetting setting )
		{
			filteredServers.Add (server);
			this.setting = setting;
		}

		public FilterServer ( string server, string server1, FilterSetting setting )
		{
			filteredServers.Add (server);
			filteredServers.Add (server1);
			this.setting = setting;
		}

		public FilterServer ( string server, string server1, string server2, FilterSetting setting )
		{
			filteredServers.Add (server);
			filteredServers.Add (server1);
			filteredServers.Add (server2);
			this.setting = setting;
		}

		public FilterServer ( string server, string server1, string server2, string server3, FilterSetting setting )
		{
			filteredServers.Add (server);
			filteredServers.Add (server1);
			filteredServers.Add (server2);
			filteredServers.Add (server3);
			this.setting = setting;
		}

		public bool IsFiltered ( string server )
		{
			return (filteredServers.Contains (server) && setting == FilterSetting.Whitelist) || (!filteredServers.Contains (server) && setting == FilterSetting.Blacklist);
		}
	}

	internal enum FilterSetting
	{
		Whitelist,
		Blacklist,
	}
}