#region Ly Hon
//using Bussiness;
//using Game.Base.Packets;
//using SqlDataProvider.Data;

//namespace Game.Server.Packets.Client
//{
//	[PacketHandler(248, "离婚")]
//	internal class DivorceApplyHandler : IPacketHandler
//	{
//		public int HandlePacket(GameClient client, GSPacketIn packet)
//		{
//			bool flag = packet.ReadBoolean();
//			if (!client.Player.PlayerCharacter.IsMarried)
//			{
//				return 1;
//			}
//			if (client.Player.PlayerCharacter.HasBagPassword && client.Player.PlayerCharacter.IsLocked)
//			{
//				client.Out.SendMessage(eMessageType.GM_NOTICE, LanguageMgr.GetTranslation("Bag.Locked"));
//				return 0;
//			}
//			if (client.Player.PlayerCharacter.IsCreatedMarryRoom)
//			{
//				client.Player.Out.SendMessage(eMessageType.ChatNormal, LanguageMgr.GetTranslation("DivorceApplyHandler.Msg2"));
//				return 1;
//			}
//			int num = GameProperties.PRICE_DIVORCED;
//			if (flag)
//			{
//				num = GameProperties.PRICE_DIVORCED_DISCOUNT;
//			}
//			if (!client.Player.MoneyDirect(num, IsAntiMult: false))
//			{
//				return 1;
//			}
//			CountBussiness.InsertSystemPayCount(client.Player.PlayerCharacter.ID, num, 0, 0, 3);
//			using (PlayerBussiness bussiness = new PlayerBussiness())
//			{
//				PlayerInfo userSingleByUserID = bussiness.GetUserSingleByUserID(client.Player.PlayerCharacter.SpouseID);
//				if (userSingleByUserID == null || userSingleByUserID.Sex == client.Player.PlayerCharacter.Sex)
//				{
//					return 1;
//				}
//				MarryApplyInfo info = new MarryApplyInfo
//				{
//					UserID = client.Player.PlayerCharacter.SpouseID,
//					ApplyUserID = client.Player.PlayerCharacter.ID,
//					ApplyUserName = client.Player.PlayerCharacter.NickName,
//					ApplyType = 3,
//					LoveProclamation = "",
//					ApplyResult = false
//				};
//				DailyRecordInfo record = new DailyRecordInfo
//				{
//					UserID = client.Player.PlayerCharacter.ID,
//					Type = 27,
//					Value = client.Player.PlayerCharacter.SpouseName
//				};
//				new PlayerBussiness().AddDailyRecord(record);
//				int id = 0;
//				if (bussiness.SavePlayerMarryNotice(info, 0, ref id))
//				{
//					GameServer.Instance.LoginServer.SendUpdatePlayerMarriedStates(userSingleByUserID.ID);
//					client.Player.LoadMarryProp();
//				}
//			}
//			client.Player.QuestInventory.ClearMarryQuest();
//			client.Player.Out.SendPlayerDivorceApply(client.Player, result: true, isProposer: true);
//			client.Player.SendMessage(LanguageMgr.GetTranslation("DivorceApplyHandler.Msg3"));
//			return 0;
//		}
//	}
//}
#endregion
using Bussiness;
using Game.Base.Packets;
using Game.Server.Managers;
using Game.Server.Packets;
using Game.Server.Packets.Client;
using Game.Server.SceneMarryRooms;
using SqlDataProvider.Data;

namespace Game.Server.Packet.Client
{
    [PacketHandler(248, "离婚")]
    class DivorceApplyHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn pkg)
        {
            bool DivorcedDiscountMoney = pkg.ReadBoolean();

            if (!client.Player.PlayerCharacter.IsMarried)
            {
                return 1;
            }

            if (client.Player.PlayerCharacter.HasBagPassword && client.Player.PlayerCharacter.IsLocked)
            {
                client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bag.Locked"));
                return 0;
            }

            if (client.Player.PlayerCharacter.IsCreatedMarryRoom)
            {
                MarryRoom room = null;
                MarryRoom[] allMarryRoom = MarryRoomMgr.GetAllMarryRoom();
                foreach (MarryRoom room2 in allMarryRoom)
                {
                    if (room2.Info.GroomID == client.Player.PlayerCharacter.ID || room2.Info.BrideID == client.Player.PlayerCharacter.ID)
                    {
                        room = room2;
                        break;
                    }
                }
                if (room == null && client.Player.PlayerCharacter.SelfMarryRoomID != 0)
                {
                    MarryRoomMgr.RemoveMarryRoom(room);
                }
               // client.Player.Out.SendMessage(eMessageType.ChatNormal, LanguageMgr.GetTranslation("DivorceApplyHandler.Msg2"));
               // return 1;
            }

            int needMoney = GameProperties.PRICE_DIVORCED;
            if (DivorcedDiscountMoney)
            {
                needMoney = GameProperties.PRICE_DIVORCED_DISCOUNT;
            }

            if (!client.Player.MoneyDirect(needMoney, IsAntiMult:false))
            {
                client.Player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("DivorceApplyHandler.Msg1"));
                return 1;
            }

            client.Player.RemoveMoney(needMoney, isConsume: false);
            using (PlayerBussiness db = new PlayerBussiness())
            {
                MarryApplyInfo info = new MarryApplyInfo();
                info.UserID = client.Player.PlayerCharacter.SpouseID;
                info.UserID = client.Player.PlayerCharacter.SpouseID;
                info.ApplyUserID = client.Player.PlayerCharacter.ID;
                info.ApplyUserName = client.Player.PlayerCharacter.NickName;
                info.ApplyType = 3;
                info.LoveProclamation = "";
                info.ApplyResult = false;
                int id = 0;
                if (db.SavePlayerMarryNotice(info, 0, ref id))
                {
                    PlayerInfo tempSpouse = db.GetUserSingleByUserID(client.Player.PlayerCharacter.SpouseID);
                    if (tempSpouse != null)
                    {
                        GameServer.Instance.LoginServer.SendUpdatePlayerMarriedStates(tempSpouse.ID);
                    }

                    client.Player.LoadMarryProp();
                }
            }
            client.Player.QuestInventory.ClearMarryQuest(); //离婚后清除结婚后任务.
            client.Player.Out.SendPlayerDivorceApply(client.Player, true, true);
            client.Player.SendMessage(LanguageMgr.GetTranslation("DivorceApplyHandler.Msg3"));
            return 0;
        }
    }
}
