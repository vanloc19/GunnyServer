using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.Pet.Handle
{
	public interface IPetCommandHadler
	{
		bool CommandHandler(GamePlayer Player, GSPacketIn packet);
	}
}
