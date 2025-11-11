namespace Game.Logic
{
	public class LivingConfig
	{
		public bool HaveShield;

		private bool m_DamageForzen;

		private byte m_isBotom;

		private bool m_isConsortiaBoss;

		private bool m_isFly;

		private bool m_isHelper;

		private bool m_isShowBlood;

		private bool m_isShowSmallMapPoint;

		private bool m_isTurn;

		private int m_reduceBloodStart;

		private bool m_canTakeDamage;

		private bool m_CanHeal;

		private bool m_CompleteStep;

		private int m_MaxStepMove;

		private int m_FirstStepMove;

		private int m_MinBlood;

		private bool m_CanFrost;

		private bool m_CanCountKill;

		private int m_BallCanDamage;

		private bool m_CanCollied;

		private bool m_KeepLife;

		private bool m_IsWorldBoss;

		public bool HasTurn = true;

        public bool DuocBan = true;

        public bool IsWorldBoss
        {
			get
            {
				return m_IsWorldBoss;
            }
			set
            {
				m_IsWorldBoss = value; ;
            }
        }

		public int BallCanDamage
        {
			get
            {
				return m_BallCanDamage;
            }
			set
            {
				m_BallCanDamage = value;
            }
        }

		public int MinBlood
        {
			get
            {
				return m_MinBlood;
            }
			set
            {
				m_MinBlood = value;
            }
        }

		public int FirstStepMove
        {
			get
            {
				return m_FirstStepMove;
            }
			set
            {
				this.m_FirstStepMove = value;
            }
        }

		public int MaxStepMove
        {
			get
            {
				return m_MaxStepMove;
            }
			set
            {
				this.m_MaxStepMove = value;
            }
        }

		public bool CompleteStep
        {
			get
            {
				return m_CompleteStep;
            }
			set
            {
				this.m_CompleteStep = value;
            }
        }

		public bool CanHeal
        {
			get
            {
				return m_CanHeal;
            }
			set
            {
				m_CanHeal = value;
            }
        }

		public bool CanTakeDamage
		{
			get
			{
				return m_canTakeDamage;
			}
			set
			{
				m_canTakeDamage = value;
			}
		}

		public bool DamageForzen
		{
			get
			{
				return m_DamageForzen;
			}
			set
			{
				m_DamageForzen = value;
			}
		}

		public byte isBotom
		{
			get
			{
				return m_isBotom;
			}
			set
			{
				m_isBotom = value;
			}
		}

		public bool isConsortiaBoss
		{
			get
			{
				return m_isConsortiaBoss;
			}
			set
			{
				m_isConsortiaBoss = value;
			}
		}

		public bool IsFly
		{
			get
			{
				return m_isFly;
			}
			set
			{
				m_isFly = value;
			}
		}

		public bool IsHelper
		{
			get
			{
				return m_isHelper;
			}
			set
			{
				m_isHelper = value;
			}
		}

		public bool isShowBlood
		{
			get
			{
				return m_isShowBlood;
			}
			set
			{
				m_isShowBlood = value;
			}
		}

		public bool isShowSmallMapPoint
		{
			get
			{
				return m_isShowSmallMapPoint;
			}
			set
			{
				m_isShowSmallMapPoint = value;
			}
		}

		public bool IsTurn
		{
			get
			{
				return m_isTurn;
			}
			set
			{
				m_isTurn = value;
			}
		}

		public int ReduceBloodStart
		{
			get
			{
				return m_reduceBloodStart;
			}
			set
			{
				m_reduceBloodStart = value;
			}
		}

		public bool CanFrost
        {
			get
            {
				return m_CanFrost;
            }
			set
            {
				m_CanFrost = value;
            }
        }

		public bool CanCountKill
        {
			get
            {
				return m_CanCountKill;
            }
			set
            {
				m_CanCountKill = value;
            }
        }

		public bool CanCollied
        {
			get
            {
				return m_CanCollied;
            }
			set
            {
				m_CanCollied = value;
            }
        }

		public bool KeepLife
        {
			get
            {
				return m_KeepLife;
            }
			set
            {
				m_KeepLife = value;
            }
        }

		public void Clone(LivingConfig _clone)
		{
			m_isHelper = _clone.IsHelper;
			CanTakeDamage = _clone.CanTakeDamage;
			HaveShield = _clone.HaveShield;
			isBotom = _clone.isBotom;
			isConsortiaBoss = _clone.isConsortiaBoss;
			IsFly = _clone.IsFly;
			isShowBlood = _clone.isShowBlood;
			isShowSmallMapPoint = _clone.isShowSmallMapPoint;
			IsTurn = _clone.IsTurn;
			ReduceBloodStart = _clone.ReduceBloodStart;
			DamageForzen = _clone.DamageForzen;
			m_CompleteStep = _clone.CompleteStep;
			m_MaxStepMove = _clone.MaxStepMove;
			m_FirstStepMove = _clone.FirstStepMove;
			m_MinBlood = _clone.MinBlood;
			m_CanFrost = _clone.CanFrost;
			m_BallCanDamage = _clone.BallCanDamage;
			CancelGuard = _clone.CancelGuard;
			m_CanCountKill = _clone.CanCountKill;
			m_CanHeal = _clone.CanHeal;
			m_CanCollied = _clone.CanCollied;
		}

		public bool CancelGuard { set; get; }

		public bool IsChristmasBoss { set; get; }

		public virtual void OnHeal(int blood)
		{
		}
	}
}
