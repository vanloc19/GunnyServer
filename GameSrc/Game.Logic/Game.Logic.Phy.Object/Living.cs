// Game.Logic.Phy.Object.Living
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Bussiness;
using Game.Base.Packets;
using Game.Logic;
using Game.Logic.Actions;
using Game.Logic.CardEffects;
using Game.Logic.Effects;
using Game.Logic.PetEffects;
using Game.Logic.Phy.Actions;
using Game.Logic.Phy.Maps;
using Game.Logic.Phy.Maths;
using Game.Logic.Phy.Object;
using SqlDataProvider.Data;

public class Living : Physics
{
	public bool ClearBuff;

	public bool AddArmor;

	public double Agility;

    public bool SayWhenDie = false;

    public double Attack;

	public double BaseDamage;

	public double BaseGuard;

	private bool m_blockTurn;

	public bool ControlBall;

	public int ReduceCritFisrtGem;

	public int ReduceCritSecondGem;

	public int countBoom;

	public int totalCritical;

	public int totalShotTurn;

	public float CurrentDamagePlus;

	public bool CurrentIsHitTarget;

	public float CurrentShootMinus;

	public double Defence;

	public int EffectsCount;

	public bool EffectTrigger;

	public int Experience;

	public int FlyingPartical;

	protected static int GHOST_MOVE_SPEED = 8;

	public int Grade;

	public bool IgnoreArmor;

	public int LastLifeTimeShoot;

	public double Lucky;

	private string m_action;

	private bool m_autoBoot;

	protected int m_blood;

	private LivingConfig m_config;

	private Rectangle m_demageRect;

	public int m_direction;

	public int AddedValueEffect;

	private int m_doAction;

	private EffectList m_effectList;

	private int m_FallCount;

	private FightBufferInfo m_fightBufferInfo;

	private int m_FindCount;

	protected BaseGame m_game;

	private bool m_isAttacking;

	private bool m_isFrost;

	private bool m_isHide;

	private bool m_isNoHole;

	private bool m_isSeal;

	protected int m_maxBlood;

	private string m_modelId;

	private string m_name;

	private int m_degree;

	private PetEffectInfo m_petEffects;

	private int m_pictureTurn;

	private int m_specialSkillDelay;

	private int m_state;

	protected bool m_syncAtTime;

	public Dictionary<int, int> lastShots;

	private int m_team;

	private eLivingType m_type;

	private bool m_vaneOpen;

	public int mau;

	public int MaxBeatDis;

	public bool NoHoleTurn;

	public bool PetEffectTrigger;

	private Random rand;

	public List<int> ScoreArr;

	public int ShootMovieDelay;

	public int TotalDameLiving;

	public int TotalDamagePlayer;

	public int TotalHitTargetCount;

	public int TotalHurt;

	public int TotalKill;

	public int TotalShootCount;

	public int TurnNum;

	public int TotalCure;

	private PetEffectList petEffectList_0;

	private bool bool_1;

	public int AttackGemLimit;

	public int Prop1;

	public int Prop2;

	protected NpcInfo npcInfo_0;

	public bool YHM_UsePow = false;
	public bool YHM_UseSkillPet = false;
	public bool YHM_UseSkillPetAfterShooted = false;
	public bool YHM_UseSkillPetWithProp = false;
	public bool YHM_UsePropAndSkillPet = false;
	public bool IsPassiveEffect = true;
	public bool IsShowEffectA = true;
	public bool IsShowEffectB = true;

	public bool isPet
	{
		get
		{
			return bool_1;
		}
		set
		{
			bool_1 = value;
		}
	}

	public string ActionStr
	{
		get
		{
			return m_action;
		}
		set
		{
			m_action = value;
		}
	}

	public bool AutoBoot
	{
		get
		{
			return m_autoBoot;
		}
		set
		{
			m_autoBoot = value;
		}
	}

	public bool BlockTurn
	{
		get
		{
			return m_blockTurn;
		}
		set
		{
			m_blockTurn = value;
		}
	}

	public int Blood
	{
		get
		{
			return m_blood;
		}
		set
		{
			m_blood = value;
		}
	}

	public LivingConfig Config
	{
		get
		{
			return m_config;
		}
		set
		{
			m_config = value;
		}
	}

	public int Degree
	{
		get
		{
			return m_degree;
		}
		set
		{
			m_degree = value;
		}
	}

	public int Direction
	{
		get
		{
			return m_direction;
		}
		set
		{
			if (m_direction != value)
			{
				m_direction = value;
				ReSetRectWithDir();
				if (m_syncAtTime)
				{
					m_game.SendLivingUpdateDirection(this);
				}
			}
		}
	}

	public int DoAction
	{
		get
		{
			return m_doAction;
		}
		set
		{
			if (m_doAction != value)
			{
				m_doAction = value;
			}
		}
	}

	public EffectList EffectList => m_effectList;

	public int FallCount
	{
		get
		{
			return m_FallCount;
		}
		set
		{
			m_FallCount = value;
		}
	}

	public FightBufferInfo FightBuffers
	{
		get
		{
			return m_fightBufferInfo;
		}
		set
		{
			m_fightBufferInfo = value;
		}
	}

	public int FindCount
	{
		get
		{
			return m_FindCount;
		}
		set
		{
			m_FindCount = value;
		}
	}

	public int FireX
	{
		get;
		set;
	}

	public int FireY
	{
		get;
		set;
	}

	public BaseGame Game => m_game;

	public bool IsAttacking => m_isAttacking;

	public bool IsFrost
	{
		get
		{
			return m_isFrost;
		}
		set
		{
			if (m_isFrost != value)
			{
				m_isFrost = value;
				if (m_syncAtTime)
				{
					m_game.SendGameUpdateFrozenState(this);
				}
			}
		}
	}

	public bool IsHide
	{
		get
		{
			return m_isHide;
		}
		set
		{
			if (m_isHide != value)
			{
				m_isHide = value;
				if (m_syncAtTime)
				{
					m_game.SendGameUpdateHideState(this);
				}
			}
		}
	}

	public bool IsNoHole
	{
		get
		{
			return m_isNoHole;
		}
		set
		{
			if (m_isNoHole != value)
			{
				m_isNoHole = value;
				if (m_syncAtTime)
				{
					m_game.SendGameUpdateNoHoleState(this);
				}
			}
		}
	}

	public bool IsSay
	{
		get;
		set;
	}

	public int MaxBlood
	{
		get
		{
			return m_maxBlood;
		}
		set
		{
			m_maxBlood = value;
		}
	}

	public string ModelId => m_modelId;

	public string Name => m_name;

	public PetEffectInfo PetEffects
	{
		get
		{
			return m_petEffects;
		}
		set
		{
			m_petEffects = value;
		}
	}

	public int PictureTurn
	{
		get
		{
			return m_pictureTurn;
		}
		set
		{
			m_pictureTurn = value;
		}
	}

	public bool SetSeal2
	{
		get
		{
			return m_isSeal;
		}
		set
		{
			if (m_isSeal != value)
			{
				m_isSeal = value;
				if (m_syncAtTime)
				{
					m_game.SendGameUpdateSealState(this, 0);
				}
			}
		}
	}

	public int SpecialSkillDelay
	{
		get
		{
			return m_specialSkillDelay;
		}
		set
		{
			m_specialSkillDelay = value;
		}
	}

	public int State
	{
		get
		{
			return m_state;
		}
		set
		{
			if (m_state != value)
			{
				m_state = value;
				if (m_syncAtTime)
				{
					m_game.SendLivingUpdateAngryState(this);
				}
			}
		}
	}

	public bool SyncAtTime
	{
		get
		{
			return m_syncAtTime;
		}
		set
		{
			m_syncAtTime = value;
		}
	}

	public int Team => m_team;

	public eLivingType Type
	{
		get
		{
			return m_type;
		}
		set
		{
			m_type = value;
		}
	}

	public bool VaneOpen
	{
		get
		{
			return m_vaneOpen;
		}
		set
		{
			m_vaneOpen = value;
		}
	}

	public int STEP_X
	{
		get
		{
			if (Game.Map.Info.ID == 1164)
			{
				return 1;
			}
			return 3;
		}
	}

