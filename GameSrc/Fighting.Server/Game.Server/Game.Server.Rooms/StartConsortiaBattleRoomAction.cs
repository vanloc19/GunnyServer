using System.Collections.Generic;
using Game.Logic;
using Game.Server.GameObjects;
using Game.Server.Games;

namespace Game.Server.Rooms
{
    public class StartConsortiaBattleRoomAction : IAction
    {
        private BaseRoom m_redRoom;

        private BaseRoom m_blueRoom;

        public StartConsortiaBattleRoomAction(BaseRoom red, BaseRoom blue)
        {
            m_redRoom = red;
            m_blueRoom = blue;
        }

        public void Execute()
        {
            List<GamePlayer> redList = m_redRoom.GetPlayers();
            List<GamePlayer> blueList = m_blueRoom.GetPlayers();

            List<IGamePlayer> redMatch = new List<IGamePlayer>();
            List<IGamePlayer> blueMatch = new List<IGamePlayer>();
            foreach (GamePlayer p in redList)
            {
                if (p != null)
                {
                    p.GuildBattleEnemyId = blueList[0].PlayerId;
                    redMatch.Add(p);
                }
            }

            foreach (GamePlayer p in blueList)
            {
                if (p != null)
                {
                    p.GuildBattleEnemyId = redList[0].PlayerId;
                    blueMatch.Add(p);
                }
            }

            BaseGame game = GameMgr.StartChallengePVPGame(redMatch, blueMatch, m_redRoom, m_blueRoom, 0, eRoomType.ConsortiaBattle, eGameType.ConsortiaBattle, 5);
            if (game != null)
            {
                m_blueRoom.StartGame(game);
                m_redRoom.StartGame(game);
            }
            else
            {
                m_blueRoom.IsPlaying = false;
                m_blueRoom.SendPlayerState();

                m_redRoom.IsPlaying = false;
                m_redRoom.SendPlayerState();
            }
        }
    }
}