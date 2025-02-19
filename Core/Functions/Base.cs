using AdminToys;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Toys;
using Hints;
using MapEditorReborn.API.Features;
using MapEditorReborn.API.Features.Objects;
using MapEditorReborn.API.Features.Serializable;
using MEC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Utf8Json.Internal.DoubleConversion;
using Utf8Json.Resolvers.Internal;
using static Tycoon.Core.Variables.Base;

namespace Tycoon.Core.Functions
{
    public static class Base
    {
        public static bool IsBase(RaycastHit hit, Player player, out int num)
        {
            num = int.Parse(hit.transform.parent.parent.name);

            return int.Parse(hit.transform.parent.parent.name) == PlayerBases[player];
        }

        public static Transform GetBase(int num)
        {
            return TycoonSchematic.transform.GetChild(0).GetChild(3).GetChild(num - 1);
        }

        public static void ResetBase(int num)
        {
            List<string> ignoredItems = new List<string>
            {
                "Door",
                "Surface"
            };

            foreach (Transform child in GetBase(num))
            {
                if (!ignoredItems.Contains(child.name))
                    DisableObject(child);

                else
                    EnableObject(child);
            }
        }

        public static void EnableObject(Transform transform)
        {
            if (!EnabledObjects.Contains(transform))
                EnabledObjects.Add(transform);

            if (Primitives.ContainsKey(transform))
                Primitives[transform].ForEach(x => { x.Primitive.Spawn(); x.gameObject.SetActive(true); });

            else
            {
                List<PrimitiveObject> primitives = transform.GetComponentsInChildren<PrimitiveObject>().ToList();

                primitives.ForEach(x => { x.Primitive.Spawn(); x.gameObject.SetActive(true); });

                Primitives.Add(transform, primitives);
            }
        }

        public static void DisableObject(Transform transform)
        {
            if (EnabledObjects.Contains(transform))
                EnabledObjects.Remove(transform);

            if (Primitives.ContainsKey(transform))
                Primitives[transform].ForEach(x => { x.Primitive.UnSpawn(); x.gameObject.SetActive(false); });

            else
            {
                List<PrimitiveObject> primitives = transform.GetComponentsInChildren<PrimitiveObject>().ToList();

                primitives.ForEach(x => { x.Primitive.UnSpawn(); x.gameObject.SetActive(false); });

                Primitives.Add(transform, primitives);
            }
        }

        public static IEnumerator<float> DropProduct(int baseNum, Vector3 start)
        {
            int power = 1;
            Transform conveyor;

            var primitive = ObjectSpawner.SpawnPrimitive(new PrimitiveSerializable
            {
                PrimitiveType = PrimitiveType.Cube,
                Position = start,
                Scale = new Vector3(0.5f, 0.5f, 0.5f),
                PrimitiveFlags = PrimitiveFlags.Visible,
                Color = "white",
                RoomType = RoomType.Surface,
                Rotation = new Vector3(0, 0, 0),
                Static = false
            });
            primitive.name = "1";

            Timing.CallDelayed(10, () =>
            {
                if (primitive != null)
                    primitive.Destroy();
            });

            while (true)
            {
                primitive.Position = primitive.Position + Vector3.down * 0.05f;

                if (Physics.Raycast(primitive.Position, Vector3.down, out RaycastHit hit1, 0.25f, (LayerMask)1))
                {
                    conveyor = hit1.transform.parent;
                    break;
                }

                yield return Timing.WaitForOneFrame;
            };

            float startToEndDuration = 5f;
            float elapsedTime = 0f;
            Vector3 startingPos = primitive.Position;
            Vector3 endPosition = conveyor.Find("End").position;

            List<string> upgradedList = new List<string> { };

            if (Physics.Raycast(start, Vector3.down, out RaycastHit hit, 10, (LayerMask)1))
            {
                conveyor = hit.transform.parent;
                Transform _base = GetBase(baseNum);

                Dictionary<string, string[]> upgraders = new Dictionary<string, string[]>
                {
                    { "Conveyor 1", new string[] { "Upgrader 1", "Upgrader 2", "Super Upgrader 1" } },
                    { "Conveyor 2", new string[] { "Upgrader 3", "Super Upgrader 2", "Ultra Upgrader 1" } },
                    { "Conveyor 3", new string[] { "Ultra Upgrader 2", "Hyper Upgrader 1", "Mega Upgrader 1" } }
                };

                foreach (string upgraderName in upgraders[conveyor.name])
                {
                    Transform upgrader = _base.Find(upgraderName);

                    if (EnabledObjects.Contains(upgrader))
                        power += int.Parse(upgrader.GetChild(3).name.Split('/')[1]);
                }
            }

            while (elapsedTime < startToEndDuration)
            {
                primitive.Position = Vector3.Lerp(startingPos, endPosition, elapsedTime / startToEndDuration);
                elapsedTime += 1;

                yield return Timing.WaitForSeconds(1);
            }

            primitive.Destroy();
            BaseDollars[baseNum] += power;

            yield break;
        }

