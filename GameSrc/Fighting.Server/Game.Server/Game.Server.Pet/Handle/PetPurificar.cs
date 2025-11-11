using System;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Logic;
using Game.Server.GameObjects;
using Game.Server.Packets;
using SqlDataProvider.Data;

namespace Game.Server.Pet.Handle
{
    [global::Pet(40)]
    public class PetPurificar : IPetCommandHadler
    {
        public bool CommandHandler(GamePlayer player, GSPacketIn packet)
        {
            int placePet = packet.ReadInt();
            bool blood = packet.ReadBoolean();
            bool attack = packet.ReadBoolean();
            bool defend = packet.ReadBoolean();
            bool agility = packet.ReadBoolean();
            bool lucky = packet.ReadBoolean();

            UsersPetInfo pet = player.PetBag.GetPetAt(placePet);
            if (pet == null)
            {
                player.SendMessage(LanguageMgr.GetTranslation("PetPurificar.Translate1"));
                return false;
            }
            int qtdNecessaria = 40;
            if (attack)
            {
                qtdNecessaria += 40;
            }

            if (defend)
            {
                qtdNecessaria += 40;
            }

            if (agility)
            {
                qtdNecessaria += 40;
            }

            if (lucky)
            {
                qtdNecessaria += 40;
            }

            if (blood)
            {
                qtdNecessaria += 40;
            }
            bool flag = attack && defend && agility && lucky && blood;
            if (flag)
            {
                player.SendMessage(LanguageMgr.GetTranslation("PetPurificar.Translate2"));
                return false;
            }
            int count = player.PropBag.GetItemCount(12656);
            if (count < qtdNecessaria)
            {
                player.SendMessage("không đủ " + ItemMgr.FindItemTemplate(12656).Name);
                return false;
            }
            PetMgr.purificar(pet, blood, attack, defend, agility, lucky);
            pet.BuildProp();
            player.PropBag.RemoveTemplate(12656, qtdNecessaria);
            player.PropBag.UpdateChangedPlaces();
            player.PetBag.UpdatePet(pet);
            player.SendMessage(LanguageMgr.GetTranslation("PetPurificar.Translate3"));
            player.EquipBag.UpdatePlayerProperties();
            player.PetBag.SaveToDatabase(false);
            return true;
        }
    }
}