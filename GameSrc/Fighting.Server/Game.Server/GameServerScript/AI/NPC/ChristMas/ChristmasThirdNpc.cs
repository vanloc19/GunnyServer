using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Maps;
using Game.Logic.Phy.Object;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;

namespace GameServerScript.AI.NPC
{
	public class ChristmasThirdNpc : ABrain
	{
		private int int_0;

		public int currentCount;

		public int Dander;

		public ChristmasThirdNpc()
		{


		}

		private void method_0(int int_1, int int_2)
		{
			this.method_3(3);
			base.Body.CurrentDamagePlus = 10f;
			base.Body.PlayMovie("beatA", 3000, 0);
			base.Body.RangeAttacking(int_1, int_2, "cry", 5000, null);
		}

		private void method_1()
		{
			this.method_3(3);
			base.Body.CurrentDamagePlus = 0.5f;
			base.Body.PlayMovie("beatA", 1000, 0);
			base.Body.RangeAttacking(base.Body.X - 1000, base.Body.X + 1000, "cry", 4000, null);
		}

		private void method_2()
		{
			this.method_3(3);
			int num = base.Game.Random.Next(550, 1200);
			int direction = base.Body.Direction;
			base.Body.MoveTo(num, base.Body.Y, "walk", 1000, ((SimpleBoss)base.Body).NpcInfo.speed);
			base.Body.ChangeDirection(base.Game.FindlivingbyDir(base.Body), 9000);
		}

		private void method_3(int int_1)
		{
			int direction = base.Body.Direction;
			for (int i = 0; i < int_1; i++)
			{
				base.Body.ChangeDirection(-direction, i * 200 + 100);
				base.Body.ChangeDirection(direction, (i + 1) * 100 + i * 200);
			}
		}

		public override void OnBeginNewTurn()
		{
			base.OnBeginNewTurn();
			base.Body.CurrentDamagePlus = 1f;
			base.Body.CurrentShootMinus = 1f;
			base.Body.SetRect(((SimpleBoss)base.Body).NpcInfo.X, ((SimpleBoss)base.Body).NpcInfo.Y, ((SimpleBoss)base.Body).NpcInfo.Width, ((SimpleBoss)base.Body).NpcInfo.Height);
			if (base.Body.Direction == -1)
			{
				base.Body.SetRect(((SimpleBoss)base.Body).NpcInfo.X, ((SimpleBoss)base.Body).NpcInfo.Y, ((SimpleBoss)base.Body).NpcInfo.Width, ((SimpleBoss)base.Body).NpcInfo.Height);
				return;
			}
			base.Body.SetRect(-((SimpleBoss)base.Body).NpcInfo.X - ((SimpleBoss)base.Body).NpcInfo.Width, ((SimpleBoss)base.Body).NpcInfo.Y, ((SimpleBoss)base.Body).NpcInfo.Width, ((SimpleBoss)base.Body).NpcInfo.Height);
		}

		public override void OnBeginSelfTurn()
		{
			base.OnBeginSelfTurn();
		}

		public override void OnCreated()
		{
			base.OnCreated();
		}

		public override void OnStartAttacking()
		{
			base.Body.Direction = base.Game.FindlivingbyDir(base.Body);
			bool flag = false;
			int num = 0;
			foreach (Player allFightPlayer in base.Game.GetAllFightPlayers())
			{
				if (!allFightPlayer.IsLiving || allFightPlayer.X <= 480 || allFightPlayer.X >= 1000)
				{
					continue;
				}
				int num1 = (int)base.Body.Distance(allFightPlayer.X, allFightPlayer.Y);
				if (num1 > num)
				{
					num = num1;
				}
				flag = true;
			}
			if (flag)
			{
				this.method_0(0, base.Game.Map.Info.ForegroundWidth + 1);
				return;
			}
			if (this.int_0 != 0)
			{
				this.method_1();
				this.int_0 = 0;
				return;
			}
			this.method_2();
			this.int_0++;
		}

		public override void OnStopAttacking()
		{
			base.OnStopAttacking();
		}
	}
}