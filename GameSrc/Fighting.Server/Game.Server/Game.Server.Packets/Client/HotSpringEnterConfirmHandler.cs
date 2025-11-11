using Bussiness;
using Game.Base.Packets;
using Game.Server.Managers;

namespace Game.Server.Packets.Client
{
	[PacketHandler(212, "礼堂数据")]
	public class HotSpringEnterConfirmHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			int id = packet.ReadInt();
			if (client.Player.CurrentHotSpringRoom == null)
			{
				if (HotSpringMgr.GetHotSpringRoombyID(id) != null)
				{
					GSPacketIn @in = new GSPacketIn(212);
					@in.WriteInt(id);
					client.Out.SendTCP(@in);
				}
				else
				{
					client.Player.SendMessage(LanguageMgr.GetTranslation("SpaRoomLoginHandler.Failed4"));
				}
			}
			return 0;
		}
	}
}
