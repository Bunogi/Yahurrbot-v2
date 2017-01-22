using Discord;
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

			LoadPoints ();
			Help.addHelp (" Goodboy (name)", "Gives a goodboy point to a player");
			Help.addHelp (" Badboy (name)", "Takes a goodboy point from a player");
		}

		/// <summary>
		/// Executes command typed in the discord chat.
		/// </summary>
		public override void ParseCommands ( string[] commdands, MessageEventArgs e )
		{
			switch (commdands[0])
			{
				case "goodboy":
				Goodboy (commdands, e);
				SavePoints ();
				break;

				case "badboy":
				Badboy (commdands, e);
				SavePoints ();
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
				SavePoints ();
				break;

				case "load":
				LoadPoints ();
				break;

				case "reset":
				File.WriteAllText (path + "/Files/Saves.txt", string.Empty);
				break;

				default:
				break;
			}
		}

		public void SavePoints ()
		{
			string json = JsonConvert.SerializeObject (users.ToArray (), Formatting.None);

			File.WriteAllText (path + "/Files/Saves.txt", json, System.Text.Encoding.UTF8);
		}

		public void LoadPoints ()
		{
			JArray j = (JArray)JsonConvert.DeserializeObject (File.ReadAllText (path + "/Files/Saves.txt", System.Text.Encoding.UTF8));
			List<BoyStatus> newUsers = new List<BoyStatus> ();

			for (int i = 0; i < j.Count; i++)
			{
				string userName = (string)j[i]["userName"];
				int points = int.Parse ((string)j[i]["points"]);
				int toSend = int.Parse ((string)j[i]["toSend"]);

				newUsers.Add (new BoyStatus (userName, points, toSend));
			}

			users = newUsers;
		}

		private void Goodboy ( string[] commdands, MessageEventArgs e )
		{
			IEnumerable<User> clients = e.Channel.FindUsers (commdands[1]);
			User user = null;
			if (client != null)
				user = clients.First ();
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
			IEnumerable<User> clients = e.Channel.FindUsers (commdands[1]);
			User user = null;

			clients = e.Channel.FindUsers (commdands[1]);
			user = null;
			if (client != null)
				user = clients.First ();
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
		[JsonIgnore]
		private string name;

		public string userName
		{
			get
			{
				return name;
			}
		}

		private int boyPoints;

		[JsonProperty ("points")]
		public int points
		{
			get
			{
				return boyPoints;
			}
		}

		private int left = 3;

		[JsonProperty ("toSend")]
		public int toSend
		{
			get
			{
				return left;
			}
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
	}
}