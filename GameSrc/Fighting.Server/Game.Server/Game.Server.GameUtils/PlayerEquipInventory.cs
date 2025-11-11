using System;
using System.Collections.Generic;
using System.Linq;
using Bussiness;
using Bussiness.Managers;
using Game.Logic;
using Game.Server.GameObjects;
using Game.Server.GMActives;
using Game.Server.Managers;
using Game.Server.Packets;
using SqlDataProvider.Data;

namespace Game.Server.GameUtils
{
	public class PlayerEquipInventory : PlayerInventory
	{
		private static readonly int[] StyleIndex = new int[15]
		{
			1,
			2,
			3,
			4,
			5,
			6,
			11,
			13,
			14,
			15,
			16,
			17,
			18,
			19,
			20
		};

		public PlayerEquipInventory(GamePlayer player)
			: base(player, saveTodb: true, 127, 0, 31, autoStack: true)
		{
		}

		public override void LoadFromDatabase()
		{
			List<ItemInfo> items = new List<ItemInfo>();
			BeginChanges();
			try
			{
				base.LoadFromDatabase();
				using (new PlayerBussiness())
				{
					for (int index = 0; index < 31; index++)
					{
						ItemInfo itemInfo = m_items[index];
						if (m_items[index] != null && !m_items[index].IsValidItem())
						{
							int firstEmptySlot = FindFirstEmptySlot(31);
							if (firstEmptySlot >= 0)
							{
								MoveItem(itemInfo.Place, firstEmptySlot, itemInfo.Count);
							}
							else
							{
								items.Add(itemInfo);
							}
						}
						if (m_items[index] != null && (!m_items[index].Template.CanCompose || !m_items[index].Template.CanStrengthen) && !m_items[index].Template.CanCompose && (m_items[index].AttackCompose > 0 || m_items[index].AgilityCompose > 0 || m_items[index].LuckCompose > 0 || m_items[index].DefendCompose > 0))
						{
							m_items[index].AttackCompose = 0;
							m_items[index].DefendCompose = 0;
							m_items[index].AgilityCompose = 0;
							m_items[index].LuckCompose = 0;
							UpdateItem(m_items[index]);
						}
					}
				}
			}
			finally
			{
				CommitChanges();
			}
			if (items.Count > 0)
			{
				m_player.Out.SendMailResponse(m_player.PlayerCharacter.ID, eMailRespose.Receiver);
			}
		}

		public void ClearStrengthenExp()
		{
			for (int index = 0; index < base.Capalility; index++)
			{
				if (m_items[index] != null && m_items[index].Template.CanAdvanced() && m_items[index].StrengthenExp > 0)
				{
					m_items[index].StrengthenExp = 0;
					UpdateItem(m_items[index]);
				}
			}
		}

		public override bool MoveItem(int fromSlot, int toSlot, int count)
		{
			if (fromSlot < 0 || toSlot < 0 || fromSlot >= m_items.Length || toSlot >= m_items.Length || m_items[fromSlot] == null)
			{
				return false;
			}
			if (IsEquipSlot(fromSlot) && !IsEquipSlot(toSlot) && m_items[toSlot] != null && m_items[toSlot].Template.CategoryID != m_items[fromSlot].Template.CategoryID)
			{
				if (!CanEquipSlotContains(fromSlot, m_items[toSlot].Template))
				{
					toSlot = FindFirstEmptySlot(31);
				}
			}
			else
			{
				if (IsEquipSlot(toSlot))
				{
					if (!CanEquipSlotContains(toSlot, m_items[fromSlot].Template))
					{
						UpdateItem(m_items[fromSlot]);
						return false;
					}
					if (!m_player.CanEquip(m_items[fromSlot].Template) || !m_items[fromSlot].IsValidItem())
					{
						UpdateItem(m_items[fromSlot]);
						return false;
					}
					if (m_items[fromSlot] != null)
					{
						m_player.OnNewGearEvent(m_items[fromSlot]);
					}
				}
				if (IsEquipSlot(fromSlot))
				{
					if (m_items[toSlot] != null && !CanEquipSlotContains(fromSlot, m_items[toSlot].Template))
					{
						UpdateItem(m_items[toSlot]);
						return false;
					}
					if (m_items[toSlot] != null)
					{
						m_player.OnNewGearEvent(m_items[toSlot]);
					}
				}
			}
			return base.MoveItem(fromSlot, toSlot, count);
		}

		public override void UpdateChangedPlaces()
		{
			int[] array = m_changedPlaces.ToArray();
			bool flag = false;
			int[] array2 = array;
			foreach (int slot in array2)
			{
				if (!IsEquipSlot(slot))
				{
					continue;
				}
				ItemInfo itemAt = GetItemAt(slot);
				if (itemAt != null)
				{
					m_player.OnUsingItem(GetItemAt(slot).TemplateID, 1);
					itemAt.IsBinds = true;
					if (!itemAt.IsUsed)
					{
						itemAt.IsUsed = true;
						itemAt.BeginDate = DateTime.Now;
					}
				}
				flag = true;
				break;
			}
			base.UpdateChangedPlaces();
			if (flag)
			{
				UpdatePlayerProperties();
			}
		}

