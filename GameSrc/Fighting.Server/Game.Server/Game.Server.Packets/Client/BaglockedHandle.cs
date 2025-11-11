using System;
using Game.Base.Packets;

namespace Game.Server.Packets.Client
{
	[PacketHandler(403, "二级密码")]
	public class BaglockedHandle : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			int num = packet.ReadByte();
			GSPacketIn @in = new GSPacketIn(403, client.Player.PlayerCharacter.ID);
			if (num == 5)
			{
				@in.WriteByte(5);
				@in.WriteBoolean(val: true);
				client.Out.SendTCP(@in);
			}
			else
			{
				Console.WriteLine("BaglockedPackageType." + (BaglockedPackageType)num);
			}
			return 0;
		}
	}
}
