using System;
using TerrariaApi.Server;
using Terraria;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;
using TShockAPI;
using System.Linq;

namespace CustomMessages
{
	[ApiVersion(2, 1)]
    public class CustomMessages : TerrariaPlugin
    {
		// terraria definition
        public CustomMessages(Main game) : base(game) { }

		// private variables
		private static Dictionary<string, string> CommandsWithResponses = new Dictionary<string, string>();
		private static readonly string ConfigPath = Path.Combine(TShock.SavePath, "commands.json");

		// tserverapi overrides
		public override Version Version => System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
        public override string Author => "Rozen4334, base by Nyan-Ko";
        public override string Description => "Adds custom command responses through configuration.";
        public override string Name => "CustomMessages";
        public override void Initialize()
        {
			// add the msgadd command
			Commands.ChatCommands.Add(new Command("custommessages.add", AddCommand, "addcommand"));
			// add the reload command
			Commands.ChatCommands.Add(new Command("custommessages.reload", ReloadMsgs, "reloadmsgs"));
		}
		
		// load the config
		private static void LoadConfig()
		{
			if (!File.Exists(ConfigPath))
			{

				Dictionary<string, string> placeholder = new Dictionary<string, string> {
					{ "entry, no need to add a /", "format stuff like this" },
					{ "second entry", "format multiple entries like this" }
				};

				File.WriteAllText(ConfigPath, JsonConvert.SerializeObject(placeholder, Formatting.Indented));
				Console.WriteLine("Ingame configurable command list is empty, just fyi.");

				CommandsWithResponses = new Dictionary<string, string>();  // don't want random placeholder commands lol
			}
			else
			{
				CommandsWithResponses = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(ConfigPath));
			}
		}

		// save the config
		private static void SaveConfig()
		{
			File.WriteAllText(ConfigPath, JsonConvert.SerializeObject(CommandsWithResponses, Formatting.Indented));
		}

		//paste commands from config
		private static void PasteCommandList()
		{
			foreach (var command in CommandsWithResponses)
			{
				Commands.ChatCommands.Add(new Command("", SendCommandResponse, command.Key));
			}
		}

		// send the response
		private static void SendCommandResponse(CommandArgs args)
		{
			if (args.Parameters.Count == 0)
			{
				args.Player?.SendInfoMessage(CommandsWithResponses[args.Message]);
			}
			else
			{
				args.Player?.SendInfoMessage(CommandsWithResponses[args.Message.Split(' ')[0]]);
			}
		}

		// add new commands
		public static void AddCommand(CommandArgs args)
		{
			List<string> parameters = args.Parameters;

			if (parameters.Count < 2)
			{
				args.Player?.SendErrorMessage("Invalid syntax, expected /addcommand (identifier) (response)");
				return;
			}

			string identifier = parameters[0].TrimStart('/');
			string message = string.Join(" ", parameters.Skip(1));

			CommandsWithResponses.Add(identifier, message);
			SaveConfig();

			args.Player?.SendSuccessMessage($"Added new command: Identifier \"{identifier}\", Response \"{message}\".");
		}

		public static void ReloadMsgs(CommandArgs args)
		{
			try
			{
				ReloadConfig();
				args.Player.SendSuccessMessage("successfully reloaded config");
			}
			catch 
			{ 
				args.Player.SendErrorMessage("Something went wrong while reloading the config. Check 'commands.json' for any missed ',' after each line."); 
			}
		}

		// reload the config
		public static void ReloadConfig()
		{
			foreach (var command in Commands.ChatCommands.ToList())
			{
				if (CommandsWithResponses.ContainsKey(command.Name))
				{
					Commands.ChatCommands.RemoveAll(x => x.Name == command.Name);
				}
			}

			LoadConfig();
			PasteCommandList();
		}
	}
}
