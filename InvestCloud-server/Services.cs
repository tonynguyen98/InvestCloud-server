using System.Text;
using System.Text.Json;
using InvestCloudServer.Models;

namespace InvestCloudServer.Services
{
    public class Services
    {
        static readonly HttpClient client = new();

        public static async Task<ResultOfInt32?> InitializeDatasets(int size)
        {
            string url = $"https://recruitment-test.investcloud.com/api/numbers/init/{size}";
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            Stream responseStream = await response.Content.ReadAsStreamAsync();
            ResultOfInt32? obj = await JsonSerializer.DeserializeAsync<ResultOfInt32>(
                responseStream
            );
            if (!obj?.Success ?? true)
                throw new Exception($"Initializing {size} matrix failed: {obj?.Cause}");

            return obj;
        }

        public static async Task<int[,]> GetDataset(string dataset, int size)
        {
            int[,] matrix = new int[size, size];
            Task[] tasks = new Task[size];

            for (int i = 0; i < size; i++)
            {
                int row = i; // Capture the row variable for the lambda expression
                tasks[i] = Task.Run(async () =>
                {
                    string url =
                        $"https://recruitment-test.investcloud.com/api/numbers/{dataset}/row/{row}";
                    HttpResponseMessage response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();
                    Stream responseStream = await response.Content.ReadAsStreamAsync();
                    ResultOfInt32Array? obj =
                        await JsonSerializer.DeserializeAsync<ResultOfInt32Array>(responseStream);

                    if (!obj?.Success ?? true)
                        throw new Exception($"Getting Matrix {dataset} failed: {obj?.Cause}");

                    if (obj?.Value.Length != size)
                        throw new Exception(
                            $"Getting Matrix {dataset} failed, return size does not match"
                        );

                    for (int j = 0; j < size; j++)
                    {
                        matrix[row, j] = obj!.Value[j];
                    }
                });
            }

            await Task.WhenAll(tasks);
            return matrix;
        }

        public static async Task<string> ValidateResult(string md5Hash)
        {
            string url = "https://recruitment-test.investcloud.com/api/numbers/validate";
            StringContent content = new($"\"{md5Hash}\"", Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync(url, content);
            response.EnsureSuccessStatusCode();

            Stream responseStream = await response.Content.ReadAsStreamAsync();
            ResultOfString? obj = await JsonSerializer.DeserializeAsync<ResultOfString>(
                responseStream
            );

            if (obj?.Value == null || (!obj?.Success ?? true))
                throw new Exception($"Validating has failed: {obj?.Cause}");

            Console.WriteLine("Validation got : " + obj!.Value);
            return obj!.Value;
        }
    }
}
