using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.GameUtils;
using SqlDataProvider.Data;
using System;

namespace Game.Server.Pet.Handle
{
	[global::Pet(20)]
	public class AddPetEquip : IPetCommandHadler
	{
		public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
		{
			int bagType = packet.ReadInt();
			int place = packet.ReadInt();
			int petPlace = packet.ReadInt();
			PlayerInventory bag = Player.GetInventory((eBageType)bagType);
			ItemInfo item = bag.GetItemAt(place);
			if (item != null && item.IsEquipPet() && item.IsValidItem())
            {
				//if (Player.PetBag.CheckEqPetLevel(petPlace, item))
    //            {
				//	Player.SendMessage(LanguageMgr.GetTranslation("Cấp PET của bạn không đủ để trang bị vật phẩm này!"));
				//	return false;
				//}
				if (item.IsUsed == false)
                {
					item.IsUsed = true;
					item.BeginDate = DateTime.Now;
                }
				if (Player.PetBag.AddEqPet(petPlace, item))
                {
					bag.TakeOutItem(item);
					Player.PetBag.OnChangedPetEquip(petPlace);
					Player.SendMessage(LanguageMgr.GetTranslation("Trang bị vật phẩm PET thành công!"));
				}
				else
                {
					Player.SendMessage(LanguageMgr.GetTranslation("Đã có lỗi sảy ra vui lòng liên hệ BQT!"));
				}
            }
			else
			{
				Player.SendMessage(LanguageMgr.GetTranslation("AddPetEquip.WrongItem"));
			}
			return false;
		}
	}
}
