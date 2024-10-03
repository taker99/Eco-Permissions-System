using Eco.Gameplay.Players;
using Eco.Gameplay.Systems.Chat;
using Eco.Gameplay.Systems.Messaging.Chat.Commands;
using Eco.Shared.Localization;
using Eco.Systems.Permissions.Groups;

namespace Eco.Systems.Permissions.Permissions
{
    // Custom chat command processor assists in overriding SLG defined Auth levels and allows us to assign standard command to our own processing logic
    public class ESPCustomChatProcessor : ICommandProcessorHandler
    {
        private readonly static Func<ChatCommand, IChatClient, bool> commandProcessor;
        private readonly static Dictionary<string, ChatCommand>[] commandsByLanguage = new Dictionary<string, ChatCommand>[Enum.GetValues(typeof(SupportedLanguage)).OfType<SupportedLanguage>().Max(x => (int)x) + 1]; // mapping between parent command or alias to ChatCommand object

        public ESPCustomChatProcessor()
        {
            commandsByLanguage[(int)SupportedLanguage.English] = new Dictionary<string, ChatCommand>();
        }

        [CommandProcessor]
        public static bool ESPProcessCommand(ChatCommand command, IChatClient chatClient)
        {
            var level = chatClient.GetChatAuthLevel();
            var adapter = CommandGroupsManager.FindAdapter(Localizer.DoStr(command.Name));

            if (adapter == null)
            {
                chatClient.ErrorLocStr(string.Format(Plugin.appName + Localizer.DoStr("Command {0} not found"), command.Name));
                return false;
            }

            // if an admin or developer & we have not overridden this in our config return true;
            if ((level >= ChatAuthorizationLevel.Admin && CommandGroupsManager.Config.DefaultAdminBehaviour) || (command.AuthLevel == ChatAuthorizationLevel.User && CommandGroupsManager.Config.DefaultUserBehaviour))
            {
                // Check For Blacklisted commands
                if (chatClient is User invokingUsr && GroupsManager.API.CommandPermitted(invokingUsr, adapter))
                {
                    commandProcessor?.Invoke(command, chatClient);
                    return true;
                }
            }

            // check the users groups permissions permissions if the caller is a user
            if (chatClient is not User invokingUser || GroupsManager.API.UserPermitted(invokingUser, adapter))
            {
                commandProcessor?.Invoke(command, chatClient);
                return true;
            }

            // default behaviour is to deny if the state is unexpected
            chatClient.ErrorLocStr(string.Format(Plugin.appName + Localizer.DoStr("You are not authorized to use the command {0}"), command.Name));

            commandProcessor?.Invoke(command, chatClient);
            return false;
        }
    }
}