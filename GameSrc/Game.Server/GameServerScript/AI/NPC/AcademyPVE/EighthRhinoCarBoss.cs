using System.Collections.Generic;
using System.Drawing;
using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.NPC
{
	public class EighthRhinoCarBoss : ABrain
	{
		private int int_0;

		private List<PhysicalObj> list_0;

		protected Player m_targer;

		private SimpleBoss simpleBoss_0;

		private int int_1;

		private int int_2;

		private List<Point> list_1;

		private Point point_0;

		public override void OnBeginSelfTurn()
		{
			base.OnBeginSelfTurn();
		}

		public override void OnBeginNewTurn()
		{
			base.OnBeginNewTurn();
			m_body.CurrentDamagePlus = 1f;
			m_body.CurrentShootMinus = 1f;
			foreach (PhysicalObj item in list_0)
			{
				base.Game.RemovePhysicalObj(item, true);
			}
			list_0 = new List<PhysicalObj>();
		}

		public override void OnCreated()
		{
			base.OnCreated();
		}

		public override void OnStartAttacking()
		{
			base.OnStartAttacking();
			if (base.Game.FindRangePlayers(0, base.Body.X).Count <= 0 && int_0 < list_1.Count)
			{
				point_0 = list_1[int_0];
				m_targer = base.Game.FindNearestPlayer(base.Body.X, base.Body.Y);
				if (point_0.X > m_targer.X)
				{
					base.Body.MoveTo(m_targer.X, m_targer.Y, "walk", 1000, method_0, 3);
				}
				else
				{
					base.Body.MoveTo(point_0.X, point_0.Y, "walk", 1000, method_0, 3);
				}
				base.Body.ChangeDirection(base.Body.FindDirection(m_targer), 500);
				int_0++;
			}
			else
			{
				method_2();
			}
		}

		private void method_0()
		{
			base.Body.PlayMovie("beatA", 500, 0);
			base.Body.CallFuction(method_1, 2500);
			base.Body.CallFuction(method_3, 5000);
		}

		private void method_1()
		{
			list_0.Add(((PVEGame)base.Game).CreatePhysicalObj(0, 0, "top", "asset.game.4.cuipao", "", 1, 1, m_targer.Id + 1));
			m_targer.SpeedMultX(18);
			m_targer.StartSpeedMult(m_targer.X + 500, m_targer.Y, 0);
			base.Body.BeatDirect(m_targer, "", 100, 1, 1);
			if (point_0.X > base.Body.X)
			{
				base.Body.MoveTo(point_0.X, point_0.Y, "walk", 3000, 3);
			}
		}

		private void method_2()
		{
			base.Body.PlayMovie("beatB", 500, 0);
			base.Body.CurrentDamagePlus = 1000f;
			base.Body.RangeAttacking(-10000, 10000, "cry", 2000, directDamage: true);
		}

		private void method_3()
		{
			m_targer.SpeedMultX(3);
		}

		public override void OnDie()
		{
			base.OnDie();
		}

		public override void OnStopAttacking()
		{
			base.OnStopAttacking();
		}

		public override void OnAfterTakedBomb()
		{
			if (base.Body.Blood == 1 && simpleBoss_0 == null && base.Body.Config.CanTakeDamage)
			{
				base.Body.Config.CanTakeDamage = false;
				base.Body.PlayMovie("out", 100, 0);
				base.Body.CallFuction(method_4, 4000);
			}
		}

		private void method_4()
		{
			simpleBoss_0 = (base.Game as PVEGame).CreateBoss(int_2, 874, 834, 1, 1, "born");
			simpleBoss_0.SetRelateDemagemRect(-42, -33, 18, 18);
			(base.Game as PVEGame).SendObjectFocus(simpleBoss_0, 1, 1000, 0);
			base.Body.Die(100);
			simpleBoss_0.CallFuction(method_5, 5000);
		}

		private void method_5()
		{
			list_0.Add(((PVEGame)base.Game).CreatePhysicalObj(-34, -28, "top", "asset.game.4.mubiao", "", 1, 1, simpleBoss_0.Id + 1));
			base.Body.CallFuction(method_6, 4000);
		}

		private void method_6()
		{
			foreach (PhysicalObj item in list_0)
			{
				base.Game.RemovePhysicalObj(item, true);
			}
			base.Game.RemoveLiving(base.Body, true);
		}

		public EighthRhinoCarBoss()
		{
			list_0 = new List<PhysicalObj>();
			int_1 = 8105;
			int_2 = 8102;
			list_1 = new List<Point>
			{
				new Point(578, 836),
				new Point(1076, 835),
				new Point(1442, 831)
			};
		}
	}
}
