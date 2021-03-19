using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using Microsoft.Extensions.Logging;

namespace ImageStubber.ImageGenerator
{
    public class ImageGenerator: IImageGenerator
    {   
        private readonly ILogger<ImageGenerator> _logger;
        
        public ImageGenerator(ILogger<ImageGenerator> logger)
        {
            _logger = logger;
        }

        private PrivateFontCollection getFontCollection()
        {
            var fontCollection = new PrivateFontCollection();
            
            var fontStream = GetType().Assembly.GetManifestResourceStream("ImageStubber.font.JetBrainsMonoNL-Medium.ttf");

            var fontdata = new byte[fontStream!.Length];
            fontStream.Read(fontdata,0,(int)fontStream.Length);
            fontStream.Close();

            unsafe
            {
                fixed(byte * pFontData = fontdata)
                {
                    fontCollection.AddMemoryFont((System.IntPtr)pFontData,fontdata.Length);
                }
            }

            return fontCollection;
        }

        public MemoryStream GenerateImage(int width, int height, string bgColor, string textColor)
        {
            using var bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            using var graphics = Graphics.FromImage(bitmap);

            var background = ColorTranslator.FromHtml($"#{bgColor}");
            var text = ColorTranslator.FromHtml($"#{textColor}");

            using var pen = new SolidBrush(text);

            graphics.FillRectangle(new SolidBrush(background), new Rectangle(0, 0, width, height));

            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

            var caption = $"{width}x{height}";

            using var pfk = getFontCollection();

            var fontCollectionFamily = pfk.Families[0];
            
            var font = new Font(fontCollectionFamily,27);

            var textArea = graphics.MeasureString(caption, font);

            var rectf = new RectangleF(
                width / 2 - textArea.Width / 2,
                height / 2 - textArea.Height / 2,
                textArea.Width,
                textArea.Height);

            graphics.DrawString(caption, font, pen, rectf);

            using var ms = new MemoryStream();
            bitmap.Save(ms, ImageFormat.Png);
            return ms;
        }
    }
}