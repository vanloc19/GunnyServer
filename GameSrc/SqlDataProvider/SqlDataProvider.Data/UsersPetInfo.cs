using System;
using System.Collections.Generic;

namespace SqlDataProvider.Data
{
	public class UsersPetInfo : DataObject
	{
		private string string_0;

		private string string_1;

		private int int_0;

		private int int_1;

		private int int_2;

		private string string_2;

		private int int_3;

		private int int_4;

		private int int_5;

		private int int_6;

		private int int_7;

		private int int_8;

		private int int_9;

		private int int_10;

		private int int_11;

		private int int_12;

		private int int_13;

		private int int_14;

		private int int_15;

		private int int_16;

		private int int_17;

		private int int_18;

		private int int_19;

		private int int_20;

		private int int_21;

		private int int_22;

		private bool bool_0;

		private int int_23;

		private int int_24;

		private bool bool_1;

		private int int_25;

		private int int_26;

		private int int_27;

		private int int_28;

		private int int_29;

		private int int_30;

		private List<PetEquipInfo> list_0;

		private string string_3;

		private string string_4;

		private int int_31;

		private int _vipLv;

		public string SkillEquip
		{
			get
			{
				return string_0;
			}
			set
			{
				string_0 = value;
				_isDirty = true;
			}
		}

		public string Skill
		{
			get
			{
				return string_1;
			}
			set
			{
				string_1 = value;
				_isDirty = true;
			}
		}

		public int ID
		{
			get
			{
				return int_0;
			}
			set
			{
				int_0 = value;
				_isDirty = true;
			}
		}

		public int VIPLevel
        {
            get
            {
				return _vipLv;
            }
			set
            {
				_vipLv = value;
				_isDirty = true;
            }
        }

		public int PetID
		{
			get
			{
				return int_1;
			}
			set
			{
				int_1 = value;
				_isDirty = true;
			}
		}

		public int TemplateID
		{
			get
			{
				return int_2;
			}
			set
			{
				int_2 = value;
				_isDirty = true;
			}
		}

		public string Name
		{
			get
			{
				return string_2;
			}
			set
			{
				string_2 = value;
				_isDirty = true;
			}
		}

		public int UserID
		{
			get
			{
				return int_3;
			}
			set
			{
				int_3 = value;
				_isDirty = true;
			}
		}

		public int Attack
		{
			get
			{
				return int_4 - method_1(int_4);
			}
			set
			{
				int_4 = value;
				_isDirty = true;
			}
		}

		public int Defence
		{
			get
			{
				return int_5 - method_1(int_5);
			}
			set
			{
				int_5 = value;
				_isDirty = true;
			}
		}

		public int Luck
		{
			get
			{
				return int_6 - method_1(int_6);
			}
			set
			{
				int_6 = value;
				_isDirty = true;
			}
		}

		public int Agility
		{
			get
			{
				return int_7 - method_1(int_7);
			}
			set
			{
				int_7 = value;
				_isDirty = true;
			}
		}

		public int Blood
		{
			get
			{
				return int_8 - method_1(int_8);
			}
			set
			{
				int_8 = value;
				_isDirty = true;
			}
		}

		public int Damage
		{
			get
			{
				return int_9 - method_1(int_9);
			}
			set
			{
				int_9 = value;
				_isDirty = true;
			}
		}

		public int Guard
		{
			get
			{
				return int_10 - method_1(int_10);
			}
			set
			{
				int_10 = value;
				_isDirty = true;
			}
		}

		public int AttackGrow
		{
			get
			{
				return int_11;
			}
			set
			{
				int_11 = value;
				_isDirty = true;
			}
		}

		public int DefenceGrow
		{
			get
			{
				return int_12;
			}
			set
			{
				int_12 = value;
				_isDirty = true;
			}
		}

		public int LuckGrow
		{
			get
			{
				return int_13;
			}
			set
			{
				int_13 = value;
				_isDirty = true;
			}
		}

		public int AgilityGrow
		{
			get
			{
				return int_14;
			}
			set
			{
				int_14 = value;
				_isDirty = true;
			}
		}

