using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using Bussiness.CenterService;
using Bussiness.Managers;
using SqlDataProvider.Data;

namespace Bussiness
{
	public class PlayerBussiness : BaseBussiness
	{

		#region EliteGame
		public PlayerEliteGameInfo[] GetAllPlayerEliteGame()
		{
			List<PlayerEliteGameInfo> infos = new List<PlayerEliteGameInfo>();
			SqlDataReader reader = null;
			try
			{
				db.GetReader(ref reader, "SP_Elite_Game_Data_All");
				while (reader.Read())
				{
					infos.Add(InitPlayerEliteGame(reader));
				}
			}
			catch (Exception e)
			{
				if (log.IsErrorEnabled)
					log.Error("GetAllPlayerEliteGame", e);
			}
			finally
			{
				if (reader != null && !reader.IsClosed)
					reader.Close();
			}
			return infos.ToArray();
		}
		public PlayerEliteGameInfo InitPlayerEliteGame(SqlDataReader reader)
		{
			PlayerEliteGameInfo info = new PlayerEliteGameInfo
			{
				UserID = (int)reader["UserID"],
				NickName = reader["NickName"] == null ? "" : reader["NickName"].ToString(),
				AreaId = (int)reader["ZoneId"],
				GroupType = (int)reader["GroupType"],
				Rank = (int)reader["Rank"],
				CurrentPoint = (int)reader["CurrentPoint"],
				Status = (int)reader["Status"],
				MatchOrderNumber = (int)reader["MatchOrderNumber"],
				Winer = (int)reader["Winer"],
				ReadyStatus = (bool)reader["ReadyStatus"],
				RoundType = (int)reader["RoundType"],
				Grade = (int)reader["Grade"],
				Blood = (int)reader["Blood"],
				FightPower = (int)reader["FightPower"],
				TotalMatch = (int)reader["TotalMatch"]
			};
			return info;
		}

		public bool AddPlayerEliteGame(PlayerEliteGameInfo item)
		{
			bool result = false;
			try
			{
				SqlParameter[] para = new SqlParameter[17];
				para[0] = new SqlParameter("@UserID", item.UserID);
				para[1] = new SqlParameter("@NickName", item.NickName);
				para[2] = new SqlParameter("@ZoneId", item.AreaId);
				para[3] = new SqlParameter("@GroupType", item.GroupType);
				para[4] = new SqlParameter("@Rank", item.Rank);
				para[5] = new SqlParameter("@CurrentPoint", item.CurrentPoint);
				para[6] = new SqlParameter("@Status", item.Status);
				para[7] = new SqlParameter("@MatchOrderNumber", item.MatchOrderNumber);
				para[8] = new SqlParameter("@Winer", item.Winer);
				para[9] = new SqlParameter("@ReadyStatus", item.ReadyStatus);
				para[10] = new SqlParameter("@RoundType", item.RoundType);
				para[11] = new SqlParameter("@Grade", item.Grade);
				para[12] = new SqlParameter("@Blood", item.Blood);
				para[13] = new SqlParameter("@FightPower", item.FightPower);
				para[14] = new SqlParameter("@UserMoneyPay", item.UserMoneyPay);
				para[15] = new SqlParameter("@Result", SqlDbType.Int);
				para[16] = new SqlParameter("@TotalMatch", item.TotalMatch);
				para[15].Direction = ParameterDirection.ReturnValue;
				db.RunProcedure("SP_Elite_Game_Data_Add", para);
				result = (int)para[15].Value == 0;
			}
			catch (Exception e)
			{
				if (log.IsErrorEnabled)
					log.Error("AddPlayerEliteGame", e);
			}
			return result;
		}

		public bool ClearEliteGameData()
		{
			bool result = false;
			try
			{
				SqlParameter[] para = new SqlParameter[0];
				result = db.RunProcedure("SP_Elite_Game_Data_Delete", para);
			}
			catch (Exception e)
			{
				if (log.IsErrorEnabled)
					log.Error("SP_Elite_Game_Data_Delete", e);
			}
			return result;
		}
		public bool ResetEliteGame(int point)
		{
			bool result = false;
			try
			{
				SqlParameter[] para = new SqlParameter[1];

				para[0] = new SqlParameter("@EliteScore", point);


				result = db.RunProcedure("SP_EliteGame_Reset", para);

				return result;

			}
			catch (Exception e)
			{
				if (log.IsErrorEnabled)
					log.Error("Init", e);
			}
			return result;
		}
		public bool UpdateEditable(string table, string column, string valueStr, string condition)
		{
			bool result = false;
			try
			{
				string sqlQuery = @"UPDATE [dbo].[" + table + "] SET [" + column + "] = @" + column + " WHERE " + condition;
				SqlParameter[] para = new SqlParameter[1];
				para[0] = new SqlParameter("@" + column, valueStr);
				result = db.Exesqlcomm(sqlQuery, para);
			}
			catch (Exception e)
			{
				log.Error(e.Message);
			}
			return result;
		}
		public ServerEventInfo[] GetServerEvent()//move this func to ProduceBussiness
		{
			List<ServerEventInfo> infos = new List<ServerEventInfo>();
			SqlDataReader reader = null;
			try
			{
				db.GetReader(ref reader, "SP_Get_ServerEvent");

				while (reader.Read())
				{
					ServerEventInfo info = new ServerEventInfo();
					info.ID = (int)reader["ID"];
					info.Name = (string)reader["Name"];
					info.Value = (string)reader["Value"];
					infos.Add(info);
				}

			}
			catch (Exception e)
			{
				if (log.IsErrorEnabled)
				{
					log.Error("SP_Get_ServerEvent", e);
				}
			}
			finally
			{
				if (reader != null && !reader.IsClosed)
				{
					reader.Close();
				}
			}

			return infos.ToArray();
		}
		#endregion

		#region gm activity
		public GmActiveRewardInfo[] GetAllGmActiveReward()
		{
			List<GmActiveRewardInfo> infos = new List<GmActiveRewardInfo>();
			SqlDataReader reader = null;
			try
			{
				db.GetReader(ref reader, "SP_GM_Active_Reward_All");
				while (reader.Read())
				{
					infos.Add(InitGmActiveRewardInfo(reader));
				}
			}
			catch (Exception e)
			{
				if (log.IsErrorEnabled)
					log.Error("InitGmActiveRewardInfo", e);
			}
			finally
			{
				if (reader != null && !reader.IsClosed)
					reader.Close();
			}
			return infos.ToArray();
		}
		public GmActiveRewardInfo InitGmActiveRewardInfo(SqlDataReader dr)
		{
			GmActiveRewardInfo info = new GmActiveRewardInfo();
			info.giftId = (string)dr["giftId"];
			info.templateId = (int)dr["templateId"];
			info.count = (int)dr["count"];
			info.isBind = (bool)dr["isBind"];
			info.occupationOrSex = (int)dr["occupationOrSex"];
			info.rewardType = (int)dr["rewardType"];
			info.validDate = (int)dr["validDate"];
			info.property = (string)dr["property"];
			info.remain1 = (string)dr["remain1"];
			return info;
		}
		public GmActiveConditionInfo[] GetAllGmActiveCondition()
		{
			List<GmActiveConditionInfo> infos = new List<GmActiveConditionInfo>();
			SqlDataReader reader = null;
			try
			{
				db.GetReader(ref reader, "SP_GM_Active_Condition_All");
				while (reader.Read())
				{
					infos.Add(InitGmActiveConditionInfo(reader));
				}
			}
			catch (Exception e)
			{
				if (log.IsErrorEnabled)
					log.Error("InitGmActiveConditionInfo", e);
			}
			finally
			{
				if (reader != null && !reader.IsClosed)
					reader.Close();
			}
			return infos.ToArray();
		}
		public GmActiveConditionInfo InitGmActiveConditionInfo(SqlDataReader dr)
		{
			GmActiveConditionInfo info = new GmActiveConditionInfo();
			info.giftbagId = (string)dr["giftbagId"];
			info.conditionIndex = (int)dr["conditionIndex"];
			info.conditionValue = (int)dr["conditionValue"];
			info.remain1 = (int)dr["remain1"];
			info.remain2 = dr["remain2"] == DBNull.Value ? null : (string)dr["remain2"];
			return info;
		}
		public GmGiftInfo[] GetAllGmGift()
		{
			List<GmGiftInfo> infos = new List<GmGiftInfo>();
			SqlDataReader reader = null;
			try
			{
				db.GetReader(ref reader, "SP_GM_Gift_All");
				while (reader.Read())
				{
					infos.Add(InitGmGiftInfo(reader));
				}
			}
			catch (Exception e)
			{
				if (log.IsErrorEnabled)
					log.Error("InitGmGiftInfo", e);
			}
			finally
			{
				if (reader != null && !reader.IsClosed)
					reader.Close();
			}
			return infos.ToArray();
		}
		public GmGiftInfo InitGmGiftInfo(SqlDataReader dr)
		{
			GmGiftInfo info = new GmGiftInfo();
			info.giftbagId = (string)dr["giftbagId"];
			info.activityId = (string)dr["activityId"];
			info.rewardMark = (int)dr["rewardMark"];
			info.giftbagOrder = (int)dr["giftbagOrder"];
			return info;
		}
		public GmActivityInfo[] GetAllGmActivity()
		{
			List<GmActivityInfo> infos = new List<GmActivityInfo>();
			SqlDataReader reader = null;
			try
			{
				db.GetReader(ref reader, "SP_GM_Activity_All");
				while (reader.Read())
				{
					infos.Add(InitGmActivityInfo(reader));
				}
			}
			catch (Exception e)
			{
				if (log.IsErrorEnabled)
					log.Error("InitGmActivityInfo", e);
			}
			finally
			{
				if (reader != null && !reader.IsClosed)
					reader.Close();
			}
			return infos.ToArray();
		}
		public GmActivityInfo InitGmActivityInfo(SqlDataReader dr)
		{
			GmActivityInfo info = new GmActivityInfo();
			info.activityId = (string)dr["activityId"];
			info.activityName = (string)dr["activityName"];
			info.activityType = (int)dr["activityType"];
			info.activityChildType = (int)dr["activityChildType"];
			info.getWay = (int)dr["getWay"];
			info.desc = (string)dr["desc"];
			info.rewardDesc = (string)dr["rewardDesc"];
			info.beginTime = (DateTime)dr["beginTime"];
			info.beginShowTime = (DateTime)dr["beginShowTime"];
			info.endTime = (DateTime)dr["endTime"];
			info.endShowTime = (DateTime)dr["endShowTime"];
			info.icon = (int)dr["icon"];
			info.isContinue = (int)dr["isContinue"];
			info.status = (int)dr["status"];
			info.remain1 = (int)dr["remain1"];
			info.remain2 = (string)dr["remain2"];
			//info.order = (int)dr["order"];
			return info;
		}
		#endregion

		public bool SendItemsToMail(List<ItemInfo> items, int playerId, int zoneId, string content, string title)
		{
			return SendItemsToMail(items, playerId, zoneId, content, title, 8);
		}
		public bool SendItemsToMail(List<ItemInfo> items, int playerId, int zoneId, string content, string title, int type)
		{
			bool result = false;
			List<ItemInfo> temp = new List<ItemInfo>();
			foreach (ItemInfo item in items)
			{
				if (item.Template.MaxCount == 1 && item.Count > 1)
				{
					for (int i = 0; i < item.Count; i++)
					{
						ItemInfo newitem = ItemInfo.CloneFromTemplate(item.Template, item);
						newitem.Count = 1;
						temp.Add(newitem);
					}
				}
				else
				{
					temp.Add(item);
				}
			}

			for (int i = 0; i < items.Count; i += 5)
			{
				ItemInfo it;
				MailInfo mail = new MailInfo
				{
					Title = (title != null) ? title : LanguageMgr.GetTranslation("Game.Server.GameUtils.Title"),
					Gold = 0,
					IsExist = true,
					Money = 0,
					Receiver = "",
					ReceiverID = playerId,
					Sender = "Administrator",
					SenderID = 0,
					Type = type,
					GiftToken = 0
				};
				List<ItemInfo> sent = new List<ItemInfo>();
				StringBuilder annexRemark = new StringBuilder();
				StringBuilder mailContent = new StringBuilder();
				annexRemark.Append(LanguageMgr.GetTranslation("Game.Server.GameUtils.CommonBag.AnnexRemark"));
				content = (content != null) ? LanguageMgr.GetTranslation(content) : "";
				int index = i;
				if (items.Count > index)
				{
					it = items[index];
					if (it.ItemID == 0)
					{
						AddGoods(it);
					}
					else
					{
						sent.Add(it);
					}
					if (title == null)
						mail.Title = it.Template.Name;

					mail.Annex1 = it.ItemID.ToString();
					mail.Annex1Name = it.Template.Name;
					annexRemark.Append(string.Concat("1、", mail.Annex1Name, "x", it.Count, ";"));
					mailContent.Append(string.Concat("1、", mail.Annex1Name, "x", it.Count, ";"));
					//sent.Add(it);
				}
				index = i + 1;
				if (items.Count > index)
				{
					it = items[index];
					if (it.ItemID == 0)
					{
						AddGoods(it);
					}
					else
					{
						sent.Add(it);
					}
					mail.Annex2 = it.ItemID.ToString();
					mail.Annex2Name = it.Template.Name;
					annexRemark.Append(string.Concat("2、", mail.Annex2Name, "x", it.Count, ";"));
					mailContent.Append(string.Concat("2、", mail.Annex2Name, "x", it.Count, ";"));
					//sent.Add(it);
				}
				index = i + 2;
				if (items.Count > index)
				{
					it = items[index];
					if (it.ItemID == 0)
					{
						AddGoods(it);
					}
					else
					{
						sent.Add(it);
					}
					mail.Annex3 = it.ItemID.ToString();
					mail.Annex3Name = it.Template.Name;
					annexRemark.Append(string.Concat("3、", mail.Annex3Name, "x", it.Count, ";"));
					mailContent.Append(string.Concat("3、", mail.Annex3Name, "x", it.Count, ";"));
					//sent.Add(it);
				}
				index = i + 3;
				if (items.Count > index)
				{
					it = items[index];
					if (it.ItemID == 0)
					{
						AddGoods(it);
					}
					else
					{
						sent.Add(it);
					}
					mail.Annex4 = it.ItemID.ToString();
					mail.Annex4Name = it.Template.Name;
					annexRemark.Append(string.Concat("4、", mail.Annex4Name, "x", it.Count, ";"));
					mailContent.Append(string.Concat("4、", mail.Annex4Name, "x", it.Count, ";"));
					//sent.Add(it);
				}
				index = i + 4;
				if (items.Count > index)
				{
					it = items[index];
					if (it.ItemID == 0)
					{
						AddGoods(it);
					}
					else
					{
						sent.Add(it);
					}
					mail.Annex5 = it.ItemID.ToString();
					mail.Annex5Name = it.Template.Name;
					annexRemark.Append(string.Concat("5、", mail.Annex5Name, "x", it.Count, ";"));
					mailContent.Append(string.Concat("5、", mail.Annex5Name, "x", it.Count, ";"));
					//sent.Add(it);
				}
				mail.AnnexRemark = annexRemark.ToString();
				if (content == null && mailContent.ToString() == null)
				{
					mail.Content = LanguageMgr.GetTranslation("Game.Server.GameUtils.Content");
				}
				else if (content != "")
				{
					mail.Content = content;
				}
				else
				{
					mail.Content = mailContent.ToString();
				}
				if (SendMail(mail, zoneId))
				{
					result = true;
				}
				else
				{
					result = false;
				}
			}
			return result;
		}
		public bool SendMail(MailInfo mail, int zoneId)
		{
			return SendMail(mail, zoneId, true);
		}
		public bool SendMail(MailInfo mail, int zoneId, bool isNotice)
		{
			bool result = false;
			try
			{
				SqlParameter[] para = new SqlParameter[29];
				para[0] = new SqlParameter("@ID", mail.ID);
				para[0].Direction = ParameterDirection.Output;
				para[1] = new SqlParameter("@Annex1", mail.Annex1 == null ? "" : mail.Annex1);
				para[2] = new SqlParameter("@Annex2", mail.Annex2 == null ? "" : mail.Annex2);
				para[3] = new SqlParameter("@Content", mail.Content == null ? "" : mail.Content);
				para[4] = new SqlParameter("@Gold", mail.Gold);
				para[5] = new SqlParameter("@IsExist", true);
				para[6] = new SqlParameter("@Money", mail.Money);
				para[7] = new SqlParameter("@Receiver", mail.Receiver == null ? "" : mail.Receiver);
				para[8] = new SqlParameter("@ReceiverID", mail.ReceiverID);
				para[9] = new SqlParameter("@Sender", mail.Sender == null ? "" : mail.Sender);
				para[10] = new SqlParameter("@SenderID", mail.SenderID);
				para[11] = new SqlParameter("@Title", mail.Title == null ? "" : mail.Title);
				para[12] = new SqlParameter("@IfDelS", false);
				para[13] = new SqlParameter("@IsDelete", false);
				para[14] = new SqlParameter("@IsDelR", false);
				para[15] = new SqlParameter("@IsRead", false);
				para[16] = new SqlParameter("@SendTime", DateTime.Now);
				para[17] = new SqlParameter("@Type", mail.Type);
				para[18] = new SqlParameter("@Annex1Name", mail.Annex1Name == null ? "" : mail.Annex1Name);
				para[19] = new SqlParameter("@Annex2Name", mail.Annex2Name == null ? "" : mail.Annex2Name);
				para[20] = new SqlParameter("@Annex3", mail.Annex3 == null ? "" : mail.Annex3);
				para[21] = new SqlParameter("@Annex4", mail.Annex4 == null ? "" : mail.Annex4);
				para[22] = new SqlParameter("@Annex5", mail.Annex5 == null ? "" : mail.Annex5);
				para[23] = new SqlParameter("@Annex3Name", mail.Annex3Name == null ? "" : mail.Annex3Name);
				para[24] = new SqlParameter("@Annex4Name", mail.Annex4Name == null ? "" : mail.Annex4Name);
				para[25] = new SqlParameter("@Annex5Name", mail.Annex5Name == null ? "" : mail.Annex5Name);
				para[26] = new SqlParameter("@ValidDate", mail.ValidDate);
				para[27] = new SqlParameter("@AnnexRemark", mail.AnnexRemark == null ? "" : mail.AnnexRemark);
				para[28] = new SqlParameter("GiftToken", mail.GiftToken);
				//        @ID = @ID OUTPUT,
				result = db.RunProcedure("SP_Mail_Send", para);
				mail.ID = (int)para[0].Value;

				if (isNotice)
				{
					using (CenterServiceClient client = new CenterServiceClient())
					{
						client.MailNotice(mail.ReceiverID);
					}
				}
			}
			catch (Exception e)
			{
				if (log.IsErrorEnabled)
					log.Error("Init", e);
			}
			finally
			{
			}
			return result;

		}
		public bool ActivePlayer(ref PlayerInfo player, string userName, string passWord, bool sex, int gold, int money, string IP, string site)
		{
			bool flag = false;
			try
			{
				player = new PlayerInfo();
				player.Agility = 0;
				player.Attack = 0;
				player.Colors = ",,,,,,";
				player.Skin = "";
				player.ConsortiaID = 0;
				player.Defence = 0;
				player.Gold = 0;
				player.GP = 1;
				player.Grade = 1;
				player.ID = 0;
				player.Luck = 0;
				player.Money = 0;
				player.NickName = "";
				player.Sex = sex;
				player.State = 0;
				player.Style = ",,,,,,";
				player.Hide = 1111111111;
				SqlParameter[] sqlParameters = new SqlParameter[21];
				sqlParameters[0] = new SqlParameter("@UserID", SqlDbType.Int);
				sqlParameters[0].Direction = ParameterDirection.Output;
				sqlParameters[1] = new SqlParameter("@Attack", player.Attack);
				sqlParameters[2] = new SqlParameter("@Colors", (player.Colors == null) ? "" : player.Colors);
				sqlParameters[3] = new SqlParameter("@ConsortiaID", player.ConsortiaID);
				sqlParameters[4] = new SqlParameter("@Defence", player.Defence);
				sqlParameters[5] = new SqlParameter("@Gold", player.Gold);
				sqlParameters[6] = new SqlParameter("@GP", player.GP);
				sqlParameters[7] = new SqlParameter("@Grade", player.Grade);
				sqlParameters[8] = new SqlParameter("@Luck", player.Luck);
				sqlParameters[9] = new SqlParameter("@Money", player.Money);
				sqlParameters[10] = new SqlParameter("@Style", (player.Style == null) ? "" : player.Style);
				sqlParameters[11] = new SqlParameter("@Agility", player.Agility);
				sqlParameters[12] = new SqlParameter("@State", player.State);
				sqlParameters[13] = new SqlParameter("@UserName", userName);
				sqlParameters[14] = new SqlParameter("@PassWord", passWord);
				sqlParameters[15] = new SqlParameter("@Sex", sex);
				sqlParameters[16] = new SqlParameter("@Hide", player.Hide);
				sqlParameters[17] = new SqlParameter("@ActiveIP", IP);
				sqlParameters[18] = new SqlParameter("@Skin", (player.Skin == null) ? "" : player.Skin);
				sqlParameters[19] = new SqlParameter("@Result", SqlDbType.Int);
				sqlParameters[19].Direction = ParameterDirection.ReturnValue;
				sqlParameters[20] = new SqlParameter("@Site", site);
				flag = db.RunProcedure("SP_Users_Active", sqlParameters);
				player.ID = (int)sqlParameters[0].Value;
				return flag;
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("Init", exception);
					return flag;
				}
				return flag;
			}
		}

