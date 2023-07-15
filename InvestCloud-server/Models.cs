namespace InvestCloudServer.Models
{
    public class ResultOfInt32
    {
        public required int Value { get; set; }
        public required string Cause { get; set; }
        public required bool Success { get; set; }
    }

    public class ResultOfInt32Array
    {
        public required int[] Value { get; set; }
        public required string Cause { get; set; }
        public required bool Success { get; set; }
    }

    public class ResultOfString
    {
        public required string Value { get; set; }
        public required string Cause { get; set; }
        public required bool Success { get; set; }
    }
}
