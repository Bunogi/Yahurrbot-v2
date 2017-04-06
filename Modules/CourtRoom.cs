using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YahurrBot_v._2.Modules
{
	[YahurrBot_v._2.Attributes.FilterServer("Kampområde mot engelsk", Attributes.FilterSetting.Whitelist)]
	internal class CourtRoom : Module
	{
		private Role judgeRole;
		private Role jurorRole;

		private bool inSession = false;

		private List<User> juryMembers;

		private List<User> queue;

		private User courtLeader;

		private void RemoveFromQueue()
		{
			if (queue.Count > 1)
				queue.RemoveAt(0);
		}

		public override void ParseCommands(string[] commands, MessageEventArgs e)
		{
			if (courtLeader == null) 
			{
				courtLeader = FindPlayer(e.Server, "Håkon");
				Console.Write("Court leader: " + courtLeader.Name);
				judgeRole = e.Server.FindRoles ("Dommerfaen", true).Last ();
				jurorRole = e.Server.FindRoles ("Jurist", true).Last ();
			}

			if (e.Channel.Name != "robottest")
				return;

			if (inSession)
			{
				if (commands[0] == "k")
				{
					e.Message.Delete();
					if (queue.Contains(e.User))
						e.Channel.SendMessage(e.User.Mention + " ER EN SLEM GUTT! Du er allerede i køen!");
					else
						queue.Add(e.User);
					return;
				}
				else if (commands[0] == "!kø")
				{
					string names = "Køen er: ";
					foreach (User user in queue)
					{
						names += user.Nickname + " ";
					}
					e.Message.Delete();
					if (e.User != courtLeader)
						e.User.SendMessage(names);
					else
						e.Channel.SendMessage(names);
					return;
				}

				if (queue.Count < 1)
				{
					e.Channel.SendMessage(e.User.Mention + " ER EN SLEM GUTT! Du må vente til det er din tur!");
					e.Message.Delete();
					return;
				}
				else if (e.User != courtLeader) 
				{
					if (queue.First() != e.User)
					{
						e.Channel.SendMessage(e.User.Mention + " ER EN SLEM GUTT! Du må vente til det er din tur!");
						e.Message.Delete();
					}
					else if (commands.Last() == "f")
						RemoveFromQueue();
					return;
				}
			}

			//Commands for the leaders
			switch (commands[0]) 
			{
				case "!rettsak":
				if (inSession)
				{
					e.Channel.SendMessage(e.User.Mention + " ER EN SLEM GUTT! Avslutt rettsaken før du starter en ny.");
					return;
				}

				inSession = true;

				//e.Server.FindUsers("Vetle André", true).Last ().AddRoles (dommerRolle);
				juryMembers = new List<User>();
				queue = new List<User>();

				foreach (string name in commands)
				{
					User user = FindPlayer(e.Server, name);
					if (user != null)
						juryMembers.Add(user);
					//bruker.AddRoles (juristRolle);
				}

				string userNames = "";
				foreach (User bruker in juryMembers.Skip(1))
					userNames += ", " + bruker.Mention;
				e.Channel.SendMessage ("Jury for dagens rettsak: " + juryMembers[0].Mention + userNames);
				return;

				case "!stopprettsak":
				if (!inSession)
				{
					e.Channel.SendMessage (e.User.Mention + " ER EN SLEM GUTT! Man kan ikke avslutte en rettsak som aldri begynte!");
					return;
				}
				//e.Server.FindUsers("Vetle André", true).Last ().RemoveRoles (dommerRolle);
				juryMembers = queue = null;
	
				/* foreach (User bruker in juryMedlemmer)
				{
					bruker.RemoveRoles (juristRolle);
				} */
				e.Channel.SendMessage("Rettsaken er over! Takk til de som deltok.");
				return;

				default:
				return;

			}
		}
	}
}
