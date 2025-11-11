using System;

namespace Game.Logic.Phy.Object
{
	public class TurnedLiving : Living
	{
		public int DefaultDelay;

		public int m_psychic;

		protected int m_delay;

		private int m_dander;

		private int int_8;

		private int int_9;

		public int Delay
		{
			get
			{
				return m_delay;
			}
			set
			{
				m_delay = value;
			}
		}

		public int Dander
		{
			get
			{
				return m_dander;
			}
			set
			{
				m_dander = value;
			}
		}

		public int PetMP
		{
			get
			{
				return int_9;
			}
			set
			{
				int_9 = value;
			}
		}

		public int PetMaxMP
		{
			get
			{
				return int_8;
			}
			set
			{
				int_8 = value;
			}
		}

		public int psychic
		{
			get
			{
				return m_psychic;
			}
			set
			{
				m_psychic = value;
			}
		}

		public void AddPetMP(int value)
		{
			if (value <= 0)
			{
				return;
			}
			if (base.IsLiving && PetMP < PetMaxMP)
			{
				int_9 += value;
				if (int_9 > PetMaxMP)
				{
					int_9 = PetMaxMP;
				}
			}
			else
			{
				int_9 = PetMaxMP;
			}
		}

		public void RemovePetMP(int value)
		{
			if (value > 0 && base.IsLiving && PetMP > 0)
			{
				int_9 -= value;
				if (int_9 < 0)
				{
					int_9 = 0;
				}
			}
		}

		public TurnedLiving(int id, BaseGame game, int team, string name, string modelId, int maxBlood, int immunity, int direction)
			: base(id, game, team, name, modelId, maxBlood, immunity, direction)
		{
			m_psychic = 999;
			int_8 = 100;
			int_9 = 10;
		}

		public override void Reset()
		{
			base.Reset();
			if (this is Player)
			{
				m_delay = GetTurnDelay();
			}
			else
			{
				m_delay = (int)Agility;
			}
		}

		public void AddDelay(int value)
		{
			if (Game is PVEGame)
			{
				m_delay = ((PVEGame)Game).MissionInfo.IncrementDelay;
			}
			else
			{
				m_delay += value;
			}
		}

		public override void PrepareSelfTurn()
		{
            #region OLD
            /*DefaultDelay = m_delay;
			if (base.IsFrost || base.BlockTurn)
			{
				if (this is Player)
				{
					AddDelay(GetTurnDelay());
				}
				else
				{
					AddDelay((this as SimpleBoss).NpcInfo.Delay);
				}
			}*/
            #endregion
            base.PrepareSelfTurn();
		}

		public int GetTurnDelay()
		{
			double baseNum = 1600;

			double rateAtt = Attack / baseNum;
			double rateAgi = Agility / baseNum;

			double realAgi = Agility / (10 + rateAgi);
			double realAtt = Attack / (10 + rateAtt);

			return (int)(baseNum - 1200 * realAgi / (realAgi + 1200) + realAtt / 10);
		}

		public void AddDander(int value)
		{
			if (value > 0 && base.IsLiving)
			{
				SetDander(m_dander + value);
			}
		}

		public void SetDander(int value)
		{
			m_dander = Math.Min(value, 200);
			if (base.SyncAtTime)
			{
				m_game.SendGameUpdateDander(this);
			}
		}

		public virtual void StartGame()
		{
		}

		public virtual void Skip(int spendTime)
		{
			if (base.IsAttacking)
			{
				StopAttacking();
				m_game.CheckState(0);
			}
		}
	}
}
