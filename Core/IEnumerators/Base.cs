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
                foreach (var player in Player.List.Where(x => x.IsAlive))
                {
                    if (PlayerBases.ContainsKey(player))
                    {
                        string hint()
                        {
                            if (Physics.Raycast(player.Position, Vector3.down, out RaycastHit hit, 1, (LayerMask)1))
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

                                        return $"<color=green>{string.Join(", ", name)}</color>을(를) 구매하셨습니다.";          
                                    }
                                    else
                                    {
                                        return $"<color=red>💲</color>{cost}이(가) 부족합니다.";
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
                            if (Physics.Raycast(player.ReferenceHub.PlayerCameraReference.position + player.ReferenceHub.PlayerCameraReference.forward * 0.2f, player.ReferenceHub.PlayerCameraReference.forward, out RaycastHit hit1, 30))
                            {
                                if (hit1.transform.name.StartsWith("Hint: "))
                                {
                                    return hit1.transform.name.Replace("Hint: ", "");
                                }
                                else if (hit1.transform.name.StartsWith("Button"))
                                {
                                    return "[ALT] 키를 눌러 물건을 생산하세요.";
                                }
                                else if (hit1.transform.name.StartsWith("Interaction"))
                                {
                                    int cost = int.Parse(hit1.transform.name.Split('/')[1]);
                                    string[] name = hit1.transform.name.Split('/')[2].Split(';');

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
                            .Replace("{dollar}", $"{BaseDollars[PlayerBases[player]]}")
                            );
                    }
                    else
                    {
                        player.ShowHint($"<size=25><b>마음에 드는 베이스로 가세요!</b></size>", 1.2f);
                    }
                }

                yield return Timing.WaitForSeconds(1);
            }
        }
    }
}
