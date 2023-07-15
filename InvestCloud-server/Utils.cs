using System.Security.Cryptography;
using System.Text;

namespace InvestCloudServer.Utils
{
    public class Utils
    {
        public static string MatrixToString(int[,] matrix)
        {
            int size = matrix.GetLength(0);
            StringBuilder sb = new();

            // Iterate through each element in the matrix and append it to the string builder
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    sb.Append(matrix[i, j]);
                }
            }

            return sb.ToString();
        }

        public static string CalculateMD5Hash(string input)
        {
            byte[] inputBytes = Encoding.ASCII.GetBytes(input);
            byte[] hashBytes = MD5.HashData(inputBytes);
            return Convert.ToHexString(hashBytes);
        }

        // Performs matrix multiplication using Strassen's algorithm
        // traditionalThreshold determines the size at which the algorithm switches to traditional multiplication
        public static int[,] MultiplyMatrices(int[,] A, int[,] B, int traditionalThreshold = 128)
        {
            int n = A.GetLength(0);
            int[,] result = new int[n, n];

            if (n <= traditionalThreshold)
            {
                // If the matrix size is small enough, use traditional matrix multiplication
                result = TraditionalMultiplyMatrices(A, B);
            }
            else
            {
                // Divide matrices into submatrices
                int halfSize = n / 2;

                int[,] A11 = new int[halfSize, halfSize];
                int[,] A12 = new int[halfSize, halfSize];
                int[,] A21 = new int[halfSize, halfSize];
                int[,] A22 = new int[halfSize, halfSize];

                int[,] B11 = new int[halfSize, halfSize];
                int[,] B12 = new int[halfSize, halfSize];
                int[,] B21 = new int[halfSize, halfSize];
                int[,] B22 = new int[halfSize, halfSize];

                DivideMatrix(A, A11, 0, 0);
                DivideMatrix(A, A12, 0, halfSize);
                DivideMatrix(A, A21, halfSize, 0);
                DivideMatrix(A, A22, halfSize, halfSize);

                DivideMatrix(B, B11, 0, 0);
                DivideMatrix(B, B12, 0, halfSize);
                DivideMatrix(B, B21, halfSize, 0);
                DivideMatrix(B, B22, halfSize, halfSize);

                // Recursive steps
                int[,] M1 = MultiplyMatrices(AddMatrices(A11, A22), AddMatrices(B11, B22));
                int[,] M2 = MultiplyMatrices(AddMatrices(A21, A22), B11);
                int[,] M3 = MultiplyMatrices(A11, SubtractMatrices(B12, B22));
                int[,] M4 = MultiplyMatrices(A22, SubtractMatrices(B21, B11));
                int[,] M5 = MultiplyMatrices(AddMatrices(A11, A12), B22);
                int[,] M6 = MultiplyMatrices(SubtractMatrices(A21, A11), AddMatrices(B11, B12));
                int[,] M7 = MultiplyMatrices(SubtractMatrices(A12, A22), AddMatrices(B21, B22));

                int[,] C11 = AddMatrices(SubtractMatrices(AddMatrices(M1, M4), M5), M7);
                int[,] C12 = AddMatrices(M3, M5);
                int[,] C21 = AddMatrices(M2, M4);
                int[,] C22 = AddMatrices(SubtractMatrices(AddMatrices(M1, M3), M2), M6);

                // Combine submatrices into result matrix
                CombineMatrices(C11, result, 0, 0);
                CombineMatrices(C12, result, 0, halfSize);
                CombineMatrices(C21, result, halfSize, 0);
                CombineMatrices(C22, result, halfSize, halfSize);
            }

            return result;
        }

        private static void DivideMatrix(int[,] source, int[,] target, int row, int col)
        {
            int size = target.GetLength(0);
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    target[i, j] = source[i + row, j + col];
                }
            }
        }

        private static void CombineMatrices(int[,] source, int[,] target, int row, int col)
        {
            int size = source.GetLength(0);
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    target[i + row, j + col] = source[i, j];
                }
            }
        }

        private static int[,] AddMatrices(int[,] A, int[,] B)
        {
            int n = A.GetLength(0);
            int[,] result = new int[n, n];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    result[i, j] = A[i, j] + B[i, j];
                }
            }
            return result;
        }

        private static int[,] SubtractMatrices(int[,] A, int[,] B)
        {
            int n = A.GetLength(0);
            int[,] result = new int[n, n];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    result[i, j] = A[i, j] - B[i, j];
                }
            }
            return result;
        }

        public static int[,] TraditionalMultiplyMatrices(int[,] A, int[,] B)
        {
            int size = A.GetLength(0);
            int[,] result = new int[size, size];

            // Use parallelism to speed up the computation
            Parallel.For(
                0,
                size,
                i =>
                {
                    for (int j = 0; j < size; j++)
                    {
                        int sum = 0;
                        for (int k = 0; k < size; k++)
                        {
                            sum += A[i, k] * B[k, j];
                        }
                        result[i, j] = sum;
                    }
                }
            );

            return result;
        }
    }
}
