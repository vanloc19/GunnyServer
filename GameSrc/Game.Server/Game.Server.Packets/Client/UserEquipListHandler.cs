using System;
using System.Collections.Generic;
using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client
{
    [PacketHandler((int)ePackageType.ITEM_EQUIP, "获取用户装备")]
    public class UserEquipListHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            int num = 0;
            string nickName = null;
            bool flag = packet.ReadBoolean();
            PlayerInfo player = null;
            List<ItemInfo> items = null;
            List<UserGemStone> userGemStone = null;
            ExplorerManualInfo explorerInfo = null;
            GamePlayer gamePlayer;
            if (!flag)
            {
                nickName = packet.ReadString();
                gamePlayer = WorldMgr.GetClientByPlayerNickName(nickName);
            }
            else
            {
                num = packet.ReadInt();
                gamePlayer = WorldMgr.GetPlayerById(num);
            }
            if (gamePlayer != null)
            {
                player = gamePlayer.PlayerCharacter;
                items = gamePlayer.EquipBag.GetItems(0, 31);
                userGemStone = gamePlayer.GemStone;
                explorerInfo = gamePlayer.PlayerCharacter.explorerManualInfo;
            }
            else
            {
                using (PlayerBussiness playerBussiness = new PlayerBussiness())
                {
                    player = (flag ? playerBussiness.GetUserSingleByUserID(num) : playerBussiness.GetUserSingleByNickName(nickName));
                    if (player != null)
                    {
                        player.Texp = playerBussiness.GetUserTexpInfoSingle(player.ID);
                        items = playerBussiness.GetUserEuqip(player.ID);
                        userGemStone = playerBussiness.GetSingleGemStones(player.ID);
                        explorerInfo = playerBussiness.InitExplorerManualInfo(player.ID);
                    }
                }
            }
            if (player != null && items != null && player.Texp != null && userGemStone != null && explorerInfo != null)
            {
                client.Out.SendUserEquip(player, items, userGemStone, explorerInfo);
                if (player.ID != client.Player.PlayerCharacter.ID)
                {
                    client.Player.PlayerProp.ViewOther(player);
                }
            }
            return 0;
        }
    }
}
