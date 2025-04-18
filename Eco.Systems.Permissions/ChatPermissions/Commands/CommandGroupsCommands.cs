﻿using Eco.Systems.Permissions.Groups;
using Eco.Gameplay.Players;
using Eco.Gameplay.Systems.Chat;
using Eco.Shared.Localization;
using Eco.Gameplay.Systems.Messaging.Chat.Commands;
using Eco.Systems.Permissions.Utils;
using Eco.Gameplay.Objects;
using Eco.Shared.Utils;
using System;
using Eco.Core.Plugins;
using Eco.Gameplay.Systems;

namespace Eco.Systems.Permissions.Permissions
{

    // Chat commands for interfacing with the Permisions Manager.
    [ChatCommandHandler]
    public class CommandGroupsCommands
    {
        [ChatCommand("Permissions Command For the Permissions System", ChatAuthorizationLevel.Admin)]
        public static void CommandPermissions(User user) { }

        // Command Groups
        [ChatSubCommand("CommandPermissions", "Used to Grant Permissions to a Group", "grant-command", ChatAuthorizationLevel.Admin)]
        public static void Grant(IChatClient client, string command, string groupName)
        {
            Group group = GroupsManager.API.GetGroup(groupName);
            ChatCommandAdapter adapter = CommandGroupsManager.FindAdapter(command);
            try
            {
                if (adapter == null)
                {
                    client.ErrorLocStr(Plugin.appName + string.Format(Localizer.DoStr("Command {0} was unable to be found in the commands set"),command));
                    return;
                }

                if (!group.AddPermission(adapter))
                {
                    client.ErrorLocStr(Plugin.appName + string.Format(Localizer.DoStr("Command {0} was unable to be added to Group {1} permissions, its identifier already exists"), adapter.Identifier, group.GroupName));
                    return;
                }

                group.AddPermission(adapter);
                client.MsgLocStr(Plugin.appName + string.Format(Localizer.DoStr("Command {0} was added to Group {1} permissions"), adapter.Identifier, group.GroupName));
                GroupsManager.API.SaveData();
            }
            catch
            {
                client.ErrorLocStr(Plugin.appName + string.Format(Localizer.DoStr("Command {0} was unable to be added to Group {1}, please create the group first"), adapter.Identifier, groupName));
            }
        }

        [ChatSubCommand("CommandPermissions", "Used To Grant all Sub Commands to a group using the Parent Command", "grant-all", ChatAuthorizationLevel.Admin)]
        public static void GrantAll(IChatClient client, string command, string groupName)
        {
            Group group = GroupsManager.API.GetGroup(groupName);
            if(group == null)
            {
                client.MsgLocStr(Plugin.appName + string.Format(Localizer.DoStr("Was unable to find Group {0}, please create the group first"), groupName));
                return;
            }
            ChatCommandAdapter[]? adapters = null;
            try
            {
                adapters = CommandGroupsManager.FindAdapterAndChildren(command);

            }
            catch
            {
                client.MsgLocStr(Plugin.appName + string.Format(Localizer.DoStr("Command {0} was unable to be added to Group {1}, potential issue with the commands?"), command, groupName));
                return;
            }

            if (!adapters.AnyNotNull())
            {
                client.ErrorLocStr(Plugin.appName + string.Format(Localizer.DoStr("Command {0} was unable to be found in the commands set"), command));
                return;
            }
            foreach (var adapter in adapters)
            {
                try
                {
                    if (!group.AddPermission(adapter))
                    {
                        client.ErrorLocStr(Plugin.appName + string.Format(Localizer.DoStr("Command {0} was unable to be added to Group {1} permissions, its identifier already exists"), adapter.Identifier, group.GroupName));
                        continue;
                    }

                    group.AddPermission(adapter);
                    client.MsgLocStr(Plugin.appName + string.Format(Localizer.DoStr("Command {0} was added to Group {1} permissions"), adapter.Identifier, group.GroupName));

                }
                catch
                {
                    client.MsgLocStr(Plugin.appName + string.Format(Localizer.DoStr("Command {0} was unable to be added to Group {1}, potential issue with the commands?"), adapter.Identifier, groupName));
                }
            }
            GroupsManager.API.SaveData();
        }

