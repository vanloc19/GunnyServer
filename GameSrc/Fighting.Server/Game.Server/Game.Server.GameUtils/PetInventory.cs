using System;
using System.Collections.Generic;
using System.Reflection;
using Bussiness;
using Bussiness.Managers;
using Game.Logic;
using Game.Server.GameObjects;
using log4net;
using Newtonsoft.Json;
using SqlDataProvider.Data;

namespace Game.Server.GameUtils
{
	public class PetInventory : PetAbstractInventory
	{

		private static readonly ILog ilog_1;

		private bool bool_0;

		private List<UsersPetInfo> list_0;

		protected GamePlayer m_player;

		private PlayerInventory playerInventory_0;

		public GamePlayer Player => m_player;

		private EatPetsInfo m_eatPets;

		public EatPetsInfo EatPets
		{
			get { return m_eatPets; }
		}

		public int MaxLevel => Convert.ToInt32(PetMgr.FindConfig("MaxLevel").Value);

		public int MaxLevelByGrade
		{
			get
			{
				if (m_player == null || m_player.Level > MaxLevel)
				{
					return MaxLevel;
				}
				return m_player.Level;
			}
		}

		public PlayerInventory Equips => playerInventory_0;

		public PetInventory(GamePlayer player, bool saveTodb, int capibility, int aCapability, int beginSlot)
			: base(capibility, aCapability, beginSlot)
		{
			m_player = player;
			bool_0 = saveTodb;
			list_0 = new List<UsersPetInfo>();
			playerInventory_0 = new PlayerInventory(player, saveTodb: true, 49, 5012, 0, autoStack: false);
		}

	public virtual void LoadFromDatabase()
	{
		if (!bool_0)
		{
			return;
		}
		using (PlayerBussiness playerBussiness = new PlayerBussiness())
		{
			int id = m_player.PlayerCharacter.ID;
			UsersPetInfo[] userPetSingles = playerBussiness.GetUserPetSingles(id, m_player.PlayerCharacter.VIPLevel);
				EatPetsInfo eatpet = playerBussiness.GetAllEatPetsByID(id);
				UsersPetInfo[] petAdoptLists = playerBussiness.GetUserAdoptPetSingles(id);
				foreach (UsersPetInfo adoptPet in petAdoptLists)
                {
					AddAdoptPetTo(adoptPet, adoptPet.Place);
                }
				Equips.LoadFromDatabase();
				BeginChanges();
				try
				{
					UsersPetInfo[] array = userPetSingles;

					foreach (UsersPetInfo pet1 in array)
					{
						if (string.IsNullOrEmpty(pet1.BaseProp))
                        {
							string realId = pet1.TemplateID.ToString();
							int oldId = -1;
							if (realId.Substring(realId.Length - 1, 1) == "1")
							{
								if (pet1.Level < 30)
								{
									oldId = pet1.TemplateID;
								}
								else if (pet1.Level < 50)
								{
									oldId = pet1.TemplateID - 1;
								}
								else
								{
									oldId = pet1.TemplateID - 2;
								}
							}
							else if (realId.Substring(realId.Length - 1, 1) == "2")
                            {
								oldId = pet1.TemplateID - 1;
                            }
							else
                            {
								oldId = pet1.TemplateID - 2;
                            }
							PetTemplateInfo tempInfo = PetMgr.FindPetTemplate(oldId);
							if (tempInfo != null)
                            {
								UsersPetInfo info = PetMgr.CreatePet(tempInfo, pet1.UserID, pet1.Place, pet1.Level, Player.PlayerCharacter.VIPLevel);
								pet1.BaseProp = JsonConvert.SerializeObject(info);
								UpdateEvolutionPet(info, pet1.Level, MaxLevelByGrade, Player.PlayerCharacter.VIPLevel);
								pet1.AttackGrow = info.AttackGrow;
								pet1.DefenceGrow = info.DefenceGrow;
								pet1.AgilityGrow = info.AgilityGrow;
								pet1.LuckGrow = info.LuckGrow;
								pet1.BloodGrow = info.BloodGrow;
								pet1.DamageGrow = info.DamageGrow;
								pet1.GuardGrow = info.GuardGrow;
                            }
						}
						pet1.GetSkillEquip();
						AddPetTo(pet1, pet1.Place);
                        #region OLD
                        //if (string.IsNullOrEmpty(pet1.BaseProp))
                        //{
                        //	string str = pet1.TemplateID.ToString();
                        //	PetTemplateInfo petTemplate = PetMgr.FindPetTemplate((str.Substring(str.Length - 1, 1) == "1") ? ((pet1.Level < 30) ? pet1.TemplateID : ((pet1.Level >= 50) ? (pet1.TemplateID - 2) : (pet1.TemplateID - 1))) : ((!(str.Substring(str.Length - 1, 1) == "2")) ? (pet1.TemplateID - 2) : (pet1.TemplateID - 1)));
                        //	if (petTemplate != null)
                        //	{
                        //		UsersPetInfo pet2 = PetMgr.CreatePet(petTemplate, pet1.UserID, pet1.Place, pet1.Level);
                        //		pet1.BaseProp = JsonConvert.SerializeObject(pet2);
                        //		UpdateEvolutionPet(pet2, pet1.Level, MaxLevelByGrade);
                        //		pet1.AttackGrow = pet2.AttackGrow;
                        //		pet1.DefenceGrow = pet2.DefenceGrow;
                        //		pet1.AgilityGrow = pet2.AgilityGrow;
                        //		pet1.LuckGrow = pet2.LuckGrow;
                        //		pet1.BloodGrow = pet2.BloodGrow;
                        //		pet1.DamageGrow = pet2.DamageGrow;
                        //		pet1.GuardGrow = pet2.GuardGrow;
                        //	}
                        //}
                        #endregion
                    }
                    if (eatpet == null)
                    {
						lock (m_lock)
                        {
							m_eatPets = new EatPetsInfo { UserID = id };
						}
                    }
					else
                    {
						lock (m_lock)
                        {
							m_eatPets = eatpet;
						}
                    }
				}
				finally
				{
					try
					{
						if (FindFirstEmptySlot(base.BeginSlot) != -1 && userPetSingles.Length != 0)
						{
							for (int index2 = 1; FindFirstEmptySlot(base.BeginSlot) < userPetSingles[userPetSingles.Length - index2].Place; index2++)
							{
								Player.PetBag.MovePet(userPetSingles[userPetSingles.Length - index2].Place, FindFirstEmptySlot(base.BeginSlot));
							}
						}
					}
					catch
					{
					}
					CommitChanges();
					for (int index = 0; index < Equips.Capalility; index++)
					{
						ItemInfo itemAt = Equips.GetItemAt(index);
						if (itemAt != null)
						{
							m_player.AddTemplate(ItemInfo.CloneFromTemplate(itemAt.Template, itemAt));
							Equips.RemoveItemAt(index);
						}
					}
				}
			}
		}

