using System;
using System.Collections.Generic;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client
{
	[PacketHandler(256, "LEAGUE_GETAWARD")]
	public class DailyLeagueGetRewardHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			int ClassID = packet.ReadInt();
			int awardGot = 0;
			int Score = 0;
			int Grade = 0;
			if (DateTime.Compare(client.Player.LastOpenCard.AddSeconds(0.5), DateTime.Now) > 0)
			{
				return 0;
			}
			string message = "DailyLeagueGetReward.Successfull";
			ProduceBussiness produceBussiness = new ProduceBussiness();
			DailyLeagueAwardList[] DailyLeagueInfo = produceBussiness.GetAllDailyLeagueAwardList();
			DailyLeagueAwardItems[] DailyLeagueGoods = produceBussiness.GetAllDailyLeagueAwardItems();
			List<ItemInfo> items = new List<ItemInfo>();
			DailyLeagueAwardList[] array = DailyLeagueInfo;
			foreach (DailyLeagueAwardList RewardList in array)
			{
				if (RewardList.Class == ClassID)
				{
					Score = RewardList.Score;
					Grade = RewardList.Grade;
					switch (Score)
					{
					case 150:
						awardGot = 1;
						break;
					case 200:
						awardGot = 3;
						break;
					case 250:
						awardGot = 7;
						break;
					case 300:
						awardGot = 15;
						break;
					case 400:
						awardGot = 31;
						break;
					case 550:
						awardGot = 63;
						break;
					case 700:
						awardGot = 127;
						break;
					case 850:
						awardGot = 255;
						break;
					case 900:
						awardGot = 511;
						break;
					default:
						awardGot = 1023;
						message = "DailyLeagueGetReward.Error";
						break;
					}
				}
			}
			awardGot = (1 << ClassID) - 1;
			DailyLeagueAwardItems[] array2 = DailyLeagueGoods;
			foreach (DailyLeagueAwardItems RewardGoods in array2)
			{
				if (RewardGoods.Class == ClassID)
				{
					ItemInfo item = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(RewardGoods.TemplateID), 1, 104);
					item.StrengthenLevel = RewardGoods.StrengthLevel;
					item.AttackCompose = RewardGoods.AttackCompose;
					item.DefendCompose = RewardGoods.DefendCompose;
					item.AgilityCompose = RewardGoods.AgilityCompose;
					item.LuckCompose = RewardGoods.LuckCompose;
					item.IsBinds = RewardGoods.IsBind;
					item.ValidDate = RewardGoods.ValidDate;
					item.Count = RewardGoods.Count;
					items.Add(item);
				}
			}
			if (items != null)
			{
				if (client.Player.PlayerCharacter.Grade >= Grade)
				{
					if (client.Player.MatchInfo.weeklyScore >= Score)
					{
						client.Player.RefreshLeagueGetReward(awardGot, Score);
						client.Player.SendItemsToMail(items, LanguageMgr.GetTranslation("DailyLeagueGetReward.Content"), LanguageMgr.GetTranslation("Game.Server.LeagueReward.Title"), eMailType.Manage);
						return 1;
					}
					message = "Você não possui pontos semanais suficientes para a compra";
					return 0;
				}
				message = "O nível não é suficiente para a coleta dos itens dessa grade";
				return 0;
			}
			client.Player.Out.SendMessage(eMessageType.GM_NOTICE, LanguageMgr.GetTranslation(message));
			client.Player.LastOpenCard = DateTime.Now;
			return 1;
		}
	}
}
