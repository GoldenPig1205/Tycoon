using Exiled.API.Extensions;
using MEC;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Tycoon.Core.Variables.Base;

namespace Tycoon.Core.IEnumerators
{
    public static class Base
    {
        public static IEnumerator<float> BGM()
        {
            while (true)
            {
                AudioClipPlayback clip = GlobalPlayer.AddClip(AudioClipStorage.AudioClips.Keys.GetRandomValue(), 0.2f, false);

                yield return Timing.WaitForSeconds((int)clip.Duration.TotalSeconds + Random.Range(1, 21));
            }
        }
    }
}
