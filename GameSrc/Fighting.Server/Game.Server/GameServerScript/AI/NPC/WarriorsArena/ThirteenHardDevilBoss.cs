using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;
using System;
using System.Collections.Generic;

namespace GameServerScript.AI.NPC
{
	public class ThirteenHardDevilBoss : ABrain
	{
		private int int_0;

		private SimpleBoss simpleBoss_0;

		protected Player m_targer;

		protected Player m_targerBig1000;

		private SimpleNpc simpleNpc_0;

		private List<PhysicalObj> list_0;

		private PhysicalObj physicalObj_0;

		private List<PhysicalObj> list_1;

		private int int_1;

		private int int_2;

		public ThirteenHardDevilBoss()
		{
			
			this.list_0 = new List<PhysicalObj>();
			this.list_1 = new List<PhysicalObj>();
			this.int_1 = 13207;
			this.int_2 = 13210;
			
		}

		private void method_0()
		{
			base.Body.ChangeDirection(-1, 500);
			base.Body.MoveTo(900, 400, "fly", 1000, new LivingCallBack(this.method_1), 10);
		}

		private void method_1()
		{
			base.Body.CurrentDamagePlus = 2.5f;
			base.Body.PlayMovie("beatA", 1200, 0);
			base.Body.CallFuction(new LivingCallBack(this.method_2), 2500);
			base.Body.RangeAttacking(base.Body.X - 10000, base.Body.X + 10000, "cry", 4500, true);
			base.Body.PlayMovie("beatC", 4000, 2000);
			base.Body.CallFuction(new LivingCallBack(this.method_12), 5000);
		}

		private void method_10()
		{
			this.m_targer = base.Game.FindRandomPlayer();
			base.Body.Say("Coi ta cắt chym tụi mi đây.", 0, 1000);
			base.Body.MoveTo(this.m_targer.X, this.m_targer.Y, "fly", 1500, new LivingCallBack(this.method_11), 10);
		}

		private void method_11()
		{
			base.Body.BeatDirect(this.m_targer, "beatE", 0, 1, 1);
			base.Body.CallFuction(new LivingCallBack(this.method_12), 2000);
		}

		private void method_12()
		{
			int num = base.Game.Random.Next(100, 1800);
			int num1 = base.Game.Random.Next(100, 850);
			base.Body.MoveTo(num, num1, "fly", 1000, new LivingCallBack(this.method_14), 10);
		}

		private void method_13()
		{
			this.simpleBoss_0.Properties1 = 1;
			this.simpleBoss_0.Config.CancelGuard = true;
			(base.Game as PVEGame).SendLivingActionMapping(this.simpleBoss_0, "stand", "standC");
			(base.Game as PVEGame).SendLivingActionMapping(this.simpleBoss_0, "cry", "standC");
		}

