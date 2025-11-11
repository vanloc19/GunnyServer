//namespace Bussiness
//{
//	public class Base64
//	{
//		private static readonly string BASE64_CHARS = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=";

//		public static byte[] decodeToByteArray(string param1)
//		{
//			byte[] buffer = new byte[param1.Length];
//			byte[] buffer2 = new byte[4];
//			byte[] buffer3 = new byte[3];
//			for (int i = 0; i < param1.Length; i += 4)
//			{
//				int index = 0;
//				int num3;
//				do
//				{
//					num3 = i + index;
//					if (index < 4)
//					{
//						buffer2[index] = (byte)BASE64_CHARS.IndexOf(param1.Substring(num3, 1));
//					}
//					index++;
//				}
//				while (num3 < param1.Length);
//				buffer3[0] = (byte)((buffer2[0] << 2) + ((buffer2[1] & 0x30) >> 4));
//				buffer3[1] = (byte)(((buffer2[1] & 0xF) << 4) + ((buffer2[2] & 0x3C) >> 2));
//				buffer3[2] = (byte)(((buffer2[2] & 3) << 6) + buffer2[3]);
//				for (int j = 0; j < buffer3.Length && buffer2[j + 1] != 64; j++)
//				{
//					buffer[i + j] = buffer3[j];
//				}
//			}
//			return buffer;
//		}

//		public static byte[] decodeToByteArray2(string param1)
//		{
//			byte[] buffer = new byte[param1.Length];
//			byte[] buffer2 = new byte[4];
//			for (int i = 0; i < param1.Length; i += 4)
//			{
//				int index = 0;
//				int num3;
//				do
//				{
//					num3 = i + index;
//					if (index < 4)
//					{
//						buffer2[index] = (byte)BASE64_CHARS.IndexOf(param1.Substring(num3, 1));
//					}
//					index++;
//				}
//				while (num3 < param1.Length);
//				for (int j = 0; j < buffer2.Length && buffer2[j] != 64; j++)
//				{
//					buffer[i + j] = buffer2[j];
//				}
//			}
//			return buffer;
//		}

//		public static string encodeByteArray(byte[] param1)
//		{
//			string str = "";
//			byte[] buffer = new byte[4];
//			for (int i = 0; i < param1.Length; i += 4)
//			{
//				byte[] buffer2 = new byte[3];
//				for (int j = 0; j < param1.Length; j++)
//				{
//					if (j < 3)
//					{
//						if (j + i > param1.Length)
//						{
//							break;
//						}
//						buffer2[j] = param1[j + i];
//					}
//				}
//				buffer[0] = (byte)((buffer2[0] & 0xFC) >> 2);
//				buffer[1] = (byte)(((buffer2[0] & 3) << 4) | (buffer2[1] >> 4));
//				buffer[2] = (byte)(((buffer2[1] & 0xF) << 2) | (buffer2[2] >> 6));
//				buffer[3] = (byte)(buffer2[2] & 0x3Fu);
//				for (int k = buffer2.Length; k < 3; k++)
//				{
//					buffer[k + 1] = 64;
//				}
//				for (int l = 0; l < buffer.Length; l++)
//				{
//					str += BASE64_CHARS.Substring(buffer[l], 1);
//				}
//			}
//			return str.Substring(0, param1.Length - 1) + "=";
//		}
//	}
//}

using System;
namespace Bussiness
{
	public class Base64
	{
		private static readonly string BASE64_CHARS = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=";
		public static string encodeByteArray(byte[] param1)
		{
			string _loc_2 = "";
			byte[] _loc_3 = new byte[4];
			for (int i = 0; i < param1.Length; i += 4)
			{
				byte[] _loc_4 = new byte[3];
				for (int _loc_5 = 0; _loc_5 < param1.Length; _loc_5++)
				{
					if (_loc_5 < 3)
					{
						if (_loc_5 + i > param1.Length)
						{
							break;
						}
						_loc_4[_loc_5] = param1[_loc_5 + i];
					}
				}
				_loc_3[0] = (byte)((_loc_4[0] & 252) >> 2);
				_loc_3[1] = (byte)((int)(_loc_4[0] & 3) << 4 | _loc_4[1] >> 4);
				_loc_3[2] = (byte)((int)(_loc_4[1] & 15) << 2 | _loc_4[2] >> 6);
				_loc_3[3] = (byte)(_loc_4[2] & 63);
				for (int _loc_6 = _loc_4.Length; _loc_6 < 3; _loc_6++)
				{
					_loc_3[_loc_6 + 1] = 64;
				}
				for (int _loc_7 = 0; _loc_7 < _loc_3.Length; _loc_7++)
				{
					_loc_2 += Base64.BASE64_CHARS.Substring((int)_loc_3[_loc_7], 1);
				}
			}
			_loc_2 = _loc_2.Substring(0, param1.Length - 1);
			return _loc_2 + "=";
		}
		public static byte[] decodeToByteArray2(string param1)
		{
			byte[] _loc_2 = new byte[param1.Length];
			byte[] _loc_3 = new byte[4];
			for (int _loc_4 = 0; _loc_4 < param1.Length; _loc_4 += 4)
			{
				int _loc_5 = 0;
				int index;
				do
				{
					index = _loc_4 + _loc_5;
					if (_loc_5 < 4)
					{
						_loc_3[_loc_5] = (byte)Base64.BASE64_CHARS.IndexOf(param1.Substring(index, 1));
					}
					_loc_5++;
				}
				while (index < param1.Length);
				int _loc_6 = 0;
				while (_loc_6 < _loc_3.Length && _loc_3[_loc_6] != 64)
				{
					_loc_2[_loc_4 + _loc_6] = _loc_3[_loc_6];
					_loc_6++;
				}
			}
			return _loc_2;
		}
		public static byte[] decodeToByteArray(string param1)
		{
			byte[] _loc_2 = new byte[param1.Length];
			byte[] _loc_3 = new byte[4];
			byte[] _loc_4 = new byte[3];
			for (int _loc_5 = 0; _loc_5 < param1.Length; _loc_5 += 4)
			{
				int _loc_6 = 0;
				int index;
				do
				{
					index = _loc_5 + _loc_6;
					if (_loc_6 < 4)
					{
						_loc_3[_loc_6] = (byte)Base64.BASE64_CHARS.IndexOf(param1.Substring(index, 1));
					}
					_loc_6++;
				}
				while (index < param1.Length);
				_loc_4[0] = (byte)(((int)_loc_3[0] << 2) + ((_loc_3[1] & 48) >> 4));
				_loc_4[1] = (byte)(((int)(_loc_3[1] & 15) << 4) + ((_loc_3[2] & 60) >> 2));
				_loc_4[2] = (byte)(((int)(_loc_3[2] & 3) << 6) + (int)_loc_3[3]);
				int _loc_7 = 0;
				while (_loc_7 < _loc_4.Length && _loc_3[_loc_7 + 1] != 64)
				{
					_loc_2[_loc_5 + _loc_7] = _loc_4[_loc_7];
					_loc_7++;
				}
			}
			return _loc_2;
		}
	}
}
