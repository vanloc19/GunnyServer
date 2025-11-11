using System;
using System.Collections.Generic;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client
{
	[PacketHandler(259, "FIRSTRECHARGE")]
	public class FirstRechargeGetAwardHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			packet.ReadInt();
			if (DateTime.Compare(client.Player.LastOpenCard.AddSeconds(0.5), DateTime.Now) > 0)
			{
				return 0;
			}
			string message = "FirstRechargeGetAward.Successfull";
			ProduceBussiness produceBussiness = new ProduceBussiness();
			EventRewardInfo[] eventsRewardInfo = produceBussiness.GetEventRewardInfoByType(6, 1);
			EventRewardGoodsInfo[] eventRewardGoodsByType = produceBussiness.GetEventRewardGoodsByType(6, 1);
			List<ItemInfo> items = new List<ItemInfo>();
			EventRewardGoodsInfo[] array = eventRewardGoodsByType;
			foreach (EventRewardGoodsInfo eventRewardGoods in array)
			{
				ItemInfo item = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(eventRewardGoods.TemplateId), 1, 104);
				item.StrengthenLevel = eventRewardGoods.StrengthLevel;
				item.AttackCompose = eventRewardGoods.AttackCompose;
				item.DefendCompose = eventRewardGoods.DefendCompose;
				item.AgilityCompose = eventRewardGoods.AgilityCompose;
				item.LuckCompose = eventRewardGoods.LuckCompose;
				item.IsBinds = eventRewardGoods.IsBind;
				item.Count = eventRewardGoods.Count;
				item.ValidDate = eventRewardGoods.ValidDate;
				items.Add(item);
			}
			if (!client.Player.PlayerCharacter.IsRecharged)
			{
				message = "FirstRechargeGetAward.NotCharge";
				return 0;
			}
			if (client.Player.PlayerCharacter.IsGetAward)
			{
				message = "FirstRechargeGetAward.AlreadyGetAward";
				return 0;
			}
			EventRewardInfo[] array2 = eventsRewardInfo;
			for (int i = 0; i < array2.Length; i++)
			{
				_ = array2[i];
				if (client.Player.PlayerCharacter.IsRecharged && !client.Player.PlayerCharacter.IsGetAward)
				{
					if (!client.Player.SendItemsToMail(items, LanguageMgr.GetTranslation("FirstRechargeGetAward.Content"), LanguageMgr.GetTranslation("FirstRechargeGetAward.Title"), eMailType.Manage))
					{
						message = "FirstRechargeGetAward.Error";
						return 0;
					}
					client.Player.PlayerCharacter.IsGetAward = true;
				}
			}
			client.Player.Out.SendUpdateFirstRecharge(client.Player.PlayerCharacter.IsRecharged, client.Player.PlayerCharacter.IsGetAward);
			client.Player.Out.SendMessage(eMessageType.GM_NOTICE, LanguageMgr.GetTranslation(message));
			client.Player.LastOpenCard = DateTime.Now;
			return 1;
		}
	}
}
