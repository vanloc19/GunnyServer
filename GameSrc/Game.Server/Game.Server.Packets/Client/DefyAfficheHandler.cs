using System;
using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;

namespace Game.Server.Packets.Client
{
	[PacketHandler(123, "场景用户离开")]
	public class DefyAfficheHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			string str = packet.ReadString();
			int num = 500;
			if (client.Player.PlayerCharacter.Money + client.Player.PlayerCharacter.MoneyLock >= num)
			{
				client.Player.RemoveMoney(num, isConsume: false);
				GSPacketIn @in = new GSPacketIn(123);
				@in.WriteString(str);
				GameServer.Instance.LoginServer.SendPacket(@in);
				client.Player.LastChatTime = DateTime.Now;
				GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
				foreach (GamePlayer player in allPlayers)
				{
					@in.ClientID = player.PlayerCharacter.ID;
					player.Out.SendTCP(@in);
				}
				client.Player.OnPlayerDispatches();
			}
			else
			{
				client.Out.SendMessage(eMessageType.ChatERROR, LanguageMgr.GetTranslation("UserBuyItemHandler.Money"));
			}
			return 0;
		}
	}
}
