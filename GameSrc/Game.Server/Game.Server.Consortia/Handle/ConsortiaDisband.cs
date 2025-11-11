using System;
using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.Consortia.Handle
{
	[global::Consortia(2)]
	public class ConsortiaDisband : IConsortiaCommandHadler
	{
		public int CommandHandler(GamePlayer Player, GSPacketIn packet)
		{
			if (Player.PlayerCharacter.ConsortiaID == 0)
			{
				return 0;
			}
			int consortiaId = Player.PlayerCharacter.ConsortiaID;
			string consortiaName = Player.PlayerCharacter.ConsortiaName;
			bool val = false;
			string msg = "ConsortiaDisbandHandler.Failed";
			using (ConsortiaBussiness consortiaBussiness = new ConsortiaBussiness())
			{
				if (consortiaBussiness.DeleteConsortia(Player.PlayerCharacter.ConsortiaID, Player.PlayerCharacter.ID, ref msg))
				{
					val = true;
					msg = "ConsortiaDisbandHandler.Success1";
					Player.ClearConsortia();
					Player.AddLog("ConsortiaDisBand", "Kick khỏi G");
					GameServer.Instance.LoginServer.SendConsortiaDelete(consortiaId);
					Player.PlayerCharacter.QuitConsortiaDate = DateTime.Now;
				}
			}
			string str = LanguageMgr.GetTranslation(msg);
			if (msg == "ConsortiaDisbandHandler.Success1")
			{
				str = str + consortiaName + LanguageMgr.GetTranslation("ConsortiaDisbandHandler.Success2");
			}
			GSPacketIn packet2 = new GSPacketIn(129);
			packet2.WriteByte(2);
			if (val)
			{
				packet2.WriteBoolean(val);
				packet2.WriteInt(Player.PlayerCharacter.ID);
				packet2.WriteString(str);
			}
			else
			{
				packet2.WriteInt(Player.PlayerCharacter.ID);
				packet2.WriteString(str);
			}
			Player.Out.SendTCP(packet2);
			return 0;
		}
	}
}
