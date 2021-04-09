using System;
using ImageStubber.Image.ImageGenerator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace ImageStubber.Image
{
    [ApiController]
    [Route("image")]
    [ResponseCache(VaryByHeader = "User-Agent", Duration = 43200)]
    public class ImageController : ControllerBase
    {
        private readonly IImageGenerator _imageGenerator;
        private readonly ILogger<ImageController> _logger;
        private IMemoryCache _cache;

        public ImageController(ILogger<ImageController> logger, IImageGenerator imageGenerator, IMemoryCache cache)
        {
            _logger = logger;
            _imageGenerator = imageGenerator;
            _cache = cache;
        }
        
        [HttpGet("{width=200}/{height=200}/{bgColor=7d7d7d}/{textColor=ffffff}")]
        public FileContentResult GetImage(int width, int height, string bgColor, string textColor)
        {

            var key = $"{width}-{height}-{bgColor}-{textColor}";
            
            byte[] image;

            if (!_cache.TryGetValue(key, out image))
            {
                var imageDescriptions = ImageParamsParser.Parse(width, height, bgColor, textColor);
                var ms = _imageGenerator.GenerateImage(imageDescriptions);
                image = ms;

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromHours(8));

                _cache.Set(key, image, cacheEntryOptions);
            }
            
            return File(image, "image/png");
        }
    }
}