using Bussiness;
using Game.Base.Packets;
using Game.Logic;
using Game.Server.GameObjects;
using Game.Server.Packets;
using System;
using System.Collections.Generic;

namespace Game.Server.Rooms
{
    class EnterRoomAction : IAction
    {
        private GamePlayer m_player;

        private int m_roomId;

        private String m_pwd;

        private int m_type;

        private bool _isInvite;

        public EnterRoomAction(GamePlayer player, int roomId, string pwd, int type, bool isInvite)
        {
            m_player = player;

            m_roomId = roomId;

            m_pwd = pwd;

            m_type = type;

            _isInvite = isInvite;
        }

        public void Execute()
        {
            if (m_player.IsActive == false)
            {
                return;
            }

            if (m_player.CurrentRoom != null)
            {
                m_player.CurrentRoom.RemovePlayerUnsafe(m_player);
            }

            BaseRoom[] rooms = RoomMgr.Rooms;
            BaseRoom rm = null;
            if (m_roomId == -1)
            {
                for (int i = 0; i < rooms.Length; i++)
                {
                    if (rooms[i].PlayerCount > 0 && rooms[i].CanAddPlayer() && rooms[i].NeedPassword == false && !rooms[i].IsPlaying)
                    {
                        if ((int)eRoomType.Freshman != m_type)
                        {
                            /*if ((int)(rooms[i].RoomType) == m_type)
                            {
                                rm = rooms[i];
                            }*/
                            if ((int)(rooms[i].EnterType) == m_type)
                            {
                                rm = rooms[i];
                            }
                        }
                        else
                        {
                            /*if ((int)(rooms[i].RoomType) == m_type && rooms[i].LevelLimits < (int)rooms[i].GetLevelLimit(m_player))
                            {
                                rm = rooms[i];
                            }*/
                            if ((int)(rooms[i].EnterType) == m_type && rooms[i].LevelLimits < (int)rooms[i].GetLevelLimit(m_player))
                            {
                                rm = rooms[i];
                            }
                        }
                    }
                }

                if (rm == null)
                {
                    m_player.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("EnterRoomAction.Msg1"));
                    m_player.Out.SendRoomLoginResult(false);
                    return;
                }
            }
            else
            {
                rm = rooms[m_roomId == 0 ? m_roomId : m_roomId - 1];
            }

            if (!rm.IsUsing)
            {
                m_player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("EnterRoomAction.Msg2"));
                return;
            }

            if (rm.IsPlaying && !this._isInvite)
            {
                m_player.Out.SendMessage(eMessageType.Normal, "Phòng đang chơi không thể tham gia!");
                return;
            }


            if (rm.PlayerCount == rm.PlacesCount)
            {
                if (rm.CanAddViewPlayer())
                {
                    RoomMgr.WaitingRoom.RemovePlayer(m_player);
                    m_player.Out.SendRoomLoginResult(true);
                    m_player.Out.SendRoomCreate(rm);

                    if (rm.AddPlayerUnsafe(m_player))
                    {
                        rm.Game?.AddPlayer(m_player);
                    }
                    RoomMgr.WaitingRoom.SendUpdateCurrentRoom(rm);
                    m_player.Out.SendGameRoomSetupChange(rm);
                    rm.UpdatePlayerState(m_player, 1, false);
                }
                else
                {
                    m_player.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("EnterRoomAction.Msg4"));
                }
            }
            else
            {
                if (rm.NeedPassword == false || rm.Password == m_pwd || _isInvite)
                {
                    if (rm.Game == null || rm.Game.CanAddPlayer())
                    {
                        if (rm.LevelLimits > (int)rm.GetLevelLimit(m_player))
                        {
                            m_player.Out.SendMessage(eMessageType.ERROR,
                                LanguageMgr.GetTranslation("EnterRoomAction.Msg5"));
                            return;
                        }

                        RoomMgr.WaitingRoom.RemovePlayer(m_player);
                        m_player.Out.SendRoomLoginResult(true);
                        m_player.Out.SendRoomCreate(rm);

                        if (rm.AddPlayerUnsafe(m_player))
                        {
                            rm.Game?.AddPlayer(m_player);
                        }

                        RoomMgr.WaitingRoom.SendUpdateCurrentRoom(rm);
                        m_player.Out.SendGameRoomSetupChange(rm);
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(rm.Password) == false)
                    {
                        if (string.IsNullOrEmpty(m_pwd) == false)
                        {
                            m_player.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("EnterRoomAction.Msg6"));
                        }

                        GSPacketIn pkg = new GSPacketIn((short)Packets.ePackageType.GAME_ROOM);
                        pkg.WriteByte((byte)eRoomPackageType.ROOM_PASS);
                        pkg.WriteInt(m_roomId);
                        m_player.SendTCP(pkg);
                    }
                    else
                    {
                        m_player.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("EnterRoomAction.Msg6"));
                        m_player.Out.SendRoomLoginResult(false);
                    }
                }
            }
        }
    }
}