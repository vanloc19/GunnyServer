using System;
using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Packets;
using SqlDataProvider.Data;

namespace Game.Server.Consortia.Handle
{
	[global::Consortia(0)]
	public class ConsortiaTryin : IConsortiaCommandHadler
	{
		public int CommandHandler(GamePlayer Player, GSPacketIn packet)
		{
			if (Player.PlayerCharacter.ConsortiaID != 0)
			{
				return 0;
			}

			if (Player.PlayerCharacter.QuitConsortiaDate.AddHours(GameProperties.TimeWaitToReJoinConsortia) >
			    DateTime.Now)
			{
				Player.Out.SendMessage(eMessageType.Normal,
					"Sau khi thoát guild, phải chờ " + GameProperties.TimeWaitToReJoinConsortia +
					" giờ mới được tham gia guild mới!");
				return 0;
			}
			int val1 = packet.ReadInt();
			bool val2 = false;
			string msg = "ConsortiaApplyLoginHandler.ADD_Failed";
			using (ConsortiaBussiness consortiaBussiness = new ConsortiaBussiness())
			{
				if (consortiaBussiness.AddConsortiaApplyUsers(new ConsortiaApplyUserInfo
				{
					ApplyDate = DateTime.Now,
					ConsortiaID = val1,
					ConsortiaName = "",
					IsExist = true,
					Remark = "",
					UserID = Player.PlayerCharacter.ID,
					UserName = Player.PlayerCharacter.NickName
				}, ref msg))
				{
					msg = ((val1 != 0) ? "ConsortiaApplyLoginHandler.ADD_Success" : "ConsortiaApplyLoginHandler.DELETE_Success");
					val2 = true;
				}
			}
			GSPacketIn packet2 = new GSPacketIn(129);
			packet2.WriteByte(0);
			packet2.WriteInt(val1);
			packet2.WriteBoolean(val2);
			packet2.WriteString(LanguageMgr.GetTranslation(msg));
			Player.Out.SendTCP(packet2);
			return 0;
		}
	}
}
