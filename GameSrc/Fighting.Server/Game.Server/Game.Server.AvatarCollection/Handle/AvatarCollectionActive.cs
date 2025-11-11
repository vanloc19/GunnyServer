using System;
using System.Collections.Generic;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Packets;
using SqlDataProvider.Data;

namespace Game.Server.AvatarCollection.Handle
{
    [AvatarCollectionHandleAttbute((byte)AvatarCollectionPackageType.ACTIVE)]
    public class AvatarCollectionActive : IAvatarCollectionCommandHadler
    {
        public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
        {
            //Player.Out.SendMessage(eMessageType.Normal, "Tính năng bị khóa!");
            //return false;
            int dataId = packet.ReadInt();
            int itemId = packet.ReadInt();
            int sex = packet.ReadInt();
            int type = 1;
            if (GameProperties.VERSION >= 8300)
            {
                type = packet.ReadInt();
            }

            if (Player.PlayerCharacter.HasBagPassword && Player.PlayerCharacter.IsLocked)
            {
                Player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bag.Locked"));
                return false;
            }

            ClothGroupTemplateInfo info = AvatarColectionMgr.FindClothGroupTemplateInfo(dataId, itemId, sex);
            if (info == null)
            {
                Player.SendMessage("Trang bị không phù hợp.");
                return false;
            }

            if (Player.PlayerCharacter.Gold < info.Cost)
            {
                Player.Out.SendMessage(eMessageType.Normal, "Vàng không đủ kích hoạt thất bại!");
                return false;
            }

            List<int> ids = new List<int>();
            ids.Add(itemId);
            if (string.IsNullOrEmpty(info.OtherTemplateID) == false)
            {
                foreach (string id in info.OtherTemplateID.Split('|'))
                {
                    ids.Add(int.Parse(id));
                }
            }

            if (!Player.AvatarBag.ActiveAvatarColection(dataId, ids, sex, type))
            {
                Player.SendMessage("Không có vật phẩm này trong hành trang hoặc đã kích hoạt rồi.");
            }
            else
            {
                UserAvatarColectionInfo infoData = Player.AvatarBag.GetAvatar(dataId);
                GSPacketIn pkg = new GSPacketIn((short)ePackageType.AVATAR_COLLECTION);
                Console.WriteLine(infoData.ActiveDress);
                if (infoData != null)
                {
                    if (infoData.FullActive())
                    {
                        Player.EquipBag.UpdatePlayerProperties();
                        Player.SendMessage("Đã kích hoạt thuộc tính tối đa cho bộ sưu tập này.");
                    }
                    else if (infoData.HaftActive() && !infoData.isValidate)
                    {
                        infoData.endTime = DateTime.Now;
                        infoData.endTime = infoData.endTime.AddDays(7);
                        Player.EquipBag.UpdatePlayerProperties();
                        pkg.WriteByte((byte)AvatarCollectionPackageType.DELAY_TIME);
                        pkg.WriteInt(dataId);
                        pkg.WriteInt(sex);
                        if (GameProperties.VERSION >= 8300)
                        {
                            pkg.WriteInt(type);
                        }

                        pkg.WriteDateTime(infoData.endTime);
                        Player.SendTCP(pkg);
                        Player.SendMessage("Đã mở thuộc tính.");
                    }
                }

                Player.RemoveGold(info.Cost);

                pkg = new GSPacketIn((short)ePackageType.AVATAR_COLLECTION);
                pkg.WriteByte((byte)AvatarCollectionPackageType.ACTIVE);
                pkg.WriteInt(dataId);
                pkg.WriteInt(itemId);
                pkg.WriteInt(sex);
                if (GameProperties.VERSION >= 8300)
                {
                    pkg.WriteInt(type);
                }

                Player.SendTCP(pkg);
            }

            return true;
        }
    }
}