using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using Game.Server.Packets;
using SqlDataProvider.Data;
using System.Collections.Generic;
using System.Linq;

namespace Game.Server.ExplorerManual.Handle
{
    [ExplorerManualHandleAttbute((byte)ExplorerManualPackageType.UPGRADE)]
    public class ExplorerManualUpgradeHandle : IExplorerManualCommandHadler
    {
        public int CommandHandler(GamePlayer player, GSPacketIn packet)
        {
            bool autoBuy = packet.ReadBoolean();
            bool param22 = packet.ReadBoolean();
            bool autoEvolution = packet.ReadBoolean();
            int maxLevel = JampsManualMgr.maxLevel;
            JampsUpgradeItemList[] condictions = JampsManualMgr.getCondictionsByLevelAndType(player.PlayerCharacter.explorerManualInfo.manualLevel, 1);
            ItemInfo consume = player.PropBag.GetItemByTemplateID(0, condictions[0].Parameter1);
            while (true)
            {
                if (consume?.Count >= condictions[0].Parameter2)
                {
                    player.PropBag.BeginChanges();
                    player.PropBag.RemoveCountFromStack(consume, 1);
                    player.PropBag.CommitChanges();
                    player.PlayerCharacter.explorerManualInfo.progress += 1;
                    if (player.PlayerCharacter.explorerManualInfo.progress ==
                        player.PlayerCharacter.explorerManualInfo.manualLevel)
                    {
                        player.PlayerCharacter.explorerManualInfo.progress = 0;
                        player.PlayerCharacter.explorerManualInfo.conditionCount = 0;
                        player.PlayerCharacter.explorerManualInfo.manualLevel += 1;
                        if (player.PlayerCharacter.explorerManualInfo.manualLevel > maxLevel)
                        {
                            player.PlayerCharacter.explorerManualInfo.manualLevel = maxLevel;
                        }

                        break;
                    }

                    if (!autoEvolution)
                        break;
                }
                else
                {
                    break;
                }
            }
            JampsManualItemList level = JampsManualMgr._jampsManualItemList[player.PlayerCharacter.explorerManualInfo.manualLevel];
            bool flag = player.PlayerCharacter.explorerManualInfo.manualLevel == 6 || player.PlayerCharacter.explorerManualInfo.manualLevel == 11 || player.PlayerCharacter.explorerManualInfo.manualLevel == 16 || player.PlayerCharacter.explorerManualInfo.manualLevel == 21;
            GSPacketIn op4 = new GSPacketIn((short)ePackageType.EXPLORER_MANUAL);
            op4.WriteByte((byte)ExplorerManualPackageType.UPGRADE);
            op4.WriteBoolean(flag);
            op4.WriteBoolean(true);
            op4.WriteBoolean(level.MagicAttack > 0);
            op4.WriteInt(player.PlayerCharacter.explorerManualInfo.manualLevel);
            op4.WriteInt(player.PlayerCharacter.explorerManualInfo.progress);
            player.EquipBag.UpdatePlayerProperties();
            player.SendTCP(op4);

            return 1;
        }
    }
}