	public int STEP_Y
	{
		get
		{
			if (Game.Map.Info.ID == 1164)
			{
				return 3;
			}
			return 7;
		}
	}

	public PetEffectList PetEffectList => petEffectList_0;

	public NpcInfo NpcInfo => npcInfo_0;

	public event KillLivingEventHanlde AfterKilledByLiving;

	public event KillLivingEventHanlde AfterKillingLiving;

	public event LivingTakedDamageEventHandle BeforeTakeDamage;

	public event LivingEventHandle BeginAttacked;

	public event LivingEventHandle BeginAttacking;

	public event LivingEventHandle BeginNextTurn;

	public event LivingEventHandle BeginSelfTurn;

	public event LivingEventHandle Died;

	public event LivingEventHandle EndAttacking;

	public event LivingTakedDamageEventHandle TakePlayerDamage;

	public event LivingTakedDamageEventHandle TakePetDamage;
    //fix sinh linh
    public event LivingEventHandle BeginUseProp;
    protected void OnBeginUseProp()
    {
        if (BeginUseProp != null) BeginUseProp(this);
    }

    public Living(int id, BaseGame game, int team, string name, string modelId, int maxBlood, int immunity, int direction)
		: base(id)
	{
		BaseDamage = 10.0;
		BaseGuard = 10.0;
		Defence = 10.0;
		Attack = 10.0;
		Agility = 10.0;
		Lucky = 10.0;
		Grade = 1;
		Experience = 10;
		m_vaneOpen = false;
		m_action = "";
		m_game = game;
		m_team = team;
		m_name = name;
		m_modelId = modelId;
		m_maxBlood = maxBlood;
		m_direction = direction;
		m_state = 0;
		m_doAction = -1;
		MaxBeatDis = 100;
		AddArmor = false;
		ReduceCritFisrtGem = 0;
		ReduceCritSecondGem = 0;
		AttackGemLimit = 0;
		m_effectList = new EffectList(this, immunity);
		petEffectList_0 = new PetEffectList(this, immunity);
		m_petEffects = new PetEffectInfo();
		m_fightBufferInfo = new FightBufferInfo();
		CardEffectList = new CardEffectList(this, immunity);
		SetupPetEffect();
		m_config = new LivingConfig();
		m_syncAtTime = true;
		m_type = eLivingType.Living;
		rand = new Random();
		ScoreArr = new List<int>();
		m_autoBoot = false;
		m_pictureTurn = 0;
		TotalCure = 0;
		bool_1 = false;
		lastShots = new Dictionary<int, int>();
	}

	public virtual int AddBlood(int value)
	{
		return AddBlood(value, 0);
	}

	public virtual int AddBlood(int value, int type)
	{
		m_blood += value;
		if (m_blood > m_maxBlood)
		{
			m_blood = m_maxBlood;
		}
		if (m_syncAtTime)
		{
			m_game.SendGameUpdateHealth(this, type, value);
		}
		if (m_blood <= 0)
		{
			Die();
		}
		return value;
	}

	public void AddEffect(AbstractEffect effect, int delay)
	{
		m_game.AddAction(new LivingDelayEffectAction(this, effect, delay));
	}

	public void AddPetEffect(AbstractPetEffect effect, int delay)
	{
		m_game.AddAction(new LivingDelayPetEffectAction(this, effect, delay));
	}

	public void SendAfterShootedFrozen(int delay)
	{
		m_game.AddAction(new LivingAfterShootedFrozen(this, delay));
	}

	public void SendAfterShootedAction(int delay)
	{
		m_game.AddAction(new LivingAfterShootedAction(this, this, delay));
	}

	public void AddRemoveEnergy(int value)
	{
		if (m_syncAtTime)
		{
			m_game.SendGamePlayerProperty(this, "energy", value.ToString());
		}
	}

	public int MakeCriticalDamage(Living target, int baseDamage)
	{
		double lucky = Lucky;
		if (lucky * 45.0 / (800.0 + lucky) + (double)PetEffects.CritRate >= (double)m_game.Random.Next(100))
		{
		int num = target.ReduceCritFisrtGem + target.ReduceCritSecondGem + target.PetEffects.ReduceCritValue;
		int num2 = (int)((0.5 + lucky * 0.00015) * (double)baseDamage);
		num2 = num2 * (100 - num) / 100;
		if (FightBuffers.ConsortionAddCritical > 0)
		{
			num2 += FightBuffers.ConsortionAddCritical;
		}

		// Giới hạn crit damage của bot không quá 20000
		if (this is Player)
		{
			Player player = this as Player;
			if (player.PlayerDetail != null && player.PlayerDetail.PlayerCharacter != null && player.PlayerDetail.PlayerCharacter.IsAutoBot)
			{
				if (num2 > 20000)
				{
					num2 = 20000;
				}
			}
		}

		return num2;
		}
		return 0;
	}

	public bool Beat(Living target, string action, int demageAmount, int criticalAmount, int delay)
	{
		return Beat(target, action, demageAmount, criticalAmount, delay, 1, 1);
	}

	public bool Beat(Living target, string action, int demageAmount, int criticalAmount, int delay, int livingCount, int attackEffect)
	{
		if (target?.IsLiving ?? false)
		{
			demageAmount = MakeDamage(target);
			OnBeforeTakedDamage(target, ref demageAmount, ref criticalAmount);
			StartAttacked();
			if ((int)target.Distance(X, Y) <= MaxBeatDis)
			{
				if (X - target.X > 0)
				{
					Direction = -1;
				}
				else
				{
					Direction = 1;
				}
				m_game.AddAction(new LivingBeatAction(this, target, demageAmount, criticalAmount, action, delay, livingCount, attackEffect));
				return true;
			}
		}
		return false;
	}

	public void BeatDirect(Living target, string action, int delay, int livingCount, int attackEffect)
	{
		m_game.AddAction(new LivingBeatDirectAction(this, target, action, delay, livingCount, attackEffect));
	}

	public void BoltMove(int x, int y, int delay)
	{
		m_game.AddAction(new LivingBoltMoveAction(this, x, y, delay));
	}

	public double BoundDistance(Point p)
	{
		List<double> list = new List<double>();
		foreach (Rectangle item in GetDirectBoudRect())
		{
			for (int i = item.X; i <= item.X + item.Width; i += 10)
			{
				list.Add(Math.Sqrt((i - p.X) * (i - p.X) + (item.Y - p.Y) * (item.Y - p.Y)));
				list.Add(Math.Sqrt((i - p.X) * (i - p.X) + (item.Y + item.Height - p.Y) * (item.Y + item.Height - p.Y)));
			}
			for (int j = item.Y; j <= item.Y + item.Height; j += 10)
			{
				list.Add(Math.Sqrt((item.X - p.X) * (item.X - p.X) + (j - p.Y) * (j - p.Y)));
				list.Add(Math.Sqrt((item.X + item.Width - p.X) * (item.X + item.Width - p.X) + (j - p.Y) * (j - p.Y)));
			}
		}
		return list.Min();
	}

	public void CallFuction(LivingCallBack func, int delay)
	{
		if (m_game != null)
		{
			m_game.AddAction(new LivingCallFunctionAction(this, func, delay));
		}
	}

	public override void CollidedByObject(Physics phy)
	{
		if (phy is SimpleBomb)
		{
			((SimpleBomb)phy).Bomb();
		}
	}

	public static double ComputDX(double v, float m, float af, float f, float dt)
	{
		return v * (double)dt + ((double)f - (double)af * v) / (double)m * (double)dt * (double)dt;
	}

	public static double ComputeVx(double dx, float m, float af, float f, float t)
	{
		return (dx - (double)(f / m * t * t / 2f)) / (double)t + (double)(af / m) * dx * 0.7;
	}

	public static double ComputeVy(double dx, float m, float af, float f, float t)
	{
		return (dx - (double)(f / m * t * t / 2f)) / (double)t + (double)(af / m) * dx * 1.3;
	}

	public void ChangeDirection(Living obj, int delay)
	{
		int direction = FindDirection(obj);
		if (delay > 0)
		{
			m_game.AddAction(new LivingChangeDirectionAction(this, direction, delay));
		}
		else
		{
			Direction = direction;
		}
	}

