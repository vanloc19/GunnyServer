using Bussiness;
using Game.Base.Packets;

namespace Game.Server.Packets.Client
{
	[PacketHandler(12, "礼堂数据")]
	public class HotSpringRoomTimeAdded : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			byte b = packet.ReadByte();
			if (b == 11)
			{
				int spaAddictionMoneyNeeded = 2500;//GameProperties.SpaAddictionMoneyNeeded;
				if (client.Player.Extra.Info.BuyTimeHotSpringCount >= 5)
				{
					client.Player.SendMessage("Đã hết lượt gia hạn suối ngày hôm nay vui lòng thử lại vào ngày mai!");
				}
				else
				{
					if (client.Player.PlayerCharacter.Money < spaAddictionMoneyNeeded)
					{
						client.Player.SendMessage("Xu của bạn không đủ.");
					}
					else
					{
						int spaPriRoomContinueTime = GameProperties.SpaPriRoomContinueTime;
						client.Player.RemoveMoney(spaAddictionMoneyNeeded, isConsume: true);
						client.Player.Extra.Info.MinHotSpring += spaPriRoomContinueTime;
						client.Player.Extra.Info.BuyTimeHotSpringCount++;
						GSPacketIn gSPacketIn = new GSPacketIn(191);
						gSPacketIn.WriteByte(12);
						client.Player.SendTCP(gSPacketIn);
						client.Player.SendMessage(string.Format("Gia hạn thành công! bạn bị trừ {0} Xu và nhận được thêm {1} Phút, số lần gia hạn còn lại là {2}/5.", spaAddictionMoneyNeeded, spaPriRoomContinueTime, client.Player.Extra.Info.BuyTimeHotSpringCount));
					}
				}
			}
			return 0;
		}
	}
}
