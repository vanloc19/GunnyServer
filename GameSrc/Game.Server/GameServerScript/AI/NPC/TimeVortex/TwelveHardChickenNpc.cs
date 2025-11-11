using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.NPC
{
    public class TwelveHardChickenNpc : ABrain
    {
        private int BossID = 12010;

        public override void OnBeginNewTurn()
        {
			base.OnBeginNewTurn();
        }

        public override void OnBeginSelfTurn()
        {
			base.OnBeginSelfTurn();
        }

        public override void OnCreated()
        {
			base.OnCreated();
        }

        private void RandMove()
        {
			int num = base.Game.Random.Next(base.Body.X - base.Game.Random.Next(200, 400), base.Body.X + base.Game.Random.Next(200, 400));
			SimpleBoss simpleBoss = base.Game.FindBossWithID(BossID);
			int x = (base.Body.X <= 600) ? (base.Body.X + base.Game.Random.Next(200, 400)) : ((base.Body.X >= 1300) ? (base.Body.X - base.Game.Random.Next(200, 400)) : num);
			int delay = 0;
			if (base.Body.Config.CanHeal)
			{
				delay = Game.GetDelayDistance(Body.X, x, 7) + 1000;
				base.Body.MoveTo(x, base.Body.Y, "walkA", 500, 7);
				base.Body.PlayMovie("standC", delay, 1000);
			}
			else
			{
				delay = Game.GetDelayDistance(Body.X, x, 5) + 1000;
				base.Body.MoveTo(x, base.Body.Y, "walk", 500, 5);
				base.Body.Direction = ((simpleBoss.X >= base.Body.X) ? 1 : (-1));
				base.Body.PlayMovie((num > 600 && num < 1300) ? "standB" : "standA", delay, 1000);
			}
        }

        public override void OnStartAttacking()
        {
			base.OnStartAttacking();
			RandMove();
        }

        public override void OnStopAttacking()
        {
			base.OnStopAttacking();
        }

        public override void OnDie()
        {
			base.OnDie();
			if (base.Body.Properties1 == 1)
			{
				//((PVEGame)base.Game).TotalKillCount--;
				((PVEGame)base.Game).Param1++;
			}
			else
			{
				((PVEGame)base.Game).TotalKillCount++;
			}
        }

        public override void OnHeal(int blood)
        {
			base.OnHeal(blood);
			if (base.Body.Config.CanHeal && base.Body.Blood >= base.Body.MaxBlood)
			{
				base.Body.Properties1 = 1;
				base.Body.CallFuction(RemoveChicken, 1500);
			}
        }

        private void RemoveChicken()
        {
			base.Body.PlayMovie("standD", 1000, 1000);
			base.Body.Die(3000);
			base.Body.Dispose();
        }
    }
}
