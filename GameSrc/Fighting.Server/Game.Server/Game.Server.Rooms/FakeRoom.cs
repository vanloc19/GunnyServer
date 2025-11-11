using Game.Logic;
using Lsj.Util.Text;
using SqlDataProvider.Data;
using System;

namespace Game.Server.Rooms
{
    public class FakeRoom : BaseRoom
    {
        private static Random random = new Random(Environment.TickCount);

        private bool isPlaying = false;

        private PveInfo pveinfo;

        public override bool IsPlaying
        {
            get
            {
                return isPlaying;
            }
        }
        public FakeRoom(int roomId, string roomName, int playerCount, int maxPlayerCount, int roomType) : base(roomId)
        {
            this.m_playerCount = playerCount;
            this.m_placesCount = maxPlayerCount;
            this.Name = roomName;
            switch (roomType)
            {
                case 1:
                    this.RoomType = eRoomType.Match;
                    break;
                case 2:
                    this.RoomType = eRoomType.Match;
                    this.Password = "12dasS12dasda44C@tchBe@st";
                    this.isPlaying = true;
                    break;
                case 3://Kien De
                    this.RoomType = eRoomType.Dungeon;
                    this.MapId = 2;
                    this.HardLevel = eHardLevel.Easy;
                    break;
                case 4://Kien Thuong
                    this.RoomType = eRoomType.Dungeon;
                    this.MapId = 2;
                    this.HardLevel = eHardLevel.Easy;
                    break;
                case 5://Kien De
                    this.RoomType = eRoomType.Dungeon;
                    this.isPlaying = true;
                    this.MapId = 2;
                    this.HardLevel = eHardLevel.Easy;
                    break;
                case 6://Kien Thuong
                    this.RoomType = eRoomType.Dungeon;
                    this.isPlaying = true;
                    this.MapId = 2;
                    this.HardLevel = eHardLevel.Easy;
                    break;
            }
        }

        private eHardLevel GetRandomHardLevel()
        {
            while (true)
            {
                var result = random.Next(0, 3);
                if (result == 0 && !this.pveinfo.SimpleGameScript.IsNullOrEmpty())
                {
                    return eHardLevel.Simple;
                }
                else if (result == 1 && !this.pveinfo.NormalGameScript.IsNullOrEmpty())
                {
                    return eHardLevel.Normal;
                }
                else if (result == 2 && !this.pveinfo.HardGameScript.IsNullOrEmpty())
                {
                    return eHardLevel.Hard;
                }
                else if (result == 3 && !this.pveinfo.TerrorGameScript.IsNullOrEmpty())
                {
                    return eHardLevel.Terror;
                }
            }
        }

        protected override void Reset()
        {
        }

        public override void Stop()
        {
        }
    }
}
