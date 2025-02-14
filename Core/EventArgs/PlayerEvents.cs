using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Tycoon.Core.Variables.Base;

namespace Tycoon.Core.EventArgs
{
    public static class PlayerEvents
    {
        public static void OnSpawned(SpawnedEventArgs ev)
        {
            if (ev.Player.IsAlive)
            {
                ev.Player.Role.Set(RoleTypeId.Tutorial);
                ev.Player.Position = FirstSpawnPoint.position;
                ev.Player.EnableEffect(EffectType.FogControl, 1);
                ev.Player.EnableEffect(EffectType.SoundtrackMute, 1);
            }
        }
    }
}
