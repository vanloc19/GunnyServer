using Game.Server.GameObjects;

namespace Game.Server.Rooms
{
    public class ExitRoomAction : IAction
    {
        BaseRoom m_room;
        GamePlayer m_player;

        public ExitRoomAction(BaseRoom room, GamePlayer player)
        {
            m_room = room;
            m_player = player;
        }

        public void Execute()
        {
            this.m_player.IsQuanChien = false;
            this.m_player.Place = 0;
            m_room.RemovePlayerUnsafe(m_player);
            RoomMgr.WaitingRoom.SendUpdateCurrentRoom(m_room);
            if (m_room.IsEmpty)
            {
                m_room.Stop();
            }
        }
    }
}