//using System;
//using Bussiness;
//using Bussiness.Managers;
//using Game.Base.Packets;
//using Game.Logic;
//using Game.Server.GameObjects;
//using Game.Server.Packets;
//using SqlDataProvider.Data;

//namespace Game.Server.Pet.Handle
//{
//    [global::Pet(33)]
//    public class EatPet : IPetCommandHadler
//    {
//        public bool CommandHandler(GamePlayer player, GSPacketIn packet)
//        {
//            int amor = packet.ReadInt();
//            int type = packet.ReadInt();
//            int count = 0;
//            int totalPoint = 0;
//            int eatItem = 201567;
//            if (type == 1)
//            {
//                count = packet.ReadInt();
//                for (int i = 0; i < count; i++)
//                {
//                    int petPlace = packet.ReadInt();
//                    int templateID = packet.ReadInt();
//                    UsersPetInfo pet = player.PetBag.GetPetAt(petPlace);
//                    if (pet != null)
//                    {
//                        PetTemplateInfo info = PetMgr.FindPetTemplate(templateID);
//                        if (info != null)
//                        {
//                            totalPoint += (int)(Math.Pow(10, info.StarLevel - 2) + 5 * Math.Max(pet.Level - 8, pet.Level * 0.2)); ;
//                        }
//                    }

//                    UpGrade(player, amor, type, totalPoint, null);
//                    player.PetBag.RemovePet(pet);
//                }
//            }
//            else
//            {
//                count = packet.ReadInt();
//                int itemCount = player.PropBag.GetItemCount(201567);
//                if (count <= itemCount && itemCount > 0)
//                {
//                    ItemInfo info = player.PropBag.GetItemByTemplateID(0, eatItem);
//                    if (player.PropBag.RemoveTemplate(eatItem, count))
//                    {
//                        if (info != null)
//                        {
//                            totalPoint = info.Template.Property1 * count;
//                        }
//                        else
//                        {
//                            totalPoint = count * 10;
//                        }
//                    }
//                    UpGrade(player, amor, type, totalPoint, info);
//                    #region OLD
//                    /*(if (info != null)
//                    {
//                        int allCount = player.GetItemCount(eatItem);
//                        if (allCount < count)
//                        {
//                            count = allCount < count ? allCount : count;
//                        }
//                        totalPoint += info.Template.Property1 * count;
//                        int useCount = UpGrade(player, amor, type, totalPoint, info);
//                        if (count == 1)
//                        {
//                            player.RemoveTemplate(eatItem, count);
//                        }
//                        else
//                        {
//                            player.RemoveTemplate(eatItem, useCount);
//                        }
//                    }
//                }
//                else
//                {
//                    player.SendMessage(LanguageMgr.GetTranslation("PetHandler.EatPetNotEnoughtCount"));
//                }*/
//                    #endregion
//                }
//            }

//            player.Out.SendEatPetsInfo(player.PetBag.EatPets);
//            player.EquipBag.UpdatePlayerProperties();
//            return false;
//        }

//        private int UpGrade(GamePlayer player, int amor, int type, int totalPoint, ItemInfo eatItem)
//        {
//            int totalExp = 0;
//            int oldLv = 0;
//            int maxLv = PetMoePropertyMgr.FindMaxLevel();
//            PetMoePropertyInfo item = null;
//            switch (amor)
//            {
//                case 0: //att, luc
//                    totalExp = player.PetBag.EatPets.weaponExp + totalPoint;
//                    oldLv = player.PetBag.EatPets.weaponLevel;
//                    for (int i = oldLv; i <= maxLv; i++)
//                    {
//                        item = PetMoePropertyMgr.FindPetMoeProperty(i + 1);
//                        if (item != null && item.Exp <= totalExp)
//                        {
//                            player.PetBag.EatPets.weaponLevel = i + 1;
//                            totalExp -= item.Exp;
//                        }
//                    }

//                    if (player.PetBag.EatPets.weaponLevel == maxLv)
//                    {
//                        totalPoint = totalExp > 0 ? totalExp : totalPoint;
//                        player.PetBag.EatPets.weaponExp = 0;
//                    }
//                    else
//                    {
//                        player.PetBag.EatPets.weaponExp = totalExp;
//                    }

//                    break;
//                case 1: //agi, hp
//                    totalExp = player.PetBag.EatPets.clothesExp + totalPoint;
//                    oldLv = player.PetBag.EatPets.clothesLevel;
//                    for (int i = oldLv; i <= maxLv; i++)
//                    {
//                        item = PetMoePropertyMgr.FindPetMoeProperty(i + 1);
//                        if (item != null && item.Exp <= totalExp)
//                        {
//                            player.PetBag.EatPets.clothesLevel = i + 1;
//                            totalExp -= item.Exp;
//                        }
//                    }

