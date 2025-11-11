using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.Managers;
using SqlDataProvider.Data;
using System.Collections.Generic;

namespace Game.Server.Packets.Client
{
	[PacketHandler(27, "打开物品")]
	public class LotteryRandomSelectHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			int templateId = 11444;
			string text = "Rương Thần Tài";
			if (client.Player.LotteryID == 190000)
			{
				templateId = 190001;
				text = "Rương Thần Tài";// tên rương là gì lẹ nào má 
			}
			if (client.Player.Lottery >= 0 && client.Player.LotteryID > 0)
			{
				if (client.Player.Lottery >= 8)
				{
					client.Player.SendMessage("Bạn đã hết lượt mở rương.");
				}
				else if (client.Player.PropBag.GetItemCount(templateId) < client.Player.Lottery + 1)
				{
					client.Player.SendMessage("Không đủ chìa để mở rương.");
				}
				else
				{
					client.Player.Lottery++;
					List<ItemInfo> list = new List<ItemInfo>();
					SpecialItemDataInfo specialInfo = new SpecialItemDataInfo();
					List<ItemBoxInfo> resultBox = new List<ItemBoxInfo>();
					ItemBoxMgr.CreateLotteryItemBox(client.Player.LotteryID, client.Player.LotteryItems, list, specialInfo, resultBox);
					if (list.Count > 0)
					{
						client.Player.LotteryAwardList.Add(list[0]);
						GSPacketIn gSPacketIn = new GSPacketIn(30);
						gSPacketIn.WriteBoolean(val: true);
						gSPacketIn.WriteInt(list[0].TemplateID);
						gSPacketIn.WriteInt(list[0].Template.Quality);
						gSPacketIn.WriteInt(list[0].StrengthenLevel);
						gSPacketIn.WriteInt(list[0].AttackCompose);
						gSPacketIn.WriteInt(list[0].DefendCompose);
						gSPacketIn.WriteInt(list[0].LuckCompose);
						gSPacketIn.WriteInt(list[0].AgilityCompose);
						gSPacketIn.WriteBoolean(list[0].IsBinds);
						gSPacketIn.WriteInt(list[0].ValidDate);
						gSPacketIn.WriteByte((byte)list[0].Count);
						client.Player.SendTCP(gSPacketIn);
						client.Player.RemoveLotteryItems(list[0].TemplateID, list[0].Count);
						client.Player.PropBag.RemoveTemplate(templateId, client.Player.Lottery);
						client.Player.AddLog("Lottery", "LotteryID:" + client.Player.LotteryID + "|ItemAward:" + list[0].TemplateID + "|Name:" + list[0].Template.Name + "|CurrentCount:" + client.Player.PropBag.GetItemCount(templateId));
						if (resultBox[0].IsTips > 0)
						{
							string translation = LanguageMgr.GetTranslation("GameServer.LotteryOpenNoticeServer", client.Player.PlayerCharacter.NickName, list[0].TemplateID, text);
							GSPacketIn packet2 = WorldMgr.SendSysNotice(eMessageType.ChatNormal, translation, list[0].ItemID, list[0].TemplateID, null);
							GameServer.Instance.LoginServer.SendPacket(packet2);
						}
					}
					else
					{
						client.Player.SendMessage("Lỗi lấy dữ liệu phần thưởng.");
					}
				}
			}
			else
			{
				client.Player.SendMessage("Bạn chưa mở rương.");
			}
			return 0;
		}
	}
}
