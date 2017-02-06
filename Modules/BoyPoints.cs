using Discord;
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace YahurrBot_v._2.Modules
{
	internal class BoyPoints : Module
	{
		private string path = Directory.GetCurrentDirectory ();
		private List<BoyStatus> users = new List<BoyStatus> ();

		private DiscordClient client;

		public override void Load ( DiscordClient client )
		{
			this.client = client;

			users = Load<List<BoyStatus>> ("BoyPoints");

			Help.addHelp (" Goodboy (name)", "Gives a goodboy point to a player");
			Help.addHelp (" Badboy (name)", "Takes a goodboy point from a player");
		}

		/// <summary>
		/// Executes command typed in the discord chat.
		/// </summary>
		public override void ParseCommands ( string[] commdands, MessageEventArgs e )
		{

			Console.WriteLine (commdands[1]);

			switch (commdands[0])
			{
				case "goodboy":
				Goodboy (commdands, e);
				Save (users, "BoyPoints", true);
				break;

				case "badboy":
				Badboy (commdands, e);
				Save (users, "BoyPoints", true);
				break;

				case "!points":
				ShowPoints (commdands, e);
				break;

				default:
				break;
			}
		}

		/// <summary>
		/// Executes command typed in the console.
		/// </summary>
		public override void ParseConsoleCommands ( string[] commdands )
		{
			switch (commdands[0])
			{
				case "say":
				client.FindServers (commdands[1]).First ().FindChannels (commdands[2]).First ().SendMessage (commdands[3].Replace ('-', ' '));
				break;

				case "save":
				Save (users, "BoyPoints", true);
				break;

				case "load":
				users = Load<List<BoyStatus>> ("BoyPoints");
				break;

				case "reset":
				users = new List<BoyStatus> ();
				Save (users, "BoyPoints", true);
				break;

				case "resetSpend":
				foreach (BoyStatus bois in users)
				{
					bois.ResetSpend ();
				}
				break;

				default:
				break;
			}
		}

		private void Goodboy ( string[] commdands, MessageEventArgs e )
		{
			User user = FindPlayer (e.Server, commdands[1]);

			if (user != null && e.Message.User != user)
			{
				BoyStatus boy = FindBoy (e.User.Name);
				if (boy.toSend > 0)
				{
					FindBoy (user.Name).AddPoint ();
					boy.SpendToSend ();
					e.Channel.SendMessage (user.Mention + " gained a good boy point.");
				}
				else
				{
					e.Channel.SendMessage ("You have no more points to use!");
				}
			}
		}

		private void Badboy ( string[] commdands, MessageEventArgs e )
		{
			User user = FindPlayer (e.Server, commdands[1]);

			if (user != null && e.Message.User != user)
			{
				BoyStatus boy = FindBoy (e.User.Name);
				if (boy.toSend > 0)
				{
					FindBoy (user.Name).RemovePoint ();
					boy.SpendToSend ();
					e.Channel.SendMessage (user.Mention + " lost a good boy point.");
				}
				else
				{
					e.Channel.SendMessage ("You have no more points to use!");
				}
			}
		}

		private void ShowPoints ( string[] commdands, MessageEventArgs e )
		{
			IEnumerable<User> clients = e.Channel.FindUsers (commdands[1]);
			BoyStatus status = FindBoy (e.User.Name);

			switch (status.points)
			{
				case 1:
				e.Channel.SendMessage (e.User.Mention + " has " + status.points + " point.");
				break;

				default:
				e.Channel.SendMessage (e.User.Mention + " has " + status.points + " points.");
				break;
			}
		}

		private BoyStatus FindBoy ( string username )
		{
			BoyStatus status = users.Find (a => { return a.userName == username; });

			if (status == null)
			{
				status = new BoyStatus (username);
				users.Add (status);
			}

			return status;
		}
	}

	internal class BoyStatus
	{
		private string name;

		public string userName
		{
			get
			{
				return name;
			}
		}

		private int boyPoints;

		public int points
		{
			get
			{
				return boyPoints;
			}
		}

		private int left = 3;

		public int toSend
		{
			get
			{
				return left;
			}
		}

		[JsonConstructor]
		public BoyStatus ( string userName, int points )
		{
			this.name = userName;
			this.boyPoints = points;
		}

		public BoyStatus ( string userName )
		{
			this.name = userName;
		}

		public BoyStatus ( string userName, int points, int toSend )
		{
			boyPoints = points;
			left = toSend;
			name = userName;
		}

		public void AddPoint ()
		{
			boyPoints++;
		}

		public void RemovePoint ()
		{
			boyPoints--;
		}

		public void SpendToSend ()
		{
			left--;
		}

		public void ResetSpend ()
		{
			left = 3;
		}
	}
}