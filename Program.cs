﻿using Discord;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace YahurrBot_v._2
{
	internal class Program
	{
		private string token = "MjI4NDYzNTEyMzQ1NzcyMDMy.C8UfvA.sG_LKY4PEsPzqntISxrKUD0mOjc";
		private static Program program;

		private static void Main ( string[] args )
		{
			program = new Program ();
			program.Init ();
			program.Loop ();
		}

		internal DiscordClient client;

		public void Init ()
		{
			client = new DiscordClient ();
		}

		public void Loop ()
		{
			client.MessageReceived += ( s, e ) =>
			{
				string line = e.Message.Text;
				string[] commands = (line + " ").Split (' ');

				if (!e.User.IsBot && commands[0] != null)
				{
					Module.Command (commands, line, e);
				}
			};

			client.MessageUpdated += ( s, e ) =>
			{
				string line = e.After.Text;
				string[] commands = (line + " ").Split (' ');

				if (!e.User.IsBot && commands[0] != null)
				{
					Module.UpdateCommand (commands, line, e);
				}
			};

			client.UserJoined += ( s, e ) =>
			{
				Module.JoinedUser (e);
			};

			client.ServerAvailable += ( s, e ) =>
			{
				if (client.FindServers (e.Server.Name) != null)
				{
					Console.WriteLine ("Connected to server: " + e.Server.Name);
				}
			};

			client.ExecuteAndWait (async () =>
			{
				await Task.Run (() =>
				{
					Console.WriteLine ("Connecting to discord...");
				});

				await client.Connect (token, TokenType.Bot);
				await Task.Run (() =>
				{
					while (client.Servers.Count () < 1) { }
				});

				await Task.Run (() =>
				{
					Console.WriteLine ("Connected to servers with token: " + token);
				});

				int loadedModules = await Module.LoadModules (client);

				await Task.Run (() =>
				{
					Console.WriteLine (loadedModules + " modules loaded. \n");
				});

				while (true)
				{
					string line = Console.ReadLine ();
					string[] commands = line.Split (' ');

					switch (commands[0])
					{
						case "say":
							await client.FindServers (commands[1]).FirstOrDefault ().FindChannels (commands[2]).FirstOrDefault ().SendMessage (commands[3].Replace ('-', ' '));
							break;

						default:
							break;
					}
				}
			});
		}
	}
}