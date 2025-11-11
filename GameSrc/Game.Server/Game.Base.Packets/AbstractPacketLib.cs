using System;
using System.Collections.Generic;
using System.Reflection;
using Bussiness;
using Bussiness.Managers;
using Game.Server;
using Game.Server.Buffer;
using Game.Server.ConsortiaTask;
using Game.Server.GameObjects;
using Game.Server.GameUtils;
using Game.Server.Managers;
using Game.Server.Packets;
using Game.Server.Quests;
using Game.Server.Rooms;
using Game.Server.SceneMarryRooms;
using log4net;
using SqlDataProvider.Data;
using Game.Server.LittleGame;
using Game.Server.GMActives;
using System.Linq;
using Newtonsoft.Json;

namespace Game.Base.Packets
{
	[PacketLib(1)]
	public class AbstractPacketLib : IPacketLib
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		protected readonly GameClient m_gameClient;

		protected GamePlayer m_gamePlayer;

		public AbstractPacketLib(GameClient client)
		{
			m_gameClient = client;
		}

		public static IPacketLib CreatePacketLibForVersion(int rawVersion, GameClient client)
		{
			Type[] derivedClasses = ScriptMgr.GetDerivedClasses(typeof(IPacketLib));
			foreach (Type type in derivedClasses)
			{
				object[] customAttributes = type.GetCustomAttributes(typeof(PacketLibAttribute), inherit: false);
				for (int j = 0; j < customAttributes.Length; j++)
				{
					if (((PacketLibAttribute)customAttributes[j]).RawVersion != rawVersion)
					{
						continue;
					}
					try
					{
						return (IPacketLib)Activator.CreateInstance(type, client);
					}
					catch (Exception exception)
					{
						if (log.IsErrorEnabled)
						{
							log.Error("error creating packetlib (" + type.FullName + ") for raw version " + rawVersion, exception);
						}
					}
				}
			}
			return null;
		}

		public void SendTCP(GSPacketIn packet)
		{
			m_gameClient.SendTCP(packet);
		}

		public void WonderfulSingleActivityInit(IGMActive gmActivity, GamePlayer player)
		{
			WonderfulActivityInit(new List<IGMActive>() { gmActivity }, player, 2);
		}

		public void WonderfulActivityInit(List<IGMActive> allActions, GamePlayer player, int type)
		{
			GSPacketIn pkg = new GSPacketIn((short)ePackageType.WONDERFUL_ACTIVITY_INIT);

			pkg.WriteInt(type);

			pkg.WriteInt(allActions.Count); // total

			foreach (IGMActive gmActiveInfo in allActions)
			{
				pkg.WriteString(gmActiveInfo.Info.activityId);

				//Console.WriteLine("gmActiveInfo.activityId: " + gmActiveInfo.Info.activityId);

				gmActiveInfo.SetStatusPacket(player, pkg);

				Dictionary<string, GmGiftInfo> gmGiftLists = gmActiveInfo.Info.GiftsGroup;
				UserGmActivityInfo userInfo = gmActiveInfo.GetPlayer(player);

				pkg.WriteInt(gmGiftLists.Count);

				foreach (GmGiftInfo gmGiftInfo in gmGiftLists.Values)
				{
					pkg.WriteString(gmGiftInfo.giftbagId);

					GiftCurInfo giftCurInfo = userInfo.GiftsReceivedList.SingleOrDefault(a => a.giftID == gmGiftInfo.giftbagId);
					if (giftCurInfo != null)
					{
						//Console.WriteLine("giftCurInfo: " + gmGiftInfo.activityId + "|" + giftCurInfo.times + "|" + giftCurInfo.allGiftGetTimes);
						pkg.WriteInt(giftCurInfo.times);//_loc2_.times = param1.readInt();
						pkg.WriteInt(giftCurInfo.allGiftGetTimes);//_loc2_.allGiftGetTimes = param1.readInt();
					}
					else
					{
						pkg.WriteInt(0);
						pkg.WriteInt(0);
					}
				}
			}

			SendTCP(pkg);
		}


		public void SendAcademyGradute(GamePlayer app, int type)
		{
			GSPacketIn packet = new GSPacketIn(141);
			packet.WriteByte(11);
			packet.WriteInt(type);
			packet.WriteInt(app.PlayerId);
			packet.WriteString(app.PlayerCharacter.NickName);
			SendTCP(packet);
		}

		public GSPacketIn SendAcademySystemNotice(string text, bool isAlert)
		{
			GSPacketIn packet = new GSPacketIn(141);
			packet.WriteByte(17);
			packet.WriteString(text);
			packet.WriteBoolean(isAlert);
			SendTCP(packet);
			return packet;
		}

		public GSPacketIn SendAcademyAppState(PlayerInfo player, int removeUserId)
		{
			GSPacketIn packet = new GSPacketIn(141);
			packet.WriteByte(10);
			packet.WriteInt(player.apprenticeshipState);
			packet.WriteInt(player.masterID);
			packet.WriteString(player.masterOrApprentices);
			packet.WriteInt(removeUserId);
			packet.WriteInt(player.graduatesCount);
			packet.WriteString(player.honourOfMaster);
			packet.WriteDateTime(player.freezesDate);
			SendTCP(packet);
			return packet;
		}

		public GSPacketIn SendConsortiaTaskInfo(BaseConsortiaTask baseTask)
		{
			GSPacketIn packet = new GSPacketIn(129);
			packet.WriteByte(22);
			packet.WriteByte(3);
			if (baseTask != null)
			{
				packet.WriteInt(baseTask.ConditionList.Count);
				foreach (KeyValuePair<int, ConsortiaTaskInfo> condition in baseTask.ConditionList)
				{
					packet.WriteInt(condition.Key);
					packet.WriteInt(3);
					packet.WriteString(condition.Value.CondictionTitle);
					packet.WriteInt(baseTask.GetTotalValueByConditionPlace(condition.Key));
					packet.WriteInt(condition.Value.Para2);
					packet.WriteInt(baseTask.GetValueByConditionPlace(m_gameClient.Player.PlayerCharacter.ID, condition.Key));
				}
				packet.WriteInt(baseTask.Info.TotalExp);
				packet.WriteInt(baseTask.Info.TotalOffer);
				packet.WriteInt(baseTask.Info.TotalRiches);
				packet.WriteInt(baseTask.Info.BuffID);
				packet.WriteDateTime(baseTask.Info.StartTime);
				packet.WriteInt(baseTask.Info.VaildDate);
			}
			else
			{
				packet.WriteInt(0);
				packet.WriteInt(0);
				packet.WriteInt(0);
				packet.WriteInt(0);
				packet.WriteInt(0);
				packet.WriteDateTime(DateTime.Now);
				packet.WriteInt(0);
			}
			SendTCP(packet);
			return packet;
		}

		public GSPacketIn SendSystemConsortiaChat(string content, bool sendToSelf)
		{
			GSPacketIn packet = new GSPacketIn(129);
			packet.WriteByte(20);
			packet.WriteByte(0);
			packet.WriteString("");
			packet.WriteString(content);
			if (sendToSelf)
			{
				SendTCP(packet);
			}
			return packet;
		}

		public void SendShopGoodsCountUpdate(List<ShopFreeCountInfo> list)
		{
			GSPacketIn packet = new GSPacketIn(168);
			packet.WriteInt(list.Count);
			foreach (ShopFreeCountInfo shopFreeCountInfo in list)
			{
				packet.WriteInt(shopFreeCountInfo.ShopID);
				packet.WriteInt(shopFreeCountInfo.Count);
			}
			packet.WriteInt(0);
			packet.WriteInt(0);
			packet.WriteInt(0);
			SendTCP(packet);
		}

		public void SendEliteGameStartRoom()
		{
			GSPacketIn packet = new GSPacketIn(162);
			packet.WriteByte(2);
			SendTCP(packet);
		}

		public void SendEliteGameInfo(int type)
		{
			GSPacketIn packet = new GSPacketIn(162);
			packet.WriteByte(1);
			packet.WriteInt(type);
			SendTCP(packet);
		}

		public GSPacketIn SendLabyrinthUpdataInfo(int ID, UserLabyrinthInfo laby)
		{
			GSPacketIn packet = new GSPacketIn(131, ID);
			packet.WriteByte(2);
			packet.WriteInt(laby.myProgress);
			packet.WriteInt(laby.currentFloor);
			packet.WriteBoolean(laby.completeChallenge);
			packet.WriteInt(laby.remainTime);
			packet.WriteInt(laby.accumulateExp);
			packet.WriteInt(laby.cleanOutAllTime);
			packet.WriteInt(laby.cleanOutGold);
			packet.WriteInt(laby.myRanking);
			packet.WriteBoolean(laby.isDoubleAward);
			packet.WriteBoolean(laby.isInGame);
			packet.WriteBoolean(laby.isCleanOut);
			packet.WriteBoolean(laby.serverMultiplyingPower);
			SendTCP(packet);
			return packet;
		}

		public GSPacketIn SendUpdateUserPet(PetInventory bag, int[] slots)//Edited
		{
			if (m_gameClient.Player == null)
			{
				return null;
			}
			GSPacketIn packet = new GSPacketIn(68, m_gameClient.Player.PlayerId);
			packet.WriteByte(1);
			packet.WriteInt(m_gameClient.Player.PlayerId);
			packet.WriteInt(m_gameClient.Player.ZoneId);
			packet.WriteInt(slots.Length);
			foreach (int slot in slots)
			{
				packet.WriteInt(slot);
				UsersPetInfo pet = bag.GetPetAt(slot);
				if(pet == null)
                {
					packet.WriteBoolean(false);
                }
				else
                {
					packet.WriteBoolean(true);
					packet.WriteInt(pet.ID);
					packet.WriteInt(pet.TemplateID);
					packet.WriteString(pet.Name);
					packet.WriteInt(pet.UserID);
					packet.WriteInt(pet.Attack);
					packet.WriteInt(pet.Defence);
					packet.WriteInt(pet.Luck);
					packet.WriteInt(pet.Agility);
					packet.WriteInt(pet.Blood);
					packet.WriteInt(pet.Damage);
					packet.WriteInt(pet.Guard);
					packet.WriteInt(pet.AttackGrow);
					packet.WriteInt(pet.DefenceGrow);
					packet.WriteInt(pet.LuckGrow);
					packet.WriteInt(pet.AgilityGrow);
					packet.WriteInt(pet.BloodGrow);
					packet.WriteInt(pet.DamageGrow);
					packet.WriteInt(pet.GuardGrow);
					packet.WriteInt(pet.Level);
					packet.WriteInt(pet.GP);
					packet.WriteInt(pet.MaxGP);
					packet.WriteInt(pet.Hunger);
					packet.WriteInt(pet.PetHappyStar);
					packet.WriteInt(pet.MP);

					string[] skills = string.IsNullOrEmpty(pet.Skill) ? new string[0] : pet.Skill.Split('|');
					int validSkillCount = 0;
					foreach (string skill in skills)
					{
						if (!string.IsNullOrEmpty(skill))
						{
							string[] parts = skill.Split(',');
							if (parts.Length >= 2)
							{
								validSkillCount++;
							}
						}
					}
					packet.WriteInt(validSkillCount);
					foreach (string skill in skills)
					{
						if (!string.IsNullOrEmpty(skill))
						{
							string[] parts = skill.Split(',');
							if (parts.Length >= 2)
							{
								packet.WriteInt(int.Parse(parts[0]));
								packet.WriteInt(int.Parse(parts[1]));
							}
						}
					}
					string[] skillEquips = string.IsNullOrEmpty(pet.SkillEquip) ? new string[0] : pet.SkillEquip.Split('|');
					int validEquipCount = 0;
					foreach (string skill in skillEquips)
					{
						if (!string.IsNullOrEmpty(skill))
						{
							string[] parts = skill.Split(',');
							if (parts.Length >= 2)
							{
								validEquipCount++;
							}
						}
					}
					packet.WriteInt(validEquipCount);
					foreach (string skill in skillEquips)
					{
						if (!string.IsNullOrEmpty(skill))
						{
							string[] parts = skill.Split(',');
							if (parts.Length >= 2)
							{
								packet.WriteInt(int.Parse(parts[1]));
								packet.WriteInt(int.Parse(parts[0]));
							}
						}
					}
					packet.WriteBoolean(pet.IsEquip);
					packet.WriteInt(pet.PetEquips.Count);
					foreach (PetEquipInfo eq in pet.PetEquips)
                    {
						packet.WriteInt(eq.eqType);
						packet.WriteInt(eq.eqTemplateID);
						packet.WriteDateTime(eq.startTime);
						packet.WriteInt(eq.ValidDate);
					}
					packet.WriteInt(pet.currentStarExp);
				}
				/*packet.WriteInt(slot);
				UsersPetInfo petAt = bag.GetPetAt(slot);
				if (petAt == null)
				{
					packet.WriteBoolean(val: false);
					continue;
				}
				packet.WriteBoolean(val: true);
				packet.WriteInt(petAt.ID);
				packet.WriteInt(petAt.TemplateID);
				packet.WriteString(petAt.Name);
				packet.WriteInt(petAt.UserID);
				packet.WriteInt(petAt.Attack);
				packet.WriteInt(petAt.Defence);
				packet.WriteInt(petAt.Luck);
				packet.WriteInt(petAt.Agility);
				packet.WriteInt(petAt.Blood);
				packet.WriteInt(petAt.Damage);
				packet.WriteInt(petAt.Guard);
				packet.WriteInt(petAt.AttackGrow);
				packet.WriteInt(petAt.DefenceGrow);
				packet.WriteInt(petAt.LuckGrow);
				packet.WriteInt(petAt.AgilityGrow);
				packet.WriteInt(petAt.BloodGrow);
				packet.WriteInt(petAt.DamageGrow);
				packet.WriteInt(petAt.GuardGrow);
				packet.WriteInt(petAt.Level);
				packet.WriteInt(petAt.GP);
				packet.WriteInt(petAt.MaxGP);
				packet.WriteInt(petAt.Hunger);
				packet.WriteInt(petAt.PetHappyStar);
				packet.WriteInt(petAt.MP);

				string[] skills = petAt.Skill.Split('|');
				packet.WriteInt(skills.Length);
				foreach (string skill in skills)
				{
					packet.WriteInt(int.Parse(skill.Split(',')[0]));
					packet.WriteInt(int.Parse(skill.Split(',')[1]));
				}

				string[] skillEquips = petAt.SkillEquip.Split('|');
				packet.WriteInt(skillEquips.Length);
				foreach (string skill in skillEquips)
				{
					packet.WriteInt(int.Parse(skill.Split(',')[1]));
					packet.WriteInt(int.Parse(skill.Split(',')[0]));
				}

				packet.WriteBoolean(petAt.IsEquip);
				packet.WriteInt(petAt.PetEquips.Count);
				foreach (PetEquipInfo petEquip in petAt.PetEquips)
				{
					packet.WriteInt(petEquip.eqType);
					packet.WriteInt(petEquip.eqTemplateID);
					packet.WriteDateTime(petEquip.startTime);
					packet.WriteInt(petEquip.ValidDate);
				}
				packet.WriteInt(petAt.currentStarExp);*/
			}
			SendTCP(packet);
			return packet;
		}

