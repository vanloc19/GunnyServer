using System.Text;
using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Packets;

namespace Game.Server.Consortia.Handle
{
	[global::Consortia(17)]
	public class ConsortiaUserRemark : IConsortiaCommandHadler
	{
		public int CommandHandler(GamePlayer Player, GSPacketIn packet)
		{
			if (Player.PlayerCharacter.ConsortiaID == 0)
			{
				return 0;
			}
			int num = packet.ReadInt();
			string str = packet.ReadString();
			if (!string.IsNullOrEmpty(str) && Encoding.Default.GetByteCount(str) <= 100)
			{
				bool val = false;
				string msg = "ConsortiaUserRemarkHandler.Failed";
				using (ConsortiaBussiness consortiaBussiness = new ConsortiaBussiness())
				{
					if (consortiaBussiness.UpdateConsortiaUserRemark(num, Player.PlayerCharacter.ConsortiaID, Player.PlayerCharacter.ID, str, ref msg))
					{
						msg = "ConsortiaUserRemarkHandler.Success";
						val = true;
					}
				}
				GSPacketIn packet2 = new GSPacketIn(129);
				packet2.WriteByte(17);
				packet2.WriteInt(num);
				packet2.WriteString(str);
				packet2.WriteBoolean(val);
				packet2.WriteString(LanguageMgr.GetTranslation(msg));
				Player.Out.SendTCP(packet2);
				return 0;
			}
			Player.Out.SendMessage(eMessageType.GM_NOTICE, LanguageMgr.GetTranslation("ConsortiaUserRemarkHandler.Long"));
			return 1;
		}
	}
}
