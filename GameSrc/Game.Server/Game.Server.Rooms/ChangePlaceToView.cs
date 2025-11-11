namespace Game.Server.Rooms
{
    public class ChangePlaceToView : IAction
    {
        private BaseRoom m_room;
        private int m_pos;
        private int m_place;
        private int m_placeView;
        private bool m_isOpened;

        public ChangePlaceToView(BaseRoom room, int pos, bool isOpened, int place, int placeView)
        {
            this.m_room = room;
            this.m_pos = pos;
            this.m_isOpened = isOpened;
            this.m_place = place;
            this.m_placeView = placeView;
        }

        public void Execute()
        {
            if (this.m_room.PlayerCount <= 0 || !this.m_room.UpdatePosUnsafe2(this.m_pos, this.m_isOpened, this.m_place, this.m_placeView))
                return;
            RoomMgr.WaitingRoom.SendUpdateCurrentRoom(this.m_room);
        }
    }
}