using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlDataProvider.Data
{
    public class CardInfo
    {
        public int ID { get; set; }
        public int SuitID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
