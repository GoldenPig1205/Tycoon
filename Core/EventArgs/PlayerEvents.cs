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

namespace Tycoon.Core.EventArgs
{
    public static class PlayerEvents
    {
        public static IEnumerator<float> OnVerified(VerifiedEventArgs ev)
        {
            ev.Player.Role.Set(RoleTypeId.Tutorial);

            if (!AudioPlayers.ContainsKey(ev.Player))
            {
                AudioPlayer audioPlayer = AudioPlayer.CreateOrGet($"Player - {ev.Player.UserId}", condition: (hub) =>
                {
                    return hub == ev.Player.ReferenceHub;
                }
                , onIntialCreation: (p) =>
                {
                    p.transform.parent = ev.Player.GameObject.transform;

                    Speaker speaker = p.AddSpeaker("Main", isSpatial: false, minDistance: 0, maxDistance: 5000);

                    speaker.transform.parent = ev.Player.GameObject.transform;
                    speaker.transform.localPosition = Vector3.zero;
                });

                AudioPlayers.Add(ev.Player, audioPlayer);
            }

            while (!PlayerBases.ContainsKey(ev.Player))
            {
                if (Physics.Raycast(ev.Player.ReferenceHub.PlayerCameraReference.position + ev.Player.ReferenceHub.PlayerCameraReference.forward * 0.2f, ev.Player.ReferenceHub.PlayerCameraReference.forward, out RaycastHit hit, 1))
                {
                    if (hit.transform.name == "[C] Door")
                    {
                        int num = int.Parse(hit.transform.parent.parent.name);

                        PlayerBases.Add(ev.Player, num);
                        BaseDollars.Add(num, 0);
                        PlayerDollars.Add(ev.Player, 0);

                        DisableObject(GetBase(num).Find("Door"));

                        foreach (var name in new List<string>()
                        {
                            "Base Spawnpoint",
                            "Base Dollar",
                            "Button 1"
                        })
                        {
                            EnableObject(GetBase(num).Find(name));
                        }
                    }
                }

                yield return Timing.WaitForOneFrame;
            }
        }

        public static void OnLeft(LeftEventArgs ev)
        {
            if (PlayerBases.ContainsKey(ev.Player))
            {

                BaseDollars.Remove(PlayerBases[ev.Player]);
                PlayerBases.Remove(ev.Player);
                PlayerDollars.Remove(ev.Player);
            }
        }

        public static void OnSpawned(SpawnedEventArgs ev)
        {
            if (ev.Player.IsAlive)
            {
                ev.Player.Position = FirstSpawnPoint.position;
                ev.Player.EnableEffect(EffectType.FogControl, 1);
                ev.Player.EnableEffect(EffectType.SoundtrackMute, 1);
            }
        }

        public static IEnumerator<float> OnDied(DiedEventArgs ev)
        {
            for (int i=0; i<5; i++)
            {
                ev.Player.ShowHint($"{5 - i}초 뒤 부활합니다.");

                yield return Timing.WaitForSeconds(1);
            }

            ev.Player.Role.Set(RoleTypeId.Tutorial);

            Timing.CallDelayed(Timing.WaitForOneFrame, () =>
            {
                if (PlayerBases.ContainsKey(ev.Player))
                {
                    ev.Player.Position = GetBase(PlayerBases[ev.Player]).Find("[SP] Respawn").position;
                }
                else
                {
                    ev.Player.Position = FirstSpawnPoint.position;
                }
            });
        }
    }
}
