using Bussiness.Managers;
using Game.Base.Packets;
using Game.Logic;
using Game.Server.GameObjects;
using Game.Server.GMActives;
using SqlDataProvider.Data;
using System;

namespace Game.Server.Pet.Handle
{
	[global::Pet(23)]
	public class PetEvolution : IPetCommandHadler
	{
		public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
		{
			int num1 = packet.ReadInt();
			int count = packet.ReadInt();
			if (num1 != 11163)
			{
				return false;
			}
			int num2 = 0;
			ItemInfo itemByTemplateId = Player.GetItemByTemplateID(num1);
			if (itemByTemplateId != null && count > 0 && num1 == 11163)
			{
				if (itemByTemplateId.Count < count)
				{
					count = itemByTemplateId.Count;
				}
				num2 = itemByTemplateId.Template.Property2 * count;
			}
			if (num2 > 0)
			{
				bool val = false;
				int evolutionGrade = Player.PlayerCharacter.evolutionGrade;
				_ = Player.PlayerCharacter.evolutionExp;
				int num3 = Player.PlayerCharacter.evolutionExp + num2;
				int evolutionMax = PetMgr.GetEvolutionMax();
				for (int index = evolutionGrade; index <= evolutionMax; index++)
				{
					PetFightPropertyInfo fightProperty = PetMgr.FindFightProperty(index + 1);
					if (fightProperty != null && fightProperty.Exp <= num3)
					{
						Player.PlayerCharacter.evolutionGrade = index + 1;
						GmActivityMgr.OnEvolution(Player, Player.PlayerCharacter.evolutionGrade);
						val = true;
					}
				}
				Player.PlayerCharacter.evolutionExp = num3;
				Player.PropBag.RemoveTemplate(num1, count);
				Player.EquipBag.UpdatePlayerProperties();
				Player.SendUpdatePublicPlayer();
				GSPacketIn pkg = new GSPacketIn(68);
				pkg.WriteByte(23);
				pkg.WriteBoolean(val);
				Player.SendTCP(pkg);
			}
			return false;
		}
	}
}
