using Accord.Imaging;
using AForge;
using System.Drawing;

namespace PanoramaFunctions
{
    public static class HarrisDetector
    {

        public static IntPoint[] Detect(Bitmap image)
        {
            HarrisCornersDetector harris = new HarrisCornersDetector(0.04f, 1000f);
            return harris.ProcessImage(image).ToArray();
        }

    }

}