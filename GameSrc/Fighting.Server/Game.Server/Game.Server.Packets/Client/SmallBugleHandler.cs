using System;
using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client
{
	[PacketHandler((int)ePackageType.S_BUGLE, "小喇叭")]
	public class SmallBugleHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			ItemInfo item = client.Player.PropBag.GetItemByCategoryID(0, 11, 4);
			if (item != null)
			{
				client.Player.PropBag.RemoveCountFromStack(item, 1);
				client.Player.OnUsingItem(item.TemplateID, 1);
				packet.ReadInt();
				packet.ReadString();
				string str = packet.ReadString();
				if (DateTime.Compare(client.Player.LastChatTime.AddSeconds(2.0), DateTime.Now) > 0)
				{
					client.Out.SendMessage(eMessageType.ChatERROR, LanguageMgr.GetTranslation("SmallBugleHandler.Msg"));
					return 1;
				}
				if (client.Player.PlayerCharacter.IsBanChat)
				{
					client.Out.SendMessage(eMessageType.Normal, "Bạn Bị Cấm Chat Từ QTV");
					return 1;
				}
				GSPacketIn @in = new GSPacketIn(71);
				@in.WriteInt(client.Player.PlayerCharacter.ID);
				@in.WriteString(client.Player.PlayerCharacter.NickName);
				@in.WriteString(str);
				GameServer.Instance.LoginServer.SendPacket(@in);
				client.Player.LastChatTime = DateTime.Now;
				GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
				foreach (GamePlayer player in allPlayers)
				{
					@in.ClientID = player.PlayerCharacter.ID;
					player.Out.SendTCP(@in);
				}
			}
			return 0;
		}
	}
}
