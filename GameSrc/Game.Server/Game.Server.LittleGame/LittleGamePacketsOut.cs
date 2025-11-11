using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.LittleGame.Objects;
using Game.Server.Packets;
using System;
using System.Collections.Generic;

namespace Game.Server.LittleGame
{
    public class LittleGamePacketsOut
    {
        public void SendEnterWorld(GamePlayer player)
        {
            GSPacketIn gSPacketIn = new GSPacketIn((byte)ePackageType.LITTLEGAME_COMMAND, player.PlayerCharacter.ID);
            gSPacketIn.WriteByte((byte)eLittleGamePackageInType.START_LOAD);
            gSPacketIn.WriteInt(1);
            gSPacketIn.WriteInt(1);
            gSPacketIn.WriteString("bogu1,bogu2,bogu3,bogu4,bogu5,bogu6,bogu7,bogu8");
            gSPacketIn.WriteString("2001");
            player.SendTCP(gSPacketIn);
        }

        public void SendLoadComplete(GamePlayer player, Dictionary<int, object> obj)
        {
            GSPacketIn gSPacketIn = new GSPacketIn((byte)ePackageType.LITTLEGAME_COMMAND, player.PlayerCharacter.ID);
            gSPacketIn.WriteByte((byte)eLittleGamePackageInType.GAME_START);
            gSPacketIn.WriteInt(obj.Values.Count);
            foreach (object o in obj.Values)
            {
                if (o is GamePlayer pl)
                {
                    gSPacketIn.WriteInt(pl.LittleGameInfo.ID);
                    gSPacketIn.WriteInt(pl.LittleGameInfo.X);
                    gSPacketIn.WriteInt(pl.LittleGameInfo.Y);
                    gSPacketIn.WriteInt(1);
                    gSPacketIn.WriteInt(pl.PlayerCharacter.ID);
                    gSPacketIn.WriteInt(pl.PlayerCharacter.Grade);
                    gSPacketIn.WriteInt(pl.PlayerCharacter.Repute);
                    gSPacketIn.WriteString(pl.PlayerCharacter.NickName);
                    gSPacketIn.WriteByte(pl.PlayerCharacter.typeVIP);
                    gSPacketIn.WriteInt(pl.PlayerCharacter.VIPLevel);
                    gSPacketIn.WriteBoolean(pl.PlayerCharacter.Sex);
                    gSPacketIn.WriteString(pl.PlayerCharacter.Style);
                    gSPacketIn.WriteString(pl.PlayerCharacter.Colors);
                    gSPacketIn.WriteString(pl.PlayerCharacter.Skin);
                    gSPacketIn.WriteInt(pl.PlayerCharacter.Hide);
                    gSPacketIn.WriteInt(pl.PlayerCharacter.FightPower);
                    gSPacketIn.WriteInt(pl.PlayerCharacter.Win);
                    gSPacketIn.WriteInt(pl.PlayerCharacter.Total);
                    gSPacketIn.WriteBoolean(false);
                    gSPacketIn.WriteInt(pl.LittleGameInfo.Actions.Count);
                    foreach (var action in pl.LittleGameInfo.Actions)
                    {
                        switch (action)
                        {
                            case "livingInhale":
                                gSPacketIn.WriteString("livingInhale");
                                gSPacketIn.WriteInt(pl.LittleGameInfo.ID);
                                gSPacketIn.WriteString(!player.LittleGameInfo.IsBack ? "inhaleSmall" : "backInhaleSmall");
                                gSPacketIn.WriteString(player.LittleGameInfo.Direction);
                                gSPacketIn.WriteInt(10);
                                gSPacketIn.WriteInt(pl.LittleGameInfo.X);
                                gSPacketIn.WriteInt(pl.LittleGameInfo.Y);
                                break;
                            case "livingUnInhale":
                                gSPacketIn.WriteString("livingUnInhale");
                                gSPacketIn.WriteInt(pl.LittleGameInfo.ID);
                                gSPacketIn.WriteString("stand");
                                gSPacketIn.WriteString(player.LittleGameInfo.Direction);
                                break;
                        }
                    }
                }
                if (o is Bogu bg)
                {
                    gSPacketIn.WriteInt(bg.ID);
                    gSPacketIn.WriteInt(bg.X);
                    gSPacketIn.WriteInt(bg.Y);
                    gSPacketIn.WriteInt(2);
                    gSPacketIn.WriteString("温泉小游戏");
                    gSPacketIn.WriteString(bg.Model);
                    gSPacketIn.WriteBoolean(true);
                    gSPacketIn.WriteInt(bg.X);
                    gSPacketIn.WriteInt(bg.Y);
                    gSPacketIn.WriteInt(bg.Action != "" ? 1 : 0);
                    switch (bg.Action)
                    {
                        case "livingInhale":
                            gSPacketIn.WriteString("livingInhale");
                            gSPacketIn.WriteInt(bg.ID);
                            gSPacketIn.WriteString(!bg.IsBack ? "inhaled" : bg.Size == 0 ? "backInhaled" : "inhaled");
                            gSPacketIn.WriteString(bg.Direction);
                            gSPacketIn.WriteInt(10); // live
                            gSPacketIn.WriteInt(bg.X);
                            gSPacketIn.WriteInt(bg.Y);
                            break;
                        case "livingUnInhale":
                            gSPacketIn.WriteString("livingUnInhale");
                            gSPacketIn.WriteInt(bg.ID);
                            gSPacketIn.WriteString("stand");
                            gSPacketIn.WriteString(bg.Direction);
                            break;
                        case "livingDie":
                            gSPacketIn.WriteString("livingDie");
                            gSPacketIn.WriteInt(bg.ID);
                            break;
                    }
                }
            }
            player.SendTCP(gSPacketIn);
        }