		public GSPacketIn SendPetInfo(int id, int zoneId, UsersPetInfo[] pets, EatPetsInfo eatpet)
		{
			GSPacketIn pkg = new GSPacketIn(68, id);
			pkg.WriteByte(1);
			pkg.WriteInt(id);
			pkg.WriteInt(zoneId);
			pkg.WriteInt(pets.Length);

			for (int i = 0; i < pets.Length; i++)
			{
				UsersPetInfo pet = pets[i];
				pkg.WriteInt(pet.Place);
				pkg.WriteBoolean(true);
				pkg.WriteInt(pet.ID);
				pkg.WriteInt(pet.TemplateID);
				pkg.WriteString(pet.Name);
				pkg.WriteInt(pet.UserID);
				pkg.WriteInt(pet.TotalAttack);
				pkg.WriteInt(pet.TotalDefence);
				pkg.WriteInt(pet.TotalLuck);
				pkg.WriteInt(pet.TotalAgility);
				pkg.WriteInt(pet.TotalBlood);
				pkg.WriteInt(pet.Damage);
				pkg.WriteInt(pet.Guard);
				pkg.WriteInt(pet.AttackGrow);
				pkg.WriteInt(pet.DefenceGrow);
				pkg.WriteInt(pet.LuckGrow);
				pkg.WriteInt(pet.AgilityGrow);
				pkg.WriteInt(pet.BloodGrow);
				pkg.WriteInt(pet.DamageGrow);
				pkg.WriteInt(pet.GuardGrow);
				pkg.WriteInt(pet.Level);
				pkg.WriteInt(pet.GP);
				pkg.WriteInt(pet.MaxGP);
				pkg.WriteInt(pet.Hunger);
				pkg.WriteInt(pet.PetHappyStar);
				pkg.WriteInt(pet.MP);

		// Debug: Log pet skill for problematic pets
		if (pet.TemplateID == 300103 || pet.TemplateID == 280103 || pet.TemplateID == 210103)
		{
			log.Info(string.Format("[SendPetInfo] Pet ID={0} TemplateID={1} Name={2}", pet.ID, pet.TemplateID, pet.Name));
			log.Info(string.Format("[SendPetInfo] Skill String: {0}", pet.Skill ?? "NULL"));
			log.Info(string.Format("[SendPetInfo] Skill Length: {0}", pet.Skill?.Length ?? 0));
		}

		string[] skills = string.IsNullOrEmpty(pet.Skill) ? new string[0] : pet.Skill.Split('|');
		int validSkillCount = 0;
				foreach (string skill in skills)
				{
			if (!string.IsNullOrEmpty(skill))
			{
				string[] parts = skill.Split(',');
				if (parts.Length >= 2)
				{
					validSkillCount++;
				}
			}
		}
		pkg.WriteInt(validSkillCount);
		// Debug: Log skill count for problematic pets
		if (pet.TemplateID == 300103 || pet.TemplateID == 280103 || pet.TemplateID == 210103)
		{
			log.Info(string.Format("[SendPetInfo] Valid Skill Count: {0}", validSkillCount));
		}
				foreach (string skill in skills)
				{
					if (!string.IsNullOrEmpty(skill))
					{
						string[] parts = skill.Split(',');
						if (parts.Length >= 2)
						{
							pkg.WriteInt(int.Parse(parts[0]));
							pkg.WriteInt(int.Parse(parts[1]));
						}
					}
				}

				string[] skillEquips = string.IsNullOrEmpty(pet.SkillEquip) ? new string[0] : pet.SkillEquip.Split('|');
				int validEquipCount = 0;
				foreach (string skill in skillEquips)
				{
					if (!string.IsNullOrEmpty(skill))
					{
						string[] parts = skill.Split(',');
						if (parts.Length >= 2)
						{
							validEquipCount++;
						}
					}
				}
				pkg.WriteInt(validEquipCount);
				foreach (string skill in skillEquips)
				{
					if (!string.IsNullOrEmpty(skill))
					{
						string[] parts = skill.Split(',');
						if (parts.Length >= 2)
						{
							pkg.WriteInt(int.Parse(parts[1]));
							pkg.WriteInt(int.Parse(parts[0]));
						}
					}
				}
				pkg.WriteBoolean(pet.IsEquip);
				pkg.WriteInt(pet.PetEquips.Count);
				foreach (PetEquipInfo eq in pet.PetEquips)
				{
					pkg.WriteInt(eq.eqType);
					pkg.WriteInt(eq.eqTemplateID);
					pkg.WriteDateTime(eq.startTime);
					pkg.WriteInt(eq.ValidDate);
				}
				pkg.WriteInt(pet.currentStarExp);
			};
			SendTCP(pkg);
			return pkg;
		}

		public GSPacketIn sendBuyBadge(int consortiaID, int BadgeID, int ValidDate, bool result, string BadgeBuyTime, int playerid)
		{
			GSPacketIn packet = new GSPacketIn(129, playerid);
			packet.WriteByte(28);
			packet.WriteInt(consortiaID);
			packet.WriteInt(BadgeID);
			packet.WriteInt(ValidDate);
			packet.WriteDateTime(Convert.ToDateTime(BadgeBuyTime));
			packet.WriteBoolean(result);
			SendTCP(packet);
			return packet;
		}

		public void SendEdictumVersion()
		{
			EdictumInfo[] allEdictumVersion = WorldMgr.GetAllEdictumVersion();
			Random random = new Random();
			if (allEdictumVersion.Length != 0)
			{
				GSPacketIn packet = new GSPacketIn(75);
				packet.WriteInt(allEdictumVersion.Length);
				EdictumInfo[] array = allEdictumVersion;
				foreach (EdictumInfo edictumInfo in array)
				{
					packet.WriteInt(edictumInfo.ID + random.Next(10000));
				}
				SendTCP(packet);
			}
		}

		public void SendLeftRouleteOpen(UsersExtraInfo info)
		{
			GSPacketIn packet = new GSPacketIn(137);
			packet.WriteInt(1);
			packet.WriteInt(1);
			packet.WriteBoolean(val: true);
			packet.WriteInt((!((double)info.LeftRoutteRate > 0.0)) ? info.LeftRoutteCount : 0);
			packet.WriteString($"{info.LeftRoutteRate:N1}");
			string leftRouterRateData = GameProperties.LeftRouterRateData;
			for (int i = 0; i < leftRouterRateData.Length; i++)
			{
				char ch = leftRouterRateData[i];
				if (ch == '.' || ch == '|')
				{
					packet.WriteInt(0);
				}
				else
				{
					packet.WriteInt(int.Parse(ch.ToString()));
				}
			}
			SendTCP(packet);
		}

		public void SendLeftRouleteResult(UsersExtraInfo info)
		{
			GSPacketIn packet = new GSPacketIn(163);
			packet.WriteInt((!((double)info.LeftRoutteRate > 0.0)) ? info.LeftRoutteCount : 0);
			packet.WriteString($"{info.LeftRoutteRate:N1}");
			SendTCP(packet);
		}

		public void SendEnthrallLight()
		{
			GSPacketIn packet = new GSPacketIn(227);
			packet.WriteBoolean(val: false);
			packet.WriteInt(0);
			packet.WriteBoolean(val: false);
			packet.WriteBoolean(val: false);
			SendTCP(packet);
		}

		public void SendLoginFailed(string msg)
		{
			GSPacketIn packet = new GSPacketIn(1);
			packet.WriteByte(1);
			packet.WriteString(msg);
			SendTCP(packet);
		}

		public void SendOpenNoviceActive(int channel, int activeId, int condition, int awardGot, DateTime startTime, DateTime endTime)
		{
			GSPacketIn packet = new GSPacketIn(258);
			packet.WriteInt(channel);
			switch (channel)
			{
			case 0:
				packet.WriteInt(activeId);
				packet.WriteInt(condition);
				packet.WriteInt(awardGot);
				packet.WriteDateTime(startTime);
				packet.WriteDateTime(endTime);
				break;
			case 1:
				packet.WriteBoolean(val: false);
				break;
			}
			SendTCP(packet);
		}

		public void SendUpdateFirstRecharge(bool isRecharge, bool isGetAward)
		{
			GSPacketIn packet = new GSPacketIn(259);
			packet.WriteBoolean(isRecharge);
			packet.WriteBoolean(isGetAward);
			SendTCP(packet);
		}

		public GSPacketIn SendOpenTimeBox(int condtion, bool isSuccess)
		{
			GSPacketIn packet = new GSPacketIn(53);
			packet.WriteBoolean(isSuccess);
			packet.WriteInt(condtion);
			SendTCP(packet);
			return packet;
		}

		public GSPacketIn SendConsortiaMail(bool result, int playerid)
		{
			GSPacketIn packet = new GSPacketIn(215, playerid);
			packet.WriteBoolean(result);
			SendTCP(packet);
			return packet;
		}

		public GSPacketIn SendAddFriend(PlayerInfo user, int relation, bool state)
		{
			GSPacketIn packet = new GSPacketIn(160, user.ID);
			packet.WriteByte(160);
			packet.WriteBoolean(state);
			if (state)
			{
				packet.WriteInt(user.ID);
				packet.WriteString(user.NickName);
				packet.WriteByte(user.typeVIP);
				packet.WriteInt(user.VIPLevel);
				packet.WriteBoolean(user.Sex);
				packet.WriteString(user.Style);
				packet.WriteString(user.Colors);
				packet.WriteString(user.Skin);
				packet.WriteInt((user.State == 1) ? 1 : 0);
				packet.WriteInt(user.Grade);
				packet.WriteInt(user.Hide);
				packet.WriteString(user.ConsortiaName);
				packet.WriteInt(user.Total);
				packet.WriteInt(user.Escape);
				packet.WriteInt(user.Win);
				packet.WriteInt(user.Offer);
				packet.WriteInt(user.Repute);
				packet.WriteInt(relation);
				packet.WriteString(user.UserName);
				packet.WriteInt(user.Nimbus);
				packet.WriteInt(user.FightPower);
				packet.WriteInt(user.apprenticeshipState);
				packet.WriteInt(user.masterID);
				packet.WriteString(user.masterOrApprentices);
				packet.WriteInt(user.graduatesCount);
				packet.WriteString(user.honourOfMaster);
				packet.WriteInt(user.AchievementPoint);
				packet.WriteString(user.Honor);
				packet.WriteBoolean(user.IsMarried);
			}
			SendTCP(packet);
			return packet;
		}

		public GSPacketIn SendFriendRemove(int FriendID)
		{
			GSPacketIn packet = new GSPacketIn(160, FriendID);
			packet.WriteByte(161);
			SendTCP(packet);
			return packet;
		}

		public GSPacketIn SendFriendState(int playerID, int state, byte typeVip, int viplevel)
		{
			GSPacketIn packet = new GSPacketIn(160, playerID);
			packet.WriteByte(165);
			packet.WriteInt(state);
			packet.WriteInt(typeVip);
			packet.WriteInt(viplevel);
			packet.WriteBoolean(val: true);
			SendTCP(packet);
			return packet;
		}

		public GSPacketIn sendOneOnOneTalk(int receiverID, bool isAutoReply, string SenderNickName, string msg, int playerid)
		{
			GSPacketIn packet = new GSPacketIn(160, playerid);
			packet.WriteByte(51);
			packet.WriteInt(receiverID);
			packet.WriteString(SenderNickName);
			packet.WriteDateTime(DateTime.Now);
			packet.WriteString(msg);
			packet.WriteBoolean(isAutoReply);
			SendTCP(packet);
			return packet;
		}

		public GSPacketIn SendUpdateConsotiaBoss(ConsortiaBossInfo bossInfo)
		{
			GSPacketIn packet = new GSPacketIn(162);
			packet.WriteByte((byte)bossInfo.typeBoss);
			packet.WriteInt(bossInfo.powerPoint);
			packet.WriteInt(bossInfo.callBossCount);
			packet.WriteDateTime(bossInfo.BossOpenTime);
			packet.WriteInt(bossInfo.BossLevel);
			packet.WriteBoolean(val: false);
			SendTCP(packet);
			return packet;
		}

		public GSPacketIn SendUpdateConsotiaBuffer(GamePlayer player, Dictionary<int, BufferInfo> bufflist)
		{
			List<ConsortiaBuffTempInfo> allConsortiaBuff = ConsortiaExtraMgr.GetAllConsortiaBuff();
			GSPacketIn packet = new GSPacketIn(129, player.PlayerId);
			packet.WriteByte(26);
			packet.WriteInt(allConsortiaBuff.Count);
			foreach (ConsortiaBuffTempInfo consortiaBuffTempInfo in allConsortiaBuff)
			{
				if (bufflist.ContainsKey(consortiaBuffTempInfo.id))
				{
					BufferInfo bufferInfo = bufflist[consortiaBuffTempInfo.id];
					packet.WriteInt(bufferInfo.TemplateID);
					packet.WriteBoolean(true);
					packet.WriteDateTime(bufferInfo.BeginDate);
					packet.WriteInt(bufferInfo.ValidDate / 24 / 60);
				}
				else
				{
					packet.WriteInt(consortiaBuffTempInfo.id);
					packet.WriteBoolean(false);
					packet.WriteDateTime(DateTime.Now);
					packet.WriteInt(0);
				}
			}
			SendTCP(packet);
			return packet;
		}

		public GSPacketIn SendPlayerDrill(int ID, Dictionary<int, UserDrillInfo> drills)
		{
			GSPacketIn packet = new GSPacketIn(121, ID);
			packet.WriteByte(6);
			packet.WriteInt(ID);
			packet.WriteInt(drills[0].HoleExp);
			packet.WriteInt(drills[1].HoleExp);
			packet.WriteInt(drills[2].HoleExp);
			packet.WriteInt(0);
			packet.WriteInt(0);
			packet.WriteInt(0);
			packet.WriteInt(drills[0].HoleLv);
			packet.WriteInt(drills[1].HoleLv);
			packet.WriteInt(drills[2].HoleLv);
			packet.WriteInt(0);
			packet.WriteInt(0);
			packet.WriteInt(0);
			SendTCP(packet);
			return packet;
		}

		public GSPacketIn SendUpdateAchievementData(List<AchievementDataInfo> infos)
		{
			bool arg_21_0;
			if (infos != null)
			{
				_ = m_gameClient.Player.PlayerCharacter.ID;
				arg_21_0 = true;
			}
			else
			{
				arg_21_0 = false;
			}
			if (!arg_21_0)
			{
				return null;
			}
			GSPacketIn pkg = new GSPacketIn(231, m_gameClient.Player.PlayerCharacter.ID);
			pkg.WriteInt(infos.Count);
			for (int i = 0; i < infos.Count; i++)
			{
				AchievementDataInfo info = infos[i];
				pkg.WriteInt(info.AchievementID);
				pkg.WriteInt(info.CompletedDate.Year);
				pkg.WriteInt(info.CompletedDate.Month);
				pkg.WriteInt(info.CompletedDate.Day);
			}
			SendTCP(pkg);
			return pkg;
		}

		public GSPacketIn SendAchievementSuccess(AchievementDataInfo d)
		{
			GSPacketIn packet = new GSPacketIn(230);
			packet.WriteInt(d.AchievementID);
			packet.WriteInt(d.CompletedDate.Year);
			packet.WriteInt(d.CompletedDate.Month);
			packet.WriteInt(d.CompletedDate.Day);
			SendTCP(packet);
			return packet;
		}

		public GSPacketIn SendUpdateAchievements(List<UsersRecordInfo> infos)
		{
			bool arg_36_0;
			if (infos != null && m_gameClient != null && m_gameClient.Player != null)
			{
				_ = m_gameClient.Player.PlayerCharacter.ID;
				arg_36_0 = true;
			}
			else
			{
				arg_36_0 = false;
			}
			if (!arg_36_0)
			{
				return null;
			}
			GSPacketIn pkg = new GSPacketIn(229, m_gameClient.Player.PlayerCharacter.ID);
			pkg.WriteInt(infos.Count);
			for (int i = 0; i < infos.Count; i++)
			{
				UsersRecordInfo info = infos[i];
				pkg.WriteInt(info.RecordID);
				pkg.WriteInt(info.Total);
			}
			SendTCP(pkg);
			return pkg;
		}

		public GSPacketIn SendUpdateAchievements(UsersRecordInfo info)
		{
			bool arg_36_0;
			if (info != null && m_gameClient != null && m_gameClient.Player != null)
			{
				_ = m_gameClient.Player.PlayerCharacter.ID;
				arg_36_0 = true;
			}
			else
			{
				arg_36_0 = false;
			}
			if (!arg_36_0)
			{
				return null;
			}
			GSPacketIn pkg = new GSPacketIn(229, m_gameClient.Player.PlayerCharacter.ID);
			pkg.WriteInt(1);
			for (int i = 0; i < 1; i++)
			{
				pkg.WriteInt(info.RecordID);
				pkg.WriteInt(info.Total);
			}
			SendTCP(pkg);
			return pkg;
		}

		public GSPacketIn SendInitAchievements(List<UsersRecordInfo> infos)
		{
			bool arg_2E_0;
			if (infos != null && m_gameClient.Player != null)
			{
				_ = m_gameClient.Player.PlayerCharacter.ID;
				arg_2E_0 = true;
			}
			else
			{
				arg_2E_0 = false;
			}
			if (!arg_2E_0)
			{
				return null;
			}
			GSPacketIn pkg = new GSPacketIn(228, m_gameClient.Player.PlayerCharacter.ID);
			pkg.WriteInt(infos.Count);
			for (int i = 0; i < infos.Count; i++)
			{
				UsersRecordInfo info = infos[i];
				pkg.WriteInt(info.RecordID);
				pkg.WriteInt(info.Total);
			}
			SendTCP(pkg);
			SendUpdateAchievements(infos);
			return pkg;
		}

