using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.GameUtils;
using SqlDataProvider.Data;

namespace Game.Server.Pet.Handle
{
	[global::Pet(7)]
	public class EquipSkillPet : IPetCommandHadler
	{
		public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
		{
			int place = packet.ReadInt();
			int killId = packet.ReadInt();
			int killindex = packet.ReadInt();
			PetInventory petBag = Player.PetBag;
			UsersPetInfo petAt = petBag.GetPetAt(place);
			bool result;
			switch(killindex)
            {
				case 0:
                    {
						string msg = "";
						Player.PetBag.EquipSkillPet(place, killId, killindex, ref msg);
						if (msg != "")
                        {
							Player.SendMessage(msg);
                        }
						result = false;
						break;
                    }
				case 1:
                    {
						if(petAt.Level < 20)
                        {
							Player.SendMessage("Chưa đủ điều kiện để Equip Skill này.");
							result = false;
                        }
						else
                        {
							string msg2 = "";
							Player.PetBag.EquipSkillPet(place, killId, killindex, ref msg2);
							if (msg2 != "")
							{
								Player.SendMessage(msg2);
							}
							result = false;
						}
						break;
					}
				case 2:
					{
						if (petAt.Level < 30)
						{
							Player.SendMessage("Chưa đủ điều kiện để Equip Skill này.");
							result = false;
						}
						else
						{
							string msg3 = "";
							Player.PetBag.EquipSkillPet(place, killId, killindex, ref msg3);
							if (msg3 != "")
							{
								Player.SendMessage(msg3);
							}
							result = false;
						}
						break;
					}
				case 3:
					{
						if (petAt.Level < 50)
						{
							Player.SendMessage("Chưa đủ điều kiện để Equip Skill này.");
							result = false;
						}
						else
						{
							string msg4 = "";
							Player.PetBag.EquipSkillPet(place, killId, killindex, ref msg4);
							if (msg4 != "")
							{
								Player.SendMessage(msg4);
							}
							result = false;
						}
						break;
					}
				case 4:
					{
						if (Player.PlayerCharacter.VIPLevel < 4)
						{
							Player.SendMessage("Chưa đủ điều kiện để Equip Skill này.");
							result = false;
						}
						else
						{
							string msg5 = "";
							Player.PetBag.EquipSkillPet(place, killId, killindex, ref msg5);
							if (msg5 != "")
							{
								Player.SendMessage(msg5);
							}
							result = false;
						}
						break;
					}
				default:
					result = false;
					break;
            }
			return result;
		}
	}
}
