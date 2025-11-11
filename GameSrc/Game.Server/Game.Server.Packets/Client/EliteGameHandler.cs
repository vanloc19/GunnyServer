using Game.Base.Packets;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using Game.Server.Managers.EliteGame;

namespace Game.Server.Packets.Client
{
    [PacketHandler((int)ePackageType.ELITEGAME, "添加拍卖")]
    public class EliteGameHandler : IPacketHandler
    {
        private static Dictionary<int, Dictionary<int, PlayerEliteGameInfo>> m_elitePlayersList = new Dictionary<int, Dictionary<int, PlayerEliteGameInfo>>();
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            byte cmd = packet.ReadByte();
            DateTime now = DateTime.Now;

            Console.WriteLine("//Elitegame CMD: " + (EliteGamePackageType)cmd);
            GSPacketIn pkg = null;
            switch (cmd)
            {
                case (int)EliteGamePackageType.ELITE_MATCH_TYPE:
                    {
                        pkg = new GSPacketIn((int)ePackageType.ELITEGAME);
                        pkg.WriteByte((byte)EliteGamePackageType.ELITE_MATCH_TYPE);
                        //5 | 10 // mo phong long than
                        if (client.Player.PlayerCharacter.Grade >= 30 && now.Hour >= 14 && now.Hour <= 17 && now.DayOfWeek == DayOfWeek.Sunday)
                        {
                            Console.WriteLine("EliteGameMode = {0}", EliteGameMgr.EliteGameMode); ;
                            pkg.WriteInt(EliteGameMgr.EliteGameMode);// 0: close | 1: _score_30_40 | 2: _champion_30_40 | 4: _score_41_50 | 8: _champion_41_50
                        }
                        else
                        {
                            pkg.WriteInt(0);
                        }
                        client.Out.SendTCP(pkg);
                    }
                    break;

                case (int)EliteGamePackageType.ELITE_MATCH_PLAYER_RANK:
                    {
                        pkg = new GSPacketIn((int)ePackageType.ELITEGAME);
                        pkg.WriteByte((byte)EliteGamePackageType.ELITE_MATCH_PLAYER_RANK);
                        pkg.WriteInt(client.Player.PlayerCharacter.EliteRank);
                        pkg.WriteInt(client.Player.PlayerCharacter.EliteScore);
                        client.Out.SendTCP(pkg);
                    }
                    break;

                case (int)EliteGamePackageType.ELITE_MATCH_RANK_DETAIL:
                    {
                        int groupType = packet.ReadInt();
                        List<PlayerEliteGameInfo> champs = EliteGameMgr.EliteGameChampionPlayersList(groupType).
                                                                        Where(a => a.Value.Rank > 0 && a.Value.Rank < 17).
                                                                        Select(a => a.Value).ToList();
                        if (champs == null)
                            champs = new List<PlayerEliteGameInfo>();

                        pkg = new GSPacketIn((int)ePackageType.ELITEGAME);
                        pkg.WriteByte((byte)EliteGamePackageType.ELITE_MATCH_RANK_DETAIL);
                        pkg.WriteInt(groupType);
                        pkg.WriteInt(champs.Count);

                        foreach (PlayerEliteGameInfo pinfo in champs)
                        {
                            pkg.WriteInt(pinfo.UserID);
                            pkg.WriteString(pinfo.NickName);
                            pkg.WriteInt(pinfo.Rank);
                            pkg.WriteInt(pinfo.Status);
                            pkg.WriteInt(pinfo.Winer);
                        }

                        client.Out.SendTCP(pkg);

                    }
                    break;

                case (int)EliteGamePackageType.ELITE_MATCH_RANK_START:
                    {
                        if (EliteGameMgr.EliteGameMode == (int)EliteGameModeType.SCORE_TIME && client.Player.PlayerCharacter.Grade >= 30)
                        {
                            client.Out.SendEliteGameStartRoom();
                        }
                    }
                    break;
            }

            return 0;
        }
    }
}