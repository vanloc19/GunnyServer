using Game.Base.Packets;
using Game.Logic.Phy.Object;

namespace Game.Logic.Cmd
{
    [GameCommand((byte)eTankCmdType.MOVESTART, "开始移动")]
    public class MoveStartCommand : ICommandHandler
    {
        public void HandleCommand(BaseGame game, Player player, GSPacketIn packet)
        {
            if (player.IsAttacking)
            {
                //GSPacketIn pkg = packet.Clone();
                //pkg.ClientID = player.PlayerDetail.PlayerCharacter.ID;
                //pkg.Parameter1 = player.Id;
                //game.SendToAll(pkg, player.PlayerDetail);
                bool recording = packet.ReadBoolean(); //_loc_7.writeBoolean(true);
                byte type = packet.ReadByte();
                int x = packet.ReadInt();
                int y = packet.ReadInt();
                byte dir = packet.ReadByte();
                bool isLiving = packet.ReadBoolean();
                short map_currentTurn = packet.ReadShort();
                if (game.TurnIndex == map_currentTurn)
                {
                }

                game.SendPlayerMove(player, type, x, y, dir);
                switch (type)
                {
                    case 0:
                    case 1:
                        player.SetXY(x, y);
                        player.StartMoving();
                        if (player.Y - y > 1 || player.IsLiving != isLiving)
                        {
                            game.SendPlayerMove(player, 3, player.X, player.Y, dir, isLiving);
                        }

                        break;
                }
            }
        }
    }
}