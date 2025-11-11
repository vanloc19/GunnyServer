using Game.Base.Packets;

namespace Game.Server.Packets.Client
{
	[PacketHandler((int)ePackageType.CLEAR_STORE_BAG, "物品强化")]
	public class StoreClearItemHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			client.Player.ClearStoreBag();
			return 0;
		}
	}
}
