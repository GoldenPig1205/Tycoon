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

            foreach (var _audioClip in System.IO.Directory.GetFiles(Paths.Configs + "/Tycoon/BGMs/"))
            {
                string name = _audioClip.Replace(Paths.Configs + "/Tycoon/BGMs/", "").Replace(".ogg", "");

                Audios["BGMs"].Add(name);

                AudioClipStorage.LoadClip(_audioClip, name);
            }

            foreach (var _audioClip in System.IO.Directory.GetFiles(Paths.Configs + "/Tycoon/SEs/"))
            {
                string name = _audioClip.Replace(Paths.Configs + "/Tycoon/SEs/", "").Replace(".ogg", "");

                Audios["SEs"].Add(name);

                AudioClipStorage.LoadClip(_audioClip, name);
            }

            GlobalPlayer = AudioPlayer.CreateOrGet($"Global AudioPlayer", onIntialCreation: (p) =>
            {
                Speaker speaker = p.AddSpeaker("Main", isSpatial: false, maxDistance: 5000);
            });

            FirstSpawnPoint = GameObject.FindObjectsOfType<Transform>().Where(t => t.name == "[SP] First").FirstOrDefault();
            TycoonSchematic = (SchematicObject)MapEditorReborn.API.API.SpawnedObjects.Where(x => x is SchematicObject).FirstOrDefault();

            foreach (Transform b in TycoonSchematic.transform.GetChild(0).GetChild(3))
            {
                Dictionary<Transform, Vector3> dict = new Dictionary<Transform, Vector3> { };

                foreach (Transform child in b)
                    dict.Add(child, child.position);

                TransformPositions.Add(b, dict);

                ResetBase(int.Parse(b.name));
            }

            for (int i = 1; i < 10; i++)
            {
                Transform ownerDoor = GetBase(i).Find("Owner Door");

                RaserDoors.Add(i, ownerDoor);
            }

            InventoryLimits.StandardCategoryLimits[ItemCategory.SpecialWeapon] = 8;
            InventoryLimits.StandardCategoryLimits[ItemCategory.SCPItem] = 8;
            InventoryLimits.Config.RefreshCategoryLimits();

            Timing.RunCoroutine(BGM());
            Timing.RunCoroutine(PlayerStat());
            Timing.RunCoroutine(AutoDropper());
            // Timing.RunCoroutine(OwnerDoor());
            Timing.RunCoroutine(ClearDecals());
            Timing.RunCoroutine(InputCooldown());
            Timing.RunCoroutine(ItemSpawner());
            Timing.RunCoroutine(IsFallDown());
        }
    }
}