	public void ChangeDirection(int direction, int delay)
	{
		if (delay > 0)
		{
			m_game.AddAction(new LivingChangeDirectionAction(this, direction, delay));
		}
		else
		{
			Direction = direction;
		}
	}

	public virtual void Die(int delay)
	{
		if (base.IsLiving && m_game != null)
		{
			m_game.AddAction(new LivingDieAction(this, delay));
		}
	}

	public override void Die()
	{
		if (m_blood > 0)
		{
			m_blood = 0;
			m_doAction = -1;
			if (m_syncAtTime)
			{
				m_game.SendGameUpdateHealth(this, 6, 0);
			}
		}
		if (base.IsLiving)
		{
			if (IsAttacking)
			{
				StopAttacking();
			}
			base.Die();
			OnDied();
			OnDieNewMethod();
			m_game.CheckState(0);
		}
	}

	public double Distance(Point p)
	{
		List<double> list = new List<double>();
		Rectangle directDemageRect = GetDirectDemageRect();
		for (int i = directDemageRect.X; i <= directDemageRect.X + directDemageRect.Width; i += 10)
		{
			list.Add(Math.Sqrt((i - p.X) * (i - p.X) + (directDemageRect.Y - p.Y) * (directDemageRect.Y - p.Y)));
			list.Add(Math.Sqrt((i - p.X) * (i - p.X) + (directDemageRect.Y + directDemageRect.Height - p.Y) * (directDemageRect.Y + directDemageRect.Height - p.Y)));
		}
		for (int j = directDemageRect.Y; j <= directDemageRect.Y + directDemageRect.Height; j += 10)
		{
			list.Add(Math.Sqrt((directDemageRect.X - p.X) * (directDemageRect.X - p.X) + (j - p.Y) * (j - p.Y)));
			list.Add(Math.Sqrt((directDemageRect.X + directDemageRect.Width - p.X) * (directDemageRect.X + directDemageRect.Width - p.X) + (j - p.Y) * (j - p.Y)));
		}
		return list.Min();
	}

	public bool FallFrom(int x, int y, string action, int delay, int type, int speed)
	{
		return FallFrom(x, y, action, delay, type, speed, null);
	}

	public bool FallFrom(int x, int y, string action, int delay, int type, int speed, LivingCallBack callback)
	{
		Point left = m_map.FindYLineNotEmptyPointDown(x, y);
		if (left == Point.Empty)
		{
			left = new Point(x, m_game.Map.Bound.Height + 1);
		}
		if (Y < left.Y)
		{
			m_game.AddAction(new LivingFallingAction(this, left.X, left.Y, speed, action, delay, type, callback));
			return true;
		}
		return false;
	}

	public bool FallFromTo(int x, int y, string action, int delay, int type, int speed, LivingCallBack callback)
	{
		m_game.AddAction(new LivingFallingAction(this, x, y, speed, action, delay, type, callback));
		return true;
	}

	public int FindDirection(Living obj)
	{
		if (obj.X > X)
		{
			return 1;
		}
		return -1;
	}

	public bool FlyTo(int X, int Y, int x, int y, string action, int delay, int speed)
	{
		return FlyTo(X, Y, x, y, action, delay, speed, null);
	}

	public bool FlyTo(int X, int Y, int x, int y, string action, int delay, int speed, LivingCallBack callback)
	{
		m_game.AddAction(new LivingFlyToAction(this, X, Y, x, y, action, delay, speed, callback));
		m_game.AddAction(new LivingFallingAction(this, x, y, 0, action, delay, 0, callback));
		return true;
	}

	public Rectangle GetDirectDemageRect()
	{
		return new Rectangle(X + m_demageRect.X, Y + m_demageRect.Y, m_demageRect.Width, m_demageRect.Height);
	}

	public List<Rectangle> GetDirectBoudRect()
	{
		return new List<Rectangle>
		{
			new Rectangle(X + base.Bound.X, Y + base.Bound.Y, base.Bound.Width, base.Bound.Height)
		};
	}

	public double getHertAddition(ItemInfo item)
	{
		if (item == null)
		{
			return 0.0;
		}
		double num = item.Template.Property7;
		double y = item.StrengthenLevel;
		return Math.Round(num * Math.Pow(1.1, y) - num) + num;
	}

	public bool GetSealState()
	{
		return m_isSeal;
	}

	public void GetShootForceAndAngle(ref int x, ref int y, int bombId, int minTime, int maxTime, int bombCount, float time, ref int force, ref int angle)
	{
		if (minTime >= maxTime)
		{
			return;
		}
		BallInfo ballInfo = BallMgr.FindBall(bombId);
		if (m_game == null || ballInfo == null)
		{
			return;
		}
		Map map = m_game.Map;
		Point shootPoint = GetShootPoint();
		float num = x - shootPoint.X;
		float num2 = y - shootPoint.Y;
		float af = map.airResistance * (float)ballInfo.DragIndex;
		float f = map.gravity * (float)ballInfo.Weight * (float)ballInfo.Mass;
		float f2 = map.wind * (float)ballInfo.Wind;
		float m = ballInfo.Mass;
		for (float num3 = time; num3 <= 4f; num3 += 0.6f)
		{
			double num4 = ComputeVx(num, m, af, f2, num3);
			double num5 = ComputeVy(num2, m, af, f, num3);
			if (num5 >= 0.0 || num4 * (double)m_direction <= 0.0)
			{
				continue;
			}
			double num6 = Math.Sqrt(num4 * num4 + num5 * num5);
			if (num6 < 2000.0)
			{
				force = (int)num6;
				angle = (int)(Math.Atan(num5 / num4) / Math.PI * 180.0);
				if (num4 < 0.0)
				{
					angle += 180;
				}
				break;
			}
		}
		x = shootPoint.X;
		y = shootPoint.Y;
	}

	public Point GetShootPoint()
	{
		return (!(this is SimpleBoss) && !(this is SimpleNpc)) ? ((m_direction <= 0) ? new Point(X + m_rect.X - 30, Y + m_rect.Y - 20) : new Point(X - m_rect.X + 30, Y + m_rect.Y - 20)) : ((m_direction <= 0) ? new Point(X + FireX, Y + FireY) : new Point(X - FireX, Y + FireY));
	}

	public bool IconPicture(eMirariType type, bool result)
	{
		m_game.SendPlayerPicture(this, (int)type, result);
		return true;
	}

	public bool IsFriendly(Living living)
	{
		if (living == null || !living.Config.IsHelper)
		{
			return !(living is Player) && living.Team == Team;
		}
		return false;
	}

	public bool JumpTo(int x, int y, string action, int delay, int type)
	{
		return JumpTo(x, y, action, delay, type, 20, null, 0);
	}

	public bool JumpTo(int x, int y, string action, int delay, int type, int speed, LivingCallBack callback)
	{
		return JumpTo(x, y, action, delay, type, speed, callback, 0);
	}

	public bool JumpTo(int x, int y, string action, int delay, int type, int speed, LivingCallBack callback, int value)
	{
		Point point = m_map.FindYLineNotEmptyPointDown(x, y);
		if (point.Y >= Y && value != 1)
		{
			return false;
		}
		m_game.AddAction(new LivingJumpAction(this, point.X, point.Y, speed, action, delay, type, callback));
		return true;
	}