		public void SendLoginSuccess()
		{
			if (m_gameClient.Player != null)
			{
				GSPacketIn packet = new GSPacketIn(1, m_gameClient.Player.PlayerCharacter.ID);
				packet.WriteByte(0);
				packet.WriteInt(m_gameClient.Player.ZoneId);
				packet.WriteInt(m_gameClient.Player.PlayerCharacter.Attack);
				packet.WriteInt(m_gameClient.Player.PlayerCharacter.Defence);
				packet.WriteInt(m_gameClient.Player.PlayerCharacter.Agility);
				packet.WriteInt(m_gameClient.Player.PlayerCharacter.Luck);
				packet.WriteInt(m_gameClient.Player.PlayerCharacter.GP);
				packet.WriteInt(m_gameClient.Player.PlayerCharacter.Repute);
				packet.WriteInt(m_gameClient.Player.PlayerCharacter.Gold);
				packet.WriteInt(m_gameClient.Player.PlayerCharacter.Money + m_gameClient.Player.PlayerCharacter.MoneyLock);
				packet.WriteInt(m_gameClient.Player.GetMedalNum());
				packet.WriteInt(0);
				packet.WriteInt(m_gameClient.Player.PlayerCharacter.Hide);
				packet.WriteInt(m_gameClient.Player.PlayerCharacter.FightPower);
				packet.WriteInt(m_gameClient.Player.PlayerCharacter.apprenticeshipState);
				packet.WriteInt(m_gameClient.Player.PlayerCharacter.masterID);
				packet.WriteString(m_gameClient.Player.PlayerCharacter.masterOrApprentices);
				packet.WriteInt(m_gameClient.Player.PlayerCharacter.graduatesCount);
				packet.WriteString(m_gameClient.Player.PlayerCharacter.honourOfMaster);
				packet.WriteDateTime(m_gameClient.Player.PlayerCharacter.freezesDate);
				packet.WriteByte(m_gameClient.Player.PlayerCharacter.typeVIP);
				packet.WriteInt(m_gameClient.Player.PlayerCharacter.VIPLevel);
				packet.WriteInt(m_gameClient.Player.PlayerCharacter.VIPExp);
				packet.WriteDateTime(m_gameClient.Player.PlayerCharacter.VIPExpireDay);
				packet.WriteDateTime(m_gameClient.Player.PlayerCharacter.LastDate);
				packet.WriteInt(m_gameClient.Player.PlayerCharacter.VIPNextLevelDaysNeeded);
				packet.WriteDateTime(DateTime.Now);
				packet.WriteBoolean(m_gameClient.Player.PlayerCharacter.CanTakeVipReward);
				packet.WriteInt(m_gameClient.Player.PlayerCharacter.OptionOnOff);
				packet.WriteInt(m_gameClient.Player.PlayerCharacter.AchievementPoint);
				packet.WriteString(m_gameClient.Player.PlayerCharacter.Honor);
				packet.WriteInt(m_gameClient.Player.PlayerCharacter.honorId);
				packet.WriteInt(m_gameClient.Player.PlayerCharacter.OnlineTime);
				packet.WriteBoolean(m_gameClient.Player.PlayerCharacter.Sex);
				packet.WriteString(m_gameClient.Player.PlayerCharacter.Style + "&" + m_gameClient.Player.PlayerCharacter.Colors);
				packet.WriteString(m_gameClient.Player.PlayerCharacter.Skin);
				packet.WriteInt(m_gameClient.Player.PlayerCharacter.ConsortiaID);
				packet.WriteString(m_gameClient.Player.PlayerCharacter.ConsortiaName);
				packet.WriteInt(m_gameClient.Player.PlayerCharacter.badgeID);
				packet.WriteInt(m_gameClient.Player.PlayerCharacter.DutyLevel);
				packet.WriteString(m_gameClient.Player.PlayerCharacter.DutyName);
				packet.WriteInt(m_gameClient.Player.PlayerCharacter.Right);
				packet.WriteString(m_gameClient.Player.PlayerCharacter.ChairmanName);
				packet.WriteInt(m_gameClient.Player.PlayerCharacter.ConsortiaHonor);
				packet.WriteInt(m_gameClient.Player.PlayerCharacter.ConsortiaRiches);
				packet.WriteBoolean(m_gameClient.Player.PlayerCharacter.HasBagPassword);
				packet.WriteString(m_gameClient.Player.PlayerCharacter.PasswordQuest1);
				packet.WriteString(m_gameClient.Player.PlayerCharacter.PasswordQuest2);
				packet.WriteInt(m_gameClient.Player.PlayerCharacter.FailedPasswordAttemptCount);
				packet.WriteString(m_gameClient.Player.PlayerCharacter.UserName);
				packet.WriteInt(m_gameClient.Player.PlayerCharacter.Nimbus);
				packet.WriteString(m_gameClient.Player.PlayerCharacter.PvePermission);
				packet.WriteString(m_gameClient.Player.PlayerCharacter.FightLabPermission);
				packet.WriteInt(99999);
				packet.WriteInt(m_gameClient.Player.PlayerCharacter.BoxProgression);
				packet.WriteInt(m_gameClient.Player.PlayerCharacter.GetBoxLevel);
				packet.WriteInt(m_gameClient.Player.PlayerCharacter.AlreadyGetBox);
				packet.WriteDateTime(m_gameClient.Player.Extra.Info.LastTimeHotSpring);
				packet.WriteDateTime(m_gameClient.Player.PlayerCharacter.ShopFinallyGottenTime);
				packet.WriteInt(m_gameClient.Player.PlayerCharacter.Riches);
				packet.WriteInt(m_gameClient.Player.PlayerCharacter.DailyScore);
				packet.WriteInt(m_gameClient.Player.PlayerCharacter.DailyWinCount);
				packet.WriteInt(m_gameClient.Player.PlayerCharacter.DailyGameCount);
				packet.WriteBoolean(m_gameClient.Player.PlayerCharacter.DailyLeagueFirst);
				packet.WriteInt(m_gameClient.Player.PlayerCharacter.DailyLeagueLastScore);
				packet.WriteInt(m_gameClient.Player.PlayerCharacter.WeeklyScore);
				packet.WriteInt(m_gameClient.Player.PlayerCharacter.WeeklyGameCount);
				packet.WriteInt(m_gameClient.Player.PlayerCharacter.WeeklyRanking);
				packet.WriteInt(m_gameClient.Player.PlayerCharacter.Texp.spdTexpExp);
				packet.WriteInt(m_gameClient.Player.PlayerCharacter.Texp.attTexpExp);
				packet.WriteInt(m_gameClient.Player.PlayerCharacter.Texp.defTexpExp);
				packet.WriteInt(m_gameClient.Player.PlayerCharacter.Texp.hpTexpExp);
				packet.WriteInt(m_gameClient.Player.PlayerCharacter.Texp.lukTexpExp);
				packet.WriteInt(m_gameClient.Player.PlayerCharacter.Texp.texpTaskCount);
				packet.WriteInt(m_gameClient.Player.PlayerCharacter.Texp.texpCount);
				packet.WriteDateTime(m_gameClient.Player.PlayerCharacter.Texp.texpTaskDate);
				packet.WriteInt(m_gameClient.Player.PlayerCharacter.fineSuitExp);
				packet.WriteBoolean(val: false);//IsOldPlayerHasValidEquitAtLogin
				packet.WriteInt(m_gameClient.Player.PlayerCharacter.badLuckNumber);//BadLuck
				packet.WriteInt(0);//luckynum
				packet.WriteDateTime(DateTime.Now);//lastluckynumDate
				packet.WriteInt(0);//lastLuckNum
				packet.WriteInt(m_gameClient.Player.PlayerCharacter.accumulativeLoginDays);
				packet.WriteInt(m_gameClient.Player.PlayerCharacter.accumulativeAwardDays);
				packet.WriteInt(m_gameClient.Player.PlayerCharacter.totemId);//totemId
				packet.WriteInt(m_gameClient.Player.PlayerCharacter.necklaceExp);
				packet.WriteInt(m_gameClient.Player.PlayerCharacter.RingExp);
                #region Explorer Manual
                packet.WriteInt(m_gameClient.Player.PlayerCharacter.explorerManualInfo.manualLevel);//_loc3_.manualProInfo.manual_Level = _loc2_.readInt();
                packet.WriteInt(m_gameClient.Player.PlayerCharacter.explorerManualInfo.Agility);//_loc3_.manualProInfo.pro_Agile = _loc2_.readInt();
                packet.WriteInt(m_gameClient.Player.PlayerCharacter.explorerManualInfo.Armor);//_loc3_.manualProInfo.pro_Armor = _loc2_.readInt();
                packet.WriteInt(m_gameClient.Player.PlayerCharacter.explorerManualInfo.Attack);// _loc3_.manualProInfo.pro_Attack = _loc2_.readInt();
                packet.WriteInt(m_gameClient.Player.PlayerCharacter.explorerManualInfo.Damage);//_loc3_.manualProInfo.pro_Damage = _loc2_.readInt();
                packet.WriteInt(m_gameClient.Player.PlayerCharacter.explorerManualInfo.Defense);//_loc3_.manualProInfo.pro_Defense = _loc2_.readInt();
                packet.WriteInt(m_gameClient.Player.PlayerCharacter.explorerManualInfo.HP);//_loc3_.manualProInfo.pro_HP = _loc2_.readInt();
                packet.WriteInt(m_gameClient.Player.PlayerCharacter.explorerManualInfo.Lucky);//_loc3_.manualProInfo.pro_Lucky = _loc2_.readInt();
                packet.WriteInt(m_gameClient.Player.PlayerCharacter.explorerManualInfo.MagicAttack);//_loc3_.manualProInfo.pro_MagicAttack = _loc2_.readInt();
                packet.WriteInt(m_gameClient.Player.PlayerCharacter.explorerManualInfo.MagicResistance);//_loc3_.manualProInfo.pro_MagicResistance = _loc2_.readInt();
                packet.WriteInt(m_gameClient.Player.PlayerCharacter.explorerManualInfo.Stamina);//_loc3_.manualProInfo.pro_Stamina = _loc2_.readInt();
                #endregion
                SendTCP(packet);
			}
		}

		public void SendLoginSuccess2()
		{
		}

		public void method_0(byte[] m, byte[] e)
		{
			GSPacketIn packet = new GSPacketIn(7);
			packet.Write(m);
			packet.Write(e);
			SendTCP(packet);
		}

		public void SendCheckCode()
		{
			if (m_gameClient.Player != null && m_gameClient.Player.PlayerCharacter.Grade >= 5 && (m_gameClient.Player.PlayerCharacter.CheckCount >= 10 || m_gameClient.Player.PlayerCharacter.ChatCount >= 999999 || m_gameClient.Player.FirstLoginCheckCode || m_gameClient.Player.PlayerCharacter.CheckTimer >= 15))
			{

				m_gameClient.Player.FirstLoginCheckCode = false;
				m_gameClient.Player.PlayerCharacter.CheckTimer = 0;
				if (m_gameClient.Player.PlayerCharacter.CheckError == 0 && m_gameClient.Player.PlayerCharacter.ChatCount == 0)
				{
					m_gameClient.Player.PlayerCharacter.CheckCount += 10000;
				}
				GSPacketIn pkg = new GSPacketIn((byte)ePackageType.CHECK_CODE, m_gameClient.Player.PlayerCharacter.ID, 10240);
				if (m_gameClient.Player.PlayerCharacter.CheckError < 1)
				{
					pkg.WriteByte(1);
				}
				else
				{
					pkg.WriteByte(2);
				}
				pkg.WriteBoolean(true);
				pkg.WriteByte(2);
				if (m_gameClient.Player.PlayerCharacter.ChatCount == 5)
				{
					pkg.WriteString(LanguageMgr.GetTranslation("CheckCode.Chat"));
				}
				else
				{
					pkg.WriteString(LanguageMgr.GetTranslation("CheckCode.Fight"));
				}
				m_gameClient.Player.PlayerCharacter.CheckCode = CheckCode.GenerateCheckCode();
				pkg.Write(CheckCode.CreateImage(m_gameClient.Player.PlayerCharacter.CheckCode));
				SendTCP(pkg);
			}
		}

		public void SendKitoff(string msg)
		{
			GSPacketIn packet = new GSPacketIn(2);
			packet.WriteString(msg);
			SendTCP(packet);
		}

		public void SendEditionError(string msg)
		{
			GSPacketIn packet = new GSPacketIn(12);
			packet.WriteString(msg);
			SendTCP(packet);
		}

		public void SendWaitingRoom(bool result)
		{
			GSPacketIn packet = new GSPacketIn(16);
			packet.WriteByte((byte)(result ? 1 : 0));
			SendTCP(packet);
		}

		public GSPacketIn SendPlayerState(int id, byte state)
		{
			GSPacketIn packet = new GSPacketIn(32, id);
			packet.WriteByte(state);
			SendTCP(packet);
			return packet;
		}

		public virtual GSPacketIn SendMessage(eMessageType type, string message)
		{
			GSPacketIn packet = new GSPacketIn(3);
			packet.WriteInt((int)type);
			packet.WriteString(message);
			SendTCP(packet);
			return packet;
		}

		public void SendReady()
		{
			SendTCP(new GSPacketIn(0));
		}

		public void SendUpdatePrivateInfo(PlayerInfo info, int medal)
		{
			GSPacketIn packet = new GSPacketIn(38, info.ID);
			packet.WriteInt(info.Money + info.MoneyLock);
			packet.WriteInt(medal);
			packet.WriteInt(info.Score);
			packet.WriteInt(info.Gold);
			packet.WriteInt(info.GiftToken);
			packet.WriteInt(info.badLuckNumber);
			packet.WriteInt(info.petScore);
			packet.WriteInt(info.hardCurrency);
			packet.WriteInt(info.damageScores);//damageScore
			packet.WriteInt(info.myHonor);//myHonor
			packet.WriteInt(info.jampsCurrency);//ExploreManual
			SendTCP(packet);
		}

		public GSPacketIn SendUpdatePublicPlayer(PlayerInfo info, UserMatchInfo matchInfo, UsersExtraInfo extraInfo)
		{
			GSPacketIn packet = new GSPacketIn(67, info.ID);
			packet.WriteInt(info.GP);
			packet.WriteInt(info.Offer);
			packet.WriteInt(info.RichesOffer);
			packet.WriteInt(info.RichesRob);
			packet.WriteInt(info.Win);
			packet.WriteInt(info.Total);
			packet.WriteInt(info.Escape);
			packet.WriteInt(info.Attack);
			packet.WriteInt(info.Defence);
			packet.WriteInt(info.Agility);
			packet.WriteInt(info.Luck);
			packet.WriteInt(info.hp);
			packet.WriteInt(info.Hide);
			packet.WriteString(info.Style);
			packet.WriteString(info.Colors);
			packet.WriteString(info.Skin);
			packet.WriteBoolean(info.IsShowConsortia);
			packet.WriteInt(info.ConsortiaID);
			packet.WriteString(info.ConsortiaName);
			packet.WriteInt(info.badgeID);
			packet.WriteInt(0);
			packet.WriteInt(0);
			packet.WriteInt(info.Nimbus);
			packet.WriteString(info.PvePermission);
			packet.WriteString(info.FightLabPermission);
			packet.WriteInt(info.FightPower);
			packet.WriteInt(info.apprenticeshipState);
			packet.WriteInt(info.masterID);
			packet.WriteString(info.masterOrApprentices);
			packet.WriteInt(info.graduatesCount);
			packet.WriteString(info.honourOfMaster);
			packet.WriteInt(info.AchievementPoint);
			packet.WriteString(info.Honor);
			packet.WriteDateTime(info.LastSpaDate);
			packet.WriteInt(info.charmGP);
			packet.WriteInt(info.charmLevel);
			packet.WriteDateTime(info.ShopFinallyGottenTime);
			packet.WriteInt(info.Riches);
			packet.WriteInt(info.DailyScore);
			packet.WriteInt(info.DailyWinCount);
			packet.WriteInt(info.DailyGameCount);
			packet.WriteInt(info.WeeklyScore);
			packet.WriteInt(info.WeeklyGameCount);
			packet.WriteInt(info.Texp.spdTexpExp);
			packet.WriteInt(info.Texp.attTexpExp);
			packet.WriteInt(info.Texp.defTexpExp);
			packet.WriteInt(info.Texp.hpTexpExp);
			packet.WriteInt(info.Texp.lukTexpExp);
			packet.WriteInt(info.Texp.texpTaskCount);
			packet.WriteInt(info.Texp.texpCount);
			packet.WriteDateTime(info.Texp.texpTaskDate);
			packet.WriteInt(0);
			packet.WriteInt(info.evolutionGrade);
			packet.WriteInt(info.evolutionExp);
			SendTCP(packet);
			return packet;
		}

		public void SendPingTime(GamePlayer player)
		{
			GSPacketIn packet = new GSPacketIn(4);
			player.PingStart = DateTime.Now.Ticks;
			packet.WriteInt(player.PlayerCharacter.AntiAddiction);
			SendTCP(packet);
		}

		public GSPacketIn SendNetWork(int id, long delay)
		{
			GSPacketIn packet = new GSPacketIn(6, id);
			packet.WriteInt((int)delay / 1000 / 10);
			SendTCP(packet);
			return packet;
		}

