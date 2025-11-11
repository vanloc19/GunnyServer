using Game.Base.Packets;

namespace Game.Server.Packets.Client
{
	[PacketHandler(132, "场景用户离开")]
	public class BattleGroundHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			byte num = packet.ReadByte();
			_ = client.Player.BattleData.LevelLimit;
			GSPacketIn @in = new GSPacketIn(132, client.Player.PlayerCharacter.ID);
			switch (num)
			{
			case 3:
			{
				byte val = packet.ReadByte();
				@in.WriteByte(3);
				@in.WriteBoolean(val: true);
				@in.WriteByte(val);
				switch (val)
				{
				case 2:
					@in.WriteInt(client.Player.BattleData.GetRank());
					break;
				case 1:
					if (client.Player.BattleData.MatchInfo == null)
					{
						@in.WriteInt(0);
						@in.WriteInt(0);
						@in.WriteInt(client.Player.BattleData.fairBattleDayPrestige);
					}
					else
					{
						@in.WriteInt(client.Player.BattleData.MatchInfo.addDayPrestge);
						@in.WriteInt(client.Player.BattleData.MatchInfo.totalPrestige);
						@in.WriteInt(client.Player.BattleData.fairBattleDayPrestige);
					}
					break;
				}
				client.Player.Out.SendTCP(@in);
				break;
			}
			case 5:
				@in.WriteByte(5);
				@in.WriteInt(client.Player.BattleData.Attack);
				@in.WriteInt(client.Player.BattleData.Defend);
				@in.WriteInt(client.Player.BattleData.Agility);
				@in.WriteInt(client.Player.BattleData.Lucky);
				@in.WriteInt(client.Player.BattleData.Damage);
				@in.WriteInt(client.Player.BattleData.Guard);
				@in.WriteInt(client.Player.BattleData.Blood);
				@in.WriteInt(client.Player.BattleData.Energy);
				client.Player.Out.SendTCP(@in);
				break;
			}
			return 0;
		}
	}
}