	protected int MakeDamage(Living target)
	{
		if (target.Config.IsChristmasBoss)
			return 1;

		double baseDamage = BaseDamage;
		double num = target.BaseGuard;
		double num2 = target.Defence;
		double attack = Attack;
		if (target.AddArmor && (target as Player).DeputyWeapon != null)
		{
			int num3 = (int)getHertAddition((target as Player).DeputyWeapon);
			num += (double)num3;
			num2 += (double)num3;
		}
		if (IgnoreArmor)
		{
			num = 0.0;
			num2 = 0.0;
		}
	float currentDamagePlus = CurrentDamagePlus;
	float currentShootMinus = CurrentShootMinus;
	double num4 = 0.95 * (num - (double)(3 * Grade)) / (500.0 + num - (double)(3 * Grade));
	double num5 = ((!(num2 - Lucky < 0.0)) ? (0.95 * (num2 - Lucky) / (600.0 + num2 - Lucky)) : 0.0);
	double num6 = baseDamage * (1.0 + attack * 0.001) * (1.0 - (num4 + num5 - num4 * num5)) * (double)currentDamagePlus * (double)currentShootMinus;
	Point point = new Point(X, Y);
	if (num6 < 0.0)
	{
		return 1;
	}
	return (int)num6;
}

public int MakeDamage(Living target, bool them = false)
	{
		double baseDamage = BaseDamage;
		double num = target.BaseGuard;
		double num2 = target.Defence;
		double attack = Attack;
		if (target.AddArmor && (target as Player).DeputyWeapon != null)
		{
			int num3 = (int)getHertAddition((target as Player).DeputyWeapon);
			num += (double)num3;
			num2 += (double)num3;
		}
		if (IgnoreArmor)
		{
			num = 0.0;
			num2 = 0.0;
		}
		float currentDamagePlus = CurrentDamagePlus;
		float currentShootMinus = CurrentShootMinus;
		double num4 = 0.95 * (num - (double)(3 * Grade)) / (500.0 + num - (double)(3 * Grade));
		double num5 = ((!(num2 - Lucky < 0.0)) ? (0.95 * (num2 - Lucky) / (600.0 + num2 - Lucky)) : 0.0);
		double num6 = baseDamage * (1.0 + attack * 0.001) * (1.0 - (num4 + num5 - num4 * num5)) * (double)currentDamagePlus * (double)currentShootMinus;
		Point point = new Point(X, Y);
		if (num6 < 0.0)
		{
			return 1;
		}
		return (int)num6;
	}

	public virtual void OnHeal(int blood)
	{
	}

	public virtual void OnDie()
    {
    }

	public virtual void OnDieNewMethod()
	{
	}

	public bool MoveTo(int x, int y, string action, string sAction, int speed, int delay, LivingCallBack callback)
	{
		return MoveTo(x, y, action, sAction, speed, delay, callback, 0);
	}

	public bool MoveTo(int x, int y, string action, int delay, string sAction, int speed)
	{
		return MoveTo(x, y, action, sAction, speed, delay, null, 0);
	}

	public bool MoveTo(int x, int y, string action, int delay, LivingCallBack callback)
	{
		return MoveTo(x, y, action, "", 3, delay, callback, 0);
	}

	public bool MoveTo(int x, int y, string action, int delay, LivingCallBack callback, int speed)
	{
		return MoveTo(x, y, action, "", speed, delay, callback, 0);
	}

	public bool MoveTo(int x, int y, string action, int delay, int speed)
	{
		return MoveTo(x, y, action, "", speed, delay, null, 0);
	}

	public bool MoveTo(int x, int y, string action, int delay)
	{
		return MoveTo(x, y, action, "", 3, delay, null, 0);
	}

	public bool MoveTo(int x, int y, string action, int delay, string sAction, int speed, LivingCallBack callback)
	{
		return MoveTo(x, y, action, sAction, speed, delay, callback, 0);
	}

	public bool MoveTo(int x, int y, string action, int speed, string sAction, int delay, LivingCallBack callback, int delayCallback)
	{
		return MoveTo(x, y, action, sAction, speed, delay, callback, delayCallback);
	}

	public bool MoveTo(int x, int y, string action, string sAction, int speed, int delay, LivingCallBack callback, int delayCallback)
	{
		if (m_x == x && m_y == y)
		{
			return false;
		}
		if (x >= 0 && x <= m_map.Bound.Width)
		{
			List<Point> list = new List<Point>();
			int x2 = m_x;
			int y2 = m_y;
			int num = (x > x2) ? 1 : (-1);
			Point point = new Point(x2, y2);
			if (x >= m_x)
			{
			}
			if (y >= m_y)
			{
			}
			if (Config.IsFly)
			{
				Point point2 = new Point(x - point.X, y - point.Y);
				while (point2.Length() > (double)speed)
				{
					point2 = point2.Normalize(speed);
					point = new Point(point.X + point2.X, point.Y + point2.Y);
					point2 = new Point(x - point.X, y - point.Y);
					if (point != Point.Empty)
					{
						list.Add(point);
						continue;
					}
					list.Add(new Point(x, y));
					break;
				}
			}
			else
			{
				while ((x - x2) * num > 0)
				{
					point = m_map.FindNextWalkPointDown(x2, y2, num, speed * STEP_X, speed * STEP_Y);
					if (!(point != Point.Empty))
					{
						break;
					}
					list.Add(point);
					x2 = point.X;
					y2 = point.Y;
				}
			}
			if (list.Count > 0)
			{
				m_game.AddAction(new LivingMoveToAction(this, list, action, sAction, speed, delay, callback, delayCallback));
				return true;
			}
			return false;
		}
		return false;
	}

	//public bool MoveTo(int x, int y, string action, int delay, int speed, LivingCallBack callback)
	//{
	//	return MoveTo(x, y, action, delay, "", speed, callback, 0);
	//}

	//public bool MoveTo(int x, int y, string action, int delay)
	//{
	//	return MoveTo(x, y, action, delay, null);
	//}

	//public bool MoveTo(int x, int y, string action, int delay, LivingCallBack callback)
	//{
	//	if (m_x != x || m_y != y)
	//	{
	//		if (x < 0 || x > m_map.Bound.Width)
	//		{
	//			return false;
	//		}
	//		List<Point> list = new List<Point>();
	//		int x2 = m_x;
	//		int y2 = m_y;
	//		int num = ((x > x2) ? 1 : (-1));
	//		while ((x - x2) * num > 0)
	//		{
	//			Point point = m_map.FindNextWalkPoint(x2, y2, num, STEP_X, STEP_Y);
	//			if (!(point != Point.Empty))
	//			{
	//				break;
	//			}
	//			list.Add(point);
	//			x2 = point.X;
	//			y2 = point.Y;
	//		}
	//		if (list.Count > 0)
	//		{
	//			m_game.AddAction(new LivingMoveToAction(this, list, action, delay, 4, callback));
	//			return true;
	//		}
	//	}
	//	return false;
	//}

	//public bool MoveTo(int x, int y, string action, int delay, int speed)
	//{
	//	return MoveTo(x, y, action, "", speed, delay, null, 0);
	//}

	//public bool MoveTo(int x, int y, string action, int delay, LivingCallBack callback, int speed)
	//{
	//	return MoveTo(x, y, action, "", speed, delay, callback, 0);
	//}

	//public bool MoveTo(int x, int y, string action, int delay, string sAction, int speed)
	//{
	//	return MoveTo(x, y, action, delay, sAction, speed, null);
	//}

	//public bool MoveTo(int x, int y, string action, int delay, string sAction, int speed, LivingCallBack callback)
	//{
	//	return MoveTo(x, y, action, delay, sAction, speed, callback, 0);
	//}

	//public bool MoveTo(int x, int y, string action, string sAction, int speed, int delay, LivingCallBack callback)
	//{
	//	return MoveTo(x, y, action, sAction, speed, delay, callback, 0);
	//}

	//public bool MoveTo(int x, int y, string action, int delay, string sAction, int speed, LivingCallBack callback, int delayCallback)
	//{
	//	if (m_x != x || m_y != y)
	//	{
	//		if (x < 0 || x > m_map.Bound.Width)
	//		{
	//			return false;
	//		}
	//		List<Point> list = new List<Point>();
	//		int x2 = m_x;
	//		int y2 = m_y;
	//		int num = ((x > x2) ? 1 : (-1));
	//		if (!(action == "fly"))
	//		{
	//			while ((x - x2) * num > 0)
	//			{
	//				Point point = m_map.FindNextWalkPoint(x2, y2, num, speed * STEP_X, speed * STEP_Y);
	//				if (!(point != Point.Empty))
	//				{
	//					break;
	//				}
	//				list.Add(point);
	//				x2 = point.X;
	//				y2 = point.Y;
	//			}
	//		}
	//		else
	//		{
	//			Point item = new Point(x, y);
	//			Point point2 = new Point(x2, y2);
	//			Point point3 = new Point(x - point2.X, y - point2.Y);
	//			while (point3.Length() > (double)speed)
	//			{
	//				point3.Normalize(speed);
	//				point2 = new Point(point2.X + point3.X, point2.Y + point3.Y);
	//				point3 = new Point(x - point2.X, y - point2.Y);
	//				if (!(point2 != Point.Empty))
	//				{
	//					list.Add(item);
	//					break;
	//				}
	//				list.Add(point2);
	//			}
	//		}
	//		if (list.Count > 0)
	//		{
	//			m_game.AddAction(new LivingMoveToAction(this, list, action, delay, speed, sAction, callback, delayCallback));
	//			return true;
	//		}
	//	}
	//	return false;
	//}

