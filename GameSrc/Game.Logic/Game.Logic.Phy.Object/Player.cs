using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Bussiness;
using Bussiness.Managers;
using Game.Logic.Actions;
using Game.Logic.Effects;
using Game.Logic.PetEffects;
using Game.Logic.Phy.Maths;
using Game.Logic.Spells;
using SqlDataProvider.Data;
using Game.Logic.CardEffect.Effects;
using Game.Logic.Game.Logic;
using Game.Logic.PetEffects.Element.Actives;
using Game.Logic.PetEffects.Element.Passives;

namespace Game.Logic.Phy.Object
{
	public class Player : TurnedLiving
	{
		public bool CoVe = false;

		public int BossCardCount;

		public int CanTakeOut;

		private static readonly int CARRY_TEMPLATE_ID = 10016;

		private int deputyWeaponResCount;

		public bool FinishTakeCard;

		public int GainGP;

		public int GainOffer;

		public bool HasPaymentTakeCard;

		private Dictionary<int, int> ItemFightBag;

		public bool LockDirection;

		private int m_AddWoundBallId;

		private int m_ballCount;

		private bool m_canGetProp;

		private BallInfo m_currentBall;

		private int m_changeSpecialball;

		private ItemInfo m_DeputyWeapon;

		private int m_energy;

		private int m_flyCoolDown;

		private ItemInfo m_Healstone;

		private bool m_isActive;

		private int m_loadingProcess;

		private int m_mainBallId;

		private int m_MultiBallId;

		private int m_oldx;

		public bool AttackInformation;

		public bool DefenceInformation;

		private int m_killedPunishmentOffer;

		private int m_powerRatio;

		public int MaxPsychic = 999;

		private int m_oldy;

		private IGamePlayer m_player;

		private int m_prop;

		private int m_shootCount;

		private int m_spBallId;

		private ArrayList m_tempBoxes;

		private ItemInfo m_weapon;

		public bool Ready;

		public Point TargetPoint;

		public int TotalAllCure;

		public int TotalAllExperience;

		public int TotalAllHitTargetCount;

		public int TotalAllHurt;

		public int TotalAllKill;

		public int TotalAllScore;

		public int TotalAllShootCount;

		public bool LimitEnergy;

		public bool CanFly = true;

		public bool CheckUseFrost = false;


		private readonly List<int> AllowedItems = new List<int>
		{
			10009,
			10010,
			10011,
			10012,
			10018,
			10021
		};

		private Random rand;

		private UsersPetInfo userPetInfo;

		private PetFightPropertyInfo petFightPropertyInfo;

		private BufferInfo m_bufferPoint;

		public int MOVE_SPEED;

		private double speedMultiplier;

		private Dictionary<int, PetSkillInfo> _petSkillCd;

		public Dictionary<int, PetSkillInfo> PetSkillCD
		{
			get { return _petSkillCd; }
		}

		public int PowerRatio
        {
			set { m_powerRatio = value; }
			get { return m_powerRatio; }
        }
		public double SpeedMult
		{
			get
			{
				return speedMultiplier;
			}
			set
			{
				speedMultiplier = value / (double)base.STEP_X;
			}
		}

		public int StepX => (int)((double)base.STEP_X * speedMultiplier);

		public int StepY => (int)((double)base.STEP_Y * speedMultiplier);

		public int BallCount
		{
			get
			{
				return m_ballCount;
			}
			set
			{
				if (m_ballCount != value)
				{
					m_ballCount = value;
				}
			}
		}

		public bool CanGetProp
		{
			get
			{
				return m_canGetProp;
			}
			set
			{
				if (m_canGetProp != value)
				{
					m_canGetProp = value;
				}
			}
		}

		public BallInfo CurrentBall => m_currentBall;

		public int ChangeSpecialBall
		{
			get
			{
				return m_changeSpecialball;
			}
			set
			{
				m_changeSpecialball = value;
			}
		}

		public ItemInfo DeputyWeapon
        {
			get
            {
				return m_DeputyWeapon;
            }
			set
            {
				m_DeputyWeapon = value;
            }
        }

		public int deputyWeaponCount => deputyWeaponResCount;

		public int Energy
		{
			get
			{
				return m_energy;
			}
			set
			{
				m_energy = value;
			}
		}

		public int flyCount => m_flyCoolDown;

		public bool IsActive => m_isActive;

		public bool IsSpecialSkill => m_currentBall.ID == m_spBallId;

		public int LoadingProcess
		{
			get
			{
				return m_loadingProcess;
			}
			set
			{
				if (m_loadingProcess != value)
				{
					m_loadingProcess = value;
					if (m_loadingProcess >= 100)
					{
						OnLoadingCompleted();
					}
				}
			}
		}

		public int KilledPunishmentOffer
		{
			get
			{
				return m_killedPunishmentOffer;
			}
			set
			{
				m_killedPunishmentOffer = value;
			}
		}

		public int OldX
		{
			get
			{
				return m_oldx;
			}
			set
			{
				m_oldx = value;
			}
		}

		public int OldY
		{
			get
			{
				return m_oldy;
			}
			set
			{
				m_oldy = value;
			}
		}

		public IGamePlayer PlayerDetail => m_player;

		public int Prop
		{
			get
			{
				return m_prop;
			}
			set
			{
				m_prop = value;
			}
		}

		public int ShootCount
		{
			get
			{
				return m_shootCount;
			}
			set
			{
				if (m_shootCount != value)
				{
					m_shootCount = value;
					m_game.SendGameUpdateShootCount(this);
				}
			}
		}

		public ItemInfo Weapon => m_weapon;

		public UsersPetInfo Pet => userPetInfo;

		public event PlayerEventHandle AfterPlayerShooted;

		public event PlayerEventHandle BeforeBomb;

		public event PlayerEventHandle BeforePlayerShoot;

		public event PlayerEventHandle CollidByObject;

		public event PlayerEventHandle LoadingCompleted;

		public event PlayerEventHandle PlayerShootCure;

		public event PlayerEventHandle PlayerBeginMoving;

		public event PlayerEventHandle PlayerBuffSkillPet;

		public event PlayerEventHandle PlayerClearBuffSkillPet;

		public event PlayerEventHandle PlayerCure;

		public event PlayerEventHandle PlayerGuard;

		public event PlayerEventHandle PlayerShoot;

		public event PlayerEventHandle PlayerCompleteShoot;

		public event PlayerEventHandle PlayerAnyShellThrow;

		public event PlayerSecondWeaponEventHandle PlayerUseSecondWeapon;

		public event PlayerMissionEventHandle MissionEventHandle;

		public Player(IGamePlayer player, int id, BaseGame game, int team, int maxBlood) : base(id, game, team, "", "", maxBlood, 0, 1)
		{
			m_tempBoxes = new ArrayList();
			m_flyCoolDown = 2;
			speedMultiplier = 1.0;
			MOVE_SPEED = 2;
			m_rect = new Rectangle(-15, -20, 30, 30);
			_petSkillCd = new Dictionary<int, PetSkillInfo>();
			ItemFightBag = new Dictionary<int, int>();
			userPetInfo = player.Pet;
			if (userPetInfo != null && game != null && !game.IsSpecialPVE())
			{
				base.isPet = true;
				PetEffects.PetBaseAtt = this.GetPetBaseAtt();
				InitPetSkillEffect(userPetInfo);
				petFightPropertyInfo = PetMgr.FindFightProperty(player.PlayerCharacter.evolutionGrade);
			}
			m_player = player;
			m_player.GamePlayerId = id;
			m_player.GameId = id;
			m_isActive = true;
			m_canGetProp = true;
			Grade = player.PlayerCharacter.Grade;
			if (base.AutoBoot)
			{
				base.VaneOpen = true;
			}
			else
			{
				base.VaneOpen = player.PlayerCharacter.Grade >= 9;
			}
			InitFightBuffer(player.FightBuffs);
			TotalAllHurt = 0;
			TotalAllHitTargetCount = 0;
			TotalAllShootCount = 0;
			TotalAllKill = 0;
			TotalAllExperience = 0;
			TotalAllScore = 0;
			TotalAllCure = 0;
			m_DeputyWeapon = m_player.SecondWeapon;
			m_Healstone = m_player.Healstone;
			ChangeSpecialBall = 0;
			base.BlockTurn = false;
			deputyWeaponResCount = ((m_DeputyWeapon == null) ? 1 : (m_DeputyWeapon.StrengthenLevel + 1));
			m_weapon = m_player.MainWeapon;
			if (m_weapon != null)
			{
				BallConfigInfo ball = BallConfigMgr.FindBall(m_weapon.TemplateID);
				if (m_weapon.isGold)
				{
					ball = BallConfigMgr.FindBall(m_weapon.GoldEquip.TemplateID);
				}
				m_mainBallId = ball.Common;
				m_spBallId = ball.Special;
				m_AddWoundBallId = ball.CommonAddWound;
				m_MultiBallId = ball.CommonMultiBall;
			}
			m_loadingProcess = 0;
			m_prop = 0;
			CanUsePetSkill = true;
			InitBuffer(m_player.EquipEffect);
			m_energy = (m_player.PlayerCharacter.AgiAddPlus + m_player.PlayerCharacter.Agility) / 30 + 240;
			m_maxBlood = m_player.PlayerCharacter.hp;
			this.IsQuanChien = this.m_player.IsQuanChien;
			this.Place = this.m_player.Place;
			if (base.FightBuffers.ConsortionAddMaxBlood > 0)
			{
				m_maxBlood += m_maxBlood * base.FightBuffers.ConsortionAddMaxBlood / 100;
			}
			if(base.FightBuffers.WorldBossHP > 0)
				m_maxBlood += FightBuffers.WorldBossHP;

			if (base.FightBuffers.WorldBossHP_MoneyBuff > 0)
				m_maxBlood += FightBuffers.WorldBossHP_MoneyBuff;

			m_maxBlood += m_player.PlayerCharacter.HpAddPlus + PetEffects.MaxBlood + (PetEffects == null ? 0 : PetEffects.AddMaxBloodValue);
			m_maxBlood += (PetEffects == null ? 0 : PetEffects.AddMaxBloodValue);
			CanFly = true;
			m_powerRatio = 100;
			if (game != null && !game.IsSpecialPVE())
			{
				BufferInfo fightBuffByType = GetFightBuffByType(BuffType.Agility);
				if (fightBuffByType != null && m_player.UsePayBuff(BuffType.Agility))
				{
					m_bufferPoint = fightBuffByType;
				}
			}
		}

		public int GetPetBaseAtt()
		{
			try
			{
				string[] skillArray = this.userPetInfo.SkillEquip.Split(new char[]
				{
					'|'
				});
				for (int i = 0; i < skillArray.Length; i++)
				{
					int skillId = Convert.ToInt32(skillArray[i].Split(new char[]
					{
						','
					})[0]);
					PetSkillInfo newBall = PetMgr.FindPetSkill(skillId);
					if (newBall != null && newBall.Damage > 0)
					{
						int result = newBall.Damage;
						return result;
					}
				}
			}
			catch (Exception e)
			{
				Console.WriteLine("______________GetPetBaseAtt ERROR______________");
				Console.WriteLine(e.Message);
				Console.WriteLine(e.StackTrace);
				Console.WriteLine("_______________________________________________");
				int result = 0;
				return result;
			}
			return 0;
		}

		public bool CanUseItem(ItemTemplateInfo item)
		{
			return this.m_energy >= item.Property4 && (base.IsAttacking || (!base.IsLiving && base.Team == this.m_game.CurrentLiving.Team));
		}
        //fix sinh linh
        public bool CanUseItem(ItemTemplateInfo item, int place)
        {
            if (m_currentBall.IsSpecial() && !AllowedItems.Contains(item.TemplateID))
            {
                return false;
            }
            if (!base.IsLiving && place == -1)
            {
                return base.psychic >= item.Property7;
            }
            if (!base.IsLiving && place != -1 && base.Team == m_game.CurrentLiving.Team)
            {
                return true;
            }
            if (m_energy < item.Property4)
            {
                return false;
            }
            if (!base.IsAttacking)
            {
                if (!base.IsLiving && base.Team == m_game.CurrentLiving.Team)
                {
                    return IsActive;
                }
                return false;
            }
            return true;
        }

