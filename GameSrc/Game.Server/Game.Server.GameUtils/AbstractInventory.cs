using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using log4net;
using SqlDataProvider.Data;

namespace Game.Server.GameUtils
{
	public abstract class AbstractInventory
	{
		public static readonly ILog ilog_0;

		protected object m_lock;

		private int int_0;

		private int int_1;

		private int int_2;

		private bool bool_0;

		protected ItemInfo[] m_items;

		protected List<int> m_changedPlaces;

		private int int_3;

		public int BeginSlot => int_2;

		public int Capalility
		{
			get
			{
				return int_1;
			}
			set
			{
				int_1 = ((value >= 0) ? ((value > m_items.Length) ? m_items.Length : value) : 0);
			}
		}

		public int BagType => int_0;

		public bool IsEmpty(int slot)
		{
			if (slot >= 0 && slot < int_1)
			{
				return m_items[slot] == null;
			}
			return true;
		}

		public AbstractInventory(int capability, int type, int beginSlot, bool autoStack)
		{
			m_lock = new object();
			m_changedPlaces = new List<int>();
			int_1 = capability;
			int_0 = type;
			int_2 = beginSlot;
			bool_0 = autoStack;
			m_items = new ItemInfo[capability];
		}

		public virtual bool AddItem(ItemInfo item)
		{
			return AddItem(item, int_2);
		}

		public virtual bool AddItem(ItemInfo item, int minSlot)
		{
			if (item == null)
			{
				return false;
			}
			int firstEmptySlot = FindFirstEmptySlot(minSlot);
			return AddItemTo(item, firstEmptySlot);
		}

		public virtual bool AddItem(ItemInfo item, int minSlot, int maxSlot)
		{
			if (item == null)
			{
				return false;
			}
			int firstEmptySlot = FindFirstEmptySlot(minSlot, maxSlot);
			return AddItemTo(item, firstEmptySlot);
		}

		public virtual bool AddItemTo(ItemInfo item, int place)
		{
			if (item == null || place >= int_1 || place < 0)
			{
				return false;
			}

			if (item.IsBring() && item.curExp == 0)
				item.curExp = item.Template.Property2;

			lock (m_lock)
			{
				if (m_items[place] != null)
				{
					place = -1;
				}
				else
				{
					m_items[place] = item;
					item.Place = place;
					item.BagType = int_0;
				}
			}
			if (place != -1)
			{
				OnPlaceChanged(place);
			}
			return place != -1;
		}

		public virtual bool TakeOutItem(ItemInfo item)
		{
			if (item == null)
			{
				return false;
			}
			int place = -1;
			lock (m_lock)
			{
				for (int index = 0; index < int_1; index++)
				{
					if (m_items[index] == item)
					{
						place = index;
						m_items[index] = null;
						break;
					}
				}
			}
			if (place != -1)
			{
				OnPlaceChanged(place);
				if (item.BagType == BagType)
				{
					item.Place = -1;
					item.BagType = -1;
				}
			}
			return place != -1;
		}

		public bool TakeOutItemAt(int place)
		{
			return TakeOutItem(GetItemAt(place));
		}

		public void RemoveAllItem(List<ItemInfo> items)
		{
			BeginChanges();
			lock (m_lock)
			{
				foreach (ItemInfo itemInfo in items)
				{
					if (itemInfo.Place >= m_items.Length)
					{
						ilog_0.Error("ERROR PLACE OUT SIZE CAPALITITY: " + itemInfo.Place + " - tempid: " + itemInfo.TemplateID);
					}
					else if (m_items[itemInfo.Place] != null)
					{
						RemoveItem(m_items[itemInfo.Place]);
					}
				}
			}
			CommitChanges();
		}

		public void RemoveAllItem(List<int> places)
		{
			BeginChanges();
			lock (m_lock)
			{
				for (int index = 0; index < places.Count; index++)
				{
					int place = places[index];
					if (m_items[place] != null)
					{
						RemoveItem(m_items[place]);
					}
				}
			}
			CommitChanges();
		}

