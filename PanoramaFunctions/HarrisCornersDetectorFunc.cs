using Accord.Imaging;
using Accord.Imaging.Filters;
using AForge;
using System.Drawing;

namespace PanoramaFunctions
{
    public class HarrisDetector
    {

        public IntPoint[] Detect(Bitmap image)
        {
            HarrisCornersDetector harris = new HarrisCornersDetector(0.04f, 1000f);
            return harris.ProcessImage(image).ToArray();
        }

    }

    public class CorrelationMatch
    {
        public IntPoint[][] Match(Bitmap image1, Bitmap image2,
            IntPoint[] harrisPoints1, IntPoint[] harrisPoints2)
        {
            CorrelationMatching matcher = new CorrelationMatching(9);
            return matcher.Match(image1, image2, harrisPoints1, harrisPoints2);

        }
    }

    public class Ransac
    {
        public MatrixH Estimate(IntPoint[] correlationPoints1, IntPoint[] correlationPoints2)
        {
            // Step 3: Create the homography matrix using a robust estimator
            RansacHomographyEstimator ran = new RansacHomographyEstimator(0.001, 0.99);
            return ran.Estimate(correlationPoints1, correlationPoints2);

        }
    }


    //private Bitmap Create(MatrixH a,Bitmap img1, Bitmap img2)
    //{
    //    Blend blend = new Blend(a, img1);
    //    return blend.Apply(img2);
    //}

}