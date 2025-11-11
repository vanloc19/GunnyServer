using System.Collections.Generic;
using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.NPC
{
	public class EighthDarkEarlBoss : ABrain
	{
		protected Player m_targer;

		private List<SimpleNpc> list_0;

		private int int_0;

		private int edjwEiujyCF;

		private int int_1;

		private bool bool_0;

		public override void OnBeginSelfTurn()
		{
			base.OnBeginSelfTurn();
			if (base.Game.FindAllNpcLiving().Length == 2)
			{
				for (int i = 0; i < edjwEiujyCF; i++)
				{
					int num = base.Game.Random.Next(base.Body.X - 300, base.Body.X + 300);
					int y = base.Game.Random.Next(base.Body.Y + 10, base.Body.Y + 250);
					LivingConfig livingConfig = (base.Game as PVEGame).BaseLivingConfig();
					livingConfig.IsFly = true;
					list_0.Add((base.Game as PVEGame).CreateNpc(int_0, (num > base.Game.Map.Info.DeadWidth) ? base.Game.Map.Info.DeadWidth : num, y, 1, -1, livingConfig));
				}
			}
		}

		public override void OnBeginNewTurn()
		{
			base.OnBeginNewTurn();
			m_body.CurrentDamagePlus = 1f;
			m_body.CurrentShootMinus = 1f;
		}

		public override void OnCreated()
		{
			base.OnCreated();
		}

		public override void OnStartAttacking()
		{
			base.OnStartAttacking();
			m_targer = base.Game.FindRandomPlayer();
			int x = base.Game.Random.Next(200, 1900);
			int y = base.Game.Random.Next(170, 560);
			if (!base.Body.MoveTo(x, y, "fly", 1000, method_0, 5))
			{
				base.Body.CallFuction(method_0, 1000);
			}
		}

		private void method_0()
		{
			switch (int_1)
			{
				case 0:
					if (bool_0)
					{
						method_1();
					}
					base.Body.ChangeDirection(base.Body.FindDirection(m_targer), 500);
					base.Body.PlayMovie("beatA", 1500, 0);
					(base.Game as PVEGame).SendObjectFocus(m_targer, 1, 3000, 0);
					base.Body.BeatDirect(m_targer, "", 4000, 1, 1);
					int_1++;
					break;
				case 1:
					base.Body.ChangeDirection(base.Body.FindDirection(m_targer), 500);
					base.Body.PlayMovie("beatB", 1500, 0);
					(base.Game as PVEGame).SendObjectFocus(m_targer, 1, 2500, 0);
					base.Body.BeatDirect(m_targer, "", 4000, 2, 1);
					(base.Game as PVEGame).SendObjectFocus(base.Body, 1, 5500, 0);
					base.Body.PlayMovie("in", 6200, 2000);
					int_1++;
					break;
				case 2:
					base.Body.PlayMovie("yinshen", 1000, 0);
					base.Body.CallFuction(TjDwEypAucP, 4000);
					bool_0 = true;
					int_1 = 0;
					break;
			}
		}

		private void TjDwEypAucP()
		{
			(base.Game as PVEGame).SendHideBlood(base.Body, 0);
			base.Body.RangeAttacking(-10000, 10000, "", 1000, directDamage: true);
		}

		public override void OnAfterTakeDamage(Living source)
		{
			base.OnAfterTakeDamage(source);
			if (bool_0)
			{
				method_1();
			}
		}

		private void method_1()
		{
			(base.Game as PVEGame).SendHideBlood(base.Body, 1);
		}

		public override void OnDie()
		{
			base.OnDie();
		}

		public override void OnStopAttacking()
		{
			base.OnStopAttacking();
		}

		public EighthDarkEarlBoss()
		{
			list_0 = new List<SimpleNpc>();
			int_0 = 10102;
			edjwEiujyCF = 5;
		}
	}
}
