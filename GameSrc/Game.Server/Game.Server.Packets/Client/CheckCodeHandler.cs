using Bussiness;
using Game.Base.Packets;
using Game.Server.Managers;
using System;

namespace Game.Server.Packets.Client
{
    [PacketHandler((int)ePackageType.CHECK_CODE, "验证码")]
    public class CheckCodeHandler : IPacketHandler
    {
        //修改:  XiaoJian
        //时间:  2020-11-7
        //描述:  验证码<测试完成>   
        //状态： 正在使用

        public int HandlePacket(GameClient client, GSPacketIn pkg)
        {
            Random rand = new Random();
            int result;
            if (string.IsNullOrEmpty(client.Player.PlayerCharacter.CheckCode))
            {
                result = 1;
            }
            else
            {
                string check = pkg.ReadString();
                if (client.Player.PlayerCharacter.CheckCode.ToLower() == check.ToLower())
                {
                    #region OLD
                    //int GP = LevelMgr.GetGP(client.Player.PlayerCharacter.Grade);
                    /*if (client.Player.PlayerCharacter.ChatCount != 5)
                    {
                        client.Player.AddGP(LevelMgr.IncreaseGP(client.Player.PlayerCharacter.Grade, client.Player.PlayerCharacter.GP));
                        client.Player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("CheckCodeHandler.Msg1", new object[] { client.Player.PlayerCharacter.Grade * 12 }));
                    }*/
                    #endregion

                    //var money = rand.Next(1, 5);
                    //client.Player.AddMoney(money);
                    //client.Player.Out.SendMessage(eMessageType.Normal, string.Format("Đáp đúng. bạn nhận được {0} xu.", money));
                    client.Player.PlayerCharacter.CheckCount = 0;
                    client.Player.PlayerCharacter.ChatCount = 0;
                    client.Player.PlayerCharacter.gameUpToCaptcha = 0;
                    pkg.ClearContext();
                    pkg.WriteByte(1);
                    pkg.WriteBoolean(false);
                    client.Player.Out.SendTCP(pkg);
                }
                else if (client.Player.PlayerCharacter.CheckError <= 3)
                {
                    client.Player.Out.SendMessage(eMessageType.ChatERROR, LanguageMgr.GetTranslation("CheckCodeHandler.Msg3", new object[0]));
                    client.Player.PlayerCharacter.CheckError++;
                    client.Player.Out.SendCheckCode();
                }
                else
                {
                    client.Player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("CheckCodeHandler.Msg3", new object[0]));
                    client.Player.Disconnect();
                }
                result = 0;
            }
            return result;
        }
    }
}

