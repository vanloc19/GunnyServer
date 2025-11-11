using Game.Logic.AI;
using Game.Logic.Phy.Object;
using Bussiness;

namespace GameServerScript.AI.NPC
{
    public class SimpleBomblingNpc : ABrain
    {
        private Player _mTarget;

        private int _mTargetDis;

        public override void OnBeginNewTurn()
        {
            base.OnBeginNewTurn();
            Body.CurrentDamagePlus = 1;
            Body.CurrentShootMinus = 1;
            if (!Body.IsSay) return;
            string msg = GetOneChat();
            int delay = Game.Random.Next(0, 5000);
            Body.Say(msg, 0, delay);
        }

        public override void OnStartAttacking()
        {
            base.OnStartAttacking();
            _mTarget = Game.FindNearestPlayer(Body.X, Body.Y);
            _mTargetDis = (int)_mTarget.Distance(Body.X, Body.Y);
            if (_mTargetDis <= 50)
            {
                Body.PlayMovie("beatA", 100, 0);
                Body.RangeAttacking(Body.X - 50, Body.X + 50, "cry", 1500, null);
                Body.Die(1000);
            }
            else
            {
                MoveToPlayer(_mTarget);
            }
        }

        //public void MoveToPlayer(Player player)
        //{
        //    int dis = Game.Random.Next(((SimpleNpc)Body).NpcInfo.MoveMin, ((SimpleNpc)Body).NpcInfo.MoveMax);
        //    if (player.X > Body.X)
        //    {
        //        if (Body.X + dis >= player.X)
        //        {
        //            Body.MoveTo(player.X - 10, Body.Y, "walk", 2000, ((SimpleNpc)Body).NpcInfo.speed, Beat);
        //        }
        //        else
        //        {
        //            Body.MoveTo(Body.X + dis, Body.Y, "walk", 2000, ((SimpleNpc)Body).NpcInfo.speed, Beat);
        //        }
        //    }
        //    else
        //    {
        //        if (Body.X - dis <= player.X)
        //        {
        //            Body.MoveTo(player.X + 10, Body.Y, "walk", 2000, ((SimpleNpc)Body).NpcInfo.speed, Beat);
        //        }
        //        else
        //        {
        //            Body.MoveTo(Body.X - dis, Body.Y, "walk", 2000, ((SimpleNpc)Body).NpcInfo.speed, Beat);
        //        }
        //    }
        //}

        public void MoveToPlayer(Player player)
        {
            int num = base.Game.Random.Next(((SimpleNpc)base.Body).NpcInfo.MoveMin, ((SimpleNpc)base.Body).NpcInfo.MoveMax);
            if (!((player.X <= base.Body.X) ? base.Body.MoveTo((base.Body.X - num > player.X) ? (base.Body.X - num) : (player.X + 50), base.Body.Y, "walk", 2000, "", 4, Beat) : base.Body.MoveTo((base.Body.X + num < player.X) ? (base.Body.X + num) : (player.X - 50), base.Body.Y, "walk", 2000, "", 4, Beat)))
            {
                base.Body.CallFuction(Beat, 1000);
            }
        }

        public void Beat()
        {
            _mTargetDis = (int)_mTarget.Distance(Body.X, Body.Y);
            if (_mTargetDis <= 50)
            {
                Body.PlayMovie("beatA", 100, 0);
                Body.RangeAttacking(Body.X - 100, Body.X + 100, "cry", 1500, null);
                Body.Die(1000);
            }
        }

        #region NPC 小炸弹人说话

        private static readonly string[] BombNpcChat = {
            LanguageMgr.GetTranslation("Game.Server.GameServerScript.AI.NPC.SimpleBomblingNpc.msg1"),
            LanguageMgr.GetTranslation("Game.Server.GameServerScript.AI.NPC.SimpleBomblingNpc.msg2"),
            LanguageMgr.GetTranslation("Game.Server.GameServerScript.AI.NPC.SimpleBomblingNpc.msg3"),
            LanguageMgr.GetTranslation("Game.Server.GameServerScript.AI.NPC.SimpleBomblingNpc.msg4")
        };
        private string GetOneChat()
        {
            int index = Game.Random.Next(0, BombNpcChat.Length);
            return BombNpcChat[index];
        }
        #endregion
    }
}