		public void UpdatePlayerProperties()
		{
			m_player.BeginChanges();
			try
			{
				int attack = 0;
				int defence = 0;
				int agility = 0;
				int lucky = 0;
				int blood = 0;
				int strengthenlevel = 0;
				string templateid = "";
				string color = "";
				string skin = "";
				int num3 = 0;
				int num4 = 0;
				int num5 = 0;
				int num6 = 0;
				int num7 = 0;
				int num8 = 0;
				int num9 = 0;
				int num10 = 0;
				int num11 = 0;
				int num12 = 0;
				int num13 = 0;
				int num14 = 0;
				int num15 = 0;
				int num16 = 0;
				int num17 = 0;
				int num18 = 0;
				int num19 = 0;
				int num20 = 0;
				int num21 = 0;
				int mh_atk = 0;
				int mh_def = 0;
				int mh_agi = 0;
				int mh_luk = 0;
				int mh_blo = 0;
				int mh_gua = 0;
				int gemAtk = 0; int gemDef = 0; int gemLuck = 0; int gemAgi = 0; int gemHp = 0;
				int setDEF = 0; int setHP = 0; int setLUK = 0; int setAGI = 0; int setATK = 0;
				int avatarattack = 0, avatardefence = 0, avataragility = 0, avatarlucky = 0, avatarhp = 0;
                int attakExplorer = 0;
                int defenceExplorer = 0;
                int agilityExplorer = 0;
                int luckExplorer = 0;
                int armorExplorer = 0;
                int magicAttackExplorer = 0;
                int magicResistenceExplorer = 0;
                int hpExplorer = 0;
                int staminaExplorer = 0;
                int damageExplorer = 0;
                int boost = 0;
				int fullstrenghLevel = 0;
				m_player.UpdatePet(m_player.PetBag.GetPetIsEquip());
				lock (m_lock)
				{
					templateid = ((m_items[0] == null) ? "" : m_items[0].TemplateID.ToString());
					color = ((m_items[0] == null) ? "" : m_items[0].Color);
					skin = ((m_items[5] == null) ? "" : m_items[5].Skin);
					for (int i = 0; i < 31; i++)
					{
						ItemInfo itemInfo = m_items[i];
						if (itemInfo != null)
						{
							attack += itemInfo.Attack;
							defence += itemInfo.Defence;
							agility += itemInfo.Agility;
							lucky += itemInfo.Luck;
							strengthenlevel = strengthenlevel > itemInfo.StrengthenLevel ? strengthenlevel : itemInfo.StrengthenLevel;
							AddBaseLatentProperty(itemInfo, ref attack, ref defence, ref agility, ref lucky);
							AddBaseGemstoneProperty(itemInfo, ref gemAtk, ref gemDef, ref gemAgi, ref gemLuck, ref gemHp);
							var info = SubActiveMgr.GetSubActiveInfo(itemInfo);
							if (info != null && SubActiveMgr.Checked())
							{
								attack += info.GetValue("1");
								defence += info.GetValue("2");
								agility += info.GetValue("3");
								lucky += info.GetValue("4");
								blood += info.GetValue("5");
							}

							if (Equip.isWeddingRing(itemInfo.Template))
							{
								var love = LoveLevelMgr.GetLevel(m_player.PlayerCharacter.RingExp);
								if (love != null)
								{
									attack += itemInfo.Template.Attack * love.Attack / 100;
									defence += itemInfo.Template.Defence * love.Defence / 100;
									agility += itemInfo.Template.Agility * love.Agility / 100;
									lucky += itemInfo.Template.Luck * love.Luck / 100;
								}
							}

							var ghostEquip = m_player.GetGhostEquip(itemInfo.BagType, itemInfo.Place);
							if (ghostEquip != null)
							{
								var infoSpirit = SpiritInfoMgr.GetSingleSpirit(ghostEquip.BagType,
									ghostEquip.Place, ghostEquip.Level);
								if (infoSpirit != null)
								{
									attack += itemInfo.Template.Attack * infoSpirit.AttackAdd / 1000;
									agility += itemInfo.Template.Agility * infoSpirit.AgilityAdd / 1000;
									defence += itemInfo.Template.Defence * infoSpirit.DefendAdd / 1000;
									lucky += itemInfo.Template.Luck * infoSpirit.LuckAdd / 1000;
								}
							}

							if (itemInfo.isGold)
							{
								GoldEquipTemplateInfo goldEquipTemplateInfo = GoldEquipMgr.FindGoldEquipByTemplate(itemInfo.Template.TemplateID, itemInfo.Template.CategoryID);
								if (goldEquipTemplateInfo != null)
								{
									attack += ((goldEquipTemplateInfo.Attack > 0) ? goldEquipTemplateInfo.Attack : 0);
									defence += ((goldEquipTemplateInfo.Defence > 0) ? goldEquipTemplateInfo.Defence : 0);
									agility += ((goldEquipTemplateInfo.Agility > 0) ? goldEquipTemplateInfo.Agility : 0);
									lucky += ((goldEquipTemplateInfo.Luck > 0) ? goldEquipTemplateInfo.Luck : 0);
									blood += ((goldEquipTemplateInfo.Boold > 0) ? goldEquipTemplateInfo.Boold : 0);
								}
							}
							AddProperty(itemInfo, ref attack, ref defence, ref agility, ref lucky);
						}
					}
					var item_Weapon = GetItemAt(6);
					var item_Cloth = GetItemAt(4);
					var item_Head = GetItemAt(0);
					var item_secWeapon = GetItemAt(15);
					var item_Glass = GetItemAt(1);
					var item_GloveA = GetItemAt(7);
					var item_GloveB = GetItemAt(8);
					var item_RingA = GetItemAt(9);
					var item_RingB = GetItemAt(10);
					var item_RingC = GetItemAt(16);
					#region King Of Strength
					if (item_Weapon != null && item_Cloth != null && item_Head != null && item_secWeapon != null)
					{
						var KingOfStreng = new int[] { item_Weapon.StrengthenLevel, item_Cloth.StrengthenLevel, item_Head.StrengthenLevel, item_secWeapon.StrengthenLevel };
						var min = KingOfStreng[0];
						for (int i = 0; i < KingOfStreng.Length; i++)
						{
							if (KingOfStreng[i] < min)
							{
								min = KingOfStreng[i];
							}
						}
						GmActivityMgr.OnStrenghUp(m_player, min);
					}
					#endregion
					#region King Of Compose
					if (item_Weapon != null && item_Cloth != null && item_Head != null && item_Glass != null && item_GloveA != null && item_GloveB != null && item_RingA != null && item_RingB != null && item_RingC != null)
					{
						var Compose = new int[] { item_Weapon.AttackCompose, item_Weapon.DefendCompose, item_Weapon.AgilityCompose, item_Weapon.LuckCompose,
						item_Cloth.AttackCompose, item_Cloth.DefendCompose, item_Cloth.AgilityCompose, item_Cloth.LuckCompose,
						item_Head.AttackCompose, item_Head.DefendCompose, item_Head.AgilityCompose, item_Head.LuckCompose,
						item_Glass.AttackCompose, item_Glass.DefendCompose, item_Glass.AgilityCompose, item_Glass.LuckCompose,
						item_GloveA.AttackCompose, item_GloveA.DefendCompose, item_GloveA.AgilityCompose, item_GloveA.LuckCompose,
						item_GloveB.AttackCompose, item_GloveB.DefendCompose, item_GloveB.AgilityCompose, item_GloveB.LuckCompose,
						item_RingA.AttackCompose, item_RingA.DefendCompose, item_RingA.AgilityCompose, item_RingA.LuckCompose,
						item_RingB.AttackCompose, item_RingB.DefendCompose, item_RingB.AgilityCompose, item_RingB.LuckCompose,
						item_RingC.AttackCompose, item_RingC.DefendCompose, item_RingC.AgilityCompose, item_RingC.LuckCompose};
						int min = Compose[0];
						for (int i = 0; i < Compose.Length; i++)
						{
							if (Compose[i] < min)
								min = Compose[i];
						}
						GmActivityMgr.OnComposeUp(m_player, min);
					}
					#endregion

					EquipBuffer();
					for (int j = 0; j < StyleIndex.Length; j++)
					{
						templateid += ",";
						color += ",";
						if (m_items[StyleIndex[j]] != null)
						{
							templateid += m_items[StyleIndex[j]].TemplateID;
							color += m_items[StyleIndex[j]].Color;
						}
					}
					num3 += ExerciseMgr.GetExercise(m_player.PlayerCharacter.Texp.attTexpExp, "A");
					num4 += ExerciseMgr.GetExercise(m_player.PlayerCharacter.Texp.defTexpExp, "D");
					num5 += ExerciseMgr.GetExercise(m_player.PlayerCharacter.Texp.spdTexpExp, "AG");
					num6 += ExerciseMgr.GetExercise(m_player.PlayerCharacter.Texp.lukTexpExp, "L");
					num7 += ExerciseMgr.GetExercise(m_player.PlayerCharacter.Texp.hpTexpExp, "H");
					Player.CardBuff.Clear();
					int minLev = 30;
					var CardOne = m_player.CardBag.GetItemAt(0);
					var CardTwo = m_player.CardBag.GetItemAt(1);
					var CardThree = m_player.CardBag.GetItemAt(2);
					if (CardOne != null && CardTwo != null && CardThree != null)
					{
						var KingOfCard = new int[] { CardOne.Level, CardTwo.Level, CardThree.Level };
						var min = KingOfCard[0];
						for (int i = 0; i < KingOfCard.Length; i++)
						{
							if (KingOfCard[i] < min)
							{
								min = KingOfCard[i];
							}
						}
						GmActivityMgr.OnCardUp(m_player, min);
					}
					foreach (UsersCardInfo card in m_player.CardBag.GetCards(0, 4))
					{
						ItemTemplateInfo itemTemplateInfo = ItemMgr.FindItemTemplate(card.TemplateID);
						if (itemTemplateInfo != null)
						{
							num13 += itemTemplateInfo.Attack + card.TotalAttack;
							num14 += itemTemplateInfo.Defence + card.TotalDefence;
							num15 += itemTemplateInfo.Agility + card.TotalAgility;
							num16 += itemTemplateInfo.Luck + card.TotalLuck;
							for (int i = 1; i <= card.Level; i++)
							{
								CardUpdateInfo updateInfo = CardMgr.GetCardUpdateInfo(card.TemplateID, i);
								if (updateInfo != null)
								{
									attack += updateInfo.Attack;
									defence += updateInfo.Defend;
									agility += updateInfo.Agility;
									lucky += updateInfo.Lucky;
								}
							}
							if (card.Level < minLev)
							{
								minLev = card.Level;//lấy cấp bé nhất của bộ thẻ
							}

							if (card.CardID != 0)
							{
								Player.CardBuff.Add(card.TemplateID);
							}
						}
					}
					if (Player.CardBuff.Count >= 2)
						Player.CardBuff.Add(minLev + 1000);// thêm cấp bé nhất vào list.



					AddBaseTotemProperty(m_player.PlayerCharacter.totemId, ref attack, ref defence, ref agility, ref lucky, ref blood);
					if (m_player.Rank.UpdateCurrentRank())
					{
						foreach (UserRankInfo rank in m_player.Rank.ListRank)
						{
							if (rank.IsValidRank())
							{
								attack += rank.Attack;
								defence += rank.Defence;
								agility += rank.Agility;
								lucky += rank.Luck;
								if (rank.Info != null)
								{
									attack += rank.Info.Att;
									defence += rank.Info.Def;
									agility += rank.Info.Agi;
									lucky += rank.Info.Luck;
								}
							}
						}
					}
					if (m_player.Pet != null)
					{
						num17 += m_player.Pet.TotalAttack;
						num18 += m_player.Pet.TotalDefence;
						num19 += m_player.Pet.TotalAgility;
						num20 += m_player.Pet.TotalLuck;
						num21 += m_player.Pet.TotalBlood;
						PetFightPropertyInfo petFightPropertyInfo = PetMgr.FindFightProperty(m_player.PlayerCharacter.evolutionGrade);
						if (petFightPropertyInfo != null)
						{
							num17 += petFightPropertyInfo.Attack;
							num18 += petFightPropertyInfo.Defence;
							num19 += petFightPropertyInfo.Agility;
							num20 += petFightPropertyInfo.Lucky;
							num21 += petFightPropertyInfo.Blood;
						}
						var eatPet = m_player.PetBag.EatPets;
						foreach (var item in m_player.Pet.PetEquips)
						{
							if (eatPet != null)
							{
								if (item.eqType == 0)
								{
									var petMoe = PetMoePropertyMgr.FindPetMoeProperty(eatPet.weaponLevel);
									if (petMoe != null)
									{
										mh_atk += petMoe.Attack;
										mh_luk += petMoe.Lucky;
									}
								}
								else if (item.eqType == 1)
								{
									var petMoe = PetMoePropertyMgr.FindPetMoeProperty(eatPet.hatLevel);
									if (petMoe != null)
									{
										mh_def += petMoe.Defence;
										//mh_gua += petMoe.Guard;
									}
								}
								else if (item.eqType == 2)
								{
									var petMoe = PetMoePropertyMgr.FindPetMoeProperty(eatPet.clothesLevel);
									if (petMoe != null)
									{
										mh_agi += petMoe.Agility;
										mh_blo += petMoe.Blood;
									}
								}
							}
						}
					}
					int[] S = luc_Chien_Suit();
					num3 += S[0];
					num4 += S[1];
					num5 += S[2];
					num6 += S[3];
					num7 += S[4];

					if (m_player.PlayerCharacter.ID > 0)
					{
                        JampsUpgradeItemList[] condictions =
                            JampsManualMgr.getCondictionsByLevel(m_player.PlayerCharacter.explorerManualInfo.manualLevel);

                        int pagesCompletas = 0;
                        int pagesCompletasChapter = 0;
                        int paginasAtivas = 0;
                        int condictionCountExplorer = 0;
                        Dictionary<int, int> pagesCompletas1 = new Dictionary<int, int>();
						Dictionary<int, int> pagesCompletasChapter1 = new Dictionary<int, int>();
						Dictionary<int, int> pagesAtivas1 = new Dictionary<int, int>();
						Dictionary<int, int> pagesAtivasChapter1 = new Dictionary<int, int>();

                        foreach (JampsManualItemList i in JampsManualMgr._jampsManualItemList.Values)
                        {
                            if (i.Level <= m_player.PlayerCharacter.explorerManualInfo.manualLevel)
                            {
                                boost += i.Boost / 10;
                                magicAttackExplorer += i.MagicAttack;
                                magicResistenceExplorer += i.MagicResistance;
                            }
                        }

                        foreach (PagesInfo i in m_player.PlayerCharacter.explorerManualInfo.activesPage)
                        {
                            JampsPageItemList page = JampsManualMgr.getPageFromID(i.pageID);
                            if (page.DebrisCount == m_player.PlayerCharacter.explorerManualInfo.debris.Values.Where(o => o.pageID == i.pageID).Count())
                            {
                                attakExplorer += page.Collect_Attack * (1 + boost);
                                defenceExplorer += page.Collect_Defense * (1 + boost);
                                agilityExplorer += page.Collect_Agile * (1 + boost);
                                luckExplorer += page.Collect_Lucky * (1 + boost);
                                armorExplorer += page.Collect_Armor * (1 + boost);
                                magicAttackExplorer += page.Collect_MagicAttack * (1 + boost);
                                magicResistenceExplorer += page.Collect_MagicResistance * (1 + boost);
                                hpExplorer += page.Collect_HP * (1 + boost);
                                staminaExplorer += page.Collect_Stamina * (1 + boost);
                                damageExplorer += page.Collect_Damage * (1 + boost);
                                pagesCompletas += 1;
                                if (pagesCompletas1.ContainsKey(page.ID))
                                {
                                    pagesCompletas1[page.ID] += 1;
                                }
                                else
                                {
                                    pagesCompletas1.Add(page.ID, 1);
                                }

                                if (pagesCompletasChapter1.ContainsKey(page.ChapterID))
                                {
                                    pagesCompletasChapter1[page.ChapterID] += 1;
                                }
                                else
                                {
                                    pagesCompletasChapter1.Add(page.ChapterID, 1);
                                }

                                if (i.activate)
                                {
                                    paginasAtivas += 1;
                                    if (pagesAtivas1.ContainsKey(page.ID))
                                    {
                                        pagesAtivas1[page.ID] += 1;
                                    }
                                    else
                                    {
                                        pagesAtivas1.Add(page.ID, 1);
                                    }

                                    if (pagesAtivasChapter1.ContainsKey(page.ChapterID))
                                    {
                                        pagesAtivasChapter1[page.ChapterID] += 1;
                                    }
                                    else
                                    {
                                        pagesAtivasChapter1.Add(page.ChapterID, 1);
                                    }

                                    attakExplorer += page.Activate_Attack * (1 + boost);
                                    defenceExplorer += page.Activate_Defence * (1 + boost);
                                    agilityExplorer += page.Activate_Agile * (1 + boost);
                                    luckExplorer += page.Activate_Lucky * (1 + boost);
                                    armorExplorer += page.Activate_Armor * (1 + boost);
                                    magicAttackExplorer += page.Activate_MagicAttack * (1 + boost);
                                    magicResistenceExplorer += page.Activate_MagicResistance * (1 + boost);
                                    hpExplorer += page.Activate_HP * (1 + boost);
                                    staminaExplorer += page.Activate_Stamina * (1 + boost);
                                    damageExplorer += page.Activate_Damage * (1 + boost);
                                }
                            }
                        }

                        foreach (JampsUpgradeItemList i in condictions)
                        {
                            switch (i.ConditionType)
                            {
                                case 2:
                                    {
                                        condictionCountExplorer += pagesCompletas;
                                        break;
                                    }

                                case 3:
                                    {
                                        /*foreach (PagesInfo ii in m_player.PlayerCharacter.explorerManualInfo.activesPage.Where(o => JampsManualMgr.getPageFromChapter(i.Parameter1).Contains(JampsManualMgr.getPageFromID(o.pageID))))
										{
											JampsPageItemList page = JampsManualMgr.getPageFromID(ii.pageID);
											if (page.DebrisCount == m_player.PlayerCharacter.explorerManualInfo.debris.Values.Where(o => o.pageID == ii.pageID).Count())
											{
												pagesCompletasChapter += 1;
											}
										}*/
                                        if (pagesCompletasChapter1.ContainsKey(i.Parameter1))
                                        {
                                            condictionCountExplorer += pagesCompletasChapter1[i.Parameter1];
                                        }
                                        else
                                        {
                                            condictionCountExplorer = 0;
                                        }

                                        break;
                                    }

                                case 4:
                                    {
                                        /*foreach (PagesInfo ii in m_player.PlayerCharacter.explorerManualInfo.activesPage.Where(o => o.pageID == i.Parameter2))
										{
											JampsPageItemList page = JampsManualMgr.getPageFromID(ii.pageID);
											if (page.DebrisCount == m_player.PlayerCharacter.explorerManualInfo.debris.Values.Where(o => o.pageID == ii.pageID).Count())
											{
												pagesCompletasChapter += 1;
											}
										}*/
                                        if (pagesCompletas1.ContainsKey(i.Parameter2))
                                        {
                                            condictionCountExplorer += pagesCompletas1[i.Parameter2];
                                        }

                                        if (pagesCompletas1.ContainsKey(i.Parameter3))
                                        {
                                            condictionCountExplorer += pagesCompletas1[i.Parameter3];
                                        }

                                        break;
                                    }

                                case 5:
                                    {
                                        if (pagesAtivasChapter1.ContainsKey(i.Parameter1))
                                        {
                                            condictionCountExplorer += pagesAtivasChapter1[i.Parameter1];
                                        }
                                        else
                                        {
                                            condictionCountExplorer = 0;
                                        }

                                        //condictionCountExplorer += m_player.PlayerCharacter.explorerManualInfo.activesPage.Where(o => JampsManualMgr.getPageFromChapter(i.Parameter1).Contains(JampsManualMgr.getPageFromID(o.pageID)) && o.activate).Count();
                                        break;
                                    }

                                case 6:
                                    {
                                        int p = 0;
                                        if (pagesAtivas1.ContainsKey(i.Parameter2))
                                        {
                                            p = pagesAtivas1[i.Parameter2];
                                        }

                                        if (pagesAtivas1.ContainsKey(i.Parameter1))
                                        {
                                            p += pagesAtivas1[i.Parameter1];
                                        }

                                        condictionCountExplorer += p;
                                        break;
                                    }

                                case 7:
                                    {
                                        condictionCountExplorer += pagesAtivas1.Count;
                                        break;
                                    }
                            }
                        }

                        m_player.PlayerCharacter.explorerManualInfo.Attack = attakExplorer;
                        m_player.PlayerCharacter.explorerManualInfo.Agility = agilityExplorer;
                        m_player.PlayerCharacter.explorerManualInfo.Armor = armorExplorer;
                        m_player.PlayerCharacter.explorerManualInfo.Damage = damageExplorer;
                        m_player.PlayerCharacter.explorerManualInfo.Defense = defenceExplorer;
                        m_player.PlayerCharacter.explorerManualInfo.Lucky = luckExplorer;
                        m_player.PlayerCharacter.explorerManualInfo.MagicAttack = magicAttackExplorer;
                        m_player.PlayerCharacter.explorerManualInfo.MagicResistance = magicResistenceExplorer;
                        m_player.PlayerCharacter.explorerManualInfo.HP = hpExplorer;
                        m_player.PlayerCharacter.explorerManualInfo.Stamina = staminaExplorer;
                        m_player.PlayerCharacter.explorerManualInfo.conditionCount = condictionCountExplorer;
                        m_player.PlayerCharacter.AgiAddPlus += staminaExplorer;
                    }
                }
                m_player.AvatarBag.AddPropAvatarColection(ref avatarattack, ref avatardefence, ref avataragility, ref avatarlucky, ref avatarhp);
				SetsBuildTempMgr.GetSetsBuildProp(m_player.PlayerCharacter.fineSuitExp, ref setDEF, ref setHP, ref setLUK, ref setAGI, ref setATK);
				attack += num3 + num8 + num13 + num17 + mh_atk + gemAtk + setATK + avatarattack + attakExplorer;
				defence += num4 + num9 + num14 + num18 + mh_def + gemDef + setDEF + avatardefence + defenceExplorer;
				agility += num5 + num10 + num15 + num19 + mh_agi + gemAgi + setAGI + avataragility + agilityExplorer;
				lucky += num6 + num11 + num16 + num20 + mh_luk + gemLuck + setLUK + avatarlucky + luckExplorer;
				blood += num7 + num12 + num21 + mh_blo + gemHp + setHP + avatarhp + hpExplorer;
				m_player.UpdateBaseProperties(attack, defence, agility, lucky, blood, mh_gua);
				m_player.UpdateStyle(templateid, color, skin);
				GetUserNimbus();
				m_player.ApertureEquip(strengthenlevel);
				m_player.UpdateWeapon(m_items[6]);
				m_player.UpdateSecondWeapon(m_items[15]);
				m_player.UpdateReduceDame(m_items[17]);
				m_player.UpdateHealstone(m_items[18]);
				m_player.PlayerProp.CreateProp(isSelf: true, "Texp", num3, num4, num5, num6, num7);
				m_player.PlayerProp.CreateProp(true, "Gem", gemAtk, gemDef, gemAgi, gemLuck, gemHp);
				m_player.PlayerProp.CreateProp(true, "Avatar", avatarattack, avatardefence, avataragility, avatarlucky, avatarhp);
				m_player.UpdateFightPower();
				m_player.PlayerProp.ViewCurrent();
				m_player.OnPropertiesChange();
			}
			finally
			{
				m_player.CommitChanges();
			}
		}

