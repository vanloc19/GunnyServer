using System;
using System.Collections.Generic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.NPC
{
    // Token: 0x0200008C RID: 140
    public class SimpleNpcAi : ABrain
    {
        // Token: 0x0600085E RID: 2142 RVA: 0x00015366 File Offset: 0x00013566
        public override void OnBeginSelfTurn()
        {
            base.OnBeginSelfTurn();
        }

        // Token: 0x0600085F RID: 2143 RVA: 0x0003D374 File Offset: 0x0003B574
        public override void OnBeginNewTurn()
        {
            base.OnBeginNewTurn();
            this.m_body.CurrentDamagePlus = 1f;
            this.m_body.CurrentShootMinus = 1f;
            bool isSay = this.m_body.IsSay;
            bool flag = isSay;
            if (flag)
            {
                string oneChat = SimpleNpcAi.GetOneChat();
                int delay = base.Game.Random.Next(0, 5000);
                this.m_body.Say(oneChat, 0, delay);
            }
        }

        // Token: 0x06000860 RID: 2144 RVA: 0x00015370 File Offset: 0x00013570
        public override void OnCreated()
        {
            base.OnCreated();
        }

        // Token: 0x06000861 RID: 2145 RVA: 0x0003D3E8 File Offset: 0x0003B5E8
        public override void OnStartAttacking()
        {
            base.OnStartAttacking();
            this.m_targer = base.Game.FindNearestPlayer(base.Body.X, base.Body.Y);
            this.Beating();
        }

        // Token: 0x06000862 RID: 2146 RVA: 0x000154F0 File Offset: 0x000136F0
        public override void OnStopAttacking()
        {
            base.OnStopAttacking();
        }

        // Token: 0x06000863 RID: 2147 RVA: 0x0003D420 File Offset: 0x0003B620
        public void MoveToPlayer(Player player)
        {
            int num = (int)player.Distance(base.Body.X, base.Body.Y);
            int num2 = base.Game.Random.Next(((SimpleNpc)base.Body).NpcInfo.MoveMin, ((SimpleNpc)base.Body).NpcInfo.MoveMax);
            bool flag = num <= 97;
            bool flag2 = !flag;
            if (flag2)
            {
                num = ((num <= ((SimpleNpc)base.Body).NpcInfo.MoveMax) ? (num - 90) : num2);
                bool flag3 = player.Y < 420 && player.X < 210;
                bool flag4 = flag3;
                if (flag4)
                {
                    bool flag5 = base.Body.Y > 420;
                    bool flag6 = flag5;
                    if (flag6)
                    {
                        bool flag7 = base.Body.X - num < 50;
                        bool flag8 = flag7;
                        if (flag8)
                        {
                            base.Body.MoveTo(25, base.Body.Y, "walk", 1200, "", ((SimpleNpc)base.Body).NpcInfo.speed, new LivingCallBack(this.Jump));
                        }
                        else
                        {
                            base.Body.MoveTo(base.Body.X - num, base.Body.Y, "walk", 1200, "", ((SimpleNpc)base.Body).NpcInfo.speed, new LivingCallBack(this.MoveBeat));
                        }
                    }
                    else
                    {
                        bool flag9 = player.X > base.Body.X;
                        bool flag10 = flag9;
                        if (flag10)
                        {
                            base.Body.MoveTo(base.Body.X + num, base.Body.Y, "walk", 1200, "", ((SimpleNpc)base.Body).NpcInfo.speed, new LivingCallBack(this.MoveBeat));
                        }
                        else
                        {
                            base.Body.MoveTo(base.Body.X - num, base.Body.Y, "walk", 1200, "", ((SimpleNpc)base.Body).NpcInfo.speed, new LivingCallBack(this.MoveBeat));
                        }
                    }
                }
                else
                {
                    bool flag11 = base.Body.Y < 420;
                    bool flag12 = flag11;
                    if (flag12)
                    {
                        bool flag13 = base.Body.X + num > 200;
                        bool flag14 = flag13;
                        if (flag14)
                        {
                            base.Body.MoveTo(200, base.Body.Y, "walk", 1200, "", ((SimpleNpc)base.Body).NpcInfo.speed, new LivingCallBack(this.Fall));
                        }
                    }
                    else
                    {
                        bool flag15 = player.X > base.Body.X;
                        bool flag16 = flag15;
                        if (flag16)
                        {
                            base.Body.MoveTo(base.Body.X + num, base.Body.Y, "walk", 1200, "", ((SimpleNpc)base.Body).NpcInfo.speed, new LivingCallBack(this.MoveBeat));
                        }
                        else
                        {
                            base.Body.MoveTo(base.Body.X - num, base.Body.Y, "walk", 1200, "", ((SimpleNpc)base.Body).NpcInfo.speed, new LivingCallBack(this.MoveBeat));
                        }
                    }
                }
            }
        }

        // Token: 0x06000864 RID: 2148 RVA: 0x0003D7FD File Offset: 0x0003B9FD
        public void MoveBeat()
        {
            base.Body.Beat(this.m_targer, "beatA", 100, 0, 0, 1, 1);
        }

        // Token: 0x06000865 RID: 2149 RVA: 0x0003D81D File Offset: 0x0003BA1D
        public void FallBeat()
        {
            base.Body.Beat(this.m_targer, "beatA", 100, 0, 2000, 1, 1);
        }

        // Token: 0x06000866 RID: 2150 RVA: 0x0003D844 File Offset: 0x0003BA44
        public void Jump()
        {
            base.Body.Direction = 1;
            base.Body.JumpTo(base.Body.X, base.Body.Y - 240, "Jump", 0, 2, 3, new LivingCallBack(this.Beating));
        }

        // Token: 0x06000867 RID: 2151 RVA: 0x0003D89C File Offset: 0x0003BA9C
        public void Beating()
        {
            bool flag = this.m_targer != null && !base.Body.Beat(this.m_targer, "beatA", 100, 0, 0, 1, 1);
            bool flag2 = flag;
            if (flag2)
            {
                this.MoveToPlayer(this.m_targer);
            }
        }

        // Token: 0x06000868 RID: 2152 RVA: 0x0003D8EC File Offset: 0x0003BAEC
        public void Fall()
        {
            base.Body.FallFrom(base.Body.X, base.Body.Y + 240, null, 0, 0, 12, new LivingCallBack(this.Beating));
        }

        // Token: 0x06000869 RID: 2153 RVA: 0x0003D934 File Offset: 0x0003BB34
        public static string GetOneChat()
        {
            int num = SimpleNpcAi.random_0.Next(0, SimpleNpcAi.string_0.Length);
            return SimpleNpcAi.string_0[num];
        }

        // Token: 0x0600086A RID: 2154 RVA: 0x0003D960 File Offset: 0x0003BB60
        public static void LivingSay(List<Living> livings)
        {
            bool flag = livings == null || livings.Count == 0;
            bool flag2 = !flag;
            if (flag2)
            {
                int count = livings.Count;
                foreach (Living living in livings)
                {
                    living.IsSay = false;
                }
                int num = (count <= 5) ? SimpleNpcAi.random_0.Next(0, 2) : ((count <= 5 || count > 10) ? SimpleNpcAi.random_0.Next(1, 4) : SimpleNpcAi.random_0.Next(1, 3));
                bool flag3 = num <= 0;
                bool flag4 = !flag3;
                if (flag4)
                {
                    int i = 0;
                    while (i < num)
                    {
                        int index = SimpleNpcAi.random_0.Next(0, count);
                        bool flag5 = !livings[index].IsSay;
                        bool flag6 = flag5;
                        if (flag6)
                        {
                            livings[index].IsSay = true;
                            int delay = SimpleNpcAi.random_0.Next(0, 5000);
                            livings[index].Say(SimpleNpcAi.GetOneChat(), 0, delay);
                            i++;
                        }
                    }
                }
            }
        }

        // Token: 0x04000395 RID: 917
        protected Player m_targer;

        // Token: 0x04000396 RID: 918
        private static Random random_0 = new Random();

        // Token: 0x04000397 RID: 919
        private static string[] string_0 = new string[]
        {
            "Để tôn vinh! Để giành chiến thắng! !",
            "Tổ chức cướp vũ khí của họ, không được run sợ!",
            "Không được bỏ cuộc!",
            "Kẻ thù ở phía trước, sẵn sàng chiến đấu!",
            "Tiến lên tiêu diệt kẻ thù",
            "Rời khỏi đây nếu không đừng trách!",
            "Nhanh chóng tiêu diệt kẻ thù!",
            "Sức mạnh số một!",
            "Với một sửa chữa nhanh chóng!",
            "Vây quanh kẻ thù và tiêu diệt chúng.",
            "Quân tiếp viện! Quân tiếp viện! Chúng tôi cần thêm quân tiếp viện!",
            "Hy sinh bản thân, sẽ không cho phép bạn có được đi với!",
            "Đừng đánh giá thấp sức mạnh của chúng tôi, nếu không bạn sẽ phải trả cho việc này."
        };
    }
}
