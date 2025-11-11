using System;
using Bussiness;
using Game.Base.Packets;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client
{
	[PacketHandler(236, "添加征婚信息")]
	public class MarryInfoAddHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			if (client.Player.PlayerCharacter.MarryInfoID != 0)
			{
				return 1;
			}
			bool flag = packet.ReadBoolean();
			string str = packet.ReadString();
			int iD = client.Player.PlayerCharacter.ID;
			eMessageType normal = eMessageType.GM_NOTICE;
			string translateId = "MarryInfoAddHandler.Fail";
			int num2 = 10000;
			if (num2 > client.Player.PlayerCharacter.Gold)
			{
				normal = eMessageType.BIGBUGLE_NOTICE;
				translateId = "MarryInfoAddHandler.Msg1";
			}
			else
			{
				MarryInfo info = new MarryInfo
				{
					UserID = iD,
					IsPublishEquip = flag,
					Introduction = str,
					RegistTime = DateTime.Now
				};
				using (PlayerBussiness bussiness = new PlayerBussiness())
				{
					if (bussiness.AddMarryInfo(info))
					{
						client.Player.RemoveGold(num2);
						translateId = "MarryInfoAddHandler.Msg2";
						client.Player.PlayerCharacter.MarryInfoID = info.ID;
						client.Out.SendMarryInfoRefresh(info, info.ID, isExist: true);
					}
				}
			}
			client.Out.SendMessage(normal, LanguageMgr.GetTranslation(translateId));
			return 0;
		}
	}
}
