using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;
using System;
using System.Collections.Generic;

namespace GameServerScript.AI.NPC
{
	public class ThirteenHardNioBoss : ABrain
	{
		private int int_0;

		private SimpleBoss simpleBoss_0;

		protected Player m_targer;

		private List<PhysicalObj> TouscBjXuol;

		private int int_1;

		private int int_2;

		private int int_3;

		private string[] string_0;

		public ThirteenHardNioBoss()
		{
			
			this.TouscBjXuol = new List<PhysicalObj>();
			this.int_1 = 13201;
			this.int_2 = 30000;
			this.int_3 = 130000;
			this.string_0 = new string[] { "Phê phê phê. Coi đánh ta kiểu gì?", "Ôi thật thần kỳ. Buff máu phê vãi", "Buff cho phê, phê như con tê tê..", "Đánh bại ta đâu có dễ. Coi ta này." };
			
		}

		private void method_0()
		{
			List<Player> players = new List<Player>();
			if (this.simpleBoss_0.IsLiving)
			{
				foreach (Player allLivingPlayer in base.Game.GetAllLivingPlayers())
				{
					if (!allLivingPlayer.IsFrost)
					{
						continue;
					}
					players.Add(allLivingPlayer);
				}
			}
			base.Body.PlayMovie("beat", 1000, 0);
			if (players.Count <= 0)
			{
				base.Body.CurrentDamagePlus = 1.5f;
				base.Body.RangeAttacking(base.Body.X - 10000, base.Body.X + 10000, "cry", 3200, false);
			}
			else
			{
				base.Body.CurrentDamagePlus = 1000f;
				foreach (Player player in players)
				{
					base.Body.BeatDirect(player, "", 3200, 1, 1);
				}
			}
		}

		private void method_1()
		{
			base.Body.CurrentDamagePlus = 1.8f;
			this.m_targer = base.Game.FindRandomPlayer();
			base.Body.ChangeDirection(this.m_targer, 500);
			if (base.Body.ShootPoint(this.m_targer.X, this.m_targer.Y, 61, 1000, 10000, 1, 1.7f, 1900))
			{
				base.Body.PlayMovie("beat2", 1000, 4000);
			}
		}

		private void method_2()
		{
			int num = base.Game.Random.Next(0, (int)this.string_0.Length);
			base.Body.Say(this.string_0[num], 0, 1000);
			base.Body.PlayMovie("renew", 1500, 3500);
			base.Body.CallFuction(new LivingCallBack(this.method_3), 3000);
		}

		private void method_3()
		{
			base.Body.AddBlood(this.int_2);
			if (this.simpleBoss_0.IsLiving)
			{
				(base.Game as PVEGame).SendObjectFocus(this.simpleBoss_0, 0, 1500, 0);
				base.Body.CallFuction(new LivingCallBack(this.method_4), 3000);
			}
		}

		private void method_4()
		{
			this.simpleBoss_0.AddBlood(this.int_3);
		}

		private void method_5(List<Player> YDi0eVVbegHrQXpBwR7)
		{
			base.Body.CurrentDamagePlus = 1000f;
			foreach (Player yDi0eVVbegHrQXpBwR7 in YDi0eVVbegHrQXpBwR7)
			{
				base.Body.BeatDirect(yDi0eVVbegHrQXpBwR7, "", 2000, 1, 1);
			}
		}

		public override void OnBeginNewTurn()
		{
			base.OnBeginNewTurn();
			this.m_body.CurrentDamagePlus = 1f;
			this.m_body.CurrentShootMinus = 1f;
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
				if (!allLivingPlayer.IsLiving || allLivingPlayer.X <= 1066)
				{
					continue;
				}
				players.Add(allLivingPlayer);
			}
			if (players.Count > 0)
			{
				this.method_5(players);
				return;
			}
			switch (this.int_0)
			{
				case 0:
				{
					this.method_1();
					break;
				}
				case 1:
				{
					this.method_2();
					break;
				}
				case 2:
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