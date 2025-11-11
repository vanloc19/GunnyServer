using System;
using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client
{
	[PacketHandler(73, "大喇叭")]
	public class CBugleHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			int templateId = 11100;
			int clientId = packet.ReadInt();
			ItemInfo itemByTemplateID = client.Player.PropBag.GetItemByTemplateID(0, templateId);
			if (DateTime.Compare(client.Player.LastChatTime.AddSeconds(15.0), DateTime.Now) > 0)
			{
				client.Out.SendMessage(eMessageType.ChatERROR, LanguageMgr.GetTranslation("GoSlow"));
				return 1;
			}
			if (client.Player.PlayerCharacter.HasBagPassword && client.Player.PlayerCharacter.IsLocked)
			{
				client.Out.SendMessage(eMessageType.GM_NOTICE, LanguageMgr.GetTranslation("Bag.Locked"));
				return 0;
			}
			GSPacketIn @in = new GSPacketIn(73, clientId);
			if (itemByTemplateID != null)
			{
				packet.ReadString();
				string str = packet.ReadString();
				client.Player.PropBag.RemoveCountFromStack(itemByTemplateID, 1);
				@in.WriteInt(client.Player.ZoneId);
				@in.WriteInt(client.Player.PlayerCharacter.ID);
				@in.WriteString(client.Player.PlayerCharacter.NickName);
				@in.WriteString(str);
				@in.WriteString(client.Player.ZoneName);
				GameServer.Instance.LoginServer.SendPacket(@in);
				/*if (GameServer.Instance.WorldServer != null)
				{
					GameServer.Instance.WorldServer.SendPacket(@in);
				}
				else
				{
					
				}*/
				client.Player.LastChatTime = DateTime.Now;
			}
			return 0;
		}
	}
}