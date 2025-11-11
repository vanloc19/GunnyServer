using System;
using System.Collections.Generic;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Packets;
using Newtonsoft.Json;
using SqlDataProvider.Data;

namespace Game.Server.SanXiao.Handle
{
    [SanXiaoHandleAttbute((byte)SanXiaoPackageType.BUY_ITEM)]
    public class BuyItems : ISanXiaoCommandHadler
    {
        public int CommandHandler(GamePlayer player, GSPacketIn packet)
        {
            int shopId = packet.ReadInt();
            int count = packet.ReadInt();

            Console.Write("ShopId = {0}, count", shopId, count);

            if (count <= 0 || count > 999)
                return 1;

            MinigameShopTemplateInfo shopItem = ShopMgr.FindMinigameShop(shopId);
            if (shopItem != null)
            {
                if (shopItem.Type == 1)
                {
                    int price = count * shopItem.Price;
                    if(player.Actives.Info.SXCrystal < price)
                    {
                        player.SendMessage("Thuỷ tinh không đủ!");
                    }
                    else
                    {
                        Dictionary<int, int> nums = JsonConvert.DeserializeObject<Dictionary<int, int>>(player.Actives.Info.MiniShopBuyCount) ?? new Dictionary<int, int>();
                        if (!nums.ContainsKey(shopItem.ID) || nums[shopItem.ID] + count <= shopItem.LimitCount)
                        {
                            ActiveSystemInfo info = player.Actives.Info;
                            info.SXCrystal = info.SXCrystal - price;
                            if (nums.ContainsKey(shopItem.ID))
                            {
                                Dictionary<int, int> item = nums;
                                int d = shopItem.ID;
                                item[d] = item[d] + count;
                            }
                            else
                            {
                                nums.Add(shopItem.ID, count);
                            }
                            player.Actives.Info.MiniShopBuyCount = JsonConvert.SerializeObject(nums);
                            ItemInfo isBind = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(shopItem.ItemID), shopItem.Count * count, 105);
                            isBind.IsBinds = shopItem.IsBind;
                            isBind.ValidDate = shopItem.Valid;
                            player.AddTemplate(isBind);
                            player.Actives.SendSXStoreData(nums);
                        }
                        else
                        {
                            player.SendMessage(LanguageMgr.GetTranslation("SanXiao.Shop.LimitCount.Msg", Array.Empty<object>()));
                        }
                    }
                    return 1;
                }
            }
            #region OLD
            /*if (shopItem == null || shopItem.Type != 1)
                return 1;

            int cryStalPay = count * shopItem.Price;
            // check crystal
            if (player.Actives.Info.SXCrystal >= cryStalPay)
            {
                // count buy
                Dictionary<int, int> storeBuys =
                    JsonConvert.DeserializeObject<Dictionary<int, int>>(player.Actives.Info.MiniShopBuyCount);
                if (storeBuys == null)
                    storeBuys = new Dictionary<int, int>();

                if (!storeBuys.ContainsKey(shopItem.ID) || (storeBuys[shopItem.ID] + count) <= shopItem.LimitCount)
                {
                    player.Actives.Info.SXCrystal -= cryStalPay;

                    if (!storeBuys.ContainsKey(shopItem.ID))
                        storeBuys.Add(shopItem.ID, count);
                    else
                        storeBuys[shopItem.ID] += count;

                    player.Actives.Info.MiniShopBuyCount = JsonConvert.SerializeObject(storeBuys);

                    ItemInfo item = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(shopItem.ItemID),
                        shopItem.Count * count, 105);
                    item.IsBinds = shopItem.IsBind;
                    item.ValidDate = shopItem.Valid;

                    player.AddTemplate(item);

                    player.Actives.SendSXStoreData(storeBuys);
                }
                else
                {
                    // qua gioi han
                    player.SendMessage(LanguageMgr.GetTranslation("SanXiao.Shop.LimitCount.Msg"));
                }
            }
            else
            {
                // not enouch crystal
                player.SendMessage(LanguageMgr.GetTranslation("SanXiao.Shop.NoCrystal.Msg"));
            }
            */
            #endregion
            return 1;
        }
    }
}