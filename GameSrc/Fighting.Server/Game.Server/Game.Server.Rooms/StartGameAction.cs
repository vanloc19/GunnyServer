using Bussiness;
using Game.Base.Packets;
using Game.Logic;
using Game.Server.Battle;
using Game.Server.GameObjects;
using Game.Server.Games;
using Game.Server.Managers;
using Game.Server.Managers.EliteGame;
using Game.Server.Packets;
using Game.Server.RingStation;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;

namespace Game.Server.Rooms
{
    public class StartGameAction : IAction
    {
        BaseRoom m_room;

        public StartGameAction(BaseRoom room)
        {
            m_room = room;
        }

        public void Execute()
        {
            if (m_room.CanStart())
            {
                m_room.StartWithNpc = false;
                m_room.PickUpNpcId = -1;
                List<GamePlayer> players = m_room.GetPlayers();
                m_room.UpdateGameStyle();
                if (m_room.RoomType == eRoomType.Freedom)
                {
                    List<IGamePlayer> red = new List<IGamePlayer>();
                    List<IGamePlayer> blue = new List<IGamePlayer>();
                    foreach (GamePlayer p in players)
                    {
                        if (p != null)
                        {
                            if (p.CurrentRoomTeam == 1)
                            {
                                red.Add(p);
                            }
                            else
                            {
                                blue.Add(p);
                            }
                        }
                    }
                    BaseGame game = GameMgr.StartPVPGame(m_room.RoomId, red, blue, m_room.MapId, m_room.RoomType, m_room.GameType, m_room.TimeMode);
                    StartGame(game);
                }
                else if (IsPVE(m_room.RoomType))
                {
                    if (GameMgr.SynDate < 0)
                    {
                        foreach (GamePlayer p in players)
                        {
                            if (p != null)
                            {
                                p.Out.SendMessage(eMessageType.ChatERROR, LanguageMgr.GetTranslation("StartGameAction.WaitingPveRestart"));
                            }
                        }

                        return;
                    }
                    List<IGamePlayer> matchPlayers = new List<IGamePlayer>();
                    foreach (GamePlayer p in players)
                    {
                        if (p != null)
                        {
                            matchPlayers.Add(p);
                        }
                    }

                    //更新房间的时间类型
                    UpdatePveRoomTimeMode();
                    BaseGame game = GameMgr.StartPVEGame(m_room.RoomId, matchPlayers, m_room.MapId, m_room.RoomType, m_room.GameType, m_room.TimeMode, m_room.HardLevel, m_room.LevelLimits, m_room.currentFloor);
                    StartGame(game);
                }
                else if (IsPVP(m_room.RoomType))
                {
                    Console.WriteLine($"StartGameAction = {m_room.RoomType}");
                    foreach (GamePlayer p in players)
                    {
                        p.Out.SendCheckCode();
                    }
                    m_room.UpdateAvgLevel();
                    if (!m_room.isCrosszone && m_room.RoomType == eRoomType.Match && m_room.GameStyle == 0 && m_room.Host != null)
                    {
                        if (m_room.Host.PlayerCharacter.Grade <= 3)
                        {
                            m_room.StartWithNpc = true;
                            m_room.PickUpNpcId = RingStationMgr.GetAutoBot(m_room.Host, (int)m_room.RoomType, (int)m_room.GameType);

                        }
                        else
                        {
                            m_room.PickUpNpcId = RingStationConfiguration.NextRoomId();
                            // m_room.StartWithNpc = true;
                            // m_room.PickUpNpcId = RingStationMgr.GetAutoBot(m_room.Host, (int)m_room.RoomType, (int)m_room.GameType);
                        }
                    }

                    if (!m_room.isCrosszone && m_room.RoomType == eRoomType.EliteGameScore && m_room.GameStyle == 0 && m_room.Host != null)
                    {
                        Console.WriteLine("Start Elite Score");
                        if (m_room.Host.PlayerCharacter.Grade <= 3)
                        {
                            m_room.StartWithNpc = true;
                            m_room.PickUpNpcId = RingStationMgr.GetAutoBot(m_room.Host, (int)m_room.RoomType, (int)m_room.GameType);

                        }
                        else
                        {
                            m_room.StartWithNpc = true;
                            m_room.PickUpNpcId = RingStationMgr.GetAutoBot(m_room.Host, (int)m_room.RoomType, (int)m_room.GameType);
                        }
                    }

                    if (!m_room.isCrosszone && m_room.RoomType == eRoomType.EliteGameChampion && m_room.GetPlayers().Count == 1 && m_room.GameStyle == 0 && m_room.Host != null)
                    {
                        Console.WriteLine("Start Elite Champion");
                        PlayerEliteGameInfo roundInfo = EliteGameMgr.FindEliteRoundByUser(m_room.Host.PlayerCharacter.ID, m_room.Host.EliteGroup());
                        if (roundInfo != null)
                        {
                            m_room.Host.CurrentEnemyId = roundInfo.MatchOrderNumber;
                            Console.WriteLine($"___{m_room.Host.CurrentEnemyId} = {roundInfo.MatchOrderNumber}___");
                        }
                        else
                        {
                            GSPacketIn pkg = m_room.Host.Out.SendMessage(eMessageType.ChatERROR, LanguageMgr.GetTranslation("StartGameAction.noEliteGamePlayer"));
                            m_room.SendToAll(pkg, m_room.Host);
                            m_room.SendCancelPickUp();
                        }
                    }

                    BattleServer server = BattleMgr.AddRoom(m_room);
                    if (server != null)
                    {
                        m_room.BattleServer = server;
                        m_room.IsPlaying = true;
                        m_room.SendStartPickUp();
                        if (m_room.RoomType == eRoomType.EliteGameChampion)
                            EliteGameMgr.UpdateEliteBattleStatus(m_room.Host.PlayerCharacter.ID, true, m_room.Host.EliteGroup());
                    }
                    else
                    {
                        GSPacketIn pkg = m_room.Host.Out.SendMessage(eMessageType.ChatERROR, LanguageMgr.GetTranslation("StartGameAction.noBattleServe"));
                        m_room.SendToAll(pkg, m_room.Host);
                        m_room.SendCancelPickUp();
                    }
                    /*Console.WriteLine($"StartGameAction = {m_room.RoomType}");
                    foreach(GamePlayer p in players)
                    {
                        p.Out.SendCheckCode();
                    }   
                    m_room.UpdateAvgLevel();
                    if (!m_room.isCrosszone && m_room.RoomType == eRoomType.Match && m_room.GameStyle == 0 && m_room.Host != null)
                    {
                        if (m_room.Host.PlayerCharacter.Grade >= 2)
                        {
                            m_room.PickUpNpcId = RingStationConfiguration.NextRoomId();
                        }
                        else
                        {
                            m_room.StartWithNpc = true;
                            m_room.PickUpNpcId = RingStationMgr.GetAutoBot(m_room.Host, (int)m_room.RoomType, (int)m_room.GameType);
                        }
                    }

                    BattleServer server = BattleMgr.AddRoom(m_room);
                    if (server != null)
                    {
                        m_room.BattleServer = server;
                        m_room.IsPlaying = true;
                        m_room.SendStartPickUp();
                    }
                    else
                    {
                        GSPacketIn pkg = m_room.Host.Out.SendMessage(eMessageType.ChatERROR, LanguageMgr.GetTranslation("StartGameAction.noBattleServe"));
                        m_room.SendToAll(pkg, m_room.Host);
                        m_room.SendCancelPickUp();
                    }*/
                }
                /*else if (m_room.RoomType == eRoomType.EliteGameScore || m_room.RoomType == eRoomType.EliteGameChampion)
                {
                    Console.WriteLine("______________1");
                    this.m_room.UpdateAvgLevel();
                    if (m_room.GetPlayers().Count == 1)
                    {
                        Console.WriteLine("______________2");
                        if (ExerciseMgr.IsBlockWeapon(this.m_room.Host.MainWeapon.TemplateID))
                        {
                            m_room.Host.SendMessage("Vũ khí bị cấm khi tham gia long thần");
                            m_room.SendCancelPickUp();
                            return;
                        }
                        if (m_room.RoomType == eRoomType.EliteGameChampion)
                        {
                            Console.WriteLine("chay vao day roi");
                            // update enemy
                            PlayerEliteGameInfo roundInfo = EliteGameMgr.FindEliteRoundByUser(m_room.Host.PlayerCharacter.ID, m_room.Host.EliteGroup());
                            if (roundInfo != null)
                            {
                                Console.WriteLine("Co Round");
                                Console.WriteLine($"CurrentEnemyId = {m_room.Host.CurrentEnemyId}___MatchOrderNumber{roundInfo.MatchOrderNumber}");
                                m_room.Host.CurrentEnemyId = roundInfo.MatchOrderNumber;
                            }
                            else
                            {
                                Console.WriteLine("MatRound");
                                m_room.Host.SendMessage("Bạn không có tư cách tham gia giải Long Thần");
                                m_room.SendCancelPickUp();
                                return;
                            }
                            Console.WriteLine("Chay Xuyen Qua");
                        }
                        BattleServer battleServer = BattleMgr.AddRoom(this.m_room);
                        if (battleServer != null)
                        {
                            m_room.BattleServer = battleServer;
                            m_room.IsPlaying = true;
                            m_room.SendStartPickUp();
                            if (m_room.RoomType == eRoomType.EliteGameChampion)
                                EliteGameMgr.UpdateEliteBattleStatus(m_room.Host.PlayerCharacter.ID, true, m_room.Host.EliteGroup());
                        }
                        else
                        {
                            GSPacketIn pkg = m_room.Host.Out.SendMessage(eMessageType.ChatERROR, "Máy chủ chiến đấu chưa sẵn sàng!");
                            m_room.SendToAll(pkg, m_room.Host);
                            m_room.SendCancelPickUp();
                        }
                    }
                }*/
                RoomMgr.WaitingRoom.SendUpdateCurrentRoom(m_room);

            }
        }

