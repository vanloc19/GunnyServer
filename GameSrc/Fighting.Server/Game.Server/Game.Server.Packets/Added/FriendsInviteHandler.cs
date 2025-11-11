using Game.Base.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Server.Packets;
using Bussiness;
using SqlDataProvider.Data;
using Game.Server.Managers;
using Bussiness.Managers;

namespace Game.Server.Packets.Client
{
    [PacketHandler(107, "FriendsInviteHandler")]
    public class FriendsInviteHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            int Type = packet.ReadInt();
            switch (Type)
            {
                case 1:
                    string CheckCode = packet.ReadString();
                    if (client.Player.PlayerCharacter.MyInviteCode == CheckCode)
                    {
                        client.Player.SendMessage("Không thể sử dụng code của chính mình vui lòng lấy code từ bạn bè!");
                    }
                    else
                    {
                        using (PlayerBussiness bussiness = new PlayerBussiness())
                        {
                            PlayerInfo checkPlayer = bussiness.GetUserSingleByInviteCode(CheckCode);
                            if (checkPlayer != null)
                            {
                                if (checkPlayer.MyInviteCode != client.Player.PlayerCharacter.MyInviteCode)
                                {
                                    if (checkPlayer.MyCodeStatus != true)
                                    {
                                        if(checkPlayer.State == 1)
                                        {
                                            client.Player.SendMessage("Người chơi đang online không thể nhập Code này");
                                            return 0;
                                        }    
                                        if (checkPlayer.MyInvitedCount >= 11)
                                        {
                                            client.Player.SendMessage("Code này đã được sử dụng hơn 10 lần nên không thể sử dụng lại nữa.");
                                        }
                                        else
                                        {
                                            client.Player.PlayerCharacter.MyRewardStatus = true;
                                            Console.WriteLine($"checkPlayer Name: {checkPlayer.UserName}, Code: {checkPlayer.CheckCode}, Count:{checkPlayer.MyInvitedCount}");
                                            checkPlayer.MyCodeStatus = true;
                                            checkPlayer.MyInvitedCount += 1;
                                            var rewards = AwardMgr.FindDailyAward(200);
                                            var items = new List<ItemInfo>();
                                            if (rewards.Count == 0)
                                            {
                                                client.Player.SendMessage("Không có vật phẩm!");
                                                return 0;
                                            }
                                            else
                                            {
                                                foreach (var reward in rewards)
                                                {
                                                    var item = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(reward.TemplateID), 1, 102);
                                                    if (item == null)
                                                        continue;
                                                    item.IsBinds = reward.IsBinds;
                                                    item.Count = reward.Count;
                                                    item.ValidDate = reward.ValidDate;
                                                    item.StrengthenLevel = 0;
                                                    item.AttackCompose = 0;
                                                    item.DefendCompose = 0;
                                                    item.AgilityCompose = 0;
                                                    item.LuckCompose = 0;
                                                    items.Add(item);
                                                }
                                                if (!client.Player.SendItemsToMail(items, "phần thưởng từ Mời Bạn Bè", "Mời Bạn Bè", eMailType.Manage)) return 0;
                                                client.Player.Out.SendMailResponse(client.Player.PlayerCharacter.ID, eMailRespose.Receiver);
                                                client.Player.SendMessage("Nhận thưởng thành công vui lòng kiểm tra hộp thư.");
                                            }
                                            client.Player.Out.SendInviteFriends(client.Player.PlayerCharacter, (int)InviteFriendPackageType.INVITE_FRIEND_FLUSHFRIENDNUM);
                                            client.Player.Out.SendInviteFriends(checkPlayer, (int)InviteFriendPackageType.INVITE_FRIEND_GETREWARD);
                                            bussiness.UpdatePlayer(checkPlayer);
                                            bussiness.UpdatePlayer(client.Player.PlayerCharacter);
                                        }
                                    }
                                    else
                                    {
                                        client.Player.SendMessage("Code đã được sử dụng");
                                    }
                                }
                            }
                            else
                            {
                                client.Player.SendMessage("Code không tồn tại trên hệ thống");
                            }
                        }
                    }
                    break;
                case 2:
                    packet.ReadInt();
                    client.Player.Out.SendInviteFriends(client.Player.PlayerCharacter, (int)InviteFriendPackageType.INVITE_FRIEND_OPENVIEW);
                    break;
                case 4:
                    int StatusID = packet.ReadInt();
                    switch (StatusID)
                    {
                        case 1:
                            if (client.Player.PlayerCharacter.AwardColumnOne == 0)
                            {
                                using (PlayerBussiness bussiness = new PlayerBussiness())
                                {
                                    client.Player.PlayerCharacter.AwardColumnOne = 1;
                                    var rewards = AwardMgr.FindDailyAward(201);
                                    var items = new List<ItemInfo>();
                                    if (rewards.Count == 0)
                                    {
                                        client.Player.SendMessage("Không có vật phẩm!");
                                        return 0;
                                    }
                                    else
                                    {
                                        foreach (var reward in rewards)
                                        {
                                            var item = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(reward.TemplateID), 1, 102);
                                            if (item == null)
                                                continue;
                                            item.IsBinds = reward.IsBinds;
                                            item.Count = reward.Count;
                                            item.ValidDate = reward.ValidDate;
                                            item.StrengthenLevel = 0;
                                            item.AttackCompose = 0;
                                            item.DefendCompose = 0;
                                            item.AgilityCompose = 0;
                                            item.LuckCompose = 0;
                                            items.Add(item);
                                        }

                                        if (!client.Player.SendItemsToMail(items, "phần thưởng từ Mời Bạn Bè", "Mời Bạn Bè", eMailType.Manage)) return 0;
                                        client.Player.Out.SendMailResponse(client.Player.PlayerCharacter.ID, eMailRespose.Receiver);
                                        client.Player.SendMessage("Nhận thưởng thành công vui lòng kiểm tra hộp thư.");
                                    }
                                    client.Player.Out.SendInviteFriends(client.Player.PlayerCharacter, (int)InviteFriendPackageType.INVITE_FRIEND_GETREWARD);
                                    bussiness.UpdatePlayer(client.Player.PlayerCharacter);
                                }

                            }
                            else
                            {
                                client.Player.SendMessage("Bạn đã nhận phần thưởng này rồi.");
                            }
                            break;
                        case 3:
                            if (client.Player.PlayerCharacter.AwardColumnTwo == 0)
                            {
                                using (PlayerBussiness bussiness = new PlayerBussiness())
                                {
                                    client.Player.PlayerCharacter.AwardColumnTwo = 1;
                                    var rewards = AwardMgr.FindDailyAward(203);
                                    var items = new List<ItemInfo>();
                                    if (rewards.Count == 0)
                                    {
                                        client.Player.SendMessage("Không có vật phẩm!");
                                        return 0;
                                    }
                                    else
                                    {
                                        foreach (var reward in rewards)
                                        {
                                            var item = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(reward.TemplateID), 1, 102);
                                            if (item == null)
                                                continue;
                                            item.IsBinds = reward.IsBinds;
                                            item.Count = reward.Count;
                                            item.ValidDate = reward.ValidDate;
                                            item.StrengthenLevel = 0;
                                            item.AttackCompose = 0;
                                            item.DefendCompose = 0;
                                            item.AgilityCompose = 0;
                                            item.LuckCompose = 0;
                                            items.Add(item);
                                        }

                                        if (!client.Player.SendItemsToMail(items, "phần thưởng từ Mời Bạn Bè", "Mời Bạn Bè", eMailType.Manage)) return 0;
                                        client.Player.Out.SendMailResponse(client.Player.PlayerCharacter.ID, eMailRespose.Receiver);
                                        client.Player.SendMessage("Nhận thưởng thành công vui lòng kiểm tra hộp thư.");
                                    }
                                    client.Player.Out.SendInviteFriends(client.Player.PlayerCharacter, (int)InviteFriendPackageType.INVITE_FRIEND_GETREWARD);
                                    bussiness.UpdatePlayer(client.Player.PlayerCharacter);
                                }

                            }
                            else
                            {
                                client.Player.SendMessage("Bạn đã nhận phần thưởng này rồi.");
                            }
                            break;
                        case 5:
                            if (client.Player.PlayerCharacter.AwardColumnThree == 0)
                            {
                                using (PlayerBussiness bussiness = new PlayerBussiness())
                                {
                                    client.Player.PlayerCharacter.AwardColumnThree = 1;
                                    var rewards = AwardMgr.FindDailyAward(205);
                                    var items = new List<ItemInfo>();
                                    if (rewards.Count == 0)
                                    {
                                        client.Player.SendMessage("Không có vật phẩm!");
                                        return 0;
                                    }
                                    else
                                    {
                                        foreach (var reward in rewards)
                                        {
                                            var item = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(reward.TemplateID), 1, 102);
                                            if (item == null)
                                                continue;
                                            item.IsBinds = reward.IsBinds;
                                            item.Count = reward.Count;
                                            item.ValidDate = reward.ValidDate;
                                            item.StrengthenLevel = 0;
                                            item.AttackCompose = 0;
                                            item.DefendCompose = 0;
                                            item.AgilityCompose = 0;
                                            item.LuckCompose = 0;
                                            items.Add(item);
                                        }

                                        if (!client.Player.SendItemsToMail(items, "phần thưởng từ Mời Bạn Bè", "Mời Bạn Bè", eMailType.Manage)) return 0;
                                        client.Player.Out.SendMailResponse(client.Player.PlayerCharacter.ID, eMailRespose.Receiver);
                                        client.Player.SendMessage("Nhận thưởng thành công vui lòng kiểm tra hộp thư.");
                                    }
                                    client.Player.Out.SendInviteFriends(client.Player.PlayerCharacter, (int)InviteFriendPackageType.INVITE_FRIEND_GETREWARD);
                                    bussiness.UpdatePlayer(client.Player.PlayerCharacter);
                                }

                            }
                            else
                            {
                                client.Player.SendMessage("Bạn đã nhận phần thưởng này rồi.");
                            }
                            break;
                        case 10:
                            if (client.Player.PlayerCharacter.AwardColumnFour == 0)
                            {
                                using (PlayerBussiness bussiness = new PlayerBussiness())
                                {
                                    client.Player.PlayerCharacter.AwardColumnFour = 1;
                                    var rewards = AwardMgr.FindDailyAward(210);
                                    var items = new List<ItemInfo>();
                                    if (rewards.Count == 0)
                                    {
                                        client.Player.SendMessage("Không có vật phẩm");
                                        return 0;
                                    }
                                    else
                                    {
                                        foreach (var reward in rewards)
                                        {
                                            var item = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(reward.TemplateID), 1, 102);
                                            if (item == null)
                                                continue;
                                            item.IsBinds = reward.IsBinds;
                                            item.Count = reward.Count;
                                            item.ValidDate = reward.ValidDate;
                                            item.StrengthenLevel = 0;
                                            item.AttackCompose = 0;
                                            item.DefendCompose = 0;
                                            item.AgilityCompose = 0;
                                            item.LuckCompose = 0;
                                            items.Add(item);
                                        }

                                        if (!client.Player.SendItemsToMail(items, "phần thưởng từ Mời Bạn Bè", "Mời Bạn Bè", eMailType.Manage)) return 0;
                                        client.Player.Out.SendMailResponse(client.Player.PlayerCharacter.ID, eMailRespose.Receiver);
                                        client.Player.SendMessage("Nhận thưởng thành công vui lòng kiểm tra hộp thư");
                                    }
                                    client.Player.Out.SendInviteFriends(client.Player.PlayerCharacter, (int)InviteFriendPackageType.INVITE_FRIEND_GETREWARD);
                                    bussiness.UpdatePlayer(client.Player.PlayerCharacter);
                                }
                            }
                            else
                            {
                                client.Player.SendMessage("Bạn đã nhận phần thưởng này rồi.");
                            }
                            break;
                    }
                    break;
            }
            return 0;
        }
    }
}