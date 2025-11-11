using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;
using System;

namespace GameServerScript.AI.NPC
{
    public class TwelveHardGunBoss : ABrain
    {
        private int BossID = 12208;
        #region NPC Chat

        private static Random random = new Random();
        private static string[] listChat = new string[] {
            "Hãy đánh bại Cá Sấu!!",
            "Tiêu diệt Nhóc lựu đạn để tăng hỏa lực",
        };

        public static string GetOneChat()
        {
            int index = random.Next(0, listChat.Length);
            return listChat[index];
        }



        #endregion

        public override void OnBeginNewTurn()
        {
            m_body.CurrentDamagePlus = 1;
            m_body.CurrentShootMinus = 1;
            if (m_body.IsSay)
            {
                string msg = GetOneChat();
                int delay = Game.Random.Next(0, 5000);
                m_body.Say(msg, 0, delay);
            }
        }

        public override void OnBeginSelfTurn()
        {
        }

        public override void OnCreated()
        {
        }

        public override void OnStartAttacking()
        {
            MoveBeat();
        }

        public void MoveBeat()
        {
            ((PVEGame)Game).SendGameFocus(0, 800, 1, 0, 2000);
            Body.Say("Thử cái này xem", 0, 0);
            Body.PlayMovie("beatA", 0, 2900);
            Body.CallFuction(new LivingCallBack(dame), 2900);
        }
        private void dame()
        {
            SimpleBoss simpleBoss = ((PVEGame)base.Game).FindBossWithID(BossID);
            simpleBoss.Properties3 = int.Parse(simpleBoss.Properties3.ToString()) + base.Body.ShootCount;
            for (int i = 0; i < Body.ShootCount; i++)
            {
                if (Body.ShootPoint(simpleBoss.X + 120, simpleBoss.Y - 125, 88, 1000, 10000, 1, 1, 500 + (i * 1000)))
                {
                    Body.PlayMovie("beatB", 0 + 500 + (1000 * i), 0);
                }
            }
            //base.Game.ClearAllChild();
            //Body.PlayMovie("beatC", Body.countBoom * 1000, 1800);
            Body.ShootCount = 0;
            Body.Config.IsTurn = false;
        }

        public override void OnStopAttacking()
        {
        }
    }
}
