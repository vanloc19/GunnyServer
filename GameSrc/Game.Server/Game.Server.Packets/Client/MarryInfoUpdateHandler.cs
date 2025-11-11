using System;
using Bussiness;
using Game.Base.Packets;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client
{
	[PacketHandler(213, "更新征婚信息")]
	public class MarryInfoUpdateHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			if (client.Player.PlayerCharacter.MarryInfoID == 0)
			{
				return 1;
			}
			bool flag = packet.ReadBoolean();
			string str = packet.ReadString();
			int marryInfoID = client.Player.PlayerCharacter.MarryInfoID;
			string translateId = "MarryInfoUpdateHandler.Fail";
			using (PlayerBussiness bussiness = new PlayerBussiness())
			{
				MarryInfo marryInfoSingle = bussiness.GetMarryInfoSingle(marryInfoID);
				if (marryInfoSingle == null)
				{
					translateId = "MarryInfoUpdateHandler.Msg1";
				}
				else
				{
					marryInfoSingle.IsPublishEquip = flag;
					marryInfoSingle.Introduction = str;
					marryInfoSingle.RegistTime = DateTime.Now;
					if (bussiness.UpdateMarryInfo(marryInfoSingle))
					{
						translateId = "MarryInfoUpdateHandler.Succeed";
					}
				}
				client.Out.SendMarryInfoRefresh(marryInfoSingle, marryInfoID, marryInfoSingle != null);
				client.Out.SendMessage(eMessageType.GM_NOTICE, LanguageMgr.GetTranslation(translateId));
			}
			return 0;
		}
	}
}
