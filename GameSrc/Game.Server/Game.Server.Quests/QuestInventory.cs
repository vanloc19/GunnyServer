using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;
using Bussiness;
using Bussiness.Managers;
using Game.Server.Buffer;
using Game.Server.GameObjects;
using Game.Server.Packets;
using log4net;
using SqlDataProvider.Data;

namespace Game.Server.Quests
{
	public class QuestInventory
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		protected ArrayList m_clearList;

		private UnicodeEncoding m_converter = new UnicodeEncoding();

		private int m_changeCount;

		protected List<BaseQuest> m_changedQuests = new List<BaseQuest>();

		protected List<QuestDataInfo> m_datas;

		protected List<BaseQuest> m_list;

		private object m_lock;

		private GamePlayer m_player;

		private byte[] m_states;

		public QuestInventory(GamePlayer player)
		{
			m_player = player;
			m_lock = new object();
			m_list = new List<BaseQuest>();
			m_clearList = new ArrayList();
			m_datas = new List<QuestDataInfo>();
		}

		private bool AddQuest(BaseQuest quest)
		{
			lock (m_list)
			{
				m_list.Add(quest);
			}
			quest.CheckRepeat();
			OnQuestsChanged(quest);
			quest.AddToPlayer(m_player);
			return true;
		}

		public bool AddQuest(QuestInfo info, out string msg)
		{
			msg = "";
			try
			{
				if (info == null)
				{
					msg = "Game.Server.Quests.NoQuest";
					return false;
				}
				if (info.TimeMode && DateTime.Now.CompareTo(info.StartDate) < 0)
				{
					msg = "Game.Server.Quests.NoTime";
				}
				if (info.TimeMode && DateTime.Now.CompareTo(info.EndDate) > 0)
				{
					msg = "Game.Server.Quests.TimeOver";
				}
				if (m_player.PlayerCharacter.Grade < info.NeedMinLevel)
				{
					msg = "Game.Server.Quests.LevelLow";
				}
				if (m_player.PlayerCharacter.Grade > info.NeedMaxLevel)
				{
					msg = "Game.Server.Quests.LevelTop";
				}
				if (info.PreQuestID != "0,")
				{
					string[] strArray = info.PreQuestID.Split(',');
					for (int i = 0; i < strArray.Length - 1; i++)
					{
						if (!IsQuestFinish(Convert.ToInt32(strArray[i])))
						{
							msg = "Game.Server.Quests.NoFinish";
						}
					}
				}
			}
			catch (Exception exception)
			{
				log.Info(exception.InnerException);
			}
			if (info.IsOther == 1 && !m_player.PlayerCharacter.IsConsortia)
			{
				msg = "Game.Server.Quest.QuestInventory.HaveMarry";
			}
			if (info.IsOther == 2 && !m_player.PlayerCharacter.IsMarried)
			{
				msg = "Game.Server.Quest.QuestInventory.HaveMarry";
			}
			BaseQuest quest = FindQuest(info.ID);
			if (quest != null && quest.Data.IsComplete)
			{
				msg = "Game.Server.Quests.Have";
			}
			if (quest != null && !quest.Info.CanRepeat)
			{
				msg = "Game.Server.Quests.NoRepeat";
			}
			if (quest != null && DateTime.Now.CompareTo(quest.Data.CompletedDate.Date.AddDays(quest.Info.RepeatInterval)) < 0 && quest.Data.RepeatFinish < 1)
			{
				msg = "Game.Server.Quests.Rest";
			}
			if (m_player.QuestInventory.FindQuest(info.ID) != null)
			{
				msg = "Game.Server.Quests.Have";
			}
			if (msg == "")
			{
				QuestMgr.GetQuestCondiction(info);
				int rand = 1;
				if ((decimal)ThreadSafeRandom.NextStatic(1000000) <= info.Rands)
				{
					rand = info.RandDouble;
				}
				BeginChanges();
				if (quest == null)
				{
					quest = new BaseQuest(info, new QuestDataInfo());
					AddQuest(quest);
					quest.Reset(m_player, rand);
				}
				else
				{
					quest.Reset(m_player, rand);
					quest.AddToPlayer(m_player);
					OnQuestsChanged(quest);
				}
				CommitChanges();
				return true;
			}
			msg = LanguageMgr.GetTranslation(msg);
			return false;
		}

