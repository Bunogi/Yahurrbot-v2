using Discord;
using Newtonsoft.Json;
using System;
using System.IO;

namespace YahurrBot_v._2.Modules
{
	[Attributes.FilterServer ("Yahurr", ".,.", Attributes.FilterSetting.Whitelist)]
	internal class Testing : Module
	{
		public override void Load ( DiscordClient client )
		{

		}

		public override void ParseCommands ( string[] commands, string line, MessageEventArgs e )
		{
			e.Channel.SendMessage (line);
		}
	}
}