using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using MapEditorReborn.API.Features.Objects;
using MEC;
using PlayerRoles;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Tycoon.Core.Variables.Base;
using static Tycoon.Core.Functions.Base;
using System.Diagnostics.Eventing.Reader;
using InventorySystem.Items;
using MultiBroadcast.API;
using PlayerRoles.FirstPersonControl.Thirdperson.Subcontrollers;
using Exiled.API.Features.DamageHandlers;
using Exiled.API.Features.Pickups;

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
                        BaseRasers.Add(num, true);
                        BaseDollars.Add(num, 0);
                        PlayerDollars.Add(ev.Player, 0);

                        DisableObject(GetBase(num).Find("Door"));

                        foreach (var name in new List<string>()
                        {
                            "Base Spawnpoint",
                            "Base Dollar",
                            "Button 1",
                            "Button 9",
                            "Button 34"
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
                ResetBase(PlayerBases[ev.Player]);

                BaseDollars.Remove(PlayerBases[ev.Player]);
                BaseRasers.Remove(PlayerBases[ev.Player]);
                PlayerBases.Remove(ev.Player);
                PlayerDollars.Remove(ev.Player);
            }
        }

        public static void OnSpawned(SpawnedEventArgs ev)
        {
            if (ev.Player.IsAlive)
            {
                GodModePlayers.Add(ev.Player);

                Timing.CallDelayed(5, () =>
                {
                    if (GodModePlayers.Contains(ev.Player))
                        GodModePlayers.Remove(ev.Player);
                });

                ev.Player.Position = FirstSpawnPoint.position;
                ev.Player.EnableEffect(EffectType.FogControl, 1);
                ev.Player.EnableEffect(EffectType.SoundtrackMute, 1);
                ev.Player.EnableEffect(EffectType.SilentWalk, 3);
            }
        }

        public static void OnHurting(HurtingEventArgs ev)
        {
            if (ev.Attacker == null)
                return;

            if (!ev.Attacker.IsNPC && GodModePlayers.Contains(ev.Player))
                ev.IsAllowed = false;
        }

        public static IEnumerator<float> OnDied(DiedEventArgs ev)
        {
            if (ev.Attacker != null)
            {
                if (PlayerBases.ContainsKey(ev.Attacker) && PlayerBases.ContainsKey(ev.Player))
                {
                    AudioPlayers[ev.Attacker].AddClip("Coin");

                    PlayerDollars[ev.Attacker] += Random.Range(1, Random.Range(10, Random.Range(30, 100)));
                }
            }

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
                    ev.Player.Position = GetBase(PlayerBases[ev.Player]).GetChild(0).GetChild(1).position;
                }
                else
                {
                    ev.Player.Position = FirstSpawnPoint.position;
                }
            });
        }

        public static void OnTogglingNoClip(TogglingNoClipEventArgs ev)
        {
            if (Physics.Raycast(ev.Player.ReferenceHub.PlayerCameraReference.position + ev.Player.ReferenceHub.PlayerCameraReference.forward * 0.2f, ev.Player.ReferenceHub.PlayerCameraReference.forward, out RaycastHit hit, 3))
            {
                if (hit.transform.name.StartsWith("Button"))
                {
                    string option = hit.transform.name.Split('/')[1];

                    if (option == "Drop")
                    {
                        if (!DropperCooldowns.Contains(ev.Player))
                        {
                            DropperCooldowns.Add(ev.Player);

                            Timing.CallDelayed(0.4f, () =>
                            {
                                DropperCooldowns.Remove(ev.Player);
                            });

                            DropProduct(int.Parse(hit.transform.parent.parent.name), hit.transform.parent.GetChild(0).position);
                        }
                    }
                    else if (option == "Raser Toggle")
                    {
                        int baseID = int.Parse(hit.transform.parent.parent.name);

                        if (baseID == PlayerBases[ev.Player])
                        {
                            if (BaseRasers[baseID])
                                BaseRasers[baseID] = false;

                            else
                                BaseRasers[baseID] = true;

                            foreach (Transform raser in hit.transform.parent)
                            {
                                if (raser.name == "Raser")
                                {
                                    if (!BaseRasers[baseID])
                                        raser.GetComponent<PrimitiveObject>().Primitive.Color = new Color(0, 1, 0, 0.5f);

                                    else
                                        raser.GetComponent<PrimitiveObject>().Primitive.Color = new Color(1, 0, 0, 0.5f);
                                }
                            }
                        }
                    }
                }
                else if (hit.transform.name.StartsWith("Item"))
                {
                    int itemID = int.Parse(hit.transform.name.Split('/')[1]);
                    ItemType itemType = (ItemType)itemID;

                    if (!ItemCooldowns.Contains(ev.Player))
                    {
                        ItemCooldowns.Add(ev.Player);

                        Timing.CallDelayed(1f, () =>
                        {
                            ItemCooldowns.Remove(ev.Player);
                        });

                        if (itemID == 1205)
                        {
                            if (PlayerDollars[ev.Player] >= 20000)
                            {
                                PlayerDollars[ev.Player] -= 20000;

                                AudioPlayers[ev.Player].AddClip("Cash Sound", volume: 2);

                                ev.Player.AddItem((ItemType)Random.Range(0, 56));
                            }
                            else
                            {
                                AudioPlayers[ev.Player].AddClip("wrong");
                            }
                        }
                        else
                        {
                            if (itemType.GetName().Contains("Armor"))
                            {
                                bool hasArmor = ev.Player.Items.Any(item => item.Type.GetName().Contains("Armor"));

                                if (hasArmor)
                                {
                                    AudioPlayers[ev.Player].AddClip("wrong");
                                }
                                else
                                {
                                    AudioPlayers[ev.Player].AddClip("Pick-Up");

                                    ev.Player.AddItem(itemType);
                                }
                            }
                            else
                            {
                                AudioPlayers[ev.Player].AddClip("Pick-Up");

                                ev.Player.AddItem(itemType);
                            }
                        }
                    }
                }
            }

            if (ev.Player.IsHuman && !ev.Player.IsCuffed)
            {
                if (TryGetLookPlayer(ev.Player, 2f, out Player player, out RaycastHit? hit1))
                {
                    if (ev.Player != player && !MeleeCooldowns.Contains(ev.Player) && !GodModePlayers.Contains(player))
                    {
                        float damageCalcu(string pos)
                        {
                            switch (pos)
                            {
                                case "Head":
                                    return 24.1f;

                                case "Chest":
                                    return 14f;

                                default:
                                    return 12.5f;
                            }
                        }

                        float damage = damageCalcu(hit1.Value.transform.name);

                        ev.Player.ShowHitMarker(damage / 14);
                        player.Hurt(ev.Player, damage, DamageType.Custom, new DamageHandlerBase.CassieAnnouncement("") { Announcement = null, SubtitleParts = null }, "무지성으로 뚜드려 맞았습니다.");

                        MeleeCooldowns.Add(ev.Player);

                        Timing.CallDelayed(1, () =>
                        {
                            MeleeCooldowns.Remove(ev.Player);
                        });
                    }
                }
            }
        }

        public static void OnChangedEmotion(ChangedEmotionEventArgs ev)
        {
            if (!EmotionCooldowns.Contains(ev.Player))
            {
                EmotionCooldowns.Add(ev.Player);

                EmotionPresetType type = ev.EmotionPresetType;

                if (type == EmotionPresetType.Neutral)
                    return;

                string emotion()
                {
                    if (type == EmotionPresetType.Happy)
                        return "행복한 표정을 짓고 있습니다";

                    else if (type == EmotionPresetType.AwkwardSmile)
                        return "뒤틀린 미소를 짓고 있습니다";

                    else if (type == EmotionPresetType.Scared)
                        return "두려운 표정을 짓고 있습니다";

                    else if (type == EmotionPresetType.Angry)
                        return "화가난 표정을 짓고 있습니다";

                    else if (type == EmotionPresetType.Chad)
                        return "꼭 채드처럼 보이는군요";

                    else
                        return "꼭 오우거같이 보이는군요";
                }

                foreach (var player in Player.List.Where(x => x.IsDead || Vector3.Distance(x.Position, ev.Player.Position) < 11))
                    player.AddBroadcast(5, $"<size=20>{BadgeFormat(ev.Player)}<color={ev.Player.Role.Color.ToHex()}>{ev.Player.DisplayNickname}</color>(은)는 {emotion()}.</size>");
            }
        }

        public static void OnPickingUpItem(PickingUpItemEventArgs ev)
        {
            if (ev.Pickup.Type == ItemType.Coin)
            {
                ev.Pickup.Destroy();
                ev.IsAllowed = false;

                AudioPlayers[ev.Player].AddClip("Coin");

                PlayerDollars[ev.Player] += Random.Range(1, Random.Range(10, Random.Range(30, 100)));
            }
        }
    }
}
