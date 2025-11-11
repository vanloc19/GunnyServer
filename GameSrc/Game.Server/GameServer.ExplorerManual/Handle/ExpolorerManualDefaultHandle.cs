using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Packets;
using SqlDataProvider.Data;
using System.Collections.Generic;
using System.Linq;

namespace Game.Server.ExplorerManual.Handle
{
    [ExplorerManualHandleAttbute((byte)ExplorerManualPackageType.DEFAULT)]
    public class ExplorerManualDefaultHandle : IExplorerManualCommandHadler
    {
        public int CommandHandler(GamePlayer player, GSPacketIn packet)
        {
            GSPacketIn pkg = new GSPacketIn((short)ePackageType.EXPLORER_MANUAL);
            pkg.WriteByte((byte)ExplorerManualPackageType.DEFAULT);
            pkg.WriteInt(player.PlayerCharacter.explorerManualInfo.manualLevel);
            pkg.WriteInt(player.PlayerCharacter.explorerManualInfo.progress);
            pkg.WriteInt(player.PlayerCharacter.explorerManualInfo.maxProgress);
            pkg.WriteInt(player.PlayerCharacter.explorerManualInfo.activesPage.Count());
            pkg.WriteInt(player.PlayerCharacter.explorerManualInfo.conditionCount);
            pkg.WriteInt(player.PlayerCharacter.explorerManualInfo.Agility);
            pkg.WriteInt(player.PlayerCharacter.explorerManualInfo.Armor);
            pkg.WriteInt(player.PlayerCharacter.explorerManualInfo.Attack);
            pkg.WriteInt(player.PlayerCharacter.explorerManualInfo.Damage);
            pkg.WriteInt(player.PlayerCharacter.explorerManualInfo.Defense);
            pkg.WriteInt(player.PlayerCharacter.explorerManualInfo.HP);
            pkg.WriteInt(player.PlayerCharacter.explorerManualInfo.Lucky);
            pkg.WriteInt(player.PlayerCharacter.explorerManualInfo.MagicAttack);
            pkg.WriteInt(player.PlayerCharacter.explorerManualInfo.MagicResistance);
            pkg.WriteInt(player.PlayerCharacter.explorerManualInfo.Stamina);
            List<PagesInfo> pages = player.PlayerCharacter.explorerManualInfo.activesPage.Where(o => o.activate).ToList();
            pkg.WriteInt(pages.Count);
            foreach (PagesInfo i in pages)
            {
                pkg.WriteInt(i.pageID);
            }
            player.SendTCP(pkg);

            return 1;
        }
    }
}
