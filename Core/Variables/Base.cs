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

        public static List<Player> DropperCooldowns = new List<Player> { };
        public static List<Player> ItemCooldowns = new List<Player> { };
        public static List<Player> MeleeCooldowns = new List<Player>();
        public static List<Player> EmotionCooldowns = new List<Player>();
        public static List<Player> ChatCooldowns = new List<Player>();
        public static List<Player> IntercomPlayers = new List<Player>();
        public static List<Player> GodModePlayers = new List<Player>();

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
        public static Dictionary<int, bool> BaseRasers = new Dictionary<int, bool> { }; // 베이스 ID, 여부
        public static Dictionary<int, Transform> RaserDoors = new Dictionary<int, Transform>();
    }
}
