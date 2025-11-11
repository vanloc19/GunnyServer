namespace SqlDataProvider.Data
{
	public class PveInfo
	{
		public string AdviceTips
		{
			get;
			set;
		}

		public string BossFightNeedMoney
		{
			get;
			set;
		}

		public string FightPower
		{
			get;
			set;
        }

		public string Description
		{
			get;
			set;
		}

		public string EpicGameScript
		{
			get;
			set;
		}

		public string EpicTemplateIds
		{
			get;
			set;
		}

		public string HardGameScript
		{
			get;
			set;
		}

		public string HardTemplateIds
		{
			get;
			set;
		}

		public int ID
		{
			get;
			set;
		}

		public int LevelLimits
		{
			get;
			set;
		}

		public string Name
		{
			get;
			set;
		}

		public string NormalGameScript
		{
			get;
			set;
		}

		public string NormalTemplateIds
		{
			get;
			set;
		}

		public int Ordering
		{
			get;
			set;
		}

		public string Pic
		{
			get;
			set;
		}

		public string SimpleGameScript
		{
			get;
			set;
		}

		public string SimpleTemplateIds
		{
			get;
			set;
		}

		public string TerrorGameScript
		{
			get;
			set;
		}

		public string TerrorTemplateIds
		{
			get;
			set;
		}

		public string NightmareGameScript
        {
			get;
			set;
        }

		public string NightmareTemplateIds
        {
			get;
			set;
        }

		public int Type
		{
			get;
			set;
		}

		public int getPrice(int selectedLevel)
		{
			int _loc_2 = 1;
			string _loc_3 = BossFightNeedMoney;
			string[] _loc_1 = _loc_3.Split('|');
			if (_loc_1.Length > 0)
            {
				switch (selectedLevel)
                {
					case 0:
						{
							_loc_2 = int.Parse(_loc_1[0]);
							break;
						}
					case 1:
                        {
							_loc_2 = int.Parse(_loc_1[1]);
							break;
                        }
					case 2:
                        {
							_loc_2 = int.Parse(_loc_1[2]);
							break;
                        }
					case 3:
                        {
							_loc_2 = int.Parse(_loc_1[3]);
							break;
                        }
					case 4:
                        {
							_loc_2 = int.Parse(_loc_1[4]);
							break;
                        }
                }
            }
			return _loc_2;
		}
	}
}
