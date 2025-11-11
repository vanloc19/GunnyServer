using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using log4net;
using SqlDataProvider.Data;

namespace Game.Server.GameUtils
{
	public abstract class CardAbstractInventory
	{
		private static readonly ILog ilog_0;

		protected object m_lock;

		private int int_0;

		private int int_1;

		protected UsersCardInfo[] m_cards;

		protected UsersCardInfo temp_card;

		protected List<int> m_changedPlaces;

		private int int_2;

		public int BeginSlot => int_1;

		public int Capalility
		{
			get
			{
				return int_0;
			}
			set
			{
				int_0 = ((value >= 0) ? ((value > m_cards.Length) ? m_cards.Length : value) : 0);
			}
		}

		public bool IsEmpty(int slot)
		{
			if (slot >= 0 && slot < int_0)
			{
				return m_cards[slot] == null;
			}
			return true;
		}

		public CardAbstractInventory(int capability, int beginSlot)
		{
			m_lock = new object();
			m_changedPlaces = new List<int>();
			int_0 = capability;
			int_1 = beginSlot;
			m_cards = new UsersCardInfo[capability];
			temp_card = new UsersCardInfo();
		}

		public virtual void UpdateTempCard(UsersCardInfo card)
		{
			lock (m_lock)
			{
				temp_card = card;
			}
		}

		public virtual void UpdateCard(UsersCardInfo card)
		{
			OnPlaceChanged(card.Place);
		}

		public virtual void UpdateCard()
		{
			int place = temp_card.Place;
			int templateId = temp_card.TemplateID;
			if (place < 5)
			{
				ReplaceCardTo(temp_card, place);
				int placeByTamplateId2 = FindPlaceByTamplateId(5, templateId);
				MoveCard(place, placeByTamplateId2);
				return;
			}
			ReplaceCardTo(temp_card, place);
			int placeByTamplateId = FindPlaceByTamplateId(0, 5, templateId);
			if (GetItemAt(placeByTamplateId) != null && GetItemAt(placeByTamplateId).TemplateID == templateId)
			{
				MoveCard(place, placeByTamplateId);
			}
		}

		public bool AddCard(UsersCardInfo card)
		{
			return AddCard(card, int_1);
		}

		public bool AddCard(UsersCardInfo card, int minSlot)
		{
			if (card == null)
			{
				return false;
			}
			int firstEmptySlot = FindFirstEmptySlot(minSlot);
			return AddCardTo(card, firstEmptySlot);
		}

		public virtual bool AddCardTo(UsersCardInfo card, int place)
		{
			if (card == null || place >= int_0 || place < 0)
			{
				return false;
			}
			lock (m_lock)
			{
				if (m_cards[place] != null)
				{
					place = -1;
				}
				else
				{
					m_cards[place] = card;
					card.Place = place;
				}
			}
			if (place != -1)
			{
				OnPlaceChanged(place);
			}
			return place != -1;
		}

		public virtual bool RemoveCardAt(int place)
		{
			return RemoveCard(GetItemAt(place));
		}

		public virtual bool RemoveCard(UsersCardInfo item)
		{
			if (item == null)
			{
				return false;
			}
			int place = -1;
			lock (m_lock)
			{
				for (int index = 0; index < int_0; index++)
				{
					if (m_cards[index] == item)
					{
						place = index;
						m_cards[index] = null;
						break;
					}
				}
			}
			if (place != -1)
			{
				OnPlaceChanged(place);
				item.Place = -1;
			}
			return place != -1;
		}

		public virtual bool ReplaceCardTo(UsersCardInfo card, int place)
		{
			if (card == null || place >= int_0 || place < 0)
			{
				return false;
			}
			lock (m_lock)
			{
				if (m_cards[place] != null)
				{
					RemoveCard(m_cards[place]);
				}
				m_cards[place] = card;
				card.Place = place;
				OnPlaceChanged(place);
			}
			return true;
		}

