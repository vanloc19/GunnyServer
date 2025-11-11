using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;
using System.Collections.Generic;

namespace GameServerScript.AI.Messions
{
    public class DCNM2401 : AMissionControl
    {
        private List<SimpleNpc> someNpc = new List<SimpleNpc>();

        private int dieRedCount;

        private int[] npcIDs = { 2401, 2402 };

        private int[] birthX = { 52, 115, 183, 253, 320, 1206, 1275, 1342, 1410, 1475 };

        public override int CalculateScoreGrade(int score)
        {
            base.CalculateScoreGrade(score);
            if (score > 1870)
            {
                return 3;
            }
            if (score > 1825)
            {
                return 2;
            }
            if (score > 1780)
            {
                return 1;
            }
            return 0;
        }

        public override void OnPrepareNewSession()
        {
            base.OnPrepareNewSession();
            int[] resources = { npcIDs[0], npcIDs[1] };
            int[] gameOverResources = { npcIDs[1], npcIDs[0], npcIDs[0], npcIDs[0] };
            base.Game.LoadResources(resources);
            base.Game.LoadNpcGameOverResources(gameOverResources);
            base.Game.SetMap(1120);
        }

        public override void OnStartGame()
        {
            base.OnStartGame();
            int index = base.Game.Random.Next(0, npcIDs.Length);
            someNpc.Add(base.Game.CreateNpc(npcIDs[index], 52, 206, 1, 1));
            index = base.Game.Random.Next(0, npcIDs.Length);
            someNpc.Add(base.Game.CreateNpc(npcIDs[index], 100, 207, 1, 1));
            index = base.Game.Random.Next(0, npcIDs.Length);
            someNpc.Add(base.Game.CreateNpc(npcIDs[index], 155, 208, 1, 1));
            index = base.Game.Random.Next(0, npcIDs.Length);
            someNpc.Add(base.Game.CreateNpc(npcIDs[index], 210, 207, 1, 1));
            index = base.Game.Random.Next(0, npcIDs.Length);
            someNpc.Add(base.Game.CreateNpc(npcIDs[index], 253, 207, 1, 1));
            index = base.Game.Random.Next(0, npcIDs.Length);
            someNpc.Add(base.Game.CreateNpc(npcIDs[index], 1275, 208, -1, 1));
            index = base.Game.Random.Next(0, npcIDs.Length);
            someNpc.Add(base.Game.CreateNpc(npcIDs[index], 1325, 206, -1, 1));
            index = base.Game.Random.Next(0, npcIDs.Length);
            someNpc.Add(base.Game.CreateNpc(npcIDs[index], 1360, 208, -1, 1));
            index = base.Game.Random.Next(0, npcIDs.Length);
            someNpc.Add(base.Game.CreateNpc(npcIDs[index], 1410, 206, -1, 1));
            index = base.Game.Random.Next(0, npcIDs.Length);
            someNpc.Add(base.Game.CreateNpc(npcIDs[index], 1475, 208, -1, 1));
        }

        public override void OnNewTurnStarted()
        {
            base.OnNewTurnStarted();
            if (base.Game.GetLivedLivings().Count == 0)
            {
                base.Game.PveGameDelay = 0;
            }
            if (base.Game.TurnIndex <= 1 || base.Game.CurrentPlayer.Delay <= base.Game.PveGameDelay || base.Game.GetLivedLivings().Count >= 10)
            {
                return;
            }
            for (int i = 0; i < 10 - base.Game.GetLivedLivings().Count; i++)
            {
                if (someNpc.Count == base.Game.MissionInfo.TotalCount)
                {
                    break;
                }
                int index = base.Game.Random.Next(0, birthX.Length);
                int NpcX = birthX[index];
                index = base.Game.Random.Next(0, npcIDs.Length);
                if (index == 1 && GetNpcCountByID(npcIDs[1]) < 10)
                {
                    if (NpcX > 700)
                    {
                        someNpc.Add(base.Game.CreateNpc(npcIDs[1], NpcX, 506, -1, 1));
                    }
                    else
                    {
                        someNpc.Add(base.Game.CreateNpc(npcIDs[1], NpcX, 506, 1, 1));
                    }
                }
                else if (NpcX > 700)
                {
                    someNpc.Add(base.Game.CreateNpc(npcIDs[1], NpcX, 506, -1, 1));
                }
                else
                {
                    someNpc.Add(base.Game.CreateNpc(npcIDs[1], NpcX, 506, 1, 1));
                }
            }
        }

        public override void OnBeginNewTurn()
        {
            base.OnBeginNewTurn();
        }

        public override bool CanGameOver()
        {
            bool result = true;
            base.CanGameOver();
            dieRedCount = 0;
            foreach (SimpleNpc item in someNpc)
            {
                if (item.IsLiving)
                {
                    result = false;
                }
                else
                {
                    dieRedCount++;
                }
            }
            if (result && dieRedCount == base.Game.MissionInfo.TotalCount)
            {
                base.Game.IsWin = true;
                return true;
            }
            return false;
        }

        public override int UpdateUIData()
        {
            return base.Game.TotalKillCount;
        }

        public override void OnGameOver()
        {
            base.OnGameOver();
            if (base.Game.GetLivedLivings().Count == 0)
            {
                base.Game.IsWin = true;
                List<LoadingFileInfo> loadingFileInfos = new List<LoadingFileInfo>();
                loadingFileInfos.Add(new LoadingFileInfo(2, "image/map/2/show2", ""));
                base.Game.SendLoadResource(loadingFileInfos);
            }
            else
            {
                base.Game.IsWin = false;
            }
        }

        protected int GetNpcCountByID(int Id)
        {
            int Count = 0;
            foreach (SimpleNpc item in someNpc)
            {
                if (item.NpcInfo.ID == Id)
                {
                    Count++;
                }
            }
            return Count;
        }
    }
}
