using Game.Base.Packets;
using Game.Server.Managers;

namespace Game.Server.Packets.Client
{
	[PacketHandler((byte)ePackageType.ITEM_STORE, "储存物品")]
	public class StoreItemHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			if (client.Player.PlayerCharacter.ConsortiaID == 0)
			{
				return 1;
			}
			int num = packet.ReadByte();
			int num2 = packet.ReadInt();
			packet.ReadInt();
			if (num == 0 && num2 < 31)
			{
				return 1;
			}
			if (ConsortiaMgr.FindConsortiaInfo(client.Player.PlayerCharacter.ConsortiaID) != null)
			{
				_ = client.Player.ConsortiaBag;
				client.Player.GetInventory((eBageType)num);
			}
			return 0;
		}
	}
}
