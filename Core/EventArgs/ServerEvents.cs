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
using static Tycoon.Core.IEnumerators.Base;

namespace Tycoon.Core.EventArgs
{
    public static class ServerEvents
    {
        public static IEnumerator<float> OnWaitingForPlayers()
        {
            yield return Timing.WaitForSeconds(1);

            Map.IsDecontaminationEnabled = false;
            Respawn.PauseWaves();
            Round.IsLocked = true;
            Round.Start();
            Server.FriendlyFire = true;
            Server.ExecuteCommand($"/mp load Tycoon");

            foreach (var _audioClip in System.IO.Directory.GetFiles(Paths.Configs + "/Tycoon/BGM/"))
                AudioClipStorage.LoadClip(_audioClip, _audioClip.Replace(Paths.Configs + "/Tycoon/BGM/", "").Replace(".ogg", ""));

            GlobalPlayer = AudioPlayer.CreateOrGet($"Global AudioPlayer", onIntialCreation: (p) =>
            {
                Speaker speaker = p.AddSpeaker("Main", isSpatial: false, maxDistance: 5000);
            });

            FirstSpawnPoint = GameObject.FindObjectsOfType<Transform>().Where(t => t.name == "[SP] First").FirstOrDefault();
            BaseSpawnPoints = GameObject.FindObjectsOfType<Transform>().Where(t => t.name.StartsWith("[SP] Baseㅣ")).ToList();

            InventoryLimits.StandardCategoryLimits[ItemCategory.SpecialWeapon] = 8;
            InventoryLimits.StandardCategoryLimits[ItemCategory.SCPItem] = 8;
            InventoryLimits.Config.RefreshCategoryLimits();

            Timing.RunCoroutine(BGM());
        }
    }
}
