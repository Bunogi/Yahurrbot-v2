using Discord;
using System;
using System.Collections.Generic;

namespace YahurrBot_v._2.Modules
{
	internal class Help : Module
	{
		private static List<Bindestrek> list = new List<Bindestrek> ();

		public override void Load ( DiscordClient client )
		{
			addHelp ("!help (page)", "Shows this list");
			list.Sort (( a, b ) =>
			{
				return a.command.CompareTo (b.command);
			});
		}

		public override void ParseCommands ( string[] commands, MessageEventArgs e )
		{
			if (commands[0] == "!help")
			{
				string test = "``` Help - page " + (page (commands[1]) + 1) + " of " + Math.Ceiling ((float)list.Count / 5) + Environment.NewLine;
				test += "-------------------------------------------------------" + Environment.NewLine;
				for (int i = 0; i < 5; i++)
				{
					int current = page (commands[1]) * 5 + i;
					try
					{
						var x = list[current];
					}
					catch
					{
						break;
					}
					test += list[current].command + " - " + list[current].description + Environment.NewLine;
				}

				test += "-------------------------------------------------------```";
				e.Channel.SendMessage (test);
			}
		}

		private int page ( string commands )
		{
			int number;
			if (Int32.TryParse (commands, out number))
			{
				number = Math.Abs (number);
				if (number > (Math.Ceiling ((float)list.Count / 5) - 1))
				{
					return ((int)list.Count / 5);
				}
				else
				{
					return ((int)number / 5);
				}
			}
			else
			{
				return 0;
			}
		}

		/// <summary>
		/// A function for adding your command to the !help list.
		/// </summary>
		/// <param name="command">The command and parameters for using the function</param>
		/// <param name="description">The description for the function</param>
		static public void addHelp ( string command, string description )
		{
			Bindestrek bindestrek = new Bindestrek (command, description);
			list.Add (bindestrek);
		}

		private class Bindestrek
		{
			public string command;
			public string description;

			public Bindestrek ( string command, string description )
			{
				this.command = command;
				this.description = description;
			}
		}
	}
}