using System.Collections.Generic;
using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.NPC
{
	public class EighthSirenBoss : ABrain
	{
		protected Player m_targer;

		private List<SimpleNpc> list_0;

		private int int_0;

		private int int_1;

		private int int_2;

		public override void OnBeginSelfTurn()
		{
			base.OnBeginSelfTurn();
		}

		public override void OnBeginNewTurn()
		{
			base.OnBeginNewTurn();
			m_body.CurrentDamagePlus = 1f;
			m_body.CurrentShootMinus = 1f;
			if (m_targer != null)
			{
				return;
			}
			foreach (Player allLivingPlayer in base.Game.GetAllLivingPlayers())
			{
				if (allLivingPlayer.BlockTurn)
				{
					m_targer = allLivingPlayer;
					break;
				}
			}
		}

		public override void OnCreated()
		{
			base.OnCreated();
		}

		public override void OnStartAttacking()
		{
			base.OnStartAttacking();
			if (m_targer != null && m_targer.BlockTurn)
			{
				base.Body.CurrentDamagePlus = 2f;
				base.Body.PlayMovie("beatB", 1000, 0);
				(base.Game as PVEGame).SendObjectFocus(m_targer, 1, 3000, 0);
				base.Body.BeatDirect(m_targer, "", 4000, 1, 1);
				return;
			}
			switch (int_1)
			{
				case 1:
					base.Body.PlayMovie("call", 1000, 0);
					base.Body.CallFuction(method_1, 3000);
					int_1 = 0;
					break;
				case 0:
					base.Body.PlayMovie("beatA", 1000, 4000);
					base.Body.RangeAttacking(-10000, 10000, "", 3000, directDamage: true);
					int_1++;
					break;
			}
		}

		private void method_0()
		{
			base.Body.PlayMovie("beatA", 500, 0);
			base.Body.BeatDirect(m_targer, "", 2000, 1, 1);
		}

		private void method_1()
		{
			for (int i = 0; i < int_2; i++)
			{
				list_0.Add((base.Game as PVEGame).CreateNpc(int_0, 758 - i * 30, 888, 1));
			}
		}

		public override void OnDie()
		{
			base.OnDie();
		}

		public override void OnStopAttacking()
		{
			base.OnStopAttacking();
		}

		public EighthSirenBoss()
		{
			list_0 = new List<SimpleNpc>();
			int_0 = 11102;
			int_2 = 4;
		}
	}
}
