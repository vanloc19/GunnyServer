using System;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.GMActives;
using Game.Server.Managers;
using Game.Server.Managers.EliteGame;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client
{
	[PacketHandler(72, "大喇叭")]
	public class BigBugleHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			int templateId = packet.ReadInt();
			ItemInfo itemByTemplateID = client.Player.PropBag.GetItemByTemplateID(0, templateId);
			if (DateTime.Compare(client.Player.LastChatTime.AddSeconds(5.0), DateTime.Now) > 0)
			{
				client.Out.SendMessage(eMessageType.ChatERROR, LanguageMgr.GetTranslation("BigBugleHandler.Msg"));
				return 1;
			}
			if (client.Player.PlayerCharacter.IsBanChat)
			{
				client.Out.SendMessage(eMessageType.Normal, "Bạn Bị Cấm Chat Từ QTV");
				return 1;
			}
			if (client.Player.PlayerCharacter.HasBagPassword && client.Player.PlayerCharacter.IsLocked)
			{
				client.Out.SendMessage(eMessageType.GM_NOTICE, LanguageMgr.GetTranslation("Bag.Locked"));
				return 0;
			}
			GSPacketIn @in = new GSPacketIn(72);
			if (itemByTemplateID != null)
			{
				packet.ReadInt();
				packet.ReadString();
				string str = packet.ReadString();
				client.Player.PropBag.RemoveCountFromStack(itemByTemplateID, 1);
				@in.WriteInt(itemByTemplateID.Template.Property2);
				@in.WriteInt(client.Player.PlayerCharacter.ID);
				@in.WriteString(client.Player.PlayerCharacter.NickName);
				@in.WriteString(str);
				EliteGameMgr.TestChampionEliteGame(str);
				GameServer.Instance.LoginServer.SendPacket(@in);
				client.Player.LastChatTime = DateTime.Now;
				GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
				foreach (GamePlayer player in allPlayers)
				{
					@in.ClientID = player.PlayerCharacter.ID;
					player.Out.SendTCP(@in);
				}
			}
			else
			{
				packet.ReadString();
				string str2 = packet.ReadString();
				ItemInfo item = client.Player.PropBag.GetItemByCategoryID(0, 11, 4);
				client.Player.PropBag.RemoveCountFromStack(item, 1);
				@in.WriteInt(client.Player.ZoneId);
				@in.WriteInt(client.Player.PlayerCharacter.ID);
				@in.WriteString(client.Player.PlayerCharacter.NickName);
				@in.WriteString(str2);
				@in.WriteString(client.Player.ZoneName);
				GameServer.Instance.LoginServer.SendPacket(@in);
				client.Player.LastChatTime = DateTime.Now;
				GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
				foreach (GamePlayer player2 in allPlayers)
				{
					@in.ClientID = player2.PlayerCharacter.ID;
					player2.Out.SendTCP(@in);
				}
			}
			//GmActivityMgr.OnUpdateUseBigBugle(client.Player, 1);
			client.Player.OnUsingItem(templateId, 1);
			return 0;
		}
	}
}
