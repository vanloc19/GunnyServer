//using Game.Base.Packets;
//using Game.Logic.Phy.Object;
//using System;
//namespace Game.Logic.Cmd
//{
//	[GameCommand(16, "游戏加载进度")]
//	public class LoadCommand : ICommandHandler
//	{
//		public void HandleCommand(BaseGame game, Player player, GSPacketIn packet)
//		{
//			player.LoadingProcess = packet.ReadInt();
//			GSPacketIn pkg = new GSPacketIn((short)ePackageType.GAME_CMD);
//			pkg.WriteByte((byte)eTankCmdType.LOAD);
//			if (game.GameState == eGameState.Loading)
//            {
//				if (player.LoadingProcess >= 100)
//                {
//					game.CheckState(0);
//                }
//				pkg.WriteInt(player.LoadingProcess);
//				pkg.WriteInt(player.PlayerDetail.ZoneId);
//				pkg.WriteInt(player.PlayerDetail.PlayerCharacter.ID);
//				game.SendToAll(pkg);
//            }
//		}
//	}
//}
using Game.Base.Packets;
using Game.Logic.Phy.Object;

namespace Game.Logic.Cmd
{
	[GameCommand(16, "游戏加载进度")]
	public class LoadCommand : ICommandHandler
	{
		public void HandleCommand(BaseGame game, Player player, GSPacketIn packet)
		{
			if (game.GameState == eGameState.Loading)
			{
				player.LoadingProcess = packet.ReadInt();
				if (player.LoadingProcess >= 100)
				{
					game.CheckState(0);
				}
				if (player.LoadingProcess < 100)
				{

				}
				GSPacketIn pkg = new GSPacketIn(91);
				pkg.WriteByte(16);
				pkg.WriteInt(player.LoadingProcess);
				pkg.WriteInt(player.PlayerDetail.ZoneId);
				pkg.WriteInt(player.PlayerDetail.PlayerCharacter.ID);
				game.SendToAll(pkg);
			}
		}
	}
}