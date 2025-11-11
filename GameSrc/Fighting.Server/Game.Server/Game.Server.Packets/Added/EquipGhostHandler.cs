//using System;
//using System.Collections.Generic;
//using System.Linq;
//using Bussiness;
//using Game.Base.Packets;
//using Game.Server.Managers;
//using SqlDataProvider.Data;

//namespace Game.Server.Packets.Client
//{
//    [PacketHandler(391, "user ac action")]
//    public class EquipGhostHandler : IPacketHandler
//    {
//        public static Random random = new Random();

//        public int HandlePacket(GameClient client, GSPacketIn packet)
//        {
//            ItemInfo item = client.Player.StoreBag.GetItemAt(1);
//            ItemInfo luckItem = client.Player.StoreBag.GetItemAt(0);
//            ItemInfo stone = client.Player.StoreBag.GetItemAt(2);
//            if (item == null || stone == null || stone.Template.Property1 != 118)
//            {
//                client.Player.SendMessage(LanguageMgr.GetTranslation("GameServer.EquipGhost.Msg1"));
//                return 0;
//            }

//            if (luckItem != null && luckItem.Template.Property1 != 117)
//            {
//                client.Player.SendMessage(LanguageMgr.GetTranslation("GameServer.EquipGhost.Msg4"));
//                return 0;
//            }

//            // get spirit item
//            List<SpiritInfo> spiList = SpiritInfoMgr.GetSpirit(item.Template.CategoryID);

//            if (spiList.Count <= 0)
//            {
//                client.Player.SendMessage(LanguageMgr.GetTranslation("GameServer.EquipGhost.Msg2"));
//                return 0;
//            }

//            // get equip ghost
//            UserEquipGhostInfo equip = client.Player.GetGhostEquip(spiList[0].BagType, spiList[0].BagPlace);

//            if (equip == null)
//            {
//                equip = new UserEquipGhostInfo();
//                equip.UserID = client.Player.PlayerId;
//                equip.BagType = spiList[0].BagType;
//                equip.Place = spiList[0].BagPlace;
//                equip.Level = 0;
//                equip.TotalGhost = 0;
//                client.Player.AddEquipGhost(equip);
//            }

//            // try math fucking text
//            SpiritInfo nextLevelInfo = spiList.SingleOrDefault(a => a.Level == equip.Level + 1);

//            if (nextLevelInfo != null)
//            {
//                double luckratio = (luckItem != null) ? (1 + (float)luckItem.Template.Property2 / 100f) : 1f;
//                double rawRatio = 5f * Math.Pow(2f, Math.Pow(2f, (stone.Template.Level - 1f)) + 2f - (float)nextLevelInfo.Level) * luckratio;

//                client.Player.StoreBag.RemoveCountFromStack(stone, 1);
//                client.Player.StoreBag.RemoveCountFromStack(luckItem, 1);

//                bool isSuccess = false;
//                var rnd = random.Next(0, 100000);
//                if ((rawRatio * 1000) * 2 >= rnd)
//                {
//                    isSuccess = true;
//                    equip.Level++;
//                    client.Player.EquipBag.UpdatePlayerProperties();
//                    client.Out.SendUserSyncEquipGhost(client.Player);
//                    GameServer.Instance.LoginServer.SendPacket(WorldMgr.SendSysNotice(eMessageType.ChatNormal, LanguageMgr.GetTranslation("ItemEquipGhost.Success", client.Player.ZoneName, client.Player.PlayerCharacter.NickName, item.TemplateID, equip.Level), item.ItemID, item.TemplateID, null));
//                }

//                GSPacketIn pkg = new GSPacketIn(391);
//                pkg.WriteBoolean(isSuccess);
//                client.SendTCP(pkg);
//            }
//            else
//            {
//                client.Player.SendMessage(LanguageMgr.GetTranslation("GameServer.EquipGhost.Msg3"));
//                return 0;
//            }

