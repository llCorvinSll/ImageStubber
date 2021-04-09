using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using Microsoft.Extensions.Logging;

namespace ImageStubber.Image.ImageGenerator
{
    public class ImageDescription
    {
        public ImageDescription(int width, int height, Color bgColor, Color textColor, bool colorError)
        {
            Width = width;
            Height = height;
            BgColor = bgColor;
            TextColor = textColor;
            ColorError = colorError;
        }

        public int Width { get; private set; }
        public int Height { get; private set; }
        public Color BgColor { get; private set; }
        public Color TextColor { get; private set; }
        public bool ColorError { get; private set; }
    }

    public class ImageGenerator: IImageGenerator
    {   
        private readonly ILogger<ImageGenerator> _logger;
        
        private const string FontCollectionFamily = "JetBrains Mono NL";
        private const int FontSize = 27;
        
        public ImageGenerator(ILogger<ImageGenerator> logger)
        {
            _logger = logger;
        }
        public byte[] GenerateImage(ImageDescription description)
        {
            using var bitmap = new Bitmap(description.Width, description.Height, PixelFormat.Format32bppArgb);
            bitmap.MakeTransparent();

            using var graphics = Graphics.FromImage(bitmap);
            using var pen = new SolidBrush(description.TextColor);
            
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            graphics.CompositingQuality = CompositingQuality.GammaCorrected;

            graphics.FillRectangle(new SolidBrush(description.BgColor), new Rectangle(0, 0, description.Width, description.Height));

            var caption = $"{description.Width}x{description.Height}";
            var fontSize = FontSize;

            if (description.ColorError)
            {
                caption = "color\nformat\nerror";
                fontSize = 24;
            }

            var font = new Font(FontCollectionFamily, fontSize);

            var textArea = graphics.MeasureString(caption, font);

            var rectf = new RectangleF(
                description.Width / 2 - textArea.Width / 2,
                description.Height / 2 - textArea.Height / 2,
                textArea.Width,
                textArea.Height);

            graphics.DrawString(caption, font, pen, rectf);
            using var ms = new MemoryStream();
            bitmap.Save(ms, ImageFormat.Png);
            return ms.ToArray();
        }
    }
}