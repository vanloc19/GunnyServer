using System.Collections.Generic;

namespace SqlDataProvider.Data
{
    public class JampsPageItemList
    {
        public int ID,
            Sort,
            DebrisCount,
            ChapterID,
            Activate_Agile,
            Activate_Armor,
            Activate_Attack,
            Activate_Damage,
            Activate_Defence,
            Activate_HP,
            Activate_Lucky,
            Activate_MagicAttack,
            Activate_MagicResistance,
            Activate_Stamina,
            ActivateCurrency,
            Collect_Agile,
            Collect_Armor,
            Collect_Attack,
            Collect_Damage,
            Collect_Defense,
            Collect_HP,
            Collect_Lucky,
            Collect_MagicAttack,
            Collect_MagicResistance,
            Collect_Stamina;

        public string Describe, Name, ImagePath;
        public Dictionary<int, JampsDebrisItemList> _fragmentos;
    }
}