        [ChatSubCommand("CommandPermissions", "Used to Remove Permissions from a Group", "revoke-command", ChatAuthorizationLevel.Admin)]
        public static void Revoke(IChatClient client, string command, string groupName)
        {
            Group group = GroupsManager.API.GetGroup(groupName);
            ChatCommandAdapter adapter = CommandGroupsManager.FindAdapter(command);

            if (adapter == null)
            {
                client.ErrorLocStr(Plugin.appName + string.Format(Localizer.DoStr("Command {0} was unable to be found in the commands set"), command));
                return;
            }

            group.DeletePermission(adapter);
            client.MsgLocStr(Plugin.appName + string.Format(Localizer.DoStr("Command {0} was removed from Group {1} permissions"), adapter.Identifier, group.GroupName));
            GroupsManager.API.SaveData();
        }

        [ChatSubCommand("CommandPermissions", "Used to set the default Admin and User behaviour. Use /Commandpermissions setbehaviour {admin/user},{true/false}", "behaviour-command", ChatAuthorizationLevel.Admin)]
        public static void SetBehaviour(User user, string behaviour, bool toggle)
        {
            behaviour = StringUtils.Sanitize(behaviour);
            if (behaviour == "admin")
            {
                // protection from jamming everyone from commands
                if (!toggle)
                {
                    Group group = GroupsManager.API.GetGroup(CommandGroupsManager.protectorGroup, true);
                    Grant(user, "setbehaviour", CommandGroupsManager.protectorGroup);
                    group.AddUser(user);
                }

                CommandGroupsManager.Config.DefaultAdminBehaviour = toggle;
            }
            else if (behaviour == "user")
            {
                CommandGroupsManager.Config.DefaultUserBehaviour = toggle;
            }
            else
            {
                user.ErrorLocStr(Plugin.appName + string.Format(Localizer.DoStr("{0} is not a valid toggle option"), behaviour));
                return;
            }

            user.ErrorLocStr(Plugin.appName + string.Format(Localizer.DoStr("{0} custom behaviour was set to {1}"), behaviour, toggle));
            CommandGroupsManager.SaveConfig();
            GroupsManager.API.SaveData();
        }

        [ChatSubCommand("CommandPermissions", "Used to Blacklist a command for a group", "blacklist-command", ChatAuthorizationLevel.Admin)]
        public static void BlacklistCommand(IChatClient client, string command, string groupName)
        {
            Group group = GroupsManager.API.GetGroup(groupName);
            ChatCommandAdapter adapter = CommandGroupsManager.FindAdapter(command);
            try
            {
                if (adapter == null)
                {
                    client.ErrorLocStr(Plugin.appName + string.Format(Localizer.DoStr("Command {0} was unable to be found in the commands set"), command));
                    return;
                }

                if (!group.AddPermission(adapter))
                {
                    group.DeletePermission(adapter);
                }
                adapter.BlackListed = true;
                group.BlacklistCommand(adapter);
                client.MsgLocStr(Plugin.appName + string.Format(Localizer.DoStr("Command {0} was Blacklisted for Group {1}"), adapter.Identifier, group.GroupName));
                GroupsManager.API.SaveData();
            }
            catch
            {
                client.ErrorLocStr(Plugin.appName + string.Format(Localizer.DoStr("Command {0} was unable to be added to Group {1}, please create the group first"), adapter.Identifier, groupName));
            }
        }

        [ChatSubCommand("CommandPermissions", "Remove a blacklisted command from a group", "remove-blacklist", ChatAuthorizationLevel.Admin)]
        public static void RemBlacklistCommand(IChatClient client, string command, string groupName)
        {
            Revoke(client, command, groupName);
        }

        [ChatSubCommand("CommandPermissions", "Opens up a window to change your configurations", "eps-config", ChatAuthorizationLevel.Admin)]
        public static void Config(User user)
        {
                var config = new ModConfig();

            ViewEditorUtils.PopupUserEditValue(user, typeof(EPSConfigUI), Localizer.DoStr("Trade Assistant Configuration"), config.ToUI(), null, OnSubmit);

            void OnSubmit(object entry)
            {
                if (entry is EPSConfigUI uiConfig)
                    config.UpdateFromUI(uiConfig);
                StorageManager.Obj.MarkDirty(EPSData.Obj);
            }
        }
    }
}
