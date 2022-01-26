using Accord.Imaging.Filters;
using System.Drawing;

namespace PanoramaFunctions
{
    public class Image
    {
        public Bitmap Create(Bitmap img1, Bitmap img2)
        {
            var haris_image_1 = HarrisDetector.Detect(img1);
            var haris_image_2 = HarrisDetector.Detect(img2);

            var correlation = CorrelationMatch.Match(img1, img2, haris_image_1, haris_image_2);

            var ransac = Ransac.Estimate(correlation[0], correlation[1]);

            Blend blend = new Blend(ransac, img1);

            return blend.Apply(img2);
        }

    }
}
