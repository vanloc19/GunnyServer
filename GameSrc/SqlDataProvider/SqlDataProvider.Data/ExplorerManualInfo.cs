using System.Collections.Generic;
using System.Linq;

namespace SqlDataProvider.Data
{
    public class ExplorerManualInfo
    {
        private int _manualLevel,
            _progress,
            _maxProgress,
            _havePage,
            _conditionCount,
            _Agility,
            _Armor,
            _Attack,
            _Damage,
            _Defense,
            _HP,
            _Lucky,
            _MagicAttack,
            _MagicResistance,
            _Stamina,
            _jampsCurrency;

        private bool _isDirty;

        public List<PagesInfo> activesPage;
        public Dictionary<int, DebrisInfo> debris;

        public bool IsDirty
        {
            get => _isDirty;
            set => _isDirty = value;
        }

        [SqlField]
        public int manualLevel
        {
            get => _manualLevel;
            set
            {
                _isDirty = true;
                _manualLevel = value;
            }
        }

        [SqlField]
        public int progress
        {
            get => _progress;
            set
            {
                _isDirty = true;
                _progress = value;
            }
        }

        [SqlField]
        public int maxProgress
        {
            get => _maxProgress;
            set
            {
                _isDirty = true;
                _maxProgress = value;
            }
        }

        [SqlField]
        public int havePage
        {
            get => _havePage;
            set
            {
                _isDirty = true;
                _havePage = value;
            }
        }

        [SqlField]
        public int conditionCount
        {
            get => _conditionCount;
            set
            {
                _isDirty = true;
                _conditionCount = value;
            }
        }

        [SqlField]
        public int Agility
        {
            get => _Agility;
            set
            {
                _isDirty = true;
                _Agility = value;
            }
        }

        [SqlField]
        public int Armor
        {
            get => _Armor;
            set
            {
                _isDirty = true;
                _Armor = value;
            }
        }

        [SqlField]
        public int Attack
        {
            get => _Attack;
            set
            {
                _isDirty = true;
                _Attack = value;
            }
        }

        [SqlField]
        public int Damage
        {
            get => _Damage;
            set
            {
                _isDirty = true;
                _Damage = value;
            }
        }

        [SqlField]
        public int Defense
        {
            get => _Defense;
            set
            {
                _isDirty = true;
                _Defense = value;
            }
        }

        [SqlField]
        public int HP
        {
            get => _HP;
            set
            {
                _isDirty = true;
                _HP = value;
            }
        }

        [SqlField]
        public int Lucky
        {
            get => _Lucky;
            set
            {
                _isDirty = true;
                _Lucky = value;
            }
        }

        [SqlField]
        public int MagicAttack
        {
            get => _MagicAttack;
            set
            {
                _isDirty = true;
                _MagicAttack = value;
            }
        }

        [SqlField]
        public int MagicResistance
        {
            get => _MagicResistance;
            set
            {
                _isDirty = true;
                _MagicResistance = value;
            }
        }

        [SqlField]
        public int Stamina
        {
            get => _Stamina;
            set
            {
                _isDirty = true;
                _Stamina = value;
            }
        }

        [SqlField]
        public int jampsCurrency
        {
            get => _jampsCurrency;
            set
            {
                _isDirty = true;
                _jampsCurrency = value;
            }
        }

        public bool addPage(PagesInfo p)
        {
            if (activesPage.Where(o => o.pageID == p.pageID).Count() == 0)
            {
                activesPage.Add(p);
                return true;
            }

            return false;
        }

        public bool acivatePage(int id)
        {
            foreach (PagesInfo p in activesPage)
            {
                if (p.pageID == id)
                {
                    p.activate = true;
                    return true;
                }
            }

            return false;
        }

        public bool addDebris(DebrisInfo debris)
        {
            if (!this.debris.ContainsKey(debris.ID))
            {
                this.debris.Add(debris.ID, debris);
                return true;
            }

            return false;
        }
    }
}