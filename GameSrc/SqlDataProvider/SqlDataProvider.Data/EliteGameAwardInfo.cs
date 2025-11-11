using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlDataProvider.Data
{
    public class EliteGameAwardInfo
    {
        public int ID { get; set; }
        public string Description { get; set; }
        public int TemplateID { get; set; }
        public int Count { get; set; }
        public int Valid { get; set; }
        public int IsBind { get; set; }
        public int Agility { get; set; }
        public int Attack { get; set; }
        public int Defend { get; set; }
        public int Luck { get; set; }
        public int GoldValidate { get; set; }
        public int Hole1 { get; set; }
        public int Hole2 { get; set; }
        public int Hole3 { get; set; }
        public int Hole4 { get; set; }
        public int Hole5 { get; set; }
        public int Hole5Exp { get; set; }
        public int Hole5Level { get; set; }
        public int Hole6 { get; set; }
        public int Hole6Exp { get; set; }
        public int Hole6Level { get; set; }
        public int StrengthenLevel { get; set; }
        public int Result { get; set; }
    }
}