		public int[] luc_Chien_Suit()
		{
			int[] A = new int[5];
			using (ProduceBussiness PR = new ProduceBussiness())
			{
				try
				{
					PR.Reset_Suit_Kill(this.Player.PlayerCharacter.ID);
				}
				catch
				{ }
			}
			List<ItemInfo> DS = new List<ItemInfo>();//danh sách item có suit
			for (int i = 0; i < 31; i++)
			{
				ItemInfo itemInfo = this.m_items[i];
				if (itemInfo != null && itemInfo.Template.SuitId > 0)
				{
					DS.Add(itemInfo);
				}
			}
			List<ItemInfo> List2 = DS.FindAll(a => DS.IndexOf(DS[DS.FindIndex(b => b.Template.SuitId == a.Template.SuitId)]) < DS.LastIndexOf(DS[DS.FindLastIndex(c => c.Template.SuitId == a.Template.SuitId)]) && (a.Template.NeedSex == 0 || DS.FindIndex(b => b.Template.NeedSex == a.Template.NeedSex) != DS.FindLastIndex(c => c.Template.NeedSex == a.Template.NeedSex)));
			List<List<ItemInfo>> ListB = new List<List<ItemInfo>>();
			List<List<ItemInfo>> ListC = new List<List<ItemInfo>>();
			List<int> Temp = new List<int>();
			List<int> Temp2 = new List<int>();
			if (List2.Count > 1)
			{
				for (int i = 0; i < List2.Count; i++)
				{
					if (!Temp.Contains(List2[i].Template.SuitId))
					{
						Temp.Add(List2[i].Template.SuitId);
						Temp2.Add(List2[i].TemplateID);
						ListB.Add(List2.FindAll(a => a.Template.SuitId == List2[i].Template.SuitId));
					}
				}
				if (ListB.Count > 0)
				{
					ListC = ListB;
					for (int i = 0; i < ListB.Count; i++)
					{
						if (ListB[i].Count < 1)
							ListC.Remove(ListB[i]);
					}
				}
			}
			if (ListC.Count > 0)
			{
				for (int i = 0; i < ListC.Count; i++)
				{
					if (ListC[i].Count > 1)
					{
						List<Suit_TemplateID> ListSuitTemplateID = GamePlayer.Load_Suit_TemplateID().FindAll(a => a.ID == ListC[i][0].Template.SuitId);
						int dem = 0;
						for (int j = 0; j < ListSuitTemplateID.Count && ListSuitTemplateID.Count > 1; j++)
						{
							for (int k = 0; k < ListC[i].Count; k++)
							{
								if (this.tachchuoi(ListSuitTemplateID[j].ContainEquip).Contains(ListC[i][k].Template.TemplateID))
									dem++;
							}
						}
						if (ListC[i].Count < dem)
							dem = ListC[i].Count;
						if (dem > 1)
						{
							int[] B = this.Congthongso(ListSuitTemplateID[0].ID, dem);
							for (int j = 0; j < A.Length; j++)
								A[j] += B[j];
						}
					}
				}
			}
			return A;
		}