		public virtual bool RemoveItem(ItemInfo item)
		{
			if (item == null)
			{
				return false;
			}
			int place = -1;
			lock (m_lock)
			{
				for (int index = 0; index < int_1; index++)
				{
					if (m_items[index] == item)
					{
						place = index;
						m_items[index] = null;
						break;
					}
				}
			}
			if (place != -1)
			{
				OnPlaceChanged(place);
				if (item.BagType == BagType)
				{
					item.Place = -1;
					item.BagType = -1;
				}
			}
			return place != -1;
		}

		public bool RemoveItemAt(int place)
		{
			return RemoveItem(GetItemAt(place));
		}

		public virtual bool AddCountToStack(ItemInfo item, int count)
		{
			if (item == null || count <= 0 || item.BagType != int_0 || item.Count + count > item.Template.MaxCount)
			{
				return false;
			}
			item.Count += count;
			OnPlaceChanged(item.Place);
			return true;
		}

		public virtual bool RemoveCountFromStack(ItemInfo item, int count)
		{
			if (item == null || count <= 0 || item.BagType != int_0 || item.Count < count)
			{
				return false;
			}
			if (item.Count == count)
			{
				return RemoveItem(item);
			}
			item.Count -= count;
			OnPlaceChanged(item.Place);
			return true;
		}

		public virtual bool AddTemplateAt(ItemInfo cloneItem, int count, int place)
		{
			return AddTemplate(cloneItem, count, place, int_1 - 1);
		}

		public virtual bool AddTemplate(ItemInfo cloneItem, int count)
		{
			return AddTemplate(cloneItem, count, int_2, int_1 - 1);
		}

		public virtual bool AddTemplate(ItemInfo cloneItem)
		{
			return AddTemplate(cloneItem, cloneItem.Count, int_2, int_1 - 1);
		}

		public virtual bool AddTemplate(ItemInfo cloneItem, int count, int minSlot, int maxSlot)
		{
			if (cloneItem == null)
			{
				return false;
			}
			ItemTemplateInfo template = cloneItem.Template;
			if (template == null || count <= 0 || minSlot < int_2 || minSlot > int_1 - 1 || maxSlot < int_2 || maxSlot > int_1 - 1 || minSlot > maxSlot)
			{
				return false;
			}
			lock (m_lock)
			{
				List<int> intList = new List<int>();
				int num1 = count;
				for (int index = minSlot; index <= maxSlot; index++)
				{
					ItemInfo to = m_items[index];
					if (to == null)
					{
						num1 -= template.MaxCount;
						intList.Add(index);
					}
					else if (bool_0 && cloneItem.CanStackedTo(to))
					{
						num1 -= template.MaxCount - to.Count;
						intList.Add(index);
					}
					if (num1 <= 0)
					{
						break;
					}
				}
				if (num1 > 0)
				{
					return false;
				}
				BeginChanges();
				try
				{
					int num2 = count;
					foreach (int place in intList)
					{
						ItemInfo itemInfo1 = m_items[place];
						if (itemInfo1 == null)
						{
							ItemInfo itemInfo2 = cloneItem.Clone();
							itemInfo2.Count = ((num2 < template.MaxCount) ? num2 : template.MaxCount);
							num2 -= itemInfo2.Count;
							AddItemTo(itemInfo2, place);
						}
						else if (itemInfo1.TemplateID == template.TemplateID)
						{
							int num3 = ((itemInfo1.Count + num2 < template.MaxCount) ? num2 : (template.MaxCount - itemInfo1.Count));
							itemInfo1.Count += num3;
							num2 -= num3;
							OnPlaceChanged(place);
						}
						else
						{
							ilog_0.Error("Add template erro: select slot's TemplateId not equest templateId");
						}
					}
					if (num2 != 0)
					{
						ilog_0.Error("Add template error: last count not equal Zero.");
					}
				}
				finally
				{
					CommitChanges();
				}
				return true;
			}
		}

