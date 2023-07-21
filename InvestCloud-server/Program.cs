using System.Diagnostics;

namespace InvestCloudServer
{
    class Program
    {
        static async Task Main()
        {
            int size = 1000; // Size of the matrices

            // Step 1: Initialize the datasets A and B
            await Services.Services.InitializeDatasets(size);

            // Start the stopwatch
            Stopwatch stopwatch = Stopwatch.StartNew();
            Console.WriteLine("Timer started");

            Stopwatch getDataset = Stopwatch.StartNew();
            Console.WriteLine("getDataset Timer started");

            // Step 2: Retrieve the datasets A and B
            Task<int[,]> getMatrixATask = Services.Services.GetDataset("A", size);
            Task<int[,]> getMatrixBTask = Services.Services.GetDataset("B", size);

            // Wait for both tasks to complete in parallel
            await Task.WhenAll(getMatrixATask, getMatrixBTask);

            getDataset.Stop();
            Console.WriteLine("getDatasetTime Ended: " + getDataset.Elapsed);

            int[,] matrixA = getMatrixATask.Result;
            int[,] matrixB = getMatrixBTask.Result;
            Console.WriteLine("matrixA Length: " + matrixA.Length);
            Console.WriteLine("matrixB Length: " + matrixB.Length);

            // Check if the matrices are square
            if (matrixA.Length != matrixB.Length)
                throw new Exception("Matrix is not a square");

            Stopwatch matrix = Stopwatch.StartNew();
            Console.WriteLine("matrix Timer started");

            // Step 3: Multiply the matrices (A x B)
            int[,] resultMatrix = Utils.Utils.MultiplyMatrices(matrixA, matrixB);
            Console.WriteLine("resultMatrix Length: " + resultMatrix.Length);

            if (resultMatrix.Length != (size * size))
                throw new Exception("Matrix is not a square");

            matrix.Stop();
            Console.WriteLine("getDatasetTime Ended: " + matrix.Elapsed);

            // Step 4: Convert the result matrix to a concatenated string
            string resultString = Utils.Utils.MatrixToString(resultMatrix);

            // End the stopwatch
            stopwatch.Stop();
            Console.WriteLine("Time Ended: " + stopwatch.Elapsed);

            // Step 5: Calculate the MD5 hash of the result string
            string md5Hash = Utils.Utils.CalculateMD5Hash(resultString);
            Console.WriteLine("md5Hash: " + md5Hash);

            // Step 6: Submit the MD5 hash for validation
            string validationResult = await Services.Services.ValidateResult(md5Hash);

            // Check if validation succeeded and the elapsed time is less than 30 seconds
            if (
                (validationResult != "Alas it didn't work")
                && (stopwatch.Elapsed < TimeSpan.FromSeconds(30))
            )
            {
                Console.WriteLine("Passphrase: " + md5Hash); // Success! Print the passphrase
            }
            else
            {
                Console.WriteLine("Validation failed!"); // Validation failed
            }
        }
    }
}
