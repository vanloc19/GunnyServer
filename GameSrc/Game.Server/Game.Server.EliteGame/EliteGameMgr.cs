using Bussiness;
using Bussiness.Interface;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Rooms;
using log4net;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;

namespace Game.Server.Managers.EliteGame
{
    public class Matchup
    {
        public Matchup()
        {
            Seed16 = new int[2] { 1, 16 };

            SeedL8a = new int[4] { 1, 16, 8, 9 };
            SeedL8b = new int[4] { 4, 13, 5, 12 };

            SeedR8a = new int[4] { 6, 11, 3, 14 };
            SeedR8b = new int[4] { 7, 10, 2, 15 };

            SeedL4 = new int[8] { 1, 16, 8, 9, 4, 13, 5, 12 };
            SeedR4 = new int[8] { 6, 11, 3, 14, 7, 10, 2, 15 };
        }

        public int MatchupID { get; set; }

        public int[] Seed16 { get; set; }

        public int[] SeedL8a { get; set; }
        public int[] SeedL8b { get; set; }
        public int[] SeedR8a { get; set; }
        public int[] SeedR8b { get; set; }

        public int[] SeedL4 { get; set; }
        public int[] SeedR4 { get; set; }
    }

    internal static class DictionaryExtensions
    {
        public static Dictionary<T1, T2> Merge<T1, T2>(this Dictionary<T1, T2> first, Dictionary<T1, T2> second)
        {
            if (first == null) throw new ArgumentNullException("first");
            if (second == null) throw new ArgumentNullException("second");

            var merged = new Dictionary<T1, T2>();
            first.ToList().ForEach(kv => merged[kv.Key] = kv.Value);
            second.ToList().ForEach(kv => merged[kv.Key] = kv.Value);

            return merged;
        }
    }

    public class EliteGameMgr
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static ReaderWriterLock m_lock = new ReaderWriterLock();

        private static Dictionary<string, ServerEventInfo> m_serverEvent;

        public static EliteGameStatusType EliteStatus { get; set; } = EliteGameStatusType.CLOSE;

        public static int EliteGameMode { get; set; } = (int)EliteGameModeType.CLOSE;

        private static Dictionary<int, Matchup> m_orderNumbers;

        private static int m_orderKey;

        private static void Branch(int seed, int level, int limit, int round)
        {
            var levelSum = (int)Math.Pow(2, level) + 1;

            if (limit == level + 1)
            {
                m_orderKey++;
                var temp = new Matchup
                {
                    MatchupID = m_orderKey
                };
                temp.Seed16 = new int[2] { seed, levelSum - seed };

                m_orderNumbers.Add(m_orderKey, temp);
                return;
            }
            else if (seed % 2 == 1)
            {
                Branch(seed, level + 1, limit, round);
                Branch(levelSum - seed, level + 1, limit, round);
            }
            else
            {
                Branch(levelSum - seed, level + 1, limit, round);
                Branch(seed, level + 1, limit, round);
            }
        }

        private static void CreateMatchOrder()
        {
            m_orderNumbers = new Dictionary<int, Matchup>();
            m_orderKey = 0;

            var numberTeams = 16;
            var limit = (int)(Math.Log(numberTeams, 2) + 1);

            for (int round = 1; round < limit; round++)
            {
                Branch(1, 1, limit - round + 1, round);
            }
        }

        private static int GetOrderNumber(int rank, int gtype, int round = 2)
        {
            gtype = gtype == 1 ? 3000 : 4000;

            if (rank > 16)
                return 0;

            if (round == 5)
                return gtype + 15;
            foreach (var key in m_orderNumbers.Keys)
            {
                var temp = m_orderNumbers[key];
                switch (key)
                {
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                    case 5:
                    case 6:
                    case 7:
                    case 8://1/16
                        if (round == 2 && temp.Seed16.Contains(rank))
                            return gtype + temp.MatchupID;
                        break;

                    case 9://1/8
                        if (round == 3 && temp.SeedL8a.Contains(rank))
                            return gtype + temp.MatchupID;
                        break;
                    case 10://1/8
                        if (round == 3 && temp.SeedL8b.Contains(rank))
                            return gtype + temp.MatchupID;
                        break;
                    case 11://1/8
                        if (round == 3&& temp.SeedR8a.Contains(rank))
                            return gtype + temp.MatchupID;
                        break;
                    case 12://1/8
                        if (round == 3 && temp.SeedR8b.Contains(rank))
                            return gtype + temp.MatchupID;
                        break;

                    case 13://1/4
                        if (round == 4 && temp.SeedL4.Contains(rank))
                            return gtype + temp.MatchupID;
                        break;
                    case 14://1/4
                        if (round == 4 && temp.SeedR4.Contains(rank))
                            return gtype + temp.MatchupID;
                        break;
                }
            }
            Console.WriteLine("GetOrderNumber #{0} round:{1} gtype:{2}, fail!", rank, round, gtype);
            return 0;
        }

        private static Dictionary<int, PlayerEliteGameInfo> m_elitePlayersG3 = new Dictionary<int, PlayerEliteGameInfo>();
        private static Dictionary<int, PlayerEliteGameInfo> m_elitePlayersG4 = new Dictionary<int, PlayerEliteGameInfo>();

        private static Dictionary<int, List<EliteGameAwardInfo>> m_eliteGameAwards = new Dictionary<int, List<EliteGameAwardInfo>>();

        public static Dictionary<int, PlayerEliteGameInfo> EliteGameChampionPlayersList(int group)
        {
            if (group == 2)
                return m_elitePlayersG4;

            return m_elitePlayersG3;
        }

        private static List<int> EliteGameDayOpening = new List<int>();

        public static bool Init()
        {
            LoadDaysOpening();
            m_elitePlayersG3.Clear();
            m_elitePlayersG4.Clear();
            AreaConfigInfo ac = WorldMgr.FindAreaConfig(GameServer.Instance.Configuration.AreaID);
            using (PlayerBussiness pb = new PlayerBussiness())
            {
                PlayerEliteGameInfo[] listPlayers = pb.GetAllPlayerEliteGame();
                var areaDatas = listPlayers.Where(s => s.AreaId == ac.AreaID);

                foreach (PlayerEliteGameInfo b in listPlayers)
                {
                    if (b.GroupType == 1 && !m_elitePlayersG3.ContainsKey(b.UserID))
                        m_elitePlayersG3.Add(b.UserID, b);

                    if (b.GroupType == 2 && !m_elitePlayersG4.ContainsKey(b.UserID))
                        m_elitePlayersG4.Add(b.UserID, b);
                }
            }

            m_serverEvent = new Dictionary<string, ServerEventInfo>();
            LoadServerEvent(m_serverEvent);

            EliteGameAwardInfo[] tempEliteGameAward = LoadEliteGameAwardDb();
            Dictionary<int, List<EliteGameAwardInfo>> tempEliteGameAwards = LoadEliteGameAwards(tempEliteGameAward);
            if (tempEliteGameAward.Length > 0)
            {
                Interlocked.Exchange(ref m_eliteGameAwards, tempEliteGameAwards);
            }

            //set current process
            EliteStatus = (EliteGameStatusType)(int.Parse(m_serverEvent["EliteStatus"].Value));
            EliteGameMode = int.Parse(m_serverEvent["EliteGameMode"].Value);
            CreateMatchOrder();
            return true;
        }

