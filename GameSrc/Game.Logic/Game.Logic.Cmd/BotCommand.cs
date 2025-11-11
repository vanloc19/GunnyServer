//using Bussiness.Managers;
//using Game.Base.Packets;
//using Game.Logic.Phy.Object;
//using SqlDataProvider.Data;
//using System;
//using System.Collections.Generic;
//using System.Linq;

//namespace Game.Logic.Cmd
//{
//    [GameCommand(143, "战胜关卡中Boss翻牌")]
//    public class BotCommand : ICommandHandler
//    {
//        public static int daocu5;
//        private Random A = new Random();
//        private float kill(int Ax, int Bx)
//        {
//            float time = 1f;
//            if (Math.Abs(Ax - Bx) < 150)
//            {
//                time = 1f;
//            }
//            else if (Math.Abs(Ax - Bx) < 300)
//            {
//                time = 1.5f;
//            }
//            else if (Math.Abs(Ax - Bx) < 500)
//            {
//                time = 2f;
//            }
//            else if (Math.Abs(Ax - Bx) < 700)
//            {
//                time = 2.5f;
//            }
//            else if (Math.Abs(Ax - Bx) < 900)
//            {
//                time = 3f;
//            }
//            else
//            {
//                time = 3.5f;
//            }
//            return time;
//        }
//        public void HandleCommand(BaseGame game, Player player, GSPacketIn packet)
//        {
//            if (game is PVPGame)
//            {
//                PVPGame pVPGame = game as PVPGame;
//                Player[] allPlayers = pVPGame.GetAllPlayers();
//                List<Player> list = new List<Player>();
//                Player[] array = allPlayers;
//                for (int i = 0; i < array.Length; i++)
//                {
//                    Player player2 = array[i];
//                    if (player2.Team != player.Team)
//                    {
//                        list.Add(player2);
//                    }
//                }
//                Random random = new Random();
//                int index = random.Next(0, list.Count);
//                Player player3 = list.ElementAt(index);
//                if (player3.X > player.X)
//                {
//                    player.ChangeDirection(1, 500);
//                }
//                else
//                {
//                    player.ChangeDirection(-1, 500);
//                }
//                Random random2 = new Random();
//                int num = random2.Next(0, 3);
//                float time = 1f;
//                int bombCount; // số  tia
//                int x;
//                int y;
//                int num2 = 0;
//                int num3 = 0;
//                int num4; // số lần dame
//                if (player3.X > player.X)// bot bên trái
//                {
//                    if (Math.Abs(player.X - player3.X) > 1500)
//                    {
//                        num4 = 1;
//                        bombCount = 1;
//                        x = player3.X;
//                        y = player3.Y;
//                        num2 = 10016;
//                        num3 = 10010;
//                        ItemTemplateInfo item2 = ItemMgr.FindItemTemplate(num2);
//                        player.UseItem(item2);
//                        ItemTemplateInfo item3 = ItemMgr.FindItemTemplate(num3);
//                        player.UseItem(item3);
//                        int b = A.Next(1, 3);
//                        if (b == 2)
//                            x = player3.X - A.Next(50, 350);
//                        else
//                            x = player3.X + A.Next(50, 150);
//                    }
//                    else
//                    {
//                        if (Math.Abs(player.X - player3.X) < 50)
//                        {
//                            num4 = 1;
//                            bombCount = 1;
//                            x = player3.X;
//                            y = player3.Y;
//                            num2 = 10016;
//                            num3 = 10010;
//                            ItemTemplateInfo item2 = ItemMgr.FindItemTemplate(num2);
//                            player.UseItem(item2);
//                            ItemTemplateInfo item3 = ItemMgr.FindItemTemplate(num3);
//                            player.UseItem(item3);
//                            int b = A.Next(1, 3);
//                            if (b == 2)
//                                x = player3.X - A.Next(50, 350);
//                            else
//                                x = player3.X + A.Next(50, 350);
//                        }
//                        else
//                        {
//                            bombCount = 1;
//                            int a = A.Next(1, 3);
//                            if (a == 2)
//                            {
//                                num4 = 2;
//                                ItemTemplateInfo item2 = ItemMgr.FindItemTemplate(10002);
//                                player.UseItem(item2);
//                            }
//                            else
//                            {
//                                num4 = 3;
//                                ItemTemplateInfo item2 = ItemMgr.FindItemTemplate(10001);
//                                player.UseItem(item2);
//                            }
//                            ItemTemplateInfo item3 = ItemMgr.FindItemTemplate(10004);
//                            player.UseItem(item3);
//                            ItemTemplateInfo item4 = ItemMgr.FindItemTemplate(10004);
//                            player.UseItem(item4);
//                            x = player3.X;
//                            y = player3.Y;
//                            time = this.kill(player.X, player3.X);
//                        }
//                    }
//                }
//                else// bot bên phải
//                {
//                    if (Math.Abs(player.X - player3.X) > 1500)
//                    {
//                        num4 = 1;
//                        bombCount = 1;
//                        x = player3.X;
//                        y = player3.Y;
//                        num2 = 10016;
//                        num3 = 10010;
//                        ItemTemplateInfo item2 = ItemMgr.FindItemTemplate(num2);
//                        player.UseItem(item2);
//                        ItemTemplateInfo item3 = ItemMgr.FindItemTemplate(num3);
//                        player.UseItem(item3);
//                        int b = A.Next(1, 3);
//                        if (b == 2)
//                            x = player3.X + A.Next(50, 350);
//                        else
//                            x = player3.X - A.Next(50, 150);
//                    }
//                    else
//                    {
//                        if (player.Y >= (player3.Y + 150))
//                        {
//                            bombCount = 1;
//                            int a = A.Next(1, 3);
//                            if (a == 2)
//                            {
//                                num4 = 2;
//                                ItemTemplateInfo item2 = ItemMgr.FindItemTemplate(10002);
//                                player.UseItem(item2);
//                            }
//                            else
//                            {
//                                num4 = 3;
//                                ItemTemplateInfo item2 = ItemMgr.FindItemTemplate(10001);
//                                player.UseItem(item2);
//                            }
//                            ItemTemplateInfo item3 = ItemMgr.FindItemTemplate(10004);
//                            player.UseItem(item3);
//                            ItemTemplateInfo item4 = ItemMgr.FindItemTemplate(10004);
//                            player.UseItem(item4);
//                            x = player3.X;
//                            y = player3.Y;
//                            time = this.kill(player.X, player3.X);
//                        }
//                        else
//                        {
//                            if (player.X - player3.X < 800)
//                            {
//                                bombCount = 1;
//                                ItemTemplateInfo item2 = ItemMgr.FindItemTemplate(10001);
//                                player.UseItem(item2);
//                                int[] prop = { 10004, 10005, 10006, 10007, 10008 };
//                                /*for (int i = 0; i < prop.Length; i++)
//                                {
//                                    ItemTemplateInfo item3 = ItemMgr.FindItemTemplate(prop[i]);
//                                    player.UseItem(item3);
//                                }*/
//                                foreach (var i in prop)
//                                {
//                                    Console.WriteLine($"Prop Template = {i}");
//                                    ItemTemplateInfo itemProp = ItemMgr.FindItemTemplate(i);
//                                    player.UseItem(itemProp); ;
//                                }
//                                if (A.Next(0, 4) == 2)
//                                {
//                                    ItemTemplateInfo item4 = ItemMgr.FindItemTemplate(10017);
//                                    player.UseItem(item4);
//                                }
//                                //bombCount = 3;
//                                num4 = 3;
//                                x = player3.X;
//                                y = player3.Y;
//                                time = this.kill(player.X, player3.X);
//                            }
//                            else
//                            {
//                                bombCount = 1;
//                                int a = A.Next(1, 3);
//                                if (a == 2)
//                                {
//                                    num4 = 2;
//                                    ItemTemplateInfo item2 = ItemMgr.FindItemTemplate(10002);
//                                    player.UseItem(item2);
//                                }
//                                else
//                                {
//                                    num4 = 3;
//                                    ItemTemplateInfo item2 = ItemMgr.FindItemTemplate(10001);
//                                    player.UseItem(item2);
//                                }
//                                ItemTemplateInfo item3 = ItemMgr.FindItemTemplate(10004);
//                                player.UseItem(item3);
//                                ItemTemplateInfo item4 = ItemMgr.FindItemTemplate(10004);
//                                player.UseItem(item4);
//                                x = player3.X;
//                                y = player3.Y;
//                                time = this.kill(player.X, player3.X);
//                            }
//                        }
//                    }
//                }
//                for (int j = 0; j < num4; j++)
//                {
//                    player.ShootPoint(x, y, player.CurrentBall.ID, 1001, 10001, bombCount, time, 2000);
//                }
//                if (player.IsAttacking)
//                {
//                    player.StopAttacking();
//                }
//                GSPacketIn gSPacketIn = new GSPacketIn(91, player.Id);
//                gSPacketIn.WriteByte(143);
//                game.SendToAll(gSPacketIn);
//            }
//        }
//    }
//}

