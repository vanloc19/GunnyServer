using Bussiness;
using Game.Base;
using Game.Base.Packets;
using Game.Logic;
using Game.Server;
using Game.Server.GameObjects;
using Game.Server.Games;
using Game.Server.GuildBattle;
using Game.Server.Managers;
using Game.Server.Packets;
using Game.Server.Rooms;
using SqlDataProvider.Data;
using System;
using System.Drawing;

namespace Game.Server.Packets.Client
{
	[PacketHandler(153, "场景用户离开")]
	public class ConsortiaBattleHander : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			byte num = packet.ReadByte();
			UserGuildBattleInfo point = GameMgr.GuildBattle.FindUser(client.Player.PlayerId);
			BaseRoom currentRoom = client.Player.CurrentRoom;
			if (currentRoom != null && point != null)
			{
				if (currentRoom.RoomType == eRoomType.ConsortiaBattle)
				{
					if (num <= 16)
					{
						switch (num)
						{
							case 3:
								{
									GameMgr.GuildBattle.SendAllPlayerList(point, false);
									break;
								}
							case 4:
								{
									int num1 = packet.ReadInt();
									int num2 = packet.ReadInt();
									string str = packet.ReadString();
									point.Postion = new Point(num1, num2);
									GameMgr.GuildBattle.SendPlayerMove(point, str);
									break;
								}
							case 5:
								{
									currentRoom.RemovePlayerUnsafe(client.Player);
									break;
								}
							case 6:
								{
									int num3 = packet.ReadInt();
									if (client.Player.MainWeapon == null)
									{
										client.Player.SendMessage(LanguageMgr.GetTranslation("Game.Server.SceneGames.NoEquip"));
										return 0;
									}
									GamePlayer playerById = WorldMgr.GetPlayerById(num3);
									if (playerById == null || !playerById.IsActive || playerById.CurrentRoom == null || playerById.CurrentRoom.IsPlaying)
									{
										client.Player.SendMessage("Lỗi bắt cặp.");
										break;
									}

									GameMgr.GuildBattle.ChallengeGame(client.Player, playerById);
									break;
								}
							default:
								{
									if (num == 16)
									{
										byte num4 = packet.ReadByte();
										GameMgr.GuildBattle.SendUpdateScore(point, num4);
										break;
									}

									Console.WriteLine(string.Concat("ConsortiaBattleType.", (ConsBatPackageType)num));
									return 0;
								}
						}
					}
					else if (num == 17)
					{
						int num5 = packet.ReadInt();
						packet.ReadBoolean();
						switch (num5)
						{
							case 1:
								{
									if (client.Player.PlayerCharacter.Money < 1000)
									{
										break;
									}
									client.Player.RemoveMoney(30, isConsume:false);
									client.Player.PlayerCharacter.ActivePowFirstGame = true;
									break;
								}
							case 2:
								{
									if (client.Player.PlayerCharacter.Money < 1000)
									{
										break;
									}
									client.Player.RemoveMoney(300, isConsume: false);
									point.DupeScoreConsortiaBattle = true;
									break;
								}
							case 3:
								{
									if (!point.IsDead || client.Player.PlayerCharacter.Money < 1000)
									{
										break;
									}
									client.Player.RemoveMoney(100, isConsume: false);
									GameMgr.GuildBattle.QuickRevive(point, false);
									break;
								}
							case 4:
								{
									if (!point.IsDead || client.Player.PlayerCharacter.Money < 1000)
									{
										break;
									}
									client.Player.RemoveMoney(800, isConsume: false);
									GameMgr.GuildBattle.QuickRevive(point, true);
									break;
								}
						}
						GameMgr.GuildBattle.SendUpdateSceneInfo(point);
					}
					else
					{
						if (num != 21)
						{
							Console.WriteLine(string.Concat("ConsortiaBattleType.", (ConsBatPackageType)num));
							return 0;
						}
						GameMgr.GuildBattle.SendAllPlayerList(point, true);
					}
					return 0;
				}
			}
			client.Player.SendMessage(LanguageMgr.GetTranslation("GameServer.GuildBattle.Error"));
			return 0;
		}
	}
}