using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Logic;
using Game.Server.GameObjects;
using Game.Server.Packets;
using SqlDataProvider.Data;

namespace Game.Server.Pet.Handle
{
    [global::Pet(8)]
    public class ReleasePet : IPetCommandHadler
    {
        public bool CommandHandler(GamePlayer player, GSPacketIn packet)
        {
            int place = packet.ReadInt();
            UsersPetInfo pet = player.PetBag.GetPetAt(place);
            if (player.PetBag.RemovePet(pet))
            {
                player.EquipBag.UpdatePlayerProperties();
                using (PlayerBussiness pb = new PlayerBussiness())
                {
                    pb.UpdateUserAdoptPet(pet.ID);
                }

                PetTemplateInfo templateInfo = PetMgr.FindPetTemplate(pet.TemplateID);
                if (templateInfo.WashGetCount > 0)
                {
                    ItemInfo item = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(12656), templateInfo.WashGetCount, 105);
                    item.IsBinds = true;
                    player.SendItemToMail(item, "", "", eMailType.Default);
                    player.SendMessage("§£§à§ß§Ñ " + item.Count + "x" + item.Template.Name);
                }
            }
            player.SendMessage(LanguageMgr.GetTranslation("PetHandler.Msg19"));
            player.PetBag.SaveToDatabase(false);
            return false;
        }
    }
}