        public void SendMoveToAll(GamePlayer player)
        {
            GSPacketIn gSPacketIn = new GSPacketIn((byte)ePackageType.LITTLEGAME_COMMAND);
            gSPacketIn.WriteByte((byte)eLittleGamePackageInType.MOVE);
            gSPacketIn.WriteInt(player.LittleGameInfo.ID);
            gSPacketIn.WriteInt(player.LittleGameInfo.X);
            gSPacketIn.WriteInt(player.LittleGameInfo.Y);
            SendToAll(gSPacketIn, player);
        }

        public void SendMoveToAll(Bogu bogu)
        {
            GSPacketIn gSPacketIn = new GSPacketIn((byte)ePackageType.LITTLEGAME_COMMAND);
            gSPacketIn.WriteByte((byte)eLittleGamePackageInType.MOVE);
            gSPacketIn.WriteInt(bogu.ID);
            gSPacketIn.WriteInt(bogu.X);
            gSPacketIn.WriteInt(bogu.Y);
            SendToAll(gSPacketIn);
        }

        public void SendActionToAll(GamePlayer player, string action = null)
        {
            GSPacketIn gSPacketIn = new GSPacketIn((byte)ePackageType.LITTLEGAME_COMMAND);
            gSPacketIn.WriteByte((byte)eLittleGamePackageInType.DoAction);
            switch (action ?? player.LittleGameInfo.Actions.Peek())
            {
                case "livingInhale":
                    gSPacketIn.WriteString("livingInhale");
                    gSPacketIn.WriteInt(player.LittleGameInfo.ID);
                    gSPacketIn.WriteString(!player.LittleGameInfo.IsBack ? "inhaleSmall" : "backInhaleSmall");
                    gSPacketIn.WriteString(player.LittleGameInfo.Direction);
                    gSPacketIn.WriteInt(0); // live
                    gSPacketIn.WriteInt(player.LittleGameInfo.X);
                    gSPacketIn.WriteInt(player.LittleGameInfo.Y);
                    break;
                case "livingUnInhale":
                    gSPacketIn.WriteString("livingUnInhale");
                    gSPacketIn.WriteInt(player.LittleGameInfo.ID);
                    gSPacketIn.WriteString("stand");
                    gSPacketIn.WriteString(player.LittleGameInfo.Direction);
                    break;
            }
            SendToAll(gSPacketIn);
        }

