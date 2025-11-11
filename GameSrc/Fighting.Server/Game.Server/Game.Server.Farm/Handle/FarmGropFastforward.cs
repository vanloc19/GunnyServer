using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Packets;
using System;

namespace Game.Server.Farm.Handle
{
    [FarmHandleAttbute((byte)FarmPackageType.FRAM_GROP_FASTFORWARD)]
    public class FarmGropFastforward : IFarmCommandHadler
    {
        public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
        {
            bool isBand = packet.ReadBoolean();
            bool isAllField = packet.ReadBoolean();
            int fieldId = packet.ReadInt();
            int ripNum = Player.Farm.ripeNum();
            int needMoney = GameProperties.FastGrowNeedMoney * ripNum;
            if (needMoney <= 0)
                return false;
            if (!isBand)
            {
                //=> Toàn bộ cây trồng.
                if (isAllField)
                {
                    if (Player.MoneyDirect(needMoney, IsAntiMult: false))
                    {
                        Player.SendMessage($"Rút ngắn 30 phút thời gian của {ripNum} cây trồng thành công. tốn {needMoney} xu.");
                        Player.Farm.GropFastforward(isAllField, fieldId);
                    }
                }
                //=> 1 cây trồng.
                else
                {
                    if (Player.MoneyDirect(5000, IsAntiMult: false))
                    {
                        Player.SendMessage($"Rút ngắn 30 phút thời gian của 1 cây trồng thành công. tốn 5000 xu.");
                        Player.Farm.GropFastforward(isAllField, fieldId);
                    }
                }
            }
            #region OLD
            /*if (isBand)
            {
                if (needMoney <= Player.PlayerCharacter.GiftToken)
                {
                    Player.RemoveGiftToken(needMoney);
                    Player.SendMessage(string.Format("Thành công! bạn đã rút ngắn 30 phút thời gian trồng của {0} cây", ripNum));
                    Player.Farm.GropFastforward(isAllField, fieldId);
                }
            }
            else
            {
                if (Player.MoneyDirect(needMoney, IsAntiMult: false))
                {
                    Player.SendMessage($"Rút ngắn 30 phút thời gian của {ripNum} cây trồng thành công. tốn {needMoney} xu.");
                    Player.Farm.GropFastforward(isAllField, fieldId);
                }
            }*/
            #endregion
            return true;
        }
    }
}