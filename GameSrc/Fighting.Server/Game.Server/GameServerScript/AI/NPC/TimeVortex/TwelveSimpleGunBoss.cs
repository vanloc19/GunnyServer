using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.NPC
{
    public class TwelveSimpleGunBoss : ABrain
    {
        int cannonball = 12007;

        private int BossID = 12008;

        public override void OnBeginNewTurn()
        {
        }

        public override void OnBeginSelfTurn()
        {
        }

        public override void OnCreated()
        {
        }

        public override void OnStartAttacking()
        {
            if (((PVEGame)Game).GetNPCLivingWithID(cannonball).Length == 0 && Body.ShootCount > 0)
            {
                Body.CurrentDamagePlus = 2f;
                ((PVEGame)Game).SendFreeFocus(Body.X, Body.Y, 1, 1, 1);
                SimpleBoss boss = ((PVEGame)Game).FindBossWithID(BossID);
                boss.Properties3 = int.Parse(boss.Properties3.ToString()) + Body.ShootCount;
                for (int i = 0; i < Body.ShootCount; i++)
                {
                    if (base.Body.ShootPoint(boss.X, boss.Y - 125, 88, 1000, 10000, 1, 2f, 4000))
                    {
                        Body.Say("ќгонь!", 0, 3000);
                        base.Body.PlayMovie("beatA", 1000, 6000);
                    }
                }
                Game.ClearAllChild();
                Body.ShootCount = 0;
            }
        }

        public override void OnStopAttacking()
        {
        }
    }
}