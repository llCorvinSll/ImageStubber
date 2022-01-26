namespace ImageStubber.Image.ImageGenerator
{
    public interface IImageGenerator
    {
        byte[] GenerateImage(ImageDescription description);

        byte[] GenerateQrImage(ImageDescription description);
    }
}