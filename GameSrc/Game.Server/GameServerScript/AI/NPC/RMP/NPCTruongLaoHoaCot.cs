using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Effects;
using Game.Logic.Phy.Object;
using System;
using System.Collections.Generic;

namespace GameServerScript.AI.NPC
{
	public class NPCTruongLaoHoaCot : ABrain
	{
		private int m_attackTurn = 0;

		private PhysicalObj moive;

		private int npcID = 71144;

		private int isSay = 0;


        #region NPC 说话内容
        private static string[] Chat1 = new string[] {
             "Mấy con lợn này!",
        };

        private static string[] Chat2 = new string[]{
             "Bố oánh chết mm!",
        };

        private static string[] Chat3 = new string[]{
            "Gảy le thần chưởng!"
        };

        private static string[] Chat4 = new string[]{
            "Nghe nhạc sếp đi , mới ngộ ra chân lý!"
        };

        private static string[] Chat5 = new string[]{
            "Cái đìn địtttt mm"
        };
        #endregion

        public override void OnBeginSelfTurn()
		{
			base.OnBeginSelfTurn();
		}

		public override void OnBeginNewTurn()
		{
			base.OnBeginNewTurn();
			base.Body.CurrentDamagePlus = 1f;
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
				if (current.IsLiving)
				{
					if (current.X > base.Body.X && current.X - base.Body.X < 150)
					{
						flag = true;
					}
					if (current.X < base.Body.X && base.Body.X - current.X < 150)
					{
						flag = true;
					}
				}
			}
			if (flag)
			{
				this.KillAttack();
			}
			else if (this.m_attackTurn == 0)
			{
				this.TankB();
				this.m_attackTurn++;
			}
			else if (this.m_attackTurn == 1)
			{
				this.TankC();
				this.m_attackTurn++;
			}
			else if (this.m_attackTurn == 2)
			{
				this.TankA();
				this.m_attackTurn++;
			}
			else if (this.m_attackTurn == 3)
			{
				this.TankD();
				this.m_attackTurn = 0;
			}
		}

		public override void OnStopAttacking()
		{
			base.OnStopAttacking();
		}

		private void KillAttack()
		{
            int index = Game.Random.Next(0, Chat5.Length);
            Body.Say(Chat5[index], 1, 0);

            Player player = base.Game.FindRandomPlayer();
            base.Body.CurrentDamagePlus = 1f;
            base.Body.PlayMovie("beatA", 3000, 0);
            base.Body.RangeAttacking(player.X - 150, player.X + 150, "cry", 4000, null);
        }

        private void TankA()
        {
            int index = Game.Random.Next(0, Chat1.Length);
            Body.Say(Chat1[index], 1, 0);

            base.Body.PlayMovie("beatA", 1700, 0);
            base.Body.CallFuction(new LivingCallBack(this.Call), 3000);
        }

        private void TankB()
        {
            int index = Game.Random.Next(0, Chat2.Length);
            Body.Say(Chat2[index], 1, 0);

            Player player = base.Game.FindRandomPlayer();
            base.Body.PlayMovie("beatB", 1700, 0);
            base.Body.RangeAttacking(player.X - 100, player.X + 100, "cry", 4000, null);
            base.Body.CallFuction(new LivingCallBack(this.SkillBong), 3000);
        }

        private void TankC()
        {
            int index = Game.Random.Next(0, Chat3.Length);
            Body.Say(Chat3[index], 1, 0);
            Player player = base.Game.FindRandomPlayer();
            base.Body.PlayMovie("beatC", 1700, 0);
            base.Body.RangeAttacking(player.X - 100, player.X + 100, "cry", 4000, null);
        }

        private void TankD()
        {
            int index = Game.Random.Next(0, Chat4.Length);
            Body.Say(Chat4[index], 1, 0);
            Player player = base.Game.FindRandomPlayer();
            base.Body.PlayMovie("beatA", 1700, 0);
            //base.Game.KichNo = true;
            base.Body.CallFuction(new LivingCallBack(this.CallNpcKill), 5000);
        }

        public void CallNpcKill()
		{
			//System.Collections.Generic.List<Living> nPCLivings = base.Game.GetNPCLivings();
			//if (nPCLivings.Count > 0)
			//{
			//	foreach (Living current in nPCLivings)
			//	{
			//		current.StartAttacking();
			//	}
			//}
			//base.Game.KichNo = false;
		}

		private void Call()
		{
			Player player = base.Game.FindRandomPlayer();
			if (player.X < 85)
			{
				((PVEGame)base.Game).CreateNpc(this.npcID, 70, 496, 1, -1);
				((PVEGame)base.Game).CreateNpc(this.npcID, player.X + 80, 496, 1, -1);
			}
			else if (player.X > 1160)
			{
				((PVEGame)base.Game).CreateNpc(this.npcID, 1177, 496, 1, -1);
				((PVEGame)base.Game).CreateNpc(this.npcID, player.X - 80, 496, 1, -1);
			}
			else
			{
				((PVEGame)base.Game).CreateNpc(this.npcID, player.X - 80, 496, 1, -1);
				((PVEGame)base.Game).CreateNpc(this.npcID, player.X + 80, 496, 1, -1);
			}
		}

		private void SkillBong()
		{
			System.Collections.Generic.List<Player> allLivingPlayers = base.Game.GetAllLivingPlayers();
			int blood = base.Game.Random.Next(750, 1000);
			foreach (Player current in allLivingPlayers)
			{
				current.AddEffect(new ContinueReduceBloodEffect(2, blood, current), 0);
			}
		}

		public override void OnKillPlayerSay()
		{
			base.OnKillPlayerSay();
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
