using System;

namespace SqlDataProvider.Data
{
	public class ActiveSystemInfo : DataObject
	{
		private int _ID;

		private int _userID;

		private int _canEagleEyeCounts;

		private int _canOpenCounts;

		private bool _isShowAll;

		private DateTime _lastFlushTime;

		private int _cardScore;

		private int _cardChipCount;

		private int _cardFreeCount;

		private string _cardListCard;

		private string _cardListAward;

		private string _cardListExchange;

		private int _SXCrystal;

		private int _SXStepRemain;

		private int _SXScore;

		private string _SXMapInfoData;

		private string _MiniShopBuyCount;

		private string _SXRewardsGet;

		private string _chickActiveData;

		public int ID
		{
			get
			{
				return this._ID;
			}
			set
			{
				this._ID = value;
				this._isDirty = true;
			}
		}

		public int UserID
		{
			get
			{
				return this._userID;
			}
			set
			{
				this._userID = value;
				this._isDirty = true;
			}
		}

		public int canEagleEyeCounts
		{
			get
			{
				return this._canEagleEyeCounts;
			}
			set
			{
				this._canEagleEyeCounts = value;
				this._isDirty = true;
			}
		}

		public int canOpenCounts
		{
			get
			{
				return this._canOpenCounts;
			}
			set
			{
				this._canOpenCounts = value;
				this._isDirty = true;
			}
		}

		public bool isShowAll
		{
			get
			{
				return this._isShowAll;
			}
			set
			{
				this._isShowAll = value;
				this._isDirty = true;
			}
		}

		public DateTime lastFlushTime
		{
			get
			{
				return this._lastFlushTime;
			}
			set
			{
				this._lastFlushTime = value;
				this._isDirty = true;
			}
		}

		public int cardScore
		{
			get { return _cardScore; }
			set
			{
				_cardScore = value;
				_isDirty = true;
			}
		}

		public int cardChipCount
		{
			get { return _cardChipCount; }
			set
			{
				_cardChipCount = value;
				_isDirty = true;
			}
		}

		public int cardFreeCount
		{
			get { return _cardFreeCount; }
			set
			{
				_cardFreeCount = value;
				_isDirty = true;
			}
		}

		public string cardListCard
		{
			get { return _cardListCard; }
			set
			{
				_cardListCard = value;
				_isDirty = true;
			}
		}

		public string cardListAward
		{
			get { return _cardListAward; }
			set
			{
				_cardListAward = value;
				_isDirty = true;
			}
		}

		public string cardListExchange
		{
			get { return _cardListExchange; }
			set
			{
				_cardListExchange = value;
				_isDirty = true;
			}
		}

		public int SXCrystal
		{
			get { return _SXCrystal; }
			set
			{
				_SXCrystal = value;
				_isDirty = true;
			}
		}

		public int SXStepRemain
		{
			get { return _SXStepRemain; }
			set
			{
				_SXStepRemain = value;
				_isDirty = true;
			}
		}

		public int SXScore
		{
			get { return _SXScore; }
			set
			{
				_SXScore = value;
				_isDirty = true;
			}
		}

		public string SXMapInfoData
		{
			get { return _SXMapInfoData; }
			set
			{
				_SXMapInfoData = value;
				_isDirty = true;
			}
		}

		public string MiniShopBuyCount
		{
			get { return _MiniShopBuyCount; }
			set
			{
				_MiniShopBuyCount = value;
				_isDirty = true;
			}
		}

		public string SXRewardsGet
		{
			get { return _SXRewardsGet; }
			set
			{
				_SXRewardsGet = value;
				_isDirty = true;
			}
		}

		private string _CryptBoss;

		public string CryptBoss
		{
			get { return _CryptBoss; }
			set
			{
				_CryptBoss = value;
				_isDirty = true;
			}
		}
		#region CatchBeats
		private int _challengeNum;
		private int _buyBuffNum;
		private int _damageNum;
		private string _boxState;
		private DateTime _lastEnterYearMonter;
		public int ChallengeNum
		{
			get { return _challengeNum; }
			set
			{
				_challengeNum = value;
				_isDirty = true;
			}
		}

		public int BuyBuffNum
		{
			get { return _buyBuffNum; }
			set
			{
				_buyBuffNum = value;
				_isDirty = true;
			}
		}

		public int DamageNum
		{
			get { return _damageNum; }
			set
			{
				_damageNum = value;
				_isDirty = true;
			}
		}

		public string BoxState
		{
			get { return _boxState; }
			set
			{
				_boxState = value;
				_isDirty = true;
			}
		}

		public DateTime lastEnterYearMonter
		{
			get { return _lastEnterYearMonter; }
			set
			{
				_lastEnterYearMonter = value;
				_isDirty = true;
			}
		}
		#endregion

		#region ChickActivation
		public string ChickActiveData
		{
			get { return _chickActiveData; }
			set
			{
				_chickActiveData = value;
				_isDirty = true;
			}
		}
		#endregion
	}
}