		private void Save_Kill_Suit(List<int> Kill_List)
		{
			try
			{
				if (Kill_List.Count > 0)
				{
					string chuoi = string.Empty;
					foreach (int a in Kill_List)
					{
						chuoi += a.ToString() + ",";
					}
					Suit_Manager A = new Suit_Manager();
					A.Kill_List = chuoi;
					A.UserID = this.Player.PlayerCharacter.ID;
					using (ProduceBussiness P = new ProduceBussiness())
					{
						P.Update_Suit_Kill(A);
					}
				}
			}
			catch
			{ }
		}
		private List<int> Load_Kill_Suit()
		{
			List<int> kill = new List<int>();
			using (PlayerBussiness A = new PlayerBussiness())
			{
				try
				{
					Suit_Manager S = new Suit_Manager();
					S = A.Get_Suit_Manager(this.Player.PlayerCharacter.ID);
					if (S.UserID > 0)
					{
						string chuoi = S.Kill_List;
						if (chuoi.Length > 2)
						{
							while (chuoi.Contains(","))
							{
								int kq = 0;
								int.TryParse(chuoi.Substring(0, chuoi.IndexOf(",")), out kq);
								if (kq == 0)
								{
									return kill;
								}
								kill.Add(kq);
								chuoi = chuoi.Remove(0, chuoi.IndexOf(",") + 1);
							}
							if (!chuoi.Contains(","))
							{
								int kq = 0;
								int.TryParse(chuoi, out kq);
								if (kq > 0)
								{
									kill.Add(kq);
									return kill;
								}
							}
						}
					}
				}
				catch
				{

				}
			}
			return kill;
		}
		private List<int> tachchuoi(string A)
		{
			List<int> B = new List<int>();
			if (!A.Contains(","))
				B.Add(int.Parse(A));
			else
			{
				while (A.Contains(","))
				{
					int vtri = A.IndexOf(",");
					if (vtri > 0)
					{
						string subA = A.Substring(0, vtri);
						B.Add(int.Parse(subA));
						A = A.Remove(0, vtri + 1);
					}
					if (!A.Contains(","))
						B.Add(int.Parse(A));
				}
			}
			return B;
		}
		private int[] Congthongso(int suitID, int count)
		{
			int[] A = new int[5];
			Suit_TemplateInfo B = GamePlayer.DS_Template_Suit_info.Find(a => a.SuitId == suitID);
			int kill = 0;
			switch (count)
			{
				case 2:
					kill = int.Parse(B.Skill2.Replace(",", ""));
					break;
				case 3:
					kill = int.Parse(B.Skill3.Replace(",", ""));
					break;
				case 4:
					kill = int.Parse(B.Skill4.Replace(",", ""));
					break;
				case 5:
					kill = int.Parse(B.Skill5.Replace(",", ""));
					break;
				default:
					return A;
			}
			switch (kill)
			{
				#region
				case 1010000:
					A[4] = 300;
					break;
				case 1010400:
					A[4] = 300;
					A[0] = 10;
					break;
				case 2000000:
					A[4] = 300;
					break;
				case 2000001:
					A[0] = 20;
					A[1] = 20;
					A[2] = 20;
					A[3] = 20;
					A[4] = 300;
					break;
				case 2000002:
					A[0] = 20;
					A[1] = 20;
					A[2] = 20;
					A[3] = 20;
					A[4] = 300;
					break;
				default:
					return A;
					#endregion
			}
			if (kill > 0)
			{
				///////////////////////
				List<int> Kill_List = new List<int>();
				Kill_List = this.Load_Kill_Suit();
				if (kill > 0 && !Kill_List.Contains(kill))
				{
					Kill_List.Add(kill);
					this.Save_Kill_Suit(Kill_List);
				}
				//////////////////
			}
			return A;
		}

