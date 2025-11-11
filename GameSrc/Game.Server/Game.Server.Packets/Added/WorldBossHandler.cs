using Bussiness;
using Game.Base.Packets;
using Game.Server.Buffer;
using Game.Server.Rooms;
using System;
namespace Game.Server.Packets.Client
{
    [PacketHandler(102, "场景用户离开")]
    public class WorldBossHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            byte b = packet.ReadByte();
            if (!RoomMgr.WorldBossRoom.WorldbossOpen)
            {
                client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Boss thế giới đã kết thúc", new object[0]));
                return 0;
            }
            GSPacketIn gSPacketIn = new GSPacketIn(102, client.Player.PlayerCharacter.ID);
            {
                switch(b)
                {
                    case 32:
                        gSPacketIn.WriteByte(2);
                        gSPacketIn.WriteBoolean(true);
                        gSPacketIn.WriteBoolean(false);
                        gSPacketIn.WriteInt(0);
                        gSPacketIn.WriteInt(0);
                        client.Out.SendTCP(gSPacketIn);
                        return 0;
                    case 33:
                        RoomMgr.WorldBossRoom.RemovePlayer(client.Player);
                        client.Player.IsInWorldBossRoom = false;
                        break;
                    case 34:
                        {
                            int x = packet.ReadInt();
                            int y = packet.ReadInt();
                            client.Player.X = x;
                            client.Player.Y = y;
                            if (client.Player.CurrentRoom != null)
                            {
                                client.Player.CurrentRoom.RemovePlayerUnsafe(client.Player);
                            }
                            BaseWorldBossRoom worldBossRoom = RoomMgr.WorldBossRoom;
                            if (client.Player.IsInWorldBossRoom)
                            {
                                gSPacketIn.WriteByte(4);
                                gSPacketIn.WriteInt(client.Player.PlayerId);
                                worldBossRoom.SendToALL(gSPacketIn);
                                worldBossRoom.RemovePlayer(client.Player);
                                client.Player.IsInWorldBossRoom = false;
                            }
                            else
                            {
                                if (worldBossRoom.AddPlayer(client.Player))
                                {
                                    worldBossRoom.ViewOtherPlayerRoom(client.Player);
                                }
                            }
                            break;
                        }
                    case 35:
                        {
                            int num = packet.ReadInt();
                            int num2 = packet.ReadInt();
                            string str = packet.ReadString();
                            gSPacketIn.WriteByte(6);
                            gSPacketIn.WriteInt(client.Player.PlayerId);
                            gSPacketIn.WriteInt(num);
                            gSPacketIn.WriteInt(num2);
                            gSPacketIn.WriteString(str);
                            client.Player.SendTCP(gSPacketIn);
                            RoomMgr.WorldBossRoom.SendToALL(gSPacketIn, client.Player);
                            client.Player.X = num;
                            client.Player.Y = num2;
                            break;
                        }
                    case 36:
                        {
                            byte b2 = packet.ReadByte();
                            if (b2 != 3 || client.Player.States != 3)
                            {
                                gSPacketIn.WriteByte(7);
                                gSPacketIn.WriteInt(client.Player.PlayerId);
                                gSPacketIn.WriteByte(b2);
                                gSPacketIn.WriteInt(client.Player.X);
                                gSPacketIn.WriteInt(client.Player.Y);
                                RoomMgr.WorldBossRoom.SendToALL(gSPacketIn);
                                if (b2 == 3 && client.Player.CurrentRoom.Game != null)
                                {
                                    client.Player.CurrentRoom.RemovePlayerUnsafe(client.Player);
                                }
                                string nickName = client.Player.PlayerCharacter.NickName;
                                RoomMgr.WorldBossRoom.SendPrivateInfo(nickName);
                            }
                            client.Player.States = b2;
                            break;
                        }
                    case 37:
                        {
                            int num3 = packet.ReadInt();
                            packet.ReadBoolean();
                            int value = RoomMgr.WorldBossRoom.reviveMoney;
                            //if (num3 == 2)
                            //{
                            //    value = RoomMgr.WorldBossRoom.reFightMoney;
                            //}
                            if (client.Player.MoneyDirect(value, false))
                            {
                                gSPacketIn.WriteByte(11);
                                gSPacketIn.WriteInt(client.Player.PlayerId);
                                RoomMgr.WorldBossRoom.SendToALL(gSPacketIn);
                            }
                            break;
                        }
                    //case 38:
                    //    {
                    //        int addInjureBuffMoney = RoomMgr.WorldBossRoom.addInjureBuffMoney;
                    //        int addInjureValue = RoomMgr.WorldBossRoom.addInjureValue;
                    //        if (client.Player.MoneyDirect(addInjureBuffMoney, false))
                    //        {
                    //            client.Player.RemoveMoney(addInjureBuffMoney);
                    //            AbstractBuffer abstractBuffer = BufferList.CreatePayBuffer(403, addInjureValue, 1);
                    //            if (abstractBuffer != null)
                    //            {
                    //                abstractBuffer.Start(client.Player);
                    //            }
                    //        }
                    //        break;
                    //    }
                    default:
                        Console.WriteLine("WorldBossPackageType." + (WorldBossPackageType)b);
                        break;
                }
                return 0;
            }
        }
    }
}