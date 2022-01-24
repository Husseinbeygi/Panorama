using Accord.Imaging;
using AForge;
using System.Drawing;

namespace PanoramaFunctions
{
    public static class CorrelationMatch
    {
        public static IntPoint[][] Match(Bitmap image1, Bitmap image2,
            IntPoint[] harrisPoints1, IntPoint[] harrisPoints2)
        {
            CorrelationMatching matcher = new CorrelationMatching(9);
            return matcher.Match(image1, image2, harrisPoints1, harrisPoints2);

        }
    }

}