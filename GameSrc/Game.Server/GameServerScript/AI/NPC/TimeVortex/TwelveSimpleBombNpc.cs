using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.NPC
{
    public class TwelveSimpleBombNpc : ABrain
    {

        private int int_1 = 0;

        protected Player m_player;

        private int HelperNPCID = 12006;

        private void MoveBeat()
        {
            this.m_player = base.Game.FindNearestPlayer(base.Body.X, base.Body.Y);
            base.Body.ChangeDirection(this.m_player, 500);
            if (this.m_player != null && this.m_player.IsLiving)
            {
                this.int_1 = (int)this.m_player.Distance(base.Body.X, base.Body.Y);
                base.Body.CallFuction(new LivingCallBack(this.MoveToPlayer), 1000);
            }
        }
        public override void OnDie()
        {
            base.OnDie();
            SimpleBoss chicken = ((PVEGame)Game).FindBossWithID(HelperNPCID);
            if (Body.Properties1 == 1)
                chicken.ShootCount += 1;
        }
        public void SetState()
        {
            Body.Properties1 = 0;
            Body.Die();
        }
        public void Beat()
        {
            if (Body.Beat(this.m_player, "beatA", 100, 0, 0, 1, 1))
            {
                Body.CallFuction(SetState, 2000);
            }
        }
        public void MoveToPlayer()
        {
            int int1 = ((SimpleNpc)base.Body).NpcInfo.MoveMax;
            if (this.int_1 < int1)
            {
                int1 = this.int_1;
            }
            int num = (base.Body.Direction == -1 ? base.Body.X - int1 : base.Body.X + int1);
            base.Body.MoveTo(num, base.Body.Y, "walk", 1200, "", ((SimpleNpc)base.Body).NpcInfo.speed, new LivingCallBack(this.Beat));
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
        }

        public override void OnCreated()
        {
            base.OnCreated();
        }

        public override void OnStartAttacking()
        {
            base.OnStartAttacking();
            if ((int)Body.Properties2 > 0)
            {
                Body.Properties2 = int.Parse(Body.Properties2.ToString()) - 1;
                return;
            }
            this.m_player = base.Game.FindNearestPlayer(base.Body.X, base.Body.Y);
            this.MoveBeat();
        }

        public override void OnStopAttacking()
        {
            base.OnStopAttacking();
        }
    }
}