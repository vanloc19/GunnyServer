using System.Collections.Generic;

namespace SqlDataProvider.Data
{
    public class JampsChapterItemList
    {
        public int ID, Sort;
        public string Name, Describe, Reserved;
        public Dictionary<int, JampsPageItemList> paginas;
    }
}