        public static EliteGameAwardInfo[] LoadEliteGameAwardDb()
        {
            using (ProduceBussiness pb = new ProduceBussiness())
            {
                EliteGameAwardInfo[] infos = pb.GetAllEliteGameAward();
                return infos;
            }
        }

        public static Dictionary<int, List<EliteGameAwardInfo>> LoadEliteGameAwards(EliteGameAwardInfo[] eliteGameAwards)
        {
            Dictionary<int, List<EliteGameAwardInfo>> infos = new Dictionary<int, List<EliteGameAwardInfo>>();
            foreach (EliteGameAwardInfo info in eliteGameAwards)
            {
                if (!infos.Keys.Contains(info.Result))
                {
                    IEnumerable<EliteGameAwardInfo> temp = eliteGameAwards.Where(s => s.Result == info.Result);
                    infos.Add(info.Result, temp.ToList());
                }
            }
            return infos;
        }

        public static List<EliteGameAwardInfo> FindEliteGameAwards(int id)
        {
            if (m_eliteGameAwards.ContainsKey(id))
            {
                List<EliteGameAwardInfo> items = m_eliteGameAwards[id];
                return items;
            }
            return null;
        }

        public static bool IsBlockWeapon(int templateid)
        {
            bool isBlock = false;

            string[] strList = GameProperties.EliteGameBlockWeapon.Split('|');
            if (strList.Contains(templateid.ToString()))
                isBlock = true;

            return isBlock;
        }

        private static void ResetEliteGameScoreData()
        {
            using (PlayerBussiness pb = new PlayerBussiness())
            {
                bool result = pb.ResetEliteGame(int.Parse(m_serverEvent["EliteStartPoint"].Value));
            }
        }

        private static void LoadDaysOpening()
        {
            EliteGameDayOpening = new List<int>();

            string[] daysOpening = GameProperties.EliteGameDayOpening.Split(',');
            foreach (string str in daysOpening)
                EliteGameDayOpening.Add(int.Parse(str));
        }

        private static void UpdateServerEvent(string condition, string value)
        {
            using (PlayerBussiness pb = new PlayerBussiness())
                pb.UpdateEditable("Server_Event", "Value", value, "Name = '" + condition + "'");
        }

        private static void UpdatePlayerEliteRank(string rank, string Uid)
        {
            using (PlayerBussiness pb = new PlayerBussiness())
                pb.UpdateEditable("Sys_Users_Detail", "EliteRank", rank, "UserID = " + Uid);
        }

        private static bool LoadServerEvent(Dictionary<string, ServerEventInfo> serverEvent)
        {
            using (PlayerBussiness db = new PlayerBussiness())
            {
                ServerEventInfo[] infos = db.GetServerEvent();
                foreach (ServerEventInfo info in infos)
                {
                    if (!serverEvent.ContainsKey(info.Name))
                    {
                        serverEvent.Add(info.Name, info);
                    }
                }
                Console.WriteLine($"serverEvent Count:{serverEvent.Count}");
            }

            return true;
        }

        public static ServerEventInfo ServerEvent(string key)
        {
            if (m_serverEvent.ContainsKey(key))
                return m_serverEvent[key];
            return null;
        }

        public static void EliteGameUpdateScore(PlayerInfo player, int group)
        {
            EliteGameUpdateScore(player, group, -1);
        }

        public static void EliteGameUpdateScore(PlayerInfo player, int group, int winner)
        {
            PlayerEliteGameInfo pinfo = new PlayerEliteGameInfo
            {
                UserID = player.ID,
                AreaId = GameServer.Instance.Configuration.AreaID,
                NickName = player.NickName,
                GroupType = group,
                CurrentPoint = player.EliteScore,
                Grade = player.Grade,
                Blood = player.EliteHpEndMatch,
                FightPower = player.FightPower          
            };
            if(winner != -1)
            {
                pinfo.Winer = winner;
            }
            pinfo.TotalMatch = 1;
            AddEliteGamePlayer(pinfo, group);
        }
        private static bool IsBreakTour30To40()
        {
            return m_elitePlayersG3.Count < 2;
        }
        private static bool IsBreakTour41Up()
        {
            return m_elitePlayersG4.Count < 8;
        }
        
        #region EliteGameCreateTOP
        private static void CreateTOP16EliteGameRound()
        {
            //CreateMatchOrder();
            // create top 16 30-40
            var playersList = m_elitePlayersG3.
                Where(a => a.Value.GroupType == 1 && a.Value.CurrentPoint > 0).
                //OrderByDescending(a => a.Value.Rank <=16).
                OrderByDescending(a => a.Value.CurrentPoint).
                Select(a => a.Key).Take(16).ToList();

            int roundid = 1;

            lock (m_elitePlayersG3)
                foreach (var key in playersList)
                {
                    var ep = m_elitePlayersG3[key];
                    ep.Rank = roundid;
                    ep.Status = 2;
                    ep.Winer = 0;
                    ep.ReadyStatus = false;
                    ep.RoundType = 16;
                    ep.MatchOrderNumber = GetOrderNumber(roundid, ep.GroupType);
                    roundid++;
                }

            // create top 16 41-50
            playersList = m_elitePlayersG4.
                Where(a => a.Value.GroupType == 2 && a.Value.TotalMatch > 0).
                //OrderByDescending(a => a.Value.Rank <=16).
                OrderByDescending(a => a.Value.CurrentPoint).
                Select(a => a.Key).Take(16).ToList();

            roundid = 1;

            lock (m_elitePlayersG4)
                foreach (var key in playersList)
                {
                    var ep = m_elitePlayersG4[key];
                    ep.Rank = roundid;
                    ep.Status = 2;
                    ep.Winer = 0;
                    ep.ReadyStatus = false;
                    ep.RoundType = 16;
                    ep.MatchOrderNumber = GetOrderNumber(roundid, ep.GroupType);
                    roundid++;
                }

            Save();
        }