        public void SendActionToAll(Bogu bogu, string action)
        {
            GSPacketIn gSPacketIn = new GSPacketIn((byte)ePackageType.LITTLEGAME_COMMAND);
            gSPacketIn.WriteByte((byte)eLittleGamePackageInType.DoAction);
            switch (action)
            {
                case "livingInhale":
                    gSPacketIn.WriteString("livingInhale");
                    gSPacketIn.WriteInt(bogu.ID);
                    gSPacketIn.WriteString(!bogu.IsBack ? "inhaled" : bogu.Size == 0 ? "backInhaled" : "inhaled");
                    gSPacketIn.WriteString(bogu.Direction);
                    gSPacketIn.WriteInt(0); // live
                    gSPacketIn.WriteInt(bogu.X);
                    gSPacketIn.WriteInt(bogu.Y);
                    break;
                case "livingUnInhale":
                    gSPacketIn.WriteString("livingUnInhale");
                    gSPacketIn.WriteInt(bogu.ID);
                    gSPacketIn.WriteString(bogu.Сaught ? (!bogu.IsBack ? "die" : bogu.Size == 0 ? "backDie" : "die") : "stand");
                    gSPacketIn.WriteString(bogu.Direction);
                    break;
                case "livingDie":
                    gSPacketIn.WriteString("livingDie");
                    gSPacketIn.WriteInt(bogu.ID);
                    break;
            }
            SendToAll(gSPacketIn);
        }

        public void SendAddPlayerToAll(GamePlayer player)
        {
            GSPacketIn gSPacketIn = new GSPacketIn((byte)ePackageType.LITTLEGAME_COMMAND, player.PlayerCharacter.ID);
            gSPacketIn.WriteByte((byte)eLittleGamePackageInType.ADD_SPRITE);
            gSPacketIn.WriteInt(player.LittleGameInfo.ID);
            gSPacketIn.WriteInt(player.LittleGameInfo.X);
            gSPacketIn.WriteInt(player.LittleGameInfo.Y);
            gSPacketIn.WriteInt(1);
            gSPacketIn.WriteInt(player.PlayerCharacter.ID);
            gSPacketIn.WriteInt(player.PlayerCharacter.Grade);
            gSPacketIn.WriteInt(player.PlayerCharacter.Repute);
            gSPacketIn.WriteString(player.PlayerCharacter.NickName);
            gSPacketIn.WriteByte(player.PlayerCharacter.typeVIP);
            gSPacketIn.WriteInt(player.PlayerCharacter.VIPLevel);
            gSPacketIn.WriteBoolean(player.PlayerCharacter.Sex);
            gSPacketIn.WriteString(player.PlayerCharacter.Style);
            gSPacketIn.WriteString(player.PlayerCharacter.Colors);
            gSPacketIn.WriteString(player.PlayerCharacter.Skin);
            gSPacketIn.WriteInt(player.PlayerCharacter.Hide);
            gSPacketIn.WriteInt(player.PlayerCharacter.FightPower);
            gSPacketIn.WriteInt(player.PlayerCharacter.Win);
            gSPacketIn.WriteInt(player.PlayerCharacter.Total);
            gSPacketIn.WriteBoolean(false);
            gSPacketIn.WriteInt(0);
            SendToAll(gSPacketIn, player);
        }
        public void SendAddBoguToAll(Bogu bogu)
        {
            GSPacketIn gSPacketIn = new GSPacketIn((byte)ePackageType.LITTLEGAME_COMMAND);
            gSPacketIn.WriteByte((byte)eLittleGamePackageInType.ADD_SPRITE);
            gSPacketIn.WriteInt(bogu.ID);
            gSPacketIn.WriteInt(bogu.X); // x
            gSPacketIn.WriteInt(bogu.Y); // y
            gSPacketIn.WriteInt(2); //  1 - player 2 - living(bogu)
            gSPacketIn.WriteString(bogu.Model);
            gSPacketIn.WriteString(bogu.Model);
            gSPacketIn.WriteBoolean(false);
            //gSPacketIn.WriteInt(pl.lX * 7);
            //gSPacketIn.WriteInt(pl.lY * 7);
            gSPacketIn.WriteInt(0);// lenght
            SendToAll(gSPacketIn);
        }

        public void SendLivingPropertyUpdateToAll(Bogu bogu, string property, string value)
        {
            GSPacketIn gSPacketIn = new GSPacketIn((byte)ePackageType.LITTLEGAME_COMMAND);
            gSPacketIn.WriteByte(90);
            gSPacketIn.WriteInt(bogu.ID);
            gSPacketIn.WriteInt(1);
            gSPacketIn.WriteString(property);
            switch (property)
            {
                case "lock":
                    gSPacketIn.WriteInt(2);
                    gSPacketIn.WriteBoolean(Boolean.Parse(value));
                    break;
            }
            SendToAll(gSPacketIn);
        }