		private bool AddQuestData(QuestDataInfo data)
		{
			lock (m_list)
			{
				m_datas.Add(data);
			}
			return true;
		}

		private void BeginChanges()
		{
			Interlocked.Increment(ref m_changeCount);
		}

		public bool ClearConsortiaQuest()
		{
			return true;
		}

		public bool ClearMarryQuest()
		{
			return true;
		}

		private void CommitChanges()
		{
			int num = Interlocked.Decrement(ref m_changeCount);
			if (num < 0)
			{
				if (log.IsErrorEnabled)
				{
					log.Error("Inventory changes counter is bellow zero (forgot to use BeginChanges?)!\n\n" + Environment.StackTrace);
				}
				Thread.VolatileWrite(ref m_changeCount, 0);
			}
			if (num <= 0 && m_changedQuests.Count > 0)
			{
				UpdateChangedQuests();
			}
		}

		public bool FindFinishQuestData(int ID, int UserID)
		{
			bool isComplete = false;
			lock (m_datas)
			{
				foreach (QuestDataInfo info in m_datas)
				{
					if (info.QuestID == ID && info.UserID == UserID)
					{
						isComplete = info.IsComplete;
					}
				}
				return isComplete;
			}
		}

		public BaseQuest FindQuest(int id)
		{
			foreach (BaseQuest quest in m_list)
			{
				if (quest.Info.ID == id)
				{
					return quest;
				}
			}
			return null;
		}

