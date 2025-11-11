using System.Collections.Generic;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client
{
	[PacketHandler((byte)ePackageType.USE_COLOR_CARD, "改变物品颜色")]
	public class UserChangeItemColorHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			eMessageType normal = eMessageType.GM_NOTICE;
			string translateId = "UserChangeItemColorHandler.Success";
			packet.ReadInt();
			int slot = packet.ReadInt();
			packet.ReadInt();
			int num2 = packet.ReadInt();
			string str2 = packet.ReadString();
			string str3 = packet.ReadString();
			int templateId = packet.ReadInt();
			ItemInfo itemAt = client.Player.EquipBag.GetItemAt(num2);
			ItemInfo item = client.Player.PropBag.GetItemAt(slot);
			if (itemAt != null)
			{
				client.Player.BeginChanges();
				try
				{
					bool flag = false;
					if (item != null && item.IsValidItem())
					{
						client.Player.PropBag.RemoveCountFromStack(item, 1);
						flag = true;
					}
					else
					{
						ItemMgr.FindItemTemplate(templateId);
						List<ShopItemInfo> list = ShopMgr.FindShopbyTemplatID(templateId);
						int num3 = 0;
						for (int i = 0; i < list.Count; i++)
						{
							if (list[i].APrice1 == -1 && list[i].AValue1 != 0)
							{
								num3 = list[i].AValue1;
							}
						}
						if (num3 <= client.Player.PlayerCharacter.Money + client.Player.PlayerCharacter.MoneyLock)
						{
							client.Player.RemoveMoney(num3, isConsume: true);
							flag = true;
						}
					}
					if (flag)
					{
						itemAt.Color = ((str2 == null) ? "" : str2);
						itemAt.Skin = ((str3 == null) ? "" : str3);
						client.Player.EquipBag.UpdateItem(itemAt);
					}
				}
				finally
				{
					client.Player.CommitChanges();
				}
			}
			client.Out.SendMessage(normal, LanguageMgr.GetTranslation(translateId));
			return 0;
		}
	}
}