		public int BloodGrow
		{
			get
			{
				return int_15;
			}
			set
			{
				int_15 = value;
				_isDirty = true;
			}
		}

		public int DamageGrow
		{
			get
			{
				return int_16;
			}
			set
			{
				int_16 = value;
				_isDirty = true;
			}
		}

		public int GuardGrow
		{
			get
			{
				return int_17;
			}
			set
			{
				int_17 = value;
				_isDirty = true;
			}
		}

		public int Level
		{
			get
			{
				return int_18;
			}
			set
			{
				int_18 = value;
				_isDirty = true;
			}
		}

		public int GP
		{
			get
			{
				return int_19;
			}
			set
			{
				int_19 = value;
				_isDirty = true;
			}
		}

		public int MaxGP
		{
			get
			{
				return int_20;
			}
			set
			{
				int_20 = value;
				_isDirty = true;
			}
		}

		public int Hunger
		{
			get
			{
				return int_21;
			}
			set
			{
				int_21 = value;
				_isDirty = true;
			}
		}

		public int PetHappyStar => method_0();

		public int MP
		{
			get
			{
				return int_22;
			}
			set
			{
				int_22 = value;
				_isDirty = true;
			}
		}

		public bool IsEquip
		{
			get
			{
				return bool_0;
			}
			set
			{
				bool_0 = value;
				_isDirty = true;
			}
		}

		public int Place
		{
			get
			{
				return int_23;
			}
			set
			{
				int_23 = value;
				_isDirty = true;
			}
		}

		public int currentStarExp
		{
			get
			{
				return int_24;
			}
			set
			{
				int_24 = value;
				_isDirty = true;
			}
		}

		public bool IsExit
		{
			get
			{
				return bool_1;
			}
			set
			{
				bool_1 = value;
				_isDirty = true;
			}
		}

		public int breakGrade
		{
			get
			{
				return int_25;
			}
			set
			{
				int_25 = value;
				_isDirty = true;
			}
		}

		public int breakAttack
		{
			get
			{
				return int_26;
			}
			set
			{
				int_26 = value;
				_isDirty = true;
			}
		}

		public int breakDefence
		{
			get
			{
				return int_27;
			}
			set
			{
				int_27 = value;
				_isDirty = true;
			}
		}

		public int breakAgility
		{
			get
			{
				return int_28;
			}
			set
			{
				int_28 = value;
				_isDirty = true;
			}
		}

		public int breakLuck
		{
			get
			{
				return int_29;
			}
			set
			{
				int_29 = value;
				_isDirty = true;
			}
		}

		public int breakBlood
		{
			get
			{
				return int_30;
			}
			set
			{
				int_30 = value;
				_isDirty = true;
			}
		}

		public List<PetEquipInfo> PetEquips
		{
			get
			{
				return list_0;
			}
			set
			{
				list_0 = value;
			}
		}

		public string eQPets
		{
			get
			{
				return string_3;
			}
			set
			{
				string_3 = value;
				_isDirty = true;
			}
		}

		public string BaseProp
		{
			get
			{
				return string_4;
			}
			set
			{
				string_4 = value;
				_isDirty = true;
			}
		}

		public int TotalAttack
		{
			get
			{
				int_31 = 0;
				foreach (PetEquipInfo petEquip in PetEquips)
				{
					if (petEquip.IsValidItem() && petEquip.Template != null)
					{
						int_31 += petEquip.Template.Attack;
					}
				}
				return Attack + int_31 + int_26;
			}
		}

		public int TotalDefence
		{
			get
			{
				int_31 = 0;
				foreach (PetEquipInfo petEquip in PetEquips)
				{
					if (petEquip.IsValidItem() && petEquip.Template != null)
					{
						int_31 += petEquip.Template.Defence;
					}
				}
				return Defence + int_31 + int_27;
			}
		}

		public int TotalLuck
		{
			get
			{
				int_31 = 0;
				foreach (PetEquipInfo petEquip in PetEquips)
				{
					if (petEquip.IsValidItem() && petEquip.Template != null)
					{
						int_31 += petEquip.Template.Luck;
					}
				}
				return Luck + int_31 + breakLuck;
			}
		}

