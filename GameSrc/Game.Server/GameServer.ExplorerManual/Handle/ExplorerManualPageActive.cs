using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using Game.Server.Packets;
using SqlDataProvider.Data;
using System.Collections.Generic;
using System.Linq;

namespace Game.Server.ExplorerManual.Handle
{
    [ExplorerManualHandleAttbute((byte)ExplorerManualPackageType.PAGE_ACTIVATE)]
    public class ExplorerManualPageActiveHandle : IExplorerManualCommandHadler
    {
        public int CommandHandler(GamePlayer player, GSPacketIn packet)
        {
            int tipoActivate = packet.ReadInt();
            int pageID = packet.ReadInt();
            bool param3 = packet.ReadBoolean();
            GSPacketIn pkg = new GSPacketIn((short)ePackageType.EXPLORER_MANUAL);
            pkg.WriteByte((byte)ExplorerManualPackageType.PAGE_ACTIVATE);
            JampsPageItemList page = JampsManualMgr.getPageFromID(pageID);
            if (tipoActivate == 2)
            {
                pkg.WriteBoolean(player.PlayerCharacter.Money >= page.ActivateCurrency);
                pkg.WriteInt(tipoActivate);
                if (player.PlayerCharacter.Money >= page.ActivateCurrency)
                {
                    player.RemoveMoney(page.ActivateCurrency);
                }
                else
                {
                    return 0;
                }
            }
            else if (tipoActivate == 1)
            {
                pkg.WriteBoolean(player.PlayerCharacter.explorerManualInfo.acivatePage(pageID));
                pkg.WriteInt(tipoActivate);
            }
            player.EquipBag.UpdatePlayerProperties();
            player.SendTCP(pkg);
            return 1;
        }
    }
}