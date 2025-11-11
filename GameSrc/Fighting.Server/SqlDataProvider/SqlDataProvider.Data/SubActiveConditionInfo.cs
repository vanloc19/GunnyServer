using System.Collections.Generic;

namespace SqlDataProvider.Data
{
	public class SubActiveConditionInfo
	{
		public int ActiveID
		{
			get;
			set;
		}

		public int AwardType
		{
			get;
			set;
		}

		public string AwardValue
		{
			get;
			set;
		}

		public int ConditionID
		{
			get;
			set;
		}

		public int ID
		{
			get;
			set;
		}

		public bool IsValid
		{
			get;
			set;
		}

		public int SubID
		{
			get;
			set;
		}

		public int Type
		{
			get;
			set;
		}

		public string Value
		{
			get;
			set;
		}

		public int GetValue(string index)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			int result;
			if (!string.IsNullOrEmpty(Value))
			{
				string[] array = Value.Split('-');
				for (int i = 1; i < array.Length; i += 2)
				{
					string key = array[i - 1];
					if (!dictionary.ContainsKey(key))
					{
						dictionary.Add(key, array[i]);
					}
					else
					{
						dictionary[key] = array[i];
					}
				}
				if (dictionary.ContainsKey(index))
				{
					result = int.Parse(dictionary[index]);
					return result;
				}
			}
			result = 0;
			return result;
		}
	}
}