		public int FindItemEpuipSlot(ItemTemplateInfo item)
		{
			switch (item.CategoryID)
			{
				case 8:
				case 28:
					lock (m_lock)
					{
						if (m_items[7] == null)
						{
							return 7;
						}
					}
					return 8;

				case 9:
				case 29:
					lock (m_lock)
					{
						if (m_items[9] == null)
						{
							return 9;
						}
					}
					return 10;
				case 13:
					return 11;
				case 14:
					return 12;
				case 15:
					return 13;
				case 16:
					return 14;
				case 27:
					return 6;
				case 17:
				case 31:
					return 15;
				case 40:
					return 17;
				case 70:
					return 18;
				case 64:
					return 20;
				default:
					return item.CategoryID - 1;

			}
		}

		public bool CanEquipSlotContains(int slot, ItemTemplateInfo temp)
		{
			if (temp.CategoryID == 8 || temp.CategoryID == 28)
			{
				return slot == 7 || slot == 8;
			}
			if (temp.CategoryID == 9 || temp.CategoryID == 29)
			{
				if (temp.IsRing())
					return slot == 9 || slot == 10 || slot == 16;

				return slot == 9 || slot == 10;
			}
			if (temp.CategoryID == 13)
			{
				return slot == 11;
			}
			if (temp.CategoryID == 14)
			{
				return slot == 12;
			}
			if (temp.CategoryID == 15)
			{
				return slot == 13;
			}
			if (temp.CategoryID == 16)
			{
				return slot == 14;
			}
			if (temp.CategoryID == 17 || temp.CategoryID == 31)
			{
				return slot == 15;
			}
			if (temp.CategoryID == 27)
			{
				return slot == 6;
			}
			if (temp.CategoryID == 40)
			{
				return slot == 17;
			}
			return temp.CategoryID - 1 == slot;
		}

