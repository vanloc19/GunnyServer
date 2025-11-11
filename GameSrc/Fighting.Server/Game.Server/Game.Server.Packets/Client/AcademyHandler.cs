using System;
using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client
{
	[PacketHandler(141, "防沉迷系统开关")]
	public class AcademyHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			switch (packet.ReadByte())
			{
			case 4:
			{
				int num1 = packet.ReadInt();
				string str1 = packet.ReadString();
				if (AcademyMgr.GetRequest(client.Player.PlayerId, num1) != null)
				{
					break;
				}
				if (client.Player.PlayerCharacter.freezesDate <= DateTime.Now)
				{
					GamePlayer playerById = WorldMgr.GetPlayerById(num1);
					if (playerById != null && playerById.PlayerCharacter.apprenticeshipState < AcademyMgr.MASTER_FULL_STATE && AcademyMgr.CheckCanMaster(playerById.PlayerCharacter.Grade))
					{
						AcademyMgr.AddRequest(new AcademyRequestInfo
						{
							SenderID = client.Player.PlayerId,
							ReceiderID = num1,
							Type = 1,
							CreateTime = DateTime.Now
						});
						GSPacketIn pkg = new GSPacketIn(141);
						pkg.WriteByte(4);
						pkg.WriteInt(client.Player.PlayerId);
						pkg.WriteString(client.Player.PlayerCharacter.NickName);
						pkg.WriteString(str1);
						playerById.SendTCP(pkg);
					}
					else
					{
						client.Player.SendMessage(string.Format("Người chơi này không online."));
					}
				}
				else
				{
					client.Player.SendMessage(string.Format("Bạn bị giới hạn do trước đó đã từ bỏ đệ tử hoặc sư phụ. Vui lòng thử lại sau {0} giờ nữa.", checkDate(client.Player.PlayerCharacter.freezesDate)));
				}
				break;
			}
			case 5:
			{
				int num2 = packet.ReadInt();
				string str2 = packet.ReadString();
				if (AcademyMgr.GetRequest(client.Player.PlayerId, num2) != null)
				{
					break;
				}
				if (client.Player.PlayerCharacter.freezesDate <= DateTime.Now)
				{
					GamePlayer playerById2 = WorldMgr.GetPlayerById(num2);
					if (playerById2 != null && playerById2.PlayerCharacter.masterID == 0 && AcademyMgr.CheckCanApp(playerById2.PlayerCharacter.Grade))
					{
						AcademyMgr.AddRequest(new AcademyRequestInfo
						{
							SenderID = client.Player.PlayerId,
							ReceiderID = num2,
							Type = 0,
							CreateTime = DateTime.Now
						});
						GSPacketIn pkg2 = new GSPacketIn(141);
						pkg2.WriteByte(5);
						pkg2.WriteInt(client.Player.PlayerId);
						pkg2.WriteString(client.Player.PlayerCharacter.NickName);
						pkg2.WriteString(str2);
						playerById2.SendTCP(pkg2);
					}
					else
					{
						client.Player.SendMessage(string.Format("Người chơi này không online."));
					}
				}
				else
				{
					client.Player.SendMessage(string.Format("Bạn bị giới hạn do trước đó đã từ bỏ đệ tử hoặc sư phụ. Vui lòng thử lại sau {0} giờ nữa.", checkDate(client.Player.PlayerCharacter.freezesDate)));
				}
				break;
			}
			case 6:
			{
				int num3 = packet.ReadInt();
				AcademyRequestInfo request1 = AcademyMgr.GetRequest(num3, client.Player.PlayerId);
				if (request1 != null && request1.Type == 1)
				{
					AcademyMgr.RemoveRequest(request1);
					if (client.Player.PlayerCharacter.freezesDate <= DateTime.Now)
					{
						if (client.Player.PlayerCharacter.apprenticeshipState < AcademyMgr.MASTER_FULL_STATE && AcademyMgr.CheckCanMaster(client.Player.PlayerCharacter.Grade))
						{
							GamePlayer playerById3 = WorldMgr.GetPlayerById(num3);
							if (playerById3 != null && AcademyMgr.CheckCanApp(playerById3.PlayerCharacter.Grade))
							{
								if (AcademyMgr.AddApprentice(client.Player, playerById3))
								{
									playerById3.Out.SendAcademySystemNotice(string.Format("[{0}] đã chấp nhận bạn làm sư phụ", client.Player.PlayerCharacter.NickName), isAlert: true);
									client.Player.SendMailToUser(new PlayerBussiness(), string.Format("Xin chúc mừng bạn có một đệ tử. Khi sư phụ đạt cấp 10/15/18 sẽ nhận được rương bảo vật của cấp tương ứng. Bên trong rương có rất nhiều thứ hay ho như Đá cường hóa, tiền vàng, điểm kinh nghiệm!"), string.Format("Thông báo khen thưởng sư đồ!"), eMailType.ItemOverdue);
									client.Player.SendMessage(string.Format("[{0}] đã chấp nhận làm đồ đệ của bạn", playerById3.PlayerCharacter.NickName));
								}
								else
								{
									client.Player.SendMessage(string.Format("Thật tiếc, đối phương đã có sư phụ hãy nhanh hơn vào lần sau"));
								}
							}
							else
							{
								client.Player.SendMessage(string.Format("Đối phương không trực tuyến, vui lòng đợi hoặc thử lại sau!"));
							}
						}
						else
						{
							client.Player.SendMessage(LanguageMgr.GetTranslation("Thật tiếc, đối phương đã là bậc thầy hãy thử lại vào lần sau!"));
						}
					}
					else
					{
						client.Player.SendMessage(string.Format("Bạn bị giới hạn do trước đó đã từ bỏ đệ tử hoặc sư phụ. Vui lòng thử lại sau {0} giờ nữa.", checkDate(client.Player.PlayerCharacter.freezesDate)));
					}
				}
				else
				{
					client.Player.SendMessage(string.Format("Sổ đăng ký thiếu hoặc đã bị xóa."));
				}
				break;
			}
			case 7:
			{
				int num4 = packet.ReadInt();
				AcademyRequestInfo request2 = AcademyMgr.GetRequest(num4, client.Player.PlayerId);
				if (request2 != null && request2.Type == 0)
				{
					AcademyMgr.RemoveRequest(request2);
					if (client.Player.PlayerCharacter.freezesDate <= DateTime.Now)
					{
						if (client.Player.PlayerCharacter.masterID == 0 && AcademyMgr.CheckCanApp(client.Player.PlayerCharacter.Grade))
						{
							GamePlayer playerById4 = WorldMgr.GetPlayerById(num4);
							if (playerById4 != null && playerById4.PlayerCharacter.Grade >= client.Player.PlayerCharacter.Grade + AcademyMgr.LEVEL_GAP && AcademyMgr.CheckCanMaster(playerById4.PlayerCharacter.Grade))
							{
								if (AcademyMgr.AddApprentice(playerById4, client.Player))
								{
									playerById4.Out.SendAcademySystemNotice(LanguageMgr.GetTranslation("Game.Server.AppSystem.ApprenticeConfirm", client.Player.PlayerCharacter.NickName), isAlert: true);
									playerById4.SendMailToUser(new PlayerBussiness(), LanguageMgr.GetTranslation("Game.Server.AppSystem.TakeApprenticeMail.Content"), LanguageMgr.GetTranslation("Game.Server.AppSystem.TakeApprenticeMail.Title"), eMailType.ItemOverdue);
									client.Player.SendMessage(LanguageMgr.GetTranslation("Game.Server.AppSystem.MasterConfirm", playerById4.PlayerCharacter.NickName));
								}
								else
								{
									client.Player.SendMessage(LanguageMgr.GetTranslation("Game.Server.AppSystem.AlreadlyHasRelationship.Apprentice"));
								}
							}
							else
							{
								client.Player.SendMessage(LanguageMgr.GetTranslation("LoginServerConnector.HandleSysMess.Msg2"));
							}
						}
						else
						{
							client.Player.SendMessage(LanguageMgr.GetTranslation("Game.Server.AppSystem.BeApprentice.Failed"));
						}
					}
					else
					{
						client.Player.SendMessage(LanguageMgr.GetTranslation("Game.Server.AppSystem.BeApprentice.Frozen", checkDate(client.Player.PlayerCharacter.freezesDate)));
					}
				}
				else
				{
					client.Player.SendMessage(LanguageMgr.GetTranslation("Game.Server.AppSystem.AppClub.RemoveInfo.RecordNotFound"));
				}
				break;
			}
			case 8:
			{
				int num5 = packet.ReadInt();
				AcademyRequestInfo request3 = AcademyMgr.GetRequest(num5, client.Player.PlayerId);
				if (request3 != null && request3.Type == 1)
				{
					AcademyMgr.RemoveRequest(request3);
					WorldMgr.GetPlayerById(num5)?.Out.SendAcademySystemNotice(LanguageMgr.GetTranslation("Game.Server.AppSystem.MasterRefuse", client.Player.PlayerCharacter.NickName), isAlert: false);
				}
				break;
			}
			case 9:
			{
				int num6 = packet.ReadInt();
				AcademyRequestInfo request4 = AcademyMgr.GetRequest(num6, client.Player.PlayerId);
				if (request4 != null && request4.Type == 0)
				{
					AcademyMgr.RemoveRequest(request4);
					WorldMgr.GetPlayerById(num6)?.Out.SendAcademySystemNotice(LanguageMgr.GetTranslation("Game.Server.AppSystem.ApprenticeRefuse", client.Player.PlayerCharacter.NickName), isAlert: false);
				}
				break;
			}
			case 12:
			{
				int removeUserId = packet.ReadInt();
				if (client.Player.RemoveGold(10000) > 0)
				{
					if (client.Player.PlayerCharacter.masterID == removeUserId && AcademyMgr.FireMaster(client.Player, isComplete: false))
					{
						client.Player.PlayerCharacter.freezesDate = DateTime.Now.AddHours(GameProperties.AcademyApprenticeFreezeHours);
						using (PlayerBussiness playerBussiness = new PlayerBussiness())
						{
							playerBussiness.UpdateAcademyPlayer(client.Player.PlayerCharacter);
						}
						client.Player.Out.SendAcademyAppState(client.Player.PlayerCharacter, removeUserId);
					}
					else
					{
						client.Player.SendMessage(LanguageMgr.GetTranslation("Game.Server.AppSystem.BreakApprentice.FireApprenticeCD"));
					}
				}
				else
				{
					client.Player.SendMessage(LanguageMgr.GetTranslation("Game.Server.AppSystem.BreakApprentice.NotEnoughGold"));
				}
				break;
			}
			case 13:
			{
				int num7 = packet.ReadInt();
				if (client.Player.RemoveGold(20000) > 0)
				{
					if (client.Player.PlayerCharacter.apprenticeshipState >= AcademyMgr.MASTER_STATE && AcademyMgr.FireApprentice(client.Player, num7, isSilent: false))
					{
						client.Player.PlayerCharacter.freezesDate = DateTime.Now.AddHours(GameProperties.AcademyMasterFreezeHours);
						using (PlayerBussiness playerBussiness2 = new PlayerBussiness())
						{
							playerBussiness2.UpdateAcademyPlayer(client.Player.PlayerCharacter);
						}
						client.Player.Out.SendAcademyAppState(client.Player.PlayerCharacter, num7);
					}
					else
					{
						client.Player.SendMessage(LanguageMgr.GetTranslation("Game.Server.AppSystem.BreakApprentice.FireApprenticeCD"));
					}
				}
				else
				{
					client.Player.SendMessage(LanguageMgr.GetTranslation("Game.Server.AppSystem.BreakApprentice.NotEnoughGold"));
				}
				break;
			}
			}
			return 0;
		}

		private int checkDate(DateTime dateTime)
		{
			if (dateTime > DateTime.Now)
			{
				return (int)Math.Ceiling((dateTime - DateTime.Now).TotalHours);
			}
			return 0;
		}
	}
}