		public GSPacketIn SendUserEquip(PlayerInfo player, List<ItemInfo> items, List<UserGemStone> userGemStone, ExplorerManualInfo explorerInfo)
		{
			GSPacketIn packet = new GSPacketIn(74, player.ID);
			packet.WriteInt(player.ID);
			packet.WriteString(player.NickName);
			packet.WriteInt(player.Agility);
			packet.WriteInt(player.Attack);
			packet.WriteString(player.Colors);
			packet.WriteString(player.Skin);
			packet.WriteInt(player.Defence);
			packet.WriteInt(player.GP);
			packet.WriteInt(player.Grade);
			packet.WriteInt(player.Luck);
			packet.WriteInt(player.hp);
			packet.WriteInt(player.Hide);
			packet.WriteInt(player.Repute);
			packet.WriteBoolean(player.Sex);
			packet.WriteString(player.Style);
			packet.WriteInt(player.Offer);
			packet.WriteByte(player.typeVIP);
			packet.WriteInt(player.VIPLevel);
			packet.WriteInt(player.Win);
			packet.WriteInt(player.Total);
			packet.WriteInt(player.Escape);
			packet.WriteInt(player.ConsortiaID);
			packet.WriteString(player.ConsortiaName);
			packet.WriteInt(player.badgeID);
			packet.WriteInt(player.RichesOffer);
			packet.WriteInt(player.RichesRob);
			packet.WriteBoolean(player.IsMarried);
			packet.WriteInt(player.SpouseID);
			packet.WriteString(player.SpouseName);
			packet.WriteString(player.DutyName);
			packet.WriteInt(player.Nimbus);
			packet.WriteInt(player.FightPower);
			packet.WriteInt(player.apprenticeshipState);
			packet.WriteInt(player.masterID);
			packet.WriteString(player.masterOrApprentices);
			packet.WriteInt(player.graduatesCount);
			packet.WriteString(player.honourOfMaster);
			packet.WriteInt(player.AchievementPoint);
			packet.WriteString(player.Honor);
			packet.WriteDateTime(DateTime.Now.AddDays(-2.0));
			packet.WriteInt(player.Texp.spdTexpExp);
			packet.WriteInt(player.Texp.attTexpExp);
			packet.WriteInt(player.Texp.defTexpExp);
			packet.WriteInt(player.Texp.hpTexpExp);
			packet.WriteInt(player.Texp.lukTexpExp);
			packet.WriteBoolean(player.DailyLeagueFirst);//DailyLeagueFirst
			packet.WriteInt(player.DailyLeagueLastScore);
			packet.WriteInt(player.totemId);
			packet.WriteInt(player.necklaceExp);
			packet.WriteInt(player.RingExp);
			packet.WriteInt(items.Count);
			foreach (ItemInfo itemInfo in items)
			{
				packet.WriteByte((byte)itemInfo.BagType);
				packet.WriteInt(itemInfo.UserID);
				packet.WriteInt(itemInfo.ItemID);
				packet.WriteInt(itemInfo.Count);
				packet.WriteInt(itemInfo.Place);
				packet.WriteInt(itemInfo.TemplateID);
				packet.WriteInt(itemInfo.AttackCompose);
				packet.WriteInt(itemInfo.DefendCompose);
				packet.WriteInt(itemInfo.AgilityCompose);
				packet.WriteInt(itemInfo.LuckCompose);
				packet.WriteInt(itemInfo.StrengthenLevel);
				packet.WriteBoolean(itemInfo.IsBinds);
				packet.WriteBoolean(itemInfo.IsJudge);
				packet.WriteDateTime(itemInfo.BeginDate);
				packet.WriteInt(itemInfo.ValidDate);
				packet.WriteString(itemInfo.Color);
				packet.WriteString(itemInfo.Skin);
				packet.WriteBoolean(itemInfo.IsUsed);
				packet.WriteInt(itemInfo.Hole1);
				packet.WriteInt(itemInfo.Hole2);
				packet.WriteInt(itemInfo.Hole3);
				packet.WriteInt(itemInfo.Hole4);
				packet.WriteInt(itemInfo.Hole5);
				packet.WriteInt(itemInfo.Hole6);
				packet.WriteString(itemInfo.Pic);
				packet.WriteInt(itemInfo.RefineryLevel);
				packet.WriteDateTime(DateTime.Now);
				packet.WriteByte((byte)itemInfo.Hole5Level);
				packet.WriteInt(itemInfo.Hole5Exp);
				packet.WriteByte((byte)itemInfo.Hole6Level);
				packet.WriteInt(itemInfo.Hole6Exp);
				packet.WriteBoolean(itemInfo.isGold);
				if (itemInfo.isGold)
				{
					packet.WriteInt(itemInfo.goldValidDate);
					packet.WriteDateTime(itemInfo.goldBeginTime);
				}
				packet.WriteString(itemInfo.latentEnergyCurStr);
				packet.WriteString(itemInfo.latentEnergyNewStr);
				packet.WriteDateTime(itemInfo.latentEnergyEndTime);
			}
			packet.WriteInt(userGemStone.Count);
			for (int p = 0; p < userGemStone.Count; p++)
			{
				packet.WriteInt(userGemStone[p].FigSpiritId);
				packet.WriteString(userGemStone[p].FigSpiritIdValue);
				packet.WriteInt(userGemStone[p].EquipPlace);
            }
			Dictionary<string, UserEquipGhostInfo> equipGhostList = JsonConvert.DeserializeObject<Dictionary<string, UserEquipGhostInfo>>(player.GhostEquipList);
			if (equipGhostList == null)
			{
				packet.WriteInt(0);
			}
			else
			{
				packet.WriteInt(equipGhostList.Count);

				foreach (UserEquipGhostInfo egInfo in equipGhostList.Values)
				{
					packet.WriteInt(egInfo.BagType);
					packet.WriteInt(egInfo.Place);
					packet.WriteInt(egInfo.Level);
					packet.WriteInt(egInfo.TotalGhost);
				}
			}

            packet.WriteInt(explorerInfo.manualLevel); //_loc7_.manualProInfo.manual_Level = _loc14_.readInt();
            packet.WriteInt(explorerInfo.Agility); //_loc7_.manualProInfo.pro_Agile = _loc14_.readInt();
            packet.WriteInt(explorerInfo.Armor); //_loc7_.manualProInfo.pro_Armor = _loc14_.readInt();
            packet.WriteInt(explorerInfo.Attack); //_loc7_.manualProInfo.pro_Attack = _loc14_.readInt();
            packet.WriteInt(explorerInfo.Damage); //_loc7_.manualProInfo.pro_Damage = _loc14_.readInt();
            packet.WriteInt(explorerInfo.Defense); //_loc7_.manualProInfo.pro_Defense = _loc14_.readInt();
            packet.WriteInt(explorerInfo.HP); //_loc7_.manualProInfo.pro_HP = _loc14_.readInt();
            packet.WriteInt(explorerInfo.Lucky); //_loc7_.manualProInfo.pro_Lucky = _loc14_.readInt();
            packet.WriteInt(explorerInfo.MagicAttack); //_loc7_.manualProInfo.pro_MagicAttack = _loc14_.readInt();
            packet.WriteInt(explorerInfo.MagicResistance); //_loc7_.manualProInfo.pro_MagicResistance = _loc14_.readInt();
            packet.WriteInt(explorerInfo.Stamina); //_loc7_.manualProInfo.pro_Stamina = _loc14_.readInt();

            packet.Compress();
			SendTCP(packet);
			return packet;
		}

		public void SendDateTime()
		{
			GSPacketIn packet = new GSPacketIn(5);
			packet.WriteDateTime(DateTime.Now);
			SendTCP(packet);
		}

		public GSPacketIn SendDailyAward(GamePlayer player)
		{
			bool val = false;
			DateTime date3 = DateTime.Now.Date;
			DateTime date2 = player.PlayerCharacter.LastAward.Date;
			if (date3 != date2)
			{
				val = true;
			}
			GSPacketIn packet = new GSPacketIn(13);
			packet.WriteBoolean(val);
			packet.WriteInt(0);
			SendTCP(packet);
			return packet;
		}

		public GSPacketIn SendUpdateRoomList(List<BaseRoom> roomlist)
		{
			GSPacketIn pkg = new GSPacketIn((byte)ePackageType.GAME_ROOM);
			pkg.WriteByte((byte)eRoomPackageType.ROOMLIST_UPDATE);
			pkg.WriteInt(roomlist.Count);
			var length = roomlist.Count < 9 ? roomlist.Count : 9;
			pkg.WriteInt(length);
			for (int i = 0; i < length; i++)
			{
				BaseRoom room = roomlist[i];
				pkg.WriteInt(room.RoomId);
				pkg.WriteByte((byte)room.RoomType);
				pkg.WriteByte(room.TimeMode);
				pkg.WriteByte((byte)room.PlayerCount);
				pkg.WriteByte(0);
				pkg.WriteByte(0);
				pkg.WriteByte((byte)room.PlacesCount);
				pkg.WriteBoolean(string.IsNullOrEmpty(room.Password) ? false : true);
				pkg.WriteInt(room.MapId);
				pkg.WriteBoolean(room.IsPlaying);
				pkg.WriteString(room.Name);
				pkg.WriteByte((byte)room.GameType);
				pkg.WriteByte((byte)room.HardLevel);
				pkg.WriteInt(room.LevelLimits);
				pkg.WriteBoolean(room.isOpenBoss);
			}

			SendTCP(pkg);
			return pkg;
		}

		public GSPacketIn SendSceneAddPlayer(GamePlayer player)
		{
			GSPacketIn packet = new GSPacketIn(18, player.PlayerCharacter.ID);
			packet.WriteInt(player.PlayerCharacter.Grade);
			packet.WriteBoolean(player.PlayerCharacter.Sex);
			packet.WriteString(player.PlayerCharacter.NickName);
			packet.WriteByte(player.PlayerCharacter.typeVIP);
			packet.WriteInt(player.PlayerCharacter.VIPLevel);
			packet.WriteString(player.PlayerCharacter.ConsortiaName);
			packet.WriteInt(player.PlayerCharacter.Offer);
			packet.WriteInt(player.PlayerCharacter.Win);
			packet.WriteInt(player.PlayerCharacter.Total);
			packet.WriteInt(player.PlayerCharacter.Escape);
			packet.WriteInt(player.PlayerCharacter.ConsortiaID);
			packet.WriteInt(player.PlayerCharacter.Repute);
			packet.WriteBoolean(player.PlayerCharacter.IsMarried);
			if (player.PlayerCharacter.IsMarried)
			{
				packet.WriteInt(player.PlayerCharacter.SpouseID);
				packet.WriteString(player.PlayerCharacter.SpouseName);
			}
			packet.WriteString(player.PlayerCharacter.UserName);
			packet.WriteInt(player.PlayerCharacter.FightPower);
			packet.WriteInt(player.PlayerCharacter.apprenticeshipState);
			SendTCP(packet);
			return packet;
		}

		public GSPacketIn SendSceneRemovePlayer(GamePlayer player)
		{
			GSPacketIn packet = new GSPacketIn(21, player.PlayerCharacter.ID);
			SendTCP(packet);
			return packet;
		}

		public GSPacketIn SendRoomPlayerAdd(GamePlayer player)
		{
			GSPacketIn packet = new GSPacketIn(94, player.PlayerId);
			packet.WriteByte(4);
			bool isInGame = player.CurrentRoom != null;
			packet.WriteBoolean(isInGame);
			packet.WriteByte((byte)player.CurrentRoomIndex);
			packet.WriteByte((byte)player.CurrentRoomTeam);
			packet.WriteBoolean(false);
			packet.WriteInt(player.PlayerCharacter.Grade);
			packet.WriteInt(player.PlayerCharacter.Offer);
			packet.WriteInt(player.PlayerCharacter.Hide);
			packet.WriteInt(player.PlayerCharacter.Repute);
			packet.WriteInt((int)player.PingTime / 1000 / 10);
			packet.WriteInt(player.ZoneId);
			packet.WriteInt(player.PlayerCharacter.ID);
			packet.WriteString(player.PlayerCharacter.NickName);
			packet.WriteByte(player.PlayerCharacter.typeVIP);
			packet.WriteInt(player.PlayerCharacter.VIPLevel);
			packet.WriteBoolean(player.PlayerCharacter.Sex);
			packet.WriteString(player.PlayerCharacter.Style);
			packet.WriteString(player.PlayerCharacter.Colors);
			packet.WriteString(player.PlayerCharacter.Skin);
			packet.WriteInt(player.EquipBag.GetItemAt(6)?.TemplateID ?? (-1));
			if (player.SecondWeapon == null)
			{
				packet.WriteInt(0);
			}
			else
			{
				packet.WriteInt(player.SecondWeapon.TemplateID);
			}
			packet.WriteInt(player.PlayerCharacter.ConsortiaID);
			packet.WriteString(player.PlayerCharacter.ConsortiaName);
			packet.WriteInt(player.PlayerCharacter.badgeID);
			packet.WriteInt(player.PlayerCharacter.Win);
			packet.WriteInt(player.PlayerCharacter.Total);
			packet.WriteInt(player.PlayerCharacter.Escape);
			packet.WriteInt(player.PlayerCharacter.ConsortiaLevel);
			packet.WriteInt(player.PlayerCharacter.ConsortiaRepute);
			packet.WriteBoolean(player.PlayerCharacter.IsMarried);
			if (player.PlayerCharacter.IsMarried)
			{
				packet.WriteInt(player.PlayerCharacter.SpouseID);
				packet.WriteString(player.PlayerCharacter.SpouseName);
			}
			packet.WriteString(player.PlayerCharacter.UserName);
			packet.WriteInt(player.PlayerCharacter.Nimbus);
			packet.WriteInt(player.PlayerCharacter.FightPower);
			packet.WriteInt(player.PlayerCharacter.apprenticeshipState);
			packet.WriteInt(player.PlayerCharacter.masterID);
			packet.WriteString(player.PlayerCharacter.masterOrApprentices);
			packet.WriteInt(player.PlayerCharacter.graduatesCount);
			packet.WriteString(player.PlayerCharacter.honourOfMaster);
			packet.WriteBoolean(player.PlayerCharacter.DailyLeagueFirst); //_loc_14.DailyLeagueFirst = _loc_2.readBoolean();
			packet.WriteInt(player.PlayerCharacter.DailyLeagueLastScore); //_loc_14.DailyLeagueLastScore = _loc_2.readInt();
			if (player.Pet == null)
			{
				packet.WriteInt(0);
			}
			else
			{
				packet.WriteInt(1);
				packet.WriteInt(player.Pet.Place);
				packet.WriteInt(player.Pet.TemplateID);
				packet.WriteInt(player.Pet.ID);
				packet.WriteString(player.Pet.Name);
				packet.WriteInt(player.PlayerCharacter.ID);
				packet.WriteInt(player.Pet.Level);
				string[] skills = player.Pet.SkillEquip.Split('|');
				packet.WriteInt(skills.Length);
				foreach (string skill in skills)
				{
					packet.WriteInt(int.Parse(skill.Split(',')[1]));
					packet.WriteInt(int.Parse(skill.Split(',')[0]));
				}
			}
			SendTCP(packet);
			return packet;
		}

		public GSPacketIn SendRoomPlayerRemove(GamePlayer player)
		{
			GSPacketIn packet = new GSPacketIn(94, player.PlayerId);
			packet.WriteByte(5);
			packet.Parameter1 = player.PlayerId;
			packet.ClientID = player.PlayerId;
			packet.WriteInt(player.ZoneId);
			SendTCP(packet);
			return packet;
		}

		public GSPacketIn SendRoomUpdatePlayerStates(byte[] states)
		{
			GSPacketIn packet = new GSPacketIn(94);
			packet.WriteByte(15);
			for (int index = 0; index < states.Length; index++)
			{
				packet.WriteByte(states[index]);
			}
			SendTCP(packet);
			return packet;
		}

		public GSPacketIn SendRoomUpdatePlacesStates(int[] states)
		{
			GSPacketIn packet = new GSPacketIn(94);
			packet.WriteByte(10);
			for (int index = 0; index < states.Length; index++)
			{
				packet.WriteInt(states[index]);
			}
			SendTCP(packet);
			return packet;
		}

		public GSPacketIn SendRoomPlayerChangedTeam(GamePlayer player)
		{
			GSPacketIn packet = new GSPacketIn(94, player.PlayerId);
			packet.WriteByte(6);
			packet.WriteByte((byte)player.CurrentRoomTeam);
			packet.WriteByte((byte)player.CurrentRoomIndex);
			SendTCP(packet);
			return packet;
		}

		public GSPacketIn SendRoomCreate(BaseRoom room)
		{
			GSPacketIn packet = new GSPacketIn(94);
			packet.WriteByte(0);
			packet.WriteInt(room.RoomId);
			packet.WriteByte((byte)room.RoomType);
			packet.WriteByte((byte)room.HardLevel);
			packet.WriteByte(room.TimeMode);
			packet.WriteByte((byte)room.PlayerCount);
			packet.WriteByte((byte)room.viewerCnt);
			packet.WriteByte((byte)room.PlacesCount);
			packet.WriteBoolean((!string.IsNullOrEmpty(room.Password)) ? true : false);
			packet.WriteInt(room.MapId);
			packet.WriteBoolean(room.IsPlaying);
			packet.WriteString(room.Name);
			packet.WriteByte((byte)room.GameType);
			packet.WriteInt(room.LevelLimits);
			packet.WriteBoolean(room.isCrosszone);
			packet.WriteBoolean(room.isWithinLeageTime);
			packet.WriteBoolean(room.isOpenBoss);
			packet.WriteString(room.Pic);
			SendTCP(packet);
			return packet;
		}

