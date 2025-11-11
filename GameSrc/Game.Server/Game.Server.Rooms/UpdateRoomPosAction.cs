namespace Game.Server.Rooms
{
    public class UpdateRoomPosAction : IAction
    {
        BaseRoom m_room;

        int m_pos;
        int m_place;
        int m_placeView;
        bool m_isOpened;

        public UpdateRoomPosAction(BaseRoom room, int pos, bool isOpened, int place, int placeView)
        {
            m_room = room;
            m_pos = pos;
            m_isOpened = isOpened;
            m_place = place;
            m_placeView = placeView;
        }

        public void Execute()
        {
            if (m_room.PlayerCount > 0 && m_room.UpdatePosUnsafe(m_pos, m_isOpened, m_place, m_placeView))
            {
                RoomMgr.WaitingRoom.SendUpdateCurrentRoom(m_room);
            }
        }
    }
}