	//public bool MoveTo(int x, int y, string action, string sAction, int speed, int delay, LivingCallBack callback, int delayCallback)
	//{
	//	if (m_x != x || m_y != y)
	//	{
	//		if (x < 0 || x > m_map.Bound.Width)
	//		{
	//			return false;
	//		}
	//		List<Point> list = new List<Point>();
	//		int x2 = m_x;
	//		int y2 = m_y;
	//		int num = ((x > x2) ? 1 : (-1));
	//		Point point = new Point(x2, y2);
	//		int x3 = m_x;
	//		int y3 = m_y;
	//		if (!Config.IsFly)
	//		{
	//			while ((x - x2) * num > 0)
	//			{
	//				point = m_map.FindNextWalkPointDown(x2, y2, num, speed * STEP_X, speed * STEP_Y);
	//				if (!(point != Point.Empty))
	//				{
	//					break;
	//				}
	//				list.Add(point);
	//				x2 = point.X;
	//				y2 = point.Y;
	//			}
	//		}
	//		else
	//		{
	//			Point point2 = new Point(x - point.X, y - point.Y);
	//			while (point2.Length() > (double)speed)
	//			{
	//				point2 = point2.Normalize(speed);
	//				point = new Point(point.X + point2.X, point.Y + point2.Y);
	//				point2 = new Point(x - point.X, y - point.Y);
	//				if (!(point != Point.Empty))
	//				{
	//					list.Add(new Point(x, y));
	//					break;
	//				}
	//				list.Add(point);
	//			}
	//		}
	//		if (list.Count > 0)
	//		{
	//			m_game.AddAction(new LivingMoveToAction2(this, list, action, sAction, speed, delay, callback, delayCallback));
	//			return true;
	//		}
	//	}
	//	return false;
	//}

	public void NoFly(bool value)
	{
		if (m_syncAtTime)
		{
			m_game.SendGamePlayerProperty(this, "nofly", value.ToString());
		}
	}

	public virtual void OnAfterKillingLiving(Living target, int damageAmount, int criticalAmount)
	{
		if (target.Team != Team)
		{
			CurrentIsHitTarget = true;
			TotalHurt += damageAmount + criticalAmount;
			if (!target.IsLiving)
			{
				TotalKill++;
			}
			m_game.CurrentTurnTotalDamage = damageAmount + criticalAmount;
			m_game.TotalHurt += damageAmount + criticalAmount;
		}
		if (this.AfterKillingLiving != null)
		{
			this.AfterKillingLiving(this, target, damageAmount, criticalAmount);
		}
		if (target.EffectTrigger && target is Player && (target as Player).DefenceInformation)
		{
			(target as Player).DefenceInformation = false;
			target.EffectTrigger = false;
		}
	}

	public virtual void OnAfterTakedBomb()
	{
	}

	public void OnAfterTakedDamage(Living target, int damageAmount, int criticalAmount)
	{
		if (this.AfterKilledByLiving != null)
		{
			this.AfterKilledByLiving(this, target, damageAmount, criticalAmount);
		}
	}

	public virtual void OnAfterTakedFrozen()
	{
	}

	public virtual void OnAfterTakeDamage(Living source)
	{
	}

	protected void OnBeforeTakedDamage(Living source, ref int damageAmount, ref int criticalAmount)
	{
		if (this.BeforeTakeDamage != null)
		{
			this.BeforeTakeDamage(this, source, ref damageAmount, ref criticalAmount);
		}
	}

	protected void OnBeginNewTurn()
	{
		if (this.BeginNextTurn != null)
		{
			this.BeginNextTurn(this);
		}
	}

	protected void OnBeginSelfTurn()
	{
		if (this.BeginSelfTurn != null)
		{
			this.BeginSelfTurn(this);
		}
	}

	protected void OnDied()
	{
		if (this.Died != null)
		{
			this.Died(this);
		}
		if (this is Player && Game is PVEGame && (this as Player).IsActive)
		{
			((PVEGame)Game).DoOther();
		}
		if ((this is SimpleBoss || this is SimpleNpc) && Game is PVEGame)
		{
			((PVEGame)Game).OnDied();
		}
	}

	public void OnSmallMap(bool state)
	{
		if (m_syncAtTime)
		{
			m_game.SendGamePlayerProperty(this, "onSmallMap", state.ToString());
		}
	}

	protected void OnStartAttacked()
	{
		if (this.BeginAttacked != null)
		{
			this.BeginAttacked(this);
		}
	}

    public virtual void OnDieByBomb()
    {

    }

    protected void OnStartAttacking()
	{
		if (this.BeginAttacking != null)
		{
			this.BeginAttacking(this);
		}
	}

	protected void OnStopAttacking()
	{
		if (this.EndAttacking != null)
		{
			this.EndAttacking(this);
		}
	}

	public void OnTakedDamage(Living source, ref int damageAmount, ref int criticalAmount)
	{
		if (this.TakePlayerDamage != null)
		{
			this.TakePlayerDamage(this, source, ref damageAmount, ref criticalAmount);
		}
	}

	public virtual void PickPhy(PhysicalObj phy)
	{
		if (m_syncAtTime)
		{
			phy.Die();
			switch (phy.Name)
			{
				case "shield-4":
					(Game as PVEGame).TotalKillCount -= 4;
					break;
				case "shield-5":
					(Game as PVEGame).TotalKillCount -= 5;
					break;
				case "shield-6":
					(Game as PVEGame).TotalKillCount -= 5;
					break;
				case "shield-1":
					(Game as PVEGame).TotalKillCount--;
					break;
				case "shield-2":
					(Game as PVEGame).TotalKillCount -= 2;
					break;
				case "shield-3":
					(Game as PVEGame).TotalKillCount -= 3;
					break;
				case "shield3":
					(Game as PVEGame).TotalKillCount += 3;
					break;
				case "shield2":
					(Game as PVEGame).TotalKillCount += 2;
					break;
				case "shield1":
					(Game as PVEGame).TotalKillCount++;
					break;
				case "shield6":
					(Game as PVEGame).TotalKillCount += 6;
					break;
				case "shield5":
					(Game as PVEGame).TotalKillCount += 5;
					break;
				case "shield4":
					(Game as PVEGame).TotalKillCount += 4;
					break;
			}
			if ((Game as PVEGame).TotalKillCount <= 0)
			{
				(Game as PVEGame).TotalKillCount = 0;
			}
		}
	}

	public virtual void PickBox(Box box)
	{
		if (box.Type > 1 && this is Player)
		{
			box.Die();
			if ((this as Player).psychic < (this as Player).MaxPsychic)
			{
				(this as Player).psychic += ((box.Type == 2) ? 10 : 20);
			}
			return;
		}
		else
		{
			box.UserID = base.Id;
			box.Die();
			if (m_syncAtTime)
			{
				m_game.SendGamePickBox(this, box.Id, 0, "");
			}
			if (base.IsLiving && this is Player)
			{
				(this as Player).OpenBox(box.Id);
			}
		}
	}

	public bool PlayerBeat(Living target, string action, int demageAmount, int criticalAmount, int delay)
	{
		if (target == null || !target.IsLiving)
		{
			return false;
		}
		demageAmount = MakeDamage(target);
		OnBeforeTakedDamage(target, ref demageAmount, ref criticalAmount);
		StartAttacked();
		m_game.AddAction(new LivingBeatAction(this, target, demageAmount, criticalAmount, action, delay, 1, 0));
		return true;
	}

	public void PlayMovie(string action, int delay, int MovieTime)
	{
		m_game.AddAction(new LivingPlayeMovieAction(this, action, delay, MovieTime));
	}

	public void PlayMovie(string action, int delay, int MovieTime, LivingCallBack call)
	{
		m_game.AddAction(new LivingPlayeMovieAction(this, action, delay, MovieTime));
	}

