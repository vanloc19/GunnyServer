using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Effects;
using Game.Logic.Phy.Object;
using System;
using System.Collections.Generic;

namespace GameServerScript.AI.NPC
{
	public class ThirteenHardBrynBoss : ABrain
	{
		private int int_0;

		private int int_1;

		private SimpleBoss simpleBoss_0;

		protected Player m_targer;

		private List<PhysicalObj> list_0;

		private int int_2;

		private int int_3;

		private int int_4;

		public ThirteenHardBrynBoss()
		{
			
			this.list_0 = new List<PhysicalObj>();
			this.int_2 = 13204;
			this.int_3 = 50;
			this.int_4 = 8000;
			
		}

		private void method_0()
		{
			this.simpleBoss_0.Delay = base.Game.GetHighDelayTurn() + 1;
			this.m_targer = base.Game.FindRandomPlayer();
			if (this.m_targer != null && this.m_targer.IsLiving)
			{
				base.Body.ChangeDirection(this.m_targer, 1000);
				if (this.m_targer.Y <= 620)
				{
					base.Body.CurrentDamagePlus = 15f;
					base.Body.Say("Ngươi dám leo lên trên đó à? Chết đi!!!", 0, 2000);
					base.Body.PlayMovie("beatC", 3000, 0);
					base.Body.BeatDirect(this.m_targer, "", 4000, 3, 1);
				}
				else if (base.Body.ShootPoint(this.m_targer.X, this.m_targer.Y, 55, 1000, 10000, 1, 1.5f, 2900))
				{
					base.Body.PlayMovie("beatB", 2000, 3000);
				}
			}
			if (this.int_1 == 0)
			{
				base.Body.CallFuction(new LivingCallBack(this.method_0), 4000);
			}
			this.int_1++;
		}

		private void method_1()
		{
			this.simpleBoss_0.Delay = base.Game.GetHighDelayTurn() + 1;
			if ((int)base.Body.Properties1 != 1)
			{
				base.Body.CurrentDamagePlus = 2.3f;
				base.Body.Say("Đậu xanh rau muống dám phá nghi lễ của ta.", 1, 2000);
				base.Body.PlayMovie("beatC", 3000, 0);
				base.Body.RangeAttacking(base.Body.X - 10000, base.Body.X + 10000, "", 4000, false);
				return;
			}
			base.Body.CurrentDamagePlus = 10f;
			base.Body.Say("Hãy xem sức mạnh của tà thần đây. Đây mới chính là sức mạnh thực sự của ta.", 1, 1000);
			(base.Game as PVEGame).SendPlayersPicture(base.Body, (int)eLivingPictureType.Damage, true);
			(base.Game as PVEGame).SendPlayersPicture(base.Body, (int)eLivingPictureType.Guard, true);
			base.Body.CallFuction(new LivingCallBack(this.method_2), 2000);
			base.Body.Properties1 = 2;
		}

		private void method_10()
		{
			foreach (Player allLivingPlayer in base.Game.GetAllLivingPlayers())
			{
				allLivingPlayer.SpeedMultX(3);
			}
		}

		private void method_11(List<Player> TKR579VXJvb19bSkrVo)
		{
			base.Body.CurrentDamagePlus = 1000f;
			base.Body.Say("Chán sống hay sao mà đứng gần ta?", 1, 1000);
			base.Body.PlayMovie("beatC", 2000, 0);
			foreach (Player tKR579VXJvb19bSkrVo in TKR579VXJvb19bSkrVo)
			{
				base.Body.BeatDirect(tKR579VXJvb19bSkrVo, "", 3500, 1, 1);
			}
		}

		private void method_2()
		{
			this.list_0.Add((base.Game as PVEGame).Createlayer(base.Body.X, base.Body.Y, "", "asset.game.ten.up", "", 1, 0));
			base.Body.PlayMovie("beatC", 2000, 5000);
			base.Body.CallFuction(new LivingCallBack(this.method_3), 3000);
			base.Body.RangeAttacking(base.Body.X - 10000, base.Body.X + 10000, "", 4000, false);
		}

		private void method_3()
		{
			base.Body.AddBlood(this.int_4);
		}

		private void method_4()
		{
			base.Body.Say("Hãy mau thực hiện nghi thức cúng tế, sức mạnh tà thần sẽ thuộc về ta!", 1000, 3000);
			base.Body.Properties1 = 0;
			this.simpleBoss_0.Delay = base.Game.GetLowDelayTurn() - 1;
		}

