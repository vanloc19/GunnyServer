using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Maps;
using Game.Logic.Phy.Object;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;

namespace GameServerScript.AI.NPC
{
	public class CrosairBoss : ABrain
	{
		private int int_0;

		public int currentCount;

		public int Dander;

		public CrosairBoss()
		{


		}

		private void akVhgRvqjd8()
		{
			base.Body.RangeAttacking(0, base.Game.Map.Info.ForegroundWidth + 1, "cry", 0, null);
		}

		private void method_0(int int_1, int int_2)
		{
			base.Body.CurrentDamagePlus = 1000f;
			base.Body.PlayMovie("beatA", 1000, 0);
			base.Body.RangeAttacking(int_1, int_2, "cry", 4000, null);
		}

		private void method_1()
		{
			base.Body.CurrentDamagePlus = 1.5f;
			base.Body.PlayMovie("beatA", 1000, 0);
			base.Body.CallFuction(new LivingCallBack(this.akVhgRvqjd8), 4000);
		}

		private void method_2()
		{
			base.Body.CurrentDamagePlus = 3.1f;
			base.Body.PlayMovie("beatC", 1000, 0);
			base.Body.CallFuction(new LivingCallBack(this.akVhgRvqjd8), 3500);
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
				if (!allFightPlayer.IsLiving || allFightPlayer.X <= 670)
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
			if (this.int_0 == 0)
			{
				this.method_1();
				this.int_0++;
				return;
			}
			if (this.int_0 != 1)
			{
				this.method_2();
				this.int_0 = 0;
				return;
			}
			this.TashgAsMuBN();
			this.int_0++;
		}

		public override void OnStopAttacking()
		{
			base.OnStopAttacking();
		}

		private void TashgAsMuBN()
		{
			base.Body.PlayMovie("beatB", 1000, 0);
			base.Body.CallFuction(new LivingCallBack(this.akVhgRvqjd8), 4000);
		}
	}
}