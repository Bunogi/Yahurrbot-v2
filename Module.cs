using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace YahurrBot_v._2
{
	internal abstract class Module
	{
		private static List<Module> loadedModules = new List<Module> (); // List of all currently loaded modules
		private static List<DataObject> savedObjects = new List<DataObject> ();

		// Finds all classes that extends Modules and creates and instance of each of them.
		// Then it calls the load function
		public static async Task<int> LoadModules ( Discord.DiscordClient client )
		{
			// Loades saved data
			string dir = Directory.GetCurrentDirectory ();
			string loaded = File.ReadAllText (dir + "/Files/Saves.txt", System.Text.Encoding.UTF8);
			List<DataObject> loadedObjects = JsonConvert.DeserializeObject<List<DataObject>> (loaded);
			savedObjects = loadedObjects == null ? savedObjects : loadedObjects;

			// Load all the available modules.
			IEnumerable<Type> types = from t in Assembly.GetExecutingAssembly ().GetTypes () where t.IsClass && typeof (Module).IsAssignableFrom (t) && t.IsAbstract == false && !Attribute.IsDefined (t, typeof (Attributes.IgnoreModule)) select t;
			foreach (Type type in types)
			{
				await Task.Run (() =>
				{
					Module module = (Module)Activator.CreateInstance (type);
					loadedModules.Add (module);

					module.Load (client);
				});
			}

			return types.Count ();
		}

		public static void ExitProgram ( object s, EventArgs e )
		{
			foreach (Module module in loadedModules)
			{
				module.OnExit ();
			}
		}

		public static void ConsoleCommand ( string[] commands, string line )
		{
			foreach (Module module in loadedModules)
			{
				module.ParseConsoleCommands (commands);
				module.ParseConsoleCommands (commands, line);
			}
		}

		public static void Command ( string[] commands, string line, Discord.MessageEventArgs e )
		{
			foreach (Module module in from t in loadedModules where Attribute.IsDefined (t.GetType (), typeof (Attributes.FilterServer)) && t.GetType ().GetCustomAttribute<Attributes.FilterServer> ().IsFiltered (e.Server.Name) || t.GetType ().CustomAttributes.Count () == 0 select t)
			{
				module.ParseCommands (commands, e);
				module.ParseCommands (commands, line, e);
			}
		}

		public static void UpdateCommand ( string[] commands, string line, Discord.MessageUpdatedEventArgs e )
		{
			foreach (Module module in from t in loadedModules where Attribute.IsDefined (t.GetType (), typeof (Attributes.FilterServer)) && t.GetType ().GetCustomAttribute<Attributes.FilterServer> ().IsFiltered (e.Server.Name) || t.GetType ().CustomAttributes.Count () == 0 select t)
			{
				module.CommandUpdated (commands, e);
				module.CommandUpdated (commands, line, e);
			}
		}

		public static void UpdateProfile ( Discord.UserUpdatedEventArgs e )
		{
			foreach (Module module in from t in loadedModules where Attribute.IsDefined (t.GetType (), typeof (Attributes.FilterServer)) && t.GetType ().GetCustomAttribute<Attributes.FilterServer> ().IsFiltered (e.Server.Name) || t.GetType ().CustomAttributes.Count () == 0 select t)
			{
				module.ProfileUpdate (e);
			}
		}

		public static void JoinedUser ( Discord.UserEventArgs e )
		{
			foreach (Module module in from t in loadedModules where Attribute.IsDefined (t.GetType (), typeof (Attributes.FilterServer)) && t.GetType ().GetCustomAttribute<Attributes.FilterServer> ().IsFiltered (e.Server.Name) || t.GetType ().CustomAttributes.Count () == 0 select t)
			{
				module.UserJoined (e);
			}
		}

		/// <summary>
		/// Called when the module is loaded. (Once)
		/// </summary>
		/// <param name="client">Client this module is using.</param>
		public virtual void Load ( Discord.DiscordClient client )
		{
		}

		/// <summary>
		/// Called just before the program exits.
		/// </summary>
		public virtual void OnExit ()
		{
		}

		/// <summary>
		/// Called when a command is typed in the console.
		/// </summary>
		/// <param name="commands">Command pluss each parameter separeted by a space.</param>
		public virtual void ParseConsoleCommands ( string[] commands )
		{
		}

		public virtual void ParseConsoleCommands ( string[] commands, string line )
		{
		}

		/// <summary>
		/// Called when a user types a command in the discord chat.
		/// </summary>
		/// <param name="commands">Command pluss each parameter separeted by a space.</param>
		/// <param name="e">Discords MessageEvent.</param>
		public virtual void ParseCommands ( string[] commands, Discord.MessageEventArgs e )
		{
		}

		public virtual void ParseCommands ( string[] commands, string line, Discord.MessageEventArgs e )
		{
		}

		/// <summary>
		/// Called when a user updates a message wich contains an command.
		/// </summary>
		/// <param name="commands">Command pluss each parameter separeted by a space.</param>
		/// <param name="e">Discords MessageUpdatedEvent.</param>
		public virtual void CommandUpdated ( string[] commands, Discord.MessageUpdatedEventArgs e )
		{

		}

		public virtual void CommandUpdated ( string[] commands, string line, Discord.MessageUpdatedEventArgs e )
		{

		}

		/// <summary>
		/// Called when a users profile has been updated. (GameChanged, NameCHange, ect...)
		/// </summary>
		/// <param name="e">Dicords UserUpdated event.</param>
		public virtual void ProfileUpdate ( Discord.UserUpdatedEventArgs e )
		{
		}

		/// <summary>
		/// Called when a user joined the server.
		/// </summary>
		/// <param name="e">Discords UserArgs event.</param>
		public virtual void UserJoined ( Discord.UserEventArgs e )
		{
		}

		public Discord.User FindPlayer ( Discord.Server server, string identefier )
		{
			identefier = identefier.Replace ("@", "");
			List<Discord.User> users = new List<Discord.User> (server.Users);

			Console.WriteLine (users.Count);

			Discord.User user = users.Find (a =>
			{
				bool name = a.Name.ToLower () == identefier.ToLower ();
				bool nick = false;
				if (a.Nickname != null)
				{
					nick = a.Nickname.ToLower () == identefier.ToLower ();
				}
				return name || nick;
			});

			return user;
		}

		public bool Save<T> ( T obj, string name, bool repleace )
		{
			DataObject found = savedObjects.Find (a => { return a.name == name; });

			if (found == null)
			{
				DataObject dataObj = new DataObject (obj, name);
				savedObjects.Add (dataObj);

				string dir = Directory.GetCurrentDirectory ();
				string json = JsonConvert.SerializeObject (savedObjects, Formatting.Indented);
				File.WriteAllText (dir + "/Files/Saves.txt", json, System.Text.Encoding.UTF8);
				return true;
			}
			else if (repleace)
			{
				found.obj = obj;

				string dir = Directory.GetCurrentDirectory ();
				string json = JsonConvert.SerializeObject (savedObjects, Formatting.Indented);
				File.WriteAllText (dir + "/Files/Saves.txt", json, System.Text.Encoding.UTF8);
			}
			return false;
		}

		public T Load<T> ( string name )
		{
			dynamic obj = savedObjects.Find (a => { return a.name == name; }).obj;

			// Just return the dynamic is it matches T. It will be changed to string when it is loaded from text.
			if (obj.GetType () == typeof (T))
			{
				return (T)obj;
			}
			else
			{
				return JsonConvert.DeserializeObject<T> (obj.ToString ());
			}
		}
	}

	internal class DataObject
	{
		public string name { get; private set; }

		public dynamic obj { get; set; }

		public DataObject ( object obj, string name )
		{
			this.name = name;
			this.obj = obj;
		}
	}
}