//            return 1;
//        }
//    }
//}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Bussiness;
using Game.Base.Packets;
using Game.Server.Managers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client
{
    [PacketHandler((int)ePackageType.EQUIP_GHOST, "user ac action")]
    public class EquipGhostHandler : IPacketHandler
    {
        public static Random random = new Random();

        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            ItemInfo item = client.Player.StoreBag.GetItemAt(1);
            ItemInfo luckItem = client.Player.StoreBag.GetItemAt(0);
            ItemInfo stone = client.Player.StoreBag.GetItemAt(2);

            var ClothLevel = 0;
            var HeadLevel = 0;
            var WeaponLevel = 0;
            bool isUp = false;

            if (item == null || stone == null || stone.Template.Property1 != 118)
            {
                client.Player.SendMessage(LanguageMgr.GetTranslation("GameServer.EquipGhost.Msg1"));
                return 0;
            }

            if (luckItem != null && luckItem.Template.Property1 != 117)
            {
                client.Player.SendMessage(LanguageMgr.GetTranslation("GameServer.EquipGhost.Msg4"));
                return 0;
            }

            // get spirit item
            List<SpiritInfo> spiList = SpiritInfoMgr.GetSpirit(item.Template.CategoryID);

            if (spiList.Count <= 0)
            {
                client.Player.SendMessage(LanguageMgr.GetTranslation("GameServer.EquipGhost.Msg2"));
                return 0;
            }

            // get equip ghost
            UserEquipGhostInfo equip = client.Player.GetGhostEquip(spiList[0].BagType, spiList[0].BagPlace);

            if (equip == null)
            {
                equip = new UserEquipGhostInfo();
                equip.UserID = client.Player.PlayerId;
                equip.BagType = spiList[0].BagType;
                equip.Place = spiList[0].BagPlace;
                equip.Level = 0;
                equip.TotalGhost = 0;
                client.Player.AddEquipGhost(equip);
            }

            // try math fucking text
            SpiritInfo nextLevelInfo = spiList.SingleOrDefault(a => a.Level == equip.Level + 1);

            if (nextLevelInfo != null)
            {
                double luckratio = (luckItem != null) ? (1 + (float)luckItem.Template.Property2 / 100f) : 1f;
                double rawRatio = 5f * Math.Pow(2f, Math.Pow(2f, (stone.Template.Level - 1f)) + 2f - (float)nextLevelInfo.Level) * luckratio;

                #region GetLevel
                if (client.Player.PlayerCharacter.GhostEquipList != null)
                {
                    JObject GhostEquipListJson;
                    try
                    {
                        GhostEquipListJson = JObject.Parse(client.Player.PlayerCharacter.GhostEquipList);
                    }
                    catch (Newtonsoft.Json.JsonReaderException exc)
                    {
                        Console.WriteLine(exc.Message);
                        return 0;
                    }
                    foreach (var token in GhostEquipListJson)
                    {
                        JObject child = (JObject)token.Value;

                        JToken childPlace = child.GetValue("Place");
                        int place = childPlace.Value<int>();
                        if (place == 0)//nón
                        {
                            JToken childt = child.GetValue("Level");
                            HeadLevel = childt.Value<int>();
                            break;
                        }
                        if (place == 4)//áo
                        {
                            JToken childt = child.GetValue("Level");
                            ClothLevel = childt.Value<int>();
                        }
                        if (place == 6)//vũ khí
                        {
                            JToken childt = child.GetValue("Level");
                            WeaponLevel = childt.Value<int>();
                        }
                    }
                }
                #endregion
                var totalLevel = HeadLevel + ClothLevel + WeaponLevel;
                #region Running
                switch (item.Template.CategoryID)
                {
                    case 7://vukhi
                        {
                            if (WeaponLevel <= HeadLevel && WeaponLevel <= ClothLevel || WeaponLevel == 0)
                            {
                                isUp = true;
                            }
                        }
                        break;
                    case 5:
                        {
                            if (ClothLevel <= HeadLevel && ClothLevel <= WeaponLevel || ClothLevel == 0)
                            {
                                isUp = true;
                            }
                        }
                        break;
                    case 1:
                        {
                            if (HeadLevel <= ClothLevel && HeadLevel <= WeaponLevel || HeadLevel == 0)
                            {
                                isUp = true;
                            }
                        }
                        break;
                }

                /*if (totalLevel * 3 == 0)
                {
                    isUp = true;
                }*/
                #endregion

                if (isUp)
                {
                    client.Player.StoreBag.RemoveCountFromStack(stone, 1);
                    client.Player.StoreBag.RemoveCountFromStack(luckItem, 1);

                    bool isSuccess = false;
                    var rnd = random.Next(0, 100000);
                    if ((rawRatio * 1000) * 2 >= rnd)
                    {
                        //success
                        isSuccess = true;
                        equip.Level++;

                        client.Player.EquipBag.UpdatePlayerProperties();
                        client.Player.SaveEquipGhost();
                        client.Out.SendUserSyncEquipGhost(client.Player);
                        GameServer.Instance.LoginServer.SendPacket(WorldMgr.SendSysNotice(eMessageType.ChatNormal, LanguageMgr.GetTranslation("ItemEquipGhost.Success", client.Player.ZoneName, client.Player.PlayerCharacter.NickName, item.TemplateID, equip.Level), item.ItemID, item.TemplateID, null));

                    }
                    GSPacketIn pkg = new GSPacketIn((int)ePackageType.EQUIP_GHOST);
                    pkg.WriteBoolean(isSuccess);
                    client.SendTCP(pkg);
                }
                else
                {
                    client.Player.SendMessage("Thao tác thất bại vui lòng nâng cấp Áo - Nón - Vũ Khí bằng nhau.");
                }
            }
            else
            {
                client.Player.SendMessage(LanguageMgr.GetTranslation("GameServer.EquipGhost.Msg3"));
                return 0;
            }

            return 1;
        }
    }
}