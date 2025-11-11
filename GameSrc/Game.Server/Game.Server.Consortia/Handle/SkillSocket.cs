using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.Buffer;
using Game.Server.GameObjects;
using Game.Server.Managers;
using Game.Server.Packets;
using SqlDataProvider.Data;
using System;

namespace Game.Server.Consortia.Handle
{
    [global::Consortia(26)]
    public class SkillSocket : IConsortiaCommandHadler
    {
        public int CommandHandler(GamePlayer Player, GSPacketIn packet)
        {
            if (Player.PlayerCharacter.ConsortiaID == 0)
                return 0;
            if (DateTime.Compare(Player.LastRequestTime.AddSeconds(2.0), DateTime.Now) > 0)
            {
                Player.SendMessage("Chậm chậm thôi.");
                return 0;
            }

            packet.ReadBoolean();
            int id = packet.ReadInt(); //_loc_5.writeInt(param2);
            int count = packet.ReadInt(); //_loc_5.writeInt(param3);
            int isMetal = packet.ReadInt(); //_loc_5.writeInt(param4);_isMetal ? (2) : (1);
            if (count < 0)
                count = 1;

            ConsortiaBuffTempInfo buffInfo = ConsortiaExtraMgr.FindConsortiaBuffInfo(id);
            {
                ConsortiaInfo myConsortia = ConsortiaMgr.FindConsortiaInfo(Player.PlayerCharacter.ConsortiaID);
                if (buffInfo == null || myConsortia == null)
                {
                    Player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Consortia.Msg1"));
                    return 0;
                }
                else
                {
                    bool isContinues = true;
                    int needMoneyOrRiches = count * buffInfo.riches;
                    int validate = 24 * 60 * count;

                    if (buffInfo.type == 2)
                    {
                        switch (isMetal)
                        {
                            case 1:
                                if (Player.PlayerCharacter.Riches < needMoneyOrRiches)
                                {
                                    isContinues = false;
                                }

                                break;
                            case 2:
                                needMoneyOrRiches = count * buffInfo.metal;
                                if (Player.GetMedalNum() < needMoneyOrRiches)
                                {
                                    isContinues = false;
                                }

                                break;
                        }
                    }
                    else if (myConsortia.Riches < needMoneyOrRiches)
                    {
                        isContinues = false;
                    }

                    if (isContinues)
                    {
                        if (buffInfo.level <= myConsortia.Level)
                        {
                            switch (buffInfo.group)
                            {
                                case 1:
                                    AbstractBuffer buffer1 = BufferList.CreatePayBuffer(
                                        (int)BuffType.ConsortionAddBloodGunCount, buffInfo.value, validate, id);
                                    if (buffer1 != null)
                                    {
                                        buffer1.Start(Player);
                                    }

                                    break;
                                case 3:
                                    AbstractBuffer buffer3 = BufferList.CreatePayBuffer(
                                        (int)BuffType.ConsortionAddCritical, buffInfo.value, validate, id);
                                    if (buffer3 != null)
                                    {
                                        buffer3.Start(Player);
                                    }

                                    break;
                                case 6:
                                    AbstractBuffer buffer6 = BufferList.CreatePayBuffer(
                                        (int)BuffType.ConsortionReduceEnergyUse, buffInfo.value, validate, id);
                                    if (buffer6 != null)
                                    {
                                        buffer6.Start(Player);
                                    }

                                    break;
                                case 11:
                                    AbstractBuffer buffer11 = BufferList.CreatePayBuffer(
                                        (int)BuffType.ConsortionAddSpellCount, buffInfo.value, validate, id);
                                    if (buffer11 != null)
                                    {
                                        buffer11.Start(Player);
                                    }

                                    break;
                                case 12:
                                    Player.Out.SendMessage(eMessageType.Normal,
                                        LanguageMgr.GetTranslation("Consortia.Msg2"));
                                    return 0;
                                case 8:
                                    Player.Out.SendMessage(eMessageType.Normal,
                                        LanguageMgr.GetTranslation("Consortia.Msg2"));
                                    return 0;
                                default:
                                    {
                                        using (PlayerBussiness db = new PlayerBussiness())
                                        {
                                            ConsortiaUserInfo[] allMembers =
                                                db.GetAllMemberByConsortia(Player.PlayerCharacter.ConsortiaID);
                                            AbstractBuffer buffer = null;
                                            switch (buffInfo.group)
                                            {
                                                case 2:
                                                    buffer = BufferList.CreatePayBuffer((int)BuffType.ConsortionAddDamage,
                                                        buffInfo.value, validate, id);
                                                    break;
                                                case 4:
                                                    buffer = BufferList.CreatePayBuffer(
                                                        (int)BuffType.ConsortionAddMaxBlood, buffInfo.value, validate, id);
                                                    break;
                                                case 5:
                                                    buffer = BufferList.CreatePayBuffer(
                                                        (int)BuffType.ConsortionAddProperty, buffInfo.value, validate, id);
                                                    break;
                                                case 7:
                                                    buffer = BufferList.CreatePayBuffer((int)BuffType.ConsortionAddEnergy,
                                                        buffInfo.value, validate, id);
                                                    break;
                                                case 9:
                                                    buffer = BufferList.CreatePayBuffer(
                                                        (int)BuffType.ConsortionAddOfferRate, buffInfo.value, validate,
                                                        id);
                                                    break;
                                                case 10:
                                                    buffer = BufferList.CreatePayBuffer(
                                                        (int)BuffType.ConsortionAddPercentGoldOrGP, buffInfo.value,
                                                        validate, id);
                                                    break;
                                            }

                                            foreach (ConsortiaUserInfo info in allMembers)
                                            {
                                                GamePlayer member = WorldMgr.GetPlayerById(info.UserID);
                                                if (member != null)
                                                {
                                                    if (buffer != null)
                                                    {
                                                        buffer.Start(member);
                                                    }
                                                    if (member != Player)
                                                    {
                                                        member.Out.SendMessage(eMessageType.Normal,LanguageMgr.GetTranslation("Consortia.Msg3"));
                                                    }
                                                }
                                                else
                                                {
                                                    if (buffer != null)
                                                    {
                                                        using (PlayerBussiness playerBussiness = new PlayerBussiness())
                                                        {
                                                            buffer.Info.UserID = info.UserID;
                                                            playerBussiness.SaveBuffer(buffer.Info);
                                                        };
                                                    }
                                                }
                                            }

                                            if (buffer != null)
                                            {
                                                ConsortiaBufferInfo conBuff = db.GetUserConsortiaBufferSingle(buffInfo.id);
                                                if (conBuff == null)
                                                {
                                                    conBuff = new ConsortiaBufferInfo();
                                                    conBuff.ConsortiaID = Player.PlayerCharacter.ConsortiaID;
                                                    conBuff.IsOpen = true;
                                                    conBuff.BufferID = buffInfo.id;
                                                    conBuff.Type = buffer.Info.Type;
                                                    conBuff.Value = buffer.Info.Value;
                                                    conBuff.ValidDate = buffer.Info.ValidDate;
                                                    conBuff.BeginDate = buffer.Info.BeginDate;
                                                }
                                                else
                                                {
                                                    conBuff.BufferID = buffInfo.id;
                                                    conBuff.Value = buffer.Info.Value;
                                                    conBuff.ValidDate += buffer.Info.ValidDate;
                                                }

                                                db.SaveConsortiaBuffer(conBuff);
                                            }
                                        }
                                    }
                                    break;
                            }

                            if (isMetal == 1)
                            {
                                if (buffInfo.type == 1)
                                {
                                    using (ConsortiaBussiness db = new ConsortiaBussiness())
                                    {
                                        int riches = needMoneyOrRiches;
                                        db.ConsortiaRichRemove(Player.PlayerCharacter.ConsortiaID, ref riches);
                                        GameServer.Instance.LoginServer.SendConsortiaRichesOffer(Player.PlayerCharacter.ConsortiaID, Player.PlayerCharacter.ID, Player.PlayerCharacter.NickName, myConsortia.Riches);
                                    }
                                }
                                else
                                {
                                    Player.RemoveRichesOffer(needMoneyOrRiches);
                                }
                            }
                            else
                            {
                                Player.RemoveMedal(needMoneyOrRiches);
                            }

                            Player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Consortia.Msg4"));
                        }
                        else
                        {
                            Player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Consortia.Msg5"));
                        }
                    }
                    else
                    {
                        Player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Consortia.Msg6"));
                    }
                }
            }
            return 0;
        }
    }
}