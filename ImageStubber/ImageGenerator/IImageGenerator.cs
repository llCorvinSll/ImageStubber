using System.IO;

namespace ImageStubber.ImageGenerator
{
    public interface IImageGenerator
    {
        MemoryStream GenerateImage(int width, int height, string bgColor, string textColor);
    }
}