        private void StartGame(BaseGame game)
        {
            if (game != null)
            {
                m_room.IsPlaying = true;
                m_room.StartGame(game);
            }
            else
            {
                m_room.IsPlaying = false;
                m_room.SendPlayerState();
                if (m_room.RoomType == eRoomType.Freedom)
                {
                    GSPacketIn pkg = m_room.Host.Out.SendMessage(eMessageType.ChatERROR, LanguageMgr.GetTranslation("StartGameAction.noBattleServe"));
                    m_room.SendToAll(pkg, m_room.Host);
                }
                else
                {
                    GSPacketIn pkg = m_room.Host.Out.SendMessage(eMessageType.ChatERROR, LanguageMgr.GetTranslation("副本尚未开启或者副本已经关闭！"));
                    m_room.SendToAll(pkg, m_room.Host);
                }

                m_room.SendCancelPickUp();
            }
        }

        private bool IsPVE(eRoomType roomType)
        {
            switch (roomType)
            {
                case eRoomType.Dungeon:
                case eRoomType.Freshman:
                case eRoomType.Academy:
                case eRoomType.Boss:
                case eRoomType.FightLab:
                case eRoomType.WordBossFight:
                case eRoomType.Christmas:
                    return true;
            }
            return false;
        }

        private bool IsPVP(eRoomType roomType)
        {
            switch (roomType)
            {
                case eRoomType.Match:
                case eRoomType.EliteGameScore:
                case eRoomType.EliteGameChampion:
                    return true;
            }

            return false;
        }

        private void UpdatePveRoomTimeMode()
        {
            switch (m_room.HardLevel)
            {
                case eHardLevel.Easy:
                    m_room.TimeMode = 3;
                    break;
                case eHardLevel.Normal:
                    m_room.TimeMode = 2;
                    break;
                case eHardLevel.Hard:
                case eHardLevel.Terror:
                case eHardLevel.Nightmare:
                    m_room.TimeMode = 1;
                    break;
            }
        }
    }
}