		public bool Finish(BaseQuest baseQuest, int selectedItem)
		{
			string message = "";
			QuestInfo info = baseQuest.Info;
			QuestDataInfo data = baseQuest.Data;
			m_player.BeginAllChanges();
			try
			{
				if (baseQuest.Finish(m_player))
				{
					List<QuestAwardInfo> questGoods = QuestMgr.GetQuestGoods(info);
					List<ItemInfo> list2 = new List<ItemInfo>();
					List<ItemInfo> list3 = new List<ItemInfo>();
					List<ItemInfo> list4 = new List<ItemInfo>();
					List<ItemInfo> items = new List<ItemInfo>();
					foreach (QuestAwardInfo info2 in questGoods)
					{
						if (info2.IsSelect && info2.RewardItemID != selectedItem)
						{
							continue;
						}
						ItemTemplateInfo goods = ItemMgr.FindItemTemplate(info2.RewardItemID);
						if (goods == null)
						{
							continue;
						}
						message = message + LanguageMgr.GetTranslation("Game.Server.Quests.FinishQuest.RewardProp", goods.Name, info2.RewardItemCount) + " ";
						int NeedSex = (m_player.PlayerCharacter.Sex ? 1 : 2);
						if (goods.NeedSex != 0 && goods.NeedSex != NeedSex)
						{
							continue;
						}
						int rewardItemCount = info2.RewardItemCount;
						if (info2.IsCount)
                        {
							rewardItemCount *= data.RandDobule;
                        }
						for (int i = 0; i < rewardItemCount; i += goods.MaxCount)
						{
							int count = ((i + goods.MaxCount > info2.RewardItemCount) ? (info2.RewardItemCount - i) : goods.MaxCount);
							ItemInfo item = ItemInfo.CreateFromTemplate(goods, count, 106);
							if (item != null)
							{
								item.ValidDate = info2.RewardItemValid;
								item.IsBinds = true;
								item.StrengthenLevel = info2.StrengthenLevel;
								item.AttackCompose = info2.AttackCompose;
								item.DefendCompose = info2.DefendCompose;
								item.AgilityCompose = info2.AgilityCompose;
								item.LuckCompose = info2.LuckCompose;
								if (goods.BagType == eBageType.PropBag)
								{
									list3.Add(item);
								}
								else if (goods.BagType == eBageType.FarmBag)
                                {
									list4.Add(item);
                                }									
								else
								{
									list2.Add(item);
								}
								if (goods.TemplateID == 11408)
								{
									m_player.LoadMedals();
									m_player.OnPlayerAddItem("Medal", count);
								}
							}
						}
					}
					if (list2.Count > 0 && m_player.EquipBag.GetEmptyCount() < list2.Count)
					{
						baseQuest.CancelFinish(m_player);
						m_player.Out.SendMessage(eMessageType.BIGBUGLE_NOTICE, m_player.GetInventoryName(eBageType.EquipBag) + LanguageMgr.GetTranslation("Game.Server.Quests.BagFull") + " ");
						return false;
					}
					if (list3.Count > 0 && m_player.PropBag.GetEmptyCount() < list3.Count)
					{
						baseQuest.CancelFinish(m_player);
						m_player.Out.SendMessage(eMessageType.BIGBUGLE_NOTICE, m_player.GetInventoryName(eBageType.PropBag) + LanguageMgr.GetTranslation("Game.Server.Quests.BagFull") + " ");
						return false;
					}
					if (list4.Count > 0 && m_player.FarmBag.GetEmptyCount() < list4.Count)
                    {
						baseQuest.CancelFinish(m_player);
						m_player.Out.SendMessage(eMessageType.BIGBUGLE_NOTICE, m_player.GetInventoryName(eBageType.FarmBag) + LanguageMgr.GetTranslation("Game.Server.Quests.BagFull") + " ");
						return false;
					}
					foreach (ItemInfo info3 in list2)
					{
						if (!m_player.EquipBag.StackItemToAnother(info3) && !m_player.EquipBag.AddItem(info3))
						{
							items.Add(info3);
						}
					}
					foreach (ItemInfo info4 in list3)
					{
						if (info4.TemplateID == 11408)
						{
							info4.Count *= data.RandDobule;
						}
						if (info4.Template.CategoryID != 10)
						{
							if (!m_player.PropBag.StackItemToAnother(info4) && !m_player.PropBag.AddItem(info4))
							{
								items.Add(info4);
							}
							continue;
						}
						switch (info4.TemplateID)
						{
						case 10001:
							m_player.PlayerCharacter.openFunction(Step.PICK_TWO_TWENTY);
							break;
						case 10003:
							m_player.PlayerCharacter.openFunction(Step.POP_WIN);
							break;
						case 10004:
							m_player.PlayerCharacter.openFunction(Step.FIFTY_OPEN);
							m_player.AddGift(eGiftType.MONEY);
							m_player.AddGift(eGiftType.BIG_EXP);
							m_player.AddGift(eGiftType.PET_EXP);
							break;
						case 10005:
							m_player.PlayerCharacter.openFunction(Step.FORTY_OPEN);
							break;
						case 10006:
							m_player.PlayerCharacter.openFunction(Step.THIRTY_OPEN);
							break;
						case 10007:
							m_player.PlayerCharacter.openFunction(Step.POP_TWO_TWENTY);
							m_player.AddGift(eGiftType.SMALL_EXP);
							break;
						case 10008:
							m_player.PlayerCharacter.openFunction(Step.GAIN_TEN_PERSENT);
							break;
						case 10024:
							m_player.PlayerCharacter.openFunction(Step.PICK_ONE);
							break;
						case 10025:
							m_player.PlayerCharacter.openFunction(Step.PLANE_OPEN);
							break;
						}
					}
					foreach (ItemInfo info5 in list4)
                    {
						if (!m_player.FarmBag.StackItemToAnother(info5) && !m_player.FarmBag.AddItem(info5))
                        {
							items.Add(info5);
                        }
					}						
					if (items.Count > 0)
					{
						m_player.SendItemsToMail(items, "Túi đầy", "Vật phẩm được gửi từ hệ thống", eMailType.ItemOverdue);
						m_player.Out.SendMailResponse(m_player.PlayerCharacter.ID, eMailRespose.Receiver);
					}
					message = LanguageMgr.GetTranslation("Game.Server.Quests.FinishQuest.Reward") + message;
					if (info.RewardBuffID > 0 && info.RewardBuffDate > 0)
					{
						ItemTemplateInfo template = ItemMgr.FindItemTemplate(info.RewardBuffID);
						if (template != null)
						{
							int validHour = info.RewardBuffDate * data.RandDobule;
							BufferList.CreateBufferHour(template, validHour).Start(m_player);
							message = message + LanguageMgr.GetTranslation("Game.Server.Quests.FinishQuest.RewardBuff", template.Name, validHour) + " ";
						}
					}
					if (info.RewardGold != 0)
					{
						int num12 = info.RewardGold * data.RandDobule;
						m_player.AddGold(num12);
						message = message + LanguageMgr.GetTranslation("Game.Server.Quests.FinishQuest.RewardGold", num12) + " ";
					}
					if (info.RewardMoney != 0)
					{
						int num13 = info.RewardMoney * data.RandDobule;
						m_player.AddMoney(info.RewardMoney * data.RandDobule);
						message = message + LanguageMgr.GetTranslation("Game.Server.Quests.FinishQuest.RewardMoney", num13) + " ";
					}
					if (info.RewardGP != 0)
					{
						int gp = info.RewardGP * data.RandDobule;
						m_player.AddGP(gp, false, false);
						message = message + LanguageMgr.GetTranslation("Game.Server.Quests.FinishQuest.RewardGB1", gp) + " ";
					}
					if (info.RewardRiches != 0 && m_player.PlayerCharacter.ConsortiaID != 0)
					{
						int num14 = info.RewardRiches * data.RandDobule;
						m_player.AddRichesOffer(num14);
						using (ConsortiaBussiness bussiness = new ConsortiaBussiness())
						{
							bussiness.ConsortiaRichAdd(m_player.PlayerCharacter.ConsortiaID, ref num14);
						}
						message = message + LanguageMgr.GetTranslation("Game.Server.Quests.FinishQuest.RewardRiches", num14) + " ";
					}
					if (info.RewardOffer != 0)
					{
						int num10 = info.RewardOffer * data.RandDobule;
						m_player.AddOffer(num10, IsRate: false);
						message = message + LanguageMgr.GetTranslation("Game.Server.Quests.FinishQuest.RewardOffer", num10) + " ";
					}
					if (info.RewardGiftToken != 0)
					{
						int num11 = info.RewardGiftToken * data.RandDobule;
						m_player.AddGiftToken(num11);
						message += LanguageMgr.GetTranslation("Game.Server.Quests.FinishQuest.RewardGiftToken", num11 + " ");
					}
					m_player.Out.SendMessage(eMessageType.GM_NOTICE, message);
					RemoveQuest(baseQuest);
					SetQuestFinish(baseQuest.Info.ID);
					m_player.PlayerCharacter.QuestSite = m_states;
				}
				OnQuestsChanged(baseQuest);
			}
			catch (Exception exception)
			{
				if (log.IsErrorEnabled)
				{
					log.Error("Quest Finish：" + exception);
				}
				return false;
			}
			finally
			{
				m_player.CommitAllChanges();
			}
			return true;
		}

