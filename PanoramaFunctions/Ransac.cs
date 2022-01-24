using Accord.Imaging;
using AForge;

namespace PanoramaFunctions
{
    public static class Ransac
    {
        public static MatrixH Estimate(IntPoint[] correlationPoints1, IntPoint[] correlationPoints2)
        {
            // Step 3: Create the homography matrix using a robust estimator
            RansacHomographyEstimator ran = new RansacHomographyEstimator(0.001, 0.99);
            return ran.Estimate(correlationPoints1, correlationPoints2);

        }
    }



}