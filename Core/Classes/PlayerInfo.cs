using RemoteAdmin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tycoon.Core.Classes
{
    public class PlayerInfo
    {
        public int Kill { get; set; } = 0;
        public int Death { get; set; } = 0;
        public int Assist { get; set; } = 0;
        public int Coin { get; set; } = 0;
    }
}