// Decompiled with JetBrains decompiler
// Type: Game.Logic.Cmd.BotCommand
// Assembly: Game.Logic, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7CB02CBF-9A8D-45F4-A081-382DECB3589E
// Assembly location: C:\Users\Hung Pham\Desktop\GunGa\Game.Logic.dll

using Bussiness.Managers;
using Game.Base.Packets;
using Game.Logic.Phy.Object;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Logic.Cmd
{
    [GameCommand(143, "սʤ\x00B9ؿ¨\x0590Boss·\x00ADņ")]
    public class BotCommand : ICommandHandler
    {
        public void HandleCommand(BaseGame game, Player player, GSPacketIn packet)
        {
            if (!(game is PVPGame))
                return;
            Player[] allPlayers = (game as PVPGame).GetAllPlayers();
            List<Player> source = new List<Player>();
            foreach (Player player1 in allPlayers)
            {
                if (player1.IsQuanChien)
                    continue;

                if (player1.Team != player.Team)
                    source.Add(player1);
            }
            int index1 = new Random().Next(0, source.Count);
            Player player2 = source.ElementAt<Player>(index1);
            if (player2.X > player.X)
                player.ChangeDirection(1, 500);
            else
                player.ChangeDirection(-1, 500);
            int num1 = new Random().Next(0, 3);
            float time = 1f;
            int bombCount;
            int x;
            int y;
            int num2;
            if (Math.Abs(player.X - player2.X) > 60)
            {
                int templateId1;
                int templateId2;
                int templateId3;
                if (num1 == 0)
                {
                    bombCount = 1;
                    templateId1 = 10002;
                    templateId2 = 10004;
                    templateId3 = 10004;
                    x = player2.X;
                    y = player2.Y;
                    num2 = 2;
                }
                else if (player2.X < player.X && player.X - player2.X > 200 && player.X - player2.X < 800)
                {
                    bombCount = 3;
                    templateId1 = 10001;
                    templateId2 = 10003;
                    templateId3 = 10004;
                    x = player2.X + 20;
                    y = player2.Y;
                    num2 = 3;
                }
                else if (Math.Abs(player.X - player2.X) > 1200)
                {
                    x = player.X <= player2.X ? player2.X - 300 : player2.X + 300;
                    bombCount = 1;
                    templateId1 = 0;
                    templateId2 = 10016;
                    templateId3 = 10010;
                    y = player2.Y - 100;
                    num2 = 1;
                }
                else
                {
                    bombCount = 1;
                    templateId1 = 10001;
                    templateId2 = 10004;
                    templateId3 = 10004;
                    x = player2.X;
                    y = player2.Y;
                    num2 = 3;
                }
                if ((uint)templateId1 > 0U)
                {
                    ItemTemplateInfo itemTemplate = ItemMgr.FindItemTemplate(templateId1);
                    player.UseItem(itemTemplate);
                }
                if ((uint)templateId2 > 0U)
                {
                    ItemTemplateInfo itemTemplate = ItemMgr.FindItemTemplate(templateId2);
                    player.UseItem(itemTemplate);
                }
                ItemTemplateInfo itemTemplate1 = ItemMgr.FindItemTemplate(templateId3);
                player.UseItem(itemTemplate1);
                time = Math.Abs(player.X - player2.X) >= 200 ? (Math.Abs(player.X - player2.X) >= 400 ? (Math.Abs(player.X - player2.X) >= 700 ? (Math.Abs(player.X - player2.X) >= 1000 ? (Math.Abs(player.X - player2.X) >= 1100 ? 3.5f : 3f) : 2.5f) : 2f) : 1.5f) : 1f;
            }
            else if ((uint)num1 > 0U)
            {
                bombCount = 1;
                int templateId1 = 10010;
                int templateId2 = 10016;
                y = player.Y;
                num2 = 1;
                time = 4f;
                x = player.X <= 700 ? player.X + 600 : player.X - 600;
                ItemTemplateInfo itemTemplate1 = ItemMgr.FindItemTemplate(templateId1);
                player.UseItem(itemTemplate1);
                ItemTemplateInfo itemTemplate2 = ItemMgr.FindItemTemplate(templateId2);
                player.UseItem(itemTemplate2);
            }
            else
            {
                bombCount = 1;
                int templateId1 = 10001;
                int templateId2 = 10004;
                int templateId3 = 10004;
                x = player2.X;
                y = player2.Y;
                num2 = 3;
                ItemTemplateInfo itemTemplate1 = ItemMgr.FindItemTemplate(templateId1);
                player.UseItem(itemTemplate1);
                ItemTemplateInfo itemTemplate2 = ItemMgr.FindItemTemplate(templateId2);
                player.UseItem(itemTemplate2);
                ItemTemplateInfo itemTemplate3 = ItemMgr.FindItemTemplate(templateId3);
                player.UseItem(itemTemplate3);
            }
            for (int index2 = 0; index2 < num2; ++index2)
                player.ShootPoint(x, y, player.CurrentBall.ID, 1001, 10001, bombCount, time, 2000);
            if (player.IsAttacking)
                player.StopAttacking();
            GSPacketIn pkg = new GSPacketIn((short)91, player.Id);
            pkg.WriteByte((byte)143);
            game.SendToAll(pkg);
        }
    }
}

