using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;

namespace Game.Server.Packets.Client
{
	[PacketHandler((int)ePackageType.SCENE_USERS_LIST, "用户列表")]
	public class SceneUsersListHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			GSPacketIn packet2 = packet.Clone();
			packet2.ClearContext();
			byte num1 = packet.ReadByte();
			byte num2 = packet.ReadByte();
			GamePlayer[] allPlayersNoGame = WorldMgr.GetAllPlayersNoGame();
			int length = allPlayersNoGame.Length;
			byte val = ((length > num2) ? num2 : ((byte)length));
			packet2.WriteByte(val);
			for (int index = num1 * num2; index < num1 * num2 + val; index++)
			{
				GamePlayer gamePlayer = allPlayersNoGame[index % length];
				packet2.WriteInt(gamePlayer.PlayerCharacter.ID);
				packet2.WriteString((gamePlayer.PlayerCharacter.NickName == null) ? "" : gamePlayer.PlayerCharacter.NickName);
				packet2.WriteByte(gamePlayer.PlayerCharacter.typeVIP);
				packet2.WriteInt(gamePlayer.PlayerCharacter.VIPLevel);
				packet2.WriteBoolean(gamePlayer.PlayerCharacter.Sex);
				packet2.WriteInt(gamePlayer.PlayerCharacter.Grade);
				packet2.WriteInt(gamePlayer.PlayerCharacter.ConsortiaID);
				packet2.WriteString((gamePlayer.PlayerCharacter.ConsortiaName == null) ? "" : gamePlayer.PlayerCharacter.ConsortiaName);
				packet2.WriteInt(gamePlayer.PlayerCharacter.Offer);
				packet2.WriteInt(gamePlayer.PlayerCharacter.Win);
				packet2.WriteInt(gamePlayer.PlayerCharacter.Total);
				packet2.WriteInt(gamePlayer.PlayerCharacter.Escape);
				packet2.WriteInt(gamePlayer.PlayerCharacter.Repute);
				packet2.WriteInt(gamePlayer.PlayerCharacter.FightPower);
			}
			client.Out.SendTCP(packet2);
			return 0;
		}
	}
}
