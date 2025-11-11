using System.Collections.Generic;

namespace SqlDataProvider.Data
{
    public class EventAwardInfo
    {
        public int ID { get; set; }
        public int ActivityType { get; set; }
        public int TemplateID { get; set; }
        public int Count { get; set; }
        public int ValidDate { get; set; }
        public bool IsBinds { get; set; }
        public int StrengthenLevel { get; set; }
        public int AttackCompose { get; set; }
        public int DefendCompose { get; set; }
        public int AgilityCompose { get; set; }
        public int LuckCompose { get; set; }
        public int Random { get; set; }
        public int SelectIndex { get; set; }
        public bool IsSelect { get; set; }

        public int Position { get; set; }

    }
}