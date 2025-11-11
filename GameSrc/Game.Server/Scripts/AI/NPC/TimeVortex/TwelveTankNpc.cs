using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.NPC
{
    public class TwelveTankNpc : ABrain
    {
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

        }
        public override void OnDie()
        {
            base.OnDie();
            Body.PlayMovie("die", 100, 1000);
        }
        public override void OnBeforeTakedDamage(Living source, ref int damage, ref int crit)
        {
            if (Body.Blood >= (Body.MaxBlood * 0.70))
            {
                Body.PlayMovie("cryA", 0, 1000);
                if (Body.ActionMovie != "cryA")
                    Body.ActionMovie = "cryA";
            }
            else if (Body.Blood < (Body.MaxBlood * 0.70) && Body.Blood > (Body.MaxBlood * 0.30))
            {
                if (Body.ActionMovie == "cryA")
                {
                    Body.PlayMovie("cryAtoB", 0, 1000);
                    if (Body.ActionMovie != "cryB")
                        Body.ActionMovie = "cryB";
                }
                else
                {
                    Body.PlayMovie("cryB", 0, 1000);
                }
            }
            else if (Body.Blood < (Body.MaxBlood * 0.30) && Body.Blood > 0)
            {
                if (Body.ActionMovie == "cryB")
                {
                    Body.PlayMovie("cryBtoC", 0, 1000);
                    if (Body.ActionMovie != "cryC")
                        Body.ActionMovie = "cryC";
                }
                else
                {
                    Body.PlayMovie("cryC", 0, 1000);
                }
            }
            base.OnBeforeTakedDamage(source, ref damage, ref crit);
        }

        public override void OnStopAttacking()
        {
            base.OnStopAttacking();
        }
    }
}