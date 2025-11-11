using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.Consortia.Handle
{
	public interface IConsortiaCommandHadler
	{
		int CommandHandler(GamePlayer Player, GSPacketIn packet);
	}
}
