using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Effects;
using Game.Logic.Phy.Maps;
using Game.Logic.Phy.Object;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace GameServerScript.AI.NPC
{
    public class TwelveSimpleFlyThirdBoss : ABrain
    {
        private int turn = 1;

        private int reduceBlood = 1000;

        int[] boltMoveXs = new int[4] { 585, 730, 1300, 1445 };

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
        private void MoveBeatD()
        {
            Body.NewMoveTo(Game.Random.Next(675, 1415), Game.Random.Next(415, 450), "fly", 100, "", 6, BeatD, 100);
        }

        private void BeatD()
        {
            Body.Say("Мн?ещ?не попадалось противника сильне?", 0, 100, 2000);
            Body.PlayMovie("beatD", 100, 6000);
            Body.CallFuction(RotatePlayers, 3000);
        }

        private void MoveBeatB()
        {
            Body.NewMoveTo(Game.Random.Next(675, 1415), Game.Random.Next(415, 450), "fly", 100, "", 6, PreBeatB, 100);
        }

        private void PreBeatB()
        {
            Player randomPlayer = Game.FindRandomPlayer();
            Body.Say("Знаешь чт?тако?сила тока? Сейчас узнаеш?", 0, 100, 2000);

            Body.NewMoveTo(randomPlayer.X, 570, "fly", 100, "", 6, new LivingCallBack(() => BeatB(randomPlayer)), 100);
        }

        private void MoveBeatC()
        {
            Body.NewMoveTo(Game.Random.Next(675, 1415), Game.Random.Next(415, 450), "fly", 100, "", 6, BeatC, 100);
        }

        private void BeatC()
        {
            Body.Say("Скор?я буду правит?миро?", 0, 100, 2000);
            Body.PlayMovie("beatC", 100, 6000);
            ((PVEGame)Game).SendObjectFocus(Game.FindRandomPlayer(), 1, 1500, 100);
            Body.CallFuction(CreateFengyinffect, 3000);
        }

        private void MoveBeatA()
        {
            Body.NewMoveTo(Game.Random.Next(675, 1415), Game.Random.Next(415, 450), "fly", 100, "", 6, BeatA, 100);
        }

        private void BeatA()
        {
            Body.Say("Не злит?ме?!!!", 0, 100, 2000);
            Body.PlayMovie("beatA", 100, 6000);
            ((PVEGame)Game).SendObjectFocus(Game.FindRandomPlayer(), 1, 3000, 100);
            Body.CallFuction(BeatPlayers, 3000);
        }

        private void BeatPlayers()
        {
            Body.CurrentDamagePlus = 25f;
            foreach (Player player in Game.GetAllLivingPlayersByProperties(2))
            {
                Body.BeatDirect(player, "", 1, 1, 1);
                player.Properties1 = 0;
                ((PVEGame)Game).SendPlayersPicture(player, (int)eLivingPictureType.Targeting, false);
                player.SetSeal(false);
            }
        }

        private void CreateFengyinffect()
        {
            List<Player> players = Game.GetAllLivingPlayers();
            if (players.Count == 1)
            {
                ((PVEGame)Game).Createlayer(players[0].X, players[0].Y, "", "asset.game.nine.fengyin", "", 1, 0, false);
                ((PVEGame)Game).SendPlayersPicture(players[0], (int)eLivingPictureType.Targeting, true);
                players[0].Seal(players[0], 0, 0);
            }
            else
            {
                foreach (Player player in Game.GetAllLivingPlayers())
                {
                    if (Game.Random.Next(100) > 50)
                    {
                        ((PVEGame)Game).Createlayer(player.X, player.Y, "", "asset.game.nine.fengyin", "", 1, 0, false);
                        ((PVEGame)Game).SendPlayersPicture(player, (int)eLivingPictureType.Targeting, true);
                        player.Seal(player, 0, 0);
                        player.Properties1 = 2;
                    }
                }
            }
        }

        private void BeatB(Player player)
        {
            Body.PlayMovie("beatB", 100, 6000);
            ((PVEGame)Game).SendFreeFocus(player.X, player.Y - 100, 1, 1, 1);
            Body.CallFuction(new LivingCallBack(() => CreateDiancipaoEffect(player)), 4600);
        }

        private void CreateDiancipaoEffect(Player player)
        {
            Body.CurrentDamagePlus = 10f;
            ((PVEGame)Game).Createlayer(player.X, player.Y, "", "asset.game.nine.diancipao", "", 1, 0, false);
            Body.BeatDirect(player, "", 1, 1, 1);
            player.AddEffect(new ContinueReduceBloodEffect(2, reduceBlood, Body), 100);
        }
        private void RotatePlayers()
        {
            Body.CurrentDamagePlus = 15f;
            for (int i = 0; i < boltMoveXs.Length; i++)
                ((PVEGame)Game).Createlayer(boltMoveXs[i], 600, "", "asset.game.nine.heidong", "in", 1, 0, true);
            foreach (Player player in Game.GetAllLivingPlayers())
            {
                Body.RangeAttacking(500, 1500, "", 1, null);
                Game.LivingChangeAngle(player, -2000, -2000, "");
                int x = boltMoveXs[Game.Random.Next(boltMoveXs.Length)];
                player.BoltMove(x, 600, 50);
            }
            ((PVEGame)Game).SendFreeFocus(585, 600, 1, 1, 1);
            ((PVEGame)Game).SendFreeFocus(1400, 600, 1, 1000, 1);
            Body.CallFuction(StopRotatePlayers, 4200);
        }

        private void StopRotatePlayers()
        {
            List<Player> players = Game.GetAllLivingPlayers();
            foreach (Player player in players)
            {
                player.BoltMove(player.X, 910, 0); // 910
                Game.LivingChangeAngle(player, 0, 0, "stand");
            }
        }

        public override void OnStartAttacking()
        {
            Body.Direction = base.Game.FindlivingbyDir(base.Body);
            switch (turn)
            {
                case 1:
                    MoveBeatA();
                    break;
                case 2:
                    MoveBeatB();
                    break;
                case 3:
                    MoveBeatC();
                    break;
                case 4:
                    MoveBeatA();
                    break;
                case 5:
                    MoveBeatB();
                    break;
                default:
                    turn = 1;
                    goto case 1;
            }
            turn++;
        }

        public override void OnStopAttacking()
        {
            base.OnStopAttacking();
        }
    }
}