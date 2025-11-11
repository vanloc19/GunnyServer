using Bussiness;
using Game.Base.Packets;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client
{
	[PacketHandler(103, "DailyRecord")]
	public class DailyRecordHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn pkg)
		{
			PlayerBussiness playerB = new PlayerBussiness();
			DailyRecordInfo[] GetDailyRecord = playerB.GetDailyRecord(client.Player.PlayerCharacter.ID);
			int length = GetDailyRecord.Length;
			GSPacketIn packet = new GSPacketIn(103, client.Player.PlayerId);
			packet.WriteInt(length);
			for (int i = 0; i < length; i++)
			{
				DailyRecordInfo Info = GetDailyRecord[i];
				packet.WriteInt(Info.Type);
				packet.WriteString(Info.Value);
				playerB.DeleteDailyRecord(client.Player.PlayerId, Info.Type);
			}
			client.Out.SendTCP(packet);
			return 1;
		}

		private bool isUpdate(int type)
		{
			if ((uint)(type - 10) <= 10u)
			{
				return true;
			}
			return false;
		}
	}
}
