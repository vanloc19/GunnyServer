using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using Game.Server.Packets;

namespace Game.Server.Consortia.Handle
{
	[global::Consortia(20)]
	public class ConsortiaChat : IConsortiaCommandHadler
	{
		public int CommandHandler(GamePlayer Player, GSPacketIn packet)
		{
			if (Player.PlayerCharacter.ConsortiaID == 0)
			{
				return 0;
			}
			if (Player.PlayerCharacter.IsBanChat)
			{
				Player.Out.SendMessage(eMessageType.GM_NOTICE, LanguageMgr.GetTranslation("ConsortiaChatHandler.IsBanChat"));
				return 1;
			}
			packet.ClientID = Player.PlayerCharacter.ID;
			packet.ReadByte();
			packet.ReadString();
			packet.ReadString();
			packet.WriteInt(Player.PlayerCharacter.ConsortiaID);
			GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
			foreach (GamePlayer allPlayer in allPlayers)
			{
				if (allPlayer.PlayerCharacter.ConsortiaID == Player.PlayerCharacter.ConsortiaID)
				{
					allPlayer.Out.SendTCP(packet);
				}
			}
			GameServer.Instance.LoginServer.SendPacket(packet);
			return 0;
		}
	}
}
