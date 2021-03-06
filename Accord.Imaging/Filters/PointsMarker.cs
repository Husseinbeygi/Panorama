// Accord Imaging Library
// Accord.NET framework
// http://www.crsouza.com
//
// Copyright © César Souza, 2009-2010
// cesarsouza at gmail.com
//

namespace Accord.Imaging.Filters
{
    using AForge;
    using AForge.Imaging;
    using AForge.Imaging.Filters;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;

    /// <summary>
    ///   Filter to mark (highlight) points in a image.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>The filter highlights points on the image using a given set of points.</para>
    /// 
    /// <para>The filter accepts 8 bpp grayscale and 24 color images for processing.</para>
    /// </remarks>
    /// 
    public class PointsMarker : BaseInPlaceFilter
    {
        private int width = 3;
        private Color markerColor = Color.White;
        private IntPoint[] points;
        private Dictionary<PixelFormat, PixelFormat> formatTransalations = new Dictionary<PixelFormat, PixelFormat>();


        /// <summary>
        /// Format translations dictionary.
        /// </summary>
        public override Dictionary<PixelFormat, PixelFormat> FormatTranslations
        {
            get { return formatTransalations; }
        }

        /// <summary>
        /// Color used to mark corners.
        /// </summary>
        public Color MarkerColor
        {
            get { return markerColor; }
            set { markerColor = value; }
        }

        /// <summary>
        ///  Gets or sets the set of points to mark.
        /// </summary>
        public IntPoint[] Points
        {
            get { return points; }
            set { points = value; }
        }

        /// <summary>
        ///   Gets or sets the width of the points to be drawn.
        /// </summary>
        public int Width
        {
            get { return width; }
            set { width = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PointsMarker"/> class.
        /// </summary>
        /// 
        public PointsMarker(IntPoint[] points)
            : this(points, Color.White, 3)
        {
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="PointsMarker"/> class.
        /// </summary>
        /// 
        public PointsMarker(IntPoint[] points, Color markerColor)
            : this(points, markerColor, 3)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PointsMarker"/> class.
        /// </summary>
        /// 
        public PointsMarker(IntPoint[] points, Color markerColor, int width)
        {
            this.points = points;
            this.markerColor = markerColor;
            this.width = width;

            formatTransalations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
            formatTransalations[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
        }

        /// <summary>
        /// Process the filter on the specified image.
        /// </summary>
        /// 
        /// <param name="image">Source image data.</param>
        ///
        protected override unsafe void ProcessFilter(UnmanagedImage image)
        {
            // mark all points
            foreach (IntPoint p in points)
            {
                Drawing.FillRectangle(image, new Rectangle(p.X - 1, p.Y - 1, width, width), markerColor);
            }
        }
    }
}