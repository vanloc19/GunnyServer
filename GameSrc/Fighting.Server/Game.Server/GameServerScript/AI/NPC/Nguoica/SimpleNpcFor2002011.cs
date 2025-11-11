using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;
using System;
using System.Collections.Generic;

namespace GameServerScript.AI.NPC
{
    public class SimpleNpcFor2002011 : ABrain
    {
        protected Player m_targer;

        private static Random random = new Random();
        //http://gunny.vcdn.vn/image/game/bonesliving/asset_game_fifteen_472a.png?lv=837

        private bool ChayLocXoay = false;
        private PhysicalObj LocXoayPhy = null;
        private int redNpcID = 2002001;
        private int redNpcID2 = 2002003;
        public override void OnBeginSelfTurn()
        {
            base.OnBeginSelfTurn();
        }

        public override void OnBeginNewTurn()
        {
            base.OnBeginNewTurn();

        }

        public override void OnCreated()
        {
            base.OnCreated();
        }

        public override void OnStartAttacking()
        {
            base.OnStartAttacking();
            if (!((PVEGame)Game).MauDuoiMucChoPhep)
            {
                BeatA();
            }
            else
            {
                if(!ChayLocXoay)
                {
                    ChayLocXoay = true;
                    Quay();
                }
                else
                {
                    ChayLocXoay = false;
                    BeatA();
                }
            }
        }

        public override void OnStopAttacking()
        {
            base.OnStopAttacking();
        }

        public void BeatA()
        {
            Body.PlayMovie("beatA", 1000, 2000);
            Body.RangeAttacking(Body.X - 2000, Body.X + 2000, "cry", 4000, null);
        }

        public void Quay()
        {
            Body.PlayMovie("beatA", 0, 2000);
            Body.CallFuction(new LivingCallBack(LocXoay), 3000);
        }
        public void LocXoay()
        {
            ((PVEGame)base.Game).CreateNpc(redNpcID2, 1022, 794, -1, 1);
            ((PVEGame)base.Game).CreateNpc(redNpcID, 1022, 794, -1, 1);
            ((PVEGame)base.Game).CreateNpc(redNpcID2, 1022, 794, -1, 1);
            var ps = ((PVEGame)base.Game).FindRandomPlayerInPoint(195, 488);
            LocXoayPhy = ((PVEGame)base.Game).Createlayer(373, 793, "moive", "asset_game_fifteen_472b", "out", 1, 1);
            foreach(var p in ps)
            {
                p.MoveToSpeed(p.X - 150, p.Y,"walk",1000,20,null);
            }
            Body.RangeAttacking(Body.X - 2000, Body.X + 2000, "cry", 2000, null);
            Body.CallFuction(new LivingCallBack(XoaLoc), 2000);
        }
        public void XoaLoc()
        {
            ((PVEGame)Game).RemovePhysicalObj(LocXoayPhy, true);
        }
    }
}
