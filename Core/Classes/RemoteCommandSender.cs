using RemoteAdmin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tycoon.Core.Classes
{
    public class RemoteCommandSender : PlayerCommandSender
    {
        public override void RaReply(string text, bool success, bool logToConsole, string overrideDisplay)
        {
            this.ReferenceHub.queryProcessor.SendToClient(text, success, logToConsole, overrideDisplay);
        }

        public override void Print(string text)
        {
            this.ReferenceHub.queryProcessor.SendToClient(text, true, true, "");
        }

        public override string SenderId
        {
            get
            {
                return this.ReferenceHub.authManager.UserId;
            }
        }

        public override string Nickname
        {
            get
            {
                return this.ReferenceHub.nicknameSync.MyNick;
            }
        }
        public new int PlayerId
        {
            get
            {
                return this.ReferenceHub.PlayerId;
            }
        }
        public override ulong Permissions { get; }
        public override byte KickPower { get; }
        public override bool FullPermissions { get; }
        public override string LogName
        {
            get
            {
                return this.Nickname + " (" + this.ReferenceHub.authManager.UserId + ")";
            }
        }

        public RemoteCommandSender(ReferenceHub hub, ulong permissions, byte kickPower, bool fullPermissions) : base(hub)
        {
            this.ReferenceHub = hub;
            this.Permissions = permissions;
            this.KickPower = kickPower;
            this.FullPermissions = fullPermissions;
        }

        public new readonly ReferenceHub ReferenceHub;
    }
}
