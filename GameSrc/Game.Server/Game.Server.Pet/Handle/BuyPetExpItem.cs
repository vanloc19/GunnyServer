using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Logic;
using Game.Server.GameObjects;
using Game.Server.Packets;
using SqlDataProvider.Data;
using System;

namespace Game.Server.Pet.Handle
{
    [global::Pet(19)]
    public class BuyPetExpItem : IPetCommandHadler
    {
        public bool CommandHandler(GamePlayer player, GSPacketIn pkg)
        {
            bool isBand = pkg.ReadBoolean();
            if (player.PlayerCharacter.HasBagPassword && player.PlayerCharacter.IsLocked)
            {
                player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bag.Locked"));
                return false;
            }
            if (player.PlayerCharacter.Grade < 999)
            {
                player.Out.SendMessage(eMessageType.Normal, "Chức năng tạm đóng!");
                return false;
            }

            UserFarmInfo farm = player.Farm.CurrentFarm;
            int timeBuy = farm.buyExpRemainNum;
            PetExpItemPriceInfo info = PetMgr.FindPetExpItemPrice(RealMoney(timeBuy));
            if (info == null || timeBuy < 1)
                return false;
            bool isContinue = false;
            int needMoney = info.Money;
            if (!isBand)
            {
                #region bug
                /*if (needMoney <= player.PlayerCharacter.Money)
                {
                    player.RemoveMoney(needMoney);
                    player.SendMessage(string.Format("Mua thành công!"));
                    isContinue = true;
                }*/
                #endregion
                if (player.MoneyDirect(needMoney, IsAntiMult: false)) ;
                {
                    player.SendMessage("mua thành công!");
                    isContinue = true;
                }
            }
            else
            {
                if (needMoney <= player.PlayerCharacter.GiftToken)
                {
                    player.RemoveGiftToken(needMoney);
                    isContinue = true;
                }
            }

            if (!isContinue)
            {
                player.SendMessage("Bạn không đủ tài chính để mua vật phẩm này!");
                return false;
            }

            ItemTemplateInfo item = ItemMgr.FindItemTemplate(334102);
            ItemInfo cloneItem = ItemInfo.CreateFromTemplate(item, info.ItemCount, 102);
            cloneItem.IsBinds = true;
            player.AddTemplate(cloneItem, cloneItem.Template.BagType, info.ItemCount, eGameView.RouletteTypeGet);
            farm.buyExpRemainNum--;
            GSPacketIn response = new GSPacketIn(68);
            response.WriteByte((byte)FarmPackageType.BUY_PET_EXP_ITEM);
            response.WriteInt(farm.buyExpRemainNum);
            player.SendTCP(response);
            player.Farm.UpdateFarm(farm);

            return false;
        }

        private int RealMoney(int timebuy)
        {
            switch (timebuy)
            {
                case 19:
                    return 2;
                case 18:
                    return 3;
                case 17:
                    return 4;
                case 16:
                    return 5;
                case 15:
                    return 6;
                case 14:
                    return 7;
                case 13:
                    return 8;
                case 12:
                    return 9;
                case 11:
                    return 10;
                case 10:
                    return 11;
                case 9:
                    return 12;
                case 8:
                    return 13;
                case 7:
                    return 14;
                case 6:
                    return 15;
                case 5:
                    return 16;
                case 4:
                    return 17;
                case 3:
                    return 18;
                case 2:
                    return 19;
                case 1:
                    return 20;
            }

            return 1;
        }
    }
}