//                    if (player.PetBag.EatPets.clothesLevel == maxLv)
//                    {
//                        totalPoint = totalExp > 0 ? totalExp : totalPoint;
//                        player.PetBag.EatPets.clothesExp = 0;
//                    }
//                    else
//                    {
//                        player.PetBag.EatPets.clothesExp = totalExp;
//                    }

//                    break;
//                case 2: //def, guard
//                    totalExp = player.PetBag.EatPets.hatExp + totalPoint;
//                    oldLv = player.PetBag.EatPets.hatLevel;
//                    for (int i = oldLv; i <= maxLv; i++)
//                    {
//                        item = PetMoePropertyMgr.FindPetMoeProperty(i + 1);
//                        if (item != null && item.Exp <= totalExp)
//                        {
//                            player.PetBag.EatPets.hatLevel = i + 1;
//                            totalExp -= item.Exp;
//                        }
//                    }

//                    if (player.PetBag.EatPets.hatLevel == maxLv)
//                    {
//                        totalPoint = totalExp > 0 ? totalExp : totalPoint;
//                        player.PetBag.EatPets.hatExp = 0;
//                    }
//                    else
//                    {
//                        player.PetBag.EatPets.hatExp = totalExp;
//                    }

//                    break;
//            }

//            if (type == 2 && eatItem != null)
//                return totalPoint / eatItem.Template.Property2;

//            return 0;
//        }
//    }
//}

using System;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Logic;
using Game.Server.GameObjects;
using Game.Server.Managers;
using Game.Server.Packets;
using SqlDataProvider.Data;

namespace Game.Server.Pet.Handle
{
    [global::Pet(33)]
    public class EatPet : IPetCommandHadler
    {
        public bool CommandHandler(GamePlayer player, GSPacketIn packet)
        {
            int amor = packet.ReadInt();
            int type = packet.ReadInt();
            int count = 0;
            int totalPoint = 0;
            int eatItem = 201567;

            int ClothLv = player.PetBag.EatPets.clothesLevel;
            int HeadLv = player.PetBag.EatPets.hatLevel;
            int WeaponLv = player.PetBag.EatPets.weaponLevel;
            int TotalLv = ClothLv + HeadLv + WeaponLv;
            bool canUpgrade = false;
            #region Check Can Bang
            if (TotalLv % 3 == 0)
            {
                canUpgrade = true;
            }
            #endregion
            #region Check Type And Upgrade
            switch (amor)
            {
                case 0://vu khi
                    {
                        if (WeaponLv <= HeadLv && WeaponLv <= ClothLv || WeaponLv == 0)
                        {
                            canUpgrade = true;
                        }
                    }
                    break;
                case 1://ao
                    {
                        if (ClothLv <= HeadLv && ClothLv <= WeaponLv || ClothLv == 0)
                        {
                            canUpgrade = true;
                        }
                    }
                    break;
                case 2://non
                    {
                        if (HeadLv <= ClothLv && HeadLv <= WeaponLv || HeadLv == 0)
                        {
                            canUpgrade = true;
                        }
                    }
                    break;
            }
            #endregion
            if (canUpgrade)
            {
                if (type == 1)
                {
                    count = packet.ReadInt();
                    int index = player.PetBag.FindFirstEmptySlot();
                    if (index == -1)
                    {
                        player.SendMessage($"Không Thể Manh Hóa");
                        return false;
                    }
                    for (int i = 0; i < count; i++)
                    {
                        int petPlace = packet.ReadInt();
                        int templateID = packet.ReadInt();
                        UsersPetInfo pet = player.PetBag.GetPetAt(petPlace);
                        if (pet != null)
                        {
                            PetTemplateInfo info = PetMgr.FindPetTemplate(templateID);
                            if (info.StarLevel >= 4)
                            {
                                if (info != null)
                                {
                                    totalPoint += 1;//(int)(Math.Pow(10, info.StarLevel - 2) + 5 * Math.Max(pet.Level - 8, pet.Level * 0.2));
                                }
                            }
                            else
                            {
                                player.SendMessage(string.Format("Không thể manh hoá pet bằng thú cưng nhỏ hơn 4 sao!!!"));
                                return false;
                            }

                        }

                        UpGrade(player, amor, type, totalPoint, null);
                        player.PetBag.RemovePet(pet);
                    }
                }
                else
                {
                    count = packet.ReadInt();
                    ItemInfo info = player.GetItemByTemplateID(eatItem);
                    if (info != null)
                    {
                        int allCount = player.GetItemCount(eatItem);
                        if (allCount < count)
                        {
                            count = allCount < count ? allCount : count;
                        }

                        totalPoint += info.Template.Property2 * count;
                        int useCount = UpGrade(player, amor, type, totalPoint, info);
                        if (count == 1)
                        {
                            player.RemoveTemplate(eatItem, count);
                        }
                        else
                        {
                            player.RemoveTemplate(eatItem, useCount);
                        }
                    }
                    else
                    {
                        player.SendMessage(string.Format("Không đủ số lượng đá manh hóa."));
                    }
                }
            }
            else
            {
                player.SendMessage("Thao tác thất bại vui lòng nâng cấp Áo - Nón - Vũ Khí bằng nhau.");
                return false;
            }
            player.Out.SendEatPetsInfo(player.PetBag.EatPets);
            player.EquipBag.UpdatePlayerProperties();
            return false;
        }

