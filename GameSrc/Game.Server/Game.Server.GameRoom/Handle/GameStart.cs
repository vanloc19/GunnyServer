using Bussiness;
using Game.Base.Packets;
using Game.Logic;
using Game.Server.GameObjects;
using Game.Server.Packets;
using Game.Server.Rooms;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Server.GameRoom.Handle
{
    [GameRoomHandleAttbute((byte)eRoomPackageType.GAME_START)]
    public class GameStart : IGameRoomCommandHadler
    {
        public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
        {
            BaseRoom room = Player.CurrentRoom;
            if (room != null && room.Host == Player)
            {

                List<GamePlayer> list = room.GetPlayers();
                bool cancelPickup = false;
                if (Player.MainWeapon == null)
                {
                    Player.SendMessage(LanguageMgr.GetTranslation("Game.Server.SceneGames.NoEquip"));
                    cancelPickup = true;
                }
                else if (room.RoomType == eRoomType.Dungeon && !Player.IsPvePermission(room.MapId, room.HardLevel))
                {
                    Player.SendMessage(LanguageMgr.GetTranslation("GameStart.Msg1"));
                    cancelPickup = true;
                }
                else if (room.GameType == eGameType.Guild && room.PlayerCount < 2)
                {
                    Player.SendMessage("Bug Hả Ghê Zị =))))");
                    cancelPickup = true;
                }
                else
                {
                    // int numOfCaptcha = (new Random()).Next(5, 15);
                    // if (room.RoomType == eRoomType.Match && Player.PlayerCharacter.gameUpToCaptcha >= numOfCaptcha)
                    // {
                    //     Player.Client.Out.SendCheckCode();
                    //     Player.CurrentRoom.IsPlaying = false;
                    //     Player.CurrentRoom.SendCancelPickUp();
                    //     return false;
                    // }
                    if (room.MapId == 13)
                    {
                        int tempIdTicket = room.GetDungeonTicketId(room.MapId, room.HardLevel);
                        if (!Player.RemoveTemplate(tempIdTicket, 1))
                        {
                            Player.SendMessage("Bạn chưa có vé cửa Đấu trường dũng sĩ");
                            return false;
                        }

                    }
                    if (room.MapId == 15)
                    {
                        #region OLD
                        /*var msg = "{0}" + LanguageMgr.GetTranslation("GameRoomRoomSetupChange.Msg5");
                        var tempIdTicket = room.GetDungeonTicketId(room.MapId, room.HardLevel);
                        var countNotTicket = 0;
                        var nick = "";

                        foreach (var p in list.Where(p => p.PropBag.GetItemCount(tempIdTicket) <= 0))
                        {
                            if (p.PlayerCharacter.ID == Player.PlayerCharacter.ID)
                            {
                                nick += LanguageMgr.GetTranslation("GameRoomRoomSetupChange.Msg4");
                            }
                            else
                            {
                                nick += p.PlayerCharacter.NickName;
                            }

                            nick += ",";
                            countNotTicket++;
                        }

                        if (countNotTicket > 0)
                        {
                            nick = nick.Substring(0, nick.Length - 1);
                            Player.SendMessage(string.Format(msg, nick));
                            cancelPickup = true;
                        }*/
                        #endregion
                        int dungeonTicketId = room.GetDungeonTicketId(room.MapId, room.HardLevel);
                        if (!Player.RemoveTemplate(dungeonTicketId, 1))
                        {
                            Player.SendMessage("Bạn chưa có vé tàu hải tặc");
                            return false;
                        }

                    }
                    if (room.MapId == 16)
                    {
                        int tempIdTicket = room.GetDungeonTicketId(room.MapId, room.HardLevel);
                        if (!Player.RemoveTemplate(tempIdTicket, 1))
                        {
                            Player.SendMessage("Bạn chưa có vé cửa đảo hải tặc");
                            return false;
                        }

                    }
                    if (room.MapId == 17)
                    {
                        int tempIdTicket = room.GetDungeonTicketId(room.MapId, room.HardLevel);
                        if (!Player.RemoveTemplate(tempIdTicket, 1))
                        {
                            Player.SendMessage("Bạn chưa có vé cửa kho báu người cá");
                            return false;
                        }

                    }

                    if (!cancelPickup)
                    {
                        foreach (GamePlayer p in list)
                        {
                            if (p == null)
                            {
                                continue;
                            }
                        }
                        Player.PlayerCharacter.gameUpToCaptcha++;
                        RoomMgr.StartGame(Player.CurrentRoom);
                    }
                }

                if (cancelPickup)
                {
                    Player.CurrentRoom.IsPlaying = false;
                    Player.CurrentRoom.SendCancelPickUp();
                }
            }

            return true;
        }
    }
}