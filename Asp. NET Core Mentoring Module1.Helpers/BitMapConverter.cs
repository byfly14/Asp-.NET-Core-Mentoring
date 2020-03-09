using System.Drawing;
using System.IO;

namespace Asp._NET_Core_Mentoring_Module1.Helpers
{
    public static class BitMapConverter
    {
        public static Bitmap GetBitmap(byte[] imageData)
        {
            Bitmap bmp;
            using (var ms = new MemoryStream(imageData))
            {
                bmp = new Bitmap(ms);
            }

            return bmp;
        }
    }
}
