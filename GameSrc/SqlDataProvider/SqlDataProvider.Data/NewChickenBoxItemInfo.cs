using System;

namespace SqlDataProvider.Data
{
	public class NewChickenBoxItemInfo : DataObject
	{
		private int _ID;

		private int _userID;

		private int _templateID;

		private int _count;

		private int _validDate;

		private int _strengthenLevel;

		private int _attackCompose;

		private int _defendCompose;

		private int _agilityCompose;

		private int _luckCompose;

		private int _position;

		private bool _isSelected;

		private bool _isSeeded;

		private bool _isBinds;

		private int _quality;

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

		public int TemplateID
		{
			get
			{
				return this._templateID;
			}
			set
			{
				this._templateID = value;
				this._isDirty = true;
			}
		}

		public int Count
		{
			get
			{
				return this._count;
			}
			set
			{
				this._count = value;
				this._isDirty = true;
			}
		}

		public int ValidDate
		{
			get
			{
				return this._validDate;
			}
			set
			{
				this._validDate = value;
				this._isDirty = true;
			}
		}

		public int StrengthenLevel
		{
			get
			{
				return this._strengthenLevel;
			}
			set
			{
				this._strengthenLevel = value;
				this._isDirty = true;
			}
		}

		public int AttackCompose
		{
			get
			{
				return this._attackCompose;
			}
			set
			{
				this._attackCompose = value;
				this._isDirty = true;
			}
		}

		public int DefendCompose
		{
			get
			{
				return this._defendCompose;
			}
			set
			{
				this._defendCompose = value;
				this._isDirty = true;
			}
		}

		public int AgilityCompose
		{
			get
			{
				return this._agilityCompose;
			}
			set
			{
				this._agilityCompose = value;
				this._isDirty = true;
			}
		}

		public int LuckCompose
		{
			get
			{
				return this._luckCompose;
			}
			set
			{
				this._luckCompose = value;
				this._isDirty = true;
			}
		}

		public int Position
		{
			get
			{
				return this._position;
			}
			set
			{
				this._position = value;
				this._isDirty = true;
			}
		}

		public bool IsSelected
		{
			get
			{
				return this._isSelected;
			}
			set
			{
				this._isSelected = value;
				this._isDirty = true;
			}
		}

		public bool IsSeeded
		{
			get
			{
				return this._isSeeded;
			}
			set
			{
				this._isSeeded = value;
				this._isDirty = true;
			}
		}

		public bool IsBinds
		{
			get
			{
				return this._isBinds;
			}
			set
			{
				this._isBinds = value;
				this._isDirty = true;
			}
		}

		public int Quality
		{
			get
			{
				return this._quality;
			}
			set
			{
				this._quality = value;
				this._isDirty = true;
			}
		}
	}
}