        public void SendLivingPropertyUpdateToAll(GamePlayer player, string property, string value)
        {
            GSPacketIn gSPacketIn = new GSPacketIn((byte)ePackageType.LITTLEGAME_COMMAND);
            gSPacketIn.WriteByte((byte)eLittleGamePackageInType.UPDATELIVINGSPROPERTY);
            gSPacketIn.WriteInt(player.LittleGameInfo.ID);
            gSPacketIn.WriteInt(1); // count
            gSPacketIn.WriteString(property);
            switch (property)
            {
                case "lock":
                    gSPacketIn.WriteInt(2); // type
                    gSPacketIn.WriteBoolean(Boolean.Parse(value)); //_loc7_ = param2.readBoolean();
                    break;
            }
            SendToAll(gSPacketIn);
        }

        public void SendRemoveLivingToAll(GamePlayer player)
        {
            GSPacketIn gSPacketIn = new GSPacketIn((byte)ePackageType.LITTLEGAME_COMMAND, player.PlayerCharacter.ID);
            gSPacketIn.WriteByte((byte)eLittleGamePackageInType.REMOVE_SPRITE);
            gSPacketIn.WriteInt(player.LittleGameInfo.ID);
            SendToAll(gSPacketIn);
        }

        public void SendRemoveLivingToAll(Bogu bogu)
        {
            GSPacketIn gSPacketIn = new GSPacketIn((byte)ePackageType.LITTLEGAME_COMMAND);
            gSPacketIn.WriteByte((byte)eLittleGamePackageInType.REMOVE_SPRITE);
            gSPacketIn.WriteInt(bogu.ID);
            SendToAll(gSPacketIn);
        }

        public void SendToAll(GSPacketIn gSPacketIn, GamePlayer player = null)
        {
            lock (LittleGameWorldMgr.ScenariObjects)
            {
                foreach (object o in LittleGameWorldMgr.ScenariObjects.Values)
                {
                    if (o is GamePlayer pl)
                    {
                        if (player == null || player != null && pl.LittleGameInfo.ID != player.LittleGameInfo.ID)
                        {
                            pl.SendTCP(gSPacketIn);
                        }
                    }
                }
            }
        }

        public void SendRemoveObject(GamePlayer player)
        {
            GSPacketIn gsPacketIn = new GSPacketIn((byte)ePackageType.LITTLEGAME_COMMAND);
            gsPacketIn.WriteByte((byte)eLittleGamePackageInType.REMOVE_OBJECT);
            gsPacketIn.WriteInt(99);
            player.Out.SendTCP(gsPacketIn);
        }
        public void SendAddObject(GamePlayer player, Bogu bogu, string obj)
        {
            GSPacketIn gsPacketIn = new GSPacketIn((byte)ePackageType.LITTLEGAME_COMMAND);
            gsPacketIn.WriteByte((byte)eLittleGamePackageInType.ADD_OBJECT);
            gsPacketIn.WriteString(obj);
            switch (obj)
            {
                case "bigbogu":
                case "normalbogu":
                    gsPacketIn.WriteInt(1);
                    gsPacketIn.WriteInt(player.LittleGameInfo.ID);
                    gsPacketIn.WriteInt(bogu.ID);
                    gsPacketIn.WriteInt(bogu.HP);
                    gsPacketIn.WriteInt(bogu.Score);
                    gsPacketIn.WriteInt(bogu.Score / bogu.HP);
                    gsPacketIn.WriteInt(10);
                    gsPacketIn.WriteInt(bogu.Catchers.Count);
                    foreach (GamePlayer pl in bogu.Catchers)
                    {
                        gsPacketIn.WriteInt(pl.LittleGameInfo.ID);
                    }
                    break;
                case "bogugiveup":
                    gsPacketIn.WriteInt(99);
                    gsPacketIn.WriteInt(bogu.ID);
                    gsPacketIn.WriteInt(bogu.MaxCatchers);
                    break;
            }
            player.Out.SendTCP(gsPacketIn);
        }
    }
}
