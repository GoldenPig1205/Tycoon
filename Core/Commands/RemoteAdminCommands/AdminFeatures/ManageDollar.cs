using System;
using System.Collections.Generic;
using System.Linq;
using CommandSystem;
using Exiled.API.Extensions;
using Exiled.API.Features;
using MapEditorReborn.API.Features;
using MapEditorReborn.API.Features.Serializable;
using Mirror;
using MultiBroadcast.API;
using PlayerRoles;
using UnityEngine;
using static Tycoon.Core.Variables.Base;

namespace Tycoon.Core.Commands.RemoteAdminCommands.AdminFeatures
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class AddDollar : ICommand
    {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player = Player.Get(arguments.At(0));

            PlayerDollars[player] += int.Parse(arguments.At(1));

            response = "Complete!";

            return true;
        }

        public string Command { get; } = "달러";

        public string[] Aliases { get; } = { };

        public string Description { get; } = "달러 지급용";

        public bool SanitizeResponse { get; } = true;
    }
}