using Game.Base.Packets;
using Game.Logic.Phy.Object;

namespace Game.Logic.Cmd
{
    [GameCommand((byte)eTankCmdType.AIRPLANE, "使用飞行技能")]
    public class FlyCommand : ICommandHandler
    {
        public void HandleCommand(BaseGame game, Player player, GSPacketIn packet)
        {
            //foreach (var p in player.Game.GetAllEnemyPlayersByPixel(player, 300))
            //{
            //    if ((p.HoaKhuyen_TruMauPixel || p.HuyenVu_NuiDe) && player.Game is PVPGame)
            //    {
            //        player.Game.method_53(player, "Không thể bay");
            //        return;
            //    }
            //}
            if (player.IsAttacking)
            {
                player.UseFlySkill();
            }
        }
    }
}