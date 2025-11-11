using Bussiness;
using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;
using System;
using System.Drawing;

namespace GameServerScript.AI.NPC
{
    public class TwelveSimpleCrocodileBoss : ABrain
    {

        private SimpleNpc[] bombs = null;

        private int turnIndex;

        private int BombID = 12007;

        private Player randomPlayer;

        private Layer targetCannon;

        private Layer Cannon;

        private void method_0(int int_1, int int_2)
        {
            base.Body.CurrentDamagePlus = 1000f;
            base.Body.PlayMovie("beatA", 1000, 0);
            base.Body.RangeAttacking(int_1, int_2, "cry", 4000, null);
        }

        private void CallBomb()
        {
            base.Body.MoveTo(1450, 990, "walk", 500, 12);
            Body.Say("—юда, верные слуги! ”ничтожьте их!", 1, 1500);
            Body.CallFuction(CreateBomb, 2500);
        }

        private void CreateBomb()
        {
            bombs = new SimpleNpc[5];
            Point[] arrPoint = {
                new Point(711, 1002),
                new Point(884, 1002),
                new Point(977, 1013),
                new Point(1210, 1008),
                new Point(Body.X, Body.Y)
            };

            ((PVEGame)Game).SendFreeFocus(Body.X, Body.Y, 1, 1, 1);
            for (int i = 0; i < 5; i++)
            {
                bombs[i] = (Game as PVEGame).CreateNpc(BombID, arrPoint[i].X, arrPoint[i].Y, 1, -1);
                bombs[i].Properties1 = 1;
                if (Body.Properties1 == 1)
                    bombs[i].Properties2 = Body.Properties3;
                else
                    bombs[i].Properties2 = 1;
            }
            if (Body.Properties1 < 1)
                Body.Properties1 = 1;
        }

        public override void OnBeginNewTurn()
        {
            base.OnBeginNewTurn();
            base.Body.CurrentDamagePlus = 1f;
            base.Body.CurrentShootMinus = 1f;
        }

        public override void OnBeginSelfTurn()
        {
            base.OnBeginSelfTurn();
        }

