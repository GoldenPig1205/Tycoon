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
using static Tycoon.Core.EventArgs.PlayerEvents;
using Tycoon.Core.Functions;
using MEC;

namespace Tycoon.Core.Commands.RemoteAdminCommands.AdminFeatures
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class TycoonStart : ICommand
    {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            float radius = 100;
            int baseCount = 35;

            float.TryParse(arguments.At(0), out radius);
            int.TryParse(arguments.At(1), out baseCount);
            
            Base.StartGame(radius, baseCount);
            AutoStart = true;

            if (Round.IsLobby)
                Round.Start();

            foreach (var player in Player.List)
                Timing.RunCoroutine(Verified(player));

            response = "[Tycoon] Starting Roblox Tycoon Simulator..";

            return true;
        }

        public string Command { get; } = "tycoon";

        public string[] Aliases { get; } = { "sty"};

        public string Description { get; } = "tycoon <radius> <baseCount>ㅣStart the Roblox Tycoon Simulator.";

        public bool SanitizeResponse { get; } = true;
    }
}