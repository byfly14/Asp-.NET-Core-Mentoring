using System;
using System.IO;
using Asp._NET_Core_Mentoring_Module1.Helpers;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Asp_.NET_MVC_Core_Mentoring_Module1.Services
{
    public class DiskImageCacheService : IDiskImageCacheService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IConfiguration _config;
        private readonly IImageHelper _imageHelper;
        private readonly ILogger _logger;
        private readonly int _maxTotalAmountOfCachedItems;
        
        public DiskImageCacheService(IMemoryCache memoryCache, 
            IConfiguration config,
            ILoggerFactory loggerFactory,
            IImageHelper imageHelper)
        {
            _memoryCache = memoryCache;
            _config = config;
            _imageHelper = imageHelper;
            _logger = loggerFactory.CreateLogger(nameof(DiskImageCacheService));

            int.TryParse(_config["MaxTotalAmountOfCachedItems"], out _maxTotalAmountOfCachedItems);
        }

        public void CacheImage(int id, byte[] bytes)
        {
            _memoryCache.Remove(id);

            var tempPath = _config["TempDirectoryPath"];

            var filePath = Path.Combine(string.IsNullOrWhiteSpace(tempPath) ? Path.GetTempPath() : tempPath, Path.GetRandomFileName());

            _imageHelper.CreateImageFileStream(filePath, bytes);

            var cacheEntryOptions = GetMemoryCacheEntryOptions();

            _memoryCache.Set(id, filePath, cacheEntryOptions);

            if (_memoryCache is MemoryCache cache && cache.Count > _maxTotalAmountOfCachedItems)
            {
                cache.Compact(0.5);
            }
        }

        public bool TryGetImagePath(int id, out string pathToCachedImage)
        {
            return _memoryCache.TryGetValue(id, out pathToCachedImage);
        }
        
        private MemoryCacheEntryOptions GetMemoryCacheEntryOptions()
        {
            int.TryParse(_config["CachedItemExpirationTime"], out var cacheExpirationTime);

            var cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(cacheExpirationTime),
                PostEvictionCallbacks =
                {
                    new PostEvictionCallbackRegistration
                    {
                        EvictionCallback = EvictionCallback
                    }
                }
            };
            return cacheEntryOptions;
        }

        private void EvictionCallback(object key, object value, EvictionReason reason, object state)
        {
            try
            {
                var path = value.ToString();
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
            }
        }
    }

    public interface IDiskImageCacheService
    {
        void CacheImage(int id, byte[] bytes);
        bool TryGetImagePath(int id, out string pathToCachedImage);
    }
}
