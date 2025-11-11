using Game.Server.GameObjects;
using System.Collections.Generic;

namespace Game.Server.Rooms
{
    public class EnterWaitingRoomAction : IAction
    {
        GamePlayer m_player;

        public EnterWaitingRoomAction(GamePlayer player)
        {
            m_player = player;
        }

        public void Execute()
        {
            if (m_player == null)
            {
                return;
            }

            if (m_player.CurrentRoom != null)
            {
                m_player.CurrentRoom.RemovePlayerUnsafe(m_player);
            }

            BaseWaitingRoom waitingRoom = RoomMgr.WaitingRoom;
            if (waitingRoom.AddPlayer(m_player))
            {
                List<BaseRoom> tempList = RoomMgr.GetAllRooms();
                m_player.Out.SendUpdateRoomList(tempList);
                GamePlayer[] players = waitingRoom.GetPlayersSafe();
                foreach (GamePlayer p in players)
                {
                    if (p != m_player)
                    {
                        m_player.Out.SendSceneAddPlayer(p);
                    }
                }
            }
        }
    }
}