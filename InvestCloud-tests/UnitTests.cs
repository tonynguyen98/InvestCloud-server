using InvestCloudServer.Models;
using InvestCloudServer.Services;
using InvestCloudServer.Utils;

namespace InvestCloud_tests;

public class Tests
{
    readonly int size = 5;
    readonly int[,] matrixA =
    {
        { -4, -1, 3, 4, 2 },
        { -1, 3, 4, 2, -2 },
        { 3, 4, 2, -2, -4 },
        { 4, 2, -2, -4, -2 },
        { 2, -2, -4, -2, 2 },
    };
    readonly int[,] matrixB =
    {
        { 1, 4, 3, 0, -4 },
        { 4, 3, 0, -4, -4 },
        { 3, 0, -4, -4, 0 },
        { 0, -4, -4, 0, 4 },
        { -4, -4, 0, 4, 4 },
    };
    readonly int[,] matrixProduct =
    {
        { -7, -43, -40, 0, 44 },
        { 31, 5, -27, -36, -8 },
        { 41, 48, 9, -40, -52 },
        { 14, 46, 36, -8, -48 },
        { -26, 2, 30, 32, 0 },
    };

    readonly string productString = "-7-43-40044315-27-36-841489-40-52144636-8-48-26230320";

    [Test]
    public async Task InitializeDatasetsValidSizeSuccess()
    {
        ResultOfInt32? result = await Services.InitializeDatasets(size);
        Assert.Multiple(() =>
        {
            Assert.That(result!.Success, Is.True);
            Assert.That(size, Is.EqualTo(result!.Value));
        });
    }

    [Test]
    public void InitializeDatasetsValidSizeFail()
    {
        int size = 2000;
        var e = Assert.ThrowsAsync<System.Exception>(async () =>
        {
            await Services.InitializeDatasets(size);
        });
        Assert.That(
            e.Message,
            Is.EqualTo($"Initializing {size} matrix failed: Size must be within [2..1000]")
        );
    }

    [Test]
    public async Task GetADatasetValidDatasetAndSizeReturnsMatrix()
    {
        string dataset = "A";

        int[,] matrix = await Services.GetDataset(dataset, size);
        Assert.Multiple(() =>
        {
            Assert.That(matrix, Has.Length.EqualTo(size * size));
            Assert.That(matrix, Is.EqualTo(matrixA));
        });
    }

    [Test]
    public async Task GetBDatasetValidDatasetAndSizeReturnsMatrix()
    {
        string dataset = "B";

        int[,] matrix = await Services.GetDataset(dataset, size);
        Assert.Multiple(() =>
        {
            Assert.That(matrix, Has.Length.EqualTo(size * size));
            Assert.That(matrix, Is.EqualTo(matrixB));
        });
    }

    [Test]
    public void MultiplyMatricesValidMatricesReturnsResultMatrix()
    {
        int[,] resultMatrix = Utils.MultiplyMatrices(matrixA, matrixB);
        Assert.Multiple(() =>
        {
            Assert.That(resultMatrix, Has.Length.EqualTo(size * size));
            Assert.That(resultMatrix, Is.EqualTo(matrixProduct));
        });
    }

    [Test]
    public void MatrixToStringValidMatrixReturnsString()
    {
        string resultString = Utils.MatrixToString(matrixProduct);
        Assert.That(resultString, Is.EqualTo(productString));
    }

    [Test]
    public void CalculateMD5HashValidInputReturnsHash()
    {
        string md5Hash = Utils.CalculateMD5Hash(productString);
        Assert.That(md5Hash.ToLower(), Is.EqualTo("99c619ff8a72e5282b6dee3e2b369133".ToLower()));
    }
}