		public virtual void SaveToDatabase(bool saveAdopt)
		{
			if (bool_0)
			{
				using (PlayerBussiness playerBussiness = new PlayerBussiness())
				{
				lock (m_lock)
				{
					for (int index = 0; index < m_pets.Length; index++)
					{
					UsersPetInfo pet = m_pets[index];
					if (pet != null && pet.IsDirty)
					{
						pet.eQPets = SerializePetEquip(pet.PetEquips);
						if (pet.ID > 0)
						{
							playerBussiness.UpdateUserPet(pet);
						}
						else
						{
							playerBussiness.AddUserPet(pet);
						}
					}
					}
				}
					if (saveAdopt)
                    {
						lock (m_lock)
                        {
							for (int i = 0; i< m_adoptPets.Length; i++)
                            {
								UsersPetInfo pet = m_adoptPets[i];
								if (pet != null && pet.IsDirty)
                                {
									if (pet.ID == 0)
                                    {
										playerBussiness.AddUserAdoptPet(pet, false);

									}
                                }
                            }
                        }
                    }
					lock (m_lock)
					{
						foreach (UsersPetInfo usersPetInfo in list_0)
						{
							playerBussiness.UpdateUserPet(usersPetInfo);
						}
						list_0.Clear();
					}
					lock (m_lock)
                    {
						if (m_eatPets != null && m_eatPets.IsDirty)
                        {
							if (m_eatPets.ID == 0)
							{
								playerBussiness.AddEatPets(m_eatPets);
							}
							else
							{
								playerBussiness.UpdateEatPets(m_eatPets);
							}
						}

					}
				}
			}
			Equips.SaveToDatabase();
		}

		public override bool AddPetTo(UsersPetInfo pet, int place)
		{
			if (!base.AddPetTo(pet, place))
			{
				return false;
			}
			pet.UserID = m_player.PlayerCharacter.ID;
			pet.PetEquips = DeserializePetEquip(pet.eQPets);
			return true;
		}

		public virtual void ReduceHunger()
		{
			UsersPetInfo pet = GetPetIsEquip();
			if (pet != null)
			{
				int baseReduce = 40;
				int maxReduece = 100;
				if (pet.Hunger >= maxReduece)
				{
					if (pet.Level >= 60)
					{
						pet.Hunger -= maxReduece;
					}
					else
					{
						pet.Hunger -= baseReduce;
					}

					UpdatePet(pet, pet.Place);
				}
			}
		}

		public bool CanAdd(ItemInfo item, List<PetEquipInfo> infos)
		{
			if (infos.Count == 3 || item.Template == null)
			{
				return false;
			}
			foreach (PetEquipInfo info in infos)
			{
				if (item.eqType() == info.eqType)
				{
					return false;
				}
			}
			return true;
		}

		public bool CheckEqPetLevel(int place, ItemInfo item)
		{
			UsersPetInfo pet = GetPetAt(place);
			if (pet == null)
				return true;

			if (pet.Level < item.Template.Property2)
			{
				return true;
			}

			return false;
		}

