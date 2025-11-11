using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Packets;
using SqlDataProvider.Data;
using System.Collections.Generic;
using System.Linq;

namespace Game.Server.ExplorerManual.Handle
{
    [ExplorerManualHandleAttbute((byte)ExplorerManualPackageType.PAGE)]
    public class ExplorerManualPageHandle : IExplorerManualCommandHadler
    {
        public int CommandHandler(GamePlayer player, GSPacketIn packet)
        {
            GSPacketIn pkg = new GSPacketIn((short)ePackageType.EXPLORER_MANUAL);
            pkg.WriteByte((byte)ExplorerManualPackageType.PAGE);
            int capitulo = packet.ReadInt();
            DebrisInfo[] debris = player.PlayerCharacter.explorerManualInfo.debris.Values.Where(o => o.chapterID == capitulo).ToArray();
            int count = debris.Length;
            pkg.WriteInt(count);
            foreach (DebrisInfo i in debris)
            {
                pkg.WriteInt(i.ID);
                pkg.WriteDateTime(i.date);
            }
            player.SendTCP(pkg);
            return 1;
        }
    }
}