		private void method_14()
		{
			if (this.physicalObj_0 != null)
			{
				(base.Game as PVEGame).SendObjectFocus(this.physicalObj_0, 1, 1000, 0);
				if (this.simpleBoss_0.X < this.physicalObj_0.X - 160 || this.simpleBoss_0.X > this.physicalObj_0.X + 160)
				{
					base.Body.CurrentDamagePlus = 10f;
					this.physicalObj_0.PlayMovie("beatA", 2000, 3000);
					foreach (Player allLivingPlayer in base.Game.GetAllLivingPlayers())
					{
						if (allLivingPlayer.X < this.physicalObj_0.X - 160 || allLivingPlayer.X > this.physicalObj_0.X + 160)
						{
							continue;
						}
						base.Body.BeatDirect(allLivingPlayer, "", 3000, 1, 1);
					}
				}
				else
				{
					this.physicalObj_0.PlayMovie("beatB", 2000, 0);
					this.simpleBoss_0.PlayMovie("ready", 3000, 0);
					this.simpleBoss_0.PlayMovie("standC", 3700, 2000);
					base.Body.CallFuction(new LivingCallBack(this.method_13), 5700);
				}
				base.Body.CallFuction(new LivingCallBack(this.method_19), 5000);
				return;
			}
			if (this.list_1.Count > 0)
			{
				List<Player> players = base.Game.FindRangePlayers(this.simpleBoss_0.X - 100, this.simpleBoss_0.X + 100);
				if (players.Count >= 4)
				{
					(base.Game as PVEGame).SendObjectFocus(this.simpleBoss_0, 1, 1000, 0);
					base.Body.CallFuction(new LivingCallBack(this.method_16), 2500);
					base.Body.CallFuction(new LivingCallBack(this.method_15), 3500);
					foreach (PhysicalObj list1 in this.list_1)
					{
						list1.PlayMovie("beatA", 5000, 0);
					}
					this.simpleBoss_0.PlayMovie("ready", 6500, 0);
					this.simpleBoss_0.PlayMovie("standC", 7200, 2000);
					base.Body.CallFuction(new LivingCallBack(this.method_13), 9200);
					return;
				}
				base.Body.CurrentDamagePlus = 5f;
				foreach (PhysicalObj physicalObj in this.list_1)
				{
					physicalObj.PlayMovie("beatA", 1000, 4000);
					players = base.Game.FindRangePlayers(physicalObj.X - 25, physicalObj.X + 25);
					if (players.Count > 0)
					{
						foreach (Player player in players)
						{
							base.Body.BeatDirect(player, "", 2000, 1, 1);
						}
					}
					base.Body.CallFuction(new LivingCallBack(this.method_16), 4600);
				}
			}
		}

		private void method_15()
		{
			this.list_1.Add((base.Game as PVEGame).Createlayer(this.simpleBoss_0.X - 100, this.simpleBoss_0.Y - 170, "", "asset.game.ten.laotie2", "", 1, 0));
			this.list_1.Add((base.Game as PVEGame).Createlayer(this.simpleBoss_0.X - 80, this.simpleBoss_0.Y - 200, "", "asset.game.ten.laotie2", "", 1, 0));
			this.list_1.Add((base.Game as PVEGame).Createlayer(this.simpleBoss_0.X + 100, this.simpleBoss_0.Y - 170, "", "asset.game.ten.laotie", "", 1, 0));
			this.list_1.Add((base.Game as PVEGame).Createlayer(this.simpleBoss_0.X + 80, this.simpleBoss_0.Y - 200, "", "asset.game.ten.laotie", "", 1, 0));
		}

		private void method_16()
		{
			foreach (PhysicalObj list1 in this.list_1)
			{
				base.Game.RemovePhysicalObj(list1, true);
			}
			this.list_1 = new List<PhysicalObj>();
		}

		private void method_17()
		{
			this.m_targerBig1000 = base.Game.FindRandomPlayer();
			base.Body.Say("Sao to và nặng value thế nhỉ?", 0, 1000);
			base.Body.PlayMovie("beatD", 2000, 0);
			(base.Game as PVEGame).SendFreeFocus(this.m_targerBig1000.X, this.m_targerBig1000.Y - 150, 1, 4000, 3000);
			base.Body.CallFuction(new LivingCallBack(this.method_18), 5000);
		}

		private void method_18()
		{
			this.physicalObj_0 = (base.Game as PVEGame).Createlayer(this.m_targerBig1000.X, this.m_targerBig1000.Y, "", "asset.game.ten.chengtuo", "", 1, 0);
		}

		private void method_19()
		{
			base.Game.RemovePhysicalObj(this.physicalObj_0, true);
			this.physicalObj_0 = null;
		}

		private void method_2()
		{
			this.list_0.Add(((PVEGame)base.Game).CreateLayerTop(500, 300, "", "asset.game.4.heip", "", 1, 1));
		}

		private void method_3()
		{
			base.Body.Say(string.Concat(base.Game.ListPlayersName(), ", ta muốn ăn thịt xông khói đây..."), 0, 1000);
			base.Body.PlayMovie("beatD", 2000, 4000);
			base.Body.CallFuction(new LivingCallBack(this.method_4), 4000);
		}

