using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using Tycoon.Core.Configs;
using static Tycoon.Core.EventArgs.MEREvents;
using static Tycoon.Core.EventArgs.ServerEvents;
using static Tycoon.Core.EventArgs.MapEvents;
using static Tycoon.Core.EventArgs.PlayerEvents;
using static Tycoon.Core.Variables.Base;

namespace Tycoon
{
    public class Main : Plugin<Config>
    {
        public static Main Instance;

        public override string Name => "Tycoon";
        public override string Author => "GoldenPig1205";
        public override Version Version { get; } = new(1, 0, 0);
        public override Version RequiredExiledVersion { get; } = new(1, 2, 0, 5);

        public override void OnEnabled()
        {
            base.OnEnabled();
            Instance = this;

            AutoStart = Config.AutoStart;

            MapEditorReborn.Events.Handlers.MapEditorObject.DeletingObject += OnDeletingObject;

            Exiled.Events.Handlers.Server.WaitingForPlayers += OnWaitingForPlayers;

            Exiled.Events.Handlers.Player.Verified += OnVerified;
            Exiled.Events.Handlers.Player.Left += OnLeft;
            Exiled.Events.Handlers.Player.Spawned += OnSpawned;
            Exiled.Events.Handlers.Player.Hurting += OnHurting;
            Exiled.Events.Handlers.Player.Died += OnDied;
            Exiled.Events.Handlers.Player.TogglingNoClip += OnTogglingNoClip;
            Exiled.Events.Handlers.Player.ChangingGroup += OnChangingGroup;
            Exiled.Events.Handlers.Player.ChangedEmotion += OnChangedEmotion;
            Exiled.Events.Handlers.Player.PickingUpItem += OnPickingUpItem;
        }

        public override void OnDisabled()
        {
            MapEditorReborn.Events.Handlers.MapEditorObject.DeletingObject -= OnDeletingObject;

            Exiled.Events.Handlers.Server.WaitingForPlayers -= OnWaitingForPlayers;

            Exiled.Events.Handlers.Player.Verified -= OnVerified;
            Exiled.Events.Handlers.Player.Left -= OnLeft;
            Exiled.Events.Handlers.Player.Spawned -= OnSpawned;
            Exiled.Events.Handlers.Player.Hurting -= OnHurting;
            Exiled.Events.Handlers.Player.Died -= OnDied;
            Exiled.Events.Handlers.Player.TogglingNoClip -= OnTogglingNoClip;
            Exiled.Events.Handlers.Player.ChangingGroup -= OnChangingGroup;
            Exiled.Events.Handlers.Player.ChangedEmotion -= OnChangedEmotion;
            Exiled.Events.Handlers.Player.PickingUpItem -= OnPickingUpItem;

            Instance = null;
            base.OnDisabled();
        }
    }
}