		public int TotalAgility
		{
			get
			{
				int_31 = 0;
				foreach (PetEquipInfo petEquip in PetEquips)
				{
					if (petEquip.IsValidItem() && petEquip.Template != null)
					{
						int_31 += petEquip.Template.Agility;
					}
				}
				return Agility + int_31 + int_28;
			}
		}

		public int TotalBlood => Blood + int_30;

		public int TotalDamage => Damage;

		public int TotalGuard => Guard;

		public List<string> GetSkill()
		{
			List<string> stringList = new List<string>();
			string string1 = string_1;
			char[] chArray = new char[1]
			{
				'|'
			};
			string[] array = string1.Split(chArray);
			foreach (string str in array)
			{
				stringList.Add(str);
			}
			return stringList;
		}

		public string[] GetSkillEquip()
		{
			string[] activeSlot = new string[] { "0,0", "-1,1", "-1,2", "-1,3", "-1,4" };

			if (Level >= 20 && Level < 30)
			{
				activeSlot[1] = "0,1";
			}

			if (Level >= 30 && Level < 50)
			{
				activeSlot[1] = "0,1";
				activeSlot[2] = "0,2";
			}

			if (Level >= 50)
			{
				activeSlot[1] = "0,1";
				activeSlot[2] = "0,2";
				activeSlot[3] = "0,3";
			}

			if (VIPLevel >= 7)
			{
				activeSlot[4] = "0,4";
			}

			string[] oldEquipSkill = SkillEquip.Split('|');
			List<string> skillIds = new List<string>();
			string[] skillArray = Skill.Split('|');
			for (int i = 0; i < skillArray.Length; i++)
			{
				skillIds.Add(skillArray[i].Split(',')[0]);
			}

			for (int i = 0; i < activeSlot.Length; i++)
			{
				if (i < oldEquipSkill.Length && skillIds.Contains(oldEquipSkill[i].Split(',')[0]))
				{
					activeSlot[i] = oldEquipSkill[i];
				}
			}

			string_0 = activeSlot[0];
			for (int s = 1; s < activeSlot.Length; s++)
			{
				string_0 += "|" + activeSlot[s];
			}

			return activeSlot;
		}

		public int MaxLevel()
		{
			switch (int_25)
			{
			case 1:
				return 63;
			case 2:
				return 65;
			case 3:
				return 68;
			case 4:
				return 70;
			default:
				return 60;
			}
		}

		private int method_0()
		{
			double num1 = (double)int_21 / 10000.0 * 100.0;
			int num2 = 0;
			if (num1 >= 80.0)
			{
				num2 = 3;
			}
			if (num1 < 80.0 && num1 >= 60.0)
			{
				num2 = 2;
			}
			if (num1 < 60.0 && num1 > 0.0)
			{
				num2 = 1;
			}
			return num2;
		}

		private int method_1(int int_32)
		{
			if (method_0() == 2)
			{
				return int_32 * 20 / 100;
			}
			if (method_0() == 1)
			{
				return int_32 * 40 / 100;
			}
			return 0;
		}

		private double[] method_2(int int_32, double[] double_0)
		{
			double[] numArray = new double[double_0.Length];
			numArray[0] = double_0[0] * Math.Pow(2.0, int_32 - 1);
			for (int index = 1; index < double_0.Length; index++)
			{
				numArray[index] = double_0[index] * Math.Pow(1.5, int_32 - 1);
			}
			return numArray;
		}

