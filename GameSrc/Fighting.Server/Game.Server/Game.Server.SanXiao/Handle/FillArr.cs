using System.Collections.Generic;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Packets;
using Newtonsoft.Json;
using SqlDataProvider.Data;

namespace Game.Server.SanXiao.Handle
{
    [SanXiaoHandleAttbute((byte)SanXiaoPackageType.FILL_ARR)]
    public class FillArr : ISanXiaoCommandHadler
    {
        public int CommandHandler(GamePlayer player, GSPacketIn packet)
        {
            Dictionary<int, Dictionary<int, SXCellDataInfo>> mapInfos =
                JsonConvert.DeserializeObject<Dictionary<int, Dictionary<int, SXCellDataInfo>>>(player.Actives.Info
                    .SXMapInfoData);

            int length = packet.ReadInt();

            for (int i = 0; i < length; i++)
            {
                int row = packet.ReadInt();
                int col = packet.ReadInt();
                int type = packet.ReadInt();

                mapInfos[row][col].type = type;
            }

            player.Actives.Info.SXMapInfoData = JsonConvert.SerializeObject(mapInfos);

            return 1;
        }
    }
}