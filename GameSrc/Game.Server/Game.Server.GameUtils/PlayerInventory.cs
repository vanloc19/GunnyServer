using System;
using System.Collections.Generic;
using System.Text;
using Bussiness;
using Game.Server.GameObjects;
using Game.Server.GMActives;
using Game.Server.Packets;
using SqlDataProvider.Data;

namespace Game.Server.GameUtils
{
	public class PlayerInventory : AbstractInventory
	{
		protected GamePlayer m_player;

		private bool bool_1;

		private List<ItemInfo> list_0;

		public GamePlayer Player => m_player;

		public PlayerInventory(GamePlayer player, bool saveTodb, int capibility, int type, int beginSlot, bool autoStack)
			: base(capibility, type, beginSlot, autoStack)
		{
			list_0 = new List<ItemInfo>();
			m_player = player;
			bool_1 = saveTodb;
		}

		public virtual void LoadFromDatabase()
		{
			if (!bool_1)
			{
				return;
			}
			using (PlayerBussiness playerBussiness = new PlayerBussiness())
			{
				ItemInfo[] userBagByType = playerBussiness.GetUserBagByType(m_player.PlayerCharacter.ID, base.BagType);
				BeginChanges();
				try
				{
					ItemInfo[] array = userBagByType;
					foreach (ItemInfo cloneItem in array)
					{
						if (IsWrongPlace(cloneItem) && cloneItem.Place < 31 && base.BagType == 0)
						{
							int firstEmptySlot = FindFirstEmptySlot(31);
							if (firstEmptySlot != -1)
							{
								MoveItem(cloneItem.Place, firstEmptySlot, cloneItem.Count);
							}
							else
							{
								m_player.AddTemplate(cloneItem);
							}
						}
						else
						{
							AddItemTo(cloneItem, cloneItem.Place);
						}
					}
				}
				finally
				{
					CommitChanges();
				}
			}
		}

		public bool IsWrongPlace(ItemInfo item)
		{
			return item != null && item.Template != null && ((item.Template.CategoryID == 7 && item.Place != 6) || (item.Template.CategoryID == 27 && item.Place != 6) || (item.Template.CategoryID == 17 && item.Place != 15) || (item.Template.CategoryID == 31 && item.Place != 15));
		}

		public virtual void SaveToDatabase()
		{
			if (!bool_1)
			{
				return;
			}
			using (PlayerBussiness playerBussiness = new PlayerBussiness())
			{
				lock (m_lock)
				{
					for (int index = 0; index < m_items.Length; index++)
					{
						ItemInfo itemInfo2 = m_items[index];
						if (itemInfo2 != null && itemInfo2.IsDirty)
						{
							if (itemInfo2.ItemID > 0)
							{
								playerBussiness.UpdateGoods(itemInfo2);
							}
							else
							{
								playerBussiness.AddGoods(itemInfo2);
							}
						}
					}
				}
				lock (list_0)
				{
					foreach (ItemInfo itemInfo in list_0)
					{
						if (itemInfo.ItemID > 0)
						{
							playerBussiness.UpdateGoods(itemInfo);
						}
					}
					list_0.Clear();
				}
			}
		}

		public virtual void SaveNewsItemIntoDatabas()
		{
			if (!bool_1)
			{
				return;
			}
			using (PlayerBussiness playerBussiness = new PlayerBussiness())
			{
				lock (m_lock)
				{
					for (int index = 0; index < m_items.Length; index++)
					{
						ItemInfo itemInfo = m_items[index];
						if (itemInfo != null && itemInfo.IsDirty && itemInfo.ItemID == 0)
						{
							playerBussiness.AddGoods(itemInfo);
						}
					}
				}
			}
		}

		public override bool AddItemTo(ItemInfo item, int place)
		{
			if (!base.AddItemTo(item, place))
			{
				return false;
			}
			item.UserID = m_player.PlayerCharacter.ID;
			item.IsExist = true;
			return true;
		}

		public override bool TakeOutItem(ItemInfo item)
		{
			if (base.TakeOutItem(item))
			{
				if (bool_1)
				{
					lock (list_0)
					{
						list_0.Add(item);
					}
				}
				return true;
			}
			return false;
		}