		public virtual bool MoveCard(int fromSlot, int toSlot)
		{
			if (fromSlot < 0 || toSlot < 0 || fromSlot >= int_0 || toSlot >= int_0)
			{
				return false;
			}
			bool flag = false;
			lock (m_lock)
			{
				flag = StackCards(fromSlot, toSlot) || ExchangeCards(fromSlot, toSlot);
			}
			if (flag)
			{
				BeginChanges();
				try
				{
					OnPlaceChanged(fromSlot);
					OnPlaceChanged(toSlot);
					return flag;
				}
				finally
				{
					CommitChanges();
				}
			}
			return flag;
		}

		protected virtual bool StackCards(int fromSlot, int toSlot)
		{
			UsersCardInfo card1 = m_cards[fromSlot];
			UsersCardInfo card2 = m_cards[toSlot];
			if (card1 == null || card2 == null || card2.TemplateID != card1.TemplateID)
			{
				return false;
			}
			card2.Count += card1.Count;
			RemoveCard(card1);
			return true;
		}

		public bool IsSolt(int slot)
		{
			if (slot >= 0)
			{
				return slot < int_0;
			}
			return false;
		}

		protected virtual bool ExchangeCards(int fromSlot, int toSlot)
		{
			UsersCardInfo card1 = m_cards[toSlot];
			UsersCardInfo card2 = m_cards[fromSlot];
			m_cards[fromSlot] = card1;
			m_cards[toSlot] = card2;
			if (card1 != null)
			{
				card1.Place = fromSlot;
			}
			if (card2 != null)
			{
				card2.Place = toSlot;
			}
			return true;
		}

		public virtual bool ResetCardSoul()
		{
			lock (m_lock)
			{
				for (int index = 0; index < 5; index++)
				{
					m_cards[index].Level = 0;
					m_cards[index].CardGP = 0;
				}
			}
			return true;
		}

		public virtual bool UpGraceSlot(int soulPoint, int lv, int place)
		{
			lock (m_lock)
			{
				m_cards[place].CardGP += soulPoint;
				m_cards[place].Level = lv;
			}
			return true;
		}

		public virtual UsersCardInfo GetItemAt(int slot)
		{
			if (slot >= 0 && slot < int_0)
			{
				return m_cards[slot];
			}
			return null;
		}

		public virtual List<UsersCardInfo> GetEquipCard()
		{
			List<UsersCardInfo> UsersCardInfoList = new List<UsersCardInfo>();
			for (int index = 0; index < 5; index++)
			{
				if (m_cards[index] != null)
				{
					UsersCardInfoList.Add(m_cards[index]);
				}
			}
			return UsersCardInfoList;
		}

		public int FindFirstEmptySlot()
		{
			return FindFirstEmptySlot(int_1);
		}

		public int FindFirstEmptySlot(int minSlot)
		{
			if (minSlot >= int_0)
			{
				return -1;
			}
			lock (m_lock)
			{
				for (int index = minSlot; index < int_0; index++)
				{
					if (m_cards[index] == null)
					{
						return index;
					}
				}
				return -1;
			}
		}

		public int FindPlaceByTamplateId(int minSlot, int templateId)
		{
			if (minSlot >= int_0)
			{
				return -1;
			}
			lock (m_lock)
			{
				for (int index = minSlot; index < int_0; index++)
				{
					if (m_cards[index] != null && m_cards[index].TemplateID == templateId)
					{
						return m_cards[index].Place;
					}
				}
				return -1;
			}
		}

		public bool FindEquipCard(int templateId)
		{
			lock (m_lock)
			{
				for (int index = 0; index < 5; index++)
				{
					if (m_cards[index].TemplateID == templateId)
					{
						return true;
					}
				}
				return false;
			}
		}

		public int FindPlaceByTamplateId(int minSlot, int maxSlot, int templateId)
		{
			if (minSlot >= int_0)
			{
				return -1;
			}
			lock (m_lock)
			{
				for (int index = minSlot; index < maxSlot; index++)
				{
					if (m_cards[index] != null && m_cards[index].TemplateID == templateId)
					{
						return m_cards[index].Place;
					}
				}
				return -1;
			}
		}