		public new bool IsEquipSlot(int slot)
		{
			if (slot >= 0)
			{
				return slot < 31;
			}
			return false;
		}

		public void GetUserNimbus()
		{
			var i = 0;
			var j = 0;

			for (var m = 0; m < 31; m++)
			{
				var item = GetItemAt(m);
				if (item != null)
				{
					if (item.StrengthenLevel >= 5 && item.StrengthenLevel <= 8)
					{
						if (item.Template.CategoryID == 1 || item.Template.CategoryID == 5)
						{
							i = i > 01 ? i : 01;
						}

						if (item.Template.CategoryID == 7 || item.Template.CategoryID == 27)
						{
							j = j > 01 ? j : 01;
						}
					}
					else if (item.StrengthenLevel >= 9 && item.StrengthenLevel <= 11)
					{
						if (item.Template.CategoryID == 1 || item.Template.CategoryID == 5)
						{
							i = i > 01 ? i : 02;
						}

						if (item.Template.CategoryID == 7 || item.Template.CategoryID == 27)
						{
							j = j > 01 ? j : 02;
						}
					}
					else if (item.StrengthenLevel >= 12 && item.StrengthenLevel <= 14)
					{
						if (item.Template.CategoryID == 1 || item.Template.CategoryID == 5)
						{
							i = i > 01 ? i : 03;
						}

						if (item.Template.CategoryID == 7 || item.Template.CategoryID == 27)
						{
							j = j > 01 ? j : 03;
						}
					}

					if (item.GoldValidDate())
					{
						if (item.Template.CategoryID == 1 || item.Template.CategoryID == 5)
						{
							i = 05;
						}

						if (item.Template.CategoryID == 7 || item.Template.CategoryID == 27)
						{
							j = 05;
						}
					}
				}
			}

			var listEquipGhost = m_player.GetAllEquipGhost();
			foreach (var egInfo in listEquipGhost)
			{
				if (egInfo.Level >= SpiritInfoMgr.MAX_LEVEL)
				{
					// get info
					var info = SpiritInfoMgr.GetSingleSpirit(egInfo.BagType, egInfo.Place, egInfo.Level);
					if (info != null && GetItemAt(egInfo.Place) != null)
					{
						if (info.CategoryId == 1 || info.CategoryId == 5)
							i = 06;
						else if (info.CategoryId == 7)
							j = 06;
					}
				}
			}

			m_player.PlayerCharacter.Nimbus = i * 100 + j;
			m_player.Out.SendUpdatePublicPlayer(m_player.PlayerCharacter, m_player.BattleData.MatchInfo, m_player.Extra.Info);
			#region OLD
			//int num1 = 0;
			//int num2 = 0;
			//for (int slot = 0; slot < 31; slot++)
			//{
			//	ItemInfo itemAt = GetItemAt(slot);
			//	if (itemAt == null)
			//	{
			//		continue;
			//	}
			//	int strengthenLevel = itemAt.StrengthenLevel;
			//	if (strengthenLevel >= 5 && strengthenLevel <= 8)
			//	{
			//		if (itemAt.Template.CategoryID == 1 || itemAt.Template.CategoryID == 5)
			//		{
			//			num1 = ((num1 <= 1) ? 1 : num1);
			//		}
			//		if (itemAt.Template.CategoryID == 7 || itemAt.Template.CategoryID == 27)
			//		{
			//			num2 = ((num2 <= 1) ? 1 : num2);
			//		}
			//	}
			//	if (strengthenLevel >= 9 && strengthenLevel <= 11)
			//	{
			//		if (itemAt.Template.CategoryID == 1 || itemAt.Template.CategoryID == 5)
			//		{
			//			num1 = ((num1 > 1) ? num1 : 2);
			//		}
			//		if (itemAt.Template.CategoryID == 7 || itemAt.Template.CategoryID == 27)
			//		{
			//			num2 = ((num2 > 1) ? num2 : 2);
			//		}
			//	}
			//	if (strengthenLevel >= 12 && strengthenLevel <= 14)
			//	{
			//		if (itemAt.Template.CategoryID == 1 || itemAt.Template.CategoryID == 5)
			//		{
			//			num1 = ((num1 > 1) ? num1 : 3);
			//		}
			//		if (itemAt.Template.CategoryID == 7 || itemAt.Template.CategoryID == 27)
			//		{
			//			num2 = ((num2 > 1) ? num2 : 3);
			//		}
			//	}
			//	if (itemAt.isGold)
			//	{
			//		if (itemAt.StrengthenLevel >= 15)
			//		{
			//			if (itemAt.Template.CategoryID == 1 || itemAt.Template.CategoryID == 5)
			//			{
			//				num1 = 6;
			//			}
			//			if (itemAt.Template.CategoryID == 7 || itemAt.Template.CategoryID == 27)
			//			{
			//				num2 = 6;
			//			}
			//		}
			//		else
			//                 {
			//			if (itemAt.Template.CategoryID == 1 || itemAt.Template.CategoryID == 5)
			//			{
			//				num1 = 5;
			//			}
			//			if (itemAt.Template.CategoryID == 7 || itemAt.Template.CategoryID == 27)
			//			{
			//				num2 = 5;
			//			}
			//		}
			//	}
			//}
			//m_player.PlayerCharacter.Nimbus = num1 * 100 + num2;
			//m_player.Out.SendUpdatePublicPlayer(m_player.PlayerCharacter, m_player.MatchInfo, m_player.Extra.Info);
			#endregion
		}