        public static void CreateTOP8EliteGameRound()
        {
            CheckRoundWinner(16);

            var gameRound = m_elitePlayersG3.
                Where(a => a.Value.RoundType == 16 && a.Value.Winer == 1).
                OrderBy(a => a.Value.MatchOrderNumber).
                Select(a => a.Key).ToList();//

            lock (m_elitePlayersG3)
                foreach (var key in gameRound)
                {
                    var ep = m_elitePlayersG3[key];
                    ep.Status = 3;
                    ep.Winer = 0;
                    ep.ReadyStatus = false;
                    ep.RoundType = 8;
                    ep.MatchOrderNumber = GetOrderNumber(ep.Rank, ep.GroupType, ep.Status);
                }

            gameRound = m_elitePlayersG4.
                Where(a => a.Value.RoundType == 16 && a.Value.Winer == 1).
                OrderBy(a => a.Value.MatchOrderNumber).
                Select(a => a.Key).ToList();

            lock (m_elitePlayersG4)
                foreach (var key in gameRound)
                {
                    var ep = m_elitePlayersG4[key];
                    ep.Status = 3;
                    ep.Winer = 0;
                    ep.ReadyStatus = false;
                    ep.RoundType = 8;
                    ep.MatchOrderNumber = GetOrderNumber(ep.Rank, ep.GroupType, ep.Status);
                }

            Save();
        }

        public static void CreateTOP4EliteGameRound()
        {
            CheckRoundWinner(8);

            var gameRound = m_elitePlayersG3.
                Where(a => a.Value.RoundType == 8 && a.Value.Winer == 1).
                OrderBy(a => a.Value.MatchOrderNumber).
                Select(a => a.Key).ToList();//

            lock (m_elitePlayersG3)
                foreach (var key in gameRound)
                {
                    var ep = m_elitePlayersG3[key];
                    ep.Status = 4;
                    ep.Winer = 0;
                    ep.ReadyStatus = false;
                    ep.RoundType = 4;
                    ep.MatchOrderNumber = GetOrderNumber(ep.Rank, ep.GroupType, ep.Status);

                }

            gameRound = m_elitePlayersG4.
                Where(a => a.Value.RoundType == 8 && a.Value.Winer == 1).
                OrderBy(a => a.Value.MatchOrderNumber).
                Select(a => a.Key).ToList();

            lock (m_elitePlayersG4)
                foreach (var key in gameRound)
                {
                    var ep = m_elitePlayersG4[key];
                    ep.Status = 4;
                    ep.Winer = 0;
                    ep.ReadyStatus = false;
                    ep.RoundType = 4;
                    ep.MatchOrderNumber = GetOrderNumber(ep.Rank, ep.GroupType, ep.Status);

                }

            Save();
        }

        public static void CreateTOP2EliteGameRound()
        {
            CheckRoundWinner(4);

            var gameRound = m_elitePlayersG3.
                Where(a => a.Value.RoundType == 4 && a.Value.Winer == 1).
                OrderBy(a => a.Value.MatchOrderNumber).
                Select(a => a.Key).ToList();

            lock (m_elitePlayersG3)
                foreach (var key in gameRound)
                {
                    var ep = m_elitePlayersG3[key];
                    ep.Status = 5;
                    ep.Winer = 0;
                    ep.ReadyStatus = false;
                    ep.RoundType = 2;
                    ep.MatchOrderNumber = GetOrderNumber(ep.Rank, ep.GroupType, ep.Status);

                }

            gameRound = m_elitePlayersG4.
                Where(a => a.Value.RoundType == 4 && a.Value.Winer == 1).
                OrderBy(a => a.Value.MatchOrderNumber).
                Select(a => a.Key).ToList();

            lock (m_elitePlayersG4)
                foreach (var key in gameRound)
                {
                    var ep = m_elitePlayersG4[key];
                    ep.Status = 5;
                    ep.Winer = 0;
                    ep.ReadyStatus = false;
                    ep.RoundType = 2;
                    ep.MatchOrderNumber = GetOrderNumber(ep.Rank, ep.GroupType, ep.Status);
                }

            Save();
        }

        public static void CreateChampionEliteGameRound()
        {
            CheckRoundWinner(2);
            var gameRound = m_elitePlayersG3.Where(a => a.Value.RoundType == 2 && a.Value.Winer == 1).Select(a => a.Value).ToList();

            foreach (PlayerEliteGameInfo ep in gameRound)
            {
                Console.WriteLine("G3 Uid#{2} NickName:{0}, roundType:{1}, become champion this round!", ep.NickName, ep.RoundType, ep.UserID);
            }

            gameRound = m_elitePlayersG4.Where(a => a.Value.RoundType == 2 && a.Value.Winer == 1).Select(a => a.Value).ToList();
            foreach (PlayerEliteGameInfo ep in gameRound)
            {
                Console.WriteLine("G4 Uid#{2} NickName:{0}, roundType:{1}, become champion this round!", ep.NickName, ep.RoundType, ep.UserID);
            }

            Save();
        }

        /// <summary>
        /// test function
        /// </summary>
        public static void TestChampionEliteGame(string value)
        {
            Console.WriteLine($"value = {value}");
            switch (value)
            {
                case "16":
                    ReloadEliteScoreRank();//test func
                    CreateTOP16EliteGameRound();
                    break;
                case "8":
                    CreateTOP8EliteGameRound();
                    break;
                case "4":
                    CreateTOP4EliteGameRound();
                    break;
                case "2":
                    CreateTOP2EliteGameRound();
                    break;
                case "1":
                    CreateChampionEliteGameRound();
                    break;
                case "200":
                    List<PlayerEliteGameInfo> playersList = m_elitePlayersG3.
                                                               Where(a => a.Value.Status >= 1).
                                                               Take(200).Select(a => a.Value).ToList();
                    Console.WriteLine("top 200:{0}", playersList.Count);
                    break;
            }
        }

        private static void CheckRoundWinner(int roundType)
        {
            var redList = m_elitePlayersG3.Where(a => a.Value.RoundType == roundType).Select(a => a.Key).ToList();
            //Console.WriteLine("currentRound.Count G3:{0}, roundType:{1}", redList.Count, roundType);
            PlayerEliteGameInfo matchPlayer = null;
            lock (m_elitePlayersG3)
                foreach (var key in redList)
                {
                    var red = m_elitePlayersG3[key];
                    if (red.Winer == 1)
                        continue;

                    matchPlayer = null;
                    foreach (PlayerEliteGameInfo blue in m_elitePlayersG3.Values)
                    {
                        if (red == blue || blue.Winer == 1 || blue.CheckedWon)
                            continue;

                        if (red.MatchOrderNumber == blue.MatchOrderNumber)
                        {
                            if (red.ReadyStatus && !blue.ReadyStatus)
                            {
                                red.Winer = 1;
                                matchPlayer = blue;
                            }
                            else if (!red.ReadyStatus && !blue.ReadyStatus)
                            {
                                red.ReadyStatus = CanWin(red, blue);
                                if (red.ReadyStatus)
                                {
                                    red.CheckedWon = true;
                                }
                            }                           
                        }
                    }

                    if (matchPlayer == null && red.ReadyStatus)//enemy ofline or online but do not accept invite
                        red.Winer = 1;
                }

            redList = m_elitePlayersG4.Where(a => a.Value.RoundType == roundType).Select(a => a.Key).ToList();
            //Console.WriteLine("currentRound.Count G4:{0}, roundType:{1}", redList.Count, roundType);
            lock (m_elitePlayersG4)
                foreach (var key in redList)
                {
                    var red = m_elitePlayersG4[key];
                    if (red.Winer == 1)
                        continue;

                    matchPlayer = null;
                    foreach (PlayerEliteGameInfo blue in m_elitePlayersG4.Values)
                    {
                        if (red == blue || blue.Winer == 1 || blue.CheckedWon)
                            continue;

                        if (red.MatchOrderNumber == blue.MatchOrderNumber)
                        {
                            if (red.ReadyStatus && !blue.ReadyStatus)
                            {
                                red.Winer = 1;
                                matchPlayer = blue;
                            }
                            else if (!red.ReadyStatus && !blue.ReadyStatus)
                            {
                                red.ReadyStatus = CanWin(red, blue);
                                if (red.ReadyStatus)
                                {
                                    red.CheckedWon = true;
                                }
                            }
                        }
                    }

                    if (matchPlayer == null && red.ReadyStatus)//enemy ofline or online but do not accept invite
                        red.Winer = 1;
                }
        }

