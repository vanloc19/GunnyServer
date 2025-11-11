using Bussiness;
using Game.Base.Packets;

namespace Game.Server.Packets.Client
{
    [PacketHandler((byte)ePackageType.LEFT_GUN_ROULETTE_SOCKET, "物品炼化")]
    public class LeftGunHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            if (client.Player.Extra.Info.LeftRoutteCount > 0 && client.Player.Extra.Info.LeftRoutteRate <= 0f)
            {
				RandomSafe randomSafe = new RandomSafe();
				float num = 0f;
				string[] array = GameProperties.LeftRouterRateData.Split('|');
				int num2 = randomSafe.Next(10000);
				if (num2 < 600)
				{
					num = float.Parse(array[4]);
				}
				else if (num2 < 1500)
				{
					num = float.Parse(array[3]);
				}
				else if (num2 < 4000)
				{
					num = float.Parse(array[2]);
				}
				else if (num2 < 7000)
				{
					num = float.Parse(array[1]);
				}
				else if (num2 < 9000)
				{
					num = float.Parse(array[0]);
				}
				if (num > 0f)
				{
					client.Player.Extra.Info.LeftRoutteCount--;
					client.Player.Extra.Info.LeftRoutteRate = num;
				}
				client.Out.SendLeftRouleteResult(client.Player.Extra.Info);
				return 0;
			}
			return 0;
		}
	}
}