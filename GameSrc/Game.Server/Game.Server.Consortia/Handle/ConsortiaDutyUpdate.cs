using System.Text;
using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Packets;
using SqlDataProvider.Data;

namespace Game.Server.Consortia.Handle
{
	[global::Consortia(10)]
	public class ConsortiaDutyUpdate : IConsortiaCommandHadler
	{
		public int CommandHandler(GamePlayer Player, GSPacketIn packet)
		{
			if (Player.PlayerCharacter.ConsortiaID == 0)
			{
				return 0;
			}
			int num = packet.ReadInt();
			int updateType = packet.ReadByte();
			string msg = "ConsortiaDutyUpdateHandler.Failed";
			using (ConsortiaBussiness consortiaBussiness = new ConsortiaBussiness())
			{
				ConsortiaDutyInfo info = new ConsortiaDutyInfo();
				info.ConsortiaID = Player.PlayerCharacter.ConsortiaID;
				info.DutyID = num;
				info.IsExist = true;
				info.DutyName = "";
				switch (updateType)
				{
				case 1:
					return 1;
				case 2:
					info.DutyName = packet.ReadString();
					if (!string.IsNullOrEmpty(info.DutyName) && Encoding.Default.GetByteCount(info.DutyName) <= 10)
					{
						info.Right = packet.ReadInt();
						break;
					}
					Player.Out.SendMessage(eMessageType.GM_NOTICE, LanguageMgr.GetTranslation("ConsortiaDutyUpdateHandler.Long"));
					return 1;
				}
				if (consortiaBussiness.UpdateConsortiaDuty(info, Player.PlayerCharacter.ID, updateType, ref msg))
				{
					_ = info.DutyID;
					GameServer.Instance.LoginServer.SendConsortiaDuty(info, updateType, Player.PlayerCharacter.ConsortiaID);
				}
			}
			return 0;
		}
	}
}