        static int GetMinBattle(int RoundType)
        {
            int min = 3;
            switch (RoundType)
            {
                case 16:
                    return min;//offline and have join less 3 war
                case 8:
                    return min + 1;//offline and join round 1/16
                case 4:
                    return min + 2;//offline and join round 1/16 + 1/8
                case 2:
                    return min + 3;//offline and join round 1/16 + 1/8 + 1/4
            }
            return 1;
        }
        /// <summary>
        /// this function use for 2 player offline
        /// </summary>
        /// <param name="red"></param>
        /// <param name="blue"></param>
        /// <returns></returns>
        private static bool CanWin(PlayerEliteGameInfo red, PlayerEliteGameInfo blue)
        {
            if (blue.CheckedWon)//blue been set won will set red lost
                return false;

            if (red.TotalMatch > blue.TotalMatch && red.TotalMatch > GetMinBattle(red.GroupType))//1: this condition make sure player online and have war
                return true;
            if (red.CurrentPoint > blue.CurrentPoint)//2: if 1 same count will check point
                return true;            
            if (red.FightPower > blue.FightPower)//3: if 2 same point will check this
                return true;
            if (red.Blood > blue.Blood)//4: if 3 same FightPower will check this
                return true;
            if (red.Grade > blue.Grade)//5: if 4 same FightPower will check this
                return true;

            return true;//if 1 2 3 4 5 false => red win
        }

        #endregion

        public static void UpdateEliteGameInfo(PlayerInfo pinfo, int group)
        {
            if (group == 1)
                lock (m_elitePlayersG3)
                {
                    if (m_elitePlayersG3.ContainsKey(pinfo.ID))
                    {
                        m_elitePlayersG3[pinfo.ID].FightPower = pinfo.FightPower;
                        m_elitePlayersG3[pinfo.ID].Grade = pinfo.Grade;
                    }
                }

            if (group == 2)
                lock (m_elitePlayersG4)
                {
                    if (m_elitePlayersG4.ContainsKey(pinfo.ID))
                    {
                        m_elitePlayersG4[pinfo.ID].FightPower = pinfo.FightPower;
                        m_elitePlayersG4[pinfo.ID].Grade = pinfo.Grade;
                    }
                }           
        }

        public static void AddEliteGamePlayer(PlayerEliteGameInfo pinfo, int group)
        {
            if (group == 1)
                lock (m_elitePlayersG3)
                {
                    if (m_elitePlayersG3.ContainsKey(pinfo.UserID))
                    {
                        m_elitePlayersG3[pinfo.UserID].UserMoneyPay = pinfo.UserMoneyPay;
                        m_elitePlayersG3[pinfo.UserID].FightPower = pinfo.FightPower;
                        m_elitePlayersG3[pinfo.UserID].Blood = pinfo.Blood;
                        m_elitePlayersG3[pinfo.UserID].Grade = pinfo.Grade;
                        // check point
                        m_elitePlayersG3[pinfo.UserID].GroupType = pinfo.GroupType;
                        m_elitePlayersG3[pinfo.UserID].CurrentPoint = pinfo.CurrentPoint;
                        m_elitePlayersG3[pinfo.UserID].Winer = pinfo.Winer;
                        if (pinfo.TotalMatch != 0)
                        {
                            m_elitePlayersG3[pinfo.UserID].TotalMatch += pinfo.TotalMatch;
                        }
                    }
                    else
                    {
                        m_elitePlayersG3.Add(pinfo.UserID, pinfo);
                    }
                }

            if (group == 2)
                lock (m_elitePlayersG4)
                {
                    if (m_elitePlayersG4.ContainsKey(pinfo.UserID))
                    {
                        m_elitePlayersG4[pinfo.UserID].UserMoneyPay = pinfo.UserMoneyPay;
                        m_elitePlayersG4[pinfo.UserID].FightPower = pinfo.FightPower;
                        m_elitePlayersG4[pinfo.UserID].Blood = pinfo.Blood;
                        m_elitePlayersG4[pinfo.UserID].Grade = pinfo.Grade;
                        // check point
                        m_elitePlayersG4[pinfo.UserID].GroupType = pinfo.GroupType;
                        m_elitePlayersG4[pinfo.UserID].CurrentPoint = pinfo.CurrentPoint;
                        m_elitePlayersG4[pinfo.UserID].Winer = pinfo.Winer;
                        if (pinfo.TotalMatch != 0)
                        {
                            m_elitePlayersG4[pinfo.UserID].TotalMatch += pinfo.TotalMatch;
                        }
                    }
                    else
                    {
                        m_elitePlayersG4.Add(pinfo.UserID, pinfo);
                    }
                }
        }

        public static void UpdateEliteBattleStatus(int userId, bool isReady, int group)
        {
            if (group == 1)
                lock (m_elitePlayersG3)
                {
                    if (m_elitePlayersG3.ContainsKey(userId))
                    {
                        // check point
                        m_elitePlayersG3[userId].ReadyStatus = isReady;
                    }
                }

            if (group == 2)
                lock (m_elitePlayersG4)
                {
                    if (m_elitePlayersG4.ContainsKey(userId))
                    {
                        // check point
                        m_elitePlayersG4[userId].ReadyStatus = isReady;
                    }
                }
        }

        public static void SendEliteInviteGameRoom(int status)
        {
            var merged = m_elitePlayersG3.Merge(m_elitePlayersG4);
            List<PlayerEliteGameInfo> eliteGames = merged.Where(a => a.Value.Status == status).Select(a => a.Value).ToList();
            if (eliteGames == null)
            {
                WorldMgr.SendSysNotice($"EliteInviteGameRoom can not get list player status #{status}");
                return;
            }

            foreach (PlayerEliteGameInfo elite in eliteGames)
            {
                GamePlayer p = WorldMgr.GetPlayerById(elite.UserID);
                WorldMgr.SendSysNotice($"EliteInviteGameRoom #{elite.NickName} [{elite.UserID}] status:{status}");
                if (p != null)
                    p.Out.SendEliteGameStartRoom();
            }
        }

