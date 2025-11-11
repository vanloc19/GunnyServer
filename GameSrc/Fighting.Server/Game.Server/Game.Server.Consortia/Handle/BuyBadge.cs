using System;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using SqlDataProvider.Data;

namespace Game.Server.Consortia.Handle
{
	[global::Consortia(28)]
	public class BuyBadge : IConsortiaCommandHadler
	{
		public int CommandHandler(GamePlayer Player, GSPacketIn packet)
		{
			if (Player.PlayerCharacter.ConsortiaID == 0)
			{
				return 0;
			}
			bool result = false;
			int num = packet.ReadInt();
			ConsortiaBadgeConfigInfo consortiaBadgeConfig = ConsortiaExtraMgr.FindConsortiaBadgeConfig(num);
			if (consortiaBadgeConfig == null)
			{
				Player.SendMessage(LanguageMgr.GetTranslation("BuyBadgeHandler.Fail"));
				return 0;
			}
			string msg = "BuyBadgeHandler.Fail";
			int ValidDate = 30;
			string BadgeBuyTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
			ConsortiaInfo consortiaInfo = ConsortiaMgr.FindConsortiaInfo(Player.PlayerCharacter.ConsortiaID);
			if (consortiaInfo == null)
			{
				return 0;
			}
			if (consortiaInfo.Riches < consortiaBadgeConfig.Cost)
			{
				Player.SendMessage(LanguageMgr.GetTranslation("BuyBadgeHandler.Fail"));
				return 0;
			}
			using (ConsortiaBussiness consortiaBussiness2 = new ConsortiaBussiness())
			{
				consortiaInfo.BadgeID = num;
				consortiaInfo.ValidDate = ValidDate;
				consortiaInfo.BadgeBuyTime = BadgeBuyTime;
				if (consortiaBussiness2.BuyBadge(Player.PlayerCharacter.ConsortiaID, Player.PlayerCharacter.ID, consortiaInfo, ref msg))
				{
					msg = "BuyBadgeHandler.Success";
					result = true;
				}
			}
			if (result)
			{
				using (PlayerBussiness playerBussiness = new PlayerBussiness())
				{
					ConsortiaUserInfo[] allMemberByConsortia = playerBussiness.GetAllMemberByConsortia(Player.PlayerCharacter.ConsortiaID);
					for (int i = 0; i < allMemberByConsortia.Length; i++)
					{
						GamePlayer playerById = WorldMgr.GetPlayerById(allMemberByConsortia[i].UserID);
						if (playerById != null && playerById.PlayerId != Player.PlayerCharacter.ID)
						{
							playerById.UpdateBadgeId(num);
							playerById.SendMessage(LanguageMgr.GetTranslation("A sua sociedade mudou de emblema!"));
							playerById.UpdateProperties();
						}
					}
				}
			}
			Player.Out.sendBuyBadge(Player.PlayerCharacter.ConsortiaID, num, ValidDate, result, BadgeBuyTime, Player.PlayerCharacter.ID);
			Player.SendMessage(LanguageMgr.GetTranslation(msg));
			Player.UpdateBadgeId(num);
			Player.UpdateProperties();
			if (result)
			{
				using (ConsortiaBussiness consortiaBussiness = new ConsortiaBussiness())
				{
					consortiaBussiness.UpdateConsortiaRiches(Player.PlayerCharacter.ConsortiaID, Player.PlayerCharacter.ID, consortiaBadgeConfig.Cost, ref msg);
				}
			}
			return 0;
		}
	}
}