		public GSPacketIn SendRoomLoginResult(bool result)
		{
			GSPacketIn packet = new GSPacketIn(94);
			packet.WriteByte(1);
			packet.WriteBoolean(result);
			SendTCP(packet);
			return packet;
		}

		public GSPacketIn SendRoomPairUpStart(BaseRoom room)
		{
			GSPacketIn packet = new GSPacketIn(94);
			packet.WriteByte(13);
			SendTCP(packet);
			return packet;
		}

		public GSPacketIn SendGameRoomInfo(GamePlayer player, BaseRoom game)
		{
			return new GSPacketIn(94, player.PlayerCharacter.ID);
		}

		public GSPacketIn SendRoomType(GamePlayer player, BaseRoom game)
		{
			GSPacketIn packet = new GSPacketIn(94);
			packet.WriteByte(12);
			packet.WriteByte((byte)game.GameType);
			SendTCP(packet);
			return packet;
		}

		public GSPacketIn SendRoomPairUpCancel(BaseRoom room)
		{
			GSPacketIn packet = new GSPacketIn(94);
			packet.WriteByte(11);
			SendTCP(packet);
			return packet;
		}

		public GSPacketIn SendRoomClear(GamePlayer player, BaseRoom game)
		{
			GSPacketIn packet = new GSPacketIn(96, player.PlayerCharacter.ID);
			packet.WriteInt(game.RoomId);
			SendTCP(packet);
			return packet;
		}

		public GSPacketIn SendEquipChange(GamePlayer player, int place, int goodsID, string style)
		{
			GSPacketIn packet = new GSPacketIn(66, player.PlayerCharacter.ID);
			packet.WriteByte((byte)place);
			packet.WriteInt(goodsID);
			packet.WriteString(style);
			SendTCP(packet);
			return packet;
		}

		public GSPacketIn SendRoomChange(BaseRoom room)
		{
			GSPacketIn packet = new GSPacketIn(94);
			packet.WriteByte(2);
			packet.WriteInt(room.MapId);
			packet.WriteByte((byte)room.RoomType);
			packet.WriteString(room.Password);
			packet.WriteString(room.Name);
			packet.WriteByte(room.TimeMode);
			packet.WriteByte((byte)room.HardLevel);
			packet.WriteInt(room.LevelLimits);
			packet.WriteBoolean(room.isCrosszone);
			SendTCP(packet);
			return packet;
		}

		public GSPacketIn SendGameRoomSetupChange(BaseRoom room)
		{
			GSPacketIn packet = new GSPacketIn(94);
			packet.WriteByte(2);
			packet.WriteBoolean(room.isOpenBoss);

			if (room.isOpenBoss)
				packet.WriteString(room.Pic);

			packet.WriteInt(room.MapId);
			packet.WriteByte((byte)room.RoomType);
			packet.WriteString((room.Password == null) ? "" : room.Password);
			packet.WriteString((room.Name == null) ? "Gunny1" : room.Name);
			packet.WriteByte(room.TimeMode);
			packet.WriteByte((byte)room.HardLevel);
			packet.WriteInt(room.LevelLimits);
			packet.WriteBoolean(room.isCrosszone);
			SendTCP(packet);
			return packet;
		}

		public GSPacketIn SendFusionPreview(GamePlayer player, Dictionary<int, double> previewItemList, bool isbind, int MinValid)
		{
			GSPacketIn packet = new GSPacketIn(76, player.PlayerCharacter.ID);
			packet.WriteInt(previewItemList.Count);
			foreach (KeyValuePair<int, double> previewItem in previewItemList)
			{
				packet.WriteInt(previewItem.Key);
				packet.WriteInt(MinValid);
				int int32 = Convert.ToInt32(previewItem.Value);
				packet.WriteInt((int32 > 100) ? 100 : ((int32 >= 0) ? int32 : 0));
			}
			packet.WriteBoolean(isbind);
			SendTCP(packet);
			return packet;
		}

		public GSPacketIn SendFusionResult(GamePlayer player, bool result)
		{
			GSPacketIn packet = new GSPacketIn(78, player.PlayerCharacter.ID);
			packet.WriteInt(2);
			packet.WriteBoolean(result);
			SendTCP(packet);
			return packet;
		}

		public GSPacketIn SendRefineryPreview(GamePlayer player, int templateid, bool isbind, ItemInfo item)
		{
			GSPacketIn packet = new GSPacketIn(111, player.PlayerCharacter.ID);
			packet.WriteInt(templateid);
			packet.WriteInt(item.ValidDate);
			packet.WriteBoolean(isbind);
			packet.WriteInt(item.AgilityCompose);
			packet.WriteInt(item.AttackCompose);
			packet.WriteInt(item.DefendCompose);
			packet.WriteInt(item.LuckCompose);
			SendTCP(packet);
			return packet;
		}

		public void SendUpdateInventorySlot(PlayerInventory bag, int[] updatedSlots)
		{
			if (m_gameClient.Player == null)
			{
				return;
			}
			GSPacketIn packet = new GSPacketIn(64, m_gameClient.Player.PlayerCharacter.ID, 10240);
			packet.WriteInt(bag.BagType);
			packet.WriteInt(updatedSlots.Length);
			foreach (int updatedSlot in updatedSlots)
			{
				packet.WriteInt(updatedSlot);
				ItemInfo itemAt = bag.GetItemAt(updatedSlot);
				if (itemAt == null)
				{
					packet.WriteBoolean(val: false);
					continue;
				}
				packet.WriteBoolean(val: true);
				packet.WriteInt(itemAt.UserID);
				packet.WriteInt(itemAt.ItemID);
				packet.WriteInt(itemAt.Count);
				packet.WriteInt(itemAt.Place);
				packet.WriteInt(itemAt.TemplateID);
				packet.WriteInt(itemAt.AttackCompose);
				packet.WriteInt(itemAt.DefendCompose);
				packet.WriteInt(itemAt.AgilityCompose);
				packet.WriteInt(itemAt.LuckCompose);
				packet.WriteInt(itemAt.StrengthenLevel);
				packet.WriteInt(itemAt.StrengthenExp);
				packet.WriteBoolean(itemAt.IsBinds);
				packet.WriteBoolean(itemAt.IsJudge);
				packet.WriteDateTime(itemAt.BeginDate);
				packet.WriteInt(itemAt.ValidDate);
				packet.WriteString((itemAt.Color == null) ? "" : itemAt.Color);
				packet.WriteString((itemAt.Skin == null) ? "" : itemAt.Skin);
				packet.WriteBoolean(itemAt.IsUsed);
				packet.WriteInt(itemAt.Hole1);
				packet.WriteInt(itemAt.Hole2);
				packet.WriteInt(itemAt.Hole3);
				packet.WriteInt(itemAt.Hole4);
				packet.WriteInt(itemAt.Hole5);
				packet.WriteInt(itemAt.Hole6);
				packet.WriteString(itemAt.Pic);
				packet.WriteInt(itemAt.RefineryLevel);
				packet.WriteDateTime(DateTime.Now.AddDays(5.0));
				packet.WriteInt(itemAt.StrengthenTimes);
				packet.WriteByte((byte)itemAt.Hole5Level);
				packet.WriteInt(itemAt.Hole5Exp);
				packet.WriteByte((byte)itemAt.Hole6Level);
				packet.WriteInt(itemAt.Hole6Exp);
				packet.WriteInt(itemAt.curExp);
				packet.WriteBoolean(itemAt.cellLocked);
				packet.WriteBoolean(itemAt.isGold);
				if (itemAt.isGold)
				{
					packet.WriteInt(itemAt.goldValidDate);
					packet.WriteDateTime(itemAt.goldBeginTime);
				}
				packet.WriteString(itemAt.latentEnergyCurStr);
				packet.WriteString(itemAt.latentEnergyNewStr);
				packet.WriteDateTime(itemAt.latentEnergyEndTime);
				//Need UnComment
			}
			SendTCP(packet);
		}

		public void SendUpdateCardData(CardInventory bag, int[] updatedSlots)
		{
			if (m_gameClient.Player == null)
			{
				return;
			}
			GSPacketIn packet = new GSPacketIn(216, m_gameClient.Player.PlayerCharacter.ID);
			packet.WriteInt(m_gameClient.Player.PlayerCharacter.ID);
			packet.WriteInt(updatedSlots.Length);
			foreach (int updatedSlot in updatedSlots)
			{
				packet.WriteInt(updatedSlot);
				UsersCardInfo itemAt = bag.GetItemAt(updatedSlot);
				if (itemAt != null && itemAt.TemplateID != 0)
				{
					packet.WriteBoolean(val: true);
					packet.WriteInt(itemAt.CardID);
					packet.WriteInt(itemAt.UserID);
					packet.WriteInt(itemAt.Count);
					packet.WriteInt(itemAt.Place);
					packet.WriteInt(itemAt.TemplateID);
					packet.WriteInt(itemAt.TotalAttack);
					packet.WriteInt(itemAt.TotalDefence);
					packet.WriteInt(itemAt.TotalAgility);
					packet.WriteInt(itemAt.TotalLuck);
					packet.WriteInt(itemAt.Damage);
					packet.WriteInt(itemAt.Guard);
					packet.WriteInt(itemAt.Level);
					packet.WriteInt(itemAt.CardGP);
					packet.WriteBoolean(itemAt.isFirstGet);
				}
				else
				{
					packet.WriteBoolean(val: false);
				}
			}
			SendTCP(packet);
		}

		public void SendUpdateCardData(PlayerInfo player, List<UsersCardInfo> userCard)
		{
			if (m_gameClient.Player == null)
			{
				return;
			}
			GSPacketIn packet = new GSPacketIn(216, player.ID);
			packet.WriteInt(player.ID);
			packet.WriteInt(userCard.Count);
			foreach (UsersCardInfo userCardInfo in userCard)
			{
				packet.WriteInt(userCardInfo.Place);
				packet.WriteBoolean(val: true);
				packet.WriteInt(userCardInfo.CardID);
				packet.WriteInt(userCardInfo.UserID);
				packet.WriteInt(userCardInfo.Count);
				packet.WriteInt(userCardInfo.Place);
				packet.WriteInt(userCardInfo.TemplateID);
				packet.WriteInt(userCardInfo.TotalAttack);
				packet.WriteInt(userCardInfo.TotalDefence);
				packet.WriteInt(userCardInfo.TotalAgility);
				packet.WriteInt(userCardInfo.TotalLuck);
				packet.WriteInt(userCardInfo.Damage);
				packet.WriteInt(userCardInfo.Guard);
				packet.WriteInt(userCardInfo.Level);
				packet.WriteInt(userCardInfo.CardGP);
				packet.WriteBoolean(userCardInfo.isFirstGet);
			}
			SendTCP(packet);
		}

		public GSPacketIn SendUpdateQuests(GamePlayer player, byte[] states, BaseQuest[] infos)
		{
			if (player == null || states == null || infos == null)
			{
				return null;
			}
			GSPacketIn packet = new GSPacketIn(178, player.PlayerCharacter.ID);
			packet.WriteInt(infos.Length);
			foreach (BaseQuest info in infos)
			{
				packet.WriteInt(info.Data.QuestID);
				packet.WriteBoolean(info.Data.IsComplete);
				packet.WriteInt(info.Data.Condition1);
				packet.WriteInt(info.Data.Condition2);
				packet.WriteInt(info.Data.Condition3);
				packet.WriteInt(info.Data.Condition4);
				packet.WriteDateTime(info.Data.CompletedDate.Date);
				packet.WriteInt(info.Data.RepeatFinish);
				packet.WriteInt(info.Data.RandDobule);
				packet.WriteBoolean(info.Data.IsExist);
			}
			packet.Write(states);
			SendTCP(packet);
			return packet;
		}

		public GSPacketIn SendUpdateBuffer(GamePlayer player, AbstractBuffer[] infos)
		{
			GSPacketIn packet = new GSPacketIn(185, player.PlayerId);
			packet.WriteInt(infos.Length);
			foreach (AbstractBuffer info in infos)
			{
				packet.WriteInt(info.Info.Type);
				packet.WriteBoolean(info.Info.IsExist);
				packet.WriteDateTime(info.Info.BeginDate);
				if (info.IsPayBuff())
				{
					packet.WriteInt(info.Info.ValidDate / 60 / 24);
				}
				else
				{
					packet.WriteInt(info.Info.ValidDate);
				}
				packet.WriteInt(info.Info.Value);
				packet.WriteInt(info.Info.ValidCount);
				packet.WriteInt(info.Info.TemplateID);
			}
			SendTCP(packet);
			return packet;
		}

		public GSPacketIn SendBufferList(GamePlayer player, List<AbstractBuffer> infos)
		{
			GSPacketIn packet = new GSPacketIn(186, player.PlayerId);
			packet.WriteInt(infos.Count);
			foreach (AbstractBuffer info3 in infos)
			{
				BufferInfo info2 = info3.Info;
				packet.WriteInt(info2.Type);
				packet.WriteBoolean(info2.IsExist);
				packet.WriteDateTime(info2.BeginDate);
				if (info3.IsPayBuff())
				{
					packet.WriteInt(info2.ValidDate / 60 / 24);
				}
				else
				{
					packet.WriteInt(info2.ValidDate);
				}
				packet.WriteInt(info2.Value);
				packet.WriteInt(info2.ValidCount);
				packet.WriteInt(info2.TemplateID);
			}
			SendTCP(packet);
			return packet;
		}

		public GSPacketIn SendMailResponse(int playerID, eMailRespose type)
		{
			GSPacketIn packet = new GSPacketIn(117);
			packet.WriteInt(playerID);
			packet.WriteInt((int)type);
			GameServer.Instance.LoginServer.SendPacket(packet);
			return packet;
		}

		public GSPacketIn SendConsortiaLevelUp(byte type, byte level, bool result, string msg, int playerid)
		{
			GSPacketIn packet = new GSPacketIn(159, playerid);
			packet.WriteByte(type);
			packet.WriteByte(level);
			packet.WriteBoolean(result);
			packet.WriteString(msg);
			SendTCP(packet);
			return packet;
		}

		public GSPacketIn SendAuctionRefresh(AuctionInfo info, int auctionID, bool isExist, ItemInfo item)
		{
			GSPacketIn packet = new GSPacketIn(195);
			packet.WriteInt(auctionID);
			packet.WriteBoolean(isExist);
			if (isExist)
			{
				packet.WriteInt(info.AuctioneerID);
				packet.WriteString(info.AuctioneerName);
				packet.WriteDateTime(info.BeginDate);
				packet.WriteInt(info.BuyerID);
				packet.WriteString(info.BuyerName);
				packet.WriteInt(info.ItemID);
				packet.WriteInt(info.Mouthful);
				packet.WriteInt(info.PayType);
				packet.WriteInt(info.Price);
				packet.WriteInt(info.Rise);
				packet.WriteInt(info.ValidDate);
				packet.WriteBoolean(item != null);
				if (item != null)
				{
					packet.WriteInt(item.Count);
					packet.WriteInt(item.TemplateID);
					packet.WriteInt(item.AttackCompose);
					packet.WriteInt(item.DefendCompose);
					packet.WriteInt(item.AgilityCompose);
					packet.WriteInt(item.LuckCompose);
					packet.WriteInt(item.StrengthenLevel);
					packet.WriteBoolean(item.IsBinds);
					packet.WriteBoolean(item.IsJudge);
					packet.WriteDateTime(item.BeginDate);
					packet.WriteInt(item.ValidDate);
					packet.WriteString(item.Color);
					packet.WriteString(item.Skin);
					packet.WriteBoolean(item.IsUsed);
					packet.WriteInt(item.Hole1);
					packet.WriteInt(item.Hole2);
					packet.WriteInt(item.Hole3);
					packet.WriteInt(item.Hole4);
					packet.WriteInt(item.Hole5);
					packet.WriteInt(item.Hole6);
					packet.WriteString(item.Pic);
					packet.WriteInt(item.RefineryLevel);
					packet.WriteDateTime(DateTime.Now);
					packet.WriteByte((byte)item.Hole5Level);
					packet.WriteInt(item.Hole5Exp);
					packet.WriteByte((byte)item.Hole6Level);
					packet.WriteInt(item.Hole6Exp);
				}
			}
			packet.Compress();
			SendTCP(packet);
			return packet;
		}

		public GSPacketIn SendAASState(bool result)
		{
			GSPacketIn packet = new GSPacketIn(224);
			packet.WriteBoolean(result);
			SendTCP(packet);
			return packet;
		}

