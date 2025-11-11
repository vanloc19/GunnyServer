using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Bussiness.Managers;
using Game.Logic;
using log4net;
using SqlDataProvider.Data;

namespace Game.Server.GameUtils
{
	public abstract class PetAbstractInventory
	{
		private static readonly ILog ilog_0;

		protected object m_lock;

		private int int_0;

		private int int_1;

		private int int_2;

		private int m_aCapalility;

		protected UsersPetInfo[] m_pets;

		protected UsersPetInfo[] m_adoptPets;

		protected ItemInfo[] m_adoptItems;

		protected List<int> m_changedPlaces;

		private int int_3;

		public int BeginSlot => int_2;

		public int Capalility
		{
			get
			{
				return int_0;
			}
			set
			{
				int_0 = ((value >= 0) ? ((value > m_pets.Length) ? m_pets.Length : value) : 0);
			}
		}

		public int ACapalility
        {
			get
            {
				return m_aCapalility;
            }
			set
            {
				m_aCapalility = (value < 0) ? 0 : ((value > m_adoptPets.Length) ? m_adoptPets.Length : value);
            }
        }

		public PetAbstractInventory(int capability, int aCapability, int beginSlot)
		{
			m_lock = new object();
			m_changedPlaces = new List<int>();
			int_0 = capability;
			m_aCapalility = aCapability;
			int_2 = beginSlot;
			m_pets = new UsersPetInfo[capability];
			m_adoptPets = new UsersPetInfo[aCapability];
			m_adoptItems = new ItemInfo[aCapability];
		}

		public virtual UsersPetInfo GetPetIsEquip()
		{
			for (int index = 0; index < int_0; index++)
			{
				if (m_pets[index] != null && m_pets[index].IsEquip)
				{
					return m_pets[index];
				}
			}
			return null;
		}

		public virtual bool AddAdoptPetTo(UsersPetInfo pet, int place)
        {
			if (pet == null || place >= m_aCapalility || place < 0)
				return false;
			lock (m_lock)
            {
				if (m_adoptPets[place] != null)
					place = -1;
				else
                {
					m_adoptPets[place] = pet;
					pet.Place = place;
                }
            }
			return place != -1;
        }

		public virtual bool AddPetTo(UsersPetInfo pet, int place)
		{
			if (pet == null || place >= int_0 || place < 0)
			{
				return false;
			}
			lock (m_lock)
			{
				if (m_pets[place] == null)
				{
					m_pets[place] = pet;
					pet.Place = place;
				}
				else
				{
					place = -1;
				}
			}
			if (place != -1)
			{
				OnPlaceChanged(place);
			}
			return place != -1;
		}

		public virtual bool RemovePet(UsersPetInfo pet)
		{
			bool result;
			if (pet == null)
				result = false;
			else
            {
				int place = -1;
				lock (m_lock)
                {
					for (int i = 0; i < int_0; i++)
                    {
						if (m_pets[i] == pet)
                        {
							place = i;
							m_pets[i] = null;
							break;
                        }
                    }
                }
				if (place != -1)
                {
					RemoveAt(ref m_pets, place);
					pet.Place = -1;
                }
				result = (place != -1);
            }
			return result;
		}

		public void RemoveAt(ref UsersPetInfo[] arr, int index)
        {
			for (int a = index; a < arr.Length - 1; a++)
            {
				arr[a] = arr[a + 1];
				OnPlaceChanged(a);
				if (arr[a] == null)
				{
					continue;
				}
				arr[a].Place = a;
            }
        }

		public virtual bool RemoveAdoptPet(UsersPetInfo pet)
        {
			if (pet == null)
				return false;
			int place = -1;
			lock (m_lock)
            {
				for (int i = 0; i < m_aCapalility; i++)
				{
					if (m_adoptPets[i] == pet)
					{
						place = i;
						m_adoptPets[i] = null;

						break;
					}
				}
			}
			return place != -1;
        }

		public virtual bool MovePet(int fromSlot, int toSlot)
		{
			if (fromSlot < 0 || toSlot < 0 || fromSlot >= int_0 || toSlot >= int_0 || fromSlot == toSlot)
			{
				return false;
			}
			bool flag = false;
			lock (m_lock)
			{
				flag = ExchangePet(fromSlot, toSlot);
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

		protected virtual bool ExchangePet(int fromSlot, int toSlot)
		{
			UsersPetInfo pet1 = m_pets[toSlot];
			UsersPetInfo pet2 = m_pets[fromSlot];
			m_pets[fromSlot] = pet1;
			m_pets[toSlot] = pet2;
			if (pet1 != null)
			{
				pet1.Place = fromSlot;
			}
			if (pet2 != null)
			{
				pet2.Place = toSlot;
			}
			return true;
		}

		public virtual UsersPetInfo GetPetAt(int slot)
		{
			if (slot >= 0 && slot < int_0)
			{
				return m_pets[slot];
			}
			return null;
		}

		public int FindFirstEmptySlot()
		{
			return FindFirstEmptySlot(int_2);
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
					if (m_pets[index] == null)
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
				for (int index = int_0 - 1; index >= 0; index--)
				{
					if (m_pets[index] == null)
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
					m_pets[index] = null;
				}
			}
		}

		public virtual UsersPetInfo GetPetByTemplateID(int minSlot, int templateId)
		{
			lock (m_lock)
			{
				for (int index = minSlot; index < int_0; index++)
				{
					if (m_pets[index] != null && m_pets[index].TemplateID == templateId)
					{
						return m_pets[index];
					}
				}
				return null;
			}
		}

		public virtual UsersPetInfo[] GetPets()
		{
			List<UsersPetInfo> usersPetInfoList = new List<UsersPetInfo>();
			for (int index = 0; index < int_0; index++)
			{
				if (m_pets[index] != null && m_pets[index].IsExit)
				{
					usersPetInfoList.Add(m_pets[index]);
				}
			}
			return usersPetInfoList.ToArray();
		}

		public int GetEmptyCount()
		{
			return GetEmptyCount(int_2);
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
					if (m_pets[index] == null)
					{
						num++;
					}
				}
				return num;
			}
		}

