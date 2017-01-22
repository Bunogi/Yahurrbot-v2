using Discord;
using System;
using System.Collections.Generic;
using System.Linq;

namespace YahurrBot_v._2.Modules
{
	internal class UselessInteraction : Module
	{
		public override void Load ( DiscordClient client )
		{
			Help.addHelp ("!8ball <question> ?", "Gives you an answer to your yes/no-question");
		}

		public override void ParseCommands ( String[] command, MessageEventArgs e )
		{
			switch (command[0])
			{
				case "hei":
				if (command.Length == 2 || command[1] == "yahurr" || command[1] == "alle")
				{
					e.Channel.SendMessage ("Hei, " + e.User.Mention + "!");
					//Debug message to find length of command
					//int messageLength = command.Length;  e.Channel.SendMessage(messageLength.ToString());
				}
				break;

				case "!8ball":
				if (command.Length >= 3 && command[command.Length - 2][command[command.Length - 2].Length - 1] == '?')
				{
					String answer = eightBall ();
					//Debug message with interesting numbers
					//e.Channel.SendMessage(answer + " " + command.Length.ToString() + " [" + command[command.Length - 2][command[command.Length - 2].Length - 1] + "]");
					e.Channel.SendMessage (answer);
				}

				break;

				case "!role":
				IEnumerable<User> player = e.Server.FindUsers (command[1]);
				if (command.Length >= 2 && player.Count () >= 1)
				{
					e.Channel.SendMessage ("Yey!");
					//IEnumerable<Role> role = e.User.Roles.
				}
				break;

				case "!snowboard":
				e.Channel.SendMessage ("Did you mean !showboard?");
				break;

				case "!fun":
				e.Channel.SendMessage ("https://reddit.com/r/clopclop");
				break;

				default:
				break;
			}
		}

		private String eightBall ()
		{
			int rand;

			Random random = new Random ();

			rand = random.Next (0, 100);
			String[] answer = new String[] { "That would be true", "That would equate to false", "No", "Yes", "You are right", "You are wrong", "Simple minds would believe that to be true", "I am " + rand + "% sure about that" };

			rand = random.Next (0, 8);
			return answer[rand];
		}
	}
}