		//public bool AddEqPet(int place, ItemInfo item)
		//{
		//	UsersPetInfo petAt = GetPetAt(place);
		//	if (petAt == null || !CanAdd(item, m_pets[place].PetEquips))
		//	{
		//		return false;
		//	}
		//	petAt.PetEquips.Add(new PetEquipInfo(item.Template)
		//	{
		//		eqTemplateID = item.TemplateID,
		//		eqType = item.eqType(),
		//		ValidDate = item.ValidDate,
		//		startTime = item.BeginDate
		//	});
		//	return true;
		//}

		public bool AddEqPet(int place, ItemInfo item)
        {
			UsersPetInfo pet = GetPetAt(place);
			if (pet == null)
				return false;

			if (CanAdd(item, m_pets[place].PetEquips))
            {
				PetEquipInfo info = new PetEquipInfo(item.Template)
				{
					eqTemplateID = item.TemplateID,
					eqType = item.eqType(),
					ValidDate = item.ValidDate,
					startTime = item.BeginDate,
				};
				pet.PetEquips.Add(info);
				return true;
            }
			return false;
        }

		public PetEquipInfo GetEqPet(List<PetEquipInfo> infos, int place)
		{
			foreach (PetEquipInfo info in infos)
			{
				if (info.eqType == place)
				{
					return info;
				}
			}
			return null;
		}

		public bool RemoveEqPet(int petPlace, int eqPlace)
		{
			UsersPetInfo petAt = GetPetAt(petPlace);
			if (petAt == null)
			{
				return false;
			}
			PetEquipInfo eqPet = GetEqPet(petAt.PetEquips, eqPlace);
			if (eqPet == null)
			{
				return false;
			}
			ChangeEqPetToItem(eqPet);
			return petAt.PetEquips.Remove(eqPet);
		}

		public void ChangeEqPetToItem(PetEquipInfo info)
		{
			if (info.Template != null)
			{
				ItemInfo fromTemplate = ItemInfo.CreateFromTemplate(info.Template, 1, 105);
				fromTemplate.IsBinds = true;
				fromTemplate.IsUsed = true;
				fromTemplate.ValidDate = info.ValidDate;
				fromTemplate.BeginDate = info.startTime;
				m_player.AddTemplate(fromTemplate);
			}
		}

		public void RemoveAllEqPet(List<PetEquipInfo> infos)
		{
			foreach (PetEquipInfo info in infos)
			{
				ChangeEqPetToItem(info);
			}
		}

		public List<PetEquipInfo> DeserializePetEquip(string eqString)
		{
			if (string.IsNullOrEmpty(eqString))
			{
				return new List<PetEquipInfo>();
			}
			List<PetEquipInfo> list = JsonConvert.DeserializeObject<List<PetEquipInfo>>(eqString);
			List<PetEquipInfo> petEquipInfoList = new List<PetEquipInfo>();
			foreach (PetEquipInfo petEquipInfo in list)
			{
				if (petEquipInfo.Template == null)
				{
					ItemTemplateInfo itemTemplate = ItemMgr.FindItemTemplate(petEquipInfo.eqTemplateID);
					if (itemTemplate != null)
					{
						petEquipInfoList.Add(new PetEquipInfo(itemTemplate)
						{
							eqTemplateID = petEquipInfo.eqTemplateID,
							eqType = petEquipInfo.eqType,
							ValidDate = petEquipInfo.ValidDate,
							startTime = petEquipInfo.startTime
						});
					}
				}
				else
				{
					petEquipInfoList.Add(petEquipInfo);
				}
			}
			return petEquipInfoList;
		}

		public string SerializePetEquip(List<PetEquipInfo> eqs)
		{
			return JsonConvert.SerializeObject(eqs);
		}

		public void UpdateEatPets()
		{
			m_player.Out.SendEatPetsInfo(EatPets);
		}

		public virtual bool OnChangedPetEquip(int place)
		{
			lock (m_lock)
			{
				if (m_pets[place] != null && m_pets[place].IsEquip)
				{
					m_player.EquipBag.UpdatePlayerProperties();
				}
			}
			OnPlaceChanged(place);
			return true;
		}

		public override bool RemovePet(UsersPetInfo pet)
		{
			if (!base.RemovePet(pet))
			{
				return false;
			}
			if (pet.PetEquips != null && pet.PetEquips.Count > 0)
			{
				RemoveAllEqPet(pet.PetEquips);
			}
			lock (list_0)
			{
				pet.IsExit = false;
				list_0.Add(pet);
			}
			return true;
		}

		public override void UpdateChangedPlaces()
		{
			m_player.Out.SendUpdateUserPet(this, m_changedPlaces.ToArray());
			base.UpdateChangedPlaces();
		}

		public virtual void ClearAdoptPets()
		{
			using (PlayerBussiness pb = new PlayerBussiness())
			{
				lock (m_lock)
				{
					for (int i = 0; i < ACapalility; i++)
					{
						if (m_adoptPets[i] != null && m_adoptPets[i].ID > 0)
							pb.ClearAdoptPet(m_adoptPets[i].ID);

						m_adoptPets[i] = null;
					}
				}
			}
		}

		static PetInventory()
		{
			ilog_1 = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		}
	}
}
