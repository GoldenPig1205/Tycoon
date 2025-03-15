using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using MapEditorReborn.API.Features.Objects;
using MEC;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Tycoon.Core.Variables.Base;
using static Tycoon.Core.Functions.Base;
using System.Diagnostics.Eventing.Reader;
using Exiled.Events.EventArgs.Map;
using MapEditorReborn.Events.EventArgs;

namespace Tycoon.Core.EventArgs
{
    public static class MEREvents
    {
        public static void OnDeletingObject(DeletingObjectEventArgs ev)
        {
            if (ev.Object.IsSchematicBlock)
                ev.IsAllowed = false;
        }
    }
}
