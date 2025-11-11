using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;
using System;

namespace GameServerScript.AI.NPC
{
	public class NPCLauDaiPhepThuat02 : ABrain
	{
		private int m_attackTurn = 0;

		private PhysicalObj moive;

		private int npcID = 71075;

		private int npcID2 = 71076;

		private int npcID3 = 71077;

		private int isSay = 0;

		public override void OnBeginSelfTurn()
		{
			base.OnBeginSelfTurn();
		}

		public override void OnBeginNewTurn()
		{
			base.OnBeginNewTurn();
			base.Body.CurrentShootMinus = 1f;
			this.isSay = 0;
		}

		public override void OnCreated()
		{
			base.OnCreated();
		}

		public override void OnStartAttacking()
		{
			base.Body.Direction = base.Game.FindlivingbyDir(base.Body);
			bool flag = false;
			foreach (Player current in base.Game.GetAllFightPlayers())
			{
				if (current.IsLiving && current.X > 1150)
				{
					flag = true;
				}
			}
			if (flag)
			{
				this.KillAttack();
			}
			else if (this.m_attackTurn == 0)
			{
				this.TankA();
				this.m_attackTurn++;
			}
			else
			{
				this.TankB();
				this.m_attackTurn = 0;
			}
		}

		public override void OnStopAttacking()
		{
			base.OnStopAttacking();
		}

		private void KillAttack()
		{
			Player player = base.Game.FindRandomPlayer();
			base.Body.CurrentDamagePlus = 5000f;
			base.Body.PlayMovie("beatA", 3000, 0);
			base.Body.RangeAttacking(player.X - 300, player.X + 300, "cry", 4000, null);
			base.Body.CallFuction(new LivingCallBack(this.HieuUngA), 3000);
		}

		private void TankA()
		{
			Player player = base.Game.FindRandomPlayer();
			base.Body.CurrentDamagePlus = 100f;
			base.Body.PlayMovie("beatA", 1700, 0);
			int num = base.Game.Random.Next(200, 300);
			base.Body.RangeAttacking(player.X - num, player.X + num, "cry", 4000, null);
			base.Body.CallFuction(new LivingCallBack(this.HieuUngA), 3000);
		}

		private void TankB()
		{
			base.Body.PlayMovie("beatB", 1700, 0);
			base.Body.CallFuction(new LivingCallBack(this.Call), 4000);
		}

		private void Call()
		{
			LivingConfig livingConfig = ((PVEGame)base.Game).BaseLivingConfig();
			livingConfig.IsFly = true;
			((PVEGame)base.Game).CreateNpc(this.npcID, 1000, 636, 1, -1, livingConfig);
			((PVEGame)base.Game).CreateNpc(this.npcID2, 830, 580, 1, -1, livingConfig);
			((PVEGame)base.Game).CreateNpc(this.npcID3, 744, 537, 1, -1, livingConfig);
		}

		private void HieuUngA()
		{
			Player player = base.Game.FindRandomPlayer();
			this.moive = ((PVEGame)base.Game).Createlayer(player.X, player.Y, "moive", "asset.game.fifteen.424a", "out", 1, 1);
			base.Body.CallFuction(new LivingCallBack(this.GoOut), 4000);
		}

		public override void OnKillPlayerSay()
		{
			base.OnKillPlayerSay();
		}

		public override void OnDiedSay()
		{
		}

		private void GoOut()
		{
			if (this.moive != null)
			{
				base.Game.RemovePhysicalObj(this.moive, true);
				this.moive = null;
			}
		}

		public override void OnShootedSay()
		{
			if (this.isSay == 0 && base.Body.IsLiving)
			{
				this.isSay = 1;
			}
		}
	}
}
