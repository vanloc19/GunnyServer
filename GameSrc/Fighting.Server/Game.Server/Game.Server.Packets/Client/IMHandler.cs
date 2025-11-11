//using Bussiness;
//using Game.Base.Packets;
//using Game.Server.GameObjects;
//using Game.Server.Managers;
//using SqlDataProvider.Data;

//namespace Game.Server.Packets.Client
//{
//	[PacketHandler(160, "添加好友")]
//	public class IMHandler : IPacketHandler
//	{
//		public int HandlePacket(GameClient client, GSPacketIn packet)
//		{
//			switch (packet.ReadByte())
//			{
//			case 51:
//			{
//				int num1 = packet.ReadInt();
//				string msg = packet.ReadString();
//				packet.ReadBoolean();
//				GamePlayer playerById = WorldMgr.GetPlayerById(num1);
//				if (playerById != null)
//				{
//					client.Player.Out.sendOneOnOneTalk(num1, isAutoReply: false, client.Player.PlayerCharacter.NickName, msg, client.Player.PlayerCharacter.ID);
//					playerById.Out.sendOneOnOneTalk(client.Player.PlayerCharacter.ID, isAutoReply: false, client.Player.PlayerCharacter.NickName, msg, num1);
//				}
//				else
//				{
//					client.Player.Out.SendMessage(eMessageType.GM_NOTICE, "O jogador não está online!");
//				}
//				break;
//			}
//			case 160:
//			{
//				string nickName = packet.ReadString();
//				int relation = packet.ReadInt();
//				if (relation < 0 || relation > 1)
//				{
//					return 1;
//				}
//				using (PlayerBussiness playerBussiness2 = new PlayerBussiness())
//				{
//					GamePlayer byPlayerNickName = WorldMgr.GetClientByPlayerNickName(nickName);
//					PlayerInfo user = ((byPlayerNickName == null) ? playerBussiness2.GetUserSingleByNickName(nickName) : byPlayerNickName.PlayerCharacter);
//					if (user != null)
//					{
//						if (client.Player.Friends.ContainsKey(user.ID) && client.Player.Friends[user.ID] == relation)
//						{
//							client.Out.SendMessage(eMessageType.GM_NOTICE, LanguageMgr.GetTranslation("FriendAddHandler.Falied"));
//						}
//						else
//						{
//							if (!playerBussiness2.AddFriends(new FriendInfo
//							{
//								FriendID = user.ID,
//								IsExist = true,
//								Remark = "",
//								UserID = client.Player.PlayerCharacter.ID,
//								Relation = relation
//							}))
//							{
//								break;
//							}
//							client.Player.FriendsAdd(user.ID, relation);
//							if (relation != 1 && user.State != 0)
//							{
//								GSPacketIn gsPacketIn = new GSPacketIn(160, client.Player.PlayerCharacter.ID);
//								gsPacketIn.WriteByte(166);
//								gsPacketIn.WriteInt(user.ID);
//								gsPacketIn.WriteString(client.Player.PlayerCharacter.NickName);
//								gsPacketIn.WriteBoolean(val: false);
//								if (byPlayerNickName != null)
//								{
//									byPlayerNickName.SendTCP(gsPacketIn);
//								}
//								else
//								{
//									GameServer.Instance.LoginServer.SendPacket(gsPacketIn);
//								}
//							}
//							client.Out.SendAddFriend(user, relation, state: true);
//							client.Out.SendMessage(eMessageType.GM_NOTICE, LanguageMgr.GetTranslation("FriendAddHandler.Success2"));
//							break;
//						}
//					}
//					else
//					{
//						client.Out.SendMessage(eMessageType.BIGBUGLE_NOTICE, LanguageMgr.GetTranslation("FriendAddHandler.Success") + nickName);
//					}
//				}
//				break;
//			}
//			case 161:
//			{
//				int num2 = packet.ReadInt();
//				using (PlayerBussiness playerBussiness = new PlayerBussiness())
//				{
//					if (playerBussiness.DeleteFriends(client.Player.PlayerCharacter.ID, num2))
//					{
//						client.Player.FriendsRemove(num2);
//						client.Out.SendFriendRemove(num2);
//					}
//				}
//				break;
//			}
//			case 165:
//			{
//				int num3 = packet.ReadInt();
//				GSPacketIn packet2 = new GSPacketIn(160, client.Player.PlayerCharacter.ID);
//				packet2.WriteByte(165);
//				packet2.WriteInt(num3);
//				packet2.WriteByte(client.Player.PlayerCharacter.typeVIP);
//				packet2.WriteInt(client.Player.PlayerCharacter.VIPLevel);
//				packet2.WriteBoolean(val: false);
//				GameServer.Instance.LoginServer.SendPacket(packet2);
//				WorldMgr.ChangePlayerState(client.Player.PlayerCharacter.ID, num3, client.Player.PlayerCharacter.ConsortiaID);
//				break;
//			}
//			}
//			return 1;
//		}
//	}
//}

