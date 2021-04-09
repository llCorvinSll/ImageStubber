using System.Drawing;
using ImageStubber.Image.ImageGenerator;
using NUnit.Framework;

namespace ImageStubber.Tests.Image.ImageGenerator
{
    public class Tests
    {
        [Test]
        [TestCaseSource(nameof(_validColors))]
        public void ShouldParseValidColors(string raw, Color color)
        {
            var success = ImageParamsParser.ParseColor(raw, out var res);
            
            Assert.IsTrue(success);
            Assert.AreEqual(color, res);
        }

        private static object[] _validColors =
        {
            new object[] {"#fc4e03", Color.FromArgb(252, 78, 3)},
            new object[] {"lightGrey", Color.LightGray},
            new object[] {"cyan", Color.Cyan},
            new object[] {"1e2832", Color.FromArgb(30, 40, 50)},
            new object[] {"rgba(30, 40, 50, 0.5)", Color.FromArgb(128,  Color.FromArgb(30, 40, 50))},
            new object[] {"rgba(30, 40, 50, 0.50000)", Color.FromArgb(128,  Color.FromArgb(30, 40, 50))},
            // new object[] {"rgb(30 40 50 / 50%)", Color.FromArgb(128,  Color.FromArgb(30, 40, 50))},
            new object[] {"#1e283280", Color.FromArgb(128, Color.FromArgb(30, 40, 50))},
            new object[] {"rgba(30,40,50,0.5)", Color.FromArgb(128,  Color.FromArgb(30, 40, 50))},
            new object[] {"rgba(30, 40, 50, 0.51111111)", Color.FromArgb(130,  Color.FromArgb(30, 40, 50))},
        };
    }
}