		private byte[] InitQuest()
		{
			byte[] buffer = new byte[200];
			for (int i = 0; i < 200; i++)
			{
				buffer[i] = 0;
			}
			return buffer;
		}

		private bool IsQuestFinish(int questId)
		{
			if (questId > m_states.Length * 8 || questId < 1)
			{
				return false;
			}
			questId--;
			int index = questId / 8;
			int num2 = questId % 8;
			return (m_states[index] & (1 << num2)) != 0;
		}

		public void LoadFromDatabase(int playerId)
		{
			lock (m_lock)
			{
				m_states = ((m_player.PlayerCharacter.QuestSite.Length == 0) ? InitQuest() : m_player.PlayerCharacter.QuestSite);
				using (PlayerBussiness bussiness = new PlayerBussiness())
				{
					QuestDataInfo[] userQuest = bussiness.GetUserQuest(playerId);
					BeginChanges();
					QuestDataInfo[] array = userQuest;
					foreach (QuestDataInfo info in array)
					{
						QuestInfo singleQuest = QuestMgr.GetSingleQuest(info.QuestID);
						if (singleQuest != null)
						{
							AddQuest(new BaseQuest(singleQuest, info));
						}
						AddQuestData(info);
					}
					CommitChanges();
				}
				_ = m_list;
			}
		}

