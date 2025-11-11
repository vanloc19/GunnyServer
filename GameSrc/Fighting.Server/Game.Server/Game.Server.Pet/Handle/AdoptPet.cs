using Bussiness;
using Game.Base.Packets;
using Game.Logic;
using Game.Server.GameObjects;
using Game.Server.GameUtils;
using Game.Server.Managers;
using Game.Server.Packets;
using Newtonsoft.Json;
using SqlDataProvider.Data;
using System.Collections.Generic;

namespace Game.Server.Pet.Handle
{
    [global::Pet(6)]
    public class AdoptPet : IPetCommandHadler
    {
        public bool CommandHandler(GamePlayer player, GSPacketIn packet)
        {
            int slot = packet.ReadInt();
            int firstEmptySlot = player.PetBag.FindFirstEmptySlot();
            PetInventory petBag = player.PetBag;
            if (firstEmptySlot == -1)
            {
                player.Out.SendRefreshPet(player, petBag.GetAdoptPet(player.PlayerCharacter.VIPLevel), (ItemInfo[])null, false);
                player.SendMessage(LanguageMgr.GetTranslation("Số lượng pet đã đạt giới hạn!"));
            }
            else
            {
                if (slot < 0)
                {
                    player.Out.SendRefreshPet(player, petBag.GetAdoptPet(player.PlayerCharacter.VIPLevel), (ItemInfo[])null, false);
                    player.SendMessage(LanguageMgr.GetTranslation("Không tìm thấy pet này!"));
                    return false;
                }
                UsersPetInfo adoptPetAt = petBag.GetAdoptPetAt(slot);
                adoptPetAt.VIPLevel = player.PlayerCharacter.VIPLevel;
                adoptPetAt.PetEquips = new List<PetEquipInfo>();
                using (PlayerBussiness playerBussiness = new PlayerBussiness())
                {
                    if (adoptPetAt.ID > 0)
                    {
                        playerBussiness.RemoveUserAdoptPet(adoptPetAt.ID);
                        adoptPetAt.ID = 0;
                    }
                }
                adoptPetAt.PetEquips = new List<PetEquipInfo>();
                adoptPetAt.BaseProp = JsonConvert.SerializeObject((object)adoptPetAt);
                if (petBag.AddPetTo(adoptPetAt, firstEmptySlot))
                {
                    PetTemplateInfo petTemplate = PetMgr.FindPetTemplate(adoptPetAt.TemplateID);
                    if (petTemplate.StarLevel > 3)
                        GameServer.Instance.LoginServer.SendPacket(WorldMgr.SendSysNotice(string.Format("Người chơi [{0}] may mắn bắt được {1} {2} sao.", (object)player.PlayerCharacter.NickName, (object)petTemplate.Name, (object)petTemplate.StarLevel)));
                    else
                        player.SendMessage("Bắt thành công.");
                    player.OnAdoptPetEvent();
                }
                petBag.SaveToDatabase(false);
                petBag.RemoveAdoptPet(adoptPetAt);
            }
            return false;
        }
    }
}