		public override bool RemoveItem(ItemInfo item)
		{
			if (!base.RemoveItem(item))
			{
				return false;
			}
			item.IsExist = false;
			if (bool_1)
			{
				lock (list_0)
				{
					list_0.Add(item);
				}
			}
			GmActivityMgr.OnUsingItemsEvent(m_player, item.TemplateID, item.Count);
			return true;
		}

		public override void UpdateChangedPlaces()
		{
			m_player.Out.SendUpdateInventorySlot(this, m_changedPlaces.ToArray());
			m_player.UpdateProperties();
			base.UpdateChangedPlaces();
		}

		public bool SendAllItemsToMail(string sender, string title, eMailType type)
		{
			if (this.bool_1)
			{
				base.BeginChanges();
				try
				{
					try
					{
						using (PlayerBussiness playerBussiness = new PlayerBussiness())
						{
							lock (this.m_lock)
							{
								List<ItemInfo> items = this.GetItems();
								int count = items.Count;
								int num = 0;
								while (num < count)
								{
									MailInfo mailInfo = new MailInfo()
									{
										SenderID = 0,
										Sender = sender,
										ReceiverID = this.m_player.PlayerCharacter.ID,
										Receiver = this.m_player.PlayerCharacter.NickName,
										Title = title,
										Type = (int)type,
										Content = ""
									};
									List<ItemInfo> itemInfos = new List<ItemInfo>();
									for (int i = 0; i < 5; i++)
									{
										int num1 = num * 5 + i;
										if (num1 < items.Count)
										{
											itemInfos.Add(items[num1]);
										}
									}
									if (!this.SendItemsToMail(itemInfos, mailInfo, playerBussiness))
									{
										return false;
									}
									else
									{
										num += 5;
									}
								}
							}
						}
					}
					catch (Exception exception)
					{
						Console.WriteLine(string.Concat("Send Items Mail Error:", exception));
					}
				}
				finally
				{
					this.SaveToDatabase();
					base.CommitChanges();
				}
				this.m_player.Out.SendMailResponse(this.m_player.PlayerCharacter.ID, eMailRespose.Receiver);
			}
			return true;
			#region
			/*if (bool_1)
			{
				BeginChanges();
				try
				{
					using (PlayerBussiness pb = new PlayerBussiness())
					{
						lock (m_lock)
						{
							List<ItemInfo> items = GetItems();
							m_player.SendItemsToMail(items, null, "Title", eMailType.ConsortionEmail);
							foreach (var item in items)
							{
								RemoveItem(item);
							}
						}
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine("Send Items Mail Error:" + ex);
				}
				finally
				{
					SaveNewsItemIntoDatabas();
					CommitChanges();
				}
				m_player.Out.SendMailResponse(m_player.PlayerCharacter.ID, eMailRespose.Receiver);
			}
			return true;
			#region
			/*if (bool_1)
			{
				BeginChanges();
				try
				{
					using (PlayerBussiness pb = new PlayerBussiness())
					{
						lock (m_lock)
						{
							List<ItemInfo> items1 = GetItems();
							int count = items1.Count;
							for (int num = 0; num < count; num += 5)
							{
								MailInfo mail = new MailInfo();
								mail.SenderID = 0;
								mail.Sender = sender;
								mail.ReceiverID = m_player.PlayerCharacter.ID;
								mail.Receiver = m_player.PlayerCharacter.NickName;
								mail.Title = title;
								mail.Type = (int)type;
								mail.Content = "";
								List<ItemInfo> items2 = new List<ItemInfo>();
								for (int index1 = 0; index1 < 5; index1++)
								{
									int index2 = num * 5 + index1;
									if (index2 < items1.Count)
									{
										items2.Add(items1[index2]);
									}
								}
								if (!SendItemsToMail(items2, mail, pb))
								{
									return false;
								}
							}
						}
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine("Send Items Mail Error:" + ex);
				}
				finally
				{
					SaveToDatabase();
					CommitChanges();
				}
				m_player.Out.SendMailResponse(m_player.PlayerCharacter.ID, eMailRespose.Receiver);
			}
			return true;
			*/
			#endregion
		}

