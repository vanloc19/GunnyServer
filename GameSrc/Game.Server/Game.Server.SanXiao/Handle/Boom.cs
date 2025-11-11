using System;
using System.Collections.Generic;
using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Packets;
using Newtonsoft.Json;
using SqlDataProvider.Data;

namespace Game.Server.SanXiao.Handle
{
    [SanXiaoHandleAttbute((byte)SanXiaoPackageType.BOOM)]
    public class Boom : ISanXiaoCommandHadler
    {
        private Random rand = new Random();

        public int CommandHandler(GamePlayer player, GSPacketIn packet)
        {
            if (player.Actives.Info.SXStepRemain <= 0)
            {
                player.SendMessage(LanguageMgr.GetTranslation("SanXiao.NoEnouchStepRemain.Msg"));
                return 1;
            }

            Dictionary<int, Dictionary<int, SXCellDataInfo>> mapInfos =
                JsonConvert.DeserializeObject<Dictionary<int, Dictionary<int, SXCellDataInfo>>>(player.Actives.Info
                    .SXMapInfoData);

            int row1 = packet.ReadInt();
            int col1 = packet.ReadInt();

            int row2 = packet.ReadInt();
            int col2 = packet.ReadInt();

            if (row1 >= 0 && col1 >= 0 && row2 >= 0 && col2 >= 0)
            {
                int tempType1 = mapInfos[row1][col1].type;
                int tempType2 = mapInfos[row2][col2].type;

                mapInfos[row1][col1].type = tempType2;
                mapInfos[row2][col2].type = tempType1;
            }

            int length = packet.ReadInt();

            int totalScore = 0;

            for (int i = 0; i < length; i++)
            {
                int legth2 = packet.ReadInt();
                int count = 0;
                while (count < legth2 * 2)
                {
                    int rowloop = packet.ReadInt();
                    int colloop = packet.ReadInt();

                    if (rowloop >= 0 && colloop >= 0)
                    {
                        mapInfos[rowloop][colloop].type = 0;
                        totalScore++;
                    }

                    count += 2;
                }
            }

            player.Actives.Info.SXScore += totalScore;

            return 0;
        }
    }
}