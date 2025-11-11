using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Effects;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.NPC
{
    public class TwelveNormalFlyCaptainBoss : ABrain
    {
        private int turnIndex = 0;

        private int count = 0;

        private int beatCount = 0;

		private int beginCount = 1;

		private int endCount = 3;

        private SimpleNpc chicken = null;

        private int RemovedBlood = 200;

        private void Beat()
        {
			chicken = ((PVEGame)base.Game).FindHealthyHelper();
			if (chicken != null)
			{
				chicken.Config.CanHeal = true;
				base.Body.PlayMovie("beat", 1000, 3000);
				base.Body.CallFuction(CreateEffectDamage, 5000);
			}
        }

        private void SuperBeat()
        {
			base.Body.PlayMovie("beat", 1000, 3000);
			base.Body.CallFuction(NextChicken, 4300);
			beatCount = base.Game.Random.Next(beginCount, endCount);
        }

        private void NextChicken()
        {
			count++;
			if (count <= beatCount)
			{
				chicken = ((PVEGame)base.Game).FindHealthyHelper();
				if (chicken != null)
				{
					chicken.Config.CanHeal = true;
					base.Body.CallFuction(CreateEffectDamage, 700);
				}
			}
			else
			{
				count = 0;
			}
        }

        private void CreateEffectDamage()
        {
			((PVEGame)base.Game).SendFreeFocus(chicken.X, chicken.Y, 1, 1, 1);
			((PVEGame)base.Game).Createlayer(chicken.X, chicken.Y, "", "asset.game.nine.duqidd", "", 1, 1);
			base.Body.CallFuction(CreateGreenReduce, 1300);
        }

        private void CreateGreenReduce()
        {
			chicken.AddEffect(new ContinueReduceGreenBloodEffect(10, RemovedBlood, chicken), 0);
			chicken.AddBlood(-RemovedBlood, 1);
			chicken.Config.CanHeal = true;
			if (count > 0)
			{
				base.Body.CallFuction(NextChicken, 1);
			}
        }

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

        public override void OnStartAttacking()
        {
			base.OnStartAttacking();
			((PVEGame)base.Game).SendObjectFocus(base.Body, 0, 1, 1);
			if (turnIndex != 1)
			{
				Beat();
				turnIndex++;
			}
			else
			{
				SuperBeat();
				turnIndex = 0;
			}
        }

        public override void OnStopAttacking()
        {
			base.OnStopAttacking();
        }
    }
}
