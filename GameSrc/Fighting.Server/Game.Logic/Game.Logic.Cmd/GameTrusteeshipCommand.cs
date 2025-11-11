using Game.Base.Packets;
using Game.Logic.Phy.Object;

namespace Game.Logic.Cmd
{
    /// <summary>
    /// 使用道具协议
    /// </summary>
    [GameCommand((byte)eTankCmdType.GAME_TRUSTEESHIP, "使用道具")]
    public class GameTrusteeshipCommand : ICommandHandler
    {
        public void HandleCommand(BaseGame game, Player player, GSPacketIn packet)
        {
            //if (game.GameState != eGameState.Playing || player.GetSealState())
            //return;
            bool trusteeshipState = packet.ReadBoolean();

            GSPacketIn pkg = new GSPacketIn((int)ePackageTypeLogic.GAME_CMD);
            pkg.WriteByte((byte)eTankCmdType.GAME_TRUSTEESHIP);
            pkg.Parameter2 = player.Id;

            pkg.WriteInt(game.PlayerCount);
            foreach (Player p in game.GetAllPlayers())
            {
                pkg.WriteInt(p.Id);
                pkg.WriteBoolean(false);
            }

            player.PlayerDetail.SendTCP(pkg);
            //var _loc_2:* = event.pkg.readInt();
            //if (_loc_2 == 0)
            //{
            //    return;
            //}
            //var _loc_3:int = 0;
            //while (_loc_3 <= (_loc_2 - 1))
            //{

            //   _loc_4 = event.pkg.readInt();
            //   _loc_5 = _gameInfo.findPlayer(_loc_4);
            //   _loc_5.playerInfo.isTrusteeship = event.pkg.readBoolean();
            //    _loc_3 = _loc_3 + 1;
            //}
        }
    }
}