		public bool AddAuction(AuctionInfo info)
		{
			bool flag = false;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[18]
				{
				new SqlParameter("@AuctionID", info.AuctionID),
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null
				};
				sqlParameters[0].Direction = ParameterDirection.Output;
				sqlParameters[1] = new SqlParameter("@AuctioneerID", info.AuctioneerID);
				sqlParameters[2] = new SqlParameter("@AuctioneerName", (info.AuctioneerName == null) ? "" : info.AuctioneerName);
				sqlParameters[3] = new SqlParameter("@BeginDate", info.BeginDate);//.ToString("MM/dd/yyyy HH:mm:ss"));
				sqlParameters[4] = new SqlParameter("@BuyerID", info.BuyerID);
				sqlParameters[5] = new SqlParameter("@BuyerName", (info.BuyerName == null) ? "" : info.BuyerName);
				sqlParameters[6] = new SqlParameter("@IsExist", info.IsExist);
				sqlParameters[7] = new SqlParameter("@ItemID", info.ItemID);
				sqlParameters[8] = new SqlParameter("@Mouthful", info.Mouthful);
				sqlParameters[9] = new SqlParameter("@PayType", info.PayType);
				sqlParameters[10] = new SqlParameter("@Price", info.Price);
				sqlParameters[11] = new SqlParameter("@Rise", info.Rise);
				sqlParameters[12] = new SqlParameter("@ValidDate", info.ValidDate);
				sqlParameters[13] = new SqlParameter("@TemplateID", info.TemplateID);
				sqlParameters[14] = new SqlParameter("Name", info.Name);
				sqlParameters[15] = new SqlParameter("Category", info.Category);
				sqlParameters[16] = new SqlParameter("Random", info.Random);
				sqlParameters[17] = new SqlParameter("goodsCount", info.goodsCount);
				flag = db.RunProcedure("SP_Auction_Add", sqlParameters);
				info.AuctionID = (int)sqlParameters[0].Value;
				return flag;
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("Init", exception);
					return flag;
				}
				return flag;
			}
		}

		public bool AddCards(UsersCardInfo item)
		{
			bool flag = false;
			try
			{
				SqlParameter[] SqlParameters = new SqlParameter[19]
				{
				new SqlParameter("@CardID", item.CardID),
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null
				};
				SqlParameters[0].Direction = ParameterDirection.Output;
				SqlParameters[1] = new SqlParameter("@UserID", item.UserID);
				SqlParameters[2] = new SqlParameter("@TemplateID", item.TemplateID);
				SqlParameters[3] = new SqlParameter("@Place", item.Place);
				SqlParameters[4] = new SqlParameter("@Count", item.Count);
				SqlParameters[5] = new SqlParameter("@Attack", item.Attack);
				SqlParameters[6] = new SqlParameter("@Defence", item.Defence);
				SqlParameters[7] = new SqlParameter("@Agility", item.Agility);
				SqlParameters[8] = new SqlParameter("@Luck", item.Luck);
				SqlParameters[9] = new SqlParameter("@Guard", item.Guard);
				SqlParameters[10] = new SqlParameter("@Damage", item.Damage);
				SqlParameters[11] = new SqlParameter("@Level", item.Level);
				SqlParameters[12] = new SqlParameter("@CardGP", item.CardGP);
				SqlParameters[14] = new SqlParameter("@isFirstGet", item.isFirstGet);
				SqlParameters[15] = new SqlParameter("@AttackReset", item.AttackReset);
				SqlParameters[16] = new SqlParameter("@DefenceReset", item.DefenceReset);
				SqlParameters[17] = new SqlParameter("@AgilityReset", item.AgilityReset);
				SqlParameters[18] = new SqlParameter("@LuckReset", item.LuckReset);
				SqlParameters[13] = new SqlParameter("@Result", SqlDbType.Int);
				SqlParameters[13].Direction = ParameterDirection.ReturnValue;
				db.RunProcedure("SP_UserCard_Add", SqlParameters);
				flag = (int)SqlParameters[13].Value == 0;
				item.CardID = (int)SqlParameters[0].Value;
				item.IsDirty = false;
				return flag;
			}
			catch (Exception ex)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("Init", ex);
					return flag;
				}
				return flag;
			}
		}

		public bool AddChargeMoney(string chargeID, string userName, int money, string payWay, decimal needMoney, ref int userID, ref int isResult, DateTime date, string IP, int UserID)
		{
			bool flag = false;
			userID = 0;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[10]
				{
				new SqlParameter("@ChargeID", chargeID),
				new SqlParameter("@UserName", userName),
				new SqlParameter("@Money", money),
				new SqlParameter("@Date", date.ToString("yyyy-MM-dd HH:mm:ss")),
				new SqlParameter("@PayWay", payWay),
				new SqlParameter("@NeedMoney", needMoney),
				new SqlParameter("@UserID", userID),
				null,
				null,
				null
				};
				sqlParameters[6].Direction = ParameterDirection.InputOutput;
				sqlParameters[7] = new SqlParameter("@Result", SqlDbType.Int);
				sqlParameters[7].Direction = ParameterDirection.ReturnValue;
				sqlParameters[8] = new SqlParameter("@IP", IP);
				sqlParameters[9] = new SqlParameter("@SourceUserID", UserID);
				flag = db.RunProcedure("SP_Charge_Money_UserId_Add", sqlParameters);
				userID = (int)sqlParameters[6].Value;
				isResult = (int)sqlParameters[7].Value;
				flag = isResult == 0;
				return flag;
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("Init", exception);
					return flag;
				}
				return flag;
			}
		}

		public bool AddFriends(FriendInfo info)
		{
			bool flag = false;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[7]
				{
				new SqlParameter("@ID", info.ID),
				new SqlParameter("@AddDate", DateTime.Now),
				new SqlParameter("@FriendID", info.FriendID),
				new SqlParameter("@IsExist", true),
				new SqlParameter("@Remark", (info.Remark == null) ? "" : info.Remark),
				new SqlParameter("@UserID", info.UserID),
				new SqlParameter("@Relation", info.Relation)
				};
				flag = db.RunProcedure("SP_Users_Friends_Add", sqlParameters);
				return flag;
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("Init", exception);
					return flag;
				}
				return flag;
			}
		}

		public bool AddGoods(ItemInfo item)
		{
			bool flag = false;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[41];
				sqlParameters[0] = new SqlParameter("@ItemID", item.ItemID);
				sqlParameters[0].Direction = ParameterDirection.Output;
				sqlParameters[1] = new SqlParameter("@UserID", item.UserID);
				sqlParameters[2] = new SqlParameter("@TemplateID", item.Template.TemplateID);
				sqlParameters[3] = new SqlParameter("@Place", item.Place);
				sqlParameters[4] = new SqlParameter("@AgilityCompose", item.AgilityCompose);
				sqlParameters[5] = new SqlParameter("@AttackCompose", item.AttackCompose);
				sqlParameters[6] = new SqlParameter("@BeginDate", item.BeginDate);
				sqlParameters[7] = new SqlParameter("@Color", (item.Color == null) ? "" : item.Color);
				sqlParameters[8] = new SqlParameter("@Count", item.Count);
				sqlParameters[9] = new SqlParameter("@DefendCompose", item.DefendCompose);
				sqlParameters[10] = new SqlParameter("@IsBinds", item.IsBinds);
				sqlParameters[11] = new SqlParameter("@IsExist", item.IsExist);
				sqlParameters[12] = new SqlParameter("@IsJudge", item.IsJudge);
				sqlParameters[13] = new SqlParameter("@LuckCompose", item.LuckCompose);
				sqlParameters[14] = new SqlParameter("@StrengthenLevel", item.StrengthenLevel);
				sqlParameters[15] = new SqlParameter("@ValidDate", item.ValidDate);
				sqlParameters[16] = new SqlParameter("@BagType", item.BagType);
				sqlParameters[17] = new SqlParameter("@Skin", (item.Skin == null) ? "" : item.Skin);
				sqlParameters[18] = new SqlParameter("@IsUsed", item.IsUsed);
				sqlParameters[19] = new SqlParameter("@RemoveType", item.RemoveType);
				sqlParameters[20] = new SqlParameter("@Hole1", item.Hole1);
				sqlParameters[21] = new SqlParameter("@Hole2", item.Hole2);
				sqlParameters[22] = new SqlParameter("@Hole3", item.Hole3);
				sqlParameters[23] = new SqlParameter("@Hole4", item.Hole4);
				sqlParameters[24] = new SqlParameter("@Hole5", item.Hole5);
				sqlParameters[25] = new SqlParameter("@Hole6", item.Hole6);
				sqlParameters[26] = new SqlParameter("@StrengthenTimes", item.StrengthenTimes);
				sqlParameters[27] = new SqlParameter("@Hole5Level", item.Hole5Level);
				sqlParameters[28] = new SqlParameter("@Hole5Exp", item.Hole5Exp);
				sqlParameters[29] = new SqlParameter("@Hole6Level", item.Hole6Level);
				sqlParameters[30] = new SqlParameter("@Hole6Exp", item.Hole6Exp);
				sqlParameters[31] = new SqlParameter("@IsGold", item.IsGold);
				sqlParameters[32] = new SqlParameter("@goldValidDate", item.goldValidDate);
				sqlParameters[33] = new SqlParameter("@goldBeginTime", item.goldBeginTime);
				sqlParameters[34] = new SqlParameter("@StrengthenExp", item.StrengthenExp);
				sqlParameters[35] = new SqlParameter("@Blood", item.Blood);
				sqlParameters[36] = new SqlParameter("@latentEnergyCurStr", item.latentEnergyCurStr);
				sqlParameters[37] = new SqlParameter("@latentEnergyNewStr", item.latentEnergyNewStr);
				sqlParameters[38] = new SqlParameter("@latentEnergyEndTime", item.latentEnergyEndTime);
				sqlParameters[39] = new SqlParameter("@cellLocked", item.cellLocked);
				sqlParameters[40] = new SqlParameter("@curExp", item.curExp);
				flag = db.RunProcedure("SP_Users_Items_Add", sqlParameters);
				item.ItemID = (int)sqlParameters[0].Value;
				item.IsDirty = false;
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("Init", exception);
				}
			}
			return flag;
		}

		public bool AddMarryInfo(MarryInfo info)
		{
			bool flag = false;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[5]
				{
				new SqlParameter("@ID", info.ID),
				null,
				null,
				null,
				null
				};
				sqlParameters[0].Direction = ParameterDirection.Output;
				sqlParameters[1] = new SqlParameter("@UserID", info.UserID);
				sqlParameters[2] = new SqlParameter("@IsPublishEquip", info.IsPublishEquip);
				sqlParameters[3] = new SqlParameter("@Introduction", info.Introduction);
				sqlParameters[4] = new SqlParameter("@RegistTime", info.RegistTime);
				flag = db.RunProcedure("SP_MarryInfo_Add", sqlParameters);
				info.ID = (int)sqlParameters[0].Value;
				return flag;
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("AddMarryInfo", exception);
					return flag;
				}
				return flag;
			}
		}

		public bool AddUserMatchInfo(UserMatchInfo info)
		{
			bool flag = false;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[16]
				{
				new SqlParameter("@ID", info.ID),
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null
				};
				sqlParameters[0].Direction = ParameterDirection.Output;
				sqlParameters[1] = new SqlParameter("@UserID", info.UserID);
				sqlParameters[2] = new SqlParameter("@dailyScore", info.dailyScore);
				sqlParameters[3] = new SqlParameter("@dailyWinCount", info.dailyWinCount);
				sqlParameters[4] = new SqlParameter("@dailyGameCount", info.dailyGameCount);
				sqlParameters[5] = new SqlParameter("@DailyLeagueFirst", info.DailyLeagueFirst);
				sqlParameters[6] = new SqlParameter("@DailyLeagueLastScore", info.DailyLeagueLastScore);
				sqlParameters[7] = new SqlParameter("@weeklyScore", info.weeklyScore);
				sqlParameters[8] = new SqlParameter("@weeklyGameCount", info.weeklyGameCount);
				sqlParameters[9] = new SqlParameter("@weeklyRanking", info.weeklyRanking);
				sqlParameters[10] = new SqlParameter("@addDayPrestge", info.addDayPrestge);
				sqlParameters[11] = new SqlParameter("@totalPrestige", info.totalPrestige);
				sqlParameters[12] = new SqlParameter("@restCount", info.restCount);
				sqlParameters[13] = new SqlParameter("@leagueGrade", info.leagueGrade);
				sqlParameters[14] = new SqlParameter("@leagueItemsGet", info.leagueItemsGet);
				sqlParameters[15] = new SqlParameter("@Result", SqlDbType.Int);
				sqlParameters[15].Direction = ParameterDirection.ReturnValue;
				db.RunProcedure("SP_UserMatch_Add", sqlParameters);
				flag = (int)sqlParameters[15].Value == 0;
				info.ID = (int)sqlParameters[0].Value;
				info.IsDirty = false;
				return flag;
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("Init", exception);
					return flag;
				}
				return flag;
			}
		}

		public bool AddUserRank(UserRankInfo item)
		{
			bool result = false;
			try
			{
				SqlParameter[] para = new SqlParameter[16];
				para[0] = new SqlParameter("@ID", item.ID);
				para[0].Direction = ParameterDirection.Output;
				para[1] = new SqlParameter("@UserID", item.UserID);
				para[2] = new SqlParameter("@Name", item.Name);
				para[3] = new SqlParameter("@Attack", item.Attack);
				para[4] = new SqlParameter("@Defence", item.Defence);
				para[5] = new SqlParameter("@Luck", item.Luck);
				para[6] = new SqlParameter("@Agility", item.Agility);
				para[7] = new SqlParameter("@HP", item.HP);
				para[8] = new SqlParameter("@Damage", item.Damage);
				para[9] = new SqlParameter("@Guard", item.Guard);
				para[10] = new SqlParameter("@BeginDate", item.BeginDate);//.ToString("MM/dd/yyyy HH:mm:ss"));
				para[11] = new SqlParameter("@Validate", item.Validate);
				para[12] = new SqlParameter("@IsExit", item.IsExit);
				para[13] = new SqlParameter("@Result", SqlDbType.Int);
				para[13].Direction = ParameterDirection.ReturnValue;
				para[14] = new SqlParameter("@NewTitleID", item.NewTitleID);
				para[15] = new SqlParameter("@EndDate", item.EndDate);
				db.RunProcedure("SP_UserRank_Add", para);
				result = (int)para[13].Value == 0;
				item.ID = (int)para[0].Value;
				item.IsDirty = false;
			}
			catch (Exception e)
			{
				if (log.IsErrorEnabled)
					log.Error("Init", e);
			}

			return result;
		}

		public bool CancelPaymentMail(int userid, int mailID, ref int senderID)
		{
			bool flag = false;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[4]
				{
				new SqlParameter("@userid", userid),
				new SqlParameter("@mailID", mailID),
				new SqlParameter("@senderID", SqlDbType.Int),
				null
				};
				sqlParameters[2].Value = senderID;
				sqlParameters[2].Direction = ParameterDirection.InputOutput;
				sqlParameters[3] = new SqlParameter("@Result", SqlDbType.Int);
				sqlParameters[3].Direction = ParameterDirection.ReturnValue;
				db.RunProcedure("SP_Mail_PaymentCancel", sqlParameters);
				flag = (int)sqlParameters[3].Value == 0;
				if (flag)
				{
					senderID = (int)sqlParameters[2].Value;
					return flag;
				}
				return flag;
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("Init", exception);
					return flag;
				}
				return flag;
			}
		}

		public bool ChargeToUser(string userName, ref int money, string nickName)
		{
			bool flag = false;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[3]
				{
				new SqlParameter("@UserName", userName),
				new SqlParameter("@money", SqlDbType.Int),
				null
				};
				sqlParameters[1].Direction = ParameterDirection.Output;
				sqlParameters[2] = new SqlParameter("@NickName", nickName);
				flag = db.RunProcedure("SP_Charge_To_User", sqlParameters);
				money = (int)sqlParameters[1].Value;
				return flag;
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("Init", exception);
					return flag;
				}
				return flag;
			}
		}

		public bool DeleteAuction(int auctionID, int userID, ref string msg)
		{
			bool flag = false;
			try
			{
				SqlParameter[] SqlParameters = new SqlParameter[3]
				{
				new SqlParameter("@AuctionID", auctionID),
				new SqlParameter("@UserID", userID),
				new SqlParameter("@Result", SqlDbType.Int)
				};
				SqlParameters[2].Direction = ParameterDirection.ReturnValue;
				db.RunProcedure("SP_Auction_Delete", SqlParameters);
				int num = (int)SqlParameters[2].Value;
				flag = num == 0;
				switch (num)
				{
					case 0:
						msg = LanguageMgr.GetTranslation("PlayerBussiness.DeleteAuction.Msg1");
						return flag;
					case 1:
						msg = LanguageMgr.GetTranslation("PlayerBussiness.DeleteAuction.Msg2");
						return flag;
					case 2:
						msg = LanguageMgr.GetTranslation("PlayerBussiness.DeleteAuction.Msg3");
						return flag;
					default:
						msg = LanguageMgr.GetTranslation("PlayerBussiness.DeleteAuction.Msg4");
						return flag;
				}
			}
			catch (Exception ex)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("Init", ex);
					return flag;
				}
				return flag;
			}
		}

		public bool DeleteFriends(int UserID, int FriendID)
		{
			bool flag = false;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[2]
				{
				new SqlParameter("@ID", FriendID),
				new SqlParameter("@UserID", UserID)
				};
				flag = db.RunProcedure("SP_Users_Friends_Delete", sqlParameters);
				return flag;
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("Init", exception);
					return flag;
				}
				return flag;
			}
		}

		public bool DeleteMail(int UserID, int mailID, out int senderID)
		{
			bool flag = false;
			senderID = 0;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[4]
				{
				new SqlParameter("@ID", mailID),
				new SqlParameter("@UserID", UserID),
				new SqlParameter("@SenderID", SqlDbType.Int),
				null
				};
				sqlParameters[2].Value = senderID;
				sqlParameters[2].Direction = ParameterDirection.InputOutput;
				sqlParameters[3] = new SqlParameter("@Result", SqlDbType.Int);
				sqlParameters[3].Direction = ParameterDirection.ReturnValue;
				flag = db.RunProcedure("SP_Mail_Delete", sqlParameters);
				if ((int)sqlParameters[3].Value == 0)
				{
					flag = true;
					senderID = (int)sqlParameters[2].Value;
					return flag;
				}
				return flag;
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("Init", exception);
					return flag;
				}
				return flag;
			}
		}

		public bool DeleteMarryInfo(int ID, int userID, ref string msg)
		{
			bool flag = false;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[3]
				{
				new SqlParameter("@ID", ID),
				new SqlParameter("@UserID", userID),
				new SqlParameter("@Result", SqlDbType.Int)
				};
				sqlParameters[2].Direction = ParameterDirection.ReturnValue;
				db.RunProcedure("SP_MarryInfo_Delete", sqlParameters);
				int num = (int)sqlParameters[2].Value;
				flag = num == 0;
				if (num == 0)
				{
					msg = LanguageMgr.GetTranslation("PlayerBussiness.DeleteAuction.Succeed");
					return flag;
				}
				return flag;
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("DeleteAuction", exception);
					return flag;
				}
				return flag;
			}
		}

		public bool DisposeMarryRoomInfo(int ID)
		{
			bool flag = false;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[2]
				{
				new SqlParameter("@ID", ID),
				new SqlParameter("@Result", SqlDbType.Int)
				};
				sqlParameters[1].Direction = ParameterDirection.ReturnValue;
				db.RunProcedure("SP_Dispose_Marry_Room_Info", sqlParameters);
				flag = (int)sqlParameters[1].Value == 0;
				return flag;
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("DisposeMarryRoomInfo", exception);
					return flag;
				}
				return flag;
			}
		}

		public ConsortiaUserInfo[] GetAllMemberByConsortia(int ConsortiaID)
		{
			List<ConsortiaUserInfo> list = new List<ConsortiaUserInfo>();
			SqlDataReader ResultDataReader = null;
			try
			{
				SqlParameter[] SqlParameters = new SqlParameter[1]
				{
				new SqlParameter("@ConsortiaID", SqlDbType.Int, 4)
				};
				SqlParameters[0].Value = ConsortiaID;
				db.GetReader(ref ResultDataReader, "SP_Consortia_Users_All", SqlParameters);
				while (ResultDataReader.Read())
				{
					list.Add(InitConsortiaUserInfo(ResultDataReader));
				}
			}
			catch (Exception ex)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("Init", ex);
				}
			}
			finally
			{
				if (ResultDataReader != null && !ResultDataReader.IsClosed)
				{
					ResultDataReader.Close();
				}
			}
			return list.ToArray();
		}

		public UserMatchInfo[] GetAllUserMatchInfo()
		{
			List<UserMatchInfo> list = new List<UserMatchInfo>();
			SqlDataReader resultDataReader = null;
			int num = 1;
			try
			{
				db.GetReader(ref resultDataReader, "SP_UserMatch_All_DESC");
				while (resultDataReader.Read())
				{
					UserMatchInfo item = new UserMatchInfo
					{
						UserID = (int)resultDataReader["UserID"],
						totalPrestige = (int)resultDataReader["totalPrestige"],
						rank = num
					};
					list.Add(item);
					num++;
				}
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("GetAllUserMatchDESC", exception);
				}
			}
			finally
			{
				if (resultDataReader != null && !resultDataReader.IsClosed)
				{
					resultDataReader.Close();
				}
			}
			return list.ToArray();
		}

		public AuctionInfo[] GetAuctionPage(int page, string name, int type, int pay, ref int total, int userID, int buyID, int order, bool sort, int size, string string_1)
		{
			List<AuctionInfo> auctionInfoList = new List<AuctionInfo>();
			try
			{
				string str1 = " IsExist=1 ";
				if (!string.IsNullOrEmpty(name))
				{
					str1 = str1 + " and Name like '%" + name + "%' ";
				}
				switch (type)
				{
					case 0:
					case 1:
					case 2:
					case 3:
					case 4:
					case 5:
					case 6:
					case 7:
					case 8:
					case 9:
					case 10:
					case 11:
					case 12:
					case 13:
					case 14:
					case 15:
					case 16:
					case 17:
					case 19:
						str1 = str1 + " and Category =" + type + " ";
						break;
					case 21:
						str1 += " and Category in(1,2,5,8,9) ";
						break;
					case 22:
						str1 += " and Category in(13,15,6,4,3) ";
						break;
					case 23:
						str1 += " and Category in(16,11,10) ";
						break;
					case 24:
						str1 += " and Category in(8,9) ";
						break;
					case 25:
						str1 += " and Category in (7,17) ";
						break;
					case 26:
						str1 += " and TemplateId>=311000 and TemplateId<=313999";
						break;
					case 27:
						str1 += " and TemplateId>=311000 and TemplateId<=311999 ";
						break;
					case 28:
						str1 += " and TemplateId>=312000 and TemplateId<=312999 ";
						break;
					case 29:
						str1 += " and TemplateId>=313000 and TempLateId<=313999";
						break;
					case 35:
						str1 += " and TemplateID in (11560,11561,11562)";
						break;
					case 1100:
						str1 += " and TemplateID in (11019,11021,11022,11023) ";
						break;
					case 1101:
						str1 += " and TemplateID='11019' ";
						break;
					case 1102:
						str1 += " and TemplateID='11021' ";
						break;
					case 1103:
						str1 += " and TemplateID='11022' ";
						break;
					case 1104:
						str1 += " and TemplateID='11023' ";
						break;
					case 1105:
						str1 += " and TemplateID in (11001,11002,11003,11004,11005,11006,11007,11008,11009,11010,11011,11012,11013,11014,11015,11016) ";
						break;
					case 1106:
						str1 += " and TemplateID in (11001,11002,11003,11004) ";
						break;
					case 1107:
						str1 += " and TemplateID in (11005,11006,11007,11008) ";
						break;
					case 1108:
						str1 += " and TemplateID in (11009,11010,11011,11012) ";
						break;
					case 1109:
						str1 += " and TemplateID in (11013,11014,11015,11016) ";
						break;
					case 1110:
						str1 += " and TemplateID='11024' ";
						break;
					case 1111:
						str1 += "and TemplateID in (11039,11041,11043,11047,11040,11042,11044,11048)";
						break;
					case 1112:
						str1 += "and TemplateID in (11037,11038,11045,11046)";
						break;
					case 1113:
						str1 += " and TemplateID in (314101,314102,314103,314104,314105,314106,314107,314108,314109,314110,314111,314112,314113,314114,314115,314116,314121,314122,314123,314124,314125,314126,314127,314128,314129,314130,314131,314132,314133,314134) ";
						break;
					case 1114:
						str1 += " and TemplateID in (314117,314118,314119,314120,314135,314136,314137,314138,314139) ";
						break;
					case 1116:
						str1 += " and TemplateID='11035' ";
						break;
					case 1117:
						str1 += " and TemplateID='11036' ";
						break;
					case 1118:
						str1 += " and TemplateID='11026' ";
						break;
					case 1119:
						str1 += " and TemplateID='11027' ";
						break;
				}
				if (pay != -1)
				{
					str1 = str1 + " and PayType =" + pay + " ";
				}
				if (userID != -1)
				{
					str1 = str1 + " and AuctioneerID =" + userID + " ";
				}
				if (buyID != -1)
				{
					str1 = str1 + " and (BuyerID =" + buyID + " or AuctionID in (" + string_1 + ")) ";
				}
				string str2 = "Category,Name,Price,dd,AuctioneerID";
				switch (order)
				{
					case 0:
						str2 = "Name";
						break;
					case 2:
						str2 = "dd";
						break;
					case 3:
						str2 = "AuctioneerName";
						break;
					case 4:
						str2 = "Price";
						break;
					case 5:
						str2 = "BuyerName";
						break;
				}
				string str3 = str2 + (sort ? " desc" : "") + ",AuctionID ";
				SqlParameter[] SqlParameters = new SqlParameter[8]
				{
				new SqlParameter("@QueryStr", "V_Auction_Scan"),
				new SqlParameter("@QueryWhere", str1),
				new SqlParameter("@PageSize", size),
				new SqlParameter("@PageCurrent", page),
				new SqlParameter("@FdShow", "*"),
				new SqlParameter("@FdOrder", str3),
				new SqlParameter("@FdKey", "AuctionID"),
				new SqlParameter("@TotalRow", total)
				};
				SqlParameters[7].Direction = ParameterDirection.Output;
				DataTable dataTable = db.GetDataTable("Auction", "SP_CustomPage", SqlParameters);
				total = (int)SqlParameters[7].Value;
				foreach (DataRow row in dataTable.Rows)
				{
					auctionInfoList.Add(new AuctionInfo
					{
						AuctioneerID = (int)row["AuctioneerID"],
						AuctioneerName = row["AuctioneerName"].ToString(),
						AuctionID = (int)row["AuctionID"],
						BeginDate = (DateTime)row["BeginDate"],
						BuyerID = (int)row["BuyerID"],
						BuyerName = row["BuyerName"].ToString(),
						Category = (int)row["Category"],
						IsExist = (bool)row["IsExist"],
						ItemID = (int)row["ItemID"],
						Name = row["Name"].ToString(),
						Mouthful = (int)row["Mouthful"],
						PayType = (int)row["PayType"],
						Price = (int)row["Price"],
						Rise = (int)row["Rise"],
						ValidDate = (int)row["ValidDate"]
					});
				}
			}
			catch (Exception ex)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("Init", ex);
				}
			}
			return auctionInfoList.ToArray();
		}

		public AuctionInfo GetAuctionSingle(int auctionID)
		{
			SqlDataReader resultDataReader = null;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[1]
				{
				new SqlParameter("@AuctionID", auctionID)
				};
				db.GetReader(ref resultDataReader, "SP_Auction_Single", sqlParameters);
				if (resultDataReader.Read())
				{
					return InitAuctionInfo(resultDataReader);
				}
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("Init", exception);
				}
			}
			finally
			{
				if (resultDataReader != null && !resultDataReader.IsClosed)
				{
					resultDataReader.Close();
				}
			}
			return null;
		}

		public BestEquipInfo[] GetCelebByDayBestEquip()
		{
			List<BestEquipInfo> list = new List<BestEquipInfo>();
			SqlDataReader resultDataReader = null;
			try
			{
				db.GetReader(ref resultDataReader, "SP_Users_BestEquip");
				while (resultDataReader.Read())
				{
					BestEquipInfo item = new BestEquipInfo
					{
						Date = (DateTime)resultDataReader["RemoveDate"],
						GP = (int)resultDataReader["GP"],
						Grade = (int)resultDataReader["Grade"],
						ItemName = ((resultDataReader["Name"] == null) ? "" : resultDataReader["Name"].ToString()),
						NickName = ((resultDataReader["NickName"] == null) ? "" : resultDataReader["NickName"].ToString()),
						Sex = (bool)resultDataReader["Sex"],
						Strengthenlevel = (int)resultDataReader["Strengthenlevel"],
						UserName = ((resultDataReader["UserName"] == null) ? "" : resultDataReader["UserName"].ToString())
					};
					list.Add(item);
				}
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("Init", exception);
				}
			}
			finally
			{
				if (resultDataReader != null && !resultDataReader.IsClosed)
				{
					resultDataReader.Close();
				}
			}
			return list.ToArray();
		}

		public FriendInfo[] GetFriendsAll(int UserID)
		{
			List<FriendInfo> friendInfoList = new List<FriendInfo>();
			SqlDataReader ResultDataReader = null;
			try
			{
				SqlParameter[] SqlParameters = new SqlParameter[1]
				{
				new SqlParameter("@UserID", SqlDbType.Int, 4)
				};
				SqlParameters[0].Value = UserID;
				db.GetReader(ref ResultDataReader, "SP_Users_Friends", SqlParameters);
				while (ResultDataReader.Read())
				{
					friendInfoList.Add(new FriendInfo
					{
						AddDate = (DateTime)ResultDataReader["AddDate"],
						Colors = ((ResultDataReader["Colors"] == null) ? "" : ResultDataReader["Colors"].ToString()),
						FriendID = (int)ResultDataReader["FriendID"],
						Grade = (int)ResultDataReader["Grade"],
						Hide = (int)ResultDataReader["Hide"],
						ID = (int)ResultDataReader["ID"],
						IsExist = (bool)ResultDataReader["IsExist"],
						NickName = ((ResultDataReader["NickName"] == null) ? "" : ResultDataReader["NickName"].ToString()),
						Remark = ((ResultDataReader["Remark"] == null) ? "" : ResultDataReader["Remark"].ToString()),
						Sex = (((bool)ResultDataReader["Sex"]) ? 1 : 0),
						State = (int)ResultDataReader["State"],
						Style = ((ResultDataReader["Style"] == null) ? "" : ResultDataReader["Style"].ToString()),
						UserID = (int)ResultDataReader["UserID"],
						ConsortiaName = ((ResultDataReader["ConsortiaName"] == null) ? "" : ResultDataReader["ConsortiaName"].ToString()),
						Offer = (int)ResultDataReader["Offer"],
						Win = (int)ResultDataReader["Win"],
						Total = (int)ResultDataReader["Total"],
						Escape = (int)ResultDataReader["Escape"],
						Relation = (int)ResultDataReader["Relation"],
						Repute = (int)ResultDataReader["Repute"],
						UserName = ((ResultDataReader["UserName"] == null) ? "" : ResultDataReader["UserName"].ToString()),
						DutyName = ((ResultDataReader["DutyName"] == null) ? "" : ResultDataReader["DutyName"].ToString()),
						Nimbus = (int)ResultDataReader["Nimbus"],
						apprenticeshipState = (int)ResultDataReader["apprenticeshipState"]
					});
				}
			}
			catch (Exception ex)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("Init", ex);
				}
			}
			finally
			{
				if (ResultDataReader != null && !ResultDataReader.IsClosed)
				{
					ResultDataReader.Close();
				}
			}
			return friendInfoList.ToArray();
		}

		public FriendInfo[] GetFriendsBbs(string condictArray)
		{
			List<FriendInfo> list = new List<FriendInfo>();
			SqlDataReader resultDataReader = null;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[1]
				{
				new SqlParameter("@SearchUserName", SqlDbType.NVarChar, 4000)
				};
				sqlParameters[0].Value = condictArray;
				db.GetReader(ref resultDataReader, "SP_Users_FriendsBbs", sqlParameters);
				while (resultDataReader.Read())
				{
					FriendInfo item = new FriendInfo
					{
						NickName = ((resultDataReader["NickName"] == null) ? "" : resultDataReader["NickName"].ToString()),
						UserID = (int)resultDataReader["UserID"],
						UserName = ((resultDataReader["UserName"] == null) ? "" : resultDataReader["UserName"].ToString()),
						IsExist = ((int)resultDataReader["UserID"] > 0)
					};
					list.Add(item);
				}
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("Init", exception);
				}
			}
			finally
			{
				if (resultDataReader != null && !resultDataReader.IsClosed)
				{
					resultDataReader.Close();
				}
			}
			return list.ToArray();
		}

		public ArrayList GetFriendsGood(string UserName)
		{
			ArrayList list = new ArrayList();
			SqlDataReader resultDataReader = null;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[1]
				{
				new SqlParameter("@UserName", SqlDbType.NVarChar)
				};
				sqlParameters[0].Value = UserName;
				db.GetReader(ref resultDataReader, "SP_Users_Friends_Good", sqlParameters);
				while (resultDataReader.Read())
				{
					list.Add((resultDataReader["UserName"] == null) ? "" : resultDataReader["UserName"].ToString());
				}
				return list;
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("Init", exception);
					return list;
				}
				return list;
			}
			finally
			{
				if (resultDataReader != null && !resultDataReader.IsClosed)
				{
					resultDataReader.Close();
				}
			}
		}

		public Dictionary<int, int> GetFriendsIDAll(int UserID)
		{
			Dictionary<int, int> dictionary = new Dictionary<int, int>();
			SqlDataReader resultDataReader = null;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[1]
				{
				new SqlParameter("@UserID", SqlDbType.Int, 4)
				};
				sqlParameters[0].Value = UserID;
				db.GetReader(ref resultDataReader, "SP_Users_Friends_All", sqlParameters);
				while (resultDataReader.Read())
				{
					if (!dictionary.ContainsKey((int)resultDataReader["FriendID"]))
					{
						dictionary.Add((int)resultDataReader["FriendID"], (int)resultDataReader["Relation"]);
					}
					else
					{
						dictionary[(int)resultDataReader["FriendID"]] = (int)resultDataReader["Relation"];
					}
				}
				return dictionary;
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("Init", exception);
					return dictionary;
				}
				return dictionary;
			}
			finally
			{
				if (resultDataReader != null && !resultDataReader.IsClosed)
				{
					resultDataReader.Close();
				}
			}
		}

		public MailInfo[] GetMailBySenderID(int userID)
		{
			List<MailInfo> list = new List<MailInfo>();
			SqlDataReader resultDataReader = null;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[1]
				{
				new SqlParameter("@UserID", SqlDbType.Int, 4)
				};
				sqlParameters[0].Value = userID;
				db.GetReader(ref resultDataReader, "SP_Mail_BySenderID", sqlParameters);
				while (resultDataReader.Read())
				{
					list.Add(InitMail(resultDataReader));
				}
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("Init", exception);
				}
			}
			finally
			{
				if (resultDataReader != null && !resultDataReader.IsClosed)
				{
					resultDataReader.Close();
				}
			}
			return list.ToArray();
		}

		public MailInfo[] GetMailByUserID(int userID)
		{
			List<MailInfo> list = new List<MailInfo>();
			SqlDataReader resultDataReader = null;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[1]
				{
				new SqlParameter("@UserID", SqlDbType.Int, 4)
				};
				sqlParameters[0].Value = userID;
				db.GetReader(ref resultDataReader, "SP_Mail_ByUserID", sqlParameters);
				while (resultDataReader.Read())
				{
					list.Add(InitMail(resultDataReader));
				}
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("Init", exception);
				}
			}
			finally
			{
				if (resultDataReader != null && !resultDataReader.IsClosed)
				{
					resultDataReader.Close();
				}
			}
			return list.ToArray();
		}

		public MailInfo GetMailSingle(int UserID, int mailID)
		{
			SqlDataReader resultDataReader = null;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[2]
				{
				new SqlParameter("@ID", mailID),
				new SqlParameter("@UserID", UserID)
				};
				db.GetReader(ref resultDataReader, "SP_Mail_Single", sqlParameters);
				if (resultDataReader.Read())
				{
					return InitMail(resultDataReader);
				}
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("Init", exception);
				}
			}
			finally
			{
				if (resultDataReader != null && !resultDataReader.IsClosed)
				{
					resultDataReader.Close();
				}
			}
			return null;
		}

		public MarryInfo[] GetMarryInfoPage(int page, string name, bool sex, int size, ref int total)
		{
			List<MarryInfo> list = new List<MarryInfo>();
			try
			{
				string str = ((!sex) ? " IsExist=1 and Sex=0 and UserExist=1" : " IsExist=1 and Sex=1 and UserExist=1");
				if (!string.IsNullOrEmpty(name))
				{
					str = str + " and NickName like '%" + name + "%' ";
				}
				string str2 = "State desc,IsMarried";
				SqlParameter[] sqlParameters = new SqlParameter[8]
				{
				new SqlParameter("@QueryStr", "V_Sys_Marry_Info"),
				new SqlParameter("@QueryWhere", str),
				new SqlParameter("@PageSize", size),
				new SqlParameter("@PageCurrent", page),
				new SqlParameter("@FdShow", "*"),
				new SqlParameter("@FdOrder", str2),
				new SqlParameter("@FdKey", "ID"),
				new SqlParameter("@TotalRow", total)
				};
				sqlParameters[7].Direction = ParameterDirection.Output;
				DataTable dataTable = db.GetDataTable("V_Sys_Marry_Info", "SP_CustomPage", sqlParameters);
				total = (int)sqlParameters[7].Value;
				foreach (DataRow row in dataTable.Rows)
				{
					MarryInfo item = new MarryInfo
					{
						ID = (int)row["ID"],
						UserID = (int)row["UserID"],
						IsPublishEquip = (bool)row["IsPublishEquip"],
						Introduction = row["Introduction"].ToString(),
						NickName = row["NickName"].ToString(),
						IsConsortia = (bool)row["IsConsortia"],
						ConsortiaID = (int)row["ConsortiaID"],
						Sex = (bool)row["Sex"],
						Win = (int)row["Win"],
						Total = (int)row["Total"],
						Escape = (int)row["Escape"],
						GP = (int)row["GP"],
						Honor = row["Honor"].ToString(),
						Style = row["Style"].ToString(),
						Colors = row["Colors"].ToString(),
						Hide = (int)row["Hide"],
						Grade = (int)row["Grade"],
						State = (int)row["State"],
						Repute = (int)row["Repute"],
						Skin = row["Skin"].ToString(),
						Offer = (int)row["Offer"],
						IsMarried = (bool)row["IsMarried"],
						ConsortiaName = row["ConsortiaName"].ToString(),
						DutyName = row["DutyName"].ToString(),
						Nimbus = (int)row["Nimbus"],
						FightPower = (int)row["FightPower"],
						typeVIP = Convert.ToByte(row["typeVIP"]),
						VIPLevel = (int)row["VIPLevel"]
					};
					list.Add(item);
				}
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("Init", exception);
				}
			}
			return list.ToArray();
		}

		public MarryInfo GetMarryInfoSingle(int ID)
		{
			SqlDataReader resultDataReader = null;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[1]
				{
				new SqlParameter("@ID", ID)
				};
				db.GetReader(ref resultDataReader, "SP_MarryInfo_Single", sqlParameters);
				if (resultDataReader.Read())
				{
					return new MarryInfo
					{
						ID = (int)resultDataReader["ID"],
						UserID = (int)resultDataReader["UserID"],
						IsPublishEquip = (bool)resultDataReader["IsPublishEquip"],
						Introduction = resultDataReader["Introduction"].ToString(),
						RegistTime = (DateTime)resultDataReader["RegistTime"]
					};
				}
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("GetMarryInfoSingle", exception);
				}
			}
			finally
			{
				if (resultDataReader != null && !resultDataReader.IsClosed)
				{
					resultDataReader.Close();
				}
			}
			return null;
		}

		public MarryProp GetMarryProp(int id)
		{
			SqlDataReader resultDataReader = null;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[1]
				{
				new SqlParameter("@UserID", id)
				};
				db.GetReader(ref resultDataReader, "SP_Select_Marry_Prop", sqlParameters);
				if (resultDataReader.Read())
				{
					return new MarryProp
					{
						IsMarried = (bool)resultDataReader["IsMarried"],
						SpouseID = (int)resultDataReader["SpouseID"],
						SpouseName = resultDataReader["SpouseName"].ToString(),
						IsCreatedMarryRoom = (bool)resultDataReader["IsCreatedMarryRoom"],
						SelfMarryRoomID = (int)resultDataReader["SelfMarryRoomID"],
						IsGotRing = (bool)resultDataReader["IsGotRing"]
					};
				}
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("GetMarryProp", exception);
				}
			}
			finally
			{
				if (resultDataReader != null && !resultDataReader.IsClosed)
				{
					resultDataReader.Close();
				}
			}
			return null;
		}

		public MarryRoomInfo[] GetMarryRoomInfo()
		{
			SqlDataReader resultDataReader = null;
			List<MarryRoomInfo> list = new List<MarryRoomInfo>();
			try
			{
				db.GetReader(ref resultDataReader, "SP_Get_Marry_Room_Info");
				while (resultDataReader.Read())
				{
					MarryRoomInfo item = new MarryRoomInfo
					{
						ID = (int)resultDataReader["ID"],
						Name = resultDataReader["Name"].ToString(),
						PlayerID = (int)resultDataReader["PlayerID"],
						PlayerName = resultDataReader["PlayerName"].ToString(),
						GroomID = (int)resultDataReader["GroomID"],
						GroomName = resultDataReader["GroomName"].ToString(),
						BrideID = (int)resultDataReader["BrideID"],
						BrideName = resultDataReader["BrideName"].ToString(),
						Pwd = resultDataReader["Pwd"].ToString(),
						AvailTime = (int)resultDataReader["AvailTime"],
						MaxCount = (int)resultDataReader["MaxCount"],
						GuestInvite = (bool)resultDataReader["GuestInvite"],
						MapIndex = (int)resultDataReader["MapIndex"],
						BeginTime = (DateTime)resultDataReader["BeginTime"],
						BreakTime = (DateTime)resultDataReader["BreakTime"],
						RoomIntroduction = resultDataReader["RoomIntroduction"].ToString(),
						ServerID = (int)resultDataReader["ServerID"],
						IsHymeneal = (bool)resultDataReader["IsHymeneal"],
						IsGunsaluteUsed = (bool)resultDataReader["IsGunsaluteUsed"]
					};
					list.Add(item);
				}
				return list.ToArray();
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("GetMarryRoomInfo", exception);
				}
			}
			finally
			{
				if (resultDataReader != null && !resultDataReader.IsClosed)
				{
					resultDataReader.Close();
				}
			}
			return null;
		}

		public MarryRoomInfo GetMarryRoomInfoSingle(int id)
		{
			SqlDataReader resultDataReader = null;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[1]
				{
				new SqlParameter("@ID", id)
				};
				db.GetReader(ref resultDataReader, "SP_Get_Marry_Room_Info_Single", sqlParameters);
				if (resultDataReader.Read())
				{
					return new MarryRoomInfo
					{
						ID = (int)resultDataReader["ID"],
						Name = resultDataReader["Name"].ToString(),
						PlayerID = (int)resultDataReader["PlayerID"],
						PlayerName = resultDataReader["PlayerName"].ToString(),
						GroomID = (int)resultDataReader["GroomID"],
						GroomName = resultDataReader["GroomName"].ToString(),
						BrideID = (int)resultDataReader["BrideID"],
						BrideName = resultDataReader["BrideName"].ToString(),
						Pwd = resultDataReader["Pwd"].ToString(),
						AvailTime = (int)resultDataReader["AvailTime"],
						MaxCount = (int)resultDataReader["MaxCount"],
						GuestInvite = (bool)resultDataReader["GuestInvite"],
						MapIndex = (int)resultDataReader["MapIndex"],
						BeginTime = (DateTime)resultDataReader["BeginTime"],
						BreakTime = (DateTime)resultDataReader["BreakTime"],
						RoomIntroduction = resultDataReader["RoomIntroduction"].ToString(),
						ServerID = (int)resultDataReader["ServerID"],
						IsHymeneal = (bool)resultDataReader["IsHymeneal"],
						IsGunsaluteUsed = (bool)resultDataReader["IsGunsaluteUsed"]
					};
				}
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("GetMarryRoomInfo", exception);
				}
			}
			finally
			{
				if (resultDataReader != null && !resultDataReader.IsClosed)
				{
					resultDataReader.Close();
				}
			}
			return null;
		}

		public void GetPasswordInfo(int userID, ref string PasswordQuestion1, ref string PasswordAnswer1, ref string PasswordQuestion2, ref string PasswordAnswer2, ref int Count)
		{
			SqlDataReader resultDataReader = null;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[1]
				{
				new SqlParameter("@UserID", userID)
				};
				db.GetReader(ref resultDataReader, "SP_Users_PasswordInfo", sqlParameters);
				while (resultDataReader.Read())
				{
					PasswordQuestion1 = ((resultDataReader["PasswordQuestion1"] == null) ? "" : resultDataReader["PasswordQuestion1"].ToString());
					PasswordAnswer1 = ((resultDataReader["PasswordAnswer1"] == null) ? "" : resultDataReader["PasswordAnswer1"].ToString());
					PasswordQuestion2 = ((resultDataReader["PasswordQuestion2"] == null) ? "" : resultDataReader["PasswordQuestion2"].ToString());
					PasswordAnswer2 = ((resultDataReader["PasswordAnswer2"] == null) ? "" : resultDataReader["PasswordAnswer2"].ToString());
					if ((DateTime)resultDataReader["LastFindDate"] == DateTime.Today)
					{
						Count = (int)resultDataReader["FailedPasswordAttemptCount"];
					}
					else
					{
						Count = 5;
					}
				}
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("Init", exception);
				}
			}
			finally
			{
				if (resultDataReader != null && !resultDataReader.IsClosed)
				{
					resultDataReader.Close();
				}
			}
		}

		public MarryApplyInfo[] GetPlayerMarryApply(int UserID)
		{
			SqlDataReader resultDataReader = null;
			List<MarryApplyInfo> list = new List<MarryApplyInfo>();
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[1]
				{
				new SqlParameter("@UserID", UserID)
				};
				db.GetReader(ref resultDataReader, "SP_Get_Marry_Apply", sqlParameters);
				while (resultDataReader.Read())
				{
					MarryApplyInfo item = new MarryApplyInfo
					{
						UserID = (int)resultDataReader["UserID"],
						ApplyUserID = (int)resultDataReader["ApplyUserID"],
						ApplyUserName = resultDataReader["ApplyUserName"].ToString(),
						ApplyType = (int)resultDataReader["ApplyType"],
						ApplyResult = (bool)resultDataReader["ApplyResult"],
						LoveProclamation = resultDataReader["LoveProclamation"].ToString(),
						ID = (int)resultDataReader["Id"]
					};
					list.Add(item);
				}
				return list.ToArray();
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("GetPlayerMarryApply", exception);
				}
			}
			finally
			{
				if (resultDataReader != null && !resultDataReader.IsClosed)
				{
					resultDataReader.Close();
				}
			}
			return null;
		}

		public PlayerInfo[] GetPlayerMathPage(int page, int size, ref int total, ref bool resultValue)
		{
			List<PlayerInfo> playerInfoList = new List<PlayerInfo>();
			try
			{
				string queryWhere = "  ";
				string fdOreder = "weeklyScore desc";
				foreach (DataRow row in GetPage("V_Sys_Users_Math", queryWhere, page, size, "*", fdOreder, "UserID", ref total).Rows)
				{
					PlayerInfo playerInfo = new PlayerInfo();
					playerInfo.ID = (int)row["UserID"];
					playerInfo.Colors = ((row["Colors"] == null) ? "" : row["Colors"].ToString());
					playerInfo.GP = (int)row["GP"];
					playerInfo.Grade = (int)row["Grade"];
					playerInfo.ID = (int)row["UserID"];
					playerInfo.NickName = ((row["NickName"] == null) ? "" : row["NickName"].ToString());
					playerInfo.Sex = (bool)row["Sex"];
					playerInfo.State = (int)row["State"];
					playerInfo.Style = ((row["Style"] == null) ? "" : row["Style"].ToString());
					playerInfo.Hide = (int)row["Hide"];
					playerInfo.Repute = (int)row["Repute"];
					playerInfo.UserName = ((row["UserName"] == null) ? "" : row["UserName"].ToString());
					playerInfo.Skin = ((row["Skin"] == null) ? "" : row["Skin"].ToString());
					playerInfo.Win = (int)row["Win"];
					playerInfo.Total = (int)row["Total"];
					playerInfo.Nimbus = (int)row["Nimbus"];
					playerInfo.FightPower = (int)row["FightPower"];
					playerInfo.AchievementPoint = (int)row["AchievementPoint"];
					playerInfo.typeVIP = Convert.ToByte(row["typeVIP"]);
					playerInfo.VIPLevel = (int)row["VIPLevel"];
					playerInfo.AddWeekLeagueScore = (int)row["weeklyScore"];
					playerInfoList.Add(playerInfo);
				}
				resultValue = true;
			}
			catch (Exception ex)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("Init", ex);
				}
			}
			return playerInfoList.ToArray();
		}

		public PlayerInfo[] GetPlayerPage(int page, int size, ref int total, int order, int userID, ref bool resultValue)
		{
			return GetPlayerPage(page, size, ref total, order, 0, userID, ref resultValue);
		}

		public PlayerInfo[] GetPlayerPage(int page, int size, ref int total, int order, int where, int userID, ref bool resultValue)
		{
			List<PlayerInfo> list = new List<PlayerInfo>();
			try
			{
				string queryWhere = " IsExist=1 and IsFirst<> 0 ";
				if (userID != -1)
				{
					queryWhere = queryWhere + " and UserID =" + userID + " ";
				}
				string str = "GP desc";
				switch (order)
				{
					case 1:
						str = "Offer desc";
						break;
					case 2:
						str = "AddDayGP desc";
						break;
					case 3:
						str = "AddWeekGP desc";
						break;
					case 4:
						str = "AddDayOffer desc";
						break;
					case 5:
						str = "AddWeekOffer desc";
						break;
					case 6:
						str = "FightPower desc";
						break;
					case 7:
						str = "EliteScore desc";
						break;
					case 8:
						str = "State desc, graduatesCount desc, FightPower desc";
						break;
					case 9:
						str = "NEWID()";
						break;
					case 10:
						str = "State desc, GP asc, FightPower desc";
						break;
					case 12:
						str = "charmGP desc";
						break;
					case 17:
						str = "DailyLeagueLastScore desc";
						break;
				}
				switch (where)
				{
					case 0:
						queryWhere += " ";
						break;
					case 1:
						queryWhere += " and Grade >= 20 ";
						break;
					case 2:
						queryWhere += " and Grade > 5 and Grade < 17 ";
						break;
					case 3:
						queryWhere += " and Grade >= 20 and apprenticeshipState != 3 and State = 1 ";
						break;
					case 4:
						queryWhere += " and Grade > 5 and Grade < 17 and masterID = 0 and State = 1 ";
						break;
				}
				string fdOreder = str + ",UserID";
				foreach (DataRow dataRow in GetPage("V_Sys_Users_Detail", queryWhere, page, size, "*", fdOreder, "UserID", ref total).Rows)
				{
					PlayerInfo playerInfo = new PlayerInfo();
					playerInfo.Agility = (int)dataRow["Agility"];
					playerInfo.Attack = (int)dataRow["Attack"];
					playerInfo.Colors = ((dataRow["Colors"] == null) ? "" : dataRow["Colors"].ToString());
					playerInfo.ConsortiaID = (int)dataRow["ConsortiaID"];
					playerInfo.Defence = (int)dataRow["Defence"];
					playerInfo.Gold = (int)dataRow["Gold"];
					playerInfo.GP = (int)dataRow["GP"];
					playerInfo.Grade = (int)dataRow["Grade"];
					playerInfo.ID = (int)dataRow["UserID"];
					playerInfo.Luck = (int)dataRow["Luck"];
					playerInfo.Money = (int)dataRow["Money"];
					playerInfo.NickName = ((dataRow["NickName"] == null) ? "" : dataRow["NickName"].ToString());
					playerInfo.Sex = (bool)dataRow["Sex"];
					playerInfo.State = (int)dataRow["State"];
					playerInfo.Style = ((dataRow["Style"] == null) ? "" : dataRow["Style"].ToString());
					playerInfo.Hide = (int)dataRow["Hide"];
					playerInfo.Repute = (int)dataRow["Repute"];
					playerInfo.UserName = ((dataRow["UserName"] == null) ? "" : dataRow["UserName"].ToString());
					playerInfo.ConsortiaName = ((dataRow["ConsortiaName"] == null) ? "" : dataRow["ConsortiaName"].ToString());
					playerInfo.Offer = (int)dataRow["Offer"];
					playerInfo.Skin = ((dataRow["Skin"] == null) ? "" : dataRow["Skin"].ToString());
					playerInfo.IsBanChat = (bool)dataRow["IsBanChat"];
					playerInfo.ReputeOffer = (int)dataRow["ReputeOffer"];
					playerInfo.ConsortiaRepute = (int)dataRow["ConsortiaRepute"];
					playerInfo.ConsortiaLevel = (int)dataRow["ConsortiaLevel"];
					playerInfo.StoreLevel = (int)dataRow["StoreLevel"];
					playerInfo.ShopLevel = (int)dataRow["ShopLevel"];
					playerInfo.SmithLevel = (int)dataRow["SmithLevel"];
					playerInfo.ConsortiaHonor = (int)dataRow["ConsortiaHonor"];
					playerInfo.RichesOffer = (int)dataRow["RichesOffer"];
					playerInfo.RichesRob = (int)dataRow["RichesRob"];
					playerInfo.DutyLevel = (int)dataRow["DutyLevel"];
					playerInfo.DutyName = ((dataRow["DutyName"] == null) ? "" : dataRow["DutyName"].ToString());
					playerInfo.Right = (int)dataRow["Right"];
					playerInfo.ChairmanName = ((dataRow["ChairmanName"] == null) ? "" : dataRow["ChairmanName"].ToString());
					playerInfo.Win = (int)dataRow["Win"];
					playerInfo.Total = (int)dataRow["Total"];
					playerInfo.Escape = (int)dataRow["Escape"];
					playerInfo.AddDayGP = (int)dataRow["AddDayGP"];//(((int)dataRow["AddDayGP"] == 0) ? playerInfo.GP : ((int)dataRow["AddDayGP"]));
					playerInfo.AddDayOffer = (int)dataRow["AddDayOffer"];//(((int)dataRow["AddDayOffer"] == 0) ? playerInfo.Offer : ((int)dataRow["AddDayOffer"]));
					playerInfo.AddWeekGP = (int)dataRow["AddWeekGP"];//(((int)dataRow["AddWeekGP"] == 0) ? playerInfo.GP : ((int)dataRow["AddWeekyGP"]));
					playerInfo.AddWeekOffer = (int)dataRow["AddWeekOffer"]; //(((int)dataRow["AddWeekOffer"] == 0) ? playerInfo.Offer : ((int)dataRow["AddWeekOffer"]));
					playerInfo.ConsortiaRiches = (int)dataRow["ConsortiaRiches"];
					playerInfo.CheckCount = (int)dataRow["CheckCount"];
					playerInfo.Nimbus = (int)dataRow["Nimbus"];
					playerInfo.GiftToken = (int)dataRow["GiftToken"];
					playerInfo.QuestSite = ((dataRow["QuestSite"] == null) ? new byte[200] : ((byte[])dataRow["QuestSite"]));
					playerInfo.PvePermission = ((dataRow["PvePermission"] == null) ? "" : dataRow["PvePermission"].ToString());
					playerInfo.FightLabPermission = ((dataRow["FightLabPermission"] == DBNull.Value) ? "" : dataRow["FightLabPermission"].ToString());
					playerInfo.FightPower = (int)dataRow["FightPower"];
					playerInfo.AchievementPoint = (int)dataRow["AchievementPoint"];
					playerInfo.Honor = (string)dataRow["Honor"];
					playerInfo.IsShowConsortia = (bool)dataRow["IsShowConsortia"];
					playerInfo.OptionOnOff = (int)dataRow["OptionOnOff"];
					playerInfo.badgeID = (int)dataRow["badgeID"];
					playerInfo.EliteScore = (int)dataRow["EliteScore"];
					playerInfo.apprenticeshipState = (int)dataRow["apprenticeshipState"];
					playerInfo.masterID = (int)dataRow["masterID"];
					playerInfo.graduatesCount = (int)dataRow["graduatesCount"];
					playerInfo.masterOrApprentices = ((dataRow["masterOrApprentices"] == DBNull.Value) ? "" : dataRow["masterOrApprentices"].ToString());
					playerInfo.honourOfMaster = ((dataRow["honourOfMaster"] == DBNull.Value) ? "" : dataRow["honourOfMaster"].ToString());
					playerInfo.IsMarried = (bool)dataRow["IsMarried"];
					playerInfo.typeVIP = Convert.ToByte(dataRow["typeVIP"]);
					playerInfo.VIPLevel = (int)dataRow["VIPLevel"];
					playerInfo.SpouseID = (int)dataRow["SpouseID"];
					playerInfo.SpouseName = ((dataRow["SpouseName"] == DBNull.Value) ? "" : dataRow["SpouseName"].ToString());
					playerInfo.charmGP = (int)dataRow["charmGP"];
					playerInfo.charmLevel = (int)dataRow["charmLevel"];
					playerInfo.DailyLeagueLastScore = (int)dataRow["DailyLeagueLastScore"];
					list.Add(playerInfo);
				}
				resultValue = true;
			}
			catch (Exception ex)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("Init", ex);
				}
			}
			return list.ToArray();
		}

		public UserMatchInfo GetSingleUserMatchInfo(int UserID)
		{
			SqlDataReader resultDataReader = null;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[1]
				{
				new SqlParameter("@UserID", SqlDbType.Int, 4)
				};
				sqlParameters[0].Value = UserID;
				db.GetReader(ref resultDataReader, "SP_GetSingleUserMatchInfo", sqlParameters);
				if (resultDataReader.Read())
				{
					return new UserMatchInfo
					{
						ID = (int)resultDataReader["ID"],
						UserID = (int)resultDataReader["UserID"],
						dailyScore = (int)resultDataReader["dailyScore"],
						dailyWinCount = (int)resultDataReader["dailyWinCount"],
						dailyGameCount = (int)resultDataReader["dailyGameCount"],
						DailyLeagueFirst = (bool)resultDataReader["DailyLeagueFirst"],
						DailyLeagueLastScore = (int)resultDataReader["DailyLeagueLastScore"],
						weeklyScore = (int)resultDataReader["weeklyScore"],
						weeklyGameCount = (int)resultDataReader["weeklyGameCount"],
						weeklyRanking = (int)resultDataReader["weeklyRanking"],
						addDayPrestge = (int)resultDataReader["addDayPrestge"],
						totalPrestige = (int)resultDataReader["totalPrestige"],
						restCount = (int)resultDataReader["restCount"],
						leagueGrade = (int)resultDataReader["leagueGrade"],
						leagueItemsGet = (int)resultDataReader["leagueItemsGet"]
					};
				}
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("SP_GetSingleUserMatchInfo", exception);
				}
			}
			finally
			{
				if (resultDataReader != null && !resultDataReader.IsClosed)
				{
					resultDataReader.Close();
				}
			}
			return null;
		}

		public List<UserRankInfo> GetSingleUserRank(int UserID)
		{
			SqlDataReader reader = null;
			List<UserRankInfo> infos = new List<UserRankInfo>();
			try
			{
				SqlParameter[] para = new SqlParameter[]
                {
					new SqlParameter("@UserID", SqlDbType.Int, 4)
                };
				//para[0] = new SqlParameter("@UserID", SqlDbType.Int, 4);
				para[0].Value = UserID;
				db.GetReader(ref reader, "SP_GetSingleUserRank", para);
				while (reader.Read())
				{
					UserRankInfo info = new UserRankInfo();
					info.ID = (int)reader["ID"];
					info.UserID = (int)reader["UserID"];
					info.Name = (string)reader["Name"];
					info.Attack = (int)reader["Attack"];
					info.Defence = (int)reader["Defence"];
					info.Luck = (int)reader["Luck"];
					info.Agility = (int)reader["Agility"];
					info.HP = (int)reader["HP"];
					info.Damage = (int)reader["Damage"];
					info.Guard = (int)reader["Guard"];
					info.BeginDate = (DateTime)reader["BeginDate"];
					info.Validate = (int)reader["Validate"];
					info.IsExit = (bool)reader["IsExit"];
					info.NewTitleID = (int)reader["NewTitleID"];
					info.EndDate = (DateTime)reader["EndDate"];
					infos.Add(info);
				}
			}
			catch (Exception e)
			{
				if (log.IsErrorEnabled)
					log.Error("SP_GetSingleUserRankInfo", e);
			}
			finally
			{
				if (reader != null && !reader.IsClosed)
					reader.Close();
			}

			return infos;
		}

		public UsersExtraInfo GetSingleUsersExtra(int UserID)
		{
			SqlDataReader resultDataReader = null;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[1]
				{
				new SqlParameter("@UserID", SqlDbType.Int, 4)
				};
				sqlParameters[0].Value = UserID;
				db.GetReader(ref resultDataReader, "SP_GetSingleUsersExtra", sqlParameters);
				if (resultDataReader.Read())
				{
					return new UsersExtraInfo
					{
						UserID = (int)resultDataReader["UserID"],
						LastTimeHotSpring = (DateTime)resultDataReader["LastTimeHotSpring"],
						LastFreeTimeHotSpring = (DateTime)resultDataReader["LastFreeTimeHotSpring"],
						MinHotSpring = (int)resultDataReader["MinHotSpring"],
						coupleBossEnterNum = (int)resultDataReader["coupleBossEnterNum"],
						coupleBossHurt = (int)resultDataReader["coupleBossHurt"],
						coupleBossBoxNum = (int)resultDataReader["coupleBossBoxNum"],
						LeftRoutteCount = ((resultDataReader["LeftRoutteCount"] == DBNull.Value) ? GameProperties.LeftRouterMaxDay : ((int)resultDataReader["LeftRoutteCount"])),
						LeftRoutteRate = ((resultDataReader["LeftRoutteRate"] == DBNull.Value) ? 0f : float.Parse(resultDataReader["LeftRoutteRate"].ToString())),
						FreeSendMailCount = (int)resultDataReader["FreeSendMailCount"],
						BuyTimeHotSpringCount = (int)resultDataReader["BuyTimeHotSpringCount"],
						BuyCountOpenBoss = (int)resultDataReader["BuyCountOpenBoss"],
						coupleNum = (int)resultDataReader["coupleNum"],
						propsNum = (int)resultDataReader["propsNum"],
						dungeonNum = (int)resultDataReader["dungeonNum"]
					};
				}
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("SP_GetSingleUsersExtra", exception);
				}
			}
			finally
			{
				if (resultDataReader != null && !resultDataReader.IsClosed)
				{
					resultDataReader.Close();
				}
			}
			return null;
		}

		public AchievementData[] GetUserAchievement(int userID)
		{
			List<AchievementData> list = new List<AchievementData>();
			SqlDataReader resultDataReader = null;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[1]
				{
				new SqlParameter("@UserID", SqlDbType.Int, 4)
				};
				sqlParameters[0].Value = userID;
				db.GetReader(ref resultDataReader, "SP_Get_User_AchievementData", sqlParameters);
				while (resultDataReader.Read())
				{
					AchievementData item = new AchievementData
					{
						UserID = (int)resultDataReader["UserID"],
						AchievementID = (int)resultDataReader["AchievementID"],
						IsComplete = (bool)resultDataReader["IsComplete"],
						CompletedDate = (DateTime)resultDataReader["CompletedDate"]
					};
					list.Add(item);
				}
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("Init", exception);
				}
			}
			finally
			{
				if (resultDataReader != null && !resultDataReader.IsClosed)
				{
					resultDataReader.Close();
				}
			}
			return list.ToArray();
		}

		public ItemInfo[] GetUserBagByType(int UserID, int bagType)
		{
			List<ItemInfo> list = new List<ItemInfo>();
			SqlDataReader resultDataReader = null;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[2]
				{
				new SqlParameter("@UserID", SqlDbType.Int, 4),
				null
				};
				sqlParameters[0].Value = UserID;
				sqlParameters[1] = new SqlParameter("@BagType", bagType);
				db.GetReader(ref resultDataReader, "SP_Users_BagByType", sqlParameters);
				while (resultDataReader.Read())
				{
					list.Add(InitItem(resultDataReader));
				}
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("Init", exception);
				}
			}
			finally
			{
				if (resultDataReader != null && !resultDataReader.IsClosed)
				{
					resultDataReader.Close();
				}
			}
			return list.ToArray();
		}

		public BufferInfo[] GetUserBuffer(int userID)
		{
			List<BufferInfo> list = new List<BufferInfo>();
			SqlDataReader resultDataReader = null;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[1]
				{
				new SqlParameter("@UserID", SqlDbType.Int, 4)
				};
				sqlParameters[0].Value = userID;
				db.GetReader(ref resultDataReader, "SP_User_Buff_All", sqlParameters);
				while (resultDataReader.Read())
				{
					BufferInfo item = new BufferInfo
					{
						BeginDate = (DateTime)resultDataReader["BeginDate"],
						Data = ((resultDataReader["Data"] == null) ? "" : resultDataReader["Data"].ToString()),
						Type = (int)resultDataReader["Type"],
						UserID = (int)resultDataReader["UserID"],
						ValidDate = (int)resultDataReader["ValidDate"],
						Value = (int)resultDataReader["Value"],
						IsExist = (bool)resultDataReader["IsExist"],
						ValidCount = (int)resultDataReader["ValidCount"],
						TemplateID = (int)resultDataReader["TemplateID"],
						IsDirty = false
					};
					list.Add(item);
				}
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("Init", exception);
				}
			}
			finally
			{
				if (resultDataReader != null && !resultDataReader.IsClosed)
				{
					resultDataReader.Close();
				}
			}
			return list.ToArray();
		}

		public List<UsersCardInfo> GetUserCardEuqip(int UserID)
		{
			List<UsersCardInfo> list = new List<UsersCardInfo>();
			SqlDataReader resultDataReader = null;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[1]
				{
				new SqlParameter("@UserID", SqlDbType.Int, 4)
				};
				sqlParameters[0].Value = UserID;
				db.GetReader(ref resultDataReader, "SP_Users_Items_Card_Equip", sqlParameters);
				while (resultDataReader.Read())
				{
					list.Add(InitCard(resultDataReader));
				}
				return list;
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("Init", exception);
					return list;
				}
				return list;
			}
			finally
			{
				if (resultDataReader != null && !resultDataReader.IsClosed)
				{
					resultDataReader.Close();
				}
			}
		}

		public ConsortiaBufferInfo[] GetUserConsortiaBuffer(int ConsortiaID)
		{
			List<ConsortiaBufferInfo> list = new List<ConsortiaBufferInfo>();
			SqlDataReader ResultDataReader = null;
			try
			{
				SqlParameter[] SqlParameters = new SqlParameter[1]
				{
				new SqlParameter("@ConsortiaID", SqlDbType.Int, 4)
				};
				SqlParameters[0].Value = ConsortiaID;
				db.GetReader(ref ResultDataReader, "SP_User_Consortia_Buff_All", SqlParameters);
				while (ResultDataReader.Read())
				{
					list.Add(new ConsortiaBufferInfo
					{
						ConsortiaID = (int)ResultDataReader["ConsortiaID"],
						BufferID = (int)ResultDataReader["BufferID"],
						IsOpen = (bool)ResultDataReader["IsOpen"],
						BeginDate = (DateTime)ResultDataReader["BeginDate"],
						ValidDate = (int)ResultDataReader["ValidDate"],
						Type = (int)ResultDataReader["Type"],
						Value = (int)ResultDataReader["Value"]
					});
				}
			}
			catch (Exception ex)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("Init SP_User_Consortia_Buff_All", ex);
				}
			}
			finally
			{
				if (ResultDataReader != null && !ResultDataReader.IsClosed)
				{
					ResultDataReader.Close();
				}
			}
			return list.ToArray();
		}

		public ConsortiaBufferInfo GetUserConsortiaBufferSingle(int ID)
		{
			SqlDataReader ResultDataReader = null;
			try
			{
				SqlParameter[] SqlParameters = new SqlParameter[1]
				{
				new SqlParameter("@ID", SqlDbType.Int, 4)
				};
				SqlParameters[0].Value = ID;
				db.GetReader(ref ResultDataReader, "SP_User_Consortia_Buff_Single", SqlParameters);
				if (ResultDataReader.Read())
				{
					return new ConsortiaBufferInfo
					{
						ConsortiaID = (int)ResultDataReader["ConsortiaID"],
						BufferID = (int)ResultDataReader["BufferID"],
						IsOpen = (bool)ResultDataReader["IsOpen"],
						BeginDate = (DateTime)ResultDataReader["BeginDate"],
						ValidDate = (int)ResultDataReader["ValidDate"],
						Type = (int)ResultDataReader["Type"],
						Value = (int)ResultDataReader["Value"]
					};
				}
			}
			catch (Exception ex)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("Init SP_User_Consortia_Buff_Single", ex);
				}
			}
			finally
			{
				if (ResultDataReader != null && !ResultDataReader.IsClosed)
				{
					ResultDataReader.Close();
				}
			}
			return null;
		}

		public List<ItemInfo> GetUserEuqip(int UserID)
		{
			List<ItemInfo> list = new List<ItemInfo>();
			SqlDataReader resultDataReader = null;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[1]
				{
				new SqlParameter("@UserID", SqlDbType.Int, 4)
				};
				sqlParameters[0].Value = UserID;
				db.GetReader(ref resultDataReader, "SP_Users_Items_Equip", sqlParameters);
				while (resultDataReader.Read())
				{
					list.Add(InitItem(resultDataReader));
				}
				return list;
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("Init", exception);
					return list;
				}
				return list;
			}
			finally
			{
				if (resultDataReader != null && !resultDataReader.IsClosed)
				{
					resultDataReader.Close();
				}
			}
		}

		public EventRewardProcessInfo[] GetUserEventProcess(int userID)
		{
			SqlDataReader resultDataReader = null;
			List<EventRewardProcessInfo> list = new List<EventRewardProcessInfo>();
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[1]
				{
				new SqlParameter("@UserID", SqlDbType.Int, 4)
				};
				sqlParameters[0].Value = userID;
				db.GetReader(ref resultDataReader, "SP_Get_User_EventProcess", sqlParameters);
				while (resultDataReader.Read())
				{
					EventRewardProcessInfo item = new EventRewardProcessInfo
					{
						UserID = (int)resultDataReader["UserID"],
						ActiveType = (int)resultDataReader["ActiveType"],
						Conditions = (int)resultDataReader["Conditions"],
						AwardGot = (int)resultDataReader["AwardGot"]
					};
					list.Add(item);
				}
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("Init", exception);
				}
			}
			finally
			{
				if (resultDataReader != null && !resultDataReader.IsClosed)
				{
					resultDataReader.Close();
				}
			}
			return list.ToArray();
		}

		public ItemInfo[] GetUserItem(int UserID)
		{
			List<ItemInfo> list = new List<ItemInfo>();
			SqlDataReader resultDataReader = null;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[1]
				{
				new SqlParameter("@UserID", SqlDbType.Int, 4)
				};
				sqlParameters[0].Value = UserID;
				db.GetReader(ref resultDataReader, "SP_Users_Items_All", sqlParameters);
				while (resultDataReader.Read())
				{
					list.Add(InitItem(resultDataReader));
				}
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("Init", exception);
				}
			}
			finally
			{
				if (resultDataReader != null && !resultDataReader.IsClosed)
				{
					resultDataReader.Close();
				}
			}
			return list.ToArray();
		}

		public ItemInfo GetUserItemSingle(int itemID)
		{
			SqlDataReader resultDataReader = null;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[1]
				{
				new SqlParameter("@ID", SqlDbType.Int, 4)
				};
				sqlParameters[0].Value = itemID;
				db.GetReader(ref resultDataReader, "SP_Users_Items_Single", sqlParameters);
				if (resultDataReader.Read())
				{
					return InitItem(resultDataReader);
				}
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("Init", exception);
				}
			}
			finally
			{
				if (resultDataReader != null && !resultDataReader.IsClosed)
				{
					resultDataReader.Close();
				}
			}
			return null;
		}

		public PlayerInfo[] GetUserLoginList(string userName)
		{
			List<PlayerInfo> list = new List<PlayerInfo>();
			SqlDataReader resultDataReader = null;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[1]
				{
				new SqlParameter("@UserName", SqlDbType.NVarChar, 200)
				};
				sqlParameters[0].Value = userName;
				db.GetReader(ref resultDataReader, "SP_Users_LoginList", sqlParameters);
				while (resultDataReader.Read())
				{
					list.Add(InitPlayerInfo(resultDataReader));
				}
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("Init", exception);
				}
			}
			finally
			{
				if (resultDataReader != null && !resultDataReader.IsClosed)
				{
					resultDataReader.Close();
				}
			}
			return list.ToArray();
		}

		public QuestDataInfo[] GetUserQuest(int userID)
		{
			List<QuestDataInfo> infos = new List<QuestDataInfo>();
			SqlDataReader reader = null;
			try
			{
				SqlParameter[] para = new SqlParameter[1]
				{
				new SqlParameter("@UserID", SqlDbType.Int, 4)
				};
				para[0].Value = userID;
				db.GetReader(ref reader, "SP_QuestData_All", para);
				while (reader.Read())
				{
					infos.Add(new QuestDataInfo
					{
						CompletedDate = (DateTime)reader["CompletedDate"],
						IsComplete = (bool)reader["IsComplete"],
						Condition1 = (int)reader["Condition1"],
						Condition2 = (int)reader["Condition2"],
						Condition3 = (int)reader["Condition3"],
						Condition4 = (int)reader["Condition4"],
						QuestID = (int)reader["QuestID"],
						UserID = (int)reader["UserId"],
						IsExist = (bool)reader["IsExist"],
						RandDobule = (int)reader["RandDobule"],
						RepeatFinish = (int)reader["RepeatFinish"],
						IsDirty = false
					});
				}
			}
			catch (Exception e)
			{
				BaseBussiness.log.Error("Init", e);
			}
			finally
			{
				if (reader != null && !reader.IsClosed)
				{
					reader.Close();
				}
			}
			return infos.ToArray();
		}

		public PlayerInfo GetUserSingleByNickName(string nickName)
		{
			SqlDataReader resultDataReader = null;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[1]
				{
				new SqlParameter("@NickName", SqlDbType.NVarChar, 200)
				};
				sqlParameters[0].Value = nickName;
				db.GetReader(ref resultDataReader, "SP_Users_SingleByNickName", sqlParameters);
				if (resultDataReader.Read())
				{
					return InitPlayerInfo(resultDataReader);
				}
			}
			catch
			{
				throw new Exception();
			}
			finally
			{
				if (resultDataReader != null && !resultDataReader.IsClosed)
				{
					resultDataReader.Close();
				}
			}
			return null;
		}

		public PlayerInfo GetUserSingleByUserID(int UserID)
		{
			SqlDataReader resultDataReader = null;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[1]
				{
				new SqlParameter("@UserID", SqlDbType.Int, 4)
				};
				sqlParameters[0].Value = UserID;
				db.GetReader(ref resultDataReader, "SP_Users_SingleByUserID", sqlParameters);
				if (resultDataReader.Read())
				{
					return InitPlayerInfo(resultDataReader);
				}
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("Init", exception);
				}
			}
			finally
			{
				if (resultDataReader != null && !resultDataReader.IsClosed)
				{
					resultDataReader.Close();
				}
			}
			return null;
		}

		public PlayerInfo GetUserSingleByUserName(string userName)
		{
			SqlDataReader resultDataReader = null;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[1]
				{
				new SqlParameter("@UserName", SqlDbType.NVarChar, 200)
				};
				sqlParameters[0].Value = userName;
				db.GetReader(ref resultDataReader, "SP_Users_SingleByUserName", sqlParameters);
				if (resultDataReader.Read())
				{
					return InitPlayerInfo(resultDataReader);
				}
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("Init", exception);
				}
			}
			finally
			{
				if (resultDataReader != null && !resultDataReader.IsClosed)
				{
					resultDataReader.Close();
				}
			}
			return null;
		}

		public TexpInfo GetUserTexpInfoSingle(int ID)
		{
			SqlDataReader resultDataReader = null;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[1]
				{
				new SqlParameter("@UserID", ID)
				};
				db.GetReader(ref resultDataReader, "SP_Get_UserTexp_By_ID", sqlParameters);
				if (resultDataReader.Read())
				{
					return new TexpInfo
					{
						UserID = (int)resultDataReader["UserID"],
						attTexpExp = (int)resultDataReader["attTexpExp"],
						defTexpExp = (int)resultDataReader["defTexpExp"],
						hpTexpExp = (int)resultDataReader["hpTexpExp"],
						lukTexpExp = (int)resultDataReader["lukTexpExp"],
						spdTexpExp = (int)resultDataReader["spdTexpExp"],
						texpCount = (int)resultDataReader["texpCount"],
						texpTaskCount = (int)resultDataReader["texpTaskCount"],
						texpTaskDate = (DateTime)resultDataReader["texpTaskDate"],
						resetCount = (int)resultDataReader["resetCount"],
						lastReset = (DateTime)resultDataReader["lastReset"]
					};
				}
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("GetTexpInfoSingle", exception);
				}
			}
			finally
			{
				if (resultDataReader != null && !resultDataReader.IsClosed)
				{
					resultDataReader.Close();
				}
			}
			return null;
		}

		public UsersCardInfo[] GetSingleUserCard(int UserID)
		{
			SqlDataReader ResultDataReader = null;
			List<UsersCardInfo> userCardInfoList = new List<UsersCardInfo>();
			try
			{
				SqlParameter[] SqlParameters = new SqlParameter[1]
				{
				new SqlParameter("@UserID", SqlDbType.Int, 4)
				};
				SqlParameters[0].Value = UserID;
				db.GetReader(ref ResultDataReader, "SP_GetSingleUserCard", SqlParameters);
				while (ResultDataReader.Read())
				{
					UsersCardInfo userCardInfo = InitCard(ResultDataReader);
					userCardInfoList.Add(userCardInfo);
				}
			}
			catch (Exception ex)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("GetSingleUserCard", ex);
				}
			}
			finally
			{
				if (ResultDataReader != null && !ResultDataReader.IsClosed)
				{
					ResultDataReader.Close();
				}
			}
			return userCardInfoList.ToArray();
		}

		public AuctionInfo InitAuctionInfo(SqlDataReader reader)
		{
			return new AuctionInfo
			{
				AuctioneerID = (int)reader["AuctioneerID"],
				AuctioneerName = ((reader["AuctioneerName"] == null) ? "" : reader["AuctioneerName"].ToString()),
				AuctionID = (int)reader["AuctionID"],
				BeginDate = (DateTime)reader["BeginDate"],
				BuyerID = (int)reader["BuyerID"],
				BuyerName = ((reader["BuyerName"] == null) ? "" : reader["BuyerName"].ToString()),
				IsExist = (bool)reader["IsExist"],
				ItemID = (int)reader["ItemID"],
				Mouthful = (int)reader["Mouthful"],
				PayType = (int)reader["PayType"],
				Price = (int)reader["Price"],
				Rise = (int)reader["Rise"],
				ValidDate = (int)reader["ValidDate"],
				Name = reader["Name"].ToString(),
				Category = (int)reader["Category"],
				goodsCount = (int)reader["goodsCount"]
			};
		}

		private UsersCardInfo InitCard(SqlDataReader sqlDataReader_0)
		{
			return new UsersCardInfo
			{
				CardID = (int)sqlDataReader_0["CardID"],
				UserID = (int)sqlDataReader_0["UserID"],
				TemplateID = (int)sqlDataReader_0["TemplateID"],
				Place = (int)sqlDataReader_0["Place"],
				Count = (int)sqlDataReader_0["Count"],
				Attack = (int)sqlDataReader_0["Attack"],
				Defence = (int)sqlDataReader_0["Defence"],
				Agility = (int)sqlDataReader_0["Agility"],
				Luck = (int)sqlDataReader_0["Luck"],
				AttackReset = (int)sqlDataReader_0["AttackReset"],
				DefenceReset = (int)sqlDataReader_0["DefenceReset"],
				AgilityReset = (int)sqlDataReader_0["AgilityReset"],
				LuckReset = (int)sqlDataReader_0["LuckReset"],
				Guard = (int)sqlDataReader_0["Guard"],
				Damage = (int)sqlDataReader_0["Damage"],
				Level = (int)sqlDataReader_0["Level"],
				CardGP = (int)sqlDataReader_0["CardGP"],
				isFirstGet = (bool)sqlDataReader_0["isFirstGet"]
			};
		}

		public ConsortiaUserInfo InitConsortiaUserInfo(SqlDataReader dr)
		{
			ConsortiaUserInfo consortiaUserInfo = new ConsortiaUserInfo();
			consortiaUserInfo.ID = (int)dr["ID"];
			consortiaUserInfo.ConsortiaID = (int)dr["ConsortiaID"];
			consortiaUserInfo.DutyID = (int)dr["DutyID"];
			consortiaUserInfo.DutyName = dr["DutyName"].ToString();
			consortiaUserInfo.IsExist = (bool)dr["IsExist"];
			consortiaUserInfo.RatifierID = (int)dr["RatifierID"];
			consortiaUserInfo.RatifierName = dr["RatifierName"].ToString();
			consortiaUserInfo.Remark = dr["Remark"].ToString();
			consortiaUserInfo.UserID = (int)dr["UserID"];
			consortiaUserInfo.UserName = dr["UserName"].ToString();
			consortiaUserInfo.Grade = (int)dr["Grade"];
			consortiaUserInfo.GP = (int)dr["GP"];
			consortiaUserInfo.Repute = (int)dr["Repute"];
			consortiaUserInfo.State = (int)dr["State"];
			consortiaUserInfo.Right = (int)dr["Right"];
			consortiaUserInfo.Offer = (int)dr["Offer"];
			consortiaUserInfo.Colors = dr["Colors"].ToString();
			consortiaUserInfo.Style = dr["Style"].ToString();
			consortiaUserInfo.Hide = (int)dr["Hide"];
			consortiaUserInfo.Skin = ((dr["Skin"] == null) ? "" : consortiaUserInfo.Skin);
			consortiaUserInfo.Level = (int)dr["Level"];
			consortiaUserInfo.LastDate = (DateTime)dr["LastDate"];
			consortiaUserInfo.Sex = (bool)dr["Sex"];
			consortiaUserInfo.IsBanChat = (bool)dr["IsBanChat"];
			consortiaUserInfo.Win = (int)dr["Win"];
			consortiaUserInfo.Total = (int)dr["Total"];
			consortiaUserInfo.Escape = (int)dr["Escape"];
			consortiaUserInfo.RichesOffer = (int)dr["RichesOffer"];
			consortiaUserInfo.RichesRob = (int)dr["RichesRob"];
			consortiaUserInfo.LoginName = ((dr["LoginName"] == null) ? "" : dr["LoginName"].ToString());
			consortiaUserInfo.Nimbus = (int)dr["Nimbus"];
			consortiaUserInfo.FightPower = (int)dr["FightPower"];
			consortiaUserInfo.typeVIP = Convert.ToByte(dr["typeVIP"]);
			consortiaUserInfo.VIPLevel = (int)dr["VIPLevel"];
			return consortiaUserInfo;
		}

		public ItemInfo InitItem(SqlDataReader reader)
		{
			ItemInfo itemInfo = new ItemInfo(ItemMgr.FindItemTemplate((int)reader["TemplateID"]));
			itemInfo.AgilityCompose = (int)reader["AgilityCompose"];
			itemInfo.AttackCompose = (int)reader["AttackCompose"];
			itemInfo.Color = reader["Color"].ToString();
			itemInfo.Count = (int)reader["Count"];
			itemInfo.DefendCompose = (int)reader["DefendCompose"];
			itemInfo.ItemID = (int)reader["ItemID"];
			itemInfo.LuckCompose = (int)reader["LuckCompose"];
			itemInfo.Place = (int)reader["Place"];
			itemInfo.StrengthenLevel = (int)reader["StrengthenLevel"];
			itemInfo.TemplateID = (int)reader["TemplateID"];
			itemInfo.UserID = (int)reader["UserID"];
			itemInfo.ValidDate = (int)reader["ValidDate"];
			itemInfo.IsDirty = false;
			itemInfo.IsExist = (bool)reader["IsExist"];
			itemInfo.IsBinds = (bool)reader["IsBinds"];
			itemInfo.IsUsed = (bool)reader["IsUsed"];
			itemInfo.BeginDate = (DateTime)reader["BeginDate"];
			itemInfo.IsJudge = (bool)reader["IsJudge"];
			itemInfo.BagType = (int)reader["BagType"];
			itemInfo.Skin = reader["Skin"].ToString();
			itemInfo.RemoveDate = (DateTime)reader["RemoveDate"];
			itemInfo.RemoveType = (int)reader["RemoveType"];
			itemInfo.Hole1 = (int)reader["Hole1"];
			itemInfo.Hole2 = (int)reader["Hole2"];
			itemInfo.Hole3 = (int)reader["Hole3"];
			itemInfo.Hole4 = (int)reader["Hole4"];
			itemInfo.Hole5 = (int)reader["Hole5"];
			itemInfo.Hole6 = (int)reader["Hole6"];
			itemInfo.Hole5Level = (int)reader["Hole5Level"];
			itemInfo.Hole5Exp = (int)reader["Hole5Exp"];
			itemInfo.Hole6Level = (int)reader["Hole6Level"];
			itemInfo.Hole6Exp = (int)reader["Hole6Exp"];
			itemInfo.StrengthenTimes = (int)reader["StrengthenTimes"];
			itemInfo.goldBeginTime = (DateTime)reader["goldBeginTime"];
			itemInfo.goldValidDate = (int)reader["goldValidDate"];
			itemInfo.StrengthenExp = (int)reader["StrengthenExp"];
			itemInfo.Blood = (int)reader["Blood"];
			itemInfo.GoldEquip = ItemMgr.FindGoldItemTemplate(itemInfo.TemplateID, itemInfo.isGold);
			itemInfo.IsDirty = false;
			itemInfo.latentEnergyCurStr = (string)reader["latentEnergyCurStr"];
			itemInfo.latentEnergyNewStr = (string)reader["latentEnergyNewStr"];
			itemInfo.latentEnergyEndTime = (DateTime)reader["latentEnergyEndTime"];
			itemInfo.cellLocked = (bool)reader["cellLocked"];
			itemInfo.curExp = (int)reader["curExp"];
			return itemInfo;
		}

		public MailInfo InitMail(SqlDataReader reader)
		{
			return new MailInfo
			{
				Annex1 = reader["Annex1"].ToString(),
				Annex2 = reader["Annex2"].ToString(),
				Content = reader["Content"].ToString(),
				Gold = (int)reader["Gold"],
				ID = (int)reader["ID"],
				IsExist = (bool)reader["IsExist"],
				Money = (int)reader["Money"],
				GiftToken = (int)reader["GiftToken"],
				Receiver = reader["Receiver"].ToString(),
				ReceiverID = (int)reader["ReceiverID"],
				Sender = reader["Sender"].ToString(),
				SenderID = (int)reader["SenderID"],
				Title = reader["Title"].ToString(),
				Type = (int)reader["Type"],
				ValidDate = (int)reader["ValidDate"],
				IsRead = (bool)reader["IsRead"],
				SendTime = (DateTime)reader["SendTime"],
				Annex1Name = ((reader["Annex1Name"] == null) ? "" : reader["Annex1Name"].ToString()),
				Annex2Name = ((reader["Annex2Name"] == null) ? "" : reader["Annex2Name"].ToString()),
				Annex3 = reader["Annex3"].ToString(),
				Annex4 = reader["Annex4"].ToString(),
				Annex5 = reader["Annex5"].ToString(),
				Annex3Name = ((reader["Annex3Name"] == null) ? "" : reader["Annex3Name"].ToString()),
				Annex4Name = ((reader["Annex4Name"] == null) ? "" : reader["Annex4Name"].ToString()),
				Annex5Name = ((reader["Annex5Name"] == null) ? "" : reader["Annex5Name"].ToString()),
				AnnexRemark = ((reader["AnnexRemark"] == null) ? "" : reader["AnnexRemark"].ToString())
			};
		}

		public PlayerInfo InitPlayerInfo(SqlDataReader reader)
		{
			PlayerInfo obj = new PlayerInfo
			{
				Password = (string)reader["Password"],
				IsConsortia = (bool)reader["IsConsortia"],
				Agility = (int)reader["Agility"],
				Attack = (int)reader["Attack"],
				hp = (int)reader["hp"],
				Colors = ((reader["Colors"] == null) ? "" : reader["Colors"].ToString()),
				ConsortiaID = (int)reader["ConsortiaID"],
				Defence = (int)reader["Defence"],
				Gold = (int)reader["Gold"],
				GP = (int)reader["GP"],
				Grade = (int)reader["Grade"],
				ID = (int)reader["UserID"],
				Luck = (int)reader["Luck"],
				Money = (int)reader["Money"],
				NickName = (((string)reader["NickName"] == null) ? "" : ((string)reader["NickName"])),
				Sex = (bool)reader["Sex"],
				State = (int)reader["State"],
				Style = ((reader["Style"] == null) ? "" : reader["Style"].ToString()),
				Hide = (int)reader["Hide"],
				Repute = (int)reader["Repute"],
				UserName = ((reader["UserName"] == null) ? "" : reader["UserName"].ToString()),
				ConsortiaName = ((reader["ConsortiaName"] == null) ? "" : reader["ConsortiaName"].ToString()),
				Offer = (int)reader["Offer"],
				Win = (int)reader["Win"],
				Total = (int)reader["Total"],
				Escape = (int)reader["Escape"],
				Skin = ((reader["Skin"] == null) ? "" : reader["Skin"].ToString()),
				IsBanChat = (bool)reader["IsBanChat"],
				ReputeOffer = (int)reader["ReputeOffer"],
				ConsortiaRepute = (int)reader["ConsortiaRepute"],
				ConsortiaLevel = (int)reader["ConsortiaLevel"],
				StoreLevel = (int)reader["StoreLevel"],
				ShopLevel = (int)reader["ShopLevel"],
				SmithLevel = (int)reader["SmithLevel"],
				ConsortiaHonor = (int)reader["ConsortiaHonor"],
				RichesOffer = (int)reader["RichesOffer"],
				RichesRob = (int)reader["RichesRob"],
				AntiAddiction = (int)reader["AntiAddiction"],
				DutyLevel = (int)reader["DutyLevel"],
				DutyName = ((reader["DutyName"] == null) ? "" : reader["DutyName"].ToString()),
				Right = (int)reader["Right"],
				ChairmanName = ((reader["ChairmanName"] == null) ? "" : reader["ChairmanName"].ToString()),
				AddDayGP = (int)reader["AddDayGP"],
				AddDayOffer = (int)reader["AddDayOffer"],
				AddWeekGP = (int)reader["AddWeekGP"],
				AddWeekOffer = (int)reader["AddWeekOffer"],
				ConsortiaRiches = (int)reader["ConsortiaRiches"],
				CheckCount = (int)reader["CheckCount"],
				IsMarried = (bool)reader["IsMarried"],
				SpouseID = (int)reader["SpouseID"],
				SpouseName = ((reader["SpouseName"] == null) ? "" : reader["SpouseName"].ToString()),
				MarryInfoID = (int)reader["MarryInfoID"],
				IsCreatedMarryRoom = (bool)reader["IsCreatedMarryRoom"],
				DayLoginCount = (int)reader["DayLoginCount"],
				PasswordTwo = ((reader["PasswordTwo"] == null) ? "" : reader["PasswordTwo"].ToString()),
				SelfMarryRoomID = (int)reader["SelfMarryRoomID"],
				IsGotRing = (bool)reader["IsGotRing"],
				Rename = (bool)reader["Rename"],
				ConsortiaRename = (bool)reader["ConsortiaRename"],
				IsDirty = false,
				IsFirst = (int)reader["IsFirst"],
				Nimbus = (int)reader["Nimbus"],
				LastAward = (DateTime)reader["LastAward"],
				GiftToken = (int)reader["GiftToken"],
				QuestSite = ((reader["QuestSite"] == null) ? new byte[200] : ((byte[])reader["QuestSite"])),
				PvePermission = ((reader["PvePermission"] == null) ? "" : reader["PvePermission"].ToString()),
				FightPower = (int)reader["FightPower"],
				PasswordQuest1 = ((reader["PasswordQuestion1"] == null) ? "" : reader["PasswordQuestion1"].ToString()),
				PasswordQuest2 = ((reader["PasswordQuestion2"] == null) ? "" : reader["PasswordQuestion2"].ToString())
			};
			PlayerInfo info2 = obj;
			if ((DateTime)reader["LastFindDate"] != DateTime.Today.Date)
			{
				info2.FailedPasswordAttemptCount = 5;
			}
			else
			{
				info2.FailedPasswordAttemptCount = (int)reader["FailedPasswordAttemptCount"];
			}
			obj.AnswerSite = (int)reader["AnswerSite"];
			obj.medal = (int)reader["Medal"];
			obj.ChatCount = (int)reader["ChatCount"];
			obj.SpaPubGoldRoomLimit = (int)reader["SpaPubGoldRoomLimit"];
			obj.LastSpaDate = (DateTime)reader["LastSpaDate"];
			obj.FightLabPermission = (string)reader["FightLabPermission"];
			obj.SpaPubMoneyRoomLimit = (int)reader["SpaPubMoneyRoomLimit"];
			obj.IsInSpaPubGoldToday = (bool)reader["IsInSpaPubGoldToday"];
			obj.IsInSpaPubMoneyToday = (bool)reader["IsInSpaPubMoneyToday"];
			obj.AchievementPoint = (int)reader["AchievementPoint"];
			obj.LastWeekly = (DateTime)reader["LastWeekly"];
			obj.LastWeeklyVersion = (int)reader["LastWeeklyVersion"];
			obj.badgeID = (int)reader["BadgeID"];
			obj.typeVIP = Convert.ToByte(reader["typeVIP"]);
			obj.VIPLevel = (int)reader["VIPLevel"];
			obj.VIPExp = (int)reader["VIPExp"];
			obj.VIPExpireDay = (DateTime)reader["VIPExpireDay"];
			obj.VIPNextLevelDaysNeeded = (int)reader["VIPNextLevelDaysNeeded"];
			obj.LastVIPPackTime = (DateTime)reader["LastVIPPackTime"];
			obj.CanTakeVipReward = (bool)reader["CanTakeVipReward"];
			obj.WeaklessGuildProgressStr = (string)reader["WeaklessGuildProgressStr"];
			obj.IsOldPlayer = (bool)reader["IsOldPlayer"];
			obj.LastDate = (DateTime)reader["LastDate"];
			obj.VIPLastDate = (DateTime)reader["VIPLastDate"];
			obj.Score = (int)reader["Score"];
			obj.OptionOnOff = (int)reader["OptionOnOff"];
			obj.isOldPlayerHasValidEquitAtLogin = (bool)reader["isOldPlayerHasValidEquitAtLogin"];
			obj.badLuckNumber = (int)reader["badLuckNumber"];
			obj.OnlineTime = (int)reader["OnlineTime"];
			obj.luckyNum = (int)reader["luckyNum"];
			obj.lastLuckyNumDate = (DateTime)reader["lastLuckyNumDate"];
			obj.lastLuckNum = (int)reader["lastLuckNum"];
			obj.IsShowConsortia = (bool)reader["IsShowConsortia"];
			obj.NewDay = (DateTime)reader["NewDay"];
			obj.Honor = (string)reader["Honor"];
			obj.BoxGetDate = (DateTime)reader["BoxGetDate"];
			obj.AlreadyGetBox = (int)reader["AlreadyGetBox"];
			obj.BoxProgression = (int)reader["BoxProgression"];
			obj.GetBoxLevel = (int)reader["GetBoxLevel"];
			obj.IsRecharged = (bool)reader["IsRecharged"];
			obj.IsGetAward = (bool)reader["IsGetAward"];
			obj.apprenticeshipState = (int)reader["apprenticeshipState"];
			obj.masterID = (int)reader["masterID"];
			obj.masterOrApprentices = ((reader["masterOrApprentices"] == DBNull.Value) ? "" : ((string)reader["masterOrApprentices"]));
			obj.graduatesCount = (int)reader["graduatesCount"];
			obj.honourOfMaster = ((reader["honourOfMaster"] == DBNull.Value) ? "" : ((string)reader["honourOfMaster"]));
			obj.freezesDate = ((reader["freezesDate"] == DBNull.Value) ? DateTime.Now : ((DateTime)reader["freezesDate"]));
			obj.charmGP = ((reader["charmGP"] != DBNull.Value) ? ((int)reader["charmGP"]) : 0);
			obj.charmLevel = ((reader["charmLevel"] != DBNull.Value) ? ((int)reader["charmLevel"]) : 0);
			obj.evolutionGrade = (int)reader["evolutionGrade"];
			obj.evolutionExp = (int)reader["evolutionExp"];
			obj.hardCurrency = (int)reader["hardCurrency"];
			obj.EliteScore = (int)reader["EliteScore"];
			obj.ShopFinallyGottenTime = ((reader["ShopFinallyGottenTime"] == DBNull.Value) ? DateTime.Now.AddDays(-1.0) : ((DateTime)reader["ShopFinallyGottenTime"]));
			obj.MoneyLock = ((reader["MoneyLock"] != DBNull.Value) ? ((int)reader["MoneyLock"]) : 0);
			obj.LastGetEgg = (DateTime)reader["LastGetEgg"];
			obj.IsFistGetPet = (bool)reader["IsFistGetPet"];
			obj.LastRefreshPet = (DateTime)reader["LastRefreshPet"];
			obj.petScore = (int)reader["petScore"];
			obj.accumulativeLoginDays = (int)reader["accumulativeLoginDays"];
			obj.accumulativeAwardDays = (int)reader["accumulativeAwardDays"];
			obj.honorId = (int)reader["honorId"];
			obj.LoginDevice = (int)reader["LoginDevice"];
			obj.MaxBuyHonor = (int)reader["MaxBuyHonor"];
			obj.myHonor = (int)reader["myHonor"];
			obj.totemId = (int)reader["totemId"];
			obj.necklaceExp = (int)reader["necklaceExp"];
			obj.necklaceExpAdd = (int)reader["necklaceExpAdd"];
			obj.RingExp = (int)reader["RingExp"];
			obj.fineSuitExp = (int)reader["fineSuitExp"];
			obj.GhostEquipList = reader["GhostEquipList"] == DBNull.Value ? "" : (string)reader["GhostEquipList"];
			obj.damageScores = (int)reader["damageScores"];
			obj.DateFinishTask = (DateTime)reader["DateFinishTask"];
			obj.MyInviteCode = reader["MyInviteCode"] == DBNull.Value ? "" : (string)reader["MyInviteCode"];
			obj.MyInvitedCount = (int)reader["MyInvitedCount"];
			obj.AwardColumnOne = (int)reader["AwardColumnOne"];
			obj.AwardColumnTwo = (int)reader["AwardColumnTwo"];
			obj.AwardColumnThree = (int)reader["AwardColumnThree"];
			obj.AwardColumnFour = (int)reader["AwardColumnFour"];
			obj.MyRewardStatus = (bool)reader["MyRewardStatus"];
			obj.MyCodeStatus = (bool)reader["MyCodeStatus"];
			obj.DailyScore = (int)reader["DailyScore"];
			obj.DailyWinCount = (int)reader["DailyWinCount"];
			obj.DailyGameCount = (int)reader["DailyGameCount"];
			obj.DailyLeagueFirst = (bool)reader["DailyLeagueFirst"];
			obj.DailyLeagueLastScore = (int)reader["DailyLeagueLastScore"];
			obj.WeeklyScore = (int)reader["WeeklyScore"];
			obj.WeeklyGameCount = (int)reader["WeeklyGameCount"];
			obj.WeeklyRanking = (int)reader["WeeklyRanking"];
			obj.jampsCurrency = (int)reader["jampsCurrency"];
			obj.awardGot = (int)reader["awardGot"];
			return obj;
		}

		public bool InsertMarryRoomInfo(MarryRoomInfo info)
		{
			bool flag = false;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[20]
				{
				new SqlParameter("@ID", info.ID),
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null
				};
				sqlParameters[0].Direction = ParameterDirection.InputOutput;
				sqlParameters[1] = new SqlParameter("@Name", info.Name);
				sqlParameters[2] = new SqlParameter("@PlayerID", info.PlayerID);
				sqlParameters[3] = new SqlParameter("@PlayerName", info.PlayerName);
				sqlParameters[4] = new SqlParameter("@GroomID", info.GroomID);
				sqlParameters[5] = new SqlParameter("@GroomName", info.GroomName);
				sqlParameters[6] = new SqlParameter("@BrideID", info.BrideID);
				sqlParameters[7] = new SqlParameter("@BrideName", info.BrideName);
				sqlParameters[8] = new SqlParameter("@Pwd", info.Pwd);
				sqlParameters[9] = new SqlParameter("@AvailTime", info.AvailTime);
				sqlParameters[10] = new SqlParameter("@MaxCount", info.MaxCount);
				sqlParameters[11] = new SqlParameter("@GuestInvite", info.GuestInvite);
				sqlParameters[12] = new SqlParameter("@MapIndex", info.MapIndex);
				sqlParameters[13] = new SqlParameter("@BeginTime", info.BeginTime);
				sqlParameters[14] = new SqlParameter("@BreakTime", info.BreakTime);
				sqlParameters[15] = new SqlParameter("@RoomIntroduction", info.RoomIntroduction);
				sqlParameters[16] = new SqlParameter("@ServerID", info.ServerID);
				sqlParameters[17] = new SqlParameter("@IsHymeneal", info.IsHymeneal);
				sqlParameters[18] = new SqlParameter("@IsGunsaluteUsed", info.IsGunsaluteUsed);
				sqlParameters[19] = new SqlParameter("@Result", SqlDbType.Int);
				sqlParameters[19].Direction = ParameterDirection.ReturnValue;
				db.RunProcedure("SP_Insert_Marry_Room_Info", sqlParameters);
				flag = (int)sqlParameters[19].Value == 0;
				if (flag)
				{
					info.ID = (int)sqlParameters[0].Value;
					return flag;
				}
				return flag;
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("InsertMarryRoomInfo", exception);
					return flag;
				}
				return flag;
			}
		}

		public int PullDown(int activeID, string awardID, int userID, ref string msg)
		{
			int num = 1;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[4]
				{
				new SqlParameter("@ActiveID", activeID),
				new SqlParameter("@AwardID", awardID),
				new SqlParameter("@UserID", userID),
				new SqlParameter("@Result", SqlDbType.Int)
				};
				sqlParameters[3].Direction = ParameterDirection.ReturnValue;
				if (!db.RunProcedure("SP_Active_PullDown", sqlParameters))
				{
					return num;
				}
				num = (int)sqlParameters[3].Value;
				switch (num)
				{
					case 0:
						msg = "ActiveBussiness.Msg0";
						return num;
					case 1:
						msg = "ActiveBussiness.Msg1";
						return num;
					case 2:
						msg = "ActiveBussiness.Msg2";
						return num;
					case 3:
						msg = "ActiveBussiness.Msg3";
						return num;
					case 4:
						msg = "ActiveBussiness.Msg4";
						return num;
					case 5:
						msg = "ActiveBussiness.Msg5";
						return num;
					case 6:
						msg = "ActiveBussiness.Msg6";
						return num;
					case 7:
						msg = "ActiveBussiness.Msg7";
						return num;
					case 8:
						msg = "ActiveBussiness.Msg8";
						return num;
					default:
						msg = "ActiveBussiness.Msg9";
						return num;
				}
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("Init", exception);
					return num;
				}
				return num;
			}
		}

		public PlayerInfo LoginGame(string username, ref int isFirst, ref bool isExist, ref bool isError, bool firstValidate, ref DateTime forbidDate, string nickname)
		{
			SqlDataReader resultDataReader = null;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[4]
				{
				new SqlParameter("@UserName", username),
				new SqlParameter("@Password", ""),
				new SqlParameter("@FirstValidate", firstValidate),
				new SqlParameter("@Nickname", nickname)
				};
				db.GetReader(ref resultDataReader, "SP_Users_LoginWeb", sqlParameters);
				if (resultDataReader.Read())
				{
					isFirst = (int)resultDataReader["IsFirst"];
					isExist = (bool)resultDataReader["IsExist"];
					forbidDate = (DateTime)resultDataReader["ForbidDate"];
					if (isFirst > 1)
					{
						isFirst--;
					}
					return InitPlayerInfo(resultDataReader);
				}
			}
			catch (Exception exception)
			{
				isError = true;
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("Init", exception);
				}
			}
			finally
			{
				if (resultDataReader != null && !resultDataReader.IsClosed)
				{
					resultDataReader.Close();
				}
			}
			return null;
		}

		public bool RegisterPlayer(string userName, string passWord, string nickName, string bStyle, string gStyle, string armColor, string hairColor, string faceColor, string clothColor, string hatColor, int sex, ref string msg, int validDate)
		{
			bool flag = false;
			try
			{
				string[] strArray = bStyle.Split(',');
				string[] strArray2 = gStyle.Split(',');
				SqlParameter[] sqlParameters = new SqlParameter[21]
				{
				new SqlParameter("@UserName", userName),
				new SqlParameter("@PassWord", passWord),
				new SqlParameter("@NickName", nickName),
				new SqlParameter("@BArmID", int.Parse(strArray[0])),
				new SqlParameter("@BHairID", int.Parse(strArray[1])),
				new SqlParameter("@BFaceID", int.Parse(strArray[2])),
				new SqlParameter("@BClothID", int.Parse(strArray[3])),
				new SqlParameter("@BHatID", int.Parse(strArray[4])),
				new SqlParameter("@GArmID", int.Parse(strArray2[0])),
				new SqlParameter("@GHairID", int.Parse(strArray2[1])),
				new SqlParameter("@GFaceID", int.Parse(strArray2[2])),
				new SqlParameter("@GClothID", int.Parse(strArray2[3])),
				new SqlParameter("@GHatID", int.Parse(strArray2[4])),
				new SqlParameter("@ArmColor", armColor),
				new SqlParameter("@HairColor", hairColor),
				new SqlParameter("@FaceColor", faceColor),
				new SqlParameter("@ClothColor", clothColor),
				new SqlParameter("@HatColor", clothColor),
				new SqlParameter("@Sex", sex),
				new SqlParameter("@StyleDate", validDate),
				new SqlParameter("@Result", SqlDbType.Int)
				};
				sqlParameters[20].Direction = ParameterDirection.ReturnValue;
				flag = db.RunProcedure("SP_Users_RegisterNotValidate", sqlParameters);
				int num = (int)sqlParameters[20].Value;
				flag = num == 0;
				switch (num)
				{
					case 2:
						msg = LanguageMgr.GetTranslation("PlayerBussiness.RegisterPlayer.Msg2");
						return flag;
					case 3:
						msg = LanguageMgr.GetTranslation("PlayerBussiness.RegisterPlayer.Msg3");
						return flag;
					default:
						return flag;
				}
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("Init", exception);
					return flag;
				}
				return flag;
			}
		}

		public bool RegisterUser(string UserName, string NickName, string Password, bool Sex, int Money, int GiftToken, int Gold)
		{
			bool flag = false;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[8]
				{
				new SqlParameter("@UserName", UserName),
				new SqlParameter("@Password", Password),
				new SqlParameter("@NickName", NickName),
				new SqlParameter("@Sex", Sex),
				new SqlParameter("@Money", Money),
				new SqlParameter("@GiftToken", GiftToken),
				new SqlParameter("@Gold", Gold),
				new SqlParameter("@Result", SqlDbType.Int)
				};
				sqlParameters[7].Direction = ParameterDirection.ReturnValue;
				db.RunProcedure("SP_Account_Register", sqlParameters);
				if ((int)sqlParameters[7].Value == 0)
				{
					flag = true;
					return flag;
				}
				return flag;
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("Init Register", exception);
					return flag;
				}
				return flag;
			}
		}

		public bool RenameNick(string userName, string nickName, string newNickName)
		{
			bool flag = false;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[4]
				{
				new SqlParameter("@UserName", userName),
				new SqlParameter("@NickName", nickName),
				new SqlParameter("@NewNickName", newNickName),
				new SqlParameter("@Result", SqlDbType.Int)
				};
				sqlParameters[3].Direction = ParameterDirection.ReturnValue;
				flag = db.RunProcedure("SP_Users_RenameNick2", sqlParameters);
				flag = (int)sqlParameters[3].Value == 0;
				return flag;
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("RenameNick", exception);
					return flag;
				}
				return flag;
			}
		}

		public bool ChangeSex(int UserId, bool newSex)
		{
			bool result = false;
			try
			{
				SqlParameter[] para = new SqlParameter[3];
				para[0] = new SqlParameter("@UserId", UserId);
				para[1] = new SqlParameter("@Sex", newSex);
				para[2] = new SqlParameter("@Result", System.Data.SqlDbType.Int);
				para[2].Direction = ParameterDirection.ReturnValue;
				result = db.RunProcedure("SP_Users_ChangSexByCard", para);
				int returnValue = (int)para[2].Value;
				result = returnValue == 0;
			}
			catch (Exception e)
			{
				if (log.IsErrorEnabled)
					log.Error("SP_Users_ChangSexByCard ", e);
			}

			return result;
		}

		public bool SaveBuffer(BufferInfo info)
		{
			bool flag = false;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[9]
				{
				new SqlParameter("@UserID", info.UserID),
				new SqlParameter("@Type", info.Type),
				new SqlParameter("@BeginDate", info.BeginDate),
				new SqlParameter("@Data", (info.Data == null) ? "" : info.Data),
				new SqlParameter("@IsExist", info.IsExist),
				new SqlParameter("@ValidDate", info.ValidDate),
				new SqlParameter("@ValidCount", info.ValidCount),
				new SqlParameter("@Value", info.Value),
				new SqlParameter("@TemplateID", info.TemplateID)
				};
				flag = db.RunProcedure("SP_User_Buff_Add", sqlParameters);
				info.IsDirty = false;
				return flag;
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("Init", exception);
					return flag;
				}
				return flag;
			}
		}

		public bool SaveConsortiaBuffer(ConsortiaBufferInfo info)
		{
			bool flag = false;
			try
			{
				flag = db.RunProcedure("SP_User_Consortia_Buff_Add", new SqlParameter[7]
				{
				new SqlParameter("@ConsortiaID", info.ConsortiaID),
				new SqlParameter("@BufferID", info.BufferID),
				new SqlParameter("@IsOpen", info.IsOpen ? 1 : 0),
				new SqlParameter("@BeginDate", info.BeginDate),
				new SqlParameter("@ValidDate", info.ValidDate),
				new SqlParameter("@Type ", info.Type),
				new SqlParameter("@Value", info.Value)
				});
				return flag;
			}
			catch (Exception ex)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("Init", ex);
					return flag;
				}
				return flag;
			}
		}

		public bool SavePlayerMarryNotice(MarryApplyInfo info, int answerId, ref int id)
		{
			bool flag = false;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[9]
				{
				new SqlParameter("@UserID", info.UserID),
				new SqlParameter("@ApplyUserID", info.ApplyUserID),
				new SqlParameter("@ApplyUserName", info.ApplyUserName),
				new SqlParameter("@ApplyType", info.ApplyType),
				new SqlParameter("@ApplyResult", info.ApplyResult),
				new SqlParameter("@LoveProclamation", info.LoveProclamation),
				new SqlParameter("@AnswerId", answerId),
				new SqlParameter("@ouototal", SqlDbType.Int),
				null
				};
				sqlParameters[7].Direction = ParameterDirection.Output;
				sqlParameters[8] = new SqlParameter("@Result", SqlDbType.Int);
				sqlParameters[8].Direction = ParameterDirection.ReturnValue;
				db.RunProcedure("SP_Insert_Marry_Notice", sqlParameters);
				id = (int)sqlParameters[7].Value;
				flag = (int)sqlParameters[8].Value == 0;
				return flag;
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("SavePlayerMarryNotice", exception);
					return flag;
				}
				return flag;
			}
		}

		public bool ScanAuction(ref string noticeUserID, double cess)
		{
			bool flag = false;
			try
			{
				SqlParameter[] SqlParameters = new SqlParameter[2]
				{
				new SqlParameter("@NoticeUserID", SqlDbType.NVarChar, 4000),
				null
				};
				SqlParameters[0].Direction = ParameterDirection.Output;
				SqlParameters[1] = new SqlParameter("@Cess", cess);
				db.RunProcedure("SP_Auction_Scan", SqlParameters);
				noticeUserID = SqlParameters[0].Value.ToString();
				flag = true;
				return flag;
			}
			catch (Exception ex)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("Init", ex);
					return flag;
				}
				return flag;
			}
		}

		public bool ScanMail(ref string noticeUserID)
		{
			bool flag = false;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[1]
				{
				new SqlParameter("@NoticeUserID", SqlDbType.NVarChar, 4000)
				};
				sqlParameters[0].Direction = ParameterDirection.Output;
				db.RunProcedure("SP_Mail_Scan", sqlParameters);
				noticeUserID = sqlParameters[0].Value.ToString();
				flag = true;
				return flag;
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("Init", exception);
					return flag;
				}
				return flag;
			}
		}

		public bool SendMail(MailInfo mail)
		{
			bool flag = false;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[29];
				sqlParameters[0] = new SqlParameter("@ID", mail.ID);
				sqlParameters[0].Direction = ParameterDirection.Output;
				sqlParameters[1] = new SqlParameter("@Annex1", (mail.Annex1 == null) ? "" : mail.Annex1);
				sqlParameters[2] = new SqlParameter("@Annex2", (mail.Annex2 == null) ? "" : mail.Annex2);
				sqlParameters[3] = new SqlParameter("@Content", (mail.Content == null) ? "" : mail.Content);
				sqlParameters[4] = new SqlParameter("@Gold", mail.Gold);
				sqlParameters[5] = new SqlParameter("@IsExist", true);
				sqlParameters[6] = new SqlParameter("@Money", mail.Money);
				sqlParameters[7] = new SqlParameter("@Receiver", (mail.Receiver == null) ? "" : mail.Receiver);
				sqlParameters[8] = new SqlParameter("@ReceiverID", mail.ReceiverID);
				sqlParameters[9] = new SqlParameter("@Sender", (mail.Sender == null) ? "" : mail.Sender);
				sqlParameters[10] = new SqlParameter("@SenderID", mail.SenderID);
				sqlParameters[11] = new SqlParameter("@Title", (mail.Title == null) ? "" : mail.Title);
				sqlParameters[12] = new SqlParameter("@IfDelS", false);
				sqlParameters[13] = new SqlParameter("@IsDelete", false);
				sqlParameters[14] = new SqlParameter("@IsDelR", false);
				sqlParameters[15] = new SqlParameter("@IsRead", false);
				sqlParameters[16] = new SqlParameter("@SendTime", DateTime.Now);
				sqlParameters[17] = new SqlParameter("@Type", mail.Type);
				sqlParameters[18] = new SqlParameter("@Annex1Name", (mail.Annex1Name == null) ? "" : mail.Annex1Name);
				sqlParameters[19] = new SqlParameter("@Annex2Name", (mail.Annex2Name == null) ? "" : mail.Annex2Name);
				sqlParameters[20] = new SqlParameter("@Annex3", (mail.Annex3 == null) ? "" : mail.Annex3);
				sqlParameters[21] = new SqlParameter("@Annex4", (mail.Annex4 == null) ? "" : mail.Annex4);
				sqlParameters[22] = new SqlParameter("@Annex5", (mail.Annex5 == null) ? "" : mail.Annex5);
				sqlParameters[23] = new SqlParameter("@Annex3Name", (mail.Annex3Name == null) ? "" : mail.Annex3Name);
				sqlParameters[24] = new SqlParameter("@Annex4Name", (mail.Annex4Name == null) ? "" : mail.Annex4Name);
				sqlParameters[25] = new SqlParameter("@Annex5Name", (mail.Annex5Name == null) ? "" : mail.Annex5Name);
				sqlParameters[26] = new SqlParameter("@ValidDate", mail.ValidDate);
				sqlParameters[27] = new SqlParameter("@AnnexRemark", (mail.AnnexRemark == null) ? "" : mail.AnnexRemark);
				sqlParameters[28] = new SqlParameter("@GiftToken", mail.GiftToken);
				flag = db.RunProcedure("SP_Mail_Send", sqlParameters);
				mail.ID = (int)sqlParameters[0].Value;
				using (CenterServiceClient client = new CenterServiceClient())
				{
					if (client == null) return false;
					else
					{
						client.MailNotice(mail.ReceiverID);
						return flag;
					}
				}
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("Init", exception);
					return flag;
				}
				return flag;
			}
		}

		public int SendMailAndItem(string title, string content, int userID, int gold, int money, string param)
		{
			bool flag = false;
			int num1 = 1;
			int num2 = 0;
			try
			{
				SqlParameter[] SqlParameters = new SqlParameter[8]
				{
				  new SqlParameter("@Title", (object) title),
				  new SqlParameter("@Content", (object) content),
				  new SqlParameter("@UserID", (object) userID),
				  new SqlParameter("@Gold", (object) gold),
				  new SqlParameter("@Money", (object) money),
				  new SqlParameter("@GiftToken", (object) num2),
				  new SqlParameter("@Param", (object) param),
				  new SqlParameter("@Result", SqlDbType.Int)
				};
				SqlParameters[7].Direction = ParameterDirection.ReturnValue;
				flag = this.db.RunProcedure("SP_Admin_SendAllItem", SqlParameters);
				num1 = (int)SqlParameters[7].Value;
				if (num1 == 0)
				{
					using (CenterServiceClient client = new CenterServiceClient())
					{
						client.MailNotice(userID);
						return num1;
					}
				}
				return num1;
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("Init", exception);
					return num1;
				}
				return num1;
			}
		}

		public bool UpdateAuction(AuctionInfo info, double cess)
		{
			bool flag = false;
			try
			{
				SqlParameter[] SqlParameters = new SqlParameter[17]
				{
				new SqlParameter("@AuctionID", info.AuctionID),
				new SqlParameter("@AuctioneerID", info.AuctioneerID),
				new SqlParameter("@AuctioneerName", (info.AuctioneerName == null) ? "" : info.AuctioneerName),
				new SqlParameter("@BeginDate", info.BeginDate),//.ToString("MM/dd/yyyy HH:mm:ss")),
				new SqlParameter("@BuyerID", info.BuyerID),
				new SqlParameter("@BuyerName", (info.BuyerName == null) ? "" : info.BuyerName),
				new SqlParameter("@IsExist", info.IsExist),
				new SqlParameter("@ItemID", info.ItemID),
				new SqlParameter("@Mouthful", info.Mouthful),
				new SqlParameter("@PayType", info.PayType),
				new SqlParameter("@Price", info.Price),
				new SqlParameter("@Rise", info.Rise),
				new SqlParameter("@ValidDate", info.ValidDate),
				new SqlParameter("@Name", info.Name),
				new SqlParameter("@Category", info.Category),
				null,
				new SqlParameter("@Cess", cess)
				};
				SqlParameters[15] = new SqlParameter("@Result", SqlDbType.Int);
				SqlParameters[15].Direction = ParameterDirection.ReturnValue;
				db.RunProcedure("SP_Auction_Update", SqlParameters);
				flag = (int)SqlParameters[15].Value == 0;
				return flag;
			}
			catch (Exception ex)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("Init", ex);
					return flag;
				}
				return flag;
			}
		}

		public bool UpdateUsersEventProcess(EventRewardProcessInfo info)
		{
			bool flag = false;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[5]
				{
				new SqlParameter("@UserID", info.UserID),
				new SqlParameter("@ActiveType", info.ActiveType),
				new SqlParameter("@Conditions", info.Conditions),
				new SqlParameter("@AwardGot", info.AwardGot),
				new SqlParameter("@Result", SqlDbType.Int)
				};
				sqlParameters[4].Direction = ParameterDirection.ReturnValue;
				db.RunProcedure("SP_UpdateUsersEventProcess", sqlParameters);
				flag = (int)sqlParameters[4].Value == 0;
				return flag;
			}
			catch (Exception exception)
			{
				BaseBussiness.log.Error("SP_UpdateUsersEventProcess", exception);
				return flag;
			}
		}

		public bool UpdateBreakTimeWhereServerStop()
		{
			bool flag = false;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[1]
				{
				new SqlParameter("@Result", SqlDbType.Int)
				};
				sqlParameters[0].Direction = ParameterDirection.ReturnValue;
				db.RunProcedure("SP_Update_Marry_Room_Info_Sever_Stop", sqlParameters);
				flag = (int)sqlParameters[0].Value == 0;
				return flag;
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("UpdateBreakTimeWhereServerStop", exception);
					return flag;
				}
				return flag;
			}
		}

		public bool UpdateCards(UsersCardInfo item)
		{
			bool result = false;
			try
            {
				SqlParameter[] para = new SqlParameter[19];
				para[0] = new SqlParameter("@CardID", item.CardID);
				para[1] = new SqlParameter("@UserID", item.UserID);
				para[2] = new SqlParameter("@TemplateID", item.TemplateID);
				para[3] = new SqlParameter("@Place", item.Place);
				para[4] = new SqlParameter("@Count", item.Count);
				para[5] = new SqlParameter("@Attack", item.Attack);
				para[6] = new SqlParameter("@Defence", item.Defence);
				para[7] = new SqlParameter("@Agility", item.Agility);
				para[8] = new SqlParameter("@Luck", item.Luck);
				para[9] = new SqlParameter("@Guard", item.Guard);
				para[10] = new SqlParameter("@Damage", item.Damage);
				para[11] = new SqlParameter("@Level", item.Level);
				para[12] = new SqlParameter("@CardGP", item.CardGP);
				para[13] = new SqlParameter("@AttackReset", item.AttackReset);
				para[14] = new SqlParameter("@DefenceReset", item.DefenceReset);
				para[15] = new SqlParameter("@AgilityReset", item.AgilityReset);
				para[16] = new SqlParameter("@LuckReset", item.LuckReset);
				para[17] = new SqlParameter("@isFirstGet", item.isFirstGet);
				para[18] = new SqlParameter("@Result", System.Data.SqlDbType.Int);
				para[18].Direction = ParameterDirection.ReturnValue;
				db.RunProcedure("SP_UpdateUserCard", para);
				int returnValue = (int)para[18].Value;
				result = returnValue == 0;
            }
			catch (Exception ex)
            {
				if (log.IsErrorEnabled)
					log.Error("Init", ex);
            }
			return result;
            #region OLD
            /*bool flag = false;
			try
			{
				SqlParameter[] SqlParameters = new SqlParameter[19]
				{
				new SqlParameter("@CardID", item.CardID),
				new SqlParameter("@UserID", item.UserID),
				new SqlParameter("@TemplateID", item.TemplateID),
				new SqlParameter("@Place", item.Place),
				new SqlParameter("@Count", item.Count),
				new SqlParameter("@Attack", item.Attack),
				new SqlParameter("@Defence", item.Defence),
				new SqlParameter("@Agility", item.Agility),
				new SqlParameter("@Luck", item.Luck),
				new SqlParameter("@Guard", item.Guard),
				new SqlParameter("@Damage", item.Damage),
				new SqlParameter("@Level", item.Level),
				new SqlParameter("@CardGP", item.CardGP),
				null,
				new SqlParameter("@AttackReset", item.AttackReset),
				new SqlParameter("@DefenceReset", item.DefenceReset),
				new SqlParameter("@AgilityReset", item.AgilityReset),
				new SqlParameter("@LuckReset", item.LuckReset),
				new SqlParameter("@isFirstGet", item.isFirstGet)
				};
				SqlParameters[13] = new SqlParameter("@Result", SqlDbType.Int);
				SqlParameters[13].Direction = ParameterDirection.ReturnValue;
				db.RunProcedure("SP_UpdateUserCard", SqlParameters);
				flag = (int)SqlParameters[13].Value == 0;
				return flag;
			}
			catch (Exception ex)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("SP_UpdateUserCard", ex);
					return flag;
				}
				return flag;
			}*/
            #endregion
        }

		public bool UpdateDbAchievementDataInfo(AchievementDataInfo info)
		{
			bool result = false;
			try
			{
				SqlParameter[] para = new SqlParameter[4]
				{
				new SqlParameter("@UserID", info.UserID),
				new SqlParameter("@AchievementID", info.AchievementID),
				new SqlParameter("@IsComplete", info.IsComplete),
				new SqlParameter("@CompletedDate", info.CompletedDate)
				};
				result = db.RunProcedure("SP_Achievement_Data_Add", para);
				info.IsDirty = false;
				return result;
			}
			catch (Exception e)
			{
				BaseBussiness.log.Error("Init_UpdateDbAchievementDataInfo", e);
				return result;
			}
		}

		public List<AchievementDataInfo> GetUserAchievementData(int userID)
		{
			List<AchievementDataInfo> infos = new List<AchievementDataInfo>();
			SqlDataReader reader = null;
			try
			{
				SqlParameter[] para = new SqlParameter[1]
				{
				new SqlParameter("@UserID", SqlDbType.Int, 4)
				};
				para[0].Value = userID;
				db.GetReader(ref reader, "SP_Achievement_Data_All", para);
				while (reader.Read())
				{
					infos.Add(new AchievementDataInfo
					{
						UserID = (int)reader["UserID"],
						AchievementID = (int)reader["AchievementID"],
						IsComplete = (bool)reader["IsComplete"],
						CompletedDate = (DateTime)reader["CompletedDate"],
						IsDirty = false
					});
				}
				return infos;
			}
			catch (Exception e)
			{
				BaseBussiness.log.Error("Init_GetUserAchievement", e);
				return infos;
			}
			finally
			{
				if (reader != null && !reader.IsClosed)
				{
					reader.Close();
				}
			}
		}

		public List<AchievementDataInfo> GetUserAchievementData(int userID, int id)
		{
			List<AchievementDataInfo> infos = new List<AchievementDataInfo>();
			SqlDataReader reader = null;
			try
			{
				SqlParameter[] para = new SqlParameter[2]
				{
				new SqlParameter("@UserID", SqlDbType.Int, 4),
				new SqlParameter("@AchievementID", id)
				};
				para[0].Value = userID;
				db.GetReader(ref reader, "SP_Achievement_Data_Single", para);
				while (reader.Read())
				{
					infos.Add(new AchievementDataInfo
					{
						UserID = (int)reader["UserID"],
						AchievementID = (int)reader["AchievementID"],
						IsComplete = (bool)reader["IsComplete"],
						CompletedDate = (DateTime)reader["CompletedDate"],
						IsDirty = false
					});
				}
				return infos;
			}
			catch (Exception e)
			{
				BaseBussiness.log.Error("Init_GetUserAchievementSingle", e);
				return infos;
			}
			finally
			{
				if (reader != null && !reader.IsClosed)
				{
					reader.Close();
				}
			}
		}

		public List<UsersRecordInfo> GetUserRecord(int userID)
		{
			List<UsersRecordInfo> infos = new List<UsersRecordInfo>();
			SqlDataReader reader = null;
			try
			{
				SqlParameter[] para = new SqlParameter[1]
				{
				new SqlParameter("@UserID", SqlDbType.Int, 4)
				};
				para[0].Value = userID;
				db.GetReader(ref reader, "SP_Users_Record_All", para);
				while (reader.Read())
				{
					infos.Add(new UsersRecordInfo
					{
						UserID = (int)reader["UserID"],
						RecordID = (int)reader["RecordID"],
						Total = (int)reader["Total"],
						IsDirty = false
					});
				}
				return infos;
			}
			catch (Exception e)
			{
				BaseBussiness.log.Error("Init_GetUserRecord", e);
				return infos;
			}
			finally
			{
				if (reader != null && !reader.IsClosed)
				{
					reader.Close();
				}
			}
		}

		public bool UpdateDbUserRecord(UsersRecordInfo info)
		{
			bool result = false;
			try
			{
				SqlParameter[] para = new SqlParameter[3]
				{
				new SqlParameter("@UserID", info.UserID),
				new SqlParameter("@RecordID", info.RecordID),
				new SqlParameter("@Total", info.Total)
				};
				result = db.RunProcedure("SP_Users_Record_Add", para);
				info.IsDirty = false;
				return result;
			}
			catch (Exception e)
			{
				BaseBussiness.log.Error("Init_UpdateDbUserRecord", e);
				return result;
			}
		}

		public bool UpdateDbQuestDataInfo(QuestDataInfo info)
		{
			bool flag = false;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[11]
				{
				new SqlParameter("@UserID", info.UserID),
				new SqlParameter("@QuestID", info.QuestID),
				new SqlParameter("@CompletedDate", info.CompletedDate),
				new SqlParameter("@IsComplete", info.IsComplete),
				new SqlParameter("@Condition1", (info.Condition1 > -1) ? info.Condition1 : 0),
				new SqlParameter("@Condition2", (info.Condition2 > -1) ? info.Condition2 : 0),
				new SqlParameter("@Condition3", (info.Condition3 > -1) ? info.Condition3 : 0),
				new SqlParameter("@Condition4", (info.Condition4 > -1) ? info.Condition4 : 0),
				new SqlParameter("@IsExist", info.IsExist),
				new SqlParameter("@RepeatFinish", (info.RepeatFinish == -1) ? 1 : info.RepeatFinish),
				new SqlParameter("@RandDobule", info.RandDobule)
				};
				flag = db.RunProcedure("SP_QuestData_Add", sqlParameters);
				info.IsDirty = false;
				return flag;
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("Init", exception);
					return flag;
				}
				return flag;
			}
		}

		public bool UpdateGoods(ItemInfo item)
		{
			bool flag = false;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[42]
				{
				new SqlParameter("@ItemID", item.ItemID),
				new SqlParameter("@UserID", item.UserID),
				new SqlParameter("@TemplateID", item.Template.TemplateID),
				new SqlParameter("@Place", item.Place),
				new SqlParameter("@AgilityCompose", item.AgilityCompose),
				new SqlParameter("@AttackCompose", item.AttackCompose),
				new SqlParameter("@BeginDate", item.BeginDate),
				new SqlParameter("@Color", (item.Color == null) ? "" : item.Color),
				new SqlParameter("@Count", item.Count),
				new SqlParameter("@DefendCompose", item.DefendCompose),
				new SqlParameter("@IsBinds", item.IsBinds),
				new SqlParameter("@IsExist", item.IsExist),
				new SqlParameter("@IsJudge", item.IsJudge),
				new SqlParameter("@LuckCompose", item.LuckCompose),
				new SqlParameter("@StrengthenLevel", item.StrengthenLevel),
				new SqlParameter("@ValidDate", item.ValidDate),
				new SqlParameter("@BagType", item.BagType),
				new SqlParameter("@Skin", item.Skin),
				new SqlParameter("@IsUsed", item.IsUsed),
				new SqlParameter("@RemoveDate", item.RemoveDate),
				new SqlParameter("@RemoveType", item.RemoveType),
				new SqlParameter("@Hole1", item.Hole1),
				new SqlParameter("@Hole2", item.Hole2),
				new SqlParameter("@Hole3", item.Hole3),
				new SqlParameter("@Hole4", item.Hole4),
				new SqlParameter("@Hole5", item.Hole5),
				new SqlParameter("@Hole6", item.Hole6),
				new SqlParameter("@StrengthenTimes", item.StrengthenTimes),
				new SqlParameter("@Hole5Level", item.Hole5Level),
				new SqlParameter("@Hole5Exp", item.Hole5Exp),
				new SqlParameter("@Hole6Level", item.Hole6Level),
				new SqlParameter("@Hole6Exp", item.Hole6Exp),
				new SqlParameter("@IsGold", item.IsGold),
				new SqlParameter("@goldBeginTime", item.goldBeginTime),
				new SqlParameter("@goldValidDate", item.goldValidDate),
				new SqlParameter("@StrengthenExp", item.StrengthenExp),
				new SqlParameter("@Blood", item.Blood),
				new SqlParameter("@latentEnergyCurStr", item.latentEnergyCurStr),
				new SqlParameter("@latentEnergyNewStr", item.latentEnergyNewStr),
				new SqlParameter("@latentEnergyEndTime", item.latentEnergyEndTime),
				new SqlParameter("@cellLocked", item.cellLocked),
				new SqlParameter("@curExp", item.curExp)
				};
				flag = db.RunProcedure("SP_Users_Items_Update", sqlParameters);
				item.IsDirty = false;
				return flag;
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("Init", exception);
					return flag;
				}
				return flag;
			}
		}

		public bool UpdateLastVIPPackTime(int ID)
		{
			bool flag = false;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[3]
				{
				new SqlParameter("@UserID", ID),
				new SqlParameter("@LastVIPPackTime", DateTime.Now.Date),
				new SqlParameter("@Result", SqlDbType.Int)
				};
				sqlParameters[2].Direction = ParameterDirection.ReturnValue;
				db.RunProcedure("SP_UpdateUserLastVIPPackTime", sqlParameters);
				flag = true;
				return flag;
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("SP_UpdateUserLastVIPPackTime", exception);
					return flag;
				}
				return flag;
			}
		}

		public bool UpdateMail(MailInfo mail, int oldMoney)
		{
			bool flag = false;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[30]
				{
				new SqlParameter("@ID", mail.ID),
				new SqlParameter("@Annex1", (mail.Annex1 == null) ? "" : mail.Annex1),
				new SqlParameter("@Annex2", (mail.Annex2 == null) ? "" : mail.Annex2),
				new SqlParameter("@Content", (mail.Content == null) ? "" : mail.Content),
				new SqlParameter("@Gold", mail.Gold),
				new SqlParameter("@IsExist", mail.IsExist),
				new SqlParameter("@Money", mail.Money),
				new SqlParameter("@Receiver", (mail.Receiver == null) ? "" : mail.Receiver),
				new SqlParameter("@ReceiverID", mail.ReceiverID),
				new SqlParameter("@Sender", (mail.Sender == null) ? "" : mail.Sender),
				new SqlParameter("@SenderID", mail.SenderID),
				new SqlParameter("@Title", (mail.Title == null) ? "" : mail.Title),
				new SqlParameter("@IfDelS", false),
				new SqlParameter("@IsDelete", false),
				new SqlParameter("@IsDelR", false),
				new SqlParameter("@IsRead", mail.IsRead),
				new SqlParameter("@SendTime", mail.SendTime),
				new SqlParameter("@Type", mail.Type),
				new SqlParameter("@OldMoney", oldMoney),
				new SqlParameter("@ValidDate", mail.ValidDate),
				new SqlParameter("@Annex1Name", mail.Annex1Name),
				new SqlParameter("@Annex2Name", mail.Annex2Name),
				new SqlParameter("@Result", SqlDbType.Int),
				null,
				null,
				null,
				null,
				null,
				null,
				null
				};
				sqlParameters[22].Direction = ParameterDirection.ReturnValue;
				sqlParameters[23] = new SqlParameter("@Annex3", (mail.Annex3 == null) ? "" : mail.Annex3);
				sqlParameters[24] = new SqlParameter("@Annex4", (mail.Annex4 == null) ? "" : mail.Annex4);
				sqlParameters[25] = new SqlParameter("@Annex5", (mail.Annex5 == null) ? "" : mail.Annex5);
				sqlParameters[26] = new SqlParameter("@Annex3Name", (mail.Annex3Name == null) ? "" : mail.Annex3Name);
				sqlParameters[27] = new SqlParameter("@Annex4Name", (mail.Annex4Name == null) ? "" : mail.Annex4Name);
				sqlParameters[28] = new SqlParameter("@Annex5Name", (mail.Annex5Name == null) ? "" : mail.Annex5Name);
				sqlParameters[29] = new SqlParameter("GiftToken", mail.GiftToken);
				db.RunProcedure("SP_Mail_Update", sqlParameters);
				flag = (int)sqlParameters[22].Value == 0;
				return flag;
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("Init", exception);
					return flag;
				}
				return flag;
			}
		}

		public bool UpdateMarryInfo(MarryInfo info)
		{
			bool flag = false;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[6]
				{
				new SqlParameter("@ID", info.ID),
				new SqlParameter("@UserID", info.UserID),
				new SqlParameter("@IsPublishEquip", info.IsPublishEquip),
				new SqlParameter("@Introduction", info.Introduction),
				new SqlParameter("@RegistTime", info.RegistTime.ToString("yyyy-MM-dd HH:mm:ss")),
				new SqlParameter("@Result", SqlDbType.Int)
				};
				sqlParameters[5].Direction = ParameterDirection.ReturnValue;
				db.RunProcedure("SP_MarryInfo_Update", sqlParameters);
				flag = (int)sqlParameters[5].Value == 0;
				return flag;
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("Init", exception);
					return flag;
				}
				return flag;
			}
		}

		public bool UpdateMarryRoomInfo(MarryRoomInfo info)
		{
			bool flag = false;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[9]
				{
				new SqlParameter("@ID", info.ID),
				new SqlParameter("@AvailTime", info.AvailTime),
				new SqlParameter("@BreakTime", info.BreakTime),
				new SqlParameter("@roomIntroduction", info.RoomIntroduction),
				new SqlParameter("@isHymeneal", info.IsHymeneal),
				new SqlParameter("@Name", info.Name),
				new SqlParameter("@Pwd", info.Pwd),
				new SqlParameter("@IsGunsaluteUsed", info.IsGunsaluteUsed),
				new SqlParameter("@Result", SqlDbType.Int)
				};
				sqlParameters[8].Direction = ParameterDirection.ReturnValue;
				db.RunProcedure("SP_Update_Marry_Room_Info", sqlParameters);
				flag = (int)sqlParameters[8].Value == 0;
				return flag;
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("UpdateMarryRoomInfo", exception);
					return flag;
				}
				return flag;
			}
		}

		public bool UpdatePasswordInfo(int userID, string PasswordQuestion1, string PasswordAnswer1, string PasswordQuestion2, string PasswordAnswer2, int Count)
		{
			bool flag = false;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[6]
				{
				new SqlParameter("@UserID", userID),
				new SqlParameter("@PasswordQuestion1", PasswordQuestion1),
				new SqlParameter("@PasswordAnswer1", PasswordAnswer1),
				new SqlParameter("@PasswordQuestion2", PasswordQuestion2),
				new SqlParameter("@PasswordAnswer2", PasswordAnswer2),
				new SqlParameter("@FailedPasswordAttemptCount", Count)
				};
				flag = db.RunProcedure("SP_Users_Password_Add", sqlParameters);
				return flag;
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("Init", exception);
					return flag;
				}
				return flag;
			}
		}

		public bool UpdatePasswordTwo(int userID, string passwordTwo)
		{
			bool flag = false;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[2]
				{
				new SqlParameter("@UserID", userID),
				new SqlParameter("@PasswordTwo", passwordTwo)
				};
				flag = db.RunProcedure("SP_Users_UpdatePasswordTwo", sqlParameters);
				return flag;
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("Init", exception);
					return flag;
				}
				return flag;
			}
		}

		public bool UpdatePlayer(PlayerInfo player)
		{
			bool flag = false;
			try
			{
				if (player.Grade < 1)
				{
					return flag;
				}
				SqlParameter[] sqlParameters = new SqlParameter[111]
				{
				new SqlParameter("@UserID", player.ID),
				new SqlParameter("@Attack", player.Attack),
				new SqlParameter("@Colors", (player.Colors == null) ? "" : player.Colors),
				new SqlParameter("@ConsortiaID", player.ConsortiaID),
				new SqlParameter("@Defence", player.Defence),
				new SqlParameter("@Gold", player.Gold),
				new SqlParameter("@GP", player.GP),
				new SqlParameter("@Grade", player.Grade),
				new SqlParameter("@Luck", player.Luck),
				new SqlParameter("@Money", player.Money),
				new SqlParameter("@Style", (player.Style == null) ? "" : player.Style),
				new SqlParameter("@Agility", player.Agility),
				new SqlParameter("@State", player.State),
				new SqlParameter("@Hide", player.Hide),
				new SqlParameter("@ExpendDate", (!player.ExpendDate.HasValue) ? "" : player.ExpendDate.ToString()),
				new SqlParameter("@Win", player.Win),
				new SqlParameter("@Total", player.Total),
				new SqlParameter("@Escape", player.Escape),
				new SqlParameter("@Skin", (player.Skin == null) ? "" : player.Skin),
				new SqlParameter("@Offer", player.Offer),
				new SqlParameter("@AntiAddiction", player.AntiAddiction),
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null
				};
				sqlParameters[20].Direction = ParameterDirection.InputOutput;
				sqlParameters[21] = new SqlParameter("@Result", SqlDbType.Int);
				sqlParameters[21].Direction = ParameterDirection.ReturnValue;
				sqlParameters[22] = new SqlParameter("@RichesOffer", player.RichesOffer);
				sqlParameters[23] = new SqlParameter("@RichesRob", player.RichesRob);
				sqlParameters[24] = new SqlParameter("@CheckCount", player.CheckCount);
				sqlParameters[24].Direction = ParameterDirection.InputOutput;
				sqlParameters[25] = new SqlParameter("@MarryInfoID", player.MarryInfoID);
				sqlParameters[26] = new SqlParameter("@DayLoginCount", player.DayLoginCount);
				sqlParameters[27] = new SqlParameter("@Nimbus", player.Nimbus);
				sqlParameters[28] = new SqlParameter("@LastAward", player.LastAward);
				sqlParameters[29] = new SqlParameter("@GiftToken", player.GiftToken);
				sqlParameters[30] = new SqlParameter("@QuestSite", player.QuestSite);
				sqlParameters[31] = new SqlParameter("@PvePermission", player.PvePermission);
				sqlParameters[32] = new SqlParameter("@FightPower", player.FightPower);
				sqlParameters[33] = new SqlParameter("@AnswerSite", player.AnswerSite);
				sqlParameters[34] = new SqlParameter("@LastAuncherAward", player.LastAward);
				sqlParameters[35] = new SqlParameter("@hp", player.hp);
				sqlParameters[36] = new SqlParameter("@ChatCount", player.ChatCount);
				sqlParameters[37] = new SqlParameter("@SpaPubGoldRoomLimit", player.SpaPubGoldRoomLimit);
				sqlParameters[38] = new SqlParameter("@LastSpaDate", player.LastSpaDate);
				sqlParameters[39] = new SqlParameter("@FightLabPermission", player.FightLabPermission);
				sqlParameters[40] = new SqlParameter("@SpaPubMoneyRoomLimit", player.SpaPubMoneyRoomLimit);
				sqlParameters[41] = new SqlParameter("@IsInSpaPubGoldToday", player.IsInSpaPubGoldToday);
				sqlParameters[42] = new SqlParameter("@IsInSpaPubMoneyToday", player.IsInSpaPubMoneyToday);
				sqlParameters[43] = new SqlParameter("@AchievementPoint", player.AchievementPoint);
				sqlParameters[44] = new SqlParameter("@LastWeekly", player.LastWeekly);
				sqlParameters[45] = new SqlParameter("@LastWeeklyVersion", player.LastWeeklyVersion);
				sqlParameters[46] = new SqlParameter("@WeaklessGuildProgressStr", player.WeaklessGuildProgressStr);
				sqlParameters[47] = new SqlParameter("@IsOldPlayer", player.IsOldPlayer);
				sqlParameters[48] = new SqlParameter("@VIPLevel", player.VIPLevel);
				sqlParameters[49] = new SqlParameter("@VIPExp", player.VIPExp);
				sqlParameters[50] = new SqlParameter("@Score", player.Score);
				sqlParameters[51] = new SqlParameter("@OptionOnOff", player.OptionOnOff);
				sqlParameters[52] = new SqlParameter("@isOldPlayerHasValidEquitAtLogin", player.isOldPlayerHasValidEquitAtLogin);
				sqlParameters[53] = new SqlParameter("@badLuckNumber", player.badLuckNumber);
				sqlParameters[54] = new SqlParameter("@luckyNum", player.luckyNum);
				sqlParameters[55] = new SqlParameter("@lastLuckyNumDate", player.lastLuckyNumDate);
				sqlParameters[56] = new SqlParameter("@lastLuckNum", player.lastLuckNum);
				sqlParameters[57] = new SqlParameter("@IsShowConsortia", player.IsShowConsortia);
				sqlParameters[58] = new SqlParameter("@NewDay", player.NewDay);
				sqlParameters[59] = new SqlParameter("@Medal", player.medal);
				sqlParameters[60] = new SqlParameter("@Honor", player.Honor);
				sqlParameters[61] = new SqlParameter("@VIPNextLevelDaysNeeded", player.GetVIPNextLevelDaysNeeded(player.VIPLevel, player.VIPExp));
				sqlParameters[62] = new SqlParameter("@IsRecharged", player.IsRecharged);
				sqlParameters[63] = new SqlParameter("@IsGetAward", player.IsGetAward);
				sqlParameters[64] = new SqlParameter("@typeVIP", player.typeVIP);
				sqlParameters[65] = new SqlParameter("@evolutionGrade", player.evolutionGrade);
				sqlParameters[66] = new SqlParameter("@evolutionExp", player.evolutionExp);
				sqlParameters[67] = new SqlParameter("@hardCurrency", player.hardCurrency);
				sqlParameters[68] = new SqlParameter("@EliteScore", player.EliteScore);
				sqlParameters[69] = new SqlParameter("@UseOffer", player.UseOffer);
				sqlParameters[70] = new SqlParameter("@ShopFinallyGottenTime", player.ShopFinallyGottenTime);
				sqlParameters[71] = new SqlParameter("@MoneyLock", player.MoneyLock);
				sqlParameters[72] = new SqlParameter("@LastGetEgg", player.LastGetEgg);
				sqlParameters[73] = new SqlParameter("@IsFistGetPet", player.IsFistGetPet);
				sqlParameters[74] = new SqlParameter("@LastRefreshPet", player.LastRefreshPet.ToString());
				sqlParameters[75] = new SqlParameter("@petScore", player.petScore);
				sqlParameters[76] = new SqlParameter("@accumulativeLoginDays", player.accumulativeLoginDays);
				sqlParameters[77] = new SqlParameter("@accumulativeAwardDays", player.accumulativeAwardDays);
				sqlParameters[78] = new SqlParameter("@honorId", player.honorId);
				sqlParameters[79] = new SqlParameter("@LoginDevice", player.LoginDevice);
				sqlParameters[80] = new SqlParameter("@charmGP", player.charmGP);
				sqlParameters[81] = new SqlParameter("@charmLevel", player.charmLevel);
				sqlParameters[82] = new SqlParameter("@MaxBuyHonor", player.MaxBuyHonor);
				sqlParameters[83] = new SqlParameter("@myHonor", player.myHonor);
				sqlParameters[84] = new SqlParameter("@totemId", player.totemId);
				sqlParameters[85] = new SqlParameter("@necklaceExp", player.necklaceExp);
				sqlParameters[86] = new SqlParameter("@necklaceExpAdd", player.necklaceExpAdd);
				sqlParameters[87] = new SqlParameter("@RingExp", player.RingExp);
				sqlParameters[88] = new SqlParameter("@fineSuitExp", player.fineSuitExp);
				sqlParameters[89] = new SqlParameter("@GhostEquipList", player.GhostEquipList);
				sqlParameters[90] = new SqlParameter("@damageScores", player.damageScores);
				sqlParameters[91] = new SqlParameter("@DateFinishTask", player.DateFinishTask);
				sqlParameters[92] = new SqlParameter("@MyInviteCode", player.MyInviteCode);
				sqlParameters[93] = new SqlParameter("@MyInvitedCount", player.MyInvitedCount);
				sqlParameters[94] = new SqlParameter("@AwardColumnOne", player.AwardColumnOne);
				sqlParameters[95] = new SqlParameter("@AwardColumnTwo", player.AwardColumnTwo);
				sqlParameters[96] = new SqlParameter("@AwardColumnThree", player.AwardColumnThree);
				sqlParameters[97] = new SqlParameter("@AwardColumnFour", player.AwardColumnFour);
				sqlParameters[98] = new SqlParameter("@MyRewardStatus", player.MyRewardStatus);
				sqlParameters[99] = new SqlParameter("@MyCodeStatus", player.MyCodeStatus);
                sqlParameters[100] = new SqlParameter("@DailyScore", player.DailyScore);
				sqlParameters[101] = new SqlParameter("@DailyWinCount", player.DailyWinCount);
				sqlParameters[102] = new SqlParameter("@DailyGameCount", player.DailyGameCount);
				sqlParameters[103] = new SqlParameter("@DailyLeagueFirst", player.DailyLeagueFirst);
				sqlParameters[104] = new SqlParameter("@DailyLeagueLastScore", player.DailyLeagueLastScore);
				sqlParameters[105] = new SqlParameter("@WeeklyScore", player.WeeklyScore);
				sqlParameters[106] = new SqlParameter("@WeeklyGameCount", player.WeeklyGameCount);
				sqlParameters[107] = new SqlParameter("@WeeklyRanking", player.WeeklyRanking);
                sqlParameters[108] = new SqlParameter("@jampsCurrency", player.jampsCurrency);
                sqlParameters[109] = new SqlParameter("@QuitConsortiaDate", player.jampsCurrency);
                sqlParameters[110] = new SqlParameter("@awardGot", player.awardGot);
                db.RunProcedure("SP_Users_Update", sqlParameters);
				flag = (int)sqlParameters[21].Value == 0;
				if (flag)
				{
					player.AntiAddiction = (int)sqlParameters[20].Value;
					player.CheckCount = (int)sqlParameters[24].Value;
				}
				player.IsDirty = false;
				return flag;
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("Init", exception);
					return flag;
				}
				return flag;
			}
		}

		public bool UpdatePlayerGotRingProp(int groomID, int brideID)
		{
			bool flag = false;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[3]
				{
				new SqlParameter("@GroomID", groomID),
				new SqlParameter("@BrideID", brideID),
				new SqlParameter("@Result", SqlDbType.Int)
				};
				sqlParameters[2].Direction = ParameterDirection.ReturnValue;
				db.RunProcedure("SP_Update_GotRing_Prop", sqlParameters);
				flag = (int)sqlParameters[2].Value == 0;
				return flag;
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("UpdatePlayerGotRingProp", exception);
					return flag;
				}
				return flag;
			}
		}

		public bool UpdatePlayerLastAward(int id, int type)
		{
			bool flag = false;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[2]
				{
				new SqlParameter("@UserID", id),
				new SqlParameter("@Type", type)
				};
				flag = db.RunProcedure("SP_Users_LastAward", sqlParameters);
				return flag;
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("UpdatePlayerAward", exception);
					return flag;
				}
				return flag;
			}
		}

		public bool UpdatePlayerMarry(PlayerInfo player)
		{
			bool flag = false;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[7]
				{
				new SqlParameter("@UserID", player.ID),
				new SqlParameter("@IsMarried", player.IsMarried),
				new SqlParameter("@SpouseID", player.SpouseID),
				new SqlParameter("@SpouseName", player.SpouseName),
				new SqlParameter("@IsCreatedMarryRoom", player.IsCreatedMarryRoom),
				new SqlParameter("@SelfMarryRoomID", player.SelfMarryRoomID),
				new SqlParameter("@IsGotRing", player.IsGotRing)
				};
				flag = db.RunProcedure("SP_Users_Marry", sqlParameters);
				return flag;
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("UpdatePlayerMarry", exception);
					return flag;
				}
				return flag;
			}
		}

		public bool UpdatePlayerMarryApply(int UserID, string loveProclamation, bool isExist)
		{
			bool flag = false;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[4]
				{
				new SqlParameter("@UserID", UserID),
				new SqlParameter("@LoveProclamation", loveProclamation),
				new SqlParameter("@isExist", isExist),
				new SqlParameter("@Result", SqlDbType.Int)
				};
				sqlParameters[3].Direction = ParameterDirection.ReturnValue;
				db.RunProcedure("SP_Update_Marry_Apply", sqlParameters);
				flag = (int)sqlParameters[3].Value == 0;
				return flag;
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("UpdatePlayerMarryApply", exception);
					return flag;
				}
				return flag;
			}
		}

		public bool UpdateUserDrillInfo(UserDrillInfo g)
		{
			bool flag = false;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[6]
				{
				new SqlParameter("@UserID", g.UserID),
				new SqlParameter("@BeadPlace", g.BeadPlace),
				new SqlParameter("@HoleExp", g.HoleExp),
				new SqlParameter("@HoleLv", g.HoleLv),
				new SqlParameter("@DrillPlace", g.DrillPlace),
				new SqlParameter("@Result", SqlDbType.Int)
				};
				sqlParameters[5].Direction = ParameterDirection.ReturnValue;
				db.RunProcedure("SP_UpdateUserDrillInfo", sqlParameters);
				flag = true;
				return flag;
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("SP_UpdateUserDrillInfo", exception);
					return flag;
				}
				return flag;
			}
		}

		public bool UpdateUserMatchInfo(UserMatchInfo info)
		{
			bool flag = false;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[16]
				{
				new SqlParameter("@ID", info.ID),
				new SqlParameter("@UserID", info.UserID),
				new SqlParameter("@dailyScore", info.dailyScore),
				new SqlParameter("@dailyWinCount", info.dailyWinCount),
				new SqlParameter("@dailyGameCount", info.dailyGameCount),
				new SqlParameter("@DailyLeagueFirst", info.DailyLeagueFirst),
				new SqlParameter("@DailyLeagueLastScore", info.DailyLeagueLastScore),
				new SqlParameter("@weeklyScore", info.weeklyScore),
				new SqlParameter("@weeklyGameCount", info.weeklyGameCount),
				new SqlParameter("@weeklyRanking", info.weeklyRanking),
				new SqlParameter("@addDayPrestge", info.addDayPrestge),
				new SqlParameter("@totalPrestige", info.totalPrestige),
				new SqlParameter("@restCount", info.restCount),
				new SqlParameter("@leagueGrade", info.leagueGrade),
				new SqlParameter("@leagueItemsGet", info.leagueItemsGet),
				new SqlParameter("@Result", SqlDbType.Int)
				};
				sqlParameters[15].Direction = ParameterDirection.ReturnValue;
				db.RunProcedure("SP_UpdateUserMatch", sqlParameters);
				flag = (int)sqlParameters[15].Value == 0;
				return flag;
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("SP_UpdateUserMatch", exception);
					return flag;
				}
				return flag;
			}
		}

		public bool UpdateUserRank(UserRankInfo item)
		{
			bool result = false;
			try
			{
				SqlParameter[] para = new SqlParameter[16];
				para[0] = new SqlParameter("@ID", item.ID);
				para[1] = new SqlParameter("@UserID", item.UserID);
				para[2] = new SqlParameter("@Name", item.Name);
				para[3] = new SqlParameter("@Attack", item.Attack);
				para[4] = new SqlParameter("@Defence", item.Defence);
				para[5] = new SqlParameter("@Luck", item.Luck);
				para[6] = new SqlParameter("@Agility", item.Agility);
				para[7] = new SqlParameter("@HP", item.HP);
				para[8] = new SqlParameter("@Damage", item.Damage);
				para[9] = new SqlParameter("@Guard", item.Guard);
				para[10] = new SqlParameter("@BeginDate", item.BeginDate);//.ToString("MM/dd/yyyy HH:mm:ss"));
				para[11] = new SqlParameter("@Validate", item.Validate);
				para[12] = new SqlParameter("@IsExit", item.IsExit);
				para[13] = new SqlParameter("@Result", SqlDbType.Int);
				para[13].Direction = ParameterDirection.ReturnValue;
				para[14] = new SqlParameter("@NewTitleID", item.NewTitleID);
				para[15] = new SqlParameter("@EndDate", item.EndDate);
				db.RunProcedure("SP_UpdateUserRank", para);
				result = (int)para[13].Value == 0;
			}
			catch (Exception exception)
			{
				if (log.IsErrorEnabled)
				{
					log.Error("SP_UpdateUserRank", exception);
				}
			}

			return result;
		}

		public bool UpdateUserExtra(UsersExtraInfo ex)
		{
			bool flag = false;
			try
			{
				flag = db.RunProcedure("SP_Update_User_Extra", new SqlParameter[17]
				{
				new SqlParameter("@UserID", ex.UserID),
				new SqlParameter("@LastTimeHotSpring", ex.LastTimeHotSpring),
				new SqlParameter("@MinHotSpring", ex.MinHotSpring),
				new SqlParameter("@coupleBossEnterNum", ex.coupleBossEnterNum),
				new SqlParameter("@coupleBossHurt", ex.coupleBossHurt),
				new SqlParameter("@coupleBossBoxNum", ex.coupleBossBoxNum),
				new SqlParameter("@LastFreeTimeHotSpring", ex.LastFreeTimeHotSpring),
				new SqlParameter("@isGetAwardMarry", ex.isGetAwardMarry),
				new SqlParameter("@isFirstAwardMarry", ex.isFirstAwardMarry),
				new SqlParameter("@LeftRoutteCount", ex.LeftRoutteCount),
				new SqlParameter("@LeftRoutteRate", ex.LeftRoutteRate),
				new SqlParameter("@FreeSendMailCount", ex.FreeSendMailCount),
				new SqlParameter("@BuyTimeHotSpringCount", ex.BuyTimeHotSpringCount),
				new SqlParameter("@BuyCountOpenBoss",ex.BuyCountOpenBoss),
				new SqlParameter("@coupleNum", ex.coupleNum),
				new SqlParameter("@propsNum",ex.propsNum),
				new SqlParameter("@dungeonNum",ex.dungeonNum)
				});
				return flag;
			}
			catch (Exception ex2)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("Init", ex2);
					return flag;
				}
				return flag;
			}
		}

		public bool UpdateUserTexpInfo(TexpInfo info)
		{
			bool flag = false;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[12]
				{
				new SqlParameter("@UserID", info.UserID),
				new SqlParameter("@attTexpExp", info.attTexpExp),
				new SqlParameter("@defTexpExp", info.defTexpExp),
				new SqlParameter("@hpTexpExp", info.hpTexpExp),
				new SqlParameter("@lukTexpExp", info.lukTexpExp),
				new SqlParameter("@spdTexpExp", info.spdTexpExp),
				new SqlParameter("@texpCount", info.texpCount),
				new SqlParameter("@texpTaskCount", info.texpTaskCount),
				new SqlParameter("@texpTaskDate", info.texpTaskDate),
				new SqlParameter("@resetCount", info.resetCount),
				new SqlParameter("@lastReset",info.lastReset),
				new SqlParameter("@Result", SqlDbType.Int)
				};
				sqlParameters[11].Direction = ParameterDirection.ReturnValue;
				db.RunProcedure("SP_UserTexp_Update", sqlParameters);
				flag = (int)sqlParameters[11].Value == 0;
				return flag;
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("Init", exception);
					return flag;
				}
				return flag;
			}
		}

		public bool UpdateVIPInfo(PlayerInfo p)
		{
			bool flag = false;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[10]
				{
				new SqlParameter("@ID", p.ID),
				new SqlParameter("@VIPLevel", p.VIPLevel),
				new SqlParameter("@VIPExp", p.VIPExp),
				new SqlParameter("@VIPOnlineDays", SqlDbType.BigInt),
				new SqlParameter("@VIPOfflineDays", SqlDbType.BigInt),
				new SqlParameter("@VIPExpireDay", p.VIPExpireDay),
				new SqlParameter("@VIPLastDate", DateTime.Now),
				new SqlParameter("@VIPNextLevelDaysNeeded", p.GetVIPNextLevelDaysNeeded(p.VIPLevel, p.VIPExp)),
				new SqlParameter("@CanTakeVipReward", p.CanTakeVipReward),
				new SqlParameter("@Result", SqlDbType.Int)
				};
				sqlParameters[9].Direction = ParameterDirection.ReturnValue;
				db.RunProcedure("SP_UpdateVIPInfo", sqlParameters);
				flag = true;
				return flag;
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("SP_UpdateVIPInfo", exception);
					return flag;
				}
				return flag;
			}
		}

		public int VIPLastdate(int ID)
		{
			int num = 0;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[2]
				{
				new SqlParameter("@UserID", ID),
				new SqlParameter("@Result", SqlDbType.Int)
				};
				sqlParameters[1].Direction = ParameterDirection.ReturnValue;
				db.RunProcedure("SP_VIPLastdate_Single", sqlParameters);
				num = (int)sqlParameters[1].Value;
				return num;
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("SP_VIPLastdate_Single", exception);
					return num;
				}
				return num;
			}
		}

		public int VIPRenewal(string nickName, int renewalDays, int typeVIP, ref DateTime ExpireDayOut)
		{
			int num = 0;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[5]
				{
				new SqlParameter("@NickName", nickName),
				new SqlParameter("@RenewalDays", renewalDays),
				new SqlParameter("@ExpireDayOut", DateTime.Now),
				new SqlParameter("@typeVIP", typeVIP),
				new SqlParameter("@Result", SqlDbType.Int)
				};
				sqlParameters[2].Direction = ParameterDirection.Output;
				sqlParameters[4].Direction = ParameterDirection.ReturnValue;
				db.RunProcedure("SP_VIPRenewal_Single", sqlParameters);
				ExpireDayOut = (DateTime)sqlParameters[2].Value;
				num = (int)sqlParameters[4].Value;
				return num;
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("SP_VIPRenewal_Single", exception);
					return num;
				}
				return num;
			}
		}

		public bool UpdateAcademyPlayer(PlayerInfo player)
		{
			bool flag = false;
			try
			{
				SqlParameter[] SqlParameters = new SqlParameter[8]
				{
				new SqlParameter("@UserID", player.ID),
				new SqlParameter("@apprenticeshipState", player.apprenticeshipState),
				new SqlParameter("@masterID", player.masterID),
				new SqlParameter("@masterOrApprentices", player.masterOrApprentices),
				new SqlParameter("@graduatesCount", player.graduatesCount),
				new SqlParameter("@honourOfMaster", player.honourOfMaster),
				null,
				new SqlParameter("@freezesDate", player.freezesDate)
				};
				SqlParameters[6] = new SqlParameter("@Result", SqlDbType.Int);
				SqlParameters[6].Direction = ParameterDirection.ReturnValue;
				db.RunProcedure("SP_UsersAcademy_Update", SqlParameters);
				flag = (int)SqlParameters[6].Value == 0;
				return flag;
			}
			catch (Exception ex)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("UpdateAcademyPlayer", ex);
					return flag;
				}
				return flag;
			}
		}

		public void AddDailyRecord(DailyRecordInfo info)
		{
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[3]
				{
				new SqlParameter("@UserID", info.UserID),
				new SqlParameter("@Type", info.Type),
				new SqlParameter("@Value", info.Value)
				};
				db.RunProcedure("SP_DailyRecordInfo_Add", sqlParameters);
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("AddDailyRecord", exception);
				}
			}
		}

		public bool DeleteDailyRecord(int UserID, int Type)
		{
			bool flag = false;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[2]
				{
				new SqlParameter("@UserID", UserID),
				new SqlParameter("@Type", Type)
				};
				flag = db.RunProcedure("SP_DailyRecordInfo_Delete", sqlParameters);
				return flag;
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("SP_DailyRecordInfo_Delete", exception);
					return flag;
				}
				return flag;
			}
		}

		public DailyRecordInfo[] GetDailyRecord(int UserID)
		{
			List<DailyRecordInfo> list = new List<DailyRecordInfo>();
			SqlDataReader resultDataReader = null;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[1]
				{
				new SqlParameter("@UserID", UserID)
				};
				db.GetReader(ref resultDataReader, "SP_DailyRecordInfo_Single", sqlParameters);
				while (resultDataReader.Read())
				{
					DailyRecordInfo item = new DailyRecordInfo
					{
						UserID = (int)resultDataReader["UserID"],
						Type = (int)resultDataReader["Type"],
						Value = (string)resultDataReader["Value"]
					};
					list.Add(item);
				}
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("GetDailyRecord", exception);
				}
			}
			finally
			{
				if (resultDataReader != null && !resultDataReader.IsClosed)
				{
					resultDataReader.Close();
				}
			}
			return list.ToArray();
		}

		public string GetASSInfoSingle(int UserID)
		{
			SqlDataReader resultDataReader = null;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[1]
				{
				new SqlParameter("@UserID", UserID)
				};
				db.GetReader(ref resultDataReader, "SP_ASSInfo_Single", sqlParameters);
				if (resultDataReader.Read())
				{
					return resultDataReader["IDNumber"].ToString();
				}
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("GetASSInfoSingle", exception);
				}
			}
			finally
			{
				if (resultDataReader != null && !resultDataReader.IsClosed)
				{
					resultDataReader.Close();
				}
			}
			return "";
		}

		public DailyLogListInfo GetDailyLogListSingle(int UserID)
		{
			SqlDataReader resultDataReader = null;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[1]
				{
				new SqlParameter("@UserID", UserID)
				};
				db.GetReader(ref resultDataReader, "SP_DailyLogList_Single", sqlParameters);
				if (resultDataReader.Read())
				{
					return new DailyLogListInfo
					{
						ID = (int)resultDataReader["ID"],
						UserID = (int)resultDataReader["UserID"],
						UserAwardLog = (int)resultDataReader["UserAwardLog"],
						DayLog = (string)resultDataReader["DayLog"],
						LastDate = (DateTime)resultDataReader["LastDate"]
					};
				}
			}
			catch (Exception exception)
			{
				BaseBussiness.log.Error("DailyLogList", exception);
			}
			finally
			{
				if (resultDataReader != null && !resultDataReader.IsClosed)
				{
					resultDataReader.Close();
				}
			}
			return null;
		}

		public bool UpdateDailyLogList(DailyLogListInfo info)
		{
			bool flag = false;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[5]
				{
				new SqlParameter("@UserID", info.UserID),
				new SqlParameter("@UserAwardLog", info.UserAwardLog),
				new SqlParameter("@DayLog", info.DayLog),
				new SqlParameter("@LastDate", info.LastDate),
				new SqlParameter("@Result", SqlDbType.Int)
				};
				sqlParameters[4].Direction = ParameterDirection.ReturnValue;
				flag = db.RunProcedure("SP_DailyLogList_Update", sqlParameters);
				return flag;
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("SP_DailyLogList_Update", exception);
					return flag;
				}
				return flag;
			}
		}

		public bool UpdateBoxProgression(int userid, int boxProgression, int getBoxLevel, DateTime addGPLastDate, DateTime BoxGetDate, int alreadyBox)
		{
			bool result = false;
			try
			{
				SqlParameter[] para = new SqlParameter[6]
				{
				new SqlParameter("@UserID", userid),
				new SqlParameter("@BoxProgression", boxProgression),
				new SqlParameter("@GetBoxLevel", getBoxLevel),
				new SqlParameter("@AddGPLastDate", DateTime.Now),
				new SqlParameter("@BoxGetDate", BoxGetDate),
				new SqlParameter("@AlreadyGetBox", alreadyBox)
				};
				result = db.RunProcedure("SP_User_Update_BoxProgression", para);
				return result;
			}
			catch (Exception e)
			{
				BaseBussiness.log.Error("User_Update_BoxProgression", e);
				return result;
			}
		}

		public bool UpdatePlayerInfoHistory(PlayerInfoHistory info)
		{
			bool flag = false;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[4]
				{
				new SqlParameter("@UserID", info.UserID),
				new SqlParameter("@LastQuestsTime", info.LastQuestsTime),
				new SqlParameter("@LastTreasureTime", info.LastTreasureTime),
				new SqlParameter("@OutPut", SqlDbType.Int)
				};
				sqlParameters[3].Direction = ParameterDirection.Output;
				db.RunProcedure("SP_User_Update_History", sqlParameters);
				flag = (int)sqlParameters[6].Value == 1;
				return flag;
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("User_Update_BoxProgression", exception);
					return flag;
				}
				return flag;
			}
		}

		public bool AddAASInfo(AASInfo info)
		{
			bool flag = false;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[5]
				{
				new SqlParameter("@UserID", info.UserID),
				new SqlParameter("@Name", info.Name),
				new SqlParameter("@IDNumber", info.IDNumber),
				new SqlParameter("@State", info.State),
				new SqlParameter("@Result", SqlDbType.Int)
				};
				sqlParameters[4].Direction = ParameterDirection.ReturnValue;
				db.RunProcedure("SP_ASSInfo_Add", sqlParameters);
				flag = (int)sqlParameters[4].Value == 0;
				return flag;
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("UpdateAASInfo", exception);
					return flag;
				}
				return flag;
			}
		}

		public void AddUserLogEvent(int UserID, string UserName, string NickName, string Type, string Content)
		{
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[5]
				{
				new SqlParameter("@UserID", UserID),
				new SqlParameter("@UserName", UserName),
				new SqlParameter("@NickName", NickName),
				new SqlParameter("@Type", Type),
				new SqlParameter("@Content", Content)
				};
				db.RunProcedure("SP_Insert_UsersLog", sqlParameters);
			}
			catch (Exception)
			{
			}
		}

		public Dictionary<int, List<string>> LoadCommands()
		{
			SqlDataReader sqlDataReader = null;
			Dictionary<int, List<string>> commands = new Dictionary<int, List<string>>();
			db.GetReader(ref sqlDataReader, "SP_GetAllCommands");
			while (sqlDataReader.Read())
			{
				string[] array = Convert.ToString(sqlDataReader["Commands"] ?? "").Split('$');
				List<string> c = new List<string>();
				string[] array2 = array;
				foreach (string s in array2)
				{
					c.Add(s);
				}
				if (!commands.ContainsKey(Convert.ToInt32(sqlDataReader["UserID"] ?? ((object)0))))
				{
					commands.Add(Convert.ToInt32(sqlDataReader["UserID"] ?? ((object)0)), c);
				}
			}
			return commands;
		}

		public bool AddUserAdoptPet(UsersPetInfo info, bool isUse)
		{
			bool result = false;
			try
			{
				SqlParameter[] para = new SqlParameter[23];
				para[0] = new SqlParameter("@TemplateID", info.TemplateID);
				para[1] = new SqlParameter("@Name", info.Name == null ? "Error!" : info.Name);
				para[2] = new SqlParameter("@UserID", info.UserID);
				para[3] = new SqlParameter("@Attack", info.Attack);
				para[4] = new SqlParameter("@Defence", info.Defence);
				para[5] = new SqlParameter("@Luck", info.Luck);
				para[6] = new SqlParameter("@Agility", info.Agility);
				para[7] = new SqlParameter("@Blood", info.Blood);
				para[8] = new SqlParameter("@Damage", info.Damage);
				para[9] = new SqlParameter("@Guard", info.Guard);
				para[10] = new SqlParameter("@AttackGrow", info.AttackGrow);
				para[11] = new SqlParameter("@DefenceGrow", info.DefenceGrow);
				para[12] = new SqlParameter("@LuckGrow", info.LuckGrow);
				para[13] = new SqlParameter("@AgilityGrow", info.AgilityGrow);
				para[14] = new SqlParameter("@BloodGrow", info.BloodGrow);
				para[15] = new SqlParameter("@DamageGrow", info.DamageGrow);
				para[16] = new SqlParameter("@GuardGrow", info.GuardGrow);
				para[17] = new SqlParameter("@Skill", info.Skill);
				para[18] = new SqlParameter("@SkillEquip", info.SkillEquip);
				para[19] = new SqlParameter("@Place", info.Place);
				para[20] = new SqlParameter("@IsExit", info.IsExit);
				para[21] = new SqlParameter("@IsUse", isUse);
				para[22] = new SqlParameter("@ID", info.ID);
				para[22].Direction = ParameterDirection.Output;
				result = db.RunProcedure("SP_User_AdoptPet", para);
				info.ID = (int)para[22].Value;
				info.IsDirty = false;
			}
			catch (Exception e)
			{
				if (log.IsErrorEnabled)
					log.Error("Init", e);
			}

			return result;
		}

		public bool RemoveUserAdoptPet(int ID)
		{
			bool result = false;
			try
			{
				SqlParameter[] para = new SqlParameter[2];
				para[0] = new SqlParameter("@ID", ID);
				para[1] = new SqlParameter("@Result", System.Data.SqlDbType.Int);
				para[1].Direction = ParameterDirection.ReturnValue;
				db.RunProcedure("SP_Remove_User_AdoptPet", para);
				int returnValue = (int)para[1].Value;
				result = returnValue == 0;
			}
			catch (Exception e)
			{
				if (log.IsErrorEnabled)
					log.Error("Init", e);
			}

			return result;
		}

		public bool UpdateUserAdoptPet(int ID)
		{
			bool result = false;
			try
			{
				SqlParameter[] para = new SqlParameter[2];
				para[0] = new SqlParameter("@ID", ID);
				para[1] = new SqlParameter("@Result", System.Data.SqlDbType.Int);
				para[1].Direction = ParameterDirection.ReturnValue;
				db.RunProcedure("SP_Update_User_AdoptPet", para);
				int returnValue = (int)para[1].Value;
				result = returnValue == 0;
			}
			catch (Exception e)
			{
				if (log.IsErrorEnabled)
					log.Error("Init", e);
			}

			return result;
		}

		public bool ClearAdoptPet(int ID)
		{
			bool result = false;
			try
			{
				SqlParameter[] para = new SqlParameter[2];
				para[0] = new SqlParameter("@ID", ID);
				para[1] = new SqlParameter("@Result", System.Data.SqlDbType.Int);
				para[1].Direction = ParameterDirection.ReturnValue;
				db.RunProcedure("SP_Clear_AdoptPet", para);
				int returnValue = (int)para[1].Value;
				result = returnValue == 0;
			}
			catch (Exception e)
			{
				if (log.IsErrorEnabled)
					log.Error("Init", e);
			}

			return result;
		}

		public UsersPetInfo[] GetUserPetSingles(int UserID, int vipLv)
		{
			List<UsersPetInfo> items = new List<UsersPetInfo>();
			SqlDataReader reader = null;
			try
			{
				SqlParameter[] para = new SqlParameter[1];
				para[0] = new SqlParameter("@UserID", SqlDbType.Int, 4);
				para[0].Value = UserID;
				db.GetReader(ref reader, "SP_Get_UserPet_By_ID", para);

				while (reader.Read())
				{
					UsersPetInfo info = InitPet(reader);
					info.VIPLevel = vipLv;
					items.Add(info);
				}
			}
			catch (Exception e)
			{
				if (log.IsErrorEnabled)
				{
					log.Error("Init", e);
				}
			}
			finally
			{
				if (reader != null && !reader.IsClosed)
				{
					reader.Close();
				}
			}
			return items.ToArray();
		}

		//public UsersPetInfo[] GetUserPetSingles(int UserID, int vipLv)
		//{
		//	List<UsersPetInfo> usersPetInfoList = new List<UsersPetInfo>();
		//	SqlDataReader ResultDataReader = null;
		//	try
		//	{
		//		SqlParameter[] SqlParameters = new SqlParameter[1]
		//		{
		//		new SqlParameter("@UserID", SqlDbType.Int, 4)
		//		};
		//		SqlParameters[0].Value = UserID;
		//		db.GetReader(ref ResultDataReader, "SP_Get_UserPet_By_ID", SqlParameters);
		//		while (ResultDataReader.Read())
		//		{
		//			usersPetInfoList.Add(InitPet(ResultDataReader));
		//		}
		//	}
		//	catch (Exception ex)
		//	{
		//		if (BaseBussiness.log.IsErrorEnabled)
		//		{
		//			BaseBussiness.log.Error("Init", ex);
		//		}
		//	}
		//	finally
		//	{
		//		if (ResultDataReader != null && !ResultDataReader.IsClosed)
		//		{
		//			ResultDataReader.Close();
		//		}
		//	}
		//	return usersPetInfoList.ToArray();
		//}

		public bool UpdateUserPet(UsersPetInfo item)
		{
			bool flag = false;
			try
			{
				SqlParameter[] SqlParameters = new SqlParameter[39]
				{
				new SqlParameter("@TemplateID", item.TemplateID),
				new SqlParameter("@Name", (item.Name == null) ? "Error!" : item.Name),
				new SqlParameter("@UserID", item.UserID),
				new SqlParameter("@Attack", item.Attack),
				new SqlParameter("@Defence", item.Defence),
				new SqlParameter("@Luck", item.Luck),
				new SqlParameter("@Agility", item.Agility),
				new SqlParameter("@Blood", item.Blood),
				new SqlParameter("@Damage", item.Damage),
				new SqlParameter("@Guard", item.Guard),
				new SqlParameter("@AttackGrow", item.AttackGrow),
				new SqlParameter("@DefenceGrow", item.DefenceGrow),
				new SqlParameter("@LuckGrow", item.LuckGrow),
				new SqlParameter("@AgilityGrow", item.AgilityGrow),
				new SqlParameter("@BloodGrow", item.BloodGrow),
				new SqlParameter("@DamageGrow", item.DamageGrow),
				new SqlParameter("@GuardGrow", item.GuardGrow),
				new SqlParameter("@Level", item.Level),
				new SqlParameter("@GP", item.GP),
				new SqlParameter("@MaxGP", item.MaxGP),
				new SqlParameter("@Hunger", item.Hunger),
				new SqlParameter("@PetHappyStar", item.PetHappyStar),
				new SqlParameter("@MP", item.MP),
				new SqlParameter("@IsEquip", item.IsEquip),
				new SqlParameter("@Place", item.Place),
				new SqlParameter("@IsExit", item.IsExit),
				new SqlParameter("@ID", item.ID),
				new SqlParameter("@Skill", item.Skill),
				new SqlParameter("@SkillEquip", item.SkillEquip),
				new SqlParameter("@currentStarExp", item.currentStarExp),
				new SqlParameter("@Result", SqlDbType.Int),
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null
				};
				SqlParameters[30].Direction = ParameterDirection.ReturnValue;
				SqlParameters[31] = new SqlParameter("@breakGrade", item.breakGrade);
				SqlParameters[32] = new SqlParameter("@breakAttack", item.breakAttack);
				SqlParameters[33] = new SqlParameter("@breakDefence", item.breakDefence);
				SqlParameters[34] = new SqlParameter("@breakAgility", item.breakAgility);
				SqlParameters[35] = new SqlParameter("@breakLuck", item.breakLuck);
				SqlParameters[36] = new SqlParameter("@breakBlood", item.breakBlood);
				SqlParameters[37] = new SqlParameter("@eQPets", item.eQPets);
				SqlParameters[38] = new SqlParameter("@BaseProp", item.BaseProp);
				db.RunProcedure("SP_UserPet_Update", SqlParameters);
				flag = (int)SqlParameters[30].Value == 0;
				return flag;
			}
			catch (Exception ex)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("Init", ex);
					return flag;
				}
				return flag;
			}
		}

		public bool AddUserPet(UsersPetInfo item)
		{
			bool flag = false;
			try
			{
				SqlParameter[] SqlParameters = new SqlParameter[39]
				{
				new SqlParameter("@TemplateID", item.TemplateID),
				new SqlParameter("@Name", (item.Name == null) ? "Error!" : item.Name),
				new SqlParameter("@UserID", item.UserID),
				new SqlParameter("@Attack", item.Attack),
				new SqlParameter("@Defence", item.Defence),
				new SqlParameter("@Luck", item.Luck),
				new SqlParameter("@Agility", item.Agility),
				new SqlParameter("@Blood", item.Blood),
				new SqlParameter("@Damage", item.Damage),
				new SqlParameter("@Guard", item.Guard),
				new SqlParameter("@AttackGrow", item.AttackGrow),
				new SqlParameter("@DefenceGrow", item.DefenceGrow),
				new SqlParameter("@LuckGrow", item.LuckGrow),
				new SqlParameter("@AgilityGrow", item.AgilityGrow),
				new SqlParameter("@BloodGrow", item.BloodGrow),
				new SqlParameter("@DamageGrow", item.DamageGrow),
				new SqlParameter("@GuardGrow", item.GuardGrow),
				new SqlParameter("@Level", item.Level),
				new SqlParameter("@GP", item.GP),
				new SqlParameter("@MaxGP", item.MaxGP),
				new SqlParameter("@Hunger", item.Hunger),
				new SqlParameter("@PetHappyStar", item.PetHappyStar),
				new SqlParameter("@MP", item.MP),
				new SqlParameter("@IsEquip", item.IsEquip),
				new SqlParameter("@Skill", item.Skill),
				new SqlParameter("@SkillEquip", item.SkillEquip),
				new SqlParameter("@Place", item.Place),
				new SqlParameter("@IsExit", item.IsExit),
				new SqlParameter("@ID", item.ID),
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null
				};
				SqlParameters[28].Direction = ParameterDirection.Output;
				SqlParameters[29] = new SqlParameter("@currentStarExp", item.currentStarExp);
				SqlParameters[30] = new SqlParameter("@Result", SqlDbType.Int);
				SqlParameters[30].Direction = ParameterDirection.ReturnValue;
				SqlParameters[31] = new SqlParameter("@breakGrade", item.breakGrade);
				SqlParameters[32] = new SqlParameter("@breakAttack", item.breakAttack);
				SqlParameters[33] = new SqlParameter("@breakDefence", item.breakDefence);
				SqlParameters[34] = new SqlParameter("@breakAgility", item.breakAgility);
				SqlParameters[35] = new SqlParameter("@breakLuck", item.breakLuck);
				SqlParameters[36] = new SqlParameter("@breakBlood", item.breakBlood);
				SqlParameters[37] = new SqlParameter("@eQPets", item.eQPets);
				SqlParameters[38] = new SqlParameter("@BaseProp", item.BaseProp);
				flag = db.RunProcedure("SP_User_Add_Pet", SqlParameters);
				flag = (int)SqlParameters[30].Value == 0;
				item.ID = (int)SqlParameters[28].Value;
				item.IsDirty = false;
				return flag;
			}
			catch (Exception ex)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("Init", ex);
					return flag;
				}
				return flag;
			}
		}

		public UsersPetInfo InitPet(SqlDataReader reader)
		{
			return new UsersPetInfo
			{
				ID = (int)reader["ID"],
				TemplateID = (int)reader["TemplateID"],
				Name = reader["Name"].ToString(),
				UserID = (int)reader["UserID"],
				Attack = (int)reader["Attack"],
				AttackGrow = (int)reader["AttackGrow"],
				Agility = (int)reader["Agility"],
				AgilityGrow = (int)reader["AgilityGrow"],
				Defence = (int)reader["Defence"],
				DefenceGrow = (int)reader["DefenceGrow"],
				Luck = (int)reader["Luck"],
				LuckGrow = (int)reader["LuckGrow"],
				Blood = (int)reader["Blood"],
				BloodGrow = (int)reader["BloodGrow"],
				Damage = (int)reader["Damage"],
				DamageGrow = (int)reader["DamageGrow"],
				Guard = (int)reader["Guard"],
				GuardGrow = (int)reader["GuardGrow"],
				Level = (int)reader["Level"],
				GP = (int)reader["GP"],
				MaxGP = (int)reader["MaxGP"],
				Hunger = (int)reader["Hunger"],
				MP = (int)reader["MP"],
				Place = (int)reader["Place"],
				IsEquip = (bool)reader["IsEquip"],
				IsExit = (bool)reader["IsExit"],
				Skill = reader["Skill"].ToString(),
				SkillEquip = reader["SkillEquip"].ToString(),
				currentStarExp = (int)reader["currentStarExp"],
				breakGrade = (int)reader["breakGrade"],
				breakAttack = (int)reader["breakAttack"],
				breakDefence = (int)reader["breakDefence"],
				breakAgility = (int)reader["breakAgility"],
				breakLuck = (int)reader["breakLuck"],
				breakBlood = (int)reader["breakBlood"],
				eQPets = ((reader["eQPets"] == null) ? "" : reader["eQPets"].ToString()),
				BaseProp = ((reader["BaseProp"] == null) ? "" : reader["BaseProp"].ToString())
			};
		}

		public bool RegisterPlayer2(string userName, string passWord, string nickName, int attack, int defence, int agility, int luck, int cateogryId, string bStyle, string bPic, string gStyle, string armColor, string hairColor, string faceColor, string clothColor, string hatchColor, int sex, ref string msg, int validDate)
		{
			bool flag = false;
			try
			{
				string[] strArray1 = bStyle.Split(',');
				string[] strArray2 = gStyle.Split(',');
				string[] strArray3 = bPic.Split(',');
				SqlParameter[] SqlParameters = new SqlParameter[31];
				SqlParameters[0] = new SqlParameter("@UserName", userName);
				SqlParameters[1] = new SqlParameter("@PassWord", passWord);
				SqlParameters[2] = new SqlParameter("@NickName", nickName);
				SqlParameters[3] = new SqlParameter("@BArmID", int.Parse(strArray1[0]));
				SqlParameters[4] = new SqlParameter("@BHairID", int.Parse(strArray1[1]));
				SqlParameters[5] = new SqlParameter("@BFaceID", int.Parse(strArray1[2]));
				SqlParameters[6] = new SqlParameter("@BClothID", int.Parse(strArray1[3]));
				SqlParameters[7] = new SqlParameter("@BHatID", int.Parse(strArray1[4]));
				SqlParameters[21] = new SqlParameter("@ArmPic", strArray3[0]);
				SqlParameters[22] = new SqlParameter("@HairPic", strArray3[1]);
				SqlParameters[23] = new SqlParameter("@FacePic", strArray3[2]);
				SqlParameters[24] = new SqlParameter("@ClothPic", strArray3[3]);
				SqlParameters[25] = new SqlParameter("@HatPic", strArray3[4]);
				SqlParameters[8] = new SqlParameter("@GArmID", int.Parse(strArray2[0]));
				SqlParameters[9] = new SqlParameter("@GHairID", int.Parse(strArray2[1]));
				SqlParameters[10] = new SqlParameter("@GFaceID", int.Parse(strArray2[2]));
				SqlParameters[11] = new SqlParameter("@GClothID", int.Parse(strArray2[3]));
				SqlParameters[12] = new SqlParameter("@GHatID", int.Parse(strArray2[4]));
				SqlParameters[13] = new SqlParameter("@ArmColor", armColor);
				SqlParameters[14] = new SqlParameter("@HairColor", hairColor);
				SqlParameters[15] = new SqlParameter("@FaceColor", faceColor);
				SqlParameters[16] = new SqlParameter("@ClothColor", clothColor);
				SqlParameters[17] = new SqlParameter("@HatColor", clothColor);
				SqlParameters[18] = new SqlParameter("@Sex", sex);
				SqlParameters[19] = new SqlParameter("@StyleDate", validDate);
				SqlParameters[26] = new SqlParameter("@CategoryID", cateogryId);
				SqlParameters[27] = new SqlParameter("@Attack", attack);
				SqlParameters[28] = new SqlParameter("@Defence", defence);
				SqlParameters[29] = new SqlParameter("@Agility", agility);
				SqlParameters[30] = new SqlParameter("@Luck", luck);
				SqlParameters[20] = new SqlParameter("@Result", SqlDbType.Int);
				SqlParameters[20].Direction = ParameterDirection.ReturnValue;
				flag = db.RunProcedure("SP_Users_RegisterNotValidate2", SqlParameters);
				int num = (int)SqlParameters[20].Value;
				flag = num == 0;
				switch (num)
				{
					case 2:
						msg = LanguageMgr.GetTranslation("PlayerBussiness.RegisterPlayer.Msg2");
						return flag;
					case 3:
						msg = LanguageMgr.GetTranslation("PlayerBussiness.RegisterPlayer.Msg3");
						return flag;
					default:
						return flag;
				}
			}
			catch (Exception ex)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error($"{userName} {passWord} {nickName} {attack} {defence} {agility} {luck} {cateogryId} {bStyle} {bPic} {gStyle}");
					BaseBussiness.log.Error("Init", ex);
					return flag;
				}
				return flag;
			}
		}

		public bool UpdateUserReputeFightPower()
		{
			bool flag = false;
			try
			{
				SqlParameter[] SqlParameters = new SqlParameter[1]
				{
				new SqlParameter("@Result", SqlDbType.Int)
				};
				SqlParameters[0].Direction = ParameterDirection.ReturnValue;
				db.RunProcedure("SP_Update_Repute_FightPower", SqlParameters);
				flag = (int)SqlParameters[0].Value == 0;
				return flag;
			}
			catch (Exception ex)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("Init", ex);
					return flag;
				}
				return flag;
			}
		}

		public UsersExtraInfo[] GetRankCaddy()
		{
			List<UsersExtraInfo> userExtraInfoList = new List<UsersExtraInfo>();
			SqlDataReader ResultDataReader = null;
			try
			{
				db.GetReader(ref ResultDataReader, "SP_Get_Rank_Caddy");
				while (ResultDataReader.Read())
				{
					userExtraInfoList.Add(new UsersExtraInfo
					{
						UserID = (int)ResultDataReader["UserID"],
						NickName = (string)ResultDataReader["NickName"],
						TotalCaddyOpen = (int)ResultDataReader["badLuckNumber"]
					});
				}
			}
			catch (Exception ex)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("SP_Get_Rank_Caddy", ex);
				}
			}
			finally
			{
				if (ResultDataReader != null && !ResultDataReader.IsClosed)
				{
					ResultDataReader.Close();
				}
			}
			return userExtraInfoList.ToArray();
		}

		public UserLabyrinthInfo GetSingleLabyrinth(int ID)
		{
			SqlDataReader ResultDataReader = null;
			try
			{
				SqlParameter[] SqlParameters = new SqlParameter[1]
				{
				new SqlParameter("@ID", SqlDbType.Int, 4)
				};
				SqlParameters[0].Value = ID;
				db.GetReader(ref ResultDataReader, "SP_GetSingleLabyrinth", SqlParameters);
				if (ResultDataReader.Read())
				{
					return new UserLabyrinthInfo
					{
						UserID = (int)ResultDataReader["UserID"],
						myProgress = (int)ResultDataReader["myProgress"],
						myRanking = (int)ResultDataReader["myRanking"],
						completeChallenge = (bool)ResultDataReader["completeChallenge"],
						isDoubleAward = (bool)ResultDataReader["isDoubleAward"],
						currentFloor = (int)ResultDataReader["currentFloor"],
						accumulateExp = (int)ResultDataReader["accumulateExp"],
						remainTime = (int)ResultDataReader["remainTime"],
						currentRemainTime = (int)ResultDataReader["currentRemainTime"],
						cleanOutAllTime = (int)ResultDataReader["cleanOutAllTime"],
						cleanOutGold = (int)ResultDataReader["cleanOutGold"],
						tryAgainComplete = (bool)ResultDataReader["tryAgainComplete"],
						isInGame = (bool)ResultDataReader["isInGame"],
						isCleanOut = (bool)ResultDataReader["isCleanOut"],
						serverMultiplyingPower = (bool)ResultDataReader["serverMultiplyingPower"],
						LastDate = (DateTime)ResultDataReader["LastDate"],
						ProcessAward = (string)ResultDataReader["ProcessAward"]
					};
				}
			}
			catch (Exception ex)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("SP_GetSingleUserLabyrinth", ex);
				}
			}
			finally
			{
				if (ResultDataReader != null && !ResultDataReader.IsClosed)
				{
					ResultDataReader.Close();
				}
			}
			return null;
		}

		public bool AddUserLabyrinth(UserLabyrinthInfo laby)
		{
			bool flag = false;
			try
			{
				SqlParameter[] SqlParameters = new SqlParameter[18]
				{
				new SqlParameter("@UserID", laby.UserID),
				new SqlParameter("@myProgress", laby.myProgress),
				new SqlParameter("@myRanking", laby.myRanking),
				new SqlParameter("@completeChallenge", laby.completeChallenge),
				new SqlParameter("@isDoubleAward", laby.isDoubleAward),
				new SqlParameter("@currentFloor", laby.currentFloor),
				new SqlParameter("@accumulateExp", laby.accumulateExp),
				new SqlParameter("@remainTime", laby.remainTime),
				new SqlParameter("@currentRemainTime", laby.currentRemainTime),
				new SqlParameter("@cleanOutAllTime", laby.cleanOutAllTime),
				new SqlParameter("@cleanOutGold", laby.cleanOutGold),
				new SqlParameter("@tryAgainComplete", laby.tryAgainComplete),
				new SqlParameter("@isInGame", laby.isInGame),
				new SqlParameter("@isCleanOut", laby.isCleanOut),
				new SqlParameter("@serverMultiplyingPower", laby.serverMultiplyingPower),
				new SqlParameter("@LastDate", laby.LastDate),
				new SqlParameter("@ProcessAward", laby.ProcessAward),
				new SqlParameter("@Result", SqlDbType.Int)
				};
				SqlParameters[17].Direction = ParameterDirection.ReturnValue;
				db.RunProcedure("SP_Users_Labyrinth_Add", SqlParameters);
				flag = (int)SqlParameters[17].Value == 0;
				return flag;
			}
			catch (Exception ex)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("Init", ex);
					return flag;
				}
				return flag;
			}
		}

		public bool UpdateLabyrinthInfo(UserLabyrinthInfo laby)
		{
			bool flag = false;
			try
			{
				SqlParameter[] SqlParameters = new SqlParameter[18]
				{
				new SqlParameter("@UserID", laby.UserID),
				new SqlParameter("@myProgress", laby.myProgress),
				new SqlParameter("@myRanking", laby.myRanking),
				new SqlParameter("@completeChallenge", laby.completeChallenge),
				new SqlParameter("@isDoubleAward", laby.isDoubleAward),
				new SqlParameter("@currentFloor", laby.currentFloor),
				new SqlParameter("@accumulateExp", laby.accumulateExp),
				new SqlParameter("@remainTime", laby.remainTime),
				new SqlParameter("@currentRemainTime", laby.currentRemainTime),
				new SqlParameter("@cleanOutAllTime", laby.cleanOutAllTime),
				new SqlParameter("@cleanOutGold", laby.cleanOutGold),
				new SqlParameter("@tryAgainComplete", laby.tryAgainComplete),
				new SqlParameter("@isInGame", laby.isInGame),
				new SqlParameter("@isCleanOut", laby.isCleanOut),
				new SqlParameter("@serverMultiplyingPower", laby.serverMultiplyingPower),
				new SqlParameter("@LastDate", laby.LastDate),
				new SqlParameter("@ProcessAward", laby.ProcessAward),
				new SqlParameter("@Result", SqlDbType.Int)
				};
				SqlParameters[17].Direction = ParameterDirection.ReturnValue;
				db.RunProcedure("SP_UpdateLabyrinthInfo", SqlParameters);
				flag = true;
				return flag;
			}
			catch (Exception ex)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("SP_UpdateLabyrinthInfo", ex);
					return flag;
				}
				return flag;
			}
		}

		public UserGiftInfo[] GetAllUserGifts(int userid, bool isReceive)
		{
			List<UserGiftInfo> userGiftInfoList = new List<UserGiftInfo>();
			SqlDataReader ResultDataReader = null;
			try
			{
				SqlParameter[] SqlParameters = new SqlParameter[2]
				{
				new SqlParameter("@UserID", userid),
				new SqlParameter("@IsReceive", isReceive)
				};
				db.GetReader(ref ResultDataReader, "SP_Users_Gift_Single", SqlParameters);
				while (ResultDataReader.Read())
				{
					userGiftInfoList.Add(new UserGiftInfo
					{
						ID = (int)ResultDataReader["ID"],
						ReceiverID = (int)ResultDataReader["ReceiverID"],
						SenderID = (int)ResultDataReader["SenderID"],
						TemplateID = (int)ResultDataReader["TemplateID"],
						Count = (int)ResultDataReader["Count"],
						CreateDate = (DateTime)ResultDataReader["CreateDate"],
						LastUpdate = (DateTime)ResultDataReader["LastUpdate"]
					});
				}
			}
			catch (Exception ex)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("GetAllUserGifts", ex);
				}
			}
			finally
			{
				if (ResultDataReader != null && !ResultDataReader.IsClosed)
				{
					ResultDataReader.Close();
				}
			}
			return userGiftInfoList.ToArray();
		}

		public UserGiftInfo[] GetAllUserReceivedGifts(int userid)
		{
			Dictionary<int, UserGiftInfo> dictionary = new Dictionary<int, UserGiftInfo>();
			SqlDataReader sqlDataReader = null;
			try
			{
				UserGiftInfo[] allUserGifts = GetAllUserGifts(userid, isReceive: true);
				if (allUserGifts != null)
				{
					UserGiftInfo[] array = allUserGifts;
					foreach (UserGiftInfo userGiftInfo in array)
					{
						if (dictionary.ContainsKey(userGiftInfo.TemplateID))
						{
							dictionary[userGiftInfo.TemplateID].Count += userGiftInfo.Count;
						}
						else
						{
							dictionary.Add(userGiftInfo.TemplateID, userGiftInfo);
						}
					}
				}
			}
			catch (Exception ex)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("GetAllUserReceivedGifts", ex);
				}
			}
			finally
			{
				if (sqlDataReader != null && !sqlDataReader.IsClosed)
				{
					sqlDataReader.Close();
				}
			}
			return dictionary.Values.ToArray();
		}

		public bool AddUserGift(UserGiftInfo info)
		{
			bool flag = false;
			try
			{
				db.RunProcedure("SP_Users_Gift_Add", new SqlParameter[4]
				{
				new SqlParameter("@SenderID", info.SenderID),
				new SqlParameter("@ReceiverID", info.ReceiverID),
				new SqlParameter("@TemplateID", info.TemplateID),
				new SqlParameter("@Count", info.Count)
				});
				flag = true;
				return flag;
			}
			catch (Exception ex)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("AddUserGift", ex);
					return flag;
				}
				return flag;
			}
		}

		public bool UpdateUserCharmGP(int userId, int int_1)
		{
			bool flag = false;
			try
			{
				SqlParameter[] SqlParameters = new SqlParameter[3]
				{
				new SqlParameter("@UserID", userId),
				new SqlParameter("@CharmGP", int_1),
				new SqlParameter("@Result", SqlDbType.Int)
				};
				SqlParameters[2].Direction = ParameterDirection.ReturnValue;
				db.RunProcedure("SP_Users_UpdateCharmGP", SqlParameters);
				flag = (int)SqlParameters[2].Value == 0;
				return flag;
			}
			catch (Exception ex)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("AddUserGift", ex);
					return flag;
				}
				return flag;
			}
		}

		public EatPetsInfo GetAllEatPetsByID(int ID)
		{
			SqlDataReader reader = null;
			try
			{
				SqlParameter[] para = new SqlParameter[1];
				para[0] = new SqlParameter("@ID", SqlDbType.Int, 4);
				para[0].Value = ID;
				db.GetReader(ref reader, "SP_Sys_Eat_Pets_All", para);
				while (reader.Read())
				{
					return InitEatPetsInfo(reader);
				}
			}
			catch (Exception e)
			{
				if (log.IsErrorEnabled)
					log.Error("InitEatPetsInfo", e);
			}
			finally
			{
				if (reader != null && !reader.IsClosed)
					reader.Close();
			}

			return null;
		}

		public EatPetsInfo InitEatPetsInfo(SqlDataReader dr)
		{
			EatPetsInfo info = new EatPetsInfo();
			info.ID = (int)dr["ID"];
			info.UserID = (int)dr["UserID"];
			info.weaponExp = (int)dr["weaponExp"];
			info.weaponLevel = (int)dr["weaponLevel"];
			info.clothesExp = (int)dr["clothesExp"];
			info.clothesLevel = (int)dr["clothesLevel"];
			info.hatExp = (int)dr["hatExp"];
			info.hatLevel = (int)dr["hatLevel"];
			return info;
		}

		public bool UpdateEatPets(EatPetsInfo info)
		{
			bool result = false;
			try
			{
				SqlParameter[] para = new SqlParameter[8];
				para[0] = new SqlParameter("@ID", info.ID);
				para[1] = new SqlParameter("@UserID", info.UserID);
				para[2] = new SqlParameter("@weaponExp", info.weaponExp);
				para[3] = new SqlParameter("@weaponLevel", info.weaponLevel);
				para[4] = new SqlParameter("@clothesExp", info.clothesExp);
				para[5] = new SqlParameter("@clothesLevel", info.clothesLevel);
				para[6] = new SqlParameter("@hatExp", info.hatExp);
				para[7] = new SqlParameter("@hatLevel", info.hatLevel);
				result = db.RunProcedure("SP_Sys_Eat_Pets_Update", para);
			}
			catch (Exception e)
			{
				if (log.IsErrorEnabled)
					log.Error("SP_Sys_Eat_Pets_Update", e);
			}

			return result;
		}

		public bool AddEatPets(EatPetsInfo info)
		{
			bool result = false;
			try
			{
				SqlParameter[] para = new SqlParameter[8];
				para[0] = new SqlParameter("@ID", info.ID);
				para[1] = new SqlParameter("@UserID", info.UserID);
				para[2] = new SqlParameter("@weaponExp", info.weaponExp);
				para[3] = new SqlParameter("@weaponLevel", info.weaponLevel);
				para[4] = new SqlParameter("@clothesExp", info.clothesExp);
				para[5] = new SqlParameter("@clothesLevel", info.clothesLevel);
				para[6] = new SqlParameter("@hatExp", info.hatExp);
				para[7] = new SqlParameter("@hatLevel", info.hatLevel);
				para[0].Direction = ParameterDirection.Output;
				result = db.RunProcedure("SP_Sys_Eat_Pets_Add", para);
				info.ID = (int)para[0].Value;
				info.IsDirty = false;
			}
			catch (Exception e)
			{
				if (log.IsErrorEnabled)
					log.Error("SP_Sys_Eat_Pets_Add", e);
			}

			return result;
		}

		public Suit_Manager Get_Suit_Manager(int UserID)
		{
			Suit_Manager items = new Suit_Manager();
			SqlDataReader reader = null;
			try
			{
				SqlParameter[] para = new SqlParameter[] { new SqlParameter("@UserID", SqlDbType.Int, 4) };
				para[0].Value = UserID;
				this.db.GetReader(ref reader, "SP_Suit_Manager_GET", para);
				while (reader.Read())
				{
					items.UserID = (int)reader["UserID"];
					items.Kill_List = (string)reader["Kill_List"];
				}
			}
			catch (Exception e)
			{
				if (log.IsErrorEnabled)
				{
					log.Error("Init", e);
				}
			}
			finally
			{
				if (reader != null && !reader.IsClosed)
				{
					reader.Close();
				}
			}
			try
			{
				if (items.UserID == 0)
				{
					try
					{
						SqlParameter[] para = new SqlParameter[] { new SqlParameter("@UserID", UserID) };
						this.db.RunProcedure("SP_Suit_Manager_ADD", para);
					}
					catch (Exception e)
					{
						if (log.IsErrorEnabled)
						{
							log.Error("SP_Suit_Manager_ADD error!", e);
						}
					}
				}
			}
			catch
			{

			}
			return items;
		}

		public bool UpdateRank()
		{
			bool result = false;
			try
			{
				SqlParameter[] para = new SqlParameter[0];
				result = db.RunProcedure("SP_Sys_Update_Consortia_DayList", para);
				result = db.RunProcedure("SP_Sys_Update_Consortia_FightPower", para);
				result = db.RunProcedure("SP_Sys_Update_Consortia_Honor", para);
				result = db.RunProcedure("SP_Sys_Update_Consortia_List", para);
				result = db.RunProcedure("SP_Sys_Update_Consortia_WeekList", para);
				result = db.RunProcedure("SP_Sys_Update_OfferList", para);
				result = db.RunProcedure("SP_Sys_Update_Users_DayList", para);
				result = db.RunProcedure("SP_Sys_Update_Users_List", para);
				result = db.RunProcedure("SP_Sys_Update_Users_WeekList", para);
				result = db.RunProcedure("SP_Sys_Update_Users_Rank_Date", para);
				result = db.RunProcedure("SP_Sys_Update_Users_Rank_League", para);
				log.Error("UpdateRanked");
			}
			catch (Exception e)
			{
				if (log.IsErrorEnabled)
					log.Error("Init UpdatePersonalRank", e);
			}

			return result;
		}

		public UserRankDateInfo[] GetAllUserRankDate()
		{
			List<UserRankDateInfo> list = new List<UserRankDateInfo>();
			SqlDataReader reader = null;
			try
			{
				SqlParameter[] para = new SqlParameter[0];
				db.GetReader(ref reader, "SP_Sys_Users_Rank_Date_All", para);
				while (reader.Read())
				{
					list.Add(InitUserRankDateInfo(reader));
				}
			}
			catch (Exception e)
			{
				if (log.IsErrorEnabled)
					log.Error("InitUserRankDateInfo", e);
			}
			finally
			{
				if (reader != null && !reader.IsClosed)
					reader.Close();
			}

			return list.ToArray();
		}

		public UserRankDateInfo GetUserRankDateByID(int userID)
		{
			SqlDataReader reader = null;
			try
			{
				SqlParameter[] para = new SqlParameter[1];
				para[0] = new SqlParameter("@UserID", SqlDbType.Int, 4);
				para[0].Value = userID;
				db.GetReader(ref reader, "SP_Sys_Users_Rank_Date", para);
				while (reader.Read())
				{
					return InitUserRankDateInfo(reader);
				}
			}
			catch (Exception e)
			{
				if (log.IsErrorEnabled)
					log.Error("InitUserRankDateInfo", e);
			}
			finally
			{
				if (reader != null && !reader.IsClosed)
					reader.Close();
			}

			return null;
		}

		public UserRankDateInfo InitUserRankDateInfo(SqlDataReader dr)
		{
			UserRankDateInfo info = new UserRankDateInfo();
			info.UserID = (int)dr["UserID"];
			info.ConsortiaID = (int)dr["ConsortiaID"];
			info.FightPower = (int)dr["FightPower"];
			info.PrevFightPower = (int)dr["PrevFightPower"];
			info.GP = (int)dr["GP"];
			info.PrevGP = (int)dr["PrevGP"];
			info.AchievementPoint = (int)dr["AchievementPoint"];
			info.PrevAchievementPoint = (int)dr["PrevAchievementPoint"];
			info.charmGP = (int)dr["charmGP"];
			info.PrecharmGP = (int)dr["PrecharmGP"];
			info.LeagueAddWeek = (int)dr["LeagueAddWeek"];
			info.PrevLeagueAddWeek = (int)dr["PrevLeagueAddWeek"];
			info.ConsortiaFightPower = (int)dr["ConsortiaFightPower"];
			info.ConsortiaPrevFightPower = (int)dr["ConsortiaPrevFightPower"];
			info.ConsortiaLevel = (int)dr["ConsortiaLevel"];
			info.ConsortiaPrevLevel = (int)dr["ConsortiaPrevLevel"];
			info.ConsortiaRiches = (int)dr["ConsortiaRiches"];
			info.ConsortiaPrevRiches = (int)dr["ConsortiaPrevRiches"];
			info.ConsortiacharmGP = (int)dr["ConsortiacharmGP"];
			info.ConsortiaPrevcharmGP = (int)dr["ConsortiaPrevcharmGP"];
			return info;
		}

		public UsersPetInfo[] GetUserAdoptPetSingles(int UserID)
		{
			List<UsersPetInfo> items = new List<UsersPetInfo>();
			SqlDataReader reader = null;
			try
			{
				SqlParameter[] para = new SqlParameter[1];
				para[0] = new SqlParameter("@UserID", SqlDbType.Int, 4);
				para[0].Value = UserID;
				db.GetReader(ref reader, "SP_Get_User_AdoptPetList", para);

				while (reader.Read())
				{
					UsersPetInfo info = new UsersPetInfo();
					info.ID = (int)reader["ID"];
					info.TemplateID = (int)reader["TemplateID"];
					info.Name = (string)reader["Name"].ToString();
					info.UserID = (int)reader["UserID"];
					info.Attack = (int)reader["Attack"];
					info.AttackGrow = (int)reader["AttackGrow"];
					info.Agility = (int)reader["Agility"];
					info.AgilityGrow = (int)reader["AgilityGrow"];
					info.Defence = (int)reader["Defence"];
					info.DefenceGrow = (int)reader["DefenceGrow"];
					info.Luck = (int)reader["Luck"];
					info.LuckGrow = (int)reader["LuckGrow"];
					info.Blood = (int)reader["Blood"];
					info.BloodGrow = (int)reader["BloodGrow"];
					info.Damage = (int)reader["Damage"];
					info.DamageGrow = (int)reader["DamageGrow"];
					info.Guard = (int)reader["Guard"];
					info.GuardGrow = (int)reader["GuardGrow"];
					info.Level = (int)reader["Level"];
					info.GP = (int)reader["GP"];
					info.MaxGP = (int)reader["MaxGP"];
					info.Hunger = (int)reader["Hunger"];
					info.MP = (int)reader["MP"];
					info.Place = (int)reader["Place"];
					info.IsEquip = (bool)reader["IsEquip"];
					info.IsExit = (bool)reader["IsExit"];
					info.Skill = (string)reader["Skill"].ToString();
					info.SkillEquip = (string)reader["SkillEquip"].ToString();
					items.Add(info);
				}
			}
			catch (Exception e)
			{
				if (log.IsErrorEnabled)
					log.Error("Init", e);
			}
			finally
			{
				if (reader != null && !reader.IsClosed)
					reader.Close();
			}

			return items.ToArray();
		}
		//Farm
		public UserFarmInfo GetSingleFarm(int Id)
		{
			SqlDataReader reader = null;
			try
			{
				SqlParameter[] para = new SqlParameter[1];
				para[0] = new SqlParameter("@ID", SqlDbType.Int, 4);
				para[0].Value = Id;
				db.GetReader(ref reader, "SP_Get_SingleFarm", para);
				while (reader.Read())
				{
					UserFarmInfo infos = new UserFarmInfo();
					infos.ID = (int)reader["ID"];
					infos.FarmID = (int)reader["FarmID"];
					infos.PayFieldMoney = (string)reader["PayFieldMoney"];
					infos.PayAutoMoney = (string)reader["PayAutoMoney"];
					infos.AutoPayTime = (DateTime)reader["AutoPayTime"];
					infos.AutoValidDate = (int)reader["AutoValidDate"];
					infos.VipLimitLevel = (int)reader["VipLimitLevel"];
					infos.FarmerName = (string)reader["FarmerName"];
					infos.GainFieldId = (int)reader["GainFieldId"];
					infos.MatureId = (int)reader["MatureId"];
					infos.KillCropId = (int)reader["KillCropId"];
					infos.isAutoId = (int)reader["isAutoId"];
					infos.isFarmHelper = (bool)reader["isFarmHelper"];
					infos.buyExpRemainNum = (int)reader["buyExpRemainNum"];
					infos.isArrange = (bool)reader["isArrange"];
					infos.TreeLevel = (int)reader["TreeLevel"];
					infos.TreeExp = (int)reader["TreeExp"];
					infos.LoveScore = (int)reader["LoveScore"];
					infos.MonsterExp = (int)reader["MonsterExp"];
					infos.PoultryState = (int)reader["PoultryState"];
					infos.CountDownTime = (DateTime)reader["CountDownTime"];
					infos.TreeCostExp = (int)reader["TreeCostExp"];
					return infos;
				}
			}
			catch (Exception e)
			{
				if (log.IsErrorEnabled)
					log.Error("GetSingleFarm", e);
			}
			finally
			{
				if (reader != null && !reader.IsClosed)
					reader.Close();
			}

			return null;
		}

		public bool AddFarm(UserFarmInfo info)
		{
			bool result = false;
			try
			{
				SqlParameter[] para = new SqlParameter[22];
				para[0] = new SqlParameter("@FarmID", info.FarmID);
				para[1] = new SqlParameter("@PayFieldMoney", info.PayFieldMoney);
				para[2] = new SqlParameter("@PayAutoMoney", info.PayAutoMoney);
				para[3] = new SqlParameter("@AutoPayTime", info.AutoPayTime.ToString());
				para[4] = new SqlParameter("@AutoValidDate", info.AutoValidDate);
				para[5] = new SqlParameter("@VipLimitLevel", info.VipLimitLevel);
				para[6] = new SqlParameter("@FarmerName", info.FarmerName);
				para[7] = new SqlParameter("@GainFieldId", info.GainFieldId);
				para[8] = new SqlParameter("@MatureId", info.MatureId);
				para[9] = new SqlParameter("@KillCropId", info.KillCropId);
				para[10] = new SqlParameter("@isAutoId", info.isAutoId);
				para[11] = new SqlParameter("@isFarmHelper", info.isFarmHelper);
				para[12] = new SqlParameter("@ID", info.ID);
				para[12].Direction = ParameterDirection.Output;
				para[13] = new SqlParameter("@buyExpRemainNum", info.buyExpRemainNum);
				para[14] = new SqlParameter("@isArrange", info.isArrange);
				para[15] = new SqlParameter("@TreeLevel", info.TreeLevel);
				para[16] = new SqlParameter("@TreeExp", info.TreeExp);
				para[17] = new SqlParameter("@LoveScore", info.LoveScore);
				para[18] = new SqlParameter("@MonsterExp", info.MonsterExp);
				para[19] = new SqlParameter("@PoultryState", info.PoultryState);
				para[20] = new SqlParameter("@CountDownTime", info.CountDownTime);
				para[21] = new SqlParameter("@TreeCostExp", info.TreeCostExp);
				result = db.RunProcedure("SP_Users_Farm_Add", para);
				info.ID = (int)para[12].Value;
				info.IsDirty = false;
			}
			catch (Exception e)
			{
				if (log.IsErrorEnabled)
					log.Error("Init", e);
			}
			finally
			{
			}

			return result;
		}

		public bool UpdateFarm(UserFarmInfo info)
		{
			bool result = false;
			try
			{
				SqlParameter[] para = new SqlParameter[22];
				para[0] = new SqlParameter("@ID", info.ID);
				para[1] = new SqlParameter("@FarmID", info.FarmID);
				para[2] = new SqlParameter("@PayFieldMoney", info.PayFieldMoney);
				para[3] = new SqlParameter("@PayAutoMoney", info.PayAutoMoney);
				para[4] = new SqlParameter("@AutoPayTime", info.AutoPayTime.ToString());
				para[5] = new SqlParameter("@AutoValidDate", info.AutoValidDate);
				para[6] = new SqlParameter("@VipLimitLevel", info.VipLimitLevel);
				para[7] = new SqlParameter("@FarmerName", info.FarmerName);
				para[8] = new SqlParameter("@GainFieldId", info.GainFieldId);
				para[9] = new SqlParameter("@MatureId", info.MatureId);
				para[10] = new SqlParameter("@KillCropId", info.KillCropId);
				para[11] = new SqlParameter("@isAutoId", info.isAutoId);
				para[12] = new SqlParameter("@isFarmHelper", info.isFarmHelper);
				para[13] = new SqlParameter("@buyExpRemainNum", info.buyExpRemainNum);
				para[14] = new SqlParameter("@isArrange", info.isArrange);
				para[15] = new SqlParameter("@TreeLevel", info.TreeLevel);
				para[16] = new SqlParameter("@TreeExp", info.TreeExp);
				para[17] = new SqlParameter("@LoveScore", info.LoveScore);
				para[18] = new SqlParameter("@MonsterExp", info.MonsterExp);
				para[19] = new SqlParameter("@PoultryState", info.PoultryState);
				para[20] = new SqlParameter("@CountDownTime", info.CountDownTime);
				para[21] = new SqlParameter("@TreeCostExp", info.TreeCostExp);
				result = db.RunProcedure("SP_Users_Farm_Update", para);
			}
			catch (Exception e)
			{
				if (log.IsErrorEnabled)
					log.Error("Init", e);
			}

			return result;
		}

		public UserFieldInfo[] GetSingleFields(int ID)
		{
			List<UserFieldInfo> infos = new List<UserFieldInfo>();
			SqlDataReader reader = null;
			try
			{
				SqlParameter[] para = new SqlParameter[1];
				para[0] = new SqlParameter("@ID", SqlDbType.Int, 4);
				para[0].Value = ID;
				db.GetReader(ref reader, "SP_Get_SingleFields", para);
				while (reader.Read())
				{
					UserFieldInfo info = new UserFieldInfo();
					info.ID = (int)reader["ID"];
					info.FarmID = (int)reader["FarmID"];
					info.FieldID = (int)reader["FieldID"];
					info.SeedID = (int)reader["SeedID"];
					info.PlantTime = (DateTime)reader["PlantTime"];
					info.AccelerateTime = (int)reader["AccelerateTime"];
					info.FieldValidDate = (int)reader["FieldValidDate"];
					info.PayTime = (DateTime)reader["PayTime"];
					info.GainCount = (int)reader["GainCount"];
					info.AutoSeedID = (int)reader["AutoSeedID"];
					info.AutoFertilizerID = (int)reader["AutoFertilizerID"];
					info.AutoSeedIDCount = (int)reader["AutoSeedIDCount"];
					info.AutoFertilizerCount = (int)reader["AutoFertilizerCount"];
					info.isAutomatic = (bool)reader["isAutomatic"];
					info.AutomaticTime = (DateTime)reader["AutomaticTime"];
					info.IsExit = (bool)reader["IsExit"];
					info.payFieldTime = (int)reader["payFieldTime"];
					infos.Add(info);
				}
			}
			catch (Exception e)
			{
				if (log.IsErrorEnabled)
					log.Error("SP_GetSingleFields", e);
			}
			finally
			{
				if (reader != null && !reader.IsClosed)
					reader.Close();
			}

			return infos.ToArray();
		}

		public bool AddFields(UserFieldInfo item)
		{
			bool result = false;
			try
			{
				SqlParameter[] para = new SqlParameter[17];
				para[0] = new SqlParameter("@FarmID", item.FarmID);
				para[1] = new SqlParameter("@FieldID", item.FieldID);
				para[2] = new SqlParameter("@SeedID", item.SeedID);
				para[3] = new SqlParameter("@PlantTime", item.PlantTime.ToString());
				para[4] = new SqlParameter("@AccelerateTime", item.AccelerateTime);
				para[5] = new SqlParameter("@FieldValidDate", item.FieldValidDate);
				para[6] = new SqlParameter("@PayTime", item.PayTime.ToString());
				para[7] = new SqlParameter("@GainCount", item.GainCount);
				para[8] = new SqlParameter("@AutoSeedID", item.AutoSeedID);
				para[9] = new SqlParameter("@AutoFertilizerID", item.AutoFertilizerID);
				para[10] = new SqlParameter("@AutoSeedIDCount", item.AutoSeedIDCount);
				para[11] = new SqlParameter("@AutoFertilizerCount", item.AutoFertilizerCount);
				para[12] = new SqlParameter("@isAutomatic", item.isAutomatic);
				para[13] = new SqlParameter("@AutomaticTime", item.AutomaticTime.ToString());
				para[14] = new SqlParameter("@IsExit", item.IsExit);
				para[15] = new SqlParameter("@payFieldTime", item.payFieldTime);
				para[16] = new SqlParameter("@ID", item.ID);
				para[16].Direction = ParameterDirection.Output;
				result = db.RunProcedure("SP_Users_Fields_Add", para);
				item.ID = (int)para[16].Value;
				item.IsDirty = false;
			}
			catch (Exception e)
			{
				if (log.IsErrorEnabled)
					log.Error("Init", e);
			}
			finally
			{
			}

			return result;
		}

		public bool UpdateFields(UserFieldInfo info)
		{
			bool result = false;
			try
			{
				SqlParameter[] para = new SqlParameter[17];
				para[0] = new SqlParameter("@ID", info.ID);
				para[1] = new SqlParameter("@FarmID", info.FarmID);
				para[2] = new SqlParameter("@FieldID", info.FieldID);
				para[3] = new SqlParameter("@SeedID", info.SeedID);
				para[4] = new SqlParameter("@PlantTime", info.PlantTime.ToString());
				para[5] = new SqlParameter("@AccelerateTime", info.AccelerateTime);
				para[6] = new SqlParameter("@FieldValidDate", info.FieldValidDate);
				para[7] = new SqlParameter("@PayTime", info.PayTime.ToString());
				para[8] = new SqlParameter("@GainCount", info.GainCount);
				para[9] = new SqlParameter("@AutoSeedID", info.AutoSeedID);
				para[10] = new SqlParameter("@AutoFertilizerID", info.AutoFertilizerID);
				para[11] = new SqlParameter("@AutoSeedIDCount", info.AutoSeedIDCount);
				para[12] = new SqlParameter("@AutoFertilizerCount", info.AutoFertilizerCount);
				para[13] = new SqlParameter("@isAutomatic", info.isAutomatic);
				para[14] = new SqlParameter("@AutomaticTime", info.AutomaticTime.ToString());
				para[15] = new SqlParameter("@IsExit", info.IsExit);
				para[16] = new SqlParameter("@payFieldTime", info.payFieldTime);
				result = db.RunProcedure("SP_Users_Fields_Update", para);
			}
			catch (Exception e)
			{
				if (log.IsErrorEnabled)
					log.Error("Init", e);
			}

			return result;
		}

		public NewChickenBoxItemInfo[] GetSingleNewChickenBox(int UserID)
		{
			List<NewChickenBoxItemInfo> list = new List<NewChickenBoxItemInfo>();
			SqlDataReader sqlDataReader = null;
			try
			{
				SqlParameter[] array = new SqlParameter[]
				{
					new SqlParameter("@UserID", SqlDbType.Int, 4)
				};
				array[0].Value = UserID;
				this.db.GetReader(ref sqlDataReader, "SP_GetSingleNewChickenBox", array);
				while (sqlDataReader.Read())
				{
					list.Add(new NewChickenBoxItemInfo
					{
						ID = (int)sqlDataReader["ID"],
						UserID = (int)sqlDataReader["UserID"],
						TemplateID = (int)sqlDataReader["TemplateID"],
						Count = (int)sqlDataReader["Count"],
						ValidDate = (int)sqlDataReader["ValidDate"],
						StrengthenLevel = (int)sqlDataReader["StrengthenLevel"],
						AttackCompose = (int)sqlDataReader["AttackCompose"],
						DefendCompose = (int)sqlDataReader["DefendCompose"],
						AgilityCompose = (int)sqlDataReader["AgilityCompose"],
						LuckCompose = (int)sqlDataReader["LuckCompose"],
						Position = (int)sqlDataReader["Position"],
						IsSelected = (bool)sqlDataReader["IsSelected"],
						IsSeeded = (bool)sqlDataReader["IsSeeded"],
						IsBinds = (bool)sqlDataReader["IsBinds"]
					});
				}
			}
			catch (Exception ex)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("SP_GetSingleNewChickenBox", ex);
				}
			}
			finally
			{
				if (sqlDataReader != null && !sqlDataReader.IsClosed)
				{
					sqlDataReader.Close();
				}
			}
			return list.ToArray();
		}

		public bool AddNewChickenBox(NewChickenBoxItemInfo info)
		{
			bool result = false;
			try
			{
				SqlParameter[] array = new SqlParameter[15];
				array[0] = new SqlParameter("@ID", info.ID);
				array[0].Direction = ParameterDirection.Output;
				array[1] = new SqlParameter("@UserID", info.UserID);
				array[2] = new SqlParameter("@TemplateID", info.TemplateID);
				array[3] = new SqlParameter("@Count", info.Count);
				array[4] = new SqlParameter("@ValidDate", info.ValidDate);
				array[5] = new SqlParameter("@StrengthenLevel", info.StrengthenLevel);
				array[6] = new SqlParameter("@AttackCompose", info.AttackCompose);
				array[7] = new SqlParameter("@DefendCompose", info.DefendCompose);
				array[8] = new SqlParameter("@AgilityCompose", info.AgilityCompose);
				array[9] = new SqlParameter("@LuckCompose", info.LuckCompose);
				array[10] = new SqlParameter("@Position", info.Position);
				array[11] = new SqlParameter("@IsSelected", info.IsSelected);
				array[12] = new SqlParameter("@IsSeeded", info.IsSeeded);
				array[13] = new SqlParameter("@IsBinds", info.IsBinds);
				array[14] = new SqlParameter("@Result", SqlDbType.Int);
				array[14].Direction = ParameterDirection.ReturnValue;
				this.db.RunProcedure("SP_NewChickenBox_Add", array);
				result = ((int)array[14].Value == 0);
				info.ID = (int)array[0].Value;
				info.IsDirty = false;
			}
			catch (Exception ex)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("SP_NewChickenBox_Add", ex);
				}
			}
			return result;
		}

		public bool UpdateNewChickenBox(NewChickenBoxItemInfo info)
		{
			bool result = false;
			try
			{
				SqlParameter[] array = new SqlParameter[]
				{
					new SqlParameter("@ID", info.ID),
					new SqlParameter("@UserID", info.UserID),
					new SqlParameter("@TemplateID", info.TemplateID),
					new SqlParameter("@Count", info.Count),
					new SqlParameter("@ValidDate", info.ValidDate),
					new SqlParameter("@StrengthenLevel", info.StrengthenLevel),
					new SqlParameter("@AttackCompose", info.AttackCompose),
					new SqlParameter("@DefendCompose", info.DefendCompose),
					new SqlParameter("@AgilityCompose", info.AgilityCompose),
					new SqlParameter("@LuckCompose", info.LuckCompose),
					new SqlParameter("@Position", info.Position),
					new SqlParameter("@IsSelected", info.IsSelected),
					new SqlParameter("@IsSeeded", info.IsSeeded),
					new SqlParameter("@IsBinds", info.IsBinds),
					new SqlParameter("@Result", SqlDbType.Int)
				};
				array[14].Direction = ParameterDirection.ReturnValue;
				this.db.RunProcedure("SP_UpdateNewChickenBox", array);
				result = ((int)array[14].Value == 0);
			}
			catch (Exception ex)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("SP_UpdateNewChickenBox", ex);
				}
			}
			return result;
		}

		public ActiveSystemInfo GetSingleActiveSystem(int UserID)
		{
			SqlDataReader reader = null;
			try
			{
				SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@UserID", SqlDbType.Int, 4)
				};
				para[0].Value = UserID;
				this.db.GetReader(ref reader, "SP_GetSingleActiveSystem", para);
				if (reader.Read())
				{
					return new ActiveSystemInfo
					{
						ID = (int)reader["ID"],
						UserID = (int)reader["UserID"],
						canEagleEyeCounts = (int)reader["canEagleEyeCounts"],
						canOpenCounts = (int)reader["canOpenCounts"],
						isShowAll = (bool)reader["isShowAll"],
						lastFlushTime = (DateTime)reader["lastFlushTime"],
						cardScore = (int)reader["cardScore"],
						cardFreeCount = (int)reader["cardFreeCount"],
						cardChipCount = (int)reader["cardChipCount"],
						cardListCard = reader["cardListCard"] == DBNull.Value ? "" : reader["cardListCard"].ToString(),
						cardListAward = reader["cardListAward"] == DBNull.Value ? "" : reader["cardListAward"].ToString(),
						cardListExchange = reader["cardListExchange"] == DBNull.Value ? "" : reader["cardListExchange"].ToString(),
						SXCrystal = (int)reader["SXCrystal"],
						SXStepRemain = (int)reader["SXStepRemain"],
						SXScore = (int)reader["SXScore"],
						SXMapInfoData = reader["SXMapInfoData"] == DBNull.Value ? "" : (string)reader["SXMapInfoData"],
						MiniShopBuyCount = reader["MiniShopBuyCount"] == DBNull.Value ? "" : (string)reader["MiniShopBuyCount"],
						SXRewardsGet = reader["SXRewardsGet"] == DBNull.Value ? "" : (string)reader["SXRewardsGet"],
						CryptBoss = reader["CryptBoss"] == DBNull.Value ? "" : reader["CryptBoss"].ToString(),
						ChallengeNum = (int)reader["ChallengeNum"],
						BuyBuffNum = (int)reader["BuyBuffNum"],
						DamageNum = (int)reader["DamageNum"],
						BoxState = reader["BoxState"] == DBNull.Value ? "" : reader["BoxState"].ToString(),
						lastEnterYearMonter = (DateTime)reader["lastEnterYearMonter"],
						ChickActiveData = reader["ChickActiveData"] == DBNull.Value ? "" : reader["ChickActiveData"].ToString()
					};
				}
			}
			catch (Exception e)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("GetSingleActiveSystem", e);
				}
			}
			finally
			{
				if (reader != null && !reader.IsClosed)
				{
					reader.Close();
				}
			}
			return null;
		}

		public bool AddActiveSystem(ActiveSystemInfo info)
		{
			bool result = false;
			try
            {
				SqlParameter[] para = new SqlParameter[26];
				para[0] = new SqlParameter("@ID", info.ID);
				para[0].Direction = ParameterDirection.Output;
				para[1] = new SqlParameter("@UserID", info.UserID);
				para[2] = new SqlParameter("@canEagleEyeCounts", info.canEagleEyeCounts);
				para[3] = new SqlParameter("@canOpenCounts", info.canOpenCounts);
				para[4] = new SqlParameter("@isShowAll", info.isShowAll);
				para[5] = new SqlParameter("@lastFlushTime", info.lastFlushTime);
				para[6] = new SqlParameter("@cardScore", info.cardScore);
				para[7] = new SqlParameter("@cardFreeCount", info.cardFreeCount);
				para[8] = new SqlParameter("@cardChipCount", info.cardChipCount);
				para[9] = new SqlParameter("@cardListCard", info.cardListCard == null ? "" : info.cardListExchange);
				para[10] = new SqlParameter("@cardListAward", info.cardListAward == null ? "" : info.cardListAward);
				para[11] = new SqlParameter("@cardListExchange", info.cardListExchange == null ? "" : info.cardListExchange);
				para[12] = new SqlParameter("@SXCrystal", info.SXCrystal);
				para[13] = new SqlParameter("@SXStepRemain", info.SXStepRemain);
				para[14] = new SqlParameter("@SXScore", info.SXScore);
				para[15] = new SqlParameter("@SXMapInfoData", info.SXMapInfoData);
				para[16] = new SqlParameter("@MiniShopBuyCount", info.MiniShopBuyCount);
				para[17] = new SqlParameter("@SXRewardsGet", info.SXRewardsGet);
				para[18] = new SqlParameter("@CryptBoss", info.CryptBoss);
				para[19] = new SqlParameter("@ChallengeNum", info.ChallengeNum);
				para[20] = new SqlParameter("@BuyBuffNum", info.BuyBuffNum);
				para[21] = new SqlParameter("@DamageNum", info.DamageNum);
				para[22] = new SqlParameter("@BoxState", info.BoxState);
				para[23] = new SqlParameter("@lastEnterYearMonter", info.lastEnterYearMonter);
				para[24] = new SqlParameter("@ChickActiveData", info.ChickActiveData);
				para[25] = new SqlParameter("@Result", SqlDbType.Int);
				para[25].Direction = ParameterDirection.ReturnValue;
				this.db.RunProcedure("SP_ActiveSystem_Add", para);
				result = ((int)para[25].Value == 0);
				info.ID = (int)para[0].Value;
				info.IsDirty = false;
			}
			catch (Exception e)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("AddActiveSystem", e);
				}
			}
			return result;
		}

		public bool UpdateActiveSystem(ActiveSystemInfo info)
		{
			bool flag = false;
			try
            {
				SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@ID", info.ID),
					new SqlParameter("@UserID", info.UserID),
					new SqlParameter("@canEagleEyeCounts",info.canEagleEyeCounts),
					new SqlParameter("@canOpenCounts",info.canOpenCounts),
					new SqlParameter("@isShowAll",info.isShowAll),
					new SqlParameter("@lastFlushTime",info.lastFlushTime),
					new SqlParameter("@cardScore",info.cardScore),
					new SqlParameter("@cardFreeCount",info.cardFreeCount),
					new SqlParameter("@cardChipCount",info.cardChipCount),
					new SqlParameter("@cardListCard",info.cardListCard),
					new SqlParameter("@cardListAward",info.cardListAward),
					new SqlParameter("@cardListExchange",info.cardListExchange),
					new SqlParameter("@SXCrystal",info.SXCrystal),
					new SqlParameter("@SXStepRemain",info.SXStepRemain),
					new SqlParameter("@SXScore",info.SXScore),
					new SqlParameter("@SXMapInfoData", info.SXMapInfoData),
					new SqlParameter("@MiniShopBuyCount", info.MiniShopBuyCount),
					new SqlParameter("@SXRewardsGet", info.SXRewardsGet),
					new SqlParameter("@CryptBoss",info.CryptBoss),
					new SqlParameter("@ChallengeNum",info.ChallengeNum),
					new SqlParameter("@BuyBuffNum",info.BuyBuffNum),
					new SqlParameter("@DamageNum",info.DamageNum),
					new SqlParameter("@BoxState", info.BoxState),
					new SqlParameter("@lastEnterYearMonter", info.lastEnterYearMonter),
					new SqlParameter("@ChickActiveData",info.ChickActiveData),
					new SqlParameter("@Result", SqlDbType.Int)
				};
				para[25].Direction = ParameterDirection.ReturnValue;
				this.db.RunProcedure("SP_UpdateActiveSystem", para);
				flag = ((int)para[25].Value == 0);
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("UpdateActiveSystem", exception);
				}
			}
			return flag;
		}

		public string[] GetUserNameAndNickNameByUserID(int UserID)
		{
			string[] returnval = new string[2];
			SqlDataReader sqlDataReader = null;
			try
			{
				SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@UserId", UserID),
				};
				this.db.GetReader(ref sqlDataReader, "SP_GetUserName_NickName_By_UserId", para);
				while (sqlDataReader.Read())
				{
					returnval[0] = (string)sqlDataReader["UserName"];
					returnval[1] = (string)sqlDataReader["NickName"];
				}
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("UpdateActiveSystem", exception);
				}
			}
			return returnval;
		}

		public bool UpdateHappyRechargeData(HappyRechargeInfo hristos)
		{
			bool value = false;
			try
			{
				SqlParameter[] sqlParameter = new SqlParameter[]
				{
					new SqlParameter("@UserID", (object)hristos.UserID),
					new SqlParameter("@LotteryCount", hristos.LotteryCount),
					new SqlParameter("@LotteryTicket", hristos.LotteryTicket)
				};
				value = this.db.RunProcedure("SP_Insert_Update_HappyRechargeData", sqlParameter);
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("SP_Insert_Update_HappyRechargeData", exception);
				}
			}
			return value;
		}

		public HappyRechargeInfo GetHappyRechargeData(int ID)
		{
			HappyRechargeInfo userHappyRechargeInfo = null;
			SqlDataReader sqlDataReader = null;
			try
			{
				try
				{
					SqlParameter[] sqlParameter = new SqlParameter[] { new SqlParameter("@UserID", SqlDbType.Int, 4) };
					sqlParameter[0].Value = ID;
					this.db.GetReader(ref sqlDataReader, "SP_GetUserHappyRechargeData", sqlParameter);
					while (sqlDataReader.Read())
					{
						userHappyRechargeInfo = this.InitHappyRechargeInfo(sqlDataReader);
					}
				}
				catch (Exception exception)
				{
					if (BaseBussiness.log.IsErrorEnabled)
						BaseBussiness.log.Error("SP_GetUserHappyRechargeData", exception);
				}
			}
			finally
			{
				if (sqlDataReader != null && !sqlDataReader.IsClosed)
					sqlDataReader.Close();
			}
			return userHappyRechargeInfo;
		}

		public HappyRechargeInfo InitHappyRechargeInfo(SqlDataReader reader)
		{
			return new HappyRechargeInfo()
			{
				ID = (int)reader["ID"],
				UserID = (int)reader["UserID"],
				LotteryCount = (int)reader["LotteryCount"],
				LotteryTicket = (int)reader["LotteryTicket"]
			};
		}

		public UserGemStone InitGemStones(SqlDataReader reader)
		{
			UserGemStone info = new UserGemStone();
			info.ID = (int)reader["ID"];
			info.UserID = (int)reader["UserID"];
			info.FigSpiritId = (int)reader["FigSpiritId"];
			info.FigSpiritIdValue = (string)reader["FigSpiritIdValue"];
			info.EquipPlace = (int)reader["EquipPlace"];
			return info;
		}

		public List<UserGemStone> GetSingleGemstones(int ID)
		{
			List<UserGemStone> infos = new List<UserGemStone>();
			SqlDataReader reader = null;
			try
			{
				SqlParameter[] para = new SqlParameter[1];
				para[0] = new SqlParameter("@ID", SqlDbType.Int, 4);
				para[0].Value = ID;
				db.GetReader(ref reader, "SP_GetSingleGemStone", para);
				while (reader.Read())
				{
					infos.Add(InitGemStones(reader));
				}
			}
			catch (Exception e)
			{
				if (log.IsErrorEnabled)
					log.Error("SP_GetSingleUserGemStones", e);
			}
			finally
			{
				if (reader != null && !reader.IsClosed)
					reader.Close();
			}

			return infos;
		}

		public List<UserGemStone> GetSingleGemStones(int ID)
		{
			List<UserGemStone> infos = new List<UserGemStone>();
			SqlDataReader reader = null;
			try
			{
				SqlParameter[] para = new SqlParameter[1];
				para[0] = new SqlParameter("@ID", SqlDbType.Int, 4);
				para[0].Value = ID;
				db.GetReader(ref reader, "SP_GetSingleGemStone", para);
				while (reader.Read())
				{
					infos.Add(InitGemStones(reader));
				}
			}
			catch (Exception e)
			{
				if (log.IsErrorEnabled)
					log.Error("SP_GetSingleUserGemStones", e);
			}
			finally
			{
				if (reader != null && !reader.IsClosed)
					reader.Close();
			}

			return infos;
		}

		public bool AddUserGemStone(UserGemStone item)
		{
			bool result = false;
			try
			{
				SqlParameter[] para = new SqlParameter[6];
				para[0] = new SqlParameter("@ID", item.ID);
				para[0].Direction = ParameterDirection.Output;
				para[1] = new SqlParameter("@UserID", item.UserID);
				para[2] = new SqlParameter("@FigSpiritId", item.FigSpiritId);
				para[3] = new SqlParameter("@FigSpiritIdValue", item.FigSpiritIdValue);
				para[4] = new SqlParameter("@EquipPlace", item.EquipPlace);
				para[5] = new SqlParameter("@Result", System.Data.SqlDbType.Int);
				para[5].Direction = ParameterDirection.ReturnValue;
				db.RunProcedure("SP_Users_GemStones_Add", para);
				int returnValue = (int)para[5].Value;
				result = returnValue == 0;
				item.ID = (int)para[0].Value;
				item.IsDirty = false;
			}
			catch (Exception e)
			{
				if (log.IsErrorEnabled)
					log.Error("Init", e);
			}
			finally
			{
			}

			return result;
		}

		public bool UpdateGemStoneInfo(UserGemStone g)
		{
			bool flag = false;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[]
				{
					new SqlParameter("@ID", g.ID),
					new SqlParameter("@UserID", g.UserID),
					new SqlParameter("@FigSpiritId", g.FigSpiritId),
					new SqlParameter("@FigSpiritIdValue", g.FigSpiritIdValue),
					new SqlParameter("@EquipPlace", g.EquipPlace),
					new SqlParameter("@Result", SqlDbType.Int)
				};
				sqlParameters[5].Direction = ParameterDirection.ReturnValue;
				db.RunProcedure("SP_UpdateGemStoneInfo", sqlParameters);
				flag = true;
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("SP_UpdateGemStoneInfo", exception);
				}
			}

			return flag;
		}

		public UserChristmasInfo GetSingleUserChristmas(int UserID)
		{
			SqlDataReader reader = null;
			try
			{
				SqlParameter[] para = new SqlParameter[1];
				para[0] = new SqlParameter("@UserID", SqlDbType.Int, 4);
				para[0].Value = UserID;
				db.GetReader(ref reader, "SP_GetSingleUserChristmas", para);
				while (reader.Read())
				{
					UserChristmasInfo info = new UserChristmasInfo();
					info.ID = (int)reader["ID"];
					info.UserID = (int)reader["UserID"];
					info.exp = (int)reader["exp"];
					info.awardState = (int)reader["awardState"];
					info.count = (int)reader["count"];
					info.packsNumber = (int)reader["packsNumber"];
					info.lastPacks = (int)reader["lastPacks"];
					info.gameBeginTime = (DateTime)reader["gameBeginTime"];
					info.gameEndTime = (DateTime)reader["gameEndTime"];
					info.isEnter = (bool)reader["isEnter"];
					info.dayPacks = (int)reader["dayPacks"];
					info.AvailTime = (int)reader["AvailTime"];
					return info;
				}
			}
			catch (Exception e)
			{
				if (log.IsErrorEnabled)
					log.Error("SP_GetSingleUserChristmas", e);
			}
			finally
			{
				if (reader != null && !reader.IsClosed)
					reader.Close();
			}

			return null;
		}

		public bool AddUserChristmas(UserChristmasInfo info)
		{
			bool result = false;
			try
			{
				SqlParameter[] para = new SqlParameter[13];
				para[0] = new SqlParameter("@ID", info.ID);
				para[0].Direction = ParameterDirection.Output;
				para[1] = new SqlParameter("@UserID", info.UserID);
				para[2] = new SqlParameter("@exp", info.exp);
				para[3] = new SqlParameter("@awardState", info.awardState);
				para[4] = new SqlParameter("@count", info.count);
				para[5] = new SqlParameter("@packsNumber", info.packsNumber);
				para[6] = new SqlParameter("@lastPacks", info.lastPacks);
				para[7] = new SqlParameter("@gameBeginTime", info.gameBeginTime);
				para[8] = new SqlParameter("@gameEndTime", info.gameEndTime);
				para[9] = new SqlParameter("@isEnter", info.isEnter);
				para[10] = new SqlParameter("@dayPacks", info.dayPacks);
				para[11] = new SqlParameter("@AvailTime", info.AvailTime);
				para[12] = new SqlParameter("@Result", System.Data.SqlDbType.Int);
				para[12].Direction = ParameterDirection.ReturnValue;
				db.RunProcedure("SP_UserChristmas_Add", para);
				result = (int)para[12].Value == 0;
				info.ID = (int)para[0].Value;
				info.IsDirty = false;
			}
			catch (Exception e)
			{
				if (log.IsErrorEnabled)
					log.Error("Init", e);
			}
			finally
			{
			}

			return result;
		}

		public bool UpdateUserChristmas(UserChristmasInfo info)
		{
			bool flag = false;
			try
			{
				SqlParameter[] para = new SqlParameter[13];
				para[0] = new SqlParameter("@ID", info.ID);
				para[1] = new SqlParameter("@UserID", info.UserID);
				para[2] = new SqlParameter("@exp", info.exp);
				para[3] = new SqlParameter("@awardState", info.awardState);
				para[4] = new SqlParameter("@count", info.count);
				para[5] = new SqlParameter("@packsNumber", info.packsNumber);
				para[6] = new SqlParameter("@lastPacks", info.lastPacks);
				para[7] = new SqlParameter("@gameBeginTime", info.gameBeginTime);
				para[8] = new SqlParameter("@gameEndTime", info.gameEndTime);
				para[9] = new SqlParameter("@isEnter", info.isEnter);
				para[10] = new SqlParameter("@dayPacks", info.dayPacks);
				para[11] = new SqlParameter("@AvailTime", info.AvailTime);
				para[12] = new SqlParameter("@Result", System.Data.SqlDbType.Int);
				para[12].Direction = ParameterDirection.ReturnValue;
				db.RunProcedure("SP_UpdateUserChristmas", para);
				flag = (int)para[12].Value == 0;
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("SP_UpdateUserChristmas", exception);
				}
			}

			return flag;
		}

		public UserAvatarColectionInfo[] GetSingleUserAvatarColectionInfo(int UserID)
		{
			List<UserAvatarColectionInfo> infos = new List<UserAvatarColectionInfo>();
			SqlDataReader reader = null;
			try
			{
				SqlParameter[] para = new SqlParameter[1];
				para[0] = new SqlParameter("@UserID", SqlDbType.Int, 4);
				para[0].Value = UserID;
				db.GetReader(ref reader, "SP_Get_Single_User_Avatar_Colection", para);
				while (reader.Read())
				{
					UserAvatarColectionInfo info = new UserAvatarColectionInfo
					{
						ID = (int)reader["ID"],
						UserID = (int)reader["UserID"],
						endTime = (DateTime)reader["endTime"],
						dataId = (int)reader["dataId"],
						ActiveCount = (int)reader["ActiveCount"],
						Sex = (int)reader["Sex"],
						ActiveDress = reader["ActiveDress"] == null ? "" : reader["ActiveDress"].ToString()
					};
					infos.Add(info);
				}
			}
			catch (Exception e)
			{
				if (log.IsErrorEnabled)
					log.Error("SP_Get_Single_User_Avatar_Colection", e);
			}
			finally
			{
				if (reader != null && !reader.IsClosed)
					reader.Close();
			}

			return infos.ToArray();
		}

		public bool AddUserAvatarColectionInfo(UserAvatarColectionInfo info)
		{
			bool result = false;
			try
			{
				SqlParameter[] para = new SqlParameter[8];
				para[0] = new SqlParameter("@ID", info.ID);
				para[0].Direction = ParameterDirection.Output;
				para[1] = new SqlParameter("@UserID", info.UserID);
				para[2] = new SqlParameter("@endTime", info.endTime);
				para[3] = new SqlParameter("@dataId", info.dataId);
				para[4] = new SqlParameter("@ActiveCount", info.ActiveCount);
				para[5] = new SqlParameter("@Sex", info.Sex);
				para[6] = new SqlParameter("@ActiveDress", info.ActiveDress);
				para[7] = new SqlParameter("@Result", System.Data.SqlDbType.Int);
				para[7].Direction = ParameterDirection.ReturnValue;
				db.RunProcedure("SP_User_Avatar_Colection_Add", para);
				result = (int)para[7].Value == 0;
				info.ID = (int)para[0].Value;
				info.IsDirty = false;
			}
			catch (Exception e)
			{
				if (log.IsErrorEnabled)
					log.Error("SP_User_Avatar_Colection_Add", e);
			}
			finally
			{
			}

			return result;
		}

		public bool UpdateUserAvatarColectionInfo(UserAvatarColectionInfo info)
		{
			bool flag = false;
			try
			{
				SqlParameter[] para = new SqlParameter[8];
				para[0] = new SqlParameter("@ID", info.ID);
				para[1] = new SqlParameter("@UserID", info.UserID);
				para[2] = new SqlParameter("@endTime", info.endTime);
				para[3] = new SqlParameter("@dataId", info.dataId);
				para[4] = new SqlParameter("@ActiveCount", info.ActiveCount);
				para[5] = new SqlParameter("@Sex", info.Sex);
				para[6] = new SqlParameter("@ActiveDress", info.ActiveDress);
				para[7] = new SqlParameter("@Result", System.Data.SqlDbType.Int);
				para[7].Direction = ParameterDirection.ReturnValue;
				db.RunProcedure("SP_User_Avatar_Colection_Update", para);
				flag = (int)para[7].Value == 0;
			}
			catch (Exception exception)
			{
				if (log.IsErrorEnabled)
				{
					log.Error("SP_User_Avatar_Colection_Update", exception);
				}
			}

			return flag;
		}

		public PlayerInfo GetUserSingleByInviteCode(string inviteCode)
		{
			SqlDataReader resultDataReader = null;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@inviteCode", SqlDbType.NVarChar, 200) };
				sqlParameters[0].Value = inviteCode;
				base.db.GetReader(ref resultDataReader, "SP_Users_SingleByInviteCode", sqlParameters);
				if (resultDataReader.Read())
				{
					return this.InitPlayerInfo(resultDataReader);
				}
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("Init", exception);
				}
			}
			finally
			{
				if (!((resultDataReader == null) || resultDataReader.IsClosed))
				{
					resultDataReader.Close();
				}
			}
			return null;
		}

		#region Pyramid
		public PyramidInfo GetSinglePyramid(int UserID)
		{
			SqlDataReader reader = null;
			try
			{
				SqlParameter[] para = new SqlParameter[1];
				para[0] = new SqlParameter("@UserID", SqlDbType.Int, 4);
				para[0].Value = UserID;
				db.GetReader(ref reader, "SP_GetSinglePyramid", para);
				while (reader.Read())
				{
					PyramidInfo info = new PyramidInfo();
					info.ID = (int)reader["ID"];
					info.UserID = (int)reader["UserID"];
					info.currentLayer = (int)reader["currentLayer"];
					info.maxLayer = (int)reader["maxLayer"];
					info.totalPoint = (int)reader["totalPoint"];
					info.turnPoint = (int)reader["turnPoint"];
					info.pointRatio = (int)reader["pointRatio"];
					info.currentFreeCount = (int)reader["currentFreeCount"];
					info.currentReviveCount = (int)reader["currentReviveCount"];
					info.isPyramidStart = (bool)reader["isPyramidStart"];
					info.LayerItems = (string)reader["LayerItems"];
					info.currentCountNow = (int)reader["currentCountNow"];
					return info;
				}
			}
			catch (Exception e)
			{
				if (log.IsErrorEnabled)
					log.Error("SP_GetSinglePyramid", e);
			}
			finally
			{
				if (reader != null && !reader.IsClosed)
					reader.Close();
			}

			return null;
		}

		public bool AddPyramid(PyramidInfo info)
		{
			bool result = false;
			try
			{
				SqlParameter[] para = new SqlParameter[13];
				para[0] = new SqlParameter("@ID", info.ID);
				para[0].Direction = ParameterDirection.Output;
				para[1] = new SqlParameter("@UserID", info.UserID);
				para[2] = new SqlParameter("@currentLayer", info.currentLayer);
				para[3] = new SqlParameter("@maxLayer", info.maxLayer);
				para[4] = new SqlParameter("@totalPoint", info.totalPoint);
				para[5] = new SqlParameter("@turnPoint", info.turnPoint);
				para[6] = new SqlParameter("@pointRatio", info.pointRatio);
				para[7] = new SqlParameter("@currentFreeCount", info.currentFreeCount);
				para[8] = new SqlParameter("@currentReviveCount", info.currentReviveCount);
				para[9] = new SqlParameter("@isPyramidStart", info.isPyramidStart);
				para[10] = new SqlParameter("@LayerItems", info.LayerItems);
				para[11] = new SqlParameter("@currentCountNow", info.currentCountNow);
				para[12] = new SqlParameter("@Result", System.Data.SqlDbType.Int);
				para[12].Direction = ParameterDirection.ReturnValue;
				db.RunProcedure("SP_Pyramid_Add", para);
				result = (int)para[12].Value == 0;
				info.ID = (int)para[0].Value;
				info.IsDirty = false;
			}
			catch (Exception e)
			{
				if (log.IsErrorEnabled)
					log.Error("SP_Pyramid_Add", e);
			}
			finally
			{
			}

			return result;
		}

		public bool UpdatePyramid(PyramidInfo info)
		{
			bool flag = false;
			try
			{
				SqlParameter[] para = new SqlParameter[13];
				para[0] = new SqlParameter("@ID", info.ID);
				para[1] = new SqlParameter("@UserID", info.UserID);
				para[2] = new SqlParameter("@currentLayer", info.currentLayer);
				para[3] = new SqlParameter("@maxLayer", info.maxLayer);
				para[4] = new SqlParameter("@totalPoint", info.totalPoint);
				para[5] = new SqlParameter("@turnPoint", info.turnPoint);
				para[6] = new SqlParameter("@pointRatio", info.pointRatio);
				para[7] = new SqlParameter("@currentFreeCount", info.currentFreeCount);
				para[8] = new SqlParameter("@currentReviveCount", info.currentReviveCount);
				para[9] = new SqlParameter("@isPyramidStart", info.isPyramidStart);
				para[10] = new SqlParameter("@LayerItems", info.LayerItems);
				para[11] = new SqlParameter("@currentCountNow", info.currentCountNow);
				para[12] = new SqlParameter("@Result", System.Data.SqlDbType.Int);
				para[12].Direction = ParameterDirection.ReturnValue;
				db.RunProcedure("SP_UpdatePyramid", para);
				flag = (int)para[12].Value == 0;
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("SP_UpdatePyramid", exception);
				}
			}

			return flag;
		}
        #endregion

        public ExplorerManualInfo InitExplorerManualInfo(int iD)
        {
            SqlDataReader sqlDataReader = null;
            try
            {
                SqlParameter[] paramenter = new SqlParameter[1];
                paramenter[0] = new SqlParameter("@UserID", iD);
                this.db.GetReader(ref sqlDataReader, "SP_Single_ExplorerManualInfo", paramenter);
                if (sqlDataReader.Read())
                {
                    ExplorerManualInfo o = new ExplorerManualInfo();
                    for (int x = 0; x < sqlDataReader.FieldCount; x++)
                    {
                        if (o.GetType().GetProperty(sqlDataReader.GetName(x)) == null)
                        {
                            BaseBussiness.log.Info("Campo " + sqlDataReader.GetName(x) + " Faltando em " + o.GetType());
                            continue;
                        }

                        o.GetType().GetProperty(sqlDataReader.GetName(x)).SetValue(o, sqlDataReader[x]);
                    }

                    o.IsDirty = false;

                    return o;
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", exception);
                }
            }
            finally
            {
                if (sqlDataReader != null && !sqlDataReader.IsClosed)
                {
                    sqlDataReader.Close();
                }
            }

            return null;
        }

        public List<PagesInfo> InitExplorerManualInfoPages(int iD)
        {
            SqlDataReader sqlDataReader = null;
            try
            {
                List<PagesInfo> list = new List<PagesInfo>();
                SqlParameter[] paramenter = new SqlParameter[1];
                paramenter[0] = new SqlParameter("@UserID", iD);
                this.db.GetReader(ref sqlDataReader, "SP_Single_ExplorerManualInfoPages", paramenter);
                while (sqlDataReader.Read())
                {
                    PagesInfo o = new PagesInfo();
                    for (int x = 0; x < sqlDataReader.FieldCount; x++)
                    {
                        if (o.GetType().GetProperty(sqlDataReader.GetName(x)) == null)
                        {
                            BaseBussiness.log.Info("Campo " + sqlDataReader.GetName(x) + " Faltando em " + o.GetType());
                            continue;
                        }

                        o.GetType().GetProperty(sqlDataReader.GetName(x)).SetValue(o, sqlDataReader[x]);
                    }

                    o.IsDirty = false;
                    list.Add(o);
                }

                return list;
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", exception);
                }
            }
            finally
            {
                if (sqlDataReader != null && !sqlDataReader.IsClosed)
                {
                    sqlDataReader.Close();
                }
            }

            return null;
        }

        public Dictionary<int, DebrisInfo> InitExplorerManualDebris(int iD)
        {
            SqlDataReader sqlDataReader = null;
            Dictionary<int, DebrisInfo> list = new Dictionary<int, DebrisInfo>();
            try
            {
                SqlParameter[] paramenter = new SqlParameter[1];
                paramenter[0] = new SqlParameter("@UserID", iD);
                this.db.GetReader(ref sqlDataReader, "SP_Single_ExplorerManualInfoDebris", paramenter);
                while (sqlDataReader.Read())
                {
                    DebrisInfo o = new DebrisInfo();
                    for (int x = 0; x < sqlDataReader.FieldCount; x++)
                    {
                        if (o.GetType().GetProperty(sqlDataReader.GetName(x)) == null)
                        {
                            BaseBussiness.log.Info("Campo " + sqlDataReader.GetName(x) + " Faltando em " + o.GetType());
                            continue;
                        }

                        o.GetType().GetProperty(sqlDataReader.GetName(x)).SetValue(o, sqlDataReader[x]);
                    }

                    o.IsDirty = false;

                    if (!list.ContainsKey(o.ID))
                    {
                        list.Add(o.ID, o);
                    }
                }

                return list;
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", exception);
                }
            }
            finally
            {
                if (sqlDataReader != null && !sqlDataReader.IsClosed)
                {
                    sqlDataReader.Close();
                }
            }

            return null;
        }

        public bool updateExplorerManualInfo(PlayerInfo m_character)
        {
            bool result = false;
            try
            {
                List<SqlParameter> list = new List<SqlParameter>();
                list.Add(new SqlParameter("@UserID", m_character.ID));
                ExplorerManualInfo info = m_character.explorerManualInfo;
                if (!info.IsDirty)
                {
                    return true;
                }

                foreach (PropertyInfo field in info.GetType().GetProperties())
                {
                    if (field.CustomAttributes.Count() > 0)
                    {
                        list.Add(new SqlParameter(field.Name, field.GetValue(info)));
                    }
                }

                db.RunProcedure("SP_Users_Update_ExplorerManualInfo", list.ToArray());
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("updateExplorerManualInfo", exception);
                }
            }

            return result;
        }

        public bool updateExplorerManualInfoPages(PlayerInfo m_character)
        {
            bool result = false;
            try
            {
                foreach (PagesInfo info in m_character.explorerManualInfo.activesPage)
                {
                    if (!info.IsDirty)
                    {
                        continue;
                    }

                    List<SqlParameter> list = new List<SqlParameter>();
                    list.Add(new SqlParameter("@UserID", m_character.ID));
                    foreach (PropertyInfo field in info.GetType().GetProperties())
                    {
                        if (field.CustomAttributes.Count() > 0)
                        {
                            list.Add(new SqlParameter(field.Name, field.GetValue(info)));
                        }
                    }

                    db.RunProcedure("SP_Users_Update_ExplorerManualPages", list.ToArray());
                }
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("updateExplorerManualPages", exception);
                }
            }

            return result;
        }

        public bool updateExplorerManualInfoDebris(PlayerInfo m_character)
        {
            bool result = false;
            try
            {
                foreach (DebrisInfo info in m_character.explorerManualInfo.debris.Values)
                {
                    if (!info.IsDirty)
                    {
                        continue;
                    }

                    List<SqlParameter> list = new List<SqlParameter>();
                    list.Add(new SqlParameter("@UserID", m_character.ID));
                    foreach (PropertyInfo field in info.GetType().GetProperties())
                    {
                        if (field.CustomAttributes.Count() > 0)
                        {
                            list.Add(new SqlParameter(field.Name, field.GetValue(info)));
                        }
                    }

                    db.RunProcedure("SP_Users_Update_ExplorerManualDebris", list.ToArray());
                }
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("updateExplorerManualPages", exception);
                }
            }

            return result;
        }

        #region Code Ga Hanh
        public int ActiveChickCode(int UserID, string Code)
		{
			int result = 3;
			try
			{
				SqlParameter[] para = new SqlParameter[3];
				para[0] = new SqlParameter("@UserID", UserID);
				para[1] = new SqlParameter("@ActiveCode", Code);
				para[2] = new SqlParameter("@Result", System.Data.SqlDbType.Int);
				para[2].Direction = ParameterDirection.ReturnValue;
				db.RunProcedure("SP_Active_ChickCode", para);
				result = (int)para[2].Value;
			}
			catch (Exception e)
			{
				if (log.IsErrorEnabled)
					log.Error("Init", e);
			}

			return result;
		}
		#endregion


		public bool IncreasePVP1vs1(int PlayerID)
		{
			bool flag = false;
			DateTime now = DateTime.Now;
			try
			{
				flag = db.RunProcedure("SP_Increase_User_PVP_1vs1_For_BXH", new SqlParameter[5]
				{
					new SqlParameter("@UserID", PlayerID),
					new SqlParameter("@Year", now.Year),
					new SqlParameter("@Month", now.Month),
					new SqlParameter("@Date", now.Day),
					new SqlParameter("@Hour", now.Hour),
				});
				return flag;
			}
			catch (Exception ex2)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("SP_Increase_User_PVP_1vs1_For_BXH", ex2);
					return flag;
				}
				return flag;
			}
		}
	}
}