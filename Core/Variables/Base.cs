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

        public static List<Transform> BaseSpawnPoints = new List<Transform>();
    }
}
