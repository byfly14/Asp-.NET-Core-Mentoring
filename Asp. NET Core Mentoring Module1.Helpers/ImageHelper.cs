using System;
using System.IO;

namespace Asp._NET_Core_Mentoring_Module1.Helpers
{
    public static class ImageHelper
    {
        public static string GetImageBase64String(byte[] bytes)
        {
            string base64Data;
            using var stream = new MemoryStream(bytes);
            {
                base64Data = Convert.ToBase64String(stream.ToArray());
            }
            return "data:image/jpg;base64," + base64Data;
        }
    }
}
