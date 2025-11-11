using System;
using Bussiness;
using Game.Base.Packets;
using Game.Logic;
using Game.Server.Buffer;
using Game.Server.GameObjects;
using Game.Server.GMActives;
using Game.Server.Managers;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client
{
	[PacketHandler((byte)ePackageType.TEXP, "场景用户离开")]
	public class TexpHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			//client.Out.SendMessage(eMessageType.Normal, "Tính năng bị khóa!");
			//return 0;
			int num = packet.ReadInt();
			int templateId = packet.ReadInt();
			int slot = packet.ReadInt();
			ItemInfo itemAt = client.Player.StoreBag.GetItemAt(slot);
			TexpInfo texp = client.Player.PlayerCharacter.Texp;
			int oldExp = 0;
			if (itemAt == null || texp == null || itemAt.TemplateID != templateId)
			{
				return 0;
			}
			if (!itemAt.isTexp())
			{
				return 0;
			}
			int LimitCountByLevel = client.Player.PlayerCharacter.Grade;
			if (client.Player.UsePayBuff(BuffType.Train_Good))
			{
				AbstractBuffer ofType = client.Player.BufferList.GetOfType(BuffType.Train_Good);
				LimitCountByLevel += ofType.Info.Value;
			}
			if (texp.texpCount >= LimitCountByLevel)
			{
				client.Out.SendMessage(eMessageType.GM_NOTICE, LanguageMgr.GetTranslation("texpSystem.texpCountToplimit"));
			}
			else
			{
				switch (num)
				{
					case 0:
						oldExp = texp.hpTexpExp;
						texp.hpTexpExp += itemAt.Template.Property2;
						client.Player.OnUsingItem(45005, 1);
						break;
					case 1:
						oldExp = texp.attTexpExp;
						texp.attTexpExp += itemAt.Template.Property2;
						client.Player.OnUsingItem(45001, 1);
						break;
					case 2:
						oldExp = texp.defTexpExp;
						texp.defTexpExp += itemAt.Template.Property2;
						client.Player.OnUsingItem(45002, 1);
						break;
					case 3:
						oldExp = texp.spdTexpExp;
						texp.spdTexpExp += itemAt.Template.Property2;
						client.Player.OnUsingItem(45003, 1);
						break;
					case 4:
						oldExp = texp.lukTexpExp;
						texp.lukTexpExp += itemAt.Template.Property2;
						client.Player.OnUsingItem(45004, 1);
						break;
				}
				texp.texpCount++;
				texp.texpTaskCount++;
				texp.texpTaskDate = DateTime.Now;
				using (PlayerBussiness bussiness = new PlayerBussiness())
				{
					bussiness.UpdateUserTexpInfo(texp);
				}
				client.Player.PlayerCharacter.Texp = texp;
				client.Player.OnUsingItem(templateId, 1);
				client.Player.StoreBag.RemoveTemplate(templateId, 1);
				client.Player.EquipBag.UpdatePlayerProperties();
				var attGrade = ExerciseMgr.getLv(client.Player.PlayerCharacter.Texp.attTexpExp);
				var defGrade = ExerciseMgr.getLv(client.Player.PlayerCharacter.Texp.defTexpExp);
				var agiGrade = ExerciseMgr.getLv(client.Player.PlayerCharacter.Texp.spdTexpExp);
				var lukGrade = ExerciseMgr.getLv(client.Player.PlayerCharacter.Texp.lukTexpExp);
				var bloGrade = ExerciseMgr.getLv(client.Player.PlayerCharacter.Texp.hpTexpExp);
				var KingOfTexp = new int[] { attGrade, defGrade, agiGrade, lukGrade, bloGrade };
				var min = KingOfTexp[0];
				for (int i = 0; i < KingOfTexp.Length; i++)
				{
					if (KingOfTexp[i] < min)
					{
						min = KingOfTexp[i];
					}
				}
				GmActivityMgr.OnTexpUp(client.Player, min);
				if (ExerciseMgr.isUp(num, oldExp, texp))
				{
					GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
					for (int i = 0; i < allPlayers.Length; i++)
					{
						//allPlayers[i].Out.SendMessage(eMessageType.ChatNormal, LanguageMgr.GetTranslation("TexpHandler.Success", client.Player.ZoneName, client.Player.PlayerCharacter.NickName, attr));
					}
				}
			}
			return 0;
		}
	}
}
