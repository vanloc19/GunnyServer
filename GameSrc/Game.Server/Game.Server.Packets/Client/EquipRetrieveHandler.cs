using System;
using System.Collections.Generic;
using System.Reflection;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Logic;
using Game.Server.GameUtils;
using log4net;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client
{
	[PacketHandler(222, "防沉迷系统开关")]
	public class EquipRetrieveHandler : IPacketHandler
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		private ThreadSafeRandom random = new ThreadSafeRandom();

		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			int probaility = 0;
			PlayerInventory inventory = client.Player.GetInventory(eBageType.Store);
			int totalQuality = 0;
			bool isBind = false;
			bool isWow = false;
			bool isTexp = false;
			bool isSuper = false;
			for (int i = 1; i < 5; i++)
			{
				ItemInfo item = inventory.GetItemAt(i);
				if (item != null)
				{
					inventory.RemoveItemAt(i);
				}
				if (item.IsBinds)
				{
					isBind = true;
				}
				#region isWow
				if (item.TemplateID >= 7015 && item.TemplateID <= 7023)
					isWow = true;
				if (item.TemplateID >= 7039 && item.TemplateID <= 7040)
					isWow = true;
				if (item.TemplateID >= 7045 && item.TemplateID <= 7048)
					isWow = true;
				if (item.TemplateID == 7050 || item.TemplateID == 7053 || item.TemplateID == 7055 || item.TemplateID == 7059 || item.TemplateID == 7060)
					isWow = true;
				if (item.TemplateID == 7065 || item.TemplateID == 7066 || item.TemplateID >= 7069 && item.TemplateID <= 7081 || item.TemplateID == 7087
					|| item.TemplateID == 7093 || item.TemplateID == 7098 || item.TemplateID == 7100 || item.TemplateID == 7142 || item.TemplateID == 7148
					|| item.TemplateID == 7186 || item.TemplateID == 7189 || item.TemplateID == 7190 || item.TemplateID == 7204 || item.TemplateID == 7209
					|| item.TemplateID == 7212 || item.TemplateID == 7218)
					isWow = true;
				#endregion

				#region
				int[] weaponSuper = { 7024, 7026, 7027,7031,7032,7041,7063,7067,7089,7090,7092,7094,7113,7114,7115,7117,7118,
				7119,7120,7121,7122,7124,7125,7126,7127,7130,7131,7132,7138,7140,7146,7150,7152,7156,7166,7176,
				7181,7183,7185,7191,7199,7205,7207,7210,7213,7214,7219,7221,7223,7225,7227,7230};
				for (int j = 0; j < weaponSuper.Length; j++)
				{
					//Console.WriteLine($"WTF = {weaponSuper[j]} >>> itemTemp = {item.TemplateID}");
					if (item.TemplateID == weaponSuper[j])
					{
						isSuper = true;
					}
				}

				#endregion

				if (item.TemplateID >= 40001 && item.TemplateID <= 40003)
				{
					isTexp = true;
				}
				totalQuality += item.Template.Quality;
			}
			if (isTexp)
			{
				switch (totalQuality)
				{
					case 12:
					case 16:
					case 20:
						probaility = totalQuality + 10;
						break;
				}
			}
			else if (isWow)
			{
				probaility = 25;
			}
			else if (isSuper)
			{
				probaility = 50;
			}
			else
			{
				switch (totalQuality)
				{
					case 8:
					case 12:
					case 16:
					case 20:
						probaility = totalQuality;
						break;
				}
			}
			List<ItemInfo> infos = null;
			DropInventory.RetrieveDrop(probaility, ref infos);
			int index = random.Next(infos.Count);
			int TemplateID = infos[index].TemplateID;
			ItemInfo RecycleItem = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(TemplateID), 1, 105);
			RecycleItem.IsBinds = isBind;
			RecycleItem.BeginDate = DateTime.Now;
			if (RecycleItem.Template.CategoryID != 11 && RecycleItem.Template.CategoryID != 20)
			{
				RecycleItem.ValidDate = 30;
				RecycleItem.IsBinds = true;
			}
			RecycleItem.IsBinds = true;
			inventory.AddItemTo(RecycleItem, 0);
			return 1;
			#region OLD
			/*PlayerInventory inventory = client.Player.GetInventory(eBageType.Store);
			int num1 = 0;
			for (int slot = 1; slot < 5; slot++)
			{
				ItemInfo itemAt = inventory.GetItemAt(slot);
				if (itemAt != null)
				{
					num1 += itemAt.Template.Quality;
					continue;
				}
				client.Player.SendMessage("Itens insuficientes para refinar");
				return 0;
			}
			inventory.ClearBag();
			int user = ((num1 <= 5) ? 1 : ((num1 <= 15) ? 2 : ((num1 >= 20) ? 4 : 3)));
			List<ItemInfo> info = null;
			DropInventory.RetrieveDrop(user, ref info);
			int index = random.Next(info.Count);
			ItemInfo fromTemplate = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(info[index].TemplateID), 1, 105);
			fromTemplate.IsBinds = info[index].IsBinds;
			fromTemplate.BeginDate = DateTime.Now;
			fromTemplate.ValidDate = info[index].ValidDate;
			if (inventory.AddItemTo(fromTemplate, 0))
			{
				client.Player.RemoveGold(400);
			}*/
			#endregion
		}
	}
}