		public virtual bool RemoveTemplate(int templateId, int count)
		{
			return RemoveTemplate(templateId, count, 0, int_1 - 1);
		}

		public virtual bool RemoveTemplate(int templateId, int count, int minSlot, int maxSlot)
		{
			if (count <= 0 || minSlot < 0 || minSlot > int_1 - 1 || maxSlot <= 0 || maxSlot > int_1 - 1 || minSlot > maxSlot)
			{
				return false;
			}
			lock (m_lock)
			{
				List<int> intList = new List<int>();
				int num1 = count;
				for (int index = minSlot; index <= maxSlot; index++)
				{
					ItemInfo itemInfo2 = m_items[index];
					if (itemInfo2 != null && itemInfo2.TemplateID == templateId)
					{
						intList.Add(index);
						num1 -= itemInfo2.Count;
						if (num1 <= 0)
						{
							break;
						}
					}
				}
				if (num1 > 0)
				{
					return false;
				}
				BeginChanges();
				int num2 = count;
				try
				{
					foreach (int place in intList)
					{
						ItemInfo itemInfo = m_items[place];
						if (itemInfo != null && itemInfo.TemplateID == templateId)
						{
							if (itemInfo.Count <= num2)
							{
								RemoveItem(itemInfo);
								num2 -= itemInfo.Count;
								continue;
							}
							int num3 = ((itemInfo.Count - num2 < itemInfo.Count) ? num2 : 0);
							itemInfo.Count -= num3;
							num2 -= num3;
							OnPlaceChanged(place);
						}
					}
					if (num2 != 0)
					{
						ilog_0.Error("Remove templat error:last itemcoutj not equal Zero.");
					}
				}
				finally
				{
					CommitChanges();
				}
				return true;
			}
		}

