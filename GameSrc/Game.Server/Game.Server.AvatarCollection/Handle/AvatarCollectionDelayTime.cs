using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Packets;
using SqlDataProvider.Data;

namespace Game.Server.AvatarCollection.Handle
{
    [AvatarCollectionHandleAttbute((byte)AvatarCollectionPackageType.DELAY_TIME)]
    public class AvatarCollectionDelayTime : IAvatarCollectionCommandHadler
    {
        public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
        {
            int dataId = packet.ReadInt();
            int delay = packet.ReadInt();
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

            ClothPropertyTemplateInfo info = AvatarColectionMgr.FindClothPropertyTemplate(dataId);
            if (info == null)
            {
                Player.SendMessage("Bộ sưu tập không thể gia hạn.");
                return false;
            }

            long cost = info.Cost * delay;
            int neeedHonor = cost > int.MaxValue ? int.MaxValue : (int)cost;
            if (Player.PlayerCharacter.myHonor < neeedHonor)
            {
                Player.Out.SendMessage(eMessageType.Normal, "Vinh dự không đủ!");
                return false;
            }

            if (!Player.AvatarBag.DelayAvatarColection(dataId, delay, type))
            {
                Player.SendMessage("Bộ sưu tập không tìm thấy, thao tác thất bại.");
            }
            else
            {
                Player.RemovemyHonor(neeedHonor);
                UserAvatarColectionInfo infoData = Player.AvatarBag.GetAvatar(dataId);
                if (infoData != null)
                {
                    GSPacketIn pkg = new GSPacketIn((short)ePackageType.AVATAR_COLLECTION);
                    pkg.WriteByte((byte)AvatarCollectionPackageType.DELAY_TIME);
                    pkg.WriteInt(dataId);
                    pkg.WriteInt(infoData.Sex);
                    if (GameProperties.VERSION >= 8300)
                    {
                        pkg.WriteInt(type);
                    }

                    pkg.WriteDateTime(infoData.endTime);
                    Player.SendTCP(pkg);
                    Player.SendMessage("Thao tác thành công.");
                }
            }

            return true;
        }
    }
}