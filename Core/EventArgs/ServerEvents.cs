using Exiled.API.Features;
using InventorySystem.Configs;
using MEC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Tycoon.Core.Variables.Base;
using static Tycoon.Core.Functions.Base;
using static Tycoon.Core.IEnumerators.Base;
using static Tycoon.Core.Extensions.Base;
using MultiBroadcast.API;
using Exiled.API.Extensions;
using MapEditorReborn.API.Features.Objects;
using MapEditorReborn.API.Features;

namespace Tycoon.Core.EventArgs
{
    public static class ServerEvents
    {
        public static IEnumerator<float> OnWaitingForPlayers()
        {
            yield return Timing.WaitForSeconds(1);

            if (AutoStart)
                StartGame();
        }
    }
}