		public virtual bool MoveItem(int fromSlot, int toSlot, int count)
		{
			if (fromSlot < 0 || toSlot < 0 || fromSlot >= int_1 || toSlot >= int_1 || fromSlot == toSlot)
			{
				return false;
			}
			if(fromSlot == toSlot)
            {
				return false;
            }
			bool flag = false;
			lock (m_lock)
			{
				flag = 
				CombineItems(fromSlot, toSlot) || StackItems(fromSlot, toSlot, count) || ExchangeItems(fromSlot, toSlot);
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

		public bool IsSolt(int slot)
		{
			if (slot >= 0)
			{
				return slot < int_1;
			}
			return false;
		}

		public void ClearBag()
		{
			BeginChanges();
			lock (m_lock)
			{
				for (int int2 = int_2; int2 < int_1; int2++)
				{
					if (m_items[int2] != null)
					{
						RemoveItem(m_items[int2]);
					}
				}
			}
			CommitChanges();
		}

		public void ClearBagWithoutPlace(int place)
		{
			BeginChanges();
			lock (m_lock)
			{
				for (int int2 = int_2; int2 < int_1; int2++)
				{
					if (m_items[int2] != null && m_items[int2].Place != place)
					{
						RemoveItem(m_items[int2]);
					}
				}
			}
			CommitChanges();
		}

		public bool StackItemToAnother(ItemInfo item)
		{
			lock (m_lock)
			{
				for (int index = int_1 - 1; index >= 0; index--)
				{
					if (item != null && m_items[index] != null && m_items[index] != item && item.CanStackedTo(m_items[index]) && m_items[index].Count + item.Count <= item.Template.MaxCount)
					{
						m_items[index].Count += item.Count;
						item.IsExist = false;
						item.RemoveType = 26;
						UpdateItem(m_items[index]);
						return true;
					}
				}
			}
			return false;
		}

		protected virtual bool CombineItems(int fromSlot, int toSlot)
		{
			return false;
		}

		/*protected virtual bool StackItems(int fromSlot, int toSlot, int itemCount)
		{
			ItemInfo to = m_items[fromSlot];
			ItemInfo itemInfo1 = m_items[toSlot];
			if (itemCount == 0 || itemCount > to.Count)
			{
				itemCount = ((to.Count <= 0) ? 1 : to.Count);
			}
			if (itemInfo1 != null && itemInfo1.TemplateID == to.TemplateID && itemInfo1.CanStackedTo(to))
			{
				if (to.Count + itemInfo1.Count > to.Template.MaxCount)
				{
					to.Count -= itemInfo1.Template.MaxCount - itemInfo1.Count;
					itemInfo1.Count = itemInfo1.Template.MaxCount;
				}
				else
				{
					if (itemCount >= to.Count)
					{
						RemoveItem(to);
					}
					else
					{
						to.Count -= itemCount;
					}
					itemInfo1.Count += itemCount;
				}
				return true;
			}
			if (itemInfo1 != null || to.Count <= itemCount)
			{
				return false;
			}
			ItemInfo itemInfo2 = to.Clone();
			itemInfo2.Count = itemCount;
			if (!AddItemTo(itemInfo2, toSlot))
			{
				return false;
			}
			to.Count -= itemCount;
			return true;
		}*/

		protected virtual bool StackItems(int fromSlot, int toSlot, int itemCount)
		{
			bool result;
			if (fromSlot == toSlot)
			{
				result = false;
			}
			else
			{
				ItemInfo fromItem = m_items[fromSlot];
				ItemInfo toItem = m_items[toSlot];
				if (fromItem == null)
				{
					result = false;
				}
				else if (itemCount < 0)
				{
					result = false;
				}
				else
				{
					if (itemCount == 0)
					{
						if (fromItem.Count > 0)
						{
							itemCount = fromItem.Count;
						}
						else
						{
							itemCount = 1;
						}
					}
					if (fromItem.Count < itemCount)
					{
						result = false;
					}
					else if (toItem != null && toItem.TemplateID == fromItem.TemplateID && toItem.CanStackedTo(fromItem))
					{
						if (itemCount + toItem.Count > fromItem.Template.MaxCount)
						{
							fromItem.Count -= toItem.Template.MaxCount - toItem.Count;
							toItem.Count = toItem.Template.MaxCount;
						}
						else
						{
							toItem.Count += itemCount;
							if (itemCount == fromItem.Count)
							{
								RemoveItem(fromItem);
							}
							else
							{
								fromItem.Count -= itemCount;
								UpdateItem(fromItem);
							}
						}
						result = true;
					}
					else if (toItem == null && fromItem.Count > itemCount)
					{
						ItemInfo newItem = fromItem.Clone();
						newItem.Count = itemCount;
						if (AddItemTo(newItem, toSlot))
						{
							fromItem.Count -= itemCount;
							result = true;
						}
						else
						{
							result = false;
						}
					}
					else
					{
						result = false;
					}
				}
			}
			return result;
		}

		protected virtual bool ExchangeItems(int fromSlot, int toSlot)
		{
			ItemInfo itemInfo1 = m_items[toSlot];
			ItemInfo itemInfo2 = m_items[fromSlot];
			m_items[fromSlot] = itemInfo1;
			m_items[toSlot] = itemInfo2;
			if (itemInfo1 != null)
			{
				itemInfo1.Place = fromSlot;
			}
			if (itemInfo2 != null)
			{
				itemInfo2.Place = toSlot;
			}
			return true;
		}

		public virtual ItemInfo GetItemAt(int slot)
		{
			if (slot >= 0 && slot < int_1)
			{
				return m_items[slot];
			}
			return null;
		}

		public int FindFirstEmptySlot()
		{
			return FindFirstEmptySlot(int_2);
		}

		public virtual int FindFirstEmptySlot(int minSlot)
		{
			if (minSlot >= int_1)
			{
				return -1;
			}
			lock (m_lock)
			{
				for (int index = minSlot; index < int_1; index++)
				{
					if (m_items[index] == null)
					{
						return index;
					}
				}
				return -1;
			}
		}

		public int CountTotalEmptySlot()
		{
			return CountTotalEmptySlot(int_2);
		}

		public int CountTotalEmptySlot(int minSlot)
		{
			if (minSlot >= int_1)
			{
				return -1;
			}
			lock (m_lock)
			{
				int num = 0;
				for (int index = minSlot; index < int_1; index++)
				{
					if (m_items[index] == null)
					{
						num++;
					}
				}
				return num;
			}
		}

		public int FindFirstEmptySlot(int minSlot, int maxSlot)
		{
			if (minSlot >= maxSlot)
			{
				return -1;
			}
			lock (m_lock)
			{
				for (int index = minSlot; index < maxSlot; index++)
				{
					if (m_items[index] == null)
					{
						return index;
					}
				}
				return -1;
			}
		}

		public int FindLastEmptySlot()
		{
			lock (m_lock)
			{
				for (int index = int_1 - 1; index >= 0; index--)
				{
					if (m_items[index] == null)
					{
						return index;
					}
				}
				return -1;
			}
		}

		public int FindLastEmptySlot(int maxSlot)
		{
			lock (m_lock)
			{
				for (int index = maxSlot - 1; index >= 0; index--)
				{
					if (m_items[index] == null)
					{
						return index;
					}
				}
				return -1;
			}
		}

		public virtual void Clear()
		{
			BeginChanges();
			lock (m_lock)
			{
				for (int place = 0; place < int_1; place++)
				{
					m_items[place] = null;
					OnPlaceChanged(place);
				}
			}
			CommitChanges();
		}

		public void Clear(int minSlot, int maxSlot)
		{
			BeginChanges();
			lock (m_lock)
			{
				for (int i = minSlot; i <= maxSlot; i++)
				{
					m_items[i] = null;
					OnPlaceChanged(i);
				}
			}
			CommitChanges();
		}

		public virtual ItemInfo GetItemByCategoryID(int minSlot, int categoryID, int property)
		{
			lock (m_lock)
			{
				for (int index = minSlot; index < int_1; index++)
				{
					if (m_items[index] != null && m_items[index].Template.CategoryID == categoryID && (property == -1 || m_items[index].Template.Property1 == property))
					{
						return m_items[index];
					}
				}
				return null;
			}
		}

		public virtual ItemInfo GetItemByTemplateID(int templateId)
		{
			return GetItemByTemplateID(this.int_2, templateId);
		}

		public virtual ItemInfo GetItemByTemplateID(int minSlot, int templateId)
		{
			lock (m_lock)
			{
				for (int index = minSlot; index < int_1; index++)
				{
					if (m_items[index] != null && m_items[index].TemplateID == templateId)
					{
						return m_items[index];
					}
				}
				return null;
			}
		}

		public virtual List<ItemInfo> GetItemsByTemplateID(int minSlot, int templateid)
		{
			lock (m_lock)
			{
				List<ItemInfo> itemInfoList = new List<ItemInfo>();
				for (int index = minSlot; index < int_1; index++)
				{
					if (m_items[index] != null && m_items[index].TemplateID == templateid)
					{
						itemInfoList.Add(m_items[index]);
					}
				}
				return itemInfoList;
			}
		}

		public virtual ItemInfo GetItemByItemID(int minSlot, int itemId)
		{
			lock (m_lock)
			{
				for (int index = minSlot; index < int_1; index++)
				{
					if (m_items[index] != null && m_items[index].ItemID == itemId)
					{
						return m_items[index];
					}
				}
				return null;
			}
		}

		public virtual int GetItemCount(int templateId)
		{
			return GetItemCount(int_2, templateId);
		}

		public int GetItemCount(int minSlot, int templateId)
		{
			int num = 0;
			lock (m_lock)
			{
				for (int index = minSlot; index < int_1; index++)
				{
					if (m_items[index] != null && m_items[index].TemplateID == templateId)
					{
						num += m_items[index].Count;
					}
				}
				return num;
			}
		}

		public virtual List<ItemInfo> GetItems()
		{
			return GetItems(0, int_1);
		}

		public virtual List<ItemInfo> GetItems(int minSlot, int maxSlot)
		{
			List<ItemInfo> itemInfoList = new List<ItemInfo>();
			lock (m_lock)
			{
				for (int index = minSlot; index < maxSlot; index++)
				{
					if (m_items[index] != null)
					{
						itemInfoList.Add(m_items[index]);
					}
				}
				return itemInfoList;
			}
		}

		public int GetEmptyCount()
		{
			return GetEmptyCount(int_2);
		}

		public virtual int GetEmptyCount(int minSlot)
		{
			if (minSlot < 0 || minSlot > int_1 - 1)
			{
				return 0;
			}
			int num = 0;
			lock (m_lock)
			{
				for (int index = minSlot; index < int_1; index++)
				{
					if (m_items[index] == null)
					{
						num++;
					}
				}
				return num;
			}
		}

		public virtual void UseItem(ItemInfo item)
		{
			bool flag = false;
			if (!item.IsBinds && (item.Template.BindType == 2 || item.Template.BindType == 3))
			{
				item.IsBinds = true;
				flag = true;
			}
			if (!item.IsUsed)
			{
				item.IsUsed = true;
				item.BeginDate = DateTime.Now;
				flag = true;
			}
			if (flag)
			{
				OnPlaceChanged(item.Place);
			}
		}

		public virtual void UpdateItem(ItemInfo item)
		{

			if (item.BagType == int_0)
			{
				if (item.Count <= 0)
					RemoveItem(item);
				else
					OnPlaceChanged(item.Place);
			}
		}

		public virtual bool RemoveCountFromStack(ItemInfo item, int count, eItemRemoveType type)
		{
			if (item == null || count <= 0 || item.BagType != int_0 || item.Count < count)
			{
				return false;
			}
			if (item.Count == count)
			{
				return RemoveItem(item);
			}
			item.Count -= count;
			OnPlaceChanged(item.Place);
			return true;
		}

		public virtual bool RemoveItem(ItemInfo item, eItemRemoveType type)
		{
			if (item == null)
			{
				return false;
			}
			int place = -1;
			lock (m_lock)
			{
				for (int index = 0; index < int_1; index++)
				{
					if (m_items[index] == item)
					{
						place = index;
						m_items[index] = null;
						break;
					}
				}
			}
			if (place != -1)
			{
				OnPlaceChanged(place);
				if (item.BagType == BagType && item.Place == place)
				{
					item.Place = -1;
					item.BagType = -1;
				}
			}
			return place != -1;
		}

		protected void OnPlaceChanged(int place)
		{
			if (!m_changedPlaces.Contains(place))
			{
				m_changedPlaces.Add(place);
			}
			if (int_3 <= 0 && m_changedPlaces.Count > 0)
			{
				UpdateChangedPlaces();
			}
		}

		public void BeginChanges()
		{
			Interlocked.Increment(ref int_3);
		}

		public void CommitChanges()
		{
			int num = Interlocked.Decrement(ref int_3);
			if (num < 0)
			{
				if (ilog_0.IsErrorEnabled)
				{
					ilog_0.Error("Inventory changes counter is bellow zero (forgot to use BeginChanges?)!\n\n" + Environment.StackTrace);
				}
				Thread.VolatileWrite(ref int_3, 0);
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

		public Dictionary<int, ItemInfo> GetRawSpaces()
		{
			Dictionary<int, ItemInfo> dics = new Dictionary<int, ItemInfo>();
			lock (m_lock)
			{
				for (int i = 0; i < m_items.Length; i++)
				{
					if (m_items[i] != null)
					{
						dics.Add(i, m_items[i]);
					}
				}
			}
			return dics;
		}

		static AbstractInventory()
		{
			ilog_0 = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		}
	}
}
