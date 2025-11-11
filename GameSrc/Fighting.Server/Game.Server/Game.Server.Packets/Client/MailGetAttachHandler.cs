using System;
using System.Collections.Generic;
using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client
{
	[PacketHandler(113, "获取邮件到背包")]
	public class MailGetAttachHandler : IPacketHandler
	{
		public bool GetAnnex(string value, GamePlayer player, ref string msg, ref bool result, ref eMessageType eMsg)
		{
			int itemID = int.Parse(value);
			using (PlayerBussiness bussiness = new PlayerBussiness())
			{
				ItemInfo userItemSingle = bussiness.GetUserItemSingle(itemID);
				if (userItemSingle != null && player.AddTemplate(userItemSingle))
				{
					eMsg = eMessageType.GM_NOTICE;
					return true;
				}
			}
			return false;
		}

		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			int mailID = packet.ReadInt();
			byte num2 = packet.ReadByte();
			List<int> list = new List<int>();
			List<string> list2 = new List<string>();
			int num3 = 0;
			int gold = 0;
			int giftToken = 0;
			string str = "";
			eMessageType normal = eMessageType.GM_NOTICE;
			if (client.Player.PlayerCharacter.HasBagPassword && client.Player.PlayerCharacter.IsLocked)
			{
				client.Out.SendMessage(eMessageType.GM_NOTICE, LanguageMgr.GetTranslation("Bag.Locked"));
				return 0;
			}
			GSPacketIn @in = new GSPacketIn(113, client.Player.PlayerCharacter.ID);
			using (PlayerBussiness bussiness = new PlayerBussiness())
			{
				client.Player.LastAttachMail = DateTime.Now;
				MailInfo mailSingle = bussiness.GetMailSingle(client.Player.PlayerCharacter.ID, mailID);
				if (mailSingle != null)
				{
					bool result = true;
					int money = mailSingle.Money;
					if (mailSingle.Type > 100 && !client.Player.MoneyDirect(money, IsAntiMult: true))
					{
						return 0;
					}
					GamePlayer playerById = WorldMgr.GetPlayerById(mailSingle.ReceiverID);
					if (!mailSingle.IsRead)
					{
						mailSingle.IsRead = true;
						mailSingle.ValidDate = 72;
						mailSingle.SendTime = DateTime.Now;
					}
					if (result && (num2 == 0 || num2 == 1) && !string.IsNullOrEmpty(mailSingle.Annex1))
					{
						list.Add(1);
						list2.Add(mailSingle.Annex1);
						mailSingle.Annex1 = null;
					}
					if (result && (num2 == 0 || num2 == 2) && !string.IsNullOrEmpty(mailSingle.Annex2))
					{
						list.Add(2);
						list2.Add(mailSingle.Annex2);
						mailSingle.Annex2 = null;
					}
					if (result && (num2 == 0 || num2 == 3) && !string.IsNullOrEmpty(mailSingle.Annex3))
					{
						list.Add(3);
						list2.Add(mailSingle.Annex3);
						mailSingle.Annex3 = null;
					}
					if (result && (num2 == 0 || num2 == 4) && !string.IsNullOrEmpty(mailSingle.Annex4))
					{
						list.Add(4);
						list2.Add(mailSingle.Annex4);
						mailSingle.Annex4 = null;
					}
					if (result && (num2 == 0 || num2 == 5) && !string.IsNullOrEmpty(mailSingle.Annex5))
					{
						list.Add(5);
						list2.Add(mailSingle.Annex5);
						mailSingle.Annex5 = null;
					}
					if ((num2 == 0 || num2 == 6) && mailSingle.Gold > 0)
					{
						list.Add(6);
						gold = mailSingle.Gold;
						mailSingle.Gold = 0;
					}
					if ((num2 == 0 || num2 == 7) && mailSingle.Type < 100 && mailSingle.Money > 0)
					{
						list.Add(7);
						num3 = mailSingle.Money;
						mailSingle.Money = 0;
					}
					if (mailSingle.Type > 100 && mailSingle.GiftToken > 0)
					{
						list.Add(8);
						giftToken = mailSingle.GiftToken;
						mailSingle.GiftToken = 0;
					}
					if (mailSingle.Type > 100 && mailSingle.Money > 0)
					{
						mailSingle.Money = 0;
						str = LanguageMgr.GetTranslation("MailGetAttachHandler.Deduct") + (string.IsNullOrEmpty(str) ? LanguageMgr.GetTranslation("MailGetAttachHandler.Success") : str);
					}
					if (bussiness.UpdateMail(mailSingle, money))
					{
						if (mailSingle.Type > 100 && money > 0)
						{
							client.Out.SendMailResponse(mailSingle.SenderID, eMailRespose.Receiver);
							client.Out.SendMailResponse(mailSingle.ReceiverID, eMailRespose.Send);
						}
						playerById.AddMoney(num3);
						playerById.AddGold(gold);
						playerById.AddGiftToken(giftToken);
						foreach (string str2 in list2)
						{
							GetAnnex(str2, client.Player, ref str, ref result, ref normal);
						}
					}
					@in.WriteInt(mailID);
					@in.WriteInt(list.Count);
					foreach (int num4 in list)
					{
						@in.WriteInt(num4);
					}
					client.Out.SendTCP(@in);
					client.Out.SendMessage(normal, string.IsNullOrEmpty(str) ? LanguageMgr.GetTranslation("MailGetAttachHandler.Success") : str);
				}
				else
				{
					client.Out.SendMessage(eMessageType.BIGBUGLE_NOTICE, LanguageMgr.GetTranslation("MailGetAttachHandler.Falied"));
				}
			}
			return 0;
		}
	}
}
