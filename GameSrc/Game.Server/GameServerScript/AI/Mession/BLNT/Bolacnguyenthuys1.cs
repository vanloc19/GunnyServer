using Bussiness;
using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;
using System.Collections.Generic;

namespace GameServerScript.AI.Messions
{
	public class Bolacnguyenthuys1 : AMissionControl
	{
		private List<SimpleNpc> m_npcList = new List<SimpleNpc>();

		private SimpleBoss m_dauThan;

		protected int m_maxBlood;

		protected int m_blood;

		private SimpleBoss m_bossBorn;

		private int m_chiensiID = 33020;

		private int m_dauthanID = 33070;

		private int m_dungsiID = 33050;

		private int m_bornBossID = 33080;

		private int m_npcCreateCount = 5;

		private int m_bossRecoveredCount = 0;

		private SimpleBoss m_dungSi;

		private int m_dungSiCreateCount = 2;

		public override int CalculateScoreGrade(int score)
		{
			base.CalculateScoreGrade(score);
			if (score > 1750)
			{
				return 3;
			}
			if (score > 1675)
			{
				return 2;
			}
			if (score > 1600)
			{
				return 1;
			}
			return 0;
		}

		public override void OnPrepareNewSession()
		{
			base.OnPrepareNewSession();
			int[] npcIds = new int[4]
			{
				m_chiensiID,
				m_dauthanID,
				m_dungsiID,
				m_bornBossID
			};
			int[] npcIds2 = new int[3]
			{
				m_chiensiID,
				m_dauthanID,
				m_dungsiID
			};
			base.Game.LoadResources(npcIds);
			base.Game.LoadNpcGameOverResources(npcIds2);
			base.Game.AddLoadingFile(1, "bombs/58.swf", "tank.resource.bombs.Bomb58");
			base.Game.SetMap(1125);
		}

		public override void OnStartGame()
		{
			base.OnStartGame();
			m_bossBorn = base.Game.CreateBoss(m_bornBossID, 210, 444, 1, 0, "");
			m_bossBorn.FallFrom(m_bossBorn.X, m_bossBorn.Y, "", 0, 0, 2000);
			m_bossBorn.Say("Các ngươi hết đường thoát rồi, giữ những tên này làm vật tế! Ngư...", 0, 1800, 1500);
			m_bossBorn.PlayMovie("castA", 3000, 0);

			m_bossBorn.CallFuction(CreateBoss, 4200);
			m_bossBorn.CallFuction(CreateNPC, 4200);
			m_bossBorn.PlayMovie("out", 5300, 1000);
			m_bossBorn.CallFuction(CreateOutGame, 6500);

			m_bossBorn.CallFuction(CreateStarGame, 8000);
		}

		public void CreateStarGame()
		{
			LivingConfig livingConfig = base.Game.BaseLivingConfig();
			livingConfig.IsHelper = true;
			livingConfig.IsTurn = false;
			livingConfig.ReduceBloodStart = 2;
			m_dauThan = base.Game.CreateBoss(m_dauthanID, 1052, 709, -1, 1, "born", livingConfig);
			m_dauThan.SetRelateDemagemRect(m_dauThan.NpcInfo.X, m_dauThan.NpcInfo.Y, m_dauThan.NpcInfo.Width, m_dauThan.NpcInfo.Height);
			m_dauThan.FallFrom(m_dauThan.X, m_dauThan.Y, "", 0, 0, 1000, null);
			m_dauThan.Say(LanguageMgr.GetTranslation("Mau dùng thiên sứ tiếp thêm sức mạnh để mở cánh cổng phong ấn , hãy ngăn chặn bọn chúng phá cánh cổng"), 0, 2500, 2000);
			Game.SendLivingActionMapping(m_dauThan, "renew", "nothing");
		}

		public void CreateBoss()
		{
			m_dungSi = base.Game.CreateBoss(m_dungsiID, 1250, 720, 1, 0, "");
			m_dungSi.SetRelateDemagemRect(m_dungSi.NpcInfo.X, m_dungSi.NpcInfo.Y, m_dungSi.NpcInfo.Width, m_dungSi.NpcInfo.Height);
			m_dungSi.FallFrom(m_dungSi.X, m_dungSi.Y, "", 0, 0, 1000, null);
			m_dungSiCreateCount = 1;
		}

		public void CreateOutGame()
		{
			m_bossBorn.Die(0);
			base.Game.RemoveLiving(m_bossBorn.Id);
		}

		public override void OnNewTurnStarted()
		{
			base.OnNewTurnStarted();
			if (m_dungSi != null && !m_dungSi.IsLiving)
			{
				if (m_dungSiCreateCount <= 0)
				{
					CreateBoss();
				}
				else
				{
					m_dungSiCreateCount--;
				}
			}
			if (CheckNPCLived() <= 0)
			{
				CreateNPC();
			}
		}

		private void CreateNPC()
		{
			int disX = 1200;
			for (int i = 0; i < m_npcCreateCount; i++)
			{
				m_npcList.Add(base.Game.CreateNpc(m_chiensiID, disX, 700, 1, 1));
				disX += 30;
			}
		}

		private int CheckNPCLived()
		{
			int cnt = 0;
			foreach (SimpleNpc item in m_npcList)
			{
				cnt += item.IsLiving ? 1 : 0;
			}
			return cnt;
		}

		public override void OnBeginNewTurn()
		{
			base.OnBeginNewTurn();
		}

		public override bool CanGameOver()
		{
			base.CanGameOver();
			//if (m_dauThan != null && m_dauThan.Blood >= m_dauThan.MaxBlood)
			if (Game.TotalKillCount > 0 && m_dauThan.Blood >= m_dauThan.MaxBlood)
			{
				m_bossRecoveredCount++;
				return true;
			}
			if (m_dauThan != null && !m_dauThan.IsLiving)
			{
				return true;
			}
			return false;
		}

		public override int UpdateUIData()
		{
			base.UpdateUIData();
			return m_bossRecoveredCount;
		}

		public override void OnGameOver()
		{
			base.OnGameOver();
			//if (m_dauThan.Blood >= m_dauThan.MaxBlood)
			if (m_dauThan.Blood >= m_dauThan.MaxBlood)
			{
				base.Game.IsWin = true;
			}
			if (!m_dauThan.IsLiving)
			{
				base.Game.IsWin = false;
			}
		}
	}
}
