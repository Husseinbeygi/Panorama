// Accord Machine Learning Library
// Accord.NET framework
// http://www.crsouza.com
//
// Copyright © César Souza, 2009-2010
// cesarsouza at gmail.com
//

namespace Accord.MachineLearning
{
    using System;

    /// <summary>
    ///   Multipurpose RANSAC algorithm.
    /// </summary>
    /// <typeparam name="TModel">The model type to be trained by RANSAC.</typeparam>
    /// <remarks>
    ///   RANSAC is an abbreviation for "RANdom SAmple Consensus". It is an iterative
    ///   method to estimate parameters of a mathematical model from a set of observed
    ///   data which contains outliers. It is a non-deterministic algorithm in the sense
    ///   that it produces a reasonable result only with a certain probability, with this
    ///   probability increasing as more iterations are allowed.
    /// </remarks>
    /// 
    public class RANSAC<TModel> where TModel : class
    {
        // Ransac parameters
        private int s;    // number of samples
        private double t; // inlier threshold
        private int maxSamplings = 100;
        private int maxEvaluations = 1000;
        private double probability = 0.99;

        // Ransac functions
        private Func<int[], TModel> fitting;
        private Func<TModel, double, int[]> distances;
        private Func<int[], bool> degenerate;



        #region Properties
        /// <summary>
        ///   Model fitting function.
        /// </summary>
        public Func<int[], TModel> Fitting
        {
            get { return fitting; }
            set { fitting = value; }
        }

        /// <summary>
        ///   Degenerative set detection function.
        /// </summary>
        public Func<int[], bool> Degenerate
        {
            get { return degenerate; }
            set { degenerate = value; }
        }

        /// <summary>
        ///   Distance function.
        /// </summary>
        public Func<TModel, double, int[]> Distances
        {
            get { return distances; }
            set { distances = value; }
        }

        /// <summary>
        ///   Gets or sets the minimum distance between a data point and
        ///   the model used to decide whether the point is an inlier or not.
        /// </summary>
        public double Threshold
        {
            get { return t; }
            set { t = value; }
        }

        /// <summary>
        ///   Gets or sets the minimum number of samples from the data
        ///   required by the fitting function to fit a model.
        /// </summary>
        public int Samples
        {
            get { return s; }
            set { s = value; }
        }

        /// <summary>
        ///   Maximum number of attempts to select a non-degenerate data set.
        /// </summary>
        /// <remarks>
        ///   The default value is 100.
        /// </remarks>
        public int MaxSamplings
        {
            get { return maxSamplings; }
            set { maxSamplings = value; }
        }

        /// <summary>
        ///   Maximum number of iterations.
        /// </summary>
        /// <remarks>
        ///   The default value is 1000.
        /// </remarks>
        public int MaxEvaluations
        {
            get { return maxEvaluations; }
            set { maxEvaluations = value; }
        }

        /// <summary>
        ///   Gets or sets the probability of obtaining a random
        ///   sample of the input points that contains no outliers.
        /// </summary>
        public double Probability
        {
            get { return probability; }
            set { probability = value; }
        }
        #endregion


        /// <summary>
        ///   Constructs a new RANSAC algorithm.
        /// </summary>
        /// <param name="minSamples">
        ///   The minimum number of samples from the data
        ///   required by the fitting function to fit a model.
        /// </param>
        public RANSAC(int minSamples)
        {
            this.s = minSamples;
        }

        /// <summary>
        ///   Constructs a new RANSAC algorithm.
        /// </summary>
        /// <param name="minSamples">
        ///   The minimum number of samples from the data
        ///   required by the fitting function to fit a model.
        /// </param>
        /// <param name="threshold">
        ///   The minimum distance between a data point and
        ///   the model used to decide whether the point is
        ///   an inlier or not.
        /// </param>
        public RANSAC(int minSamples, double threshold)
        {
            this.s = minSamples;
            this.t = threshold;
        }

        /// <summary>
        ///   Constructs a new RANSAC algorithm.
        /// </summary>
        /// <param name="minSamples">
        ///   The minimum number of samples from the data
        ///   required by the fitting function to fit a model.
        /// </param>
        /// <param name="threshold">
        ///   The minimum distance between a data point and
        ///   the model used to decide whether the point is
        ///   an inlier or not.
        /// </param>
        /// <param name="probability">
        ///   The probability of obtaining a random sample of
        ///   the input points that contains no outliers.
        /// </param>
        public RANSAC(int minSamples, double threshold, double probability)
        {
            if (minSamples < 0) throw new ArgumentOutOfRangeException("minSamples");
            if (threshold < 0) throw new ArgumentOutOfRangeException("threshold");
            if (probability > 1.0 || probability < 0.0)
                throw new ArgumentException("Probability should be a value between 0 and 1", "probability");

            this.s = minSamples;
            this.t = threshold;
            this.probability = probability;
        }


        /// <summary>
        ///   Computes the model using the RANSAC algorithm.
        /// </summary>
        /// <param name="size">The total number of points in the data set.</param>
        public TModel Compute(int size)
        {
            int[] inliers;
            return Compute(size, out inliers);
        }

        /// <summary>
        ///   Computes the model using the RANSAC algorithm.
        /// </summary>
        /// <param name="size">The total number of points in the data set.</param>
        /// <param name="inliers">The indexes of the outlier points in the data set.</param>
        public TModel Compute(int size, out int[] inliers)
        {
            // We are going to find the best model (which fits
            //  the maximum number of inlier points as possible).
            TModel bestModel = null;
            int[] bestInliers = null;
            int maxInliers = 0;

            // For this we are going to search for random samples
            //  of the original points which contains no outliers.

            int count = 0;  // Total number of trials performed
            double N = maxEvaluations;   // Estimative of number of trials needed.

            // While the number of trials is less than our estimative,
            //   and we have not surpassed the maximum number of trials
            while (count < N && count < maxEvaluations)
            {
                TModel model = null;
                int[] sample = null;
                int samplings = 0;

                // While the number of samples attempted is less
                //   than the maximum limit of attempts
                while (samplings < maxSamplings)
                {
                    // Select at random s datapoints to form a trial model.
                    sample = Statistics.Tools.Random(size, s);

                    // If the sampled points are not in a degenerate configuration,
                    if (!degenerate(sample))
                    {
                        // Fit model using the random selection of points
                        model = fitting(sample);
                        break; // Exit the while loop.
                    }

                    samplings++; // Increase the samplings counter
                }

                // Now, evaluate the distances between total points and the model returning the
                //  indices of the points that are inliers (according to a distance threshold t).
                inliers = distances(model, t);

                // Check if the model was the model which highest number of inliers:
                if (inliers.Length > maxInliers)
                {
                    // Yes, this model has the highest number of inliers.

                    maxInliers = inliers.Length;  // Set the new maximum,
                    bestModel = model;            // This is the best model found so far,
                    bestInliers = inliers;        // Store the indices of the current inliers.

                    // Update estimate of N, the number of trials to ensure we pick, 
                    //   with probability p, a data set with no outliers.
                    double pInlier = (double)inliers.Length / (double)size;
                    double pNoOutliers = 1.0 - System.Math.Pow(pInlier, s);

                    N = System.Math.Log(1.0 - probability) / System.Math.Log(pNoOutliers);
                }

                count++; // Increase the trial counter.
            }

            inliers = bestInliers;
            return bestModel;
        }


    }
}
