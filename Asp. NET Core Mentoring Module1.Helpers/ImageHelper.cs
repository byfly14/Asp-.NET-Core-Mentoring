using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Asp._NET_Core_Mentoring_Module1.Helpers
{
    public class ImageHelper : IImageHelper
    {
        private readonly IConfiguration _configuration;

        public ImageHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private const int AmountOfGarbageBytes = 78;

        public string GetImageUrl(string fileName, byte[] bytes)
        {
            var bytesArray = SkipGarbageBytes(bytes);

            var imageServerUrl = _configuration["ImageServerUrl"];
            var imageStoragePath = _configuration["ImageStoragePath"];

            if (!Path.HasExtension(fileName))
            {
                fileName = $"{fileName}.bmp";
            }

            var invalid = Path.GetInvalidFileNameChars();

            fileName = invalid.Aggregate(fileName, (current, c) => current.Replace(c.ToString(), "_"));

            var filePath = Path.Combine(imageStoragePath, fileName);
            var imageUrl = Path.Combine(imageServerUrl, fileName);

            if (File.Exists(filePath))
            {
                return imageUrl;
            }

            CreateImageFileStream(filePath, bytesArray);

            return imageUrl;
        }

        public FileStream CreateImageFileStream(string filePath, byte[] imageBytes)
        {
            var fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
            var bytes = SkipGarbageBytes(imageBytes);
            fs.Write(bytes, 0, bytes.Length);
            fs.Flush();
            fs.Seek(0, SeekOrigin.Begin);

            return fs;
        }

        public async Task<FileStream> CreateImageFileStreamAsync(string filePath, byte[] imageBytes)
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

    public interface IImageHelper
    {
        string GetImageUrl(string fileName, byte[] bytes);
        FileStream CreateImageFileStream(string filePath, byte[] imageBytes);
        Task<FileStream> CreateImageFileStreamAsync(string filePath, byte[] imageBytes);

    }
}
