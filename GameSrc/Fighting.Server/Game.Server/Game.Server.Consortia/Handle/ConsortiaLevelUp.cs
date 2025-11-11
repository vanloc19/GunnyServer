using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using SqlDataProvider.Data;

namespace Game.Server.Consortia.Handle
{
	[global::Consortia(21)]
	public class ConsortiaLevelUp : IConsortiaCommandHadler
	{
		public int CommandHandler(GamePlayer Player, GSPacketIn packet)
		{
			if (Player.PlayerCharacter.ConsortiaID == 0)
			{
				return 0;
			}
			byte val1 = packet.ReadByte();
			string msg = "";
			string str = "";
			byte val2 = 0;
			bool val3 = false;
			ConsortiaInfo consortiaInfo = ConsortiaMgr.FindConsortiaInfo(Player.PlayerCharacter.ConsortiaID);
			switch (val1)
			{
			case 1:
			{
				msg = "ConsortiaUpGradeHandler.Failed";
				using (ConsortiaBussiness consortiaBussiness2 = new ConsortiaBussiness())
				{
					ConsortiaInfo consortiaSingle = consortiaBussiness2.GetConsortiaSingle(Player.PlayerCharacter.ConsortiaID);
					if (consortiaSingle == null)
					{
						msg = "ConsortiaUpGradeHandler.NoConsortia";
					}
					else
					{
						ConsortiaLevelInfo consortiaLevelInfo = ConsortiaExtraMgr.FindConsortiaLevelInfo(consortiaSingle.Level + 1);
						if (consortiaLevelInfo == null)
						{
							msg = "ConsortiaUpGradeHandler.NoUpGrade";
						}
						else if (consortiaLevelInfo.NeedGold > Player.PlayerCharacter.Gold)
						{
							msg = "ConsortiaUpGradeHandler.NoGold";
						}
						else
						{
							using (ConsortiaBussiness consortiaBussiness3 = new ConsortiaBussiness())
							{
								if (consortiaBussiness3.UpGradeConsortia(Player.PlayerCharacter.ConsortiaID, Player.PlayerCharacter.ID, ref msg))
								{
									consortiaSingle.Level++;
									Player.RemoveGold(consortiaLevelInfo.NeedGold);
									GameServer.Instance.LoginServer.SendConsortiaUpGrade(consortiaSingle);
									msg = "ConsortiaUpGradeHandler.Success";
									val3 = true;
									val2 = (byte)consortiaSingle.Level;
								}
							}
						}
					}
					if (consortiaSingle.Level >= 5)
					{
						str = LanguageMgr.GetTranslation("ConsortiaUpGradeHandler.Notice", consortiaSingle.ConsortiaName, consortiaSingle.Level);
					}
				}
				break;
			}
			case 2:
			{
				msg = "ConsortiaStoreUpGradeHandler.Failed";
				if (consortiaInfo == null)
				{
					msg = "ConsortiaStoreUpGradeHandler.NoConsortia";
					break;
				}
				using (ConsortiaBussiness consortiaBussiness = new ConsortiaBussiness())
				{
					if (consortiaBussiness.UpGradeStoreConsortia(Player.PlayerCharacter.ConsortiaID, Player.PlayerCharacter.ID, ref msg))
					{
						consortiaInfo.StoreLevel++;
						GameServer.Instance.LoginServer.SendConsortiaStoreUpGrade(consortiaInfo);
						msg = "ConsortiaStoreUpGradeHandler.Success";
						val3 = true;
						val2 = (byte)consortiaInfo.StoreLevel;
					}
				}
				break;
			}
			case 3:
				msg = "ConsortiaShopUpGradeHandler.Failed";
				if (consortiaInfo == null)
				{
					msg = "ConsortiaShopUpGradeHandler.NoConsortia";
				}
				else
				{
					using (ConsortiaBussiness consortiaBussiness5 = new ConsortiaBussiness())
					{
						if (consortiaBussiness5.UpGradeShopConsortia(Player.PlayerCharacter.ConsortiaID, Player.PlayerCharacter.ID, ref msg))
						{
							consortiaInfo.ShopLevel++;
							GameServer.Instance.LoginServer.SendConsortiaShopUpGrade(consortiaInfo);
							msg = "ConsortiaShopUpGradeHandler.Success";
							val3 = true;
							val2 = (byte)consortiaInfo.ShopLevel;
						}
					}
				}
				if (consortiaInfo.ShopLevel >= 2)
				{
					str = LanguageMgr.GetTranslation("ConsortiaShopUpGradeHandler.Notice", Player.PlayerCharacter.ConsortiaName, consortiaInfo.ShopLevel);
				}
				break;
			case 4:
				msg = "ConsortiaSmithUpGradeHandler.Failed";
				if (consortiaInfo == null)
				{
					msg = "ConsortiaSmithUpGradeHandler.NoConsortia";
				}
				else
				{
					using (ConsortiaBussiness consortiaBussiness6 = new ConsortiaBussiness())
					{
						if (consortiaBussiness6.UpGradeSmithConsortia(Player.PlayerCharacter.ConsortiaID, Player.PlayerCharacter.ID, ref msg))
						{
							consortiaInfo.SmithLevel++;
							GameServer.Instance.LoginServer.SendConsortiaSmithUpGrade(consortiaInfo);
							msg = "ConsortiaSmithUpGradeHandler.Success";
							val3 = true;
							val2 = (byte)consortiaInfo.SmithLevel;
						}
					}
				}
				if (consortiaInfo.SmithLevel >= 3)
				{
					str = LanguageMgr.GetTranslation("ConsortiaSmithUpGradeHandler.Notice", Player.PlayerCharacter.ConsortiaName, consortiaInfo.SmithLevel);
				}
				break;
			case 5:
				msg = "ConsortiaBufferUpGradeHandler.Failed";
				if (consortiaInfo == null)
				{
					msg = "ConsortiaUpGradeHandler.NoConsortia";
				}
				else
				{
					using (ConsortiaBussiness consortiaBussiness4 = new ConsortiaBussiness())
					{
						if (consortiaBussiness4.UpGradeSkillConsortia(Player.PlayerCharacter.ConsortiaID, Player.PlayerCharacter.ID, ref msg))
						{
							consortiaInfo.SkillLevel++;
							GameServer.Instance.LoginServer.SendConsortiaKillUpGrade(consortiaInfo);
							msg = "ConsortiaBufferUpGradeHandler.Success";
							val3 = true;
							val2 = (byte)consortiaInfo.SkillLevel;
						}
					}
				}
				if (consortiaInfo.SkillLevel >= 3)
				{
					str = LanguageMgr.GetTranslation("ConsortiaBufferUpGradeHandler.Notice", Player.PlayerCharacter.ConsortiaName, consortiaInfo.SmithLevel);
				}
				break;
			}
			GSPacketIn packet2 = new GSPacketIn(129);
			packet2.WriteByte(21);
			packet2.WriteByte(val1);
			packet2.WriteByte(val2);
			packet2.WriteBoolean(val3);
			packet2.WriteString(LanguageMgr.GetTranslation(msg));
			Player.Out.SendTCP(packet2);
			if (str != "")
			{
				GSPacketIn packet3 = new GSPacketIn(10);
				packet3.WriteInt(2);
				packet3.WriteString(str);
				GameServer.Instance.LoginServer.SendPacket(packet3);
				GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
				foreach (GamePlayer allPlayer in allPlayers)
				{
					if (allPlayer != Player)
					{
						allPlayer.Out.SendTCP(packet3);
					}
				}
			}
			return 0;
		}
	}
}
