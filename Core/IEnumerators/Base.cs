using Exiled.API.Extensions;
using Exiled.API.Features;
using MEC;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Tycoon.Core.Variables.Base;
using static Tycoon.Core.Functions.Base;
using InventorySystem.Items;
using Exiled.API.Features.Pickups;
using Exiled.API.Features.Items;
using PluginAPI.Events;
using PlayerRoles.FirstPersonControl;
using PlayerRoles;

namespace Tycoon.Core.IEnumerators
{
    public static class Base
    {
        public static IEnumerator<float> BGM()
        {
            while (true)
            {
                AudioClipPlayback clip = GlobalPlayer.AddClip(Audios["BGMs"].GetRandomValue(), 0.2f, false);

                yield return Timing.WaitForSeconds((int)clip.Duration.TotalSeconds + Random.Range(1, 21));
            }
        }

        public static IEnumerator<float> PlayerStat()
        {
            while (true)
            {
                try
                {
                    foreach (var player in Player.List.Where(x => x.IsAlive))
                    {
                        RaycastHit hit = new RaycastHit();
                        RaycastHit hit1 = new RaycastHit();

                        if (PlayerBases.ContainsKey(player))
                        {
                            string hint()
                            {
                                if (Physics.Raycast(player.Position, Vector3.down, out hit, 1, (LayerMask)1))
                                {
                                    if (hit.transform.name.StartsWith("Interaction") && IsBase(hit, player, out int num))
                                    {
                                        int cost = int.Parse(hit.transform.name.Split('/')[1]);
                                        string[] name = hit.transform.name.Split('/')[2].Split(';');

                                        if (PlayerDollars[player] >= cost)
                                        {
                                            PlayerDollars[player] -= cost;

                                            foreach (string n in name)
                                                EnableObject(GetBase(PlayerBases[player]).Find(n));

                                            DisableObject(hit.transform.parent);

                                            AudioPlayers[player].AddClip("build", volume: 2);

                                            return $"<color=green>{string.Join(", ", name.Where(n => !n.Contains("Button")).ToArray())}</color>을(를) 구매하셨습니다.";
                                        }
                                        else
                                        {
                                            AudioPlayers[player].AddClip("wrong");

                                            return $"<color=red>💲</color>{cost - PlayerDollars[player]}이(가) 부족합니다.";
                                        }
                                    }
                                    else if (hit.transform.name == "Receive Dollar")
                                    {
                                        IsBase(hit, player, out int num1);

                                        if (BaseDollars[num1] > 0)
                                        {
                                            AudioPlayers[player].AddClip("Cash Sound", volume: 2);

                                            PlayerDollars[player] += BaseDollars[num1];
                                            BaseDollars[num1] = 0;
                                        }
                                    }
                                }
                                if (Physics.Raycast(player.ReferenceHub.PlayerCameraReference.position + player.ReferenceHub.PlayerCameraReference.forward * 0.2f, player.ReferenceHub.PlayerCameraReference.forward, out hit1, 30))
                                {
                                    if (hit1.transform.name.StartsWith("Hint: "))
                                    {
                                        return hit1.transform.name.Replace("Hint: ", "");
                                    }
                                    else if (hit1.transform.name.StartsWith("Button"))
                                    {
                                        string[] splitName = hit1.transform.name.Split('/');

                                        if (splitName.Length > 1)
                                        {
                                            string option = splitName[1];

                                            if (option == "Drop")
                                                return "[ALT] 키를 눌러 물건을 생산하세요.";

                                            else if (option == "Raser Toggle")
                                                return "[ALT] 키를 눌러 보안문 레이저를 조작하세요.";
                                        }
                                    }
                                    else if (hit1.transform.name.StartsWith("Item"))
                                    {
                                        int ItemID = int.Parse(hit1.transform.name.Split('/')[1]);

                                        if (ItemID == 1205)
                                        {
                                            return $"[ALT] 키를 눌러 <color=green>💲</color>20000(을)를 지불하고 <b><i><color=#FF0000>랜</color><color=#AA554B>덤</color><color=#55AA96>한</color> <color=#53FF96>아</color><color=#A7FF4B>이</color><color=#FBFF00>템</color></i></b>(을)를 획득하세요.";
                                        }
                                        else
                                        {
                                            ItemType itemType = (ItemType)ItemID;

                                            return $"[ALT] 키를 눌러 <color=#ffd700>{itemType.GetName()}</color>(을)를 획득하세요.";
                                        }
                                    }
                                    else if (hit1.transform.name.StartsWith("Interaction"))
                                    {
                                        int cost = int.Parse(hit1.transform.name.Split('/')[1]);
                                        string[] name = hit1.transform.name.Split('/')[2].Split(';').Where(n => !n.Contains("Button")).ToArray();

                                        return $"{string.Join(", ", name)}을(를) 구매하려면 <color=green>💲</color>{cost}이(가) 필요합니다.";
                                    }
                                    else
                                    {
                                        return "";
                                    }
                                }

                                return "";
                            }

                            player.ShowHint(
    $"""
<align=right>
<size=25><b>
베이스 ID: {PlayerBases[player]}
<color=green>💲</color>{PlayerDollars[player]}
</b></size>
</align>









<size=25><b>{hint()}</b></size>











"""
.Replace("{dollar}", $"{(hit1.transform != null ? (int.TryParse(hit1.transform.parent.parent.name, out int result) ? BaseDollars[result] : "Error!") : "")}")
                                );
                        }
                        else
                        {
                            player.ShowHint($"<size=25><b>마음에 드는 베이스로 가세요!</b></size>", 1.2f);
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    Log.Error($"Exception: {ex.Message}\nStack Trace: {ex.StackTrace}");
                }

                yield return Timing.WaitForSeconds(1);
            }
        }

        public static IEnumerator<float> AutoDropper()
        {
            while (true)
            {
                foreach (var num in PlayerBases.Values)
                {
                    foreach (Transform dropper in GetBase(num))
                    {
                        if (dropper.name.Contains("Dropper"))
                        {
                            Transform up = dropper.GetChild(0);

                            if (EnabledObjects.Contains(dropper))
                            {
                                int Power = int.Parse(up.name.Split('/')[1]);

                                IEnumerator<float> task()
                                {
                                    for (int i = 0; i < Power; i++)
                                    {
                                        Timing.RunCoroutine(DropProduct(num, up.position));

                                        yield return Timing.WaitForSeconds(0.5f);
                                    }
                                }

                                Timing.RunCoroutine(task());
                            }
                        }
                    }
                }

                yield return Timing.WaitForSeconds(2);
            }
        }

        public static IEnumerator<float> OwnerDoor()
        {
            while (true)
            {
                foreach (var ownerDoor in RaserDoors)
                {
                    foreach (var player in Player.List.Where(PlayerBases.ContainsKey))
                    {
                        if (Vector3.Distance(player.Position, ownerDoor.Value.position) < 1 && PlayerBases[player] != ownerDoor.Key && BaseRasers[ownerDoor.Key])
                            player.Kill("보안문 레이저에 의해 구워졌습니다.");
                    }
                }

                yield return Timing.WaitForOneFrame;
            }
        }

        public static IEnumerator<float> ClearDecals()
        {
            Map.CleanAllRagdolls();
            Map.CleanAllItems();

            while (true)
            {
                List<string> list = new List<string> 
                {
                    "bulletholes",
                    "blood",
                    "ragdolls",
                    "items"
                };

                foreach (var s in list)
                {
                    Server.ExecuteCommand($"/cleanup {s}");
                }

                yield return Timing.WaitForSeconds(300);
            }
        }

        public static IEnumerator<float> InputCooldown()
        {
            while (true)
            {
                ChatCooldowns.Clear();
                EmotionCooldowns.Clear();

                yield return Timing.WaitForSeconds(2f);
            }
        }

        public static IEnumerator<float> ItemSpawner()
        {
            while (true)
            {
                try
                {
                    int num = Random.Range(1, 1001);

                    if (num < 250)
                    {
                        Item coin = Item.Create(ItemType.Coin);

                        coin.CreatePickup(new Vector3(Random.Range(-8.125f, 91f), 1100, Random.Range(-88.8125f, 10.39847f)), new Quaternion(Random.Range(0, 180), Random.Range(0, 180), Random.Range(0, 180), Random.Range(0, 180)));
                    }

                    if (num == 1)
                    {
                        ItemType itemType = EnumToList<ItemType>().GetRandomValue();

                        Pickup.CreateAndSpawn(itemType, new Vector3(Random.Range(-8.125f, 91f), 1100, Random.Range(-88.8125f, 10.39847f)), new Quaternion(Random.Range(0, 180), Random.Range(0, 180), Random.Range(0, 180), Random.Range(0, 180)));
                    }
                }
                catch (System.Exception ex)
                {
                    Log.Error($"{ex.ToString()}");
                }

                yield return Timing.WaitForSeconds(1);
            }
        }

        public static IEnumerator<float> IsFallDown()
        {
            while (true)
            {
                foreach (var player in Player.List.Where(x => x.IsAlive))
                {
                    if (OnGround.ContainsKey(player) && !player.IsNoclipPermitted && player.Role.Type != RoleTypeId.Scp079)
                    {
                        if (FpcExtensionMethods.IsGrounded(player.ReferenceHub))
                            OnGround[player] = 5;
                        else
                        {
                            OnGround[player] -= 0.1f;

                            if (OnGround[player] <= 0)
                            {
                                player.Kill("공허에 빨려들어갔습니다. (5초 이상 낙하)");

                                OnGround[player] = 5;
                            }
                        }
                    }
                }

                yield return Timing.WaitForSeconds(0.1f);
            }
        }
    }
}
