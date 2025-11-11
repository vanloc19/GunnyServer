using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using Game.Server.Packets;

namespace Game.Server.Consortia.Handle
{
	[global::Consortia(6)]
	public class ConsortiaRichesOffer : IConsortiaCommandHadler
	{
		public int CommandHandler(GamePlayer Player, GSPacketIn packet)
		{
			if (Player.PlayerCharacter.ConsortiaID == 0)
			{
				return 0;
			}
			int val1 = packet.ReadInt();
			if (Player.PlayerCharacter.HasBagPassword && Player.PlayerCharacter.IsLocked)
			{
				Player.Out.SendMessage(eMessageType.GM_NOTICE, LanguageMgr.GetTranslation("Bag.Locked"));
				return 1;
			}
			if (val1 >= 1 && Player.PlayerCharacter.Money + Player.PlayerCharacter.MoneyLock >= val1)
			{
				bool val2 = false;
				string translateId = "ConsortiaRichesOfferHandler.Failed";
				using (ConsortiaBussiness consortiaBussiness = new ConsortiaBussiness())
				{
					int riches = val1 / 2;
					if (consortiaBussiness.ConsortiaRichAdd(Player.PlayerCharacter.ConsortiaID, ref riches, 5, Player.PlayerCharacter.NickName))
					{
						val2 = true;
						Player.AddRichesOffer(riches);
						Player.RemoveMoney(val1, isConsume: true);
						translateId = "ConsortiaRichesOfferHandler.Successed";
						Player.SaveIntoDatabase();
						ConsortiaMgr.ReLoad();
						GameServer.Instance.LoginServer.SendConsortiaRichesOffer(Player.PlayerCharacter.ConsortiaID, Player.PlayerCharacter.ID, Player.PlayerCharacter.NickName, riches);
					}
				}
				GSPacketIn packet2 = new GSPacketIn(129);
				packet2.WriteByte(6);
				packet2.WriteInt(val1);
				packet2.WriteBoolean(val2);
				packet2.WriteString(LanguageMgr.GetTranslation(translateId));
				Player.Out.SendTCP(packet2);
				return 0;
			}
			Player.Out.SendMessage(eMessageType.GM_NOTICE, LanguageMgr.GetTranslation("ConsortiaRichesOfferHandler.NoMoney"));
			return 1;
		}
	}
}