		public void EquipBuffer()
		{
			m_player.EquipEffect.Clear();
			for (int slot = 0; slot < 31; slot++)
			{
				ItemInfo itemAt = GetItemAt(slot);
				if (itemAt != null)
				{
					if (itemAt.Hole1 > 0)
					{
						m_player.EquipEffect.Add(itemAt.Hole1);
					}
					if (itemAt.Hole2 > 0)
					{
						m_player.EquipEffect.Add(itemAt.Hole2);
					}
					if (itemAt.Hole3 > 0)
					{
						m_player.EquipEffect.Add(itemAt.Hole3);
					}
					if (itemAt.Hole4 > 0)
					{
						m_player.EquipEffect.Add(itemAt.Hole4);
					}
					if (itemAt.Hole5 > 0)
					{
						m_player.EquipEffect.Add(itemAt.Hole5);
					}
					if (itemAt.Hole6 > 0)
					{
						m_player.EquipEffect.Add(itemAt.Hole6);
					}
				}
			}
		}

		public void AddProperty(ItemInfo item, ref int attack, ref int defence, ref int agility, ref int lucky)
		{
			if (item != null)
			{
				if (item.Hole1 > 0)
				{
					AddBaseProperty(item.Hole1, ref attack, ref defence, ref agility, ref lucky);
				}
				if (item.Hole2 > 0)
				{
					AddBaseProperty(item.Hole2, ref attack, ref defence, ref agility, ref lucky);
				}
				if (item.Hole3 > 0)
				{
					AddBaseProperty(item.Hole3, ref attack, ref defence, ref agility, ref lucky);
				}
				if (item.Hole4 > 0)
				{
					AddBaseProperty(item.Hole4, ref attack, ref defence, ref agility, ref lucky);
				}
				if (item.Hole5 > 0)
				{
					AddBaseProperty(item.Hole5, ref attack, ref defence, ref agility, ref lucky);
				}
				if (item.Hole6 > 0)
				{
					AddBaseProperty(item.Hole6, ref attack, ref defence, ref agility, ref lucky);
				}
			}
		}

