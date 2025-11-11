using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GMActives;
using Game.Server.Managers;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Server.Packets.Client
{
    [PacketHandler((short)ePackageType.WONDERFUL_GETREWARD, "场景用户离开")]
    public class WonderfulGetRewardHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            int total = packet.ReadInt();

            //UserRankRechargeInfo myRank = WorldMgr.GetSingleRankRecharge(client.Player);

            for (int i = 0; i < total; i++)
            {
                string activityId = packet.ReadString();//_loc2_.writeUTF(param1[_loc4_].activityId);
                int awardCount = packet.ReadInt();//_loc2_.writeInt(param1[_loc4_].awardCount);
                int giftIdCount = packet.ReadInt();//_loc2_.writeInt(param1[_loc4_].giftIdArr.length);

                //Console.WriteLine(string.Format("activityId: {0} - awardCount: {1} - giftIdCount: {2}", activityId, awardCount, giftIdCount));

                // check activityId
                IGMActive gmActiveInfo = GmActivityMgr.GetSingleGMAction(activityId);

                if (gmActiveInfo != null && gmActiveInfo.Info.IsAvalible)
                {
                    for (int j = 0; j < giftIdCount; j++)
                    {
                        string giftId = packet.ReadString();
                        int indexGift = packet.ReadInt();

                        Console.WriteLine(string.Format("giftId: {0} - indexGift: {1}", giftId, indexGift));
                        
                        if (!gmActiveInfo.GetAward(client.Player, giftId, indexGift))
                        {
                            client.Player.SendMessage(LanguageMgr.GetTranslation("WonderfulActivity.GetRewards.Error2"));
                            return 0;
                        }
                    }

                    client.Player.SendMessage(LanguageMgr.GetTranslation("WonderfulActivity.GetRewards.Success"));
                    client.Out.WonderfulSingleActivityInit(gmActiveInfo, client.Player);
                }
                else
                {
                    client.Player.SendMessage(LanguageMgr.GetTranslation("WonderfulActivity.GetRewards.Error"));
                    return 0;
                }
            }
            return 0;
        }
    }
}