	public override void PrepareNewTurn()
	{
		ShootMovieDelay = 0;
		CurrentDamagePlus = 1f;
		CurrentShootMinus = 1f;
		IgnoreArmor = false;
		ControlBall = false;
		NoHoleTurn = false;
		CurrentIsHitTarget = false;
		AddedValueEffect = 1;
		Prop1 = 0;
		Prop2 = 0;
		ClearBuff = false;
		totalCritical = 0;
		totalShotTurn = 0;
		lastShots.Clear();
		YHM_UsePow = false;
		YHM_UseSkillPet = false;
		OnBeginNewTurn();
	}

	public virtual void PrepareSelfTurn()
	{
		PrepareAttackGemLilit();
		OnBeginSelfTurn();
	}

	public void PrepareAttackGemLilit()
	{
		if (AttackGemLimit > 0)
		{
			AttackGemLimit--;
		}
	}

	public CardEffectList CardEffectList { get; }

	public virtual void Reset()
	{
		m_blood = m_maxBlood;
		m_isFrost = false;
		m_isHide = false;
		m_isNoHole = false;
		m_isLiving = true;
		m_blockTurn = false;
		TurnNum = 0;
		TotalHurt = 0;
		TotalKill = 0;
		TotalShootCount = 0;
		TotalHitTargetCount = 0;
		TotalCure = 0;
		if (this is Player && m_game.RoomType == eRoomType.ConsortiaBattle)
		{
			PlayerInfo info = (this as Player).PlayerDetail.PlayerCharacter;
			if (info.ReduceStartBlood > 0 && info.ReduceStartBlood < m_maxBlood)
			{
				m_blood = info.ReduceStartBlood;
				m_maxBlood = info.hp;
			}
		}
	}

	public void Say(string msg, int type, int delay)
	{
		m_game.AddAction(new LivingSayAction(this, msg, type, delay, 1000));
	}

	public void Say(string msg, int type, int delay, int finishTime)
	{
		m_game.AddAction(new LivingSayAction(this, msg, type, delay, finishTime));
	}

	public void Seal(Player player, int type, int delay)
	{
		m_game.AddAction(new LivingSealAction(this, player, type, delay));
	}

	public void Seal(Living target, int type, int delay)
	{
		m_game.AddAction(new LivingSealAction(this, target, type, delay));
	}

	public void SetHidden(bool state)
	{
		if (m_syncAtTime)
		{
			m_game.SendGamePlayerProperty(this, "visible", state.ToString());
		}
	}

	public void SetIceFronze(Living living)
	{
		new IceFronzeEffect(2).Start(this);
		this.BeginNextTurn -= new LivingEventHandle(this.SetIceFronze);// = (LivingEventHandle)Delegate.Remove(this.BeginNextTurn, new LivingEventHandle(SetIceFronze));
	}

	public void SetIndian(bool state)
	{
		if (m_syncAtTime)
		{
			m_game.SendPlayerPicture(this, 34, state);
		}
	}

	public void SetNiutou(bool state)
	{
		if (m_syncAtTime)
		{
			m_game.SendPlayerPicture(this, 33, state);
		}
	}

	public void SetOffsetY(int p)
	{
		m_game.SendGamePlayerProperty(this, "offsetY", p.ToString());
	}

	public void ReSetRectWithDir()
	{
		SetRect(-m_rect.X - m_rect.Width, m_rect.Y, m_rect.Width, m_rect.Height);
		SetRectBomb(-m_rectBomb.X - m_rectBomb.Width, m_rectBomb.Y, m_rectBomb.Width, m_rectBomb.Height);
		SetRelateDemagemRect(-m_demageRect.X - m_demageRect.Width, m_demageRect.Y, m_demageRect.Width, m_demageRect.Height);
	}

	public void SetRelateDemagemRect(int x, int y, int width, int height)
	{
		m_demageRect.X = x;
		m_demageRect.Y = y;
		m_demageRect.Width = width;
		m_demageRect.Height = height;
	}

	public void SetSeal(bool state)
	{
		if (m_isSeal != state)
		{
			m_isSeal = state;
			if (m_syncAtTime)
			{
				m_game.SendGamePlayerProperty(this, "silenceMany", state.ToString());
			}
		}
	}

	public void SetupPetEffect()
	{
		m_petEffects = new PetEffectInfo();
		m_petEffects.PetDelay = 0;
		m_petEffects.PetBaseAtt = 0;
		m_petEffects.CritActive = false;
		m_petEffects.ActivePetHit = false;
		m_petEffects.CurrentUseSkill = 0;
		m_petEffects.ActiveGuard = false;
	}

	public void SetVisible(bool state)
	{
		m_game.SendGamePlayerProperty(this, "visible", state.ToString());
	}

	public void SetSystemState(bool state)
	{
		if (m_isSeal != state)
		{
			m_isSeal = state;
		}
		m_game.SendGamePlayerProperty(this, "system", state.ToString());
	}

	public bool ShootImp(int bombId, int x, int y, int force, int angle, int bombCount, int shootCount)
	{
		BallInfo ballInfo = BallMgr.FindBall(bombId);
		Tile shape = BallMgr.FindTile(bombId);
		BombType ballType = BallMgr.GetBallType(bombId);
		int _wind = (int)(m_map.wind * 10);//(int)((double)m_map.wind * 10.0);
		if (ballInfo == null)
		{
			return false;
		}
		GSPacketIn gSPacketIn = new GSPacketIn(91, base.Id);
		gSPacketIn.Parameter1 = base.Id;
		gSPacketIn.WriteByte(2);
		gSPacketIn.WriteInt(_wind);
		gSPacketIn.WriteBoolean(_wind > 0);
		gSPacketIn.WriteByte(m_game.GetVane(_wind, 1));
		gSPacketIn.WriteByte(m_game.GetVane(_wind, 2));
		gSPacketIn.WriteByte(m_game.GetVane(_wind, 3));
		gSPacketIn.WriteInt(bombCount);
		float num2 = 0f;
		SimpleBomb simpleBomb = null;
		for (int i = 0; i < bombCount; i++)
		{
			double num3 = 1.0;
			int num4 = 0;
			switch (i)
			{
				case 2:
					num3 = 1.1;
					num4 = 5;
					break;
				case 1:
					num3 = 0.9;
					num4 = -5;
					break;
			}
			int num5 = (int)((double)force * num3 * Math.Cos((double)(angle + num4) / 180.0 * Math.PI));
			int num6 = (int)((double)force * num3 * Math.Sin((double)(angle + num4) / 180.0 * Math.PI));
			BaseGame game = m_game;
			SimpleBomb simpleBomb2 = new SimpleBomb(game.PhysicalId++, ballType, this, m_game, ballInfo, shape, ControlBall, angle);
			simpleBomb2.SetXY(x, y);
			simpleBomb2.setSpeedXY(num5, num6);
			totalShotTurn = ((totalShotTurn == 0) ? (bombCount * shootCount) : totalShotTurn);
			m_map.AddPhysical(simpleBomb2);
			simpleBomb2.StartMoving();
			if (i == 0)
			{
				simpleBomb = simpleBomb2;
			}
			gSPacketIn.WriteInt(bombCount);
			gSPacketIn.WriteInt(shootCount);
			gSPacketIn.WriteBoolean(simpleBomb2.DigMap);
			gSPacketIn.WriteInt(simpleBomb2.Id);
			gSPacketIn.WriteInt(x);
			gSPacketIn.WriteInt(y);
			gSPacketIn.WriteInt(num5);
			gSPacketIn.WriteInt(num6);
			gSPacketIn.WriteInt(simpleBomb2.BallInfo.ID);
			if (FlyingPartical != 0)
			{
				gSPacketIn.WriteString(FlyingPartical.ToString());
			}
			else
			{
				gSPacketIn.WriteString(ballInfo.FlyingPartical);
			}
			gSPacketIn.WriteInt(simpleBomb2.BallInfo.Radii * 1000 / 4);
			gSPacketIn.WriteInt((int)simpleBomb2.BallInfo.Power * 1000);
			gSPacketIn.WriteInt(simpleBomb2.Actions.Count);
			foreach (BombAction action in simpleBomb2.Actions)
			{
				gSPacketIn.WriteInt(action.TimeInt);
				gSPacketIn.WriteInt(action.Type);
				gSPacketIn.WriteInt(action.Param1);
				gSPacketIn.WriteInt(action.Param2);
				gSPacketIn.WriteInt(action.Param3);
				gSPacketIn.WriteInt(action.Param4);
			}
			num2 = Math.Max(num2, simpleBomb2.LifeTime);
		}
		int count = simpleBomb.PetActions.Count;
		if (count > 0 && PetEffects.PetBaseAtt > 0)
		{
			if (simpleBomb.PetActions[0].Type == -1)
			{
				gSPacketIn.WriteInt(0);
			}
			else
			{
				gSPacketIn.WriteInt(count);
				foreach (BombAction petAction in simpleBomb.PetActions)
				{
					gSPacketIn.WriteInt(petAction.Param1);
					gSPacketIn.WriteInt(petAction.Param2);
					gSPacketIn.WriteInt(petAction.Param4);
					gSPacketIn.WriteInt(petAction.Param3);
				}
			}
			gSPacketIn.WriteInt(1);
		}
		else
		{
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt(0);
		}
		m_game.SendToAll(gSPacketIn);
		LastLifeTimeShoot = (int)((num2 + 2f + (float)(bombCount / 3)) * 1000f) + PetEffects.Delay + SpecialSkillDelay;
		m_game.WaitTime(LastLifeTimeShoot);
		return true;
	}

