#region OLD
//using Bussiness;
//using Game.Base.Packets;
//using Game.Server.GameObjects;
//using Game.Server.Managers;
//using SqlDataProvider.Data;

//namespace Game.Server.Pet.Handle
//{
//	[global::Pet(1)]
//	public class UpdatePet : IPetCommandHadler
//	{
//		public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
//		{
//			int num = packet.ReadInt();
//			GamePlayer playerById = WorldMgr.GetPlayerById(num);
//			UsersPetInfo[] pets;
//			EatPetsInfo eatpet;
//			if (playerById != null)
//			{
//				pets = playerById.PetBag.GetPets();
//				eatpet = playerById.PetBag.EatPets;
//			}
//			else
//			{
//				using (PlayerBussiness playerBussiness = new PlayerBussiness())
//				{
//					pets = playerBussiness.GetUserPetSingles(num);
//					eatpet = playerBussiness.GetAllEatPetsByID(num);
//					for (int index = 0; index < pets.Length; index++)
//					{
//						pets[index].PetEquips = Player.PetBag.DeserializePetEquip(pets[index].eQPets);
//					}
//				}
//			}
//			if (pets != null && eatpet != null)
//			{
//				Player.Out.SendPetInfo(num, Player.ZoneId, pets, eatpet);
//			}
//			return false;
//		}
//	}
//}
#endregion
using System.Linq;
using Bussiness;
using Bussiness.Helpers;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using Game.Server.Packets;
using SqlDataProvider.Data;

namespace Game.Server.Pet.Handle
{
    [global::Pet(1)]
    public class UpdatePet : IPetCommandHadler
    {
        public bool CommandHandler(GamePlayer player, GSPacketIn packet)
        {
            int playerID = packet.ReadInt();

            GamePlayer gp = WorldMgr.GetPlayerById(playerID);
            UsersPetInfo[] pets;
            EatPetsInfo eatpet;

            if (gp != null)
            {
                pets = gp.PetBag.GetPets();
                eatpet = gp.PetBag.EatPets;
            }
            else
            {
                using (PlayerBussiness pb = new PlayerBussiness())
                {
                    pets = pb.GetUserPetSingles(playerID, player.PlayerCharacter.VIPLevel);
                    eatpet = pb.GetAllEatPetsByID(playerID);
                    for (int i = 0; i < pets.Length; i++)
                    {
                        pets[i].PetEquips = player.PetBag.DeserializePetEquip(pets[i].eQPets);
                    }
                }
            }

            if (pets != null && eatpet != null)
            {
                if (pets.Length > 20)
                {
                    var newPets = pets.Split(20);
                    foreach (var petsSend in newPets)
                    {
                        player.Out.SendPetInfo(playerID, player.ZoneId, petsSend.ToArray(), eatpet);
                    }
                }
                else
                {
                    player.Out.SendPetInfo(playerID, player.ZoneId, pets, eatpet);
                }
            }

            return false;
        }
    }
}