        public static void EliteGameReload(int startPoint)
        {
            foreach (GamePlayer p in WorldMgr.GetAllPlayers())
            {
                p.PlayerCharacter.EliteScore = startPoint;
                p.PlayerCharacter.EliteRank = 0;

            }
        }

        public static void EliteGameKickOut(int type)
        {
            // find all server
            RoomMgr.KickUsingRoom(type);
        }

        public static void ReloadEliteScoreRank()
        {
            BuildEliteMatchPlayerList();//Build:elitematchplayerlist.xml

            int rank = 1;

            List<PlayerEliteGameInfo> listSort = m_elitePlayersG3.
                   OrderByDescending(a => a.Value.CurrentPoint).
                   Select(a => a.Value).ToList();

            lock (m_elitePlayersG3)
            {
                if (listSort != null)
                {
                    foreach (PlayerEliteGameInfo p in listSort)
                    {
                        p.Rank = rank;
                        GamePlayer gp = WorldMgr.GetPlayerById(p.UserID);
                        if (gp != null)
                            gp.UpdateEliteRank(rank);
                        else
                            UpdatePlayerEliteRank(rank.ToString(), p.UserID.ToString());

                        if (rank <= 100 && p.TotalMatch > 0)//filter player have 1000point and dont have any battle
                            p.Status = 1;

                        rank++;
                    }
                }
            }

            rank = 1;

            listSort = m_elitePlayersG4.
                   OrderByDescending(a => a.Value.CurrentPoint).
                   Select(a => a.Value).ToList();

            lock (m_elitePlayersG4)
            {
                if (listSort != null)
                {
                    foreach (PlayerEliteGameInfo p in listSort)
                    {
                        p.Rank = rank;
                        var gp = WorldMgr.GetPlayerById(p.UserID);
                        if (gp != null)
                            gp.UpdateEliteRank(rank);
                        else
                            UpdatePlayerEliteRank(rank.ToString(), p.UserID.ToString());

                        if (rank <= 100 && p.TotalMatch > 0)//filter player have 1000point and dont have any battle
                            p.Status = 1;

                        rank++;
                    }
                }
            }

            Save();
        }

        public static PlayerEliteGameInfo FindEliteRoundByUser(int userId, int group)
        {
            List<PlayerEliteGameInfo> roundSort = m_elitePlayersG3.Values.OrderByDescending(a => a.RoundType).ToList();
            if (group == 2)
            {
                roundSort = m_elitePlayersG4.Values.OrderByDescending(a => a.RoundType).ToList();
            }
            foreach (PlayerEliteGameInfo round in roundSort)
            {
                if (round.UserID == userId)
                    return round;
            }

            return null;
        }

        private static int m_timeBeforeScoreNotice = 5;

        private static bool m_updateRoundElite = false;

        private static bool m_sendInviteGameRoom = false;

        public static void ResetEliteGameData()
        {
            EliteGameReload(int.Parse(m_serverEvent["EliteStartPoint"].Value));
            m_elitePlayersG3 = new Dictionary<int, PlayerEliteGameInfo>();
            m_elitePlayersG4 = new Dictionary<int, PlayerEliteGameInfo>();
            m_sendInviteGameRoom = false;
            m_updateRoundElite = false;
            ResetEliteGameScoreData();
            ClearEliteGameData();
        }

        public static void ClearEliteGameData()
        {
            using (PlayerBussiness pb = new PlayerBussiness())
                pb.ClearEliteGameData();
        }

        private static void BuildEliteMatchPlayerList()
        {
            AreaConfigInfo ac = WorldMgr.FindAreaConfig(GameServer.Instance.Configuration.AreaID);
            if (ac != null && !string.IsNullOrEmpty(ac.RequestUrl))
            {
                string url = ac.RequestUrl + "elitematchplayerlist.ashx";
                try
                {
                    string resultStr = BaseInterface.RequestContent(url);
                    Console.WriteLine(resultStr);
                }
                catch { Console.WriteLine("Wrong url {0}", url); }
            }
        }

        public static void Save()
        {
            var merged = m_elitePlayersG3.Merge(m_elitePlayersG4);

            List<PlayerEliteGameInfo> list = merged.
                    Where(a => a.Value.CurrentPoint > 0).
                    OrderByDescending(a => a.Value.CurrentPoint).
                    Select(a => a.Value).ToList();

            using (PlayerBussiness pb = new PlayerBussiness())
            {
                foreach (var p in list)
                {
                    pb.AddPlayerEliteGame(p);
                }
            }
        }

