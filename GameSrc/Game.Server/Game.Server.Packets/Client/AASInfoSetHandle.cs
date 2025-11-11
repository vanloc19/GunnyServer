using System;
using System.Text.RegularExpressions;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameUtils;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client
{
	internal class AASInfoSetHandle : IPacketHandler
	{
		private static Regex _objRegex;

		private static Regex _objRegex1;

		private static Regex _objRegex2;

		private static string[] cities;

		private static char[] checkCode;

		private static int[] WI;

		static AASInfoSetHandle()
		{
			_objRegex = new Regex("\\d{18}|\\d{15}");
			_objRegex1 = new Regex("/^[1-9]\\d{7}((0\\d)|(1[0-2]))(([0|1|2]\\d)|3[0-1])\\d{3}$/");
			_objRegex2 = new Regex("/^[1-9]\\d{5}[1-9]\\d{3}((0\\d)|(1[0-2]))(([0|1|2]\\d)|3[0-1])\\d{4}$/");
			cities = new string[92]
			{
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
				"北京",
				"天津",
				"河北",
				"山西",
				"内蒙古",
				null,
				null,
				null,
				null,
				null,
				"辽宁",
				"吉林",
				"黑龙江",
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				"上海",
				"江苏",
				"浙江",
				"安微",
				"福建",
				"江西",
				"山东",
				null,
				null,
				null,
				"河南",
				"湖北",
				"湖南",
				"广东",
				"广西",
				"海南",
				null,
				null,
				null,
				"重庆",
				"四川",
				"贵州",
				"云南",
				"西藏",
				null,
				null,
				null,
				null,
				null,
				null,
				"陕西",
				"甘肃",
				"青海",
				"宁夏",
				"新疆",
				null,
				null,
				null,
				null,
				null,
				"台湾",
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				"香港",
				"澳门",
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				"国外"
			};
			WI = new int[17]
			{
				7,
				9,
				10,
				5,
				8,
				4,
				2,
				1,
				6,
				3,
				7,
				9,
				10,
				5,
				8,
				4,
				2
			};
			checkCode = new char[11]
			{
				'1',
				'0',
				'X',
				'9',
				'8',
				'7',
				'6',
				'5',
				'4',
				'3',
				'2'
			};
		}

		private bool CheckIDNumber(string IDNum)
		{
			bool flag = false;
			if (!_objRegex.IsMatch(IDNum))
			{
				return false;
			}
			int index = int.Parse(IDNum.Substring(0, 2));
			if (cities[index] == null)
			{
				return false;
			}
			if (IDNum.Length == 18)
			{
				int num2 = 0;
				for (int i = 0; i < 17; i++)
				{
					num2 += int.Parse(IDNum[i].ToString()) * WI[i];
				}
				int num3 = num2 % 11;
				if (IDNum[17] == checkCode[num3])
				{
					flag = true;
				}
			}
			return flag;
		}

		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			AASInfo info = new AASInfo
			{
				UserID = client.Player.PlayerCharacter.ID
			};
			bool result = false;
			bool flag3;
			if (packet.ReadBoolean())
			{
				info.Name = "";
				info.IDNumber = "";
				info.State = 0;
				flag3 = true;
			}
			else
			{
				info.Name = packet.ReadString();
				info.IDNumber = packet.ReadString();
				flag3 = CheckIDNumber(info.IDNumber);
				if (info.IDNumber != "")
				{
					client.Player.IsAASInfo = true;
					int num = Convert.ToInt32(info.IDNumber.Substring(6, 4));
					int num2 = Convert.ToInt32(info.IDNumber.Substring(10, 2));
					if (DateTime.Now.Year.CompareTo(num + 18) > 0 || (DateTime.Now.Year.CompareTo(num + 18) == 0 && DateTime.Now.Month.CompareTo(num2) >= 0))
					{
						client.Player.IsMinor = false;
					}
				}
				if (info.Name != "" && flag3)
				{
					info.State = 1;
				}
				else
				{
					info.State = 0;
				}
			}
			if (flag3)
			{
				client.Out.SendAASState(result: false);
				using (PlayerBussiness playerB = new PlayerBussiness())
				{
					result = playerB.AddAASInfo(info);
					client.Out.SendAASInfoSet(result);
				}
			}
			if (result && info.State == 1)
			{
				ItemTemplateInfo goods = ItemMgr.FindItemTemplate(11019);
				if (goods != null)
				{
					ItemInfo item = ItemInfo.CreateFromTemplate(goods, 1, 107);
					if (item != null)
					{
						item.IsBinds = true;
						AbstractInventory itemInventory = client.Player.GetItemInventory(item.Template);
						if (itemInventory.AddItem(item, itemInventory.BeginSlot))
						{
							client.Out.SendMessage(eMessageType.ChatNormal, string.Format("Kiểm tra thành công nhận <{0}> ", item.Template.Name));
						}
						else
						{
							client.Out.SendMessage(eMessageType.ChatNormal, string.Format("Đã kiểm tra, túi đầy nhận vật phẩm thất bại"));
						}
					}
				}
			}
			return 0;
		}
	}
}
