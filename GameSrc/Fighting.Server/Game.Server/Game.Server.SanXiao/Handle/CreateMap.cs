using System.Collections.Generic;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Packets;
using Newtonsoft.Json;
using SqlDataProvider.Data;

namespace Game.Server.SanXiao.Handle
{
    [SanXiaoHandleAttbute((byte)SanXiaoPackageType.CREATE_MAP)]
    public class CreateMap : ISanXiaoCommandHadler
    {
        public int CommandHandler(GamePlayer player, GSPacketIn packet)
        {
            Dictionary<int, Dictionary<int, SXCellDataInfo>> mapInfos =
                JsonConvert.DeserializeObject<Dictionary<int, Dictionary<int, SXCellDataInfo>>>(player.Actives.Info
                    .SXMapInfoData);

            int length = packet.ReadInt();
            int i = 0;

            while (i < length * 3)
            {
                int row = packet.ReadInt();
                int col = packet.ReadInt();
                int type = packet.ReadInt();

                mapInfos[row][col].type = type;

                i += 3;
            }

            player.Actives.Info.SXMapInfoData = JsonConvert.SerializeObject(mapInfos);
            return 0;
        }
    }
}