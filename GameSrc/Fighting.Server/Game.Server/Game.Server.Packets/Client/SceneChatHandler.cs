using System;
using System.IO;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Logic;
using Game.Server.GameObjects;
using Game.Server.GMActives;
using Game.Server.Managers;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client
{
	[PacketHandler((int)ePackageType.SCENE_CHAT, "用户场景聊天")]
	public class SceneChatHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			packet.ClientID = client.Player.PlayerCharacter.ID;
			byte val = packet.ReadByte();
			bool flag = packet.ReadBoolean();
			packet.ReadString();
			string str = packet.ReadString();
			string[] message = str.Split('$');
			if (message.Length > 1 && message.Length <= 5)
			{
				if (message[1].Equals("ban") && CheckAdmin(client.Player.PlayerCharacter.ID, message[1]))
				{
					DateTime dt4 = DateTime.Now.AddYears(20);
					if (message.Length >= 4 && message[3].Length >= 8)
					{
						if (message.Length == 5)
						{
							int days = 0;
							if (!int.TryParse(message[4], out days))
							{
								client.Player.SendMessage("Failed converting to days");
								return 0;
							}
							dt4 = DateTime.Now.AddDays(days);
						}
						using (ManageBussiness manageBussiness = new ManageBussiness())
						{
							if (manageBussiness.ForbidPlayerByNickName(message[2], dt4, isExist: false, message[3]))
							{
								GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
								for (int i = 0; i < allPlayers.Length; i++)
								{
									allPlayers[i].SendMessage("O usuário " + message[2] + " foi banido do jogo.");
								}
								using (StreamWriter w2 = File.AppendText("BanLog.txt"))
								{
									Log(client.Player.PlayerCharacter.NickName + " banned the user " + message[2] + " for " + (dt4 - DateTime.Now).TotalDays + " days, reason: " + message[3], w2);
								}
							}
							else
							{
								client.Player.SendMessage("Failed to ban the user " + message[2]);
							}
						}
					}
					else
					{
						client.Player.SendMessage("Comando em formato inválido. Use $ban$nick$motivo$dias ou $ban$nick$motivo para ban permanente. Motivo não pode ser vazio ou muito curto!");
					}
				}
				if (message[1].Equals("mute") && CheckAdmin(client.Player.PlayerCharacter.ID, message[1]))
				{
					if (message.Length == 5 && message[4].Length >= 8)
					{
						DateTime dt3 = DateTime.Now.AddMinutes(Convert.ToInt32(message[3]));
						GamePlayer p = WorldMgr.GetClientByPlayerNickName(message[2]);
						if (p == null || dt3 <= DateTime.Now)
						{
							client.Player.SendMessage("Failed to mute the user" + message[2]);
							return 0;
						}
						p.PlayerCharacter.IsBanChat = true;
						p.PlayerCharacter.BanChatEndDate = dt3;
						using (StreamWriter w3 = File.AppendText("MuteLog.txt"))
						{
							Log(client.Player.PlayerCharacter.NickName + " muted the user " + message[2] + " for " + message[3] + " minutes, reason: " + message[4], w3);
						}
					}
					else
					{
						client.Player.SendMessage("Comando em formato inválido. Use $mute$nick$minutos$motivo. Motivo não pode ser vazio ou muito curto!");
					}
				}
				if (message[1].Equals("unban") && CheckAdmin(client.Player.PlayerCharacter.ID, message[1]))
				{
					if (message.Length == 4 && message[3].Length >= 8)
					{
						DateTime dt2 = DateTime.Now;
						using (ManageBussiness manageBussiness2 = new ManageBussiness())
						{
							if (manageBussiness2.ForbidPlayerByNickName(message[2], dt2, isExist: true))
							{
								using (StreamWriter w4 = File.AppendText("BanLog.txt"))
								{
									Log(client.Player.PlayerCharacter.NickName + " unban the user " + message[2] + ", reason: " + message[3], w4);
								}
							}
							else
							{
								client.Player.SendMessage("Failed to unban the user " + message[2]);
							}
						}
					}
					else
					{
						client.Player.SendMessage("Comando em formato inválido. Use $desban$nick$motivo. Motivo não pode ser vazio ou muito curto!");
					}
				}
				if (message[1].Equals("kick") && CheckAdmin(client.Player.PlayerCharacter.ID, message[1]))
				{
					if (message.Length == 4 && message[3].Length >= 8)
					{
						using (ManageBussiness managebussiness = new ManageBussiness())
						{
							if (managebussiness.KitoffUserByNickName(message[2], "Kick") == 0)
							{
								using (StreamWriter w = File.AppendText("KickLog.txt"))
								{
									Log(client.Player.PlayerCharacter.NickName + " kicked the user " + message[2] + ", reason: " + message[3], w);
								}
							}
							else
							{
								client.Player.SendMessage("Failed to kick the user " + message[2]);
							}
						}
					}
					else
					{
						client.Player.SendMessage("Comando em formato inválido. Use $kick$nick$motivo. Motivo não pode ser vazio ou muito curto!");
					}
				}
			}
			else
			{
				GSPacketIn @in = new GSPacketIn(19, client.Player.PlayerCharacter.ID);
				@in.WriteInt(client.Player.ZoneId);
				@in.WriteByte(val);
				@in.WriteBoolean(flag);
				@in.WriteString(client.Player.PlayerCharacter.NickName);
				@in.WriteString(str);
				if (client.Player.CurrentRoom != null && client.Player.CurrentRoom.RoomType == eRoomType.Match && client.Player.CurrentRoom.Game != null)
				{
					if (val != 3)
					{
						client.Player.CurrentRoom.BattleServer.Server.SendChatMessage(str, client.Player, flag);
					}
					else
					{
						if (client.Player.PlayerCharacter.ConsortiaID == 0)
						{
							return 0;
						}
						if (client.Player.PlayerCharacter.IsBanChat)
						{
							client.Out.SendMessage(eMessageType.ChatERROR, LanguageMgr.GetTranslation("ConsortiaChatHandler.IsBanChat"));
							return 1;
						}
						@in.WriteInt(client.Player.PlayerCharacter.ConsortiaID);
						GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
						foreach (GamePlayer player2 in allPlayers)
						{
							if (player2.PlayerCharacter.ConsortiaID == client.Player.PlayerCharacter.ConsortiaID && !player2.IsBlackFriend(client.Player.PlayerCharacter.ID))
							{
								player2.Out.SendTCP(@in);
							}
						}
					}
					return 1;
				}
				switch (val)
				{
					case 3:
						{
							if (client.Player.PlayerCharacter.ConsortiaID == 0)
							{
								return 0;
							}
							if (client.Player.PlayerCharacter.IsBanChat)
							{
								client.Out.SendMessage(eMessageType.ChatERROR, LanguageMgr.GetTranslation("ConsortiaChatHandler.IsBanChat"));
								return 1;
							}
							@in.WriteInt(client.Player.PlayerCharacter.ConsortiaID);
							GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
							foreach (GamePlayer player in allPlayers)
							{
								if (player.PlayerCharacter.ConsortiaID == client.Player.PlayerCharacter.ConsortiaID && !player.IsBlackFriend(client.Player.PlayerCharacter.ID))
								{
									player.Out.SendTCP(@in);
								}
							}
							break;
						}
					case 9:
						if (client.Player.CurrentMarryRoom == null)
						{
							return 1;
						}
						client.Player.CurrentMarryRoom.SendToAllForScene(@in, client.Player.MarryMap);
						break;
					case 13:
						if (client.Player.CurrentHotSpringRoom == null)
						{
							return 1;
						}
						client.Player.CurrentHotSpringRoom.SendToRoomPlayer(@in);
						break;
					default:
						{
							if (client.Player.CurrentRoom != null)
							{
								if (flag)
								{
									client.Player.CurrentRoom.SendToTeam(@in, client.Player.CurrentRoomTeam, client.Player);
								}
								else
								{
									client.Player.CurrentRoom.SendToAll(@in);
								}
								break;
							}
							#region OLD
							/*if (str == "xoatrangbi")
							{
								ItemInfo item = client.Player.EquipBag.GetItemAt(78);
								if (item == null)
                                {
									client.Out.SendMessage(eMessageType.ERROR, string.Format("không tìm thấy vật phẩm cần xóa tại ô cuối BAG1 trang bị"));
                                }
								else
                                {
									client.Out.SendMessage(eMessageType.ERROR, string.Format("Hủy vật phẩm thành công!"));
									client.Player.RemoveItem(item);
								}									
							}
							if (str == "xoadaocu")
                            {
								ItemInfo daocu = client.Player.PropBag.GetItemAt(47);
								if (daocu == null)
                                {
									client.Out.SendMessage(eMessageType.ERROR, string.Format("không tìm thấy vật phẩm cần xóa tại ô cuối BAG1 đạo cụ"));
								}
								else
                                {
									client.Out.SendMessage(eMessageType.ERROR, string.Format("Hủy vật phẩm thành công!"));
									client.Player.RemoveItem(daocu);
								}								
                            }
							else if (str != "xoatrangbi" || str != "xoadaocu")
							{
								if (DateTime.Compare(client.Player.LastChatTime.AddSeconds(30.0), DateTime.Now) > 0)
								{
									client.Out.SendMessage(eMessageType.ChatERROR, LanguageMgr.GetTranslation("SceneChatHandler.Fast"));
									return 1;
								}
							}*/
							#endregion
							if (str == "reloadserver" && client.Player.PlayerCharacter.UserName == "hungchivass" || client.Player.PlayerCharacter.UserName == "kentaruu2")
							{
								DropMgr.ReLoad();
								QuestMgr.ReLoad();
								ShopMgr.ReLoad();
								ItemBoxMgr.ReLoad();
								ItemMgr.ReLoad();
								client.Player.SendMessage("reload thành công.");
							}
							if(str == "xoatrangbi")
                            {
								ItemInfo itemDelete = client.Player.GetItemAt(eBageType.EquipBag, 78);
								if(itemDelete != null)
                                {
									client.Player.RemoveItem(itemDelete);
									client.Player.SendMessage($"Xóa thành công trang bị {itemDelete.Template.Name}");
                                }
								else
                                {
									client.Player.SendMessage("không có item nào để xóa!");
								}									
                            }
							if(str == "xoadaocu")
                            {
								ItemInfo itemDelete = client.Player.GetItemAt(eBageType.PropBag, 47);
								if (itemDelete != null)
								{
									client.Player.RemoveItem(itemDelete);
									client.Player.SendMessage($"Xóa thành công đạo cụ {itemDelete.Template.Name}");
								}
								else
                                {
									client.Player.SendMessage("không có item nào để xóa!");
                                }									
							}								
							if (str == "Úm ba la,hụt đi!" && client.Player.PlayerCharacter.UserName == "dzvcl" || client.Player.PlayerCharacter.UserName == "anhtai30" || client.Player.PlayerCharacter.UserName == "trankhaiquang")
							{
								GmActivityMgr.OnUpdateWin(client.Player, 3);
							}
							if (str == "Ready 3,2,1!" && client.Player.PlayerCharacter.UserName == "dzvcl" || client.Player.PlayerCharacter.UserName == "anhtai30" || client.Player.PlayerCharacter.UserName == "trankhaiquang")
							{
								GmActivityMgr.OnUpdateOnline(client.Player, 10);
							}
							if (str == "reloaddrop" && client.Player.PlayerCharacter.UserName == "khangbro")
							{
								DropMgr.ReLoad();
								client.Player.SendMessage("reload drop phó bản thành công.");
								return 1;
							}
							if (str == "reloadquest" && client.Player.PlayerCharacter.UserName == "khangbro")
							{
								QuestMgr.ReLoad();
								client.Player.SendMessage("reload nhiệm vụ thành công.");
								return 1;
							}
							if (str == "reloadshop" && client.Player.PlayerCharacter.UserName == "khangbro")
							{
								ShopMgr.ReLoad();
								client.Player.SendMessage("reload shop thành công.");
								return 1;
							}
							if (str == "reloaditems" && client.Player.PlayerCharacter.UserName == "khangbro")
							{
								ItemMgr.ReLoad();
								client.Player.SendMessage("reload tất cả vật phẩm thành công.");
								return 1;
							}
							if (str == "reloaditembox" && client.Player.PlayerCharacter.UserName == "khangbro")
							{
								ItemBoxMgr.ReLoad();
								client.Player.SendMessage("reload tất cả vật phẩm rương thành công.");
								return 1;
							}
							if (str == "reloadNPC" && client.Player.PlayerCharacter.UserName == "khangbro")
							{
								NPCInfoMgr.ReLoad();
								client.Player.SendMessage("reload bossses thanh cong.");
								return 1;
							}
							if (str == "reloadavatar" && client.Player.PlayerCharacter.UserName == "khangbro")
							{
								AvatarColectionMgr.ReLoad();
								client.Player.SendMessage("reload Avatar thanh cong.");
								return 1;
							}
							if (DateTime.Compare(client.Player.LastChatTime.AddSeconds(1.0), DateTime.Now) > 0 && val == 5)
							{
								return 1;
							}
							if (flag)
							{
								return 1;
							}
							client.Player.LastChatTime = DateTime.Now;
							GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
							foreach (GamePlayer player3 in allPlayers)
							{
								if (player3.CurrentRoom == null && player3.CurrentMarryRoom == null && player3.CurrentHotSpringRoom == null && !player3.IsBlackFriend(client.Player.PlayerCharacter.ID))
								{
									player3.Out.SendTCP(@in);
								}
							}
							break;
						}
				}
			}
			return 1;
		}

		public bool CheckAdmin(int UserID, string Command)
		{
			return CommandsMgr.CheckAdmin(UserID, Command);
		}

		public static void Log(string logMessage, TextWriter w)
		{
			w.Write("\r\nLog Entry : ");
			w.WriteLine("{0} {1}", DateTime.Now.ToLongTimeString(), DateTime.Now.ToLongDateString());
			w.WriteLine("  :");
			w.WriteLine("  :{0}", logMessage);
			w.WriteLine("-------------------------------");
		}
	}
}
