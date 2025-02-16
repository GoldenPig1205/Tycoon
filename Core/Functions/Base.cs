using Exiled.API.Features;
using Hints;
using MapEditorReborn.API.Features.Objects;
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
            }
        }

        public static void EnableObject(Transform transform)
        {
            transform.position = TransformPositions[transform.parent][transform];
        }

        public static void DisableObject(Transform transform)
        {
            transform.position = new Vector3(5000, 5000, 5000);
        }
    }
}
