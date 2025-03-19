using Eco.Gameplay.Players;
using Eco.Gameplay.Systems.Chat;
using Eco.Shared.Utils;
using System.Linq;
using System.Text;
using Eco.Shared.Localization;
using Eco.Gameplay.Systems.Messaging.Chat.Commands;
using Eco.Systems.Permissions.Utils;

namespace Eco.Systems.Permissions.Groups
{
    [ChatCommandHandler]
    public class GroupsCommands
    {
        [ChatCommand("Groups System Commands", ChatAuthorizationLevel.Admin)]
        public static void Groups(User user) { }

        [ChatSubCommand("Groups", "Used to Create a New Group", "grp-add", ChatAuthorizationLevel.Admin)]
        public static void AddGroup(IChatClient user, string groupName)
        {
            Group group = GroupsManager.Data.GetOrAddGroup(groupName, true);
            user.MsgLocStr($"Group {groupName} was created");

            GroupsManager.API.SaveData();
        }

        [ChatSubCommand("Groups", "Used to Delete an Existing Group", "grp-del", ChatAuthorizationLevel.Admin)]
        public static void DeleteGroup(IChatClient user, string groupName)
        {
            var maingroups = StringUtils.Sanitize(groupName);
            if (maingroups == "admin" || maingroups == "default")
            {
                user.ErrorLocStr($"Group {groupName} is a default group and cannot be deleted");
                return;
            }

            if (GroupsManager.Data.DeleteGroup(groupName))
                user.ErrorLocStr($"Group {groupName} was deleted");
            else
                user.ErrorLocStr($"Group {groupName} was unable to be found.");

            GroupsManager.API.SaveData();
        }

        [ChatSubCommand("Groups", "Used to print a list of groups to the chat window", "grp-list", ChatAuthorizationLevel.Admin)]
        public static void ListGroups(IChatClient client)
        {
            StringBuilder sb = new();
            var groups = GroupsManager.Data.Groups;
            sb.Append(string.Format("\n"));
            groups.ForEach(g =>
            {
                sb.Append(g.GroupName);

                if (g != groups.Last())
                    sb.Append(string.Format(", "));
                else
                    sb.Append(string.Format("\n"));
            });

            var user = UserManager.FindUser(client.Name);
            if (user is null)
            {
                client.ErrorLocStr($"Groups: {sb}");
            }
            else
                user.TempServerMessage(Localizer.DoStr(sb.ToString()));
        }
        
        [ChatSubCommand("Groups", "Used to print a list of groups for rcon use", "rcongrp-list", ChatAuthorizationLevel.Admin)]
        public static void RconGroupPermissions(IChatClient client, string groupName)
        {
        }
        
        [ChatSubCommand("Groups", "Used to print a list permissions assigned to a group", "grp-perms", ChatAuthorizationLevel.Admin)]
        public static void GroupPermissions(IChatClient client, string groupName)
        {
            Group group = GroupsManager.Data.GetOrAddGroup(groupName, true);

            StringBuilder sb = new();
            group.Permissions.ForEach(perm =>
            {
                sb.Append(perm.Identifier);

                if (perm != group.Permissions.Last())
                    sb.Append(", ");
            });

            var user = UserManager.FindUser(client.Name);
            if(user is null)
            {
                client.ErrorLocStr($"Permissions for Group: {group.GroupName}: {sb}");
            }
            else
                user.TempServerMessage(Localizer.DoStr(string.Format("\nGroup {0}:\nPermissions: {1}", group.GroupName, sb.ToString())));
        }

        [ChatSubCommand("Groups", "Used to add a user to a group", "grp-adduser", ChatAuthorizationLevel.Admin)]
        public static void AddUserToGroup(IChatClient client, User user, string groupName)
        {
            Group group = GroupsManager.Data.GetOrAddGroup(groupName, true);
            
            if (group.AddUser(user))
                client.MsgLocStr($"User {user.Name} was added to Group {group.GroupName}");
            else
                client.ErrorLocStr($"User {user.Name} Already Exists in Group: {group.GroupName}");
            
            GroupsManager.API.SaveData();
        }

        [ChatSubCommand("Groups", "Used to remove a user from a group", "grp-remuser", ChatAuthorizationLevel.Admin)]
        public static void RemoveUserFromGroup(IChatClient client, User user, string groupName)
        {
            Group group = GroupsManager.Data.GetOrAddGroup(groupName, false);
            if (group == null)
            {
                client.ErrorLocStr($"Group {groupName} was unable to be found.");
            }

            if (group.RemoveUser(user))
                client.MsgLocStr($"User {user.Name} was removed from Group {group.GroupName}");
            else
                client.ErrorLocStr($"User {user.Name} was unable to be found in Group: {group.GroupName}");

            GroupsManager.API.SaveData();
        }

        [ChatSubCommand("Groups", "Used to force save the groups just incase it didn't auto save.", "grp-fs", ChatAuthorizationLevel.Admin)]
        public static void ForceSave(User user)
        {
            GroupsManager.API.SaveData();
            user.MsgLocStr("Save Complete");
        }
    }
}
