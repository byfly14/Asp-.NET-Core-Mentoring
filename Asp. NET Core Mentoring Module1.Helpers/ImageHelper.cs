using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Asp._NET_Core_Mentoring_Module1.Helpers
{
    public static class ImageHelper
    {
        private const int AmountOfGarbageBytes = 78;

        public static string GetImageBase64String(byte[] bytes)
        {
            string base64Data;

            var bytesArray = SkipGarbageBytes(bytes);

            using var stream = new MemoryStream(bytesArray);
            {
                base64Data = Convert.ToBase64String(stream.ToArray());
            }
            return "data:image/bmp;base64," + base64Data;
        }

        public static FileStream CreateImageFileStream(string filePath, byte[] imageBytes)
        {
            var fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
            var bytes = SkipGarbageBytes(imageBytes);
            fs.Write(bytes, 0, bytes.Length);
            fs.Flush();
            fs.Seek(0, SeekOrigin.Begin);

            return fs;
        }

        public static async Task<FileStream> CreateImageFileStreamAsync(string filePath, byte[] imageBytes)
        {
            var fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
            var bytes = SkipGarbageBytes(imageBytes);
            await fs.WriteAsync(bytes, 0, bytes.Length).ConfigureAwait(false);
            await fs.FlushAsync().ConfigureAwait(false);
            fs.Seek(0, SeekOrigin.Begin);

            return fs;
        }

        private static byte[] SkipGarbageBytes(byte[] bytes)
        {
            return bytes.Skip(bytes[0] == 0x42 && bytes[1] == 0x4D ? 0 : AmountOfGarbageBytes).ToArray();
        }
    }
}
