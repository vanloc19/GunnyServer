using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Bussiness;

namespace Tank.Request
{
	public class ValidateCode : Page
	{
		public static Color[] colors = new Color[4]
		{
			Color.Blue,
			Color.DarkRed,
			Color.Green,
			Color.Gold
		};

		protected HtmlForm form1;

		protected Button Button1;

		protected void Page_Load(object sender, EventArgs e)
		{
			byte[] image = CheckCode.CreateImage(CheckCode.GenerateCheckCode());
			base.Response.ClearContent();
			base.Response.ContentType = "image/Gif";
			base.Response.BinaryWrite(image);
		}

		protected void Button1_Click(object sender, EventArgs e)
		{
			CreateCheckCodeImage(GenerateCheckCode());
		}

		private string GenerateCheckCode()
		{
			string empty = string.Empty;
			Random random = new Random();
			for (int index = 0; index < 4; index++)
			{
				empty += (char)(65 + (ushort)(random.Next() % 26));
			}
			return empty;
		}

		private void CreateCheckCodeImage(string checkCode)
		{
			if (checkCode == null || checkCode.Trim() == string.Empty)
			{
				return;
			}
			Bitmap bitmap = new Bitmap((int)Math.Ceiling((double)checkCode.Length * 40.5), 44);
			Graphics graphics = Graphics.FromImage(bitmap);
			try
			{
				Random random = new Random();
				Color color = colors[random.Next(colors.Length)];
				graphics.Clear(Color.Transparent);
				for (int index = 0; index < 2; index++)
				{
					int num2 = random.Next(bitmap.Width);
					random.Next(bitmap.Width);
					int num3 = random.Next(bitmap.Height);
					random.Next(bitmap.Height);
					graphics.DrawArc(new Pen(color, 2f), -num2, -num3, bitmap.Width * 2, bitmap.Height, 45, 100);
				}
				Font font = new Font("Arial", 24f, FontStyle.Bold | FontStyle.Italic);
				LinearGradientBrush linearGradientBrush = new LinearGradientBrush(new Rectangle(0, 0, bitmap.Width, bitmap.Height), color, color, 1.2f, isAngleScaleable: true);
				graphics.DrawString(checkCode, font, linearGradientBrush, 2f, 2f);
				int num = 40;
				Math.Sin(Math.PI * (double)num / 180.0);
				Math.Cos(Math.PI * (double)num / 180.0);
				Math.Atan(Math.PI * (double)num / 180.0);
				if (num > 0)
				{
					_ = bitmap.Width;
				}
				new TextureBrush(bitmap).RotateTransform(30f);
				bitmap.Save("c:\\1.jpg", ImageFormat.Png);
				MemoryStream memoryStream = new MemoryStream();
				bitmap.Save(memoryStream, ImageFormat.Png);
				base.Response.ClearContent();
				base.Response.ContentType = "image/Gif";
				base.Response.BinaryWrite(memoryStream.ToArray());
			}
			finally
			{
				graphics.Dispose();
				bitmap.Dispose();
			}
		}

		public static Bitmap KiRotate(Bitmap bmp, float angle, Color bkColor)
		{
			int width = bmp.Width + 2;
			int height = bmp.Height + 2;
			PixelFormat format = ((!(bkColor == Color.Transparent)) ? bmp.PixelFormat : PixelFormat.Format32bppArgb);
			Bitmap bitmap1 = new Bitmap(width, height, format);
			Graphics graphics = Graphics.FromImage(bitmap1);
			graphics.Clear(bkColor);
			graphics.DrawImageUnscaled(bmp, 1, 1);
			graphics.Dispose();
			GraphicsPath graphicsPath = new GraphicsPath();
			graphicsPath.AddRectangle(new RectangleF(0f, 0f, width, height));
			Matrix matrix = new Matrix();
			matrix.Rotate(angle);
			RectangleF bounds = graphicsPath.GetBounds(matrix);
			Bitmap bitmap2 = new Bitmap((int)bounds.Width, (int)bounds.Height, format);
			Graphics graphics2 = Graphics.FromImage(bitmap2);
			graphics2.Clear(bkColor);
			graphics2.TranslateTransform(0f - bounds.X, 0f - bounds.Y);
			graphics2.RotateTransform(angle);
			graphics2.InterpolationMode = InterpolationMode.HighQualityBilinear;
			graphics2.DrawImageUnscaled(bitmap1, 0, 0);
			graphics2.Dispose();
			bitmap1.Dispose();
			return bitmap2;
		}
	}
}