using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using SqlDataProvider.Data;
using System;

namespace Game.Server.Packets.Client
{
    [PacketHandler((byte)ePackageType.IM_CMD, "添加好友")]
    public class IMHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            var im_cmd = packet.ReadByte();
            switch (im_cmd)
            {
                case (byte)IMPackageType.FRIEND_ADD:
                    {
                        string nickName = packet.ReadString(); //_loc_5.writeUTF(param1);
                        int relation = packet.ReadInt(); //_loc_5.writeInt(param2);
                        if (relation < 0 || relation > 1)
                        {
                            return 1;
                        }
                        using (PlayerBussiness db = new PlayerBussiness())
                        {
                            PlayerInfo user = null;
                            GamePlayer player = WorldMgr.GetClientByPlayerNickName(nickName);
                            if (player != null)
                            {
                                user = player.PlayerCharacter;
                            }
                            else
                            {
                                user = db.GetUserSingleByNickName(nickName);
                            }
                            if (user != null)
                            {
                                if (!client.Player.Friends.ContainsKey(user.ID) || client.Player.Friends[user.ID] != relation)
                                {
                                    FriendInfo friend = new FriendInfo();
                                    friend.FriendID = user.ID;
                                    friend.IsExist = true;
                                    friend.Remark = "";
                                    friend.UserID = client.Player.PlayerCharacter.ID;
                                    friend.Relation = relation;
                                    if (db.AddFriends(friend))
                                    {
                                        client.Player.FriendsAdd(user.ID, relation);
                                        if (relation != 1 && user.State != 0)
                                        {
                                            GSPacketIn response = new GSPacketIn((byte)ePackageType.IM_CMD, client.Player.PlayerCharacter.ID);
                                            response.WriteByte((byte)IMPackageType.FRIEND_RESPONSE);
                                            response.WriteInt(user.ID);
                                            response.WriteString(client.Player.PlayerCharacter.NickName);
                                            response.WriteBoolean(false);
                                            if (player != null)
                                            {
                                                player.SendTCP(response);
                                            }
                                            else
                                            {
                                                GameServer.Instance.LoginServer.SendPacket(response);
                                            }
                                        }
                                        client.Out.SendAddFriend(user, relation, true);
                                        client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("FriendAddHandler.Success2"));
                                    }
                                }
                                else
                                {
                                    client.Out.SendMessage(eMessageType.Normal,
                                        LanguageMgr.GetTranslation("FriendAddHandler.Falied"));
                                }
                            }
                            else
                            {
                                client.Out.SendMessage(eMessageType.ERROR,
                                    LanguageMgr.GetTranslation("FriendAddHandler.Success") + nickName);
                            }
                        }
                    }
                    break;
                case (byte)IMPackageType.FRIEND_REMOVE:
                    {
                        int id = packet.ReadInt();
                        using (PlayerBussiness db = new PlayerBussiness())
                        {
                            if (db.DeleteFriends(client.Player.PlayerCharacter.ID, id))
                            {
                                client.Player.FriendsRemove(id);
                                client.Out.SendFriendRemove(id);
                            }
                        }
                    }
                    break;
                case (byte)IMPackageType.FRIEND_STATE:
                    {
                        int state = packet.ReadInt();
                        GSPacketIn response = new GSPacketIn((byte)ePackageType.IM_CMD, client.Player.PlayerCharacter.ID);
                        response.WriteByte((byte)IMPackageType.FRIEND_STATE);
                        response.WriteInt(state);
                        response.WriteInt(client.Player.PlayerCharacter.typeVIP);
                        response.WriteInt(client.Player.PlayerCharacter.VIPLevel);
                        response.WriteBoolean(false);
                        GameServer.Instance.LoginServer.SendPacket(response);
                        WorldMgr.ChangePlayerState(client.Player.PlayerCharacter.ID, state, client.Player.PlayerCharacter.ConsortiaID);
                    }
                    break;
                case (byte)IMPackageType.ONE_ON_ONE_TALK:
                    {
                        int receiverID = packet.ReadInt();
                        string msg = packet.ReadString();
                        packet.ReadBoolean();
                        GamePlayer player = WorldMgr.GetPlayerById(receiverID);
                        if (player != null)
                        {
                            client.Player.Out.sendOneOnOneTalk(receiverID, false, client.Player.PlayerCharacter.NickName, msg, client.Player.PlayerCharacter.ID);
                            player.Out.sendOneOnOneTalk(client.Player.PlayerCharacter.ID, false, client.Player.PlayerCharacter.NickName, msg, receiverID);
                        }
                        else
                        {
                            client.Player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("FriendAddHandler.Ofline"));
                        }
                    }
                    break;
                default:
                    Console.WriteLine("IMPackageType." + (IMPackageType)im_cmd);
                    break;
            }
            return 1;
        }
    }
}