		/*public void BuildProp(UsersPetInfo petInfo)
		{
			double[] numArray1 = new double[5]
			{
				BloodGrow * 10,
				AttackGrow,
				DefenceGrow,
				AgilityGrow,
				LuckGrow
			};
			double[] double_0 = new double[5]
			{
				BloodGrow,
				AttackGrow,
				DefenceGrow,
				AgilityGrow,
				LuckGrow
			};
			double[] numArray2 = numArray1;
			double[] numArray3 = numArray1;
			double[] numArray4 = numArray1;
			double[] numArray5 = method_2(1, double_0);
			double[] numArray6 = method_2(2, double_0);
			double[] numArray7 = method_2(3, double_0);
			_ = new double[numArray2.Length];
			double[] numArray8;
			if (Level < 30)
			{
				for (int index3 = 0; index3 < numArray2.Length; index3++)
				{
					numArray2[index3] += (double)(Level - 1) * numArray5[index3];
					numArray2[index3] = Math.Ceiling(numArray2[index3] / 10.0) / 10.0;
				}
				numArray8 = numArray2;
			}
			else if (Level < 50)
			{
				for (int index2 = 0; index2 < numArray3.Length; index2++)
				{
					numArray3[index2] += (double)(Level - 30) * numArray6[index2] + 29.0 * numArray5[index2];
					numArray3[index2] = Math.Ceiling(numArray3[index2] / 10.0) / 10.0;
				}
				numArray8 = numArray3;
			}
			else
			{
				for (int index = 0; index < numArray4.Length; index++)
				{
					numArray4[index] += (double)(Level - 50) * numArray7[index] + 20.0 * numArray6[index] + 29.0 * numArray5[index];
					numArray4[index] = Math.Ceiling(numArray4[index] / 10.0) / 10.0;
				}
				numArray8 = numArray4;
			}
			int_8 = (int)numArray8[0];
			int_4 = (int)numArray8[1];
			int_5 = (int)numArray8[2];
			int_7 = (int)numArray8[3];
			int_6 = (int)numArray8[4];
		}*/

		public void BuildProp()
		{
			double[] _oldPropArr = new double[] { BloodGrow * 10, AttackGrow, DefenceGrow, AgilityGrow, LuckGrow };
			double[] _oldGrowArr = new double[] { BloodGrow, AttackGrow, DefenceGrow, AgilityGrow, LuckGrow };

			double[] _propLevelArr_one = _oldPropArr;
			double[] _propLevelArr_two = _oldPropArr;
			double[] _propLevelArr_three = _oldPropArr;

			double[] _growLevelArr_one = method_2(1, _oldGrowArr);
			double[] _growLevelArr_two = method_2(2, _oldGrowArr);
			double[] _growLevelArr_three = method_2(3, _oldGrowArr);

			double[] propArr = new double[_propLevelArr_one.Length];

			if (Level < 30)
			{
				for (int p1 = 0; p1 < _propLevelArr_one.Length; p1++)
				{
					_propLevelArr_one[p1] = _propLevelArr_one[p1] + ((Level - 1) * _growLevelArr_one[p1]);
					_propLevelArr_one[p1] = Math.Ceiling(_propLevelArr_one[p1] / 10) / 10;
				}

				propArr = _propLevelArr_one;
			}
			else if (Level < 50)
			{
				for (int p2 = 0; p2 < _propLevelArr_two.Length; p2++)
				{
					_propLevelArr_two[p2] = _propLevelArr_two[p2] +
											((Level - 30) * _growLevelArr_two[p2] + 29 * _growLevelArr_one[p2]);
					_propLevelArr_two[p2] = Math.Ceiling(_propLevelArr_two[p2] / 10) / 10;
				}

				propArr = _propLevelArr_two;
			}
			else
			{
				for (int p3 = 0; p3 < _propLevelArr_three.Length; p3++)
				{
					_propLevelArr_three[p3] = _propLevelArr_three[p3] +
											  ((Level - 50) * _growLevelArr_three[p3] + 20 * _growLevelArr_two[p3] +
											   29 * _growLevelArr_one[p3]);
					_propLevelArr_three[p3] = Math.Ceiling(_propLevelArr_three[p3] / 10) / 10;
				}

				propArr = _propLevelArr_three;
			}

			int_8 = (int)propArr[0];
			int_4 = (int)propArr[1];
			int_5 = (int)propArr[2];
			int_7 = (int)propArr[3];
			int_6 = (int)propArr[4];
		}

		public UsersPetInfo()
		{
			list_0 = new List<PetEquipInfo>();
		}
	}
}
