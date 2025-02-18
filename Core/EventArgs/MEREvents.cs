using Exiled.API.Features;
using MapEditorReborn.Events.EventArgs;
using MEC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tycoon.Core.EventArgs
{
    public static class MEREvents
    {
        public static void OnDeletingObject(DeletingObjectEventArgs ev)
        {
            Log.Info(ev.Object.name);

            if (ev.Object.name == "Tycoon")
                ev.IsAllowed = false;
        }
    }
}
