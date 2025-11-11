using System.Drawing;

namespace Game.Server.Rooms
{
    public class MonterInfo
    {
        public int ID { set; get; }
        public int state { set; get; }
        public Point MonsterPos { set; get; }
        public Point MonsterNewPos { set; get; }
        public int type { set; get; }
        public int PlayerID { set; get; }
    }
}