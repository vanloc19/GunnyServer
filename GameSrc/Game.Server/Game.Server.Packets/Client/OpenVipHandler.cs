using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using System;

namespace Game.Server.Packets.Client
{
	[PacketHandler(92, "场景用户离开")]
	public class OpenVipHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			//client.Out.SendMessage(eMessageType.Normal, "Tính năng bị khóa!");
			//return 0;
			//packet.ReadString();
			//packet.ReadInt();
			//client.Out.SendAcademySystemNotice($"Chức năng tiếp phí VIP tạm tắt!!!", true);
			#region OpenVip
			string NickName = packet.ReadString();
			int reneval_days = packet.ReadInt();
			int ONE_MONTH_PAY = 320000;
			int THREE_MONTH_PAY = 960000;
			int SIX_MONTH_PAY = 1920000;
			int ONE_YEAR_PAY = 3380000;
			int money;
			int days = reneval_days;
			string msg = string.Format("Kích hoạt VIP thành công!");

			switch (reneval_days)
			{
				case 31:
					money = ONE_MONTH_PAY;
					break;
				case 93:
					money = THREE_MONTH_PAY;
					break;
				case 186:
					money = SIX_MONTH_PAY;
					break;
				case 365:
					money = ONE_YEAR_PAY;
					break;
				default:
					money = days / 31 * 320000;
					break;
			}

			GamePlayer player = Managers.WorldMgr.GetClientByPlayerNickName(NickName);
			//if (client.Player.MoneyDirect(money, false))
			if (client.Player.RemoveMoney(money, true) > 0)
			{
				DateTime ExpireDayOut = DateTime.Now;
				using (PlayerBussiness pb = new PlayerBussiness())
				{
					int TypeVIP = client.Player.SetTypeVIP(days);
					pb.VIPRenewal(NickName, reneval_days, TypeVIP, ref ExpireDayOut);
					if (player == null)
					{
						msg = string.Format("Tiếp phí VIP cho {0} thàng công!", NickName);
					}
					else if (client.Player.PlayerCharacter.NickName == NickName)
					{
						if (client.Player.PlayerCharacter.VIPLevel == 9)
						{
							client.Player.SendMessage("Cấp VIP đã đạt tối đa!");
							return 0;
						}
						else
						{
							if (client.Player.PlayerCharacter.typeVIP == 0)
							{
								client.Player.OpenVIP(days, ExpireDayOut);
								client.Player.PlayerCharacter.VIPNextLevelDaysNeeded = 10;
							}
							else
							{
								client.Player.ContinuousVIP(days, ExpireDayOut);
								msg = string.Format("Gia hạn VIP thành công!");
							}
						}
						client.Out.SendOpenVIP(client.Player);
					}
					else
					{
						string msg1;
						if (player.PlayerCharacter.typeVIP == 0)
						{
							player.OpenVIP(days, ExpireDayOut);
							player.PlayerCharacter.VIPNextLevelDaysNeeded = 10;
							msg = string.Format("Kích hoạt VIP cho {0} thàng công!", NickName);
							msg1 = string.Format("{0}, tiếp phí VIP cho bạn thành công!", client.Player.PlayerCharacter.NickName);
						}
						else
						{
							player.ContinuousVIP(days, ExpireDayOut);
							msg = string.Format("Gia hạn VIP cho {0} thàng công!", NickName);
							msg1 = string.Format("{0}, gia hạn VIP cho bạn thành công!", client.Player.PlayerCharacter.NickName);
						}
						player.Out.SendOpenVIP(player);
						player.Out.SendMessage(eMessageType.Normal, msg1);
					}
					if(money == ONE_YEAR_PAY)
					{
                        client.Player.AddExpVip(1200);
					}
					else
					{
                        client.Player.AddExpVip(money / 3200);
                    }
					
					client.Out.SendMessage(eMessageType.Normal, msg);
					if (client.Player.PlayerCharacter.typeVIP > 0)
					{
						client.Player.PlayerCharacter.VIPNextLevelDaysNeeded = client.Player.GetVIPNextLevelDaysNeeded(client.Player.PlayerCharacter.VIPLevel, client.Player.PlayerCharacter.VIPExp);
					}
				}
			}
			#endregion
			return 0;
		}
	}
}