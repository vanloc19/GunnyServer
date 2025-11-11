using System;
using System.Collections.Generic;
using System.Linq;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Logic;
using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.Pet.Handle
{
    [global::Pet(5)]
    public class RefereshPet : IPetCommandHadler
    {
        public bool CommandHandler(GamePlayer player, GSPacketIn packet)
        {
            bool refreshBtn = packet.ReadBoolean();
            int refereshCost = Convert.ToInt32(PetMgr.FindConfig("AdoptRefereshCost").Value);
            int refereshId = Convert.ToInt32(PetMgr.FindConfig("FreeRefereshID").Value);
            ItemInfo freeRefereshItem = player.PropBag.GetItemByTemplateID(0, refereshId);

            List<UsersPetInfo> lists = player.PetBag.GetAdoptPet(1).ToList();
            if (refreshBtn)
            {
                bool canAddScore = true;
                if (freeRefereshItem != null)
                {
                    player.PropBag.RemoveTemplate(refereshId, 1);
                    canAddScore = false;
                }

                else if (!player.MoneyDirect(refereshCost, IsAntiMult:false))
                {
                    return false;
                }

                if (canAddScore)
                {
                    player.AddPetScore((int)(refereshCost * 0.1));
                }

                lists = PetMgr.CreateAdoptList(player.PlayerCharacter.ID, player.PetBag.MaxLevelByGrade, player.PlayerCharacter.VIPLevel);
                player.PetBag.ClearAdoptPets();
                foreach (UsersPetInfo current in lists)
                {
                    player.PetBag.AddAdoptPetTo(current, current.Place);
                }
            }

            player.Out.SendRefreshPet(player, lists.ToArray(), null, refreshBtn);
            return false;
        }
    }
}