        public override void OnCreated()
        {
            base.OnCreated();
        }
        public override void OnDie()
        {
            base.OnDie();
            Body.Say("ћое сокровище...", 1, 100);
            Body.PlayMovie("dieA", 100, 6000);
        }
        private void RandMove()
        {
            int x = ThreadSafeRandom.NextStatic(1020, 1608);
            switch (turnIndex)
            {
                case 1:
                    Body.NewMoveTo(x, Body.Y, "walk", 1000, "", 12, BeatA, 100);
                    break;
                case 2:
                    Body.NewMoveTo(x, Body.Y, "walk", 1000, "", 12, BeatC, 100);
                    break;
                case 3:
                    Body.NewMoveTo(x, Body.Y, "walk", 1000, "", 12, BeatD, 100);
                    break;
                case 4:
                    Body.NewMoveTo(x, Body.Y, "walk", 1000, "", 12, CannonBeat, 100);
                    break;
            }

        }
        private void BeatA()
        {
            Console.WriteLine("BeatA");
            randomPlayer = Game.FindRandomPlayer();
            Body.Direction = randomPlayer.X > Body.X ? 1 : -1;
            Body.Say("я теб¤ с землей сровн¤ю!", 1, 1000);
            Body.PlayMovie("beatA", 2000, 1000);
            base.Body.BeatDirect(randomPlayer, "", 2500, 1, 1);
            Body.CallFuction(EffectBeatRandomPlayer, 2500);

        }
        private void BeatC()
        {
            Console.WriteLine("BeatC");
            randomPlayer = Game.FindRandomPlayer();
            Body.Direction = randomPlayer.X > Body.X ? 1 : -1;
            Body.Say("—коро ты превратишь¤ в решето.", 1, 1000);
            Body.PlayMovie("beatC", 2000, 1000);
            Body.CallFuction(EffectBeatAllPlayer, 2800);


        }
        private void CannonBeat()
        {
            Console.WriteLine("CanonBeat");
            Body.Direction = randomPlayer.X > Body.X ? 1 : -1;
            Body.Say("Ќу-ну...давайте, покажите мне, на что вы способны!", 1, 100);
            Body.PlayMovie("beatD", 500, 2000);
            Body.CallFuction(CreateCannonBoom, 2000);
            ((PVEGame)base.Game).SendFreeFocus(targetCannon.X, targetCannon.Y, 1, 1700, 100);
        }
        private void BeatD()
        {
            Console.WriteLine("BeatD");
            randomPlayer = Game.FindRandomPlayer();
            Body.Direction = randomPlayer.X > Body.X ? 1 : -1;
            Body.Say("ƒумаешь мне нечем ответить?", 1, 100);
            Body.PlayMovie("beatD", 500, 2000);
            Body.CallFuction(CreateTargetCannon, 2000);
        }
        private void CreateCannonBoom()
        {
            int rand = ThreadSafeRandom.NextStatic(50);
            Cannon = ((PVEGame)base.Game).Createlayer(targetCannon.X, targetCannon.Y, "", "asset.game.nine.dapao", rand < 25 ? "beatA" : "beatB", 1, 1);
            Body.CallFuction(CannonDamage, 1500);
            Body.CallFuction(RemoveCannonObjects, 3500);

        }
        private void CannonDamage()
        {
            base.Body.CurrentDamagePlus = 1500f;
            Body.RangeAttacking(targetCannon.X - 125, targetCannon.X + 125, "", 100, null);
        }
        private void RemoveCannonObjects()
        {
            ((PVEGame)Game).RemovePhysicalObj(targetCannon, true);
            ((PVEGame)Game).RemovePhysicalObj(Cannon, true);
            if (targetCannon != null)
                targetCannon = null;
            if (Cannon != null)
                Cannon = null;
        }
        private void CreateTargetCannon()
        {
            targetCannon = ((PVEGame)base.Game).Createlayer(randomPlayer.X, randomPlayer.Y, "", "asset.game.nine.biaojiA", "", 1, 1);
        }
        private void EffectBeatRandomPlayer()
        {
            ((PVEGame)base.Game).Createlayer(randomPlayer.X, randomPlayer.Y - 10, "", "asset.game.4.xiaopao", "", 1, 1);
        }
        private void EffectBeatAllPlayer()
        {
            foreach (Player allLivingPlayer in base.Game.GetAllLivingPlayers())
            {
                if (!allLivingPlayer.IsLiving)
                {
                    continue;
                }
                ((PVEGame)base.Game).Createlayer(allLivingPlayer.X, allLivingPlayer.Y - 10, "", "asset.game.4.xiaopao", "", 1, 1);
                base.Body.BeatDirect(allLivingPlayer, "", 500, 4, 1);
            }
        }
        public override void OnStartAttacking()
        {
            base.Body.CurrentDamagePlus = 1f;
            bool flag = false;
            int num = 0;
            foreach (Player allFightPlayer in base.Game.GetAllFightPlayers())
            {
                if (!allFightPlayer.IsLiving || allFightPlayer.X <= 1280)
                {
                    continue;
                }
                int num1 = (int)base.Body.Distance(allFightPlayer.X, allFightPlayer.Y);
                if (num1 > num)
                {
                    num = num1;
                }
                flag = true;
            }
            if (flag)
            {
                method_0(0, base.Game.Map.Info.ForegroundWidth + 1);
                return;
            }
            switch (turnIndex)
            {
                case 0:
                    CallBomb();
                    turnIndex++;
                    break;
                case 1:
                    RandMove();
                    turnIndex++;
                    break;
                case 2:
                    RandMove();
                    turnIndex++;
                    break;
                case 3:
                    RandMove();
                    turnIndex++;
                    break;
                case 4:
                    RandMove();
                    turnIndex = 0;
                    break;

            }
        }

        public override void OnStopAttacking()
        {
            base.OnStopAttacking();
        }
    }
}