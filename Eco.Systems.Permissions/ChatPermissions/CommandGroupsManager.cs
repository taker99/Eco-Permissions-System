using Eco.Core.Plugins.Interfaces;
using Eco.Systems.Permissions.FileManager;
using Eco.Shared.Utils;
using Eco.Shared.Localization;
using Eco.Gameplay.Systems.Messaging.Chat.Commands;
using Eco.Gameplay.Systems.Messaging.Chat;
using Eco.Core.Utils;
using Eco.Core;
using Eco.WebServer;
using Eco.World;
using Eco.Plugins.Networking;
using Eco.Shared.Networking;
using Eco.Shared.Logging;

namespace Eco.Systems.Permissions.Permissions
{
    [PriorityAfter([typeof(NetworkManager)])]
    public class CommandGroupsManager : Singleton<CommandGroupsManager>, IModKitPlugin, IInitializablePlugin
    {
        // The currently internally cached set of commands.
        private IEnumerable<ChatCommand> _commands;
        private static HashSet<ChatCommandAdapter>? Commands;
        private static ChatCommandService ChatCommandService = new();
        private const string _configFile = "CommandGroupsConfig.json";
        internal static string protectorGroup = "command_admin";
        private static string _subPath = Path.DirectorySeparatorChar + "ESP" + Path.DirectorySeparatorChar +"CommandGroups";

        public static CommandGroupsConfig? Config { get; private set; }

        public CommandGroupsManager()
        {
            Config = LoadConfig();

            if (!File.Exists(Plugin.SaveLocation + _subPath + _configFile))
                SaveConfig();
        }

        /// <summary>
        /// Gets a full list of all commands in game and caches them to reduce requests from the mod
        /// </summary>
        public void GetCommandsAndSet()
        {
            _commands = LoadCommandsInternal();
            Commands = new HashSet<ChatCommandAdapter>();


            CreateAdapters();
        }

        /// <summary>
        /// Look for an appropriate command by its command name, 
        /// here we check if its a full command name or a shortcut command.
        /// </summary>
        /// <param name="dirtyCommand"></param>
        /// <returns>Command or null</returns>
        public static ChatCommandAdapter FindAdapter(string dirtyCommand)
        {
            var cleanCommand = Utils.StringUtils.Sanitize(dirtyCommand);

            if (Commands?.FirstOrDefault(adpt => adpt.Identifier == cleanCommand) != null)
                return Commands.FirstOrDefault(adpt => adpt.Identifier.Equals(cleanCommand));

            else
                return Commands.FirstOrDefault(adpt => adpt.ShortCut.ToLower() == cleanCommand);
        }

        /// <summary>
        /// Here we attempt to find any and all children commands or "sub commands" as noted in eco commands
        /// This is used to grant all sub commands of a parent command to a group without needing to do the entire list
        /// </summary>
        /// <param name="dirtyCommand"></param>
        /// <returns>Command List or null</returns>
        public static ChatCommandAdapter[] FindAdapterAndChildren(string dirtyCommand)
        {
            var cleanCommand = Utils.StringUtils.Sanitize(dirtyCommand);

            IEnumerable<ChatCommand> commands = ChatManager.Obj.ChatCommandService.GetAllCommands();

            ChatCommandAdapter[]? Results = null;

            //Cycle Through the list of commands and find our desired command
            foreach(var c in commands)
            {
                if (c.Name == cleanCommand)
                {
                    //cycle through the command and check for sub commands and add to the results
                    if (c.HasSubCommands)
                    {
                        Results?.AddNotNull(Commands?.FirstOrDefault(adpt => adpt.Identifier == c.Name));

                        foreach (var sub in c.SubCommands)
                        {
                            Results?.AddNotNull(Commands?.FirstOrDefault(adpt => adpt.Identifier == sub.Name));
                        }

                    }
                    else break;
                }
                else break;
            }
            return Results;
        }

        // Permission system Server GUI Status
        public string GetStatus()
        {
            return "Chat Command Permissions Active";
        }

        public override string ToString()
        {
            return Localizer.DoStr("ESP - Permissions");
        }

        public void Initialize(TimedTask timer)
        {
            GetCommandsAndSet();
        }

        /// <summary>
        /// Turns a command into an Adaptor for internal use
        /// </summary>
        private void CreateAdapters()
        {
            _commands?.ForEach(c =>
            {
                if (!Commands.Any(adpt => adpt.Identifier == c.Name))
                    Commands?.Add(new ChatCommandAdapter(c));
            });
        }

        /// Internally cache all the commands. so we can save them for later.
        private IEnumerable<ChatCommand> LoadCommandsInternal()
        {
            IEnumerable<ChatCommand> commands = ChatManager.Obj.GetAllCommands();

            return commands;
        }

        private CommandGroupsConfig LoadConfig()
        {
            return FileManager<CommandGroupsConfig>.ReadFromFile(Plugin.SaveLocation + _subPath, _configFile);
        }

        internal static void SaveConfig()
        {
            FileManager<CommandGroupsConfig>.WriteToFile(Config, Plugin.SaveLocation + _subPath, _configFile);
        }

        public string GetCategory() => "ESP";
    }
}
