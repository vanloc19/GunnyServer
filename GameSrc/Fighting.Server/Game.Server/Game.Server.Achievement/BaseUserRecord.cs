using System.Collections;
using Game.Server.GameObjects;

namespace Game.Server.Achievement
{
	public class BaseUserRecord
	{
		protected GamePlayer m_player;

		protected int m_type;

		public BaseUserRecord(GamePlayer player, int type)
		{
			m_player = player;
			m_type = type;
		}

		public virtual void AddTrigger(GamePlayer player)
		{
		}

		public virtual void RemoveTrigger(GamePlayer player)
		{
		}

		public static void CreateCondition(Hashtable ht, GamePlayer m_player)
		{
			foreach (DictionaryEntry item in ht)
			{
				int type = int.Parse(item.Key.ToString());
				switch (type)
				{
				case 1:
					new ChangeAttackCondition(m_player, type);
					break;
				case 2:
					new ChangeDefenceCondition(m_player, type);
					break;
				case 3:
					new ChangeAgilityCondition(m_player, type);
					break;
				case 4:
					new ChangeLuckyCondition(m_player, type);
					break;
				case 5:
					new Mission4KillCondition(m_player, type);
					break;
				case 6:
					new Mission9OverCondition(m_player, type);
					break;
				case 7:
					new MissionKillChiefCondition(m_player, type);
					break;
				case 8:
					new MissionKillMinotaurCondition(m_player, type);
					break;
				case 9:
					new ChangeFightPowerCondition(m_player, type);
					break;
				case 10:
					new ChangeGradeCondition(m_player, type);
					break;
				case 11:
					new ChangeTotalCondition(m_player, type);
					break;
				case 12:
					new ChangeWinCondition(m_player, type);
					break;
				case 13:
					new ChangeOnlineTimeCondition(m_player, type);
					break;
				case 14:
					new FightByFreeCondition(m_player, type);
					break;
				case 15:
					new FightByGuildCondition(m_player, type);
					break;
				case 17:
					new FightByGuildSpanAreaCondition(m_player, type);
					break;
				case 18:
					new MarryApplyReplyCondition(m_player, type);
					break;
				case 19:
					new GameKillByGameCondition(m_player, type);
					break;
				case 20:
					new FightDispatchesCondition(m_player, type);
					break;
				case 21:
					new QuestBlueCondition(m_player, type);
					break;
				case 22:
					new QuestDailyCondition(m_player, type);
					break;
				case 23:
					new PlayerGoodsPresentCondition(m_player, type);
					break;
				case 24:
					new AddRichesOfferCondition(m_player, type);
					break;
				case 25:
					new AddRichesRobCondition(m_player, type);
					break;
				case 26:
					new Mission1KillCondition(m_player, type);
					break;
				case 27:
					new Mission2KillCondition(m_player, type);
					break;
				case 28:
					new Mission1OverCondition(m_player, type);
					break;
				case 29:
					new Mission2OverCondition(m_player, type);
					break;
				case 30:
					new Mission8OverCondition(m_player, type);
					break;
				case 31:
					new Mission3KillCondition(m_player, type);
					break;
				case 32:
					new ItemStrengthenCondition(m_player, type);
					break;
				case 33:
					new HotSpringCondition(m_player, type);
					break;
				case 34:
					new UsingIgnoreArmorCondition(m_player, type);
					break;
				case 35:
					new UsingAtomicBombCondition(m_player, type);
					break;
				case 36:
					new ChangeColorsCondition(m_player, type);
					break;
				case 37:
					new PlayerLoginCondition(m_player, type);
					break;
				case 38:
					new AddGoldCondition(m_player, type);
					break;
				case 39:
					new AddGiftTokenCondition(m_player, type);
					break;
				case 40:
					new AddMedalCondition(m_player, type);
					break;
				case 41:
					new FightOneBloodIsWinCondition(m_player, type);
					break;
				case 42:
					new UsingSecondWeaponTrueAngelCondition(m_player, type);
					break;
				case 43:
					new UsingGEMCondition(m_player, type);
					break;
				case 44:
					new UsingRenameCardCondition(m_player, type);
					break;
				case 45:
					new UsingSalutingGunCondition(m_player, type);
					break;
				case 46:
					new UsingSpanAreaBugleCondition(m_player, type);
					break;
				case 47:
					new UsingBigBugleCondition(m_player, type);
					break;
				case 48:
					new UsingSmallBugleCondition(m_player, type);
					break;
				case 49:
					new UsingEngagementRingCondition(m_player, type);
					break;
				case 50:
					new FightAddOfferCondition(m_player, type);
					break;
				case 51:
					new FightCoupleCondition(m_player, type);
					break;
				case 52:
					new Mission3OverCondition(m_player, type);
					break;
				case 53:
					new Mission4OverCondition(m_player, type);
					break;
				case 54:
					new Mission5OverCondition(m_player, type);
					break;
				case 55:
					new Mission6OverCondition(m_player, type);
					break;
				case 56:
					new Mission7OverCondition(m_player, type);
					break;
				case 57:
					new ItemStrengthenCondition(m_player, type);
					break;
				case 58:
					new UsingSuperWeaponCondition(m_player, type);
					break;
				case 59:
					new QuestGoodManCardCondition(m_player, type);
					break;
				case 60:
					new MissionKillTerrorKingCondition(m_player, type);
					break;
				case 61:
					new MissionKillTerrorBoguCondition(m_player, type);
					break;
				case 64:
					new FightWithWeaponCondition(m_player, type);
					break;
				case 65:
					new FightWithWeaponCondition(m_player, type);
					break;
				case 66:
					new FightWithWeaponCondition(m_player, type);
					break;
				case 67:
					new FightWithWeaponCondition(m_player, type);
					break;
				case 68:
					new FightWithWeaponCondition(m_player, type);
					break;
				case 69:
					new FightWithWeaponCondition(m_player, type);
					break;
				case 70:
					new FightWithWeaponCondition(m_player, type);
					break;
				case 71:
					new FightWithWeaponCondition(m_player, type);
					break;
				case 72:
					new FightWithWeaponCondition(m_player, type);
					break;
				case 73:
					new FightByFreeSpanAreaCondition(m_player, type);
					break;
				case 74:
					new VIPCondition(m_player, type);
					break;
				case 75:
					new GetApprenticeCondition(m_player, type);
					break;
				case 76:
					new ApprenticeCompleteCondition(m_player, type);
					break;
				case 77:
					new GetMasterCondition(m_player, type);
					break;
				case 78:
					new MasterCompleteCondition(m_player, type);
					break;
				case 79:
					new Mission5KillCondition(m_player, type);
					break;
				case 80:
					new Mission10OverCondition(m_player, type);
					break;
				case 81:
					new MissionKillMekaCondition(m_player, type);
					break;
				case 82:
					new Mission11OverCondition(m_player, type);
					break;
				case 88:
					new StartMissionCondition(m_player, type);
					break;
				case 89:
					new MissionKillBatosCondition(m_player, type);
					break;
				case 90:
					new Mission12OverCondition(m_player, type);
					break;
				case 94:
					new FightLabPrimaryCondition(m_player, type);
					break;
				}
			}
		}
	}
}