		private void method_5()
		{
			this.simpleBoss_0.Delay = base.Game.GetHighDelayTurn() + 1;
			base.Body.PlayMovie("callA", 1000, 0);
			this.m_targer = base.Game.FindRandomPlayer();
			if (this.m_targer != null)
			{
				(base.Game as PVEGame).SendObjectFocus(this.m_targer, 1, 2000, 0);
			}
			base.Body.CallFuction(new LivingCallBack(this.method_6), 2800);
		}

		private void method_6()
		{
			base.Body.CurrentDamagePlus = 1.8f;
			foreach (Player allLivingPlayer in base.Game.GetAllLivingPlayers())
			{
				int num = base.Game.Random.Next(0, 3);
				if (num == 0)
				{
					this.list_0.Add((base.Game as PVEGame).Createlayer(allLivingPlayer.X, allLivingPlayer.Y, "", "asset.game.ten.jiaodu", "", 1, 0));
					allLivingPlayer.AddEffect(new LockDirectionEffect(2), 1500);
				}
				else if (num == 1)
				{
					this.list_0.Add((base.Game as PVEGame).Createlayer(allLivingPlayer.X, allLivingPlayer.Y, "", "asset.game.ten.pilao", "", 1, 0));
					allLivingPlayer.AddEffect(new ReduceStrengthEffect(2, this.int_3), 1500);
				}
				else
				{
					this.list_0.Add((base.Game as PVEGame).Createlayer(allLivingPlayer.X, allLivingPlayer.Y, "", "asset.game.ten.pilao", "", 1, 0));
					base.Body.BeatDirect(allLivingPlayer, "", 1500, 1, 1);
				}
				base.Body.BeatDirect(allLivingPlayer, "", 1500, 1, 1);
			}
		}

		private void method_7()
		{
			this.m_targer = base.Game.FindRandomPlayer();
			if (this.m_targer != null)
			{
				(base.Game as PVEGame).SendObjectFocus(this.m_targer, 1, 2000, 0);
			}
			base.Body.PlayMovie("callA", 1000, 0);
			base.Body.CallFuction(new LivingCallBack(this.method_8), 3000);
		}

		private void method_8()
		{
			foreach (Player allLivingPlayer in base.Game.GetAllLivingPlayers())
			{
				this.list_0.Add((base.Game as PVEGame).Createlayer(allLivingPlayer.X, allLivingPlayer.Y, "", "asset.game.ten.baozha", "", 1, 0));
			}
			base.Body.CallFuction(new LivingCallBack(this.method_9), 1200);
		}

		private void method_9()
		{
			foreach (Player allLivingPlayer in base.Game.GetAllLivingPlayers())
			{
				if (allLivingPlayer.IsLiving)
				{
					allLivingPlayer.SpeedMultX(20);
					if (base.Body.FindDirection(allLivingPlayer) != 1)
					{
						allLivingPlayer.StartSpeedMult(allLivingPlayer.X + base.Game.Random.Next(400, 600), allLivingPlayer.Y, 0);
					}
					else
					{
						allLivingPlayer.StartSpeedMult(allLivingPlayer.X - base.Game.Random.Next(400, 600), allLivingPlayer.Y, 0);
					}
				}
				base.Body.BeatDirect(allLivingPlayer, "", 1500, 1, 1);
			}
			base.Body.CallFuction(new LivingCallBack(this.method_10), 5000);
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
			if ((int)base.Body.Properties1 == 2)
			{
				(base.Game as PVEGame).SendPlayersPicture(base.Body, (int)eLivingPictureType.Damage, false);
				(base.Game as PVEGame).SendPlayersPicture(base.Body, (int)eLivingPictureType.Guard, false);
				base.Body.Properties1 = 0;
			}
		}

		public override void OnBeginSelfTurn()
		{
			base.OnBeginSelfTurn();
			if (this.simpleBoss_0 == null)
			{
				SimpleBoss[] simpleBossArray = ((PVEGame)base.Game).FindLivingTurnBossWithID(this.int_2);
				if (simpleBossArray.Length != 0)
				{
					this.simpleBoss_0 = simpleBossArray[0];
				}
			}
			this.int_1 = 0;
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
			List<Player> players = new List<Player>();
			foreach (Player allLivingPlayer in base.Game.GetAllLivingPlayers())
			{
				if (!allLivingPlayer.IsLiving || base.Body.X - 150 > allLivingPlayer.X || base.Body.X + 150 < allLivingPlayer.X || allLivingPlayer.Y < 780)
				{
					continue;
				}
				players.Add(allLivingPlayer);
			}
			if (players.Count > 0)
			{
				this.method_11(players);
				return;
			}
			switch (this.int_0)
			{
				case 0:
				{
					this.method_7();
					break;
				}
				case 1:
				{
					this.method_5();
					break;
				}
				case 2:
				{
					this.method_4();
					break;
				}
				case 3:
				{
					this.method_1();
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