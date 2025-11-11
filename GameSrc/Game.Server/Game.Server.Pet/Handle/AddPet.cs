using System.Collections.Generic;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Logic;
using Game.Server.GameObjects;
using Game.Server.GameUtils;
using Game.Server.Managers;
using Game.Server.Packets;
using Newtonsoft.Json;
using SqlDataProvider.Data;

namespace Game.Server.Pet.Handle
{
    [global::Pet(2)]
    public class AddPet : IPetCommandHadler
    {
        public bool CommandHandler(GamePlayer player, GSPacketIn packet)
        {
            int itemPlace = packet.ReadInt();
            int bagType = packet.ReadInt();
            int userId = player.PlayerCharacter.ID;

            PetInventory petBag = player.PetBag;
            int index = petBag.FindFirstEmptySlot();

            if (player.PlayerCharacter.Grade < 25)
            {
                player.SendMessage(LanguageMgr.GetTranslation("PetHandler.Msg2"));
                return false;
            }

            if (index == -1)
            {
                player.SendMessage(LanguageMgr.GetTranslation("PetHandler.Msg3"));
            }
            else
            {
                ItemInfo egg = player.GetItemAt((eBageType)bagType, itemPlace);
                PetTemplateInfo tempInfo = PetMgr.FindPetTemplate(egg.Template.Property5);
                if (tempInfo == null)
                {
                    player.SendMessage(LanguageMgr.GetTranslation("PetHandler.Msg4"));
                    return false;
                }

                UsersPetInfo info = PetMgr.CreatePet(tempInfo, userId, index, petBag.MaxLevelByGrade, player.PlayerCharacter.VIPLevel);
                info.IsExit = true;
                info.PetEquips = new List<PetEquipInfo>();
                info.BaseProp = JsonConvert.SerializeObject(info);
                info.VIPLevel = player.PlayerCharacter.VIPLevel;
                petBag.AddPetTo(info, index);
                player.RemoveCountFromStack(egg, 1);

                if (tempInfo.StarLevel > 4)
                {
                    string tip = LanguageMgr.GetTranslation("PetHandler.Msg5", player.PlayerCharacter.NickName, tempInfo.Name, tempInfo.StarLevel);
                    GSPacketIn sysNotice = WorldMgr.SendSysNotice(tip);
                    GameServer.Instance.LoginServer.SendPacket(sysNotice);
                }
                else
                {
                    player.SendMessage(LanguageMgr.GetTranslation("PetHandler.Msg6", tempInfo.Name, tempInfo.StarLevel));
                }

                petBag.SaveToDatabase(false);
                GSPacketIn pkg = new GSPacketIn(68);
                pkg.WriteByte(2);
                pkg.WriteInt(tempInfo.TemplateID);
                pkg.WriteBoolean(true);
                player.SendTCP(pkg);
            }

            return false;
        }
    }
}