namespace SqlDataProvider.Data
{
	public class BallInfo
	{
		public int ActionType
		{
			get;
			set;
		}

		public int Amount
		{
			get;
			set;
		}

		public int AttackResponse
		{
			get;
			set;
		}

		public string BombPartical
		{
			get;
			set;
		}

		public string BombSound
		{
			get;
			set;
		}

		public string Crater
		{
			get;
			set;
		}

		public int Delay
		{
			get;
			set;
		}

		public int DragIndex
		{
			get;
			set;
		}

		public string FlyingPartical
		{
			get;
			set;
		}

		public bool HasTunnel
		{
			get;
			set;
		}

		public int ID
		{
			get;
			set;
		}

		public bool IsSpin
		{
			get;
			set;
		}

		public int Mass
		{
			get;
			set;
		}

		public string Name
		{
			get;
			set;
		}

		public double Power
		{
			get;
			set;
		}

		public int Radii
		{
			get;
			set;
		}

		public bool Shake
		{
			get;
			set;
		}

		public string ShootSound
		{
			get;
			set;
		}

		public int SpinV
		{
			get;
			set;
		}

		public double SpinVA
		{
			get;
			set;
		}

		public int Weight
		{
			get;
			set;
		}

		public int Wind
		{
			get;
			set;
		}

		public bool IsSpecial()
		{
			switch (ID)
            {
				case 1:
				case 3:
				case 5:
				case 16:
				case 64:
				case 59:
				case 97:
				case 98:
				case 110:
				case 117:
				case 10001:
				case 10002:
				case 10003:
				case 10004:
				case 10005:
				case 10006:
				case 10007:
				case 10008:
				case 10009:
				case 10010:
				case 10011:
				case 10012:
				case 10013:
				case 10014:
				case 10015:
				case 10016:
				case 10017:
				case 10018:
				case 10019:
				case 10020:
				case 10021:
				case 10022:
				case 10023:
					return true;
				default:
					return false;
			}				
            #region OLD Bomb
            //int d = ID;
            //if (d <= 64)
            //{
            //	if (d > 16)
            //	{
            //		if (d == 59 || d == 64)
            //		{
            //			return true;
            //		}
            //		return false;
            //	}
            //	switch (d)
            //	{
            //	case 2:
            //	case 4:
            //		return false;
            //	default:
            //		return false;
            //	case 1:
            //	case 3:
            //	case 5:
            //	case 16:
            //		break;
            //	}
            //}
            //else
            //{
            //	if (d <= 98)
            //	{
            //		if (d == 97 || d == 98)
            //		{
            //			return true;
            //		}
            //		return false;
            //	}
            //	if (d != 110 && d != 117 && (uint)(d - 10001) > 22u)
            //	{
            //		return false;
            //	}
            //}
            //return true;
            #endregion
        }
    }
}
