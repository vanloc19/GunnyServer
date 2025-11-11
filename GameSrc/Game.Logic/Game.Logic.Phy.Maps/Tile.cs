using System;
using System.Drawing;
using System.IO;

namespace Game.Logic.Phy.Maps
{
	public class Tile
	{
		private int _bh;

		private int _bw;

		private byte[] _data;

		private bool _digable;

		private int _height;

		private Rectangle _rect;

		private int _width;

		public Rectangle Bound => _rect;

		public byte[] Data => _data;

		public int Height => _height;

		public int Width => _width;

		public Tile(Bitmap bitmap, bool digable)
		{
			_width = bitmap.Width;
			_height = bitmap.Height;
			_bw = _width / 8 + 1;
			_bh = _height;
			_data = new byte[_bw * _bh];
			_digable = digable;
			for (int i = 0; i < bitmap.Height; i++)
			{
				for (int j = 0; j < bitmap.Width; j++)
				{
					byte num3 = (byte)((bitmap.GetPixel(j, i).A > 100) ? 1 : 0);
					byte[] buffer = _data;
					int index = i * _bw + j / 8;
					buffer[index] = (byte)(buffer[index] | (byte)(num3 << 7 - j % 8));
				}
			}
			_rect = new Rectangle(0, 0, _width, _height);
			GC.AddMemoryPressure(_data.Length);
		}

		public Tile(string file, bool digable)
		{
			BinaryReader reader = new BinaryReader(File.Open(file, FileMode.Open));
			_width = reader.ReadInt32();
			_height = reader.ReadInt32();
			_bw = _width / 8 + 1;
			_bh = _height;
			_data = reader.ReadBytes(_bw * _bh);
			_digable = digable;
			_rect = new Rectangle(0, 0, _width, _height);
			reader.Close();
			GC.AddMemoryPressure(_data.Length);
		}

		public Tile(byte[] data, int width, int height, bool digable)
		{
			_data = data;
			_width = width;
			_height = height;
			_digable = digable;
			_bw = _width / 8 + 1;
			_bh = _height;
			_rect = new Rectangle(0, 0, _width, _height);
			GC.AddMemoryPressure(data.Length);
		}

		protected void Add(int x, int y, Tile tile)
		{
		}

		public Tile Clone()
		{
			return new Tile(_data.Clone() as byte[], _width, _height, _digable);
		}

		public void Dig(int cx, int cy, Tile surface, Tile border)
		{
			if (_digable && surface != null)
			{
				int x = cx - surface.Width / 2;
				int y = cy - surface.Height / 2;
				Remove(x, y, surface);
				if (border != null)
				{
					x = cx - border.Width / 2;
					y = cy - border.Height / 2;
					Add(x, y, surface);
				}
			}
		}

		public Point FindNotEmptyPoint(int x, int y, int h)
		{
			if (x >= 0 && x < _width)
			{
				y = ((y >= 0) ? y : 0);
				h = ((y + h > _height) ? (_height - y) : h);
				for (int i = 0; i < h; i++)
				{
					if (!IsEmpty(x, y + i))
					{
						return new Point(x, y + i);
					}
				}
				return new Point(-1, -1);
			}
			return new Point(-1, -1);
		}

		public bool IsEmpty(int x, int y)
		{
			if (x >= 0 && x < _width && y >= 0 && y < _height)
			{
				byte num = (byte)(1 << 7 - x % 8);
				return (_data[y * _bw + x / 8] & num) == 0;
			}
			return true;
		}

		public bool IsRectangleEmptyQuick(Rectangle rect)
		{
			rect.Intersect(_rect);
			if (IsEmpty(rect.Right, rect.Bottom) && IsEmpty(rect.Left, rect.Bottom) && IsEmpty(rect.Right, rect.Top))
			{
				return IsEmpty(rect.Left, rect.Top);
			}
			return false;
		}

		public bool IsYLineEmtpy(int x, int y, int h)
		{
			if (x >= 0 && x < _width)
			{
				y = ((y >= 0) ? y : 0);
				h = ((y + h > _height) ? (_height - y) : h);
				for (int i = 0; i < h; i++)
				{
					if (!IsEmpty(x, y + i))
					{
						return false;
					}
				}
				return true;
			}
			return true;
		}

		protected void Remove(int x, int y, Tile tile)
		{
			byte[] buffer = tile._data;
			Rectangle bound = tile.Bound;
			bound.Offset(x, y);
			bound.Intersect(_rect);
			if (bound.Width == 0 || bound.Height == 0)
			{
				return;
			}
			bound.Offset(-x, -y);
			int num = bound.X / 8;
			int num8 = (bound.X + x) / 8;
			int num12 = bound.Y;
			int num13 = bound.Width / 8 + 1;
			int height = bound.Height;
			if (bound.X == 0)
			{
				if (num13 + num8 < _bw)
				{
					num13++;
					num13 = ((num13 > tile._bw) ? tile._bw : num13);
				}
				int num14 = (bound.X + x) % 8;
				for (int i = 0; i < height; i++)
				{
					int num15 = 0;
					for (int j = 0; j < num13; j++)
					{
						int index = (i + y + num12) * _bw + j + num8;
						int num2 = (i + num12) * tile._bw + j + num;
						byte num16 = buffer[num2];
						int num3 = num16 >> num14;
						int num4 = _data[index];
						num4 &= ~(num4 & num3);
						if (num15 != 0)
						{
							num4 &= ~(num4 & num15);
						}
						_data[index] = (byte)num4;
						num15 = num16 << 8 - num14;
					}
				}
				return;
			}
			int num5 = bound.X % 8;
			for (int k = 0; k < height; k++)
			{
				for (int l = 0; l < num13; l++)
				{
					int num6 = (k + y + num12) * _bw + l + num8;
					int num7 = (k + num12) * tile._bw + l + num;
					int num9 = buffer[num7] << num5;
					int num10 = ((l < num13 - 1) ? (buffer[num7 + 1] >> 8 - num5) : 0);
					int num11 = _data[num6];
					num11 &= ~(num11 & num9);
					if (num10 != 0)
					{
						num11 &= ~(num11 & num10);
					}
					_data[num6] = (byte)num11;
				}
			}
		}

		public Bitmap ToBitmap()
		{
			Bitmap bitmap = new Bitmap(_width, _height);
			for (int i = 0; i < _height; i++)
			{
				for (int j = 0; j < _width; j++)
				{
					if (IsEmpty(j, i))
					{
						bitmap.SetPixel(j, i, Color.FromArgb(0, 0, 0, 0));
					}
					else
					{
						bitmap.SetPixel(j, i, Color.FromArgb(255, 0, 0, 0));
					}
				}
			}
			return bitmap;
		}
	}
}
