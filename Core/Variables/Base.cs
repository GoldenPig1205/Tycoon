using Exiled.API.Features;
using MapEditorReborn.API.Features.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Tycoon.Core.Variables
{
    public static class Base
    {
        public static AudioPlayer GlobalPlayer;
        public static Transform FirstSpawnPoint;
        public static SchematicObject TycoonSchematic;

        public static Dictionary<Transform, Dictionary<Transform, Vector3>> TransformPositions = new Dictionary<Transform, Dictionary<Transform, Vector3>> { };
        public static Dictionary<Player, AudioPlayer> AudioPlayers = new Dictionary<Player, AudioPlayer> { };
        public static Dictionary<string, List<string>> Audios = new Dictionary<string, List<string>> 
        {
            { "BGMs", new List<string> { } },
            { "SEs", new List<string> { } }
        };
        public static Dictionary<Player, int> PlayerBases = new Dictionary<Player, int> { }; // 플레이어, 베이스 ID
        public static Dictionary<Player, int> PlayerDollars = new Dictionary<Player, int> { }; // 플레이어, 달러
        public static Dictionary<int, int> BaseDollars = new Dictionary<int, int> { }; // 베이스 ID, 달러
    }
}