		public GSPacketIn SendIDNumberCheck(bool result)
		{
			GSPacketIn packet = new GSPacketIn(226);
			packet.WriteBoolean(result);
			SendTCP(packet);
			return packet;
		}

		public GSPacketIn SendAASInfoSet(bool result)
		{
			GSPacketIn packet = new GSPacketIn(224);
			packet.WriteBoolean(result);
			SendTCP(packet);
			return packet;
		}

		public GSPacketIn SendAASControl(bool result, bool bool_0, bool IsMinor)
		{
			GSPacketIn packet = new GSPacketIn(227);
			packet.WriteBoolean(val: true);
			packet.WriteInt(1);
			packet.WriteBoolean(val: true);
			packet.WriteBoolean(IsMinor);
			SendTCP(packet);
			return packet;
		}

		public GSPacketIn SendMarryRoomInfo(GamePlayer player, MarryRoom room)
		{
			GSPacketIn packet = new GSPacketIn(241, player.PlayerCharacter.ID);
			bool val = room != null;
			packet.WriteBoolean(val);
			if (val)
			{
				packet.WriteInt(room.Info.ID);
				packet.WriteBoolean(room.Info.IsHymeneal);
				packet.WriteString(room.Info.Name);
				packet.WriteBoolean(!(room.Info.Pwd == ""));
				packet.WriteInt(room.Info.MapIndex);
				packet.WriteInt(room.Info.AvailTime);
				packet.WriteInt(room.Count);
				packet.WriteInt(room.Info.PlayerID);
				packet.WriteString(room.Info.PlayerName);
				packet.WriteInt(room.Info.GroomID);
				packet.WriteString(room.Info.GroomName);
				packet.WriteInt(room.Info.BrideID);
				packet.WriteString(room.Info.BrideName);
				packet.WriteDateTime(room.Info.BeginTime);
				packet.WriteByte((byte)room.RoomState);
				packet.WriteString(room.Info.RoomIntroduction);
			}
			SendTCP(packet);
			return packet;
		}

		public GSPacketIn SendMarryRoomLogin(GamePlayer player, bool result)
		{
			GSPacketIn packet = new GSPacketIn(242, player.PlayerCharacter.ID);
			packet.WriteBoolean(result);
			if (result)
			{
				packet.WriteInt(player.CurrentMarryRoom.Info.ID);
				packet.WriteString(player.CurrentMarryRoom.Info.Name);
				packet.WriteInt(player.CurrentMarryRoom.Info.MapIndex);
				packet.WriteInt(player.CurrentMarryRoom.Info.AvailTime);
				packet.WriteInt(player.CurrentMarryRoom.Count);
				packet.WriteInt(player.CurrentMarryRoom.Info.PlayerID);
				packet.WriteString(player.CurrentMarryRoom.Info.PlayerName);
				packet.WriteInt(player.CurrentMarryRoom.Info.GroomID);
				packet.WriteString(player.CurrentMarryRoom.Info.GroomName);
				packet.WriteInt(player.CurrentMarryRoom.Info.BrideID);
				packet.WriteString(player.CurrentMarryRoom.Info.BrideName);
				packet.WriteDateTime(player.CurrentMarryRoom.Info.BeginTime);
				packet.WriteBoolean(player.CurrentMarryRoom.Info.IsHymeneal);
				packet.WriteByte((byte)player.CurrentMarryRoom.RoomState);
				packet.WriteString(player.CurrentMarryRoom.Info.RoomIntroduction);
				packet.WriteBoolean(player.CurrentMarryRoom.Info.GuestInvite);
				packet.WriteInt(player.MarryMap);
				packet.WriteBoolean(player.CurrentMarryRoom.Info.IsGunsaluteUsed);
			}
			SendTCP(packet);
			return packet;
		}

		public GSPacketIn SendPlayerEnterMarryRoom(GamePlayer player)
		{
			GSPacketIn packet = new GSPacketIn(243, player.PlayerCharacter.ID);
			packet.WriteInt(player.PlayerCharacter.Grade);
			packet.WriteInt(player.PlayerCharacter.Hide);
			packet.WriteInt(player.PlayerCharacter.Repute);
			packet.WriteInt(player.PlayerCharacter.ID);
			packet.WriteString(player.PlayerCharacter.NickName);
			packet.WriteByte(player.PlayerCharacter.typeVIP);
			packet.WriteInt(player.PlayerCharacter.VIPLevel);
			packet.WriteBoolean(player.PlayerCharacter.Sex);
			packet.WriteString(player.PlayerCharacter.Style);
			packet.WriteString(player.PlayerCharacter.Colors);
			packet.WriteString(player.PlayerCharacter.Skin);
			packet.WriteInt(player.X);
			packet.WriteInt(player.Y);
			packet.WriteInt(player.PlayerCharacter.FightPower);
			packet.WriteInt(player.PlayerCharacter.Win);
			packet.WriteInt(player.PlayerCharacter.Total);
			packet.WriteInt(player.PlayerCharacter.Offer);
			SendTCP(packet);
			return packet;
		}

		public GSPacketIn SendMarryInfoRefresh(MarryInfo info, int ID, bool isExist)
		{
			GSPacketIn packet = new GSPacketIn(239);
			packet.WriteInt(ID);
			packet.WriteBoolean(isExist);
			if (isExist)
			{
				packet.WriteInt(info.UserID);
				packet.WriteBoolean(info.IsPublishEquip);
				packet.WriteString(info.Introduction);
			}
			SendTCP(packet);
			return packet;
		}

		public GSPacketIn SendPlayerMarryStatus(GamePlayer player, int userID, bool isMarried)
		{
			GSPacketIn packet = new GSPacketIn(246, player.PlayerCharacter.ID);
			packet.WriteInt(userID);
			packet.WriteBoolean(isMarried);
			SendTCP(packet);
			return packet;
		}

		public GSPacketIn SendPlayerMarryApply(GamePlayer player, int userID, string userName, string loveProclamation, int id)
		{
			GSPacketIn packet = new GSPacketIn(247, player.PlayerCharacter.ID);
			packet.WriteInt(userID);
			packet.WriteString(userName);
			packet.WriteString(loveProclamation);
			packet.WriteInt(id);
			SendTCP(packet);
			return packet;
		}

		public GSPacketIn SendPlayerDivorceApply(GamePlayer player, bool result, bool isProposer)
		{
			GSPacketIn packet = new GSPacketIn(248, player.PlayerCharacter.ID);
			packet.WriteBoolean(result);
			packet.WriteBoolean(isProposer);
			SendTCP(packet);
			return packet;
		}

		public GSPacketIn SendMarryApplyReply(GamePlayer player, int UserID, string UserName, bool result, bool isApplicant, int id)
		{
			GSPacketIn packet = new GSPacketIn(250, player.PlayerCharacter.ID);
			packet.WriteInt(UserID);
			packet.WriteBoolean(result);
			packet.WriteString(UserName);
			packet.WriteBoolean(isApplicant);
			packet.WriteInt(id);
			SendTCP(packet);
			return packet;
		}

		public GSPacketIn SendBigSpeakerMsg(GamePlayer player, string msg)
		{
			GSPacketIn packet = new GSPacketIn(72, player.PlayerCharacter.ID);
			packet.WriteInt(player.PlayerCharacter.ID);
			packet.WriteString(player.PlayerCharacter.NickName);
			packet.WriteString(msg);
			GameServer.Instance.LoginServer.SendPacket(packet);
			GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
			for (int i = 0; i < allPlayers.Length; i++)
			{
				allPlayers[i].Out.SendTCP(packet);
			}
			return packet;
		}

		public GSPacketIn SendPlayerLeaveMarryRoom(GamePlayer player)
		{
			GSPacketIn packet = new GSPacketIn(244, player.PlayerCharacter.ID);
			SendTCP(packet);
			return packet;
		}

		public GSPacketIn SendMarryRoomInfoToPlayer(GamePlayer player, bool state, MarryRoomInfo info)
		{
			GSPacketIn packet = new GSPacketIn(252, player.PlayerCharacter.ID);
			packet.WriteBoolean(state);
			if (state)
			{
				packet.WriteInt(info.ID);
				packet.WriteString(info.Name);
				packet.WriteInt(info.MapIndex);
				packet.WriteInt(info.AvailTime);
				packet.WriteInt(info.PlayerID);
				packet.WriteInt(info.GroomID);
				packet.WriteInt(info.BrideID);
				packet.WriteDateTime(info.BeginTime);
				packet.WriteBoolean(info.IsGunsaluteUsed);
			}
			SendTCP(packet);
			return packet;
		}

		public GSPacketIn SendMarryInfo(GamePlayer player, MarryInfo info)
		{
			GSPacketIn packet = new GSPacketIn(235, player.PlayerCharacter.ID);
			packet.WriteString(info.Introduction);
			packet.WriteBoolean(info.IsPublishEquip);
			SendTCP(packet);
			return packet;
		}

		public GSPacketIn SendContinuation(GamePlayer player, MarryRoomInfo info)
		{
			GSPacketIn packet = new GSPacketIn(249, player.PlayerCharacter.ID);
			packet.WriteByte(3);
			packet.WriteInt(info.AvailTime);
			SendTCP(packet);
			return packet;
		}

		public GSPacketIn SendMarryProp(GamePlayer player, MarryProp info)
		{
			GSPacketIn packet = new GSPacketIn(234, player.PlayerCharacter.ID);
			packet.WriteBoolean(info.IsMarried);
			packet.WriteInt(info.SpouseID);
			packet.WriteString(info.SpouseName);
			packet.WriteBoolean(info.IsCreatedMarryRoom);
			packet.WriteInt(info.SelfMarryRoomID);
			packet.WriteBoolean(info.IsGotRing);
			SendTCP(packet);
			return packet;
		}

		public void SendWeaklessGuildProgress(PlayerInfo player)
		{
			GSPacketIn packet = new GSPacketIn(15, player.ID);
			packet.WriteInt(player.weaklessGuildProgress.Length);
			for (int index = 0; index < player.weaklessGuildProgress.Length; index++)
			{
				packet.WriteByte(player.weaklessGuildProgress[index]);
			}
			SendTCP(packet);
		}

		public void SendUserLuckyNum()
		{
			GSPacketIn packet = new GSPacketIn(161);
			packet.WriteInt(1);
			packet.WriteString("");
			SendTCP(packet);
		}

		public GSPacketIn SendUserRanks(int Id, List<UserRankInfo> ranks)
		{
			GSPacketIn packet = new GSPacketIn(34, Id);
			packet.WriteInt(ranks.Count);
			foreach (UserRankInfo rank in ranks)
			{
				packet.WriteInt(rank.NewTitleID);
				packet.WriteString(rank.Name);
				packet.WriteDateTime(rank.BeginDate);
				if (rank.Validate == 0)
					packet.WriteDateTime(DateTime.Now.AddYears(1));
				else
					packet.WriteDateTime(rank.EndDate);
			}
			SendTCP(packet);
			return packet;
		}

		public GSPacketIn SendOpenVIP(GamePlayer Player)
		{
			GSPacketIn packet = new GSPacketIn(92, Player.PlayerCharacter.ID);
			packet.WriteByte(1);
			packet.WriteInt(Player.PlayerCharacter.VIPLevel);
			packet.WriteInt(Player.PlayerCharacter.VIPExp);
			packet.WriteDateTime(Player.PlayerCharacter.VIPExpireDay);
			packet.WriteDateTime(Player.PlayerCharacter.LastDate);
			packet.WriteInt(Player.PlayerCharacter.VIPNextLevelDaysNeeded);
			packet.WriteBoolean(Player.PlayerCharacter.CanTakeVipReward);
			SendTCP(packet);
			return packet;
		}

		public GSPacketIn SendEnterHotSpringRoom(GamePlayer player)
		{
			if (player.CurrentHotSpringRoom == null)
			{
				return null;
			}
			GSPacketIn packet = new GSPacketIn(202, player.PlayerCharacter.ID);
			packet.WriteInt(player.CurrentHotSpringRoom.Info.roomID);
			packet.WriteInt(player.CurrentHotSpringRoom.Info.roomNumber);
			packet.WriteString(player.CurrentHotSpringRoom.Info.roomName);
			packet.WriteString(player.CurrentHotSpringRoom.Info.roomPassword);
			packet.WriteInt(player.CurrentHotSpringRoom.Info.effectiveTime);
			packet.WriteInt(player.CurrentHotSpringRoom.Count);
			packet.WriteInt(player.CurrentHotSpringRoom.Info.playerID);
			packet.WriteString(player.CurrentHotSpringRoom.Info.playerName);
			packet.WriteDateTime(player.CurrentHotSpringRoom.Info.startTime);
			packet.WriteString(player.CurrentHotSpringRoom.Info.roomIntroduction);
			packet.WriteInt(player.CurrentHotSpringRoom.Info.roomType);
			packet.WriteInt(player.CurrentHotSpringRoom.Info.maxCount);
			packet.WriteDateTime(player.Extra.Info.LastTimeHotSpring);
			packet.WriteInt(player.Extra.Info.MinHotSpring);
			SendTCP(packet);
			return packet;
		}

		public GSPacketIn SendHotSpringUpdateTime(GamePlayer player, int expAdd)
		{
			if (player.CurrentHotSpringRoom == null)
			{
				return null;
			}
			GSPacketIn packet = new GSPacketIn(191, player.PlayerCharacter.ID);
			packet.WriteByte(7);
			packet.WriteInt(player.Extra.Info.MinHotSpring);
			packet.WriteInt(expAdd);
			SendTCP(packet);
			return packet;
		}

		public GSPacketIn SendGetUserGift(PlayerInfo player, UserGiftInfo[] allGifts)
		{
			GSPacketIn packet = new GSPacketIn(218);
			packet.WriteInt(player.ID);
			packet.WriteInt(player.charmGP);
			packet.WriteInt(allGifts.Length);
			foreach (UserGiftInfo allGift in allGifts)
			{
				packet.WriteInt(allGift.TemplateID);
				packet.WriteInt(allGift.Count);
			}
			SendTCP(packet);
			return packet;
		}

		public GSPacketIn SendEatPetsInfo(EatPetsInfo info)
		{
			if (info == null)
				return null;
			GSPacketIn pkg = new GSPacketIn((byte)ePackageType.PET, m_gameClient.Player.PlayerId);
			pkg.WriteByte((byte)PetPackageType.EAT_PETS);
			pkg.WriteInt(info.weaponExp); // _loc_2.add("weaponExp", event.pkg.readInt());
			pkg.WriteInt(info.weaponLevel); //_loc_2.add("weaponLevel", event.pkg.readInt());
			pkg.WriteInt(info.clothesExp); //_loc_2.add("clothesExp", event.pkg.readInt());
			pkg.WriteInt(info.clothesLevel); //_loc_2.add("clothesLevel", event.pkg.readInt());
			pkg.WriteInt(info.hatExp); //_loc_2.add("hatExp", event.pkg.readInt());
			pkg.WriteInt(info.hatLevel); //_loc_2.add("hatLevel", event.pkg.readInt());
			SendTCP(pkg);
			return pkg;
		}
		public GSPacketIn SendRefreshPet(GamePlayer player, UsersPetInfo[] pets, ItemInfo[] items, bool refreshBtn)
        {
			if (pets.Length == 0)
				return null;
			int MaxActiveSkillCount = 10;
			int MaxStaticSkillCount = 10;
			int MaxSkillCount = 100;

			GSPacketIn pkg = new GSPacketIn((byte)ePackageType.PET, player.PlayerCharacter.ID);
			pkg.WriteByte((byte)PetPackageType.REFRESH_PET);
			pkg.WriteBoolean(refreshBtn);
			pkg.WriteInt(pets.Length);
			for (int i = 0; i < pets.Length; i++)
            {
				UsersPetInfo pet = pets[i];
				pkg.WriteInt(pet.Place); //_loc_11.Place = _loc_7;
				pkg.WriteInt(pet.TemplateID); //_loc_11.TemplateID = _loc_2.readInt();
				pkg.WriteString(pet.Name); //_loc_11.Name = _loc_2.readUTF();
				pkg.WriteInt(pet.Attack); //_loc_11.Attack = _loc_2.readInt();
				pkg.WriteInt(pet.Defence); //_loc_11.Defence = _loc_2.readInt();
				pkg.WriteInt(pet.Luck); //_loc_11.Luck = _loc_2.readInt();
				pkg.WriteInt(pet.Agility); //_loc_11.Agility = _loc_2.readInt();
				pkg.WriteInt(pet.Blood); //_loc_11.Blood = _loc_2.readInt();
				pkg.WriteInt(pet.Damage); //_loc_11.Damage = _loc_2.readInt();
				pkg.WriteInt(pet.Guard); //_loc_11.Guard = _loc_2.readInt();
				pkg.WriteInt(pet.AttackGrow); //_loc_11.AttackGrow = _loc_2.readInt();
				pkg.WriteInt(pet.DefenceGrow); //_loc_11.DefenceGrow = _loc_2.readInt();
				pkg.WriteInt(pet.LuckGrow); //_loc_11.LuckGrow = _loc_2.readInt();
				pkg.WriteInt(pet.AgilityGrow); //_loc_11.AgilityGrow = _loc_2.readInt();
				pkg.WriteInt(pet.BloodGrow); //_loc_11.BloodGrow = _loc_2.readInt();
				pkg.WriteInt(pet.DamageGrow); //_loc_11.DamageGrow = _loc_2.readInt();
				pkg.WriteInt(pet.GuardGrow); //_loc_11.GuardGrow = _loc_2.readInt();
				pkg.WriteInt(pet.Level); //_loc_11.Level = _loc_2.readInt();
				pkg.WriteInt(pet.GP); //_loc_11.GP = _loc_2.readInt();
				pkg.WriteInt(pet.MaxGP); //_loc_11.MaxGP = _loc_2.readInt();
				pkg.WriteInt(pet.Hunger); //_loc_11.Hunger = _loc_2.readInt();
				pkg.WriteInt(pet.MP); //_loc_11.MP = _loc_2.readInt();
				string[] skills = pet.Skill.Split('|');
				pkg.WriteInt(skills.Length); // _loc_2.readInt();==>all Ky nang thu cung
				foreach (string skill in skills)
				{
					pkg.WriteInt(int.Parse(skill.Split(',')[0])); //_loc_2.readInt();killID
					pkg.WriteInt(int.Parse(skill.Split(',')[1])); //_loc_17 = _loc_2.readInt();Place
				}

				pkg.WriteInt(MaxActiveSkillCount); // _loc_7.MaxActiveSkillCount = _loc_2.readInt();
				pkg.WriteInt(MaxStaticSkillCount); //_loc_7.MaxStaticSkillCount = _loc_2.readInt();
				pkg.WriteInt(MaxSkillCount); //MaxSkillCount = _loc_2.readInt();
			}

			if (!player.PlayerCharacter.IsFistGetPet)
			{
				pkg.WriteInt(0);
			}

			SendTCP(pkg);
			return pkg;
		}

