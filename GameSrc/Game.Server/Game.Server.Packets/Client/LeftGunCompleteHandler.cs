using Bussiness;
using Game.Base.Packets;
using Game.Server.Managers;
using System;

namespace Game.Server.Packets.Client
{
    [PacketHandler((byte)ePackageType.LEFT_GUN_ROULETTE_COMPLETTE, "物品炼化")]
    public class LeftGunCompleteHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            if (client.Player.Extra.Info.LeftRoutteRate > 0f)
            {
                int num = (int)(client.Player.Extra.Info.LeftRoutteRate * 100);
                WorldMgr.SendSysTipNotice(LanguageMgr.GetTranslation("GameServer.LeftRotter.Notice.Msg", client.Player.PlayerCharacter.NickName, num));
            }
            return 0;
        }
    }
}