		public bool SendItemsToMail(List<ItemInfo> items, MailInfo mail, PlayerBussiness pb)
		{
			if (mail == null || items.Count > 5 || !bool_1)
			{
				return false;
			}
			List<ItemInfo> itemInfoList = new List<ItemInfo>();
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(LanguageMgr.GetTranslation("Game.Server.GameUtils.CommonBag.AnnexRemark"));
			if (items.Count > 0 && TakeOutItem(items[0]))
			{
				ItemInfo itemInfo6 = items[0];
				mail.Annex1 = itemInfo6.ItemID.ToString();
				mail.Annex1Name = itemInfo6.Template.Name;
				stringBuilder.Append("1、" + mail.Annex1Name + "x" + itemInfo6.Count + ";");
				itemInfoList.Add(itemInfo6);
			}
			if (items.Count > 1 && TakeOutItem(items[1]))
			{
				ItemInfo itemInfo5 = items[1];
				mail.Annex2 = itemInfo5.ItemID.ToString();
				mail.Annex2Name = itemInfo5.Template.Name;
				stringBuilder.Append("2、" + mail.Annex2Name + "x" + itemInfo5.Count + ";");
				itemInfoList.Add(itemInfo5);
			}
			if (items.Count > 2 && TakeOutItem(items[2]))
			{
				ItemInfo itemInfo4 = items[2];
				mail.Annex3 = itemInfo4.ItemID.ToString();
				mail.Annex3Name = itemInfo4.Template.Name;
				stringBuilder.Append("3、" + mail.Annex3Name + "x" + itemInfo4.Count + ";");
				itemInfoList.Add(itemInfo4);
			}
			if (items.Count > 3 && TakeOutItem(items[3]))
			{
				ItemInfo itemInfo3 = items[3];
				mail.Annex4 = itemInfo3.ItemID.ToString();
				mail.Annex4Name = itemInfo3.Template.Name;
				stringBuilder.Append("4、" + mail.Annex4Name + "x" + itemInfo3.Count + ";");
				itemInfoList.Add(itemInfo3);
			}
			if (items.Count > 4 && TakeOutItem(items[4]))
			{
				ItemInfo itemInfo2 = items[4];
				mail.Annex5 = itemInfo2.ItemID.ToString();
				mail.Annex5Name = itemInfo2.Template.Name;
				stringBuilder.Append("5、" + mail.Annex5Name + "x" + itemInfo2.Count + ";");
				itemInfoList.Add(itemInfo2);
			}
			mail.AnnexRemark = stringBuilder.ToString();
			if (pb.SendMail(mail))
			{
				return true;
			}
			foreach (ItemInfo itemInfo in itemInfoList)
			{
				AddItem(itemInfo);
			}
			return false;
		}

		public bool SendItemToMail(ItemInfo item)
		{
			if (!bool_1)
			{
				return false;
			}
			using (PlayerBussiness pb = new PlayerBussiness())
			{
				return SendItemToMail(item, pb, null);
			}
		}

		public bool SendItemToMail(ItemInfo item, PlayerBussiness pb, MailInfo mail)
		{
			if (!bool_1 || item.BagType != base.BagType)
			{
				return false;
			}
			if (mail == null)
			{
				mail = new MailInfo();
				mail.Annex1 = item.ItemID.ToString();
				mail.Content = LanguageMgr.GetTranslation("Game.Server.GameUtils.Title");
				mail.Gold = 0;
				mail.IsExist = true;
				mail.Money = 0;
				mail.Receiver = m_player.PlayerCharacter.NickName;
				mail.ReceiverID = item.UserID;
				mail.Sender = m_player.PlayerCharacter.NickName;
				mail.SenderID = item.UserID;
				mail.Title = LanguageMgr.GetTranslation("Game.Server.GameUtils.Title");
				mail.Type = 9;
			}
			if (!pb.SendMail(mail))
			{
				return false;
			}
			RemoveItem(item);
			item.IsExist = true;
			return true;
		}

		public virtual void SaveNewItemToDatabase()
		{
			if (bool_1)
			{
				using (PlayerBussiness pb = new PlayerBussiness())
				{
					lock (m_lock)
					{
						for (int i = 0; i < m_items.Length; i++)
						{
							ItemInfo item = m_items[i];
							if (item != null && item.IsDirty && item.ItemID == 0)
								pb.AddGoods(item);
						}
					}
				}
			}
		}
	}
}