		public void AddBaseProperty(int templateid, ref int attack, ref int defence, ref int agility, ref int lucky)
		{
			ItemTemplateInfo itemTemplate = ItemMgr.FindItemTemplate(templateid);
			if (itemTemplate != null && itemTemplate.CategoryID == 11 && itemTemplate.Property1 == 31 && itemTemplate.Property2 == 3)
			{
				attack += itemTemplate.Property3;
				defence += itemTemplate.Property4;
				agility += itemTemplate.Property5;
				lucky += itemTemplate.Property6;
			}
		}

		public void AddBaseTotemProperty(int totemId, ref int attack, ref int defence, ref int agility, ref int lucky, ref int hp)
		{
			attack += TotemMgr.GetTotemProp(totemId, "att");
			defence += TotemMgr.GetTotemProp(totemId, "def");
			agility += TotemMgr.GetTotemProp(totemId, "agi");
			lucky += TotemMgr.GetTotemProp(totemId, "luc");
			hp += TotemMgr.GetTotemProp(totemId, "blo");
		}

		public void AddBaseLatentProperty(ItemInfo item, ref int attack, ref int defence, ref int agility,
			ref int lucky)
		{
			if (item != null)
			{
				if (!item.IsValidLatentEnergy())
				{
					var value = item.latentEnergyCurStr.Split(',');
					attack += Convert.ToInt32(value[0]);
					defence += Convert.ToInt32(value[1]);
					agility += Convert.ToInt32(value[2]);
					lucky += Convert.ToInt32(value[3]);
				}
			}
		}

		public void AddBaseGemstoneProperty(ItemInfo item, ref int attack, ref int defence, ref int agility,
			ref int lucky, ref int hp)
		{
			var _gemStone = m_player.GemStone;
			var addAttack = 0;
			var rdcDamage = 0;
			foreach (var gem in _gemStone)
			{
				try
				{
					var values = gem.FigSpiritIdValue.Split('|');

					var id = gem.FigSpiritId;
					var place = item.Place;
					for (var i = 0; i < values.Length; i++)
					{
						var lv = Convert.ToInt32(values[i].Split(',')[0]);
						switch (item.Place)
						{
							case 2:
								attack += FightSpiritTemplateMgr.GetProp(id, lv, place, ref addAttack, ref rdcDamage);
								break;
							case 11:
								defence += FightSpiritTemplateMgr.GetProp(id, lv, place, ref addAttack, ref rdcDamage);
								break;
							case 5:
								agility += FightSpiritTemplateMgr.GetProp(id, lv, place, ref addAttack, ref rdcDamage);
								break;
							case 3:
								lucky += FightSpiritTemplateMgr.GetProp(id, lv, place, ref addAttack, ref rdcDamage);
								break;
							case 13:
								hp += FightSpiritTemplateMgr.GetProp(id, lv, place, ref addAttack, ref rdcDamage);
								break;
						}
					}
				}
				catch
				{
					ilog_0.ErrorFormat(
						"Add Base Gemstone UserID: {0}, UserName: {1}, FigSpiritId {2}, FigSpiritIdValue: {3}, have error can not get Property",
						m_player.PlayerCharacter.ID, m_player.PlayerCharacter.UserName, gem.FigSpiritId,
						gem.FigSpiritIdValue);
				}
			}

			m_player.PlayerCharacter.GoldenAddAttack = addAttack;
			m_player.PlayerCharacter.GoldenReduceDamage = rdcDamage;
		}
	}
}
