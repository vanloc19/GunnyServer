using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Effects;
using Game.Logic.Phy.Object;
using System;
using System.Collections.Generic;

namespace GameServerScript.AI.NPC
{
	public class ThirteenSimpleBrotherNpc : ABrain
	{
		private int int_0;

		private SimpleBoss simpleBoss_0;

		protected Player m_targer;

		private PhysicalObj physicalObj_0;

		private int int_1;

		private int int_2;

		private int int_3;

		public ThirteenSimpleBrotherNpc()
		{
			
			this.int_1 = 13005;
			this.int_2 = 80;
			this.int_3 = 25;
			
		}

		private void method_0()
		{
			base.Body.Properties1 = 0;
			base.Body.Say("Tà thần vĩ đại, hãy ban sức mạnh của ngài cho chúng tôi!", 1, 1000);
			base.Body.PlayMovie("walk", 2000, 0);
			base.Body.BoltMove(1660, 485, 2500);
			List<Player> players = new List<Player>();
			foreach (Player allLivingPlayer in base.Game.GetAllLivingPlayers())
			{
				if (!allLivingPlayer.IsLiving || allLivingPlayer.X < 1003 || allLivingPlayer.X > 1300 || allLivingPlayer.Y > 620)
				{
					continue;
				}
				players.Add(allLivingPlayer);
			}
			if (players.Count <= 0)
			{
				base.Body.PlayMovie("beatB", 4000, 0);
				base.Body.CallFuction(new LivingCallBack(this.method_1), 11000);
				this.simpleBoss_0.Properties1 = 1;
				base.Body.PlayMovie("walk", 12000, 0);
				base.Body.BoltMove(1604, 594, 12500);
			}
			else
			{
				base.Body.CurrentDamagePlus = 2f;
				base.Body.PlayMovie("castA", 4000, 0);
				this.physicalObj_0.PlayMovie("beatA", 5000, 0);
				foreach (Player player in base.Game.GetAllLivingPlayers())
				{
					if (player.IsLiving && players.Contains(player))
					{
						base.Body.BeatDirect(player, "", 7500, 1, 1);
					}
					player.AddEffect(new AddDamageTurnEffect(this.int_2, 2), 7000);
					player.AddEffect(new AddGuardTurnEffect(this.int_3, 2), 7000);
				}
				(base.Game as PVEGame).SendObjectFocus(this.simpleBoss_0, 0, 9000, 0);
				this.simpleBoss_0.Say("Có chuyện gì vậy? Cúng tế bị sai cmnr.", 0, 10500, 2000);
				base.Body.PlayMovie("walk", 8000, 0);
				base.Body.BoltMove(1604, 594, 8500);
				base.Body.CallFuction(new LivingCallBack(this.method_1), 13000);
			}
			this.simpleBoss_0.Delay = base.Game.GetLowDelayTurn() - 1;
			(base.Body as SimpleBoss).Delay = base.Game.GetHighDelayTurn() + 1;
		}

		private void method_1()
		{
			base.Game.RemovePhysicalObj(this.physicalObj_0, true);
		}

		private void method_2()
		{
			base.Body.Properties1 = 1;
			base.Body.Say("Lễ cúng tế sắp bắt đầu! Chúng ta sẽ có sức mạnh của tà thần! Hãy cố lên….", 1, 1000);
			base.Body.PlayMovie("call", 1100, 4000);
			base.Body.CallFuction(new LivingCallBack(this.method_3), 3000);
		}

		private void method_3()
		{
			this.physicalObj_0 = (base.Game as PVEGame).Createlayer(1150, 572, "front", "asset.game.ten.jitan", "1", 1, 0);
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
			base.Body.Properties1 = 0;
		}

		public override void OnDie()
		{
			base.OnDie();
		}

		public override void OnStartAttacking()
		{
			base.OnStartAttacking();
			if ((int)base.Body.Properties1 == 0)
			{
				this.method_2();
				return;
			}
			this.method_0();
		}

		public override void OnStopAttacking()
		{
			base.OnStopAttacking();
		}
	}
}