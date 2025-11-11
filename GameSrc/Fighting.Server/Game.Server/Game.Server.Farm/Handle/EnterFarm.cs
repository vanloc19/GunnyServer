using System;
using System.Collections.Generic;
using Bussiness;
using Game.Base.Packets;
using Game.Logic;
using Game.Server.GameObjects;
using Game.Server.Packets;
using SqlDataProvider.Data;

namespace Game.Server.Farm.Handle
{
    [FarmHandleAttbute((byte)FarmPackageType.ENTER_FARM)]
    public class EnterFarm : IFarmCommandHadler
    {
        public bool CommandHandler(GamePlayer player, GSPacketIn pkg)
        {
            //player.SendMessage(LanguageMgr.GetTranslation("Tính năng bị khóa!"));
            //return false;
            if (player.PlayerCharacter.Grade < 10)
            {
                player.SendMessage(LanguageMgr.GetTranslation("Bạn chưa đạt cấp độ 10 không thể vào Nông Trại!"));
                return false;
            }

            int userID = pkg.ReadInt();
            if (userID == player.PlayerCharacter.ID)
            {
                var RipeNum = player.Farm.ripeNum();
                player.PlayerCharacter.TotalMoneyFastForWard = 5000;
                player.Farm.EnterFarm(true);
                if (player.PlayerCharacter.IsFistGetPet)
                {
                    player.PetBag.ClearAdoptPets();
                    List<UsersPetInfo> list = PetMgr.CreateFirstAdoptList(userID, player.Level, player.PlayerCharacter.VIPLevel);
                    foreach (UsersPetInfo pet in list)
                    {
                        player.PetBag.AddAdoptPetTo(pet, pet.Place);
                    }
                    player.RemoveFistGetPet();
                }
                else if (player.PlayerCharacter.LastRefreshPet.Date < DateTime.Now.Date)
                {
                    player.PetBag.ClearAdoptPets();
                    List<UsersPetInfo> list = PetMgr.CreateFirstAdoptList(userID, player.Level, player.PlayerCharacter.VIPLevel);
                    foreach (UsersPetInfo pet in list)
                    {
                        player.PetBag.AddAdoptPetTo(pet, pet.Place);
                    }
                    player.RemoveLastRefreshPet();
                }
            }
            else
            {
                player.Farm.EnterFriendFarm(userID);
            }
            return true;
        }
    }
}
