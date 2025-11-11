using Game.Logic.Phy.Object;
using Game.Base.Packets;
using System;
using Bussiness;
using SqlDataProvider.Data;

namespace Game.Logic.Cmd
{
    [GameCommand(119, "关卡失败再试一次")]//Byte(119);
    public class TryAgainCommand : ICommandHandler
    {
        public void HandleCommand(BaseGame game, Player player, GSPacketIn packet)
        {
            if (game is PVEGame)
            {
                PVEGame pve = game as PVEGame;
                var MissionAgain = packet.ReadInt();
                bool tryAgain = packet.ReadBoolean();
                bool isHost = packet.ReadBoolean();

                if (isHost)
                {
                    if (tryAgain)
                    {
                        if (MissionAgain == 1)
                        {
                            if (player.PlayerDetail.PlayerCharacter.HasBagPassword && player.PlayerDetail.PlayerCharacter.IsLocked)
                            {
                                player.PlayerDetail.SendMessage("Túi chưa mở khóa, không thể tiếp phí ải!");
                                pve.WantTryAgain = 0;
                            }
                            else
                            {
                                bool canOpen = false;
                                bool check = false;
                                BufferInfo buffInfo = player.GetFightBuffByType(BuffType.Level_Try);
                                if (buffInfo != null && !game.IsSpecialPVE() && player.PlayerDetail.UsePayBuff(BuffType.Level_Try))
                                {
                                    string msg1 = string.Format("Bạn được một lần tiếp phí ải miễn phí từ ưu đãi Chúc Phúc Thần Gà!");
                                    player.PlayerDetail.SendMessage(msg1);
                                    canOpen = true;
                                    check = true;
                                }
                                else
                                {
                                    canOpen = player.PlayerDetail.RemoveMoney(500, isConsume:true) > 0 ? true : false;
                                }

                                if (canOpen)
                                {
                                    if (pve.WantTryAgain == 2)
                                    {
                                        //退回关卡结算
                                        pve.WantTryAgain = 1;
                                        string msg = string.Format("Bạn tiếp phí thành công!");
                                        string msg1 = "";

                                        if (check)
                                        {
                                            msg1 = string.Format(", miễn phí 1 lần từ Ưu đãi Chúc Phúc Thần Gà");

                                        }
                                        pve.OpenTryAgain = true;
                                        player.PlayerDetail.SendMessage(msg1);
                                    }
                                }
                                else
                                {
                                    player.PlayerDetail.SendInsufficientMoney((int)eBattleRemoveMoneyType.TryAgain);
                                }
                            }
                        }
                        else
                        {
                            //退回房间
                            pve.WantTryAgain = 0;
                        }
                    }
                    else
                    {


                        //退回房间
                        pve.WantTryAgain = 0;
                    }
                    pve.SendMissionTryAgain();

                    pve.ClearWaitTimer();

                    pve.CheckState(0);

                }
            }
        }
    }
}
