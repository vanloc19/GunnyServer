using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.Pet.Handle
{
	[global::Pet(21)]
	public class DelPetEquip : IPetCommandHadler
	{
		public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
		{
			int num = packet.ReadInt();
			int eqPlace = packet.ReadInt();
			if (Player.PetBag.RemoveEqPet(num, eqPlace))
			{
				Player.PetBag.OnChangedPetEquip(num);
			}
			return false;
		}
	}
}