        private int UpGrade(GamePlayer player, int amor, int type, int totalPoint, ItemInfo eatItem)
        {
            int totalExp = 0;
            int oldLv = 0;
            int maxLv = PetMoePropertyMgr.FindMaxLevel();
            GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
            PetMoePropertyInfo item = null;
            switch (amor)
            {
                case 0: //att, luc
                    totalExp = player.PetBag.EatPets.weaponExp + totalPoint;
                    oldLv = player.PetBag.EatPets.weaponLevel;
                    for (int i = oldLv; i <= maxLv; i++)
                    {
                        item = PetMoePropertyMgr.FindPetMoeProperty(i + 1);
                        if (item != null && item.Exp <= totalExp)
                        {
                            player.PetBag.EatPets.weaponLevel = i + 1;
                            totalExp -= item.Exp;
                            for (int z = 0; z < allPlayers.Length; z++)
                            {
                                allPlayers[z].Out.SendMessage(eMessageType.ChatNormal, string.Format("|Manh Hóa| - Chúc mừng [{0}] vừa manh hóa thành công Vũ Khí lên cấp {1} lực chiến tăng lên tầm cao mới", player.PlayerCharacter.NickName, player.PetBag.EatPets.weaponLevel));
                            }
                        }
                    }

                    if (player.PetBag.EatPets.weaponLevel == maxLv)
                    {
                        totalPoint = totalExp > 0 ? totalExp : totalPoint;
                        player.PetBag.EatPets.weaponExp = 0;
                    }
                    else
                    {
                        player.PetBag.EatPets.weaponExp = totalExp;
                    }

                    break;
                case 1: //agi, hp
                    totalExp = player.PetBag.EatPets.clothesExp + totalPoint;
                    oldLv = player.PetBag.EatPets.clothesLevel;
                    for (int i = oldLv; i <= maxLv; i++)
                    {
                        item = PetMoePropertyMgr.FindPetMoeProperty(i + 1);
                        if (item != null && item.Exp <= totalExp)
                        {
                            player.PetBag.EatPets.clothesLevel = i + 1;
                            totalExp -= item.Exp;
                            for (int z = 0; z < allPlayers.Length; z++)
                            {
                                allPlayers[z].Out.SendMessage(eMessageType.ChatNormal, string.Format("|Manh Hóa| - Chúc mừng [{0}] vừa manh hóa thành công Giáp lên cấp {1} lực chiến tăng lên tầm cao mới", player.PlayerCharacter.NickName, player.PetBag.EatPets.clothesLevel));
                            }
                        }
                    }

                    if (player.PetBag.EatPets.clothesLevel == maxLv)
                    {
                        totalPoint = totalExp > 0 ? totalExp : totalPoint;
                        player.PetBag.EatPets.clothesExp = 0;
                    }
                    else
                    {
                        player.PetBag.EatPets.clothesExp = totalExp;
                    }

                    break;
                case 2: //def, guard
                    totalExp = player.PetBag.EatPets.hatExp + totalPoint;
                    oldLv = player.PetBag.EatPets.hatLevel;
                    for (int i = oldLv; i <= maxLv; i++)
                    {
                        item = PetMoePropertyMgr.FindPetMoeProperty(i + 1);
                        if (item != null && item.Exp <= totalExp)
                        {
                            player.PetBag.EatPets.hatLevel = i + 1;
                            totalExp -= item.Exp;
                            for (int z = 0; z < allPlayers.Length; z++)
                            {
                                allPlayers[z].Out.SendMessage(eMessageType.ChatNormal, string.Format("|Manh Hóa| - Chúc mừng [{0}] vừa manh hóa thành công Nón lên cấp {1} lực chiến tăng lên tầm cao mới", player.PlayerCharacter.NickName, player.PetBag.EatPets.hatLevel));
                            }
                        }
                    }

                    if (player.PetBag.EatPets.hatLevel == maxLv)
                    {
                        totalPoint = totalExp > 0 ? totalExp : totalPoint;
                        player.PetBag.EatPets.hatExp = 0;
                    }
                    else
                    {
                        player.PetBag.EatPets.hatExp = totalExp;
                    }

                    break;
            }

            if (type == 2 && eatItem != null)
                return totalPoint / eatItem.Template.Property2;

            return 0;
        }
    }
}