using System.IO;
using System.Drawing.Imaging;
using System.Drawing;

namespace Baetoti.Shared.Extentions
{
	public static class CommonExtension
	{
		public static byte[] BitmapToByteArray(this Bitmap bitmap)
		{
			using (MemoryStream ms = new MemoryStream())
			{
				bitmap.Save(ms, ImageFormat.Png);
				return ms.ToArray();
			}
		}

	}
}