        public static void CheckEliteGameEvent()
        {          
            DateTime now = DateTime.Now;
            #region Checked
            //Console.WriteLine($"EliteGameDayOpening = {!EliteGameDayOpening.Contains((int)now.DayOfWeek)}");
            //Console.WriteLine($"EliteStatus = {EliteStatus}");
            #endregion
            if (!EliteGameDayOpening.Contains((int)now.DayOfWeek))
                return;
            switch (EliteStatus)
            {
                case EliteGameStatusType.CLOSE:
                    {
                        DateTime timeOpenScore = TimeWithToday(m_serverEvent["EliteScoreStartTime"].Value, now);
                        if (SameDateTime(timeOpenScore, now))
                        {
                            EliteStatus = EliteGameStatusType.OPEN_SCORE;
                            UpdateServerEvent("EliteStatus", ((int)EliteStatus).ToString());
                            Console.WriteLine("-- EliteGame Event SCORE STATUS Is OPEN!");
                            m_timeBeforeScoreNotice = 5;
                            ResetEliteGameData();
                            EliteGameMode = (int)EliteGameModeType.SCORE_TIME;
                            UpdateServerEvent("EliteGameMode", EliteGameMode.ToString());
                            WorldMgr.SendSysNotice("Giải đấu Vua Gà đã mở. Tham gia ngay để nhận 'Long Thương Chiến' và phần thưởng hấp dẫn mỗi tuần.");
                        }
                        else if (now < timeOpenScore)
                        {
                            TimeSpan spanTime = timeOpenScore - now;
                            if (Math.Ceiling(spanTime.TotalMinutes) <= 5 && Math.Ceiling(spanTime.TotalMinutes) == m_timeBeforeScoreNotice)
                            {
                                m_timeBeforeScoreNotice--;
                                WorldMgr.SendSysNotice(string.Format("Giải đấu Vua Gà chuẩn bị mở trong vòng {0} phút nữa.", Math.Ceiling(spanTime.TotalMinutes)));
                            }
                        }
                    }
                    break;
                case EliteGameStatusType.OPEN_SCORE:
                    {
                        DateTime timeCloseScore = TimeWithToday(m_serverEvent["EliteScoreEndTime"].Value, now);
                        if (SameDateTime(timeCloseScore, now))
                        {
                            EliteStatus = EliteGameStatusType.WATING_CHAMPION;
                            UpdateServerEvent("EliteStatus", ((int)EliteStatus).ToString());
                            m_timeBeforeScoreNotice = 3;
                            m_updateRoundElite = false;
                            m_sendInviteGameRoom = false;
                            Console.WriteLine("-- EliteGame Event SCORE STATUS Is CLOSED!");
                            ReloadEliteScoreRank();
                            EliteGameKickOut(12);
                            EliteGameMode = (int)EliteGameModeType.CHAMPION_TIME;
                            UpdateServerEvent("EliteGameMode", EliteGameMode.ToString());
                            WorldMgr.SendSysNotice("Vòng 1 giải Vua Gà đã kết thúc.");
                        }
                        else if (timeCloseScore > now)
                        {
                            TimeSpan spanTime = timeCloseScore - now;

                            if (Math.Ceiling(spanTime.TotalMinutes) <= 5 && Math.Ceiling(spanTime.TotalMinutes) == m_timeBeforeScoreNotice)
                            {
                                m_timeBeforeScoreNotice--;
                                WorldMgr.SendSysNotice(string.Format("Vòng 1 giải Vua Gà sẽ đóng lại trong {0} phút nữa.", Math.Ceiling(spanTime.TotalMinutes)));
                            }
                        }
                    }
                    break;
                case EliteGameStatusType.WATING_CHAMPION:
                    {
                        if(IsBreakTour30To40() && DateTime.Now.DayOfWeek != DayOfWeek.Saturday)// && IsBreakTour41Up() && DateTime.Now.DayOfWeek != DayOfWeek.Saturday)
                        {
                            EliteStatus = EliteGameStatusType.CLOSE;
                            Console.WriteLine("EliteGame close because total player < 8!");
                            break;
                        }
                        DateTime timeOpenCham16 = TimeWithToday(m_serverEvent["EliteChampion16StartTime"].Value, now);
                        if (SameDateTime(timeOpenCham16, now))
                        {
                            EliteStatus = EliteGameStatusType.START_16_CHAP;
                            UpdateServerEvent("EliteStatus", ((int)EliteStatus).ToString());
                            m_timeBeforeScoreNotice = 3;
                            Console.WriteLine("-- EliteGame Event CHAMPION 16 STATUS is OPEN!");
                            WorldMgr.SendSysNotice("Vòng loại giải Vua Gà đã mở.");
                        }
                        else if (timeOpenCham16 > now)
                        {
                            TimeSpan spanTime = timeOpenCham16 - now;
                            if (Math.Ceiling(spanTime.TotalMinutes) <= 3 && Math.Ceiling(spanTime.TotalMinutes) == m_timeBeforeScoreNotice)
                            {
                                m_timeBeforeScoreNotice--;
                                WorldMgr.SendSysNotice(string.Format("Vòng loại giải Vua Gà sẽ mở sau {0} phút nữa.", Math.Ceiling(spanTime.TotalMinutes)));
                            }
                            if (!m_updateRoundElite)
                            {
                                m_updateRoundElite = true;
                                CreateTOP16EliteGameRound();
                            }
                            if (Math.Ceiling(spanTime.TotalSeconds) <= 30 && !m_sendInviteGameRoom)
                            {
                                m_sendInviteGameRoom = true;
                                SendEliteInviteGameRoom(2);
                            }
                        }
                    }
                    break;
                case EliteGameStatusType.START_16_CHAP:
                    {
                        DateTime timeEndCham16 = TimeWithToday(m_serverEvent["EliteChampion16EndTime"].Value, now);
                        if (SameDateTime(timeEndCham16, now))
                        {
                            EliteStatus = EliteGameStatusType.END_16_CHAP;
                            UpdateServerEvent("EliteStatus", ((int)EliteStatus).ToString());
                            m_updateRoundElite = false;
                            m_sendInviteGameRoom = false;
                            m_timeBeforeScoreNotice = 3;
                            Console.WriteLine("-- EliteGame Event CHAMPION 16 STATUS is CLOSED!");
                            EliteGameKickOut(13);
                            WorldMgr.SendSysNotice("Vòng loại giải Vua Gà đã đóng. Người chơi lọt vào Tứ Kết vui lòng chờ thông báo.");
                        }
                        else if (timeEndCham16 > now)
                        {
                            TimeSpan spanTime = timeEndCham16 - now;
                            if (Math.Ceiling(spanTime.TotalMinutes) <= 3 && Math.Ceiling(spanTime.TotalMinutes) == m_timeBeforeScoreNotice)
                            {
                                m_timeBeforeScoreNotice--;
                                WorldMgr.SendSysNotice(string.Format("Vòng loại giải Vua Gà sẽ đóng lại sau {0} phút nữa.", Math.Ceiling(spanTime.TotalMinutes)));
                            }
                        }
                    }
                    break;
                case EliteGameStatusType.END_16_CHAP:
                    {
                        DateTime timeStartCham8 = TimeWithToday(m_serverEvent["EliteChampion8StartTime"].Value, now);
                        if (SameDateTime(timeStartCham8, now))
                        {
                            EliteStatus = EliteGameStatusType.START_8_CHAP;
                            UpdateServerEvent("EliteStatus", ((int)EliteStatus).ToString());
                            m_timeBeforeScoreNotice = 3;
                            Console.WriteLine("-- EliteGame Event CHAMPION 8 STATUS is OPEN!");
                            WorldMgr.SendSysNotice(LanguageMgr.GetTranslation("Vòng Tứ Kết giải Vua Gà đã mở."));
                        }
                        else if (timeStartCham8 > now)
                        {
                            TimeSpan spanTime = timeStartCham8 - now;

                            if (Math.Ceiling(spanTime.TotalMinutes) <= 3 && Math.Ceiling(spanTime.TotalMinutes) == m_timeBeforeScoreNotice)
                            {
                                m_timeBeforeScoreNotice--;
                                WorldMgr.SendSysNotice(string.Format("Vòng Tứ Kết giải Vua Gà sẽ mở trong {0} phút nữa.", Math.Ceiling(spanTime.TotalMinutes)));
                            }
                            if (!m_updateRoundElite)
                            {
                                m_updateRoundElite = true;
                                CreateTOP8EliteGameRound();
                            }
                            if (Math.Ceiling(spanTime.TotalSeconds) <= 30 && !m_sendInviteGameRoom)
                            {
                                m_sendInviteGameRoom = true;
                                SendEliteInviteGameRoom(3);
                            }
                        }
                    }
                    break;
                case EliteGameStatusType.START_8_CHAP:
                    {
                        DateTime timeEndCham8 = TimeWithToday(m_serverEvent["EliteChampion8EndTime"].Value, now);
                        if (SameDateTime(timeEndCham8, now))
                        {
                            EliteStatus = EliteGameStatusType.END_8_CHAP;
                            UpdateServerEvent("EliteStatus", ((int)EliteStatus).ToString());
                            m_timeBeforeScoreNotice = 3;
                            m_updateRoundElite = false;
                            m_sendInviteGameRoom = false;
                            Console.WriteLine("-- EliteGame Event CHAMPION 8 STATUS is CLOSED!");
                            EliteGameKickOut(13);
                            WorldMgr.SendSysNotice("Vòng Tứ Kết giải Vua Gà đã đóng lại. Người chơi lọt vào vòng Bán Kết vui lòng chờ thông báo.");
                        }
                        else if (timeEndCham8 > now)
                        {
                            TimeSpan spanTime = timeEndCham8 - now;

                            if (Math.Ceiling(spanTime.TotalMinutes) <= 3 && Math.Ceiling(spanTime.TotalMinutes) == m_timeBeforeScoreNotice)
                            {
                                m_timeBeforeScoreNotice--;
                                WorldMgr.SendSysNotice(string.Format("Vòng Tứ Kết giải Vua Gà sẽ đóng lại sau {0} phút nữa.", Math.Ceiling(spanTime.TotalMinutes)));
                            }
                        }
                    }
                    break;

                case EliteGameStatusType.END_8_CHAP:
                    {
                        DateTime timeStartCham4 = TimeWithToday(m_serverEvent["EliteChampion4StartTime"].Value, now);
                        if (SameDateTime(timeStartCham4, now))
                        {
                            // mở vòng tứ kết
                            EliteStatus = EliteGameStatusType.START_4_CHAP;
                            UpdateServerEvent("EliteStatus", ((int)EliteStatus).ToString());
                            m_timeBeforeScoreNotice = 3;
                            Console.WriteLine("-- EliteGame Event CHAMPION 4 STATUS is OPEN!");
                            WorldMgr.SendSysNotice("Vòng Bán Kết giải Vua Gà đã mở.");
                        }
                        else if (timeStartCham4 > now)
                        {
                            TimeSpan spanTime = timeStartCham4 - now;

                            if (Math.Ceiling(spanTime.TotalMinutes) <= 3 && Math.Ceiling(spanTime.TotalMinutes) == m_timeBeforeScoreNotice)
                            {
                                m_timeBeforeScoreNotice--;
                                WorldMgr.SendSysNotice(string.Format("Vòng Bán Kết giải Vua Gà sẽ mở trong {0} phút nữa.", Math.Ceiling(spanTime.TotalMinutes)));
                            }

                            if (!m_updateRoundElite)
                            {
                                m_updateRoundElite = true;
                                CreateTOP4EliteGameRound();
                            }

                            if (Math.Ceiling(spanTime.TotalSeconds) <= 30 && !m_sendInviteGameRoom)
                            {
                                m_sendInviteGameRoom = true;
                                SendEliteInviteGameRoom(4);
                            }
                        }
                    }
                    break;

                case EliteGameStatusType.START_4_CHAP:
                    {
                        DateTime timeEndCham4 = TimeWithToday(m_serverEvent["EliteChampion4EndTime"].Value, now);
                        if (SameDateTime(timeEndCham4, now))
                        {
                            EliteStatus = EliteGameStatusType.END_4_CHAP;
                            UpdateServerEvent("EliteStatus", ((int)EliteStatus).ToString());
                            m_timeBeforeScoreNotice = 3;
                            m_updateRoundElite = false;
                            m_sendInviteGameRoom = false;
                            Console.WriteLine("-- EliteGame Event CHAMPION 4 STATUS is CLOSED!");

                            EliteGameKickOut(13);
                            WorldMgr.SendSysNotice("Vòng Bán Kết giải Vua Gà đã đóng lại. Người chơi lọt vào vòng Chung Kết vui lòng chờ thông báo.");
                        }
                        else if (timeEndCham4 > now)
                        {
                            TimeSpan spanTime = timeEndCham4 - now;

                            if (Math.Ceiling(spanTime.TotalMinutes) <= 3 && Math.Ceiling(spanTime.TotalMinutes) == m_timeBeforeScoreNotice)
                            {
                                m_timeBeforeScoreNotice--;
                                WorldMgr.SendSysNotice(string.Format("Vòng Bán Kết giải Vua Gà sẽ đóng lại sau {0} phút nữa.", Math.Ceiling(spanTime.TotalMinutes)));
                            }
                        }
                    }
                    break;

                case EliteGameStatusType.END_4_CHAP:
                    {
                        DateTime timeStartCham2 = TimeWithToday(m_serverEvent["EliteChampion2StartTime"].Value, now);
                        if (SameDateTime(timeStartCham2, now))
                        {
                            EliteStatus = EliteGameStatusType.START_2_CHAP;
                            UpdateServerEvent("EliteStatus", ((int)EliteStatus).ToString());
                            m_timeBeforeScoreNotice = 3;
                            Console.WriteLine("-- EliteGame Event CHAMPION 2 STATUS is OPEN!");

                            WorldMgr.SendSysNotice("Vòng Chung Kết giải Vua Gà đã mở.");
                        }
                        else if (timeStartCham2 > now)
                        {
                            TimeSpan spanTime = timeStartCham2 - now;

                            if (Math.Ceiling(spanTime.TotalMinutes) <= 3 && Math.Ceiling(spanTime.TotalMinutes) == m_timeBeforeScoreNotice)
                            {
                                m_timeBeforeScoreNotice--;
                                WorldMgr.SendSysNotice(string.Format("Vòng Chung Kết giải Vua Gà sẽ mở trong {0} phút nữa.", Math.Ceiling(spanTime.TotalMinutes)));
                            }

                            if (!m_updateRoundElite)
                            {
                                m_updateRoundElite = true;
                                CreateTOP2EliteGameRound();
                            }

                            if (Math.Ceiling(spanTime.TotalSeconds) <= 30 && !m_sendInviteGameRoom)
                            {
                                m_sendInviteGameRoom = true;
                                SendEliteInviteGameRoom(5);
                            }
                        }
                    }
                    break;

                case EliteGameStatusType.START_2_CHAP:
                    {
                        DateTime timeEndCham2 = TimeWithToday(m_serverEvent["EliteChampion2EndTime"].Value, now);
                        if (SameDateTime(timeEndCham2, now))
                        {
                            EliteStatus = EliteGameStatusType.END_2_CHAP;
                            UpdateServerEvent("EliteStatus", ((int)EliteStatus).ToString());
                            m_timeBeforeScoreNotice = 3;
                            Console.WriteLine("-- EliteGame Event CHAMPION 2 STATUS is CLOSED!");

                            EliteGameKickOut(13);
                            EliteGameMode = (int)EliteGameModeType.CLOSE;
                            UpdateServerEvent("EliteGameMode", EliteGameMode.ToString());
                            WorldMgr.SendSysNotice("Vòng Chung Kết giải Vua Gà đã đóng lại. Phần thưởng sẽ được trao sau ít phút.");
                        }
                        else if (timeEndCham2 > now)
                        {
                            TimeSpan spanTime = timeEndCham2 - now;

                            if (Math.Ceiling(spanTime.TotalMinutes) <= 3 && Math.Ceiling(spanTime.TotalMinutes) == m_timeBeforeScoreNotice)
                            {
                                m_timeBeforeScoreNotice--;
                                WorldMgr.SendSysNotice(string.Format("Vòng Chung Kết giải Vua Gà sẽ đóng lại sau {0} phút nữa.", Math.Ceiling(spanTime.TotalMinutes)));
                            }
                        }
                    }
                    break;

                case EliteGameStatusType.END_2_CHAP:
                    {
                        EliteStatus = EliteGameStatusType.CLOSE;
                        UpdateServerEvent("EliteStatus", ((int)EliteStatus).ToString());
                        CreateChampionEliteGameRound();
                        var merged = m_elitePlayersG3.Merge(m_elitePlayersG4);
                        var playersList = merged.Where(a => a.Value.Status >= 1).
                                                                Take(200).Select(a => a.Value).ToList();

                        foreach (PlayerEliteGameInfo player in playersList)
                        {
                            switch (player.Status)
                            {
                                case 1://100
                                    if (player.GroupType == 1)
                                    {
                                        SendAward(player.UserID, FindEliteGameAwards(3000), LanguageMgr.GetTranslation("GameServer.EliteGame.Msg16"));
                                    }
                                    else
                                    {
                                        SendAward(player.UserID, FindEliteGameAwards(4000), LanguageMgr.GetTranslation("GameServer.EliteGame.Msg16"));
                                    }
                                    break;

                                case 2://16
                                    if (player.Winer == 0)
                                    {
                                        if (player.GroupType == 1)
                                            SendAward(player.UserID, FindEliteGameAwards(3001), LanguageMgr.GetTranslation("GameServer.EliteGame.Msg18"));
                                        else
                                            SendAward(player.UserID, FindEliteGameAwards(4001), LanguageMgr.GetTranslation("GameServer.EliteGame.Msg18"));
                                    }
                                    break;
                                case 3://8
                                    if (player.Winer == 0)
                                    {
                                        if (player.GroupType == 1)
                                            SendAward(player.UserID, FindEliteGameAwards(3002), LanguageMgr.GetTranslation("GameServer.EliteGame.Msg18"));
                                        else
                                            SendAward(player.UserID, FindEliteGameAwards(4002), LanguageMgr.GetTranslation("GameServer.EliteGame.Msg18"));
                                    }
                                    break;
                                case 4://4
                                    if (player.Winer == 0)
                                    {
                                        if (player.GroupType == 1)
                                            SendAward(player.UserID, FindEliteGameAwards(3003), LanguageMgr.GetTranslation("GameServer.EliteGame.Msg18"));
                                        else
                                            SendAward(player.UserID, FindEliteGameAwards(4003), LanguageMgr.GetTranslation("GameServer.EliteGame.Msg18"));
                                    }
                                    break;
                                case 5://2
                                       //if (player.Status == 5 && player.Winer == 1)
                                    if (player.Winer == 1)
                                    {
                                        if (player.GroupType == 1)
                                            SendAward(player.UserID, FindEliteGameAwards(3005), LanguageMgr.GetTranslation("GameServer.EliteGame.Msg17"));
                                        else
                                            SendAward(player.UserID, FindEliteGameAwards(4005), LanguageMgr.GetTranslation("GameServer.EliteGame.Msg17"));
                                    }
                                    else
                                    {
                                        if (player.GroupType == 1)
                                            SendAward(player.UserID, FindEliteGameAwards(3004), LanguageMgr.GetTranslation("GameServer.EliteGame.Msg18"));
                                        else
                                            SendAward(player.UserID, FindEliteGameAwards(4004), LanguageMgr.GetTranslation("GameServer.EliteGame.Msg18"));
                                    }
                                    break;
                            }
                        }
                    }
                    break;
            }
        }