        public void capnhatstate(string loai1, string loai2)
		{
			m_game.capnhattrangthai(this, loai1, loai2);
		}

		public override void CollidedByObject(Physics phy)
		{
			base.CollidedByObject(phy);
			if (phy is SimpleBomb)
			{
				OnCollidedByObject();
			}
		}

		public bool CheckShootPoint(int x, int y)
		{
			return true;
		}

		public void DeadLink()
		{
			m_isActive = false;
			if (base.IsLiving)
			{
				Die();
			}
		}

	public override void Die()
	{
		if (base.IsLiving)
		{
			m_y -= 70;
			base.Die();
		}
	}

	public void InitBuffer(List<int> equpedEffect)
		{
			for (int index = 0; index < equpedEffect.Count; index++)
			{
				ItemTemplateInfo itemTemplate = ItemMgr.FindItemTemplate(equpedEffect[index]);
				switch (itemTemplate.Property3)
				{
					case 1://Cường kích.
						new AddAttackEffect(itemTemplate.Property4, itemTemplate.Property5).Start(this);
						break;
					case 2://Thạch phu.
						new AddDefenceEffect(itemTemplate.Property4, itemTemplate.Property5).Start(this);
						break;
					case 3://Unknow
						new AddAgilityEffect(itemTemplate.Property4, itemTemplate.Property5).Start(this);
						break;
					case 4://Ban phúc.
						new AddLuckyEffect(itemTemplate.Property4, itemTemplate.Property5).Start(this);
						break;
					case 5://Thương nặng.
						new AddDamageEffect(itemTemplate.Property4, itemTemplate.Property5).Start(this);
						break;
					case 6://Kiên cố.
						new ReduceDamageEffect(itemTemplate.Property4, itemTemplate.Property5).Start(this);
						break;
					case 7://Unknow
						new AddBloodEffect(itemTemplate.Property4, itemTemplate.Property5).Start(this);
						break;
					case 8://Dẫn dắt
						new FatalEffect(itemTemplate.Property4, itemTemplate.Property5).Start(this);
						break;
					case 9://Hàn băng.
						new IceFronzeEquipEffect(itemTemplate.Property4, itemTemplate.Property5).Start(this);
						break;
					case 10://Miễn kháng.
						new NoHoleEquipEffect(itemTemplate.Property4, itemTemplate.Property5).Start(this);
						break;
					case 11://Hạt nhân
						new AtomBombEquipEffect(itemTemplate.Property4, itemTemplate.Property5).Start(this);
						break;
					case 12://Châu báu xuyên giáp.
						new ArmorPiercerEquipEffect(itemTemplate.Property4, itemTemplate.Property5).Start(this);
						break;
					case 13://Giảm thương.
						new AvoidDamageEffect(itemTemplate.Property4, itemTemplate.Property5).Start(this);
						break;
					case 14://Bạo kích.
						new MakeCriticalEffect(itemTemplate.Property4, itemTemplate.Property5).Start(this);
						break;
					case 15://Hấp thụ.
						new AssimilateDamageEffect(itemTemplate.Property4, itemTemplate.Property5).Start(this);
						break;
					case 16://Hút máu.
						new AssimilateBloodEffect(itemTemplate.Property4, itemTemplate.Property5).Start(this);
						break;
					case 17://Phong ấn.
						new SealEquipEffect(itemTemplate.Property4, itemTemplate.Property5).Start(this);
						break;
					case 18://Thần tốc.
						new AddTurnEquipEffect(itemTemplate.Property4, itemTemplate.Property5, itemTemplate.TemplateID).Start(this);
						break;
					case 19://Phẫn nộ.
						new AddDanderEquipEffect(itemTemplate.Property4, itemTemplate.Property5).Start(this);
						break;
					case 20://Phản xạ.
						new ReflexDamageEquipEffect(itemTemplate.Property4, itemTemplate.Property5).Start(this);
						break;
					case 21://Mệt.
						new ReduceStrengthEquipEffect(itemTemplate.Property4, itemTemplate.Property5).Start(this);
						break;
					case 22://Bỏng.
						new ContinueReduceBloodEquipEffect(itemTemplate.Property4, itemTemplate.Property5).Start(this);
						break;
					case 23://Xích.
						new LockDirectionEquipEffect(itemTemplate.Property4, itemTemplate.Property5).Start(this);
						break;
					case 24://Liên kích.
						new AddBombEquipEffect(itemTemplate.Property4, itemTemplate.Property5).Start(this);
						break;
					case 25://Nhược hóa.
						new ContinueReduceDamageEquipEffect(itemTemplate.Property4, itemTemplate.Property5).Start(this);
						break;
					case 26://Hồi phục
						new RecoverBloodEffect(itemTemplate.Property4, itemTemplate.Property5).Start(this);
						break;
					default:
						break;
						#region OLD Rune
						//case 1:
						//	new AddAttackEffect(itemTemplate.Property4, itemTemplate.Property5).Start(this);
						//	break;
						//case 2:
						//	new AddDefenceEffect(itemTemplate.Property4, itemTemplate.Property5).Start(this);
						//	break;
						//case 3:
						//	new AddAgilityEffect(itemTemplate.Property4, itemTemplate.Property5).Start(this);
						//	break;
						//case 4:
						//	new AddLuckyEffect(itemTemplate.Property4, itemTemplate.Property5).Start(this);
						//	break;
						//case 5:
						//	new AddDamageEffect(itemTemplate.Property4, itemTemplate.Property5).Start(this);
						//	break;
						//case 6:
						//	new ReduceDamageEffect(itemTemplate.Property4, itemTemplate.Property5).Start(this);
						//	break;
						//case 7:
						//	new AddBloodEffect(itemTemplate.Property4, itemTemplate.Property5).Start(this);
						//	break;
						//case 8:
						//	new FatalEffect(itemTemplate.Property4, itemTemplate.Property5).Start(this);
						//	break;
						//case 9:
						//	new IceFronzeEquipEffect(itemTemplate.Property4, itemTemplate.Property5).Start(this);
						//	break;
						//case 10:
						//	new NoHoleEquipEffect(itemTemplate.Property4, itemTemplate.Property5).Start(this);
						//	break;
						//case 11:
						//	new AtomBombEquipEffect(itemTemplate.Property4, itemTemplate.Property5).Start(this);
						//	break;
						//case 12:
						//	new ArmorPiercerEquipEffect(itemTemplate.Property4, itemTemplate.Property5).Start(this);
						//	break;
						//case 13:
						//	new AvoidDamageEffect(itemTemplate.Property4, itemTemplate.Property5).Start(this);
						//	break;
						//case 14:
						//	new MakeCriticalEffect(itemTemplate.Property4, itemTemplate.Property5).Start(this);
						//	break;
						//case 15:
						//	new AssimilateDamageEffect(itemTemplate.Property4, itemTemplate.Property5).Start(this);
						//	break;
						//case 16:
						//	new AssimilateBloodEffect(itemTemplate.Property4, itemTemplate.Property5).Start(this);
						//	break;
						//case 17:
						//	new SealEquipEffect(itemTemplate.Property4, itemTemplate.Property5).Start(this);
						//	break;
						//case 18:
						//	new AddTurnEquipEffect(itemTemplate.Property4, itemTemplate.Property5, itemTemplate.TemplateID).Start(this);
						//	break;
						//case 19:
						//	new AddDanderEquipEffect(itemTemplate.Property4, itemTemplate.Property5).Start(this);
						//	break;
						//case 20:
						//	new ReflexDamageEquipEffect(itemTemplate.Property4, itemTemplate.Property5).Start(this);
						//	break;
						//case 21:
						//	new ReduceStrengthEquipEffect(itemTemplate.Property4, itemTemplate.Property5).Start(this);
						//	break;
						//case 22:
						//	new ContinueReduceBloodEquipEffect(itemTemplate.Property4, itemTemplate.Property5).Start(this);
						//	break;
						//case 23:
						//	new LockDirectionEquipEffect(itemTemplate.Property4, itemTemplate.Property5).Start(this);
						//	break;
						//case 24:
						//	new AddBombEquipEffect(itemTemplate.Property4, itemTemplate.Property5).Start(this);
						//	break;
						//case 25:
						//	new ContinueReduceDamageEquipEffect(itemTemplate.Property4, itemTemplate.Property5).Start(this);
						//	break;
						//case 26:
						//	new RecoverBloodEffect(itemTemplate.Property4, itemTemplate.Property5).Start(this);
						//	break;
						#endregion
				}
            }
		}

