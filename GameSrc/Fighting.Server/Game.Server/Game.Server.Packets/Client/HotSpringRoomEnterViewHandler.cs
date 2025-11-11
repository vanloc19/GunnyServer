using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.Packets.Client
{
	[PacketHandler(201, "礼堂数据")]
	public class HotSpringRoomEnterViewHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			if (client.Player.CurrentHotSpringRoom != null)
			{
				GamePlayer[] allPlayers = client.Player.CurrentHotSpringRoom.GetAllPlayers();
				foreach (GamePlayer player in allPlayers)
				{
					GSPacketIn pkg = new GSPacketIn(198);
					pkg.WriteInt(player.PlayerCharacter.ID);
					pkg.WriteInt(player.PlayerCharacter.Grade);
					pkg.WriteInt(player.PlayerCharacter.Hide);
					pkg.WriteInt(player.PlayerCharacter.Repute);
					pkg.WriteString(player.PlayerCharacter.NickName);
					pkg.WriteByte(player.PlayerCharacter.typeVIP);
					pkg.WriteInt(player.PlayerCharacter.VIPLevel);
					pkg.WriteBoolean(player.PlayerCharacter.Sex);
					pkg.WriteString(player.PlayerCharacter.Style);
					pkg.WriteString(player.PlayerCharacter.Colors);
					pkg.WriteString(player.PlayerCharacter.Skin);
					pkg.WriteInt(player.Hot_X);
					pkg.WriteInt(player.Hot_Y);
					pkg.WriteInt(player.PlayerCharacter.FightPower);
					pkg.WriteInt(player.PlayerCharacter.Win);
					pkg.WriteInt(player.PlayerCharacter.Total);
					pkg.WriteInt(player.Hot_Direction);
					client.Player.SendTCP(pkg);
				}
			}
			return 0;
		}
	}
}
