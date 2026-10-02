using ImageMagick;
using ImageMagick.Drawing;
using System.Globalization;

namespace PhotoConverter.Helpers
{
    public class Converter
    {
        public void ConvertHeicToJpg(
            string inputPath,
            string outputPath,
            uint quality = 90,
            bool addDate = true)
        {
            using var image = new MagickImage(inputPath);

            if (addDate)
            {
                AddCaptureDate(image);
            }

            image.Quality = quality;
            image.Format = MagickFormat.Jpeg;

            image.Write(outputPath);
        }

        public void ConvertCr2ToJpg(string cr2Path, string jpgPath, uint quality = 90)
        {
            using var image = new MagickImage(cr2Path);

            image.Format = MagickFormat.Jpeg;
            image.Quality = quality;

            image.Write(jpgPath);
        }

        private void AddCaptureDate(MagickImage image)
        {
            var date = GetCaptureDate(image);

            if (date == null)
                return;

            string text = date.Value.ToString(
                "dd-MM-yyyy",
                CultureInfo.InvariantCulture);

            double minDimension = Math.Min(
                image.Width,
                image.Height);

            double fontSize = Math.Max(
                24,
                minDimension * 0.035);

            var textColor = MagickColors.White;

            var strokeColor = MagickColors.Black;

            var shadowColor = new MagickColor("#000000");

            var drawables = new Drawables()
                .Font("Consolas")
                .FontPointSize(fontSize)
                .FillColor(textColor)
                .StrokeColor(strokeColor)
                .StrokeWidth(1)
                .Gravity(Gravity.Southeast)
                .Text(0, 0, text);

            drawables
                .FillColor(shadowColor)
                .StrokeColor(shadowColor)
                .StrokeWidth(1)
                .Text(2, 2, text);

            drawables
                .FillColor(textColor)
                .StrokeColor(shadowColor)
                .StrokeWidth(1)
                .Text(0, 0, text);

            image.Draw(drawables);
        }

        private DateTime? GetCaptureDate(MagickImage image)
        {
            var profile = image.GetExifProfile();

            if (profile == null)
                return null;

            var value = profile.GetValue(
                ExifTag.DateTimeOriginal);

            if (value == null)
                return null;

            var date = value.Value;

            if (string.IsNullOrWhiteSpace(date))
                return null;

            if (DateTime.TryParseExact(
                date,
                "yyyy:MM:dd HH:mm:ss",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var result))
            {
                return result;
            }

            return null;
        }
    }
}