        private static void SendAward(int userId, List<EliteGameAwardInfo> awards, string title)
        {
            if (awards == null)
            {
                Console.WriteLine("Not found award for #{0} [{1}]", userId, title);
                return;
            }
            using (PlayerBussiness pb = new PlayerBussiness())
            {
                List<ItemInfo> items = new List<ItemInfo>();
                foreach (EliteGameAwardInfo a in awards)
                {
                    ItemTemplateInfo temp = ItemMgr.FindItemTemplate(a.TemplateID);
                    if (temp == null)
                        continue;

                    ItemInfo item = ItemInfo.CreateFromTemplate(temp, 1, 106);
                    item.Count = a.Count;
                    item.ValidDate = a.Valid;
                    item.IsBinds = a.IsBind == 0 ? false : true;
                    if (a.GoldValidate != 0)
                    {
                        item.goldBeginTime = DateTime.Now;
                        item.goldValidDate = a.GoldValidate;
                    }
                    item.StrengthenLevel = a.StrengthenLevel;
                    item.AgilityCompose = a.Agility;
                    item.LuckCompose = a.Luck;
                    item.DefendCompose = a.Defend;
                    item.AttackCompose = a.Attack;
                    item.Hole1 = a.Hole1;
                    item.Hole2 = a.Hole2;
                    item.Hole3 = a.Hole3;
                    item.Hole4 = a.Hole4;
                    item.Hole5 = a.Hole5;
                    item.Hole5Exp = a.Hole5Exp;
                    item.Hole5Level = a.Hole5Level;
                    item.Hole6 = a.Hole6;
                    item.Hole6Exp = a.Hole6Exp;
                    item.Hole6Level = a.Hole6Level;
                    items.Add(item);
                }
                pb.SendItemsToMail(items, userId, GameServer.Instance.Configuration.AreaID, LanguageMgr.GetTranslation("GameServer.EliteGame.Msg19"), title);
            }

        }

        public static DateTime TimeWithToday(string date, DateTime now)
        {
            return TimeWithToday(Convert.ToDateTime(date), now);
        }

        public static DateTime TimeWithToday(DateTime date, DateTime now)
        {
            return new DateTime(now.Year, now.Month, now.Day, date.Hour, date.Minute, date.Second, date.Millisecond, now.Kind);
            //string dateTime = string.Format("{0}/{1}/{2} {3}:{4}:{5}.{6}", now.Month, now.Day, now.Year, date.Hour, date.Minute, now.Second, now.Millisecond);
            //return Convert.ToDateTime(dateTime);
        }

        public static bool SameDateTime(DateTime t1, DateTime t2)
        {
            DateTime v1 = new DateTime(t1.Year, t1.Month, t1.Day, t1.Hour, t1.Minute, 0);
            DateTime v2 = new DateTime(t2.Year, t2.Month, t2.Day, t2.Hour, t2.Minute, 0);

            if (v1.CompareTo(v2) == 0)
                return true;
            else
                return false;
        }
    }
}
