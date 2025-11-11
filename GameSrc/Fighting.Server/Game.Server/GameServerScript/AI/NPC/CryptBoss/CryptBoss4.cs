using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Maps;
using Game.Logic.Phy.Object;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;

namespace GameServerScript.AI.NPC
{
	public class CryptBoss4 : ABrain
	{
		private int int_0;

		public int currentCount;

		public int Dander;

		public CryptBoss4()
		{
			
			
		}

		private void method_0(int int_1, int int_2)
		{
			base.Body.CurrentDamagePlus = 1000f;
			base.Body.PlayMovie("beatC", 1000, 0);
			base.Body.RangeAttacking(int_1, int_2, "cry", 4000, null);
		}

		private void method_1()
		{
			Player player = base.Game.FindNearestPlayer(0, 1500);
			if (player != null)
			{
				base.Body.MoveTo(player.X + 250, player.Y, "walk", 1000, "", 10, new LivingCallBack(this.method_2));
			}
		}

		private void method_10()
		{
			base.Body.CurrentDamagePlus = 0.9f;
			base.Body.RangeAttacking(0, base.Game.Map.Info.ForegroundWidth + 1, "cry", 0, null);
		}

		private void method_2()
		{
			base.Body.CurrentDamagePlus = 0.5f;
			base.Body.Direction = base.Game.FindlivingbyDir(base.Body);
			base.Body.PlayMovie("beatA", 3000, 0);
			base.Body.CallFuction(new LivingCallBack(this.method_3), 4500);
		}

		private void method_3()
		{
			base.Body.RangeAttacking(0, base.Game.Map.Info.ForegroundWidth + 1, "cry", 0, null);
			base.Body.CallFuction(new LivingCallBack(this.method_8), 1500);
		}

		private void method_4()
		{
			Player player = base.Game.FindNearestPlayer(0, 1500);
			if (player != null)
			{
				base.Body.MoveTo(player.X + 250, player.Y, "walk", 1000, "", 10, new LivingCallBack(this.method_5));
			}
		}

		private void method_5()
		{
			base.Body.CurrentDamagePlus = 0.7f;
			base.Body.Direction = base.Game.FindlivingbyDir(base.Body);
			base.Body.PlayMovie("beatC", 3000, 0);
			base.Body.CallFuction(new LivingCallBack(this.method_6), 4500);
		}

		private void method_6()
		{
			base.Body.RangeAttacking(0, base.Game.Map.Info.ForegroundWidth + 1, "cry", 0, null);
			base.Body.CallFuction(new LivingCallBack(this.method_8), 1500);
		}

		private void method_7()
		{
			base.Body.PlayMovie("beatB", 1000, 0);
			base.Body.CallFuction(new LivingCallBack(this.method_10), 3000);
		}

		private void method_8()
		{
			base.Body.Direction = base.Game.FindlivingbyDir(base.Body);
			base.Body.MoveTo(796, base.Body.Y, "walk", 1000, "", 10, new LivingCallBack(this.method_9));
		}

		private void method_9()
		{
			base.Body.Direction = base.Game.FindlivingbyDir(base.Body);
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
			foreach (Player allFightPlayer in base.Game.GetAllFightPlayers())
			{
				if (!allFightPlayer.IsLiving || allFightPlayer.X <= base.Body.X - 150 || allFightPlayer.X >= base.Body.X + 150)
				{
					continue;
				}
				flag = true;
			}
			if (flag)
			{
				this.method_0(base.Body.X - 150, base.Body.X + 150);
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
				this.method_4();
				this.int_0 = 0;
				return;
			}
			this.method_7();
			this.int_0++;
		}

		public override void OnStopAttacking()
		{
			base.OnStopAttacking();
		}
	}
}