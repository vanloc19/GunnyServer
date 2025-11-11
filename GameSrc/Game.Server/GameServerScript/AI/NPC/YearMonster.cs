using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.NPC
{
	public class YearMonster : ABrain
	{
		private int int_0;

		public int currentCount;

		public int Dander;

		private PhysicalObj physicalObj_0;

		private Player player_0;

		public YearMonster()
		{


		}

		private void method_0(int int_1, int int_2)
		{
			base.Body.CurrentDamagePlus = 1000f;
			base.Body.PlayMovie("beatE", 2000, 0);
			base.Body.RangeAttacking(int_1, int_2, "cry", 3000, null);
		}

		private void method_1()
		{
			base.Body.CurrentDamagePlus = 1.5f;
			base.Body.PlayMovie("beatA", 1000, 0);
			base.Body.CallFuction(new LivingCallBack(this.method_2), 3000);
		}

		private void method_10()
		{
			base.Body.RangeAttacking(0, base.Game.Map.Info.ForegroundWidth + 1, "cry", 0, null);
		}

		private void method_2()
		{
			Player[] allPlayers = base.Game.GetAllPlayers();
			for (int i = 0; i < (int)allPlayers.Length; i++)
			{
				Player player = allPlayers[i];
				player.StartSpeedMult(player.X - 200, player.Y, 0);
			}
			base.Body.CallFuction(new LivingCallBack(this.method_10), 1000);
		}

		private void method_3()
		{
			base.Body.PlayMovie("beatB", 3000, 0);
			base.Body.CallFuction(new LivingCallBack(this.method_6), 4000);
		}

		private void method_4()
		{
			base.Body.CurrentDamagePlus = 3.1f;
			base.Body.PlayMovie("beatC", 3000, 0);
			base.Body.CallFuction(new LivingCallBack(this.method_10), 4500);
		}

		private void method_5()
		{
			base.Body.PlayMovie("beatD", 3000, 0);
			base.Body.CallFuction(new LivingCallBack(this.method_8), 4000);
		}

		private void method_6()
		{
			base.Body.CurrentDamagePlus = 2.5f;
			this.player_0 = base.Game.FindRandomPlayer();
			if (this.player_0 != null)
			{
				((PVEGame)base.Game).SendGameFocus(this.player_0, 0, 1000);
				this.physicalObj_0 = ((PVEGame)base.Game).Createlayer(this.player_0.X, this.player_0.Y, "moive", "asset.game.fifteen.305b", "out", 1, 1);
				base.Body.CallFuction(new LivingCallBack(this.method_7), 2000);
				base.Body.CallFuction(new LivingCallBack(this.method_10), 1000);
			}
		}

		private void method_7()
		{
			if (this.physicalObj_0 != null)
			{
				base.Game.RemovePhysicalObj(this.physicalObj_0, true);
				this.physicalObj_0 = null;
			}
		}

		private void method_8()
		{
			base.Body.CurrentDamagePlus = 7.5f;
			this.player_0 = base.Game.FindRandomPlayer();
			if (this.player_0 != null)
			{
				((PVEGame)base.Game).SendGameFocus(this.player_0, 0, 1000);
				this.physicalObj_0 = ((PVEGame)base.Game).Createlayer(this.player_0.X, this.player_0.Y, "moive", "asset.game.fifteen.305d", "out", 1, 1);
				base.Body.CallFuction(new LivingCallBack(this.method_9), 2000);
				base.Body.CallFuction(new LivingCallBack(this.method_10), 1000);
			}
		}

		private void method_9()
		{
			((PVEGame)base.Game).SendGameFocus(base.Body, 0, 1000);
			base.Body.PlayMovie("born", 1000, 0);
			if (this.physicalObj_0 != null)
			{
				base.Game.RemovePhysicalObj(this.physicalObj_0, true);
				this.physicalObj_0 = null;
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
				if (!allFightPlayer.IsLiving || allFightPlayer.X <= 1000)
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
			if (this.int_0 == 1)
			{
				this.method_3();
				this.int_0++;
				return;
			}
			if (this.int_0 == 2)
			{
				this.method_4();
				this.int_0++;
				return;
			}
			this.method_5();
			this.player_0 = base.Game.FindRandomPlayer();
			if (this.player_0 != null)
			{
				if (this.player_0.X < 400)
				{
					this.int_0 = 0;
					return;
				}
				this.int_0 = 1;
			}
		}

		public override void OnStopAttacking()
		{
			base.OnStopAttacking();
		}
	}
}