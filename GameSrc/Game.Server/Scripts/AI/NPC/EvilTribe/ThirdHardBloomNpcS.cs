using System;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.NPC
{
    public class ThirdHardBloomNpcS : ABrain
    {
        private int addBloom = 1200;

        public override void OnBeginSelfTurn()
        {
            base.OnBeginSelfTurn();
            Body.PlayMovie("", 0, 1000);
        }

        public override void OnBeginNewTurn()
        {
            base.OnBeginNewTurn();
            int rand = Game.Random.Next(0, 5);
            if (rand == 0)
            {
                string msg;
                int index = Game.Random.Next(0, AntChat.Length);
                msg = AntChat[index];
                m_body.Say(msg, 0, 1000);
            }
        }

        public override void OnCreated()
        {
            base.OnCreated();
        }

        public override void OnStartAttacking()
        {
            base.OnStartAttacking();
            int count = 0;
            foreach (Player player in Game.GetAllFightPlayers())
            {
                if (player.IsLiving && player.X > Body.X - 100 && player.X < Body.X + 100)
                {
                    count++;
                }
            }
            Body.PlayMovie("renew", 0, 0);
            foreach (Player player in Game.GetAllFightPlayers())
            {
                if (player.IsLiving && player.X > Body.X - 100 && player.X < Body.X + 100)
                {
                    player.AddBlood(addBloom / count);
                }
            }
        }

        public override void OnStopAttacking()
        {
            base.OnStopAttacking();
        }

        public override void OnKillPlayerSay()
        {
            base.OnKillPlayerSay();
        }

        public override void OnDiedSay()
        {
        }

        public override void OnShootedSay(int delay)
        {
        }

        #region NPC 小怪说话

        private static Random random = new Random();
        private static string[] AntChat = new string[] {
            "Cẩn thận... chúng ta phải tiêu diệt tà thần",

             "Không được đầu hàng !",

             "Tôi sẽ tiếp cho các bạn sức mạnh！",

             "Chúng ta sẽ đẩy lùi bộ tộc tà thần！",
        };
        #endregion
    }
}