		public List<QuestDataInfo> GetAllQuestData()
		{
			return m_datas;
		}

		protected void OnQuestsChanged(BaseQuest quest)
		{
			if (!m_changedQuests.Contains(quest))
			{
				m_changedQuests.Add(quest);
			}
			if (m_changeCount <= 0 && m_changedQuests.Count > 0)
			{
				UpdateChangedQuests();
			}
		}

		public bool RemoveQuest(BaseQuest quest)
		{
			int random = 1;
			if (!quest.Info.CanRepeat)
			{
				bool flag = false;
				lock (m_list)
				{
					if (m_list.Remove(quest))
					{
						m_clearList.Add(quest);
						flag = true;
					}
				}
				if (flag)
				{
					quest.RemoveFromPlayer(m_player);
					OnQuestsChanged(quest);
				}
				return flag;
			}
			if ((decimal)ThreadSafeRandom.NextStatic(1000000) <= quest.Info.Rands)
			{
				random = quest.Info.RandDouble;
			}
			quest.Reset(m_player, random);
			QuestDataInfo data = quest.Data;
			data.RepeatFinish--;
			if (data.RepeatFinish <= 0)
			{
				data.IsComplete = true;
			}
			quest.SaveData();
			OnQuestsChanged(quest);
			return true;
		}

		public void SaveToDatabase()
		{
			lock (m_lock)
			{
				using (PlayerBussiness bussiness = new PlayerBussiness())
				{
					foreach (BaseQuest quest in m_list)
					{
						quest.SaveData();
						if (quest.Data.IsDirty)
						{
							bussiness.UpdateDbQuestDataInfo(quest.Data);
						}
					}
					foreach (BaseQuest quest2 in m_clearList)
					{
						quest2.SaveData();
						bussiness.UpdateDbQuestDataInfo(quest2.Data);
					}
					m_clearList.Clear();
				}
			}
		}

		private bool SetQuestFinish(int questId)
		{
			if (questId > m_states.Length * 8 || questId < 1)
			{
				return false;
			}
			questId--;
			int index = questId / 8;
			int num2 = questId % 8;
			m_states[index] = (byte)(m_states[index] | (1 << num2));
			return true;
		}

		public void Update(BaseQuest quest)
		{
			OnQuestsChanged(quest);
		}

		public void UpdateChangedQuests()
		{
			m_player.Out.SendUpdateQuests(m_player, m_states, m_changedQuests.ToArray());
			m_changedQuests.Clear();
		}

		public bool Restart()
		{
			bool flag = false;
			foreach (QuestDataInfo quest in GetAllQuestData())
			{
				BaseQuest baseQuest = FindQuest(quest.QuestID);
				if (baseQuest != null && baseQuest.Info.CanRepeat && baseQuest.Data.IsComplete)
				{
					List<QuestConditionInfo> conditions = QuestMgr.GetQuestCondiction(baseQuest.Info);
					if (conditions.Count > 0)
					{
						baseQuest.Data.Condition1 = conditions[0].Para2;
					}
					if (conditions.Count > 1)
					{
						baseQuest.Data.Condition2 = conditions[1].Para2;
					}
					if (conditions.Count > 2)
					{
						baseQuest.Data.Condition3 = conditions[2].Para2;
					}
					if (conditions.Count > 3)
					{
						baseQuest.Data.Condition4 = conditions[3].Para2;
					}
					baseQuest.Data.RepeatFinish--;
					baseQuest.Data.IsComplete = false;
					baseQuest.Reset(m_player);
					baseQuest.Update();
					SaveToDatabase();
					flag = true;
				}
			}
			return flag;
		}
	}
}