	public virtual bool PetTakeDamage(Living source, ref int damageAmount, ref int criticalAmount, string msg)
	{
		bool result = false;
		if (m_blood > 0)
		{
			m_blood -= damageAmount + criticalAmount;
			if (m_blood <= 0)
			{
				if (this.Config.MinBlood > 0)
                {
					m_blood = 1;
                }
				else
                {
					Die();
                }
			}
			result = true;
		}
		return result;
	}

	public bool ShootPoint(int x, int y, int bombId, int minTime, int maxTime, int bombCount, float time, int delay)
	{
		return ShootPoint(x, y, bombId, minTime, maxTime, bombCount, time, delay, null);
	}

	public bool ShootPoint(int x, int y, int bombId, int minTime, int maxTime, int bombCount, float time, int delay, LivingCallBack callBack)
	{
		m_game.AddAction(new LivingShootAction(this, bombId, x, y, 0, 0, bombCount, minTime, maxTime, time, delay, callBack));
		return true;
	}

	public virtual void SpeedMultX(int value)
	{
		if (m_syncAtTime)
		{
			m_game.SendGamePlayerProperty(this, "speedX", value.ToString());
		}
	}

	public void SpeedMultX(int value, string _tpye)
	{
		if (m_syncAtTime)
		{
			m_game.SendGamePlayerProperty(this, _tpye, value.ToString());
		}
	}

	public void SpeedMultY(int value)
	{
		if (m_syncAtTime)
		{
			m_game.SendGamePlayerProperty(this, "speedY", value.ToString());
		}
	}

	public void OffSeal(Living target, int delay)
	{
		m_game.AddAction(new LivingOffSealAction(this, target, delay));
	}

	public void StartAttacked()
	{
		OnStartAttacked();
	}

	public virtual void StartAttacking()
	{
		if (!m_isAttacking)
		{
			YHM_UseSkillPetAfterShooted = false;
			m_isAttacking = true;
			OnStartAttacking();
		}
	}

	public override void StartMoving()
	{
		StartMoving(0, 30);
	}

	public virtual void StartMoving(int delay, int speed)
	{
		if (Config.IsFly)
		{
			return;
		}
		Point left = m_map.FindYLineNotEmptyPointDown(X, Y);
		if (left == Point.Empty)
		{
			left = new Point(X, m_game.Map.Bound.Height + 1);
		}
		if (left.Y == Y)
		{
			return;
		}
		if (m_map.IsOutMap(left.X, left.Y))
		{
			Die();
			if (Game.CurrentLiving != this && Game.CurrentLiving is Player && this is Player && Team != Game.CurrentLiving.Team)
			{
				Player player = Game.CurrentLiving as Player;
				player.PlayerDetail.OnKillingLiving(m_game, 1, base.Id, base.IsLiving, 0);
				Game.CurrentLiving.TotalKill++;
				player.CalculatePlayerOffer(this as Player);
			}
		}
		if (m_map.IsEmpty(X, Y))
		{
			FallFrom(X, Y, null, delay, 0, speed);
		}
		base.StartMoving();
	}

	public virtual void StopAttacking()
	{
		if (m_isAttacking)
		{
			m_isAttacking = false;
			OnStopAttacking();
		}
	}

	public virtual void BeforeTakedDamage(Living source, ref int damageAmount, ref int criticalAmount)
	{
		OnBeforeTakedDamage(source, ref damageAmount, ref criticalAmount);
	}

	private int Load_Kill_Suit(int userid)
	{
		List<int> list = new List<int>();
		using (PlayerBussiness playerBussiness = new PlayerBussiness())
		{
			try
			{
				Suit_Manager suit_Manager = new Suit_Manager();
				suit_Manager = playerBussiness.Get_Suit_Manager(userid);
				if (suit_Manager.UserID > 0)
				{
					string text = suit_Manager.Kill_List;
					if (text.Length > 2)
					{
						while (text.Contains(","))
						{
							int result = 0;
							int.TryParse(text.Substring(0, text.IndexOf(",")), out result);
							if (result == 0)
							{
								break;
							}
							list.Add(result);
							text = text.Remove(0, text.IndexOf(",") + 1);
						}
						if (!text.Contains(","))
						{
							int result2 = 0;
							int.TryParse(text, out result2);
							if (result2 > 0)
							{
								list.Add(result2);
							}
						}
					}
				}
			}
			catch
			{
			}
		}
		if (list.Count > 0)
		{
			List<int> list2 = list;
			int num = 0;
			foreach (int item in list2)
			{
				int num2 = item;
				int num3 = num2;
				switch (num3)
				{
					case 1010807:
						num += 5;
						continue;
					case 1010808:
						num += 5;
						continue;
					case 1010809:
						num += 5;
						continue;
					case 1010812:
						num++;
						continue;
					case 1010813:
						num += 2;
						continue;
					case 1010814:
						num++;
						continue;
					case 1010815:
						num += 2;
						continue;
					case 1010816:
						num += 2;
						continue;
					case 1010822:
						num += 2;
						continue;
					case 1010810:
					case 1010811:
					case 1010817:
					case 1010818:
					case 1010819:
					case 1010820:
					case 1010821:
						continue;
				}
				if (num3 == 2000002)
				{
					num += 2;
				}
			}
			return num;
		}
		return 0;
	}

	public virtual bool TakeDamage(Living source, ref int damageAmount, ref int criticalAmount, string msg)
	{
		if (Config.IsHelper && (this is SimpleNpc || this is SimpleBoss) && source is Player)
		{
			return false;
		}
		bool result = false;
		if (!IsFrost && m_blood > 0)
		{
			if (source != this || source.Team == Team)
			{
				OnBeforeTakedDamage(source, ref damageAmount, ref criticalAmount);
			}
			int num = damageAmount + criticalAmount;
			if (this is Player && num != 0)
			{
				PlayerInfo playerCharacter = (this as Player).PlayerDetail.PlayerCharacter;
				int reduceDamePlus = playerCharacter.ReduceDamePlus + playerCharacter.GoldenReduceDamage;
				int num2 = num * reduceDamePlus / 100;
				num -= num2;
			}
			m_blood -= num;
			if (criticalAmount > 0 && this is Player && source is Player && this != source)
			{
				m_game.AddAction(new FightAchievementAction(source, eFightAchievementType.CharacterExplosion, source.Direction, 1200));
			}
			if (m_syncAtTime)
			{
				if (this is SimpleBoss && ((SimpleBoss)this).NpcInfo.ID == 0)
				{
					m_game.SendGameUpdateHealth(this, 6, num);
				}
				else
				{
					m_game.SendGameUpdateHealth(this, 1, num);
				}
			}
			OnAfterTakedDamage(source, damageAmount, criticalAmount);
			if (!(this is Player) && m_blood <= 0 && Config.MinBlood > 0)
			{
				m_blood = 1;
			}
			if (m_blood <= 0)
			{
				if (criticalAmount > 0 && this is Player && source is Player && this != source)
				{
					m_game.AddAction(new FightAchievementAction(source, eFightAchievementType.ExpertInStrokes, source.Direction, 1200));
				}
				if (this is SimpleBoss && (this as SimpleBoss).NpcInfo.ID == 71009)
				{
					this.PlayMovie("dieA", 0, 0);
					this.Die(1500);
				}
				else
				{
					if (source is Player)
					{
						this.DieAction((Player)source);
					}
					this.Die();
				}
				//Die();
			}
			source.OnAfterKillingLiving(this, damageAmount, criticalAmount);
			if (this.CoGiapGai)
			{
				if (criticalAmount > 0)
				{
					if (rand.Next(100) < this.GiapGai_PhanTram)
					{
						criticalAmount = 0;
					}
				}

			}
			result = true;
		}
		EffectList.StopEffect(typeof(IceFronzeEffect));
		EffectList.StopEffect(typeof(HideEffect));
		EffectList.StopEffect(typeof(NoHoleEffect));
		return result;
	}

