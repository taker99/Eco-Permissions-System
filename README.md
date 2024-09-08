# Eco-Permissions-System
The EM Permissions System Pulled From EM Framework as a standalone mod

### Groups System

What do groups offer you as a server owner?

Groups offer you the chance to assign users to a newly made group and allows you and modders alike to Assign configuration settings per mod or even allow the group to use a specific command they normally couldn't.

Using the groups system is pretty easy as a server owner, here is a list of all the commands and how to use them as a server owner.

            
```ts
 // Used to create a new group
 Command:     /groups addgroup groupname
 Shortcut:    /grp-add groupname
 // used to delete a created group
 Command:     /groups deletegroup groupname
 Shortcut:    /grp-del groupname
 // will list all groups you have on the server
 Command:     /groups listgroups
 Shortcut:    /grp-list
 // Will list all the permissions the named group has
 Command:     /groups grouppermissions groupname
 Shortcut:    /grp-perms groupname
 // will add a user to a selected group or will create the group then add the user too it
 Command:     /groups addusertogroup username, groupname
 Shortcut:    /grp-adduser username
 // will remove a user from that group, if the group doesn't exist it will tell you
 Command:     /groups removeuserfromgroup username, groupname
 Shortcut:    /grp-remuser username
 //forces the groups system to save everything just incase a save fails
 Command:     /groups forcesave
 Shortcut:    /grp-fs
                
```
    
All commands that use a username are case sensitive so make sure your naming is correct otherwise it will say that user doesn't exist

Config File Structure - If you are not comfortable editing this file then please just use the in game commands - Located in Configs/Mods/EM/Groups/ElixrMods-GroupsData.json

```json
 {
     "Groups": [ //each group added
         {
         "GroupName": "admin",
         "GroupUsers": [
             {
                 "Name": "User",
                 "SlgID": "slg",
                 "SteamID": ""
             }
         ],
         "Permissions": [
             {
                 "$type": "Eco.EM.Homes.HomeConfig, em-home",
                 "CalorieCost": 500,
                 "MaxTeleports": 5,
                 "MaxHomeCount": 5
             }
         ]
     },
     {
         "GroupName": "default",
         "GroupUsers": [],
         "Permissions": [
              {
                 "$type": "Eco.EM.Homes.HomeConfig, em-home",
                 "CalorieCost": 500,
                 "MaxTeleports": 5,
                 "MaxHomeCount": 1
              }
         ]
       }
     ],
     "AllUsers": [ // All users that login to the server
         {
             "Name": "User",
             "SlgID": "slg",
             "SteamID": ""
         }
     ]
 }
```                
---

### Permissions System

The Permissions Part of the permissions system is what we use for granting groups use of commands and more.

How to use as a server owner:

            
```ts
 // Used to give groups permission to use a command
 Command:     /CommandPermissions grant command, groupname
 Shortcut:    /grant-command command, groupname

 //Used To Blacklist a command for a group
 Command:     /CommandPermissions blacklistcommand command, groupname
 Shortcut:    /blacklist-command command, groupname

 //Used to remove a blacklisted command from a group
 Command:     /CommandPermissions RemBlacklistcommand command, groupname
 Shortcut:    /remove-blacklist command, groupname

 // Used to revoke a groups permission to a command
 Command:     /CommandPermissions revoke command, groupname
 Shortcut:    /revoke-command command, groupname

 //this is used to allow Admins or users default access to their default commands
 Command:     /CommandPermissions setbehaviour admin/user, true/false
 Shortcut:    /behaviour-command admin/user, true/false
```                

    
The "command" In the commands is any command in the eco game, regardless if its registered by a mod or a core command!

If it exists in Eco we can get the command.

You can now use shortcuts to assign commands to a group!

Here are some examples of using the commands:

                
```ts
//Using the proper command
 /CommandPermissions grant fly, VIP
 /grant-command fly, VIP
 //this gives the group VIP access to use the /fly command

//Using a shortcut
 /CommandPermissions grant fgive, VIP
 /grant-command fgive, VIP
 //this gives the group VIP access to use the /fgive or /force give command

 /CommandPermissions revoke fly, VIP 
 /revoke-command fly, VIP
 // Will take away the ability for the VIP group to use the /fly command

 /CommandPermissions setbehaviour admin, false 
 /behaviour-command admin, false
 // will mean any user that was made an admin will not have access to admin commands anymore
 // and you will need to assign then use of admin commands via a group.

 /CommandPermissions setbehaviour user, false 
 /behaviour-command user, false
 // will mean any user that is not in a group or an admin, will not be able to use any command witho
