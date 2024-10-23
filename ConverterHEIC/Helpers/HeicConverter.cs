using ImageMagick;

namespace ConverterHEIC.Helpers
{
    public class HeicConverter
    {
        public void ConvertHeicToJpg(string inputPath, string outputPath, uint quality = 90)
        {
            using (var image = new MagickImage(inputPath))
            {
                image.Quality = quality;
                image.Write(outputPath);
            }
        }
    }
}