		public void SendLeagueNotice(int id)
		{
			GSPacketIn pkg = new GSPacketIn((byte)ePackageType.LEAGUE_START_NOTICE, id);
			SendTCP(pkg);
		}

		public GSPacketIn SendEnterFarm(PlayerInfo Player, UserFarmInfo farm, UserFieldInfo[] fields)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(81, Player.ID);
			gSPacketIn.WriteByte(1);
			gSPacketIn.WriteInt(farm.FarmID);
			gSPacketIn.WriteBoolean(farm.isFarmHelper);
			gSPacketIn.WriteInt(farm.isAutoId);
			gSPacketIn.WriteDateTime(farm.AutoPayTime);
			gSPacketIn.WriteInt(farm.AutoValidDate);
			gSPacketIn.WriteInt(farm.GainFieldId);
			gSPacketIn.WriteInt(farm.KillCropId);
			gSPacketIn.WriteInt(fields.Length);
			foreach (UserFieldInfo userFieldInfo in fields)
			{
				gSPacketIn.WriteInt(userFieldInfo.FieldID);
				gSPacketIn.WriteInt(userFieldInfo.SeedID);
				gSPacketIn.WriteDateTime(userFieldInfo.PayTime);
				gSPacketIn.WriteDateTime(userFieldInfo.PlantTime);
				gSPacketIn.WriteInt(userFieldInfo.GainCount);
				gSPacketIn.WriteInt(userFieldInfo.FieldValidDate);
				gSPacketIn.WriteInt(userFieldInfo.AccelerateTime);
			}
			if (farm.FarmID == Player.ID)
			{
				gSPacketIn.WriteInt(Player.TotalMoneyFastForWard);
				gSPacketIn.WriteString(farm.PayFieldMoney);
				gSPacketIn.WriteString(farm.PayAutoMoney);
				gSPacketIn.WriteDateTime(farm.AutoPayTime);
				gSPacketIn.WriteInt(farm.AutoValidDate);
				gSPacketIn.WriteInt(Player.VIPLevel);
				gSPacketIn.WriteInt(farm.buyExpRemainNum);
			}
			else
			{
				gSPacketIn.WriteBoolean(farm.isArrange);
			}
			SendTCP(gSPacketIn);
			return gSPacketIn;
		}

        public GSPacketIn SendSeeding(PlayerInfo Player, UserFieldInfo field)
		{
			GSPacketIn packet = new GSPacketIn((byte)ePackageType.FARM, Player.ID);
			packet.WriteByte((byte)FarmPackageType.GROW_FIELD);
			packet.WriteInt(field.FieldID); //_loc_3:* = fieldId.readInt();
			packet.WriteInt(field.SeedID); // _loc_4:* = seedID.readInt();
			packet.WriteDateTime(field.PlantTime); // _loc_5:* = plantTime.readDate();
			packet.WriteDateTime(field.PayTime); // _loc_6:* = _loc_2.readDate();
			packet.WriteInt(field.GainCount); // _loc_7:* = gainCount.readInt();
			packet.WriteInt(field.FieldValidDate); // _loc_8:* = _loc_2.readInt();
			SendTCP(packet);
			return packet;
		}

		public GSPacketIn SenddoMature(PlayerFarm farm)
		{
			GSPacketIn packet = new GSPacketIn((byte)ePackageType.FARM, farm.Player.PlayerCharacter.ID);
			packet.WriteByte((byte)FarmPackageType.ACCELERATE_FIELD);
			packet.WriteInt(farm.CurrentFields.Length);
			foreach (UserFieldInfo field in farm.CurrentFields)
			{
				if (field != null)
				{
					packet.WriteBoolean(true);
					packet.WriteInt(field.FieldID);
					packet.WriteInt(field.GainCount);
					packet.WriteInt(field.AccelerateTime);
				}
				else
				{
					packet.WriteBoolean(false);
				}
			}
			SendTCP(packet);
			return packet;
		}

		public GSPacketIn SendtoGather(PlayerInfo Player, UserFieldInfo field)
		{
			GSPacketIn packet = new GSPacketIn((byte)ePackageType.FARM, Player.ID);
			packet.WriteByte((byte)FarmPackageType.GAIN_FIELD);
			packet.WriteBoolean(true); //var _loc_3:* = event.pkg.readBoolean();
			packet.WriteInt(field.FieldID); //model.gainFieldId = event.pkg.readInt();
			packet.WriteInt(field.SeedID); //_loc_2.seedID = event.pkg.readInt();
			packet.WriteDateTime(field.PlantTime); //_loc_2.plantTime = event.pkg.readDate();
			packet.WriteInt(field.GainCount); //_loc_2.gainCount = event.pkg.readInt();
			packet.WriteInt(field.AccelerateTime); //_loc_2.AccelerateTime = event.pkg.readInt();
			SendTCP(packet);
			return packet;
		}

		public GSPacketIn SendCompose(GamePlayer Player)
		{
			GSPacketIn packet = new GSPacketIn((byte)ePackageType.FARM, Player.PlayerCharacter.ID);
			packet.WriteByte((byte)FarmPackageType.COMPOSE_FOOD);
			SendTCP(packet);
			return packet;
		}

		public GSPacketIn SendPayFields(GamePlayer Player, List<int> fieldIds)
		{
			GSPacketIn packet = new GSPacketIn((byte)ePackageType.FARM, Player.PlayerCharacter.ID);
			packet.WriteByte((byte)FarmPackageType.PAY_FIELD);
			packet.WriteInt(Player.PlayerCharacter.ID);
			packet.WriteInt(fieldIds.Count); // _loc_9:* = _loc_2.readInt();//field count
			foreach (int id in fieldIds)
			{
				UserFieldInfo field = Player.Farm.GetFieldAt(id);
				packet.WriteInt(field.FieldID); //_loc_11 = _loc_2.readInt();//fieldID
				packet.WriteInt(field.SeedID); //_loc_12 = _loc_2.readInt();//seedID :332112
				packet.WriteDateTime(field.PayTime); //_loc_13 = _loc_2.readDate();//payTime
				packet.WriteDateTime(field.PlantTime); //_loc_14 = _loc_2.readDate();//plantTime
				packet.WriteInt(field.GainCount); //_loc_15 = _loc_2.readInt();//gainCount
				packet.WriteInt(field.FieldValidDate); //_loc_16 = _loc_2.readInt();//fieldValidDate
				packet.WriteInt(field.AccelerateTime); //_loc_17 = _loc_2.readInt();//AccelerateTime
			}

			SendTCP(packet);
			return packet;
		}

		public GSPacketIn SendKillCropField(PlayerInfo Player, UserFieldInfo field)
		{
			GSPacketIn packet = new GSPacketIn((byte)ePackageType.FARM, Player.ID);
			packet.WriteByte((byte)FarmPackageType.KILLCROP_FIELD);
			packet.WriteBoolean(true);
			packet.WriteInt(field.FieldID); //_loc_3:* = fieldId.readInt();
			packet.WriteInt(field.SeedID); // _loc_4:* = seedID.readInt();
			packet.WriteInt(field.AccelerateTime); // _loc_8:* = _loc_2.readInt();
			SendTCP(packet);
			return packet;
		}

		public GSPacketIn SendHelperSwitchField(PlayerInfo Player, UserFarmInfo farm)
		{
			GSPacketIn packet = new GSPacketIn((byte)ePackageType.FARM, Player.ID);
			packet.WriteByte((byte)FarmPackageType.HELPER_SWITCH_FIELD);
			packet.WriteBoolean(farm.isFarmHelper); // _loc_3:* = _loc_2.readBoolean(); isFarmHelper/isAutomatic
			packet.WriteInt(farm.isAutoId); // _loc_4:* = _loc_2.readInt(); isAutoId/autoSeedID
			packet.WriteDateTime(farm.AutoPayTime); // _loc_5:* = _loc_2.readDate();//startdate
			packet.WriteInt(farm.AutoValidDate); // _loc_6:* = _loc_2.readInt();//_autoTime
			packet.WriteInt(farm.GainFieldId); // _loc_7:* = _loc_2.readInt();//_needSeed
			packet.WriteInt(farm.KillCropId); // _loc_8:* = _loc_2.readInt();//_getSeed
			SendTCP(packet);
			return packet;
		}

		public void SendLittleGameActived()
		{
			GSPacketIn pkg = new GSPacketIn((byte)ePackageType.LITTLEGAME_ACTIVED);
			pkg.WriteBoolean(LittleGameWorldMgr.IsOpen);//(true);//(LittleGameWorldMgr.IsOpen);
			m_gameClient.SendTCP(pkg);
		}

		public GSPacketIn SendChickenBoxOpen(int ID, int flushPrice, int[] openCardPrice, int[] eagleEyePrice)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(87, ID);
			gSPacketIn.WriteInt(1);
			gSPacketIn.WriteInt(openCardPrice.Length);
			for (int i = openCardPrice.Length; i > 0; i--)
			{
				gSPacketIn.WriteInt(openCardPrice[i - 1]);
			}
			gSPacketIn.WriteInt(eagleEyePrice.Length);
			for (int j = eagleEyePrice.Length; j > 0; j--)
			{
				gSPacketIn.WriteInt(eagleEyePrice[j - 1]);
			}
			gSPacketIn.WriteInt(flushPrice);
			gSPacketIn.WriteDateTime(System.DateTime.Parse(GameProperties.NewChickenEndTime));
			this.SendTCP(gSPacketIn);
			return gSPacketIn;
		}

		public GSPacketIn SendSingleRoomCreate(BaseRoom room)
        {
			return SendSingleRoomCreate(room, null);
        }

		public GSPacketIn SendSingleRoomCreate(BaseRoom room, GamePlayer player)
        {
			GSPacketIn pkg = new GSPacketIn(94);
			pkg.WriteByte(18);
			pkg.WriteInt(room.RoomId);//_loc4_.ID = _loc3_.readInt();
			pkg.WriteByte((byte)room.RoomType);//_loc4_.type = _loc3_.readByte();
			pkg.WriteBoolean(room.IsPlaying);//_loc4_.isPlaying = _loc3_.readBoolean();
			pkg.WriteByte((byte)room.GameType);//_loc4_.gameMode = _loc3_.readByte();
			pkg.WriteInt(room.MapId);//loc4_.mapId = _loc3_.readInt();
			pkg.WriteBoolean(room.isCrosszone);//_loc4_.isCrossZone = _loc3_.readBoolean();
			pkg.WriteInt(room.AreaID);//PlayerManager.Instance.Self.ZoneID = _loc3_.readInt();
			if (player == null)
            {
				SendTCP(pkg);
            }
			else
            {
				player.SendTCP(pkg);
            }
			return pkg;
        }

		public GSPacketIn SendConsortiaBattleOpenClose(int ID, bool result)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(153, ID);
			gSPacketIn.WriteByte(1);
			gSPacketIn.WriteBoolean(result);
			if (result)
			{
				gSPacketIn.WriteDateTime(DateTime.Now);
				gSPacketIn.WriteDateTime(DateTime.Now.AddMinutes(90.0));
				gSPacketIn.WriteBoolean(result);
			}
			this.SendTCP(gSPacketIn);
			return gSPacketIn;
		}

		public GSPacketIn SendUpdatePlayerProperty(PlayerInfo info, PlayerProperty PlayerProp)
        {
			string[] array = new string[] { "Attack", "Defence", "Agility", "Luck" };
			GSPacketIn pkg = new GSPacketIn(164, info.ID);
			pkg.WriteInt(info.ID);
			string[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
            {
				string key = array2[i];
				pkg.WriteInt(0);
				pkg.WriteInt(0);//pkg.WriteInt(PlayerProp.Current["Texp"][key]);
				pkg.WriteInt(0);//pkg.WriteInt(PlayerProp.Current["Card"][key]);
				pkg.WriteInt(0);//pkg.WriteInt(PlayerProp.Current["Pet"][key]);
				pkg.WriteInt(0);//pkg.WriteInt(PlayerProp.Current["Suit"][key]);
				pkg.WriteInt(PlayerProp.Current["Avatar"][key]);
			}
			pkg.WriteInt(0);
			pkg.WriteInt(0);
			pkg.WriteInt(0);
			pkg.WriteInt(0);
			pkg.WriteInt(PlayerProp.Current["Avatar"]["HP"]);
			pkg.WriteInt(0);
			pkg.WriteInt(0);
			pkg.WriteInt(0);
			pkg.WriteInt(0);
			pkg.WriteInt(PlayerProp.Current["Damage"]["Avatar"]); //_loc_4["Damage"]["Avatar"] = _loc_2.readInt();
			pkg.WriteInt(PlayerProp.Current["Armor"]["Avatar"]); //_loc_4["Armor"]["Avatar"] = _loc_2.readInt();
			pkg.WriteInt(PlayerProp.totalDamage);
			pkg.WriteInt(PlayerProp.totalArmor);
			this.SendTCP(pkg);
			return pkg;
		}

		public void SendOpenHappyRecharge(int playerID)
        {
			GSPacketIn pkg = new GSPacketIn(145, playerID);
			pkg.WriteByte(178);
			pkg.WriteBoolean(GameProperties.HappyRechargeOpenClose);
			SendTCP(pkg);
        }

		public void SendCatchBeastOpen(int playerID, bool isOpen)
		{
			GSPacketIn pkg = new GSPacketIn(145, playerID);
			pkg.WriteByte(32);
			pkg.WriteBoolean(isOpen);
			SendTCP(pkg);
		}

		public GSPacketIn SendUpdateUpCount(PlayerInfo player)
		{
			GSPacketIn pkg = new GSPacketIn(96, player.ID);
			pkg.WriteInt(player.MaxBuyHonor);
			SendTCP(pkg);
			return pkg;
		}

		public GSPacketIn SendPlayerRefreshTotem(PlayerInfo player)
		{
			GSPacketIn pkg = new GSPacketIn(136, player.ID);
			pkg.WriteInt(1);
			pkg.WriteInt(player.myHonor);
			pkg.WriteInt(player.totemId);
			SendTCP(pkg);
			return pkg;
		}

		public GSPacketIn SendPlayerFigSpiritinit(int ID, List<UserGemStone> gems)
		{
			GSPacketIn pkg = new GSPacketIn(209, ID);
			pkg.WriteByte(1);
			pkg.WriteBoolean(true);
			pkg.WriteInt(gems.Count);
			foreach (UserGemStone g in gems)
			{
				pkg.WriteInt(g.UserID); //_loc_5.userId = event.pkg.readInt();
				pkg.WriteInt(g.FigSpiritId); //_loc_5.figSpiritId = event.pkg.readInt();
				pkg.WriteString(g
					.FigSpiritIdValue); //_loc_5.figSpiritIdValue = event.pkg.readUTF();"0,0,0|0,0,0" => "lv,exp,place|lv,exp,place"
				pkg.WriteInt(g.EquipPlace); //_loc_5.equipPlace = event.pkg.readInt();
			}

			SendTCP(pkg);
			return pkg;
		}

		public GSPacketIn SendPlayerFigSpiritUp(int ID, UserGemStone gem, bool isUp, bool isMaxLevel, bool isFall,
			int num, int dir)
		{
			GSPacketIn pkg = new GSPacketIn(209, ID);
			pkg.WriteByte(2);
			string[] figSpiritIdValue = gem.FigSpiritIdValue.Split('|');
			pkg.WriteBoolean(isUp); //_loc_2.isUp = event.pkg.readBoolean();
			pkg.WriteBoolean(isMaxLevel); //_loc_2.isMaxLevel = event.pkg.readBoolean();
			pkg.WriteBoolean(isFall); //_loc_2.isFall = event.pkg.readBoolean();
			pkg.WriteInt(num); //_loc_2.num = event.pkg.readInt(); //3
			pkg.WriteInt(figSpiritIdValue.Length); //var _loc_4:* = event.pkg.readInt();
			int loc5 = 0;
			while (loc5 < figSpiritIdValue.Length)
			{
				string figSpiritId = figSpiritIdValue[loc5];
				pkg.WriteInt(gem.FigSpiritId); //_loc_6.fightSpiritId = event.pkg.readInt();
				pkg.WriteInt(Convert.ToInt32(figSpiritId.Split(',')[0])); //_loc_6.level = event.pkg.readInt();
				pkg.WriteInt(Convert.ToInt32(figSpiritId.Split(',')[1])); //_loc_6.exp = event.pkg.readInt();
				pkg.WriteInt(
					Convert.ToInt32(figSpiritId.Split(',')[2])); //_loc_6.place = event.pkg.readInt();
				loc5++;
			}

			pkg.WriteInt(gem.EquipPlace); // _loc_2.equipPlace = event.pkg.readInt();
			pkg.WriteInt(dir); //_loc_2.dir = event.pkg.readInt();//1
			SendTCP(pkg);
			return pkg;
		}

		public GSPacketIn SendNecklaceStrength(PlayerInfo player)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(95, player.ID);
			gSPacketIn.WriteInt(player.necklaceExp);
			gSPacketIn.WriteInt(player.necklaceExpAdd);
			this.SendTCP(gSPacketIn);
			return gSPacketIn;
		}
		public void SendOpenOrCloseChristmas(int lastPacks, bool isOpen)
        {
			GSPacketIn packet = new GSPacketIn(145);
			packet.WriteByte(16);
			packet.WriteBoolean(isOpen);
			if (isOpen)
			{
				DateTime beginTime = DateTime.Parse(GameProperties.ChristmasBeginDate);
				DateTime endTime = DateTime.Parse(GameProperties.ChristmasEndDate);
				packet.WriteDateTime(beginTime);
				packet.WriteDateTime(endTime);
				string[] christmasGifts = GameProperties.ChristmasGifts.Split('|');
				packet.WriteInt(christmasGifts.Length);
				int loc3 = 0;
				while (loc3 < christmasGifts.Length)
				{
					string[] _loc_4 = christmasGifts[loc3].Split(',');
					packet.WriteInt(int.Parse(_loc_4[0]));
					packet.WriteInt(int.Parse(_loc_4[1]));
					loc3++;
				}

				packet.WriteInt(lastPacks);
				packet.WriteInt(GameProperties.ChristmasBuildSnowmanDoubleMoney);
			}
			SendTCP(packet);
		}

		public GSPacketIn SendUserSyncEquipGhost(GamePlayer p)
		{
			GSPacketIn pkg = new GSPacketIn(392);

			List<UserEquipGhostInfo> lists = p.GetAllEquipGhost();
			pkg.WriteInt(lists.Count);
			foreach (UserEquipGhostInfo info in lists)
			{
				pkg.WriteInt(info.BagType);
				pkg.WriteInt(info.Place);
				pkg.WriteInt(info.Level);
				pkg.WriteInt(info.TotalGhost);
			}

			SendTCP(pkg);
			return pkg;
		}

		public void SendOpenWorldBoss(int pX, int pY)
		{
			BaseWorldBossRoom worldBossRoom = RoomMgr.WorldBossRoom;
			GSPacketIn gSPacketIn = new GSPacketIn(102);
			gSPacketIn.WriteByte(0);
			gSPacketIn.WriteString(worldBossRoom.BossResourceId);
			gSPacketIn.WriteInt(worldBossRoom.CurrentPVE);
			gSPacketIn.WriteString("Thần thú");
			gSPacketIn.WriteString(worldBossRoom.Name);
			gSPacketIn.WriteLong(worldBossRoom.MaxBlood);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt(1);
			gSPacketIn.WriteInt((pX == 0 ? worldBossRoom.playerDefaultPosX : pX));
			gSPacketIn.WriteInt((pY == 0 ? worldBossRoom.playerDefaultPosY : pY));
			gSPacketIn.WriteDateTime(worldBossRoom.Begin_time);
			gSPacketIn.WriteDateTime(worldBossRoom.End_time);
			gSPacketIn.WriteInt(worldBossRoom.Fight_time);
			gSPacketIn.WriteBoolean(worldBossRoom.FightOver);
			gSPacketIn.WriteBoolean(worldBossRoom.RoomClose);
			gSPacketIn.WriteInt(worldBossRoom.ticketID);
			gSPacketIn.WriteInt(worldBossRoom.need_ticket_count);
			gSPacketIn.WriteInt(worldBossRoom.timeCD);
			gSPacketIn.WriteInt(worldBossRoom.reviveMoney);
			//gSPacketIn.WriteInt(worldBossRoom.reFightMoney);
			//gSPacketIn.WriteInt(worldBossRoom.addInjureBuffMoney);
			//gSPacketIn.WriteInt(worldBossRoom.addInjureValue);
			gSPacketIn.WriteInt(0);
			//gSPacketIn.WriteInt(1);
			//gSPacketIn.WriteString("Thần thúa");
			//gSPacketIn.WriteInt(123);
			//gSPacketIn.WriteString("Thần thúb");
			//gSPacketIn.WriteInt(1);
			gSPacketIn.WriteBoolean(true);
			gSPacketIn.WriteBoolean(false);
			this.SendTCP(gSPacketIn);
		}

		public GSPacketIn SendAvatarColectionAllInfo(int PlayerId, Dictionary<int, UserAvatarColectionInfo> infos)
		{
			Dictionary<int, List<int>> types = new Dictionary<int, List<int>>();
			List<int> dressGroups = new List<int>();
			List<int> WeaponGroups = new List<int>();
			foreach (int key in infos.Keys)
			{
				if (string.IsNullOrEmpty(infos[key].ActiveDress))
					continue;
				if (infos[key].Sex == 3)
				{
					WeaponGroups.Add(key);
				}
				else
				{
					dressGroups.Add(key);
				}
			}

			GSPacketIn pkg = new GSPacketIn((short)ePackageType.AVATAR_COLLECTION, PlayerId);
			pkg.WriteByte((byte)AvatarCollectionPackageType.GET_ALL_INFO);
			if (GameProperties.VERSION >= 8300)
			{
				types.Add(1, dressGroups);
				types.Add(2, WeaponGroups);

				pkg.WriteInt(types.Count);
				foreach (int index in types.Keys)
				{
					pkg.WriteInt(index);
					pkg.WriteInt(types[index].Count);
					foreach (int key in types[index])
					{
						pkg.WriteInt(key);
						pkg.WriteInt(infos[key].Sex);
						pkg.WriteInt(infos[key].CurrentGroup.Length);
						foreach (string id in infos[key].CurrentGroup)
						{
							pkg.WriteInt(int.Parse(id));
						}
						pkg.WriteDateTime(infos[key].endTime);
					}

				}
			}
			else
			{
				pkg.WriteInt(dressGroups.Count);
				foreach (int key in dressGroups)
				{
					pkg.WriteInt(key);
					pkg.WriteInt(infos[key].Sex);
					pkg.WriteInt(infos[key].CurrentGroup.Length);
					foreach (string id in infos[key].CurrentGroup)
					{
						pkg.WriteInt(int.Parse(id));
					}

					pkg.WriteDateTime(infos[key].endTime);
				}
			}

			SendTCP(pkg);
			return pkg;
		}

		public void SendLoginChickActivation(UserChickActiveInfo chickInfo)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(84);
			gSPacketIn.WriteInt(2);
			gSPacketIn.WriteInt(1);
			gSPacketIn.WriteInt(chickInfo.IsKeyOpened);
			gSPacketIn.WriteInt(1);
			gSPacketIn.WriteDateTime(chickInfo.KeyOpenedTime);
			gSPacketIn.WriteInt(chickInfo.KeyOpenedType);
			gSPacketIn.WriteInt((chickInfo.EveryDay.Day < DateTime.Now.Day && DateTime.Now.DayOfWeek == DayOfWeek.Monday) ? 0 : 1);
			gSPacketIn.WriteInt((chickInfo.EveryDay.Day < DateTime.Now.Day && DateTime.Now.DayOfWeek == DayOfWeek.Tuesday) ? 0 : 1);
			gSPacketIn.WriteInt((chickInfo.EveryDay.Day < DateTime.Now.Day && DateTime.Now.DayOfWeek == DayOfWeek.Wednesday) ? 0 : 1);
			gSPacketIn.WriteInt((chickInfo.EveryDay.Day < DateTime.Now.Day && DateTime.Now.DayOfWeek == DayOfWeek.Thursday) ? 0 : 1);
			gSPacketIn.WriteInt((chickInfo.EveryDay.Day < DateTime.Now.Day && DateTime.Now.DayOfWeek == DayOfWeek.Friday) ? 0 : 1);
			gSPacketIn.WriteInt((chickInfo.EveryDay.Day < DateTime.Now.Day && DateTime.Now.DayOfWeek == DayOfWeek.Saturday) ? 0 : 1);
			gSPacketIn.WriteInt((chickInfo.EveryDay.Day < DateTime.Now.Day && DateTime.Now.DayOfWeek == DayOfWeek.Sunday) ? 0 : 1);
			gSPacketIn.WriteInt((chickInfo.AfterThreeDays.Day < DateTime.Now.Day && chickInfo.OnThreeDay(DateTime.Now)) ? 0 : 1);
			gSPacketIn.WriteInt((chickInfo.AfterThreeDays.Day < DateTime.Now.Day && chickInfo.OnThreeDay(DateTime.Now)) ? 0 : 1);
			gSPacketIn.WriteInt((chickInfo.AfterThreeDays.Day < DateTime.Now.Day && chickInfo.OnThreeDay(DateTime.Now)) ? 0 : 1);
			gSPacketIn.WriteInt((chickInfo.Weekly < chickInfo.StartOfWeek(DateTime.Now, DayOfWeek.Saturday) && DateTime.Now.DayOfWeek == DayOfWeek.Saturday) ? 0 : 1);
			gSPacketIn.WriteInt(chickInfo.CurrentLvAward);
			this.SendTCP(gSPacketIn);
		}

		public void SendUpdateChickActivation(UserChickActiveInfo chickInfo)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(84);
			gSPacketIn.WriteInt(2);
			gSPacketIn.WriteInt(2);
			gSPacketIn.WriteInt(chickInfo.IsKeyOpened);
			gSPacketIn.WriteInt(1);
			gSPacketIn.WriteDateTime(chickInfo.KeyOpenedTime);
			gSPacketIn.WriteInt(chickInfo.KeyOpenedType);
			gSPacketIn.WriteInt((chickInfo.EveryDay.Day < DateTime.Now.Day && DateTime.Now.DayOfWeek == DayOfWeek.Monday) ? 0 : 1);
			gSPacketIn.WriteInt((chickInfo.EveryDay.Day < DateTime.Now.Day && DateTime.Now.DayOfWeek == DayOfWeek.Tuesday) ? 0 : 1);
			gSPacketIn.WriteInt((chickInfo.EveryDay.Day < DateTime.Now.Day && DateTime.Now.DayOfWeek == DayOfWeek.Wednesday) ? 0 : 1);
			gSPacketIn.WriteInt((chickInfo.EveryDay.Day < DateTime.Now.Day && DateTime.Now.DayOfWeek == DayOfWeek.Thursday) ? 0 : 1);
			gSPacketIn.WriteInt((chickInfo.EveryDay.Day < DateTime.Now.Day && DateTime.Now.DayOfWeek == DayOfWeek.Friday) ? 0 : 1);
			gSPacketIn.WriteInt((chickInfo.EveryDay.Day < DateTime.Now.Day && DateTime.Now.DayOfWeek == DayOfWeek.Saturday) ? 0 : 1);
			gSPacketIn.WriteInt((chickInfo.EveryDay.Day < DateTime.Now.Day && DateTime.Now.DayOfWeek == DayOfWeek.Sunday) ? 0 : 1);
			gSPacketIn.WriteInt((chickInfo.AfterThreeDays.Day < DateTime.Now.Day && chickInfo.OnThreeDay(DateTime.Now)) ? 0 : 1);
			gSPacketIn.WriteInt((chickInfo.AfterThreeDays.Day < DateTime.Now.Day && chickInfo.OnThreeDay(DateTime.Now)) ? 0 : 1);
			gSPacketIn.WriteInt((chickInfo.AfterThreeDays.Day < DateTime.Now.Day && chickInfo.OnThreeDay(DateTime.Now)) ? 0 : 1);
			gSPacketIn.WriteInt((chickInfo.Weekly < chickInfo.StartOfWeek(DateTime.Now, DayOfWeek.Saturday) && DateTime.Now.DayOfWeek == DayOfWeek.Saturday) ? 0 : 1);
			gSPacketIn.WriteInt(chickInfo.CurrentLvAward);
			this.SendTCP(gSPacketIn);
		}

		public GSPacketIn SendInviteFriends(PlayerInfo player, int type)
		{
			GSPacketIn packet = new GSPacketIn(107, player.ID);
			packet.WriteInt(type);
			switch (type)
			{
				case 2:
					packet.WriteInt(0); //null
					packet.WriteInt(0); //null
					packet.WriteInt(player.MyInvitedCount); // Arkadaş Davet Sayısı
					break;
				case 3:
					packet.WriteInt(player.MyRewardStatus == true ? 1 : 0); // Ödül Alma Durumum
					break;
				case 4:
					packet.WriteInt(0); //null
					packet.WriteBoolean(true);
					packet.WriteInt(player.AwardColumnOne); // Ödül Alma Sutunü 1
					packet.WriteInt(player.AwardColumnTwo); // Ödül Alma Sutunü 2
					packet.WriteInt(player.AwardColumnThree); // Ödül Alma Sutunü 3
					packet.WriteInt(player.AwardColumnFour); // Ödül Alma Sutunü 4
					break;
				case 5:
					packet.WriteString(player.MyInviteCode); // Benim Rasgele Kodum
					packet.WriteInt(player.MyRewardStatus == true ? 1 : 0); // Ödül Alma Durumum
					packet.WriteDateTime(DateTime.Now); // lastFB_Time // faceyi kapatacaz zaten
					packet.WriteInt(player.AwardColumnOne); // Ödül Alma Sutunü 1
					packet.WriteInt(player.AwardColumnTwo); // Ödül Alma Sutunü 2
					packet.WriteInt(player.AwardColumnThree); // Ödül Alma Sutunü 3
					packet.WriteInt(player.AwardColumnFour); // Ödül Alma Sutunü 4
					packet.WriteInt(4); // serverID
					break;
				case 6:
					packet.WriteDateTime(DateTime.Now); // lastFB_Time // faceyi kapatacaz zaten
					break;
			}
			this.SendTCP(packet);
			return packet;
		}

		public void SendPyramidOpenClose(PyramidConfigInfo info)
		{
			GSPacketIn pkg = new GSPacketIn(145, info.UserID);
			pkg.WriteByte((byte)ActiveSystemPackageType.PYRAMID_OPENORCLOSE);
			pkg.WriteBoolean(info.isOpen); //model.isOpen = param1.readBoolean();
			pkg.WriteBoolean(info.isScoreExchange); //model.isScoreExchange = param1.readBoolean();
			pkg.WriteDateTime(info.beginTime); //    this.model.beginTime = param1.readDate();
			pkg.WriteDateTime(info.endTime); //    this.model.endTime = param1.readDate();
			pkg.WriteInt(info.freeCount); //    this.model.freeCount = param1.readInt();
			pkg.WriteInt(info.turnCardPrice); //    this.model.turnCardPrice = param1.readInt();
			pkg.WriteInt(info.revivePrice.Length); //    _loc_2 = param1.readInt();
			int _loc_3 = 0;
			while (_loc_3 < info.revivePrice.Length)
			{
				pkg.WriteInt(info.revivePrice[_loc_3]); //       this.model.revivePrice.push(param1.readInt());
				_loc_3++;
			}
			SendTCP(pkg);
		}
	}
}