		public virtual UsersPetInfo GetAdoptPetAt(int slot)
        {
			if (slot < 0 || slot >= m_aCapalility) return null;
			return m_adoptPets[slot];
        }

		public virtual UsersPetInfo[] GetAdoptPet(int vipLv)
        {
			List<UsersPetInfo> lists = new List<UsersPetInfo>();
			for (int i = 0; i < m_aCapalility; i++)
			{
				if (m_adoptPets[i] != null)
				{
					if (m_adoptPets[i].IsExit)
					{
						lists.Add(m_adoptPets[i]);
					}
				}
			}

			lists.Add(PetMgr.CreateNewPet(vipLv));
			return lists.ToArray();
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

		public virtual bool RenamePet(int place, string name)
		{
			lock (m_lock)
			{
				if (m_pets[place] != null)
				{
					m_pets[place].Name = name;
				}
			}
			OnPlaceChanged(place);
			return true;
		}

		public virtual bool EquipSkillPet(int place, int killId, int skillPlace, ref string msg)
		{
			string skill = killId + "," + skillPlace;
			lock (m_lock)
			{
				UsersPetInfo pet = m_pets[place];
				if (pet == null)
					return false;

				string[] checkList = pet.GetSkillEquip();

				if (killId == 0)
				{
					m_pets[place].SkillEquip = SetSkillEquip(pet.SkillEquip, checkList, skillPlace, skill);
					OnPlaceChanged(place);
					return true;
				}
				else
				{
					foreach (string currSlot in checkList)
					{
						if (currSlot.Split(',')[0] == "-1")
							continue;

						if (currSlot.Split(',')[0] == killId.ToString())
						{
							msg = "Skill đã được trang bị!";
							return false;
						}
					}

					m_pets[place].SkillEquip = SetSkillEquip(pet.SkillEquip, checkList, skillPlace, skill);
					OnPlaceChanged(place);
					return true;
				}
				#region OLD
				/*string skill = killId + "," + skillPlace;

				lock (m_lock)
				{
					UsersPetInfo pet = m_pets[place];
					if (pet == null)
						return false;

					string[] checkList = pet.GetSkillEquip();

					if (killId == 0)
					{
						m_pets[place].SkillEquip = SetSkillEquip(pet.SkillEquip, checkList, skillPlace, skill);
						OnPlaceChanged(place);
						return true;
					}
					else
					{
						foreach (string currSlot in checkList)
						{
							if (currSlot.Split(',')[0] == "-1")
								continue;

							if (currSlot.Split(',')[0] == killId.ToString())
							{
								msg = "PetHandler.Msg18";
								return false;
							}
						}

						m_pets[place].SkillEquip = SetSkillEquip(pet.SkillEquip, checkList, skillPlace, skill);
						OnPlaceChanged(place);
						return true;
					}
				}
				*/
				#endregion
			}
		}

        public string SetSkillEquip(string skillEquip, string[] list, int place, string skill)
		{
			string skills = "";
			try
			{
				list[place] = skill;
				skills = list[0];
				for (int s = 1; s < list.Length; s++)
				{
					skills += "|" + list[s];
				}
			}
			catch (Exception e)
			{
				if (ilog_0.IsErrorEnabled)
					ilog_0.Error("SetSkillEquip: " + e);

				return skillEquip;
			}

			return skills;
        }

        public virtual bool UpdatePet(UsersPetInfo pet, int place)
		{
			if (pet == null)
			{
				return false;
			}
			int place2 = -1;
			lock (m_lock)
			{
				for (int index = 0; index < m_pets.Length; index++)
				{
					if (m_pets[index] != null)
					{
						place2 = m_pets[index].Place;
						if (place2 == place)
						{
							m_pets[index] = pet;
						}
						OnPlaceChanged(place2);
					}
				}
			}
			return place2 > -1;
		}

		public virtual void UpdatePetFiveKillSlot(int vipLv)
        {
			lock (m_lock)
            {
				for (int index = 0; index < m_pets.Length; index++)
                {
					if (m_pets[index] != null)
                    {
						m_pets[index].SkillEquip = PetMgr.ActiveEquipSkill(m_pets[index].Level, vipLv);
                    }
                }
            }
        }

		public virtual bool EquipPet(int place, bool isEquip)
		{
			int place2 = -1;
			lock (m_lock)
			{
				for (int index = 0; index < m_pets.Length; index++)
				{
					if (m_pets[index] == null)
					{
						continue;
					}
					place2 = m_pets[index].Place;
					if (place2 == place)
					{
						if (m_pets[index].Hunger == 0)
						{
							return false;
						}
						m_pets[index].IsEquip = isEquip;
					}
					else
					{
						m_pets[index].IsEquip = false;
					}
					OnPlaceChanged(place2);
				}
			}
			return place2 > -1;
		}

		public double GetRandomDouble(Random random, double min, double max)
		{
			return min + random.NextDouble() * (max - min);
		}

		public virtual void UpdatePet(UsersPetInfo pet)
		{
			if (pet.Place <= Capalility && pet.Place >= 0)
			{
				lock (m_lock)
				{
					m_pets[pet.Place] = pet;
				}
				OnPlaceChanged(pet.Place);
			}
		}

		//public virtual bool UpdateEvolutionPet(UsersPetInfo pet, int level, int maxLevel, int vipLv)
		//{
		//	int TemplateID = PetMgr.UpdateEvolution(pet.TemplateID, level);
		//	if (TemplateID > pet.TemplateID)
		//	{
		//		pet.TemplateID = TemplateID;
		//		PetTemplateInfo petTemplate = PetMgr.FindPetTemplate(TemplateID);
		//		if (petTemplate != null)
		//		{
		//			double[] propArr = null;
		//			double[] growArr = null;
		//			PetMgr.GetEvolutionPropArr(pet, petTemplate, ref propArr, ref growArr);
		//			if (propArr != null && growArr != null)
		//			{
		//				Random random = new Random();
		//				double min = (double)petTemplate.RareLevel * 0.1;
		//				if (min < growArr[0])
		//				{
		//					pet.BloodGrow += (int)(GetRandomDouble(random, min, growArr[0]) * 10.0);
		//				}
		//				if (min < growArr[1])
		//				{
		//					pet.AttackGrow += (int)(GetRandomDouble(random, min, growArr[1]) * 10.0);
		//				}
		//				if (min < growArr[2])
		//				{
		//					pet.DefenceGrow += (int)(GetRandomDouble(random, min, growArr[2]) * 10.0);
		//				}
		//				if (min < growArr[3])
		//				{
		//					pet.AgilityGrow += (int)(GetRandomDouble(random, min, growArr[3]) * 10.0);
		//				}
		//				if (min < growArr[4])
		//				{
		//					pet.LuckGrow += (int)(GetRandomDouble(random, min, growArr[4]) * 10.0);
		//				}
		//			}
		//		}
		//	}
		//	string skill = pet.Skill;
		//	string str = PetMgr.UpdateSkillPet(level, pet.TemplateID, maxLevel);
		//	pet.Skill = ((str == "") ? skill : str);
		//	pet.SkillEquip = PetMgr.ActiveEquipSkill(level);
		//	pet.BuildProp(pet);
		//	return true;
		//}

		public virtual bool UpdateEvolutionPet(UsersPetInfo pet, int level, int maxLevel, int vipLv)
        {
			int tempID = PetMgr.UpdateEvolution(pet.TemplateID, level);
			if (tempID > pet.TemplateID)
			{
				pet.TemplateID = tempID;
				PetTemplateInfo tempInfo = PetMgr.FindPetTemplate(tempID);
				if (tempInfo != null)
				{
					double[] propArr = null;
					double[] growArr = null;
					PetMgr.GetEvolutionPropArr(pet, tempInfo, ref propArr, ref growArr);
					if (propArr != null && growArr != null)
					{
						Random rd = new Random();
						double minRate = tempInfo.RareLevel * 0.1;
						if (minRate < growArr[0])
							pet.BloodGrow += (int)(GetRandomDouble(rd, minRate, growArr[0]) * 10);
						if (minRate < growArr[1])
							pet.AttackGrow += (int)(GetRandomDouble(rd, minRate, growArr[1]) * 10);
						if (minRate < growArr[2])
							pet.DefenceGrow += (int)(GetRandomDouble(rd, minRate, growArr[2]) * 10);
						if (minRate < growArr[3])
							pet.AgilityGrow += (int)(GetRandomDouble(rd, minRate, growArr[3]) * 10);
						if (minRate < growArr[4])
							pet.LuckGrow += (int)(GetRandomDouble(rd, minRate, growArr[4]) * 10);
					}
				}
			}
		string oldSkill = pet.Skill;
		string newSkill = PetMgr.UpdateSkillPet(level, pet.TemplateID, maxLevel);
		pet.Skill = newSkill == "" ? oldSkill : newSkill;
		pet.SkillEquip = PetMgr.ActiveEquipSkill(level, vipLv);
		pet.IsDirty = true;
		pet.BuildProp();
		return true;
		}

		static PetAbstractInventory()
		{
			ilog_0 = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		}
	}
}