        public static List<T> EnumToList<T>()
        {
            Array items = Enum.GetValues(typeof(T));
            List<T> itemList = new List<T>();

            foreach (T item in items)
            {
                if (!item.ToString().Contains("None"))
                    itemList.Add(item);
            }

            return itemList;
        }

        public static bool TryGetLookPlayer(Player player, float Distance, out Player target, out RaycastHit? raycastHit)
        {
            target = null;
            raycastHit = null;

            if (Physics.Raycast(player.ReferenceHub.PlayerCameraReference.position + player.ReferenceHub.PlayerCameraReference.forward * 0.2f, player.ReferenceHub.PlayerCameraReference.forward, out RaycastHit hit, Distance) &&
                    hit.collider.TryGetComponent<IDestructible>(out IDestructible destructible))
            {
                if (Player.TryGet(hit.collider.GetComponentInParent<ReferenceHub>(), out Player t) && player != t)
                {
                    target = t;
                    raycastHit = hit;

                    return true;
                }
            }

            return false;
        }

        public static string ColorFormat(string cn)
        {
            if (ColorUtility.TryParseHtmlString(cn, out Color color))
                return color.ToHex();

            else
            {
                Dictionary<string, string> Colors = new Dictionary<string, string>
                {
                    // {"gold", "#EFC01A"},
                    // {"teal", "#008080"},
                    // {"blue", "#005EBC"},
                    // {"purple", "#8137CE"},
                    // {"light_red", "#FD8272"},
                    {"pink", "#FF96DE"},
                    {"red", "#C50000"},
                    {"default", "#FFFFFF"},
                    {"brown", "#944710"},
                    {"silver", "#A0A0A0"},
                    {"light_green", "#32CD32"},
                    {"crimson", "#DC143C"},
                    {"cyan", "#00B7EB"},
                    {"aqua", "#00FFFF"},
                    {"deep_pink", "#FF1493"},
                    {"tomato", "#FF6448"},
                    {"yellow", "#FAFF86"},
                    {"magenta", "#FF0090"},
                    {"blue_green", "#4DFFB8"},
                    // {"silver_blue", "#666699"},
                    {"orange", "#FF9966"},
                    // {"police_blue", "#002DB3"},
                    {"lime", "#BFFF00"},
                    {"green", "#228B22"},
                    {"emerald", "#50C878"},
                    {"carmine", "#960018"},
                    {"nickel", "#727472"},
                    {"mint", "#98FB98"},
                    {"army_green", "#4B5320"},
                    {"pumpkin", "#EE7600"}
                };

                if (Colors.ContainsKey(cn))
                    return Colors[cn];

                else
                    return "#FFFFFF";
            }
        }

        public static string BadgeFormat(Player player)
        {
            if (player.RankName != null && !player.BadgeHidden)
                return $"[<color={ColorFormat(player.RankColor)}>{player.RankName}</color>] ";

            else
                return "";
        }
    }
}
