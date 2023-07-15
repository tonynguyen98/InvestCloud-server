namespace InvestCloudServer.Models
{
    // https://recruitment-test.investcloud.com/Help/ResourceModel?modelName=ResultOfInt32
    public class ResultOfInt32
    {
        public required int Value { get; set; }
        public required string Cause { get; set; }
        public required bool Success { get; set; }
    }

    // https://recruitment-test.investcloud.com/Help/ResourceModel?modelName=ResultOfInt32%5B%5D
    public class ResultOfInt32Array
    {
        public required int[] Value { get; set; }
        public required string Cause { get; set; }
        public required bool Success { get; set; }
    }

    // https://recruitment-test.investcloud.com/Help/ResourceModel?modelName=ResultOfString
    public class ResultOfString
    {
        public required string Value { get; set; }
        public required string Cause { get; set; }
        public required bool Success { get; set; }
    }
}