	public void OnMakeDamage(Living living)
	{
		if (this.BeginAttacked != null)
		{
			this.BeginAttacked(living);
		}
	}

	public void ChangeDamage(double value)
	{
		BaseDamage += value;
		if (BaseDamage < 0.0)
		{
			BaseDamage = 0.0;
		}
	}

    public bool RangeAttacking(int fx, int tx, string action, int delay, List<Player> players)
    {
        return RangeAttacking(fx, tx, action, delay, removeFrost: true, directDamage: false, players);
    }

    public bool RangeAttacking(int fx, int tx, string action, int delay, bool directDamage)
    {
        return RangeAttacking(fx, tx, action, delay, removeFrost: true, directDamage, null);
    }

    public bool RangeAttacking(int fx, int tx, string action, int delay, bool removeFrost, List<Player> players)
    {
        return RangeAttacking(fx, tx, action, delay, removeFrost, directDamage: false, players);
    }

    public bool RangeAttacking(int fx, int tx, string action, int delay, bool removeFrost, bool directDamage, List<Player> players)
    {
        if (base.IsLiving)
        {
            m_game.AddAction(new LivingRangeAttackingAction(this, fx, tx, action, delay, removeFrost, directDamage, players, 0, 0));
            return true;
        }
        return false;
    }

    public void OnTakedPetDamage(Living source, ref int damageAmount, ref int criticalAmount)
	{
		if (this.TakePetDamage != null)
		{
			this.TakePetDamage(this, source, ref damageAmount, ref criticalAmount);
		}
	}

	public virtual int ReduceBlood(int value)
    {
		if ((this.Name == "Bò" || this.Name == "Quạ Bí Ẩn" || this.Name == "Thuyền Viên Gà Con" || this.Name == "Thuyền Viên Gà") && this is SimpleNpc)
        {
			value = 1;
        }
		this.m_blood -= value;
		if(this.m_blood < 0)
        {
			this.m_blood = 0;
			this.Die();
        }
		this.m_game.SendGameUpdateHealth(this, 7, value);
		return value;

	}

	public virtual int ReducedBlood(int value)
	{
		m_blood += value;
		if (m_blood > m_maxBlood)
		{
			m_blood = m_maxBlood;
		}
		if (m_syncAtTime)
		{
			m_game.SendGameUpdateHealth(this, 1, value);
		}
		if (m_blood <= 0)
		{
			Die();
		}
		EffectList.StopEffect(typeof(IceFronzeEffect));
		EffectList.StopEffect(typeof(HideEffect));
		EffectList.StopEffect(typeof(NoHoleEffect));
		return value;
	}

	public bool NewFlyTo(int x, int y, string action, int delay, string sAction, int speed)
    {
		return this.NewFlyTo(x, y, action, delay, sAction, speed, null);
    }

	public bool NewFlyTo(int x, int y, string action, int delay, string sAction, int speed, LivingCallBack callback)
	{
		return this.NewFlyTo(x, y, action, delay, sAction, speed, callback, 0);
	}

	public bool NewFlyTo(int x, int y, string action, int delay, string sAction, int speed, LivingCallBack callback, int delayCallback)
    {
		bool result;
		if (this.m_x != x || this.m_y != y)
        {
			if (x < 0 || x > this.m_map.Bound.Width)
			{
				result = false;
				return result;
			}
		}
		List<Point> list = new List<Point>();
		int x2 = this.m_x;
		int y2 = this.m_y;
		Point item = new Point(x, y);
		Point point = new Point(x2, y2);
		Point point2 = new Point(x - point.X, y - point.Y);
		while (point2.Length() > (double)speed)
		{
			point2.Normalize(speed);
			point = new Point(point.X + point2.X, point.Y + point2.Y);
			point2 = new Point(x - point.X, y - point.Y);
			if (!(point != Point.Empty))
			{
				list.Add(item);
				break;
			}
			list.Add(point);
		}
		if (list.Count > 0)
		{
			this.m_game.AddAction(new NewLivingMoveToAction(this, list, action, delay, speed, sAction, callback, delayCallback));
			result = true;
			return result;
		}
		result = false;
		return result;
	}

	public string strWrongAttack = "";

	public bool CanAttack = true;

	public bool DaCanBang = false;

	public int CanNang;

	public override void TakedDameAction()
	{
	}
	public override void DieAction(Player p)
	{
	}

	protected static int StepX = 1;
	protected static int StepY = 3;

    public bool MoveToSpeed(int x, int y, string action, int delay, int speed, LivingCallBack callback)
    {
        if ((base.m_x != x) || (base.m_y != y))
        {
            if ((x < 0) || (x > base.m_map.Bound.Width))
            {
                return false;
            }
            List<Point> path = new List<Point>();
            int num = base.m_x;
            int num2 = base.m_y;
            int direction = (x > num) ? 1 : -1;
            while (((x - num) * direction) > 0)
            {
                Point item = base.m_map.FindNextWalkPoint(num, num2, direction, (speed / 2) - 2, STEP_Y);
                if (!(item != Point.Empty))
                {
                    break;
                }
                path.Add(item);
                num = item.X;
                num2 = item.Y;
            }
            if (path.Count > 0)
            {
                this.m_game.AddAction(new LivingWalkToAction(this, path, action, delay, speed, callback));
                return true;
            }
        }
        return false;
    }

    public bool NewMoveTo(int x, int y, string action, int delay, string sAction, int speed, LivingCallBack callback, int delayCallback)
    {
		if (m_x == x && m_y == y)
			return false;

		if (x < 0 || x > m_map.Bound.Width) return false;
		List<Point> path = new List<Point>();

		int tx = m_x;
		int ty = m_y;
		int direction = x > tx ? 1 : -1;
		Point currentPoint = new Point(tx, ty);

		if (Config.IsFly)
		{
			Point offset = new Point(x - currentPoint.X, y - currentPoint.Y);
			while (offset.Length() > speed)
			{
				offset = offset.Normalize(speed);

				currentPoint = new Point(currentPoint.X + offset.X, currentPoint.Y + offset.Y);

				offset = new Point(x - currentPoint.X, y - currentPoint.Y);
				if (currentPoint != Point.Empty)
				{
					path.Add(currentPoint);
					continue;
				}

				path.Add(new Point(x, y));
				break;
			}
		}
		else
		{
			while ((x - tx) * direction > 0)
			{
				currentPoint = m_map.FindNextWalkPointDown(tx, ty, direction, speed * StepX, speed * StepY);
				if (currentPoint != Point.Empty)
				{
					path.Add(currentPoint);
					tx = currentPoint.X;
					ty = currentPoint.Y;
					continue;
				}

				break;
			}
		}

		if (path.Count > 0)
		{
			m_game.AddAction(new NewLivingMoveToAction(this, path, action, delay, speed, sAction, callback,
				delayCallback));
			return true;
		}
		else
		{
			return false;
		}
	}
	public int NewId;
	public bool CoGiapGai = false;
	public bool XuyenThau = false;
	public bool LienKetQua = false;
	public int GiapGai_PhanTram = 0;

	public bool IsQuanChien { get; set; }
	public int Place { get; set; }
}
