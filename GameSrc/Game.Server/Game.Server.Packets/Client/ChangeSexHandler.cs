using Bussiness;
using Game.Base.Packets;
using Game.Server.GameUtils;
using Game.Server.Packets.Client;
using SqlDataProvider.Data;
using System;

namespace Game.Server.Packet.Client
{
    [PacketHandler(252, "场景用户离开")]
    public class ChangeSexHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn pkg)
        {
            int bagType = pkg.ReadByte();
            int place = pkg.ReadInt();
            PlayerInventory inventory = client.Player.GetInventory((eBageType)bagType);
            ItemInfo card = inventory.GetItemAt(place);

            if (card == null)
                return 0;

            if (card.TemplateID == 11569)
            {
                using (PlayerBussiness pb = new PlayerBussiness())
                {
                    PlayerInfo tempSpouse = pb.GetUserSingleByUserID(client.Player.PlayerCharacter.SpouseID);
                    if (tempSpouse == null || tempSpouse.Sex == client.Player.PlayerCharacter.Sex)
                    {
                        MarryApplyInfo info = new MarryApplyInfo();
                        info.UserID = client.Player.PlayerCharacter.SpouseID;
                        info.ApplyUserID = client.Player.PlayerCharacter.ID;
                        info.ApplyUserName = client.Player.PlayerCharacter.NickName;
                        info.ApplyType = 3;
                        info.LoveProclamation = "";
                        info.ApplyResult = false;
                        int id = 0;
                        if (pb.SavePlayerMarryNotice(info, 0, ref id))
                        {
                            GameServer.Instance.LoginServer.SendUpdatePlayerMarriedStates(tempSpouse.ID);
                            client.Player.LoadMarryProp();
                        }

                        client.Player.QuestInventory.ClearMarryQuest();
                    }

                    bool newSex = client.Player.PlayerCharacter.Sex ? false : true;
                    if (pb.ChangeSex(client.Player.PlayerCharacter.ID, newSex))
                    {
                        inventory.RemoveCountFromStack(card, 1);
                        client.Player.SendMessage(LanguageMgr.GetTranslation("ChangeSexHandlerHandler.Success"));
                    }
                    else
                    {
                        client.Player.SendMessage(LanguageMgr.GetTranslation("ChangeSexHandlerHandler.Fail"));
                    }
                }
            }

            else if (card.TemplateID == 12545)
            {
                TexpInfo texpInfo = client.Player.PlayerCharacter.Texp;
                if (texpInfo.resetCount >= card.Template.Property2 && texpInfo.lastReset.Date < DateTime.Now.Date)
                {
                    texpInfo.resetCount = 0;
                }

                if (texpInfo.resetCount < card.Template.Property2)
                {
                    texpInfo.texpCount = 0;
                    texpInfo.resetCount++;
                    texpInfo.lastReset = DateTime.Now.Date;
                    using (PlayerBussiness db = new PlayerBussiness())
                    {
                        db.UpdateUserTexpInfo(texpInfo);
                    }

                    inventory.RemoveCountFromStack(card, 1);
                    client.Player.EquipBag.UpdatePlayerProperties();
                    client.Player.SendMessage(LanguageMgr.GetTranslation("ChangeSexHandlerHandler.Translate1"));
                }
                else
                {
                    client.Player.SendMessage(LanguageMgr.GetTranslation("ChangeSexHandlerHandler.Translate2"));
                }
            }
            return 0;
        }
    }
}