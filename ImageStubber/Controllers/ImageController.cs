using System;
using ImageStubber.ImageGenerator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace ImageStubber.Controllers
{
    [ApiController]
    [Route("image")]
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
                var ms = _imageGenerator.GenerateImage(width, height, bgColor, textColor);
                image = ms.ToArray();

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromSeconds(3));

                _cache.Set(key, image, cacheEntryOptions);
            }
            
            return File(image, "image/png");
        }
    }
}