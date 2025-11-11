using System.Collections.Generic;
using System.Reflection;
using Game.Server.GameObjects;
using log4net;
using SqlDataProvider.Data;

namespace Game.Server.GameUtils
{
	public class PlayerProperty
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		protected int m_loading;

		private Dictionary<string, Dictionary<string, int>> m_otherPlayerProperty;

		protected GamePlayer m_player;

		private Dictionary<string, Dictionary<string, int>> m_playerProperty;

		protected int m_totalArmor;

		protected int m_totalDamage;

		public Dictionary<string, Dictionary<string, int>> Current
		{
			get
			{
				return m_playerProperty;
			}
			set
			{
				m_playerProperty = value;
			}
		}

		public int Loading
		{
			get
			{
				return m_loading;
			}
			set
			{
				m_loading = value;
			}
		}

		public Dictionary<string, Dictionary<string, int>> OtherPlayerProperty
		{
			get
			{
				return m_playerProperty;
			}
			set
			{
				m_playerProperty = value;
			}
		}

		public GamePlayer Player => m_player;

		public int totalArmor
		{
			get
			{
				return m_totalArmor;
			}
			set
			{
				m_totalArmor = value;
			}
		}

		public int totalDamage
		{
			get
			{
				return m_totalDamage;
			}
			set
			{
				m_totalDamage = value;
			}
		}

		public PlayerProperty(GamePlayer player)
		{
			m_player = player;
			m_playerProperty = new Dictionary<string, Dictionary<string, int>>();
			m_otherPlayerProperty = new Dictionary<string, Dictionary<string, int>>();
			m_loading = 0;
			m_totalDamage = 0;
			m_totalArmor = 0;
			CreateProp(true, "Texp", 0, 0, 0, 0, 0);
			CreateProp(true, "Card", 0, 0, 0, 0, 0);
			CreateProp(true, "Pet", 0, 0, 0, 0, 0);
			CreateProp(true, "Suit", 0, 0, 0, 0, 0);
			CreateProp(true, "Gem", 0, 0, 0, 0, 0);
			CreateProp(true, "Bead", 0, 0, 0, 0, 0);
			CreateProp(true, "Avatar", 0, 0, 0, 0, 0);
			CreateProp(true, "MagicStone", 0, 0, 0, 0, 0);
			CreateProp(true, "Horse", 0, 0, 0, 0, 0);
			CreateProp(true, "HorsePicCherish", 0, 0, 0, 0, 0);
			CreateProp(true, "Enchant", 0, 0, 0, 0, 0);
			CreateProp(true, "Temple", 0, 0, 0, 0, 0);
			CreateProp(true, "PetAtlas", 0, 0, 0, 0, 0);
			CreateProp(true, "Mark", 0, 0, 0, 0, 0);
			CreateProp(true, "SeloPet", 0, 0, 0, 0, 0);
			CreateProp(true, "CardAchievement", 0, 0, 0, 0, 0);
			CreateBaseProp(true);

			CreateProp(false, "Texp", 0, 0, 0, 0, 0);
			CreateProp(false, "Card", 0, 0, 0, 0, 0);
			CreateProp(false, "Pet", 0, 0, 0, 0, 0);
			CreateProp(false, "Suit", 0, 0, 0, 0, 0);
			CreateProp(false, "Gem", 0, 0, 0, 0, 0);
			CreateProp(false, "Bead", 0, 0, 0, 0, 0);
			CreateProp(false, "Avatar", 0, 0, 0, 0, 0);
			CreateProp(false, "MagicStone", 0, 0, 0, 0, 0);
			CreateProp(false, "Horse", 0, 0, 0, 0, 0);
			CreateProp(false, "HorsePicCherish", 0, 0, 0, 0, 0);
			CreateProp(false, "Enchant", 0, 0, 0, 0, 0);
			CreateProp(false, "Temple", 0, 0, 0, 0, 0);
			CreateProp(false, "PetAtlas", 0, 0, 0, 0, 0);
			CreateProp(false, "Mark", 0, 0, 0, 0, 0);
			CreateProp(false, "SeloPet", 0, 0, 0, 0, 0);
			CreateProp(false, "CardAchievement", 0, 0, 0, 0, 0);
			CreateBaseProp(false);
		}

		public void AddOtherProp(string key, Dictionary<string, int> propAdd)
		{
			if (!m_playerProperty.ContainsKey(key))
			{
				m_otherPlayerProperty.Add(key, propAdd);
			}
			else
			{
				m_otherPlayerProperty[key] = propAdd;
			}
		}

		public void AddProp(string key, Dictionary<string, int> propAdd)
		{
			if (!m_playerProperty.ContainsKey(key))
			{
				m_playerProperty.Add(key, propAdd);
			}
			else
			{
				m_playerProperty[key] = propAdd;
			}
		}

