using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;

namespace YahurrBot_v._2.Modules
{


	class GameLog : Module
	{
		public override void Load ( DiscordClient client )
		{
			IEnumerable<Channel> channels = client.Servers.FirstOrDefault ().AllChannels;
			List<ChatLog> logs = new List<ChatLog> ();

			foreach (Channel channel in channels)
			{
				ChatLog log = new ChatLog ();
				foreach (Message mes in channel.Messages)
				{
					log.AddMessage (mes);
				}
				logs.Add (log);
			}

			//Save (logs, "YahurrLog", true);
			List<ChatLog> newLogs = Load<List<ChatLog>> ("YahurrLog");
		}

		class ChatLog
		{
			List<Message> messages = new List<Message> ();

			public void AddMessage ( Message message )
			{
				messages.Add (message);
			}
		}
	}
}