		private void method_4()
		{
			foreach (Player allLivingPlayer in base.Game.GetAllLivingPlayers())
			{
				this.list_1.Add(((PVEGame)base.Game).Createlayer(allLivingPlayer.X, allLivingPlayer.Y - 50, "", "asset.game.ten.laotie", "born", 1, 1));
			}
		}

		private void method_5()
		{
			this.m_targer = base.Game.FindRandomPlayer();
			base.Body.ChangeDirection(base.Game.FindRandomPlayer(), 1000);
			base.Body.Say("Bắt chúng nhốt lại cho ta.", 0, 2000);
			base.Body.PlayMovie("beatD", 3000, 0);
			base.Body.CallFuction(new LivingCallBack(this.method_7), 4000);
			(base.Game as PVEGame).SendObjectFocus(this.m_targer, 1, 6000, 0);
			base.Body.CallFuction(new LivingCallBack(this.method_8), 7000);
			base.Body.CallFuction(new LivingCallBack(this.method_9), 8000);
			base.Body.CallFuction(new LivingCallBack(this.method_6), 8000);
		}

		private void method_6()
		{
			this.m_targer.BoltMove(this.simpleNpc_0.X, this.simpleNpc_0.Y, 100);
			((PVEGame)base.Game).SendObjectFocus(this.simpleNpc_0, 1, 800, 0);
			this.simpleNpc_0.PlayMovie("AtoB", 1500, 2000);
		}

		private void method_7()
		{
			int num = base.Game.Random.Next(200, 1700);
			LivingConfig livingConfig = ((PVEGame)base.Game).BaseLivingConfig();
			livingConfig.CanTakeDamage = false;
			this.simpleNpc_0 = ((SimpleBoss)base.Body).CreateChild(this.int_2, num, 1000, false, livingConfig);
			this.simpleNpc_0.Properties1 = this.m_targer.Id;
			(base.Game as PVEGame).SendObjectFocus(this.simpleNpc_0, 1, 0, 0);
		}

		private void method_8()
		{
			this.list_0.Add(((PVEGame)base.Game).Createlayer(this.m_targer.X, this.m_targer.Y, "", "asset.game.4.lanhuo", "", 1, 1));
		}

		private void method_9()
		{
			this.m_targer.SetVisible(false);
			this.m_targer.BlockTurn = true;
		}

		public override void OnBeginNewTurn()
		{
			base.OnBeginNewTurn();
			this.m_body.CurrentDamagePlus = 1f;
			this.m_body.CurrentShootMinus = 1f;
			if (this.list_0 != null && this.list_0.Count > 0)
			{
				foreach (PhysicalObj list0 in this.list_0)
				{
					base.Game.RemovePhysicalObj(list0, true);
				}
				this.list_0.Clear();
			}
		}

		public override void OnBeginSelfTurn()
		{
			base.OnBeginSelfTurn();
			if (this.simpleBoss_0 == null)
			{
				SimpleBoss[] simpleBossArray = ((PVEGame)base.Game).FindLivingTurnBossWithID(this.int_1);
				if (simpleBossArray.Length != 0)
				{
					this.simpleBoss_0 = simpleBossArray[0];
				}
			}
		}

		public override void OnCreated()
		{
			base.OnCreated();
			base.Body.Properties1 = 0;
		}

		public override void OnDie()
		{
			base.OnDie();
		}

		public override void OnStartAttacking()
		{
			base.OnStartAttacking();
			switch (this.int_0)
			{
				case 0:
				{
					this.method_17();
					break;
				}
				case 1:
				{
					this.method_10();
					break;
				}
				case 2:
				{
					this.method_5();
					break;
				}
				case 3:
				{
					this.method_3();
					break;
				}
				case 4:
				{
					this.method_0();
					this.int_0 = -1;
					break;
				}
			}
			this.int_0++;
		}

		public override void OnStopAttacking()
		{
			base.OnStopAttacking();
		}
	}
}