using Bussiness;
using Game.Base.Packets;
using Game.Server.ConsortiaTask;
using Game.Server.GameObjects;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;

namespace Game.Server.Consortia.Handle
{
    [global::Consortia(22)]
    public class CConsortiaTask : IConsortiaCommandHadler
    {
        public int CommandHandler(GamePlayer Player, GSPacketIn packet)
        {
            if (Player.PlayerCharacter.ConsortiaID == 0)
                return 0;
            int type = packet.ReadInt();
            switch (type)
            {
                case 0://Release_Task
                case 1://Reset_Task
                    using (ConsortiaBussiness consortiaBussiness = new ConsortiaBussiness())
                    {
                        ConsortiaInfo consortiaSingle = consortiaBussiness.GetConsortiaSingle(Player.PlayerCharacter.ConsortiaID);
                        if (consortiaSingle != null && consortiaSingle.ChairmanID == Player.PlayerCharacter.ID)
                        {
                            if (consortiaSingle.Level < 3)
                            {
                                Player.SendMessage("Cấp Guild không đủ để mở SMG!");
                                return 0;
                            }
                            bool canRefresh = Player.PlayerCharacter.DateFinishTask.Date == DateTime.Now.Date;
                            if (type == 1 && canRefresh)
                            {
                                Player.SendMessage("Thao tác thất bại. hoặc sứ mệnh Guild của bạn đã hoàn thành.");
                                return 0;
                            }
                            int riches = GameProperties.MissionRichesArr()[consortiaSingle.Level - 1];
                            if (type == 1)//Reset SMG
                                riches = 500;

                            bool result = consortiaSingle.DateOpenTask.Date != DateTime.Now.Date;

                            if (type == 1)
                                result = true;

                            if (result)
                            {
                                if (ConsortiaTaskMgr.GetSingleConsortiaTask(Player.PlayerCharacter.ConsortiaID) == null ? consortiaBussiness.ConsortiaRichRemove(consortiaSingle.ConsortiaID, ref riches) : Player.RemoveMoney(riches, isConsume: false) > 0)
                                {
                                    int consortiaId = consortiaSingle.ConsortiaID;
                                    consortiaBussiness.ConsortiaTaskUpdateDate(consortiaId, DateTime.Now);
                                    if (ConsortiaTaskMgr.AddConsortiaTask(consortiaSingle.ConsortiaID, consortiaSingle.Level, type == 1))
                                    {
                                        BaseConsortiaTask singleConsortiaTask = ConsortiaTaskMgr.GetSingleConsortiaTask(Player.PlayerCharacter.ConsortiaID);
                                        GSPacketIn pkg = new GSPacketIn((short)129);
                                        pkg.WriteByte((byte)22);
                                        pkg.WriteByte((byte)0);
                                        pkg.WriteInt(singleConsortiaTask.ConditionList.Count);
                                        foreach (KeyValuePair<int, ConsortiaTaskInfo> condition in singleConsortiaTask.ConditionList)
                                        {
                                            pkg.WriteInt(condition.Key);
                                            pkg.WriteString(condition.Value.CondictionTitle);
                                        }
                                        Player.SendTCP(pkg);
                                        break;
                                    }
                                    Player.SendMessage(LanguageMgr.GetTranslation("GameServer.ConsortiaTask.msg6"));
                                    break;
                                }
                                Player.SendMessage(LanguageMgr.GetTranslation("GameServer.ConsortiaTask.msg8"));
                                break;
                            }
                            Player.SendMessage(LanguageMgr.GetTranslation("GameServer.ConsortiaTask.msg5"));
                            break;
                        }
                        Player.SendMessage(LanguageMgr.GetTranslation("GameServer.ConsortiaTask.msg7"));
                        break;
                    }
                case 2://Submit_Task
                    using (ConsortiaBussiness consortiaBussiness = new ConsortiaBussiness())
                    {
                        ConsortiaInfo consortiaSingle =
                            consortiaBussiness.GetConsortiaSingle(Player.PlayerCharacter.ConsortiaID);
                        if (consortiaSingle != null && consortiaSingle.ChairmanID == Player.PlayerCharacter.ID)
                        {
                            if (consortiaSingle.Level < 3)
                            {
                                Player.SendMessage("Cấp Guild không đủ để mở SMG!");
                                return 0;
                            }
                        }
                        else
                        {
                            Player.SendMessage(LanguageMgr.GetTranslation("GameServer.ConsortiaTask.msg7"));
                            return 0;
                        }
                        BaseConsortiaTask singleConsortiaTask1 = ConsortiaTaskMgr.GetSingleConsortiaTask(Player.PlayerCharacter.ConsortiaID);
                        bool val = false;
                        if (singleConsortiaTask1 != null && !singleConsortiaTask1.Info.IsActive && ConsortiaTaskMgr.ActiveTask(Player.PlayerCharacter.ConsortiaID))
                        {
                            val = true;
                            Player.Out.SendConsortiaTaskInfo(singleConsortiaTask1);
                        }
                        GSPacketIn pkg1 = new GSPacketIn((short)129);
                        pkg1.WriteByte((byte)22);
                        pkg1.WriteByte((byte)2);
                        pkg1.WriteBoolean(val);
                        Player.SendTCP(pkg1);
                        break;
                    }
                case 3://Get_Task
                    BaseConsortiaTask singleConsortiaTask2 = ConsortiaTaskMgr.GetSingleConsortiaTask(Player.PlayerCharacter.ConsortiaID);
                    Player.Out.SendConsortiaTaskInfo(singleConsortiaTask2);
                    break;
            }
            return 0;
        }
    }
}
