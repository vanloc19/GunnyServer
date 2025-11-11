using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Packets;
using Game.Server.Rooms;
using System;

namespace Game.Server.GameRoom.Handle
{
    [GameRoomHandleAttbute((byte)eRoomPackageType.GAME_ROOM_UPDATE_PLACE)]
    public class UpdatePlaces : IGameRoomCommandHadler
    {
        public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
        {
            byte playerPlace = packet.ReadByte();
            int place = packet.ReadInt();
            bool isOpen = packet.ReadBoolean();
            int placeView = packet.ReadInt();
            Console.WriteLine($"playerName = {Player.PlayerCharacter.NickName} | isQuanChien = {Player.IsQuanChien}|playerPlace = {playerPlace} | place = {place} | isOpen = {isOpen} | placeView = {placeView}");
            if (Player.CurrentRoom != null && Player == Player.CurrentRoom.Host)
            {

                if (Player.CurrentRoom != null && Player == Player.CurrentRoom.Host)
                {

                    if (playerPlace == 8 || playerPlace == 9)
                    {
                        Player.SendMessage("Coming Soon!");
                        return false;
                    }

                    RoomMgr.UpdateRoomPos(Player.CurrentRoom, (int)playerPlace, isOpen, place, placeView);
                }                
            }
            else
            {
                if (placeView > 7 && Player.EquipBag.GetItemAt(6) == null)
                {
                    Player.SendMessage("Vui lòng mang vũ khí chính vào mới dc quan chiến");
                    return false;
                }
                RoomMgr.ChangePlaceToView(Player.CurrentRoom, (int)playerPlace, isOpen, place, placeView);
                Player.CurrentRoom.UpdateStateViewToPlay(playerPlace);
                Player.Place = placeView;
                Console.WriteLine(placeView.ToString());
                if (placeView < 8)
                {
                    Player.IsQuanChien = false;
                }
                else
                {
                    Player.IsQuanChien = true;
                }
            }
            return true;
        }
    }
}