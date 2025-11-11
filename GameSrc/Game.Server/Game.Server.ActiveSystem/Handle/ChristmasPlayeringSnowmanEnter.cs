using System;
using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Packets;
using Game.Server.Rooms;
using SqlDataProvider.Data;

namespace Game.Server.ActiveSystem.Handle
{
    [ActiveSystemHandleAttbute((byte)ActiveSystemPackageType.CHRISTMAS_PLAYERING_SNOWMAN_ENTER)]
    public class ChristmasPlayeringSnowmanEnter : IActiveSystemCommandHadler
    {
        public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
        {
            GSPacketIn pkg = new GSPacketIn(145, Player.PlayerCharacter.ID);
            byte cmd = packet.ReadByte(); //_loc_2.writeByte(2);
            int vx = -1; //_loc_2.writeInt(param1.x);
            int vy = -1; //_loc_2.writeInt(param1.y);
            BaseChristmasRoom christmasRoom = RoomMgr.ChristmasRoom;
            UserChristmasInfo info = Player.Actives.Christmas;
            if (cmd == 2) //load monter
            {
                vx = packet.ReadInt(); //_loc_2.writeInt(param1.x);
                vy = packet.ReadInt(); //_loc_2.writeInt(param1.y);     
                Player.X = vx;
                Player.Y = vy;
                if (Player.CurrentRoom != null)
                {
                    Player.CurrentRoom.RemovePlayerUnsafe(Player);
                }

                christmasRoom.AddMoreMonters();
                christmasRoom.SetMonterDie(Player.PlayerCharacter.ID);
                if (!Player.Actives.AvailTime())
                {
                    Player.SendMessage(LanguageMgr.GetTranslation("ActiveSystemHandler.Msg1"));
                    //christmas.RemovePlayer(client.Player);
                    return false;
                }

                pkg.WriteByte((byte)ActiveSystemPackageType.CHRISTMAS_MONSTER);
                pkg.WriteByte(0); //var _loc_4:* = _loc_3.readByte();
                pkg.WriteInt(christmasRoom.Monters.Count); //_monsterCount = _loc_3.readInt();
                foreach (MonterInfo monter in christmasRoom.Monters.Values)
                {
                    pkg.WriteInt(monter.ID); //_loc_7.ID = _loc_3.readInt();
                    pkg.WriteInt(monter.type); //_loc_7.type = _loc_3.readInt();
                    pkg.WriteInt(monter.state); //_loc_7.State = _loc_3.readInt();
                    pkg.WriteInt(monter.MonsterPos.X);
                    pkg.WriteInt(monter.MonsterPos
                        .Y); //_loc_7.MonsterPos = new Point(_loc_3.readInt(), _loc_3.readInt());                                
                }

                Player.Out.SendTCP(pkg);
                christmasRoom.ViewOtherPlayerRoom(Player);
            }
            else if (cmd == 0) // enter
            {
                Player.X = christmasRoom.DefaultPosX;
                Player.Y = christmasRoom.DefaultPosY;
                christmasRoom.AddPlayer(Player);
                int min = GameProperties.ChristmasMinute;
                if (!info.isEnter)
                {
                    info.gameBeginTime = DateTime.Now;
                    info.gameEndTime = DateTime.Now.AddMinutes(min);
                    info.isEnter = true;
                    info.AvailTime = min;
                }
                else
                {
                    min = info.AvailTime;
                    info.gameBeginTime = DateTime.Now;
                    info.gameEndTime = DateTime.Now.AddMinutes(min);
                }

                bool isEnter = Player.Actives.AvailTime();
                pkg.WriteByte((byte)ActiveSystemPackageType.CHRISTMAS_PLAYERING_SNOWMAN_ENTER);
                pkg.WriteBoolean(isEnter); //this._model.isEnter = param1.readBoolean();
                pkg.WriteDateTime(info.gameBeginTime); //    this._model.gameBeginTime = param1.readDate();
                pkg.WriteDateTime(info.gameEndTime); //    this._model.gameEndTime = param1.readDate();
                pkg.WriteInt(info.count); //    this._model.count = param1.readInt();
                Player.Out.SendTCP(pkg);
            }
            else if (cmd == 1) // exit
            {
                christmasRoom.RemovePlayer(Player);
            }

            return true;
        }
    }
}