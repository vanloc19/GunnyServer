using System.Collections.Generic;
using Bussiness;
using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.GameUtils
{
	public class CardInventory : CardAbstractInventory
	{
		protected GamePlayer m_player;

		private bool bool_0;

		private List<UsersCardInfo> list_0;

		public GamePlayer Player => m_player;

		public CardInventory(GamePlayer player, bool saveTodb, int capibility, int beginSlot)
			: base(capibility, beginSlot)
		{
			list_0 = new List<UsersCardInfo>();
			m_player = player;
			bool_0 = saveTodb;
		}

		public virtual void LoadFromDatabase()
		{
			if (!bool_0)
			{
				return;
			}
			using (PlayerBussiness playerBussiness = new PlayerBussiness())
			{
				UsersCardInfo[] singleUserCard = playerBussiness.GetSingleUserCard(m_player.PlayerCharacter.ID);
				BeginChanges();
				try
				{
					UsersCardInfo[] array = singleUserCard;
					foreach (UsersCardInfo card in array)
					{
						AddCardTo(card, card.Place);
					}
				}
				finally
				{
					CommitChanges();
				}
			}
		}

		public virtual void SaveToDatabase()
		{
			if (!bool_0)
			{
				return;
			}
			using (PlayerBussiness playerBussiness = new PlayerBussiness())
			{
				lock (m_lock)
				{
					for (int index = 0; index < m_cards.Length; index++)
					{
						UsersCardInfo card = m_cards[index];
						if (card != null && card.IsDirty)
						{
							if (card.CardID > 0)
							{
								playerBussiness.UpdateCards(card);
							}
							else if (card.CardID == 0 && card.Place != -1)
							{
								playerBussiness.AddCards(card);
							}
						}
					}
				}
				lock (list_0)
				{
					foreach (UsersCardInfo UsersCardInfo in list_0)
					{
						if (UsersCardInfo.CardID > 0)
						{
							playerBussiness.UpdateCards(UsersCardInfo);
						}
					}
					list_0.Clear();
				}
			}
		}

		public virtual bool AddCard(int templateId, int count)
		{
			UsersCardInfo itemByTemplateId = GetItemByTemplateID(templateId);
			if (itemByTemplateId == null)
			{
				return AddCard(new UsersCardInfo(m_player.PlayerCharacter.ID, templateId, count));
			}
			itemByTemplateId.Count += count;
			UpdateCard(itemByTemplateId);
			return true;
		}

		public override bool AddCardTo(UsersCardInfo item, int place)
		{
			if (!base.AddCardTo(item, place))
			{
				return false;
			}
			item.UserID = m_player.PlayerCharacter.ID;
			return true;
		}

		public override bool RemoveCardAt(int place)
		{
			UsersCardInfo itemAt = GetItemAt(place);
			if (itemAt == null)
			{
				return false;
			}
			list_0.Add(itemAt);
			base.RemoveCardAt(place);
			return true;
		}

		public override bool RemoveCard(UsersCardInfo item)
		{
			if (item == null)
			{
				return false;
			}
			list_0.Add(item);
			base.RemoveCard(item);
			return true;
		}

		public override void UpdateChangedPlaces()
		{
			m_player.Out.SendUpdateCardData(this, m_changedPlaces.ToArray());
			base.UpdateChangedPlaces();
		}

		public bool IsCardEquip(int templateid)
		{
			foreach (UsersCardInfo item in GetEquipCard())
			{
				if (item.TemplateID == templateid)
				{
					return true;
				}
			}
			return false;
		}
	}
}
