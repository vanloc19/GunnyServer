using System;
using System.Collections.Generic;
using Bussiness.Managers;
using Game.Logic;
using Game.Server.GameObjects;
using Game.Server.Statics;
using SqlDataProvider.Data;

namespace Game.Server.Quests
{
	public class TurnPropertyCondition : BaseCondition
	{
		private GamePlayer m_player;

		private BaseQuest m_quest;

		public TurnPropertyCondition(BaseQuest quest, QuestConditionInfo info, int value)
			: base(quest, info, value)
		{
			m_quest = quest;
		}

		public override void AddTrigger(GamePlayer player)
		{
			m_player = player;
			player.GameKillDrop += QuestDropItem;
			base.AddTrigger(player);
		}

		public override bool CancelFinish(GamePlayer player)
		{
			ItemTemplateInfo goods = ItemMgr.FindItemTemplate(m_info.Para1);
			if (goods != null)
			{
				ItemInfo cloneItem = ItemInfo.CreateFromTemplate(goods, m_info.Para2, 117);
				return player.AddTemplate(cloneItem, eBageType.TempBag, m_info.Para2, eGameView.OtherTypeGet);
			}
			return false;
		}

		public override bool Finish(GamePlayer player)
		{
			if(player.PropBag.CountTotalEmptySlot() <= 5 || player.EquipBag.CountTotalEmptySlot() <= 5)
            {
				player.SendMessage("Trang bị hoặc đạo cụ đã đầy vui lòng sắp xếp lại để hoàn thành nhiệm vụ.");
				return false;
            }				
			return player.RemoveTemplate(m_info.Para1, m_info.Para2);
		}

		public override bool IsCompleted(GamePlayer player)
		{
			bool flag = false;
			if (player.GetItemCount(m_info.Para1) >= m_info.Para2)
			{
				base.Value = 0;
				flag = true;
			}
			return flag;
		}

		private void QuestDropItem(AbstractGame game, int copyId, int npcId, bool playResult)
		{
			if (m_player.GetItemCount(m_info.Para1) >= m_info.Para2)
			{
				return;
			}
			List<ItemInfo> list = null;
			int gold = 0;
			int money = 0;
			int giftToken = 0;
			if (game is PVEGame)
			{
				DropInventory.PvEQuestsDrop(npcId, ref list);
			}
			if (game is PVPGame)
			{
				DropInventory.PvPQuestsDrop(game.RoomType, playResult, ref list);
			}
			if (list == null)
			{
				return;
			}
			foreach (ItemInfo info in list)
			{
				ItemInfo.FindSpecialItemInfo(info, ref gold, ref money, ref giftToken);
				if (info != null)
				{
					m_player.TempBag.AddTemplate(info, info.Count);
				}
			}
			m_player.AddGold(gold);
			m_player.AddGiftToken(giftToken);
			m_player.AddMoney(money);
			LogMgr.LogMoneyAdd(LogMoneyType.Award, LogMoneyType.Award_Drop, m_player.PlayerCharacter.ID, money, m_player.PlayerCharacter.Money, 0, 0, 0, "", "", "");
		}

		public override void RemoveTrigger(GamePlayer player)
		{
			player.GameKillDrop -= QuestDropItem;
			base.RemoveTrigger(player);
		}
	}
}