		public int FindLastEmptySlot()
		{
			lock (m_lock)
			{
				for (int index = int_0 - 1; index >= 0; index--)
				{
					if (m_cards[index] == null)
					{
						return index;
					}
				}
				return -1;
			}
		}

		public virtual void Clear()
		{
			lock (m_lock)
			{
				for (int index = 0; index < int_0; index++)
				{
					m_cards[index] = null;
				}
			}
		}

		public virtual UsersCardInfo GetItemByTemplateID(int templateId)
		{
			return GetItemByTemplateID(int_1, templateId);
		}

		public virtual UsersCardInfo GetItemByTemplateID(int minSlot, int templateId)
		{
			lock (m_lock)
			{
				for (int index = minSlot; index < int_0; index++)
				{
					if (m_cards[index] != null && m_cards[index].TemplateID == templateId)
					{
						return m_cards[index];
					}
				}
				return null;
			}
		}

		public virtual UsersCardInfo GetItemByPlace(int minSlot, int place)
		{
			lock (m_lock)
			{
				for (int index = minSlot; index < int_0; index++)
				{
					if (m_cards[index] != null && m_cards[index].Place == place)
					{
						return m_cards[index];
					}
				}
				return null;
			}
		}

		public virtual List<UsersCardInfo> GetCards()
		{
			return GetCards(0, int_0 - 1);
		}

		public virtual List<UsersCardInfo> GetCards(int minSlot, int maxSlot)
		{
			List<UsersCardInfo> UsersCardInfoList = new List<UsersCardInfo>();
			lock (m_lock)
			{
				for (int index = minSlot; index <= maxSlot; index++)
				{
					if (m_cards[index] != null)
					{
						UsersCardInfoList.Add(m_cards[index]);
					}
				}
				return UsersCardInfoList;
			}
		}

		public UsersCardInfo GetCardEquip(int templateid)
		{
			foreach (UsersCardInfo card in GetCards(0, 4))
			{
				if (card.TemplateID == templateid)
				{
					return card;
				}
			}
			return null;
		}

		public int GetEmptyCount()
		{
			return GetEmptyCount(int_1);
		}

		public virtual int GetEmptyCount(int minSlot)
		{
			if (minSlot < 0 || minSlot > int_0 - 1)
			{
				return 0;
			}
			int num = 0;
			lock (m_lock)
			{
				for (int index = minSlot; index < int_0; index++)
				{
					if (m_cards[index] == null)
					{
						num++;
					}
				}
				return num;
			}
		}

		protected void OnPlaceChanged(int place)
		{
			if (!m_changedPlaces.Contains(place))
			{
				m_changedPlaces.Add(place);
			}
			if (int_2 <= 0 && m_changedPlaces.Count > 0)
			{
				UpdateChangedPlaces();
			}
		}

		public void BeginChanges()
		{
			Interlocked.Increment(ref int_2);
		}

		public void CommitChanges()
		{
			int num = Interlocked.Decrement(ref int_2);
			if (num < 0)
			{
				if (ilog_0.IsErrorEnabled)
				{
					ilog_0.Error("Inventory changes counter is bellow zero (forgot to use BeginChanges?)!\n\n" + Environment.StackTrace);
				}
				Thread.VolatileWrite(ref int_2, 0);
			}
			if (num <= 0 && m_changedPlaces.Count > 0)
			{
				UpdateChangedPlaces();
			}
		}

		public virtual void UpdateChangedPlaces()
		{
			m_changedPlaces.Clear();
		}

		public void ClearBag()
		{
			BeginChanges();
			lock (m_lock)
			{
				for (int index = 5; index < int_0; index++)
				{
					if (m_cards[index] != null)
					{
						RemoveCard(m_cards[index]);
					}
				}
			}
			CommitChanges();
		}

		public UsersCardInfo[] GetRawSpaces()
		{
			lock (m_lock)
			{
				return m_cards.Clone() as UsersCardInfo[];
			}
		}

		static CardAbstractInventory()
		{
			ilog_0 = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		}
	}
}
