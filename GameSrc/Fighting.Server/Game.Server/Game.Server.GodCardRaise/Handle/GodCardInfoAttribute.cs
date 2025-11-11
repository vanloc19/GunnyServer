using System;
using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Packets;
using SqlDataProvider.Data;

namespace Game.Server.GodCardRaise.Handle
{
    [GodCardRaiseHandleAttbute((byte)GodCardRaisePackageType.INFO_ATTRIBUTE)]
    public class GodCardInfoAttribute : IGodCardRaiseCommandHadler
    {
        public int CommandHandler(GamePlayer player, GSPacketIn packet)
        {
            GSPacketIn gSPacketIn = new GSPacketIn((short)ePackageType.SAN_XIAO);
            gSPacketIn.WriteByte((byte)GodCardRaisePackageType.INFO_ATTRIBUTE);
            gSPacketIn.WriteInt(player.Actives.ListCards.Count); //_loc_3.readInt(); // sô loại thẻ trong túi
            foreach (GodCardUser card in player.Actives.ListCards)
            {
                gSPacketIn.WriteInt(card.CardId); //_loc_8 = _loc_3.readInt(); // Id thẻ
                gSPacketIn.WriteInt(card.Count); //_loc_2 = _loc_3.readInt(); // Số lượng
            }

            gSPacketIn.WriteInt(player.Actives.Info.cardScore); //_model.score = _loc_3.readInt(); // tích điểm
            gSPacketIn.WriteInt(player.Actives.Info.cardChipCount); //_model.chipCount = _loc_3.readInt(); // mảnh thẻ
            gSPacketIn.WriteInt(player.Actives.Info
                .cardFreeCount); //_model.freeCount = _loc_3.readInt(); // số lần bốc miễn phí
            gSPacketIn.WriteInt(player.Actives.ListAwards
                .Count); //var _loc_5:* = _loc_3.readInt(); // tổng số tích lũy đã nhận
            foreach (int award in player.Actives.ListAwards)
            {
                gSPacketIn.WriteInt(
                    award); //    _model.awardIds[_loc_3.readInt()] = 1; // phần thưởng tích lũy đã nhận theo awardId
            }

            gSPacketIn.WriteInt(player.Actives.ListExchanges
                .Count); //var _loc_12:* = _loc_3.readInt(); // tổng số đã đổi
            foreach (GodCardGroupUser exchange in player.Actives.ListExchanges)
            {
                gSPacketIn.WriteInt(exchange.GroupId); //    _loc_9 = _loc_3.readInt(); // GroupID
                gSPacketIn.WriteInt(exchange.Count); //    _loc_10 = _loc_3.readInt(); // số lượng đã đổi 
            }
            gSPacketIn.WriteDateTime(DateTime.Now);
            /*if (GameProperties.VERSION >= 8700)
            {
                gSPacketIn.WriteDateTime(DateTime.Now); // doubleTime
            }*/

            player.Out.SendTCP(gSPacketIn);
            return 1;
        }
    }
}