using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Logic;
using Game.Server.GameObjects;
using Newtonsoft.Json;
using SqlDataProvider.Data;

namespace Game.Server.Pet.Handle
{
	[global::Pet(22)]
	public class PetRisingStar : IPetCommandHadler
	{
		public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
		{
			int templateID = packet.ReadInt();
			int count = packet.ReadInt();
			int slot = packet.ReadInt();
			bool val = false;
			UsersPetInfo petAt = Player.PetBag.GetPetAt(slot);
			if (petAt == null)
			{
				Player.SendMessage(LanguageMgr.GetTranslation("PetRisingStar.PetNotFound"));
			}
			else
			{
				int num1 = 11162;
				ItemInfo itemByTemplateId = Player.GetItemByTemplateID(templateID);
				bool flag = false;
				if (itemByTemplateId == null)
				{
					Player.SendMessage(LanguageMgr.GetTranslation("PetRisingStar.ItemNotFound"));
				}
				else
				{
					PetStarExpInfo petStarExp = PetMgr.FindPetStarExp(petAt.TemplateID);
					if (petStarExp == null)
					{
						Player.SendMessage(LanguageMgr.GetTranslation("PetRisingStar.UnSupport"));
					}
					else if (itemByTemplateId.TemplateID == num1)
					{
						if (itemByTemplateId.Count < count)
						{
							count = itemByTemplateId.Count;
						}
						int num2 = itemByTemplateId.Template.Property2 * count;
						int num3 = petAt.currentStarExp + num2;
						if (num3 >= petStarExp.Exp)
						{
							int num4 = petStarExp.Exp - petAt.currentStarExp;
							if (num4 < num2 && !flag)
							{
								count = (num2 - num4) / itemByTemplateId.Template.Property2;
							}
							petAt.currentStarExp = 0;
							PetTemplateInfo petTemplate = PetMgr.FindPetTemplate(petStarExp.NewID);
							if (petTemplate != null)
							{
								UsersPetInfo pet = PetMgr.CreatePet(petTemplate, petAt.UserID, petAt.Place, petAt.Level, Player.PlayerCharacter.VIPLevel);
								petAt.BaseProp = JsonConvert.SerializeObject(pet);
								pet.Level = petAt.Level;
								pet.VIPLevel = Player.PlayerCharacter.VIPLevel;
								Player.PetBag.UpdateEvolutionPet(pet, petAt.Level, Player.PetBag.MaxLevelByGrade, Player.PlayerCharacter.VIPLevel);
								petAt.TemplateID = pet.TemplateID;

								petAt.AttackGrow = petAt.AttackGrow;//pet.AttackGrow;
								petAt.DefenceGrow = petAt.DefenceGrow;
								petAt.AgilityGrow = petAt.AgilityGrow;
								petAt.LuckGrow = petAt.LuckGrow;
								petAt.BloodGrow = petAt.BloodGrow;
								petAt.DamageGrow = petAt.DamageGrow;
								petAt.GuardGrow = petAt.GuardGrow;

								petAt.Attack = pet.Attack;
								petAt.Defence = pet.Defence;
								petAt.Agility = pet.Agility;
								petAt.Luck = pet.Luck;
								petAt.Blood = pet.Blood;
								petAt.Damage = pet.Damage;
								petAt.Guard = pet.Guard;

								val = true;
							}
						}
						else
						{
							petAt.currentStarExp = num3;
						}
						Player.PetBag.UpdatePet(petAt);
						if (!flag)
						{
							Player.RemoveCountFromStack(itemByTemplateId, count);
						}
					}
					else
					{
						Player.SendMessage(LanguageMgr.GetTranslation("PetRisingStar.ItemNotFound"));
					}
				}
			}
			GSPacketIn pkg = new GSPacketIn(68);
			pkg.WriteByte(22);
			pkg.WriteBoolean(val);
			Player.SendTCP(pkg);
			return false;
		}
	}
}
