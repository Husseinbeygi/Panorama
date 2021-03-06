// Accord Math Library
// Accord.NET framework
// http://www.crsouza.com
//
// Modifications copyright ? C?sar Souza, 2009-2010
// cesarsouza at gmail.com
//
// Original work copyright ? Lutz Roeder, 2000
//  Adapted from Mapack for .NET, September 2000
//  Adapted from Mapack for COM and Jama routines
//  http://www.aisto.com/roeder/dotnet
//

namespace Accord.Math.Decompositions
{
    using System;

    /// <summary>
    ///		Cholesky Decomposition of a symmetric, positive definite matrix.
    ///	</summary>
    /// <remarks>
    ///   <para>
    ///		For a symmetric, positive definite matrix <c>A</c>, the Cholesky decomposition is a
    ///		lower triangular matrix <c>L</c> so that <c>A = L * L'</c>.
    ///		If the matrix is not symmetric or positive definite, the constructor returns a partial 
    ///		decomposition and sets two internal variables that can be queried using the
    ///		<see cref="Symmetric"/> and <see cref="PositiveDefinite"/> properties.</para>
    ///   <para>
    ///     Any square matrix A with non-zero pivots can be written as the product of a
    ///     lower triangular matrix L and an upper triangular matrix U; this is called
    ///     the LU decomposition. However, if A is symmetric and positive definite, we
    ///     can choose the factors such that U is the transpose of L, and this is called
    ///     the Cholesky decomposition. Both the LU and the Cholesky decomposition are
    ///     used to solve systems of linear equations.</para>
    ///   <para>
    ///     When it is applicable, the Cholesky decomposition is twice as efficient
    ///     as the LU decomposition.</para>
    ///	</remarks>
    ///	
    public sealed class CholeskyDecomposition : ICloneable
    {

        private double[,] L;
        private bool symmetric;
        private bool positiveDefinite;

        /// <summary>Constructs a Cholesky Decomposition.</summary>
        public CholeskyDecomposition(double[,] value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value", "Matrix cannot be null.");
            }

            if (value.GetLength(0) != value.GetLength(1))
            {
                throw new ArgumentException("Matrix is not square.", "value");
            }

            int dimension = value.GetLength(0);
            L = new double[dimension, dimension];

            double[,] a = value;

            this.positiveDefinite = true;
            this.symmetric = true;

            unsafe
            {
                fixed (double* l = L)
                {
                    for (int j = 0; j < dimension; j++)
                    {
                        double* Lrowj = l + j * dimension;
                        double d = 0.0;
                        for (int k = 0; k < j; k++)
                        {
                            double* Lrowk = l + k * dimension;

                            double s = 0.0;
                            for (int i = 0; i < k; i++)
                            {
                                s += Lrowk[i] * Lrowj[i];
                            }

                            Lrowj[k] = s = (a[j, k] - s) / Lrowk[k];
                            d = d + s * s;

                            this.symmetric = this.symmetric & (a[k, j] == a[j, k]);
                        }

                        d = a[j, j] - d;

                        this.positiveDefinite = this.positiveDefinite & (d > 0.0);
                        Lrowj[j] = System.Math.Sqrt(System.Math.Max(d, 0.0));
                        for (int k = j + 1; k < dimension; k++)
                        {
                            Lrowj[k] = 0.0;
                        }
                    }
                }
            }
        }

        /// <summary>Returns <see langword="true"/> if the matrix is symmetric.</summary>
        public bool Symmetric
        {
            get { return this.symmetric; }
        }

        /// <summary>Returns <see langword="true"/> if the matrix is positive definite.</summary>
        public bool PositiveDefinite
        {
            get { return this.positiveDefinite; }
        }

        /// <summary>Returns the left triangular factor <c>L</c> so that <c>A = L * L'</c>.</summary>
        public double[,] LeftTriangularFactor
        {
            get { return this.L; }
        }

        /// <summary>Solves a set of equation systems of type <c>A * X = B</c>.</summary>
        /// <param name="value">Right hand side matrix with as many rows as <c>A</c> and any number of columns.</param>
        /// <returns>Matrix <c>X</c> so that <c>L * L' * X = B</c>.</returns>
        /// <exception cref="T:System.ArgumentException">Matrix dimensions do not match.</exception>
        /// <exception cref="T:System.InvalidOperationException">Matrix is not symmetric and positive definite.</exception>
        public double[,] Solve(double[,] value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            if (value.GetLength(0) != L.GetLength(0))
            {
                throw new ArgumentException("Matrix dimensions do not match.");
            }

            if (!this.symmetric)
            {
                throw new InvalidOperationException("Matrix is not symmetric.");
            }

            if (!this.positiveDefinite)
            {
                throw new InvalidOperationException("Matrix is not positive definite.");
            }

            int dimension = L.GetLength(0);
            int count = value.GetLength(1);

            double[,] B = (double[,])value.Clone();


            // Solve L*Y = B;
            for (int k = 0; k < dimension; k++)
            {
                for (int j = 0; j < count; j++)
                {
                    for (int i = 0; i < k; i++)
                    {
                        B[k, j] -= B[i, j] * L[k, i];
                    }
                    B[k, j] /= L[k, k];
                }
            }

            // Solve L'*X = Y;
            for (int k = dimension - 1; k >= 0; k--)
            {
                for (int j = 0; j < count; j++)
                {
                    for (int i = k + 1; i < dimension; i++)
                    {
                        B[k, j] -= B[i, j] * L[i, k];
                    }
                    B[k, j] /= L[k, k];
                }
            }

            return B;
        }

        /// <summary>Solves a set of equation systems of type <c>A * x = b</c>.</summary>
        /// <param name="value">Right hand side column vector with as many rows as <c>A</c>.</param>
        /// <returns>Vector <c>x</c> so that <c>L * L' * x = b</c>.</returns>
        /// <exception cref="T:System.ArgumentException">Matrix dimensions do not match.</exception>
        /// <exception cref="T:System.InvalidOperationException">Matrix is not symmetric and positive definite.</exception>
        public double[] Solve(double[] value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            if (value.Length != L.GetLength(0))
            {
                throw new ArgumentException("Matrix dimensions do not match.");
            }

            if (!this.symmetric)
            {
                throw new InvalidOperationException("Matrix is not symmetric.");
            }

            if (!this.positiveDefinite)
            {
                throw new InvalidOperationException("Matrix is not positive definite.");
            }

            int dimension = L.GetLength(0);

            double[] B = (double[])value.Clone();


            // Solve L*Y = B;
            for (int k = 0; k < dimension; k++)
            {
                for (int i = 0; i < k; i++)
                    B[k] -= B[i] * L[k, i];
                B[k] /= L[k, k];
            }

            // Solve L'*X = Y;
            for (int k = dimension - 1; k >= 0; k--)
            {
                for (int i = k + 1; i < dimension; i++)
                    B[k] -= B[i] * L[i, k];
                B[k] /= L[k, k];
            }

            return B;
        }




        #region ICloneable Members

        private CholeskyDecomposition()
        {
        }

        /// <summary>
        ///   Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        ///   A new object that is a copy of this instance.
        /// </returns>
        public object Clone()
        {
            var clone = new CholeskyDecomposition();
            clone.L = (double[,])L.Clone();
            clone.positiveDefinite = positiveDefinite;
            clone.symmetric = symmetric;
            return clone;
        }

        #endregion
    }
}