		public void InitPetSkillEffect(UsersPetInfo pets)
		{
			string[] array = pets.SkillEquip.Split('|');
			PetSkillInfo skillInfo = null;
			foreach (string skill in array)
			{
				int skillId = int.Parse(skill.Split(',')[0]);
				skillInfo = PetMgr.FindPetSkill(skillId);
				if (skillInfo == null)
					continue;
				string[] elementIDs = skillInfo.ElementIDs.Split(',');
				int coldDown = skillInfo.ColdDown;
				int probability = skillInfo.Probability;
				int delay = skillInfo.Delay;
				int gameType = skillInfo.GameType;
				if (!_petSkillCd.ContainsKey(skillId))
				{
					_petSkillCd.Add(skillId, skillInfo);
				}
				foreach (string element in elementIDs)
				{
					if (string.IsNullOrEmpty(element))
						continue;
					switch (element)
					{
						case "1133"://Sát thương tăng 120%
							new AE1133(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1039"://Sát thương tăng 150%
							new AE1039(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1134"://Sát thương tăng 180%
							new AE1134(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1046"://Tăng phòng ngự.100
							new AE1046(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1047"://Tăng phòng ngự.300
							new AE1047(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1048"://Tăng phòng ngự.500
							new AE1048(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1138"://100% xác suất bạo kích
							new AE1138(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1135"://Giảm 500 điểm sát thương chịu được
							new AE1135(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1032"://Càng Đánh Càng Mạnh Lv1
							new AE1032(coldDown, probability, gameType, skillId, delay, "1029").Start(this);
							break;
						case "1033"://Càng Đánh Càng Mạnh Lv2
							new AE1033(coldDown, probability, gameType, skillId, delay, "1030").Start(this);
							break;
						case "1034"://Càng Đánh Càng Mạnh Lv3
							new AE1034(coldDown, probability, gameType, skillId, delay, "1031").Start(this);
							break;
						case "1067"://Gai Kiến Lv1
							new AE1067(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1068"://Gai Kiến Lv2
							new AE1068(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1120"://Kén Trùng Lv1
							new PE1120(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1121"://Kén Trùng Lv2
							new PE1121(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1113"://Phản 300 điểm sát thương
							new PE1113(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1114"://Phản 500 điểm sát thương
							new PE1114(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1115"://Hồi phục 500 điểm HP
							new AE1115(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1116"://Hồi phục 1000 điểm HP
							new AE1116(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1117"://Tiếp Cận
							new AE1117(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1106"://Nhận được 1 điểm ma pháp
							new PE1106(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1043"://Hồi phục chậm Lv1
							new AE1043(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1044"://Hồi phục chậm Lv2
							new AE1044(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1045"://Hồi phục chậm Lv3
							new AE1045(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1026"://Hoa Nở Lv1
							new AE1026(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1027"://Hoa Nở Lv2
							new AE1027(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1028"://Hoa Nở Lv3
							new AE1028(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1058"://Hồi phục 1000 HP
							new AE1058(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1059"://Hồi phục 2000 HP
							new AE1059(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1063"://Bách Luân Nở Hoa Lv1
							new AE1063(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1064"://Bách Luân Nở Hoa Lv2
							new AE1064(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1118"://Tăng Tốc Sinh Trưởng Lv1
						case "1119"://Tăng Tốc Sinh Trưởng Lv2
							new AE1118(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1080"://Hồi phục 300 điểm HP
							new PE1080(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1081"://Hồi phục 600 điểm HP
							new PE1081(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1104"://Pháo Bay Lượn*800
							new AE1104(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1105"://Pháo Bay Lượn*1000
							new AE1105(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1108"://Nhận được 3 điểm ma pháp
							new PE1108(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1125":// gây 100% sát thương cơ bản
							new AE1125(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1126":// gây 120% sát thương cơ bản
							new AE1126(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1127":// gây 140% sát thương cơ bản
							new AE1127(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1038"://hiệu ứng dẫn đường, duy trì 1 turn.
							new AE1038(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1017"://Di chuyển không thể. Duy trì 2 TURN
							new AE1017(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1018"://Bất Động Lv1
							new AE1018(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1019"://Bất Động Lv2
							new AE1019(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1020"://Bất Động Lv3
							new AE1020(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1132"://Công kích giảm 100
							new AE1132(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1021": //miễn kháng. Duy trì 2 TURN
							new AE1021(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1049"://Tăng hộ giáp, hiệu quả sẽ biến mất khi di chuyển.
							new AE1049(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1050"://Tăng hộ giáp, hiệu quả sẽ biến mất khi di chuyển.
							new AE1050(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1055"://Hiệu quả giải trừ.
							new AE1053(coldDown, probability, gameType, skillId, delay, "1053").Start(this);
							break;
						case "1070"://Hiệu quả giải trừ 1072, 1073
							//new AE1071(coldDown, probability, gameType, skillId, delay, "1071").Start(this);
							break;
						case "1072"://Hỏa Lực Cao Lv1
							new PE1072(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1073"://Hỏa Lực Cao Lv2
							new PE1073(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1076"://Thú cưng gây 200 sát thương xung quanh
							new PE1076(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1077"://Thú cưng gây 400 sát thương xung quanh
							new PE1077(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1082"://Luôn miễn kháng
							new AE1082(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1083"://Pháo Đài V3 Lv1
							new AE1083(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1084"://Pháo Đài V3 Lv2
							new AE1084(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1085"://May mắn tăng 500
							new AE1085(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1086"://May mắn tăng 800
							new AE1086(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1087"://Phòng ngự giảm 1000
							new AE1087(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1088"://Phòng ngự giảm 800
							new AE1088(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1089"://Giải trừ hiệu quả
							new AE1090(coldDown, probability, gameType, skillId, delay, "1090").Start(this);
							new AE1091(coldDown, probability, gameType, skillId, delay, "1091").Start(this);
							break;
						case "1040"://tăng 100đ may mắn cho bản thân, duy trì 2 turn.
							new AE1040(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1041"://tăng 300đ may mắn cho bản thân, duy trì 2 turn.
							new AE1041(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1042"://tăng 500đ may mắn cho bản thân, duy trì 2 turn.
							new AE1042(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1022"://Bắn bất kỳ loại đạn nào trong TURN cũng sẽ tăng 100 hộ giáp cho đồng đội, duy trì 2 TURN.
							new AE1022(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1023"://Bắn bất kỳ loại đạn nào trong TURN cũng sẽ tăng 300 hộ giáp cho đồng đội, duy trì 2 TURN.
							new AE1023(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1024"://Bắn bất kỳ loại đạn nào trong TURN cũng sẽ tăng 100 sát thương cho đồng đội, duy trì 2 TURN.
							new AE1024(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1025"://Bắn bất kỳ loại đạn nào trong TURN cũng sẽ tăng 300 sát thương cho đồng đội, duy trì 2 TURN.
							new AE1025(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1056"://Hồi phục 1500 HP cho tất cả đồng đội trên toàn bản đồ.
							new AE1056(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1057"://Hồi phục 3000 HP cho tất cả đồng đội trên toàn bản đồ.
							new AE1057(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1074"://Tăng 300 điểm hiệu quả cho các vũ khí phụ loại thiên sứ ban phúc.
							new PE1074(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1075"://Tăng 600 điểm hiệu quả cho các vũ khí phụ loại thiên sứ ban phúc.
							new PE1075(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1078"://Sử dụng vũ khí phụ loại khiên sẽ lập tức hồi phục 500 HP.
							new PE1078(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1079"://Sử dụng vũ khí phụ loại khiên sẽ lập tức hồi phục 1000 HP.
							new PE1079(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1092"://Tăng 100 công kích cho tất cả chiến hữu. Duy trì 3 TURN.
							new AE1092(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1093"://Tăng 300 công kích cho tất cả chiến hữu. Duy trì 3 TURN.
							new AE1093(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1094"://Tăng 100 phòng ngự cho tất cả chiến hữu. Duy trì 3 TURN.
							new AE1094(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1095"://Tăng 300 phòng ngự cho tất cả chiến hữu. Duy trì 3 TURN.
							new AE1095(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1096"://Tăng 100 nhanh nhẹn cho tất cả chiến hữu. Duy trì 3 TURN.
							new AE1096(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1097"://Tăng 300 nhanh nhẹn cho tất cả chiến hữu. Duy trì 3 TURN.
							new AE1097(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1098"://Tăng 100 may mắn cho tất cả chiến hữu. Duy trì 3 TURN.
							new AE1098(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1099"://Tăng 300 may mắn cho tất cả chiến hữu. Duy trì 3 TURN.
							new AE1099(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1100": //Tăng 1000 HP tối đa cho tất cả chiến hữu. Duy trì 3 TURN.
							new AE1100(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1101"://Tăng 2000 HP tối đa cho tất cả chiến hữu. Duy trì 3 TURN.
							new AE1101(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1107"://TURN đầu tiên sẽ nhận được 50 ma pháp.
							new PE1107(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1109"://Giải trừ 50 điểm phép thuật
						case "1110":
							new PE1110(coldDown, probability, gameType, skillId, delay, "1110").Start(this);
							break;
						case "1178"://tăng 100đ tấn công cho bản thân, duy trì 2 turn.
							new AE1178(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1179"://tăng 300đ tấn công cho bản thân, duy trì 2 turn.
							new AE1179(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1180"://tăng 500đ tấn công cho bản thân, duy trì 2 turn.
							new AE1180(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1181"://Tăng 100 điểm hộ giáp cho tất cả đồng đội, duy trì 2 turn.Không cộng dồn để dùng.
							new AE1181(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1182"://Tăng 200 điểm hộ giáp cho tất cả đồng đội, duy trì 2 turn.Không cộng dồn để dùng.
							new AE1182(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1183"://Tăng 300 điểm hộ giáp cho tất cả đồng đội, duy trì 2 turn.Không cộng dồn để dùng.
							new AE1183(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1184"://tăng 150 điểm sát thương, di chuyển sẽ hủy. Khi HP không đủ, dùng kỹ năng này sẽ tử vong.
							new AE1184(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1185"://tăng 300 điểm sát thương, di chuyển sẽ hủy. Khi HP không đủ, dùng kỹ năng này sẽ tử vong.
							new AE1185(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1186"://Mỗi turn giảm 500 HP, di chuyển sẽ hủy. Khi HP không đủ, dùng kỹ năng này sẽ tử vong.
							new AE1186(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1187"://Mỗi turn giảm 800 HP, di chuyển sẽ hủy. Khi HP không đủ, dùng kỹ năng này sẽ tử vong.
							new AE1187(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1188":
						case "1189"://Xóa hiệu ứng Địa Ngục Băng Giá
							new AE1189(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1224"://Mỗi turn giảm 500 HP.
							new AE1224(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1225"://Mỗi turn giảm 800 HP.
							new AE1225(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1190"://Tăng nhanh nhẹn 300 điểm.
							new PE1190(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1191"://Tăng nhanh nhẹn 500 điểm.
							new PE1191(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1192"://Tăng 100 điểm tấn công cho toàn bộ đồng đội.
							new PE1192(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1193"://Tăng 300 điểm tấn công cho toàn bộ đồng đội.
							new PE1193(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1194"://Tăng 200 điểm tấn công, duy trì 1 turn.
							new AE1194(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1195"://Tăng 300 điểm tấn công, duy trì 1 turn.
							new AE1195(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1196"://100 sát thương, duy trì 1 turn.
							new AE1196(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1197"://150 sát thương, duy trì 1 turn.
							new AE1197(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1198"://Tăng 30% crit, duy trì 1 turn.
							new AE1198(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1199"://Tăng 50% crit, duy trì 1 turn.
							new AE1199(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1200"://Khi đến lượt thi triển, tăng 2 điểm ma pháp cho toàn bộ thú cưng cùng phe.
							new PE1200(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1150"://mục tiêu bị đánh trúng mỗi turn mất 1% HP, duy trì 3 turn. (Chỉ có hiệu quả khi chiến đấu)
							new AE1150(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1151"://mục tiêu bị đánh trúng mỗi turn mất 2% HP, duy trì 3 turn. (Chỉ có hiệu quả khi chiến đấu)
							new AE1151(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1152"://mục tiêu bị đánh trúng mỗi turn mất 3% HP, duy trì 3 turn. (Chỉ có hiệu quả khi chiến đấu)
							new AE1152(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1153"://Sát thương cơ bản +15%, duy trì 3 turn.
							new AE1153(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1154"://Sát thương cơ bản +25%, duy trì 3 turn.
							new AE1154(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1155"://hộ giáp giảm 500 điểm, duy trì 3 turn.
							new AE1155(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1156"://hộ giáp giảm 650 điểm, duy trì 3 turn.
							new AE1156(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1172"
							: //Mỗi lần bị tấn công, có xác suất 50% thức tỉnh Hồn Rồng hồi phục 2% HP. Duy trì 3 turn.
							new AE1172(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1173"
							: //Mỗi lần bị tấn công, có xác suất 50% thức tỉnh Hồn Rồng hồi phục 4% HP. Duy trì 3 turn.
							new AE1173(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1174": //Rồng Bảo Vệ Lv1. Duy trì 3 turn.
							new AE1174(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1175": //Rồng Bảo Vệ Lv2. Duy trì 3 turn.
							new AE1175(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1170"
							: //Mỗi lần tới lượt địch tấn công, địch sẽ chịu bỏng Ấn Rồng Lửa! Mất 1000 HP, chỉ có hiệu quả khi chiến đấu. Duy trì 3 turn.
							new AE1170(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1171"
							: //Mỗi lần tới lượt địch tấn công, địch sẽ chịu bỏng Ấn Rồng Lửa! Mất 2000 HP chỉ có hiệu quả khi chiến đấu. Duy trì 3 turn.
							new AE1171(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1176"
							: //Mỗi lần tới lượt địch tấn công, địch sẽ chịu bỏng Ấn Rồng Lửa! Mất 2% HP hiện tại, chỉ có hiệu quả khi chiến đấu. Duy trì 3 turn.
							new AE1176(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1177"
							: //Mỗi lần tới lượt địch tấn công, địch sẽ chịu bỏng Ấn Rồng Lửa! Mất 4% HP hiện tại, chỉ có hiệu quả khi chiến đấu. Duy trì 3 turn.
							new AE1177(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1163": //Tấn công tăng 150.
							new PE1163(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1164": //Tấn công tăng 300..
							new PE1164(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1165": //sát thương tăng 100.
							new PE1165(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1166": //sát thương tăng 200.
							new PE1166(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1161"
							: //Chân Long Tại Thiên, gây cho tất cả địch 3000 sát thương. (Chỉ có hiệu quả khi chiến đấu).
							new AE1161(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1162"
							: //Chân Long Tại Thiên, gây cho tất cả địch 5000 sát thương. (Chỉ có hiệu quả khi chiến đấu).
							new AE1162(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1238": //Thi triển Xung kích tăng 45 điểm s.thương bạn thân,kéo dài 2 hiệp.
							new AE1238(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1239": //Thi triển Xung kích tăng 75 điểm s.thương bạn thân,kéo dài 2 hiệp.
							new AE1239(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1240": //Thi triển Xung kích tăng 115 điểm s.thương bạn thân,kéo dài 2 hiệp.
							new AE1240(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1231": //Nhận 500 điểm giảm thương
                            new AE1231(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1246": //Nhận 650 điểm giảm thương
							new AE1246(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1247": //Nhận 750 điểm giảm thương
							new AE1247(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1232":
						case "1248": //mỗi lần bị tấn công giảm thêm 200 điểm s.thương,kéo dài 1 hiệp
							new AE1248(coldDown, probability, gameType, skillId, delay, "1248").Start(this);
							break;
						case "1233":
						case "1249": //mỗi lần bị tấn công giảm thêm 250 điểm s.thương,kéo dài 1 hiệp
							new AE1249(coldDown, probability, gameType, skillId, delay, "1249").Start(this);
							break;
						case "1234":
						case "1250": //mỗi lần bị tấn công giảm thêm 350 điểm s.thương,kéo dài 1 hiệp
							new AE1250(coldDown, probability, gameType, skillId, delay, "1250").Start(this);
							break;
						case "1228": //Mỗi lần bị tấn công,phản đòn 600 điểm sát thương,kéo dài 2 hiệp
							new AE1228(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1229": //Mỗi lần bị tấn công,phản đòn 1200 điểm sát thương,kéo dài 2 hiệp
							new AE1229(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1230": //Duy trì miêu trảo, kéo dài 2 hiệp
							new AE1230(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1241": //Mở che chắn,tăng 90 điểm hộ giáp
							new PE1241(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1242": //Mở che chắn,tăng 130 điểm hộ giáp
							new PE1242(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1235": //Phản đòn
							new PE1235(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1236": //Mỗi lần bị tấn công có 35% xác suất phản đòn 500 điểm sát thương
							new PE1236(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1237": //Mỗi lần bị tấn công có 45% xác suất phản đòn 800 điểm sát thương
							new PE1237(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1253": //Mỗi lần bị tấn công nhận được 2 điểm ma pháp.
							new PE1253(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1243": //hồi phục 1500 HP,kéo dài 1 hiệp,chỉ khi đối chiến vối người mới có hiệu lực
							new AE1243(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1244": //hồi phục 2500 HP,kéo dài 1 hiệp,chỉ khi đối chiến vối người mới có hiệu lực
							new AE1244(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1201": //Bắn ra nọc độc, mục tiêu trúng phải giảm 100 sát thương, duy trì 3 turn.
							new AE1201(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1202": //Bắn ra nọc độc, mục tiêu trúng phải giảm 200 sát thương, duy trì 3 turn.
							new AE1202(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1203": //Bắn ra nọc độc, mục tiêu trúng phải giảm 300 sát thương, duy trì 3 turn.
							new AE1203(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1204"
							: //Giảm 300 tấn công toàn bộ phe địch, duy trì 2 turn. Kỹ năng chỉ hiệu quả trong chiến đấu.
							new AE1204(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1205"
							: //Giảm 500 tấn công toàn bộ phe địch, duy trì 2 turn. Kỹ năng chỉ hiệu quả trong chiến đấu.
							new AE1205(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1206"
							: //Giảm 300 phòng thủ toàn bộ phe địch, duy trì 2 turn. Kỹ năng chỉ hiệu quả trong chiến đấu.
							new AE1206(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1207"
							: //Giảm 500 phòng thủ toàn bộ phe địch, duy trì 2 turn. Kỹ năng chỉ hiệu quả trong chiến đấu.
							new AE1207(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1208"
							: //Giảm 10 điểm ma pháp của tất cả thú cưng bên địch. Kỹ năng chỉ hiệu quả trong chiến đấu.
							new AE1208(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1209"
							: //Giảm 30 điểm ma pháp của tất cả thú cưng bên địch. Kỹ năng chỉ hiệu quả trong chiến đấu.
							new AE1209(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1210": //Mỗi turn giảm 500 HP. Duy trì 3 turn.
							new AE1210(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1211": //Mỗi turn giảm 1000 HP. Duy trì 3 turn.
							new AE1211(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1226": //Mỗi turn giảm 500 HP.
							new AE1226(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1227": //Mỗi turn giảm 1000 HP.
							new AE1227(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1212": //tăng 20% bạo kích. Duy trì 3 turn.
							new AE1212(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1213": //tăng 50% bạo kích. Duy trì 3 turn.
							new AE1213(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1214": //Tăng 100 hộ giáp
							new PE1214(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1215": //Tăng 200 hộ giáp
							new PE1215(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1216": //Tăng 1500 HP tối đa.
							new PE1216(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1217": //Tăng 3000 HP tối đa.
							new PE1217(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1218":
						case "1219":
							//empty skill element.
							break;
						case "1222": //Không thể di chuyển, duy trì 2 turn.
							new AE1222(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1220": //Giảm 100 hộ giáp tất cả phe địch, duy trì 2 turn.
							new AE1220(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1221": //Giảm 200 hộ giáp tất cả phe địch, duy trì 2 turn.
							new AE1221(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1223": //Mỗi lần bị tấn công nhận được 2 điểm ma pháp.
							new PE1223(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1254": //mục tiêu bị trúng giảm 100 tấn công, duy trì 3 Turn.AE
							new AE1254(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1255": //mục tiêu bị trúng giảm 200 tấn công, duy trì 3 Turn.AE
							new AE1255(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1256": //mục tiêu bị trúng giảm 300 tấn công, duy trì 3 Turn.AE
							new AE1256(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1257": //Tất cả đồng đội tăng 100 hộ giáp. Duy trì 2 Turn, chỉ có hiệu quả khi chiến đấu.
							new AE1257(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1258": //Tất cả đồng đội tăng 200 hộ giáp. Duy trì 2 Turn, chỉ có hiệu quả khi chiến đấu.
							new AE1258(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1259": //mỗi Turn hồi 800 điểm HP. Duy trì 2 Turn, chỉ có hiệu quả khi chiến đấu.
							new AE1259(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1260": //mỗi Turn hồi 1000 điểm HP. Duy trì 2 Turn, chỉ có hiệu quả khi chiến đấu.
							new AE1260(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1266": //Giảm 10 điểm ma pháp Pet địch bị bắn trúng. Chỉ có hiệu quả khi chiến đấu.
							new AE1266(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1267": //Giảm 20 điểm ma pháp Pet địch bị bắn trúng. Chỉ có hiệu quả khi chiến đấu.
							new AE1267(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1268": //tăng 5 điểm ma pháp Pet đồng đội. Chỉ có hiệu quả khi chiến đấu.
							new AE1268(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1269": //tăng 10 điểm ma pháp Pet đồng đội. Chỉ có hiệu quả khi chiến đấu.
							new AE1269(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1261": //May mắn +300.
							new PE1261(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1262": //May mắn +500.
							new PE1262(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1264": //Tất cả đồng đội +100 sát thương.
							new PE1264(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1265": //Tất cả đồng đội +300 sát thương.
							new PE1265(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1270": //mỗi turn gây 800 sát thương người chơi bị bắn trúng, duy trì 2 turn.
							new AE1270(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1271": //mỗi turn gây 1300 sát thương người chơi bị bắn trúng, duy trì 2 turn.
							new AE1271(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1274"://hạt nhân
							new PE1274(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
						case "1263": //Mỗi lần bị tấn công nhận được 2 điểm ma pháp.
							new PE1263(coldDown, probability, gameType, skillId, delay, element).Start(this);
							break;
				case "2977": //Ánh Lửa Bập Bùng - Sát Thương, Hộ Giáp, Ma Công, Ma Kháng tăng 5%
					new PE2977(coldDown, probability, gameType, skillId, delay, element).Start(this);
					break;
				case "2978": //Ánh Lửa Bập Bùng - Hộ Giáp +5%
					new PE2978(coldDown, probability, gameType, skillId, delay, element).Start(this);
					break;
				case "2979": //Ánh Lửa Bập Bùng - Ma Công +5%
					new PE2979(coldDown, probability, gameType, skillId, delay, element).Start(this);
					break;
				case "2980": //Ánh Lửa Bập Bùng - Ma Kháng +5%
					new PE2980(coldDown, probability, gameType, skillId, delay, element).Start(this);
					break;
				case "2981": //Phượng Hoàng Niết Bàn - Khi chịu đến chí mạng sẽ kích hoạt
					new PE2981(coldDown, probability, gameType, skillId, delay, element).Start(this);
					break;
						default:
							Console.WriteLine($"Not Found SkillID : {skillId}.");
							break;
					}
				}
			}
		}

		public void InitFightBuffer(List<BufferInfo> buffers)
		{
			foreach (BufferInfo info in buffers)
			{
				switch (info.Type)
				{
					case 101:
						base.FightBuffers.ConsortionAddBloodGunCount = info.Value;
						break;
					case 102:
						base.FightBuffers.ConsortionAddDamage = info.Value;
						break;
					case 103:
						base.FightBuffers.ConsortionAddCritical = info.Value;
						break;
					case 104:
						base.FightBuffers.ConsortionAddMaxBlood = info.Value;
						break;
					case 105:
						base.FightBuffers.ConsortionAddProperty = info.Value;
						break;
					case 106:
						base.FightBuffers.ConsortionReduceEnergyUse = info.Value;
						break;
					case 107:
						base.FightBuffers.ConsortionAddEnergy = info.Value;
						break;
					case 108:
						base.FightBuffers.ConsortionAddEffectTurn = info.Value;
						break;
					case 109:
						base.FightBuffers.ConsortionAddOfferRate = info.Value;
						break;
					case 110:
						base.FightBuffers.ConsortionAddPercentGoldOrGP = info.Value;
						break;
					case 111:
						base.FightBuffers.ConsortionAddSpellCount = info.Value;
						break;
					case 112:
						base.FightBuffers.ConsortionReduceDander = info.Value;
						break;
					case (int)BuffType.WorldBossHP:
						FightBuffers.WorldBossHP = info.Value;
						break;
					case (int)BuffType.WorldBossAttrack:
						FightBuffers.WorldBossAttrack = info.Value;
						break;
					case (int)BuffType.WorldBossHP_MoneyBuff:
						FightBuffers.WorldBossHP_MoneyBuff = info.Value;
						break;
					case (int)BuffType.WorldBossAttrack_MoneyBuff:
						FightBuffers.WorldBossAttrack_MoneyBuff = info.Value;
						break;
					case (int)BuffType.WorldBossMetalSlug:
						FightBuffers.WorldBossMetalSlug = info.Value;
						break;
					case (int)BuffType.WorldBossAncientBlessings:
						FightBuffers.WorldBossAncientBlessings = info.Value;
						break;
					case (int)BuffType.WorldBossAddDamage:
						FightBuffers.WorldBossAddDamage = info.Value;
						break;
					default:
						break;
				}
			}
		}
		#region card buff
		private bool CheckCondition(int condition)
		{
			if (Game is PVEGame)
			{
				PVEGame pve = Game as PVEGame;
				if (pve.Info.ID == 1 && (condition == 2 || condition == 3))
					return true;
				if (pve.Info.ID == 2 && condition == 8)
					return true;
				if (pve.Info.ID == 3 && (condition == 9 || condition == 10))
					return true;
				if (pve.Info.ID == 4 && (condition == 4 || condition == 5))
					return true;
				if (pve.Info.ID == 5 && (condition == 11 || condition == 12 || condition == 13))
					return true;
				if (pve.Info.ID == 6 && (condition == 5 || condition == 16 || condition == 17))
					return true;
				if (pve.Info.ID == 7 && (condition == 14 || condition == 15))
					return true;
				if (pve.Info.ID == 11 && (condition == 7 || condition == 20))
					return true;
				if (pve.Info.ID == 13 && (condition == 8 || condition == 21))
					return true;
			}
			return Game is PVPGame;
		}
		public void InitCardBuffer(List<int> cards)
		{
			//lấy minlv card
			int minLv = 30;
			foreach (int card in cards)
			{
				if (card < 1100)
					minLv = card - 1000;
			}
			//xác định vị trí value
			int indexVal = 0;
			if (minLv >= 10)
				indexVal = 1;
			if (minLv >= 20)
				indexVal = 2;
			if (minLv >= 30)
				indexVal = 3;

			//kiểm tra bộ card
			Dictionary<int, List<CardGroupInfo>> groups = CardBuffMgr.GetAllCard();
			List<CardBuffInfo> buffs = new List<CardBuffInfo>();
			int counter = 0;
			string msg = string.Empty;
			List<CardBuffInfo> ListArr = new List<CardBuffInfo>();

			foreach (int key in groups.Keys)
			{
				counter = 0;
				foreach (var card in groups[key])
				{
					foreach (int id in cards)
					{
						if (id == card.TemplateID)
							counter++;
					}
				}
				buffs = CardBuffMgr.FindCardBuffs(key);
				if (buffs == null)
				{
					continue;
				}
				buffs.Reverse();
				foreach (var buff in buffs)
				{
					if (counter >= buff.Condition)
					{
						CardInfo cardNotice = CardBuffMgr.FindCard(key);
						if (cardNotice != null && CheckCondition(buff.PropertiesDscripID))
						{
							msg = "Kích hoạt <" + cardNotice.Name + "> hiệu ứng " + buff.Condition + " thẻ!";
							if (buff != null)
								ListArr.Add(buff);
						}
					}
				}
			}
			bool isActivated = false;
			foreach (var item in ListArr)
			{
				switch(item.CardID)
                {
					case 1:
						{
							new AntCaveEffect(indexVal, item).Start(this);
							isActivated = true;
						}
						break;
					case 2:
						if (item.Condition >= 2)
                        {
							new GuluKingdom2Effect(indexVal, item).Start(this);
							isActivated = true;
						}
						if (item.Condition >= 4)
                        {
							new GuluKingdom4Effect(indexVal, item).Start(this);
							isActivated = true;
						}
						break;
					case 3:
						if (item.Condition >= 3)
                        {
							new EvilTribe3Effect(indexVal, item).Start(this);
							isActivated = true;
                        }
						if (item.Condition >= 5)
						{
							new EvilTribe5Effect(indexVal, item).Start(this);
							isActivated = true;
						}
						break;
					case 4:
						if (item.Condition >= 2)
                        {
							new ShadowDevil2Effect(indexVal, item).Start(this);
							isActivated = true;
                        }
						if (item.Condition >= 4)
                        {
							new ShadowDevil4Effect(indexVal, item).Start(this);
							isActivated = true;
                        }
						break;
					case 5:
						if (item.Condition >= 2)
                        {
							new FourArtifacts2Effect(indexVal, item).Start(this);
							isActivated = true;
                        }
						if (item.Condition >= 4)
                        {
							new FourArtifacts4Effect(indexVal, item).Start(this);
							isActivated = true;
                        }
						break;
					case 6:
						if (item.Condition >= 2)
                        {
							new Goblin2Effect(indexVal, item).Start(this);
							isActivated = true;
                        }
						if (item.Condition >= 4)
						{
							new Goblin4Effect(indexVal, item).Start(this);
							isActivated = true;
						}
						if (item.Condition >= 5)
						{
							new Goblin5Effect(indexVal, item).Start(this);
							isActivated = true;
						}
						break;
					case 7:
						if (item.Condition >= 2)
                        {
							new RunRunChicken2Effect(indexVal, item).Start(this);
							isActivated = true;
                        }
						if (item.Condition >= 4)
                        {
							new RunRunChicken4Effect(indexVal, item).Start(this);
							isActivated = true;
                        }
						break;
					case 8:
						if (item.Condition >= 2)
						{
							new GuluSportsMeeting2Effect(indexVal, item).Start(this);
							isActivated = true;
						}
						if (item.Condition >= 4)
                        {
							new GuluSportsMeeting4Effect(indexVal, item).Start(this);
							isActivated = true;
                        }
						if (item.Condition >= 5)
                        {
							new GuluSportsMeeting5Effect(indexVal, item).Start(this);
							isActivated = true;
                        }
						break;
					case 9:
						if(item.Condition >= 2)
                        {
							new FiveGodSoldier2Effect(indexVal, item).Start(this);
							isActivated = true;
                        }
						if(item.Condition >= 5)
                        {
							new FiveGodSoldier5Effect(indexVal, item).Start(this);
							isActivated = true;
                        }
						break;
					case 10:
						if (item.Condition >= 3)
						{
							new TimeVortex3Effect(indexVal, item).Start(this);
							isActivated = true;
						}
						if (item.Condition >= 5)
						{
							new TimeVortex5Effect(indexVal, item).Start(this);
							isActivated = true;
						}
						break;
					case 11:
						if (item.Condition >= 3)
                        {
							new WarriorsArena3Effect(indexVal, item).Start(this);
							isActivated = true;
                        }
						if (item.Condition >= 5)
                        {
							new WarriorsArena5Effect(indexVal, item).Start(this);
							isActivated = true;
                        }
						break;
					case 12:
                        {
							new PioneerEffect(indexVal, item).Start(this);
							isActivated = true;
                        }
						break;
					case 13:
                        {
							new WeaponMasterEffect(indexVal, item).Start(this);
							isActivated = true;
                        }
						break;
					case 14:
                        {
							new DivineEffect(indexVal, item).Start(this);
							isActivated = true;
                        }
						break;
					case 15:
                        {
							new LuckyEffect(indexVal, item).Start(this);
							isActivated = true;
                        }
						break;
					default:
						Console.WriteLine("CardBuffer --- CardID Not Found");
						break;
				}
			}
			if (Game is PVEGame && isActivated)
				m_player.SendMessage(msg);
            #region OLD CardBuff
            //if (finalBuff != null)
            //{
            //	switch (finalBuff.CardID)
            //	{
            //		case 1:
            //			new AntCaveEffect(indexVal, finalBuff).Start(this);
            //			if (Game is PVEGame)
            //				m_player.SendMessage(msg);
            //			break;
            //		case 2:
            //			if (finalBuff.Condition >= 4 && buffs != null)
            //			{
            //				foreach (var buff in buffs)
            //				{
            //					if (buff.Condition >= 4)
            //						new GuluKingdom4Effect(indexVal, buff).Start(this);
            //					if (buff.Condition >= 2)
            //						new GuluKingdom2Effect(indexVal, buff).Start(this);
            //				}
            //			}
            //			if (finalBuff.Condition >= 2)
            //			{
            //				new GuluKingdom2Effect(indexVal, finalBuff).Start(this);
            //			}
            //			if (Game is PVEGame)
            //				m_player.SendMessage(msg);
            //			break;
            //		case 3:
            //			if (finalBuff.Condition >= 5 && buffs != null)
            //			{
            //				foreach (var buff in buffs)
            //				{
            //					if (buff.Condition >= 5)
            //						new EvilTribe5Effect(indexVal, buff).Start(this);
            //					if (buff.Condition >= 3)
            //						new EvilTribe3Effect(indexVal, buff).Start(this);
            //				}
            //			}
            //			if (finalBuff.Condition >= 3)
            //			{
            //				new EvilTribe3Effect(indexVal, finalBuff).Start(this);
            //			}
            //			if (Game is PVEGame)
            //				m_player.SendMessage(msg);
            //			break;
            //		case 4:
            //			if (finalBuff.Condition >= 4 && buffs != null)
            //			{
            //				foreach (var buff in buffs)
            //				{
            //					if (buff.Condition >= 4)
            //						new ShadowDevil4Effect(indexVal, buff).Start(this);
            //					if (buff.Condition >= 2)
            //						new ShadowDevil2Effect(indexVal, buff).Start(this);
            //				}
            //			}
            //			if (finalBuff.Condition >= 2)
            //			{
            //				new ShadowDevil2Effect(indexVal, finalBuff).Start(this);
            //			}
            //			if (Game is PVEGame)
            //				m_player.SendMessage(msg);
            //			break;
            //		case 5:
            //			if (finalBuff.Condition >= 4)
            //			{
            //				foreach (var buff in buffs)
            //				{
            //					if (buff.Condition >= 4)
            //						new FourArtifacts4Effect(indexVal, buff).Start(this);
            //					if (buff.Condition >= 2)
            //						new FourArtifacts2Effect(indexVal, buff).Start(this);
            //				}
            //			}
            //			if (finalBuff.Condition >= 2)
            //			{
            //				new FourArtifacts2Effect(indexVal, finalBuff).Start(this);
            //			}
            //			if (Game is PVEGame)
            //				m_player.SendMessage(msg);
            //			break;
            //		case 6:
            //			if (finalBuff.Condition >= 5 && buffs != null)
            //			{
            //				foreach (var buff in buffs)
            //				{
            //					if (buff.Condition >= 5)
            //						new Goblin5Effect(indexVal, buff).Start(this);
            //					if (buff.Condition >= 4)
            //						new Goblin4Effect(indexVal, buff).Start(this);
            //					if (buff.Condition >= 2)
            //						new Goblin2Effect(indexVal, buff).Start(this);
            //				}
            //			}
            //			if (finalBuff.Condition >= 4 && buffs != null)
            //			{
            //				foreach (var buff in buffs)
            //				{
            //					if (buff.Condition >= 4)
            //						new Goblin4Effect(indexVal, buff).Start(this);
            //					if (buff.Condition >= 2)
            //						new Goblin2Effect(indexVal, buff).Start(this);
            //				}
            //			}
            //			if (finalBuff.Condition >= 2)
            //			{
            //				new Goblin2Effect(indexVal, finalBuff).Start(this);
            //			}
            //			if (Game is PVEGame)
            //				m_player.SendMessage(msg);
            //			break;
            //		case 7:
            //			if (finalBuff.Condition >= 4 && buffs != null)
            //			{
            //				foreach (var buff in buffs)
            //				{
            //					if (buff.Condition >= 4)
            //						new RunRunChicken4Effect(indexVal, buff).Start(this);
            //					if (buff.Condition >= 2)
            //						new RunRunChicken2Effect(indexVal, buff).Start(this);
            //				}
            //			}
            //			if (finalBuff.Condition >= 2)
            //			{
            //				new RunRunChicken2Effect(indexVal, finalBuff).Start(this);
            //			}
            //			if (Game is PVEGame)
            //				m_player.SendMessage(msg);
            //			break;
            //		case 8:
            //			if (finalBuff.Condition >= 5 && buffs != null)
            //			{
            //				foreach (var buff in buffs)
            //				{
            //					if (buff.Condition >= 5)
            //						new GuluSportsMeeting5Effect(indexVal, buff).Start(this);
            //					if (buff.Condition >= 4)
            //						new GuluSportsMeeting4Effect(indexVal, buff).Start(this);
            //					if (buff.Condition >= 2)
            //						new GuluSportsMeeting2Effect(indexVal, buff).Start(this);
            //				}
            //			}
            //			if (finalBuff.Condition >= 4 && buffs != null)
            //			{
            //				foreach (var buff in buffs)
            //				{
            //					if (buff.Condition >= 4)
            //						new GuluSportsMeeting4Effect(indexVal, buff).Start(this);
            //					if (buff.Condition >= 2)
            //						new GuluSportsMeeting2Effect(indexVal, buff).Start(this);
            //				}
            //			}
            //			if (finalBuff.Condition >= 2)
            //			{
            //				new GuluSportsMeeting2Effect(indexVal, finalBuff).Start(this);
            //			}
            //			if (Game is PVEGame)
            //				m_player.SendMessage(msg);
            //			break;
            //		case 9:
            //			if (finalBuff.Condition >= 5 && buffs != null)
            //			{
            //				foreach (var buff in buffs)
            //				{
            //					if (buff.Condition >= 5)
            //						new FiveGodSoldier5Effect(indexVal, buff).Start(this);
            //					if (buff.Condition >= 2)
            //						new FiveGodSoldier2Effect(indexVal, buff).Start(this);
            //				}
            //			}
            //			if (finalBuff.Condition >= 2)
            //			{
            //				new FiveGodSoldier2Effect(indexVal, finalBuff).Start(this);
            //			}
            //			if (Game is PVEGame)
            //				m_player.SendMessage(msg);
            //			break;
            //		case 10:
            //			if (finalBuff.Condition >= 5 && buffs != null)
            //			{
            //				foreach (var buff in buffs)
            //				{
            //					if (buff.Condition >= 5)
            //						new TimeVortex5Effect(indexVal, buff).Start(this);
            //					if (buff.Condition >= 3)
            //						new TimeVortex3Effect(indexVal, buff).Start(this);
            //				}
            //			}
            //			if (finalBuff.Condition >= 3)
            //			{
            //				new TimeVortex3Effect(indexVal, finalBuff).Start(this);
            //			}
            //			if (Game is PVEGame)
            //				m_player.SendMessage(msg);
            //			break;
            //		case 11:
            //			if (finalBuff.Condition >= 5 && buffs != null)
            //			{
            //				foreach (var buff in buffs)
            //				{
            //					if (buff.Condition >= 5)
            //						new WarriorsArena5Effect(indexVal, buff).Start(this);
            //					if (buff.Condition >= 3)
            //						new WarriorsArena3Effect(indexVal, buff).Start(this);
            //				}
            //			}
            //			if (finalBuff.Condition >= 3)
            //			{
            //				new WarriorsArena3Effect(indexVal, finalBuff).Start(this);
            //			}
            //			if (Game is PVEGame)
            //				m_player.SendMessage(msg);
            //			break;
            //		case 12:
            //			new PioneerEffect(indexVal, finalBuff).Start(this);//maxhp
            //			if (string.IsNullOrEmpty(msg) && Game is PVPGame) PlayerDetail.SendMessage(msg);
            //			break;
            //		case 13:
            //			new WeaponMasterEffect(indexVal, finalBuff).Start(this);
            //			if (string.IsNullOrEmpty(msg) && Game is PVPGame) PlayerDetail.SendMessage(msg);
            //			break;
            //		case 14:
            //			new DivineEffect(indexVal, finalBuff).Start(this);//maxhp
            //			if (string.IsNullOrEmpty(msg) && Game is PVPGame) PlayerDetail.SendMessage(msg);
            //			break;
            //		case 15:
            //			new LuckyEffect(indexVal, finalBuff).Start(this);
            //			if (string.IsNullOrEmpty(msg) && Game is PVPGame) PlayerDetail.SendMessage(msg);
            //			break;
            //	}
            //}
            #endregion
        }

        #endregion
        public bool IsCure()
		{
			switch (Weapon.TemplateID)
			{
				case 17000:
				case 17001:
				case 17002:
				case 17005:
				case 17007:
				case 17010:
				case 17100:
				case 17102:
					return true;
				default:
					return false;
			}
		}

		public bool IsSkillPet(int skillID)
        {
			switch(skillID)
            {
				case 25:
				case 26:
				case 27:
				case 28:
				case 29:
				case 30:
				case 31:
				case 32:
				case 33:
				case 34:
				case 35:
				case 36:
				case 103:
				case 104:
				case 105:
				case 124:
				case 125:
				case 126:
				case 143:
				case 144:
				case 145:
				case 160:
				case 161:
				case 162:
				case 178:
				case 179:
				case 180:
				case 195:
				case 196:
				case 197:
					return true;
				default:
					return false;
            }
        }

		public void CalculatePlayerOffer(Player player)
		{
			/*if (m_game.RoomType == eRoomType.Match && (m_game.GameType == eGameType.Guild || m_game.GameType == eGameType.Free || m_game.GameType == eGameType.Leage || m_game.GameType == eGameType.GuildLeage))
			{
				if (!player.IsLiving)
				{
					int robOffer;
					if (Game.GameType == eGameType.Guild || m_game.GameType == eGameType.GuildLeage)
					{
						robOffer = 10;
					}
					else if (PlayerDetail.PlayerCharacter.ConsortiaID != 0 && player.PlayerDetail.PlayerCharacter.ConsortiaID != 0)
					{
						robOffer = 3;
					}
					else
					{
						robOffer = 1;
					}
					if (robOffer > player.PlayerDetail.PlayerCharacter.Offer)
					{
						robOffer = player.PlayerDetail.PlayerCharacter.Offer;
					}
					if (robOffer > 0)
					{
						GainOffer += robOffer;
						player.KilledPunishmentOffer = robOffer;
					}
				}
			}*/
			if (m_game.RoomType == eRoomType.Match && (m_game.GameType == eGameType.Guild || m_game.GameType == eGameType.Free || m_game.GameType == eGameType.Leage || m_game.GameType == eGameType.GuildLeage) && !player.IsLiving)
			{
				int robOffer = ((base.Game.GameType == eGameType.Guild || base.Game.GameType == eGameType.GuildLeage) ? 10 : ((PlayerDetail.PlayerCharacter.ConsortiaID == 0 || player.PlayerDetail.PlayerCharacter.ConsortiaID == 0) ? 1 : 3));
				if (robOffer > player.PlayerDetail.PlayerCharacter.Offer)
				{
					robOffer = player.PlayerDetail.PlayerCharacter.Offer;
				}
				robOffer += TotalHurt / 2000;
				if (robOffer > 0)
				{
					GainOffer += robOffer;
					player.KilledPunishmentOffer = robOffer;
				}
			}
		}

		public override void OnAfterKillingLiving(Living target, int damageAmount, int criticalAmount)
		{
			base.OnAfterKillingLiving(target, damageAmount, criticalAmount);
			if (target is Player)
			{
				m_player.OnKillingLiving(m_game, 1, target.Id, target.IsLiving, damageAmount + criticalAmount);
				CalculatePlayerOffer(target as Player);
				return;
			}
			int id = 0;
			if (target is SimpleBoss)
			{
				id = (target as SimpleBoss).NpcInfo.ID;
			}
			if (target is SimpleNpc)
			{
				id = (target as SimpleNpc).NpcInfo.ID;
			}
			m_player.OnKillingLiving(m_game, 2, id, target.IsLiving, damageAmount + criticalAmount);
		}

		public void OnAfterDamageLiving(Living target, int damageAmount, int criticalAmount)
		{
			NpcInfo npc;
			if (target is SimpleBoss)
			{
				npc = (target as SimpleBoss).NpcInfo;
			} else if (target is SimpleNpc)
			{
				npc = (target as SimpleNpc).NpcInfo;
			} else
            {
				return;
            }
			m_player.OnDamageBoss(m_game, npc, damageAmount + criticalAmount);
		}

		protected void OnAfterPlayerShoot()
		{
			YHM_UseSkillPetAfterShooted = true;
			if (this.AfterPlayerShooted != null)
			{
				this.AfterPlayerShooted(this);
			}
		}

		protected void OnBeforePlayerShoot()
		{
			if (this.BeforePlayerShoot != null)
			{
				this.BeforePlayerShoot(this);
			}
		}

		protected void OnCollidedByObject()
		{
			if (this.CollidByObject != null)
			{
				this.CollidByObject(this);
			}
		}

		protected void OnLoadingCompleted()
		{
			if (this.LoadingCompleted != null)
			{
				this.LoadingCompleted(this);
			}
		}

		public void OnPlayerBuffSkillPet()
		{
			jaUsouSkill = true;
			if (PlayerBuffSkillPet != null)
				PlayerBuffSkillPet(this);
		}

		public void OnPlayerClearBuffSkillPet()
        {
			if (this.PlayerClearBuffSkillPet != null)
            {
				this.PlayerClearBuffSkillPet(this);
            }
        }

		public void OnPlayerCure()
		{
			if (this.PlayerCure != null)
			{
				this.PlayerCure(this);
			}
		}

		public void OnPlayerGuard()
		{
			if (this.PlayerGuard != null)
			{
				this.PlayerGuard(this);
			}
		}

		public void OnPlayerShootCure()
		{
			if (PlayerShootCure != null)
			{
				PlayerShootCure(this);
			}
		}

		protected void OnPlayerMoving()
		{
			if (this.PlayerBeginMoving != null)
			{
				this.PlayerBeginMoving(this);
			}
		}

		public void OnPlayerShoot()
		{
			if (this.PlayerShoot != null)
			{
				this.PlayerShoot(this);
			}
		}

		protected void OnPlayerCompleteShoot()
		{
			if (PlayerCompleteShoot != null)
			{
				PlayerCompleteShoot(this);
			}
		}

		public void OnPlayerAnyShellThrow()
		{
			if (PlayerAnyShellThrow != null)
			{
				PlayerAnyShellThrow(this);
			}
		}

		public void OnPlayerUseSecondWeapon(int type)
		{
			if (PlayerUseSecondWeapon != null)
			{
				PlayerUseSecondWeapon(this, type);
			}
		}
		#region new event
		public event PlayerEventHandle PlayerBeforeReset;
		public void OnPlayerBeforeReset()
		{
			PlayerBeforeReset?.Invoke(this);
		}

		public event PlayerEventHandle PlayerAfterReset;
		public void OnPlayerAfterReset()
		{
			PlayerAfterReset?.Invoke(this);
		}
		#endregion
		public void OpenBox(int boxId)
		{
			Box box = null;
			foreach (Box box2 in m_tempBoxes)
			{
				if (box2.Id == boxId)
				{
					box = box2;
					break;
				}
			}
			if (box == null || box.Item == null)
			{
				return;
			}
			ItemInfo item = box.Item;
			switch (item.TemplateID)
			{
				case -1100:
					m_player.AddGiftToken(item.Count);
					break;
				case -200:
					m_player.AddMoney(item.Count, igroneAll: false);
					m_player.LogAddMoney(AddMoneyType.Box, AddMoneyType.Box_Open, m_player.PlayerCharacter.ID, item.Count, m_player.PlayerCharacter.Money);
					break;
				case -100:
					m_player.AddGold(item.Count);
					break;
				default:
					if (item.Template.CategoryID == 10)
					{
						if (m_player.AddTemplate(item, eBageType.FightBag, item.Count, eGameView.RouletteTypeGet))
						{
						}
					}
					else
					{
						m_player.AddTemplate(item, eBageType.TempBag, item.Count, eGameView.dungeonTypeGet);
					}
					break;
			}
			m_tempBoxes.Remove(box);
		}

		public override void PickBox(Box box)
		{
			m_tempBoxes.Add(box);
			base.PickBox(box);
		}

        public override void PrepareNewTurn()
        {
			if(CurrentIsHitTarget)
            {
				TotalHitTargetCount++;
            }
			m_energy = (int)Agility / 30 + 240;
			if (FightBuffers.ConsortionAddEnergy > 0)
            {
				m_energy += FightBuffers.ConsortionAddEnergy;
            }
			PetEffects.CurrentUseSkill = 0;
			PetEffects.PetDelay = 0;
			PetEffects.Delay = 500;
			SpecialSkillDelay = 0;
			m_shootCount = 1;
			m_ballCount = 1;
			EffectTrigger = false;
			PetEffectTrigger = false;
			PetEffects.DisibleActiveSkill = false;
			SetCurrentWeapon(PlayerDetail.MainWeapon);
			if (m_currentBall.ID != m_mainBallId)
            {
				m_currentBall = BallMgr.FindBall(m_mainBallId);
            }
			if(!IsLiving)
            {
				StartGhostMoving();
				TargetPoint = Point.Empty;
            }
			AttackInformation = true;
			DefenceInformation = true;
			CanFly = true;
			CheckUseFrost = false;
			YHM_UsePropAndSkillPet = false;
			base.PrepareNewTurn();
            #region OLD
            /*ItemFightBag.Clear();
            if (CurrentIsHitTarget)
            {
                TotalHitTargetCount++;
            }
            m_energy = (int)Agility / 30 + 240;
            if (base.FightBuffers.ConsortionAddEnergy > 0)
            {
                m_energy += base.FightBuffers.ConsortionAddEnergy;
            }
            base.PetEffects.CurrentUseSkill = 0;
            base.PetEffects.PetDelay = 0;
            base.SpecialSkillDelay = 0;
            PetEffectTrigger = false;
            base.SpecialSkillDelay = 0;
            m_shootCount = 1;
            m_ballCount = 1;
            AttackInformation = true;
            DefenceInformation = true;
            EffectTrigger = false;
            PetEffectTrigger = false;
			PetEffects.DisibleActiveSkill = false;
			if (base.Game.RoomType != eRoomType.ConsortiaBattle)
			{
				m_flyCoolDown--;
			}
            SetCurrentWeapon(PlayerDetail.MainWeapon);
            if (m_currentBall.ID != m_mainBallId)
            {
                m_currentBall = BallMgr.FindBall(m_mainBallId);
            }
            if (!base.IsLiving)
            {
                StartGhostMoving();
                TargetPoint = Point.Empty;
            }
            if (!base.PetEffects.StopMoving)
            {
                base.SpeedMultX(3);
            }
            CanFly = true;
			CheckUseFrost = false;
			YHM_UsePropAndSkillPet = false;
            base.PrepareNewTurn();*/
            #endregion
        }

        public override void PrepareSelfTurn()
        {
            DefaultDelay = m_delay;
			PetEffects.BallType = 0;
			if (base.Game.RoomType != eRoomType.ConsortiaBattle)
			{
				m_flyCoolDown--;
			}
			if(IsFrost || BlockTurn)
            {
				AddDelay(GetTurnDelay());
            }
            m_game.method_51(this);
			if (userPetInfo != null)
            {
				foreach (int skillId in _petSkillCd.Keys)
                {
					if (_petSkillCd[skillId].Turn > 0)
                    {
						_petSkillCd[skillId].Turn--;
                    }
                }
            }
			jaUsouSkill = false;
			base.PrepareSelfTurn();
		}

        public void PrepareShoot(byte speedTime)
		{
			int turnWaitTime = m_game.GetTurnWaitTime();
			int num2 = ((speedTime > turnWaitTime) ? turnWaitTime : speedTime);
			AddDelay(num2 * 20);
			TotalShootCount++;
		}

		public bool ReduceEnergy(int value)
		{
            if (value > m_energy)
            {
                value = m_energy;
            }
            m_energy -= value;
            return true;
        }

		public void ResetSkillCd()
		{
            #region
            /*if (userPetInfo != null)
            {
				string[] listSkills = userPetInfo.SkillEquip.Split('|');
				foreach (string skill in listSkills)
                {
					int skillId = int.Parse(skill.Split(',')[0]);
					if (dictionary_0.ContainsKey(skillId))
                    {
						dictionary_0[skillId].Turn = dictionary_0[skillId].ColdDown;
                    }
                }
            }*/
            #endregion
            if (userPetInfo != null)
            {
				string[] listSkills = userPetInfo.SkillEquip.Split('|');
				foreach (string skill in listSkills)
                {
					int skillId = int.Parse(skill.Split(',')[0]);
					if (_petSkillCd.ContainsKey(skillId))
                    {
						_petSkillCd[skillId].Turn = _petSkillCd[skillId].ColdDown;
                    }
                }
            }
		}

		public override void Reset()
		{
			if (m_game.RoomType == eRoomType.Dungeon)
			{
				m_game.Cards = new int[21];
			}
			else
			{
				m_game.Cards = new int[9];
			}
			base.EffectList.StopAllEffect();
			base.CardEffectList.StopAllEffect();
			base.Dander = (m_game.RoomType == eRoomType.ConsortiaBattle && m_player.PlayerCharacter.ActivePowFirstGame) ? 200 : 0;
			base.PetMP = 10;
			base.psychic = 0;
			base.IsLiving = true;
			FinishTakeCard = false;
			if (base.AutoBoot)
			{
				base.VaneOpen = true;
			}
			else
			{
				base.VaneOpen = m_player.PlayerCharacter.Grade >= 9;
			}
			m_Healstone = m_player.Healstone;
			m_changeSpecialball = 0;
			m_DeputyWeapon = m_player.SecondWeapon;
			m_weapon = m_player.MainWeapon;
			BallConfigInfo info = BallConfigMgr.FindBall(m_weapon.TemplateID);
			m_mainBallId = info.Common;
			m_spBallId = info.Special;
			m_AddWoundBallId = info.CommonAddWound;
			m_MultiBallId = info.CommonMultiBall;
			BaseDamage = m_player.GetBaseAttack();
			BaseGuard = m_player.GetBaseDefence();
			Attack = m_player.PlayerCharacter.Attack;
			Defence = m_player.PlayerCharacter.Defence;
			Agility = m_player.PlayerCharacter.Agility;
			Lucky = m_player.PlayerCharacter.Luck;
			m_maxBlood = m_player.PlayerCharacter.hp;
			BaseDamage += m_player.PlayerCharacter.DameAddPlus;

			OnPlayerBeforeReset();

			if (base.FightBuffers.ConsortionAddDamage > 0)
			{
				BaseDamage += base.FightBuffers.ConsortionAddDamage;
			}

			if (m_game.RoomType == eRoomType.ConsortiaBattle)
			{
				Attack += Attack / 100 * m_player.PlayerCharacter.AttPlusGuildBattle;
				Agility += Agility / 100 * m_player.PlayerCharacter.AgiPlusGuildBattle;
			}

			BaseGuard += m_player.PlayerCharacter.GuardAddPlus;
			Attack += m_player.PlayerCharacter.AttackAddPlus + PetEffects.BonusAttack;
			Defence += m_player.PlayerCharacter.DefendAddPlus;
			Agility += m_player.PlayerCharacter.AgiAddPlus + PetEffects.BonusAgility;
			Lucky += m_player.PlayerCharacter.LuckAddPlus;
			Attack += m_player.PlayerCharacter.StrengthEnchance;
			Defence += m_player.PlayerCharacter.StrengthEnchance;
			Agility += m_player.PlayerCharacter.StrengthEnchance;
			Lucky += m_player.PlayerCharacter.StrengthEnchance;
			if (base.FightBuffers.ConsortionAddMaxBlood > 0)
			{
				m_maxBlood += m_maxBlood * base.FightBuffers.ConsortionAddMaxBlood / 100;
			}
			if (base.FightBuffers.WorldBossHP > 0)
				m_maxBlood += FightBuffers.WorldBossHP;

			if (base.FightBuffers.WorldBossHP_MoneyBuff > 0)
				m_maxBlood += FightBuffers.WorldBossHP_MoneyBuff;

			m_maxBlood += m_player.PlayerCharacter.HpAddPlus + base.PetEffects.MaxBlood + (PetEffects == null ? 0 : PetEffects.AddMaxBloodValue);
            if (m_bufferPoint != null)
            {
                Attack += Attack / 100.0 * (double)m_bufferPoint.Value;
                Defence += Defence / 100.0 * (double)m_bufferPoint.Value;
                Agility += Agility / 100.0 * (double)m_bufferPoint.Value;
                Lucky += Lucky / 100.0 * (double)m_bufferPoint.Value;
            }

			if (base.FightBuffers.ConsortionAddProperty > 0)
			{
				Attack += base.FightBuffers.ConsortionAddProperty;
				Defence += base.FightBuffers.ConsortionAddProperty;
				Agility += base.FightBuffers.ConsortionAddProperty;
				Lucky += base.FightBuffers.ConsortionAddProperty;
			}
			m_energy = (int)Agility / 30 + 240;
			if (base.FightBuffers.ConsortionAddEnergy > 0)
			{
				m_energy += base.FightBuffers.ConsortionAddEnergy;
			}
			if (petFightPropertyInfo != null)
			{
				Attack += petFightPropertyInfo.Attack;
				Defence += petFightPropertyInfo.Defence;
				Agility += petFightPropertyInfo.Agility;
				Lucky += petFightPropertyInfo.Lucky;
				m_maxBlood += petFightPropertyInfo.Blood;
			}
		// Apply pet effect percentage modifiers
		if (PetEffects.AttackPercent > 0)
		{
			Attack += Attack / 100 * PetEffects.AttackPercent;
		}
		if (PetEffects.DefencePercent > 0)
		{
			Defence += Defence / 100 * PetEffects.DefencePercent;
		}
		if (PetEffects.AgilityPercent > 0)
		{
			Agility += Agility / 100 * PetEffects.AgilityPercent;
		}
		if (PetEffects.DamagePercent > 0)
		{
			BaseDamage += BaseDamage / 100 * PetEffects.DamagePercent;
		}
			m_currentBall = BallMgr.FindBall(m_mainBallId);
			m_shootCount = 1;
			m_ballCount = 1;
			CurrentIsHitTarget = false;
			LimitEnergy = false;
			TotalCure = 0;
			TotalHitTargetCount = 0;
			TotalHurt = 0;
			TotalKill = 0;
			TotalShootCount = 0;
			LockDirection = false;
			GainGP = 0;
			GainOffer = 0;
			Ready = false;
			InitCardBuffer(PlayerDetail.CardBuff);
			InitBuffer(PlayerDetail.EquipEffect);
			PlayerDetail.ClearTempBag();
			LoadingProcess = 0;
			base.PetEffects.CritRate = 0;
			m_killedPunishmentOffer = 0;
			m_prop = 0;
			this.IsQuanChien = this.m_player.IsQuanChien;
			this.Place = this.m_player.Place;
			IsPassiveEffect = true;
			IsShowEffectA = true;
			IsShowEffectB = true;
			CanFly = true;
			if (m_DeputyWeapon != null)
			{
				deputyWeaponResCount = m_DeputyWeapon.StrengthenLevel + 1;
			}
			else
			{
				deputyWeaponResCount = 1;
			}
			ResetSkillCd();
			//TO DO: effect card
			OnPlayerAfterReset();
			m_powerRatio = 100;
			base.Reset();
		}

		public virtual int AddMaxBlood(int value)
		{
			if (value != 0)
			{
				MaxBlood += value;
			}
			return value;
		}

		public void SetBall(int ballId)
		{
			SetBall(ballId, special: false);
		}

		public void SetBall(int ballId, bool special)
		{
			if (ballId != m_currentBall.ID)
			{
				if (BallMgr.FindBall(ballId) != null)
				{
					m_currentBall = BallMgr.FindBall(ballId);
				}
				m_game.SendGameUpdateBall(this, special);
			}
		}

		public void SetCurrentWeapon(ItemInfo item)
		{
			m_weapon = item;
			BallConfigInfo info = BallConfigMgr.FindBall(m_weapon.TemplateID);
			if (m_weapon.isGold)
			{
				info = BallConfigMgr.FindBall(m_weapon.GoldEquip.TemplateID);
			}
			if (ChangeSpecialBall > 0)
			{
				info = BallConfigMgr.FindBall(70396);
			}
			m_mainBallId = info.Common;
			m_spBallId = info.Special;
			m_AddWoundBallId = info.CommonAddWound;
			m_MultiBallId = info.CommonMultiBall;
			SetBall(m_mainBallId);
		}

		public override void SetXY(int x, int y)
		{
			if (m_x == x && m_y == y)
			{
				return;
			}
			int value = Math.Abs(m_x - x);
			m_x = x;
			m_y = y;
			if (base.IsLiving && !LimitEnergy)
			{
				m_energy -= Math.Abs(m_x - x);
				if (value > 0)
				{
					OnPlayerMoving();
				}
				return;
			}
			Rectangle rect = m_rect;
			rect.Offset(m_x, m_y);
			Physics[] array = m_map.FindPhysicalObjects(rect, this);
			Physics[] array2 = array;
			foreach (Physics physics in array2)
			{
				if (physics is Box)
				{
					Box box = physics as Box;
					PickBox(box);
					base.Game.CheckBox();
				}
			}
			#region OLD
			//if (m_x == x && m_y == y)
			//	return;

			//int energyReduce = (int)((double)Math.Abs(x - m_x) * ((double)PowerRatio / 100.0));

			//m_x = x;
			//m_y = y;

			//if (IsLiving)
			//         {
			//	if (!LimitEnergy)
			//	{
			//		this.m_energy -= (int)((double)Math.Abs(x - m_x) * ((double)PowerRatio / 100.0));
			//		if (energyReduce > 0)
			//		{
			//			OnPlayerMoving();
			//		}
			//	}
			//         }
			//else
			//         {
			//	if (m_map == null)
			//		return;

			//	Rectangle rect = m_rect;
			//	rect.Offset(m_x, m_y);
			//	Physics[] phys = m_map.FindPhysicalObjects(rect, this);
			//	foreach (Physics p in phys)
			//             {
			//		Box b = p as Box;
			//		PickBox(b);
			//             }
			//         }
			//     }
			#endregion
		}


        public bool Shoot(int x, int y, int force, int angle)
		{
			if (m_shootCount == 1)
            {
				base.PetEffects.ActivePetHit = true;
			}
			if (IsCure() || CheckUseFrost)
			{
				m_shootCount = 1;
				m_ballCount = 1;
			}
			if (YHM_UseSkillPetWithProp)
            {
				m_shootCount = 1;
				m_ballCount = 1;
				YHM_UseSkillPetWithProp = false;
            }
			if (m_shootCount > 0)
			{
				OnPlayerShoot();
				int iD = m_currentBall.ID;
				if (m_ballCount >= 1 && !IsSpecialSkill)
				{
					if (Prop == 20002)
					{
						iD = m_MultiBallId;
					}
					if (Prop == 20008 || Prop == 30009)
					{
						iD = m_AddWoundBallId;
					}
				}
				OnPlayerAnyShellThrow();
				OnBeforePlayerShoot();
				if (IsSpecialSkill)
				{
					ControlBall = false;
					m_ballCount = m_currentBall.Amount;
					base.SpecialSkillDelay = 2000;
				}
				if (ShootImp(iD, x, y, force, angle, m_ballCount, ShootCount))
				{
					if (iD == 4)
					{
						m_game.AddAction(new FightAchievementAction(this, eFightAchievementType.SuperMansNuclearExplosion, base.Direction, 1200));
					}
					m_shootCount--;
					if (m_shootCount <= 0 || !base.IsLiving)
					{
						StopAttacking();
						AddDelay(m_currentBall.Delay + (m_weapon.isGold ? m_weapon.GoldEquip.Property8 : m_weapon.Template.Property8));
						AddDander(20);
                        AddPetMP(10);
						m_prop = 0;
						if (CanGetProp)
						{
							int gold = 0;
							int money = 0;
							int giftToken = 0;
							int medal = 0;
							int honor = 0;
							int hardCurrency = 0;
							int token = 0;
							int dragonToken = 0;
							int magicStonePoint = 0;
							List<ItemInfo> list = null;
							if (DropInventory.FireDrop(m_game.RoomType, ref list) && list != null)
							{
								foreach (ItemInfo info in list)
								{
									ShopMgr.FindSpecialItemInfo(info, ref gold, ref money, ref giftToken, ref medal, ref honor, ref hardCurrency, ref token, ref dragonToken, ref magicStonePoint);
									if (info == null || !base.VaneOpen || info.TemplateID <= 0)
									{
										continue;
									}
									if (info.Template.CategoryID == 10)
									{
										if (PlayerDetail.AddTemplate(info, eBageType.FightBag, info.Count, eGameView.RouletteTypeGet))
										{
										}
									}
									else
									{
										PlayerDetail.AddTemplate(info, eBageType.TempBag, info.Count, eGameView.dungeonTypeGet);
									}
								}
								PlayerDetail.AddGold(gold);
								PlayerDetail.AddMoney(money, igroneAll: false);
								PlayerDetail.LogAddMoney(AddMoneyType.Game, AddMoneyType.Game_Shoot, PlayerDetail.PlayerCharacter.ID, money, PlayerDetail.PlayerCharacter.Money);
								PlayerDetail.AddGiftToken(giftToken);
							}
						}
						OnPlayerCompleteShoot();
					}
					SendAttackInformation();
					OnAfterPlayerShoot();
					return true;
				}
			}
			return false;
		}

		public override void Skip(int spendTime)
		{
			if (IsAttacking)
            {
				Game.SendSkipNext(this);
				m_prop = 0;
				AddDelay(100);
				AddDander(20);
				AddPetMP(10);
				base.Skip(spendTime);
            }
		}

		public bool CanUsePetSkill;

		public bool jaUsouSkill { get; set; }

		public void PetUseKill(int skillId, int type)
		{
			if (YHM_UsePropAndSkillPet && IsSkillPet(skillId))
			{
				m_player.SendMessage("thao tác không đúng");
			}
			else
			{
				if (CanUsePetSkill && PetSkillCD.ContainsKey(skillId) && !jaUsouSkill && !YHM_UseSkillPetAfterShooted)
				{
					PetSkillInfo skillInfo = _petSkillCd[skillId];
					if (PetMP > 0 && PetMP >= skillInfo.CostMP)
					{
						if (skillInfo.NewBallID != -1)
						{
							PetEffects.Delay += skillInfo.Delay;
							SetBall(skillInfo.NewBallID);
						}
						PetMP -= skillInfo.CostMP;
						PetEffects.CurrentUseSkill = skillId;
						PetEffects.BallType = skillInfo.BallType;
						OnPlayerBuffSkillPet();
						m_game.SendPetUseKill(this, type);
						skillInfo.Turn = skillInfo.ColdDown + 1;
					}
					else
					{
						m_player.SendMessage("Ma Pháp không đủ.");
					}
				}
			}
		}
		public bool CanUseSkill(int Id)
		{
			if (userPetInfo != null)
			{
				string[] array = userPetInfo.SkillEquip.Split('|');
				for (int i = 0; i < array.Length; i++)
				{
					if (int.Parse(array[i].Split(',')[0]) == Id)
					{
						return true;
					}
				}
			}
			return false;
		}

		public override void StartAttacking()
		{
			if (base.IsAttacking)
			{
				return;
			}
			if (m_Healstone != null && m_blood < m_maxBlood && !base.Game.IsSpecialPVE() && m_player.RemoveHealstone())
			{
				int property2 = m_Healstone.Template.Property2;
				BufferInfo fightBuffByType = GetFightBuffByType(BuffType.ReHealth);
				if (fightBuffByType != null && m_player.UsePayBuff(BuffType.ReHealth))
				{
					property2 *= fightBuffByType.Value;
				}
				AddBlood(property2);
			}
			AddDelay(GetTurnDelay());
			base.StartAttacking();
		}

		public BufferInfo GetFightBuffByType(BuffType buff)
		{
			foreach (BufferInfo fightBuff in m_player.FightBuffs)
			{
				if (fightBuff.Type == (int)buff)
				{
					return fightBuff;
				}
			}
			return null;
		}

		public void SendAttackInformation()
		{
			if (EffectTrigger && AttackInformation)
			{
				EffectTrigger = false;
				AttackInformation = false;
			}
		}

		public void StartGhostMoving()
		{
			if (!TargetPoint.IsEmpty)
			{
				Point point = new Point(TargetPoint.X - X, TargetPoint.Y - Y);
				if (point.Length() > 160.0)
				{
					point.Normalize(160);
				}
				m_game.AddAction(new GhostMoveAction(this, new Point(X + point.X, Y + point.Y)));
			}
		}

		public override void StartMoving()
		{
			if (m_map == null)
			{
				return;
			}
			Point point = m_map.FindYLineNotEmptyPointDown(m_x, m_y);
			if (point.IsEmpty)
			{
				if (m_map.Ground != null)
				{
					m_y = m_map.Ground.Height;
				}
			}
			else
			{
				m_x = point.X;
				m_y = point.Y;
			}
			if (point.IsEmpty)
			{
				m_syncAtTime = false;
				Die();
			}
		}

		public override void StartMoving(int delay, int speed)
		{
			if (m_map != null)
			{
				Point point = m_map.FindYLineNotEmptyPointDown(m_x, m_y);
				if (point.IsEmpty)
				{
					m_y = m_map.Ground.Height;
				}
				else
				{
					m_x = point.X;
					m_y = point.Y;
				}
				base.StartMoving(delay, speed);
				if (point.IsEmpty)
				{
					m_syncAtTime = false;
					Die();
				}
			}
		}

		public void StartSpeedMult(int x, int y, int delay)
		{
			Point point = new Point(x - X, y - Y);
			m_game.AddAction(new PlayerSpeedMultAction(this, new Point(X + point.X, Y + point.Y), delay));
		}

		public override bool TakeDamage(Living source, ref int damageAmount, ref int criticalAmount, string msg)
		{
			if ((source == this || source.Team == base.Team) && damageAmount + criticalAmount >= m_blood)
			{
				damageAmount = m_blood - 1;
				criticalAmount = 0;
			}
			bool flag = base.TakeDamage(source, ref damageAmount, ref criticalAmount, msg);
			if (base.IsLiving)
			{
				AddDander((damageAmount * 2 / 5 + 5) / 2);
				if (!base.Game.IsSpecialPVEForBuffer() && base.Blood < base.MaxBlood / 100 * 30)
				{
					BufferInfo fightBuffByType = GetFightBuffByType(BuffType.Save_Life);
					if (fightBuffByType != null && m_player.UsePayBuff(BuffType.Save_Life))
					{
						int num = base.MaxBlood / 100 * fightBuffByType.Value;
						AddBlood(num);
						m_game.method_53(this, LanguageMgr.GetTranslation("GameServer.PayBuff.ReLife.UseNotice", PlayerDetail.PlayerCharacter.NickName, num));
					}
				}
			}
			return flag;
		}

		public void UseFlySkill()
		{
			if (m_flyCoolDown > 0 && base.Game.RoomType == eRoomType.ConsortiaBattle)
			{
				this.m_flyCoolDown--;
				m_game.SendPlayerUseProp(this, -2, -2, CARRY_TEMPLATE_ID);
				SetBall(3);
				m_energy -= (int)EquipType.FLY_ENERGY;
			}
			else
			{
				if (CanFly)
				{
					m_game.SendPlayerUseProp(this, -2, -2, CARRY_TEMPLATE_ID);
					SetBall(3);
				}
			}
		}

		public bool UseItem(ItemTemplateInfo item)
		{
			bool result;
			if(this.CanUseItem(item))
            {
				this.m_energy -= item.Property4;
				this.m_delay += item.Property5;
				this.m_game.SendPlayerUseProp(this, -2, -2, item.TemplateID, this);
				YHM_UsePropAndSkillPet = true;
				SpellMgr.ExecuteSpell(m_game, this, item);
				result = true; ;
            }
			else
            {
				result = false;
            }
			return result;
		}
        //fix sinh linh
        public bool UseItem(ItemTemplateInfo item, int place)
        {
            if (!CanUseItem(item, place))
            {
                return false;
            }
            if (base.IsLiving)
            {
                ReduceEnergy(item.Property4);
                AddDelay(item.Property5);
            }
            else if (place == -1)
            {
                base.psychic -= item.Property7;
                base.Game.CurrentLiving.AddDelay(item.Property5);
            }
            m_game.method_39(this, -2, -2, item.TemplateID);
            SpellMgr.ExecuteSpell(m_game, m_game.CurrentLiving as Player, item);
            if (item.Property6 == 1 && base.IsAttacking)
            {
                StopAttacking();
                m_game.CheckState(0);
            }
            OnBeginUseProp();
            return true;
        }

        public void UseSecondWeapon()
		{
			if (CanUseItem(m_DeputyWeapon.Template))
			{
				if (m_DeputyWeapon.Template.Property3 == 31)
				{
					bool isArrmor = false;
					if (new List<int> { 17006, 17012, 17013 }.Contains(m_DeputyWeapon.TemplateID))
					{
						isArrmor = true;
					}
					new AddGuardEquipEffect((int)getHertAddition(m_DeputyWeapon), 1, isArrmor).Start(this);
					OnPlayerGuard();
				}
				else
				{
					SetCurrentWeapon(m_DeputyWeapon);
					OnPlayerCure();
				}
				ShootCount = 1;
				m_energy -= m_DeputyWeapon.Template.Property4;
				m_delay += m_DeputyWeapon.Template.Property5;
				m_game.SendPlayerUseProp(this, -2, -2, m_DeputyWeapon.Template.TemplateID);
				if (deputyWeaponResCount > 0)
				{
					deputyWeaponResCount--;
					m_game.SendUseDeputyWeapon(this, deputyWeaponResCount);
				}
				OnPlayerUseSecondWeapon(m_DeputyWeapon.Template.Property3);
			}
		}

		public void UseSpecialSkill()
		{
			if (base.Dander >= 200)
			{
				SetBall(m_spBallId, special: true);
				m_ballCount = m_currentBall.Amount;
				SetDander(0);
			}
		}

		public override void SpeedMultX(int value)
		{
			SpeedMult = value;
			MOVE_SPEED = value - 1;
			base.SpeedMultX(value);
		}

		public bool canMoveDirection(int dir)
		{
			return !m_map.IsOutMap(X + (15 + MOVE_SPEED) * dir, Y);
		}

		public Point getNextWalkPoint(int dir)
		{
			if (canMoveDirection(dir))
			{
				return m_map.FindNextWalkPoint(X, Y, dir, StepX, StepY);
			}
			return Point.Empty;
		}

		public Point FindYLineNotEmptyPointDown(int tx, int ty)
		{
			_ = m_map.Bound;
			return m_map.FindYLineNotEmptyPointDown(tx, ty, m_map.Bound.Height);
		}

		public void OnBeforeBomb(int delay)
		{
			if (this.BeforeBomb != null)
			{
				this.BeforeBomb(this);
			}
		}
	}
}
