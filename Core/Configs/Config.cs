using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Interfaces;

namespace Tycoon.Core.Configs
{
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = false;
        public bool AutoStart { get; set; } = false;
        public float radius { get; set; } = 100;
        public int baseCount { get; set; } = 35;
    }
}