		public void CreateBaseProp(bool isSelf)
		{
			Dictionary<string, int> magicAttack = new Dictionary<string, int>
			{
				{"MagicStone", 0},
				{"Horse", 0},
				{"HorsePicCherish", 0},
				{"Enchant", 0},
				{"Suit", 0},
				{"Texp", 0},
				{"Card", 0},
				{"Mark", 0},
				{"SeloPet", 0},
				{"CardAchievement", 0}
			};

			Dictionary<string, int> magicDefence = new Dictionary<string, int>
			{
				{"MagicStone", 0},
				{"Horse", 0},
				{"HorsePicCherish", 0},
				{"Enchant", 0},
				{"Suit", 0},
				{"Texp", 0},
				{"Temple", 0},
				{"Card", 0},
				{"Mark", 0},
				{"SeloPet", 0},
				{"CardAchievement", 0}
			};

			Dictionary<string, int> damage = new Dictionary<string, int>
			{
				{"Bead", 0},
				{"Suit", 0},
				{"Texp", 0},
				{"Avatar", 0},
				{"Horse", 0},
				{"HorsePicCherish", 0},
				{"Mark", 0},
				{"SeloPet", 0},
				{"CardAchievement", 0}
			};

			Dictionary<string, int> armor = new Dictionary<string, int>
			{
				{"Bead", 0},
				{"Suit", 0},
				{"Texp", 0},
				{"Avatar", 0},
				{"Horse", 0},
				{"HorsePicCherish", 0},
				{"Temple", 0},
				{"Pet", 0},
				{"Mark", 0},
				{"SeloPet", 0},
				{"CardAchievement", 0}
			};

			#region ??????
			var fire = new Dictionary<string, int>
			{
				{"Emblem", 0},
				{"Elf", 0},
			};
			var water = new Dictionary<string, int>
			{
				{"Emblem", 0},
				{"Elf", 0},
			};
			var wind = new Dictionary<string, int>
			{
				{"Emblem", 0},
				{"Elf", 0},
			};
			var land = new Dictionary<string, int>
			{
				{"Emblem", 0},
				{"Elf", 0},
			};
			var fireResistance = new Dictionary<string, int>
			{
				{"Emblem", 0},
				{"Elf", 0},
			};
			var waterResistance = new Dictionary<string, int>
			{
				{"Emblem", 0},
				{"Elf", 0},
			};
			var windResistance = new Dictionary<string, int>
			{
				{"Emblem", 0},
				{"Elf", 0},
			};
			var landResistance = new Dictionary<string, int>
			{
				{"Emblem", 0},
				{"Elf", 0},
			};
			#endregion


			if (isSelf)
			{
				AddProp("Damage", damage);
				AddProp("Armor", armor);
				AddProp("MagicAttack", magicAttack);
				AddProp("MagicDefence", magicDefence);

				AddProp("Fire", fire);
				AddProp("Water", water);
				AddProp("Wind", wind);
				AddProp("Land", land);

				AddProp("FireResistance", fireResistance);
				AddProp("WaterResistance", waterResistance);
				AddProp("WindResistance", windResistance);
				AddProp("LandResistance", landResistance);
			}
			else
			{
				AddOtherProp("Damage", damage);
				AddOtherProp("Armor", armor);
				AddOtherProp("MagicAttack", magicAttack);
				AddOtherProp("MagicDefence", magicDefence);

				AddOtherProp("Fire", fire);
				AddOtherProp("Water", water);
				AddOtherProp("Wind", wind);
				AddOtherProp("Land", land);

				AddOtherProp("FireResistance", fireResistance);
				AddOtherProp("WaterResistance", waterResistance);
				AddOtherProp("WindResistance", windResistance);
				AddOtherProp("LandResistance", landResistance);
			}
		}

        #region
        /*public void CreateBaseProp(bool isSelf, double beaddefence, double beadattack, double suitdefence, double suitattack, double avatarattack, double avatardefence, double magicstoneattack, double magicstonedefence)
		{
			Dictionary<string, int> propAdd = new Dictionary<string, int>();
			propAdd.Add("Bead", (int)beadattack);
			propAdd.Add("Suit", (int)suitattack);
			propAdd.Add("Avatar", (int)avatarattack);
			Dictionary<string, int> dictionary2 = new Dictionary<string, int>();
			dictionary2.Add("Bead", (int)beaddefence);
			dictionary2.Add("Suit", (int)suitdefence);
			dictionary2.Add("Avatar", (int)avatardefence);
			Dictionary<string, int> dictionary3 = new Dictionary<string, int>();
			dictionary3.Add("MagicStone", (int)magicstoneattack);
			Dictionary<string, int> dictionary4 = new Dictionary<string, int>();
			dictionary4.Add("MagicStone", (int)magicstonedefence);
			if (isSelf)
			{
				AddProp("Damage", propAdd);
				AddProp("Armor", dictionary2);
				AddProp("MagicAttack", dictionary3);
				AddProp("MagicDefence", dictionary4);
			}
			else
			{
				AddOtherProp("Damage", propAdd);
				AddOtherProp("Armor", dictionary2);
				AddOtherProp("MagicAttack", dictionary3);
				AddOtherProp("MagicDefence", dictionary4);
			}
		}*/
        #endregion

        public void CreateProp(bool isSelf, string skey, int attack, int defence, int agility, int lucky, int hp)
		{
			Dictionary<string, int> propAdd = new Dictionary<string, int>();
			propAdd.Add("Attack", attack);
			propAdd.Add("Defence", defence);
			propAdd.Add("Agility", agility);
			propAdd.Add("Luck", lucky);
			propAdd.Add("HP", hp);
			if (isSelf)
			{
				AddProp(skey, propAdd);
			}
			else
			{
				AddOtherProp(skey, propAdd);
			}
		}

		public void UpadateBaseProp(bool isSelf, string mainKey, string subKey, double value)
		{
			if (isSelf)
			{
				if (m_playerProperty.ContainsKey(mainKey) && m_playerProperty[mainKey].ContainsKey(subKey))
				{
					m_playerProperty[mainKey][subKey] = (int)value;
				}
			}
			else if (m_otherPlayerProperty.ContainsKey(mainKey) && m_otherPlayerProperty[mainKey].ContainsKey(subKey))
			{
				m_otherPlayerProperty[mainKey][subKey] = (int)value;
			}
		}

		public void ViewCurrent()
		{
			if (m_player.ShowPP)
			{
				m_player.Out.SendUpdatePlayerProperty(m_player.PlayerCharacter, this);
			}
		}

		public void ViewOther(PlayerInfo player)
		{
			m_player.Out.SendUpdatePlayerProperty(player, this);
		}
	}
}
