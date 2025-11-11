using System;
using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Packets;
using SqlDataProvider.Data;

namespace Game.Server.Consortia.Handle
{
	[global::Consortia(11)]
	public class ConsortiaInviteAdd : IConsortiaCommandHadler
	{
		public int CommandHandler(GamePlayer Player, GSPacketIn packet)
		{
			if (Player.PlayerCharacter.ConsortiaID == 0)
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
			string str = packet.ReadString();
			bool val = false;
			string msg = "ConsortiaInviteAddHandler.Failed";
			if (string.IsNullOrEmpty(str))
			{
				return 0;
			}
			using (ConsortiaBussiness consortiaBussiness = new ConsortiaBussiness())
			{
				ConsortiaInviteUserInfo info = new ConsortiaInviteUserInfo();
				info.ConsortiaID = Player.PlayerCharacter.ConsortiaID;
				info.ConsortiaName = Player.PlayerCharacter.ConsortiaName;
				info.InviteDate = DateTime.Now;
				info.InviteID = Player.PlayerCharacter.ID;
				info.InviteName = Player.PlayerCharacter.NickName;
				info.IsExist = true;
				info.Remark = "";
				info.UserID = 0;
				info.UserName = str;
				if (consortiaBussiness.AddConsortiaInviteUsers(info, ref msg))
				{
					msg = "ConsortiaInviteAddHandler.Success";
					val = true;
					GameServer.Instance.LoginServer.SendConsortiaInvite(info.ID, info.UserID, info.UserName, info.InviteID, info.InviteName, info.ConsortiaName, info.ConsortiaID);
				}
			}
			GSPacketIn packet2 = new GSPacketIn(129);
			packet2.WriteByte(11);
			packet2.WriteString(str);
			packet2.WriteBoolean(val);
			packet2.WriteString(LanguageMgr.GetTranslation(msg));
			Player.Out.SendTCP(packet2);
			return 0;
		}
	}
}
